"""PM_86 §5: the five background music candidates (&muzik=a..e), all CC0 from OpenGameArt (docs/art/pm86/MUZIK_LISANS.md).
Each becomes a seamless loop: the four Cozy Puzzle tracks are whole bars already (their length is exactly N beats at
their tempo), Feel Good Island has a lead-in and a 3 s reverb tail, which is folded back over its start (the loop is
the music up to its last bar, the tail sounds under the first bar as it would on a repeat). The seam is checked for a click: the sample step
across it against the track's own steps. Loudness matched to the old music (RMS .085), so the game's music volume stays under the effects.
Out: Assets/Audio/Music86/music-X.mp3 (web, 128 kbps) + .ogg (iOS) and Assets/Audio/Music86/loops.json, which gives
each MP3's loop start and length in seconds (the encoder's lead-in measured by decoding the MP3 back).
    python -I Tools/make_music86.py <downloaded files dir>      (lameenc, soundfile, numpy)"""
import json, os, sys
import lameenc, numpy as np, soundfile as sf

SRC = sys.argv[1]
OUT = 'Assets/Audio/Music86'
TRACKS = [   # letter, file, tempo (beats per minute), how it loops
    ('a', 'cozy_puzzle_in-game_3_bpm108_0.ogg', 108, 'whole'),
    ('b', 'cozy_puzzle_title_bpm95_0.ogg', 95, 'whole'),
    ('c', 'cozy_puzzle_in-game_2_bpm90_0.ogg', 90, 'whole'),
    ('d', 'cozy_puzzle_in-game_1_bpm118_0.ogg', 118, 'whole'),
    ('e', 'island_0_0.ogg', None, 'fold'),
]
TARGET_RMS = .085


def tempo(mono, sr):
    """Beats per minute from the energy envelope's autocorrelation (70..140)."""
    hop = 512
    env = np.array([np.sqrt((mono[i:i + hop] ** 2).mean()) for i in range(0, len(mono) - hop, hop)])
    on = np.maximum(0, np.diff(env))
    on -= on.mean()
    ac = np.correlate(on, on, 'full')[len(on) - 1:]
    lags = np.arange(len(ac)) * hop / sr
    ok = (lags >= 60 / 140) & (lags <= 60 / 70)
    lag = lags[ok][np.argmax(ac[ok])]
    return 60 / lag


def loop_of(x, sr, bpm, how):
    mono = x.mean(axis=1)
    if how == 'whole':
        n = int(round(len(x) / (sr * 60 / bpm))) * sr * 60 / bpm   # a whole number of beats
        return x[:int(round(n))], bpm
    a = np.abs(mono)
    start = int(np.argmax(a > .003))
    end = len(a) - int(np.argmax(a[::-1] > .003))
    bpm = tempo(mono[start:], sr)
    bar = 4 * 60 / bpm * sr
    music = (end - start) - 2.5 * sr                    # the music before the reverb tail (about 3 s)
    length = int(round(round(music / bar) * bar))
    body = x[start:start + length].copy()
    tail = x[start + length:end]
    k = min(len(tail), len(body))
    body[:k] += tail[:k]                                # the tail sounds under the first bar, as on a repeat
    return body, bpm


def seam(x):
    """The loop is left whole (bar-exact); the click test: the sample step across the seam against the usual step."""
    step = float(np.abs(x[0] - x[-1]).max())
    usual = float(np.percentile(np.abs(np.diff(x, axis=0)).max(axis=1), 99.9))
    return step, usual


def main():
    os.makedirs(OUT, exist_ok=True)
    loops = {}
    for letter, f, bpm, how in TRACKS:
        x, sr = sf.read(os.path.join(SRC, f), dtype='float32')
        if x.ndim == 1: x = np.stack([x, x], 1)
        y, bpm = loop_of(x, sr, bpm, how)
        y *= TARGET_RMS / max(1e-6, float(np.sqrt((y ** 2).mean())))
        peak = float(np.abs(y).max())
        if peak > .98: y *= .98 / peak
        pcm = (np.clip(y, -1, 1) * 32767).astype('<i2')
        enc = lameenc.Encoder(); enc.set_bit_rate(128); enc.set_in_sample_rate(sr); enc.set_channels(2); enc.set_quality(2)
        mp3 = enc.encode(pcm.tobytes()) + enc.flush()
        path = f'{OUT}/music-{letter}.mp3'
        open(path, 'wb').write(mp3)
        with sf.SoundFile(f'{OUT}/music-{letter}.ogg', 'w', sr, 2, format='OGG', subtype='VORBIS', compression_level=.6) as o:
            for i in range(0, len(y), sr): o.write(y[i:i + sr])
        # the MP3's lead-in: where the decoded MP3 matches the loop's first second
        d, dsr = sf.read(path, dtype='float32')
        ref = y[:sr // 2].mean(axis=1); dec = d[:sr].mean(axis=1)
        corr = np.correlate(dec, ref, 'valid')
        lead = int(np.argmax(corr))
        step, usual = seam(y)
        loops[letter] = {'start': lead / dsr, 'length': len(y) / sr, 'bpm': round(bpm, 1), 'file': f}
        print(f'{letter}: {f} {len(y) / sr:.2f} s, {bpm:.1f} bpm, mp3 {len(mp3) / 1e6:.2f} MB, lead-in {lead} samples, '
              f'seam step {step:.4f} (99.9 % of steps under {usual:.4f}), peak {min(peak, .98):.2f}')
    json.dump(loops, open(f'{OUT}/loops.json', 'w'), indent=1)


main()
