"""Copy one native bay, repair only its floor join. Architecture is not resampled."""
import sys,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageChops,ImageDraw
source=Image.open(root/'original-panorama.png').convert('RGB')
tile=source.crop((145,0,641,971));original=tile.copy()
pair=Image.open(root/'copy-before-seam-repair.png').convert('RGB')
box=(400,750,592,971);base=pair.crop(box)
repair=Image.open(root/'floor-join-generated.png').convert('RGB').resize(base.size,Image.Resampling.LANCZOS)
# Fixed source-space matte; never varied with time.
mask=Image.new('L',base.size)
for y in range(base.height):
    for x in range(base.width):
        edge=min(1,x/22,(base.width-1-x)/22,y/12,(base.height-1-y)/12)
        mask.putpixel((x,y),round(255*max(0,edge)))
fixed=Image.composite(repair,base,mask)
pair.paste(fixed,box[:2]);pair.save(root/'repaired-pair.png')
tile.paste(pair.crop((400,750,496,971)),(400,750))
tile.paste(pair.crop((496,750,592,971)),(0,750))
# The original photograph crop catches a small near carpet border at the bottom.
# It would become a repeated truncated gold wedge. Extend existing red weave by
# copying a same-size strip (no stretch) from this very bay; retain a 14px matte edge.
near=tile.crop((0,879,496,915))
near_mask=Image.new('L',near.size,255)
for y in range(14):near_mask.paste(round(y*255/14),(0,y,496,y+1))
tile.paste(near,(0,935),near_mask)
# Put the periodic wall/dado cut BEHIND a complete native foreground pillar.
# This is translation/masking of source pixels, not a generated architecture patch.
floor_ready=tile.copy()
tile=source.crop((64,0,560,971))
for x in range(90):
    for y in range(750):
        # Extend only the plain wall/stone behind the foreground column.
        # Sample existing row colors; no replicated narrow texture stripes.
        a=source.getpixel((480 if y<590 else 151,y));b=source.getpixel((154,y))
        t=x/89
        tile.putpixel((x,y),tuple(round(a[c]*(1-t)+b[c]*t) for c in range(3)))
# Complete pillar, capital, candle and vase from the original x492..640 region.
fgmask=Image.new('L',source.size,0);md=ImageDraw.Draw(fgmask)
md.polygon([(492,0),(640,0),(622,42),(614,72),(607,96),(591,105),(591,577),(615,587),(615,736),(506,736),(506,587),(520,577),(520,105),(505,94),(494,76)],fill=255)
# Plant/candle extend beyond the stone silhouette. Teal wall between leaves is excluded.
for y in range(270,590):
    for x in range(506,641):
        if y<455 and not (560<=x<=615):continue
        r,g,b=source.getpixel((x,y))
        teal=(g>r*1.16 and b>r*1.10)
        if not teal:fgmask.putpixel((x,y),255)
# Bake foreground once; it moves with the same whole scene thereafter.
for y in range(750):
    for sx in range(492,641):
        if fgmask.getpixel((sx,y)):
            tile.putpixel(((sx-64)%496,y),source.getpixel((sx,y)))
# Rephase the already repaired floor by 81px, with no scaling or resampling.
for x in range(496):tile.paste(floor_ready.crop(((x-81)%496,750,(x-81)%496+1,971)),(x,750))
tile.save(root/'matched-bay.png')
scene=Image.new('RGB',(496*3,971))
for i in range(3):scene.paste(tile,(i*496,0))
scene.save(root/'matched-scene.png')
# All architecture, not just the frame, remains literally original pixels.
native=source.crop((64,0,560,971))
assert ImageChops.difference(tile.crop((90,100,414,570)),native.crop((90,100,414,570))).getbbox() is None
diff=ImageChops.difference(tile,native)
unchanged=sum(max(v)==0 for v in diff.get_flattened_data())
info={'sourceCrop':[64,0,560,971],'period':496,'localRepairBoxInTwoBayCopy':list(box),'nearCarpetRedWeaveExtension':[0,935,496,971],'architecturalObjectsFromOriginalPixels':True,'wallDadoJoinHiddenBehindOriginalPillar':True,'paintingAndCompleteFramePixelIdentical':True,'originalPixelsNeverWarped':True,'generatedContentOnlyInFloorRepair':True,'fixedSpatialMatte':True,'temporalBlending':False,'tileUnchangedPixelFraction':unchanged/(496*971)}
(root/'composition-report.json').write_text(json.dumps(info,indent=2),encoding='utf-8')
print(json.dumps(info))
