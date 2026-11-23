"""PM_96 §5: the win fanfares and their beat map.
For every fanfare (0: the old one, a-c PM_95's jingles, d-g PM_96's new candidates):
  - the WAV the web plays (music/pm96/kazanma-X.wav on GitHub Pages; Assets/Audio/Fanfare96/ here): the old ones as the
    game played them (the old clip peak-normalised, as Unity imports it), the new ones levelled to b's loudness (Zeyd's
    pick), all starting at their first sound;
  - the new ones also go to Resources/Art/audio/pm96/kazanma-X.wav (the editor and iOS play them there);
  - their beats: onsets (librosa spectral flux, every attack sharpened to the steepest rise of the 1 ms envelope), the
    5-7 the win screen uses (the strongest, at least .17 s apart, the first one always), their strengths and the
    strongest -> Resources/Art/audio/pm96/vurus_haritasi.json. A changed fanfare: run this again; the screen follows.
    python -I Tools/make_beatmap96.py <new fanfares dir: kazanma-d..g.wav>        (librosa, soundfile, numpy, pyloudnorm)"""
import json, os, sys
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln

SR = 44100
NEW = sys.argv[1]
WEB = 'Assets/Audio/Fanfare96'
RES = 'Assets/_Game/Presentation/Resources/Art/audio/pm96'
OLD = {'0': 'Assets/_Game/Presentation/Resources/Art/audio/win.wav',
       'a': 'Assets/_Game/Presentation/Resources/Art/audio/pm95/kazan-a1.wav',
       'b': 'Assets/_Game/Presentation/Resources/Art/audio/pm95/kazan-b1.wav',
       'c': 'Assets/_Game/Presentation/Resources/Art/audio/pm95/kazan-c1.wav'}
NEW_LETTERS = 'defg'
MIN_GAP, MAX_BEATS, MIN_BEATS, MAX_LENGTH = .14, 7, 5, 2.5


def read(path):
    x, sr = sf.read(path, always_2d=True)
    if x.shape[1] == 1: x = np.repeat(x, 2, axis=1)
    x = x[:, :2]
    if sr != SR: x = np.stack([librosa.resample(x[:, c], orig_sr=sr, target_sr=SR) for c in range(2)], 1)
    return x


def trim(x):
    """From the first sound (-45 dB under the peak), 1 ms before it."""
    a = np.max(np.abs(x), 1)
    on = int(np.argmax(a > a.max() * 10 ** (-45 / 20)))
    return x[max(0, on - SR // 1000):]


def beats_of(x, max_beats=None):
    max_beats = max_beats or MAX_BEATS
    mono = x.mean(1)
    pad = int(.3 * SR)
    y = np.concatenate([np.zeros(pad), mono, np.zeros(pad)])
    hop = 128
    env = librosa.onset.onset_strength(y=y, sr=SR, hop_length=hop, n_fft=1024)
    frames = librosa.onset.onset_detect(onset_envelope=env, sr=SR, hop_length=hop, delta=.04, wait=int(.06 * SR / hop))
    # the 1 ms RMS envelope: an onset is where it rises fastest, within 30 ms before the flux peak (and 10 ms after)
    w = SR // 1000
    rms = np.sqrt(np.convolve(y ** 2, np.ones(w) / w, mode='same')[::w] + 1e-12)
    db = 20 * np.log10(rms)
    rise = np.diff(db, prepend=db[0])
    out = []
    for f in frames:
        t = f * hop / SR
        lo, hi = int((t - .03) * 1000), int((t + .01) * 1000)
        lo, hi = max(1, lo), min(len(rise) - 1, hi)
        k = lo + int(np.argmax(rise[lo:hi])) if hi > lo else int(t * 1000)
        onset = (k - 1) / 1000 - pad / SR
        if onset < -.005 or onset > len(mono) / SR: continue
        a, b = int(max(0, onset) * SR), int((max(0, onset) + .08) * SR)
        peak = float(np.max(np.abs(mono[a:b]))) if b > a else 0
        flux = float(env[min(len(env) - 1, f)])
        out.append((max(0.0, onset), peak, flux))
    if not out: return [], []
    pk = max(p for _, p, _ in out); fx = max(f for *_, f in out)
    hits = [(t, .6 * p / pk + .4 * f / fx) for t, p, f in out]
    # strong ones only (a quiet grace note is not a beat), at least MIN_GAP apart (the stronger stays)
    for floor in (.25, .18, .12):   # a fanfare of soft notes: a lower floor, until there are MIN_BEATS
        keep = []
        for t, s in hits:
            if s < floor: continue
            if keep and t - keep[-1][0] < MIN_GAP:
                if s > keep[-1][1] and len(keep) > 1: keep[-1] = (t, s)
                continue
            keep.append((t, s))
        if len(keep) >= MIN_BEATS: break
    while len(keep) > max_beats:   # the weakest goes, never the first
        i = 1 + int(np.argmin([s for _, s in keep[1:]]))
        keep.pop(i)
    return [round(t, 4) for t, _ in keep], [round(s, 3) for _, s in keep]


def limit(x, ceiling=10 ** (-1 / 20)):
    """A gentle peak limiter: the gain follows the 5 ms peak envelope down at once and back up over 60 ms."""
    w = int(.005 * SR)
    a = np.max(np.abs(x), 1)
    env = np.array([a[max(0, i - w):i + w].max() for i in range(0, len(a), w)])
    g = np.minimum(1, ceiling / np.maximum(env, 1e-9))
    rel = np.exp(-1 / (.06 / .005))
    for i in range(1, len(g)): g[i] = min(g[i], g[i - 1] * rel + (1 - rel))
    gain = np.interp(np.arange(len(a)), np.arange(len(g)) * w, g)
    return x * gain[:, None]


def level_to(x, lufs):
    """To `lufs` (integrated), the few peaks over -1 dBFS limited; a fanfare longer than MAX_LENGTH fades out by then."""
    if len(x) > MAX_LENGTH * SR:
        x = x[:int(MAX_LENGTH * SR)].copy(); n = int(.3 * SR)
        x[-n:] *= np.linspace(1, 0, n)[:, None] ** 2
    meter = pyln.Meter(SR, block_size=.2)
    for _ in range(4):   # the limiter takes a little loudness back: again
        x = limit(x * 10 ** ((lufs - meter.integrated_loudness(x)) / 20))
    return x


os.makedirs(WEB, exist_ok=True); os.makedirs(RES, exist_ok=True)
files = {}
for k, p in OLD.items():
    x = read(p)
    if k == '0': x = x / np.max(np.abs(x))   # Unity imports the old clip normalised
    files[k] = trim(x)
target = pyln.Meter(SR, block_size=.2).integrated_loudness(files['b'])
for k in NEW_LETTERS:
    p = next((os.path.join(NEW, f) for f in sorted(os.listdir(NEW)) if f.startswith(f'kazanma-{k}.')), None)
    if p is None: continue
    files[k] = level_to(trim(read(p)), target)

# PM_99 §5: the short (5-beat) cut of a 7-beat fanfare (&kazanmavurus=5): its two middle beats (3 and 4) left out, from
# 12 ms before beat 3 to 12 ms before beat 5 (a whole number of its eighth notes, so the rhythm runs on), a 10 ms
# equal-power crossfade; the last chord as it was. Written as kazanma-<letter>5.
SHORT = 'b'
short_beats = {}
for k in SHORT:
    x = files[k]; b, _ = beats_of(x)
    if len(b) != 7: print(f'{k}: {len(b)} beats, no short cut'); continue
    c1, c2, f = int((b[2] - .012) * SR), int((b[4] - .012) * SR), int(.01 * SR)
    t = np.linspace(0, np.pi / 2, f)[:, None]
    files[k + '5'] = np.concatenate([x[:c1], x[c1:c1 + f] * np.cos(t) + x[c2:c2 + f] * np.sin(t), x[c2 + f:]])
    d = (c2 - c1) / SR   # its beats: the full one's 1, 2, 5, 6, 7, the last three earlier by the cut
    short_beats[k + '5'] = [round(b[0], 4), round(b[1], 4), round(b[4] - d, 4), round(b[5] - d, 4), round(b[6] - d, 4)]
    print(f'{k}5: {k} without {b[2]:.3f}-{b[4]:.3f} s ({(c2 - c1) / SR:.3f} s)')

entries = []
for k, x in sorted(files.items()):
    sf.write(os.path.join(WEB, f'kazanma-{k}.wav'), x, SR, subtype='PCM_16')
    if k in set(NEW_LETTERS) or k in short_beats: sf.write(os.path.join(RES, f'kazanma-{k}.wav'), x, SR, subtype='PCM_16')
    beats, strength = beats_of(x)
    if k in short_beats:
        full = dict(zip(*beats_of(files[k[0]])))
        beats = short_beats[k]; fs = list(full.values()); strength = [fs[0], fs[1], fs[4], fs[5], fs[6]]
    strong = int(np.argmax(strength[2:])) + 2 if len(strength) > 2 else len(strength) - 1
    length = round(len(x) / SR, 3)
    entries.append(dict(letter=k, length=length, beats=beats, strength=strength, strong=strong))
    lufs = pyln.Meter(SR, block_size=.2).integrated_loudness(x)
    flag = '' if MIN_BEATS <= len(beats) <= (5 if k.endswith('5') else MAX_BEATS) else '  (!)'
    print(f'{k}: {length:.2f} s, {lufs:.1f} LUFS, {len(beats)} beats{flag}: ' + ' '.join(f'{b:.3f}' for b in beats) + f'  strongest {strong + 1}')
with open(os.path.join(RES, 'vurus_haritasi.json'), 'w', encoding='utf8') as f:
    json.dump(dict(fanfares=entries), f, indent=1)
