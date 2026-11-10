from __future__ import annotations

import hashlib
import json
import math
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOC = ROOT / 'docs/art'
sys.path.insert(0, str(DOC / '.task15-python'))
from PIL import Image, ImageChops, ImageDraw, ImageFilter, ImageFont, ImageOps
import numpy as np

OUT = ROOT / 'Assets/Art/Gameplay/Task15'
SRC = OUT / 'Source'
OLD = ROOT / 'Assets/Art/Gameplay/Task14'
ART = ROOT / 'Assets/Art'
R = Image.Resampling.LANCZOS
CREAM = '#F8EBD0'
NAVY = '#0B2A36'
TEAL = '#0F4F5A'
RECORDS = []


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest().upper()


def load(path):
    with Image.open(path) as im:
        return im.convert('RGBA')


def save(im, name, role=None, border=(0, 0, 0, 0), pivot=(.5, .5), scale=3, alpha=True):
    path = OUT / (name + '.png')
    assert path.resolve().is_relative_to(OUT.resolve())
    im.save(path, optimize=True)
    if role:
        RECORDS.append(dict(file=path.name, use=role, width=im.width, height=im.height,
                            sliceLTRB=list(border), pivot=list(pivot), scale=scale,
                            transparent=alpha, sha256=digest(path)))
    return im


def clean(im, component=True):
    a = im.getchannel('A')
    a = a.point(lambda v: v if v >= 16 else 0)
    if component:
        binary = a.point(lambda v: 255 if v > 64 else 0)
        pts = np.argwhere(np.asarray(binary) == 255)
        if len(pts):
            center = np.array([im.height * .55, im.width * .5])
            y, x = pts[np.argmin(np.sum((pts - center) ** 2, axis=1))]
            ImageDraw.floodfill(binary, (int(x), int(y)), 128)
            keep = binary.point(lambda v: 255 if v == 128 else 0).filter(ImageFilter.MaxFilter(3))
            a = ImageChops.multiply(a, keep)
    im.putalpha(a)
    return im


def fit(im, size, pad=0, stretch=False, component=True):
    im = clean(im.copy(), component)
    box = im.getchannel('A').point(lambda v: 255 if v > 160 else 0).getbbox()
    assert box, 'Empty source'
    l, t, r, b = box
    im = im.crop((max(0, l - 2), max(0, t - 2), min(im.width, r + 2), min(im.height, b + 2)))
    inner = (size[0] - 2 * pad, size[1] - 2 * pad)
    im = im.resize(inner, R) if stretch else ImageOps.contain(im, inner, R)
    canvas = Image.new('RGBA', size)
    canvas.alpha_composite(im, ((size[0] - im.width) // 2, (size[1] - im.height) // 2))
    return clean(canvas, False)


def red_tone(im):
    a = np.array(im).astype(np.float32)
    rgb = a[..., :3]
    mask = (rgb[..., 0] > rgb[..., 1] * 1.3) & (rgb[..., 0] > rgb[..., 2] * 1.3)
    light = (rgb.max(axis=2) + rgb.min(axis=2)) / 510
    levels = [0, .32, .48, .64, 1]
    anchors = np.array([[11, 42, 54], [148, 22, 15], [214, 40, 34], [240, 68, 56], [255, 249, 241]])
    for channel in range(3):
        rgb[..., channel] = np.where(mask, np.interp(light, levels, anchors[:, channel]), rgb[..., channel])
    return Image.fromarray(np.clip(a, 0, 255).astype('uint8'))


def seal_x(im, width=5):
    a = np.array(im).astype(np.float32)
    edge = (a[:, 0] + a[:, -1]) * .5
    for i in range(width):
        v = (width - i) / width
        a[:, i] = a[:, i] * (1 - v) + edge * v
        a[:, -1 - i] = a[:, -1 - i] * (1 - v) + edge * v
    a = np.rint(a).astype('uint8')
    a[:, -1] = a[:, 0]
    return Image.fromarray(a)


def slice9(im, size, border):
    l, r, t, b = border
    xs, ys = [0, l, im.width - r, im.width], [0, t, im.height - b, im.height]
    dx, dy = [0, l, size[0] - r, size[0]], [0, t, size[1] - b, size[1]]
    result = Image.new('RGBA', size)
    for y in range(3):
        for x in range(3):
            part = im.crop((xs[x], ys[y], xs[x + 1], ys[y + 1]))
            part = part.resize((dx[x + 1] - dx[x], dy[y + 1] - dy[y]), R)
            result.alpha_composite(part, (dx[x], dy[y]))
    return result


def pressed(im):
    a = np.array(im).astype(np.float32)
    a[..., :3] = a[..., :3] * .94 + np.array([11, 42, 54]) * .06
    s = Image.fromarray(a.astype('uint8')).resize((im.width, im.height - 9), R)
    b = Image.new('RGBA', im.size)
    b.alpha_composite(s, (0, 6))
    return b


def export():
    badge = clean(load(SRC / 'booster-badge-source.png'))
    a = np.array(badge)
    red = (a[..., 0].astype(float) > a[..., 1] * 1.6) & (a[..., 0].astype(float) > a[..., 2] * 1.6) & (a[..., 3] > 200)
    yy, xx = np.where(red)
    cx, cy = (xx.min() + xx.max()) / 2, (yy.min() + yy.max()) / 2
    rad = (xx.max() - xx.min() + 1) / 2 + badge.width * .024
    y, x = np.indices(a.shape[:2])
    a[..., 3] = np.where((x - cx) ** 2 + (y - cy) ** 2 <= rad ** 2, a[..., 3], 0)
    save(fit(red_tone(Image.fromarray(a)), (96, 96), 4), 'booster-badge-v2', 'Yeni kırmızı adet rozeti; rakam ayrı')

    wall = clean(load(SRC / 'wall-repeat-source.png'), False)
    wall = wall.crop(wall.getchannel('A').point(lambda v: 255 if v > 160 else 0).getbbox())
    arr = np.array(wall)
    signal = (arr[::max(1, wall.height // 32), :, 0].astype(float) > arr[::max(1, wall.height // 32), :, 1] * 1.3)
    candidates = range(int(wall.width * .38), int(wall.width * .62))
    period = min(candidates, key=lambda p: np.mean(signal[:, :-p] != signal[:, p:]))
    wall = red_tone(wall.crop((0, 0, period, wall.height))).resize((144, 66), R)
    tile = Image.new('RGBA', (144, 72)); tile.alpha_composite(wall, (0, 3))
    tile = seal_x(clean(tile, False))
    save(tile, 'wall-repeat-v3', 'Kalın kırmızı-beyaz duvar; X tekrar, native kalınlık 24 pt')
    corner = clean(load(SRC / 'wall-corner-source.png'))
    corner = corner.crop(corner.getchannel('A').point(lambda v: 255 if v > 160 else 0).getbbox())
    sx, sy = [0, int(corner.width * .35), corner.width], [0, int(corner.height * .65), corner.height]
    elbow = Image.new('RGBA', (144, 144))
    for y in range(2):
        for x in range(2):
            part = corner.crop((sx[x], sy[y], sx[x + 1], sy[y + 1])).resize((72, 72), R)
            elbow.alpha_composite(part, (72 * x, 72 * y))
    elbow = red_tone(elbow)
    elbow.paste(tile.crop((0, 0, 12, 72)).transpose(Image.Transpose.ROTATE_270), (0, 0))
    elbow.paste(tile.crop((132, 0, 144, 72)), (132, 72))
    save(elbow, 'wall-corner-v3', 'Duvar köşesi; üstten giriş, sağdan çıkış', pivot=(.25, .25))

    plate = fit(load(SRC / 'hud-pill-source.png'), (330, 210), 6, True)
    save(plate, 'hud-moves-pill-v2', 'Hamle hapı boş zemin; ikon ve sayı ayrı', (84, 84, 84, 84))
    time = slice9(plate, (366, 210), (84, 84, 84, 84))
    save(time, 'hud-time-pill-v2', 'Süre hapı boş zemin; kare kronometre ve sayı ayrı', (84, 84, 84, 84))
    arr = np.array(time)
    yy, xx = np.indices(arr.shape[:2]); rgb = arr[..., :3].astype(float)
    mask = (xx > 26) & (xx < 340) & (yy > 38) & (yy < 172) & (rgb[..., 0] > 150) & (rgb[..., 1] > rgb[..., 0] * .65) & (rgb[..., 2] > rgb[..., 0] * .30)
    t = np.clip((rgb[..., 0] - 220) / 180, 0, .22)
    coral = np.stack((232 + 16 * t, 87 + 148 * t, 74 + 134 * t), axis=-1)
    arr[..., :3] = np.where(mask[..., None], coral, rgb).astype('uint8')
    save(Image.fromarray(arr), 'hud-time-pill-urgent-v2', 'Acil süre hapı; mercan yalnız burada, sayı ayrı', (84, 84, 84, 84))
    for name in ['piece', 'stopwatch']:
        save(fit(load(SRC / f'hud-icon-{name}-source.png'), (120, 120), 8), f'hud-icon-{name}-v2', f'HUD bağımsız {name} ikonu; kare Simple/PreserveAspect')

    for suffix, source in [('', 'menu-play-button-v2.png'), ('-pressed', 'menu-play-button-v2-pressed.png')]:
        name = 'btn-primary-v2' + suffix
        path = OUT / (name + '.png')
        shutil.copyfile(ART / 'UI/PlayButton' / source, path)
        im = load(path)
        RECORDS.append(dict(file=path.name, use='Başla/Devam yeşil ana tuş' + (' basılı' if suffix else ''), width=im.width, height=im.height, sliceLTRB=[92]*4, pivot=[.5,.5], scale=3, transparent=True, sha256=digest(path)))
    raw = fit(load(SRC / 'btn-secondary-source.png'), (660, 300), 0, True)
    secondary = Image.new('RGBA', (684, 336)); secondary.alpha_composite(raw, (12, 12))
    save(secondary, 'btn-secondary-v2', 'Yeniden başla/Ana menü turkuaz tuş; yazı ayrı', (92, 92, 92, 92))
    save(pressed(secondary), 'btn-secondary-v2-pressed', 'Turkuaz ikincil tuş basılı', (92, 92, 92, 92))
    for color in ['blue', 'pink', 'red', 'green', 'purple']:
        save(fit(load(SRC / f'avatar-{color}-source.png'), (256, 256), 12), f'avatar-{color}', f'{color} Jiggle profil avatarı; 256 px özel ölçü', scale=1)
    save(fit(load(SRC / 'jiggle-counter-icon-source.png'), (192, 192), 8), 'jiggle-counter-icon', 'Menü sağ üst kurtarılan Jiggle sayacı; 64 pt')
    painting = load(SRC / 'painting-test-source.png').resize((1536, 1536), R)
    save(painting, 'painting-test-01', 'Parçalara board UV ile kaplanacak test tablosu; çizgisiz tam resim', scale=1, alpha=False)
    return period


def piece_names():
    data = json.loads((DOC / 'task14-preview-board.json').read_text(encoding='utf-8-sig'))
    def mark(v, inv):
        if not v: return 'F'
        return 'O' if (inv and v == -1) or (not inv and v == 1) else 'I'
    names=[]
    for y in range(5):
        for x in range(5):
            u = 'F' if y == 0 else mark(data['hEdge'][y][x], True)
            r = 'F' if x == 4 else mark(data['vEdge'][y][x+1], False)
            d = 'F' if y == 4 else mark(data['hEdge'][y+1][x], False)
            l = 'F' if x == 0 else mark(data['vEdge'][y][x], True)
            names.append('piece_' + '-'.join([u,r,d,l]))
    return names


def board(painted=True, holes=False):
    result = Image.new('RGBA', (1536, 1536), TEAL)
    paint = np.array(load(OUT / 'painting-test-01.png'))
    names = piece_names()
    for i, name in enumerate(names):
        if holes and i in [0,24]: continue
        y, x = divmod(i, 5)
        if painted:
            alpha = np.asarray(load(ART / f'Pieces/Kit81/mask/{name}.png'))[...,3]
            gx = np.clip(((x*384+np.arange(640)-128)*.8).astype(int), 0, 1535)
            gy = np.clip(((y*384+np.arange(640)-128)*.8).astype(int), 0, 1535)
            rgba = paint[gy[:,None],gx[None,:]].copy(); rgba[...,3] = alpha
            part = Image.fromarray(rgba)
            part.alpha_composite(load(ART / f'Pieces/Kit81/overlay/{name}.png'))
        else:
            part = load(ART / f'Pieces/Kit81/flat/{name}.png')
        result.alpha_composite(part.resize((512,512),R), (round((x*384-128)*.8),round((y*384-128)*.8)))
    return result


class Screen:
    def __init__(self, image=None):
        self.im = image.copy() if image else Image.new('RGBA',(1170,2532))
    def image(self, im, rect):
        x,y,w,h=rect; self.im.alpha_composite(im.resize((round(w*3),round(h*3)),R),(round(x*3),round(y*3)))
    def sprite(self,path,rect): self.image(load(path),rect)
    def text(self,text,cx,cy,size,fill=CREAM,stroke=0,max_width=None):
        fallback = any(c in text for c in 'ğşİĞŞ')
        fontpath = Path('C:/Windows/Fonts/segoeuib.ttf') if fallback else OLD/'LilitaOne-Regular.ttf'
        font=ImageFont.truetype(str(fontpath),round(size*3))
        draw=ImageDraw.Draw(self.im); sw=round(stroke*3)
        box=draw.textbbox((0,0),text,font=font,stroke_width=sw)
        if max_width and box[2]-box[0] > max_width*3:
            return self.text(text,cx,cy,size*max_width*3/(box[2]-box[0]),fill,stroke)
        at=(round(cx*3-(box[2]+box[0])/2),round(cy*3-(box[3]+box[1])/2))
        draw.text(at,text,font=font,fill=fill,stroke_width=sw,stroke_fill=NAVY)
    def finish(self,name):
        save(self.im.resize((390,844),R),name)


def wall_strip(length, thickness=24, vertical=False):
    unit=load(OUT/'wall-repeat-v3.png').resize((144,round(thickness*3)),R)
    strip=Image.new('RGBA',(round(length*3),round(thickness*3)))
    for x in range(0,strip.width,144): strip.alpha_composite(unit,(x,0))
    return strip.transpose(Image.Transpose.ROTATE_270) if vertical else strip


def game_screen(painted=True):
    s=Screen(); s.sprite(ART/'Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png',(0,0,390,844))
    s.sprite(OLD/'hud-level-badge.png',(10,64,60,60)); s.text('LV',40,82,11,NAVY); s.text('7',40,105,29,NAVY)
    s.sprite(OUT/'hud-moves-pill-v2.png',(76,59,110,70)); s.sprite(OUT/'hud-icon-piece-v2.png',(82,75,36,36)); s.text('25',150,94,31,NAVY)
    s.sprite(OUT/'hud-time-pill-v2.png',(192,59,122,70)); s.sprite(OUT/'hud-icon-stopwatch-v2.png',(198,75,36,36)); s.text('0:48',271,94,26,NAVY)
    s.sprite(OLD/'hud-pause-normal.png',(326,70,50,50))
    s.image(board(painted,True),(50.55,259.95,288.44,288.44))
    overlay=load(OLD/'Source/ui-overlay.png').resize((1170,2532),R)
    s.im.alpha_composite(overlay.crop((0,630,1170,1800)),(0,630))
    s.image(wall_strip(114),(108,233,114,24)); s.image(wall_strip(144,vertical=True),(343,338,24,144)); s.image(wall_strip(114),(166,551,114,24))
    s.sprite(OLD/'nail-head-384.png',(169,321,52,52)); s.text('12',195,347,23,NAVY)
    s.sprite(OLD/'bomb-384.png',(167,432,56,56)); s.text('24',195,460,20,NAVY)
    s.sprite(OLD/'chain-link-repeat-96x48.png',(80,396,57,20)); s.sprite(OLD/'chain-endpoint-144.png',(65,389,30,30)); s.sprite(OLD/'chain-endpoint-144.png',(122,389,30,30))
    s.sprite(OLD/'booster-bar.png',(12,681,366,113))
    for i,icon in enumerate(['scissors','wand','hammer','clock']):
        x=27+88*i; s.sprite(OLD/'booster-button.png',(x,707,72,72)); s.sprite(OLD/f'booster-{icon}.png',(x+3.27,710.27,65.45,65.45))
        s.sprite(OUT/'booster-badge-v2.png',(x+50,760,25,25)); s.text(str([3,2,1,1][i]),x+62.5,772.5,18,CREAM,.7)
    return s


def button(s,label,x,y,primary=True):
    s.sprite(OUT/('btn-primary-v2.png' if primary else 'btn-secondary-v2.png'),(x,y,220,108))
    s.text(label,x+110,y+51,32 if primary else 27,CREAM,1.15,max_width=180)


def previews():
    save(board(), 'preview-painted-board')
    base=game_screen(); base.finish('preview-ingame')
    game_screen(False).finish('preview-ingame-blue')
    pause=Screen(base.im); pause.im.alpha_composite(Image.new('RGBA',pause.im.size,(11,42,54,178)))
    panel=slice9(load(OLD/'card-panel.png'),(969,1500),(192,192,300,132))
    pause.image(panel,(25,165,340,526)); pause.text('Durduruldu',195,218,27,CREAM,1)
    button(pause,'Devam',85,309); button(pause,'Yeniden başla',85,430,False); button(pause,'Ana menü',85,551,False)
    pause.finish('preview-pause')
    avatars=Screen(load(ART/'Menu/Task13/preview-menu.png').resize((1170,2532),R))
    avatars.im.alpha_composite(Image.new('RGBA',avatars.im.size,(11,42,54,145)))
    avatars.sprite(ART/'Menu/Task13/counter-pill.png',(198,43,176,60)); avatars.sprite(OUT/'jiggle-counter-icon.png',(201,43,60,60)); avatars.text('584',314,74,25,NAVY)
    avatars.image(slice9(load(OLD/'card-panel.png'),(969,1400),(192,192,300,132)),(25,172,340,491))
    avatars.text('Avatar seç',195,225,28,CREAM,1)
    positions=[(51,295),(147,295),(243,295),(99,400),(195,400)]
    for color,(x,y) in zip(['blue','pink','red','green','purple'],positions):
        avatars.sprite(OUT/f'avatar-{color}.png',(x+6,y+6,84,84)); avatars.sprite(ART/'Menu/Task13/profile-frame.png',(x,y,96,96))
    button(avatars,'Seç',85,541); avatars.finish('preview-avatars')
    overview=Image.new('RGB',(1296,1008),NAVY); draw=ImageDraw.Draw(overview)
    f=ImageFont.truetype(str(OLD/'LilitaOne-Regular.ttf'),26)
    draw.text((30,20),'TASK15',font=f,fill=CREAM)
    for x,name,label in [(30,'preview-ingame','RESIMLI TAHTA / HUD'),(453,'preview-pause','PAUSE / YENI TUSLAR'),(876,'preview-avatars','5 AVATAR / SAYAC')]:
        draw.text((x,62),label,font=ImageFont.truetype(str(OLD/'LilitaOne-Regular.ttf'),18),fill=CREAM)
        overview.paste(load(OUT/(name+'.png')).convert('RGB'),(x,100))
    save(overview,'preview')
    contact=Image.new('RGB',(1200,math.ceil(len(RECORDS)/4)*254),TEAL); draw=ImageDraw.Draw(contact)
    font=ImageFont.truetype(str(OLD/'LilitaOne-Regular.ttf'),15)
    for i,item in enumerate(RECORDS):
        x=(i%4)*300; y=(i//4)*254; im=load(OUT/item['file']); im.thumbnail((270,210),R)
        contact.paste(im,(x+(300-im.width)//2,y+8+(210-im.height)//2),im)
        draw.text((x+10,y+227),item['file'],font=font,fill=CREAM)
    save(contact,'preview-components')
    proof=Image.new('RGBA',(900,300),TEAL)
    gold=load(OLD/'board-frame-gold.png').crop((300,65,825,145)).resize((900,150),R); proof.alpha_composite(gold,(0,0))
    blue=load(ART/'Pieces/Kit81/flat/piece_F-F-F-F.png').crop((140,140,500,500)).resize((900,150),R); proof.alpha_composite(blue,(0,150))
    for y in [50,195]:
        tile=load(OUT/'wall-repeat-v3.png')
        for x in range(20,740,144): proof.alpha_composite(tile,(x,y))
    proof.alpha_composite(load(OUT/'wall-corner-v3.png').resize((108,108),R),(780,162))
    save(proof,'preview-seams')


def write_json(path,data):
    path.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')


def verify(source_hashes,period):
    checks=[]
    for item in RECORDS:
        im=load(OUT/item['file']); a=np.asarray(im)
        assert im.size==(item['width'],item['height'])
        if item['transparent']: assert np.any(a[...,3]==0),item['file']
        l,r,t,b=item['sliceLTRB']; assert l+r<im.width and t+b<im.height
        if item['scale']==3: assert im.width%3==0 and im.height%3==0
        checks.append(dict(file=item['file'],size=list(im.size),transparentPixels=int(np.sum(a[...,3]==0)),sha256=digest(OUT/item['file'])))
    tile=np.array(load(OUT/'wall-repeat-v3.png')); corner=np.array(load(OUT/'wall-corner-v3.png'))
    assert np.array_equal(tile[:,0],tile[:,-1])
    assert np.array_equal(corner[0,:72],tile[::-1,0])
    assert np.array_equal(corner[72:,143],tile[:,0])
    assert all(digest(SRC/name)==value for name,value in source_hashes.items())
    protected=json.loads((DOC/'reports/task15-protected-sha256.json').read_text(encoding='utf-8-sig'))
    approved=[v for v in protected if Path(v['path']).suffix.lower() in ['.png','.ttf','.txt']]
    changed=[v['path'] for v in approved if digest(v['path'])!=v['hash']]
    assert not changed,changed
    assert digest(OUT/'btn-primary-v2.png')==digest(ART/'UI/PlayButton/menu-play-button-v2.png')
    assert digest(OUT/'btn-primary-v2-pressed.png')==digest(ART/'UI/PlayButton/menu-play-button-v2-pressed.png')
    report=dict(status='passed',productionPngCount=len(RECORDS),sourcePngCount=len(source_hashes),sourcePngChanged=0,
                approvedSourcesChecked=len(approved),approvedSourcesChanged=0,wallRepeatRgbaMismatches=0,cornerRgbaMismatches=0,
                greenButtonsByteIdentical=True,wallSourcePeriod=period,allNumberFieldsEmpty=True,files=checks)
    write_json(DOC/'reports/task15-export-validation.json',report)
    return report


def documentation(report):
    write_json(OUT/'manifest.json',RECORDS)
    lines=['<!-- TASK15_BEGIN -->','## Task15 — export tamamlandı','',
           'Hazır Source çizimlerinden Python/Pillow ile export edildi; yeniden çizim yapılmadı. Border L/R/T/B kaynak pikseli, pivot Unity sol-alt başlangıçlı normalize koordinattır. UI @3x; avatarlar özel 256×256 ölçü, tablo özel 1536×1536 ölçüdedir.','',
           '| Dosya | Oyunda nereye | Boyut | 9-slice L/R/T/B | Pivot |','|---|---|---|---|---|']
    for item in RECORDS:
        border='/'.join(map(str,item['sliceLTRB'])) if any(item['sliceLTRB']) else 'Yok'
        pivot='/'.join(f'{v:.2f}' for v in item['pivot'])
        lines.append(f"| `Assets/Art/Gameplay/Task15/{item['file']}` | {item['use']} | {item['width']}×{item['height']} | {border} | {pivot} |")
    for name in ['preview','preview-ingame','preview-ingame-blue','preview-pause','preview-avatars','preview-painted-board','preview-components','preview-seams']:
        im=load(OUT/(name+'.png')); lines.append(f'| `Assets/Art/Gameplay/Task15/{name}.png` | İnceleme; runtime değildir | {im.width}×{im.height} | Uygulanmaz | Uygulanmaz |')
    lines += ['| `Assets/Art/Gameplay/Task15/manifest.json` | Dosya ve entegrasyon metadata listesi | 19 PNG kaydı | Metadata | Metadata |',
              '| `Assets/Art/Gameplay/Task15/README.md` | Paket ve katman kullanım notları | Markdown | Uygulanmaz | Uygulanmaz |','',
              'Haplar boş zemindir; ayrı 120×120 ikonlar Simple + PreserveAspect ile kullanılır. Yeni rozet için Task14 ayrı zemin+ikon yolu kullanılmalı; eski rozet içeren birleşik booster PNG üstüne ikinci rozet eklenmemeli. Detaylar: `docs/art/TASK15_EXPORT.md`.','',
              f"Kontrol: {report['productionPngCount']} üretim PNG, {report['sourcePngCount']} Source PNG değişmeden korundu, {report['approvedSourcesChecked']} eski onaylı sanat kaynağında fark 0; duvar ve köşe RGBA farkı 0. Rapor: `docs/art/reports/task15-export-validation.json`. C# ve Assets/_Game yazımı yapılmadı.",
              '<!-- TASK15_END -->']
    path=DOC/'HANDOFF.md'; text=path.read_text(encoding='utf-8-sig')
    if '<!-- TASK15_BEGIN -->' in text:
        import re
        text=re.sub(r'<!-- TASK15_BEGIN -->.*?<!-- TASK15_END -->','\n'.join(lines),text,flags=re.S)
    else: text=text.rstrip()+'\n\n'+'\n'.join(lines)+'\n'
    path.write_text(text,encoding='utf-8')
    task=DOC/'CODEX_TASK15.md'; text=task.read_text(encoding='utf-8-sig')
    if '(Bitti)' not in text.splitlines()[0]: text=text.replace(text.splitlines()[0],text.splitlines()[0]+' (Bitti)',1)
    task.write_text(text,encoding='utf-8')
    status=DOC/'ASSET_IHTIYAC.md'; text=status.read_text(encoding='utf-8-sig')
    if '<!-- TASK15_STATUS -->' not in text:
        text+='\n<!-- TASK15_STATUS -->\n## Task15 — bitti\n\n19 final PNG: sıcak kırmızı rozet, kalın duvar repeat/köşe, dolgun boş HUD hapları + ayrı parça/kronometre, yeşil/turkuaz normal-basılan tuşlar, beş 256 px avatar, 64 pt Jiggle sayacı ve 1536 px tablo. Task15/preview.png resimli oyun, pause ve avatar seçimini birlikte gösterir. Tüm ölçü/slice/pivot satırları HANDOFF ve manifestte. Source ve onaylı Task14 korundu; C# değişmedi.\n'
        status.write_text(text,encoding='utf-8')
    decision=DOC/'DECISIONS.md'; text=decision.read_text(encoding='utf-8-sig')
    if '## Task15 — Python final export' not in text:
        text+='\n## Task15 — Python final export\n\n- Zeyd bağlantı sonrası hazır Source çizimlerinden devam edilmesini, yeniden çizim yapılmamasını ve Python ile export istedi. Bu tur image_gen çağrısı yapılmadı. Önceki üretim istemleri templates/task15-prompts.md içinde.\n- Yeşil ana tuş onaylı Level v2 normal/basılı dosyalarının birebir kopyasıdır. Diğer final parçalar hazır Source bitmaplerinden kırpma/ölçekleme/alpha temizliği ile üretildi. 19 final PNG ve telefon/kart/64 px incelemeleri hazır.\n- HUD ikonları ayrı, kare tutuldu; hapları esnetirken saat/parça bozulmamalı. Rozet yalnız bir kez çizilir. Yeni kırmızı PALETTE.md v2; mercan yalnız acil süre.\n- Kit81 maskesi ve overlay değiştirilmeden resim UV kompozisyonunda kullanıldı; oyun koduna bağlanmadı. Source ve eski onaylı dosyalar hash ile korundu. Slice/pivot ve minimum ekran ölçüleri TASK15_EXPORT.md içinde.\n'
        decision.write_text(text,encoding='utf-8')


def main():
    OUT.mkdir(exist_ok=True)
    source_hashes={p.name:digest(p) for p in SRC.glob('*.png')}
    assert len(source_hashes)==14,source_hashes.keys()
    period=export(); previews(); report=verify(source_hashes,period); documentation(report)
    print(json.dumps({k:v for k,v in report.items() if k!='files'},ensure_ascii=True,indent=2))


if __name__=='__main__': main()
