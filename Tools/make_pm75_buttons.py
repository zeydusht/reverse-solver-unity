"""PM_75: the small blue member of the menu button family ("Menü", "Hayır"): the menu's green "level-button"
(and its pressed state) recoloured to the game's navy blue, same shape, gold rim and 9-slice borders.
    python Tools/make_pm75_buttons.py"""
import colorsys, os, shutil
from PIL import Image

ART = os.path.join(os.path.dirname(__file__), "..", "Assets", "_Game", "Presentation", "Resources", "Art")


def recolour(src, dst):
    im = Image.open(os.path.join(ART, src + ".png")).convert("RGBA")
    px = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r, g, b, a = px[x, y]
            if a == 0:
                continue
            h, l, s = colorsys.rgb_to_hls(r / 255, g / 255, b / 255)
            # the green body (hue ~ 80-160 deg) turns navy blue (~ 216 deg); the gold rim and highlights stay
            if .2 <= h <= .47 and s > .25:
                h = .60
                l = l * .86
                s = min(1, s * .95)
            r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
            px[x, y] = (int(r2 * 255), int(g2 * 255), int(b2 * 255), a)
    im.save(os.path.join(ART, dst + ".png"))
    meta_src = os.path.join(ART, src + ".png.meta")
    meta_dst = os.path.join(ART, dst + ".png.meta")
    if not os.path.exists(meta_dst):
        # same import settings (9-slice borders), a new guid
        import uuid
        text = open(meta_src, encoding="utf-8").read().splitlines()
        text = [("guid: " + uuid.uuid4().hex) if line.startswith("guid: ") else line for line in text]
        open(meta_dst, "w", encoding="utf-8", newline="\n").write("\n".join(text) + "\n")
    print("wrote", dst)


if __name__ == "__main__":
    recolour("level-button", "level-button-blue")
    recolour("level-button-pressed", "level-button-blue-pressed")
