"""Closed wall + original art relief, and one orthonormal world-space floor material."""
from pathlib import Path
import sys,math,json,hashlib,shutil
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image,ImageDraw,ImageFilter,ImageChops,ImageOps
ART=ROOT/'Assets/Art/Menu/CorridorVictoryV2';RES=ROOT/'Assets/_Game/Presentation/Resources/CorridorVictory'
SRC=Image.open(ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png').convert('RGBA')
W,H=SRC.size;TW,TH=1536,2048;K=.585;F=1647.;Z=5.32;XMIN=-1.4;PERIOD=3.42
def seal_floor(image):
    width,height=image.size
    if ImageChops.difference(image.crop((0,0,1,height)),image.crop((width-1,0,width,height))).getbbox() is None:return image
    for j in range(height):
        for i in range(16):
            a=image.getpixel((i,j));b=image.getpixel((width-1-i,j));q=(1-i/16)*.5
            image.putpixel((i,j),tuple(round(u*(1-q)+v*q) for u,v in zip(a,b)))
            image.putpixel((width-1-i,j),tuple(round(v*(1-q)+u*q) for u,v in zip(a,b)))
    return image
if '--seal-floor-only' in sys.argv:
    seal_floor(Image.open(ART/'floor.png').convert('RGB')).save(ART/'floor.png')
    shutil.copyfile(ART/'floor.png',RES/'floor.png')
    print('Floor repeat edge texels sealed; geometry and all other assets unchanged.');raise SystemExit(0)
def poly(points):
    out=Image.new('L',SRC.size);ImageDraw.Draw(out).polygon(points,fill=255);return out
left=poly([(0,0),(178,0),(178,120),(142,145),(138,674),(177,715),(177,998),(149,1020),(162,1165),(151,1247),(0,1285)])
body=poly([(775,250),(860,250),(856,950),(869,975),(869,1128),(780,1145),(778,1000)])
arch=poly([(686,0),(1024,0),(924,40),(879,111),(862,184),(868,220),(864,273),(774,281),(738,239),(738,215),(771,178),(747,93)])
candle=poly([(848,637),(866,681),(862,708),(862,742),(843,771),(812,770),(812,700),(844,700)])
columns_mask=ImageChops.lighter(left,ImageChops.lighter(body,ImageChops.lighter(arch,candle)))
for y in range(H):
    for x in range(W):
        if columns_mask.getpixel((x,y)):
            r,g,b,_=SRC.getpixel((x,y))
            if (g>r*1.15 and b>r*1.2 and abs(g-b)<max(g,b)*.3) or (x>680 and not(r>b*1.05 and r>g*.85)):columns_mask.putpixel((x,y),0)
columns_mask=columns_mask.filter(ImageFilter.GaussianBlur(.35))
for y in range(600,690):
    for x in range(180):columns_mask.putpixel((x,y),round(columns_mask.getpixel((x,y))*(690-y)/90))
ImageDraw.Draw(columns_mask).rectangle((0,690,180,H),fill=0)
frame_mask=poly([(333,280),(680,350),(680,850),(333,835)])
for y in range(210,938):
    for x in range(270,735):
        r,g,b,_=SRC.getpixel((x,y));teal=g>r*1.15 and b>r*1.2 and abs(g-b)<max(g,b)*.3
        if not teal:frame_mask.putpixel((x,y),255)
frame=SRC.copy();frame.putalpha(frame_mask)
columns=SRC.copy();columns.putalpha(columns_mask)
holes=ImageChops.lighter(columns_mask,frame_mask)
known=ImageChops.invert(holes)
# Only the original main wall is valid. No far facade/window is ever sampled.
ImageDraw.Draw(known).rectangle((775,0,W,H),fill=0)
ImageDraw.Draw(known).rectangle((0,0,180,H),fill=0)
columns_mask.save(ART/'columns-source-mask.png')
known.save(ART/'wall-source-validity.png')
if '--masks-only' in sys.argv:
    print('Saved source masks: the exterior facade is excluded from both wall and foreground.');raise SystemExit(0)
def warp_plane(image,offset):
    dx=PERIOD/TW;dy=5/TH;x=XMIN+dx*.5;y=3.5-dy*.5;z=Z-offset+K*x
    # Continue cropped roof members above the photographic top edge, instead
    # of turning missing pixels into a rectangular hole during the pullback.
    pad=512
    padded=Image.new(image.mode,(W,H+pad))
    padded.paste(image,(0,pad));padded.paste(image.crop((0,0,W,1)).resize((W,pad)),(0,0))
    coeff=((512*K+F)*dx/z,0,512+F*x/z,(700+pad)*K*dx/z,F*dy/z,700+pad-F*y/z,K*dx/z,0)
    return padded.transform((TW,TH),Image.Transform.PERSPECTIVE,coeff,Image.Resampling.BICUBIC)
wall_valid=warp_plane(known,0)
# The new section is an actual solid wall at z=5.32+.585*x, not an outdoor card.
plaster=SRC.crop((180,100,270,940)).convert('RGB').resize((TW,TH),Image.Resampling.BICUBIC)
wall=plaster.convert('RGBA')
panel=SRC.crop((180,965,740,1175)).convert('RGB')
panel_top=round((3.5+.92)/5*TH)
wall.paste(panel.resize((TW,TH-panel_top),Image.Resampling.BICUBIC),(0,panel_top))
wall_rgb=warp_plane(SRC,0)
wall=Image.composite(wall_rgb,wall,wall_valid);wall.putalpha(255)
frame_tile=warp_plane(frame,.08);col_tile=warp_plane(columns,.2)
def hidden_left(wx,wy):
    z=Z-.2+K*wx;px=round(512+F*wx/z);py=round(700-F*wy/z)
    if px>=0 or not 0<=py<H:return (0,0,0,0)
    sx=min(W-1,-px);rgb=SRC.getpixel((sx,py));a=columns_mask.getpixel((sx,py))
    # Only the cropped plant/vase and gold capital continue outside the photo.
    if py<690:a=0
    else:
        r,g,b,_=rgb
        green=g>r*1.05 and g>b*1.15
        blue=b>r*1.2 and b>g*1.2
        flower=min(r,g)>165 and b>100 and max(r,g,b)-min(r,g,b)<80
        gold=py>920 and r>g*.9 and g>b*1.3 and b<g*.6
        if not(green or blue or flower or gold):a=0
    return (*rgb[:3],a)
def over(b,a):
    af=a[3]/255;ab=b[3]/255;aa=af+ab*(1-af)
    if not aa:return (0,0,0,0)
    return tuple(round((a[c]*af+b[c]*ab*(1-af))/aa) for c in range(3))+(round(aa*255),)
for j in range(TH):
    wy=3.5-(j+.5)/TH*5
    for i in list(range(220))+list(range(TW-220,TW)):
        wx=XMIN+(i+.5)/TW*PERIOD
        ext=hidden_left(wx if i<220 else wx-PERIOD,wy)
        col_tile.putpixel((i,j),over(col_tile.getpixel((i,j)),ext))
# Wall texture joins are concealed by real relief; equal edge texels avoid wrap cracks.
for j in range(TH):
    for i in range(32):
        a=wall.getpixel((i,j));b=wall.getpixel((TW-1-i,j));q=(1-i/32)*.5
        wall.putpixel((i,j),tuple(round(u*(1-q)+v*q) for u,v in zip(a,b)))
        wall.putpixel((TW-1-i,j),tuple(round(v*(1-q)+u*q) for u,v in zip(a,b)))
wall.save(ART/'wall.png');frame_tile.save(ART/'frame.png')
# One atlas, two independent depth surfaces: columns left, complete plant right.
# Keep the generated transparent alpha; no source column/background travels with it.
plant=Image.open(ART/'plant-extracted.png').convert('RGBA')
bbox=plant.getchannel('A').point(lambda a:255 if a>192 else 0).getbbox()
bbox=(max(0,bbox[0]-20),max(0,bbox[1]-20),min(plant.width,bbox[2]+20),min(plant.height,bbox[3]+20))
plant=plant.crop(bbox)
packed=Image.new('RGBA',(2048,2048))
packed.paste(col_tile.resize((1024,2048),Image.Resampling.LANCZOS),(0,0))
scale=min(1024/plant.width,2048/plant.height)
plant=plant.resize((round(plant.width*scale),round(plant.height*scale)),Image.Resampling.LANCZOS)
packed.paste(plant,(1024+(1024-plant.width)//2,2048-plant.height))
packed.save(ART/'columns.png')
# Rectify original materials once. No row folding, mirrored perspective floor,
# source-footprint triangle or per-frame photograph/atlas switch remains.
def floor_patch(x0,x1,v0,v1,size):
    aw,ah=size;dx=(x1-x0)/aw;dv=(v1-v0)/ah;x=x0+dx*.5;v=v1-dv*.5;z=v+K*x
    coeff=((512*K+F)*dx/z,-512*dv/z,512+F*x/z,700*K*dx/z,-700*dv/z,700+F*1.5/z,K*dx/z,-dv/z)
    return SRC.convert('RGB').transform(size,Image.Transform.PERSPECTIVE,coeff,Image.Resampling.BICUBIC)
def periodic(im,margin=24):
    im=im.copy();ww,hh=im.size
    for i in range(margin):
        q=(1-i/margin)*.5
        for j in range(hh):
            a=im.getpixel((i,j));b=im.getpixel((ww-1-i,j))
            im.putpixel((i,j),tuple(round(u*(1-q)+v*q) for u,v in zip(a,b)))
            im.putpixel((ww-1-i,j),tuple(round(v*(1-q)+u*q) for u,v in zip(a,b)))
    return im
red=periodic(floor_patch(-.55,.59,3.55,4.6,(684,512)))
red=periodic(red.transpose(Image.Transpose.TRANSPOSE)).transpose(Image.Transpose.TRANSPOSE)
gold=periodic(floor_patch(-.4,.74,4.78,5.12,(684,160)))
marble=periodic(floor_patch(-.4,.4,5.08,5.20,(684,128)))
marble=periodic(marble.transpose(Image.Transpose.TRANSPOSE)).transpose(Image.Transpose.TRANSPOSE)
FW,FH=2048,2048;norm=math.sqrt(1+K*K);nmin=1.5/norm;nmax=5.5/norm
near=2.9/norm;far=4.95/norm
floor=Image.new('RGB',(FW,FH))
for j in range(FH):
    n=nmax-(j+.5)/FH*(nmax-nmin)
    border=min(abs(n-near),abs(n-far))
    for i in range(FW):
        # All material coordinates are affine in the same orthonormal S/N plane.
        u=(i/FW*3)%1
        if border<.105:
            d=(n-far) if abs(n-far)<abs(n-near) else (near-n)
            gy=max(0,min(159,round((.105-d)/.21*159)))
            c=gold.getpixel((min(683,int(u*684)),gy))
        elif near<n<far:
            c=red.getpixel((min(683,int(u*684)),int((n*.95%1)*512)))
        else:
            c=marble.getpixel((min(683,int(u*684)),int((n*2.4%1)*128)))
            # Fine, straight grout in world space; never image-shaped patches.
            s=i/FW*PERIOD*norm;tile_n=math.floor(n/.42);tile_s=PERIOD*norm/6;stagger=(tile_n%2)*tile_s/2
            edge=min((s+stagger)%tile_s,tile_s-(s+stagger)%tile_s,n%.42,.42-n%.42)
            if edge<.006:c=tuple(round(v*.80) for v in c)
        floor.putpixel((i,j),c)
seal_floor(floor).save(ART/'floor.png')
flags=Image.merge('RGB',(wall_valid.resize((2048,2048),Image.Resampling.BILINEAR),Image.new('L',(2048,2048)),Image.new('L',(2048,2048))))
flags.save(ART/'arches.png')
for name in ['wall','floor','frame','columns','arches']:shutil.copyfile(ART/f'{name}.png',RES/f'{name}.png')
for directory in [ART,RES]:shutil.copyfile(ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png',directory/'Source.png')
if not (RES/'Static.png').exists():
    shutil.copyfile(ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png',RES/'Static.png')
print('Closed wall, isolated relief and one affine world-space floor exported. No outdoor backdrop or frame-dependent floor sampling.')

