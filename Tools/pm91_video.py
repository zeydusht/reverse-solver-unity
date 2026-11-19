"""PM_91 §4: preview MP4s from pm72_tour.js captures (frames f*.jpg + times.json, the page's clock), at a constant
60 fps (each tick shows the newest frame captured by then), 390 x 844 a side, H.264 -preset veryfast, no sound.
The ffmpeg is imageio-ffmpeg's.
usage: python Tools/pm91_video.py <out.mp4> <from> <to> <capture dir> <tour log> [<capture dir> <tour log> ...]
  from / to: seconds from the dive's start (its first [cerceve] line), or 'rec' / 'end' for the first / last frame.
  With two captures they are put side by side (left | right), each from its own dive start, labelled ESKİ | YENİ
  (or --labels A,B)."""
import glob, io, json, re, subprocess, sys
import imageio_ffmpeg
from PIL import Image, ImageDraw, ImageFont

out, t0s, t1s = sys.argv[1], sys.argv[2], sys.argv[3]
argv = sys.argv[:]
labels = ['ESKİ', 'YENİ']
if '--labels' in argv:   # PM_92: e.g. --labels PM_91,PM_92
    i = argv.index('--labels'); labels = argv[i + 1].split(','); del argv[i:i + 2]
caps = [(argv[i], argv[i + 1]) for i in range(4, len(argv), 2)]
W, H, FPS = 390, 844, 60
num = lambda s: float(s.replace(',', '.'))


def load(d, logf):
    start = None
    for line in open(logf, encoding='utf8', errors='replace'):
        m = re.match(r'^([\d.,]+) \[cerceve\] 0,000 ', line) or re.match(r'^([\d.,]+) \[cerceve\] 0\.000 ', line)
        if m: start = num(m.group(1)) / 1000; break
    times = json.load(open(d + '/times.json')); files = sorted(glob.glob(d + '/f*.jpg'))
    n = min(len(times), len(files))
    return start, times[:n], files[:n]


clips = [load(*c) for c in caps]
for (s, times, _), c in zip(clips, caps):
    if s is None and not (t0s == 'rec' and t1s == 'end'): sys.exit(f'no dive start in {c[1]}')
# the time span, relative to each clip's own dive start
spans = []
for s, times, _ in clips:
    a = times[0] - (s or times[0]) if t0s == 'rec' else float(t0s)
    b = times[-1] - (s or times[0]) if t1s == 'end' else float(t1s)
    spans.append((a, b))
a, b = max(x[0] for x in spans), min(x[1] for x in spans) if t1s == 'end' else float(t1s)
ticks = int((b - a) * FPS)
font = ImageFont.truetype('C:/Windows/Fonts/arialbd.ttf', 22)
labels = labels if len(clips) == 2 else []
cmd = [imageio_ffmpeg.get_ffmpeg_exe(), '-y', '-loglevel', 'error', '-f', 'rawvideo', '-pix_fmt', 'rgb24',
       '-s', f'{W * len(clips)}x{H}', '-r', str(FPS), '-i', '-', '-an', '-c:v', 'libx264', '-preset', 'veryfast',
       '-crf', '20', '-pix_fmt', 'yuv420p', '-movflags', '+faststart', out]
p = subprocess.Popen(cmd, stdin=subprocess.PIPE)
cache = {}
idx = [0] * len(clips)
for k in range(ticks):
    t = a + k / FPS
    frame = Image.new('RGB', (W * len(clips), H))
    for c, (s, times, files) in enumerate(clips):
        at = (s or times[0]) + t
        while idx[c] + 1 < len(times) and times[idx[c] + 1] <= at: idx[c] += 1
        f = files[idx[c]]
        if f not in cache:
            cache.clear(); cache[f] = Image.open(f).convert('RGB').resize((W, H), Image.LANCZOS)
        frame.paste(cache[f], (c * W, 0))
    if labels:
        d = ImageDraw.Draw(frame, 'RGBA')
        for c, lab in enumerate(labels):
            x = c * W + W // 2; tw = d.textlength(lab, font=font)
            d.rounded_rectangle((x - tw / 2 - 10, 54, x + tw / 2 + 10, 84), 8, fill=(0, 0, 0, 150))
            d.text((x - tw / 2, 57), lab, font=font, fill='white')
        d.rectangle((W - 1, 0, W, H), fill=(255, 255, 255, 255))
    p.stdin.write(frame.tobytes())
p.stdin.close(); p.wait()
print(f'{out}: {ticks} frames, {ticks / FPS:.2f} s, {W * len(clips)}x{H}')
