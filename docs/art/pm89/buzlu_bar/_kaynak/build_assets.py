"""Export built-in image_gen atlases using Pillow; never touch game files.

Run with Python + Pillow. --refresh replaces only this script's generated outputs.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
import random
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont, ImageOps

HERE = Path(__file__).resolve().parent
ROOT = HERE.parent
RESAMPLE = Image.Resampling.LANCZOS
REFRESH = argparse.ArgumentParser()
REFRESH.add_argument('--refresh', action='store_true')
ALLOW_REFRESH = REFRESH.parse_args().refresh
NAVY = '#102A43'
CREAM = '#F3E8CE'
GOLD = '#C6AA6A'
SIZES = {'track.png': (600, 84), 'fill.png': (600, 60),
         'crystal.png': (128, 128), 'panel_frost.png': (339, 291)}
SIZES.update({f'shard_{i}.png': (96, 96) for i in range(1, 7)})
SIZES.update({f'crack_{i}.png': (600, 84) for i in range(1, 4)})

# Pixel regions in the original 1254-square generated atlases.
BOXES = {
    'A': {'track': (42, 42, 1215, 174), 'fill': (42, 176, 1215, 293),
          'cracks': [(45, 309, 1210, 398), (45, 400, 1210, 531),
                     (45, 532, 1210, 660)],
          'frost': (35, 692, 682, 1240), 'crystal': (808, 665, 1070, 914),
          'shards': [(682, 923, 837, 1058), (854, 923, 1015, 1058),
                     (1034, 923, 1210, 1058), (682, 1071, 837, 1222),
                     (854, 1071, 1015, 1222), (1034, 1071, 1210, 1222)]},
    'B': {'track': (42, 25, 1215, 172), 'fill': (42, 176, 1215, 290),
          'cracks': [(45, 308, 1210, 398), (45, 400, 1210, 529),
                     (45, 532, 1210, 641)],
          'frost': (30, 687, 665, 1235), 'crystal': (805, 640, 1065, 875),
          'shards': [(680, 876, 814, 1035), (853, 887, 1007, 1035),
                     (1040, 879, 1210, 1035), (675, 1051, 814, 1210),
                     (842, 1051, 1007, 1210), (1040, 1051, 1210, 1210)]},
    'C': {'track': (42, 37, 1215, 169), 'fill': (42, 176, 1215, 290),
          'cracks': [(45, 308, 1210, 398), (45, 400, 1210, 518),
                     (45, 520, 1210, 652)],
          'frost': (35, 675, 681, 1230), 'crystal': (815, 658, 1066, 885),
          'shards': [(680, 891, 817, 1035), (856, 890, 1007, 1035),
                     (1040, 890, 1210, 1035), (675, 1051, 817, 1210),
                     (849, 1051, 1007, 1210), (1040, 1051, 1210, 1210)]},
}


def save(im, path, **kwargs):
    if path.exists() and not ALLOW_REFRESH:
        raise FileExistsError(f'Existing output protected: {path}')
    path.parent.mkdir(parents=True, exist_ok=True)
    im.save(path, **kwargs)


def clean_alpha(im):
    im = im.convert('RGBA')
    # Generated transparent backgrounds contain harmless alpha=1 fringe noise.
    a = im.getchannel('A').point(lambda v: 0 if v <= 8 else v)
    im.putalpha(a)
    im.paste((0, 0, 0, 0), mask=a.point(lambda v: 255 if v == 0 else 0))
    return im


def extract(atlas, box, size, pad=3, contain=False):
    im = clean_alpha(atlas.crop(box))
    bbox = im.getchannel('A').point(lambda v: 255 if v > 16 else 0).getbbox()
    if not bbox:
        raise ValueError(f'Empty source region: {box}')
    # Keep a few pixels of the original antialiased/glowing edge.
    l, t, r, b = bbox
    im = im.crop((max(0, l-3), max(0, t-3), min(im.width, r+3), min(im.height, b+3)))
    target = (size[0] - 2*pad, size[1] - 2*pad)
    im = ImageOps.contain(im, target, RESAMPLE) if contain else im.resize(target, RESAMPLE)
    out = Image.new('RGBA', size)
    out.alpha_composite(im, ((size[0]-im.width)//2, (size[1]-im.height)//2))
    return clean_alpha(out)


def uniform_center(im, cap):
    """Retain image_gen end caps; repeat one source column across the middle."""
    w, h = im.size
    column = im.crop((w//2, 0, w//2+1, h))
    im.paste(column.resize((w-2*cap, h), Image.Resampling.NEAREST), (cap, 0))
    # Blend only INSIDE the fixed caps so seams meet the uniform middle exactly.
    for j in range(10):
        amount = (j+1)/10
        x = cap-10+j
        im.paste(Image.blend(im.crop((x, 0, x+1, h)), column, amount), (x, 0))
        x = w-cap+9-j
        im.paste(Image.blend(im.crop((x, 0, x+1, h)), column, amount), (x, 0))
    return im


def alpha_scale(im, amount):
    im = im.copy()
    im.putalpha(im.getchannel('A').point(lambda a: round(a*max(0, min(1, amount)))))
    return im


def export_variant(v):
    atlas = Image.open(HERE/f'atlas_{v}.png').convert('RGBA')
    if atlas.size != (1254, 1254):
        raise ValueError('Atlas coordinates require the original 1254px sheet')
    box = BOXES[v]
    art = {}
    art['track'] = uniform_center(extract(atlas, box['track'], (600, 84), 4), 42)
    art['fill'] = uniform_center(extract(atlas, box['fill'], (600, 60), 3), 30)
    if v == 'B':
        art['track'] = alpha_scale(art['track'], .78)
    for name in ('track', 'fill'):
        if name == 'track' and v == 'B':
            continue
        a = art[name].getchannel('A').point(lambda a: 255 if a >= 230 else a)
        art[name].putalpha(a)
    art['crystal'] = extract(atlas, box['crystal'], (128, 128), 6, contain=True)
    art['panel_frost'] = extract(atlas, box['frost'], (339, 291), 2)
    art['panel_frost'].paste((0, 0, 0, 0), (69, 70, 270, 221))
    if v == 'A':
        art['panel_frost'] = alpha_scale(art['panel_frost'], .82)
    for i, region in enumerate(box['cracks'], 1):
        # Center cracks on the track, leaving a transparent border on every side.
        art[f'crack_{i}'] = extract(atlas, region, (600, 84), 6, contain=True)
    for i, region in enumerate(box['shards'], 1):
        im = extract(atlas, region, (96, 96), 8, contain=True)
        pixels = []
        for r, g, b, a in im.get_flattened_data():
            # Keep white rims bright; blue bodies stay visibly translucent.
            scale = .96 if min(r, g, b) > 218 else .76
            pixels.append((r, g, b, round(a*scale)))
        im.putdata(pixels)
        art[f'shard_{i}'] = im
    for name, im in art.items():
        save(im, ROOT/v/f'{name}.png', optimize=True)
    return art


FONT_DIR = Path('C:/Windows/Fonts')


def font(size, bold=False):
    return ImageFont.truetype(str(FONT_DIR/('arialbd.ttf' if bold else 'arial.ttf')), size)


def centered(draw, x, y, text, f, fill):
    bounds = draw.textbbox((0, 0), text, font=f)
    draw.text((x-(bounds[2]-bounds[0])/2, y), text, font=f, fill=fill)


def resize_slice(im, size, cap):
    # Full center repetition is already guaranteed in the exported source.
    w, h = size
    c = max(1, round(cap*h/im.height))
    out = Image.new('RGBA', size)
    out.alpha_composite(im.crop((0, 0, cap, im.height)).resize((c, h), RESAMPLE))
    out.alpha_composite(im.crop((cap, 0, im.width-cap, im.height)).resize((w-2*c, h), RESAMPLE), (c, 0))
    out.alpha_composite(im.crop((im.width-cap, 0, im.width, im.height)).resize((c, h), RESAMPLE), (w-c, 0))
    return out


def make_frame(v, art, n):
    frame = Image.new('RGBA', (390, 200), NAVY)
    draw = ImageDraw.Draw(frame)
    px, py, pw, ph = 108, 7, 174, 149
    # Cream timer placeholder, with the same panel aspect ratio as the brief.
    draw.rounded_rectangle((px+2, py+3, px+pw+2, py+ph+3), 18, fill='#081D31')
    draw.rounded_rectangle((px, py, px+pw, py+ph), 18, fill=GOLD)
    draw.rounded_rectangle((px+2, py+2, px+pw-2, py+ph-2), 16, fill='#30597A')
    draw.rounded_rectangle((px+6, py+6, px+pw-6, py+ph-6), 12, fill=CREAM)
    centered(draw, 195, 21, 'Süre', font(17, True), '#31506A')
    centered(draw, 195, 57, '0:42', font(46, True), '#173B56')
    frost_opacity = 1 if n < 51 else max(0, (58-n)/7)
    frame.alpha_composite(alpha_scale(art['panel_frost'].resize((pw, ph), RESAMPLE), frost_opacity), (px, py))
    track_opacity = 1 if n <= 50 else max(0, (57-n)/7)
    bx, by, bw, bh = 118, 160, 154, 22
    bar = Image.new('RGBA', (bw, bh))
    bar.alpha_composite(resize_slice(art['track'], (bw, bh), 42))
    fill = resize_slice(art['fill'], (bw-6, 16), 30)
    # Clear FROM LEFT TO RIGHT, as specified. Ice remains anchored to the right.
    cleared = min(fill.width, round(fill.width*min(1, n/41)))
    if cleared < fill.width:
        bar.alpha_composite(fill.crop((cleared, 0, fill.width, fill.height)), (3+cleared, 3))
    if 42 <= n <= 50:
        stage = min(3, 1+(n-42)//3)
        crack = art[f'crack_{stage}'].resize((bw, bh), RESAMPLE)
        # Growing crack luminance makes all three frames of each stage distinct.
        bar.alpha_composite(alpha_scale(crack, (.45, .75, 1)[(n-42)%3]))
    frame.alpha_composite(alpha_scale(bar, track_opacity), (bx, by))
    if n < 42:
        for j, u in enumerate((.16, .49, .82)):
            twinkle = max(0, math.sin(n*.43+j*2.2))**3
            if u*bw >= cleared and twinkle > .02:
                size = 15 + (j % 2)*4
                star = alpha_scale(art['crystal'].resize((size, size), RESAMPLE), twinkle*.9)
                frame.alpha_composite(star, (round(bx+bw*u-size/2), by-size//2+2))
    if n >= 49:
        rnd = random.Random(89+ord(v))
        count, base = {'A': (20, 10), 'B': (7, 21), 'C': (12, 15)}[v]
        t = (n-49)/30
        for i in range(count):
            x0 = bx + rnd.uniform(6, bw-6)
            vx, vy = rnd.uniform(-110, 110), rnd.uniform(-145, -45)
            size = base + rnd.randrange(0, 7)
            angle, spin = rnd.uniform(0, 360), rnd.uniform(-400, 400)
            shard = art[f'shard_{i%6+1}'].resize((size, size), RESAMPLE)
            shard = shard.rotate(angle+spin*t, resample=Image.Resampling.BICUBIC, expand=True)
            shard = alpha_scale(shard, max(0, (60-n)/8))
            x, y = round(x0+vx*t-shard.width/2), round(by+5+vy*t+650*t*t)
            frame.alpha_composite(shard, (x, y))
    return frame


def previews(v, art):
    frames = [make_frame(v, art, n).convert('RGB') for n in range(60)]
    # One shared palette prevents palette shimmer; GIF time units are 10ms.
    montage = Image.new('RGB', (390*10, 200*6))
    for n, im in enumerate(frames):
        montage.paste(im, ((n%10)*390, (n//10)*200))
    palette = montage.quantize(colors=256, method=Image.Quantize.MEDIANCUT)
    frames = [im.quantize(palette=palette, dither=Image.Dither.NONE) for im in frames]
    save(frames[0], ROOT/v/'onizleme.gif', save_all=True, append_images=frames[1:],
         duration=[30, 30, 40]*20, loop=0, disposal=2, optimize=False)


def comparison(sets):
    out = Image.new('RGB', (1230, 330), '#091E32')
    draw = ImageDraw.Draw(out)
    draw.text((28, 17), 'PM_89  /  BUZLU SAAT', font=font(20, True), fill=CREAM)
    draw.text((28, 46), 'Dolu durum · Soldan sağa boşalma · Çatlama ve erime', font=font(14), fill='#9DBBD0')
    labels = {'A': ('A  /  LACİVERT + ALTIN', 'İnce altın kenar · Küçük, çok sayıda parça'),
              'B': ('B  /  BUZ YUVA', 'Saydam buz kenar · Daha iri, az sayıda parça'),
              'C': ('C  /  KREM + MAVİ', 'Belirgin kırağı · Küçük buz sarkıtları')}
    for i, v in enumerate('ABC'):
        x = 20+i*405
        draw.rounded_rectangle((x, 77, x+390, 314), 12, fill=NAVY, outline='#30516B', width=1)
        draw.text((x+15, 87), labels[v][0], font=font(15, True), fill=CREAM)
        out.paste(make_frame(v, sets[v], 0).convert('RGB'), (x, 109))
        draw.text((x+15, 292), labels[v][1], font=font(11), fill='#A9CADF')
    save(out, ROOT/'karsilastirma.png', optimize=True)


def qa_montage(sets):
    out = Image.new('RGB', (1230, 1050), NAVY)
    d = ImageDraw.Draw(out)
    for row, n in enumerate((0, 21, 44, 47, 51, 55, 59)):
        for col, v in enumerate('ABC'):
            x, y = col*410, row*150
            with Image.open(ROOT/v/'onizleme.gif') as gif:
                gif.seek(n)
                im = gif.convert('RGB').resize((292, 150), RESAMPLE)
            out.paste(im, (x+65, y))
            d.text((x+8, y+8), f'{v} / {n:02}', font=font(13), fill=CREAM)
    save(out, HERE/'animasyon_kontrol.png', optimize=True)
    sheet = Image.new('RGBA', (1260, 1110), NAVY)
    d = ImageDraw.Draw(sheet)
    for col, v in enumerate('ABC'):
        x = 20+col*420
        d.text((x, 10), f'{v} / RGBA + 9-dilim', font=font(17, True), fill=CREAM)
        for name, y in [('track', 50), ('fill', 120), ('crack_1', 190), ('crack_2', 260), ('crack_3', 330)]:
            im = sets[v][name].resize((390, round(sets[v][name].height*.65)), RESAMPLE)
            sheet.alpha_composite(im, (x, y))
            d.text((x, y-15), name, font=font(11), fill='#91B2CA')
        sheet.alpha_composite(sets[v]['panel_frost'], (x+25, 435))
        sheet.alpha_composite(sets[v]['crystal'], (x+125, 740))
        for j in range(6):
            sheet.alpha_composite(sets[v][f'shard_{j+1}'], (x+(j%3)*130, 880+(j//3)*110))
    save(sheet.convert('RGB'), HERE/'parca_kontrol.png', optimize=True)


def validate():
    report = {'tool': 'built-in image_gen + Python/Pillow', 'variants': {}}
    for v in 'ABC':
        details = {'png_count': 0, 'sizes_rgba': True, 'transparent_borders': True}
        for filename, size in SIZES.items():
            im = Image.open(ROOT/v/filename)
            assert im.mode == 'RGBA' and im.size == size, (v, filename, im.mode, im.size)
            a = im.getchannel('A')
            assert a.getextrema()[0] == 0 and a.getextrema()[1] > 0
            for edge in ((0, 0, im.width, 1), (0, im.height-1, im.width, im.height),
                         (0, 0, 1, im.height), (im.width-1, 0, im.width, im.height)):
                assert a.crop(edge).getextrema() == (0, 0), (v, filename, 'edge')
            details['png_count'] += 1
        for name, cap in [('track', 42), ('fill', 30)]:
            im = Image.open(ROOT/v/f'{name}.png')
            ref = im.crop((cap, 0, cap+1, im.height)).tobytes()
            assert all(im.crop((x, 0, x+1, im.height)).tobytes() == ref
                       for x in range(cap, im.width-cap)), (v, name, 'nonuniform center')
            details[f'{name}_cap_px'] = cap
            details[f'{name}_center_columns_identical'] = True
        frost = Image.open(ROOT/v/'panel_frost.png').getchannel('A')
        assert frost.crop((69, 70, 270, 221)).getextrema() == (0, 0)
        details['frost_center_200x150_alpha_zero'] = True
        densities = []
        for i in range(1, 4):
            a = Image.open(ROOT/v/f'crack_{i}.png').getchannel('A')
            densities.append(sum(a.histogram()[40:]))
        assert densities[0] < densities[1] < densities[2], densities
        details['crack_progression_pixels'] = densities
        gif = Image.open(ROOT/v/'onizleme.gif')
        assert gif.size == (390, 200) and gif.n_frames == 60
        durations = []
        for i in range(gif.n_frames):
            gif.seek(i)
            durations.append(gif.info['duration'])
        assert sum(durations) == 2000
        details.update(gif_frames=gif.n_frames, gif_ms=sum(durations), gif_fps=30,
                       gif_frame_durations_ms=sorted(set(durations)), drain_direction='left-to-right')
        report['variants'][v] = details
    assert Image.open(ROOT/'karsilastirma.png').size == (1230, 330)
    report['reference_sha256'] = hashlib.sha256((ROOT/'referans_hud.png').read_bytes()).hexdigest()
    report['files_over_10_MB'] = [str(p.relative_to(ROOT)) for p in ROOT.rglob('*')
                                 if p.is_file() and p.stat().st_size > 10_000_000]
    assert not report['files_over_10_MB']
    path = HERE/'kontrol.json'
    if path.exists() and not ALLOW_REFRESH:
        raise FileExistsError(path)
    path.write_text(json.dumps(report, indent=2, ensure_ascii=False)+'\n', encoding='utf-8')
    print(json.dumps(report, indent=2, ensure_ascii=False))


def slice_montage(sets):
    out = Image.new('RGBA', (1100, 570), NAVY)
    draw = ImageDraw.Draw(out)
    draw.text((20, 12), '9-dilim / Sabit uçlar, aynı sütunlardan oluşan orta bölge',
              font=font(18, True), fill=CREAM)
    for row, v in enumerate('ABC'):
        y = 60+row*165
        draw.text((20, y), v, font=font(20, True), fill=CREAM)
        for width, x in [(120, 65), (240, 215), (600, 480)]:
            track = resize_slice(sets[v]['track'], (width, 42), 42)
            fill = resize_slice(sets[v]['fill'], (width, 30), 30)
            out.alpha_composite(track, (x, y))
            out.alpha_composite(fill, (x, y+55))
            draw.text((x, y+93), f'{width}px / 42px + 30px', font=font(12), fill='#A9CADF')
    save(out.convert('RGB'), HERE/'dilim_kontrol.png', optimize=True)


if __name__ == '__main__':
    sets = {v: export_variant(v) for v in 'ABC'}
    for v, art in sets.items():
        previews(v, art)
    comparison(sets)
    qa_montage(sets)
    slice_montage(sets)
    validate()
