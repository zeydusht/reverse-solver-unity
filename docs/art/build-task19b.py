"""Compose the two Task19B pilots from image_gen backplates and approved assets.

No flat UI surfaces are drawn here. Only existing sprites, painting inserts,
Fredoka text and the requested comparison board are composited.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageOps, ImageFilter, features
import json, unicodedata, hashlib

ROOT=Path(__file__).resolve().parents[2]
ART=ROOT/'Assets/Art'; OUT=ART/'Menu/Task19B'; SRC=OUT/'source'; LAY=OUT/'layers'
for p in (OUT,SRC,LAY):p.mkdir(parents=True,exist_ok=True)
SIZE=(1125,2436); SCALE=3
INK='#0B2A36'; CREAM='#F8EBD0'
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
RECORDS=[]; USED=set()
MARKS={}
def read(p):
    p=Path(p);p=p if p.is_absolute() else ART/p
    USED.add(str(p.relative_to(ROOT)).replace('\\','/'))
    return Image.open(p).convert('RGBA')
def sprite(im,path,box,fit=False):
    a=read(path) if not isinstance(path,Image.Image) else path
    x,y,w,h=box;size=(round(w*3),round(h*3))
    a=ImageOps.fit(a,size,method=Image.Resampling.LANCZOS) if fit else a.resize(size,Image.Resampling.LANCZOS)
    im.alpha_composite(a,(round(x*3),round(y*3)))
def font(size):
    return ImageFont.truetype(str(FONT),round(size*3))
def txt(t,x,y,size=15,color=INK,anchor='mm',outline=0,shadow=False,maxw=None):
    # BASIC FreeType lacks combining-mark placement (RAQM unavailable).
    # Position genuine Fredoka mark glyphs over the corresponding base glyph.
    # Keep existing precomposed Turkish glyphs (ö/ü/ç/ı) intact.
    shaped=t.translate(str.maketrans({'ş':'s','Ş':'S','ğ':'g','Ğ':'G','İ':'I'}))
    f=font(size)
    if maxw:assert f.getlength(shaped)<=maxw*3,(t,f.getlength(shaped)/3,maxw)
    mask=Image.new('L',SIZE);d=ImageDraw.Draw(mask)
    d.text((x*3,y*3),shaped,font=f,anchor=anchor,fill=255)
    bbox=d.textbbox((x*3,y*3),shaped,font=f,anchor=anchor)
    baseline=bbox[1]-f.getbbox(shaped,anchor='ls')[1]
    width=f.getlength(shaped)
    left=x*3-(width/2 if anchor[0]=='m' else width if anchor[0]=='r' else 0)
    for i,c in enumerate(t):
        if c not in 'şŞğĞİ':continue
        cp=807 if c in 'şŞ' else 774 if c in 'ğĞ' else 775
        key=(round(size*3),cp)
        if key not in MARKS:
            mark=Image.new('L',(int(size*12),int(size*12)))
            ImageDraw.Draw(mark).text((size*4,size*6),chr(cp),font=f,anchor='ls',fill=255)
            MARKS[key]=mark.crop(mark.getbbox())
        mark=MARKS[key]
        cx=left+f.getlength(shaped[:i])+f.getlength(shaped[i])/2
        cb=d.textbbox((left+f.getlength(shaped[:i]),baseline),shaped[i],font=f,anchor='ls')
        my=cb[3]-1 if cp==807 else cb[1]-mark.height-max(2,round(size*.05*3))
        mask.paste(mark,(round(cx-mark.width/2),round(my)),mark)
    # One-pixel optical emboldening gives the available SemiBold a Bold weight.
    mask=mask.filter(ImageFilter.MaxFilter(3))
    sw=round(outline*3)
    edge=mask.filter(ImageFilter.MaxFilter(sw*2+1)) if sw else mask
    if shadow:
        layer=Image.new('RGBA',SIZE,INK);layer.putalpha(edge)
        TEXT.alpha_composite(layer,(0,5))
    if sw:
        layer=Image.new('RGBA',SIZE,INK);layer.putalpha(edge);TEXT.alpha_composite(layer)
    layer=Image.new('RGBA',SIZE,color);layer.putalpha(mask);TEXT.alpha_composite(layer)
    RECORDS.append(dict(text=t,renderText=shaped,font='Fredoka-SemiBold',requestedWeight='Bold',
                        fontSizePx=round(size*3),x=round(x*3),y=round(y*3),color=color,anchor=anchor,
                        outlinePx=sw,shadow=shadow))
def start(name):
    global TEXT,RECORDS
    TEXT=Image.new('RGBA',SIZE);RECORDS=[]
    filename='koleksiyon-art-v2.png' if name=='koleksiyon' else f'{name}-art.png'
    return read(SRC/filename).resize(SIZE,Image.Resampling.LANCZOS)
def nav(im,active):
    sprite(im,'Menu/Task13/bottom-nav.png',(0,719,375,59))
    for i,(n,label) in enumerate(zip(('home','collection','jiggles','trophy'),('Ev','Koleksiyon','Jiggles','Başarılar'))):
        x=47+94*i
        if i==active:sprite(im,'Menu/Task13/nav-active.png',(x-42,701,84,76))
        sz=49 if i==active else 43
        sprite(im,f'Menu/Task13/nav-{n}.png',(x-sz/2,706 if i==active else 713,sz,sz))
        # Render labels last and below icon silhouettes; never behind selection frame.
        txt(label,x,766,15,CREAM,outline=.45,maxw=92)
    sprite(im,'Menu/Task13/notification-badge.png',(342,710,17,17))
def save(im,name):
    assert all(r['fontSizePx']>=45 for r in RECORDS)
    im.save(LAY/f'{name}-art.png')
    TEXT.save(LAY/f'{name}-text.png')
    (LAY/f'{name}-text.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
    result=Image.alpha_composite(im,TEXT);result.convert('RGB').save(OUT/f'{name}.png')
    assert result.size==SIZE
    assert TEXT.crop((0,0,1125,132)).getbbox() is None
    assert TEXT.crop((0,2334,1125,2436)).getbbox() is None
    return result

im=start('koleksiyon')
txt('Koleksiyon',139,108,29,CREAM,outline=1,shadow=True,maxw=208)
# Reuse original Task13 counter pill and approved star cap.
sprite(im,'Menu/Task13/counter-pill.png',(268,96,93,37))
sprite(im,'Menu/Task13/token-star.png',(301,76,28,28))
txt('11 / 40',316,114,18,maxw=80)
# Frame openings measured on generated backplate (853x1844 source).
# All actual game paintings are inserted inside their original ornate rims.
cols=[(107,263),(349,503),(591,746)]
rows=[(394,485),(595,687),(796,889),(998,1089),(1199,1291)]
sx=375/853;sy=812/1844
for n in range(1,16):
    col=(n-1)%3;row=(n-1)//3
    x0,x1=cols[col];y0,y1=rows[row]
    x=x0*sx;y=y0*sy;w=(x1-x0)*sx;h=(y1-y0)*sy
    if n<=11:sprite(im,f'Paintings/painting-{n:02}.jpg',(x,y,w,h),fit=True)
    # Revised generated backplate already contains the chunky locks for 13–15.
    cx=(x0+x1)/2*sx;labely=(y1+43)*sy
    if n<=11:
        count=3 if n<=9 else 2
        for j in range(count):sprite(im,'Gameplay/Task14/star-full.png',(cx-count*15/2+j*15,labely-9,15,15))
    elif n==12:txt('Sıradaki',cx,labely,15,CREAM,outline=.5,maxw=94)
    else:txt(f'Bölüm {n}',cx,labely,15,CREAM,outline=.5,maxw=95)
nav(im,1);COLLECTION=save(im,'02-koleksiyon')

im=start('basarilar')
txt('Başarılar',187,132,32,CREAM,outline=1,shadow=True,maxw=245)
# Header counter is placed in the bright architectural margin above the plaque.
sprite(im,'Menu/Task13/counter-pill.png',(259,52,103,34));txt('5 / 16',313,69,18,maxw=85)
items=[
    ('İlk Tablo',['İlk bölümü bitir'],'1/1',193.5),
    ('Koleksiyoncu',['10 tablo topla'],'10/10',287.5),
    ('Bomba İmha Uzmanı',['10 bombalı bölümü','ilk denemede geç'],'7/10',381.8),
    ('Yıldız Avcısı',['30 bölümde','3 yıldız al'],'9/30',476),
    ('Baron’a Kafa Tut',['Tüm zor bölümleri geç'],'0/7',569.2),
]
for i,(name,desc,count,y) in enumerate(items):
    txt(name,131,y+11,15.5,anchor='lm',maxw=164)
    if len(desc)==1:txt(desc[0],131,y+34,15,anchor='lm',maxw=164)
    else:
        txt(desc[0],131,y+27,15,anchor='lm',maxw=164)
        txt(desc[1],131,y+41,15,anchor='lm',maxw=164)
    txt(count,205,y+60.6,15,CREAM,outline=.45,maxw=90)
txt('5 / 16 başarı kazanıldı',187,683,15,CREAM,outline=.6,shadow=True,maxw=300)
nav(im,3);ACHIEVEMENTS=save(im,'04-basarilar')

# Four equal-size phones: originals on the left, pilots on the right.
BW=1692;BH=1100
board=Image.new('RGB',(BW,BH),INK);d=ImageDraw.Draw(board)
bf=ImageFont.truetype(str(FONT),30)
for x,label in [(32,'ONAYLI OYUN'),(860,'TASK19B / 2 EKRAN')]:
    d.text((x,21),label,font=bf,fill=CREAM)
entries=[('Menu/Task13/preview-menu.png','Ana menü · Task13'),
         ('Gameplay/Task15/preview-pause.png','Durduruldu · Task15'),
         (OUT/'02-koleksiyon.png','Koleksiyon · 11/40'),
         (OUT/'04-basarilar.png','Kupa salonu · 5/16')]
for i,(p,label) in enumerate(entries):
    a=read(p);target=(375,812)
    # Preserve the approved 390x844 phone's aspect ratio, with subpixel fit only.
    a=ImageOps.contain(a,target,Image.Resampling.LANCZOS)
    x=32+i*415;y=104
    d.rounded_rectangle((x-6,y-6,x+381,y+818),radius=25,fill='#0F4F5A',outline='#FBBA0E',width=2)
    board.paste(a,(x+(375-a.width)//2,y+(812-a.height)//2),a)
    d.text((x,947),label,font=ImageFont.truetype(str(FONT),24),fill=CREAM)
d.text((32,1030),'Task13 / Task15                         Task19B: 2 ekran',font=ImageFont.truetype(str(FONT),25),fill=CREAM)
board.save(OUT/'pilot-karsilastirma.png')

(SRC/'sources.json').write_text(json.dumps(sorted(USED),ensure_ascii=False,indent=2),encoding='utf8')
report=dict(mainScreens=2,size=SIZE,textMinPx=45,shaping='Fredoka base + positioned native combining glyphs; precomposed Latin retained',
            raqm=features.check_feature('raqm'),safeTopPx=132,safeBottomPx=102,navY=2157,
            completedPaintings=11,nextLevel=12,collectionStars=31,
            outputSha256={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in OUT.glob('*.png')})
(SRC/'validation.json').write_text(json.dumps(report,indent=2),encoding='utf8')
(OUT/'inceleme.html').write_text('''<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task19B · İki ekranlık pilot</title><style>*{box-sizing:border-box}body{margin:28px;background:#0B2A36;color:#F8EBD0;font:16px system-ui}a{color:#FBBA0E}img{display:block;max-width:100%;height:auto}.phones{display:flex;gap:30px;overflow:auto;margin:32px 0}.phone{min-width:330px;max-width:375px}.phone img{border:6px solid #0F4F5A;border-radius:24px}h1{font-size:26px}p{line-height:1.5}</style><h1>Task19B · Onaylı stille iki ekranlık pilot</h1><p>Royal Match referansları yalnız yerleşim ve işçilik için incelendi. Yeni sahneler, onaylı Task13/15 ve Level v2 dili temel alınarak üretildi.</p><a href="pilot-karsilastirma.png"><img src="pilot-karsilastirma.png" alt="Solda onaylı menü ve Durduruldu; sağda Koleksiyon ve Başarılar pilotları"></a><div class="phones"><div class="phone"><a href="02-koleksiyon.png"><img src="02-koleksiyon.png" alt="Koleksiyon pilotu"></a><p>11 kazanılmış tablo, 12. tablo sıradaki, üç kilitli hücre.</p><a href="layers/02-koleksiyon-text.png">Metin katmanı</a></div><div class="phone"><a href="04-basarilar.png"><img src="04-basarilar.png" alt="Başarılar pilotu"></a><p>İlk iki satır tamamlandı; ilerleyen ve kilitli başarı örnekleri.</p><a href="layers/04-basarilar-text.png">Metin katmanı</a></div></div><p>Her ekran 1125×2436 px. Pilot onayından sonra diğer yedi ekran aynı görünüşle ele alınacak.</p></html>''',encoding='utf8')
from html.parser import HTMLParser
class Links(HTMLParser):
    def handle_starttag(self,tag,attrs):
        for key,value in attrs:
            if key in ('src','href'):assert (OUT/value).is_file(),value
Links().feed((OUT/'inceleme.html').read_text(encoding='utf8'))
print('2 pilots @1125x2436 + comparison + review HTML saved.')
print('Safe areas and minimum 45px text assertions passed. RAQM:',report['raqm'])
