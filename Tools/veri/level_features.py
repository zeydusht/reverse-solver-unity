"""Reverse Solver - statik bolum ozellikleri (veri sistemi v0, PM 31 Ekim 2026).

Bolum JSON'undan (redesign.json bicimi) her bolum icin sayilabilir ozellikleri cikarir.
Yalniz dosyadan okunabilenleri hesaplar; hamle yasalligi gerektiren metrikler
(acilis hamlesi k0, serbestlik, darbogaz, en kisa kazma yolu) FunSim/SmartPlayer'dan gelir
ve ayni tabloya `sim_*` sutunlari olarak eklenir.

Kullanim:
  python Tools/veri/level_features.py Assets/_Game/Levels/redesign.json > docs/veri/level_features_v4.csv
"""
import csv, json, sys, statistics as st


def features(l, set_name='redesign-v4'):
    pieces = l['pieces']
    sol = [s['piece'] for s in l.get('solution') or []]
    pos = {p: i + 1 for i, p in enumerate(sol)}          # kacinci cikista cikiyor (1 tabanli)
    bombs = {int(k): v for k, v in (l.get('bombs') or {}).items()}
    nails = {int(k): v for k, v in (l.get('nails') or {}).items()}
    sealed = l.get('sealed') or {}
    # kayitli cozume gore fitil payi: fitil - (bomba cikana kadar gecen cikis sayisi)
    slack = [f - pos[b] for b, f in bombs.items() if b in pos]
    n_units = len(pieces)
    sol_len = len(sol)
    return {
        'set_name': set_name,
        'level_id': l['id'],
        'level_version': l.get('version', 1),
        'slot': l.get('lv'),
        'w': l['w'], 'h': l['h'], 'cells': l['w'] * l['h'],
        'pieces': n_units,
        'chains': sum(1 for p in pieces if len(p) > 1),
        'bombs': len(bombs),
        'fuse_min': min(bombs.values()) if bombs else None,
        'fuse_mean': round(st.mean(bombs.values()), 2) if bombs else None,
        'fuse_slack_min': min(slack) if slack else None,
        'fuse_slack_mean': round(st.mean(slack), 2) if slack else None,
        'nails': len(nails),
        'nail_max': max(nails.values()) if nails else None,
        'sealed_lanes': sum(len(v) for v in sealed.values()),
        'sealed_sides': len([k for k, v in sealed.items() if v]),
        'timer_s': l['timer'],
        'sol_len': sol_len,
        'sec_per_move': round(l['timer'] / sol_len, 3) if sol_len else None,
        'open_edge_pct': l.get('open'),
        'genre': l.get('genre'),
        'hard_card': bool(l.get('hard')),
        'boosters': sum((l.get('boosters') or {}).values()),
    }


COLS = list(features({'id': 'x', 'w': 1, 'h': 1, 'timer': 1, 'pieces': [[[0, 0]]], 'solution': [{'piece': 0}]}).keys())

if __name__ == '__main__':
    data = json.load(open(sys.argv[1], encoding='utf-8'))
    set_name = sys.argv[2] if len(sys.argv) > 2 else 'redesign-v4'
    w = csv.DictWriter(sys.stdout, fieldnames=COLS)
    w.writeheader()
    for l in data['levels']:
        w.writerow(features(l, set_name))
