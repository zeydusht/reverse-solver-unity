// PM_69: Logs/pm69/moves_pt1.json + plays_pt1.json (Tools/pm69_fetch.js) -> moves.tsv (by row_id, move_no) and plays.tsv
const fs = require('fs');
const M = JSON.parse(fs.readFileSync('Logs/pm69/moves_pt1.json'));
M.forEach((m, i) => m.i = i);
M.sort((a, b) => a.row_id < b.row_id ? -1 : a.row_id > b.row_id ? 1 : (a.move_no - b.move_no) || (a.i - b.i));
const c = v => v == null ? '' : String(v).replace(/[\t\n]/g, ' ');
fs.writeFileSync('Logs/pm69/moves.tsv', M.map(m => [m.row_id, m.player, m.level_id, m.level_version, m.attempt, m.move_no, m.kind, m.piece, m.dir, m.t_ms, m.think_ms, m.time_left, m.fuses].map(c).join('\t')).join('\n'));
const P = JSON.parse(fs.readFileSync('Logs/pm69/plays_pt1.json'));
fs.writeFileSync('Logs/pm69/plays.tsv', P.map(p => [p.row_id, p.result, p.moves, p.jams, p.dur].map(c).join('\t')).join('\n'));
console.log(`moves ${M.length}, plays ${P.length}`);
