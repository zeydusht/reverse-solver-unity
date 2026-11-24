#!/usr/bin/env node
// PM_104_EK: the game's sounds in WebKit (Safari's engine), with Playwright. Every decodeAudioData call the page
// makes (Unity's clips, Music.jslib's loops and fanfare, the opening story's tracks) is wrapped: its size, its
// container (the first bytes), ok or the error. An alert, a page error or an unhandled rejection is recorded too.
// It plays a flow that triggers the sounds (menu, a level, pieces leaving, a jam, the boosters, a win, the panel,
// the tabs) through the page's debug hooks where there are any, and the listening page decodes every candidate.
//
// usage: node Tools/webkit_audio_check.js <game url> [--page <listening page url>] [--out file.json]
//        [--wait 25] [--pw <playwright-core dir>] [--webkit <Playwright.exe>] [--chrome <chrome.exe>]
// --chrome runs the same in Chrome (real GPU): Playwright's WebKit on Windows has no Web Audio at all, so the decodes
// themselves (which files, which container, and with ?sestest=bozuk every decode failing: no alert) are measured there.
// Exit code 0 when no decode failed, no alert showed and no page error happened.
const fs = require('fs'), path = require('path'), os = require('os');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const url = args.find(a => /^https?:/.test(a));
const pageUrl = opt('page', null);
const pwDir = opt('pw', path.join(process.env.APPDATA || '', 'npm/node_modules/n8n/node_modules/playwright-core'));
const exe = opt('webkit', null);
const waitS = +opt('wait', 25);
const { webkit, chromium } = require(pwDir);
const chrome = opt('chrome', null);

// Runs in the page before anything else.
const hook = () => {
  const log = window.__rsAudioLog = [];
  const kind = b => {
    const u = new Uint8Array(b.slice ? b.slice(0, 12) : b, 0, 12), s = String.fromCharCode.apply(null, u);
    if (s.slice(4, 8) === 'ftyp') return 'mp4/aac';
    if (s.slice(0, 4) === 'RIFF') return 'wav';
    if (s.slice(0, 4) === 'OggS') return 'ogg';
    if (s.slice(0, 3) === 'ID3' || (u[0] === 0xff && (u[1] & 0xe0) === 0xe0)) return 'mp3';
    return 'other ' + Array.from(u.slice(0, 4)).map(x => x.toString(16)).join(' ');
  };
  const C = window.AudioContext || window.webkitAudioContext;
  if (C) {
    const orig = C.prototype.decodeAudioData;
    C.prototype.decodeAudioData = function (buf, ok, no) {
      const e = { size: buf.byteLength, kind: kind(buf), t: performance.now() | 0, url: window.__rsLastUrl || '' };
      log.push(e);
      const p = orig.call(this, buf, b => { e.ok = true; e.dur = +b.duration.toFixed(3); e.ch = b.numberOfChannels; e.sr = b.sampleRate; ok && ok(b); },
        err => { e.ok = false; e.err = String(err && (err.name + ': ' + err.message) || err); no && no(err); });
      if (p && p.catch) p.catch(() => {});
      return p;
    };
  }
  const f = window.fetch;
  window.fetch = function (u) { window.__rsLastUrl = String(u && u.url || u); return f.apply(this, arguments); };
  window.__rsAlerts = [];
  window.alert = m => window.__rsAlerts.push(String(m));
  window.__rsRejections = [];
  window.addEventListener('unhandledrejection', e => window.__rsRejections.push(String(e.reason && (e.reason.name + ': ' + e.reason.message) || e.reason)));
};

(async () => {
  const browser = chrome
    ? await chromium.launch({ executablePath: chrome, headless: true, args: ['--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist', '--autoplay-policy=no-user-gesture-required'] })
    : await webkit.launch(exe ? { executablePath: exe } : {});
  console.log(`[webkit] ${chrome ? 'chrome' : 'webkit'} ${browser.version()}`);
  const out = { url, decodes: [], alerts: [], rejections: [], pageErrors: [], banners: [] };
  const ctx = await browser.newContext({ viewport: { width: 390, height: 844 }, deviceScaleFactor: 1, hasTouch: false });
  await ctx.route(/supabase\.co/, r => r.abort());
  const page = await ctx.newPage();
  await page.addInitScript(hook);
  page.on('pageerror', e => out.pageErrors.push(String(e.message || e)));
  out.audioLog = [];
  page.on('console', m => { const t = m.text(); if (/\[audio\]|Decode error/.test(t)) out.audioLog.push(t.slice(0, 200)); });
  page.on('dialog', d => { out.alerts.push(d.message()); d.dismiss().catch(() => {}); });
  await page.goto(url, { waitUntil: 'load', timeout: 120000 });
  const box = { x: 195, y: 422 };
  const tap = async (x, y) => { await page.mouse.click(x, y); await page.waitForTimeout(400); };
  // the first touch starts the audio; then wander: taps across the screen and drags across the board for a while
  const end = Date.now() + waitS * 1000;
  await page.waitForTimeout(8000);
  await tap(box.x, box.y);
  const flow = [[195, 760], [195, 422], [60, 800], [130, 800], [260, 800], [330, 800], [195, 800], [350, 60], [40, 60], [195, 600]];
  let i = 0;
  while (Date.now() < end) {
    const [x, y] = flow[i++ % flow.length];
    await tap(x, y);
    await page.mouse.move(195, 420); await page.mouse.down();
    await page.mouse.move(195 + (i % 2 ? 160 : -160), 420 + (i % 3 - 1) * 160, { steps: 6 }); await page.mouse.up();
    await page.waitForTimeout(300);
  }
  if (await page.evaluate(() => typeof window.rsDebugSounds === 'function')) await page.evaluate(() => window.rsDebugSounds());
  await page.waitForTimeout(3000);
  Object.assign(out, await page.evaluate(() => ({ decodes: window.__rsAudioLog, alertsHooked: window.__rsAlerts, rejections: window.__rsRejections, guardFails: (window.rsAudioFails || []).length })));
  out.banners = await page.evaluate(() => Array.from(document.querySelectorAll('#unity-warning div, .unity-warning')).map(d => d.textContent));
  if (pageUrl) {
    const p2 = await ctx.newPage();
    await p2.addInitScript(hook);
    const errs = [];
    p2.on('pageerror', e => errs.push(String(e.message || e)));
    await p2.goto(pageUrl, { waitUntil: 'load', timeout: 120000 });
    await p2.mouse.click(10, 10);
    // decode every audio file the page names (its .wav / .mp3 strings), found the way the page finds them
    const files = await p2.evaluate(() => Array.from(new Set((document.documentElement.innerHTML.match(/[\w./-]+\.(?:wav|mp3)(?=['"])/g) || []))));
    const res = await p2.evaluate(async list => {
      const C = window.AudioContext || window.webkitAudioContext, r = [];
      if (!C) return list.map(f => ({ f, ok: false, err: 'no Web Audio' }));
      const c = new C();
      for (const f of list) {
        let b = null;
        for (const u of [f, '../../pm100/onizleme/' + f, '../../pm99/onizleme/' + f]) {
          try { const x = await fetch(u); if (x.ok) { b = await x.arrayBuffer(); break; } } catch (e) {}
        }
        if (!b) { r.push({ f, ok: true, missing: true }); continue; }
        try { await c.decodeAudioData(b); r.push({ f, ok: true }); }
        catch (e) { r.push({ f, ok: false, err: String(e && e.name) }); }
      }
      return r;
    }, files);
    out.listening = { url: pageUrl, files: res.length, missing: res.filter(r => r.missing).length, failed: res.filter(r => !r.ok), pageErrors: errs };
  }
  await browser.close();
  const failed = out.decodes.filter(d => d.ok === false), pending = out.decodes.filter(d => d.ok === undefined);
  const byKind = {};
  for (const d of out.decodes) { const k = d.kind + (d.ok === false ? ' FAIL' : d.ok ? ' ok' : ' pending'); byKind[k] = (byKind[k] || 0) + 1; }
  const alerts = out.alerts.concat(out.alertsHooked || []);
  console.log(`[webkit] decodes ${out.decodes.length}: ${JSON.stringify(byKind)}`);
  for (const d of failed.slice(0, 20)) console.log(`[webkit]   FAIL ${d.kind} ${d.size} B ${d.url ? path.basename(d.url) : ''} ${d.err}`);
  console.log(`[webkit] alerts ${alerts.length}${alerts.length ? ': ' + alerts[0].slice(0, 160) : ''}; unhandled rejections ${out.rejections.length}; page errors ${out.pageErrors.length}${out.pageErrors.length ? ': ' + out.pageErrors[0].slice(0, 160) : ''}`);
  console.log(`[webkit] the page's guard caught ${out.guardFails} decode failures; game log: ${out.audioLog.filter(t => /\[audio\] decode fail [A-Za-z]/.test(t)).length} files reported${out.audioLog.length ? ' (' + out.audioLog.slice(-3).join(' | ') + ')' : ''}`);
  if (out.listening) console.log(`[webkit] listening page: ${out.listening.files} files (${out.listening.missing} not found), ${out.listening.failed.length} failed${out.listening.failed.length ? ' (' + out.listening.failed.slice(0, 5).map(f => path.basename(f.f)).join(', ') + ')' : ''}, page errors ${out.listening.pageErrors.length}`);
  const file = opt('out', path.join(os.tmpdir(), 'webkit_audio_check.json'));
  fs.writeFileSync(file, JSON.stringify(out, null, 1));
  console.log(`[webkit] details ${file}`);
  const bad = failed.length + alerts.length + out.pageErrors.length + (out.listening ? out.listening.failed.length + out.listening.pageErrors.length : 0);
  console.log(`[webkit] ${bad ? 'KALDI' : 'GEÇTİ'}`);
  process.exit(bad ? 1 : 0);
})().catch(e => { console.error('[webkit] error', e.message); process.exit(2); });
