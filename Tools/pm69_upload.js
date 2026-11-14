// PM_69: uploads docs/analiz/v2/{level_features_v2,replay_pt1,replay_pt1_steps}.csv with the publishable key.
// Needs the temporary anon insert policies of docs/sql/018 (drop them right after).   node Tools/pm69_upload.js
const fs = require('fs');
const path = require('path');
const ROOT = path.resolve(__dirname, '..');
const settings = fs.readFileSync(path.join(ROOT, 'Assets/_Game/Platform/Web/TelemetrySettings.cs'), 'utf8');
const url = /SupabaseUrl\s*=\s*"([^"]+)"/.exec(settings)[1].replace(/\/rest\/v1\/?$/, '').replace(/\/$/, '');
const key = /AnonKey\s*=\s*"([^"]+)"/.exec(settings)[1];
const JSONB = new Set(['fuse', 'slack_true', 'nat_slack_per_bomb_rand', 'distract_per_step', 'distract_vs_slack']);
const TEXT = new Set(['set_name', 'level_id', 'row_id', 'player', 'result', 'reason', 'inferred', 'limiters']);
function parseCsv(file) {
  const lines = fs.readFileSync(path.join(ROOT, file.startsWith('replay') ? 'Logs/pm69' : 'docs/analiz/v2', file), 'utf8').trim().split(/\r?\n/);
  const split = l => { const out = []; let cur = '', q = false; for (const ch of l) { if (q) { if (ch === '"') q = false; else cur += ch; } else if (ch === '"') q = true; else if (ch === ',') { out.push(cur); cur = ''; } else cur += ch; } out.push(cur); return out; };
  const head = split(lines[0]);
  return lines.slice(1).map(l => {
    const v = split(l), o = {};
    head.forEach((h, i) => {
      const x = v[i] ?? '';
      if (JSONB.has(h)) o[h] = x === '' ? [] : x.split(';').map(y => y === '' ? null : Number(y));
      else if (TEXT.has(h)) o[h] = x === '' ? null : x;
      else if (x === 't' || x === 'f') o[h] = x === 't';
      else o[h] = x === '' ? null : Number(x);
    });
    return o;
  });
}
async function put(table, rows) {
  for (let i = 0; i < rows.length; i += 1000) {
    const r = await fetch(`${url}/rest/v1/${table}`, { method: 'POST', body: JSON.stringify(rows.slice(i, i + 1000)),
      headers: { apikey: key, Authorization: `Bearer ${key}`, 'Content-Type': 'application/json', Prefer: 'return=minimal' } });
    if (!r.ok) throw new Error(`${table} @${i}: HTTP ${r.status} ${await r.text()}`);
  }
  console.log(`${table}: ${rows.length}`);
}
(async () => {
  const only = process.argv[2];
  for (const [t, f] of [['level_features_v2', 'level_features_v2.csv'], ['replay_pt1', 'replay_pt1.csv'], ['replay_pt1_steps', 'replay_pt1_steps.csv']])
    if (!only || only === t) await put(t, parseCsv(f));
})().catch(e => { console.error(e.message); process.exit(1); });
