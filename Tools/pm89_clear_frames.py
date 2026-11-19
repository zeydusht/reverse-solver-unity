"""PM_89 §1: the gallery's painted-in frames come off the wall (the game now draws each frame at its level's
proportions: Ui/Frame89.cs).
- The menu's room (oda.png): a generous patch of wall round the old frame (its shadow too, up to the wall's
  corner and above the dado) is filled smoothly from its edge (push-pull), then given the wall's own grain (a
  high-pass of a plain stretch of the same wall, tiled) and feathered in, so a small frame (a 3 x 3 board) leaves
  no stain.
- The corridor's strip: each bay's frame, its picture and shadow, filled the same way.
  python Tools/pm89_clear_frames.py [--from-git <rev>] [--preview] [--oda-only]
  (--from-git: start from the plates as git has them at <rev>, e.g. the commit before PM_89)"""
import json, math, os, subprocess, sys, io
import numpy as np
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RES = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources')
ODA = os.path.join(RES, 'Art', 'yeni', 'pm78', 'diorama', 'oda.png')
STRIP = os.path.join(RES, 'corridor_strip.png')
PREVIEW = '--preview' in sys.argv
REV = sys.argv[sys.argv.index('--from-git') + 1] if '--from-git' in sys.argv else None


def source(path):
    if not REV: return Image.open(path)
    rel = os.path.relpath(path, ROOT).replace('\\', '/')
    return Image.open(io.BytesIO(subprocess.check_output(['git', 'show', f'{REV}:{rel}'], cwd=ROOT)))


def push_pull(img, hole):
    """img float HxWx3, hole bool HxW: the hole filled smoothly from its edge (a mask-weighted pyramid)."""
    levels = []
    c = img * (~hole)[..., None]; w = (~hole).astype(np.float64)
    while min(c.shape[:2]) > 2:
        levels.append((c, w))
        h2, w2 = (c.shape[0] + 1) // 2, (c.shape[1] + 1) // 2
        cp = np.zeros((h2 * 2, w2 * 2, 3)); wp = np.zeros((h2 * 2, w2 * 2))
        cp[:c.shape[0], :c.shape[1]] = c; wp[:w.shape[0], :w.shape[1]] = w
        c = cp.reshape(h2, 2, w2, 2, 3).sum((1, 3)); w = wp.reshape(h2, 2, w2, 2).sum((1, 3))
    filled = c / np.maximum(w, 1e-9)[..., None]
    for c, w in reversed(levels):
        up = np.repeat(np.repeat(filled, 2, 0), 2, 1)[:c.shape[0], :c.shape[1]]
        up = np.asarray(Image.fromarray(np.clip(up, 0, 255).astype(np.uint8)).filter(ImageFilter.BoxBlur(1)), dtype=np.float64)
        mean = np.where((w > 1e-9)[..., None], c / np.maximum(w, 1e-9)[..., None], 0)
        a = np.clip(w, 0, 1)[..., None]
        filled = mean * a + up * (1 - a)
    return filled


def laplace(img, hole, scales=(8, 4, 2, 1), iters=(3000, 600, 200, 80)):
    """The hole filled as a membrane stretched over its edge (Laplace's equation, Jacobi, coarse to fine): only the
    wall right at the edge counts (push-pull alone averages whole coarse blocks: the sunlit corner and the column
    leaked in as brown)."""
    H, W = hole.shape
    sol = None
    for s, n in zip(scales, iters):
        h, w = -(-H // s), -(-W // s)
        pad = np.zeros((h * s, w * s, 3)); pk = np.zeros((h * s, w * s))
        pad[:H, :W] = img * (~hole)[..., None]; pk[:H, :W] = ~hole
        c = pad.reshape(h, s, w, s, 3).sum((1, 3)); k = pk.reshape(h, s, w, s).sum((1, 3))
        known = k >= s * s * .99   # whole blocks only: the edge is the wall itself
        val = c / np.maximum(k, 1e-9)[..., None]
        if sol is None:
            x = np.where(known[..., None], val, val[known].mean(0))
        else:
            up = np.repeat(np.repeat(sol, 2, 0), 2, 1)[:h, :w]
            x = np.where(known[..., None], val, up)
        for _ in range(n):
            p = np.pad(x, ((1, 1), (1, 1), (0, 0)), mode='edge')
            avg = (p[:-2, 1:-1] + p[2:, 1:-1] + p[1:-1, :-2] + p[1:-1, 2:]) / 4
            x = np.where(known[..., None], x, avg)
        sol = x
    return np.where(hole[..., None], sol[:H, :W], img)


def clear(path, quads, grain_box, feather, out_preview, grain_k=1.0):
    im = source(path)
    mode = im.mode
    rgb = np.asarray(im.convert('RGB'), dtype=np.float64)
    mask_im = Image.new('L', im.size, 0)
    d = ImageDraw.Draw(mask_im)
    for q in quads: d.polygon([tuple(p) for p in q], fill=255)
    hole = np.asarray(mask_im) > 0
    filled = laplace(rgb, hole)
    # the wall's grain: a plain stretch of the same wall, high-passed, tiled (mirrored) over the hole
    x0, y0, x1, y1 = grain_box
    patch = rgb[y0:y1, x0:x1]
    hp = patch - np.asarray(Image.fromarray(patch.astype(np.uint8)).filter(ImageFilter.GaussianBlur(4)), dtype=np.float64)
    tile = np.concatenate([hp, hp[:, ::-1]], 1); tile = np.concatenate([tile, tile[::-1]], 0)
    H, W = hole.shape
    grain = np.tile(tile, (H // tile.shape[0] + 1, W // tile.shape[1] + 1, 1))[:H, :W]
    soft = np.asarray(mask_im.filter(ImageFilter.GaussianBlur(feather)), dtype=np.float64)[..., None] / 255
    out = rgb * (1 - soft) + (filled + grain * grain_k) * soft
    res = Image.fromarray(np.clip(out, 0, 255).astype(np.uint8), 'RGB')
    if mode == 'RGBA':
        res = res.convert('RGBA'); res.putalpha(im.getchannel('A'))
    if PREVIEW:
        os.makedirs(os.path.dirname(out_preview), exist_ok=True)
        res.convert('RGB').save(out_preview)
    else:
        res.save(path)
    print(f'{os.path.basename(path)}: {hole.sum()} px filled')


def homography(src, dst):
    A = []
    for (x, y), (u, v) in zip(src, dst):
        A.append([x, y, 1, 0, 0, 0, -u * x, -u * y, -u]); A.append([0, 0, 0, x, y, 1, -v * x, -v * y, -v])
    _, _, vt = np.linalg.svd(np.array(A)); return vt[-1].reshape(3, 3) / vt[-1][-1]


def apply(h, p):
    v = h @ np.array([p[0], p[1], 1.0]); return v[:2] / v[2]


def oda():
    a = json.load(open(os.path.join(RES, 'Art', 'yeni', 'pm78', 'diorama', 'diorama.json')))
    F, Cx, Cy = a['f'], a['cx'], a['cy']; th = math.radians(a['start_deg']); D, H = a['d'], a['h']
    d0 = np.array([math.sin(th), 0, -math.cos(th)]); r0 = np.array([math.cos(th), 0, math.sin(th)])
    cam = np.array([-D * math.sin(th), H, D * math.cos(th)])
    ox, oy = a['image_origin']

    def plate(x, y):   # a wall point (z 0) in the plate's px
        v = np.array([x, y, 0.0]) - cam; z = v @ d0
        return (Cx + F * (v @ r0) / z + ox, Cy - F * v[1] / z + oy)
    # the canvas was x -.5 .. .5, y .885 .. 2.25; the frame, its crest and its shadow fit well inside this; the wall
    # turns its corner at x .93 (the column, a layer in front, stands at .79)
    x0, x1, y0, y1 = -.8, .87, .55, 2.68
    clear(ODA, [[plate(x0, y1), plate(x1, y1), plate(x1, y0), plate(x0, y0)]], (180, 500, 270, 900), 14,
          os.path.join(ROOT, 'Logs', 'pm89', 'oda_preview.jpg'))


def strip():
    bay = [(208.3, 291.2), (585.6, 332.9), (576.9, 882.2), (196.2, 865.7)]
    period = 862.25
    flat = [(0, 0), (1, 0), (1, 1.45), (0, 1.45)]
    quads = []
    for k in (0, 1):
        h = homography(flat, [(x + k * period, y) for x, y in bay])
        x0, x1, y0, y1 = -.3, 1.26, -.36, 1.45 + .36
        quads.append([tuple(apply(h, p)) for p in [(x0, y0), (x1, y0), (x1, y1), (x0, y1)]])
    # the strip slides past quickly: its wall smooth (its own grain sample repeated visibly)
    clear(STRIP, quads, (650, 350, 700, 850), 12, os.path.join(ROOT, 'Logs', 'pm89', 'strip_preview.jpg'), grain_k=0)


if __name__ == '__main__':
    oda()
    if '--oda-only' not in sys.argv: strip()
