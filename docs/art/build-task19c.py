"""Task19C artifact composer. Writes five final PNGs plus working files in docs/art.

Surfaces/icons are image_gen artwork. This script assembles native art, inserts
real paintings and text, and preserves current-game UI as screenshot crops.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageOps, ImageFilter
import ast, json, hashlib, math
from collections import deque
import numpy as np

ROOT=Path(__file__).resolve().parents[2]; ART=ROOT/'Assets/Art'
OUT=ART/'Menu/Task19C'; WORK=ROOT/'docs/art/task19c'; SRC=WORK/'sources'; LAY=WORK/'layers'
for p in (OUT,SRC,LAY):p.mkdir(parents=True,exist_ok=True)
SIZE=(1125,2436);INK='#0B2A36';CREAM='#F8EBD0'
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
MARKS={};RECORDS=[];USED=set();UI_BOUNDS=[]
# Reuse only the typography function definitions, never the archived renderer's
# top-level code. Native Fredoka combining glyphs supply missing Turkish letters.
tree=ast.parse((ROOT/'docs/art/build-task19b.py').read_text(encoding='utf8'))
defs=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ('font','txt')]
exec(compile(ast.Module(body=defs,type_ignores=[]),'<shared Fredoka typography>','exec'))

def read(p):
    p=Path(p);p=p if p.is_absolute() else ART/p
    USED.add(str(p.relative_to(ROOT)).replace('\\','/'))
    return Image.open(p).convert('RGBA')
def sprite(im,p,box,fit=False):
    a=read(p) if not isinstance(p,Image.Image) else p
    x,y,w,h=box;sz=(round(w*3),round(h*3))
    a=ImageOps.fit(a,sz,method=Image.Resampling.LANCZOS) if fit else a.resize(sz,Image.Resampling.LANCZOS)
    im.alpha_composite(a,(round(x*3),round(y*3)))
def begin(source=None,color=INK):
    global TEXT,RECORDS
    TEXT=Image.new('RGBA',SIZE);RECORDS=[]
    return read(source).resize(SIZE,Image.Resampling.LANCZOS) if source else Image.new('RGBA',SIZE,color)
def save(im,name):
    assert all(r['fontSizePx']>=45 for r in RECORDS)
    im.save(LAY/f'{name}-art.png');TEXT.save(LAY/f'{name}-text.png')
    (LAY/f'{name}-text.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
    assert TEXT.crop((0,0,1125,132)).getbbox() is None
    assert TEXT.crop((0,2334,1125,2436)).getbbox() is None
    final=Image.alpha_composite(im,TEXT);final.convert('RGB').save(OUT/f'{name}.png')
    return final
def tight(a):
    bbox=a.getchannel('A').point(lambda x:255 if x>40 else 0).getbbox()
    return a.crop(bbox)

# These are sprite extractions from image_gen sheets, not reconstructed UI.
support=read(SRC/'nav-support.png')
scale=support.width/1254
def rect_scaled(box,s):return tuple(round(x*s) for x in box)
BAR=tight(support.crop(rect_scaled((0,345,1254,510),scale)))
BUBBLE=tight(support.crop(rect_scaled((409,624,846,1050),scale)))
atlas=read(SRC/'nav-atlas.png');ICONS=[];BADGE_CENTER=None
for i in range(4):
    a=tight(atlas.crop((round(i*atlas.width/4),round(atlas.height*1075/1254),round((i+1)*atlas.width/4),atlas.height)))
    ICONS.append(a)
    if i==3:
        hsv=np.asarray(a.convert('HSV'));alpha=np.asarray(a.getchannel('A'))
        m=((hsv[:,:,0]<=10)|(hsv[:,:,0]>=245))&(hsv[:,:,1]>120)&(hsv[:,:,2]>110)&(alpha>128)
        # Use the largest red component; brown shadows and alpha crumbs must
        # not displace the numbered badge away from the blank original badge.
        seen=np.zeros(m.shape,bool);best=[]
        for py,px in np.argwhere(m):
            if seen[py,px]:continue
            todo=deque([(int(px),int(py))]);seen[py,px]=True;part=[]
            while todo:
                qx,qy=todo.popleft();part.append((qx,qy))
                for nx,ny in ((qx-1,qy),(qx+1,qy),(qx,qy-1),(qx,qy+1)):
                    if 0<=ny<m.shape[0] and 0<=nx<m.shape[1] and m[ny,nx] and not seen[ny,nx]:
                        seen[ny,nx]=True;todo.append((nx,ny))
            if len(part)>len(best):best=part
        xs,ys=zip(*best);BADGE_CENTER=((min(xs)+max(xs))/2/a.width,(min(ys)+max(ys))/2/a.height)
def nav(im,active,base=723):
    sprite(im,BAR,(0,base,375,55))
    for i,label in enumerate(('Ev','Koleksiyon','Jiggles','Başarılar')):
        cx=(i+.5)*93.75
        if i==active:sprite(im,BUBBLE,(cx-29,base-24,58,58))
        h=54.6 if i==active else 42;w=h*ICONS[i].width/ICONS[i].height
        y=base-23 if i==active else base-9;x=cx-w/2
        a=ICONS[i].copy()
        if i!=active:a.putalpha(a.getchannel('A').point(lambda v:round(v*.83)))
        sprite(im,a,(x,y,w,h))
        txt(label,cx,base+43,15,CREAM,outline=.45,maxw=92)
        if i==3:
            bx=x+BADGE_CENTER[0]*w;by=y+BADGE_CENTER[1]*h
            sprite(im,'Gameplay/Task15/booster-badge-v2.png',(bx-9.5,by-9.5,19,19))
            txt('1',bx,by+.3,15,CREAM,outline=.3)

# Collection: source frame openings were measured after image generation.
im=begin(SRC/'koleksiyon-art-v2.png')
txt('Koleksiyon',146,132,28,CREAM,outline=.85,shadow=True,maxw=208)
txt('11 / 40',312,135,18,maxw=80)
cols=[(99,270),(343,515),(584,756)]
rows=[(408,527),(606,724),(802,920),(999,1117),(1196,1314)]
sx=375/853;sy=812/1844;CELLS=[]
for n in range(1,16):
    x0,x1=cols[(n-1)%3];y0,y1=rows[(n-1)//3]
    x=x0*sx;y=y0*sy;w=(x1-x0)*sx;h=(y1-y0)*sy
    CELLS.append([round(x*3),round(y*3),round((x+w)*3),round((y+h)*3)])
    if n<=11:sprite(im,f'Paintings/painting-{n:02}.jpg',(x,y,w,h),fit=True)
    elif n>=13:sprite(im,'Gameplay/Task14/booster-lock.png',(x+w/2-12,y+h/2-14,24,28))
    label='Sıradaki' if n==12 else f'Bölüm {n}'
    txt(label,x+w/2,(y1+42)*sy,15,CREAM,outline=.4,maxw=94)
nav(im,1);COLLECTION=save(im,'02-koleksiyon')

im=begin(SRC/'basarilar-art-v2.png')
txt('Başarılar',146,134,30,CREAM,outline=.9,shadow=True,maxw=219)
txt('5 / 16',317,133,18,maxw=75)
items=[('İlk Tablo',['İlk bölümü bitir'],'1/1',196),
       ('Koleksiyoncu',['10 tablo topla'],'10/10',299),
       ('Bomba İmha Uzmanı',['10 bombalı bölümü','ilk denemede geç'],'7/10',404),
       ('Hızlı Parmaklar',['10 bölümü sürenin','yarısı dolmadan bitir'],'6/10',508),
       ('Baron’a Kafa Tut',['Tüm zor bölümleri geç'],'0/7',612)]
for name,desc,value,y in items:
    txt(name,130,y+17,15.5,anchor='lm',maxw=166)
    if len(desc)==1:txt(desc[0],130,y+39,15,anchor='lm',maxw=166)
    else:
        txt(desc[0],130,y+35,15,anchor='lm',maxw=166)
        txt(desc[1],130,y+51,15,anchor='lm',maxw=166)
    txt(value,207,y+68.5,15,CREAM,outline=.4,maxw=90)
nav(im,3);ACHIEVEMENTS=save(im,'04-basarilar')

# Four selected states plus a standalone inactive set, at the same width/scale.
im=begin(color=CREAM)
txt('Alt sekme çubuğu',187.5,77,27,maxw=335)
txt('Dört seçili durum',187.5,109,15)
for i,label in enumerate(('Ev seçili','Koleksiyon seçili','Jiggles seçili','Başarılar seçili')):
    base=165+i*132
    txt(label,22,base-37,17,anchor='lm',maxw=320)
    nav(im,i,base)
txt('Seçili olmayan ikonlar',187.5,659,21,maxw=330)
for i,label in enumerate(('Ev','Koleksiyon','Jiggles','Başarılar')):
    cx=(i+.5)*93.75;h=42;w=h*ICONS[i].width/ICONS[i].height
    a=ICONS[i].copy();a.putalpha(a.getchannel('A').point(lambda v:round(v*.83)))
    sprite(im,a,(cx-w/2,690,w,h));txt(label,cx,751,15,maxw=92)
    if i==3:
        bx=cx-w/2+BADGE_CENTER[0]*w;by=690+BADGE_CENTER[1]*h
        sprite(im,'Gameplay/Task15/booster-badge-v2.png',(bx-9.5,by-9.5,19,19));txt('1',bx,by+.3,15,CREAM,outline=.3)
NAV=save(im,'08-sekme-cubugu')

# Current home is preserved as current-game pixels, not regenerated art. User
# approved safe-area header translation and the minimum CTA lift for navigation.
QA=ROOT/'docs/qa/2026-10-29-p19/tr_390x844_2_menu.png'
original=read(QA);im=begin(SRC/'home-clean.png')
factor=1125/original.width
# Freeze the entire existing hero painting and corridor at their exact positions.
patch=original.crop((0,126,original.width,1360))
mask=Image.new('L',patch.size,255);m=np.full((patch.height,patch.width),255,dtype=np.uint8)
for j in range(24):m[j,:]=round(j/23*255);m[-j-1,:]=round(j/23*255)
mask=Image.fromarray(m);patch.putalpha(mask)
im.alpha_composite(patch.resize((1125,round(patch.height*factor)),Image.Resampling.LANCZOS),(0,round(126*factor)))
dy=132-10*factor
def current_crop(name,box,shapes,target_y=None):
    a=original.crop(box);mask=Image.new('L',a.size)
    d=ImageDraw.Draw(mask)
    for kind,rect,r in shapes:
        loc=(rect[0]-box[0],rect[1]-box[1],rect[2]-box[0],rect[3]-box[1])
        if kind=='ellipse':d.ellipse(loc,fill=255)
        else:d.rounded_rectangle(loc,radius=r,fill=255)
    a.putalpha(mask.filter(ImageFilter.GaussianBlur(.5)))
    x=round(box[0]*factor);y=round(box[1]*factor+dy) if target_y is None else round(target_y*3)
    sz=(round(a.width*factor),round(a.height*factor))
    im.alpha_composite(a.resize(sz,Image.Resampling.LANCZOS),(x,y))
    UI_BOUNDS.append(dict(name=name,sourceBox=box,targetBox=[x,y,x+sz[0],y+sz[1]],shiftY=y-round(box[1]*factor)))
current_crop('profile',(20,10,341,126),[('round',(22,11,139,125),22),('round',(116,27,340,111),30)])
current_crop('jiggle-counter',(361,27,624,110),[('round',(364,28,438,109),19),('round',(410,40,624,101),28)])
current_crop('settings',(651,24,750,120),[('ellipse',(652,25,749,119),0)])
current_crop('level-button',(166,1422,613,1632),[('round',(168,1426,611,1630),35)],target_y=606)
sprite(im,'Menu/Task13/levels-button.png',(17,621,44,44));txt('Bölümler',39,679,15,CREAM,outline=.5,maxw=78)
nav(im,0);HOME=save(im,'01-ev')
(WORK/'home-layout.json').write_text(json.dumps(dict(userDecision='Preserve appearance; shift header into safe area and lift CTA only as needed',components=UI_BOUNDS,heroSourceFixed=[0,126,780,1360]),indent=2),encoding='utf8')

# Comparison: current QA at left, Task19B pair in middle, Task19C pair at right.
entries=[(QA,'GUNCEL OYUN'),(ART/'Menu/Task19B/02-koleksiyon.png','19B / Koleksiyon'),
         (ART/'Menu/Task19B/04-basarilar.png','19B / Kupa'),(OUT/'02-koleksiyon.png','19C / Koleksiyon'),(OUT/'04-basarilar.png','19C / Kupa')]
board=Image.new('RGB',(2100,1020),INK);d=ImageDraw.Draw(board)
for i,(p,label) in enumerate(entries):
    x=25+i*415;a=ImageOps.contain(read(p),(375,812),Image.Resampling.LANCZOS)
    d.text((x,24),label,font=font(8),fill=CREAM)
    board.paste(a,(x+(375-a.width)//2,85+(812-a.height)//2),a)
d.text((25,943),'Guncel menu                  Task19B: altin agirlikli                                   Task19C: ince vurgu, turkuaz kenar',font=font(7),fill=CREAM)
board.save(OUT/'pilot-karsilastirma.png')

# Reproducible checks and a material-color proxy, not a semantic gold detector.
def gold_proxy(p,exclude=()):
    a=read(p).convert('RGB').resize(SIZE);hsv=np.asarray(a.convert('HSV'))
    valid=np.ones((2436,1125),bool)
    for x0,y0,x1,y1 in exclude:valid[y0:y1,x0:x1]=False
    gold=(hsv[:,:,0]>=13)&(hsv[:,:,0]<=38)&(hsv[:,:,1]>=140)&(hsv[:,:,2]>=130)
    return round(float(gold[valid].mean())*100,2)
checks={}
for n in ('01-ev','02-koleksiyon','04-basarilar','08-sekme-cubugu'):
    p=OUT/f'{n}.png';assert Image.open(p).size==SIZE
    rec=json.loads((LAY/f'{n}-text.json').read_text(encoding='utf8'))
    assert all(r['fontSizePx']>=45 for r in rec)
    if n in ('02-koleksiyon','04-basarilar'):assert not any('yıldız' in r['text'].lower() or '⭐' in r['text'] for r in rec)
    checks[n]=dict(size=SIZE,sha256=hashlib.sha256(p.read_bytes()).hexdigest())
report=dict(outputs=checks,selectedIconScale=1.3,normalIconHeightPx=126,selectedIconHeightPx=163.8,
            safeTopPx=132,safeBottomPx=102,navBaseY=2169,navBottomY=2334,
            goldColorProxy=dict(method='PIL HSV hue13..38 / saturation>=140 / value>=130. Warm art/materials can also match; not semantic gold area.',
                                collection19B=gold_proxy(ART/'Menu/Task19B/02-koleksiyon.png',CELLS),collection19C=gold_proxy(OUT/'02-koleksiyon.png',CELLS),
                                achievements19B=gold_proxy(ART/'Menu/Task19B/04-basarilar.png'),achievements19C=gold_proxy(OUT/'04-basarilar.png')))
assert report['goldColorProxy']['collection19C']<10
assert report['goldColorProxy']['achievements19C']<10
(WORK/'validation.json').write_text(json.dumps(report,indent=2),encoding='utf8')
(WORK/'sources.json').write_text(json.dumps(sorted(USED),indent=2,ensure_ascii=False),encoding='utf8')
print('Saved 4 screens @1125x2436 + comparison. Text safe areas, 45px minimum, star removal verified.')
print(json.dumps(report['goldColorProxy']))
