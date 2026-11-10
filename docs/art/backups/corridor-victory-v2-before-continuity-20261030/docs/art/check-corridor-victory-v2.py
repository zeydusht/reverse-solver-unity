from pathlib import Path
import sys,json,hashlib
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image,ImageDraw,ImageFont,ImageChops,ImageStat
QA=ROOT/'docs/art/corridor-victory/qa'
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',20)
small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',14)
sheet=Image.new('RGB',(1170,558),'#0b2a36');draw=ImageDraw.Draw(sheet)
for i,(name,label) in enumerate([('0','0% - approved'),('0-25','25%'),('0-5','50%'),('0-75','75%'),('1','100% - approved')]):
    im=Image.open(QA/f'frame-{name}.png').convert('RGB').resize((234,506),Image.Resampling.LANCZOS)
    sheet.paste(im,(i*234,42));draw.text((i*234+8,10),label,font=font,fill='#f8ebd0')
sheet.save(QA/'checkpoints-v2.png')
sheet=Image.new('RGB',(1053,290),'#0b2a36');draw=ImageDraw.Draw(sheet)
for i,frame in enumerate(range(86,95)):
    sheet.paste(Image.open(QA/f'temporal/frame-{frame:03d}.png').convert('RGB'),(i*117,30))
    draw.text((i*117+5,7),f'{frame/60:.3f} s',font=small,fill='#f8ebd0')
sheet.save(QA/'consecutive-middle-v2.png')
old=ROOT/'docs/art/backups/corridor-victory-v1-20261030/docs/art/corridor-victory/qa/frame-0-5.png'
sheet=Image.new('RGB',(800,890),'#0b2a36');draw=ImageDraw.Draw(sheet)
for i,(path,label) in enumerate([(old,'Previous 50%'),(QA/'frame-0-5.png','Original-art reconstruction 50%')]):
    sheet.paste(Image.open(path).convert('RGB').resize((390,844)),(i*400,40))
    draw.text((i*400+8,10),label,font=font,fill='#f8ebd0')
sheet.save(QA/'before-after-v2.png')
errors=[];prior=None
for frame in range(181):
    im=Image.open(QA/f'temporal/frame-{frame:03d}.png').convert('RGB')
    if prior is not None:errors.append(sum(ImageStat.Stat(ImageChops.difference(im,prior)).mean)/3)
    prior=im
spikes=[]
for i in range(2,len(errors)-2):
    surrounding=sorted(errors[i-2:i]+errors[i+1:i+3]);median=(surrounding[1]+surrounding[2])/2
    if errors[i]>3*median+.5:spikes.append(i+1)
protected=json.loads((ROOT/'docs/art/backups/corridor-victory-v1-20261030/protected-hashes.json').read_text(encoding='utf-8-sig'))
checks=[]
for item in protected:
    actual=hashlib.sha256(Path(item['Path']).read_bytes()).hexdigest().upper()
    checks.append(dict(path=str(Path(item['Path']).relative_to(ROOT)),unchanged=actual==item['Hash']))
assert all(item['unchanged'] for item in checks),'Camera, behavior or approved original changed'
assert not spikes,'Isolated temporal discontinuity detected'
art=ROOT/'Assets/Art/Menu/CorridorVictoryV2'
manifest=dict(version=2,original='Assets/_Game/Presentation/Resources/menu_bg.png',originalSha256=hashlib.sha256((ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png').read_bytes()).hexdigest(),protectedFilesUnchanged=True,perFrameGeneration=False,endpointArtCrossfade=False,hiddenRepairSource='Assets/Art/Menu/CorridorVictoryV2/join-repair-source.png',repairPrompts='docs/art/corridor-victory-v2-prompts.json',repairCalibration=dict(progress=.5,camera=[-1.71,0,-1.32035],preview=[390,844],screenX=[.50,.72]),assets={name:dict(size=list(Image.open(art/f'{name}.png').size),sha256=hashlib.sha256((art/f'{name}.png').read_bytes()).hexdigest()) for name in ['wall','floor','frame','columns','arches']})
(art/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
browser=json.loads((QA/'report.json').read_text())
assert browser['entryDifference']['meanRgbError']<.15,'Visible entry handoff difference'
assert browser['exitDifference']['meanRgbError']<.15,'Visible exit handoff difference'
result=dict(protectedFiles=checks,consecutiveFrames=181,interFrameMeanRgbErrors=errors,isolatedTemporalSpikes=spikes,entryDifference=browser['entryDifference'],exitDifference=browser['exitDifference'],scope='Automated continuity measurements support visual inspection; they are not proof of artistic quality. Five checkpoint and nine consecutive middle-frame contact sheets are supplied.')
(QA/'art-consistency-report.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(dict(protectedFilesUnchanged=True,consecutiveFrames=181,isolatedTemporalSpikes=spikes,entryMeanRgbError=browser['entryDifference']['meanRgbError'],exitMeanRgbError=browser['exitDifference']['meanRgbError'])))
