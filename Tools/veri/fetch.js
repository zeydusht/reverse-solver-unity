// Veri sistemi (PM_MESAJ_25): every plays row with a level_id and every moves row, read with the game's publishable
// key (anon SELECT), paged. Out: Logs/veri/plays.json, Logs/veri/moves.json. Who counts as whom is decided later
// (Tools/veri/oyuncular.py): the raw rows are kept as they are.
//
//   node Tools/veri/fetch.js [--cohort pt1]      (--cohort: only that cohort's rows)
const fs = require('fs'), path = require('path');
const KEY = 'sb_publishable_S-DzN5GrUyMR2izo8vHc3Q_0kaGiVDK', SUPA = 'https://vwneqdhzfevgbuwoadod.supabase.co';
const i = process.argv.indexOf('--cohort'), cohort = i > 0 ? process.argv[i + 1] : null;
const OUT = path.join(__dirname, '..', '..', 'Logs', 'veri');

async function all(table, select) {
  const rows = [];
  for (let from = 0; ; from += 1000) {
    const q = `${SUPA}/rest/v1/${table}?select=${select}&order=id${cohort ? '&cohort=eq.' + cohort : ''}`;
    const r = await fetch(q, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY, Range: `${from}-${from + 999}` } });
    if (!r.ok) throw new Error(`${table}: ${r.status} ${await r.text()}`);
    const page = await r.json(); rows.push(...page);
    if (page.length < 1000) return rows;
  }
}

(async () => {
  fs.mkdirSync(OUT, { recursive: true });
  const plays = (await all('plays', '*')).filter(r => r.level_id);
  const moves = await all('moves', '*');
  fs.writeFileSync(path.join(OUT, 'plays.json'), JSON.stringify(plays));
  fs.writeFileSync(path.join(OUT, 'moves.json'), JSON.stringify(moves));
  console.log(`[veri] plays ${plays.length}, moves ${moves.length}${cohort ? ' (cohort ' + cohort + ')' : ''} -> Logs/veri`);
})().catch(e => { console.error(e.message); process.exit(1); });
