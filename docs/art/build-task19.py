"""Task19 review composites. Only writes Task19; approved sources stay untouched."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageOps, ImageFilter
import json, math, html

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Assets/Art/Menu/Task19'
ART=ROOT/'Assets/Art'
OUT.mkdir(parents=True,exist_ok=True)
(OUT/'layers').mkdir(exist_ok=True)
S=3; W=1125; H=2436
INK='#0B2A36'; CREAM='#F8EBD0'; GOLD='#FBBA0E'; TEAL='#1C7F8C'
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
USED=set(); RECORDS=[]; SCREENS=[]
def load(p):
    p=Path(p); p=p if p.is_absolute() else ART/p
    USED.add(str(p.relative_to(ROOT)).replace('\\','/'))
    return Image.open(p).convert('RGBA')
def sprite(im,p,box,crop=False):
    a=load(p) if not isinstance(p,Image.Image) else p
    if crop:a=a.crop(a.getbbox())
    x,y,w,h=box; im.alpha_composite(a.resize((round(w*3),round(h*3)),Image.Resampling.LANCZOS),(round(x*3),round(y*3)))
def rr(im,box,fill,outline=None,r=12,width=2):
    ImageDraw.Draw(im).rounded_rectangle(tuple(round(v*3) for v in box),radius=r*3,fill=fill,outline=outline,width=width*3)
def line(im,pts,fill,width=2):ImageDraw.Draw(im).line([(int(x*3),int(y*3)) for x,y in pts],fill=fill,width=width*3,joint='curve')
def text(t,x,y,size=18,color=INK,anchor='mm',maxw=None):
    f=ImageFont.truetype(str(FONT),round(size*3))
    while maxw and f.getlength(t)>maxw*3:
        size-=.5; f=ImageFont.truetype(str(FONT),round(size*3))
    # Repo Fredoka is Latin-subset: compose missing Turkish marks separately.
    base=t.translate(str.maketrans({'ş':'s','Ş':'S','ğ':'g','Ğ':'G','İ':'I'}))
    d=ImageDraw.Draw(T);d.text((x*3,y*3),base,font=f,fill=color,anchor=anchor)
    width=f.getlength(base);left=x*3-(width/2 if anchor[0]=='m' else width if anchor[0]=='r' else 0)
    for i,c in enumerate(t):
        if c not in 'şŞğĞİ':continue
        cx=left+f.getlength(base[:i])+f.getlength(base[i])/2
        cb=d.textbbox((left+f.getlength(base[:i]),y*3),base[i],font=f,anchor='l'+anchor[1])
        thick=max(2,round(size*.10*3))
        if c=='İ':
            cy=cb[1]-size*.14*3;rad=size*.055*3
            d.ellipse((cx-rad,cy-rad,cx+rad,cy+rad),fill=color)
        elif c in 'şŞ':
            cy=cb[3]+size*.03*3
            d.line([(cx+size*.06*3,cy),(cx,cy+size*.10*3),(cx-size*.07*3,cy+size*.13*3)],fill=color,width=thick)
        else:
            cy=cb[1]-size*.19*3
            d.arc((cx-size*.18*3,cy-size*.12*3,cx+size*.18*3,cy+size*.12*3),0,180,fill=color,width=thick)
    RECORDS.append(dict(text=t,x=x*3,y=y*3,fontSize=round(size*3),color=color,anchor=anchor))
def start(wall=True):
    global T,RECORDS
    T=Image.new('RGBA',(W,H)); RECORDS=[]
    im=Image.new('RGBA',(W,H),INK)
    sprite(im,'Review/Koridor/2-ortali-yan-blok-v04.png',(0,0,375,812))
    if wall:
        shade=Image.new('RGBA',(W,H),(11,42,54,220));im=Image.alpha_composite(im,shade)
    return im
def title(im,name,sub):
    text(name,26,83,31,CREAM,'lm');text(sub,348,85,18,CREAM,'rm')
    line(im,[(26,116),(349,116)],GOLD,1)
def nav(im,active,y=716):
    sprite(im,'Menu/Task13/bottom-nav.png',(0,y+8,375,54))
    names=['home','collection','jiggles','trophy']; labels=['Ev','Koleksiyon','Jiggles','Başarılar']
    for i,(n,l) in enumerate(zip(names,labels)):
        x=47+i*94
        if i==active:sprite(im,'Menu/Task13/nav-active.png',(x-41,y-9,82,72))
        sz=59 if i==active else 48
        sprite(im,f'Menu/Task13/nav-{n}.png',(x-sz/2,y-3,sz,sz))
        text(l,x,y+53,12,CREAM)
    sprite(im,'Menu/Task13/notification-badge.png',(343,y,15,15))
def button(im,label,x,y,w=240,h=70,primary=False):
    p='UI/PlayButton/menu-play-button-v2.png' if primary else 'Gameplay/Task15/btn-secondary-v2.png'
    sprite(im,p,(x,y,w,h));text(label,x+w/2,y+h*.49,25 if primary else 20,CREAM,maxw=w-30)
def progress(im,x,y,w,frac,label):
    sprite(im,'Menu/Task13/progress-track.png',(x,y,w,28))
    # Clipped existing turquoise fill, avoiding green on secondary UI.
    a=load('Menu/Task13/progress-fill.png').resize((int((w-12)*3),36))
    a=a.crop((0,0,max(1,int(a.width*frac)),a.height));im.alpha_composite(a,(int((x+6)*3),int((y+8)*3)))
    text(label,x+w/2,y+14,12,CREAM)
def panel(im,box,heading):
    x,y,w,h=box;sprite(im,'Gameplay/Task14/card-panel.png',box)
    text(heading,x+w/2,y+h*.064,27,CREAM)
def close(im,x=320,y=188):
    rr(im,(x-15,y-15,x+15,y+15),TEAL,GOLD,r=10);text('×',x,y-1,26,CREAM)
def star(im,x,y,size=14):sprite(im,'Gameplay/Task14/star-full.png',(x,y,size,size))
def stars(im,x,y,n=3,size=14):
    for i in range(n):star(im,x+i*(size+1),y,size)
FRAME=load('Menu/InfiniteCorridor/frame-empty-v01.png');FRAME=FRAME.crop(FRAME.getbbox())
def painting(im,n,box,locked=False,hard=False):
    x,y,w,h=box
    if locked:rr(im,(x+8,y+10,x+w-8,y+h-10),INK,r=1)
    else:sprite(im,f'Paintings/painting-{n:02}.jpg',(x+10,y+12,w-20,h-24))
    sprite(im,FRAME,box)
    if locked:sprite(im,'Gameplay/Task14/booster-lock.png',(x+w/2-10,y+h/2-12,20,24))
    if hard:
        ImageDraw.Draw(im).polygon([(int((x+w-26)*3),int((y+5)*3)),(int((x+w-4)*3),int((y+5)*3)),(int((x+w-4)*3),int((y+27)*3))],fill='#D62822')
def glow(im,box,r=14):
    a=Image.new('RGBA',im.size);rr(a,box,'#FBBA0E',r=r)
    a=a.filter(ImageFilter.GaussianBlur(15));a.putalpha(a.getchannel('A').point(lambda p:int(p*.45)));im.alpha_composite(a)
def finish(im,name,desc):
    im.save(OUT/'layers'/f'{name}-art.png');T.save(OUT/'layers'/f'{name}-text.png')
    (OUT/'layers'/f'{name}-text.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
    full=Image.alpha_composite(im,T); full.convert('RGB').save(OUT/f'{name}.png')
    SCREENS.append((name,desc)); return full

im=start(False)
# Preserve approved corridor perspective while inserting real level images.
# Main and side pictures are placed within existing carved frame openings.
def quad_insert(im,n,quad):
    # Pillow QUAD maps output rectangle to source quadrilateral; solve perspective inverse.
    import numpy as np
    src=[(0,0),(1024,0),(1024,1024),(0,1024)]; dst=[(x*3,y*3) for x,y in quad]
    A=[];b=[]
    for (x,y),(u,v) in zip(dst,src):
        A.extend([[x,y,1,0,0,0,-u*x,-u*y],[0,0,0,x,y,1,-v*x,-v*y]]);b.extend([u,v])
    coef=np.linalg.solve(A,b)
    p=load(f'Paintings/painting-{n:02}.jpg').transform((W,H),Image.Transform.PERSPECTIVE,coef,Image.Resampling.BICUBIC)
    mask=Image.new('L',(W,H));ImageDraw.Draw(mask).polygon(dst,fill=255);im.paste(p,(0,0),mask)
quad_insert(im,12,[(128,160),(242,186),(242,438),(127,432)])
quad_insert(im,11,[(324,350),(337,356),(337,446),(324,440)])
sprite(im,'Menu/Task13/profile-box.png',(17,50,172,56));sprite(im,'Gameplay/Task15/avatar-blue.png',(19,51,51,51))
text('Zeyd',124,69,18);text('Bölüm 12/40',126,89,11)
sprite(im,'Menu/Task13/settings-button.png',(309,51,49,49))
for x,token,label in [(79,'token-star','31'),(199,'token-jiggle','1.248')]:
    sprite(im,'Menu/Task13/counter-pill.png',(x,119,110,34));sprite(im,f'Menu/Task13/{token}.png',(x-8,115,37,37));text(label,x+66,136,17)
progress(im,72,553,235,.3,'12/40 tablo')
button(im,'Bölüm 12',67,590,255,115,True)
sprite(im,'Menu/Task13/levels-button.png',(14,604,51,51));text('Bölümler',40,667,11,CREAM)
nav(im,0);HOME=finish(im,'01-ev','Onaylı koridor, gerçek 12. tablo ve ana eylem.')

HARD={19,23,25,30,35,36,38}
def collection_page(first=1):
    im=start();title(im,'Koleksiyon','12 / 40')
    for k,n in enumerate(range(first,min(first+15,41))):
        x=19+(k%3)*119;y=139+(k//3)*108
        if n==12:glow(im,(x+2,y+2,x+98,y+74))
        painting(im,n,(x,y,100,76),locked=n>12,hard=n in HARD)
        if n<12:stars(im,x+29,y+77,size=12)
        elif n==12:
            rr(im,(x+12,y+74,x+88,y+92),TEAL,GOLD,r=7);text('Sıradaki',x+50,y+83,10,CREAM)
        else:text(f'Bölüm {n}',x+50,y+84,11,CREAM)
    text('40 tablo · Müzende keşfet',187,695,12,CREAM);nav(im,1);return im
im=collection_page();finish(im,'02-koleksiyon','Üç sütun; kazanılan, sıradaki ve kilitli tablolar. Aşağı kaydırılır.')
# Additional review pages expose all 40 cells and every hard marker.
for first in (16,31):
    im=collection_page(first);Image.alpha_composite(im,T).save(OUT/'source'/f'koleksiyon-{first:02}.png')

im=start();panel(im,(22,163,331,483),'Tablo detayı');close(im,324,194)
painting(im,7,(57,243,261,197));
themes={p['level']:p['theme'] for b in range(1,5) for p in json.loads((ART/f'Paintings/manifest-batch-{b}.json').read_text(encoding='utf-8-sig'))['paintings']}
text(themes[7],187,468,24,maxw=280);text("Bölüm 7'de kazanıldı",187,504,15)
stars(im,137,529,size=21);text('· 0:42',229,539,15)
button(im,'Tekrar oyna',67,570,241,58);finish(im,'02b-tablo-detay','Gerçek tablo adı, yıldızlar, süre ve tekrar oynama.')

im=start();title(im,'Jiggles','Ailen büyüyor')
sprite(im,'Gameplay/Task15/jiggle-counter-icon.png',(30,139,57,57));text('1.248 Jiggle',221,153,27,CREAM);text('kurtarıldı',221,184,18,CREAM)
shelf=OUT/'source/jiggles-shelf.png'
if not shelf.exists():raise RuntimeError('Generate and copy shelf illustration first.')
sprite(im,shelf,(17,222,341,262))
for x,y,n in [(152,326,211),(225,326,190),(119,425,328),(188,425,277),(256,425,242)]:
    rr(im,(x-24,y-10,x+24,y+10),CREAM,GOLD,r=7);text(str(n),x,y,13)
text('Avatarım',27,514,23,CREAM,'lm')
for i,c in enumerate(['blue','pink','red','green','purple']):
    x=24+i*67
    if i==0:rr(im,(x-3,539,x+62,604),GOLD,r=33)
    sprite(im,f'Gameplay/Task15/avatar-{c}.png',(x,542,59,59))
text('Birlikte daha ileri',187,631,18,CREAM)
line(im,[(57,665),(318,665)],GOLD,3)
for x,n,label in [(58,'500','Çerçeve'),(187,'1.000','Rozet'),(317,'2.000','Portre')]:
    if label=='Çerçeve':sprite(im,FRAME,(x-16,654,32,22))
    elif label=='Rozet':sprite(im,'Gameplay/Task14/star-full.png',(x-13,651,26,26))
    else:sprite(im,'Gameplay/Task15/avatar-blue.png',(x-13,651,26,26))
    text(n,x,686,11,CREAM);text(label,x,701,10,CREAM)
nav(im,2);finish(im,'03-jiggles','Beş renk aile sahnesi, avatar seçimi ve görsel ödüller.')

ach=[('İlk Tablo','İlk bölümü bitir','1/1','collection'),('Koleksiyoncu','10 tablo topla','10/10','collection'),('Müze Sahibi','40 tabloyu topla','12/40','home'),('Bomba İmha Uzmanı','10 bombalı bölümü ilk denemede geç','7/10','bomb'),('Soğukkanlı','3 saniyeden az süre kala bitir','1/1','clock'),('Hatasız','Hiç sıkışmadan bitir','1/1','check'),('Yıldız Avcısı','30 bölümde 3 yıldız al','9/30','star'),('Zincir Kıran','25 zincirli parçayı çıkar','25/25','chain'),("Baron’a Kafa Tut",'Tüm zor bölümleri geç','0/7','baron')]
def medal(im,kind,x,y,locked=False):
    rr(im,(x-20,y-20,x+20,y+20),INK if locked else GOLD,'#CE5D01',r=22)
    paths={'collection':'Menu/Task13/nav-collection.png','home':'Menu/Task13/nav-home.png','bomb':'Gameplay/Task14/bomb-384.png','clock':'Gameplay/Task14/booster-clock.png','star':'Gameplay/Task14/star-full.png','chain':'Gameplay/Task14/chain-link-repeat-96x48.png'}
    if kind=='collector':
        sprite(im,FRAME,(x-15,y-13,25,18));sprite(im,FRAME,(x-8,y-4,25,18))
    elif kind=='bomb':
        rr(im,(x-12,y-8,x+12,y+16),INK,r=13)
        line(im,[(x+3,y-9),(x+6,y-16),(x+13,y-15)],INK,3)
        line(im,[(x+13,y-18),(x+13,y-12)],'#D62822',2)
        rr(im,(x-7,y-4,x-3,y),CREAM,r=3)
    elif kind=='hourglass':
        line(im,[(x-10,y-13),(x+10,y-13),(x-8,y+13),(x+8,y+13),(x-10,y-13)],INK,2)
        line(im,[(x-12,y-14),(x+12,y-14)],INK,3);line(im,[(x-12,y+14),(x+12,y+14)],INK,3)
    elif kind in paths:sprite(im,paths[kind],(x-15,y-15,30,30))
    elif kind=='check':line(im,[(x-10,y),(x-2,y+8),(x+11,y-9)],INK,4)
    else:
        # Baron hat silhouette, a dedicated achievement emblem.
        hat='#586A70' if locked else INK
        rr(im,(x-10,y-13,x+10,y+8),hat,r=4);rr(im,(x-16,y+6,x+16,y+11),hat,r=3);line(im,[(x-9,y+3),(x+9,y+3)],'#D62822',3)
im=start();title(im,'Başarılar','5 / 16')
for i,(name,desc,p,kind) in enumerate(ach):
    if i==1:kind='collector'
    if i==4:kind='hourglass'
    y=132+i*62;rr(im,(18,y,357,y+57),CREAM,GOLD,r=11)
    locked=i==8;medal(im,kind,46,y+27,locked)
    text(name,76,y+14,15,anchor='lm',maxw=238);text(desc,76,y+32,10,anchor='lm',maxw=238)
    a,b=map(int,p.split('/'));rr(im,(77,y+44,273,y+49),INK,r=2)
    if a:rr(im,(77,y+44,77+196*a/b,y+49),TEAL,r=2)
    text(p,299,y+47,10)
    if a==b:line(im,[(330,y+25),(335,y+30),(345,y+18)],TEAL,3)
    elif locked:sprite(im,'Gameplay/Task14/booster-lock.png',(328,y+15,18,22))
    else:text('•',337,y+25,19)
text('9 başarı gösteriliyor · 16 toplam',187,706,11,CREAM);nav(im,3);finish(im,'04-basarilar','İstenen dokuz başarı; beşi tamamlandı, üçü sürüyor, biri kilitli.')

im=start();panel(im,(22,137,331,546),'Profil');close(im,324,172)
sprite(im,'Gameplay/Task15/avatar-blue.png',(145,207,86,86));text('Zeyd',187,315,29)
line(im,[(239,317),(249,307)],TEAL,4);line(im,[(237,321),(240,316)],INK,3)
stats=[('12/40','Bölüm','home'),('31','Yıldız','star'),('1.248','Kurtarılan Jiggle','jiggle'),('18 dk','Toplam süre','clock'),('%75','İlk denemede','check'),('5/16','Başarı','trophy')]
for i,(v,l,k) in enumerate(stats):
    x=44+i%2*149;y=349+i//2*80;rr(im,(x,y,x+138,y+73),CREAM,'#CE5D01',r=10)
    if k=='jiggle':sprite(im,'Gameplay/Task15/jiggle-counter-icon.png',(x+9,y+8,23,23))
    elif k=='trophy':sprite(im,'Menu/Task13/nav-trophy.png',(x+9,y+8,23,23))
    else:medal(im,k,x+22,y+20)
    text(v,x+86,y+24,23);text(l,x+69,y+55,12)
button(im,'Avatarı değiştir',66,602,243,59);finish(im,'05-profil','Oyuncu adı, düzenleme simgesi ve altı istatistik.')

im=start();panel(im,(22,166,331,481),'Ayarlar');close(im,323,198)
for i,(label,on,icon) in enumerate([('Ses',True,'♪'),('Müzik',True,'♫'),('Titreşim',False,'≋')]):
    y=268+i*66
    if i<2:
        line(im,[(59,y+7),(59,y-10),(70,y-13),(70,y+4)],INK,2)
        rr(im,(53,y+4,61,y+10),INK,r=4)
        if i==1:rr(im,(64,y+1,72,y+7),INK,r=4)
    else:
        rr(im,(58,y-12,70,y+12),INK,r=3)
        line(im,[(51,y-7),(48,y),(51,y+7)],TEAL,2);line(im,[(77,y-7),(80,y),(77,y+7)],TEAL,2)
    text(label,100,y,21,anchor='lm')
    rr(im,(255,y-15,319,y+15),'#4A9D05' if on else INK,GOLD,r=16)
    cx=303 if on else 271;rr(im,(cx-11,y-11,cx+11,y+11),CREAM,r=12)
text('Dil',62,478,19,anchor='lm')
rr(im,(124,456,319,501),TEAL,GOLD,r=12);rr(im,(128,460,220,497),INK,r=9)
text('Türkçe',173,478,16,CREAM);text('English',269,478,16,CREAM)
button(im,'Gizlilik',47,543,133,46);button(im,'Bize yaz',196,543,133,46)
text('v0.5.1',187,616,12,'#586A70');finish(im,'06-ayarlar','Ses ve müzik açık, titreşim kapalı; Türkçe seçili.')

def levels_page(first=7):
    im=start();text('Bölümler',60,83,31,CREAM,'lm');text('40 tablo',348,85,18,CREAM,'rm');text('‹',25,83,39,CREAM)
    # Upward path: completed levels at bottom, future at top.
    nodes=[]
    for i,n in enumerate(range(first,first+7)):
        x=118 if i%2==0 else 255;y=646-i*77;nodes.append((x,y,n))
    line(im,[(x,y) for x,y,n in nodes],'#94160F',27);line(im,[(x,y) for x,y,n in nodes],'#D62822',16)
    for x,y,n in nodes:
        current=n==12;complete=n<12;hard=n in HARD
        radius=33 if current else 27
        if current:glow(im,(x-35,y-35,x+35,y+35),r=35)
        rr(im,(x-radius,y-radius,x+radius,y+radius),'#1C7F8C' if complete else INK,'#D62822' if hard else GOLD,r=radius+1,width=4 if current else 2)
        text(str(n),x,y,22,CREAM)
        if complete:stars(im,x-19,y+28,size=12)
        if current:
            sprite(im,'Gameplay/Task15/avatar-blue.png',(x-20,y-69,40,40));text('Şu anki',x+55,y,12,CREAM)
        if n>12:sprite(im,'Gameplay/Task14/booster-lock.png',(x+20,y-16,16,20))
        if hard:
            face=load('Characters/Baron-reference-v01.png').crop((430,610,707,872))
            mask=Image.new('L',face.size);ImageDraw.Draw(mask).ellipse((0,0,face.width,face.height),fill=255);face.putalpha(mask)
            sprite(im,face,(x+22,y-17,34,34))
        if n%10==0:painting(im,n,(x+48 if x<180 else x-108,y-30,60,45))
    text('Yukarı kaydır · Sıradaki tabloyu bul',187,709,12,CREAM);return im
im=levels_page();finish(im,'07-bolumler','Tamamlanan, şu anki ve kilitli düğümler; yukarı ilerleyen halı yolu.')
for first in (17,22,28,34):
    im=levels_page(first);Image.alpha_composite(im,T).save(OUT/'source'/f'bolumler-{first:02}.png')
im=start();title(im,'Sekme durumları','');text('4 görünüm',349,116,12,CREAM,'rm')
for i,l in enumerate(['Ev','Koleksiyon','Jiggles','Başarılar']):
    y=183+i*141;text(l,25,y-27,20,CREAM,'lm');nav(im,i,y)
finish(im,'08-sekme-durumlari','Dört seçili durum; Başarılar üzerinde yeni bildirim örneği.')

(OUT/'source/sources.json').write_text(json.dumps(sorted(USED),indent=2,ensure_ascii=False),encoding='utf8')
# Review board with the complete scroll references beside the nine phone mockups.
cards=''.join(f'<article><a href="{n}.png"><div class="phone"><img src="{n}.png" alt="{html.escape(d)}"></div></a><h2>{n}</h2><p>{d}</p><details><summary>Metin katmanı</summary><a href="layers/{n}-text.png">Şeffaf PNG</a> · <a href="layers/{n}-text.json">Fredoka konumları</a></details></article>' for n,d in SCREENS)
extra=''.join(f'<a href="source/{p.name}"><img src="source/{p.name}" alt="Kaydırma devamı {p.stem}"></a>' for p in sorted((OUT/'source').glob('*.png')) if p.stem.startswith(('koleksiyon-','bolumler-')))
(OUT/'inceleme.html').write_text('''<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Task19 · Menü incelemesi</title><style>*{box-sizing:border-box}body{background:#0B2A36;color:#F8EBD0;font:16px system-ui;margin:28px}h1{font-size:28px}a{color:#FBBA0E}.row{display:flex;gap:26px;overflow:auto;align-items:flex-start;padding:20px 0 35px}article{flex:0 0 297px}.phone{padding:8px;background:#06151c;border:2px solid #49626a;border-radius:36px;box-shadow:0 15px 35px #0008}.phone img{display:block;width:277px;border-radius:27px}h2{font-size:18px}p{line-height:1.6}details{font-size:13px}.extras{display:flex;gap:20px;overflow:auto}.extras img{width:210px;border-radius:16px}</style><h1>Reverse Solver · Task19</h1><p>9 tam ekran tasarım · 375×812 pt / 1125×2436 px · @3x<br>Üst 44 pt ve alt 34 pt sistem alanı; dört sekmede şerit aynı konumda. Görsele tıkla: tam boy PNG.</p><div class="row">'''+cards+'''</div><h2>Kaydırılan içeriğin devamı</h2><p>Koleksiyonda 40 hücrenin tamamı; bölüm yolunda zor düğümler ve 20/30/40 tablo durakları.</p><div class="extras">'''+extra+'''</div><p>Metinler ayrı şeffaf katman ve konum JSON'u olarak saklanır. Task20 parça export'u bu ekranların onayından sonra yapılacak.</p></html>''',encoding='utf8')
thumb=Image.new('RGB',(375*3,812*3),INK)
for i,(n,_) in enumerate(SCREENS):thumb.paste(Image.open(OUT/f'{n}.png').resize((375,812)),(i%3*375,i//3*812))
thumb.save(OUT/'source/contact-sheet.jpg',quality=90)
print('Rendered',len(SCREENS),'screens; dimensions verified below.')
for n,_ in SCREENS:
    p=OUT/f'{n}.png';assert Image.open(p).size==(1125,2436)
    a=Image.open(OUT/'layers'/f'{n}-text.png'); assert a.crop((0,0,W,132)).getbbox() is None;assert a.crop((0,2334,W,H)).getbbox() is None
print('9 PNGs @1125x2436; all text layers preserve top/bottom safe areas.')
