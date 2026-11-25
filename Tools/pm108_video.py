"""PM_108: the preview MP4s with sound, from Tools/pm108_dalis.js captures (frames + times.json on the page's clock,
efektler.webm / muzik.webm with meta.json's start on the same clock, log.txt for the dive's start: its first [cerceve]
line). A constant 60 fps (each tick the newest frame captured by then), 390 x 844 a side, H.264 + AAC.
  one capture   the dive with its own sound
  two captures  side by side (left | right, labelled), played twice: first with the left's sound, then the right's
                (the side heard is marked "SES")
usage: python -I Tools/pm108_video.py <out.mp4> <from> <to> <capture dir> [<capture dir>] [--labels ŞİMDİKİ,YENİ]
  from / to: seconds from each capture's dive start."""
import glob, json, os, re, subprocess, sys, tempfile
import numpy as np
import imageio_ffmpeg
from PIL import Image, ImageDraw, ImageFont

argv = sys.argv[1:]
labels = ['ŞİMDİKİ', 'YENİ']
if '--labels' in argv:
    i = argv.index('--labels'); labels = argv[i + 1].split(','); del argv[i:i + 2]
out, a, b, caps = argv[0], float(argv[1]), float(argv[2]), argv[3:]
W, H, FPS, SR = 390, 844, 60, 48000
FF = imageio_ffmpeg.get_ffmpeg_exe()


def decode(f):
    """a webm to float32 stereo at SR"""
    if not os.path.exists(f) or os.path.getsize(f) < 100: return np.zeros((0, 2), np.float32)
    raw = subprocess.run([FF, '-loglevel', 'error', '-i', f, '-f', 'f32le', '-ac', '2', '-ar', str(SR), '-'], capture_output=True).stdout
    return np.frombuffer(raw, np.float32).reshape(-1, 2)


def load(d):
    start = None
    for line in open(os.path.join(d, 'log.txt'), encoding='utf8', errors='replace'):
        m = re.match(r'^([\d.]+) \[cerceve\] 0[.,]000 ', line)
        if m: start = float(m.group(1)) / 1000; break
    if start is None: sys.exit(f'no dive start in {d}')
    times = json.load(open(os.path.join(d, 'times.json'))); files = sorted(glob.glob(os.path.join(d, 'f*.jpg')))
    n = min(len(times), len(files))
    meta = json.load(open(os.path.join(d, 'meta.json')))
    rec0 = meta['epoch'] / 1000
    fx, mu = decode(os.path.join(d, 'efektler.webm')), decode(os.path.join(d, 'muzik.webm'))
    # the sound of [start + a, start + b] on the page's clock (the 25 Hz marker hum taken out: a high-pass at 40 Hz)
    L = int((b - a) * SR); mix = np.zeros((L, 2), np.float32)
    for sig, at in ((fx, rec0), (mu, rec0 + meta.get('musicStart', 0))):
        off = int(round((start + a - at) * SR))
        s0, d0 = max(0, off), max(0, -off)
        k = min(len(sig) - s0, L - d0)
        if k > 0: mix[d0:d0 + k] += sig[s0:s0 + k]
    rc = 1 / (2 * np.pi * 40); al = rc / (rc + 1 / SR); y = np.zeros_like(mix); prev_x = np.zeros(2); prev_y = np.zeros(2)
    for i in range(L):   # one-pole high-pass
        prev_y = al * (prev_y + mix[i] - prev_x); prev_x = mix[i]; y[i] = prev_y
    return start, times[:n], files[:n], y


clips = [load(d) for d in caps]
font = ImageFont.truetype('C:/Windows/Fonts/arialbd.ttf', 22)
small = ImageFont.truetype('C:/Windows/Fonts/arialbd.ttf', 18)
ticks = int((b - a) * FPS)
passes = [0, 1] if len(clips) == 2 else [0]
tmp = tempfile.mkdtemp(prefix='pm108v-')
wav = os.path.join(tmp, 'ses.wav')
audio = np.concatenate([clips[p][3] for p in passes])
pcm = (np.clip(audio, -1, 1) * 32767).astype('<i2')
import wave
with wave.open(wav, 'wb') as w:
    w.setnchannels(2); w.setsampwidth(2); w.setframerate(SR); w.writeframes(pcm.tobytes())
cmd = [FF, '-y', '-loglevel', 'error', '-f', 'rawvideo', '-pix_fmt', 'rgb24', '-s', f'{W * len(clips)}x{H}', '-r', str(FPS), '-i', '-',
       '-i', wav, '-c:v', 'libx264', '-preset', 'slow', '-crf', '23', '-pix_fmt', 'yuv420p', '-c:a', 'aac', '-b:a', '160k',
       '-shortest', '-movflags', '+faststart', out]
p = subprocess.Popen(cmd, stdin=subprocess.PIPE)
for heard in passes:
    idx = [0] * len(clips); cache = {}
    for k in range(ticks):
        t = a + k / FPS
        frame = Image.new('RGB', (W * len(clips), H))
        for c, (s, times, files, _) in enumerate(clips):
            at = s + t
            while idx[c] + 1 < len(times) and times[idx[c] + 1] <= at: idx[c] += 1
            f = files[idx[c]]
            if f not in cache: cache[f] = Image.open(f).convert('RGB').resize((W, H), Image.LANCZOS)
            frame.paste(cache[f], (c * W, 0))
        if len(cache) > 8: cache.clear()
        d = ImageDraw.Draw(frame, 'RGBA')
        if len(clips) == 2:
            for c, lab in enumerate(labels):
                x = c * W + W // 2; tw = d.textlength(lab, font=font)
                d.rounded_rectangle((x - tw / 2 - 10, 54, x + tw / 2 + 10, 84), 8, fill=(0, 0, 0, 150))
                d.text((x - tw / 2, 57), lab, font=font, fill='white')
                if c == heard:
                    tw2 = d.textlength('SES', font=small)
                    d.rounded_rectangle((x - tw2 / 2 - 8, 90, x + tw2 / 2 + 8, 114), 7, fill=(227, 181, 78, 230))
                    d.text((x - tw2 / 2, 92), 'SES', font=small, fill=(14, 34, 68))
            d.rectangle((W - 1, 0, W, H), fill=(255, 255, 255, 255))
        clock = f'{max(0.0, t):.2f} s'
        d.text((W * len(clips) - d.textlength(clock, font=small) - 10, H - 30), clock, font=small, fill=(255, 255, 255, 200))
        p.stdin.write(frame.tobytes())
p.stdin.close(); p.wait()
print(f'[video] {out}: {ticks * len(passes)} frames, {ticks * len(passes) / FPS:.2f} s, {W * len(clips)}x{H}, {os.path.getsize(out) / 1e6:.2f} MB')
