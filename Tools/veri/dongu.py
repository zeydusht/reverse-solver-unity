"""Veri sistemi, one command (PM_MESAJ_25 step 15): fetch -> replay -> fit bots -> population Monte Carlo -> back test.
Unity must be closed (batch mode). Out: docs/veri/botlar_<run>.json, tahmin_<run>.csv, geri_test_<run>.md, rapor_<run>.md.

  python Tools/veri/dongu.py --cohort pt1 --run v2 [--hepsi] [--n 1000]
    --cohort  only that cohort's rows are learned from (pt1: the 20 testers); --hepsi: every real player's rows
    --n       virtual players per level (the back test uses 300)"""
import argparse, csv, os, subprocess, sys

ROOT = os.path.join(os.path.dirname(__file__), '..', '..')


def run(*cmd):
    print('>', ' '.join(cmd), flush=True)
    r = subprocess.run(list(cmd), cwd=ROOT, capture_output=True, text=True, encoding='utf-8', errors='replace')
    lines = [l for l in (r.stdout + r.stderr).splitlines() if l.startswith('[') or 'error' in l.lower() or 'Traceback' in l or 'Hüküm' in l]
    print('\n'.join(lines[-12:]), flush=True)
    if r.returncode != 0 and 'unity_batch' not in ' '.join(cmd): sys.exit(f'durdu: {" ".join(cmd)}')
    return r


def main():
    ap = argparse.ArgumentParser(); ap.add_argument('--cohort'); ap.add_argument('--run', required=True)
    ap.add_argument('--hepsi', action='store_true'); ap.add_argument('--n', default='1000'); a = ap.parse_args()
    only = [] if a.hepsi or not a.cohort else ['--cohort', a.cohort]
    run('node', 'Tools/veri/fetch.js')
    run(sys.executable, 'Tools/veri/hazirla.py', *only)
    run('bash', 'Tools/unity_batch.sh', 'method', 'ReverseSolver.LevelEditor.Veri.Replay')
    run(sys.executable, 'Tools/veri/bot_fit.py', '--run', a.run)
    run('bash', 'Tools/unity_batch.sh', 'method', 'ReverseSolver.LevelEditor.Veri.Bots', '-rsRun', a.run, '-rsN', a.n)
    run(sys.executable, 'Tools/veri/geri_test.py', '--run', a.run)
    pred = list(csv.DictReader(open(os.path.join(ROOT, f'docs/veri/tahmin_{a.run}.csv'), encoding='utf-8')))
    test = open(os.path.join(ROOT, f'docs/veri/geri_test_{a.run}.md'), encoding='utf-8').read()
    rep = [f'# Veri döngüsü {a.run}', '', f'Öğrenilen veri: {"kohort " + a.cohort if only else "bütün gerçek oyuncular"}. '
           f'Dosyalar: botlar_{a.run}.json, tahmin_{a.run}.csv, geri_test_{a.run}.md.', '',
           '| # | Bölüm | p_win1 (%90) | bomba | süre | tahmini puan |', '|---|---|---|---|---|---|']
    rep += [f"| {r['slot']} | {r['level_id']} v{r['level_version']} | {float(r['p_win1']):.0%} ({float(r['p_win1_lo']):.0%}–{float(r['p_win1_hi']):.0%}) | "
            f"{float(r['p_fail_bomb']):.0%} | {float(r['p_fail_time']):.0%} | {r['pred_rating']} |" for r in pred]
    rep += ['', '## Geri test', '', test.split('\n', 2)[2]]
    open(os.path.join(ROOT, f'docs/veri/rapor_{a.run}.md'), 'w', encoding='utf-8', newline='\n').write('\n'.join(rep) + '\n')
    print(f'[dongu] bitti: docs/veri/rapor_{a.run}.md')


if __name__ == '__main__':
    main()
