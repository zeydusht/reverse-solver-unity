# ZeydBot v1, the person half (overnight 2026-10-20). Plain numpy-free Python, every number from the CSVs in
# docs/analiz/zeydbot (ZeydAnalysis + ZeydBot.Paths). Three small parts, each explainable:
#   1. trap choice: at a move where some options are traps, P(Zeyd picks one) = sigmoid(a + b * logit(trap share))
#   2. thinking time: log(think_ms) = c0 + c1 log(options) + c2 [only one right] + c3 log(pieces left) + level effect + noise
#   3. 1-10 score: linear on the bot's loss chance and the share of the timer it uses
# The bot plays the right-only paths of ZeydBot.Paths: at each step it may fall into a trap (loss) and spends
# thinking time; out of time = loss. Only first attempts of Z01-Z20 are used; Z21-Z30 results are never read.
#
#   python Tools/zeydbot_model.py loo                 -> leave-one-out on Z01-Z20 vs two baselines (loo.md)
#   python Tools/zeydbot_model.py predict Z21 .. Z30  -> fitted on all of Z01-Z20, predictions (stdout table)
import csv, math, random, sys, os

D = os.path.join(os.path.dirname(__file__), '..', 'docs', 'analiz', 'zeydbot')
SIMS = 400           # bot plays per path
MOVE_FIRST_FREE = True   # the clock starts on the first move (ClockOnFirstMove): the first think is free


def rows(name):
    with open(os.path.join(D, name), encoding='utf-8') as f:
        return list(csv.DictReader(f))


def num(v, d=None):
    try: return float(v)
    except (TypeError, ValueError): return d


def sig(x): return 1 / (1 + math.exp(-max(-30, min(30, x))))


def logit(q):
    q = min(max(q, 0.02), 0.98)
    return math.log(q / (1 - q))


# ---------------------------------------------------------------- data
def load():
    att = [r for r in rows('denemeler.csv') if r['level_id'] <= 'Z20']
    first = {r['level_id']: r for r in att if r['attempt'] == '1'}
    score = {}
    for r in att:                                    # the 1-10 answer, whichever attempt it was given on
        if r['zorluk_puani']: score[r['level_id']] = int(r['zorluk_puani'])
    moves = [m for m in rows('hamleler.csv') if m['level_id'] <= 'Z20' and m['attempt'] == '1']
    feat = {r['level_id']: r for r in rows('bolum_ozellik.csv')}
    paths = {}
    for r in rows('yollar.csv'):
        paths.setdefault(r['level_id'], {}).setdefault(r['run'], []).append(r)
    return first, score, moves, feat, paths


# ---------------------------------------------------------------- 1. trap choice
def trap_samples(moves, levels):
    """Exit moves until (and with) the first trap pick, where at least one option is a trap."""
    out, dead = [], set()
    for m in moves:
        if m['kind'] != 'exit' or not m['secenek'] or m['level_id'] not in levels or m['level_id'] in dead: continue
        n, r = int(m['secenek']), int(m['dogru'])
        if r == 0: dead.add(m['level_id']); continue
        if r < n: out.append((logit((n - r) / n), 1 if m['secilen'] == 'tuzak' else 0))
        if m['secilen'] == 'tuzak': dead.add(m['level_id'])
    return out


def fit_logistic(xy, l2=0.5):
    a, b = -1.0, 1.0
    for _ in range(100):
        ga = gb = 0; haa = hab = hbb = l2
        ga -= l2 * a * 0; gb -= l2 * (b - 1)        # ridge pulls b toward 1 (random picker) and nothing on a
        for x, y in xy:
            p = sig(a + b * x); w = p * (1 - p)
            ga += y - p; gb += (y - p) * x
            haa += w; hab += w * x; hbb += w * x * x
        det = haa * hbb - hab * hab
        a += (hbb * ga - hab * gb) / det; b += (haa * gb - hab * ga) / det
    return a, b


# ---------------------------------------------------------------- 2. thinking time
def think_x(n, r, left):
    return [1.0, math.log(max(n, 1)), 1.0 if r == 1 else 0.0, math.log(max(left, 1))]


def ols(X, y, l2=1e-6):
    k = len(X[0])
    A = [[sum(x[i] * x[j] for x in X) + (l2 if i == j else 0) for j in range(k)] for i in range(k)]
    v = [sum(x[i] * yy for x, yy in zip(X, y)) for i in range(k)]
    for i in range(k):                                # Gauss-Jordan
        p = A[i][i]
        for j in range(k): A[i][j] /= p
        v[i] /= p
        for r in range(k):
            if r != i:
                f = A[r][i]
                for j in range(k): A[r][j] -= f * A[i][j]
                v[r] -= f * v[i]
    return v


def fit_think(moves, levels):
    X, y, lv = [], [], []
    for m in moves:
        if m['kind'] != 'exit' or not m['secenek'] or m['level_id'] not in levels or m['move_no'] == '1': continue
        t = int(m['think_ms'])
        if t <= 0: continue
        X.append(think_x(int(m['secenek']), int(m['dogru']), int(m['kalan_parca']))); y.append(math.log(t)); lv.append(m['level_id'])
    c = ols(X, y)
    res = [yy - sum(ci * xi for ci, xi in zip(c, x)) for x, yy in zip(X, y)]
    by = {}
    for l, e in zip(lv, res): by.setdefault(l, []).append(e)
    means = [sum(v) / len(v) for v in by.values()]
    tau = math.sqrt(max(sum(m * m for m in means) / len(means) - 0.0, 0))
    within = [e - sum(by[l]) / len(by[l]) for l, e in zip(lv, res)]
    sigma = math.sqrt(sum(e * e for e in within) / len(within))
    return c, tau, sigma


# ---------------------------------------------------------------- the bot
def play(level, feat, paths, trap, think, seed=1):
    a, b = trap; c, tau, sigma = think
    timer = float(feat[level]['sure_limiti'])
    rng = random.Random(seed)
    wins = 0; lefts = []; used = []
    runs = list(paths.get(level, {}).values())
    for k in range(SIMS):
        steps = runs[k % len(runs)]
        lvl = rng.gauss(0, tau)
        t = 0.0; lost = False
        for i, s in enumerate(steps):
            n, r, left = int(s['secenek']), int(s['dogru']), int(s['kalan_parca'])
            if n == 0: break
            if i > 0 or not MOVE_FIRST_FREE:
                mu = sum(ci * xi for ci, xi in zip(c, think_x(n, r, left)))
                t += math.exp(mu + lvl + rng.gauss(0, sigma)) / 1000
            if r < n and rng.random() < sig(a + b * logit((n - r) / n)): lost = True; break
            if t > timer: lost = True; break
        used.append(min(t, timer) / timer)
        if not lost: wins += 1; lefts.append(timer - t)
    return wins / SIMS, (sum(lefts) / len(lefts) if lefts else 0.0), sum(used) / len(used)


def fit_score(pairs):
    """pairs: (p_loss, used_share, score) -> score = s0 + s1 p_loss + s2 used_share"""
    return ols([[1, p, u] for p, u, _ in pairs], [s for _, _, s in pairs], l2=0.3)


def clamp_score(v): return max(1.0, min(10.0, v))


# ---------------------------------------------------------------- evaluation helpers
def spearman(a, b):
    def rank(v):
        o = sorted(range(len(v)), key=lambda i: v[i]); r = [0] * len(v); i = 0
        while i < len(o):
            j = i
            while j + 1 < len(o) and v[o[j + 1]] == v[o[i]]: j += 1
            for k in range(i, j + 1): r[o[k]] = (i + j) / 2
            i = j + 1
        return r
    ra, rb = rank(a), rank(b); n = len(a)
    ma, mb = sum(ra) / n, sum(rb) / n
    cov = sum((x - ma) * (y - mb) for x, y in zip(ra, rb))
    return cov / math.sqrt(sum((x - ma) ** 2 for x in ra) * sum((y - mb) ** 2 for y in rb))


def fit_all(train, first, score, moves, feat, paths):
    trap = fit_logistic(trap_samples(moves, train))
    think = fit_think(moves, train)
    sim = {l: play(l, feat, paths, trap, think) for l in train}
    sc = fit_score([(1 - sim[l][0], sim[l][2], score[l]) for l in train if l in score])
    return trap, think, sc, sim


def slack_x(feat, l):
    v = num(feat[l]['baslangic_en_dar_pay'])
    return 10.0 if v is None else v                 # no bomb = plenty of slack


def baseline(xname, xf, train, test, first, score, feat):
    """One design number -> win (logistic), left share (linear), score (linear), fitted on train only."""
    xs = [(xf(l), 1 if first[l]['result'] == 'win' else 0) for l in train]
    lo = fit_logistic([(x, y) for x, y in xs], l2=0.01)
    won = [l for l in train if first[l]['result'] == 'win']
    ls = ols([[1, xf(l)] for l in won], [float(first[l]['left_s']) / float(feat[l]['sure_limiti']) for l in won], l2=0.01)
    ss = ols([[1, xf(l)] for l in train if l in score], [score[l] for l in train if l in score], l2=0.01)
    x = xf(test)
    return sig(lo[0] + lo[1] * x), (ls[0] + ls[1] * x) * float(feat[test]['sure_limiti']), clamp_score(ss[0] + ss[1] * x)


def loo():
    first, score, moves, feat, paths = load()
    levels = sorted(first)
    res = {}
    for t in levels:
        train = [l for l in levels if l != t]
        trap, think, sc, _ = fit_all(train, first, score, moves, feat, paths)
        w, left, used = play(t, feat, paths, trap, think)
        bot = (w, left, clamp_score(sc[0] + sc[1] * (1 - w) + sc[2] * used))
        rnd = baseline('random', lambda l: num(feat[l]['rastgele_kazanma']), train, t, first, score, feat)
        slk = baseline('slack', lambda l: slack_x(feat, l), train, t, first, score, feat)
        res[t] = {'bot': bot, 'rastgele': rnd, 'pay': slk}
        print(t, 'bot', ['%.2f' % v for v in bot], 'rastgele', ['%.2f' % v for v in rnd], 'pay', ['%.2f' % v for v in slk], flush=True)
    out = ['| Bölüm | Gerçek: ilk deneme / kalan s / puan | ZeydBot | Taban: rastgele oyuncu | Taban: bomba payı |', '|---|---|---|---|---|']
    for t in levels:
        f = first[t]
        real = f"{'kazandı' if f['result'] == 'win' else f['result']} / {f['left_s'] if f['result'] == 'win' else '—'} / {score.get(t, '—')}"
        cell = lambda v: f"%{v[0] * 100:.0f} / {v[1]:.0f} / {v[2]:.1f}"
        out.append(f"| {t}{' (kör değil)' if t in ('Z01', 'Z07') else ''} | {real} | {cell(res[t]['bot'])} | {cell(res[t]['rastgele'])} | {cell(res[t]['pay'])} |")
    out.append('')
    out.append('| Ölçüt | ZeydBot | Rastgele oyuncu | Bomba payı |')
    out.append('|---|---|---|---|')
    rated = [t for t in levels if t in score]
    won = [t for t in levels if first[t]['result'] == 'win']
    def m(name):
        sp = spearman([res[t][name][2] for t in rated], [score[t] for t in rated])
        acc = sum((res[t][name][0] >= .5) == (first[t]['result'] == 'win') for t in levels) / len(levels)
        brier = sum((res[t][name][0] - (first[t]['result'] == 'win')) ** 2 for t in levels) / len(levels)
        errs = [abs(res[t][name][1] - float(first[t]['left_s'])) / float(first[t]['left_s']) for t in won]
        within = sum(e <= .25 for e in errs) / len(errs)
        mae = sum(errs) / len(errs)
        return sp, acc, brier, within, mae
    ms = {n: m(n) for n in ('bot', 'rastgele', 'pay')}
    labels = [('Puan sırası (Spearman, %d bölüm)' % len(rated), 0, '%.2f'), ('Kazandı / kaybetti doğruluğu (eşik %%50, %d)' % len(levels), 1, '%%%.0f'),
              ('Brier (düşük iyi)', 2, '%.3f'), ('Kalan s ±%%25 içinde (%d kazanma)' % len(won), 3, '%%%.0f'), ('Kalan s ortalama hata', 4, '%%%.0f')]
    for lab, i, fmt in labels:
        f2 = lambda v: fmt % (v * 100) if '%%' in fmt else fmt % v
        out.append(f"| {lab} | {f2(ms['bot'][i])} | {f2(ms['rastgele'][i])} | {f2(ms['pay'][i])} |")
    open(os.path.join(D, 'loo.md'), 'w', encoding='utf-8').write('\n'.join(out) + '\n')
    print('\n'.join(out[-7:]))


def predict(targets):
    first, score, moves, feat, paths = load()
    levels = sorted(first)
    trap, think, sc, _ = fit_all(levels, first, score, moves, feat, paths)
    print(f"parametreler: tuzak a={trap[0]:.3f} b={trap[1]:.3f}; düşünme c={['%.3f' % v for v in think[0]]} tau={think[1]:.3f} sigma={think[2]:.3f}; puan s={['%.3f' % v for v in sc]}")
    print('| Bölüm | Tür | ZeydBot: ilk deneme kazanma / kalan s / puan | Taban rastgele oyuncu | Taban bomba payı |')
    print('|---|---|---|---|---|')
    for t in targets:
        w, left, used = play(t, feat, paths, trap, think)
        rnd = baseline('random', lambda l: num(feat[l]['rastgele_kazanma']), levels, t, first, score, feat)
        slk = baseline('slack', lambda l: slack_x(feat, l), levels, t, first, score, feat)
        cell = lambda v: f"%{v[0] * 100:.0f} / {v[1]:.0f} / {v[2]:.1f}"
        print(f"| {t} | {feat[t]['tur']} | {cell((w, left, clamp_score(sc[0] + sc[1] * (1 - w) + sc[2] * used)))} | {cell(rnd)} | {cell(slk)} |")


if __name__ == '__main__':
    if sys.argv[1] == 'loo': loo()
    else: predict(sys.argv[2:])
