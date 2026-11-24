"""PM_105 §1: the before/after GIF of a chained pair's knock, from the frames the PlayMode test measured
(Pm105ChainKnockTests -> docs/art/pm105/zincir_kareler.json: each cell's drawn body every frame, the old whole-piece
scale and the new cell-by-cell one). Left ÖNCE, right SONRA, slowed down 4x; the chain is the line between the two
cells' centres, the blockers grey, each cell's contact a red line.
    python -I Tools/make_zincir_gif.py [docs/art/pm105/zincir_kareler.json] [docs/art/pm105/zincir_once_sonra.gif]"""
import json, sys
from PIL import Image, ImageDraw, ImageFont

src = sys.argv[1] if len(sys.argv) > 1 else 'docs/art/pm105/zincir_kareler.json'
out = sys.argv[2] if len(sys.argv) > 2 else 'docs/art/pm105/zincir_once_sonra.gif'
d = json.load(open(src, encoding='utf8'))
C, W, H = d['cell'], d['w'], d['h']
A, B = d['a'], d['b']
S = 0.75                      # px per point
CASES = [('3 boşluk, temas yakın', 3, 5), ('3 boşluk, zincir ucu yakın', 5, 3)]
SLOW = 4
try: font = ImageFont.truetype('arial.ttf', 15); small = ImageFont.truetype('arial.ttf', 12)
except OSError: font = small = ImageFont.load_default()


def frames(name, per):
    c = next(k for k in d['cases'] if k['name'] == name and k['perCell'] == per)
    fr, last = [], -1
    for f in c['frames']:
        if f[0] - last >= 1 / 60: fr.append(f); last = f[0]
    return fr


def panel(img, ox, oy, f, title, gaps, color):
    g = ImageDraw.Draw(img)
    g.text((ox, oy - 40), title, fill=(20, 30, 40), font=font)
    bw, bh = W * C * S, H * C * S
    g.rectangle([ox, oy, ox + bw, oy + bh], fill=(232, 236, 240), outline=(120, 130, 140))
    for x in range(1, W): g.line([ox + x * C * S, oy, ox + x * C * S, oy + bh], fill=(214, 219, 224))
    for y in range(1, H): g.line([ox, oy + y * C * S, ox + bw, oy + y * C * S], fill=(214, 219, 224))
    for (cx, cy), gap in ((A, gaps[0]), (B, gaps[1])):
        by = cy + gap + 1
        if by < H:
            g.rectangle([ox + cx * C * S + 2, oy + by * C * S + 2, ox + (cx + 1) * C * S - 2, oy + (by + 1) * C * S - 2], fill=(160, 166, 172))
            g.line([ox + cx * C * S - 4, oy + by * C * S, ox + (cx + 1) * C * S + 4, oy + by * C * S], fill=(220, 60, 50), width=2)
    ra, rb = f[1:5], f[5:9]
    ca = (ox + (ra[0] + ra[2]) / 2 * S, oy + (ra[1] + ra[3]) / 2 * S); cb = (ox + (rb[0] + rb[2]) / 2 * S, oy + (rb[1] + rb[3]) / 2 * S)
    g.line([ca, cb], fill=(70, 80, 90), width=3)
    for r in (ra, rb):
        g.rounded_rectangle([ox + r[0] * S, oy + r[1] * S, ox + r[2] * S, oy + r[3] * S], radius=6, fill=color, outline=(18, 63, 70), width=2)
    # the far end's offset from the near end's (cells): the fling
    split = abs((ra[3] - (A[1] + 1) * C) - (rb[3] - (B[1] + 1) * C)) / C
    g.text((ox, oy + bh + 8), f'uçlar arası fark {split:.2f} hücre', fill=(180, 40, 30) if split > .05 else (30, 110, 60), font=small)


pw, ph = W * C * S, H * C * S
imgs = []
for name, ga, gb in CASES:
    old, new = frames(name, False), frames(name, True)
    n = max(len(old), len(new))
    for i in range(n):
        img = Image.new('RGB', (int(pw * 2 + 90), int(ph + 110)), (250, 250, 247))
        panel(img, 30, 60, old[min(i, len(old) - 1)], 'ÖNCE: ' + name, (ga, gb), (231, 156, 120))
        panel(img, 60 + pw, 60, new[min(i, len(new) - 1)], 'SONRA: ' + name, (ga, gb), (120, 190, 170))
        imgs.append(img)
    for _ in range(20): imgs.append(imgs[-1])
imgs[0].save(out, save_all=True, append_images=imgs[1:], duration=int(1000 / 60 * SLOW), loop=0, optimize=True)
print(f'[gif] {len(imgs)} kare -> {out}')
