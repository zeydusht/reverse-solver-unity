from pathlib import Path
import sys,json,hashlib,re,math
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image,ImageDraw,ImageFont,ImageChops,ImageStat
ART=ROOT/'Assets/Art/Menu/CorridorVictoryV2';QA=ROOT/'docs/art/corridor-victory/qa';EXPORT=ROOT/'docs/art/corridor-victory/export'
BACKUP=ROOT/'docs/art/backups/corridor-victory-v2-before-continuity-20261030'
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',17)
tiny=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',13)
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
protected=json.loads((BACKUP/'protected-hashes.json').read_text(encoding='utf-8-sig'))
unchanged=[]
for item in protected:
    p=Path(item['Path'])
    if p.name in ['CorridorVictoryQueue.cs','GameRoot.cs','menu_bg.png']:
        assert sha(p).upper()==item['Hash'],str(p)+' changed unexpectedly'
        unchanged.append(str(p.relative_to(ROOT)))
clock=ROOT/'Assets/_Game/Presentation/Ui/CorridorVictory.cs'
old=(BACKUP/'Assets/_Game/Presentation/Ui/CorridorVictory.cs').read_text(encoding='utf-8-sig')
new=clock.read_text(encoding='utf-8-sig')
camera=lambda s:re.search(r'public static Vector3 CameraAt\(float progress\)\s*\{.*?\n        \}',s,re.S).group()
assert camera(old)==camera(new),'Geometric camera trajectory changed'
assert 'public const float Duration = 3f;' in new
original=ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png'
assert sha(original)==sha(ART/'Source.png')
browser=json.loads((QA/'report.json').read_text())
assert browser['endpointPixelsIdentical'] and browser['idleStops'] and browser['completions']==1
assert browser['entryDifference']['meanRgbError']<1
assert browser['exitDifference']['meanRgbError']<1
shader=(ROOT/'Assets/_Game/Presentation/Resources/CorridorVictory/CorridorProjection.cginc').read_text(encoding='utf-8-sig')
assert 'floorPhoto' not in shader and 'floorKnown' not in shader,'Perspective photograph fallback returned to floor'
assert 'SideDistance' in shader and 'PillarBound' in shader
wall_source=Image.open(ART/'wall-source-validity.png')
column_source=Image.open(ART/'columns-source-mask.png')
assert wall_source.crop((775,0,1024,1536)).getbbox() is None
# Leave the four-pixel antialias fringe of the near pillar's x=869 base moulding.
# Everything beyond that silhouette (arches/openings/far facade) must be absent.
assert column_source.crop((874,300,1024,1090)).getbbox() is None,'Outdoor source facade leaked into the foreground'
floor_image=Image.open(ART/'floor.png').convert('RGB')
seam=ImageChops.difference(floor_image.crop((0,0,1,floor_image.height)),floor_image.crop((floor_image.width-1,0,floor_image.width,floor_image.height)))
seam_error=max(ImageStat.Stat(seam).mean)
assert seam_error<1,'Floor repeat boundary is not continuous'
errors=[];previous=None
for n in range(181):
    im=Image.open(QA/f'temporal/frame-{n:03d}.png').convert('RGB')
    if previous is not None:errors.append(sum(ImageStat.Stat(ImageChops.difference(im,previous)).mean)/3)
    previous=im
spikes=[]
for i in range(2,len(errors)-2):
    near=sorted(errors[i-2:i]+errors[i+1:i+3]);median=(near[1]+near[2])/2
    if errors[i]>3*median+.5:spikes.append(i+1)
assert not spikes,'Isolated temporal spikes: '+str(spikes)
# Whole move, at 0.05-second intervals, plus all consecutive critical-middle frames.
sheet=Image.new('RGB',(1170,1890),'#0b2a36');draw=ImageDraw.Draw(sheet)
for k,n in enumerate(range(0,181,3)):
    x=k%10*117;y=k//10*270
    sheet.paste(Image.open(QA/f'temporal/frame-{n:03d}.png').convert('RGB'),(x,y+17))
    draw.text((x+4,y+1),f'{n/60:.2f} s',font=tiny,fill='#f8ebd0')
sheet.save(QA/'continuity-whole-move.png')
sheet=Image.new('RGB',(1170,1080),'#0b2a36');draw=ImageDraw.Draw(sheet)
for k,n in enumerate(range(78,121)):
    x=k%10*117;y=k//10*270
    if y+270>sheet.height:
        extra=Image.new('RGB',(1170,y+270),'#0b2a36');extra.paste(sheet,(0,0));sheet=extra;draw=ImageDraw.Draw(sheet)
    sheet.paste(Image.open(QA/f'temporal/frame-{n:03d}.png').convert('RGB'),(x,y+17))
    draw.text((x+4,y+1),f'{n/60:.3f}',font=tiny,fill='#f8ebd0')
sheet.save(QA/'continuity-middle-consecutive.png')
times=[0,.75,1.3,1.5,1.75,2,2.25,3]
sheet=Image.new('RGB',(1440,427),'#0b2a36');draw=ImageDraw.Draw(sheet)
for i,t in enumerate(times):
    p=EXPORT/('geometry-'+f'{t:.2f}'.replace('.','-')+'.png')
    if not p.exists():continue
    sheet.paste(Image.open(p).convert('RGB').resize((180,390),Image.Resampling.LANCZOS),(i*180,30))
    draw.text((i*180+5,6),f'{t:.2f} s',font=font,fill='#f8ebd0')
sheet.save(QA/'continuity-checkpoints.png')
floor=Image.new('RGB',(1248,410),'#0b2a36');draw=ImageDraw.Draw(floor)
for i,t in enumerate([1.3,1.5,1.75,2]):
    p=EXPORT/('geometry-'+f'{t:.2f}'.replace('.','-')+'.png')
    if p.exists():
        im=Image.open(p).convert('RGB');im=im.crop((0,round(im.height*.60),im.width,im.height)).resize((312,360),Image.Resampling.LANCZOS)
        floor.paste(im,(i*312,35));draw.text((i*312+8,8),f'{t:.2f} s - one floor plane',font=font,fill='#f8ebd0')
floor.save(QA/'continuity-floor-critical.png')
# Compare the supplied MP4's 2s frame with the corrected actual-browser pose.
before=EXPORT/'input-video-2.png';after=EXPORT/'geometry-2-00.png'
if before.exists() and after.exists():
    sheet=Image.new('RGB',(800,900),'#0b2a36');draw=ImageDraw.Draw(sheet)
    oldim=Image.open(before).convert('RGB').crop((710,0,1210,1080)).resize((390,844),Image.Resampling.LANCZOS)
    newim=Image.open(after).convert('RGB').resize((390,844),Image.Resampling.LANCZOS)
    sheet.paste(oldim,(0,42));sheet.paste(newim,(400,42))
    draw.text((8,10),'Supplied MP4 - 2.0 s',font=font,fill='#f8ebd0');draw.text((408,10),'Closed wall / planar floor - 2.0 s',font=font,fill='#f8ebd0')
    sheet.save(QA/'continuity-before-after-2s.png')
manifest=dict(revision='V2 continuity correction',source='Assets/_Game/Presentation/Resources/menu_bg.png',sourceSha256=sha(original),originalPreserved=True,wallPlane=[-.585,0,1,-5.32],floorY=-1.5,columnExtrusion=.2,frameOffset=.08,plantForeground='camera-facing fixed object at repeated world positions',floorCoordinates='orthonormal S=(x+.585z)/sqrt(1+.585^2), N=(z-.585x)/sqrt(1+.585^2)',floorPhotoFallback=False,outdoorFacade=False,staticReference='Static.png rendered from same scene at camera origin',cameraTrajectoryUnchanged=True,duration=3,easing='22% smooth acceleration + 56% constant velocity + 22% smooth deceleration',oldPeakSpeed=1.875,newPeakSpeed=1/.78,perFrameGeneration=False,plantExtractionPrompts='docs/art/corridor-continuity-prompts.json',assets={n:dict(size=list(Image.open(ART/f'{n}.png').size),sha256=sha(ART/f'{n}.png')) for n in ['Source','Static','wall','floor','frame','columns','arches']})
(ART/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
result=dict(unchangedFiles=unchanged,cameraTrajectoryUnchanged=True,duration=3,wholeMoveFrames=181,criticalConsecutiveFrames=43,isolatedTemporalSpikes=spikes,interFrameMeanRgbErrors=errors,entryDifference=browser['entryDifference'],exitDifference=browser['exitDifference'],endpointPixelsIdentical=True,floorPhotoFallback=False,scope='Browser full-sequence render review and source/topology checks. Unity game integration is reported separately in qa/unity/win-menu-integration.json; device/player validation is not implied.')
result['outsideFacadeExcludedFromSourceMasks']=True
result['floorRepeatMeanRgbError']=seam_error
(QA/'continuity-report.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:result[k] for k in ['cameraTrajectoryUnchanged','duration','wholeMoveFrames','criticalConsecutiveFrames','isolatedTemporalSpikes','entryDifference','exitDifference']}))
