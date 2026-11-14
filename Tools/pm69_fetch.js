// PM_69: downloads pt1 moves (all attempts) and plays with the game's publishable key (anon SELECT).
//   node Tools/pm69_fetch.js  -> Logs/pm69/moves_pt1.json
const fs = require('fs');
const path = require('path');
const ROOT = path.resolve(__dirname, '..');
const settings = fs.readFileSync(path.join(ROOT, 'Assets/_Game/Platform/Web/TelemetrySettings.cs'), 'utf8');
const url = /SupabaseUrl\s*=\s*"([^"]+)"/.exec(settings)[1].replace(/\/rest\/v1\/?$/, '').replace(/\/$/, '');
const key = /AnonKey\s*=\s*"([^"]+)"/.exec(settings)[1];
async function get(table, query) {
  const rows = [];
  for (let from = 0; ; from += 1000) {
    const r = await fetch(`${url}/rest/v1/${table}?${query}`, { headers: { apikey: key, Authorization: `Bearer ${key}`, Range: `${from}-${from + 999}` } });
    if (!r.ok) throw new Error(`${table}: HTTP ${r.status} ${await r.text()}`);
    const page = await r.json();
    rows.push(...page);
    if (page.length < 1000) return rows;
  }
}
(async () => {
  const moves = await get('moves', 'select=row_id,player,level_id,level_version,attempt,move_no,kind,piece,dir,t_ms,think_ms,time_left,fuses&cohort=eq.pt1&order=id');
  const plays = await get('plays', 'select=row_id,player,level_id,level_version,attempt,result,dur,left_s,moves,jams&cohort=eq.pt1&order=id');
  const out = path.join(ROOT, 'Logs/pm69');
  fs.mkdirSync(out, { recursive: true });
  fs.writeFileSync(path.join(out, 'moves_pt1.json'), JSON.stringify(moves));
  fs.writeFileSync(path.join(out, 'plays_pt1.json'), JSON.stringify(plays));
  console.log(`moves ${moves.length}, plays ${plays.length}`);
})().catch(e => { console.error(e.message); process.exit(1); });
