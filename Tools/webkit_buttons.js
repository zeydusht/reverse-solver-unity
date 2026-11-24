#!/usr/bin/env node
// PM_105 §3: every tappable thing plays Durdur's tok, and the old "çıt" (pm98/ui-tick-*, pm95/tik-*) never plays.
// Opens the game in WebKit (Playwright; Safari's engine) on each screen and taps a grid over it (9 x 18 points), with
// &seslog=1: the game logs every UI sound it plays ("[ses] ui <kind> <file>") and every effect ("[ses] <kind> <file>").
// Screens: the menu and its four tabs, Ayarlar, a level (lv=3: the lock), the pause panel, the time-up panel
// (&devam=1: gold and ad continue), the win screen (&kazan=1) and the chest (&sandik=1).
// Writes the files played per screen and passes when a tok played on every screen and no tick ever did.
// usage: node Tools/webkit_buttons.js <game base url, e.g. http://127.0.0.1:8123/br/> [--out file.json] [--pw <playwright-core dir>] [--chrome <chrome.exe>]
const fs = require('fs'), path = require('path');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const base = args.find(a => /^https?:/.test(a));
const pwDir = opt('pw', path.join(process.env.APPDATA || '', 'npm/node_modules/n8n/node_modules/playwright-core'));
const chrome = opt('chrome', null);
const { webkit, chromium } = require(pwDir);
const OLD = /ui-tick-|(^|[^-])tik-[a-c]\d/;
const q = 'c=p23&lang=tr&seslog=1&test=1&intro=0';
const SCREENS = [
  { name: 'menü', url: `?${q}`, taps: 'grid' },
  { name: 'menü sekmeleri', url: `?${q}`, taps: [[49, 810], [117, 810], [195, 810], [273, 810], [341, 810], [49, 810]] },
  { name: 'bölüm 3 (kilit)', url: `?${q}&lv=3`, taps: 'grid' },
  { name: 'durdur paneli', url: `?${q}&lv=3`, pre: [[355, 52]], taps: 'grid' },
  { name: 'süre doldu (altın / reklam ile devam)', url: `?${q}&lv=3&devam=1`, taps: 'grid' },
  { name: 'kazanma', url: `?${q}&lv=3&kazan=1`, taps: 'grid' },
  { name: 'sandık', url: `?${q}&lv=3&sandik=1`, taps: 'grid' },
];

(async () => {
  const browser = chrome
    ? await chromium.launch({ executablePath: chrome, headless: true, args: ['--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist', '--autoplay-policy=no-user-gesture-required'] })
    : await webkit.launch({});
  console.log(`[dugmeler] ${chrome ? 'chrome' : 'webkit'} ${browser.version()}`);
  const out = { base, screens: [], old: [], pageErrors: [], alerts: [] };
  for (const s of SCREENS) {
    const ctx = await browser.newContext({ viewport: { width: 390, height: 844 }, deviceScaleFactor: 1 });
    await ctx.route(/supabase\.co/, r => r.abort());
    const page = await ctx.newPage();
    const played = [];
    page.on('console', m => {
      const t = m.text(), r = /\[ses\] (ui )?(\w+) (\S+-\S+) \d/.exec(t);   // a play (not the start's list)
      if (r) played.push({ ui: !!r[1], kind: r[2], file: r[3] });
    });
    page.on('pageerror', e => out.pageErrors.push(s.name + ': ' + String(e.message || e).slice(0, 160)));
    page.on('dialog', d => { out.alerts.push(d.message()); d.dismiss().catch(() => {}); });
    await page.goto(base + s.url, { waitUntil: 'load', timeout: 120000 });
    await page.waitForTimeout(+opt('wait', 45) * 1000);   // WebKit on Windows starts slowly (software WebGL)
    const tap = async (x, y, wait = 450) => { await page.mouse.click(x, y, { delay: 70 }); await page.waitForTimeout(wait); };   // held 70 ms: Unity reads the press on a frame
    await tap(195, 300, 1200);   // the first touch (audio unlocks; an intro or a card goes)
    for (const [x, y] of s.pre || []) await tap(x, y, 900);
    if (opt('shots', null)) await page.screenshot({ path: path.join(opt('shots'), s.name.replace(/\W+/g, '_') + '.png') });
    const pts = s.taps === 'grid' ? [] : s.taps;
    const gw = +opt('grid', 9), gh = Math.round(gw * 2);
    if (s.taps === 'grid') for (let gy = 0; gy < gh; gy++) for (let gx = 0; gx < gw; gx++) pts.push([22 + gx * 346 / (gw - 1), 40 + gy * 765 / (gh - 1)]);
    for (const [x, y] of pts) await tap(x, y, +opt('tapms', s.taps === 'grid' ? 260 : 700));
    await page.waitForTimeout(600);
    const mine = played.slice(0);
    const files = {};
    for (const p of mine) files[(p.ui ? 'ui ' : '') + p.kind + ' ' + p.file] = (files[(p.ui ? 'ui ' : '') + p.kind + ' ' + p.file] || 0) + 1;
    const toks = mine.filter(p => p.ui && /^ui-tok-/.test(p.file)).length;
    const old = mine.filter(p => OLD.test(p.file));
    out.old.push(...old.map(o => s.name + ': ' + o.file));
    out.screens.push({ name: s.name, taps: pts.length, played: mine.length, toks, files });
    console.log(`[dugmeler] ${s.name}: ${pts.length} dokunuş, ${mine.length} ses (${toks} tok), eski çıt ${old.length}; ` + Object.entries(files).map(([k, v]) => `${k} x${v}`).join(', '));
    await ctx.close();
  }
  await browser.close();
  const noTok = out.screens.filter(s => s.toks === 0).map(s => s.name);
  out.pass = out.old.length === 0 && noTok.length === 0 && out.alerts.length === 0;
  const file = opt('out', null);
  if (file) fs.writeFileSync(file, JSON.stringify(out, null, 1));
  console.log(`[dugmeler] eski çıt ${out.old.length}, toksuz ekran ${noTok.length ? noTok.join(', ') : 0}, uyarı ${out.alerts.length}, sayfa hatası ${out.pageErrors.length}: ${out.pass ? 'GEÇTİ' : 'KALDI'}`);
  process.exit(out.pass ? 0 : 1);
})().catch(e => { console.error(e); process.exit(2); });
