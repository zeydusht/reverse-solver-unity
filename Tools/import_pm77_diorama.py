"""PM_77: the menu room's diorama layers (Codex, Assets/Art/_yeni/PM77/diorama/) into
Resources/Art/yeni/pm77/diorama/ at 4096 px wide at most, every side a multiple of 4 (block compression;
Assets/_Game/Editor/MenuArtImport.cs crunches them), with diorama.json (sizes, canvas, px per unit) for Diorama77.
    python Tools/import_pm77_diorama.py"""
import json, os, sys
import numpy as np
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Art", "_yeni", "PM77", "diorama")
DST = os.path.join(ROOT, "Assets", "_Game", "Presentation", "Resources", "Art", "yeni", "pm77", "diorama")


def four(v):
    return max(4, int(round(v / 4.0)) * 4)


# The menu's painting nearly fills its bay, the pilasters (banner, sconce) just beside the frame; Codex's wall has
# 0.4 units of plain wall on each side. Narrowed here by content-aware seams (seam carving) inside the plain strips
# between the pilasters and the frame only (source px; the frame, the pilasters and the moldings are kept whole).
NARROW = [(2034, 2184, 110), (2612, 2768, 110)]   # (x0, x1, px removed) in the 4800 px source


def carve(im, x0, x1, n):
    a = np.asarray(im).astype(np.float32)
    for _ in range(n):
        reg = a[:, x0:x1, :3]
        gx = np.abs(np.diff(reg, axis=1, append=reg[:, -1:])).sum(axis=2)
        gy = np.abs(np.diff(reg, axis=0, append=reg[-1:])).sum(axis=2)
        e = gx + gy
        h, w = e.shape
        m = e.copy(); back = np.zeros((h, w), np.int32)
        for y in range(1, h):
            prev = m[y - 1]
            l = np.concatenate(([np.inf], prev[:-1])); r = np.concatenate((prev[1:], [np.inf]))
            stack = np.stack((l, prev, r)); k = stack.argmin(axis=0)
            m[y] += stack[k, np.arange(w)]; back[y] = np.arange(w) + k - 1
        x = int(m[-1].argmin()); seam = np.zeros(h, np.int32)
        for y in range(h - 1, -1, -1):
            seam[y] = x; x = back[y, x]
        keep = np.ones(a.shape[:2], bool); keep[np.arange(h), x0 + seam] = False
        a = a[keep].reshape(h, a.shape[1] - 1, a.shape[2])
        x1 -= 1
    return Image.fromarray(a.clip(0, 255).astype(np.uint8), im.mode)


def narrow(wall, canvas):
    removed_left = 0
    for x0, x1, n in sorted(NARROW, reverse=True):   # right strip first: the left one's coordinates stay valid
        wall = carve(wall, x0, x1, n)
        if x1 <= canvas[0]: removed_left += n
    return wall, [canvas[0] - removed_left, canvas[1], canvas[2] - removed_left, canvas[3]]


def main():
    os.makedirs(DST, exist_ok=True)
    cfg = json.load(open(os.path.join(SRC, "diorama.json"), encoding="utf-8")) if os.path.exists(os.path.join(SRC, "diorama.json")) else {}
    canvas = cfg.get("canvas", {}).get("rect_px", [2244, 974, 2556, 1374])   # Codex: canvas.rect_px, x0 y0 x1 y1
    wall, canvas = narrow(Image.open(os.path.join(SRC, "duvar.png")).convert("RGBA"), canvas)
    scale = min(1.0, 4096.0 / 4800)   # every layer at the same scale (the wall was 4800 px before narrowing)
    sizes = {}
    for name in ["duvar", "zemin", "on_sutun"]:
        im = wall if name == "duvar" else Image.open(os.path.join(SRC, name + ".png")).convert("RGBA")
        w, h = four(im.width * scale), four(im.height * scale)
        if name == "on_sutun" and h > 2048:   # the column under 2048 px tall
            k = 2048.0 / h; w, h = four(w * k), 2048
        im.resize((w, h), Image.LANCZOS).save(os.path.join(DST, name + ".png"), optimize=True)
        sizes[name] = (w, h, im.width, im.height)
    ww, wh, w0, h0 = sizes["duvar"]
    sx, sy = ww / w0, wh / h0
    ppu = float(cfg.get("pixels_per_unit", 400))
    cw, ch, c0w, c0h = sizes["on_sutun"]
    out = {
        "wall_size": [ww, wh], "floor_size": list(sizes["zemin"][:2]), "column_size": [cw, ch],
        "canvas": [round(canvas[0] * sx, 2), round(canvas[1] * sy, 2), round(canvas[2] * sx, 2), round(canvas[3] * sy, 2)],
        "px_per_unit": round(ppu * sx, 4),
        # the floor and the column keep their own scale: Diorama77 reads them in the wall's units
        "floor_px_per_unit": round(ppu * sizes["zemin"][0] / sizes["zemin"][2], 4),
        "column_px_per_unit": round(ppu * cw / c0w, 4),
    }
    json.dump(out, open(os.path.join(DST, "diorama.json"), "w"), indent=1)
    print("imported", out)


if __name__ == "__main__":
    sys.exit(main())
