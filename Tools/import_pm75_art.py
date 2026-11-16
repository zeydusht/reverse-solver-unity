"""PM_75: copies the Codex art (Assets/Art/_yeni/PM75) into Resources/Art/yeni/pm75 as @3x sprites (3 px per pt),
the import settings of the PM_74 effect sprites; the name plates get 9-slice borders (the ring on the left, the
rounded end on the right) so the plate stretches without bending its ring.
    python Tools/import_pm75_art.py"""
import os, re, shutil, uuid

ROOT = os.path.join(os.path.dirname(__file__), "..")
SRC = os.path.join(ROOT, "Assets", "Art", "_yeni", "PM75")
DST = os.path.join(ROOT, "Assets", "_Game", "Presentation", "Resources", "Art", "yeni", "pm75")
TEMPLATE = os.path.join(ROOT, "Assets", "_Game", "Presentation", "Resources", "Art", "yeni", "pm74", "makas", "open.png.meta")
BORDERS = {"name_plate": "{x: 196, y: 0, z: 64, w: 0}"}   # left, bottom, right, top (px)


def main():
    template = open(TEMPLATE, encoding="utf-8").read()
    n = 0
    for folder, _, files in os.walk(SRC):
        for f in files:
            if not f.endswith(".png"):
                continue
            rel = os.path.relpath(os.path.join(folder, f), SRC)
            out = os.path.join(DST, rel)
            os.makedirs(os.path.dirname(out), exist_ok=True)
            shutil.copyfile(os.path.join(folder, f), out)
            meta = out + ".meta"
            guid = None
            if os.path.exists(meta):
                m = re.search(r"^guid: (\w+)", open(meta, encoding="utf-8").read(), re.M)
                guid = m.group(1) if m else None
            text = re.sub(r"^guid: \w+", "guid: " + (guid or uuid.uuid4().hex), template, flags=re.M)
            border = next((b for k, b in BORDERS.items() if k in f), "{x: 0, y: 0, z: 0, w: 0}")
            text = re.sub(r"spriteBorder: \{[^}]*\}", "spriteBorder: " + border, text)
            open(meta, "w", encoding="utf-8", newline="\n").write(text)
            n += 1
    print("imported", n, "sprites ->", os.path.normpath(DST))


if __name__ == "__main__":
    main()
