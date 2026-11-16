"""PM_76: the menu painting's camera arc. Copies Codex's angle layers (Assets/Art/_yeni/PM76/<set>/) into
Resources/Art/yeni/pm76/aci/: four 1180x1770 backgrounds (a0 is the menu's own view, enlarged 15 %, with the
canvas cut out), the foregrounds cropped to their content, and views.json (canvas corners and foreground boxes,
working pixels, top-left origin) for Dive75. Also the corridor's parallax layers (koridor/fg.png, wall.png).
    python Tools/import_pm76_angles.py [aci|aci_b]"""
import json, os, re, sys, uuid
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Art", "_yeni", "PM76")
DST = os.path.join(ROOT, "Assets", "_Game", "Presentation", "Resources", "Art", "yeni", "pm76")
TEMPLATE = os.path.join(ROOT, "Assets", "_Game", "Presentation", "Resources", "Art", "yeni", "pm74", "makas", "open.png.meta")


def meta_for(path, max_size=2048):
    template = open(TEMPLATE, encoding="utf-8").read()
    meta = path + ".meta"
    guid = None
    if os.path.exists(meta):
        m = re.search(r"^guid: (\w+)", open(meta, encoding="utf-8").read(), re.M)
        guid = m.group(1) if m else None
    text = re.sub(r"^guid: \w+", "guid: " + (guid or uuid.uuid4().hex), template, flags=re.M)
    text = re.sub(r"maxTextureSize: \d+", "maxTextureSize: %d" % max_size, text)
    text = re.sub(r"spritePixelsToUnits: \d+", "spritePixelsToUnits: 1", text)
    open(meta, "w", encoding="utf-8", newline="\n").write(text)


def main():
    angle_set = sys.argv[1] if len(sys.argv) > 1 else "aci"
    src = os.path.join(SRC, angle_set)
    cfg = json.load(open(os.path.join(src, "aci.json"), encoding="utf-8"))
    out = os.path.join(DST, "aci")
    os.makedirs(out, exist_ok=True)
    # 75 % of the working size (they are seen for under a second, moving): 884x1328, both multiples of 4 (ASTC
    # blocks); the corners and boxes scale with them
    S = .75
    W, H = 884, 1328
    sx, sy = W / 1180, H / 1770
    views = []
    for k in ["a0", "a1", "a2", "a3"]:
        a = cfg["angles"][k]
        bg = a.get("working_bg", a["bg"]); fg = a.get("working_fg", a["fg"])
        im = Image.open(os.path.join(src, bg)).convert("RGBA")
        assert im.size == (1180, 1770), (k, im.size)
        im.resize((W, H), Image.LANCZOS).save(os.path.join(out, k + "_bg.png"), optimize=True)
        meta_for(os.path.join(out, k + "_bg.png"))
        f = Image.open(os.path.join(src, fg)).convert("RGBA").resize((W, H), Image.LANCZOS)
        box = f.getchannel("A").point(lambda v: 255 if v > 8 else 0).getbbox() or (0, 0, 4, 4)
        x0, y0 = max(0, box[0] - 2) // 4 * 4, max(0, box[1] - 2) // 4 * 4
        x1, y1 = min(W, (box[2] + 5) // 4 * 4), min(H, (box[3] + 5) // 4 * 4)
        f.crop((x0, y0, x1, y1)).save(os.path.join(out, k + "_fg.png"), optimize=True)
        meta_for(os.path.join(out, k + "_fg.png"))
        canvas = [[round(c[0] * sx, 2), round(c[1] * sy, 2)] for c in a["canvas_corners"]]
        views.append({"name": k, "canvas": canvas, "fg": [x0, y0, x1, y1]})
    json.dump({"size": [W, H], "views": views}, open(os.path.join(out, "views.json"), "w"), indent=1)
    meta = os.path.join(out, "views.json.meta")
    if not os.path.exists(meta):
        open(meta, "w", newline="\n").write("fileFormatVersion: 2\nguid: %s\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n" % uuid.uuid4().hex)
    # the corridor's parallax layers
    kor = os.path.join(SRC, "koridor")
    if os.path.exists(os.path.join(kor, "wall.png")) and os.path.exists(os.path.join(kor, "fg.png")):
        o = os.path.join(DST, "koridor"); os.makedirs(o, exist_ok=True)
        Image.open(os.path.join(kor, "wall.png")).convert("RGBA").save(os.path.join(o, "wall.png"), optimize=True)
        meta_for(os.path.join(o, "wall.png"))
        # only the middle column is used (tiled once per bay): strip x 690..950
        Image.open(os.path.join(kor, "fg.png")).convert("RGBA").crop((690, 0, 950, 1688)).save(os.path.join(o, "fg.png"), optimize=True)
        meta_for(os.path.join(o, "fg.png"))
    print("imported", angle_set, [v["fg"] for v in views])


if __name__ == "__main__":
    main()
