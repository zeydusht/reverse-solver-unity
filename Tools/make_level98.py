"""PM_98 §2 A: four original level-music loops in the reference's manner (arcade puzzle, ~136 bpm, the rhythm in front).
Real recorded instruments of the Versilian Community Sample Library (VCSL, CC0: https://github.com/sgossner/VCSL) for the
drums (bass drum + cajon, snare, claps, hi-hat, shaker, tambourine) and the tunes (marimba, xylophone, glockenspiel,
harp, kalimba, piano); the bass is our own synthesis (sine + a few harmonics, plucked envelope). Our melodies.

Each loop: 4/4, 40 bars = A (16: the 8-bar motif twice) + B (16: the answer, busier drums) + A' (8), rendered round
(the tails after the last bar wrap onto the first: no seam). The melodic bus level is searched so the percussive share
(HPSS energy, the same measure as the reference's 0.92) lands at 0.80; then -18 LUFS, peaks under -1 dBFS.
Out: Assets/Audio/Music98/music-A..D.mp3 (+ .ogg, loops.json) and docs/art/pm98/BESTE.md (the note plan).
    python -I Tools/make_level98.py <VCSL samples dir>        (numpy, soundfile, librosa, lameenc, pyloudnorm)"""
import glob, json, os, re, sys
import numpy as np, soundfile as sf, librosa, lameenc, pyloudnorm as pyln

SR = 44100
SRC = sys.argv[1]
OUT = 'Assets/Audio/Music98'
DOC = 'docs/art/pm98/BESTE.md'
NOTE = {'C': 0, 'C#': 1, 'D': 2, 'D#': 3, 'E': 4, 'F': 5, 'F#': 6, 'G': 7, 'G#': 8, 'A': 9, 'A#': 10, 'B': 11}
MAJOR = [0, 2, 4, 5, 7, 9, 11]
rng = np.random.default_rng(98)


def midi(name):
    m = re.match(r'([A-G]#?)(-?\d)', name)
    return 12 * (int(m.group(2)) + 1) + NOTE[m.group(1)]


def load(path, keep=None):
    x, sr = sf.read(path, always_2d=True)
    x = x[:, :2] if x.shape[1] > 1 else np.repeat(x, 2, axis=1)
    if sr != SR: x = np.stack([librosa.resample(x[:, c], orig_sr=sr, target_sr=SR) for c in range(2)], 1)
    a = np.max(np.abs(x), 1); pk = a.max() + 1e-9
    on = int(np.argmax(a > pk * .03))
    x = x[max(0, on - SR // 1000):] / pk
    if keep:   # a drum cut short: fades out over its last 40 %
        n = min(len(x), int(keep * SR)); x = x[:n].copy(); f = int(n * .4)
        x[-f:] *= np.linspace(1, 0, f)[:, None] ** 2
    return x


class Pitched:
    def __init__(self, folder, pattern):
        self.notes = {}
        for f in glob.glob(os.path.join(SRC, f'*{folder}*')):
            m = re.search(pattern, os.path.basename(f))
            if m: self.notes.setdefault(midi(m.group(1)), f)
        assert self.notes, folder
        self.cache = {}

    def note(self, n, length):
        base = min(self.notes, key=lambda k: (abs(k - n), k > n))
        if base not in self.cache: self.cache[base] = load(self.notes[base])
        x = self.cache[base]; r = 2 ** ((n - base) / 12)
        if abs(r - 1) > 1e-6:
            t = np.arange(int(len(x) / r)) * r
            x = np.stack([np.interp(t, np.arange(len(x)), x[:, c]) for c in range(2)], 1)
        n_ = min(len(x), int((length + .25) * SR)); x = x[:n_].copy(); f = min(n_, int(.08 * SR))
        x[-f:] *= np.linspace(1, 0, f)[:, None]
        return x


class Drum:
    def __init__(self, pattern, keep=None):
        self.takes = [load(f, keep) for f in sorted(glob.glob(os.path.join(SRC, pattern)))]
        assert self.takes, pattern
        self.i = 0

    def note(self, *_):
        x = self.takes[self.i % len(self.takes)]; self.i += 1
        return x


def bass_note(n, length):
    """Our synth bass: a plucked sine with 2nd/3rd harmonics (warm, clear on phones)."""
    f0 = 440 * 2 ** ((n - 69) / 12); dur = length * .92
    t = np.arange(int((dur + .06) * SR)) / SR
    y = np.sin(2 * np.pi * f0 * t) + .35 * np.sin(4 * np.pi * f0 * t) + .12 * np.sin(6 * np.pi * f0 * t)
    env = np.minimum(1, t / .004) * (.55 + .45 * np.exp(-t / .09))
    env *= np.clip((dur + .06 - t) / .06, 0, 1)
    y = y * env * .6
    return np.stack([y, y], 1)


class Bass:
    def note(self, n, length): return bass_note(n, length)


INST = dict(
    marimba=lambda: Pitched('Marimba', r'Outrigger_([A-G]#?\d)_loud'),
    xylo=lambda: Pitched('Xylophone', r'Xylo_Hard_([A-G]#?\d)_ff'),
    glock=lambda: Pitched('Glockenspiel', r'glock_loud_([A-G]#?\d)'),
    harp=lambda: Pitched('Concert Harp', r'KSHarp_([A-G]#?\d)_f1'),
    kalimba=lambda: Pitched('Kalimba, Kenya', r'MainSpirit_([A-G]#?\d)_k'),
    piano=lambda: Pitched('Grand Piano', r'GPiano_sus_([A-G]#?\d)_v3'),
)
DRUMS = dict(
    kick=lambda: Drum('*Bass Drum 1*', .26), cajon=lambda: Drum('*Cajon_hit1_f*', .14),
    snare=lambda: Drum('*Snare Drum, Modern 1*_HitNS_v5*', .22), clap=lambda: Drum('*Claps__Clap_rr*', .16),
    hat=lambda: Drum('*HiHat_HitC_v3*', .08), ohat=lambda: Drum('*HiHat_HitC_v4*', .2),
    shaker=lambda: Drum('*ShakerHighFaster*', .1), tamb=lambda: Drum('*Tambourine 1*Hit_v2*', .15),
)

# ---------------------------------------------------------------- the four songs (steps: 16 per bar)
# a motif: (beat 0..31 in its 8 bars, scale degree (0 = the key's root, 7 = an octave up), length in beats)
SONGS = [
    dict(letter='A', name='Marimba Pop', bpm=136, key='C5', lead='marimba', lead2='glock', chords='harp',
         progA=[0, 4, 5, 3], progB=[3, 4, 2, 5],
         motif=[(0, 4, .5), (.5, 4, .5), (1, 5, .5), (1.5, 4, .5), (2, 2, 1), (3.5, 0, .5), (4, 2, .5), (4.5, 4, 1.5), (6.5, 5, .5), (7, 4, 1),
                (8, 4, .5), (8.5, 5, .5), (9, 7, 1), (10.5, 5, .5), (11, 4, 1), (12, 2, .5), (12.5, 1, .5), (13, 2, 2),
                (16, 4, .5), (16.5, 4, .5), (17, 5, .5), (17.5, 4, .5), (18, 2, 1), (19.5, 0, .5), (20, 2, .5), (20.5, 4, 1.5), (22.5, 5, .5), (23, 7, 1),
                (24, 6, .5), (24.5, 5, .5), (25, 4, .5), (25.5, 2, .5), (26, 1, 1), (27.5, 2, .5), (28, 0, 2)],
         answer=[(0, 7, 1), (1, 9, .5), (1.5, 7, .5), (2, 5, 1), (3, 4, 1), (4, 5, .5), (4.5, 4, .5), (5, 2, 1), (6, 4, 2),
                 (8, 7, 1), (9, 9, .5), (9.5, 11, .5), (10, 9, 1), (11, 7, 1), (12, 5, .5), (12.5, 4, .5), (13, 5, 1), (14, 4, 2)],
         drums='four'),
    dict(letter='B', name='Ksilofon Arcade', bpm=138, key='F5', lead='xylo', lead2='glock', chords='marimba',
         progA=[5, 3, 0, 4], progB=[1, 4, 0, 5],
         motif=[(0, 0, .5), (.5, 2, .5), (1, 4, .5), (1.5, 7, 1), (3, 6, .5), (3.5, 4, .5), (4, 5, 1), (5, 4, .5), (5.5, 2, .5), (6, 4, 2),
                (8, 0, .5), (8.5, 2, .5), (9, 4, .5), (9.5, 7, 1), (11, 9, .5), (11.5, 7, .5), (12, 8, 1), (13, 7, .5), (13.5, 4, .5), (14, 7, 2),
                (16, 0, .5), (16.5, 2, .5), (17, 4, .5), (17.5, 7, 1), (19, 6, .5), (19.5, 4, .5), (20, 5, 1), (21, 4, .5), (21.5, 2, .5), (22, 1, 2),
                (24, 2, .5), (24.5, 4, .5), (25, 1, .5), (25.5, 2, .5), (26, -1, 1), (27, 0, 3)],
         answer=[(0, 4, .5), (.75, 4, .25), (1, 5, .5), (2, 4, .5), (2.5, 2, .5), (3, 0, 1), (4, 1, .5), (4.75, 1, .25), (5, 2, .5), (6, 4, 2),
                 (8, 4, .5), (8.75, 4, .25), (9, 5, .5), (10, 7, .5), (10.5, 5, .5), (11, 4, 1), (12, 2, .5), (12.5, 1, .5), (13, 0, 3)],
         drums='break'),
    dict(letter='C', name='Kalimba Zıplama', bpm=132, key='G4', lead='kalimba', lead2='xylo', chords='harp',
         progA=[0, 2, 3, 4], progB=[5, 3, 0, 4],
         motif=[(0, 4, .75), (.75, 4, .75), (1.5, 5, .5), (2, 7, 1), (3, 5, 1), (4, 4, .75), (4.75, 2, .75), (5.5, 1, .5), (6, 2, 2),
                (8, 4, .75), (8.75, 4, .75), (9.5, 5, .5), (10, 7, 1), (11, 9, 1), (12, 7, .75), (12.75, 5, .75), (13.5, 4, .5), (14, 4, 2),
                (16, 4, .75), (16.75, 4, .75), (17.5, 5, .5), (18, 7, 1), (19, 5, 1), (20, 4, .75), (20.75, 2, .75), (21.5, 1, .5), (22, 2, 2),
                (24, 5, .75), (24.75, 4, .75), (25.5, 2, .5), (26, 1, 1), (27, -1, 1), (28, 0, 3)],
         answer=[(0, 9, .5), (.5, 7, .5), (1, 9, .5), (1.5, 11, .5), (2, 9, 2), (4, 7, .5), (4.5, 5, .5), (5, 7, .5), (5.5, 9, .5), (6, 7, 2),
                 (8, 5, .5), (8.5, 4, .5), (9, 5, .5), (9.5, 7, .5), (10, 5, 1), (11, 4, 1), (12, 2, 1), (13, 4, 1), (14, 2, 2)],
         drums='four'),
    dict(letter='D', name='Piyano Koşu', bpm=136, key='D5', lead='glock', lead2='marimba', chords='piano',
         progA=[0, 5, 1, 4], progB=[3, 0, 3, 4],
         motif=[(0, 0, .5), (.5, 4, .5), (1, 7, .5), (1.5, 4, .5), (2, 5, .5), (2.5, 4, .5), (3, 2, 1), (4, 1, .5), (4.5, 2, .5), (5, 4, .5), (5.5, 2, .5), (6, 1, 2),
                (8, -1, .5), (8.5, 1, .5), (9, 4, .5), (9.5, 1, .5), (10, 3, .5), (10.5, 2, .5), (11, 1, 1), (12, 0, .5), (12.5, 1, .5), (13, 2, .5), (13.5, 4, .5), (14, 4, 2),
                (16, 0, .5), (16.5, 4, .5), (17, 7, .5), (17.5, 4, .5), (18, 5, .5), (18.5, 4, .5), (19, 2, 1), (20, 1, .5), (20.5, 2, .5), (21, 4, .5), (21.5, 5, .5), (22, 7, 2),
                (24, 6, .5), (24.5, 4, .5), (25, 2, .5), (25.5, 1, .5), (26, 4, 1), (27, 1, 1), (28, 0, 3)],
         answer=[(0, 7, 1.5), (1.5, 6, .5), (2, 7, .5), (2.5, 9, .5), (3, 7, 1), (4, 5, 1.5), (5.5, 4, .5), (6, 2, 2),
                 (8, 3, 1.5), (9.5, 4, .5), (10, 5, .5), (10.5, 7, .5), (11, 5, 1), (12, 4, 1), (13, 2, 1), (14, 4, 2)],
         drums='break'),
]

# drum patterns: per part a 16-step bar; 'x' a hit (accent), 'o' a softer one, '.' nothing
PATTERNS = {
    'four': dict(kick='x...x...x...x...', snare='....x.......x...', clap='....x.......x...', hat='x.o.x.o.x.o.x.o.',
                 ohat='..x...x...x...x.', shaker='oxooxoooxooooxoo', tamb='................'),
    'break': dict(kick='x.....x...x.....', snare='....x.......x..o', clap='....x.......x...', hat='xoxoxoxoxoxoxoxo',
                  ohat='..............x.', shaker='o.o.o.o.o.o.o.o.', tamb='....x.......x...'),
}
GAIN = dict(kick=1.0, cajon=.55, snare=.55, clap=.45, hat=.12, ohat=.08, shaker=.09, tamb=.11)   # the cymbals kept low: the reference's brightness (~1560 Hz)


def degree(d, key):
    o, s = divmod(d, 7)
    return midi(key) + 12 * o + MAJOR[s]


def render(song, melodic):
    beat = 60 / song['bpm']; bar = 4 * beat; bars = 40
    total = int(bars * bar * SR)
    out = np.zeros((total + 3 * SR, 2)); mel = np.zeros_like(out)
    inst = {k: INST[song[k]]() for k in ('lead', 'lead2', 'chords')}
    drums = {k: f() for k, f in DRUMS.items()}
    bass = Bass()

    def put(buf, x, t, gain, pan=0.):
        i = int(round(t * SR)); x = x * gain * np.array([min(1, 1 - pan), min(1, 1 + pan)])
        buf[i:i + len(x)] += x[:len(buf) - i]

    plan = []   # the note plan for BESTE.md
    for b in range(bars):
        part = 'A' if b < 16 or b >= 32 else 'B'
        prog = song['progA'] if part == 'A' else song['progB']
        chord = prog[b % 4]
        t0 = b * bar
        # drums
        pat = PATTERNS[song['drums']]
        for name, steps in pat.items():
            for s, c in enumerate(steps):
                if c == '.': continue
                g = GAIN[name] * (1 if c == 'x' else .55) * (1 + rng.uniform(-.06, .06))
                if part == 'B' and name in ('hat', 'shaker'): g *= 1.25
                put(out, drums[name].note(), t0 + s * beat / 4, g, {'hat': .25, 'ohat': .25, 'shaker': -.3, 'tamb': -.25}.get(name, 0))
                if name == 'kick': put(out, drums['cajon'].note(), t0 + s * beat / 4, GAIN['cajon'] * (1 if c == 'x' else .6))
        if part == 'B':   # B: a tambourine on every off-beat and a kick pickup
            for s in (2, 6, 10, 14): put(out, drums['tamb'].note(), t0 + s * beat / 4, GAIN['tamb'] * .8, -.25)
            if b % 2 == 1: put(out, drums['kick'].note(), t0 + 14 * beat / 4, GAIN['kick'] * .7)
        if b % 8 == 7:    # a fill on the last beat of every 8 bars
            for s in (12, 13, 14, 15): put(out, drums['snare'].note(), t0 + s * beat / 4, GAIN['snare'] * (.45 + .12 * (s - 12)))
        # bass: the chord root, eighth notes, octave jump on the off-beats
        root = degree(chord, song['key']) - 24
        for e in range(8):
            n = root + (12 if e % 2 == 1 and song['drums'] == 'four' else 0) + (7 if e == 6 and song['drums'] == 'break' else 0)
            put(mel, bass.note(n, beat / 2), t0 + e * beat / 2, .8)
        # chords: off-beat plucks (2 and 4 "and"), a triad from the chord degree
        for s in (2, 6, 10, 14):
            for k, d in enumerate((chord, chord + 2, chord + 4)):
                put(mel, inst['chords'].note(degree(d, song['key']) - 12, beat / 2), t0 + s * beat / 4 + k * .004, .22, (k - 1) * .3)
        plan.append(f'{b + 1:2d} {part} akor {["I", "ii", "iii", "IV", "V", "vi", "vii"][chord % 7]}')
    # melody: A = motif bars 1-16 (twice 8 bars, the second ends on the root), B = the answer twice, A' = motif's first 8
    def play_motif(m, start_bar, lead, gain, up=0):
        for beat_at, d, ln in m:
            t = start_bar * bar + beat_at * beat
            put(mel, inst[lead].note(degree(d, song['key']) + up, ln * beat), t, gain, .1)
            if lead == 'lead': put(mel, inst['lead2'].note(degree(d, song['key']) + 12, ln * beat), t + .006, gain * .25, -.2)
    play_motif(song['motif'], 0, 'lead', .55)
    play_motif(song['answer'], 16, 'lead', .5); play_motif(song['answer'], 24, 'lead', .5, 0)
    play_motif([m for m in song['motif'] if m[0] < 32], 32, 'lead', .55)
    mix = out + mel * melodic
    loop = mix[:total].copy()
    loop[:len(mix) - total] += mix[total:]   # the tails wrap round: no seam
    return loop, plan


def perc_share(x):
    y = librosa.resample(x.mean(1), orig_sr=SR, target_sr=22050)[:22050 * 60]
    S = np.abs(librosa.stft(y)); H, P = librosa.decompose.hpss(S)
    return float((P ** 2).sum() / ((S ** 2).sum() + 1e-9))


def limit(x, ceiling=10 ** (-1 / 20)):
    w = int(.005 * SR); a = np.max(np.abs(x), 1)
    env = np.array([a[max(0, i - w):i + w].max() for i in range(0, len(a), w)])
    g = np.minimum(1, ceiling / np.maximum(env, 1e-9)); rel = np.exp(-1 / 12)
    for i in range(1, len(g)): g[i] = min(g[i], g[i - 1] * rel + (1 - rel))
    return x * np.interp(np.arange(len(a)), np.arange(len(g)) * w, g)[:, None]


def main():
    os.makedirs(OUT, exist_ok=True); os.makedirs(os.path.dirname(DOC), exist_ok=True)
    loops, doc = {}, ['# PM_98: kendi bestelerimiz (bölüm müziği A–D), nota planı', '',
                      'Araç: `Tools/make_level98.py`. Hepsi 4/4, 40 ölçü: A (16 ölçü: 8 ölçülük motif iki kez) + B (16: cevap, davul daha yoğun, tef) + A′ (8). '
                      'Döngü yuvarlak çizilir: son ölçünün kuyrukları başa eklenir, dikiş yok.', '']
    for song in SONGS:
        lo, hi, melodic = .05, 2.0, .6
        for _ in range(7):   # the melodic level for a 0.80 percussive share
            x, plan = render(song, melodic)
            p = perc_share(x)
            if abs(p - .80) < .02: break
            if p > .80: lo = melodic
            else: hi = melodic
            melodic = (lo + hi) / 2
        meter = pyln.Meter(SR)
        for _ in range(4): x = limit(x * 10 ** ((-18 - meter.integrated_loudness(np.concatenate([x, x]))) / 20))
        lufs = meter.integrated_loudness(np.concatenate([x, x]))
        tempo = librosa.beat.beat_track(y=librosa.resample(x.mean(1), orig_sr=SR, target_sr=22050), sr=22050, start_bpm=song['bpm'], units='time')
        bt = tempo[1]; ibi = np.diff(bt)
        pcm = (np.clip(x, -1, 1) * 32767).astype('<i2')
        enc = lameenc.Encoder(); enc.set_bit_rate(128); enc.set_in_sample_rate(SR); enc.set_channels(2); enc.set_quality(2)
        mp3 = enc.encode(pcm.tobytes()) + enc.flush()
        path = f'{OUT}/music-{song["letter"]}.mp3'; open(path, 'wb').write(mp3)
        with sf.SoundFile(f'{OUT}/music-{song["letter"]}.ogg', 'w', SR, 2, format='OGG', subtype='VORBIS', compression_level=.6) as o:
            for i in range(0, len(x), SR): o.write(x[i:i + SR])
        d, _ = sf.read(path, dtype='float32'); ref = x[:SR // 2].mean(1); lead = int(np.argmax(np.correlate(d[:SR].mean(1), ref, 'valid')))
        step = float(np.abs(x[0] - x[-1]).max()); usual = float(np.percentile(np.abs(np.diff(x, axis=0)).max(1), 99.9))
        cent = float(librosa.feature.spectral_centroid(y=librosa.resample(x.mean(1), orig_sr=SR, target_sr=22050), sr=22050).mean())
        loops[song['letter']] = {'start': lead / SR, 'length': len(x) / SR, 'bpm': song['bpm'], 'lufs': round(lufs, 1), 'perc': round(p, 2),
                                 'name': song['name'], 'centroid': int(cent), 'beat_bpm': round(60 / float(np.median(ibi)), 1), 'beat_jitter_ms': round(float(np.std(ibi)) * 1000, 1)}
        print(f'{song["letter"]} {song["name"]}: {len(x) / SR:.1f} s, {song["bpm"]} bpm (ölçülen {60 / np.median(ibi):.1f}, vuruş aralığı sapması {np.std(ibi) * 1000:.1f} ms), '
              f'vurmalı {p:.2f} (melodi {melodic:.2f}), {lufs:.1f} LUFS, parlaklık {cent:.0f} Hz, dikiş {step:.4f} < {usual:.4f}, mp3 {len(mp3) / 1e6:.2f} MB, lead-in {lead}')
        doc += [f'## {song["letter"]}: {song["name"]}', '',
                f'- {song["bpm"]} BPM, ton {song["key"][:-1]} majör; davul kalıbı `{song["drums"]}`; motif {song["lead"]} (+ bir oktav üstte {song["lead2"]}), akor {song["chords"]} (2. ve 4. vuruşun arkası), bas bizim sentezimiz (sekizlik).',
                f'- Davul (16’lık adımlar, x vurgu, o hafif): ' + ', '.join(f'{k} `{v}`' for k, v in PATTERNS[song["drums"]].items()) + '; B’de tef her arka vuruşta, 2 ölçüde bir 4. vuruş arkasında ek bas davul; her 8 ölçünün son vuruşunda trampet geçişi.',
                f'- Akorlar: A {" – ".join(["I", "ii", "iii", "IV", "V", "vi", "vii"][c] for c in song["progA"])}, B {" – ".join(["I", "ii", "iii", "IV", "V", "vi", "vii"][c] for c in song["progB"])} (ölçü başına bir akor).',
                f'- Motif (vuruş, derece, uzunluk; derece 0 = ton kökü, 7 = bir oktav üstü): `{song["motif"]}`',
                f'- Cevap (B): `{song["answer"]}`',
                f'- Ölçüm: {loops[song["letter"]]}', '']
    json.dump(loops, open(f'{OUT}/loops.json', 'w'), indent=1)
    open(DOC, 'w', encoding='utf8').write('\n'.join(doc) + '\n')


main()
