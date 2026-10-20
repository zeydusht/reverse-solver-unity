# ZeydBot v2 (zeydbot-v2 branch, PM 2026-10-20). v1 (Tools/zeydbot_model.py, frozen fc4ef7c) stays as it was.
# Changes against v1, all fitted inside each leave-one-out fold from the training levels only:
#   1. bomb-first rate: Zeyd's share of slack-keeping moves; the paths (ZeydBot.Paths, which logs per step how
#      many right moves keep the slack and whether the pick was one of them) are reweighted to that rate
#      (importance weights), so the rate changes per fold without new Unity runs
#   2. learning: the trap and thinking parts get "levels played before" (Zeyd played Z01, Z02 ... in order)
#   3. score: 1-10 = s0 + s1 loss chance + s2 timer used + s3 misleading pieces per step (Z12: looks free, is not)
# Data: first attempts of every Z level with a result; Z01 / Z07 flagged (maybe not blind); the five levels lost
# on the first try have their 1-10 from the second attempt (flag puan_2): reported with and without them.
#
#   python Tools/zeydbot_model_v2.py loo                      -> docs/analiz/zeydbot/loo_v2.md
#   python Tools/zeydbot_model_v2.py predict Z31 Z32 ...      -> table (fitted on every level with a result)
import csv, math, random, sys, os
sys.path.insert(0, os.path.dirname(__file__))
from zeydbot_model import D, rows, num, sig, logit, ols, spearman, think_x, clamp_score, slack_x

SIMS = 400
FLAG = ('Z01', 'Z07')


def load():
    att = [r for r in rows('denemeler.csv') if r['kayit'] == '']
    first = {r['level_id']: r for r in att if r['attempt'] == '1'}
    score, from2 = {}, set()
    for r in sorted(att, key=lambda r: int(r['attempt'])):
        if r['zorluk_puani'] and r['level_id'] not in score:
            score[r['level_id']] = int(r['zorluk_puani'])
            if r['attempt'] != '1': from2.add(r['level_id'])
    for r in rows('denemeler.csv'):              # Z01 has no move rows but a result
        if r['attempt'] == '1' and r['level_id'] not in first: first[r['level_id']] = r
    moves = [m for m in rows('hamleler.csv') if m['attempt'] == '1']
    feat = {r['level_id']: r for r in rows('bolum_ozellik.csv')}
    mis = {}
    for r in rows('bolum_yol.csv'):
        mis.setdefault(r['level_id'], []).append(float(r['yaniltici']))
    for l in mis: mis[l] = sum(mis[l]) / len(mis[l])
    paths = {}
    for r in rows('yollar.csv'):
        paths.setdefault(r['level_id'], {}).setdefault(r['run'], []).append(r)
    return first, score, from2, moves, feat, paths, mis


def played_before(level): return int(level[1:]) - 1     # Z01 first, in order (sessions were played in order)


# ---------------------------------------------------------------- 1. bomb-first rate + path weights
def dig_rate(moves, levels):
    keep = n = 0
    for m in moves:
        if m['level_id'] in levels and m['kind'] == 'exit' and m['en_dar_pay'] and m['secim_sonrasi_en_dar_pay']:
            n += 1; keep += int(m['secim_sonrasi_en_dar_pay']) >= int(m['en_dar_pay'])
    return keep / n


def path_weight(steps, d):
    """Likelihood ratio of this path under rate d against the rate it was made with (column dig)."""
    w = 1.0
    for s in steps:
        k, r = int(s['koruyan']), int(s['dogru'])
        if k == 0 or r == 0: continue
        d0 = float(s['dig'])
        p = lambda dd: (dd / k + (1 - dd) / r) if s['koruyan_secildi'] == '1' else (1 - dd) / r
        w *= p(d) / p(d0)
    return w


# ---------------------------------------------------------------- 2. trap choice with learning
def trap_samples(moves, levels):
    out, dead = [], set()
    for m in moves:
        l = m['level_id']
        if m['kind'] != 'exit' or not m['secenek'] or l not in levels or l in dead: continue
        n, r = int(m['secenek']), int(m['dogru'])
        if r == 0: dead.add(l); continue
        if r < n: out.append(((logit((n - r) / n), math.log1p(played_before(l))), 1 if m['secilen'] == 'tuzak' else 0))
        if m['secilen'] == 'tuzak': dead.add(l)
    return out


def fit_logistic2(xy, l2=0.5):
    """P = sig(a + b x + c t), ridge: b toward 1, a and c toward 0."""
    a, b, c = -1.0, 1.0, 0.0
    for _ in range(60):
        g = [0.0, -l2 * (b - 1), -l2 * c]
        H = [[1e-6, 0, 0], [0, l2, 0], [0, 0, l2]]
        for (x, t), y in xy:
            v = [1, x, t]; p = sig(a + b * x + c * t); w = p * (1 - p)
            for i in range(3):
                g[i] += (y - p) * v[i]
                for j in range(3): H[i][j] += w * v[i] * v[j]
        st = ols_solve(H, g)
        a += st[0]; b += st[1]; c += st[2]
    return a, b, c


def ols_solve(A, v):
    A = [row[:] for row in A]; v = v[:]; k = len(v)
    for i in range(k):
        p = A[i][i]
        for j in range(k): A[i][j] /= p
        v[i] /= p
        for r in range(k):
            if r != i:
                f = A[r][i]
                for j in range(k): A[r][j] -= f * A[i][j]
                v[r] -= f * v[i]
    return v


# ---------------------------------------------------------------- thinking time with learning
def fit_think(moves, levels):
    X, y, lv = [], [], []
    for m in moves:
        if m['kind'] != 'exit' or not m['secenek'] or m['level_id'] not in levels or m['move_no'] == '1': continue
        t = int(m['think_ms'])
        if t <= 0: continue
        X.append(think_x(int(m['secenek']), int(m['dogru']), int(m['kalan_parca'])) + [math.log1p(played_before(m['level_id']))])
        y.append(math.log(t)); lv.append(m['level_id'])
    c = ols(X, y)
    res = [yy - sum(ci * xi for ci, xi in zip(c, x)) for x, yy in zip(X, y)]
    by = {}
    for l, e in zip(lv, res): by.setdefault(l, []).append(e)
    means = [sum(v) / len(v) for v in by.values()]
    tau = math.sqrt(sum(m * m for m in means) / len(means))
    within = [e - sum(by[l]) / len(by[l]) for l, e in zip(lv, res)]
    return c, tau, math.sqrt(sum(e * e for e in within) / len(within))


# ---------------------------------------------------------------- the bot
def play(level, feat, paths, trap, think, dig, seed=1):
    a, b, c3 = trap; c, tau, sigma = think
    timer = float(feat[level]['sure_limiti'])
    skill = math.log1p(played_before(level))
    rng = random.Random(seed)
    runs = list(paths.get(level, {}).values())
    ws = [path_weight(st, dig) for st in runs]
    tot = sum(ws) or 1.0
    wins = 0.0; left_sum = 0.0; used_sum = 0.0
    for k in range(SIMS):
        i = k % len(runs); steps = runs[i]; wt = ws[i] / tot * len(runs)
        lvl = rng.gauss(0, tau); t = 0.0; lost = False
        for j, s in enumerate(steps):
            n, r, left = int(s['secenek']), int(s['dogru']), int(s['kalan_parca'])
            if n == 0: break
            if j > 0:
                mu = sum(ci * xi for ci, xi in zip(c, think_x(n, r, left) + [skill]))
                t += math.exp(mu + lvl + rng.gauss(0, sigma)) / 1000
            if r < n and rng.random() < sig(a + b * logit((n - r) / n) + c3 * skill): lost = True; break
            if t > timer: lost = True; break
        used_sum += wt * min(t, timer) / timer
        if not lost: wins += wt; left_sum += wt * (timer - t)
    return wins / SIMS, (left_sum / wins if wins > 0 else 0.0), used_sum / SIMS


def fit_all(train, first, score, moves, feat, paths, mis):
    dig = dig_rate(moves, train)
    trap = fit_logistic2(trap_samples(moves, train))
    think = fit_think(moves, train)
    sim = {l: play(l, feat, paths, trap, think, dig) for l in train}
    pairs = [l for l in train if l in score]
    sc = ols([[1, 1 - sim[l][0], sim[l][2], mis.get(l, 0)] for l in pairs], [score[l] for l in pairs], l2=0.3)
    return dig, trap, think, sc


def predict_one(l, model, feat, paths, mis):
    dig, trap, think, sc = model
    w, left, used = play(l, feat, paths, trap, think, dig)
    return w, left, clamp_score(sc[0] + sc[1] * (1 - w) + sc[2] * used + sc[3] * mis.get(l, 0))


def baseline(xf, train, test, first, score, feat):
    from zeydbot_model import fit_logistic
    lo = fit_logistic([(xf(l), 1 if first[l]['result'] == 'win' else 0) for l in train], l2=0.01)
    won = [l for l in train if first[l]['result'] == 'win']
    ls = ols([[1, xf(l)] for l in won], [float(first[l]['left_s']) / float(feat[l]['sure_limiti']) for l in won], l2=0.01)
    rated = [l for l in train if l in score]
    ss = ols([[1, xf(l)] for l in rated], [score[l] for l in rated], l2=0.01)
    x = xf(test)
    return sig(lo[0] + lo[1] * x), (ls[0] + ls[1] * x) * float(feat[test]['sure_limiti']), clamp_score(ss[0] + ss[1] * x)


def metrics(res, levels, first, score, keep=lambda l: True):
    L = [l for l in levels if keep(l)]
    rated = [l for l in L if l in score]
    won = [l for l in L if first[l]['result'] == 'win']
    sp = spearman([res[l][2] for l in rated], [score[l] for l in rated])
    acc = sum((res[l][0] >= .5) == (first[l]['result'] == 'win') for l in L)
    brier = sum((res[l][0] - (first[l]['result'] == 'win')) ** 2 for l in L) / len(L)
    w25 = sum(abs(res[l][1] - float(first[l]['left_s'])) <= .25 * float(first[l]['left_s']) for l in won)
    return sp, f'{acc}/{len(L)}', brier, f'{w25}/{len(won)}', len(rated)


def loo():
    first, score, from2, moves, feat, paths, mis = load()
    levels = sorted(l for l in first if l in feat and l in paths)
    res = {'bot': {}, 'rastgele': {}, 'pay': {}}
    for t in levels:
        train = [l for l in levels if l != t]
        model = fit_all(train, first, score, moves, feat, paths, mis)
        res['bot'][t] = predict_one(t, model, feat, paths, mis)
        res['rastgele'][t] = baseline(lambda l: num(feat[l]['rastgele_kazanma']), train, t, first, score, feat)
        res['pay'][t] = baseline(lambda l: slack_x(feat, l), train, t, first, score, feat)
        print(t, 'dig %.3f' % model[0], 'bot', ['%.2f' % v for v in res['bot'][t]], flush=True)
    out = [f'# ZeydBot v2 — leave-one-out ({len(levels)} bölüm)', '',
           '| Bölüm | Gerçek: ilk deneme / kalan s / puan | ZeydBot v2 | Rastgele | Bomba payı |', '|---|---|---|---|---|']
    cell = lambda v: f"%{v[0] * 100:.0f} / {v[1]:.0f} / {v[2]:.1f}"
    for t in levels:
        f = first[t]
        tag = (' (kör değil?)' if t in FLAG else '') + (' (puan 2. denemeden)' if t in from2 else '')
        out.append(f"| {t}{tag} | {f['result']} / {f['left_s'] if f['result'] == 'win' else '—'} / {score.get(t, '—')} | {cell(res['bot'][t])} | {cell(res['rastgele'][t])} | {cell(res['pay'][t])} |")
    for title, keep in (('Tümü', lambda l: True), ('Z01/Z07 ve puanı 2. denemeden olanlar hariç', lambda l: l not in FLAG and l not in from2)):
        ms = {k: metrics(res[k], levels, first, score, keep) for k in res}
        out += ['', f'**{title}**', '', '| Ölçüt | ZeydBot v2 | Rastgele | Bomba payı |', '|---|---|---|---|',
                f"| Puan sırası (Spearman, {ms['bot'][4]} bölüm) | " + ' | '.join('%.2f' % ms[k][0] for k in res) + ' |',
                '| Kazandı/kaybetti (eşik %50) | ' + ' | '.join(ms[k][1] for k in res) + ' |',
                '| Brier (düşük iyi) | ' + ' | '.join('%.3f' % ms[k][2] for k in res) + ' |',
                '| Kalan s ±%25 | ' + ' | '.join(ms[k][3] for k in res) + ' |']
    open(os.path.join(D, 'loo_v2.md'), 'w', encoding='utf-8').write('\n'.join(out) + '\n')
    print('\n'.join(out[-15:]))


def predict(targets):
    first, score, from2, moves, feat, paths, mis = load()
    levels = sorted(l for l in first if l in feat and l in paths)
    model = fit_all(levels, first, score, moves, feat, paths, mis)
    dig, trap, think, sc = model
    print(f"parametreler (eğitim {levels[0]}–{levels[-1]}, {len(levels)} bölüm): bombaya yönelme {dig:.3f}; tuzak a={trap[0]:.3f} b={trap[1]:.3f} öğrenme={trap[2]:.3f}; "
          f"düşünme c={['%.3f' % v for v in think[0]]} tau={think[1]:.3f} sigma={think[2]:.3f}; puan s={['%.3f' % v for v in sc]}")
    print('| Bölüm | ZeydBot v2: ilk deneme kazanma / kalan s / puan | Taban rastgele | Taban bomba payı |')
    print('|---|---|---|---|')
    cell = lambda v: f"%{v[0] * 100:.0f} / {v[1]:.0f} / {v[2]:.1f}"
    for t in targets:
        bot = predict_one(t, model, feat, paths, mis)
        rnd = baseline(lambda l: num(feat[l]['rastgele_kazanma']), levels, t, first, score, feat)
        slk = baseline(lambda l: slack_x(feat, l), levels, t, first, score, feat)
        print(f"| {t} | {cell(bot)} | {cell(rnd)} | {cell(slk)} |")


if __name__ == '__main__':
    if sys.argv[1] == 'loo': loo()
    else: predict(sys.argv[2:])
