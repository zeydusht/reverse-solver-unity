"""PM_114: docs/art/pm114/kenar_kareler.png: n frames evenly over the pull back (the bottom to the end), each capture a
row (before over after), each frame cut to the board on the screen ([dl] b, the frame nearest in time) with a margin, at
one size, the time over it.
usage: python -I Tools/pm114_frames.py <out.png> <n> <capture>:<label> [<capture>:<label>]"""
import glob, json, os, re, sys
import numpy as np
from PIL import Image, ImageDraw, ImageFont

out, n, specs = sys.argv[1], int(sys.argv[2]), sys.argv[3:]
num = lambda s: float(s.replace(',', '.'))
TILE, PAD, HEAD, COLS = 330, .14, 22, 6
try: font = ImageFont.truetype('arialbd.ttf', 15); small = ImageFont.truetype('arial.ttf', 13)
except OSError: font = small = ImageFont.load_default()


def load(cap):
    text = open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace').read()
    start = None; rects = []
    for line in text.splitlines():
        m = re.match(r'^([\d.]+) \[cerceve\] 0[.,]000 ', line)
        if m and start is None: start = float(m.group(1)) / 1000
        m = re.match(r'^([\d.]+) \[dl\] ([\d.,-]+) [\d.,]+ b ((?:[\d.,-]+ ?){8})', line)
        if m:
            q = np.array([num(x) for x in m.group(3).split()]).reshape(4, 2)
            rects.append((float(m.group(1)) / 1000, num(m.group(2)), q[:, 0].min(), q[:, 1].min(), q[:, 0].max(), q[:, 1].max()))
    tb = num(re.search(r'the bottom at ([\d,]+) s', text).group(1)); end = num(re.search(r'ended at ([\d,]+) s', text).group(1))
    times = np.array(json.load(open(os.path.join(cap, 'times.json')))); files = sorted(glob.glob(os.path.join(cap, 'f*.jpg')))
    return start, rects, tb, end, times, files


rows = [load(s.rsplit(':', 1)[0]) for s in specs]
labels = [s.rsplit(':', 1)[1] for s in specs]
LW, lines = 150, (n + COLS - 1) // COLS
sheet = Image.new('RGB', (LW + COLS * (TILE + 6), len(rows) * (lines * (TILE + HEAD + 6) + 20) + 8), '#fcfcfb'); dr = ImageDraw.Draw(sheet)
for r, ((start, rects, tb, end, times, files), label) in enumerate(zip(rows, labels)):
    top = 8 + r * (lines * (TILE + HEAD + 6) + 20)
    dr.text((10, top + HEAD + TILE / 2 - 10), label, fill='#0b0b0b', font=font)
    if r: dr.line((10, top - 12, sheet.width - 10, top - 12), fill='#e6e5e0', width=2)
    for k in range(n):
        y0 = top + (k // COLS) * (TILE + HEAD + 6)
        want = tb + (end - tb) * k / (n - 1)   # on the dive's clock
        # the log line at that clock, then the first captured frame after it was drawn
        rc = min(rects, key=lambda x: abs(x[1] - want))
        i = int(np.searchsorted(times, rc[0] + .004)); i = min(i, len(files) - 1)
        im = Image.open(files[i])
        x0, y1, x1, y2 = rc[2:]
        cx, cy, half = (x0 + x1) / 2, (y1 + y2) / 2, max(x1 - x0, y2 - y1) / 2 * (1 + PAD)
        box = (int(round(cx - half)), int(round(cy - half)), int(round(cx + half)), int(round(cy + half)))
        tile = Image.new('RGB', (box[2] - box[0], box[3] - box[1]), '#fcfcfb')
        tile.paste(im.crop((max(0, box[0]), max(0, box[1]), min(im.width, box[2]), min(im.height, box[3]))), (max(0, -box[0]), max(0, -box[1])))
        x = LW + (k % COLS) * (TILE + 6)
        sheet.paste(tile.resize((TILE, TILE), Image.LANCZOS), (x, y0 + HEAD))
        share = (rc[1] - tb) / (end - tb)
        dr.text((x + 2, y0 + 3), f'{rc[1]:.2f} sn  (%{share * 100:.0f})', fill='#52514e', font=small)
sheet.save(out)
print(f'[frames] {out}: {len(rows)} rows of {n}')
