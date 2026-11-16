"""PM_77: the menu -> level arc alone, as a GIF from a Tools/pm72_tour.js capture (60 fps frames, times.json).
The motion's start is found from the frames (the first one that differs from the menu); the GIF runs from a little
before it for <seconds>, at <speed> (1 normal, .5 half speed). GIF delays are in 1/100 s and browsers slow down
delays under 2/100 s, so the 60 fps capture is shown at 50 frames a second at normal speed and frame for frame
(each frame twice its real interval) at half speed.
    python -I Tools/arc_gif.py <capture dir> <out.gif> [speed=1] [seconds=2.4] [width=300] [label] [+<capture dir 2> label2]"""
import json, os, sys
from PIL import Image, ImageChops, ImageDraw, ImageFont, ImageStat


def frames(d):
    times = json.load(open(os.path.join(d, "times.json")))
    files = sorted(f for f in os.listdir(d) if f.startswith("f") and f.endswith(".jpg"))
    return [(times[i] - times[0], os.path.join(d, f)) for i, f in enumerate(files[:len(times)])]


def start(fr):
    first = Image.open(fr[0][1]).convert("L").resize((98, 211))
    for t, f in fr[1:]:
        cur = Image.open(f).convert("L").resize((98, 211))
        if ImageStat.Stat(ImageChops.difference(first, cur)).mean[0] > 1.5:
            return t
    return 0.0


def pick(fr, t0, seconds, speed):
    """(path, delay ms) pairs: normal speed sampled at 50 fps; slower speeds every frame, stretched."""
    out = []
    if speed >= 1:
        j = 0
        for k in range(int(seconds * 50) + 1):
            t = t0 + k / 50
            while j + 1 < len(fr) and fr[j + 1][0] <= t: j += 1
            out.append((fr[j][1], 20))
        return out
    sel = [(t, f) for t, f in fr if t0 <= t <= t0 + seconds]
    shown = 0.0   # in 1/100 s, carried so the rounding never adds up (33.3 ms frames: 30, 40, 30 ...)
    for i, (t, f) in enumerate(sel):
        nxt = sel[i + 1][0] if i + 1 < len(sel) else t + 1 / 60
        due = (nxt - sel[0][0]) / speed * 100
        cs = max(2, round(due - shown)); shown += cs
        out.append((f, cs * 10))
    return out


def label_img(im, text):
    if not text: return im
    d = ImageDraw.Draw(im)
    try: font = ImageFont.truetype("arialbd.ttf", 18)
    except OSError: font = ImageFont.load_default()
    d.rectangle((0, 0, im.width, 26), fill=(20, 20, 30))
    d.text((8, 3), text, fill=(255, 255, 255), font=font)
    return im


def main():
    a = sys.argv[1:]
    src, dst = a[0], a[1]
    speed = float(a[2]) if len(a) > 2 else 1
    seconds = float(a[3]) if len(a) > 3 else 2.4
    width = int(a[4]) if len(a) > 4 else 300
    label = a[5] if len(a) > 5 else ""
    second = a[6][1:] if len(a) > 6 and a[6].startswith("+") else None
    label2 = a[7] if len(a) > 7 else ""
    fr = frames(src); t0 = max(0, start(fr) - .12)
    seq = pick(fr, t0, seconds, speed)
    seq2 = None
    if second:
        fr2 = frames(second); t2 = max(0, start(fr2) - .12)
        seq2 = pick(fr2, t2, seconds, speed)
        n = min(len(seq), len(seq2)); seq, seq2 = seq[:n], seq2[:n]
    images, delays = [], []
    for i, (f, ms) in enumerate(seq):
        im = Image.open(f).convert("RGB")
        h = int(im.height * width / im.width)
        im = label_img(im.resize((width, h), Image.LANCZOS), label)
        if seq2:
            im2 = Image.open(seq2[i][0]).convert("RGB").resize((width, h), Image.LANCZOS)
            both = Image.new("RGB", (width * 2 + 6, h), (255, 255, 255))
            both.paste(im, (0, 0)); both.paste(label_img(im2, label2), (width + 6, 0)); im = both
        images.append(im.quantize(colors=128, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE))
        delays.append(ms)
    images[0].save(dst, save_all=True, append_images=images[1:], duration=delays, loop=0, optimize=True, disposal=1)
    print(f"{dst}: {len(images)} frames, {sum(delays) / 1000:.2f} s, motion from {t0 + .12:.2f} s of the capture, {os.path.getsize(dst) // 1024} KB")


if __name__ == "__main__":
    main()
