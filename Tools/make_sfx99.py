"""PM_99 §2-3: soft exit sounds (a slide, no boing) and the rubber knock made loud enough.
Exit candidates f-j (&sescikis=), all recorded CC0 (docs/art/pm99/LISANS.md), 3-4 takes each:
  f  soft swish            artisticdude "Swishes Sound Pack" swish-3, -4, -7, -8 (the low, gentle ones)
  g  cloth rub             Kenney RPG Audio cloth1-4
  h  short card slide      Kenney Casino Audio card-slide-4, -5, -6, -8
  i  card push             Kenney Casino Audio card-shove-1, -3, -4
  j  paper slide           Kenney Casino Audio cards-pack-take-out-1, -2, card-fan-1
Each take: from its first sound, at most 0.25 s, a 15 ms fade in (no hard attack) and a 40 ms fade out, levelled to
-17 LUFS (the loudest 400 ms, as played: the tick is -14). A softness score (crest factor, attack, brightness,
length) picks the default. For the listening page: every candidate (and today's c) as 10 exits in a row
(0.42 s apart, a different take each time, +-4 % pitch).
The knock (carpma a, Zeyd keeps it): its takes levelled to -13 LUFS (the tick's level +1 dB), with its own upper harmonics
added (gentle saturation) so a phone's small speaker plays it too.
Out: Resources/Art/audio/pm99/ (cikis-f1.., carpma-a1..; read before pm95) and docs/art/pm99/onizleme/sesler/ + sesler99.json.
    python -I Tools/make_sfx99.py <Kenney packs dir (casino-audio, rpg-audio)> <swishes dir>"""
import json, os, sys, uuid
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln
from scipy.signal import butter, sosfilt

SR = 44100
KEN, SWISH = sys.argv[1], sys.argv[2]
PM95 = 'Assets/_Game/Presentation/Resources/Art/audio/pm95'
OUT = 'Assets/_Game/Presentation/Resources/Art/audio/pm99'
PAGE = 'docs/art/pm99/onizleme'
CEIL = 10 ** (-1 / 20)
META = open(os.path.join(PM95, 'tik-b1.wav.meta'), encoding='utf8').read()
rng = np.random.default_rng(99)
casino = lambda f: os.path.join(KEN, 'casino-audio', 'Audio', f)
rpg = lambda f: os.path.join(KEN, 'rpg-audio', 'Audio', f)
sw = lambda f: os.path.join(SWISH, f)
CANDS = [
    ('f', 'Yumuşak swish', 'artisticdude, Swishes Sound Pack', 'https://opengameart.org/content/swishes-sound-pack', [sw(f'swish-{n}.wav') for n in (3, 4, 7, 8)]),
    ('g', 'Kumaş sürtünmesi', 'Kenney, RPG Audio', 'https://kenney.nl/assets/rpg-audio', [rpg(f'cloth{n}.ogg') for n in (1, 2, 3, 4)]),
    ('h', 'Kısa kart kayması', 'Kenney, Casino Audio', 'https://kenney.nl/assets/casino-audio', [casino(f'card-slide-{n}.ogg') for n in (4, 5, 6, 8)]),
    ('i', 'Kart itme', 'Kenney, Casino Audio', 'https://kenney.nl/assets/casino-audio', [casino(f'card-shove-{n}.ogg') for n in (1, 3, 4)]),
    ('j', 'Kağıt kayması', 'Kenney, Casino Audio', 'https://kenney.nl/assets/casino-audio', [casino('cards-pack-take-out-1.ogg'), casino('cards-pack-take-out-2.ogg'), casino('card-fan-1.ogg')]),
]


def read(p):
    x, sr = sf.read(p, always_2d=True); x = x.mean(1)
    if sr != SR: x = librosa.resample(x, orig_sr=sr, target_sr=SR)
    return x


def env(x):
    w = int(.002 * SR); return np.convolve(np.abs(x), np.ones(w) / w, 'same')


def shape(x, cap=.25):
    e = env(x); on = int(np.argmax(e > e.max() * .05))
    x = x[max(0, on - int(.005 * SR)):][:int(cap * SR)].copy()
    fi, fo = int(.015 * SR), min(len(x) // 3, int(.04 * SR))
    x[:fi] *= np.linspace(0, 1, fi) ** 2
    x[-fo:] *= np.linspace(1, 0, fo) ** 2
    return x


def loud(x):
    m = pyln.Meter(SR, block_size=.4); w = int(.4 * SR)
    y = np.concatenate([np.zeros(w), x, np.zeros(w)]); best = -99.0
    for i in range(0, len(y) - w, int(.02 * SR)):
        seg = y[i:i + w]
        if np.abs(seg).max() < 1e-6: continue
        best = max(best, m.integrated_loudness(np.stack([seg, seg], 1).repeat(2, 0)))
    return best


def limit(x):
    w = int(.002 * SR); a = np.abs(x)
    e = np.array([a[max(0, i - w):i + w].max() for i in range(0, len(a), w)])
    g = np.minimum(1, CEIL / np.maximum(e, 1e-9))
    for i in range(1, len(g)): g[i] = min(g[i], g[i - 1] * .85 + .15)
    return x * np.interp(np.arange(len(a)), np.arange(len(g)) * w, g)


def level(x, target):
    for _ in range(5): x = limit(x * 10 ** ((target - loud(x)) / 20))
    return x


def write(folder, name, x, meta=True):
    os.makedirs(folder, exist_ok=True); path = os.path.join(folder, name)
    sf.write(path, np.stack([x, x], 1), SR, subtype='PCM_16')
    if not meta: return
    m = path + '.meta'; guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    lines = [f'guid: {guid or uuid.uuid4().hex}' if l.startswith('guid:') else l for l in META.splitlines()]
    open(m, 'w', newline='\n').write('\n'.join(lines) + '\n')


def softness(takes):
    """Lower is softer: crest factor (dB), attack (10 ms units), brightness (kHz), length (0.1 s units)."""
    s = []
    for x in takes:
        e = env(x); on = int(np.argmax(e > e.max() * .05)); pk = int(np.argmax(e))
        crest = 20 * np.log10(np.abs(x).max() / (np.sqrt(np.mean(x ** 2)) + 1e-9))
        cent = librosa.feature.spectral_centroid(y=x.astype(float), sr=SR).mean() / 1000
        attack = (pk - on) / SR * 1000
        s.append(crest * .5 + cent + len(x) / SR * 10 - min(attack, 60) / 20)
    return float(np.mean(s))


def ten(takes, gap=.42, pitch=.04):
    out = np.zeros(int((gap * 10 + .5) * SR)); last = -1
    for k in range(10):
        i = int(rng.integers(len(takes)))
        if len(takes) > 1 and i == last: i = (i + 1) % len(takes)
        last = i; r = 1 + rng.uniform(-pitch, pitch); x = takes[i]
        t = np.arange(int(len(x) / r)) * r; y = np.interp(t, np.arange(len(x)), x)
        a = int(k * gap * SR); out[a:a + len(y)] += y[:len(out) - a]
    return out


os.makedirs(OUT, exist_ok=True)
for f in os.listdir(OUT):
    if f.endswith('.wav') or f.endswith('.wav.meta'): os.remove(os.path.join(OUT, f))
page, scores = {'cikis': []}, {}
for letter, name, credit, link, files in CANDS:
    takes = [level(shape(read(p)), -17) for p in files]
    for n, x in enumerate(takes): write(OUT, f'cikis-{letter}{n + 1}.wav', x); write(os.path.join(PAGE, 'sesler'), f'cikis-{letter}{n + 1}.wav', x, False)
    write(os.path.join(PAGE, 'sesler'), f'cikis-{letter}-10.wav', ten(takes), False)
    scores[letter] = softness(takes)
    page['cikis'].append(dict(letter=letter, name=name, credit=credit, link=link,
                              takes=[dict(file=f'cikis-{letter}{n + 1}.wav', seconds=round(len(x) / SR, 3)) for n, x in enumerate(takes)],
                              ten=f'cikis-{letter}-10.wav', softness=round(scores[letter], 2),
                              lufs=round(float(np.mean([loud(x) for x in takes])), 1), length=round(max(len(x) for x in takes) / SR, 3)))
    print(f'{letter} {name}: {len(takes)} kayıt, en uzun {max(len(x) for x in takes) / SR:.2f} s, {np.mean([loud(x) for x in takes]):.1f} LUFS, yumuşaklık {scores[letter]:.2f} (düşük = yumuşak)')
best = min(scores, key=scores.get)
for c in page['cikis']: c['default'] = c['letter'] == best
# today's c, as played (volume 1), for comparison
old = [read(os.path.join(PM95, f'cikis-c{n}.wav')) for n in range(1, 5) if os.path.exists(os.path.join(PM95, f'cikis-c{n}.wav'))]
write(os.path.join(PAGE, 'sesler'), 'cikis-c-10.wav', ten(old, pitch=.05), False)
print(f'varsayılan: {best}')
# the knock: Zeyd's a, louder
for n in range(1, 5):
    p = os.path.join(PM95, f'carpma-a{n}.wav')
    if not os.path.exists(p): continue
    x = read(p); x = x / np.abs(x).max()
    # a phone's speaker plays little under ~300 Hz and this knock is a ~130 Hz thud: gentle saturation (tanh, drive 8)
    # adds its own upper harmonics, mixed in over 350 Hz, so the same thud is heard on a phone (as loud as the tick there)
    sat = np.tanh(8 * x) / np.tanh(8)
    x = x + sosfilt(butter(2, 350, 'hp', fs=SR, output='sos'), sat)
    x = level(x, -13); write(OUT, f'carpma-a{n}.wav', x)
    hp = librosa.effects.preemphasis(x, coef=.97)   # a phone speaker hears little under ~300 Hz: roughly what is left
    print(f'carpma-a{n}: {loud(x):.1f} LUFS, tepe {20 * np.log10(np.abs(x).max()):.1f} dBFS (küçük hoparlörde kalan ~ {loud(hp):.1f})')
json.dump(dict(page, default=best), open(os.path.join(PAGE, 'sesler99.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
