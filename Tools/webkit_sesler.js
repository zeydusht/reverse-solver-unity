#!/usr/bin/env node
// PM_106_EK2: the listening page (onizleme/sesler.html) on the iPhone's engine. Two runs:
//  1. WebKit (Playwright; Safari's engine) at iPhone 13's size (390 x 844): the page builds, nothing throws, every
//     section is there and none says "yüklenemedi"; the screenshot.
//  2. Every section's first ▶ pressed and waited on until a sound really starts (AudioBufferSourceNode.start with a
//     decoded buffer), no decode failing. Playwright's WebKit on Windows has no Web Audio at all (no AudioContext), so
//     this runs in Chrome (--chrome) unless the WebKit build has one (then in WebKit too).
// usage: node Tools/webkit_sesler.js <page url> [--chrome <chrome.exe>] [--out file.json] [--shot file.png] [--pw <playwright-core dir>]
const fs = require('fs'), path = require('path');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const url = args.find(a => /^https?:/.test(a));
const pwDir = opt('pw', path.join(process.env.APPDATA || '', 'npm/node_modules/n8n/node_modules/playwright-core'));
const chrome = opt('chrome', null);
const { webkit, chromium, devices } = require(pwDir);
const SECTIONS = '#today section, #sections > section';

const hook = () => {
  window.__plays = 0; window.__decodeErrors = [];
  if (!window.AudioBufferSourceNode) return;
  const S = AudioBufferSourceNode.prototype.start;
  AudioBufferSourceNode.prototype.start = function () { if (this.buffer && this.buffer.duration > 0) window.__plays++; return S.apply(this, arguments); };
  const P = (window.BaseAudioContext || window.AudioContext).prototype, D = P.decodeAudioData;
  P.decodeAudioData = function (a, ok, no) {
    const fail = e => window.__decodeErrors.push(String(e && e.message || e));
    const p = D.call(this, a, ok, e => { fail(e); if (no) no(e); });
    if (p && p.catch) p.catch(fail);
    return p;
  };
};

async function run(browser, ctxOpt, play, shot) {
  const ctx = await browser.newContext(ctxOpt);
  const page = await ctx.newPage();
  const errors = [];
  page.on('pageerror', e => errors.push(e.message));
  await page.addInitScript(hook);
  await page.goto(url, { waitUntil: 'networkidle' });
  await page.waitForTimeout(1500);
  if (shot) await page.screenshot({ path: shot });
  const audio = await page.evaluate(() => !!(window.AudioContext || window.webkitAudioContext));
  const n = await page.locator(SECTIONS).count(), sections = [];
  for (let i = 0; i < n; i++) {
    const sec = page.locator(SECTIONS).nth(i);
    const title = (await sec.locator('h2').first().textContent()).trim();
    const broken = await sec.evaluate(s => s.classList.contains('broken'));
    const cards = await sec.locator('.card').count(), plays = await sec.locator('.play').count();
    const links = await sec.locator('a.btn').count();   // a section of links into the game only (7 mi 5 mi?)
    let played = null;
    if (play && audio && !broken && plays) {
      played = false;
      const btn = sec.locator('.play').first();
      if (await btn.count()) {
        const before = await page.evaluate(() => window.__plays);
        await btn.click();
        for (let t = 0; t < 50 && !played; t++) { await page.waitForTimeout(100); played = await page.evaluate(b => window.__plays > b, before); }
      }
    }
    sections.push({ title, broken, cards, links, played });
  }
  const decodeErrors = await page.evaluate(() => window.__decodeErrors);
  await ctx.close();
  return { audio, sections, pageErrors: errors, decodeErrors };
}

(async () => {
  const wkb = await webkit.launch();
  const wk = await run(wkb, { ...devices['iPhone 13'] }, true, opt('shot', null));
  await wkb.close();
  let pl = wk.audio ? wk : null;
  if (!pl && chrome) {
    const cb = await chromium.launch({ executablePath: chrome, headless: true, args: ['--autoplay-policy=no-user-gesture-required'] });
    pl = await run(cb, { viewport: { width: 390, height: 844 }, isMobile: false }, true, null);
    pl.engine = 'chrome';
    await cb.close();
  } else if (pl) pl.engine = 'webkit';
  for (const s of wk.sections) {
    const p = pl && pl.sections.find(x => x.title === s.title);
    console.log(`${s.broken ? 'YÜKLENEMEDİ' : 'kuruldu'} (${s.cards} kart)  ${p ? (p.played == null ? 'yalnız bağlantı' : p.played ? 'çaldı' : 'ÇALMADI') : '-'}  ${s.title}`);
  }
  const built = wk.pageErrors.length === 0 && wk.sections.length > 0 && wk.sections.every(s => !s.broken && (s.cards > 0 || s.links > 0));
  const played = !!pl && pl.pageErrors.length === 0 && pl.decodeErrors.length === 0 && pl.sections.length === wk.sections.length && pl.sections.every(s => s.played !== false) && pl.sections.some(s => s.played);
  const result = { url, webkit: { viewport: '390x844', audio: wk.audio, sections: wk.sections, pageErrors: wk.pageErrors },
    play: pl && { engine: pl.engine, sections: pl.sections, pageErrors: pl.pageErrors, decodeErrors: pl.decodeErrors }, pass: built && played };
  const out = opt('out', null);
  if (out) fs.writeFileSync(out, JSON.stringify(result, null, 1));
  console.log(`[sesler] WebKit: bölüm ${wk.sections.length}, yüklenemedi ${wk.sections.filter(s => s.broken).length}, sayfa hatası ${wk.pageErrors.length}, Web Audio ${wk.audio ? 'var' : 'yok'}`);
  if (pl) console.log(`[sesler] çalma (${pl.engine}): çalan ${pl.sections.filter(s => s.played).length}/${pl.sections.filter(s => s.played != null).length} (yalnız bağlantı ${pl.sections.filter(s => s.played == null).length}), çözme hatası ${pl.decodeErrors.length}, sayfa hatası ${pl.pageErrors.length}`);
  console.log(`[sesler] ${result.pass ? 'GEÇTİ' : 'KALDI'}`);
  for (const e of wk.pageErrors.concat(pl ? pl.pageErrors.concat(pl.decodeErrors) : [])) console.log('  ' + e);
  process.exit(result.pass ? 0 : 1);
})().catch(e => { console.error(e); process.exit(2); });
