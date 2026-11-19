"""PM_89 §1: before / after frames at 20 fps round the dive's end, one block per recording (2 rows of 12 frames).
usage: python Tools/pm89_sheet.py <out.png> <label>=<frames dir>@<arrival s from the first frame | log>[:from:to] ...
  a log path: the arrival is its [devir] line (pm72_tour.js log with TOUR_LOG_TS=1)."""
import glob, json, os, re, sys
from PIL import Image, ImageDraw, ImageFont

out = sys.argv[1]
W, H, PER = 96, 208, 12
blocks = []
for spec in sys.argv[2:]:
    label, rest = spec.split('=', 1)
    d, arr = rest.split('@', 1)
    a0, a1 = -.2, 1.0
    if ':' in arr[2:]:
        parts = arr.rsplit(':', 2); arr, a0, a1 = parts[0], float(parts[1]), float(parts[2])
    times = json.load(open(os.path.join(d, 'times.json')))
    files = sorted(glob.glob(os.path.join(d, 'f*.jpg')))
    if os.path.exists(arr):
        m = [re.match(r'^([\d.,]+) \[devir\]', l) for l in open(arr, encoding='utf8', errors='replace')]
        arrival = [float(x.group(1).replace(',', '.')) / 1000 for x in m if x][0]
    else:
        arrival = times[0] + float(arr)
    picks, t = [], a0
    while t <= a1 + 1e-6 and len(picks) < 2 * PER:
        picks.append(min(range(len(times)), key=lambda i: abs(times[i] - arrival - t))); t += .05
    blocks.append((label, files, picks, a0))
font = ImageFont.load_default()
sheet = Image.new('RGB', (W * PER, len(blocks) * (2 * H + 22)), (14, 34, 68))
dr = ImageDraw.Draw(sheet)
for b, (label, files, picks, a0) in enumerate(blocks):
    y0 = b * (2 * H + 22)
    dr.text((6, y0 + 4), f'{label}  (20 fps, {a0:+.2f} s .. from the arrival)', fill=(248, 235, 208), font=font)
    for k, i in enumerate(picks):
        im = Image.open(files[i]).convert('RGB').resize((W, H))
        sheet.paste(im, ((k % PER) * W, y0 + 22 + (k // PER) * H))
sheet.save(out)
print(out, sheet.size)
