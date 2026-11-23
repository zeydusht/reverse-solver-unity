"""PM_96 §3: three original win fanfares (e, f, g), played from real recorded instruments of the Versilian Community
Sample Library (VCSL, CC0: https://github.com/sgossner/VCSL). Our own melodies; each 1.9-2.4 s with 5-7 clear hits
and a bright last chord, for the win screen's events (Tools/make_beatmap96.py finds the hits).
    python -I Tools/make_fanfare96.py <VCSL samples dir> <out dir>
The samples dir holds the VCSL files flattened (folder/sub/file.wav -> folder__sub__file.wav). Out: kazanma-e/f/g.wav
(44.1 kHz stereo, 16 bit), peak -1 dBFS before Tools/make_beatmap96.py levels them."""
import glob, os, re, sys
import numpy as np, soundfile as sf

SR = 44100
SRC, OUT = sys.argv[1], sys.argv[2]
NOTE = {'C': 0, 'C#': 1, 'D': 2, 'D#': 3, 'E': 4, 'F': 5, 'F#': 6, 'G': 7, 'G#': 8, 'A': 9, 'A#': 10, 'B': 11}


def midi(name):
    m = re.match(r'([A-G]#?)(-?\d)', name)
    return 12 * (int(m.group(2)) + 1) + NOTE[m.group(1)]


def load(path):
    x, sr = sf.read(path, always_2d=True)
    if x.shape[1] == 1: x = np.repeat(x, 2, axis=1)
    x = x[:, :2]
    if sr != SR:
        t = np.arange(int(len(x) * SR / sr)) * sr / SR
        x = np.stack([np.interp(t, np.arange(len(x)), x[:, c]) for c in range(2)], 1)
    peak = np.max(np.abs(x)) + 1e-9
    on = np.argmax(np.max(np.abs(x), 1) > peak * .03)   # the hit itself: no lead-in
    return x[max(0, on - int(.002 * SR)):] / peak


class Instrument:
    def __init__(self, folder, pattern):
        self.notes = {}
        for f in glob.glob(os.path.join(SRC, f'*{folder}*')):
            m = re.search(pattern, os.path.basename(f))
            if m: self.notes.setdefault(midi(m.group(1)), f)
        assert self.notes, folder
        self.cache = {}

    def note(self, n, length=None):
        base = min(self.notes, key=lambda k: (abs(k - n), k > n))
        if base not in self.cache: self.cache[base] = load(self.notes[base])
        x = self.cache[base]
        ratio = 2 ** ((n - base) / 12)
        if abs(ratio - 1) > 1e-6:
            t = np.arange(int(len(x) / ratio)) * ratio
            x = np.stack([np.interp(t, np.arange(len(x)), x[:, c]) for c in range(2)], 1)
        if length:
            n_ = int(length * SR)
            x = x[:n_].copy()
            fade = min(len(x), int(.06 * SR))
            x[-fade:] *= np.linspace(1, 0, fade)[:, None]
        return x


class Hit:
    def __init__(self, files):
        self.files = sorted(glob.glob(os.path.join(SRC, files)))
        assert self.files, files
        self.i = 0

    def note(self, *_):
        f = self.files[self.i % len(self.files)]; self.i += 1
        return load(f)


def N(s):
    return midi(s)


def render(events, length):
    out = np.zeros((int(length * SR) + SR, 2))
    for at, inst, note, gain, pan, *rest in events:
        x = inst.note(note if isinstance(note, int) else (N(note) if note else 0), *(rest or [None]))
        x = x * gain
        x = x * np.array([min(1, 1 - pan), min(1, 1 + pan)])
        i = int(at * SR)
        n = min(len(x), len(out) - i)
        out[i:i + n] += x[:n]
    out = out[:int(length * SR)]
    fade = int(.25 * SR)
    out[-fade:] *= np.linspace(1, 0, fade)[:, None] ** 2
    return out / (np.max(np.abs(out)) + 1e-9) * 10 ** (-1 / 20)


glock = Instrument('Glockenspiel', r'glock_loud_([A-G]#?\d)')
marimba = Instrument('Marimba', r'Outrigger_([A-G]#?\d)_loud')
xylo = Instrument('Xylophone', r'Xylo_Hard_([A-G]#?\d)_ff')
piano = Instrument('Grand Piano', r'GPiano_sus_([A-G]#?\d)_v3')
harp = Instrument('Concert Harp', r'KSHarp_([A-G]#?\d)_f1')
bells = Instrument('Tubular Bells', r'chimes_([A-G]#?\d)_ff')
snare = Hit('*Snare Drum, Modern 1*_HitNS_v5*')
crash = Hit('*Clash Cymbals 1*ff2*')
timp = Hit('*Timpani 1__Hit__Timpani1_Hit_v4*')
kick = Hit('*Bass Drum 1*')
clap = Hit('*Claps*')
belltree = Hit('*Bell Tree*Hit_C5*')
chimes = Hit('*Mark Trees*fastAsc1*')
tri = Hit('*Triangles*hit_mf*')
tamb = Hit('*Tambourine 1*Hit_v2*')


def chord(at, inst, notes, gain, spread=.0, length=None):
    return [(at + i * spread, inst, n, gain, (i - (len(notes) - 1) / 2) * .25, length) for i, n in enumerate(notes)]


# e: "Marimba parade", C major, 140 bpm: an arpeggio climbing in eighths, a two-note turn, the big chord.
q = 60 / 140 / 2
e = []
for k, (n, hi, g) in enumerate([('C5', 'C6', .9), ('E5', 'E6', .9), ('G5', 'G6', .95), ('C6', 'C7', 1.0)]):
    e += [(k * q, marimba, n, g, -.15), (k * q, glock, hi, .38, .2)]
    e += [(k * q, snare, None, .30 if k else .42, 0)]
e += [(4.5 * q, marimba, 'A5', .8, -.1), (4.5 * q, glock, 'A6', .3, .2), (4.5 * q, clap, None, .35, 0)]
e += [(5.0 * q, marimba, 'B5', .85, -.1), (5.0 * q, glock, 'B6', .32, .2), (5.0 * q, clap, None, .35, 0)]
end = 6 * q
e += chord(end, marimba, ['C5', 'E5', 'G5', 'C6'], .7) + chord(end, glock, ['E6', 'G6', 'C7'], .35)
e += [(end, crash, None, .55, 0), (end, timp, None, .7, 0), (end, kick, None, .5, 0), (end + .02, belltree, None, .35, .3)]
fanfares = {'e': (e, 2.25)}

# f: "Xylophone hop", F major, 150 bpm: a two-note pickup, two hops up, the top chord with a cymbal (the strongest),
# then a glockenspiel sparkle (the last).
b = 60 / 150 / 2
f = []
f += [(0, xylo, 'C5', .8, -.1), (0, harp, 'F3', .45, .2), (0, tamb, None, .3, .3)]
f += [(b, xylo, 'E5', .82, -.1), (b, tamb, None, .25, .3)]
f += [(2 * b, xylo, 'F5', .95, -.1), (2 * b, harp, 'A3', .55, .2), (2 * b, clap, None, .4, 0), (2 * b, kick, None, .4, 0)]
f += [(4 * b, xylo, 'A5', .95, -.1), (4 * b, harp, 'C4', .55, .2), (4 * b, clap, None, .4, 0), (4 * b, kick, None, .4, 0)]
top = 6 * b
f += chord(top, xylo, ['F5', 'A5', 'C6'], .75) + chord(top, harp, ['F3', 'C4', 'F4'], .55, .012)
f += [(top, crash, None, .6, 0), (top, timp, None, .65, 0), (top, kick, None, .55, 0)]
f += [(top + 2.25 * b, glock, 'F6', .6, .25), (top + 2.25 * b, glock, 'C7', .4, -.2), (top + 2.25 * b, tri, None, .3, -.3), (top + 2.25 * b + .03, chimes, None, .3, .2)]
fanfares['f'] = (f, 2.3)

# g: "Piano ta-da", G major -> C: three rising chords, a held breath, the big C with bells.
g = []
s = .3
for k, ch in enumerate([['G3', 'B3', 'D4', 'G4'], ['A3', 'C4', 'E4', 'A4'], ['B3', 'D4', 'F#4', 'B4']]):
    g += chord(k * s, piano, ch, .55, .006, .28) + [(k * s, snare, None, .28 + .05 * k, 0)]
g += chord(3 * s, piano, ['C4', 'E4', 'G4', 'C5'], .6, .006, .3) + [(3 * s, snare, None, .4, 0), (3 * s, kick, None, .45, 0)]
fin = 3 * s + .55
g += [(fin - .12, piano, 'G4', .45, 0, .1)]
g += chord(fin, piano, ['C3', 'G3', 'C4', 'E4', 'G4', 'C5', 'E5'], .5, .004) + chord(fin, bells, ['C5', 'G5'], .5)
g += [(fin, crash, None, .55, 0), (fin, timp, None, .7, 0), (fin, glock, 'C7', .4, .25), (fin + .03, belltree, None, .3, -.2)]
fanfares['g'] = (g, 2.45)

os.makedirs(OUT, exist_ok=True)
for letter, (events, length) in fanfares.items():
    x = render(events, length)
    sf.write(os.path.join(OUT, f'kazanma-{letter}.wav'), x, SR, subtype='PCM_16')
    print(letter, f'{len(x) / SR:.2f} s, {len(set(round(ev[0], 3) for ev in events))} hit times')
