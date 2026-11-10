"""Rebuild nine screens from Task20 exported sprites + real game art + text.

No Task19D screen/art PNG is pasted into a reconstruction. Only its text/number
layout JSON is read as a design specification. Every artwork layer is exported.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter
import ast,json,hashlib,math
import numpy as np

ROOT=Path(__file__).resolve().parents[2];ART=ROOT/'Assets/Art';OUT=ART/'Menu/Task20';PREV=OUT/'Preview';WORK=ROOT/'docs/art/task20'
SIZE=(1125,2436);INK='#0B2A36';CREAM='#F8EBD0';FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
MARKS={};RECORDS=[];LAYERS=[];ALL_LAYOUTS={};SPRITES={};ITEMS={x['file'][:-4]:x for x in json.loads((OUT/'manifest.json').read_text(encoding='utf8'))}
defs=ast.parse((ROOT/'docs/art/build-task19b.py').read_text(encoding='utf8'))
exec(compile(ast.Module(body=[x for x in defs.body if isinstance(x,ast.FunctionDef) and x.name in ('font','txt')],type_ignores=[]),'<Fredoka>','exec'))
def pic(n):
    if n not in SPRITES:SPRITES[n]=Image.open(OUT/(n+'.png')).convert('RGBA')
    return SPRITES[n]
def s9(a,sz,b):
    l,t,r,bot=b;w,h=a.size;tw,th=sz
    if not any(b):return a.resize(sz,Image.Resampling.LANCZOS)
    assert tw>=l+r and th>=t+bot,(sz,b)
    xs=[0,l,w-r,w];ys=[0,t,h-bot,h];dx=[0,l,tw-r,tw];dy=[0,t,th-bot,th];o=Image.new('RGBA',sz)
    for j in range(3):
        for i in range(3):
            p=a.crop((xs[i],ys[j],xs[i+1],ys[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),Image.Resampling.LANCZOS);o.alpha_composite(p,(dx[i],dy[j]))
    return o
def start(n,bg):
    global IM,TEXT,RECORDS,LAYERS,NAME
    NAME=n;TEXT=Image.new('RGBA',SIZE);RECORDS=[];LAYERS=[];IM=Image.new('RGBA',SIZE)
    add(bg,(0,0,375,812))
def add(n,box,fraction=None,clipY=None):
    x,y,w,h=box;sz=(round(w*3),round(h*3));a=s9(pic(n),sz,ITEMS[n]['sliceLTRB'])
    if fraction is not None:a=a.crop((0,0,max(1,round(a.width*fraction)),a.height))
    if clipY is not None:a=a.crop((0,0,a.width,max(0,min(a.height,round((clipY-y)*3)))))
    if a.height:IM.alpha_composite(a,(round(x*3),round(y*3)))
    LAYERS.append(dict(sprite=n+'.png',rectPt=list(box),fraction=fraction,clipY=clipY))
def art(n,box,mask=None,source=None):
    path=ART/f'Paintings/painting-{n:02}.jpg' if source is None else ART/source
    a=Image.open(path).convert('RGBA');cw=round(a.height*.75);l=(a.width-cw)//2;a=a.crop((l,0,l+cw,a.height))
    x,y,w,h=box;a=ImageOps.fit(a,(round(w*3),round(h*3)),Image.Resampling.LANCZOS)
    if mask:a.putalpha(pic(mask).getchannel('A').resize(a.size,Image.Resampling.LANCZOS))
    IM.alpha_composite(a,(round(x*3),round(y*3)));LAYERS.append(dict(gameArt=str(path.relative_to(ROOT)).replace('\\','/'),crop='center 3:4',rectPt=list(box),mask=mask))
def nav(active,base=718):
    add('nav-bar-base',(0,base,375,60))
    for i,label in enumerate(('Ev','Koleksiyon','Jiggles','Başarılar')):
        x=i*93.75;cx=x+46.875
        add('nav-slot-selected' if i==active else 'nav-slot-normal',(x+3,base+(1 if i==active else 7),87.75,56 if i==active else 49))
        if i==active:add('nav-bubble',(cx-29,base-24,58,58))
        n=f"nav-icon-{['home','collection','jiggles','trophy'][i]}-{'selected' if i==active else 'normal'}"
        h=54.6 if i==active else 42;w=h*pic(n).width/pic(n).height;y=base-23 if i==active else base-9
        add(n,(cx-w/2,y,w,h));txt(label,cx,base+48,15,CREAM,outline=.45,maxw=92)
        if i==3:
            bx=cx+w*.40;by=y+h*.13;add('notification-badge',(bx-9.5,by-9.5,19,19));txt('1',bx,by+.3,15,CREAM,outline=.3)
def label_spec():
    rows=json.loads((ROOT/f'docs/art/task19d/layers/{NAME}-text.json').read_text(encoding='utf8'))
    for r in rows:
        t=r['text'];x=r['x']/3;y=r['y']/3
        if (t in ('Ev','Koleksiyon','Jiggles','Başarılar') and y>180) or t=='1':continue
        if NAME=='07-bolumler' and t.isdigit():continue
        # Actors are separate from shelf: the new upper tier has a slightly
        # lower baseline than the flattened illustration; counts follow feet.
        if NAME=='03-jiggles' and t in ('211','190'):y+=21
        txt(t,x,y,r['fontSizePx']/3,r['color'],anchor=r['anchor'],outline=r.get('outlinePx',0)/3,shadow=r.get('shadow',False))
def done():
    label_spec();a=Image.alpha_composite(IM,TEXT);a.convert('RGB').save(PREV/(NAME+'.png'))
    ALL_LAYOUTS[NAME]=dict(layers=LAYERS,text=RECORDS)
    return a

start('01-ev','scene-home')
art(1,(62,114,235,378),'hero-picture-mask',source='Gameplay/Task15/painting-test-01.png');add('hero-frame',(62,114,235,378))
add('profile-name-box',(9.67,52,162.33,40));add('profile-avatar-frame',(9.67,44,58,56));add('avatar-disc',(13.5,48,49,48));add('avatar-blue',(13.5,48,49,48))
add('hud-jiggle-pill',(188,54,112,29));add('jiggle-counter-icon',(174,51,38,38));txt('0',253,72,23)
add('button-settings',(313,51,49,49));add('button-primary',(79.67,606,215,101));txt('Level 1',187,657,40,CREAM,outline=.7)
add('button-levels',(17,621,44,44));nav(0);done()

start('02-koleksiyon','scene-collection');add('title-plaque',(32,106,229,48));add('counter-pill',(269,116,89,35))
for n in range(1,16):
    x=27+120*((n-1)%3);y=166+130*((n-1)//3)
    if n<=11:art(n,(x+6,y+8,69,92))
    else:add('portrait-placeholder',(x,y,81,108),clipY=718)
    add('portrait-frame',(x,y,81,108),clipY=718)
nav(1);done()

start('02b-tablo-detay','scene-achievements');add('modal-panel',(23,145,329,515));add('title-plaque',(58,154,255,58));add('button-close',(314,166,28,28))
art(7,(86,228,204,272));add('portrait-frame-large',(80,220,216,288));add('button-secondary',(73,577,229,59));nav(1);done()

start('03-jiggles','scene-jiggles');add('title-plaque',(25,87,232,48));add('counter-pill',(267,103,90,40))
add('jiggles-shelf',(20,178,340,300))
for c,cx,feet,h in [('green',135,306,132),('purple',246,306,132),('blue',92,446,132),('pink',188,446,132),('red',285,446,132)]:
    add('jiggle-'+c+'-standing',(cx-44,feet-h,88,h))
for x,y in [(135,318),(246,318),(92,461),(188,461),(285,461)]:add('color-count-pill',(x-24,y-10,48,20))
for i,c in enumerate(('blue','pink','red','green','purple')):
    x=25+i*67;add('avatar-disc',(x,510,58,58));add('avatar-'+c,(x+2,512,54,54));add('avatar-ring-selected' if i==0 else 'avatar-ring',(x,510,58,58))
add('milestone-rail',(35,593,306,51))
for n,x in [('frame',69),('badge',189),('portrait',309)]:add('milestone-'+n,(x-26,594,52,52))
nav(2);done()

start('04-basarilar','scene-achievements');add('title-plaque',(24,103,250,59));add('counter-pill',(278,116,82,35))
for i,(n,y,frac) in enumerate([('first-picture',196,1),('collector',299,1),('bomb',404,.7),('speed',508,.6),('baron-hat',612,0)]):
    add('achievement-tile',(31,y,313,98));add('achievement-'+n,(47,y+11,74,74))
    add('progress-track',(128,y+58.5,158,20))
    if frac:add('progress-fill',(132,y+62,150,13),fraction=frac)
    if i<2:add('icon-green-check',(297.5,y+33.5,23,23))
    if i==4:add('icon-lock',(298,y+36,22,26))
nav(3);done()

start('05-profil','scene-achievements');add('modal-panel',(23,122,329,560));add('title-plaque',(24,116,328,65));add('button-close',(316,136,28,28))
add('avatar-disc',(138,194,100,100));add('avatar-blue',(141,197,94,94));add('avatar-ring-selected',(131,187,114,114))
add('name-band',(45,300,284,46));add('icon-pencil',(287,308,32,32))
icons=['nav-icon-home-normal','jiggle-counter-icon','stat-icon-clock','stat-icon-piece','nav-icon-trophy-normal','stat-icon-clock']
for i,n in enumerate(icons):
    x=45+(i%2)*146;y=354+(i//2)*73;add('stat-tile',(x,y,137,65));add(n,(x+8,y+7,22,22))
add('button-secondary',(64,580,247,59));nav(0);done()

start('06-ayarlar','scene-achievements');add('modal-panel',(23,170,329,460));add('title-plaque',(28,121,320,72));add('button-close',(312,124,39,39))
for n,y,on in [('sound',253,True),('music',333,True),('vibration',410,False)]:
    add('settings-icon-'+n,(64,y-34,68,68));add('toggle-on-track' if on else 'toggle-off-track',(245,y-18,65,36));add('toggle-knob',(279 if on else 248,y-14,28,28))
add('language-left-selected',(94,473,190,50));add('button-small',(53,550,126,45));add('button-small',(196,550,126,45));nav(0);done()

start('07-bolumler','scene-levels');add('levels-path-repeat',(25,120,325,650));add('title-plaque',(15,51,232,64));add('counter-pill',(263,61,96,36))
node_positions={}
for n,cx,y in [(9,116,622),(10,263,579),(11,119,536),(12,276,497),(13,126,443),(14,266,405),(15,132,356),(16,258,320),(17,140,270),(18,257,244),(19,131,199),(20,250,180)]:
    node_positions[n]=(cx,y)
    state='complete' if n<12 else 'current' if n==12 else 'hard-locked' if n==19 else 'locked'
    w,h=(60,58) if n==12 else (50,48)
    if n==12:add('node-glow',(cx-37.5,y-37.5,75,75))
    add('node-'+state,(cx-w/2,y-h/2,w,h));txt(str(n),cx,y+(15 if n>12 else 1),17,CREAM if n!=12 else INK,outline=.4 if n!=12 else 0)
    if n==12:add('avatar-blue',(cx-17,y-60,34,34))
for n in (10,20):
    cx,y=node_positions[n];x=cx-83 if cx>187 else cx+33;top=y-28
    art(n,(x+6,top+8,30,40));add('milestone-picture-frame',(x,top,42,56))
nav(0);done()

start('08-sekme-cubugu','scene-home')
# This is a UI contact/reference board, not a game background; plain cream is intentional.
IM=Image.new('RGBA',SIZE,CREAM);LAYERS=[dict(referenceBoardColor=CREAM)]
for i in range(4):nav(i,165+i*132)
for i,n in enumerate(('home','collection','jiggles','trophy')):
    p='nav-icon-'+n+'-normal';h=42;w=h*pic(p).width/pic(p).height;cx=(i+.5)*93.75;add(p,(cx-w/2,690,w,h));txt(['Ev','Koleksiyon','Jiggles','Başarılar'][i],cx,751,15,maxw=92)
done()

names=list(ALL_LAYOUTS);board=Image.new('RGB',SIZE,INK)
for i,n in enumerate(names):board.paste(Image.open(PREV/(n+'.png')).resize((375,812),Image.Resampling.LANCZOS),(i%3*375,i//3*812))
board.save(OUT/'preview-kontak.png')
(WORK/'preview-layout.json').write_text(json.dumps(ALL_LAYOUTS,ensure_ascii=False,indent=2),encoding='utf8')
diff={}
for n in names:
    a=np.asarray(Image.open(PREV/(n+'.png')).convert('RGB'),dtype=float);b=np.asarray(Image.open(ART/'Menu/Task19D'/(n+'.png')).convert('RGB'),dtype=float)
    diff[n]=dict(meanAbsolutePixelDifference=round(float(np.abs(a-b).mean()),3),reason='Temiz sahne + bağımsız 9-slice katmanlar; piksel özdeşliği beklenmez.')
(WORK/'preview-differences.json').write_text(json.dumps(diff,ensure_ascii=False,indent=2),encoding='utf8')
print('Rebuilt',len(names),'screens strictly from manifest sprites, real game art and dynamic text.')
