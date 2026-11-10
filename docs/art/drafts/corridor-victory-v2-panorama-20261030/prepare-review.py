"""Derive approval crops/annotations from ONE image. Never edits runtime assets."""
import sys, json, hashlib
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[2] / '.task16-python'))
from PIL import Image, ImageDraw, ImageFont

root = Path(__file__).resolve().parent
im = Image.open(root / 'panorama-clean.png').convert('RGB')
w, h = im.size
# Exact 390:844 aspect with fractional source crop, full source height.
# Keep the decorative outer frame scrollwork inside both endpoint crops.
cw, ch = h * 390 / 844, h
y = 0
margin = w * 0.056
positions = [margin, (w-cw)/2, w-cw-margin]
names = ['start', 'middle', 'end']
labels = ['BAŞLANGIÇ', 'ORTA', 'BİTİŞ']
colors = ['#63e3ff', '#ffd15b', '#ff8ca3']
crops = []
for name, x in zip(names, positions):
    crop = im.transform((390,844), Image.Transform.EXTENT, (x,y,x+cw,y+ch), Image.Resampling.BICUBIC)
    crop.save(root / ('frame-' + name + '.png'))
    crops.append(crop)

def font(n, bold=False):
    return ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf' if bold else 'C:/Windows/Fonts/segoeui.ttf', n)

# Review graphics are drawn only on this separate board.
scale = min(1, 1800/w)
pw, ph = round(w*scale), round(h*scale)
pad, header = 48, 186
thumbh = 650
thumbw = round(thumbh*390/844)
board = Image.new('RGB', (pw+pad*2, header+ph+100+thumbh+130), '#102b33')
d = ImageDraw.Draw(board)
d.text((pad,24), 'CORRIDOR VICTORY V2  /  SAHNE ONAYI', font=font(32,True), fill='#fff0d4')
d.text((pad,72), 'Tek panorama • sabit perspektif • kesintisiz duvar • kadraj 390:844', font=font(24), fill='#c9dfdf')
ay=142
d.line((pad+pw-20,ay,pad+20,ay), fill='#ffcd62', width=7)
d.polygon([(pad,ay),(pad+26,ay-15),(pad+26,ay+15)],fill='#ffcd62')
d.text((pad+pw//2-235,ay-32), 'GÖRÜNTÜ EKRANDA SAĞDAN SOLA KAYAR',font=font(21,True),fill='#ffcd62')
board.paste(im.resize((pw,ph),Image.Resampling.LANCZOS),(pad,header))
d = ImageDraw.Draw(board)
for x,label,color in zip(positions,labels,colors):
    box=(pad+round(x*scale),header+round(y*scale),pad+round((x+cw)*scale)-2,header+round((y+ch)*scale)-2)
    d.rectangle(box,outline=color,width=5)
    d.rectangle((box[0]+6,box[1]+7,box[0]+218,box[1]+47),fill='#102b33')
    d.text((box[0]+14,box[1]+9),label,font=font(23,True),fill=color)
ty=header+ph+80
for i,(crop,label,color) in enumerate(zip(crops,labels,colors)):
    tx=pad+round((pw-thumbw)*i/2)
    board.paste(crop.resize((thumbw,thumbh),Image.Resampling.LANCZOS),(tx,ty))
    d.rectangle((tx-2,ty-2,tx+thumbw+1,ty+thumbh+1),outline=color,width=3)
    d.text((tx,ty-40),label+'  ·  390:844',font=font(23,True),fill=color)
d.text((pad,ty+thumbh+35),'Üç görüntü de aynı panoramanın kırpımıdır. Ayrı kare üretimi veya animasyon uygulaması yapılmadı.',font=font(22),fill='#c9dfdf')
board.save(root/'review-framing.png')

manifest={'status':'awaiting visual approval; no runtime integration', 'panoramaSize':[w,h], 'viewportAspect':[390,844], 'cropSourceSize':[cw,ch], 'cropOutputSize':[390,844], 'cropY':y,'cropXs':positions,'imageSlides':'right to left','viewportSamples':'left, center, right across one fixed perspective panorama','travelPixels':positions[-1]-positions[0],'travelViewportWidths':round((positions[-1]-positions[0])/cw,3),'files':{}}
for name in ['panorama-clean.png','review-framing.png']+['frame-'+n+'.png' for n in names]:
    p=root/name
    manifest['files'][name]={'size':list(Image.open(p).size),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
baseline=json.loads((root/'protected-before.json').read_text(encoding='utf-8-sig'))
changed=[item['path'] for item in baseline if hashlib.sha256(Path(item['path']).read_bytes()).hexdigest().upper()!=item['sha256'].upper()]
manifest['protectedFilesUnchanged']=not changed
manifest['changedProtectedFiles']=changed
(root/'review-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
assert not changed, changed
print(json.dumps(manifest,ensure_ascii=False,indent=2))
