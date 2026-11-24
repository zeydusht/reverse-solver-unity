"""PM_107 §C2-C4: the chosen sounds mastered to their place in the mix, into Resources/Art/audio/pm107/<same name>.wav
(SesSecim.Load plays these over the originals; &miks=0 the originals).

Each file the game plays by default (docs/art/pm104/onizleme/bugun.json, written by Tools/ses_eslesme.js from the
game's own choices) gets, by its group:
  - its level: gain so its loudest 400 ms meets the group's target (LUFS); the peak held at -1.5 dBFS at most (a soft
    knee above -4.5 dBFS, never a hard clip);
  - smooth (the slides, the exits, the chain, the whooshes): a high shelf -4 dB from 7 kHz (the scratch in the highs
    gone, the body kept) and a 4 ms raised-cosine attack;
  - every file: a 1 ms fade in and a 5 ms fade out (no click at the start or the cut).
Writes docs/art/pm107/miks/hedefler.json: each file before and after (LUFS, peak).
    python -I Tools/make_miks107.py
"""
import json, os, re, glob
import numpy as np, soundfile as sf, pyloudnorm as pyln
from scipy.signal import lfilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
AUDIO = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources', 'Art', 'audio')
OUT = os.path.join(AUDIO, 'pm107')
BUGUN = os.path.join(ROOT, 'docs', 'art', 'pm104', 'onizleme', 'bugun.json')
REPORT = os.path.join(ROOT, 'docs', 'art', 'pm107', 'miks', 'hedefler.json')

# (pattern on the file name, target LUFS, smooth) — first match wins; None: the level kept
GROUPS = [
    (r'^cikis-', -17.0, True),                  # the slide out (Zeyd's u)
    (r'^carpma-', -17.5, False),                # the knock
    (r'^ui-tok-', -20.0, False),                # every button
    (r'^ui-(open|close)', -23.0, True),         # the panels' swish, under the tok
    (r'^ui-(toggle|dull)', -21.0, False),
    (r'^bomba-', -17.5, False),
    (r'^zincir-', -22.5, True),                 # the chain's bed (its level then follows the chain's speed)
    (r'^zincirger-', -17.0, False),             # the taut ting (played .71: -20)
    (r'^kilit', -18.5, False),
    (r'^patlama-', -16.0, False),               # the bang: the loudest effect, the music ducks under it
    (r'^duvar-', -17.0, False),                 # the red wall's bonk: a knock, not louder than one
    (r'^booster-saat-tiktak', -21.5, False),    # the tick-tock: in front while the clock runs (was -29.6: drowned)
    (r'^booster-cekic-kirinti', -21.0, True),
    (r'^booster-', -18.0, False),
    (r'^gecis-koridor', -25.0, True),           # the corridor's air (also the dive's, Gecis100.Dive)
    (r'^gecis-dalis', -24.0, True),             # the dive's whoosh into the picture (was -33: "almost nothing")
    (r'^gecis-', None, True),                   # the light transition cues: kept under the music
]


def lufs400(y, sr):
    meter = pyln.Meter(sr, block_size=.4)
    n = int(.4 * sr)
    if len(y) < n: y = np.concatenate([y, np.zeros(n - len(y))])
    best = -70.0
    for a in range(0, len(y) - n + 1, int(.1 * sr)):
        try:
            v = meter.integrated_loudness(y[a:a + n])
        except ValueError:
            continue
        if np.isfinite(v): best = max(best, v)
    return best


def high_shelf(y, sr, f0=7000.0, gain_db=-4.0, s=.7):
    """RBJ high shelf."""
    A = 10 ** (gain_db / 40); w0 = 2 * np.pi * f0 / sr; cw, sw = np.cos(w0), np.sin(w0)
    alpha = sw / 2 * np.sqrt((A + 1 / A) * (1 / s - 1) + 2)
    b0 = A * ((A + 1) + (A - 1) * cw + 2 * np.sqrt(A) * alpha); b1 = -2 * A * ((A - 1) + (A + 1) * cw)
    b2 = A * ((A + 1) + (A - 1) * cw - 2 * np.sqrt(A) * alpha); a0 = (A + 1) - (A - 1) * cw + 2 * np.sqrt(A) * alpha
    a1 = 2 * ((A - 1) - (A + 1) * cw); a2 = (A + 1) - (A - 1) * cw - 2 * np.sqrt(A) * alpha
    return lfilter(np.array([b0, b1, b2]) / a0, np.array([1, a1 / a0, a2 / a0]), y, axis=0)


def fade(y, sr, a_ms, r_ms):
    y = y.copy(); n = len(y)
    na, nr = min(n, int(sr * a_ms / 1000)), min(n, int(sr * r_ms / 1000))
    if na > 1: y[:na] *= (0.5 - 0.5 * np.cos(np.linspace(0, np.pi, na)))[:, None]
    if nr > 1: y[-nr:] *= (0.5 + 0.5 * np.cos(np.linspace(0, np.pi, nr)))[:, None]
    return y


def soft_ceiling(y, ceil_db=-1.5, knee_db=3.0):
    """Gain so the peak sits at the ceiling at most; above (ceil - knee) a tanh knee instead of a clip."""
    c = 10 ** (ceil_db / 20); k = 10 ** ((ceil_db - knee_db) / 20)
    a = np.abs(y)
    over = a > k
    out = y.copy()
    out[over] = np.sign(y[over]) * (k + (c - k) * np.tanh((a[over] - k) / (c - k)))
    return out


def files_today():
    d = json.load(open(BUGUN, encoding='utf-8'))
    names = []
    for e in d['events']:
        for f in e['files']: names.append(os.path.splitext(f)[0])
    return sorted(set(names))


def source_of(name):
    hits = [f for f in glob.glob(os.path.join(AUDIO, '**', name + '.wav'), recursive=True) if os.sep + 'pm107' + os.sep not in f]
    # the newest folder wins (pm106 over pm99 ...), as the loaders prefer
    hits.sort(key=lambda f: int(re.search(r'pm(\d+)', f).group(1)) if re.search(r'pm(\d+)', f) else 0)
    return hits[-1] if hits else None


def main(extra=()):
    os.makedirs(OUT, exist_ok=True)
    report = {}
    for name in sorted(set(files_today()) | set(extra)):
        src = source_of(name)
        if src is None: continue
        group = next((g for g in GROUPS if re.search(g[0], name)), None)
        if group is None: continue
        target, smooth = group[1], group[2]
        y, sr = sf.read(src, always_2d=True)
        y = y.astype(float)
        before = (lufs400(y.mean(axis=1), sr), 20 * np.log10(np.max(np.abs(y)) + 1e-9))
        z = y
        if smooth:
            z = high_shelf(z, sr)
            z = fade(z, sr, 4, 5)
        else:
            z = fade(z, sr, 1, 5)
        cur = lufs400(z.mean(axis=1), sr)
        if target is not None: z = z * 10 ** ((target - cur) / 20)
        z = soft_ceiling(z)
        after = (lufs400(z.mean(axis=1), sr), 20 * np.log10(np.max(np.abs(z)) + 1e-9))
        sf.write(os.path.join(OUT, name + '.wav'), z if z.shape[1] > 1 else z[:, 0], sr, subtype='PCM_16')
        report[name] = dict(source=os.path.relpath(src, ROOT).replace('\\', '/'), target=target, smooth=smooth,
                            before=dict(lufs=round(before[0], 1), peak=round(before[1], 1)), after=dict(lufs=round(after[0], 1), peak=round(after[1], 1)))
        print(f"[miks107] {name:28s} {before[0]:6.1f} -> {after[0]:6.1f} LUFS  tepe {before[1]:5.1f} -> {after[1]:5.1f}" + ('  yumuşatıldı' if smooth else ''))
    os.makedirs(os.path.dirname(REPORT), exist_ok=True)
    json.dump(report, open(REPORT, 'w', encoding='utf-8'), indent=1, ensure_ascii=False)
    print(f"[miks107] {len(report)} dosya -> {os.path.relpath(OUT, ROOT)}")


if __name__ == '__main__':
    import sys
    main(sys.argv[1:])
