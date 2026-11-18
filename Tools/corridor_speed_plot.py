"""PM_86 §1: the corridor slide's speed against time, before (PM_85: Glide, a plateau) and after (PM_86: Ease,
smootherstep, a bell), from CorridorTransition's own formulas (1.5 s: .12 s fade-in, .08 s beat, 1.3 s slide; the
strip's 862.25 px period at the view's scale, shown as share of the slide per second).
    python -I Tools/corridor_speed_plot.py <out.png>"""
import sys
from PIL import Image, ImageDraw, ImageFont

def glide(u, rise=.3, fall=.3):
    u = min(1, max(0, u))
    I = lambda x: x ** 3 - x ** 4 / 2
    total = 1 - rise / 2 - fall / 2
    if u < rise: p = rise * I(u / rise)
    elif u < 1 - fall: p = rise / 2 + (u - rise)
    else: p = rise / 2 + (1 - rise - fall) + fall * (.5 - I((1 - u) / fall))
    return p / total

def ease(u):
    u = min(1, max(0, u))
    return u ** 3 * (u * (u * 6 - 15) + 10)

LEAD, SLIDE, N = .2, 1.3, 600
def speeds(f):
    out = []
    for i in range(N + 1):
        t = 1.5 * i / N
        u0, u1 = (t - LEAD) / SLIDE, (t + 1e-3 - LEAD) / SLIDE
        out.append((t, (f(u1) - f(u0)) / 1e-3))
    return out

W, H, L, R, T, B = 900, 380, 60, 20, 40, 50
img = Image.new('RGB', (W, H), 'white'); d = ImageDraw.Draw(img)
try: font = ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf', 14)
except OSError: font = ImageFont.load_default()
top = 1.6
X = lambda t: L + (W - L - R) * t / 1.5
Y = lambda v: H - B - (H - B - T) * v / top
d.line([(L, H - B), (W - R, H - B)], fill=(120, 120, 120)); d.line([(L, T), (L, H - B)], fill=(120, 120, 120))
for t in [0, .25, .5, .75, 1, 1.25, 1.5]:
    d.text((X(t) - 10, H - B + 6), f'{t:.2f}', fill=(80, 80, 80), font=font)
for v in [0, .4, .8, 1.2, 1.6]:
    d.text((10, Y(v) - 8), f'{v:.1f}', fill=(80, 80, 80), font=font)
for f, col, name in [(glide, (200, 90, 60), 'Önce (PM_85): plato, ortası sabit hız'), (ease, (30, 90, 200), 'Sonra (PM_86): çan eğrisi (smootherstep)')]:
    pts = [(X(t), Y(v)) for t, v in speeds(f)]
    d.line(pts, fill=col, width=3)
d.text((L + 10, 8), 'PM_86 §1: koridor kayışının hızı (kayışın payı / sn) — süre (sn)', fill=(20, 20, 20), font=font)
d.text((L + 14, T + 6), 'Önce (PM_85): plato, ortası sabit hız', fill=(200, 90, 60), font=font)
d.text((L + 14, T + 26), 'Sonra (PM_86): çan eğrisi (smootherstep)', fill=(30, 90, 200), font=font)
img.save(sys.argv[1])
print('peak before %.2f, after %.2f' % (max(v for _, v in speeds(glide)), max(v for _, v in speeds(ease))))
