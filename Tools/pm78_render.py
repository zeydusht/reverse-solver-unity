"""PM_78: the diorama rendered in Python, independently of Unity, from the game's own files
(Resources/Art/yeni/pm78/diorama): a ray from the camera for every pixel, the front cut-outs (column, banner, sconce)
first, then the floor or the wall; each hit is looked up in the plate through the start camera.
  docs/art/pm78/baslangic_karsilastirma.png  the start pose next to the menu as the phone shows it, and the two
                                             over each other at 50 % (and the difference)
  docs/art/pm78/zeyd_kareleri.png            Zeyd's four keys
  docs/art/pm78/karsidan.png                 the wall square on (an elevation) and the floor from above
    python -I Tools/pm78_render.py [w h]"""
import json, math, os, sys
import numpy as np
from PIL import Image, ImageDraw, ImageFont

sys.path.insert(0, os.path.dirname(__file__))
from pm78_geom import Menu, Flight, zeyd_keys, IMG_W, IMG_H   # noqa: E402

ROOT = os.path.join(os.path.dirname(__file__), "..")
DIO = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/yeni/pm78/diorama")
MENU = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/menu_bg.png")
OUT = os.path.join(ROOT, "docs/art/pm78")


def bilinear(img, x, y):
    h, w = img.shape[:2]
    x = np.clip(x - .5, 0, w - 1.001); y = np.clip(y - .5, 0, h - 1.001)
    x0 = np.floor(x).astype(int); y0 = np.floor(y).astype(int); fx = (x - x0)[..., None]; fy = (y - y0)[..., None]
    a = img[y0, x0]; b = img[y0, x0 + 1]; c = img[y0 + 1, x0]; d = img[y0 + 1, x0 + 1]
    return (a * (1 - fx) + b * fx) * (1 - fy) + (c * (1 - fx) + d * fx) * fy


class Scene:
    def __init__(self):
        self.j = json.load(open(os.path.join(DIO, "diorama.json"), encoding="utf8"))
        keys = zeyd_keys()[0]
        self.m = Menu(math.degrees(math.atan2(-keys[0][0], keys[0][1])))
        self.plate = np.asarray(Image.open(os.path.join(DIO, "oda.png")).convert("RGB"), dtype=np.float32)
        self.oy = self.j["image_origin"][1]
        self.fronts = [(f, np.asarray(Image.open(os.path.join(DIO, f["name"] + ".png")).convert("RGBA"), dtype=np.float32)) for f in self.j["fronts"]]

    def to_image(self, P):
        """3D points (..., 3) -> start-camera picture px."""
        m = self.m; c, d, r, u = m.basis()
        v = P - np.array(c)
        z = v @ np.array(d); x = v @ np.array(r)
        return m.cx + m.f * x / z, m.cy - m.f * v[..., 1] / z

    def render(self, fl, o, scale=2.0):
        W, H = int(fl.sw * scale), int(fl.sh * scale)
        ys, xs = np.mgrid[0:H, 0:W].astype(np.float64)
        sx = (xs + .5) / scale; sy = (ys + .5) / scale
        cam, th, mid = fl.pose(o)
        cam = np.array(cam); d = np.array([math.sin(th), 0, -math.cos(th)]); r = np.array([math.cos(th), 0, math.sin(th)])
        dirs = d * fl.f + r * (sx - mid[0])[..., None] - np.array([0, 1, 0]) * (sy - mid[1])[..., None]
        with np.errstate(divide="ignore", invalid="ignore"):
            tf = -cam[1] / dirs[..., 1]
            pf = cam + dirs * tf[..., None]
            floor = (dirs[..., 1] < 0) & (pf[..., 2] > 0)
            tw = -cam[2] / dirs[..., 2]
            pw = cam + dirs * tw[..., None]
        P = np.where(floor[..., None], pf, pw)
        ix, iy = self.to_image(P)
        out = bilinear(self.plate, ix, iy + self.oy)
        for f, tex in self.fronts:
            dz = f["depth"]
            with np.errstate(divide="ignore", invalid="ignore"):
                t = (dz - cam[2]) / dirs[..., 2]
            ph = cam + dirs * t[..., None]
            fx, fy = self.to_image(ph)
            x0, y0, x1, y1 = f["px"]
            inside = (fx >= x0) & (fx < x1) & (fy >= y0) & (fy < y1)
            if not inside.any(): continue
            s = bilinear(tex, fx - x0, fy - y0)
            a = (s[..., 3:4] / 255) * inside[..., None]
            out = out * (1 - a) + s[..., :3] * a
        return Image.fromarray(np.clip(out, 0, 255).astype(np.uint8))


def menu_view(fl, scale=2.0):
    """The menu picture as the phone shows it (cover fit, centred)."""
    src = np.asarray(Image.open(MENU).convert("RGB"), dtype=np.float32)
    W, H = int(fl.sw * scale), int(fl.sh * scale)
    ys, xs = np.mgrid[0:H, 0:W].astype(np.float64)
    ix, iy = fl.to_image((xs + .5) / scale, (ys + .5) / scale)   # the same sampling as the render: only geometry differs
    return Image.fromarray(np.clip(bilinear(src, ix, iy), 0, 255).astype(np.uint8))


def font(n):
    try: return ImageFont.truetype("arialbd.ttf", n)
    except OSError: return ImageFont.load_default()


def label(im, text):
    d = ImageDraw.Draw(im); d.rectangle((0, 0, im.width, 34), fill=(20, 20, 30)); d.text((10, 6), text, fill=(255, 255, 255), font=font(22))
    return im


def main():
    sw, sh = (int(sys.argv[1]), int(sys.argv[2])) if len(sys.argv) > 2 else (390, 844)
    sc = Scene(); fl = Flight(sc.m, (sw, sh))
    start = sc.render(fl, 0); ref = menu_view(fl)
    a = np.asarray(start, dtype=np.float32); b = np.asarray(ref, dtype=np.float32)
    diff = np.abs(a - b).mean(axis=2)
    stats = f"ortalama fark {diff.mean():.2f}/255, %99'u {np.percentile(diff, 99):.1f}, en büyük {diff.max():.0f}"
    over = Image.blend(start, ref, .5)
    heat = Image.fromarray(np.clip(diff * 8, 0, 255).astype(np.uint8)).convert("RGB")
    W, H = start.size
    sheet = Image.new("RGB", (W * 4 + 30, H + 80), (255, 255, 255))
    for i, (im, t) in enumerate(((ref, "menü resmi (telefonda)"), (start, "diorama, 1. kare"), (over, "üst üste %50"), (heat, "fark x8"))):
        sheet.paste(label(im.copy(), t), (i * (W + 10), 0))
    ImageDraw.Draw(sheet).text((10, H + 20), f"{sw}x{sh} pt, 2x. {stats}", fill=(0, 0, 0), font=font(26))
    sheet.save(os.path.join(OUT, "baslangic_karsilastirma.png"), optimize=True)
    print("start frame vs menu:", stats)
    # Zeyd's four keys (o = 0, 1/3, 2/3, 1: each segment 1.2 s of the 3.6)
    keys = [sc.render(fl, o, 1.0) for o in (0, 1 / 3, 2 / 3, 1)]
    w, h = keys[0].size
    sheet = Image.new("RGB", (w * 4 + 30, h), (255, 255, 255))
    for i, im in enumerate(keys):
        cam, th, _ = fl.pose(i / 3)
        sheet.paste(label(im, f"kare {i + 1}: {math.degrees(th):.1f}°"), (i * (w + 10), 0))
    sheet.save(os.path.join(OUT, "zeyd_kareleri.png"), optimize=True)
    # square on: the wall's elevation and the floor from above (what the layers are, without perspective)
    m = sc.m; ppw = 260
    xs = np.linspace(-1.9, 2.6, int(4.5 * ppw)); ys = np.linspace(3.5, 0, int(3.5 * ppw))
    X, Y = np.meshgrid(xs, ys)
    ix, iy = sc.to_image(np.stack([X, Y, np.zeros_like(X)], -1))
    wall = Image.fromarray(np.clip(bilinear(sc.plate, ix, iy + sc.oy), 0, 255).astype(np.uint8))
    zs = np.linspace(0, 3.4, int(3.4 * ppw))
    X, Z = np.meshgrid(xs, zs)
    ix, iy = sc.to_image(np.stack([X, np.zeros_like(X), Z], -1))
    floor = Image.fromarray(np.clip(bilinear(sc.plate, ix, iy + sc.oy), 0, 255).astype(np.uint8))
    sheet = Image.new("RGB", (wall.width, wall.height + floor.height + 10), (255, 255, 255))
    sheet.paste(label(wall, "duvar, tam karşıdan (görünmeyen yerler kenar pikselleriyle uzar)"), (0, 0))
    sheet.paste(label(floor, "zemin, tepeden (üst kenar duvarın dibi)"), (0, wall.height + 10))
    sheet.resize((sheet.width // 2, sheet.height // 2), Image.LANCZOS).save(os.path.join(OUT, "karsidan.png"), optimize=True)
    print("written: baslangic_karsilastirma.png, zeyd_kareleri.png, karsidan.png")


if __name__ == "__main__":
    main()
