"""PM_96 §5: measures the win screen against the fanfare in a recording of Tools/pm96_record.js.
The sound: the fanfare is found in the recorded sound (cross-correlation with its WAV), its beats are its beat map's
(Resources/Art/audio/pm96/vurus_haritasi.json) from there. The picture: the frames at their own times (the recording's
timestamps), each event the first frame its place on the screen changes (the light: the screen brightens; the title;
each Jiggle; the confetti over Jigo; the +50 gold). Prints each event's distance to its beat (+: after it) and writes
the delivery video: 60 fps, with its sound, at most 10 MB.
    python -I Tools/pm96_measure.py <recording dir> <fanfare letter> <out.mp4>      (imageio-ffmpeg, numpy, scipy, soundfile)"""
import json, os, re, subprocess, sys
import numpy as np, soundfile as sf, imageio_ffmpeg
from scipy.signal import fftconvolve

REC, LETTER, OUT = sys.argv[1], sys.argv[2], sys.argv[3]
FF = imageio_ffmpeg.get_ffmpeg_exe()
SR = 44100
webm = os.path.join(REC, 'kayit.webm')
fanmap = {f['letter']: f for f in json.load(open('Assets/_Game/Presentation/Resources/Art/audio/pm96/vurus_haritasi.json', encoding='utf8'))['fanfares']}[LETTER]

# the sound and where the fanfare starts in it
wav = os.path.join(REC, 'ses.wav')
subprocess.run([FF, '-y', '-v', 'error', '-i', webm, '-vn', '-ac', '1', '-ar', str(SR), wav], check=True)
rec, _ = sf.read(wav)
fan, fsr = sf.read(f'Assets/Audio/Fanfare96/kazanma-{LETTER}.wav')
fan = fan.mean(1) if fan.ndim > 1 else fan
corr = fftconvolve(rec, fan[::-1], mode='valid')
start = int(np.argmax(corr)) / SR
norm = float(corr.max() / (np.linalg.norm(fan) * np.linalg.norm(rec[int(start * SR):int(start * SR) + len(fan)]) + 1e-9))
beats = [start + b for b in fanmap['beats']]

# the frames at their own times (half size: 1 px = 1 pt of the 390 x 844 screen)
W, H = 390, 844
proc = subprocess.run([FF, '-v', 'info', '-i', webm, '-an', '-fps_mode', 'passthrough', '-vf', f'scale={W}:{H},showinfo',
                       '-f', 'rawvideo', '-pix_fmt', 'rgb24', '-'], capture_output=True)
pts = [float(x) for x in re.findall(r'pts_time:([0-9.]+)', proc.stderr.decode('utf8', 'ignore'))]
frames = np.frombuffer(proc.stdout, np.uint8).reshape(-1, H, W, 3).astype(np.float32)
n = min(len(pts), len(frames)); pts = np.array(pts[:n]); frames = frames[:n]
dt = float(np.median(np.diff(pts)))


def region(x0, y0, x1, y1): return frames[:, y0:y1, x0:x1].mean(axis=3)


def first_change(r, around, threshold, brighter=False):
    """The first frame from 250 ms before the beat on whose region changes by more than `threshold` (mean of |diff|),
    or (brighter) gets brighter by more than it."""
    d = np.diff(r, axis=0)
    score = d.mean(axis=(1, 2)) if brighter else np.abs(d).mean(axis=(1, 2))
    for i in range(len(score)):
        t = pts[i + 1]
        if t < around - .25: continue
        if t > around + .25: break
        if score[i] > threshold: return t
    return None


events = [('ışık', 0, (20, 250, 370, 330), 2, True), ('başlık', 1, (60, 112, 330, 205), 5, False)]
plan = fanmap['beats']; strong = fanmap['strong']; last = len(plan) - 1
free = [i for i in range(2, last) if i != strong]
spots = [(58, 488), (332, 488), (112, 514), (278, 514)]
for j in range(min(4, len(free))):   # one Jiggle per free beat (P21WinView.LayoutJiggles)
    events.append((f'Jiggle {j + 1}', free[j], (spots[j][0] - 18, spots[j][1] - 30, spots[j][0] + 18, spots[j][1] + 30), 3, False))
events += [('konfeti + Jigo', strong, (120, 280, 270, 350), 5, False), ('altın +50', last, (150, 546, 240, 575), 5, False)]

print(f'fanfar {LETTER}: kayıtta {start:.3f} sn (benzerlik {norm:.2f}), kare aralığı {dt * 1000:.1f} ms ({1 / dt:.0f} fps), {n} kare')
rows, worst = [], 0
for name, beat, box, thr, brighter in events:
    b = beats[beat]
    t = first_change(region(*box), b, thr, brighter)
    if t is None: rows.append((name, beat + 1, b, None)); print(f'  {name:15s} vuruş {beat + 1} ({b - start:.3f}): bulunamadı'); continue
    worst = max(worst, abs(t - b))
    rows.append((name, beat + 1, b, t))
    print(f'  {name:15s} vuruş {beat + 1} ({b - start:.3f} sn): ilk kare {(t - b) * 1000:+6.1f} ms')
print(f'en büyük uzaklık {worst * 1000:.1f} ms (1 kare = {dt * 1000:.1f} ms)')
json.dump(dict(letter=LETTER, start=start, similarity=norm, frame_ms=dt * 1000,
               events=[dict(event=a, beat=b, beat_time=round(c - start, 4), first_frame_ms=None if d is None else round((d - c) * 1000, 1)) for a, b, c, d in rows]),
          open(os.path.join(REC, 'olcum.json'), 'w', encoding='utf8'), indent=1, ensure_ascii=False)

# the delivery: from a second before the fanfare to the lobby's music back, 60 fps, with its sound
a, b = max(0, start - 1.2), start + fanmap['length'] + 1.6
for crf in (20, 24, 28):
    subprocess.run([FF, '-y', '-v', 'error', '-ss', f'{a:.3f}', '-to', f'{b:.3f}', '-i', webm, '-vf', f'scale={W}:{H},fps=60',
                    '-c:v', 'libx264', '-crf', str(crf), '-preset', 'slow', '-pix_fmt', 'yuv420p', '-c:a', 'aac', '-b:a', '160k',
                    '-movflags', '+faststart', OUT], check=True)
    if os.path.getsize(OUT) <= 10e6: break
print(f'video {OUT}: {os.path.getsize(OUT) / 1e6:.2f} MB')
