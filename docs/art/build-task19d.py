"""Task19D: nine review composites, with native image_gen art and real text.

Only writes Assets/Art/Menu/Task19D and docs/art/task19d. No Unity code/import edits.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter
from collections import deque
import ast,json,hashlib,html,math
import numpy as np

ROOT=Path(__file__).resolve().parents[2];ART=ROOT/'Assets/Art'
OUT=ART/'Menu/Task19D';WORK=ROOT/'docs/art/task19d';SRC=WORK/'sources';LAY=WORK/'layers'
for p in (OUT,SRC,LAY):p.mkdir(parents=True,exist_ok=True)
SIZE=(1125,2436);INK='#0B2A36';CREAM='#F8EBD0'
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
MARKS={};RECORDS=[];USED=set();SCREENS=[];PORTRAITS=[];CHECKS=[]
NAME='Zeynep Yılmaz'
# Import function definitions only; never run the archived script's main code.
tree=ast.parse((ROOT/'docs/art/build-task19b.py').read_text(encoding='utf8'))
exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ('font','txt')],type_ignores=[]),'<Fredoka typography>','exec'))
def read(p):
    p=Path(p);p=p if p.is_absolute() else ART/p
    USED.add(str(p.relative_to(ROOT)).replace('\\','/'))
    return Image.open(p).convert('RGBA')
def sprite(im,p,box,fit=False):
    a=read(p) if not isinstance(p,Image.Image) else p
    x,y,w,h=box;sz=(round(w*3),round(h*3))
    a=ImageOps.fit(a,sz,method=Image.Resampling.LANCZOS) if fit else a.resize(sz,Image.Resampling.LANCZOS)
    im.alpha_composite(a,(round(x*3),round(y*3)))
def tight(a):
    return a.crop(a.getchannel('A').point(lambda x:255 if x>40 else 0).getbbox())
def begin(source=None,color=INK):
    global TEXT,RECORDS
    TEXT=Image.new('RGBA',SIZE);RECORDS=[]
    return read(source).resize(SIZE,Image.Resampling.LANCZOS) if source else Image.new('RGBA',SIZE,color)
def fitted(t,x,y,maxsize,width,**kw):
    size=maxsize
    shaped=t.translate(str.maketrans({'ş':'s','Ş':'S','ğ':'g','Ğ':'G','İ':'I'}))
    while font(size).getlength(shaped)>width*3:size-=.1
    assert size>=9,(t,size)
    txt(t,x,y,size,maxw=width,**kw)
    return size
def save(im,name,description,active):
    im.save(LAY/f'{name}-art.png');TEXT.save(LAY/f'{name}-text.png')
    (LAY/f'{name}-text.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
    assert TEXT.crop((0,0,1125,132)).getbbox() is None,name
    assert TEXT.crop((0,2334,1125,2436)).getbbox() is None,name
    assert not any('yıldız' in r['text'].lower() or '⭐' in r['text'] for r in RECORDS),name
    result=Image.alpha_composite(im,TEXT);result.convert('RGB').save(OUT/f'{name}.png')
    SCREENS.append((name,description));CHECKS.append(dict(screen=name,size=SIZE,activeTab=active,
        minFontPx=min(r['fontSizePx'] for r in RECORDS),sha256=hashlib.sha256((OUT/f'{name}.png').read_bytes()).hexdigest()))
    return result

# Preserve the already-approved icon family and selection bubble, replacing
# ONLY their flat underlying rail with four newly generated recessed-bay rails.
source_c=(ROOT/'docs/art/build-task19c.py').read_text(encoding='utf8')
old_src=SRC;SRC=ROOT/'docs/art/task19c/sources'
exec(source_c[source_c.index('support=read('):source_c.index('def nav(')])
SRC=old_src
bay=read(SRC/'nav-bays.png');a=bay.getchannel('A');strong=a.point(lambda v:255 if v>64 else 0)
runs=[]
for y in range(a.height):
    if strong.crop((0,y,a.width,y+1)).getbbox():
        if not runs or y>runs[-1][1]+1:runs.append([y,y])
        else:runs[-1][1]=y
runs=[r for r in runs if r[1]-r[0]>40];assert len(runs)==4
BARS=[tight(bay.crop((0,max(0,y0-8),bay.width,min(bay.height,y1+9)))) for y0,y1 in runs]
def nav(im,active,base=718):
    sprite(im,BARS[active],(0,base,375,60))
    for i,label in enumerate(('Ev','Koleksiyon','Jiggles','Başarılar')):
        cx=(i+.5)*93.75
        if i==active:sprite(im,BUBBLE,(cx-29,base-24,58,58))
        h=54.6 if i==active else 42;w=h*ICONS[i].width/ICONS[i].height
        y=base-23 if i==active else base-9;x=cx-w/2
        ico=ICONS[i].copy()
        if i!=active:ico.putalpha(ico.getchannel('A').point(lambda v:round(v*.83)))
        sprite(im,ico,(x,y,w,h));txt(label,cx,base+48,15,CREAM,outline=.45,maxw=92)
        if i==3:
            bx=x+BADGE_CENTER[0]*w;by=y+BADGE_CENTER[1]*h
            sprite(im,'Gameplay/Task15/booster-badge-v2.png',(bx-9.5,by-9.5,19,19));txt('1',bx,by+.3,15,CREAM,outline=.3)
def portrait(im,n,box,screen):
    # User explicitly asks for central portrait crop of the real square artwork.
    pic=read(f'Paintings/painting-{n:02}.jpg');w=round(pic.height*.75);left=(pic.width-w)//2
    pic=pic.crop((left,0,left+w,pic.height));sprite(im,pic,box)
    x,y,bw,bh=box;assert abs(bw/bh-.75)<.001,(screen,box)
    PORTRAITS.append(dict(screen=screen,level=n,sourceCrop=[left,0,left+w,pic.height],targetBox=list(box),ratio=.75))
def circle_asset(p):
    a=read(p);mask=Image.new('L',a.size);ImageDraw.Draw(mask).ellipse((0,0,a.width-1,a.height-1),fill=255)
    a.putalpha(Image.fromarray(np.minimum(np.asarray(mask),np.asarray(a.getchannel('A')))))
    return a
def close(x,y):txt('×',x,y-1,24,CREAM,outline=.45)
def nine_slice(a,sz,src_border=34,dst_border=26):
    w,h=a.size;tw,th=sz;out=Image.new('RGBA',sz)
    xs=[0,src_border,w-src_border,w];ys=[0,src_border,h-src_border,h]
    dx=[0,dst_border,tw-dst_border,tw];dy=[0,dst_border,th-dst_border,th]
    for j in range(3):
        for i in range(3):
            part=a.crop((xs[i],ys[j],xs[i+1],ys[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),Image.Resampling.LANCZOS)
            out.alpha_composite(part,(dx[i],dy[j]))
    return out

# 01: retain the approved C home pixels; restore background behind the old nav,
# then draw the new common nav. Replace the clipped baked name with real text.
im=begin(WORK.parent/'task19c/layers/01-ev-art.png')
clean=read(WORK.parent/'task19c/sources/home-clean.png').resize(SIZE,Image.Resampling.LANCZOS)
im.paste(clean.crop((0,2094,1125,2436)),(0,2094))
approved_home=read(WORK.parent/'task19c/layers/01-ev-art.png')
im.alpha_composite(approved_home.crop((239,1818,884,2121)),(239,1818))
sprite(im,'Menu/Task13/profile-box.png',(55,52,117,40))
old=read(WORK.parent/'task19c/layers/01-ev-art.png')
im.alpha_composite(old.crop((29,132,205,299)),(29,132))
name_size=fitted(NAME,118,72,15,94,color=INK)
txt('Bölümler',39,679,15,CREAM,outline=.5,maxw=78)
nav(im,0);save(im,'01-ev','Onaylı Ev yerleşimi; tam oyuncu adı ve yeni gömülü yuvalı çubuk.',0)

# 02: exact portrait cells and explicit fifth-row clip. Frames are reusable
# image_gen art, nine-sliced as real UI instead of relying on image-model counts.
FRAME=read(SRC/'koleksiyon-v2.png').crop((92,384,293,620))
FRAME=nine_slice(FRAME,(243,324)) # 81x108pt == 3:4
im=begin(SRC/'koleksiyon-clean.png');txt('Koleksiyon',146,132,28,CREAM,outline=.85,shadow=True,maxw=208);txt('11 / 40',312,135,18,maxw=80)
grid=Image.new('RGBA',SIZE)
for n in range(1,16):
    x=27+120*((n-1)%3);y=166+130*((n-1)//3)
    sprite(grid,FRAME,(x,y,81,108))
    if n<=11:portrait(grid,n,(x+6,y+8,69,92),'02-koleksiyon')
    elif n>12:sprite(grid,'Gameplay/Task14/booster-lock.png',(x+29,y+39,23,29))
    if n<=12:txt('Sıradaki' if n==12 else f'Bölüm {n}',x+40.5,y+117,15,CREAM,outline=.4,maxw=104)
grid.paste((0,0,0,0),(0,2154,1125,2436)) # y718 viewport edge
im.alpha_composite(grid);nav(im,1);save(im,'02-koleksiyon','3:4 dikey tablolar; dört tam sıra ve beşinci sıranın başlangıcı.',1)

# 02b: actual 7th painting/title, portrait crop and no obsolete stars.
themes={p['level']:p['theme'] for b in range(1,5) for p in json.loads((ART/f'Paintings/manifest-batch-{b}.json').read_text(encoding='utf-8-sig'))['paintings']}
im=begin(SRC/'detay.png');txt('Tablo detayı',185,182,26,CREAM,outline=.7,maxw=235);close(328,181)
portrait(im,7,(96,242,180,240),'02b-tablo-detay')
txt(themes[7],187,526,23,maxw=280);txt("Bölüm 7'de kazanıldı · 0:42",187,551,15,maxw=300)
txt('Tekrar oyna',187,606,23,CREAM,outline=.6,maxw=250)
nav(im,1);save(im,'02b-tablo-detay','Dikey gerçek tablo, kazanılma bilgisi ve yeniden oynama.',1)

# 03: new family shelf art, approved five-color counts and avatar/milestones.
im=begin(SRC/'jiggles.png');txt('Jiggles',146,110,30,CREAM,outline=.85,shadow=True,maxw=210);txt('5 renk',312,120,17,maxw=80)
txt('1.248 Jiggle kurtarıldı',187,151,20,CREAM,outline=.5,maxw=338)
for x,y,value in [(135,297,211),(246,297,190),(92,461,328),(188,461,277),(285,461,242)]:txt(str(value),x,y,16,maxw=49)
txt('Avatarım',187,483,16,maxw=300)
for x,value,label in [(69,'500','Çerçeve'),(189,'1.000','Rozet'),(309,'2.000','Portre')]:
    txt(value,x,657,16,INK,maxw=90);txt(label,x,680,15,INK,maxw=100)
nav(im,2);save(im,'03-jiggles','Beş renk aile, renk sayaçları, avatarlar ve üç görsel ödül durağı.',2)

# 04: preserve approved surfaces; descriptions are now single-line and safely
# above the tracks. Task19D explicitly permits smaller text on the long sentence.
im=begin(WORK.parent/'task19c/sources/basarilar-art-v2.png')
txt('Başarılar',146,134,30,CREAM,outline=.9,shadow=True,maxw=219);txt('5 / 16',317,133,18,maxw=75)
ach=[('İlk Tablo','İlk bölümü bitir','1/1',196),('Koleksiyoncu','10 tablo topla','10/10',299),('Bomba İmha Uzmanı','10 bombalı bölümü ilk denemede geç','7/10',404),('Hızlı Parmaklar','10 bölümü yarı sürede bitir','6/10',508),('Baron’a Kafa Tut','Tüm zor bölümleri geç','0/7',612)]
for title,desc,value,y in ach:
    txt(title,130,y+17,15.5,anchor='lm',maxw=166)
    fs=fitted(desc,130,y+42,15,201 if y==404 else 210 if y==508 else 166,anchor='lm')
    assert y+42+fs*.65<y+60
    txt(value,207,y+68.5,15,CREAM,outline=.4,maxw=90)
nav(im,3);save(im,'04-basarilar','Açıklamalar tek satır; metinler ilerleme çubuklarından ayrı.',3)

# 05: six current-game statistics, no star currency or old star statistic.
im=begin(SRC/'profil.png');txt('Profil',187,151,29,CREAM,outline=.6,maxw=250);close(330,151)
sprite(im,circle_asset('Gameplay/Task15/avatar-blue.png'),(141,195,94,94));fitted(NAME,185,324,23,236)
stats=[('12/40',['Bölüm'],'home'),('1.248',['Kurtarılan Jiggle'],'jiggle'),('18 dk',['Toplam süre'],'clock'),('%75',['İlk denemede'],'check'),('5/16',['Başarı'],'trophy'),('0:46',['Ortalama kalan','süre'],'clock')]
for i,(value,labels,icon) in enumerate(stats):
    x=45+(i%2)*146;y=354+(i//2)*73
    if icon=='jiggle':p='Gameplay/Task15/jiggle-counter-icon.png'
    elif icon=='clock':p='Gameplay/Task15/hud-icon-stopwatch-v2.png'
    elif icon=='check':p='Gameplay/Task15/hud-icon-piece-v2.png'
    else:p=ICONS[0 if icon=='home' else 3]
    sprite(im,p,(x+8,y+7,22,22));txt(value,x+87,y+19,21,maxw=95)
    if len(labels)==1:txt(labels[0],x+68,y+47,15,maxw=128)
    else:
        txt(labels[0],x+68,y+40,15,maxw=128);txt(labels[1],x+68,y+56,15,maxw=128)
txt('Avatarı değiştir',187,609,22,CREAM,outline=.5,maxw=250)
nav(im,0);save(im,'05-profil','Altı istatistik: bölüm, Jiggle, süre, ilk deneme, başarı ve ortalama kalan süre.',0)

# 06: D explicitly returns to switches/segmented language selection.
im=begin(SRC/'ayarlar.png');txt('Ayarlar',187,157,30,CREAM,outline=.7,maxw=250)
for label,y in [('Ses',253),('Müzik',333),('Titreşim',410)]:txt(label,138,y,20,anchor='lm',maxw=106)
txt('Dil',58,496,18,maxw=60)
txt('Türkçe',145,496,17,CREAM,outline=.35,maxw=87);txt('English',235,496,17,maxw=88)
txt('Gizlilik',113,574,19,CREAM,outline=.4,maxw=112);txt('Bize yaz',265,574,19,CREAM,outline=.4,maxw=112);txt('v0.5.1',187,605,12,'#586A70')
nav(im,0);save(im,'06-ayarlar','Ses/müzik açık, titreşim kapalı; Türkçe seçili, bağlantılar ve sürüm.',0)

# 07: source provides genuinely thick carpet/stone geometry, not a line. The
# visible window is levels9..20, with 10/20 portrait milestones and hard19.
im=begin(SRC/'bolumler-task20.png');txt('Bölümler',146,77,28,CREAM,outline=.8,maxw=210);txt('40 bölüm',311,79,17,maxw=90);txt('‹',43,77,31,CREAM,outline=.5)
nodes=[(9,116,622),(10,263,579),(11,119,536),(12,276,497),(13,126,443),(14,266,405),(15,132,356),(16,258,320),(17,140,270),(18,257,244),(19,131,199),(20,250,180)]
for n,x,y in nodes:
    txt(str(n),x,y+(15 if n>=13 else 1),17,CREAM if n!=12 else INK,outline=.4 if n!=12 else 0)
sprite(im,'Gameplay/Task14/booster-lock.png',(122,180,18,24))
sprite(im,'Gameplay/Task15/avatar-blue.png',(258,437,34,34))
baron=read('Characters/Baron-reference-v01.png').crop((430,610,707,872));mask=Image.new('L',baron.size);ImageDraw.Draw(mask).ellipse((0,0,baron.width,baron.height),fill=255);baron.putalpha(mask)
sprite(im,baron,(147,170,31,31))
portrait(im,10,(179,559,30,40),'07-bolumler')
sprite(im,SRC/'portrait-frame-task20.png',(173,551,42,56))
portrait(im,20,(173,126,30,40),'07-bolumler')
txt('20',187,185,15,CREAM,outline=.4)
nav(im,0);save(im,'07-bolumler','Hacimli halı/taş yolu; 12 güncel, 19 zor, 10/20 dikey tablo durakları.',0)

# 08: all four full-slot selections on the new substantial recessed rail.
im=begin(color=CREAM);txt('Alt sekme çubuğu',187.5,77,27,maxw=335);txt('Dört gömülü yuva · dört seçili durum',187.5,109,15,maxw=350)
for i,label in enumerate(('Ev seçili','Koleksiyon seçili','Jiggles seçili','Başarılar seçili')):
    base=165+i*132;txt(label,22,base-37,17,anchor='lm',maxw=320);nav(im,i,base)
txt('Seçili olmayan ikonlar',187.5,659,21,maxw=330)
for i,label in enumerate(('Ev','Koleksiyon','Jiggles','Başarılar')):
    cx=(i+.5)*93.75;h=42;w=h*ICONS[i].width/ICONS[i].height
    ico=ICONS[i].copy();ico.putalpha(ico.getchannel('A').point(lambda v:round(v*.83)));sprite(im,ico,(cx-w/2,690,w,h));txt(label,cx,751,15,maxw=92)
save(im,'08-sekme-cubugu','Seçili yuvanın tamamı yükselir ve parlar; ikonlar/kabarcıklar korunur.',None)

# Contact board and complete review page, nine phones side by side.
board=Image.new('RGB',(1125,2436),INK)
for i,(n,_) in enumerate(SCREENS):board.paste(Image.open(OUT/f'{n}.png').resize((375,812),Image.Resampling.LANCZOS),(i%3*375,i//3*812))
board.save(OUT/'kontak.png')
cards=''.join(f'<article><a href="{n}.png"><div class="phone"><img src="{n}.png" alt="{html.escape(d)}"></div></a><h2>{n}</h2><p>{d}</p></article>' for n,d in SCREENS)
(OUT/'inceleme.html').write_text('''<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task19D · Tam menü seti</title><style>*{box-sizing:border-box}body{margin:28px;background:#0B2A36;color:#F8EBD0;font:16px system-ui}h1{font-size:28px}a{color:#FBBA0E}.row{display:flex;gap:26px;overflow:auto;padding:22px 0 35px;align-items:flex-start}article{flex:0 0 297px}.phone{padding:7px;border:2px solid #1C7F8C;background:#06151c;border-radius:33px;box-shadow:0 12px 30px #0007}.phone img{display:block;width:277px;border-radius:24px}h2{font-size:18px}p{line-height:1.6}.contact{max-width:100%;width:900px;height:auto}</style><h1>Reverse Solver · Task19D</h1><p>9 ekran · 1125×2436 px @3x · Yeni yuvalı alt çubuk bütün ekranlarda aynı konumda.<br>Dikey tablolar, yıldızsız içerik, güncel profil/istatistikler. Görsele tıklayarak tam boy aç.</p><div class="row">'''+cards+'''</div><h2>Kontak</h2><a href="kontak.png"><img class="contact" src="kontak.png" alt="Dokuz ekranın kontak görünümü"></a><p><a href="../../../../docs/art/TASK19_PARCALAR.md">Task20 parça planı</a> · <a href="../../../../docs/art/task19d/prompts.json">image_gen üretim promptları</a></p></html>''',encoding='utf8')
(WORK/'portraits.json').write_text(json.dumps(PORTRAITS,indent=2),encoding='utf8')
hard={19,23,25,30,35,36,38}
collection_model=[dict(level=n,title=themes[n],source=f'Assets/Art/Paintings/painting-{n:02}.jpg',
    state='earned' if n<12 else 'next' if n==12 else 'locked',hard=n in hard,
    crop=[128,0,896,1024],cellRatio='3:4') for n in range(1,41)]
(WORK/'collection-model.json').write_text(json.dumps(collection_model,ensure_ascii=False,indent=2),encoding='utf8')
(WORK/'level-model.json').write_text(json.dumps([dict(level=n,state='complete' if n<12 else 'current' if n==12 else 'locked',
    hard=n in hard,milestone=n%10==0) for n in range(1,41)],indent=2),encoding='utf8')
(WORK/'sources.json').write_text(json.dumps(sorted(USED),ensure_ascii=False,indent=2),encoding='utf8')
report=dict(screens=CHECKS,navBaseY=2154,navBottomY=2334,normalIconPt=42,selectedIconPt=54.6,
    portraitSourceCrop=[128,0,896,1024],gridCellPt=[81,108],gridImagePt=[69,92],fourRowsComplete=True,fifthRowClipPt=718,
    name=NAME,homeNameFontPt=round(name_size,1),descriptionSingleLine=True)
(WORK/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf8')
from html.parser import HTMLParser
class Links(HTMLParser):
    def handle_starttag(self,tag,attrs):
        for k,v in attrs:
            if k in ('href','src'):assert (OUT/v).is_file(),v
Links().feed((OUT/'inceleme.html').read_text(encoding='utf8'))
assert len(SCREENS)==9
print('Rendered 9 screens + contact + review. Dimensions, safe text areas, portrait ratios, no-star text and HTML links passed.')
