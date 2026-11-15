"""PM_73: the chosen T50 parts (win B, map C) for the build.

Copies Assets/Art/_yeni/T50/{kazanma/B,harita/C} into Resources/Art/yeni/T50/{win,map}; the two button faces lose
their baked Turkish words (the game writes the label in its own font, so English works too), and the full-screen
glow is halved (it is a soft gradient). Metas come from an existing @3x sprite (pm72-gear) with a new guid.
  python Tools/make_pm73_art.py
"""
import hashlib, os, re, shutil
import numpy as np
from PIL import Image, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "Assets/Art/_yeni/T50")
DST = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/yeni/T50")
META = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/pm72-gear.png.meta")

WIN = ["win_bg_glow", "win_title_tr", "win_title_en", "win_jigo_jump", "coin", "chest_bar_bg", "chest_bar_fill",
       "btn_green_big", "btn_blue_small"]
MAP = ["node_completed", "node_current", "node_hard", "node_lesson", "node_locked", "badge_bomb", "badge_chain",
       "badge_check", "badge_kont", "badge_lock", "badge_wall", "jigo_marker", "path_dot"]


def blank_button(img, fill_test, keep=22, ramp=14):
    """Removes the baked label: every row's inner fill span (its rounded ends kept) becomes the row's median clean
    fill colour, blended into the original over a short ramp; the rows are then smoothed vertically."""
    a = np.array(img).astype(np.float32)
    h, w, _ = a.shape
    fill = fill_test(a) & (a[..., 3] > 250)
    out = a.copy()
    rows = np.zeros((h, 3), np.float32); spans = []
    for y in range(h):
        xs = np.nonzero(fill[y])[0]
        if len(xs) < 40:
            spans.append(None); continue
        x0, x1 = xs.min() + keep, xs.max() - keep
        clean = xs[(xs < x0 + 30) | (xs > x1 - 30)]   # beside the words
        rows[y] = np.median(a[y, clean if len(clean) > 4 else xs, :3], axis=0)
        spans.append((x0, x1))
    ok = [y for y in range(h) if spans[y]]
    # rows where the words cover almost all of the fill: span and colour from the nearest clean rows
    for y in range(ok[0], ok[-1] + 1):
        if spans[y]:
            continue
        up = max(k for k in ok if k < y); dn = min(k for k in ok if k > y)
        t = (y - up) / (dn - up)
        rows[y] = rows[up] * (1 - t) + rows[dn] * t
        spans[y] = (max(spans[up][0], spans[dn][0]), min(spans[up][1], spans[dn][1]))
    ok = [y for y in range(h) if spans[y]]
    sm = rows.copy()
    for y in ok:   # vertical smoothing of the row colours
        near = [k for k in range(y - 2, y + 3) if 0 <= k < h and spans[k]]
        sm[y] = rows[near].mean(axis=0)
    for y in ok:
        x0, x1 = spans[y]
        for x in range(x0, x1 + 1):
            t = min(1.0, (x - x0) / ramp, (x1 - x) / ramp)
            out[y, x, :3] = a[y, x, :3] * (1 - t) + sm[y] * t
    return Image.fromarray(out.clip(0, 255).astype(np.uint8))


def green(a):
    return (a[..., 1] > a[..., 0] + 40) & (a[..., 1] > a[..., 2] + 30) & (a[..., 1] > 100)


def blue(a):
    return (a[..., 2] > a[..., 0] + 60) & (a[..., 2] > a[..., 1] + 40) & (a[..., 2] > 140)


def pad4(img):
    """Block compression needs sides in multiples of 4 (MenuArtImport crunches Resources/Art)."""
    w, h = (img.width + 3) // 4 * 4, (img.height + 3) // 4 * 4
    if (w, h) == img.size:
        return img
    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    out.paste(img, ((w - img.width) // 2, (h - img.height) // 2))
    return out


def write_meta(path):
    text = open(META, encoding="utf-8").read()
    guid = hashlib.md5(("pm73/" + os.path.relpath(path, ROOT).replace("\\", "/")).encode()).hexdigest()
    text = re.sub(r"guid: [0-9a-f]{32}", "guid: " + guid, text, count=1)
    open(path + ".meta", "w", encoding="utf-8", newline="\n").write(text)


def main():
    for sub, names, folder in (("kazanma/B", WIN, "win"), ("harita/C", MAP, "map")):
        os.makedirs(os.path.join(DST, folder), exist_ok=True)
        for n in names:
            img = Image.open(os.path.join(SRC, sub, n + ".png")).convert("RGBA")
            if n == "btn_green_big":
                img = blank_button(img, green)
            elif n == "btn_blue_small":
                img = blank_button(img, blue)
            elif n == "win_bg_glow":
                img = img.resize((472, 1024), Image.LANCZOS)   # the build caps yeni/ art at 1024 px
            elif n == "jigo_marker":
                img = img.crop(img.getbbox())                   # the map sizes Jigo by his own height
            img = pad4(img)
            out = os.path.join(DST, folder, n + ".png")
            img.save(out, optimize=True)
            if not os.path.exists(out + ".meta"):
                write_meta(out)
    for folder in ("win", "map"):
        d = os.path.join(DST, folder)
        if not os.path.exists(d + ".meta"):
            guid = hashlib.md5(("pm73/" + folder).encode()).hexdigest()
            open(d + ".meta", "w", newline="\n").write(f"fileFormatVersion: 2\nguid: {guid}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")
    if not os.path.exists(DST + ".meta"):
        guid = hashlib.md5(b"pm73/T50").hexdigest()
        open(DST + ".meta", "w", newline="\n").write(f"fileFormatVersion: 2\nguid: {guid}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")
    print("ok", DST)


if __name__ == "__main__":
    main()
