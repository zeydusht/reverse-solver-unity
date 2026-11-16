"""PM_74: Codex's booster parts (Assets/Art/_yeni/PM74/{saat,efekt/...}) for the build.

Copies the chosen PNGs into Resources/Art/yeni/pm74/{saat,cekic,makas,degnek} (the keys BoosterFx74 asks for), trimmed
to their content (the scissors poses and the clock layers keep their full canvas: they are stacked on a shared pivot),
scaled so the long side is at most 512 px and padded to multiples of 4. Metas come from an existing @3x sprite
(pm72-gear) with a new guid.
  python Tools/make_pm74_art.py
"""
import hashlib, os, re, sys
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "Assets/Art/_yeni/PM74")
DST = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/yeni/pm74")
META = os.path.join(ROOT, "Assets/_Game/Presentation/Resources/Art/pm72-gear.png.meta")

# (source relative to SRC, destination key, keep the full canvas)
PARTS = [
    ("saat/clock_big.png", "saat/clock_big", True),
    ("saat/bells_ring_01.png", "saat/bells_ring_01", True),
    ("saat/bells_ring_02.png", "saat/bells_ring_02", True),
    ("saat/bells_ring_03.png", "saat/bells_ring_03", True),
    ("saat/frost_burst_01.png", "saat/frost_burst_01", True),
    ("saat/frost_burst_02.png", "saat/frost_burst_02", True),
    ("saat/frost_burst_03.png", "saat/frost_burst_03", True),
    ("saat/frost_burst_04.png", "saat/frost_burst_04", True),
    ("saat/frost_burst_05.png", "saat/frost_burst_05", True),
    ("saat/frost_burst_06.png", "saat/frost_burst_06", True),
    ("saat/snowflake.png", "saat/snowflake", False),
    ("saat/ice_glaze.png", "saat/ice_glaze", True),
    ("saat/glow_ring.png", "saat/glow_ring", True),
    ("saat/hand_long.png", "saat/hand_long", True),
    ("saat/hand_short.png", "saat/hand_short", True),
    ("efekt/makas/open.png", "makas/open", True),
    ("efekt/makas/half.png", "makas/half", True),
    ("efekt/makas/closed.png", "makas/closed", True),
    ("efekt/makas/link_a.png", "makas/link_a", False),
    ("efekt/makas/link_b.png", "makas/link_b", False),
    ("efekt/makas/snip_flash.png", "makas/snip_flash", True),
    ("efekt/cekic/crack_01.png", "cekic/crack_01", True),
    ("efekt/cekic/crack_02.png", "cekic/crack_02", True),
    ("efekt/cekic/crack_03.png", "cekic/crack_03", True),
    ("efekt/cekic/shard_a.png", "cekic/shard_a", False),
    ("efekt/cekic/shard_b.png", "cekic/shard_b", False),
    ("efekt/cekic/shard_c.png", "cekic/shard_c", False),
    ("efekt/cekic/spark.png", "cekic/spark", False),
    ("efekt/cekic/dust.png", "cekic/dust", True),
    ("efekt/degnek/sparkle.png", "degnek/sparkle", False),
    ("efekt/degnek/swirl_01.png", "degnek/swirl_01", True),
    ("efekt/degnek/swirl_02.png", "degnek/swirl_02", True),
    ("efekt/degnek/swirl_03.png", "degnek/swirl_03", True),
    ("efekt/degnek/swirl_04.png", "degnek/swirl_04", True),
    ("efekt/degnek/swirl_05.png", "degnek/swirl_05", True),
    ("efekt/degnek/swirl_06.png", "degnek/swirl_06", True),
]


def pad4(img):
    w, h = (img.width + 3) // 4 * 4, (img.height + 3) // 4 * 4
    if (w, h) == img.size:
        return img
    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    out.paste(img, ((w - img.width) // 2, (h - img.height) // 2))
    return out


def write_meta(path):
    text = open(META, encoding="utf-8").read()
    guid = hashlib.md5(("pm74/" + os.path.relpath(path, ROOT).replace("\\", "/")).encode()).hexdigest()
    text = re.sub(r"guid: [0-9a-f]{32}", "guid: " + guid, text, count=1)
    open(path + ".meta", "w", encoding="utf-8", newline="\n").write(text)


def main(only=None):
    done, missing = 0, []
    for src, key, full in PARTS:
        if only and not key.startswith(only):
            continue
        path = os.path.join(SRC, src)
        if not os.path.exists(path):
            missing.append(src); continue
        img = Image.open(path).convert("RGBA")
        if not full:
            box = img.getchannel("A").point(lambda v: 255 if v > 8 else 0).getbbox()
            if box:
                img = img.crop(box)
        if max(img.size) > 512:
            k = 512 / max(img.size)
            img = img.resize((max(1, round(img.width * k)), max(1, round(img.height * k))), Image.LANCZOS)
        img = pad4(img)
        out = os.path.join(DST, key + ".png")
        os.makedirs(os.path.dirname(out), exist_ok=True)
        img.save(out, optimize=True)
        if not os.path.exists(out + ".meta"):
            write_meta(out)
        done += 1
    print(f"[pm74-art] {done} parts copied" + (f"; missing: {', '.join(missing)}" if missing else ""))


if __name__ == "__main__":
    main(sys.argv[1] if len(sys.argv) > 1 else None)
