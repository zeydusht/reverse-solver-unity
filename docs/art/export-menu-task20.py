"""Text-free Task20 production sprites. Task14/15-style alpha/state processing.

UI art is generated/existing raster art; only extraction, alpha cleanup, 9-slice
normalization, state derivation and repeat seam sealing happen here.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter,ImageChops
from collections import deque
import ast,json,hashlib,math,shutil
import numpy as np

ROOT=Path(__file__).resolve().parents[2];ART=ROOT/'Assets/Art';OUT=ART/'Menu/Task20'
WORK=ROOT/'docs/art/task20';SRC=WORK/'sources';PREV=OUT/'Preview'
for p in (OUT,WORK,SRC,PREV):p.mkdir(parents=True,exist_ok=True)
R=Image.Resampling.LANCZOS;INK='#0B2A36';CREAM='#F8EBD0';RECORDS=[];CACHE={}
def load(p):
    p=Path(p);return Image.open(p if p.is_absolute() else ART/p).convert('RGBA')
def clean(im,component=True):
    im=im.copy();a=im.getchannel('A').point(lambda v:v if v>=8 else 0)
    if component:
        binary=a.point(lambda v:255 if v>48 else 0);pts=np.argwhere(np.asarray(binary)==255)
        assert len(pts),'Empty source'
        cy,cx=np.array(binary.size)[::-1]*[.52,.5];py,px=pts[np.argmin((pts[:,0]-cy)**2+(pts[:,1]-cx)**2)]
        ImageDraw.floodfill(binary,(int(px),int(py)),128)
        keep=binary.point(lambda v:255 if v==128 else 0).filter(ImageFilter.MaxFilter(3))
        a=ImageChops.multiply(a,keep)
    im.putalpha(a);return im
def fit(im,sz,pad=3,stretch=False,component=True):
    im=clean(im,component);b=im.getchannel('A').point(lambda v:255 if v>40 else 0).getbbox();assert b
    im=im.crop((max(0,b[0]-2),max(0,b[1]-2),min(im.width,b[2]+2),min(im.height,b[3]+2)))
    target=(sz[0]-pad*2,sz[1]-pad*2)
    im=im.resize(target,R) if stretch else ImageOps.contain(im,target,R)
    out=Image.new('RGBA',sz);out.alpha_composite(im,((sz[0]-im.width)//2,(sz[1]-im.height)//2));return out
def pressed(im):
    a=np.asarray(im).astype(float);a[:,:,:3]=a[:,:,:3]*.94+np.array([11,42,54])*.06
    d=max(3,min(9,im.height//24));part=Image.fromarray(np.clip(a,0,255).astype('uint8')).resize((im.width,im.height-d),R)
    out=Image.new('RGBA',im.size);out.alpha_composite(part,(0,d));return out
def save(im,name,use,border=(0,0,0,0),pivot=(.5,.5),interactive=False,**extra):
    assert im.mode=='RGBA';p=OUT/(name+'.png')
    unchanged=False
    if p.exists():
        with Image.open(p) as old:unchanged=old.size==im.size and old.convert('RGBA').tobytes()==im.tobytes()
    if not unchanged:im.save(p,optimize=True)
    CACHE[name]=im
    rec=dict(file=p.name,use=use,width=im.width,height=im.height,sliceLTRB=list(border),
             pivot=list(pivot),scale=3,pixelsPerUnit=3,transparent=bool(im.getchannel('A').getextrema()[0]<255),
             isInteractive=interactive or extra.get('state')=='pressed',state='normal',
             sha256=hashlib.sha256(p.read_bytes()).hexdigest())
    rec.update(extra)
    RECORDS.append(rec)
    if interactive:
        d=max(3,min(9,im.height//24));details=dict(extra)
        details.update(state='pressed',normalFile=p.name,pressOffsetPx=[0,d],pressScaleY=(im.height-d)/im.height)
        save(pressed(im),name+'-pressed',use+'; basılı',border,pivot,**details)
    return im
def cell(sheet,box,reference=(800,800),component=True):
    sx=sheet.width/reference[0];sy=sheet.height/reference[1]
    return clean(sheet.crop(tuple(round(v*(sx if i%2==0 else sy)) for i,v in enumerate(box))),component)
def ellipse_from(source,cx,cy,r,sz):
    # Export a round icon from an existing opaque UI plate, with no surroundings.
    a=source.crop(tuple(round(v*3) for v in (cx-r,cy-r,cx+r,cy+r)))
    mask=Image.new('L',a.size);ImageDraw.Draw(mask).ellipse((2,2,a.width-3,a.height-3),fill=255)
    a.putalpha(mask.filter(ImageFilter.GaussianBlur(.55)));return fit(a,sz,3,component=False)
def punch_frame(a):
    a=a.copy();alpha=a.getchannel('A')
    # The approved D frame already uses this precise 3:4 image aperture.
    ImageDraw.Draw(alpha).rectangle((18,24,224,299),fill=0)
    a.putalpha(alpha);return a
def s9(a,sz,b):
    l,t,r,bot=b;w,h=a.size;tw,th=sz
    assert tw>=l+r and th>=t+bot
    xs=[0,l,w-r,w];ys=[0,t,h-bot,h];dx=[0,l,tw-r,tw];dy=[0,t,th-bot,th]
    out=Image.new('RGBA',sz)
    for j in range(3):
        for i in range(3):
            part=a.crop((xs[i],ys[j],xs[i+1],ys[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),R)
            out.alpha_composite(part,(dx[i],dy[j]))
    return out
def fit9(a,sz,b,pad=4):
    a=clean(a);a=a.crop(a.getchannel('A').point(lambda v:255 if v>40 else 0).getbbox())
    inner=(sz[0]-pad*2,sz[1]-pad*2);a=ImageOps.contain(a,inner,R)
    # Source corners are scaled uniformly; only center/edges extend to target.
    a=s9(a,inner,b);out=Image.new('RGBA',sz);out.alpha_composite(a,(pad,pad));return out

# Five opaque clean scenes, under the maximum long edge of 2048px.
for file,key in [('scene-home','bg-home'),('scene-collection','bg-gallery'),('scene-jiggles','bg-jiggles'),('scene-achievements','bg-achievements'),('scene-levels','bg-levels')]:
    a=load(SRC/(key+'.png'));a.thumbnail((2048,2048),R)
    save(a,file,'Tam temiz arka plan; UI/panel yok',referenceCanvasPx=[1125,2436],intendedRectPt=[375,812],category='background',pixelsPerUnit=1)

# Image-gen UI sheet contains genuinely blank opaque faces and transparent rings.
sheet=load(SRC/'ui-sheet.png')
defs=[('title-plaque',(0,90,235,230),(720,180),(108,54,108,54),False),
('achievement-tile',(235,90,420,230),(945,294),(48,48,48,48),False),
('stat-tile',(420,90,590,230),(411,195),(36,36,36,36),False),
('counter-pill',(590,90,800,230),(288,108),(48,36,48,36),False),
('button-secondary',(0,268,234,365),(735,177),(60,48,60,48),True),
('button-small',(234,268,412,365),(405,132),(48,36,48,36),True),
('name-band',(412,280,592,360),(810,150),(48,36,48,36),False),
('progress-track',(590,280,800,360),(570,60),(24,18,24,18),False),
('progress-fill',(0,465,234,530),(540,36),(18,12,18,12),False),
('avatar-ring',(402,390,597,605),(174,174),(0,0,0,0),True),
('avatar-ring-selected',(597,390,800,605),(174,174),(0,0,0,0),True),
('toggle-on-track',(0,635,224,747),(195,108),(36,30,36,30),True),
('toggle-off-track',(224,635,415,747),(195,108),(36,30,36,30),True),
('toggle-knob',(421,635,522,747),(84,84),(0,0,0,0),True),
('language-left-selected',(530,635,800,747),(570,150),(48,36,48,36),True)]
for name,box,sz,b,button in defs:
    a=cell(sheet,box);a=fit9(a,sz,b) if any(b) else fit(a,sz,4)
    save(a,name,name.replace('-',' '),b,interactive=button)
save(ImageOps.mirror(CACHE['language-left-selected']),'language-right-selected','Dil seçici; sağ hücre seçili, yazı ayrı',(48,36,48,36),interactive=True)
save(fit(load(SRC/'modal.png'),(990,1680),6,stretch=True),'modal-panel','Yazısız krem/turkuaz modal gövdesi',(84,84,84,84))

# Exact approved D portrait frame, alpha aperture is 207x276 = 3:4.
source=load(ROOT/'docs/art/task19d/sources/koleksiyon-v2.png').crop((92,384,293,620))
frame=s9(source,(243,324),(34,34,34,34))
alpha=Image.new('L',frame.size);ImageDraw.Draw(alpha).rounded_rectangle((2,2,240,321),radius=22,fill=255)
frame.putalpha(alpha.filter(ImageFilter.GaussianBlur(.4)));frame=punch_frame(frame)
save(frame,'portrait-frame','Koleksiyon; şeffaf 3:4 merkez',(26,26,26,26),interactive=True,innerRectPx=[18,24,207,276],cropRatio='3:4')
placeholder=Image.new('RGBA',(243,324))
navy=load(ROOT/'docs/art/task19d/sources/koleksiyon-v2.png').crop((120,425,263,589)).resize((207,276),R)
placeholder.alpha_composite(navy,(18,24))
save(placeholder,'portrait-placeholder','Kazanılmamış tablo için ayrı lacivert içerik; çerçeveye gömülü değildir',contentRectPx=[18,24,207,276],cropRatio='3:4')
frame.save(ROOT/'docs/art/task19d/sources/portrait-frame-task20.png')
save(s9(frame,(540,720),(26,26,26,26)),'portrait-frame-large','Büyük detay çerçevesi; şeffaf merkez',(26,26,26,26),interactive=True,cropRatio='3:4',innerRectPx=[18,24,504,672])
save(s9(frame,(126,168),(26,26,26,26)),'milestone-picture-frame','Bölüm10/20/30/40 tablo işareti',(26,26,26,26),interactive=True,cropRatio='3:4',innerRectPx=[18,24,90,120])
CACHE['milestone-picture-frame'].save(ROOT/'docs/art/task19d/sources/portrait-frame-task20.png')
hero=fit(load(SRC/'hero-frame.png'),(705,1134),4,stretch=True)
save(hero,'hero-frame','Ev büyük oyma çerçeve; içi/dışı alfa',interactive=False)
hole=hero.getchannel('A').point(lambda v:255 if v<24 else 0)
ImageDraw.floodfill(hole,(hole.width//2,hole.height//2),128)
mask=hole.point(lambda v:255 if v==128 else 0)
hm=Image.new('RGBA',hero.size,'white');hm.putalpha(mask)
save(hm,'hero-picture-mask','Ev dinamik resim maskesi; resim çerçevenin altında',category='mask')

# Existing approved text-free controls, with new filenames. Never overwrite inputs.
reuse=[('button-primary','Gameplay/Task15/btn-primary-v2.png',(645,303),(72,60,72,60),True),
('button-levels','Menu/Task13/levels-button.png',(132,132),(0,0,0,0),True),
('button-settings','Menu/Task13/settings-button.png',(147,147),(0,0,0,0),True),
('profile-name-box','Menu/Task13/profile-box.png',(351,120),(60,36,60,36),True),
('profile-avatar-frame','Menu/Task13/profile-frame.png',(174,174),(0,0,0,0),True),
('hud-jiggle-pill','Menu/Task13/counter-pill.png',(330,108),(48,30,48,30),False),
('jiggle-counter-icon','Gameplay/Task15/jiggle-counter-icon.png',(126,126),(0,0,0,0),False),
('notification-badge','Gameplay/Task15/booster-badge-v2.png',(57,57),(0,0,0,0),False),
('stat-icon-clock','Gameplay/Task15/hud-icon-stopwatch-v2.png',(66,66),(0,0,0,0),False),
('stat-icon-piece','Gameplay/Task15/hud-icon-piece-v2.png',(66,66),(0,0,0,0),False)]
for name,p,sz,b,but in reuse:save(fit(load(p),sz,3,stretch=True),name,name,b,interactive=but,source=p)

# Preserve approved nav icon/bubble pixels, except badge-free trophy extraction.
atlas=load(ROOT/'docs/art/task19c/sources/nav-atlas.png');icons=[]
for i in range(3):icons.append(clean(atlas.crop((round(i*atlas.width/4),round(atlas.height*1075/1254),round((i+1)*atlas.width/4),atlas.height))))
icons.append(clean(load(SRC/'trophy-clean.png')))
for i,n in enumerate(['home','collection','jiggles','trophy']):
    base=clean(icons[i]);base=base.crop(base.getchannel('A').point(lambda v:255 if v>40 else 0).getbbox())
    for state,h in [('normal',126),('selected',164)]:
        w=round(h*base.width/base.height);a=fit(base,(w,h),2,stretch=True)
        if state=='normal':a.putalpha(a.getchannel('A').point(lambda v:round(v*.83)))
        save(a,f'nav-icon-{n}-{state}',f'Alt sekme {n}; {state}',interactive=True)
support=load(ROOT/'docs/art/task19c/sources/nav-support.png');ratio=support.width/1254
bubble=clean(support.crop(tuple(round(x*ratio) for x in (409,624,846,1050))))
save(fit(bubble,(174,174),3),'nav-bubble','Seçili ikonun açık turkuaz kabarcığı',interactive=True)
rail_sheet=load(ROOT/'docs/art/task19d/sources/nav-bays.png')
# Four existing selected layouts -> a truly neutral base and separate slot assets.
bounds=[(27,200),(219,392),(411,585),(604,778)]
rails=[fit(rail_sheet.crop((0,y0,rail_sheet.width,y1)),(1125,180),0,stretch=True) for y0,y1 in bounds]
neutral=rails[0].copy();neutral.paste(rails[1].crop((0,0,282,180)),(0,0))
save(neutral,'nav-bar-base','Çubuk tabanı; tüm yuvalar koyu, tek üst altın şerit',(36,30,36,30))
for n,a in [('normal',rails[0].crop((287,26,556,168))),('selected',rails[0].crop((6,23,275,173)))]:
    mask=Image.new('L',a.size);ImageDraw.Draw(mask).rounded_rectangle((0,0,a.width-1,a.height-1),radius=20,fill=255)
    a.putalpha(ImageChops.multiply(a.getchannel('A'),mask))
    save(a,'nav-slot-'+n,'Sekme yuvası '+n,(24,24,24,24),interactive=True)

# Standalone family actors, shelf, portrait heads, and real milestone medallions.
family=load(SRC/'family-sheet.png')
for n,box in [('blue',(0,0,290,400)),('pink',(285,0,540,400)),('red',(535,0,800,400)),('green',(0,400,260,800)),('purple',(255,400,493,800))]:
    actor=load(SRC/'purple-clean.png') if n=='purple' else cell(family,box)
    save(fit(actor,(240,360),4),f'jiggle-{n}-standing','Raf üstünde ayrı Jiggle aktörü',pivot=(.5,0))
    save(fit(load(f'Gameplay/Task15/avatar-{n}.png'),(174,174),4),f'avatar-{n}','Avatar portresi; halka ayrı')
save(fit(load(SRC/'shelf-clean.png'),(1080,816),4,stretch=True),'jiggles-shelf','Boş iki katlı raf; aktör/sayaç yok')
save(fit(CACHE['counter-pill'],(144,60),2,stretch=True),'color-count-pill','Renk sayacı; yazısız',(24,18,24,18))

# Circle icons from source plates are icon-only crops, not tiled screenshot panels.
ach=load(ROOT/'docs/art/task19c/sources/basarilar-art-v2.png').resize((1125,2436),R)
for name,cy in [('first-picture',244),('collector',347),('bomb',452),('speed',556),('baron-hat',660)]:
    save(ellipse_from(ach,84,cy,37,(216,216)),'achievement-'+name,'Başarı madalyonu; '+name)
access=load(SRC/'accessories-sheet.png')
for name,box in [('museum',(0,50,270,365)),('hourglass',(270,50,535,365)),('flawless',(535,50,800,365)),('chain',(0,425,270,740))]:
    save(fit(cell(access,box),(216,216),3),'achievement-'+name,'Başarı madalyonu; '+name)
save(fit(cell(access,(270,385,570,760)),(126,144),3),'baron-head','Zor bölüm Baron portresi, çevresi alfa')
save(fit(cell(access,(570,450,800,730)),(45,45),2),'hard-corner-flag','Zor koleksiyon hücresi köşe işareti')
settings=load(ROOT/'docs/art/task19d/sources/ayarlar.png').resize((1125,2436),R)
for name,cy in [('sound',253),('music',332),('vibration',409)]:save(ellipse_from(settings,98,cy,34,(186,186)),'settings-icon-'+name,'Ayarlar '+name+' ikonu')
prof=load(ROOT/'docs/art/task19d/sources/profil.png').resize((1125,2436),R)
save(ellipse_from(prof,304,324,17,(72,72)),'icon-pencil','Adı düzenleme ikonu',interactive=True)
jig=load(ROOT/'docs/art/task19d/sources/jiggles.png').resize((1125,2436),R)
for n,cx in [('frame',69),('badge',189),('portrait',309)]:save(ellipse_from(jig,cx,620,31,(156,156)),'milestone-'+n,'Görsel kilometre taşı '+n)
save(s9(CACHE['progress-track'],(900,120),(24,18,24,18)),'milestone-rail','Kilometre taşı kalın ray',(24,18,24,18))
check=ach.crop((870,666,984,780));rgb=np.asarray(check).copy()
green=(rgb[:,:,1]>rgb[:,:,0]*1.05)&(rgb[:,:,1]>rgb[:,:,2]*1.15)&(rgb[:,:,1]>70)
core=Image.fromarray((green*255).astype('uint8'));near=np.asarray(core.filter(ImageFilter.MaxFilter(7)))>0
dark=rgb[:,:,:3].max(axis=2)<135
check.putalpha(Image.fromarray(((green|(near&dark))*255).astype('uint8')).filter(ImageFilter.GaussianBlur(.35)))
save(fit(check,(69,69),2),'icon-green-check','Tamamlandı yeşil tiki; çevresi tam alfa')

# Number-free physical level nodes and compositions of independently exported icons.
nodes=load(SRC/'nodes-sheet.png')
for n,box,sz in [('complete',(0,0,268,262),(150,144)),('current',(268,0,538,262),(180,174)),('locked-base',(538,0,800,262),(150,144)),('hard-base',(0,262,270,533),(150,144))]:
    save(fit(cell(nodes,box,(800,533)),sz,4),'node-'+n,'Bölüm düğümü '+n,interactive=True)
save(fit(cell(nodes,(270,262,538,533),(800,533),False),(225,225),2,component=False),'node-glow','Güncel düğüm parlama halkası')
save(fit(cell(nodes,(540,262,800,533),(800,533)),(66,78),2),'icon-lock','Büyük bölüm kilidi')
for n,base,lock,baron in [('locked','node-locked-base',True,False),('hard-open','node-hard-base',False,True),('hard-locked','node-hard-base',True,True)]:
    a=CACHE[base].copy()
    if lock:a.alpha_composite(ImageOps.contain(CACHE['icon-lock'],(45,54),R),(53,22))
    if baron:a.alpha_composite(ImageOps.contain(CACHE['baron-head'],(48,54),R),(98,1))
    save(a,'node-'+n,'Bölüm '+n+'; sayıyı oyun yazar',interactive=True)
disc=CACHE['node-complete'].crop((38,24,112,110)).resize((174,174),R)
mask=Image.new('L',(174,174));ImageDraw.Draw(mask).ellipse((4,4,169,169),fill=255);disc.putalpha(mask.filter(ImageFilter.GaussianBlur(.5)))
save(disc,'avatar-disc','Portre altı turkuaz hacimli zemin; halka ve portre ayrı')

# Native existing blank badge + typographic symbols used purely as UI icons.
def glyph_icon(ch,sz,fs):
    a=Image.new('RGBA',sz);d=ImageDraw.Draw(a)
    f=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'),fs)
    d.text((sz[0]/2,sz[1]/2),ch,font=f,anchor='mm',fill=CREAM,stroke_width=2,stroke_fill=INK);return a
close=fit(load('Gameplay/Task15/booster-badge-v2.png'),(84,84),2);close.alpha_composite(glyph_icon('×',(84,84),66))
save(close,'button-close','Kırmızı kapat; X ikon, metin değil',interactive=True)
save(glyph_icon('‹',(72,72),84),'button-back','Geri ok ikonu',interactive=True)

# Repeat seam is sealed in premultiplied-alpha space, exactly matching first/last rows.
path=fit(load(SRC/'path-repeat.png'),(768,1536),0,stretch=True,component=False)
a=np.asarray(path).astype(float)
def landmarks(row):
    xs=np.where(row[:,3]>80)[0]
    red=np.where((row[:,0]>row[:,1]*2.5)&(row[:,0]>row[:,2]*2.5)&(row[:,0]>40)&(row[:,3]>80))[0]
    assert len(xs) and len(red)
    return np.array([xs.min(),red.min(),red.max(),xs.max()],dtype=float)
# Match silhouette AND both carpet borders before blending color. Averaging
# unmatched edge positions alone produces a doubled/ghosted gold carpet edge.
target=(landmarks(a[0])+landmarks(a[-1]))*.5
native=a.copy();a[:,:,:3]*=a[:,:,3:4]/255
xx=np.arange(a.shape[1],dtype=float)
for i in range(96):
    weight=(1-i/96)**2
    for yy in (i,a.shape[0]-i-1):
        old=landmarks(native[yy]);new=old*(1-weight)+target*weight
        dest=np.r_[0,new,a.shape[1]-1];src=np.r_[0,old,a.shape[1]-1]
        dest,keep=np.unique(dest,return_index=True);src=src[keep]
        sample=np.interp(xx,dest,src)
        row=a[yy].copy()
        for c in range(4):a[yy,:,c]=np.interp(sample,xx,row[:,c])
edge=(a[0]+a[-1])*.5
for i in range(24):
    t=1-i/24;a[i]=a[i]*(1-t)+edge*t;a[-i-1]=a[-i-1]*(1-t)+edge*t
a[-1]=a[0];alpha=a[:,:,3:4];a[:,:,:3]=np.divide(a[:,:,:3]*255,alpha,out=np.zeros_like(a[:,:,:3]),where=alpha>0)
path=Image.fromarray(np.rint(np.clip(a,0,255)).astype('uint8'))
save(path,'levels-path-repeat','Dikey tekrar: üst/alt RGBA kenarları bire bir eşit',pivot=(.5,0),wrapMode='Repeat',repeatAxis='Y',seamMethod='silhouette/carpet landmark alignment + premultiplied-alpha color seal')
assert np.array_equal(np.asarray(path)[0],np.asarray(path)[-1])

# Every production PNG has exactly one manifest/HANDOFF row. Previews are separate.
(OUT/'manifest.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
(WORK/'export-count.json').write_text(json.dumps(dict(count=len(RECORDS)),indent=2),encoding='utf8')
print('Exported',len(RECORDS),'text-free production PNGs. Run render-task20-preview.py next.')
