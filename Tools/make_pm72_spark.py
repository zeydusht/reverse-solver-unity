"""PM_72: the exit spark, 6 frames of 256 px: an opaque gold four-point flash and eight gold sparks flying out,
dark-gold rim so it reads on light and dark boards. -> Resources/Art/yeni/juice21/spark/01..06.png"""
import math, os
from PIL import Image, ImageDraw
OUT = 'Assets/_Game/Presentation/Resources/Art/yeni/juice21/spark/'
os.makedirs(OUT, exist_ok=True)
N, S = 6, 256
GOLD, LIGHT, RIM = (255, 206, 72, 255), (255, 244, 190, 255), (160, 102, 24, 255)

def star(d, cx, cy, r, inner, rot, fill, rim, w):
    pts = []
    for k in range(8):
        rr = r if k % 2 == 0 else inner
        a = rot + k * math.pi / 4
        pts.append((cx + rr * math.cos(a), cy + rr * math.sin(a)))
    d.polygon(pts, fill=fill, outline=rim, width=w)

for i in range(N):
    t = i / (N - 1)
    im = Image.new('RGBA', (S * 2, S * 2), (0, 0, 0, 0))   # drawn at 2x, then halved (smooth edges)
    d = ImageDraw.Draw(im)
    c = S
    flash = (1 - t) ** 1.5
    if flash > .05:
        star(d, c, c, S * .55 * (.55 + .45 * flash), S * .12 * (.6 + .4 * flash), -math.pi / 2, GOLD, RIM, 8)
        r = S * .10 * flash
        d.ellipse((c - r, c - r, c + r, c + r), fill=LIGHT)
    for k in range(8):
        a = k * math.pi / 4 + math.pi / 8
        dist = S * (.30 + .52 * (1 - (1 - t) ** 2))
        size = S * (.11 * (1 - .65 * t))
        x, y = c + dist * math.cos(a), c + dist * math.sin(a)
        star(d, x, y, size, size * .42, a, GOLD, RIM, 6)
    im.resize((S, S), Image.LANCZOS).save(OUT + '%02d.png' % (i + 1))
print('ok')
