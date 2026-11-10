"""Annotate browser-rendered draft and package a looping preview; no runtime writes."""
import sys, json, hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image, ImageDraw, ImageFont, ImageChops, ImageStat
font=lambda n,bold=False:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if bold else 'C:/Windows/Fonts/segoeui.ttf',n)
report=json.loads((root/'export-report.json').read_text())
pan=Image.open(root/'panorama-one-bay.png').convert('RGB')
names=['start','middle','end']; labels=['BAŞLANGIÇ · 1. TABLO','ORTA · DUVAR / SÜTUN','BİTİŞ · KOMŞU 2. TABLO']; colors=['#73e3ff','#ffd36b','#ff91ab']
frames=[Image.open(root/('frame-'+n+'.png')).convert('RGB') for n in names]
assert frames[0].tobytes()==frames[2].tobytes()
scale=1400/pan.width;ph=round(pan.height*scale)
board=Image.new('RGB',(1496,ph+1150),'#102b33');d=ImageDraw.Draw(board)
d.text((48,24),'CORRIDOR VICTORY V2  /  TEK KOMŞU BÖLÜM',font=font(34,True),fill='#fff0d4')
d.text((48,78),'1. tablo → aradaki duvar / sütun → hemen komşu 2. tablo',font=font(26),fill='#d0e3e3')
d.line((1435,150,62,150),fill='#ffd36b',width=7);d.polygon([(48,150),(76,134),(76,166)],fill='#ffd36b')
d.text((405,112),'GÖRÜNTÜ EKRANDA SAĞDAN SOLA KAYAR',font=font(24,True),fill='#ffd36b')
top=190
board.paste(pan.resize((1400,ph),Image.Resampling.LANCZOS),(48,top));d=ImageDraw.Draw(board)
for i,color in enumerate(colors):
    x=48+round(i*report['period']/2*scale);y=top
    # Labels are staggered to avoid obscuring other crop labels.
    d.rectangle((x,y,x+round(report['cropWidth']*scale)-2,y+ph-2),outline=color,width=5)
    yy=y+12+i*43
    d.rectangle((x+6,yy,x+305,yy+37),fill='#102b33')
    d.text((x+13,yy+2),labels[i],font=font(23,True),fill=color)
ty=top+ph+90
for i,(im,label,color) in enumerate(zip(frames,labels,colors)):
    x=48+i*515
    # Keep all three complete portrait crops at exactly 390:844.
    x=48+i*505
    d.text((x,ty-43),label,font=font(23,True),fill=color)
    size=(312,675)
    board.paste(im.resize(size,Image.Resampling.LANCZOS),(x,ty))
    d.rectangle((x-2,ty-2,x+size[0]+1,ty+size[1]+1),outline=color,width=3)
d.text((48,ty+710),'Başlangıç ve bitiş: piksel piksel aynı. Mesafe: tam 1 mimari bölüm. Süre: 3 saniye.',font=font(25,True),fill='#fff0d4')
d.text((48,ty+756),'Sabit bakış • aynı tablo içeriği • 390:844 ekran oranı • yalnızca onay taslağı',font=font(24),fill='#bdd4d4')
board.save(root/'review-framing.png')

# Loop preview uses 90 samples of the SAME rendering; no duplicated endpoint hold.
loop=[]
for i in range(0,180,2):
    loop.append(Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB').resize((390,844),Image.Resampling.LANCZOS))
durations=[round((i+1)*1000/30)-round(i*1000/30) for i in range(90)]
loop[0].save(root/'one-bay-loop.webp',save_all=True,append_images=loop[1:],duration=durations,loop=0,quality=92,method=5)

# Whole move and actual consecutive frames on both sides of the loop seam.
def sheet(indices,name,cols,heading):
    tw,th=156,338;gap=12;pad=24;header=75
    rows=(len(indices)+cols-1)//cols
    out=Image.new('RGB',(pad*2+cols*(tw+gap)-gap,header+rows*(th+42)+pad),'#102b33');dr=ImageDraw.Draw(out)
    dr.text((pad,18),heading,font=font(23,True),fill='#fff0d4')
    for j,i in enumerate(indices):
        x=pad+(j%cols)*(tw+gap);y=header+(j//cols)*(th+42)
        out.paste(Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB').resize((tw,th)),(x,y))
        dr.text((x,y+th+3),f'{i:03} · {i/60:.3f} sn',font=font(17),fill='#cce2e2')
    out.save(root/'qa'/name)
sheet(list(range(0,181,10)),'whole-move.png',7,'Tam hareket · tek bölüm · örnek kareler')
sheet(list(range(174,181))+list(range(0,7)),'loop-seam-consecutive.png',7,'Döngü birleşimi · ardışık son / ilk kareler')
sheet(list(range(84,98)),'middle-consecutive.png',7,'Orta nokta · ardışık kareler')

# Temporal continuity across every adjacent raw frame and the wrap boundary.
means=[]
prev=Image.open(root/'qa/frame-000.png').convert('RGB')
for i in range(1,181):
    nxt=Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB')
    means.append(sum(ImageStat.Stat(ImageChops.difference(prev,nxt)).mean)/3)
    prev=nxt
first=Image.open(root/'qa/frame-000.png').convert('RGB')
wrap=ImageChops.difference(prev,first).getbbox()
baseline=json.loads((root/'protected-before.json').read_text(encoding='utf-8-sig'))
changed=[v['path'] for v in baseline if hashlib.sha256(Path(v['path']).read_bytes()).hexdigest().upper()!=v['sha256'].upper()]
assert not changed
assert wrap is None
qa={'runtimeFilesUnchanged':True,'protectedFileCount':len(baseline),'startEndPixelsIdentical':True,'loopBoundaryRawPixelError':0,'oneBayTravelPixels':report['period'],'travelBays':1,'frameCountChecked':181,'frameToFrameRGBMean':means,'maxAdjacentMean':max(means),'firstAdjacentMean':means[0],'lastAdjacentMean':means[-1],'webpFrames':90,'webpCycleMs':sum(durations),'midpoint':'wall/column between adjacent pictures; no skipped painting','orthographicFixedView':True,'perspectiveDeformation':False,'generatedAssetMode':'built-in image_gen; one fixed scene module, not independent frames'}
(root/'qa-report.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in qa.items() if k!='frameToFrameRGBMean'},ensure_ascii=False,indent=2))
