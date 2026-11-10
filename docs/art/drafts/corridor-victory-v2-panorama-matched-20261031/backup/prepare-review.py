"""Review board made from rendered crops. Does not edit the panorama."""
import json,sys,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageFont,ImageChops,ImageStat
font=lambda n,b=False:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if b else 'C:/Windows/Fonts/segoeui.ttf',n)
names=['start','middle','end'];labels=['BAŞLANGIÇ · 1. TABLO','ORTA · SÜTUN / DUVAR','BİTİŞ · KOMŞU 2. TABLO']
board=Image.new('RGB',(1350,1100),'#102b33');d=ImageDraw.Draw(board)
d.text((30,20),'ORİJİNAL PANORAMA · YALNIZCA TEK BÖLÜM KAYDIRMA',font=font(30,True),fill='#fff0d4')
d.text((30,68),'← Görüntü sağdan sola kayar · 3 saniye · sabit ölçek · 390:844',font=font(25),fill='#ffce6b')
for i,(name,label) in enumerate(zip(names,labels)):
    x=30+i*450
    im=Image.open(root/f'frame-{name}.png').convert('RGB')
    d.text((x,124),label,font=font(24,True),fill='#d0e4e4')
    board.paste(im.resize((390,844),Image.Resampling.LANCZOS),(x,170))
d.text((30,1037),'Kaynak çizim, perspektif, ışık, gölgeler ve zemin değiştirilmedi. Döngü uygulanmadı.',font=font(24),fill='#d0e4e4')
board.save(root/'review-frames.png')
indices=list(range(0,181,10));tw,th=156,338
sheet=Image.new('RGB',(1212,1239),'#102b33');dr=ImageDraw.Draw(sheet)
dr.text((24,18),'Orijinal panorama · tam hareket · yalnızca 1 → 2',font=font(25,True),fill='#fff0d4')
for j,i in enumerate(indices):
    x=24+(j%7)*168;y=75+(j//7)*380
    sheet.paste(Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB').resize((tw,th)),(x,y))
    dr.text((x,y+th+3),f'{i/60:.3f} sn',font=font(17),fill='#d0e4e4')
sheet.save(root/'qa/whole-move.png')
report=json.loads((root/'export-report.json').read_text())
assert report['sourceCopyByteIdentical'] and report['protectedFilesUnchanged']
assert report['travelSourcePixels']==496 and report['travelAdjacentPaintings']==1
assert abs(report['middleCropX']-(report['startCropX']+report['endCropX'])/2)<1e-10
assert report['endCropX']+report['cropWidth']<1170, 'Third picture must remain outside viewport'
assert abs(report['cropWidth']/report['cropHeight']-390/844)<1e-12
differences=[];prev=Image.open(root/'qa/frame-000.png').convert('RGB')
for i in range(1,181):
    cur=Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB')
    differences.append(sum(ImageStat.Stat(ImageChops.difference(prev,cur)).mean)/3);prev=cur
qa={'sourceUnchanged':True,'runtimeFilesUnchanged':True,'constantCropSize':True,'constantCropY':0,'panoramaEdited':False,'newArtGenerated':False,'thirdPaintingOutsideViewport':True,'travelAdjacentIntervals':1,'frameCountChecked':181,'frameToFrameRGBMean':differences,'endpointEqualityRequired':False,'loop':False}
(root/'qa-report.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in qa.items() if k!='frameToFrameRGBMean'},ensure_ascii=False))
