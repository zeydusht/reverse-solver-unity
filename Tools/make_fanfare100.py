"""PM_100 §1: the win fanfare b (Zeyd: "the piano is good, the music and the screen fit") in other instruments, the same
notes, the same rhythm, the same length: its beat map (Resources/Art/audio/pm96/vurus_haritasi.json, b and b5) stays as
it is and the win screen stays in sync.
The notes are b's own, transcribed from the recording (Fupi's "Win Jingle", CC0): a C7 arpeggio (C E G), Eb (Eb G Bb),
F (F A C), Bb (Bb D F Bb), every note's attack on b's own onset (the beats exactly on b's beats). The 5-beat cut (b5)
is the same score without the notes between its cut points, the later ones earlier by the cut (b5's beats).
Instruments: real recordings, CC0 (docs/art/pm100/LISANS.md):
  marimba   marimba + glockenspiel                     VCSL
  pizz      violin section pizzicato + cello pizzicato + hand chimes          VSCO 2 CE, VCSL
  ksilofon  xylophone + marimba bass + hand claps      VCSL
  kutu      music box: glockenspiel (soft) + kalimba, a soft synth pad (ours) under it          VCSL
  bakir     trumpet staccato + tuba staccato + timpani                         VSCO 2 CE, VCSL
Each levelled to b's loudness (integrated, the way Tools/make_beatmap96.py levels the fanfares).
    python -I Tools/make_fanfare100.py <samples dir: VCSL and VSCO 2 CE files flattened (a/b/c.wav -> a__b__c.wav)>
Out: Assets/Audio/Fanfare96/kazanma-b-<tone>.wav and kazanma-b5-<tone>.wav (the web plays them from GitHub Pages,
music/pm96/; the editor loads them from here), docs/art/pm100/onizleme/sesler/kazanma-<tone>.wav (the page), and
docs/art/pm100/fanfar100.json (each one's beats as found in it, against b's)."""
import glob, json, os, re, sys
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln

SR = 44100
SRC = sys.argv[1]
WEB = 'Assets/Audio/Fanfare96'
PAGE = 'docs/art/pm100/onizleme/sesler'
NOTE = {'C': 0, 'C#': 1, 'D': 2, 'D#': 3, 'E': 4, 'F': 5, 'F#': 6, 'G': 7, 'G#': 8, 'A': 9, 'A#': 10, 'B': 11}
FULL_LEN, B = 2.54, [0.0, .525, .782, .944, 1.407, 1.753, 1.909]   # b's length and beats (the map)
CUT0, CUT1 = B[2] - .012, B[4] - .012                                  # b5: make_beatmap96.py's cut
rng = np.random.default_rng(100)


def m(name):
    g = re.match(r'([A-G]#?)(-?\d)', name)
    return 12 * (int(g.group(2)) + 1) + NOTE[g.group(1)]


# b's score: (time s, midi, velocity 0..1, part). mel: the tune; bass; beat: a beat of b (the second instrument's
# accent); the beats are the loudest notes, so a fanfare's own onsets give back b's beats.
C2, Eb2, F2, Bb2 = 36, 39, 41, 46
C3, D3, Eb3, E3, F3, G3, A3, Bb3 = 48, 50, 51, 52, 53, 55, 57, 58
C4, D4, F4, Bb4 = 60, 62, 65, 70
SCORE = [
    (0.000, C3, .80, 'mel'), (0.000, C4, .70, 'mel'), (0.000, C2, .85, 'bass'), (0.000, C4, 1, 'beat'),
    (0.160, E3, .55, 'mel'),
    (0.326, G3, .58, 'mel'),
    (0.525, Eb3, .80, 'mel'), (0.525, Eb2, .90, 'bass'), (0.525, Eb3, .8, 'beat'),
    (0.637, G3, .55, 'mel'),
    (0.782, Bb3, .82, 'mel'), (0.782, Bb3, .8, 'beat'),
    (0.944, F3, .82, 'mel'), (0.944, F2, .90, 'bass'), (0.944, F3, .8, 'beat'),
    (1.090, A3, .55, 'mel'),
    (1.252, C4, .58, 'mel'),
    (1.407, Bb3, .95, 'mel'), (1.407, D3, .70, 'mel'), (1.407, Bb2, 1.0, 'bass'), (1.407, Bb3, 1, 'beat'),
    (1.477, F3, .45, 'mel'),
    (1.559, D4, .55, 'mel'),
    (1.753, F4, .85, 'mel'), (1.753, F4, .85, 'beat'),
    (1.909, Bb4, .95, 'mel'), (1.909, F4, .62, 'mel'), (1.909, D4, .62, 'mel'), (1.909, Bb3, .62, 'mel'),
    (1.909, Bb2, .95, 'bass'), (1.909, Bb4, 1, 'beat'),
]
# the chords under it (the pad): from, to, notes
CHORDS = [(0.0, .525, [C3, E3, G3, Bb3]), (.525, .944, [Eb3, G3, Bb3]), (.944, 1.407, [F3, A3, C4]), (1.407, FULL_LEN, [Bb2, D3, F3, Bb3])]


def short(score):
    """b5: without the notes in [CUT0, CUT1), the later ones earlier by the cut."""
    d = CUT1 - CUT0
    return [(t if t < CUT0 else t - d, n, v, p) for t, n, v, p in score if not (CUT0 <= t < CUT1)]


def load(path):
    x, sr = sf.read(path, always_2d=True)
    if x.shape[1] == 1: x = np.repeat(x, 2, axis=1)
    x = x[:, :2].astype(np.float64)
    if sr != SR: x = np.stack([librosa.resample(x[:, c], orig_sr=sr, target_sr=SR) for c in range(2)], 1)
    peak = np.max(np.abs(x)) + 1e-9
    on = int(np.argmax(np.max(np.abs(x), 1) > peak * .03))   # the hit itself, 2 ms before it
    return x[max(0, on - int(.002 * SR)):] / peak


def measured_pitch(x, named):
    """The sample's own pitch (midi, fractional): the strongest partial within a semitone of where its name puts it
    (VCSL and VSCO name their files an octave low: C4 sounds C5; the kalimbas are not equal-tempered)."""
    seg = x[int(.02 * SR):int(.02 * SR) + 16384].mean(1)
    seg = np.pad(seg, (0, max(0, 16384 - len(seg))))
    N = 1 << 17
    S = np.abs(np.fft.rfft(seg * np.hanning(len(seg)), N)); f = np.fft.rfftfreq(N, 1 / SR)
    want = librosa.midi_to_hz(named + 12)
    band = (f > want * 2 ** (-1 / 12)) & (f < want * 2 ** (1 / 12))
    if not band.any() or S[band].max() < S.max() * .05: return named + 12.0
    return float(librosa.hz_to_midi(f[band][np.argmax(S[band])]))


class Instrument:
    def __init__(self, glob_pat, name_re):
        self.notes = {}
        for f in sorted(glob.glob(os.path.join(SRC, glob_pat))):
            g = re.search(name_re, os.path.basename(f))
            if g: self.notes.setdefault(m(g.group(1)), f)
        assert self.notes, glob_pat
        self.cache = {}

    def sample(self, named):
        if named not in self.cache:
            x = load(self.notes[named]); self.cache[named] = (x, measured_pitch(x, named))
        return self.cache[named]

    def note(self, n, length=None):
        # the nearest sample by its sounding pitch, preferring one above (shifting down sounds more natural)
        best = min(self.notes, key=lambda k: (abs(k + 12 - n), k + 12 < n))
        x, pitch = self.sample(best)
        ratio = 2 ** ((n - pitch) / 12)
        if abs(ratio - 1) > 1e-4:
            t = np.arange(int(len(x) / ratio)) * ratio
            x = np.stack([np.interp(t, np.arange(len(x)), x[:, c]) for c in range(2)], 1)
        if length:
            k = int(length * SR); x = x[:k].copy(); fade = min(len(x), int(.08 * SR))
            x[-fade:] *= np.linspace(1, 0, fade)[:, None] ** 2
        return x


class Hits:
    def __init__(self, glob_pat):
        self.files = sorted(glob.glob(os.path.join(SRC, glob_pat))); assert self.files, glob_pat
        self.cache = {}; self.i = 0

    def hit(self):
        f = self.files[self.i % len(self.files)]; self.i += 1
        if f not in self.cache: self.cache[f] = load(f)
        return self.cache[f]


def pad(chords, samples, vel=1.0):
    """A soft synth pad: three detuned saws per note through a gentle low-pass, slow in and out (ours)."""
    t = np.arange(samples) / SR
    out = np.zeros((len(t), 2))
    for a, b, notes in chords:
        i0, i1 = int(a * SR), min(len(t), int(b * SR) + int(.12 * SR))
        tt = t[i0:i1] - a
        env = np.minimum(1, tt / .12) * np.clip((b + .12 - a - tt) / .25, 0, 1)
        for n in notes:
            f0 = librosa.midi_to_hz(n + 12)
            for det, ch in ((-.07, 0), (.07, 1), (0, None)):
                f = f0 * 2 ** (det / 12)
                ph = rng.random() * 2 * np.pi
                saw = 2 * ((f * tt + ph / (2 * np.pi)) % 1) - 1
                if ch is None: out[i0:i1] += (saw * env)[:, None] * .5
                else: out[i0:i1, ch] += saw * env
    from scipy.signal import butter, sosfilt
    out = sosfilt(butter(2, 1400, fs=SR, output='sos'), out, axis=0)
    return out / (np.max(np.abs(out)) + 1e-9) * vel


def attack(x):
    """Where a note is heard to start: its 5 ms envelope reaching half its peak (a bowed or blown note rises for
    10-20 ms; a mallet's at once)."""
    w = int(.005 * SR); e = np.sqrt(np.convolve(np.mean(x[:int(.25 * SR)] ** 2, 1), np.ones(w) / w, mode='same'))
    return int(np.argmax(e > .5 * e.max()))


def place(buf, x, t, gain):
    i = int(round(t * SR)) - attack(x)   # the note heard to start on t
    if i < 0: x = x[-i:]; i = 0
    n = min(len(x), len(buf) - i)
    if n > 0: buf[i:i + n] += x[:n] * gain


def render(tone, score, length):
    buf = np.zeros((int(length * SR) + SR, 2))
    for t, n, v, part in score:
        g = v ** 1.6
        if tone == 'marimba':
            if part == 'mel': place(buf, MARIMBA.note(n + 12), t, g)
            elif part == 'bass': place(buf, MARIMBA.note(n + 12), t, .8 * g)
            else: place(buf, GLOCK.note(n + 24), t, .32 * g)
        elif tone == 'pizz':
            if part == 'mel': place(buf, VIOLINS.note(n + 12), t, g)
            elif part == 'bass': place(buf, CELLOS.note(n + 12), t, .9 * g)
            elif t in (0.0, B[4], B[6]) or abs(t - (B[4] - (CUT1 - CUT0))) < 1e-6 or abs(t - (B[6] - (CUT1 - CUT0))) < 1e-6:
                place(buf, CHIMES.note(n + 12, 1.2), t, .3 * g)   # the bell on the first beat, the strongest and the last
        elif tone == 'ksilofon':
            if part == 'mel': place(buf, XYLO.note(n + 24), t, .85 * g)
            elif part == 'bass': place(buf, MARIMBA.note(n + 12), t, .55 * g)
            else: place(buf, CLAPS.hit(), t, .5 * g)
        elif tone == 'kutu':
            if part == 'mel': place(buf, KALIMBA.note(n + 12), t, .8 * g); place(buf, GLOCK.note(n + 24), t, .45 * g)
            elif part == 'bass': place(buf, KALIMBA.note(n + 12), t, .4 * g)
        elif tone == 'bakir':
            if part == 'mel': place(buf, TRUMPET.note(n + 12, .45), t, g)
            elif part == 'bass': place(buf, TUBA.note(n, .45), t, .8 * g)
            elif t == 0.0 or t == score[-1][0] or abs(t - B[4]) < 1e-6 or abs(t - (B[4] - (CUT1 - CUT0))) < 1e-6:
                place(buf, TIMPANI.hit()[:int(.9 * SR)], t, .3 * g)
    if tone == 'kutu':
        sh = length < FULL_LEN
        chords = [(a if a < CUT0 or not sh else a - (CUT1 - CUT0), b if b < CUT0 or not sh else b - (CUT1 - CUT0), ns)
                  for a, b, ns in CHORDS if not (sh and CUT0 <= a < CUT1)]
        if sh: chords = [(a, min(b, CUT0) if a < CUT0 else b, ns) for a, b, ns in chords]
        p = pad(chords, len(buf))
        buf += p * .16 * np.max(np.abs(buf))
    buf = buf[:int(length * SR)]
    n = int(.3 * SR); buf[-n:] *= np.linspace(1, 0, n)[:, None] ** 2
    return buf


# the beat map tool's own functions (trim, beats_of, level_to): the same beats and the same loudness rule
_src = open('Tools/make_beatmap96.py', encoding='utf8').read().split('os.makedirs(WEB')[0]
_ns = {'__name__': 'beatmap', 'sys': type('S', (), {'argv': ['', '']})}
exec(compile(_src.replace('NEW = sys.argv[1]', 'NEW = ""'), 'make_beatmap96', 'exec'), _ns)
_ns['MAX_LENGTH'] = 3.0   # b is 2.54 s: no fade-out at 2.5 s here
beats_of, level_to, read_b = _ns['beats_of'], _ns['level_to'], _ns['read']


def onsets(x):
    """Every attack (librosa spectral flux, each sharpened to the steepest rise of the 1 ms envelope, as beats_of)."""
    y = x.mean(1); padn = int(.3 * SR); y = np.concatenate([np.zeros(padn), y, np.zeros(padn)]); hop = 128
    env = librosa.onset.onset_strength(y=y, sr=SR, hop_length=hop, n_fft=1024)
    frames = librosa.onset.onset_detect(onset_envelope=env, sr=SR, hop_length=hop, delta=.04, wait=int(.03 * SR / hop))
    w = SR // 1000; db = 20 * np.log10(np.sqrt(np.convolve(y ** 2, np.ones(w) / w, mode='same')[::w] + 1e-12))
    rise = np.diff(db, prepend=db[0]); out = []
    for f in frames:
        t = f * hop / SR; lo, hi = max(1, int((t - .03) * 1000)), min(len(rise) - 1, int((t + .01) * 1000))
        k = lo + int(np.argmax(rise[lo:hi])) if hi > lo else int(t * 1000)
        out.append(max(0.0, (k - 1) / 1000 - padn / SR))
    return np.array(out)

MARIMBA = Instrument('*Marimba__Marimba_hit_Outrigger_*', r'Outrigger_([A-G]#?\d)_')
GLOCK = Instrument('*Glockenspiel__glock_soft_*', r'soft_([A-G]#?\d)_')
XYLO = Instrument('*Xylophone__*Xylo_Medium_*', r'Medium_([A-G]#?\d)_')
CLAPS = Hits('*Claps__Clap_rr*')
KALIMBA = Instrument('*Kalimba, Kenya__*', r'MainSpirit_([A-G]#?\d)_')
VIOLINS = Instrument('*Violin Section__Pizz__*', r'Pizz_([A-G]#?\d)_')
CELLOS = Instrument('*Cello Section__pizzT__*', r'pizzT_([A-G]#?\d)_')
CHIMES = Instrument('*Hand Chimes__sus_*', r'sus_([A-G]#?\d)_')
TRUMPET = Instrument('*Trumpet__stac__*', r'stac_([A-G]#?\d)_')
TUBA = Instrument('*Tuba__stac__*', r'stac_([A-G]#?\d)_')
TIMPANI = Hits('*Timpani 1__Hit__*')
TONES = ['marimba', 'pizz', 'ksilofon', 'kutu', 'bakir']

if __name__ == '__main__':
    os.makedirs(WEB, exist_ok=True); os.makedirs(PAGE, exist_ok=True)
    b = read_b(os.path.join(WEB, 'kazanma-b.wav'))
    meter = pyln.Meter(SR, block_size=.2)
    target = meter.integrated_loudness(b)
    report = {'target_lufs': round(target, 2), 'b': B, 'b5': [0.0, .525, .782, 1.128, 1.284], 'tones': {}}
    for tone in TONES:
        rec = {}
        for key, score, length, ref in (('b', SCORE, FULL_LEN, report['b']), ('b5', short(SCORE), FULL_LEN - (CUT1 - CUT0), report['b5'])):
            x = level_to(render(tone, score, length), target)
            x = x[:int(round((2.54 if key == 'b' else 1.915) * SR))]
            sf.write(os.path.join(WEB, f'kazanma-{key}-{tone}.wav'), x, SR, subtype='PCM_16')
            if key == 'b5': sf.write(os.path.join(PAGE, f'kazanma-{tone}.wav'), x, SR, subtype='PCM_16')
            on = onsets(x)   # each of b's beats: the nearest attack in this one (ms)
            dev = [round(1000 * float(on[np.argmin(np.abs(on - r))] - r), 1) for r in ref]
            found = [round(float(on[np.argmin(np.abs(on - r))]), 4) for r in ref]
            rec[key] = dict(length=round(len(x) / SR, 3), lufs=round(meter.integrated_loudness(x), 2),
                            peak=round(20 * np.log10(np.max(np.abs(x))), 2), beats=found, dev_ms=dev)
            print(f'{tone:9s} {key:3s} {len(x) / SR:.3f} s {rec[key]["lufs"]:6.2f} LUFS peak {rec[key]["peak"]:5.1f}  beat attacks off by (ms) {dev}')
        report['tones'][tone] = rec
    with open('docs/art/pm100/fanfar100.json', 'w', encoding='utf8') as f:
        json.dump(report, f, indent=1)
