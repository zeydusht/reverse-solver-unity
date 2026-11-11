"""T46 deterministic export. Writes exclusively to the two task directories.
Run with python -B; Pillow and NumPy are the only dependencies.
Source sheets: built-in image_gen, exact T44 Jigo references.
"""
from pathlib import Path
import hashlib, json, math, uuid
import numpy as np
from PIL import Image, ImageDraw, ImageFont, ImageFilter

ROOT = Path(__file__).resolve().parents[3]
DOC = ROOT / 'docs/art/task46'
OUT = ROOT / 'Assets/Art/_yeni/T46'
SRC = ROOT / 'Assets/Art/_yeni/T44/Jigo'
R = Image.Resampling.LANCZOS
tips = {}

def save(im, path, tip=None):
    p = OUT / path
    p.parent.mkdir(parents=True, exist_ok=True)
    im.convert('RGBA').save(p)
    if tip is not None: tips[path] = tip

def cell(sheet, i):
    x,y = i%3*512, i//3*512
    return sheet.crop((x,y,x+512,y+512))

def padded(im):
    out = Image.new('RGBA',(352,256)); out.alpha_composite(im); return out

normal = Image.open(SRC/'normal/01.png').convert('RGBA')
happy = Image.open(SRC/'mutlu/01.png').convert('RGBA')
sheet = Image.open(DOC/'clock_source.png').convert('RGBA')
poses = [padded(normal)]
# Thimble apex stays near source (128, 8); all limbs remain baked together.
anchors = [(256,18),(236,18),(225,18),(202,20),(187,20),(174,20)]
rawtips = [(30,120),(85,240),(395,248),(443,241),(478,273),(499,284)]
tiplist = [(23,119)]
for i in range(1,6):
    im = cell(sheet,i).resize((256,256),R)
    dx,dy = round(128-anchors[i][0]/2),round(8-anchors[i][1]/2)
    p = Image.new('RGBA',(352,256)); p.alpha_composite(im,(dx,dy)); poses.append(p)
    tiplist.append((round(rawtips[i][0]/2+dx),round(rawtips[i][1]/2+dy)))
for i,p in enumerate(poses): save(p,f'clock/extend/{i+1:02}.png',tiplist[i])
tx,ty = tiplist[-1]
for i,s in enumerate([1,1.008,1,.992]):
    # Affine of the ENTIRE bust around contact, keeping the brush tip stationary.
    p = poses[-1].transform((352,256),Image.Transform.AFFINE,
        (1/s,0,tx*(1-1/s),0,1/s,ty*(1-1/s)),Image.Resampling.BICUBIC)
    save(p,f'clock/hold/{i+1:02}.png',(tx,ty))
for i,j in enumerate([4,3,2,1,0]): save(poses[j],f'clock/retract/{i+1:02}.png',tiplist[j])

# Redrawn expression inbetweens, with original endpoint frames.
sheet = Image.open(DOC/'jigo_refined_source.png').convert('RGBA')
transition = [normal]+[cell(sheet,i).resize((256,256),R) for i in [1,3,4]]+[happy]

def sample(a,x,y):
    h,w = a.shape[:2]
    valid = (x>=0)&(y>=0)&(x<=w-1)&(y<=h-1)
    x=np.clip(x,0,w-1); y=np.clip(y,0,h-1)
    ix=x.astype(int); iy=y.astype(int); jx=np.minimum(ix+1,w-1); jy=np.minimum(iy+1,h-1)
    fx=x-ix; fy=y-iy
    if a.ndim==3: fx=fx[...,None]; fy=fy[...,None]; valid=valid[...,None]
    return ((a[iy,ix]*(1-fx)+a[iy,jx]*fx)*(1-fy)+(a[jy,ix]*(1-fx)+a[jy,jx]*fx)*fy)*valid

def midpoint(a,b):
    # Premultiplied temporal sampling. No optical-flow deformation of pupils
    # or eyebrows; approved source keys and source timing are preserved.
    pa=np.asarray(a,dtype=np.float32)/255; pb=np.asarray(b,dtype=np.float32)/255
    pa[:,:,:3]*=pa[:,:,3:4]; pb[:,:,:3]*=pb[:,:,3:4]
    p=(pa+pb)*.5
    p[:,:,:3]/=np.maximum(p[:,:,3:4],1e-6)
    return Image.fromarray(np.uint8(np.clip(np.rint(p*255),0,255)))

# Brush height in the expression intermediates was redrawn by image_gen;
# no piecewise deformation of the shoulder or thimble is used.
for i,p in enumerate(transition): save(p,f'jigo/normal_to_happy/{i+1:02}.png')
for i,p in enumerate(reversed(transition)): save(p,f'jigo/happy_to_normal/{i+1:02}.png')

for mood,dest in [('normal','normal_loop'),('mutlu','happy_loop')]:
    frames=[Image.open(SRC/mood/f'{i:02}.png').convert('RGBA') for i in range(1,7)]
    for i,a in enumerate(frames):
        save(a,f'jigo/{dest}/{2*i+1:02}.png')
        save(midpoint(a,frames[(i+1)%6]),f'jigo/{dest}/{2*i+2:02}.png')

# Crisp UI effects rendered at 3x then reduced. No mascot anatomy is composited.
def star(d,c,r,color,angle=0):
    x,y=c
    pts=[]
    for k in range(8):
        a=angle+k*math.pi/4; rr=r if k%2==0 else r*.21
        pts.append((x+math.cos(a)*rr,y+math.sin(a)*rr))
    d.polygon(pts,fill=color)

for i,r in enumerate([10,23,33,29,21,10]):
    im=Image.new('RGBA',(288,288)); d=ImageDraw.Draw(im)
    star(d,(144,144),r*3,(255,173,36,210),math.pi/2)
    star(d,(144,144),r*2.3,(255,239,169,255),math.pi/2)
    star(d,(144,144),r*1.5,(255,255,246,255),math.pi/2)
    for j in range(5):
        a=j*math.tau/5+.2; rr=(15+i*5)*3
        star(d,(144+math.cos(a)*rr,144+math.sin(a)*rr),max(3,14-i*2),(255,211,89,255),a)
    save(im.resize((96,96),R),f'clock/spark/{i+1:02}.png')

W,H=339,290; S=3
ice=Image.new('RGBA',(W*S,H*S)); d=ImageDraw.Draw(ice)
d.rounded_rectangle((3,3,W*S-4,H*S-4),radius=36,fill=(97,191,233,62),outline=(192,239,255,225),width=9)
d.rounded_rectangle((14,14,W*S-15,H*S-15),radius=28,outline=(241,252,255,180),width=3)
rng=np.random.default_rng(46)
for side in range(4):
    count=16 if side%2==0 else 14
    for j in range(count):
        v=(j+.5)/count
        depth=int(rng.integers(9,24))*S; spread=int(rng.integers(5,11))*S
        if side==0: x=int(v*W*S); pts=[(x-spread,4),(x+spread,4),(x+spread//3,depth),(x,depth//2)]
        elif side==2: x=int(v*W*S); pts=[(x-spread,H*S-4),(x+spread,H*S-4),(x+spread//3,H*S-depth),(x,H*S-depth//2)]
        elif side==1: y=int(v*H*S); pts=[(W*S-4,y-spread),(W*S-4,y+spread),(W*S-depth,y+spread//3),(W*S-depth//2,y)]
        else: y=int(v*H*S); pts=[(4,y-spread),(4,y+spread),(depth,y+spread//3),(depth//2,y)]
        d.polygon(pts,fill=(204,244,255,205)); d.line(pts[:3],fill=(245,254,255,245),width=3)
for x,y in [(25,24),(311,36),(318,259),(32,265),(176,13)]:
    star(d,(x*S,y*S),7*S,(248,255,255,245),math.pi/2)
ice=ice.resize((W,H),R); save(ice,'clock/ice_overlay.png')

cracked=ice.copy(); d=ImageDraw.Draw(cracked)
hub=(169,137)
ends=[(0,0),(112,0),(239,0),(338,0),(338,98),(338,210),(338,289),(230,289),(112,289),(0,289),(0,196),(0,95)]
for x,y in ends:
    m=(round((x+hub[0])*.5+8),round((y+hub[1])*.5-6))
    d.line([hub,m,(x,y)],fill=(235,253,255,245),width=2)
    d.line([(m[0]+2,m[1]),(x+2,y)],fill=(63,153,202,210),width=1)
pieces=[]
for j,e in enumerate(ends):
    mask=Image.new('L',(W,H)); ImageDraw.Draw(mask).polygon([hub,e,ends[(j+1)%len(ends)]],fill=255)
    p=cracked.copy(); p.putalpha(Image.fromarray((np.asarray(p.getchannel('A'),dtype=np.uint16)*np.asarray(mask)//255).astype('uint8')))
    pieces.append(p)
for i in range(7):
    out=Image.new('RGBA',(339,434))
    if i==0: out.alpha_composite(ice)
    elif i==1: out.alpha_composite(cracked)
    elif i<6:
        t=(i-1)/4
        for j,p in enumerate(pieces):
            dx=round((ends[j][0]-169)*t*.25)
            dy=round(310*t*t+20*t+(j%3)*10*t)
            out.alpha_composite(p.rotate((j%2*2-1)*t*15,Image.Resampling.BICUBIC,center=hub),(dx,dy))
    save(out,f'clock/ice_break/{i+1:02}.png')

def bar(size,fill):
    w,h=size; im=Image.new('RGBA',(w*3,h*3)); d=ImageDraw.Draw(im)
    if not fill:
        d.rounded_rectangle((0,0,w*3-1,h*3-1),radius=27,fill='#234d73')
        d.rounded_rectangle((6,6,w*3-7,h*3-7),radius=22,fill='#f8e6bb')
        d.rounded_rectangle((12,12,w*3-13,h*3-13),radius=17,fill='#11344e')
        d.line((28,8,w*3-29,8),fill='#fff8dc',width=3)
    else:
        d.rounded_rectangle((0,0,w*3-1,h*3-1),radius=15,fill='#44aee0')
        d.rounded_rectangle((3,3,w*3-4,h*3-8),radius=12,fill='#89def4')
        d.rounded_rectangle((8,5,w*3-9,13),radius=4,fill='#dbfaff')
    return im.resize(size,R)
save(bar((300,42),False),'clock/freeze_bar_bg.png')
save(bar((288,30),True),'clock/freeze_bar_fill.png')

def font(n,bold=False):
    return ImageFont.truetype('C:/Windows/Fonts/'+('segoeuib.ttf' if bold else 'segoeui.ttf'),n)

def thumb(board,im,box):
    x,y,w,h=box; im=im.copy(); im.thumbnail((w,h),R)
    board.alpha_composite(im,(round(x+(w-im.width)/2),round(y+(h-im.height)/2)))

def preview():
    p=Image.new('RGBA',(390,844),'#142b42'); d=ImageDraw.Draw(p)
    def text(x,y,s,n=12,c='#f9ebca',bold=False): d.text((x,y),s,font=font(n,bold),fill=c)
    text(18,12,'T46 / SAAT + JIGO',21,bold=True)
    text(18,43,'Tek parça pozlar · 390 × 844',12,'#9dc4dc')
    # Actual HUD dimensions, at 390 px. The mascot contact is placed at panel left edge.
    d.rounded_rectangle((16,68,374,220),14,fill='#203e5a')
    panel=Image.open(ROOT/'Assets/Art/_yeni/HUD31/m1/counter-panel.png').convert('RGBA').resize((113,97),R)
    p.alpha_composite(panel,(266,91))
    d.text((305,98),'Süre',font=font(14,True),fill='#244969')
    d.text((293,125),'0:42',font=font(25,True),fill='#244969')
    med=Image.open(ROOT/'Assets/Art/_yeni/HUD31/m1/portrait-window.png').convert('RGBA').resize((127,104),R)
    p.alpha_composite(med,(128,91))
    # Whole-pose scale is .475, so the fixed tip lands on the panel's left edge.
    q=poses[-1].resize((167,122),R); pos=(round(266-tx*.475),round(142-ty*.475))
    p.alpha_composite(q,pos)
    p.alpha_composite(ice.resize((113,97),R),(266,91))
    spark=Image.open(OUT/'clock/spark/03.png').resize((40,40),R)
    p.alpha_composite(spark,(246,122))
    p.alpha_composite(Image.open(OUT/'clock/freeze_bar_bg.png').resize((100,14),R),(273,193))
    p.alpha_composite(Image.open(OUT/'clock/freeze_bar_fill.png').resize((96,10),R),(275,195))
    text(26,76,'TEMAS / 10 sn',10,'#ffce65',True)
    y=231
    for label,group,count in [('UZAT · 12 FPS','clock/extend',6),('TUT · 12 FPS','clock/hold',4),('GERİ AL · 12 FPS','clock/retract',5)]:
        text(18,y,label,11,'#ffce65',True)
        w=354/count
        for i in range(count):
            im=Image.open(OUT/group/f'{i+1:02}.png')
            thumb(p,im,(18+i*w,y+18,w-3,61))
            text(round(18+i*w+w/2-5),y+78,f'{i+1:02}',8,'#93b5cf')
        y+=96
    text(18,521,'BUZ KIRILIR · 18 FPS',11,'#ffce65',True)
    for i in range(7):
        thumb(p,Image.open(OUT/f'clock/ice_break/{i+1:02}.png'),(18+i*51,540,48,61))
        text(34+i*51,601,str(i+1),8,'#93b5cf')
    text(18,622,'NORMAL → MUTLU / 20 FPS',11,'#ffce65',True)
    for i,im in enumerate(transition): thumb(p,im,(18+i*71,642,67,64))
    text(18,713,'MUTLU → NORMAL / 20 FPS',11,'#ffce65',True)
    for i,im in enumerate(reversed(transition)): thumb(p,im,(18+i*71,733,67,64))
    text(18,811,'Döngüler: 6 → 12 kare · 20 FPS · RGBA',11,'#9dc4dc')
    p.convert('RGB').save(DOC/'onizleme.png')
preview()

# Animated review, not a runtime dependency. All sequences include their seam.
def review_gif(paths,name,duration,hold=0):
    frames=[]
    for path in paths:
        im=Image.open(OUT/path).convert('RGBA')
        bg=Image.new('RGBA',(352,280),'#142b42')
        bg.alpha_composite(im,((352-im.width)//2,(256-im.height)//2))
        frames.append(bg.convert('RGB'))
    if hold: frames=frames+[frames[-1]]*hold
    frames[0].save(DOC/name,save_all=True,append_images=frames[1:],duration=duration,loop=0,disposal=2)
review_gif([f'clock/extend/{i:02}.png' for i in range(1,7)]+[f'clock/hold/{i:02}.png' for _ in range(3) for i in range(1,5)]+[f'clock/retract/{i:02}.png' for i in range(1,6)],'clock_review.gif',83,5)
review_gif([f'jigo/normal_loop/{i:02}.png' for i in range(1,13)]+[f'jigo/normal_to_happy/{i:02}.png' for i in range(1,6)]+[f'jigo/happy_loop/{i:02}.png' for i in range(1,13)]+[f'jigo/happy_to_normal/{i:02}.png' for i in range(1,6)],'jigo_review.gif',50)

# Import settings only INSIDE T46; no T46.meta is written beside the allowed root.
for p in OUT.rglob('*.png'):
    rel=p.relative_to(OUT).as_posix()
    if rel.startswith('clock/extend') or rel.startswith('clock/hold') or rel.startswith('clock/retract'):
        pivot=(128/352,.5); border=(0,0,0,0)
    elif rel.startswith('clock/ice_break'):
        pivot=(.5,1-145/434); border=(0,0,0,0)
    else: pivot=(.5,.5); border=(0,0,0,0)
    if 'freeze_bar_bg' in rel: border=(14,14,14,14)
    if 'freeze_bar_fill' in rel: border=(10,10,10,10)
    guid=uuid.uuid5(uuid.NAMESPACE_URL,'ReverseSolver/T46/'+rel).hex
    p.with_suffix('.png.meta').write_text(f'''fileFormatVersion: 2
guid: {guid}
TextureImporter:
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 1
  textureSettings:
    filterMode: 1
    aniso: 1
    wrapU: 1
    wrapV: 1
  nPOTScale: 0
  maxTextureSize: 2048
  spriteMode: 1
  spriteMeshType: 1
  alignment: 9
  spritePivot: {{x: {pivot[0]:.9f}, y: {pivot[1]:.9f}}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: {border[0]}, y: {border[1]}, z: {border[2]}, w: {border[3]}}}
  alphaUsage: 1
  alphaIsTransparency: 1
  textureType: 8
  textureShape: 1
  textureCompression: 0
  platformSettings: []
  userData: T46
  assetBundleName:
  assetBundleVariant:
''',encoding='utf-8')

manifest=[]
for p in sorted(OUT.rglob('*.png')):
    im=Image.open(p); rel=p.relative_to(OUT).as_posix()
    manifest.append({'path':rel,'size':im.size,'mode':im.mode,'tip_px':tips.get(rel),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
assert len(manifest)==65
assert all(x['mode']=='RGBA' for x in manifest)
assert np.array_equal(np.array(Image.open(OUT/'clock/retract/05.png'))[:,:256],np.array(normal))
assert Image.open(OUT/'clock/retract/05.png').getchannel('A').crop((256,0,352,256)).getbbox() is None
for mood,source in [('normal_loop','normal'),('happy_loop','mutlu')]:
    for i in range(6):
        assert np.array_equal(np.array(Image.open(OUT/f'jigo/{mood}/{i*2+1:02}.png')),np.array(Image.open(SRC/source/f'{i+1:02}.png')))
assert Image.open(DOC/'onizleme.png').size==(390,844)
(DOC/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
print('Verified: 65 RGBA sprites, 12 original loop keys, exact return endpoint, 390x844 preview.')
