"""Original-art layers plus one fixed repair of disoccluded pixels. No frame generation."""
from pathlib import Path
import sys, math, json, hashlib, shutil
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image, ImageDraw, ImageFilter, ImageChops
ART=ROOT/'Assets/Art/Menu/CorridorVictoryV2';ART.mkdir(exist_ok=True)
RES=ROOT/'Assets/_Game/Presentation/Resources/CorridorVictory'
src=Image.open(ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png').convert('RGBA')
W,H=src.size;f=1647.;z0=5.32;k=.585;fy=-1.5
def poly(points):
    mask=Image.new('L',src.size);ImageDraw.Draw(mask).polygon(points,fill=255);return mask
left=poly([(0,0),(178,0),(178,120),(142,145),(138,674),(177,715),(177,998),(149,1020),(162,1165),(151,1247),(0,1285)])
right=poly([(686,0),(1024,0),(1024,1090),(868,1130),(777,1145),(775,280),(740,235),(740,219),(772,179),(753,102)])
arch=Image.new('L',src.size)
columns_mask=ImageChops.lighter(ImageChops.lighter(left,right),arch)
for y in range(H):
    for x in range(W):
        if not columns_mask.getpixel((x,y)):continue
        r,g,b,_=src.getpixel((x,y))
        if x<870 and g>r*1.15 and b>r*1.2 and abs(g-b)<max(g,b)*.3:columns_mask.putpixel((x,y),0)
frame_mask=poly([(333,280),(680,350),(680,850),(333,835)])
for y in range(210,938):
    for x in range(270,735):
        r,g,b,_=src.getpixel((x,y))
        teal=g>r*1.15 and b>r*1.2 and abs(g-b)<max(g,b)*.3
        if not teal:frame_mask.putpixel((x,y),255)
frame=src.copy();frame.putalpha(frame_mask);frame.save(ART/'frame.png')
columns=src.copy();columns.putalpha(columns_mask);columns.save(ART/'columns.png')
holes=ImageChops.lighter(columns_mask,frame_mask)
wall=src.copy()
for y in range(H):
    for x in range(W):
        if holes.getpixel((x,y)):
            sx=190+(x%75) if y<950 else 250+(x%400)
            wall.putpixel((x,y),(*src.getpixel((sx,y))[:3],0))
wall.save(ART/'wall-source.png')
Image.new('RGBA',(1,1)).save(ART/'arches.png')
# Unproject all layers into the same bay coordinates before making the joint.
# Unlike wrapping perspective photographs, every height now meets the same height.
TW,TH=1536,2048
def sample(image,offset,wx,wy):
    z=5.32-offset+.585*wx
    px=round(512+1647*wx/z);py=round(700-1647*wy/z)
    if 0<=px<W:return image.getpixel((px,max(0,min(H-1,py))))
    if offset>.5 and px<0:
        if 150<=py<=690:
            sx=795+min(14,(-px)//20)
            sy=round(700-1647*wy/5.18)
        else:sx=min(W-1,-px);sy=py
        rgb=src.getpixel((sx,max(0,min(H-1,sy))))
        alpha=image.getpixel((min(W-1,-px),max(0,min(H-1,py))))[3]
        foreground=(*rgb[:3],alpha)
        background=sample(image,offset,wx+3.42,wy)
        return over(background,foreground)
    if offset>.5 and px>=W:
        # Continue distant bays toward the existing horizon without reflecting
        # arches (reflection creates a different, pointed architectural shape).
        sx=float(px);sy=float(py)
        for repeat in range(8):
            if sx<W:break
            if sx>=1100:return image.getpixel((1023,700))
            sx=1100+(sx-1100)*2;sy=700+(sy-700)*2
        return image.getpixel((max(0,min(W-1,round(sx))),max(0,min(H-1,round(sy)))))
    return (0,0,0,0) if offset else (*image.getpixel((max(0,min(W-1,px)),max(0,min(H-1,py))))[:3],0)
def over(b,a):
    af=a[3]/255;ab=b[3]/255;alpha=af+ab*(1-af)
    if alpha<=0:return (0,0,0,0)
    return tuple(round((a[c]*af+b[c]*ab*(1-af))/alpha) for c in range(3))+(round(alpha*255),)
def unproject(image,offset):
    tile=Image.new('RGBA',(TW,TH))
    for j in range(TH):
        wy=3.5-(j+.5)/TH*5.
        for i in range(TW):
            wx=-1.4+(i+.5)/TW*3.42
            tile.putpixel((i,j),sample(image,offset,wx,wy))
    return tile
wall_tile=unproject(wall,0)
frame_tile=unproject(frame,.08)
column_tile=unproject(columns,.65)
# Close the disoccluded seam outside the approved central framing. Reuse the
# original cropped pillar side across the right edge, with one fixed texture
# transition rather than a time-dependent image crossfade.
for j in range(TH):
    wy=3.5-(j+.5)/TH*5.
    for i in range(TW-240,TW):
        wx=-1.4+(i+.5)/TW*3.42
        a=sample(columns,.65,wx-3.42,wy)
        b=column_tile.getpixel((i,j))
        column_tile.putpixel((i,j),over(b,a))
    for i in range(48):
        a=wall_tile.getpixel((i,j));b=wall_tile.getpixel((TW-1-i,j));q=(1-i/48)*.5
        wall_tile.putpixel((i,j),tuple(round(v*(1-q)+u*q) for v,u in zip(a,b)))
        wall_tile.putpixel((TW-1-i,j),tuple(round(u*(1-q)+v*q) for v,u in zip(a,b)))
# Bake the single repaired hidden join back onto the fixed plane. Original
# visible pixels still sample menu_bg directly in the shader. No frame synthesis.
repair=Image.open(ART/'join-repair-source.png').convert('RGB').resize((390,844),Image.Resampling.LANCZOS)
cover=(390/844)/(1024/1536)
for j in range(TH):
    wy=3.5-(j+.5)/TH*5.
    for i in list(range(300))+list(range(TW-300,TW)):
        wx=-1.4+(i+.5)/TW*3.42
        for bay in [-1,0]:
            vx=wx+3.42*bay+1.71
            vz=4.67+.585*wx+2.0007*bay+1.32035
            u=.5+1.6083984375*vx/vz
            v=.5442708333+1.072265625*wy/vz
            sx=(u-.5)/cover+.5;sy=1-v
            if .50<sx<.72 and 0<=sy<1:
                weight=min(1.,(sx-.50)/.025,(.72-sx)/.025)
                a=repair.getpixel((min(389,int(sx*390)),min(843,int(sy*844))))
                b=column_tile.getpixel((i,j))
                source_u=.5+1.6083984375*wx/(4.67+.585*wx)
                hidden=source_u<0 or source_u>1 or wx>1.6
                alpha=b[3]
                if hidden:
                    alpha=round(alpha+(255-alpha)*weight)
                    ground=max(0.,min(1.,(wy+1.35)/.02))
                    alpha=round(alpha*ground)
                color=tuple(round(b[c]*(1-weight)+a[c]*weight) for c in range(3)) if b[3] else a
                column_tile.putpixel((i,j),color+(alpha,))
wall_valid=wall_tile.getchannel('A').resize((2048,2048),Image.Resampling.BILINEAR)
wall_tile.putalpha(255)
wall_tile.save(ART/'wall.png');frame_tile.save(ART/'frame.png');column_tile.save(ART/'columns.png')
AW,AH=2048,1536
floor=Image.new('RGBA',(AW,AH))
def row_at(v):
    row={}
    for i in range(AW):
        x=-1.4+(i+.5)/AW*3.42;z=v+k*x
        if z<=.01:continue
        px=round(512+f*x/z);py=round(700-f*fy/z)
        if 0<=px<W and 0<=py<H:
            row[i]=src.getpixel((px,py))
    return row
for j in range(AH):
    v=(1-(j+.5)/AH)*z0
    photographed=row_at(v)
    sample_v=v if v>=3.15 else min(5.15,7.9-v)
    if v<2.75:sample_v=5.06+.08*(1-abs(((v*3)%2)-1))
    row=row_at(sample_v)
    if len(row)<128:row=row_at(4.1)
    lo=min(row);hi=max(row);n=hi-lo+1
    values=[row[i] for i in range(lo,hi+1)]
    for i in range(AW):
        t=(i-lo)%(2*n)
        if t>=n:t=2*n-1-t
        c=photographed.get(i,values[t])
        floor.putpixel((i,j),(*c[:3],255 if i in photographed else 0))
for i in range(384):
    q=(1-i/384)*.5
    for j in range(AH):
        a=floor.getpixel((i,j));b=floor.getpixel((AW-1-i,j))
        floor.putpixel((i,j),tuple(round(x*(1-q)+y*q) for x,y in zip(a,b)))
        floor.putpixel((AW-1-i,j),tuple(round(y*(1-q)+x*q) for x,y in zip(a,b)))
for i in range(AW):
    weight=min(1.,min(i,AW-1-i)/128)
    for j in range(AH):
        c=floor.getpixel((i,j));floor.putpixel((i,j),(*c[:3],round(c[3]*weight)))
floor_valid=floor.getchannel('A').resize((2048,2048),Image.Resampling.BILINEAR)
floor.putalpha(255);floor.save(ART/'floor.png')
Image.merge('RGB',(wall_valid,floor_valid,Image.new('L',(2048,2048)))).save(ART/'arches.png')
for name in ['wall','floor','frame','columns','arches']:shutil.copyfile(ART/f'{name}.png',RES/f'{name}.png')
source_path=ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png'
digest=lambda path:hashlib.sha256(path.read_bytes()).hexdigest()
protected=json.loads((ROOT/'docs/art/backups/corridor-victory-v1-20261030/protected-hashes.json').read_text(encoding='utf-8-sig'))
assert all(digest(Path(item['Path'])).upper()==item['Hash'] for item in protected),'Protected movement, behavior or original artwork changed'
manifest=dict(version=2,original=str(source_path.relative_to(ROOT)),originalSha256=digest(source_path),protectedFilesUnchanged=True,perFrameGeneration=False,endpointArtCrossfade=False,hiddenRepairSource='Assets/Art/Menu/CorridorVictoryV2/join-repair-source.png',repairPrompts='docs/art/corridor-victory-v2-prompts.json',repairCalibration=dict(progress=.5,camera=[-1.71,0,-1.32035],preview=[390,844],screenX=[.50,.72]),assets={name:dict(size=list(Image.open(ART/f'{name}.png').size),sha256=digest(ART/f'{name}.png')) for name in ['wall','floor','frame','columns','arches']})
(ART/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
print('V2 original-art layers + fixed hidden repair exported; camera, behavior and original hashes unchanged.')
