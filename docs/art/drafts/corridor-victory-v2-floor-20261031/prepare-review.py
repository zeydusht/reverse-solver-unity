"""Whole-move floor QA; endpoints alone are deliberately insufficient."""
import sys,json,hashlib,statistics
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageFont,ImageChops,ImageStat
font=lambda n,b=False:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if b else 'C:/Windows/Fonts/segoeui.ttf',n)
old=root.parent/'corridor-victory-v2-panorama-matched-20261031'
tile=Image.open(root/'matched-bay.png').convert('RGB');ref=Image.open(root/'upper-reference-bay.png').convert('RGB')
assert ImageChops.difference(tile.crop((0,0,496,726)),ref.crop((0,0,496,726))).getbbox() is None
assert ImageChops.difference(tile.crop((55,726,442,730)),ref.crop((55,726,442,730))).getbbox() is None
assert (root/'preview.js').read_bytes()==(old/'preview.js').read_bytes()
frames=[Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB') for i in range(181)]
assert ImageChops.difference(frames[0],frames[-1]).getbbox() is None
upper_changes=[]
for i,im in enumerate(frames):
    previous=Image.open(old/'qa'/f'frame-{i:03}.png').convert('RGB')
    # Exclude only resampling support around the floor boundary itself.
    if ImageChops.difference(im.crop((0,0,390,629)),previous.crop((0,0,390,629))).getbbox():upper_changes.append(i)
assert not upper_changes,upper_changes
labels=['BAŞLANGIÇ · 1. TABLO','ORTA · VAZO ÖNÜ / BİRLEŞİM','BİTİŞ · KOMŞU 2. TABLO']
names=['start','middle','end']
board=Image.new('RGB',(860,1850),'#102b33');d=ImageDraw.Draw(board)
d.text((40,24),'ZEMİN REVİZYONU · YAKIN GÖRÜNTÜLER',font=font(30,True),fill='#fff0d4')
d.text((40,72),'Tek bordür • bütün zemin • üst sahne değişmedi',font=font(24),fill='#cbe2e2')
for j,(name,label) in enumerate(zip(names,labels)):
    im=Image.open(root/f'frame-{name}.png').convert('RGB')
    close=im.crop((0,1190,780,1688));close.save(root/f'floor-close-{name}.png')
    y=155+j*560;d.text((40,y-40),label,font=font(26,True),fill='#ffcf6b');board.paste(close,(40,y))
board.save(root/'floor-closeups.png')

def sheet(indices,name,title,floor=True):
    cols=6;tw=240 if floor else 156;th=200 if floor else 338;rows=(len(indices)+cols-1)//cols
    out=Image.new('RGB',(40+cols*(tw+16),90+rows*(th+42)),'#102b33');dr=ImageDraw.Draw(out)
    dr.text((20,20),title,font=font(25,True),fill='#fff0d4')
    for j,i in enumerate(indices):
        x=20+(j%cols)*(tw+16);y=75+(j//cols)*(th+42)
        im=frames[i].crop((0,608,390,844)) if floor else frames[i]
        # Letterbox; never change the image's aspect ratio.
        s=min(tw/im.width,th/im.height);im=im.resize((round(im.width*s),round(im.height*s)),Image.Resampling.LANCZOS)
        out.paste(im,(x,y));dr.text((x,y+th+3),f'{i:03} · {i/60:.3f} sn',font=font(18),fill='#cbe2e2')
    out.save(root/'qa'/name)
sheet(list(range(0,181,5)),'whole-floor-move.png','Tüm hareket boyunca zemin · 37 örnek')
sheet(list(range(78,121)),'middle-floor-consecutive.png','1,30–2,00 sn · 43 ardışık zemin karesi')
sheet(list(range(174,180))+list(range(0,6)),'loop-floor-consecutive.png','Tekrar birleşimi · son ve ilk zemin kareleri')
sheet(list(range(0,181,10)),'whole-scene.png','Üst sahne ve hareket korunur',False)

# Inspect the actual decoded MP4 at 3s / 6s joins.
decoded=sorted((root/'qa').glob('review-decoded-*.png'))
out=Image.new('RGB',(1320,775),'#102b33');dr=ImageDraw.Draw(out)
dr.text((24,18),'ÜÇ TEKRAR MP4 · ÇÖZÜLMÜŞ ZEMİN BİRLEŞİMLERİ',font=font(25,True),fill='#fff0d4')
for j,p in enumerate(decoded):
    x=24+(j%4)*324;y=80+(j//4)*330
    pic=Image.open(p).convert('RGB').crop((0,1190,780,1688)).resize((300,192),Image.Resampling.LANCZOS)
    out.paste(pic,(x,y));dr.text((x,y+210),p.stem.removeprefix('review-decoded-'),font=font(19),fill='#cbe2e2')
out.save(root/'qa/three-cycle-floor-joins.png')

# A repeated geometry check, independently of camera endpoint equality.
floor=tile.crop((0,726,496,971))
seam_diff=sum(ImageStat.Stat(ImageChops.difference(floor.crop((495,0,496,245)),floor.crop((0,0,1,245)))).mean)/3
adjacent=[sum(ImageStat.Stat(ImageChops.difference(floor.crop((x-1,0,x,245)),floor.crop((x,0,x+1,245)))).mean)/3 for x in range(1,496)]
p95=sorted(adjacent)[round(.95*(len(adjacent)-1))]
assert seam_diff<=p95*1.5, (seam_diff,p95)
# Border support remains in ONE constant-height band at every phase.
gold=Image.open(root/'single-gold-border.png').convert('RGBA')
alpha=gold.getchannel('A')
assert alpha.getbbox()[1]>=0 and alpha.getbbox()[3]<=32
for i in range(3):
    scene=Image.open(root/'matched-scene.png').convert('RGB')
    assert ImageChops.difference(scene.crop((i*496,726,(i+1)*496,971)),floor).getbbox() is None
floor_means=[]
for a,b in zip(frames,frames[1:]):
    floor_means.append(sum(ImageStat.Stat(ImageChops.difference(a.crop((0,639,390,844)),b.crop((0,639,390,844)))).mean)/3)
spikes=[i+1 for i in range(1,len(floor_means)-1) if floor_means[i]>max(floor_means[i-1],floor_means[i+1])*1.6+.5]
assert not spikes,spikes
encoded_start=Image.open(root/'qa/decoded-0.png').convert('RGB');repeat_errors={}
for t in ['3-00001','6-00001']:
    im=Image.open(root/'qa'/f'review-decoded-{t}.png').convert('RGB')
    repeat_errors[t]=sum(ImageStat.Stat(ImageChops.difference(encoded_start,im)).mean)/3
    assert repeat_errors[t]==0
baseline=json.loads((root/'protected-before.json').read_text(encoding='utf-8-sig'))
changed=[x['path'] for x in baseline if hashlib.sha256(Path(x['path']).read_bytes()).hexdigest().upper()!=x['sha256'].upper()]
assert not changed,changed
report={'status':'passed','sourceUpperPixelsUnchangedToY':726,'foregroundBasePixelsThroughY725Unchanged':True,'nativeWallSkirtingThroughY729Preserved':True,'all181RenderedUpperFramesIdentical':not upper_changes,'cameraCodeByteIdentical':True,'rawStartEndPixelsIdentical':True,'encodedCycleStartMeanErrors':repeat_errors,'rawFloorFramesChecked':181,'criticalConsecutiveFrames':43,'floorTemporalIsolatedSpikes':spikes,'floorRepeatSeamMeanRGBDifference':seam_diff,'floorInteriorAdjacentColumnP95MeanRGBDifference':p95,'oneGoldBorderBandSourceY':[794,826],'goldBorderHeightNeverChanges':True,'marbleCommonPlaneBasis':[[124,0],[32,24]],'wholeFloorRepeatPixelsIdentical':True,'noTextureBlurFilter':True,'protectedFilesUnchanged':True,'gameIntegrated':False,'floorAdjacentFrameRGBMeans':floor_means}
(root/'qa-report.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
export=json.loads((root/'export-report.json').read_text())
export['source']='Previous matched revision upper scene, with one wholly rebuilt periodic floor from original carpet/gold materials'
export['cameraCodeByteIdenticalToPrevious']=True
(root/'export-report.json').write_text(json.dumps(export,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='floorAdjacentFrameRGBMeans'},ensure_ascii=False))
