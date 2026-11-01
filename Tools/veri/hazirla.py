"""Veri sistemi Faz 2 (PM_MESAJ_25), step 1 after Tools/veri/fetch.js: who is who, and which level version.

Out (Logs/veri/):
  seviyeler.json   every level version we have (redesign.json, redesign_v3/v2, zeydbot.json), ids "S47@2"
  hamle_girdi.csv  the moves of real players' attempts, ordered, with player_key and level_key, for Veri.Replay
  denemeler.csv    one row per attempt (plays): player_key, level_key, attempt, result, dur, left_s, jams, platform

Players: test names, the verification players and the demo are left out; Zeyd's names are one player ("zeyd").
A cohort row is keyed by its device_id (two people may share a name), an older row by its name.

  python Tools/veri/hazirla.py [--cohort pt1]"""
import csv, json, os, re, sys

ROOT = os.path.join(os.path.dirname(__file__), '..', '..')
LV, LOG = os.path.join(ROOT, 'Assets/_Game/Levels'), os.path.join(ROOT, 'Logs/veri')
ZEYD = {'ddd', 'deneme1', 'zeyd testt', 'zeyd denimi', 'zeyd', 'test_zeyd'}
SKIP = re.compile(r'^(test|ic_kontrol$|show$|<$)', re.I)


def player_key(r):
    name = (r.get('player') or '').strip()
    if name.lower() in ZEYD: return 'zeyd'
    if not name or SKIP.match(name): return None
    return ('dev:' + r['device_id']) if r.get('cohort') and r.get('device_id') else 'ad:' + name


def main():
    cohort = sys.argv[sys.argv.index('--cohort') + 1] if '--cohort' in sys.argv else None
    book = {}
    for f in ('redesign.json', 'redesign_v3.json', 'redesign_v2.json', 'zeydbot.json'):
        p = os.path.join(LV, f)
        if not os.path.exists(p): continue
        for l in json.load(open(p, encoding='utf-8'))['levels']:
            k = f"{l['id']}@{l.get('version', 1)}"
            if k not in book: book[k] = dict(l, id=k)
    json.dump({'format': 1, 'levels': list(book.values())}, open(os.path.join(LOG, 'seviyeler.json'), 'w', encoding='utf-8'), ensure_ascii=False)

    plays = json.load(open(os.path.join(LOG, 'plays.json'), encoding='utf-8'))
    moves = json.load(open(os.path.join(LOG, 'moves.json'), encoding='utf-8'))
    if cohort:
        plays = [r for r in plays if r.get('cohort') == cohort]
        moves = [m for m in moves if m.get('cohort') == cohort]
    by_row = {r['row_id']: r for r in plays if r.get('row_id')}
    att = {}
    for r in plays:
        k = player_key(r)
        lk = f"{r['level_id']}@{r.get('level_version') or 1}"
        if k and lk in book:
            att[r['row_id']] = dict(player_key=k, level_key=lk, attempt=r['attempt'], result=r['result'], dur=r['dur'], left_s=r['left_s'],
                                    jams=r['jams'], moves=r['moves'], platform=r.get('platform') or '', zorluk=r.get('zorluk_puani') if r.get('zorluk_puani') is not None else '')
    with open(os.path.join(LOG, 'denemeler.csv'), 'w', encoding='utf-8', newline='') as f:
        w = csv.writer(f); w.writerow(['row_id', 'player_key', 'level_key', 'attempt', 'result', 'dur', 'left_s', 'jams', 'moves', 'platform', 'zorluk'])
        for rid, a in att.items(): w.writerow([rid] + list(a.values()))

    n = 0
    with open(os.path.join(LOG, 'hamle_girdi.csv'), 'w', encoding='utf-8', newline='') as f:
        w = csv.writer(f)
        w.writerow(['row_id', 'player_key', 'level_key', 'attempt', 'move_no', 'kind', 'piece', 'dir', 't_ms', 'think_ms', 'time_left', 'platform'])
        for m in sorted(moves, key=lambda m: (m['row_id'], m['move_no'])):
            a = att.get(m['row_id'])
            if a is None:                          # an attempt with no plays row yet (abandoned): keyed from the moves row
                k = player_key(m); lk = f"{m['level_id']}@{m.get('level_version') or 1}"
                if not k or lk not in book: continue
                a = dict(player_key=k, level_key=lk, attempt=m['attempt'], platform='')
            w.writerow([m['row_id'], a['player_key'], a['level_key'], a['attempt'], m['move_no'], m['kind'], m['piece'] if m['piece'] is not None else -1,
                        m['dir'] or '', m['t_ms'] or 0, m['think_ms'] or 0, m['time_left'] if m['time_left'] is not None else -1, a['platform']])
            n += 1
    print(f'[veri] {len(book)} bölüm sürümü, {len(att)} deneme, {n} hamle satırı -> Logs/veri')


if __name__ == '__main__':
    main()
