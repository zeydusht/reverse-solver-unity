"""PM_98 §4: the UI sounds, loud enough to hear over the music (Zeyd: "I can't tell if I pressed it").
The game plays the music at 0.35 of its -18 LUFS (about -27 LUFS heard); the button tick played at -32 (PM_95's -25.5
take at 0.45). Every UI sound here is levelled to its target as the game plays it (at volume 1), measured as the
loudest 400 ms (momentary loudness, ITU-R BS.1770, pyloudnorm), the peak kept under -1 dBFS by a gentle limiter:
    tick (every button)        -14 LUFS   PM_95's button tick candidates (tik-a/b/c, Zeyd's b the default)
    toggle (a switch)          -14 LUFS   Kenney UI Audio "switch16", "switch18" (real switches)
    open (a panel opens)       -17 LUFS   artisticdude "Swishes Sound Pack" swish-4 / swish-5 (soft swishes)
    close (a panel closes)     -17 LUFS   the same swishes played backwards
    dull (a passive button)    -15 LUFS   Kenney Impact Sounds "impactWood_light" 000 / 002 (a dull wooden knock)
All CC0 (docs/art/pm98/LISANS.md). Out: Assets/_Game/Presentation/Resources/Art/audio/pm98/ui-*.wav (+ .meta: not
normalised, so the level here is the level heard).
    python -I Tools/make_ui98.py <Kenney packs dir (ui-audio, impact-sounds)> <swishes dir>"""
import os, sys, uuid
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln

SR = 44100
KENNEY, SWISH = sys.argv[1], sys.argv[2]
PM95 = 'Assets/_Game/Presentation/Resources/Art/audio/pm95'
OUT = 'Assets/_Game/Presentation/Resources/Art/audio/pm98'
CEIL = 10 ** (-1 / 20)
META = open(os.path.join(PM95, 'tik-b1.wav.meta'), encoding='utf8').read()


def read(p):
    x, sr = sf.read(p, always_2d=True)
    x = x.mean(1)
    if sr != SR: x = librosa.resample(x, orig_sr=sr, target_sr=SR)
    a = np.abs(x); on = int(np.argmax(a > a.max() * .02))
    return x[max(0, on - SR // 2000):]


def loud(x):
    """The loudest 400 ms, the clip padded with silence."""
    m = pyln.Meter(SR, block_size=.4); w = int(.4 * SR)
    y = np.concatenate([np.zeros(w), x, np.zeros(w)]); best = -99.0
    for i in range(0, len(y) - w, int(.02 * SR)):
        seg = y[i:i + w]
        if np.abs(seg).max() < 1e-6: continue
        best = max(best, m.integrated_loudness(np.stack([seg, seg], 1).repeat(2, 0)))
    return best


def limit(x):
    w = int(.002 * SR); a = np.abs(x)
    env = np.array([a[max(0, i - w):i + w].max() for i in range(0, len(a), w)])
    g = np.minimum(1, CEIL / np.maximum(env, 1e-9))
    for i in range(1, len(g)): g[i] = min(g[i], g[i - 1] * .85 + .15)
    return x * np.interp(np.arange(len(a)), np.arange(len(g)) * w, g)


def level(x, target):
    for _ in range(5): x = limit(x * 10 ** ((target - loud(x)) / 20))
    return x


def write(name, x):
    path = os.path.join(OUT, name)
    sf.write(path, np.stack([x, x], 1), SR, subtype='PCM_16')
    m = path + '.meta'; guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    if guid is None: guid = uuid.uuid4().hex
    lines = META.splitlines()
    lines = [f'guid: {guid}' if l.startswith('guid:') else l for l in lines]
    open(m, 'w', newline='\n').write('\n'.join(lines) + '\n')
    return loud(x), 20 * np.log10(np.abs(x).max())


os.makedirs(OUT, exist_ok=True)
jobs = []
for letter in 'abc':
    n = 1
    while os.path.exists(os.path.join(PM95, f'tik-{letter}{n}.wav')):
        jobs.append((f'ui-tick-{letter}{n}.wav', read(os.path.join(PM95, f'tik-{letter}{n}.wav')), -14)); n += 1
for i, f in enumerate(['switch16.ogg', 'switch18.ogg']):
    jobs.append((f'ui-toggle{i + 1}.wav', read(os.path.join(KENNEY, 'ui-audio', 'Audio', f)), -14))
for i, f in enumerate(['swish-4.wav', 'swish-5.wav']):
    x = read(os.path.join(SWISH, f))
    jobs.append((f'ui-open{i + 1}.wav', x, -17))
    jobs.append((f'ui-close{i + 1}.wav', x[::-1].copy(), -17))
for i, f in enumerate(['impactWood_light_000.ogg', 'impactWood_light_002.ogg']):
    jobs.append((f'ui-dull{i + 1}.wav', read(os.path.join(KENNEY, 'impact-sounds', 'Audio', f)), -15))
for name, x, target in jobs:
    fade = min(len(x) // 4, int(.01 * SR)); x = x.copy(); x[-fade:] *= np.linspace(1, 0, fade)
    l, p = write(name, level(x, target))
    print(f'{name:18s} {len(x) / SR:.2f} s  {l:6.1f} LUFS (hedef {target})  tepe {p:5.1f} dBFS')
