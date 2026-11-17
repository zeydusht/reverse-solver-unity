"""PM_78: Codex's completed menu picture -> the game's diorama (Resources/Art/yeni/pm78/diorama/, read by
ArcAssets76 / Diorama78).
  oda.png         the plate (1408x1728, menu_bg at (0,128)): the wall and the floor, with the column, its banner and its
                  sconce painted out (Codex's oda_temiz) where those stand in front
  on_sutun.png    the column (capital to foot), from oda_flamasiz (its banner and sconce painted out)
  on_flama.png    the banner, on_aplik.png the sconce, from oda_genis
  diorama.json    the plate, the solved start camera (Tools/pm78_geom.py), Zeyd's keys, the cut-outs and their depths
The original picture is pasted back pixel for pixel over every Codex file first: only what menu_bg does not have comes
from Codex.
    python -I Tools/pm78_import.py [codex dir]"""
import json, math, os, sys
from PIL import Image, ImageChops, ImageFilter

sys.path.insert(0, os.path.dirname(__file__))
from pm78_geom import Menu, zeyd_keys, CANVAS, IMG_W, IMG_H   # noqa: E402

ROOT = os.path.join(os.path.dirname(__file__), "..")
MENU = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/menu_bg.png")
IN = os.path.join(ROOT, "Assets/Art/_yeni/PM78/girdi")
CODEX = os.path.join(ROOT, "Assets/Art/_yeni/PM78/oda")
DST = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/yeni/pm78/diorama")
PAD_TOP, PLATE = 128, (1408, 1728)
DEPTH = {"on_sutun": .05, "on_flama": .11, "on_aplik": .17}   # W in front of the wall (the canvas is 1 wide)


def grown(m, px):
    return m.filter(ImageFilter.MaxFilter(px * 2 + 1)) if px else m


def with_original(img, menu, keep=None):
    """img with menu_bg pasted back at (0, PAD_TOP), except inside `keep` (L mask, plate size)."""
    out = img.convert("RGB").resize(PLATE, Image.LANCZOS) if img.size != PLATE else img.convert("RGB")
    orig = out.copy(); orig.paste(menu, (0, PAD_TOP))
    inside = Image.new("L", PLATE, 0); inside.paste(255, (0, PAD_TOP, IMG_W, PAD_TOP + IMG_H))
    if keep is not None: inside = ImageChops.subtract(inside, keep)
    return Image.composite(orig, out, inside)


def soft(mask):
    return mask.filter(ImageFilter.GaussianBlur(.6))


def cut(src, mask, name):
    """The masked part of src as an RGBA cut-out, its rect a multiple of 4; returns (png, rect in picture px)."""
    alpha = soft(mask)
    x0, y0, x1, y1 = mask.getbbox()
    x0 -= 2; y0 -= 2; x1 += 2; y1 += 2
    w = (x1 - x0 + 3) // 4 * 4; h = (y1 - y0 + 3) // 4 * 4
    x1, y1 = x0 + w, y0 + h
    rgba = src.crop((x0, y0, x1, y1)).convert("RGBA")
    rgba.putalpha(alpha.crop((x0, y0, x1, y1)))
    rgba.save(os.path.join(DST, name + ".png"))
    return [x0, y0 - PAD_TOP, x1, y1 - PAD_TOP]


def main():
    codex = sys.argv[1] if len(sys.argv) > 1 else CODEX
    os.makedirs(DST, exist_ok=True)
    menu = Image.open(MENU).convert("RGB")
    m_col = Image.open(os.path.join(IN, "maske_sutun.png")).convert("L")
    m_ban = Image.open(os.path.join(IN, "maske_flama.png")).convert("L")
    m_sco = Image.open(os.path.join(IN, "maske_aplik.png")).convert("L")
    genis = with_original(Image.open(os.path.join(codex, "oda_genis.png")), menu)
    flamasiz = with_original(Image.open(os.path.join(codex, "oda_flamasiz.png")), menu, grown(ImageChops.lighter(m_ban, m_sco), 4))
    temiz = with_original(Image.open(os.path.join(codex, "oda_temiz.png")), menu, grown(ImageChops.lighter(m_col, ImageChops.lighter(m_ban, m_sco)), 4))
    # Codex's painted-out pixels only where a cut-out covers them fully (alpha 1) in the start view; wherever a cut-out
    # is see-through (its soft edge, outside it) the picture's own pixels: the start view stays the menu picture exactly
    def covered(*masks):
        a = masks[0]
        for x in masks[1:]: a = ImageChops.lighter(a, x)
        return soft(a).point(lambda v: 255 if v >= 253 else 0)
    flamasiz = Image.composite(flamasiz, genis, covered(m_ban, m_sco))
    temiz = Image.composite(temiz, genis, covered(m_col, m_ban, m_sco))
    temiz.save(os.path.join(DST, "oda.png"))
    fronts = []
    for name, src, m in (("on_sutun", flamasiz, m_col), ("on_flama", genis, m_ban), ("on_aplik", genis, m_sco)):
        fronts.append({"name": name, "px": cut(src, m, name), "depth": DEPTH[name]})
    keys, durs, z = zeyd_keys()
    m = Menu(math.degrees(math.atan2(-keys[0][0], keys[0][1])))
    data = {
        "source": "menu_bg.png at (0,128) + Codex's completion (docs/art/pm78/CODEX_PM78.md); Tools/pm78_import.py",
        "plate_size": list(PLATE), "image_origin": [0, PAD_TOP], "image_size": [IMG_W, IMG_H],
        "f": round(m.f, 3), "cx": round(m.cx, 3), "cy": round(m.cy, 3),
        "start_deg": round(math.degrees(m.theta), 3), "d": round(m.D, 4), "h": round(m.h, 4), "yc": round(m.yc, 4),
        "canvas_px": [list(p) for p in CANVAS],
        "keys": [list(k) for k in keys], "durs": durs, "seconds": z["toplam_sn"],
        "fronts": fronts,
    }
    with open(os.path.join(DST, "diorama.json"), "w", encoding="utf8") as f:
        json.dump(data, f, indent=1)
    for n in os.listdir(DST):
        if n.endswith(".png"): print(n, Image.open(os.path.join(DST, n)).size, os.path.getsize(os.path.join(DST, n)) // 1024, "KB")
    print("fronts", fronts)


if __name__ == "__main__":
    main()
