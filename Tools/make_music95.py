"""PM_95 §2: eight new background music candidates (&muzik=f..m), all CC0 from OpenGameArt (docs/art/pm95/LISANS.md).
Each becomes a seamless loop:
  whole: made to loop by its composer; the loop is the file, cut to a whole number of beats at its tempo.
  fold:  a track that repeats itself but was not cut as a loop. The loop is one period of it, from `start` for
         `length` seconds (found by Tools/pm95_loopfind.py, refined here to the sample by the waveform), and the
         music right after the period is crossfaded under the loop's first 40 ms, so the seam carries on as the
         track itself carries on (no click, no jump).
Loudness: -18 LUFS integrated (ITU-R BS.1770, pyloudnorm), the peak kept under -1 dBFS (a gentle limiter on the few
transients over it); the game plays music at .35 of
that, under the effects (the old loops are -19.4 LUFS).
Out: Assets/Audio/Music95/music-X.mp3 (web, 128 kbps) + .ogg (iOS) and Assets/Audio/Music95/loops.json, which gives
each MP3's loop start (the encoder's lead-in, measured by decoding the MP3 back) and length in seconds.
    python -I Tools/make_music95.py <downloaded files dir>      (lameenc, soundfile, numpy, pyloudnorm)"""
import json, os, sys
import lameenc, numpy as np, soundfile as sf, pyloudnorm as pyln

SRC = sys.argv[1]
OUT = 'Assets/Audio/Music95'
TRACKS = [   # letter, file, tempo, how, (start s, length s) for fold
    ('f', 'apple_cider_loop.wav', 100, 'whole', None),
    ('g', 'magic_puzzle_title_bpm100_0.ogg', 100, 'whole', None),
    ('h', 'magic_puzzle_in-game_1_bpm110_0.ogg', 110, 'whole', None),
    ('i', 'cozy_puzzle_stage_select_bpm100_0.ogg', 100, 'whole', None),
    ('j', 'magic_puzzle_in-game_2_bpm120_0.ogg', 120, 'whole', None),
    ('k', 'Quaint_Town_-_LOOPABLE.wav', None, 'fold', (0.0, 82.3)),
    ('l', 'little_cafe_0.wav', 100, 'fold', (23.2, 96.0)),
    ('m', 'Lovely_town_0.ogg', None, 'fold', (52.4, 57.2)),
]
TARGET_LUFS = -18.0
PEAK = 10 ** (-1 / 20)
FADE = .04


def refine(mono, sr, start, length):
    """The period to the sample: the lag (within 0.3 s of `length`) at which the 3 s after the loop's end best
    matches the 3 s after its start (waveform correlation, normalised)."""
    a = int(start * sr) + sr // 10
    ref = mono[a:a + 3 * sr]
    best, lag = -2.0, int(length * sr)
    for d in range(-int(.3 * sr), int(.3 * sr) + 1, 8):
        b = a + int(length * sr) + d
        seg = mono[b:b + 3 * sr]
        if len(seg) < len(ref): break
        c = float(np.dot(ref, seg) / (np.linalg.norm(ref) * np.linalg.norm(seg) + 1e-9))
        if c > best: best, lag = c, int(length * sr) + d
    for d in range(-8, 9):   # then sample by sample
        b = a + lag + d
        seg = mono[b:b + 3 * sr]
        c = float(np.dot(ref, seg) / (np.linalg.norm(ref) * np.linalg.norm(seg) + 1e-9))
        if c > best: best, lag = c, lag + d
    return lag, best


def loop_of(x, sr, bpm, how, at):
    if how == 'whole':
        n = int(round(len(x) / (sr * 60 / bpm))) * sr * 60 / bpm   # a whole number of beats
        return x[:int(round(n))].copy(), f'whole, {n / sr / (60 / bpm):.0f} beats'
    mono = x.mean(axis=1)
    start, length = at
    s = int(start * sr)
    lag, corr = refine(mono, sr, start, length)
    y = x[s:s + lag].copy()
    f = int(FADE * sr)
    t = np.linspace(0, np.pi / 2, f)[:, None]
    after = x[s + lag:s + lag + f]   # what the track plays right after one period
    y[:f] = y[:f] * np.sin(t) + after * np.cos(t)   # equal power: the seam begins as the track goes on
    return y, f'fold at {start:.1f} s, period {lag / sr:.3f} s (waveform match {corr:.2f})'


def limit(y, sr):
    """A gentle peak limiter for the few transients over -1 dBFS after the gain: a gain per 5 ms block, held over
    the blocks around it (the attack before the peak) and eased back over 50 ms; the loop wraps (the seam too)."""
    blk = sr // 200
    n = -(-len(y) // blk)
    pad = np.concatenate([y, y[:n * blk - len(y)]]) if n * blk > len(y) else y
    pk = np.abs(pad).max(axis=1).reshape(n, blk).max(axis=1)
    g = np.minimum(1, PEAK / np.maximum(pk, 1e-9))
    g = np.min(np.stack([np.roll(g, k) for k in (-2, -1, 0, 1, 2)]), axis=0)   # held: no block next to a peak loud
    for _ in range(2):                                                             # eased back (one-pole, 50 ms), both ways
        for order in (1, -1):
            idx = range(n) if order == 1 else range(n - 1, -1, -1)
            prev = 1.0
            out = np.empty(n)
            for i in idx:
                prev = min(g[i], prev + (1 - prev) * .1); out[i] = prev
            g = out
    gain = np.interp(np.arange(len(y)), np.arange(n) * blk + blk / 2, g)
    return y * gain[:, None].astype(y.dtype), int((pk > PEAK).sum())


def seam(x):
    """The click test: the sample step across the seam against the track's usual step."""
    step = float(np.abs(x[0] - x[-1]).max())
    usual = float(np.percentile(np.abs(np.diff(x, axis=0)).max(axis=1), 99.9))
    return step, usual


def main():
    os.makedirs(OUT, exist_ok=True)
    loops = {}
    for letter, f, bpm, how, at in TRACKS:
        x, sr = sf.read(os.path.join(SRC, f), dtype='float32', always_2d=True)
        if x.shape[1] == 1: x = np.concatenate([x, x], 1)
        assert sr == 44100, (f, sr)
        y, note = loop_of(x, sr, bpm, how, at)
        meter = pyln.Meter(sr)
        lufs = meter.integrated_loudness(np.concatenate([y, y]))   # as it plays: round the seam
        y *= 10 ** ((TARGET_LUFS - lufs) / 20)
        y, limited = limit(y, sr)
        peak = float(np.abs(y).max())
        if peak > PEAK: y *= PEAK / peak
        after = meter.integrated_loudness(np.concatenate([y, y]))
        pcm = (np.clip(y, -1, 1) * 32767).astype('<i2')
        enc = lameenc.Encoder(); enc.set_bit_rate(128); enc.set_in_sample_rate(sr); enc.set_channels(2); enc.set_quality(2)
        mp3 = enc.encode(pcm.tobytes()) + enc.flush()
        path = f'{OUT}/music-{letter}.mp3'
        open(path, 'wb').write(mp3)
        with sf.SoundFile(f'{OUT}/music-{letter}.ogg', 'w', sr, 2, format='OGG', subtype='VORBIS', compression_level=.6) as o:
            for i in range(0, len(y), sr): o.write(y[i:i + sr])
        d, dsr = sf.read(path, dtype='float32')
        ref = y[:sr // 2].mean(axis=1); dec = d[:sr].mean(axis=1)
        lead = int(np.argmax(np.correlate(dec, ref, 'valid')))
        step, usual = seam(y)
        loops[letter] = {'start': lead / dsr, 'length': len(y) / sr, 'bpm': bpm, 'lufs': round(after, 1), 'file': f}
        print(f'{letter}: {f} {len(y) / sr:.2f} s ({note}), {lufs:.1f} -> {after:.1f} LUFS, peak {min(peak, PEAK):.2f} ({limited} limited 5 ms blocks), '
              f'mp3 {len(mp3) / 1e6:.2f} MB, lead-in {lead}, seam step {step:.4f} (99.9 % of steps under {usual:.4f})')
    json.dump(loops, open(f'{OUT}/loops.json', 'w'), indent=1)


main()
