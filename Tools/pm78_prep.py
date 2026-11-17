"""PM_78: Codex's inputs for completing the ORIGINAL menu picture (menu_bg.png), not a new room.
  - oda_girdi.png  1408x1728: menu_bg placed at (0, 128); what the camera path sees past its edges is magenta
  - maske_sutun / maske_flama / maske_aplik.png: the right column (capital to foot), its banner, its sconce (white)
  - maskeler_onizleme.png: the three masks over the picture
The plate's pixel grid is menu_bg's own (x the same, y + 128), so the diorama's first frame redraws menu_bg exactly.
    python -I Tools/pm78_prep.py"""
import os
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/menu_bg.png")
OUT = os.path.join(ROOT, "Assets/Art/_yeni/PM78/girdi")
PAD_TOP, PLATE = 128, (1408, 1728)

# menu_bg pixels (x, y), traced on 3x / 2x crops
COLUMN = [(758, 190), (864, 190), (866, 240), (862, 272), (858, 282), (857, 955), (866, 958), (870, 1126), (778, 1126),
          (778, 962), (785, 955), (782, 282), (762, 272), (756, 230)]
BANNER = [(818, 262), (842, 264), (846, 272), (860, 286), (870, 292), (872, 302), (866, 308), (866, 548), (862, 560),
          (852, 556), (847, 586), (840, 586), (834, 560), (826, 560), (822, 540), (822, 300), (816, 282)]
SCONCE = [(824, 684), (832, 676), (842, 676), (850, 668), (851, 654), (860, 652), (862, 668), (862, 690), (868, 698),
          (870, 716), (864, 726), (852, 728), (843, 738), (838, 768), (828, 768), (822, 740), (822, 700)]


def mask(poly, grow=0):
    m = Image.new("L", PLATE, 0)
    ImageDraw.Draw(m).polygon([(x, y + PAD_TOP) for x, y in poly], fill=255)
    if grow: m = m.filter(ImageFilter.MaxFilter(grow * 2 + 1))
    return m


def main():
    os.makedirs(OUT, exist_ok=True)
    src = Image.open(SRC).convert("RGB")
    plate = Image.new("RGB", PLATE, (255, 0, 255))
    plate.paste(src, (0, PAD_TOP))
    plate.save(os.path.join(OUT, "oda_girdi.png"))
    for name, poly in (("sutun", COLUMN), ("flama", BANNER), ("aplik", SCONCE)):
        mask(poly).save(os.path.join(OUT, f"maske_{name}.png"))
    prev = plate.copy()
    tint = Image.new("RGB", PLATE)
    for poly, col in ((COLUMN, (40, 120, 255)), (BANNER, (255, 220, 0)), (SCONCE, (0, 255, 120))):
        tint.paste(col, (0, 0, *PLATE)); prev = Image.composite(Image.blend(prev, tint, .55), prev, mask(poly))
    prev.crop((620, 0, 1100, 1350)).save(os.path.join(OUT, "maskeler_onizleme.png"))
    print("written to", OUT)


if __name__ == "__main__":
    main()
