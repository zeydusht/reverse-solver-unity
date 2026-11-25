"""PM_107 §D3: the bottom tab bar from Codex's chosen option (docs/art/pm107/codex/alt_cubuk/final_*, else A_*) into the
files MenuTabs draws (Resources/Art/yeni/pm86/menu/pm86-tab-*, t21-tab-icon-*-normal), each at its own size, so the bar's
code and its motion (the face widening, the dividers) stay as they are:
  strip     the bar's ground, 1170x240 (9-slice: 60 | 60, top 24)
  selected  the chosen tab's face, 420x270, 9-sliced (70, 40, 70, 70): the corners kept, the middle stretched
  divider   24x200
  icons     fitted into 168x148, centred, aspect kept
The old files' .meta (slicing, pixels per unit) stay.
    python -I Tools/pm107_altcubuk.py [A|B|C|final]
"""
import os, sys
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources', 'Art')
SRC = os.path.join(ROOT, 'docs', 'art', 'pm107', 'codex', 'alt_cubuk')
pick = sys.argv[1] if len(sys.argv) > 1 else ('final' if os.path.exists(os.path.join(SRC, 'final_selected.png')) else 'A')
src = lambda n: Image.open(os.path.join(SRC, f'{pick}_{n}.png')).convert('RGBA')


def trim(im):
    bb = im.getchannel('A').point(lambda a: 255 if a > 6 else 0).getbbox()
    return im.crop(bb) if bb else im


def nine(im, w, h, l, t, r, b):
    """im's corners (l, t, r, b px at the target scale) kept, the edges and middle stretched to w x h."""
    s = h / im.height
    im = im.resize((max(l + r + 2, round(im.width * s)), h), Image.LANCZOS)
    W, H = im.size
    out = Image.new('RGBA', (w, h), (0, 0, 0, 0))
    cols = [(0, l, 0, l), (l, W - r, l, w - r), (W - r, W, w - r, w)]
    rows = [(0, t, 0, t), (t, H - b, t, h - b), (H - b, H, h - b, h)]
    for sx0, sx1, dx0, dx1 in cols:
        for sy0, sy1, dy0, dy1 in rows:
            if sx1 <= sx0 or sy1 <= sy0 or dx1 <= dx0 or dy1 <= dy0: continue
            out.paste(im.crop((sx0, sy0, sx1, sy1)).resize((dx1 - dx0, dy1 - dy0), Image.LANCZOS), (dx0, dy0))
    return out


def fit(im, w, h):
    im = trim(im)
    s = min(w / im.width, h / im.height)
    im = im.resize((max(1, round(im.width * s)), max(1, round(im.height * s))), Image.LANCZOS)
    out = Image.new('RGBA', (w, h), (0, 0, 0, 0)); out.paste(im, ((w - im.width) // 2, (h - im.height) // 2), im)
    return out


M = os.path.join(ART, 'yeni', 'pm86', 'menu')
src('strip').resize((1170, 240), Image.LANCZOS).save(os.path.join(M, 'pm86-tab-strip.png'))
nine(trim(src('selected')), 420, 270, 70, 70, 70, 40).save(os.path.join(M, 'pm86-tab-selected.png'))
fit(src('divider'), 24, 200).save(os.path.join(M, 'pm86-tab-divider.png'))
for key, n in [('home', 'home'), ('collection', 'collection'), ('jiggles', 'jiggles'), ('trophy', 'trophy')]:
    fit(src('icon_' + n), 168, 148).save(os.path.join(ART, f't21-tab-icon-{key}-normal.png'))
print(f'[altcubuk] {pick}: strip, selected (9-slice), divider, 4 icons written')
