"""Frames from Tools/gif_capture.js -> GIF at a fixed frame rate, by real capture times.
usage: python -I Tools/gif_encode.py <frame dir> <out.gif> [fps=15] [width=300] [label] [crop x0,y0,x1,y1] [colors=128]
Two dirs joined with '+' are placed side by side (before | after); label 'Önce|Sonra' captions them."""
import json, os, sys
from PIL import Image, ImageDraw, ImageFont

def load(d):
    times = json.load(open(os.path.join(d, 'times.json')))
    files = sorted(f for f in os.listdir(d) if f.startswith('f') and f.endswith('.jpg'))
    return [(times[i] - times[0], os.path.join(d, f)) for i, f in enumerate(files[:len(times)])]

def sample(frames, fps, length):
    out, j = [], 0
    for k in range(int(length * fps) + 1):
        t = k / fps
        while j + 1 < len(frames) and frames[j + 1][0] <= t: j += 1
        out.append(frames[j][1])
    return out

def main():
    src, dst = sys.argv[1], sys.argv[2]
    fps = int(sys.argv[3]) if len(sys.argv) > 3 else 15
    width = int(sys.argv[4]) if len(sys.argv) > 4 else 300
    labels = sys.argv[5].split('|') if len(sys.argv) > 5 and sys.argv[5] else None
    crop = tuple(int(v) for v in sys.argv[6].split(',')) if len(sys.argv) > 6 and sys.argv[6] else None
    colors = int(sys.argv[7]) if len(sys.argv) > 7 else 128
    dirs = src.split('+')
    seqs = [load(d) for d in dirs]
    length = max(s[-1][0] for s in seqs)
    picks = [sample(s, fps, length) for s in seqs]
    try: font = ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf', 20)
    except OSError: font = ImageFont.load_default()
    frames = []
    for k in range(len(picks[0])):
        tiles = []
        for p in picks:
            im = Image.open(p[k]).convert('RGB')
            if crop: im = im.crop(crop)
            im = im.resize((width, round(im.height * width / im.width)), Image.LANCZOS)
            tiles.append(im)
        h = tiles[0].height + (28 if labels else 0)
        sheet = Image.new('RGB', (width * len(tiles) + 6 * (len(tiles) - 1), h), (16, 30, 52))
        for i, im in enumerate(tiles):
            x = i * (width + 6)
            sheet.paste(im, (x, 28 if labels else 0))
            if labels: ImageDraw.Draw(sheet).text((x + 8, 3), labels[i], fill=(255, 245, 214), font=font)
        frames.append(sheet.quantize(colors=colors, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE))
    frames[0].save(dst, save_all=True, append_images=frames[1:], duration=round(1000 / fps), loop=0, optimize=True)
    print(f'{dst}: {len(frames)} frames, {os.path.getsize(dst) // 1024} KB')

main()
