"""Veri sistemi M2 (PM_MESAJ_25, docs/VERI_SISTEMI.md 6): player bots from docs/veri/secimler.csv (Veri.Replay).

Choice: conditional logit over the candidates of each exit,
    u = right*Right + learn*Right*(attempt>1) + bomb*Bomb + slack*SlackDelta + free*Frees + dist*Dist + chain*Chain
Time per exit (s, from the previous exit; the first exit is left out, the clock starts with it):
    log t ~ N(mu + mu_opts*log(options) + mu_press*pressure + mu_later*(attempt>1), sigma)
jam_rate: jams per exit (Logs/veri/hamle_girdi.csv).
Partial pooling: every player = population mean + own deviation, MAP with a normal prior around the mean (empirical
Bayes): with little data a player stays near the mean. Between-player spread: estimated from the players when there
are 3 or more with data, otherwise the PRIOR_SD assumption below (written into the json).

  python Tools/veri/bot_fit.py --run v1 [--out docs/veri/botlar_v1.json] [--drop-level S47@2,...] [--drop-player zeyd]
"""
import argparse, csv, json, math, os, subprocess
from collections import defaultdict
import numpy as np

ROOT = os.path.join(os.path.dirname(__file__), '..', '..')
CHOICE = ['right', 'bomb', 'slack', 'free', 'dist', 'chain', 'learn']
THINK = ['mu', 'mu_opts', 'mu_press', 'mu_later']
NAMES = CHOICE + THINK + ['sigma', 'jam_rate']          # Veri.cs Names order
# Assumed between-player spread while there are fewer than 3 players (one person's data cannot show it).
PRIOR_SD = dict(right=.6, bomb=.3, slack=.15, free=.15, dist=.5, chain=.3, learn=.4, mu=.3, mu_opts=.1, mu_press=.2, mu_later=.15, sigma=.1, jam_rate=.1)
RIDGE = .05                                              # weak pull to 0 for the pooled fit (a coefficient no data moves)


def load(path, drop_levels, drop_players):
    sets = defaultdict(list)
    for r in csv.DictReader(open(path, encoding='utf-8')):
        if r['level_key'] in drop_levels or r['player_key'] in drop_players: continue
        sets[(r['player_key'], r['row_id'], int(r['step']))].append(r)
    return sets


def design(rows):
    later = 1.0 if int(rows[0]['attempt']) > 1 else 0.0
    X = np.array([[float(r['right']), float(r['bomb']), float(r['slack_delta']), float(r['frees']), float(r['dist']), float(r['chain']),
                   float(r['right']) * later] for r in rows])
    y = next(i for i, r in enumerate(rows) if r['chosen'] == '1')
    return X, y


def pack(groups):
    """Choice sets padded into one array: X [G, C, k], mask [G, C], chosen index [G]."""
    G, C, k = len(groups), max(len(X) for X, _ in groups), groups[0][0].shape[1]
    Xa = np.zeros((G, C, k)); M = np.zeros((G, C), bool); y = np.zeros(G, int)
    for i, (X, yi) in enumerate(groups): Xa[i, :len(X)] = X; M[i, :len(X)] = True; y[i] = yi
    return Xa, M, y


def fit_logit(groups, mean=None, prec=None, iters=40):
    """Newton for the conditional logit; prior N(mean, 1/prec) per coefficient (ridge when mean is None)."""
    k = len(CHOICE)
    mean = np.zeros(k) if mean is None else mean
    prec = np.full(k, RIDGE) if prec is None else prec
    b = mean.copy()
    if not groups: return b, np.full(k, np.inf), 0.0
    Xa, M, y = pack(groups)
    G = np.arange(len(y))
    for _ in range(iters):
        u = np.where(M, Xa @ b, -np.inf); u -= u.max(axis=1, keepdims=True)
        p = np.exp(u); p /= p.sum(axis=1, keepdims=True)
        ll = float(np.log(np.clip(p[G, y], 1e-300, None)).sum())
        xb = np.einsum('gc,gck->gk', p, Xa)
        g = (Xa[G, y] - xb).sum(axis=0) - prec * (b - mean)
        H = -(np.einsum('gc,gck,gcl->kl', p, Xa, Xa) - xb.T @ xb) - np.diag(prec)
        step = np.linalg.solve(H, -g)
        b = b + step
        if np.abs(step).max() < 1e-6: break
    se = np.sqrt(np.clip(np.diag(np.linalg.inv(-H)), 0, None))
    return b, se, ll


def think_rows(groups_rows):
    X, y = [], []
    for rows in groups_rows:
        ch = next(r for r in rows if r['chosen'] == '1')
        if int(ch['step']) == 0: continue
        dt = float(ch['dt_ms']) / 1000
        if dt <= 0: continue
        X.append([1, math.log(max(1, int(ch['n_opts']))), float(ch['pressure']), 1.0 if int(ch['attempt']) > 1 else 0.0])
        y.append(math.log(max(dt, .15)))
    return np.array(X), np.array(y)


def fit_think(X, y, mean=None, prec=None):
    k = len(THINK)
    mean = np.zeros(k) if mean is None else mean
    prec = np.full(k, 1e-3) if prec is None else prec
    if len(y) == 0: return mean, np.full(k, np.inf), .6
    A = X.T @ X + np.diag(prec); b = np.linalg.solve(A, X.T @ y + prec * mean)
    res = y - X @ b; s = float(np.sqrt(res @ res / max(1, len(y) - k)))
    return b, np.sqrt(np.diag(np.linalg.inv(A))) * s, s


def jam_rates(drop_levels, drop_players):
    jams, exits = defaultdict(int), defaultdict(int)
    for r in csv.DictReader(open(os.path.join(ROOT, 'Logs/veri/hamle_girdi.csv'), encoding='utf-8')):
        if r['level_key'] in drop_levels or r['player_key'] in drop_players: continue
        if r['kind'] == 'jam': jams[r['player_key']] += 1
        elif r['kind'] == 'exit': exits[r['player_key']] += 1
    return jams, exits


def fit(path, drop_levels=(), drop_players=()):
    sets = load(path, set(drop_levels), set(drop_players))
    by_player = defaultdict(list)
    for (pk, _, _), rows in sets.items(): by_player[pk].append(rows)
    players = sorted(by_player)
    allg = [design(r) for rows in by_player.values() for r in rows]
    pooled_b, pooled_se, _ = fit_logit(allg)
    TX, Ty = think_rows([r for rows in by_player.values() for r in rows])
    pooled_t, _, pooled_s = fit_think(TX, Ty)
    jams, exits = jam_rates(set(drop_levels), set(drop_players))
    pooled_jam = sum(jams.values()) / max(1, sum(exits.values()))
    mean = dict(zip(CHOICE, pooled_b)); mean.update(zip(THINK, pooled_t)); mean['sigma'] = pooled_s; mean['jam_rate'] = pooled_jam

    estimated = len(players) >= 3
    sd = dict(PRIOR_SD)
    bots = []
    raw = {}
    for pk in players:
        g = [design(r) for r in by_player[pk]]
        b, se, _ = fit_logit(g, pooled_b, np.full(len(CHOICE), RIDGE))
        raw[pk] = (b, se)
    if estimated:                                        # method of moments: spread of the player estimates minus their noise
        B = np.array([raw[p][0] for p in players]); S = np.array([raw[p][1] for p in players])
        tau2 = np.clip(B.var(axis=0) - (S ** 2).mean(axis=0), .01, None)
        for i, n in enumerate(CHOICE): sd[n] = float(math.sqrt(tau2[i]))
    for pk in players:
        rows = by_player[pk]
        g = [design(r) for r in rows]
        prec = np.array([1 / sd[n] ** 2 for n in CHOICE])
        b, se, ll = fit_logit(g, pooled_b, prec)
        tx, ty = think_rows(rows)
        t, _, s = fit_think(tx, ty, pooled_t, np.array([1 / sd[n] ** 2 for n in THINK]) * max(1, len(ty)) / 50)
        j, e = jams[pk], exits[pk]
        jam = (j + pooled_jam * 20) / (e + 20)          # beta-binomial pull, 20 exits' worth of prior
        p = dict(zip(CHOICE, map(float, b))); p.update(zip(THINK, map(float, t))); p['sigma'] = float(s); p['jam_rate'] = float(jam)
        bots.append(dict(bot_id=pk, player=pk, kind='clone', n_moves=len(rows), n_attempts=len({r[0]['row_id'] for r in rows}),
                         params={k: round(v, 4) for k, v in p.items()}, se={n: round(float(x), 4) for n, x in zip(CHOICE, se)}, loglik=round(ll, 2)))
    if estimated and len(players) >= 3:
        P = np.array([[bt['params'][n] for n in NAMES] for bt in bots])
        cov = np.cov(P.T)
    else:
        cov = np.diag([sd[n] ** 2 for n in NAMES])
    return dict(mean={k: round(float(v), 4) for k, v in mean.items()}, cov=np.round(cov, 6).tolist(), names=NAMES,
                spread='estimated' if estimated else 'assumed (PRIOR_SD; fewer than 3 players)',
                pooled_se={n: round(float(x), 4) for n, x in zip(CHOICE, pooled_se)}, n_choices=len(allg), n_think=int(len(Ty)),
                players=len(players)), bots


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--run', default='v1'); ap.add_argument('--out'); ap.add_argument('--input', default=os.path.join(ROOT, 'docs/veri/secimler.csv'))
    ap.add_argument('--drop-level', default=''); ap.add_argument('--drop-player', default=''); ap.add_argument('--levels', default='')
    a = ap.parse_args()
    pop, bots = fit(a.input, [x for x in a.drop_level.split(',') if x], [x for x in a.drop_player.split(',') if x])
    commit = subprocess.run(['git', 'rev-parse', '--short', 'HEAD'], capture_output=True, text=True, cwd=ROOT).stdout.strip()
    out = dict(run_id=a.run, commit=commit, population=pop, bots=bots, levels=[x for x in a.levels.split(',') if x])
    path = a.out or os.path.join(ROOT, f'docs/veri/botlar_{a.run}.json')
    json.dump(out, open(path, 'w', encoding='utf-8'), ensure_ascii=False, indent=1)
    m = pop['mean']
    print(f"[bot_fit] {a.run}: {pop['players']} oyuncu, {pop['n_choices']} seçim, {pop['n_think']} süre; "
          + ', '.join(f'{k} {m[k]:.2f}' for k in NAMES) + f' -> {os.path.relpath(path, ROOT)}')


if __name__ == '__main__':
    main()
