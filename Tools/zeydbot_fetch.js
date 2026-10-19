// ZeydBot: downloads Zeyd's Z-level rows (plays + moves) with the game's own publishable key
// (read-only; anon SELECT on both tables). Player: TEST_ZEYD, and "ddd" from session
// ad844d4abb404f5f97b5d9e1c28b23f7 on (docs/analiz/zeydbot_notlar.md).
//
//   node Tools/zeydbot_fetch.js            -> Logs/zeydbot/plays.json, Logs/zeydbot/moves.json
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');
const settings = fs.readFileSync(path.join(ROOT, 'Assets/_Game/Platform/Web/TelemetrySettings.cs'), 'utf8');
const url = /SupabaseUrl\s*=\s*"([^"]+)"/.exec(settings)[1].replace(/\/rest\/v1\/?$/, '').replace(/\/$/, '');
const key = /AnonKey\s*=\s*"([^"]+)"/.exec(settings)[1];
const DDD_FROM_SESSION = 'ad844d4abb404f5f97b5d9e1c28b23f7';

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
  const plays = await get('plays', 'select=*&level_id=like.Z*&player=in.(TEST_ZEYD,ddd)&order=created_at');
  // ddd counts as Zeyd from the given session on
  const start = plays.find(p => p.session === DDD_FROM_SESSION);
  const startAt = start ? start.created_at : null;
  // Z21-Z30 are the frozen test set: their rows are never downloaded here (overnight 2026-10-20)
  const zeyd = plays.filter(p => p.player === 'TEST_ZEYD' || (p.player === 'ddd' && startAt && p.created_at >= startAt))
    .filter(p => p.level_id <= 'Z20');
  const ids = new Set(zeyd.map(p => p.row_id));
  const moves = (await get('moves', 'select=*&level_id=like.Z*&order=row_id,move_no')).filter(m => ids.has(m.row_id));
  const out = path.join(ROOT, 'Logs/zeydbot');
  fs.mkdirSync(out, { recursive: true });
  fs.writeFileSync(path.join(out, 'plays.json'), JSON.stringify(zeyd, null, 1));
  fs.writeFileSync(path.join(out, 'moves.json'), JSON.stringify(moves, null, 1));
  const byLevel = {};
  for (const m of moves) byLevel[m.level_id] = (byLevel[m.level_id] || 0) + 1;
  // raw copies for the record (docs/analiz/zeydbot)
  const csv = (rows) => { const k = [...new Set(rows.flatMap(Object.keys))]; return [k.join(','), ...rows.map(r => k.map(c => { const v = r[c] == null ? '' : String(r[c]); return /[",\n]/.test(v) ? '"' + v.replace(/"/g, '""') + '"' : v; }).join(','))].join('\n') + '\n'; };
  const docs = path.join(ROOT, 'docs/analiz/zeydbot');
  fs.mkdirSync(docs, { recursive: true });
  fs.writeFileSync(path.join(docs, 'ham_plays.csv'), csv(zeyd));
  fs.writeFileSync(path.join(docs, 'ham_moves.csv'), csv(moves));
  console.log(`plays ${zeyd.length} (ddd from ${startAt || '—'}), moves ${moves.length}`);
  console.log(Object.keys(byLevel).sort().map(k => `${k}:${byLevel[k]}`).join(' '));
})().catch(e => { console.error(e.message); process.exit(1); });
