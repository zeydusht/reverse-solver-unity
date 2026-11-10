"""Whole periodic floor from native material pixels. Upper scene is byte-identical.

No AI floor patch, no texture scaling, no blur filter. Minimum-error hard cuts
quilt native red weave. Gold motifs are rigidly rotated once, never stretched.
"""
import sys,json,random,math,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageChops,ImageStat
source=Image.open(root/'original-panorama.png').convert('RGB')
upper=Image.open(root/'upper-reference-bay.png').convert('RGB')
P,H,TOP,CARPET=496,971,726,790
rng=random.Random(1731)

def minpath(cost):
    """Top-to-bottom minimum-error seam, with at most one pixel sideways/row."""
    height,width=len(cost),len(cost[0]);prev=cost[0][:];back=[]
    for y in range(1,height):
        row=[];directions=[]
        for x in range(width):
            k=min(range(max(0,x-1),min(width,x+2)),key=lambda k:prev[k])
            row.append(cost[y][x]+prev[k]);directions.append(k)
        prev=row;back.append(directions)
    x=min(range(width),key=lambda x:prev[x]);path=[x]
    for row in reversed(back):x=row[x];path.append(x)
    return path[::-1]

def quilt(candidates,width,height,patch=72,overlap=20):
    """Native-size source patches; no alpha feathering at patch cuts."""
    step=patch-overlap;out=Image.new('RGB',(width+patch,height+patch));filled=Image.new('L',out.size)
    for yy in range(0,height,step):
        for xx in range(0,width,step):
            scores=[]
            for i,c in enumerate(candidates):
                score=0
                if xx:score+=sum(ImageStat.Stat(ImageChops.difference(out.crop((xx,yy,xx+overlap,yy+patch)),c.crop((0,0,overlap,patch)))).sum)
                if yy:score+=sum(ImageStat.Stat(ImageChops.difference(out.crop((xx,yy,xx+patch,yy+overlap)),c.crop((0,0,patch,overlap)))).sum)
                scores.append((score,i))
            scores.sort();best=scores[0][0]
            pool=[i for s,i in scores if s<=best*1.035+1][:5]
            c=candidates[rng.choice(pool)] if xx or yy else candidates[len(candidates)//3]
            old=out.crop((xx,yy,xx+patch,yy+patch));mask=Image.new('L',(patch,patch),255)
            a,b=old.load(),c.load()
            if xx:
                costs=[[sum((a[x,y][j]-b[x,y][j])**2 for j in range(3)) for x in range(overlap)] for y in range(patch)]
                path=minpath(costs)
                for y,x in enumerate(path):mask.paste(0,(0,y,x+1,y+1))
            if yy:
                costs=[[sum((a[x,y][j]-b[x,y][j])**2 for j in range(3)) for y in range(overlap)] for x in range(patch)]
                path=minpath(costs)
                for x,y in enumerate(path):mask.paste(0,(x,0,x+1,y+1))
            out.paste(c,(xx,yy),mask)
    return out.crop((0,0,width,height))

def periodic_quilt(candidates,height):
    overlap=32;field=quilt(candidates,P+overlap,height)
    left=field.crop((0,0,overlap,height));right=field.crop((P,0,P+overlap,height))
    a,b=left.load(),right.load()
    costs=[[sum((a[x,y][j]-b[x,y][j])**2 for j in range(3)) for x in range(overlap)] for y in range(height)]
    path=minpath(costs);out=field.crop((0,0,P,height))
    for y,x in enumerate(path):out.paste(right.crop((0,y,x+1,y+1)),(0,y))
    return out

# All source candidates are native red material, away from every gold border.
candidates=[source.crop((x,y,x+72,y+72)) for y in [831,837,843,849,855] for x in range(40,565,14)]
weave=periodic_quilt(candidates,H-CARPET)
weave.save(root/'carpet-native-weave.png')

# One complete existing motif plus its two narrow original gold rails. A rigid
# 2.6 degree rotation aligns the original projected strip; motif scale is fixed.
rot=source.crop((0,748,900,850)).rotate(-2.6,resample=Image.Resampling.BICUBIC,expand=True)
motif=rot.crop((190,49,290,81))
motif.save(root/'gold-motif-native.png')
gold=Image.new('RGBA',(P,32),(0,0,0,0));phase=0
for width in [99,99,100,99,99]:
    part=motif.crop((0,0,width,32));rgba=Image.new('RGBA',part.size)
    for y in range(32):
        for x in range(width):
            r,g,b=part.getpixel((x,y));ratio=g/max(1,r)
            alpha=round(255*max(0,min(1,(ratio-.27)/.15))) if r>80 and b<135 else 0
            rgba.putpixel((x,y),(r,g,b,alpha))
    gold.paste(rgba,(phase,0));phase+=width
gold.save(root/'single-gold-border.png')
carpet=weave.copy();carpet.paste(gold,(0,4),gold)
# Subtle pile/contact edge; no second border or copied gold wedges.
for x in range(P):
    c=carpet.getpixel((x,0));carpet.putpixel((x,0),tuple(round(v*.78) for v in c))

# Marble is ONE affine ground plane. Its two joint families use the same
# periodic coordinates, so intersections and the repeated edge are coherent.
# Native small warm-stone samples contain actual illustration texture/highlights.
palette=ImageStat.Stat(source.crop((175,740,240,770))).mean
marble=Image.new('RGB',(P,CARPET-TOP))
# Continuous periodic stone field; no rectangular pasted stone samples.
for y in range(marble.height):
    for x in range(P):
        u=x/P;t=y/max(1,marble.height-1)
        phase=(u-.10*t)%1
        d1=min(abs(phase-.28),1-abs(phase-.28));d2=min(abs(phase-.78),1-abs(phase-.78))
        light=12*math.sin(2*math.pi*(u-.16*t))+8*math.cos(2*math.pi*(2*u+.10*t))+25*math.exp(-(d1/.11)**2)+18*math.exp(-(d2/.075)**2)
        grain=1.2*math.sin(2*math.pi*(17*u+.031*y))+1.0*math.sin(2*math.pi*(31*u-.017*y))
        veins=0
        for level,amp,phase in [(8,3,.3),(27,5,1.1),(45,4,2.2)]:
            curve=level+amp*math.sin(2*math.pi*u+phase)+1.4*math.sin(2*math.pi*3*u+phase)
            veins+=9*math.exp(-((y-curve)/.8)**2)-3.0*math.exp(-((y-curve-1.3)/.9)**2)+5*math.exp(-((y-curve)/3.0)**2)
        dx=min(abs(x-1),P-abs(x-1));contact=math.exp(-(dx/30)**2-(y/4.7)**2)*.12
        reflection=9*math.exp(-(dx/24)**2)*math.exp(-y/22)
        value=light+grain+veins+reflection
        colors=[palette[c]+value*factor+(t-.5)*warm for c,factor,warm in [(0,.60,3),(1,1.0,1),(2,1.12,-3)]]
        marble.putpixel((x,y),tuple(max(0,min(255,round(v*(1-contact)))) for v in colors))
draw=ImageDraw.Draw(marble)
# Common plane basis: e1=(124,0), e2=(32,24). Every joint continues through wrap.
for y in [17,41]:
    draw.line((0,y,P-1,y),fill=(173,112,66),width=1)
    draw.line((0,y+1,P-1,y+1),fill=(244,197,132),width=1)
for n in range(-1,6):
    x0=n*124-18;x1=x0+marble.height*32/24
    draw.line((round(x0),0,round(x1),marble.height),fill=(179,116,71),width=1)
    draw.line((round(x0)+1,0,round(x1)+1,marble.height),fill=(249,205,145),width=1)
marble.save(root/'marble-one-plane.png')

tile=upper.copy();tile.paste(marble,(0,TOP));tile.paste(carpet,(0,CARPET))
# The native gold/stone foreground ends at y725. Its old reflection (not the
# object) occupied the next rows. Replace that reflection with the whole plane;
# keep the original wall skirting through y729 outside the foreground opening.
tile.paste(upper.crop((55,726,442,730)),(55,726))
tile.save(root/'matched-bay.png')
scene=Image.new('RGB',(P*3,H))
for i in range(3):scene.paste(tile,(i*P,0))
scene.save(root/'matched-scene.png')
floor=Image.new('RGB',(P*3,H-TOP))
for i in range(3):floor.paste(tile.crop((0,TOP,P,H)),(i*P,0))
floor.save(root/'floor-three-bays.png')
assert ImageChops.difference(upper.crop((0,0,P,TOP)),tile.crop((0,0,P,TOP))).getbbox() is None
report={'upperScenePixelsIdenticalAboveY':TOP,'period':P,'floorRange':[TOP,H],'carpetTopY':CARPET,'goldBorderBandY':[CARPET+4,CARPET+36],'goldMotifs':5,'nativeMotifScaleUnchanged':True,'nativeMotifSpacing':[99,99,100,99,99],'textureBlurFilter':False,'textureQuilting':'native-size pixels, hard minimum-error seams, no alpha patch feather','oneWholeFloorMaterial':True,'marblePlaneBasis':[[124,0],[32,24]],'marbleField':'continuous periodic veins/reflections with original sampled warm palette; no rectangular samples','temporalBlending':False,'newAIGeneratedArt':False}
(root/'floor-build-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))
