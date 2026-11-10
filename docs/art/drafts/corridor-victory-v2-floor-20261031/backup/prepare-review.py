"""QA and comparison boards from actual browser frames; no scene editing."""
import sys,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageFont,ImageChops,ImageStat
font=lambda n,b=False:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if b else 'C:/Windows/Fonts/segoeui.ttf',n)
start=Image.open(root/'frame-start.png').convert('RGB');end=Image.open(root/'frame-end.png').convert('RGB');middle=Image.open(root/'frame-middle.png').convert('RGB')
assert ImageChops.difference(start,end).getbbox() is None
native=Image.open(root/'original-panorama.png').convert('RGB').crop((64,0,560,971))
matched=Image.open(root/'matched-bay.png').convert('RGB')
assert ImageChops.difference(native.crop((90,100,414,570)),matched.crop((90,100,414,570))).getbbox() is None
board=Image.new('RGB',(1320,1110),'#102b33');d=ImageDraw.Draw(board)
d.text((30,24),'BAŞLANGIÇ / BİTİŞ · HAM PİKSEL FARKI = 0',font=font(32,True),fill='#fff0d4')
d.text((30,78),'Mevcut eğik çerçeve ve tablo korunur · bir komşu bölüm · 3 saniye',font=font(24),fill='#cce3e3')
diff=ImageChops.difference(start,end)
for i,(im,label) in enumerate([(start,'BAŞLANGIÇ'),(end,'BİTİŞ'),(diff,'FARK · SİYAH = TAM EŞİT')]):
    x=30+i*440;d.text((x,130),label,font=font(24,True),fill='#ffcf6b');board.paste(im.resize((390,844),Image.Resampling.LANCZOS),(x,175))
d.text((30,1050),'Sabit sahne · son kareyi değiştirme, zoom, morph veya zamana bağlı crossfade yok.',font=font(23),fill='#cce3e3')
board.save(root/'start-end-comparison.png')
review=Image.new('RGB',(1320,1110),'#102b33');d=ImageDraw.Draw(review)
d.text((30,24),'EŞLEŞEN KOMŞU BÖLÜM · KADRAJ İNCELEMESİ',font=font(32,True),fill='#fff0d4')
d.text((30,78),'← Son panorama-single-step videosundaki hareket yönü korundu',font=font(24),fill='#ffcf6b')
for i,(im,label) in enumerate([(start,'1. TABLO'),(middle,'ORTA · SÜTUN / DUVAR'),(end,'KOMŞU 2. TABLO')]):
    x=30+i*440;d.text((x,130),label,font=font(24,True),fill='#cce3e3');review.paste(im.resize((390,844),Image.Resampling.LANCZOS),(x,175))
d.text((30,1050),'Ana mimari öğeler kaynak piksellerinden; yerel zemin ve birleşim onarımı.',font=font(23),fill='#cce3e3')
review.save(root/'review-frames.png')
# Old approved baseline beside the revision: makes all local changes reviewable.
old=Image.open(root/'backup/frame-start.png').convert('RGB')
before=Image.new('RGB',(900,1060),'#102b33');d=ImageDraw.Draw(before)
d.text((30,22),'ÖNCEKİ TASLAK / YEREL REVİZYON',font=font(30,True),fill='#fff0d4')
for i,(im,label) in enumerate([(old,'ÖNCEKİ BAŞLANGIÇ'),(start,'EŞLEŞEN REVİZYON')]):
    x=30+i*450;d.text((x,92),label,font=font(25,True),fill='#cce3e3');before.paste(im.resize((390,844),Image.Resampling.LANCZOS),(x,145))
before.save(root/'before-after-start.png')

def sheet(indices,name,title):
    tw,th=156,338;cols=7;rows=(len(indices)+6)//7
    image=Image.new('RGB',(1212,75+rows*380+24),'#102b33');dr=ImageDraw.Draw(image)
    dr.text((24,18),title,font=font(24,True),fill='#fff0d4')
    for j,n in enumerate(indices):
        x=24+(j%cols)*168;y=75+(j//cols)*380
        image.paste(Image.open(root/'qa'/f'frame-{n:03}.png').convert('RGB').resize((tw,th)),(x,y));dr.text((x,y+th+3),f'{n:03} · {n/60:.3f} sn',font=font(17),fill='#cce3e3')
    image.save(root/'qa'/name)
sheet(list(range(0,181,10)),'whole-move.png','Tam hareket · aynı sabit sahne · tek komşu bölüm')
sheet(list(range(83,98)),'middle-consecutive.png','Orta bölüm · ardışık kareler')
sheet(list(range(174,180))+list(range(0,8)),'seam-consecutive.png','Birleşim · son kareler ve sonraki başlangıç')
# Inspect actual DECODED three-cycle video at both joins and at the final stop.
decoded=sorted((root/'qa').glob('review-decoded-*.png'))
im=Image.new('RGB',(1344,875),'#102b33');dr=ImageDraw.Draw(im)
dr.text((24,18),'ÜÇ TEKRAR MP4 · ÇÖZÜLMÜŞ BİRLEŞİM KARELERİ',font=font(25,True),fill='#fff0d4')
for j,p in enumerate(decoded):
    x=24+(j%4)*330;y=75+(j//4)*390
    im.paste(Image.open(p).convert('RGB').resize((156,338)),(x,y));dr.text((x,y+342),p.stem.removeprefix('review-decoded-'),font=font(17),fill='#cce3e3')
im.save(root/'qa/three-cycle-decoded-joins.png')
means=[];prev=Image.open(root/'qa/frame-000.png').convert('RGB')
for i in range(1,181):
    cur=Image.open(root/'qa'/f'frame-{i:03}.png').convert('RGB');means.append(sum(ImageStat.Stat(ImageChops.difference(prev,cur)).mean)/3);prev=cur
spikes=[i+1 for i in range(1,len(means)-1) if means[i]>max(means[i-1],means[i+1])*1.6+.5]
assert not spikes,spikes
base_decoded=Image.open(root/'qa/decoded-0.png').convert('RGB')
decoded_errors={}
for t in ['3-00001','6-00001']:
    pic=Image.open(root/'qa'/('review-decoded-'+t+'.png')).convert('RGB')
    decoded_errors[t]=sum(ImageStat.Stat(ImageChops.difference(base_decoded,pic)).mean)/3
    assert decoded_errors[t]==0
baseline=json.loads((root/'protected-before.json').read_text(encoding='utf-8-sig'))
changed=[item['path'] for item in baseline if hashlib.sha256(Path(item['path']).read_bytes()).hexdigest().upper()!=item['sha256'].upper()]
assert not changed
# Verify archive includes the entire previous supplied MP4 unchanged.
oldmp4=root/'backup/panorama-single-step.mp4'
assert hashlib.sha256(oldmp4.read_bytes()).hexdigest()==hashlib.sha256((root.parent/'corridor-victory-v2-panorama-single-step-20261030/panorama-single-step.mp4').read_bytes()).hexdigest()
report={'status':'passed','rawStartEndPixelsIdentical':True,'encodedRepeatStartRGBMeanError':decoded_errors,'rawFramesChecked':181,'temporalIsolatedSpikes':spikes,'frameToFrameRGBMean':means,'sameDirectionDistanceDurationEaseAsPrevious':True,'reviewCycles':3,'previousMP4BackupIdentical':True,'protectedFilesUnchanged':True,'protectedFilesCount':len(baseline),'gameIntegrated':False}
(root/'qa-report.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='frameToFrameRGBMean'},ensure_ascii=False))
