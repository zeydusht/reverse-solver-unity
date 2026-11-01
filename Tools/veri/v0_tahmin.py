"""Reverse Solver - v0 on-kayit tahmini (PM, 31 Ekim 2026). docs/VERI_SISTEMI.md bolum 6.

Ne yapar:
  1. Egitim: Zeyd'in 40 bolume verdigi 1-10 puan (deneme1 + Z oturumlari, fit.py'deki tablo).
     Ayarlanan 9 bolum icin ESKI surumun ozellikleri kullanilir (puan eski surume verildi).
  2. Model A - ozellik regresyonu (ridge, lambda=1): puan ~ bomba + fitil_payi + duvar_seridi + zincir + civi
     Model B - FunSim kalibrasyonu: puan ~ logit(ilk deneme kazanma) + logit(rastgele kazanma)
     v0 = (A + B) / 2   (FunSim degeri olmayan bolumde yalniz A)
  3. Puan -> nufus ilk deneme kazanma: VARSAYIM A1  win1 = 0.97 - 0.075 * (puan - 1)
     Kayip nedeni: VARSAYIM A2  bombali bolumde kayiplarin %75'i bomba, bombasizda %100 sure.
  4. Ciktilar: docs/veri/tahmin_v0.csv + LOO karnesi (ekrana).

Calistir (repo kokunden):  python Tools/veri/v0_tahmin.py
"""
import csv, json, math, os, re, sys
import numpy as np
sys.path.insert(0, os.path.dirname(__file__))
from level_features import features

LV = 'Assets/_Game/Levels/'
RATING = {'S47': 1, 'S20': 2, 'S51': 1, 'S18': 2, 'S17': 2, 'S10': 1, 'S50': 1, 'S52': 1, 'S08': 1, 'S06': 2, 'S44': 1,
          'S45': 2, 'S46': 1, 'S13': 4, 'S28': 9, 'S24': 7, 'S19': 4, 'S27': 9, 'S14': 4, 'S35': 10, 'S37': 10, 'S31': 9,
          'S32': 8, 'S36': 8, 'S29': 9, 'S38': 10, 'S40': 8, 'S33': 10, 'S21': 3, 'S34': 5, 'S22': 5, 'S39': 6, 'Z04': 3,
          'Z21': 3, 'Z23': 5, 'Z24': 3, 'Z25': 4, 'S41': 1, 'S42': 1, 'S43': 1}
COLS = ['bombs', 'slack', 'sealed', 'chains', 'nails']
L2 = 1.0


def win1(r): return max(0.05, min(0.99, 0.97 - 0.075 * (r - 1)))           # A1
def lg(p): p = min(max(p, .01), .99); return math.log(p / (1 - p))
def num(x, d=None): return float(x) if x not in ('', None) else d


def x_row(f):
    v = {'bombs': f['bombs'], 'slack': min(f['fuse_slack_min'] if f['fuse_slack_min'] is not None else 12, 12),
         'sealed': f['sealed_lanes'], 'chains': f['chains'], 'nails': f['nails']}
    return [1.0] + [float(v[c]) for c in COLS]


def ridge(A, y):
    I = np.eye(A.shape[1]); I[0, 0] = 0
    return np.linalg.solve(A.T @ A + L2 * I, A.T @ y)


def spearman(a, b):
    return float(np.corrcoef(np.argsort(np.argsort(a)), np.argsort(np.argsort(b)))[0, 1])


def main():
    v4 = {l['id']: features(l) for l in json.load(open(LV + 'redesign.json', encoding='utf-8'))['levels']}
    v3 = {l['id']: features(l, 'redesign-v3') for l in json.load(open(LV + 'redesign_v3.json', encoding='utf-8'))['levels']}
    sec = json.load(open('Logs/setv4/secim.json', encoding='utf-8'))
    kal = {r['id']: r for r in csv.DictReader(open('Logs/setv4/kal.csv'))}
    olc = {}
    for n in ('olcum', 'olcum2', 'olcum3'):
        olc.update({r['id']: r for r in csv.DictReader(open(f'Logs/setv4/{n}.csv'))})

    train = [(i, v3[i] if i in sec else (v4.get(i) or v3[i]), y) for i, y in RATING.items()]
    A = np.array([x_row(f) for _, f, _ in train]); y = np.array([t[2] for t in train], float)
    b = ridge(A, y)
    fs_rows = [(i, y) for i, y in RATING.items() if i in kal]
    Ak = np.array([[1, lg(float(kal[i]['first'])), lg(float(kal[i]['mc']))] for i, _ in fs_rows])
    bk = np.linalg.lstsq(Ak, np.array([y for _, y in fs_rows], float), rcond=None)[0]

    # LOO karnesi (secilen model icin; ozellik secimi ayni veride yapildigi icin hafif iyimser)
    P, Y = [], []
    for k, (i, f, yy) in enumerate(train):
        m = [j for j in range(len(train)) if j != k]
        bb = ridge(A[m], y[m]); pr = float(np.clip(np.array(x_row(f)) @ bb, 1, 10))
        if i in kal:
            mk = [j for j, (ii, _) in enumerate(fs_rows) if ii != i]
            bkk = np.linalg.lstsq(Ak[mk], np.array([fs_rows[j][1] for j in mk], float), rcond=None)[0]
            pr = (pr + float(np.clip(bkk @ [1, lg(float(kal[i]['first'])), lg(float(kal[i]['mc']))], 1, 10))) / 2
        P.append(pr); Y.append(yy)
    P, Y = np.array(P), np.array(Y)
    print(f'LOO n={len(Y)}  MAE={abs(P - Y).mean():.2f}  (sabit ortalama {abs(Y - Y.mean()).mean():.2f})  Spearman={spearman(P, Y):.2f}')
    print('regresyon katsayilari:', {c: round(v, 3) for c, v in zip(['sabit'] + COLS, b)})

    target = {}
    for line in open('docs/SET40_V4.md', encoding='utf-8'):
        m = re.match(r'\| (\d+) \|', line)
        if m:
            t = [c.strip() for c in line.split('|')][6]
            lo, hi = (t.split('–') + [t])[:2]; target[int(m.group(1))] = (float(lo), float(hi))

    out = []
    for i, f in sorted(v4.items(), key=lambda kv: kv[1]['slot']):
        preg = float(np.clip(np.array(x_row(f)) @ b, 1, 10))
        r = olc.get(sec[i]) if i in sec else kal.get(i)
        pfs = float(np.clip(bk @ [1, lg(float(r['first'])), lg(float(r['mc']))], 1, 10)) if r else None
        pr = (preg + pfs) / 2 if pfs is not None else preg
        w = win1(pr); bshare = 0.75 if f['bombs'] else 0.0
        lo, hi = target[f['slot']]
        out.append(dict(slot=f['slot'], level_id=i, level_version=f['level_version'],
                        pred_rating=round(pr, 2), pred_rating_lo=round(max(1, pr - 2.2), 2), pred_rating_hi=round(min(10, pr + 2.2), 2),
                        pred_rating_reg=round(preg, 2), pred_rating_funsim=None if pfs is None else round(pfs, 2),
                        funsim_first=None if not r else float(r['first']), funsim_mc=None if not r else float(r['mc']),
                        p_win1=round(w, 3), p_win1_lo=round(win1(min(10, pr + 2.2)), 3), p_win1_hi=round(win1(max(1, pr - 2.2)), 3),
                        p_fail_bomb=round((1 - w) * bshare, 3), p_fail_time=round((1 - w) * (1 - bshare), 3),
                        target_rating_lo=lo, target_rating_hi=hi,
                        target_win1_lo=round(win1(hi), 3), target_win1_hi=round(win1(lo), 3)))
    os.makedirs('docs/veri', exist_ok=True)
    with open('docs/veri/tahmin_v0.csv', 'w', newline='', encoding='utf-8') as fh:
        wr = csv.DictWriter(fh, fieldnames=list(out[0].keys())); wr.writeheader(); wr.writerows(out)
    print('yazildi: docs/veri/tahmin_v0.csv')


if __name__ == '__main__':
    main()
