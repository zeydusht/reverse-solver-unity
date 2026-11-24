#!/usr/bin/env node
// PM_104 §1: does the listening page play, for every choice in ses.json, the same file the game plays for it?
// The game's files: docs/art/pm104/oyun_sesleri.json (Unity: ReverseSolver.EditorTools.SesEslesme.Write, what the
// loaders pick with no flag). The page's: its JSON lists (sesler.json pm95, sesler99.json, sesler100.json,
// sesler102.json) at the letter of ses.json, the files under <onizleme>/sesler/. Same file = same SHA-1 of the bytes.
// With --page <dir> it also copies every file the game plays into <dir>/sesler/ (the page then plays the game's own
// files) and writes <dir>/bugun.json: the listening page's "Bugünkü seçim", every event with its files.
// usage: node Tools/ses_eslesme.js <onizleme dir (p23/onizleme of gh-pages)> [--out docs/art/pm104/ses_eslesme.md] [--page docs/art/pm104/onizleme]
const fs = require('fs'), path = require('path'), crypto = require('crypto');
const args = process.argv.slice(2);
const dir = args[0];
const out = (i => i >= 0 ? args[i + 1] : 'docs/art/pm104/ses_eslesme.md')(args.indexOf('--out'));
const game = JSON.parse(fs.readFileSync('docs/art/pm104/oyun_sesleri.json', 'utf8'));
const K = game.secim;
const J = f => { try { return JSON.parse(fs.readFileSync(path.join(dir, f), 'utf8')); } catch (e) { return {}; } };
const S95 = J('sesler.json'), S99 = J('sesler99.json'), S100 = J('sesler100.json'), S102 = J('sesler102.json'), S104 = J('sesler104.json'), S105 = J('sesler105.json');
const pageOut = (i => i >= 0 ? args[i + 1] : null)(args.indexOf('--page'));
const sha = f => fs.existsSync(f) ? crypto.createHash('sha1').update(fs.readFileSync(f)).digest('hex').slice(0, 10) : null;
const pageFile = f => { for (const d of [path.join(dir, 'sesler')]) { const p = path.join(d, f); if (fs.existsSync(p)) return p; } return null; };
const byBase = {};
for (const f of game.files) byBase[path.basename(f)] = f;
const used = new Set(), today = [];

// event, ses.json key, the page's files at that letter, the game's file name pattern
const pick = (list, l) => (list || []).find(c => c.letter === l);
const takes = c => c ? (c.takes || c.files || (c.file ? [{ file: c.file }] : [])).map(t => t.file) : [];
const rows = [
  ['Kaydırma (parça çıkışı)', 'cikis', takes(pick(S104.cikis, K.cikis) || pick(S99.cikis, K.cikis) || pick(S95.cikis, K.cikis)), new RegExp(`/cikis-${K.cikis}\\d`)],
  ['Kalkış (takılmada, kısa ve uzun)', 'cikis', [], new RegExp(`/kalkisk?-${K.cikis}\\d`)],
  ['Çarpma (takılma)', 'carpma', takes(pick(S95.carpma, K.carpma)), new RegExp(`/carpma-${K.carpma}\\d`)],
  // PM_105: every button plays the tok, one take (the tick is gone)
  ['Düğme (tok): bütün düğmeler', 'dugme', takes(pick(S104.dugme, K.dugme)).slice(0, 1), new RegExp(`/ui-tok-${K.dugme}\\d`)],
  ['Panel açılır / kapanır (vuş)', null, [], /\/ui-(open|close)\d/],
  ['Anahtar (ses, müzik, titreşim)', null, [], /\/ui-toggle\d/],
  ['Olmayan düğme (pasif)', null, [], /\/ui-dull\d/],
  ['Bomba sökülür', null, [], /\/bomba-a\d/],
  ['Kilit açılır (eski kırılma, yedek)', null, [], /\/kilit-a\d/],
  // PM_105 §2: the obstacles' sounds
  ...['zincir', 'zincirger', 'kilitdokun', 'kilitac', 'patlama', 'duvar'].map(k => [(S105[k] || {}).title || k, k, takes(pick((S105[k] || {}).candidates, K[k])), new RegExp(`/pm105/${k}-${K[k]}\\d`)]),
  ['Makas', 'makas', takes(pick((S102.booster || {}).makas, K.makas)), new RegExp(`/booster-makas-\\w+-${K.makas}\\.`)],
  ['Değnek', 'degnek', takes(pick((S102.booster || {}).degnek, K.degnek)), new RegExp(`/booster-degnek-\\w+-${K.degnek}\\.`)],
  ['Çekiç', 'cekic', takes(pick((S102.booster || {}).cekic, K.cekic)), new RegExp(`/booster-cekic-\\w+-${K.cekic}\\.`)],
  ['Saat', 'saat', takes(pick((S102.booster || {}).saat, K.saat)), new RegExp(`/booster-saat-\\w+-${K.saat}\\.`)],
  ['Koridor', 'koridor', takes(pick((S100.gecis || {}).koridor, K.koridor)), new RegExp(`/gecis-koridor-${K.koridor}\\.`)],
  ['Dalış (+ ting)', 'dalis', takes(pick((S100.gecis || {}).dalis, K.dalis)).concat(takes(pick((S100.gecis || {}).ting, K.dalis))), new RegExp(`/gecis-(dalis|ting)-${K.dalis}\\.`)],
  ['Çizgi', 'cizgi', takes(pick((S100.gecis || {}).cizgi, K.cizgi)), new RegExp(`/gecis-cizgi-${K.cizgi}\\.`)],
  ['Geri çekilme (+ HUD tıkı)', 'geri', takes(pick((S100.gecis || {}).geri, K.geri)).concat(takes(pick((S100.gecis || {}).hud, K.geri))), new RegExp(`/gecis-(geri|hud)-${K.geri}\\.`)],
  ['Sekme', 'sekme', takes(pick((S100.gecis || {}).sekme, K.sekme)), new RegExp(`/gecis-sekme-${K.sekme}\\.`)],
];
const lines = ['| Olay | ses.json harfi | sayfadaki dosya | oyunda çalan dosya(lar) | aynı mı |', '|---|---|---|---|---|'];
let bad = 0;
for (const [name, key, pageFiles, re] of rows) {
  const g = game.files.filter(f => re.test(f));
  g.forEach(f => used.add(f));
  let same;
  if (!pageFiles.length) same = 'sayfada yok';
  else {
    const ok = pageFiles.map(pf => { const p = pageFile(pf), gf = byBase[pf]; return p && gf && sha(p) === sha(gf); });
    const allIn = g.every(gf => pageFiles.includes(path.basename(gf)));
    same = ok.every(Boolean) && allIn && g.length === pageFiles.length ? 'evet' : 'HAYIR';
    if (same === 'HAYIR') {
      bad++;
      const why = pageFiles.map(pf => { const p = pageFile(pf), gf = byBase[pf]; return !p ? pf + ' sayfada bulunamadı' : !gf ? pf + ' oyunda yok' : sha(p) !== sha(gf) ? pf + ' farklı (' + sha(p) + ' / ' + sha(gf) + ')' : null; }).filter(Boolean);
      same += why.length ? ': ' + why.join('; ') : ': oyunda ' + g.length + ', sayfada ' + pageFiles.length + ' kayıt';
    }
  }
  const short = f => f.replace(/^.*Resources\/Art\/audio\//, '');
  lines.push(`| ${name} | ${key ? K[key] : '—'} | ${pageFiles.join(', ') || '—'} | ${g.map(short).join(', ') || '—'} | ${same} |`);
  if (!/yedek/.test(name)) today.push({ name, letter: key ? K[key] : '', files: g.filter(f => f.endsWith('.wav')).map(f => path.basename(f)) });
}
if (pageOut) {
  fs.mkdirSync(path.join(pageOut, 'sesler'), { recursive: true });
  for (const f of game.files) if (f.endsWith('.wav')) fs.copyFileSync(f, path.join(pageOut, 'sesler', path.basename(f)));
  fs.writeFileSync(path.join(pageOut, 'bugun.json'), JSON.stringify({ secim: K, events: today.filter(t => t.files.length) }, null, 1));
  console.log(`[eslesme] ${game.files.filter(f => f.endsWith('.wav')).length} dosya ${pageOut}/sesler, bugun.json`);
}
const rest = game.files.filter(f => !used.has(f)).map(f => f.replace(/^.*Resources\/Art\/audio\//, ''));
const md = lines.join('\n') + '\n\nAyrıca oyunun çaldığı (ses.json\'da harfi olmayan): ' + (rest.join(', ') || '—') + '\n';
fs.writeFileSync(out + '.table', md);
console.log(md);
console.log(`[eslesme] ${rows.length} olay, uyuşmayan ${bad}`);
