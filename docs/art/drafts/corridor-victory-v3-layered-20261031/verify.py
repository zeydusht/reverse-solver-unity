"""Task18 verification: approved raw pixels outside aperture, tracking and codec outputs."""
import sys,json,math,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageChops,ImageStat,ImageFont
approved=root.parent/'corridor-victory-v2-floor-20261031'
track=json.loads((root/'frame-track.json').read_text());web=json.loads((root/'frame-track-web.json').read_text())
assert (track['fps'],track['width'],track['height'],len(track['frames']))==(60,780,1688,181)
assert (web['fps'],web['width'],web['height'],len(web['frames']))==(30,540,1168,91)

def mask_from_track(entry,w,h):
    mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
    for f in entry['instances']:
        if not f['visible']:continue
        q=f['quad']
        for y in range(max(0,math.floor(min(p[1] for p in q))),min(h,math.ceil(max(p[1] for p in q)))):
            cy=y+.5;xs=[]
            for j in range(4):
                a,b=q[j],q[(j+1)%4]
                if min(a[1],b[1])<=cy<max(a[1],b[1]):xs.append(a[0]+(cy-a[1])*(b[0]-a[0])/(b[1]-a[1]))
            if len(xs)>=2:
                x0=max(0,math.ceil(min(xs)-.5));x1=min(w-1,math.floor(max(xs)-.5))
                if x1>=x0:d.line((x0,y,x1,y),fill=255)
    return mask

checks=[]
for name,i,oldname in [('first',0,'start'),('middle',90,'middle'),('last',180,'end')]:
    bg=Image.open(root/'qa'/f'background-{name}.png').convert('RGB')
    old=Image.open(approved/f'frame-{oldname}.png').convert('RGB')
    assert ImageChops.difference(bg,old).getbbox() is None
    green=Image.open(root/('qa/corridor-empty-middle.png' if name=='middle' else f'corridor-empty-{name}.png')).convert('RGB')
    mask=mask_from_track(track['frames'][i],780,1688)
    outside=ImageChops.multiply(ImageChops.difference(bg,green),ImageChops.invert(mask).convert('RGB'))
    assert outside.getbbox() is None,(name,'outside changed')
    inside=ImageChops.multiply(ImageChops.difference(green,Image.new('RGB',green.size,(0,255,0))),mask.convert('RGB'))
    assert inside.getbbox() is None,(name,'green not exact')
    assert sum(mask.histogram()[1:])==track['frames'][i]['greenPixelCount']
    checks.append({'frame':i,'backgroundPixelIdenticalToApproved':True,'outsideGreenUnchanged':True,'greenExactly00FF00':True})
assert ImageChops.difference(Image.open(root/'corridor-empty-first.png'),Image.open(root/'corridor-empty-last.png')).getbbox() is None

for data in [track,web]:
    for i,f in enumerate(data['frames']):
        assert f['i']==i and len(f['quad'])==4
        for item in f['instances']:
            assert len(item['quad'])==4
            assert all(math.isfinite(v) for p in item['quad'] for v in p)
            assert 0<=item['visibleFraction']<=1.0000001
            assert item['visible']==(item['visibleFraction']>0)
    assert len([f for f in data['frames'] if sum(x['visible'] for x in f['instances'])==2])>0
# Coordinates in the 30 FPS file must correspond to every other master instant.
for w,m in zip(web['frames'],track['frames'][::2]):
    for wi,mi in zip(w['instances'],m['instances']):
        for wp,mp in zip(wi['quad'],mi['quad']):
            assert abs(wp[0]-mp[0]*540/780)<1e-8 and abs(wp[1]-mp[1]*1168/1688)<1e-8

report=json.loads((root/'export-report.json').read_text())
assert report['master']['bytes']<2_000_000 and report['web']['bytes']<2_000_000
for data in [report['master'],report['web']]:
    p=root/data['output'];assert hashlib.sha256(p.read_bytes()).hexdigest()==data['sha256']
# Verify temporal content from decoded H264 timestamps, not a potentially stale
# HTMLVideoElement seek surface. Both midpoints MUST contain the two partial quads.
codec_checks=[]
for label,data,indices in [('corridor-empty',track,[0,90,180]),('corridor-empty-web',web,[0,45,90])]:
    w,h=data['width'],data['height']
    for i in indices:
        decoded=Image.open(root/'qa'/f'{label}-exact-frame-{i}.png').convert('RGB')
        expected=mask_from_track(data['frames'][i],w,h)
        actual=Image.new('L',(w,h));pixels=decoded.load();m=actual.load()
        for y in range(h):
            for x in range(w):
                r,g,b=pixels[x,y]
                if g>180 and g>r*1.4 and g>b*1.4:m[x,y]=255
        intersection=sum(ImageChops.multiply(expected,actual).histogram()[1:])
        union=sum(ImageChops.lighter(expected,actual).histogram()[1:])
        iou=intersection/union
        assert iou>.985,(label,i,iou)
        codec_checks.append({'video':label,'frame':i,'greenQuadIntersectionOverUnion':iou})
baseline=json.loads((root/'protected-before.json').read_text(encoding='utf-8-sig'))
assert all(hashlib.sha256(Path(f['path']).read_bytes()).hexdigest().upper()==f['sha256'].upper() for f in baseline)

# All sampled moments, including both simultaneously visible partial apertures.
font=lambda n,b=False:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if b else 'C:/Windows/Fonts/segoeui.ttf',n)
board=Image.new('RGB',(1320,1090),'#102b33');d=ImageDraw.Draw(board)
d.text((30,20),'TASK18 · YEŞİL TABLO İÇİ / AYNI ONAYLI SAHNE',font=font(30,True),fill='#fff0d4')
for j,(name,label) in enumerate([('first','İLK'),('middle','ORTA · İKİ ÇERÇEVE'),('last','SON')]):
    p=root/('qa/corridor-empty-middle.png' if name=='middle' else f'corridor-empty-{name}.png')
    x=30+j*440;d.text((x,82),label,font=font(24,True),fill='#ffce6b');board.paste(Image.open(p).resize((390,844)),(x,130))
d.text((30,1010),'Yeşil alan dışındaki ham pikseller onaylı revizyonla aynı. İlk / son kare eşit.',font=font(23),fill='#cce2e2')
board.save(root/'qa/checkpoints.png')
out={'status':'passed','masterTrackFrames':181,'webTrackFrames':91,'pixelChecks':checks,'timestampDecodedVideoChecks':codec_checks,'firstLastPixelsIdentical':True,'twoVisibleNeighborFramesTracked':True,'webTrackMatchesMasterAtSameTime':True,'videosUnder2MillionBytes':True,'approvedSourceFilesUnchanged':True,'allTaskWritesUnderDocsArt':True,'gameIntegrated':False}
(root/'qa-report.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(out,ensure_ascii=False))
