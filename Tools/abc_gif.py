"""PM_80: several dive captures side by side in one GIF (A, B, C), each from its own motion start; a shorter one
holds its last frame. Frames every 30 ms of GIF time (GIF_DELAY=40 in the environment: every 40 ms) (browsers slow delays under 20 ms down); at speed .5 that is
every 15 ms of the capture.
    python -I Tools/abc_gif.py <out.gif> <speed> <seconds> <width> <capture dir> <label> [<capture dir> <label> ...]
(GIF_COLORS=96 in the environment: a smaller palette)"""
import os, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from PIL import Image
from arc_gif import frames, start, label_img

DELAY = int(os.environ.get("GIF_DELAY", 30))


def main():
    a = sys.argv[1:]
    dst, speed, seconds, width = a[0], float(a[1]), float(a[2]), int(a[3])
    panels = [(a[i], a[i + 1]) for i in range(4, len(a), 2)]
    seqs = []
    for d, label in panels:
        fr = frames(d); t0 = max(0, start(fr) - .12)
        seqs.append((fr, t0, label))
    n = int(seconds / speed * 1000 / DELAY) + 1
    images = []
    for k in range(n):
        t = k * DELAY / 1000 * speed
        row = []
        for fr, t0, label in seqs:
            j = 0
            while j + 1 < len(fr) and fr[j + 1][0] <= t0 + t: j += 1
            im = Image.open(fr[j][1]).convert("RGB")
            h = int(im.height * width / im.width)
            row.append(label_img(im.resize((width, h), Image.LANCZOS), label))
        both = Image.new("RGB", (width * len(row) + 6 * (len(row) - 1), row[0].height), (255, 255, 255))
        for i, im in enumerate(row): both.paste(im, (i * (width + 6), 0))
        images.append(both.quantize(colors=int(os.environ.get("GIF_COLORS", 128)), method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE))
    images[0].save(dst, save_all=True, append_images=images[1:], duration=DELAY, loop=0, optimize=True, disposal=1)
    print(f"{dst}: {len(images)} frames, {len(images) * DELAY / 1000:.2f} s, {os.path.getsize(dst) // 1024} KB")


if __name__ == "__main__":
    main()
