"""PM_100 §2: measures Tools/pm100_record.js's recording of the game's own effects: for every move, the sounds after
its release (onset, end, loudest 400 ms, spectral centroid); for a jam that slides, whether the lift-off is over before
the knock starts. Writes <dir>/olcum.json and prints a line per move.
    python -I Tools/pm100_measure.py <dir>          (imageio-ffmpeg's ffmpeg; numpy, soundfile, librosa)"""
import json, os, subprocess, sys
import numpy as np, soundfile as sf, librosa

d = sys.argv[1]
wav = os.path.join(d, 'efektler.wav')
import imageio_ffmpeg
subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), '-y', '-loglevel', 'error', '-i', os.path.join(d, 'efektler.webm'), '-af', 'aresample=async=1:first_pts=0', '-ac', '1', '-ar', '44100', wav], check=True)
y, SR = sf.read(wav); y = y.astype(float)
from scipy.signal import butter, sosfilt
y = sosfilt(butter(4, 70, 'hp', fs=SR, output='sos'), y)   # the recorder's 25 Hz hum out
moves = json.load(open(os.path.join(d, 'moves.json')))['moves']
w = int(.002 * SR)
env = np.sqrt(np.convolve(y ** 2, np.ones(w) / w, 'same'))
floor = np.percentile(env, 20) + 1e-5


def events(a, b):
    """Sounds in [a, b] s: runs of the 2 ms envelope above 1/30 of the window's peak (gaps under 8 ms joined)."""
    i0, i1 = int(a * SR), min(len(y), int(b * SR)); e = env[i0:i1]
    if i1 - i0 < 100 or e.max() < floor * 4: return []
    on = e > max(e.max() / 30, floor * 3)
    out, i = [], 0
    while i < len(on):
        if not on[i]: i += 1; continue
        j = i
        while j < len(on) and (on[j] or np.any(on[j:j + int(.008 * SR)])): j += 1
        if j - i > int(.004 * SR): out.append((i0 + i, i0 + j))
        i = j
    return out


res = []
for m in moves:
    ev = events(m['at'], m['release'] + .9)   # an exit sounds as the piece crosses the edge, before the release
    parts = []
    for a, b in ev:
        seg = y[a:b]
        c = float(librosa.feature.spectral_centroid(y=seg + 1e-9, sr=SR, n_fft=512, hop_length=128).mean()) if len(seg) > 600 else float('nan')
        parts.append(dict(start=round(a / SR - m['at'], 3), ms=round(1000 * (b - a) / SR), peak_db=round(20 * np.log10(np.abs(seg).max() + 1e-9), 1), centroid=round(c)))
    r = dict(m, sounds=parts)
    if m['what'].startswith('jam') and len(parts) >= 2:
        r['gap_ms'] = round(1000 * (parts[1]['start'] - parts[0]['start']) - parts[0]['ms'])   # > 0: the lift-off over before the knock
    res.append(r)
    print(f"{m['piece']:2d} {m['dir']} {m['what']:4s}: " + ' | '.join(f"+{p['start'] * 1000:.0f} ms {p['ms']} ms {p['peak_db']} dB {p['centroid']} Hz" for p in parts) + (f"  ara {r['gap_ms']} ms" if 'gap_ms' in r else ''))
json.dump(res, open(os.path.join(d, 'olcum.json'), 'w'), indent=1)
