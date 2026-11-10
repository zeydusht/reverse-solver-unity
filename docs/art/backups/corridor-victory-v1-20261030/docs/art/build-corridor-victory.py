from pathlib import Path
import sys, json, hashlib, shutil
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image, ImageDraw
ART=ROOT/'Assets/Art/Menu/CorridorVictory';ART.mkdir(parents=True,exist_ok=True)
RES=ROOT/'Assets/_Game/Presentation/Resources/CorridorVictory';RES.mkdir(parents=True,exist_ok=True)
spec=json.loads((ROOT/'docs/art/corridor-victory-input.json').read_text(encoding='utf-8-sig'))
def copy_source(key, filename):
    source=ROOT/spec[key];target=ART/filename
    if source.resolve()!=target.resolve():shutil.copyfile(source,target)
    return target
original=ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png'
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
before=sha(original)
src=Image.open(original).convert('RGBA');W,H=src.size
assert (W,H)==(1024,1536)
plate=Image.open(copy_source('plate','clean-plate-source.png')).convert('RGBA').resize(src.size,Image.Resampling.LANCZOS)
def polygon(points):
    mask=Image.new('L',src.size);ImageDraw.Draw(mask).polygon(points,fill=255);return mask
frame=polygon([(288,269),(282,245),(303,228),(340,226),(353,250),(451,270),(488,235),(520,253),(530,278),(674,301),(700,294),(722,316),(717,365),(714,839),(727,874),(713,904),(683,908),(669,892),(549,903),(524,930),(485,920),(469,896),(337,890),(301,900),(277,884),(280,847),(289,809)])
layer=src.copy();layer.putalpha(frame);layer.save(ART/'frame.png',optimize=True)
# A true planar wall atlas: horizontal mouldings stay at constant world height.
# Only the unobstructed middle of the generated clean plate is used.
wall_atlas=Image.new('RGBA',(1024,1536))
for j in range(1536):
    wy=3.-(j+.5)/1536*4.5
    sy=max(0,min(H-1,round(700-1647*wy/5.32)))
    for i in range(1024):
        sx=round(180+(i+.5)/1024*540)
        wall_atlas.putpixel((i,j),plate.getpixel((sx,sy)))
for i in range(128):
    q=(128-i)/256
    for j in range(1536):
        a=wall_atlas.getpixel((i,j));b=wall_atlas.getpixel((1023-i,j))
        wall_atlas.putpixel((i,j),tuple(round(v*(1-q)+z*q) for v,z in zip(a,b)))
        wall_atlas.putpixel((1023-i,j),tuple(round(z*(1-q)+v*q) for v,z in zip(a,b)))
wall_atlas.save(ART/'wall.png',optimize=True)
pillar_source=copy_source('pillar','pillar-source.png')
shutil.copyfile(pillar_source,ART/'columns.png')
copy_source('arches','arches.png')
# Continuous top-down carpet/marble material, sampled on the shader's horizontal floor.
AW,AH=2048,1024;f=1647.;z0=5.32;k=.585;xmin=-1.4;period=3.42;fy=-1.5
floor=Image.open(copy_source('floor','floor-material-source.png')).convert('RGBA').resize((AW,AH),Image.Resampling.LANCZOS)
for i in range(128):
    q=(128-i)/128*.5
    for j in range(AH):
        a=floor.getpixel((i,j));b=floor.getpixel((AW-1-i,j))
        floor.putpixel((i,j),tuple(round(v*(1-q)+z*q) for v,z in zip(a,b)))
        floor.putpixel((AW-1-i,j),tuple(round(z*(1-q)+v*q) for v,z in zip(a,b)))
floor.save(ART/'floor.png',optimize=True)
for name in ['wall','floor','frame','columns','arches']:shutil.copyfile(ART/f'{name}.png',RES/f'{name}.png')
src.save(ART/'reference.png',optimize=True)
assert sha(original)==before
report=dict(source=str(original.relative_to(ROOT)),sourceSha256=before,sourceUnchanged=True,projection=dict(focalPixels=f,horizonY=700,z0=z0,wallSlope=k,period=period,xmin=xmin,floorY=fy),assets={n:dict(size=list(Image.open(ART/f'{n}.png').size),sha256=sha(ART/f'{n}.png')) for n in ['wall','floor','frame','columns','arches','reference']},prompts='docs/art/corridor-victory-prompts.json')
(ART/'manifest.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
print('Corridor wall/floor/frame/column layers exported; approved menu image unchanged.')
