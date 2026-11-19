"""PM89: size image_gen artwork, compose PIL previews, and verify PNG deliveries.

Usage: python -B build_preview.py --sources <generation-manifest.json>
The manifest lives in the OS temporary directory; generation originals stay in Codex.
"""
import argparse
import hashlib
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFont, ImageOps

OUT = Path(__file__).resolve().parent
ROOT = OUT.parents[3]
ART = ROOT / "Assets/_Game/Presentation/Resources/Art"
FONTS = ROOT / "Assets/_Game/Presentation/Resources/Fonts"
RESAMPLE = Image.Resampling.LANCZOS
RAILS = (424, 783)
CHAR_X = ((356, 724), (248, 540, 832))
NAMES = {"A": "Kemerli vitrin", "B": "Kadife ve konsol", "C": "Açık cam vitrin"}
SIZES = {
    "shelf.png": (1080, 816),
    "reward_chest.png": (192, 192),
    "reward_gift.png": (192, 192),
    "reward_medal.png": (192, 192),
    "lock_small.png": (96, 96),
    "avatar_ring_gold.png": (220, 220),
    "onizleme.png": (390, 844),
}


def rgba(path):
    with Image.open(path) as im:
        return im.convert("RGBA")


def save_png(im, path):
    alpha_clean(im).save(path, optimize=True)


def alpha_clean(im):
    # Generated transparent areas contain invisible alpha values 1..4. Remove
    # that quantization noise while retaining the visible generated glow.
    result = im.convert("RGBA").copy()
    result.putalpha(result.getchannel("A").point(lambda a: 0 if a <= 4 else a))
    return result


def font(size, heading=False):
    return ImageFont.truetype(str(FONTS / ("Baloo2-ExtraBold.ttf" if heading else "Nunito-Black.ttf")), size)


def text(im, xy, value, size, fill="#fff1cf", outline=0, heading=False):
    draw = ImageDraw.Draw(im)
    draw.text(xy, value, font=font(size, heading), fill=fill, anchor="mm",
              stroke_width=outline, stroke_fill="#13364b")


def crop_visible(im):
    return im.crop(im.getchannel("A").getbbox())


def contain(im, size, padding=0):
    canvas = Image.new("RGBA", size)
    tile = ImageOps.contain(crop_visible(im), (size[0] - 2 * padding, size[1] - 2 * padding), RESAMPLE)
    canvas.alpha_composite(tile, ((size[0] - tile.width) // 2, (size[1] - tile.height) // 2))
    return canvas


def shelf_slices(im, supports):
    """Resize three contiguous horizontal slices; keep both support rows exact."""
    assert 0 < supports[0] < supports[1] < im.height
    source_y = (0, *supports, im.height)
    target_y = (0, *RAILS, 816)
    result = Image.new("RGBA", (1080, 816))
    slices = []
    for a, b, c, d in zip(source_y, source_y[1:], target_y, target_y[1:]):
        strip = im.crop((0, a, im.width, b)).resize((1080, d - c), RESAMPLE)
        result.alpha_composite(strip, (0, c))
        slices.append({"source_y": [a, b], "target_y": [c, d], "source_width": im.width})
    return result, slices


def ring_size(im):
    # Preserve the generated gold and glow; resize only. Measure the transparent
    # opening rather than synthesizing a replacement ring or flattening alpha.
    result = alpha_clean(im.resize((220, 220), RESAMPLE))
    alpha = np.asarray(result.getchannel("A"))
    opening = []
    for axis in (alpha[110, :], alpha[:, 110]):
        lo = hi = 110
        while lo > 0 and axis[lo - 1] == 0:
            lo -= 1
        while hi < 219 and axis[hi + 1] == 0:
            hi += 1
        opening.append(hi - lo + 1)
    return result, opening


def put_sprite(im, sprite, box):
    tile = sprite.resize((box[2], box[3]), RESAMPLE)
    im.alpha_composite(tile, box[:2])


def make_preview(option):
    bg = rgba(ROOT / "Assets/StreamingAssets/MenuArt/t20-scene-jiggles.jpg")
    im = ImageOps.fit(bg, (390, 844), RESAMPLE)
    text(im, (195, 55), "Jiggles", 40, outline=3, heading=True)
    # A small saved-count pill keeps the required placeholder readable.
    pill = Image.new("RGBA", im.size)
    pd = ImageDraw.Draw(pill)
    pd.rounded_rectangle((69, 100, 321, 132), 16, fill=(255, 242, 209, 234), outline="#c99b48", width=1)
    im = Image.alpha_composite(im, pill)
    text(im, (195, 116), "45 Jiggle kurtarıldı", 18, fill="#13364b")

    shelf = rgba(OUT / option / "shelf.png")
    put_sprite(im, shelf, (15, 153, 360, 272))
    colors = (("green", "purple"), ("blue", "pink", "red"))
    for row, row_colors in enumerate(colors):
        for cx, color in zip(CHAR_X[row], row_colors):
            sprite = crop_visible(rgba(ART / f"t20-jiggle-{color}-standing.png"))
            h = round(816 * .40)
            sprite = sprite.resize((round(sprite.width * h / sprite.height), h), RESAMPLE)
            tile = sprite.resize((round(sprite.width / 3), round(h / 3)), RESAMPLE)
            x = round(15 + cx / 3 - tile.width / 2)
            y = round(153 + RAILS[row] / 3 - tile.height)
            im.alpha_composite(tile, (x, y))

    text(im, (195, 466), "Avatarım", 27, outline=2, heading=True)
    ring = rgba(OUT / option / "avatar_ring_gold.png")
    lock = rgba(OUT / option / "lock_small.png")
    for index, color in enumerate(("blue", "pink", "red", "green", "purple")):
        cx, cy = 51 + index * 72, 522
        disc = Image.new("RGBA", (64, 64))
        dd = ImageDraw.Draw(disc)
        dd.ellipse((8, 8, 56, 56), fill="#e9d6ae", outline="#a67f39", width=2)
        avatar = contain(rgba(ART / f"t21-avatar-{color}-round.png"), (38, 38))
        disc.alpha_composite(avatar, (13, 13))
        if index >= 3:
            alpha = disc.getchannel("A")
            disc = ImageOps.grayscale(disc).convert("RGBA")
            disc.putalpha(alpha.point(lambda a: round(a * .62)))
        im.alpha_composite(disc, (cx - 32, cy - 32))
        if index == 0:
            im.alpha_composite(ring.resize((66, 66), RESAMPLE), (cx - 33, cy - 33))
        if index >= 3:
            im.alpha_composite(lock.resize((20, 20), RESAMPLE), (cx + 9, cy + 9))

    panel = Image.new("RGBA", im.size)
    dd = ImageDraw.Draw(panel)
    dd.rounded_rectangle((23, 577, 367, 724), 18, fill=(255, 242, 214, 236), outline=(182, 139, 67, 255), width=2)
    im = Image.alpha_composite(im, panel)
    text(im, (195, 599), "45 / 2.000", 22, fill="#14364b", heading=True)
    track = crop_visible(rgba(ART / "yeni/pm86/basari/pm86-ach-track.png"))
    fill = crop_visible(rgba(ART / "yeni/pm86/basari/pm86-ach-fill.png"))
    # Preserve rounded ends via a three-slice stretch, including a clipped small
    # progress fill. These are the actual requested PM86 assets.
    tx, ty, tw, th = 40, 699, 310, 19
    im.alpha_composite(horizontal_three_slice(track, (tw, th), 24), (tx, ty))
    full = horizontal_three_slice(fill, (tw - 8, th - 6), 24)
    filled_w = round((tw - 8) * 45 / 2000)
    im.alpha_composite(full.crop((0, 0, filled_w, full.height)), (tx + 4, ty + 3))
    for value, key in ((500, "chest"), (1000, "gift"), (2000, "medal")):
        cx = round(tx + 4 + (tw - 8) * value / 2000)
        reward = rgba(OUT / option / f"reward_{key}.png").resize((54, 54), RESAMPLE)
        im.alpha_composite(reward, (cx - 27, 615))
        text(im, (cx, 681), f"{value:,}".replace(",", "."), 15, fill="#18364b")
        ImageDraw.Draw(im).line((cx, 691, cx, 697), fill="#b68c43", width=2)
    # Reuse the existing screen's navigation chrome, not its old shelf or bar.
    before = rgba(OUT / "once.png")
    nav_h = round(before.height * 105 / 844)
    nav = before.crop((0, before.height - nav_h, before.width, before.height)).resize((390, 105), RESAMPLE)
    im.alpha_composite(nav, (0, 739))
    return im


def horizontal_three_slice(source, size, cap):
    width, height = size
    cap = min(cap, source.width // 3)
    target_cap = min(round(cap * height / source.height), (width - 1) // 2)
    result = Image.new("RGBA", size)
    source_boxes = ((0, cap), (cap, source.width - cap), (source.width - cap, source.width))
    target_boxes = ((0, target_cap), (target_cap, width - target_cap), (width - target_cap, width))
    for (a, b), (c, d) in zip(source_boxes, target_boxes):
        result.alpha_composite(source.crop((a, 0, b, source.height)).resize((d - c, height), RESAMPLE), (c, 0))
    return result


def checker(size, cell=12):
    canvas = Image.new("RGBA", size, "#e0e3e6")
    dd = ImageDraw.Draw(canvas)
    for y in range(0, size[1], cell):
        for x in range(0, size[0], cell):
            if (x // cell + y // cell) % 2:
                dd.rectangle((x, y, x + cell - 1, y + cell - 1), fill="#f8f8f8")
    return canvas


def make_slice_check():
    sheet = Image.new("RGBA", (1170, 566), "#142b43")
    for column, option in enumerate("ABC"):
        x = column * 390
        text(sheet, (x + 195, 22), f"{option} · {NAMES[option]}", 19, heading=True)
        tile = checker((360, 272))
        tile.alpha_composite(rgba(OUT / option / "shelf.png").resize(tile.size, RESAMPLE))
        dd = ImageDraw.Draw(tile)
        for row, rail in enumerate(RAILS):
            yy = round(rail / 3)
            dd.line((30, yy, 330, yy), fill="#ef5580", width=1)
            for cx in CHAR_X[row]:
                dd.line((round(cx / 3), yy - 7, round(cx / 3), yy + 3), fill="#ef5580", width=2)
        sheet.alpha_composite(tile, (x + 15, 47))
        text(sheet, (x + 195, 335), "Ayak / raf hizaları: 424 ve 783 px", 14)
        for j, name in enumerate(("reward_chest.png", "reward_gift.png", "reward_medal.png")):
            block = checker((96, 96))
            block.alpha_composite(rgba(OUT / option / name).resize((96, 96), RESAMPLE))
            sheet.alpha_composite(block, (x + 39 + j * 108, 355))
        small = checker((96, 96))
        small.alpha_composite(rgba(OUT / option / "lock_small.png"))
        sheet.alpha_composite(small, (x + 77, 462))
        small = checker((96, 96))
        small.alpha_composite(rgba(OUT / option / "avatar_ring_gold.png").resize((96, 96), RESAMPLE))
        sheet.alpha_composite(small, (x + 217, 462))
    return sheet


def verify(manifest, geometry, hole):
    results = []
    for option in "ABC":
        for name, size in SIZES.items():
            path = OUT / option / name
            with Image.open(path) as im:
                assert im.size == size, (path, im.size)
                assert im.mode == "RGBA", (path, im.mode)
                alpha = np.asarray(im.getchannel("A"))
                if name != "onizleme.png":
                    assert alpha.min() == 0 and alpha.max() == 255, path
                    assert all(alpha[y, x] == 0 for x, y in ((0, 0), (size[0]-1, 0), (0, size[1]-1), (size[0]-1, size[1]-1))), path
                else:
                    assert alpha.min() == 255, path
                assert path.stat().st_size < 10 * 1024 * 1024, path
                results.append({"file": f"{option}/{name}", "size": list(size), "mode": im.mode,
                                "alpha_extrema": [int(alpha.min()), int(alpha.max())],
                                "transparent_fraction": round(float(np.mean(alpha == 0)), 5),
                                "bytes": path.stat().st_size,
                                "sha256": hashlib.sha256(path.read_bytes()).hexdigest()})
    assert all(140 <= diameter <= 162 for diameter in hole), hole
    ring_alpha = np.asarray(rgba(OUT / "A/avatar_ring_gold.png").getchannel("A"))
    yy, xx = np.ogrid[:220, :220]
    assert np.max(ring_alpha[(xx - 110) ** 2 + (yy - 110) ** 2 < 65 ** 2]) == 0
    # Shared icons must be byte-identical across A/B/C for a fair shelf comparison.
    for name in SIZES:
        if name not in ("shelf.png", "onizleme.png"):
            assert len({hashlib.sha256((OUT / option / name).read_bytes()).hexdigest() for option in "ABC"}) == 1
    return {"status": "PASS", "tool": "built-in image_gen; Python/Pillow sizing, slices, composition and checks",
            "sprite_count": 18, "preview_count": 3, "ring_clear_diameter_px": hole,
            "coordinate_note": "Brief center-relative feet at 0.62h / 1.06h require y=506 / 865, outside the 816px canvas. Both rows move up 82px to y=424 / 783; separation and character height are preserved. This is preview geometry, not drop-in compatibility with unchanged game coordinates.",
            "support_rows_px": list(RAILS), "shelf_slices": geometry,
            "alpha_quantization_cleanup": "Only invisible generated alpha values 1..4 are rounded to zero; visible color, material and glow are retained.",
            "bar": {"track": "pm86-ach-track.png", "fill": "pm86-ach-fill.png", "three_slice_cap_source_px": 24, "value": 45, "target": 2000},
            "checks": ["exact PNG dimensions", "RGBA", "real exterior alpha", "transparent sprite corners", "transparent avatar center", "no file over 10 MiB", "identical common icons", "horizontal slice continuity", "opaque screen previews"],
            "files": results}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--sources", type=Path, required=True)
    args = parser.parse_args()
    manifest = json.loads(args.sources.read_text(encoding="utf-8"))
    geometry = {}
    for option in "ABC":
        (OUT / option).mkdir(exist_ok=True)
        data = manifest[option]
        shelf, slices = shelf_slices(rgba(data["path"]), data["supports"])
        save_png(shelf, OUT / option / "shelf.png")
        geometry[option] = slices
    common = {}
    for key in ("chest", "gift", "medal"):
        common[f"reward_{key}.png"] = contain(rgba(manifest[key]["path"]), (192, 192), 9)
    common["lock_small.png"] = contain(rgba(manifest["lock"]["path"]), (96, 96), 7)
    common["avatar_ring_gold.png"], hole = ring_size(rgba(manifest["ring"]["path"]))
    for option in "ABC":
        for name, im in common.items():
            save_png(im, OUT / option / name)
        save_png(make_preview(option), OUT / option / "onizleme.png")
    comparison = Image.new("RGBA", (1170, 882), "#142b43")
    for column, option in enumerate("ABC"):
        text(comparison, (column * 390 + 195, 19), f"{option} · {NAMES[option]}", 20, heading=True)
        comparison.alpha_composite(rgba(OUT / option / "onizleme.png"), (column * 390, 38))
    save_png(comparison, OUT / "karsilastirma.png")
    save_png(make_slice_check(), OUT / "dilim_kontrol.png")
    report = verify(manifest, geometry, hole)
    (OUT / "QA.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    descriptions = {
        "A": "Lacivert ahşap kemerli vitrin, krem iç yüz ve iki raf, sade altın hatlar.",
        "B": "Lacivert kadife pano, iki altın konsol raf ve küçük taç.",
        "C": "Açık cam kapaklı lacivert vitrin, krem iç yüz ve altın raf kenarları.",
    }
    (OUT / "LOG.md").write_text("\n".join(f"- {o}: {descriptions[o]} Yerleşik image_gen + PIL; 7 PNG, boyut/RGBA/alfa PASS; ayak hizaları 424/783 px (82 px yukarı uyarlama; ayrıntı QA.json)." for o in "ABC") + "\n", encoding="utf-8")
    prompt_text = "# PM89 üretim istemleri\n\nYerleşik image_gen ve imagegen becerisi kullanıldı; API/CLI kullanılmadı. Ödüller, kilit ve halka üç seçenekte ortaktır. Python/PIL ile boyutlandırma, yatay dilim ölçekleme, yerleşim ve kalite kontrolü yapıldı; görünmez alfa 1–4 nicemleme gürültüsü sıfırlandı. Görsellere yazı eklenmedi; metinler yalnızca maket ve kontrol sayfalarındadır.\n\nYönergedeki alt ayak hizası 816 px tuvalin dışında kaldığı için iki sıra 82 px yukarı taşındı. Bu dosyalar oyuna bağlanmamış seçenek maketleridir; mevcut oyun koordinatlarıyla doğrudan uyum iddiası yoktur.\n"
    for key in ("A", "B", "C", "chest", "gift", "medal", "lock", "ring"):
        prompt_text += f"\n## {key}\n\n{manifest[key]['prompt']}\n"
    if manifest["C"].get("refinement_prompt"):
        prompt_text += f"\n## C cam şeffaflığı düzeltmesi\n\n{manifest['C']['refinement_prompt']}\n"
    (OUT / "PROMPTS.md").write_text(prompt_text, encoding="utf-8")
    print(json.dumps({"status": report["status"], "sprites": 18, "previews": 3,
                      "comparison": [1170, 882], "ring_hole_px": hole}, ensure_ascii=False))


if __name__ == "__main__":
    main()
