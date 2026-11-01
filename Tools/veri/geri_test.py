"""Veri sistemi, geri test (PM_MESAJ_25 step 14): does the bot population (v1) predict first-try wins better than v0
and the fixed baselines, on data the bots did not see?

Truth: real players' first attempts (Logs/veri/denemeler.csv, quits left out): won = 1, bomb / time = 0.
Folds:
  level-out   for every level version with a first attempt: bots fitted without that level's moves, the population
              plays it (Veri.Bots -rsFolds, -rsN virtual players)
  player-out  for every player: bots fitted without that player, played on that player's levels (needs >= 2 players)
Rivals: v0 (docs/veri/tahmin_v0.csv, only where its level version was played), the mean of the other levels
(fixed), and a logistic on the start's tightest bomb slack (fuse-only), both refitted per fold.
Scores: Brier, MAE (|y - p|), Spearman of p against the outcome, and of the predicted rating against the player's
own 1-10 where given.  Out: docs/veri/geri_test_<run>.md

  python Tools/veri/geri_test.py [--run v1] [--n 300] [--skip-unity]"""
import argparse, csv, json, math, os, shutil, subprocess, sys
from collections import defaultdict
import numpy as np

sys.path.insert(0, os.path.dirname(__file__))
import bot_fit

ROOT = os.path.join(os.path.dirname(__file__), '..', '..')
FOLDS = os.path.join(ROOT, 'Logs/veri/folds')


def rank(x):
    o = np.argsort(x, kind='mergesort'); r = np.empty(len(x)); r[o] = np.arange(len(x))
    for v in np.unique(x):                                 # ties: mean rank
        idx = np.where(x == v)[0]; r[idx] = r[idx].mean()
    return r


def spearman(a, b):
    a, b = np.asarray(a, float), np.asarray(b, float)
    if len(a) < 3 or a.std() == 0 or b.std() == 0: return float('nan')
    return float(np.corrcoef(rank(a), rank(b))[0, 1])


def scores(y, p):
    y, p = np.asarray(y, float), np.asarray(p, float)
    return dict(n=len(y), brier=float(((p - y) ** 2).mean()), mae=float(np.abs(p - y).mean()), rho=spearman(p, y))


def logistic(x, y, iters=50):
    X = np.c_[np.ones(len(x)), x]; b = np.zeros(X.shape[1])
    for _ in range(iters):
        p = 1 / (1 + np.exp(-X @ b)); W = p * (1 - p)
        H = X.T @ (X * W[:, None]) + .1 * np.eye(len(b)); b += np.linalg.solve(H, X.T @ (y - p) - .1 * b)
    return b


def main():
    ap = argparse.ArgumentParser(); ap.add_argument('--run', default='v1'); ap.add_argument('--n', type=int, default=300)
    ap.add_argument('--skip-unity', action='store_true'); a = ap.parse_args()
    sec = os.path.join(ROOT, 'docs/veri/secimler.csv')
    slack0, seen = {}, set()
    for r in csv.DictReader(open(sec, encoding='utf-8')):
        seen.add(r['level_key'])
        if r['step'] == '0': slack0[r['level_key']] = int(r['min_slack'])
    first = [r for r in csv.DictReader(open(os.path.join(ROOT, 'Logs/veri/denemeler.csv'), encoding='utf-8'))
             if r['attempt'] == '1' and r['result'] != 'quit' and r['level_key'] in seen]
    levels = sorted({r['level_key'] for r in first})
    players = sorted({r['player_key'] for r in first})

    if not a.skip_unity:
        shutil.rmtree(FOLDS, ignore_errors=True); os.makedirs(FOLDS)
        for lk in levels:
            pop, _ = bot_fit.fit(sec, drop_levels=[lk])
            json.dump(dict(levels=[lk], population=pop), open(os.path.join(FOLDS, f'fold_L_{lk.replace("@", "_")}.json'), 'w'))
        if len(players) >= 2:
            for pk in players:
                pop, _ = bot_fit.fit(sec, drop_players=[pk])
                lv = sorted({r['level_key'] for r in first if r['player_key'] == pk})
                json.dump(dict(levels=lv, population=pop), open(os.path.join(FOLDS, f'fold_P_{pk.replace(":", "_")}.json'), 'w'))
        print(f'[geri_test] {len(os.listdir(FOLDS))} kat yazıldı; Unity koşuyor', flush=True)
        r = subprocess.run(['bash', 'Tools/unity_batch.sh', 'method', 'ReverseSolver.LevelEditor.Veri.Bots', '-rsFolds', 'Logs/veri/folds', '-rsN', str(a.n)],
                           cwd=ROOT, capture_output=True, text=True)
        print('\n'.join(l for l in r.stdout.splitlines() if '[veri]' in l or 'error' in l))
    pred = defaultdict(dict)
    for r in csv.DictReader(open(os.path.join(FOLDS, 'tahmin.csv'), encoding='utf-8')):
        kind = 'level' if r['fold'].startswith('fold_L_') else 'player:' + r['fold'][7:]
        pred[kind][r['level_key']] = (float(r['p_win1']), float(r['pred_rating']))
    v0 = {f"{r['level_id']}@{r['level_version']}": float(r['p_win1']) for r in csv.DictReader(open(os.path.join(ROOT, 'docs/veri/tahmin_v0.csv'), encoding='utf-8'))}

    y = np.array([1.0 if r['result'] == 'win' else 0.0 for r in first])
    keys = [r['level_key'] for r in first]
    out = [f'# Geri test {a.run} (PM_MESAJ_25 adım 14)', '',
           f'Veri: gerçek oyuncuların ilk denemeleri (yarıda bırakılanlar hariç), {len(first)} deneme, {len(levels)} bölüm sürümü, '
           f'{len(players)} oyuncu ({", ".join(players)}). Kazanma {int(y.sum())}/{len(y)}. Bot: {a.n} sanal oyuncu/bölüm.', '']
    rows = []
    # level-out
    if pred['level']:
        idx = [i for i, k in enumerate(keys) if k in pred['level']]
        yy = y[idx]
        p_v1 = np.array([pred['level'][keys[i]][0] for i in idx])
        p_mean = np.array([np.delete(y, [j for j, k in enumerate(keys) if k == keys[i]]).mean() for i in idx])
        p_fuse = []
        for i in idx:
            tr = [j for j, k in enumerate(keys) if k != keys[i]]
            f = lambda k: (min(slack0.get(k, 99), 15), 1.0 if slack0.get(k, 99) < 99 else 0.0)
            b = logistic(np.array([f(keys[j]) for j in tr]), y[tr])
            x = f(keys[i]); p_fuse.append(1 / (1 + math.exp(-(b[0] + b[1] * x[0] + b[2] * x[1]))))
        rows += [('bölüm dışarıda', 'v1 botlar', scores(yy, p_v1)), ('bölüm dışarıda', 'sabit ortalama', scores(yy, p_mean)),
                 ('bölüm dışarıda', 'yalnız fitil payı', scores(yy, p_fuse))]
        both = [n for n, i in enumerate(idx) if keys[i] in v0]
        if both:
            rows += [('v0 ile ortak bölümler', 'v1 botlar', scores(yy[both], p_v1[both])), ('v0 ile ortak bölümler', 'v0', scores(yy[both], [v0[keys[idx[n]]] for n in both])),
                     ('v0 ile ortak bölümler', 'sabit ortalama', scores(yy[both], p_mean[both]))]
        rated = [(pred['level'][r['level_key']][1], float(r['zorluk'])) for r in first if r['zorluk'] and r['level_key'] in pred['level']]
        rated_v0 = [(v0[r['level_key']], float(r['zorluk'])) for r in first if r['zorluk'] and r['level_key'] in v0]
    players_out = [k for k in pred if k.startswith('player:')]
    if not players_out: out.append('**Oyuncu dışarıda bırakma yapılamadı:** veride tek gerçek oyuncu var (Zeyd\'in isimleri tek oyuncu sayıldı). pt1 gelince aynı komut bunu da yapar.\n')
    out += ['| Kat | Model | n | Brier ↓ | MAE ↓ | Spearman (p, sonuç) ↑ |', '|---|---|---|---|---|---|']
    out += [f"| {k} | {m} | {s['n']} | {s['brier']:.3f} | {s['mae']:.3f} | {s['rho']:.2f} |" for k, m, s in rows]
    if pred['level']:
        out += ['', f"Zeyd'in 1–10 puanıyla sıra korelasyonu: v1 tahmini puan ρ = {spearman(*zip(*rated)) if len(rated) > 2 else float('nan'):.2f} (n {len(rated)})"
                + (f"; v0 p_win1 (ters) ρ = {-spearman(*zip(*rated_v0)):.2f} (n {len(rated_v0)})" if len(rated_v0) > 2 else '') + '.']
        by = {(k, m): s for k, m, s in rows}
        v1b, mb = by[('bölüm dışarıda', 'v1 botlar')]['brier'], by[('bölüm dışarıda', 'sabit ortalama')]['brier']
        fb = by[('bölüm dışarıda', 'yalnız fitil payı')]['brier']
        verdict = 'v1 sabit ortalamayı ve fitil tabanını Brier\'de GEÇİYOR' if v1b < min(mb, fb) else \
                  'v1 tabanları GEÇEMİYOR' + (' (sabit ortalamadan kötü)' if v1b >= mb else ' (fitil tabanından kötü)')
        out += ['', f'**Hüküm:** {verdict} (v1 {v1b:.3f}, sabit {mb:.3f}, fitil {fb:.3f}).',
                'Not: veri tek oyuncudan (uzman tasarımcı, kazanma oranı yüksek); sınıflar dengesiz, n küçük. Bu test boru hattının çalıştığını gösterir; '
                'v1\'in asıl değeri pt1 verisiyle ölçülecek (VERI_SISTEMI 7.3).']
    path = os.path.join(ROOT, f'docs/veri/geri_test_{a.run}.md')
    open(path, 'w', encoding='utf-8', newline='\n').write('\n'.join(out) + '\n')
    print('\n'.join(out[-8:]))


if __name__ == '__main__':
    main()
