from pathlib import Path
import sys, math, json, hashlib
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'docs/art/.task16-python'))
from PIL import Image, ImageDraw, ImageFilter, ImageFont, ImageChops
OUT=ROOT/'Assets/Art/Gameplay/Task17'; OUT.mkdir(parents=True,exist_ok=True)
S=4; R=Image.Resampling.LANCZOS
P={'navy':'#0B2A36','dark':'#0F4F5A','teal':'#1C7F8C','gold':'#FBBA0E','mid':'#F38A03','shade':'#CE5D01','light':'#FFF1C2','cream':'#F8EBD0'}
protected=[ROOT/'Assets/Art/Gameplay/Task14'/n for n in ['chain-link-repeat-96x48.png','chain-endpoint-144.png']]+[ROOT/f'Assets/Art/Paintings/painting-{n}.jpg' for n in (40,24)]
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
before={str(p.relative_to(ROOT)):sha(p) for p in protected}
def canvas(size):return Image.new('RGBA',(size[0]*S,size[1]*S))
def box(b):return tuple(round(v*S) for v in b)
def color(c):return tuple(bytes.fromhex(c.lstrip('#')))
def gradient(size,stops):
    im=Image.new('RGBA',size);d=ImageDraw.Draw(im)
    for y in range(size[1]):
        t=y/max(1,size[1]-1)
        for i in range(len(stops)-1):
            a,ca=stops[i];b,cb=stops[i+1]
            if a<=t<=b:
                q=(t-a)/(b-a);rgb=tuple(round(x*(1-q)+z*q) for x,z in zip(color(ca),color(cb)));break
        d.line((0,y,size[0],y),fill=rgb+(255,))
    return im
def ring(im,b,width,anchors):
    mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
    d.rounded_rectangle(box(b),radius=(b[3]-b[1])*S/2,fill=255)
    inner=tuple(v+(width if i<2 else -width) for i,v in enumerate(b))
    if inner[2]>inner[0] and inner[3]>inner[1]:
        d.rounded_rectangle(box(inner),radius=max(1,((b[3]-b[1])/2-width)*S),fill=0)
    g=gradient((im.width,round((b[3]-b[1])*S)),anchors)
    full=Image.new('RGBA',im.size);full.paste(g,(0,round(b[1]*S)));im.alpha_composite(Image.composite(full,Image.new('RGBA',im.size),mask))
def steel(im,cx,cy,w=92,h=46):
    b=(cx-w/2,cy-h/2,cx+w/2,cy+h/2)
    ring(im,b,13,[(0,P['dark']),(.4,P['navy']),(1,P['dark'])])
    top=Image.new('RGBA',im.size)
    ring(top,(b[0]+2,b[1]+2,b[2]-2,b[3]-2),3.5,[(0,P['gold']),(.45,P['mid']),(1,P['shade'])])
    clip=Image.new('L',im.size);ImageDraw.Draw(clip).rectangle(box((b[0]-1,b[1]-1,b[2]+1,cy-2)),fill=255)
    top.putalpha(ImageChops.multiply(top.getchannel('A'),clip));im.alpha_composite(top)
    d=ImageDraw.Draw(im);d.line(box((cx-w*.25,b[1]+3,cx+w*.12,b[1]+3)),fill=P['light'],width=S)
def halo(im):
    im=im.resize((im.width//S,im.height//S),R)
    a=im.getchannel('A');a=a.point(lambda v:0 if v<6 else v);im.putalpha(a)
    rim=a.filter(ImageFilter.MaxFilter(9))
    result=Image.new('RGBA',im.size,P['cream']);result.putalpha(rim);result.alpha_composite(im)
    return result
# Render several periods before cropping so off-edge links retain their full halo.
strip=canvas((128*7,64))
for period in range(-1,8):
    steel(strip,period*128+96,32,80,24)
    steel(strip,period*128+32,32)
chain=halo(strip).crop((128*3,0,128*4,64))
# Equal RGBA border samples prevent filtering seams in repeat import modes.
for y in range(64):
    a=chain.getpixel((0,y));b=chain.getpixel((127,y));v=tuple(round((x+z)/2) for x,z in zip(a,b))
    chain.putpixel((0,y),v);chain.putpixel((127,y),v)
chain.save(OUT/'chain-link-v3.png',optimize=True)
lock=canvas((160,160))
ring(lock,(49,29,111,100),13,[(0,P['light']),(.24,P['gold']),(.65,P['mid']),(1,P['shade'])])
steel(lock,80,27,91,44)
def ellipse(b,anchors):
    mask=Image.new('L',lock.size);ImageDraw.Draw(mask).ellipse(box(b),fill=255)
    g=gradient((lock.width,round((b[3]-b[1])*S)+1),anchors);full=Image.new('RGBA',lock.size);full.paste(g,(0,round(b[1]*S)))
    lock.alpha_composite(Image.composite(full,Image.new('RGBA',lock.size),mask))
ellipse((27,57,133,155),[(0,P['gold']),(.5,P['mid']),(1,P['shade'])])
ellipse((30,56,130,149),[(0,P['light']),(.2,P['gold']),(.58,P['mid']),(1,P['shade'])])
ellipse((40,67,120,140),[(0,P['navy']),(.22,P['teal']),(1,P['dark'])])
ellipse((44,71,116,136),[(0,P['teal']),(1,P['dark'])])
d=ImageDraw.Draw(lock)
d.arc(box((33,59,127,146)),195,315,fill=P['light'],width=3*S)
d.ellipse(box((69,88,91,110)),fill=P['navy'])
d.polygon([tuple(round(z*S) for z in p) for p in [(76,104),(84,104),(89,122),(71,122)]],fill=P['navy'])
d.ellipse(box((71,88,89,106)),fill=P['gold'])
d.polygon([tuple(round(z*S) for z in p) for p in [(77,101),(83,101),(86,119),(74,119)]],fill=P['gold'])
lock=halo(lock);lock.save(OUT/'chain-lock-v3.png',optimize=True)
old=Image.open(protected[0]).convert('RGBA');old_lock=Image.open(protected[1]).convert('RGBA')
font=lambda n:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf',n)
preview=Image.new('RGBA',(1120,1320),P['navy']);draw=ImageDraw.Draw(preview)
draw.text((28,20),'ZİNCİR V3 — RESİMLİ TAHTADA OKUNURLUK',font=font(27),fill=P['cream'])
draw.text((28,62),'Aynı tahta ve aynı sprite yüksekliği · 6×6 hücre · çapraz yerleşim',font=font(16),fill=P['cream'])
ink=chain.getchannel('A').getbbox();ink_height=ink[3]-ink[1]
cell=500/6; sprite_height=round(cell*.19*64/ink_height)
def diagonal(board,tile,endpoint):
    tile=tile.resize((sprite_height*2,sprite_height),R)
    length=math.hypot(352, -328)
    belt=Image.new('RGBA',(math.ceil(length)+tile.width,sprite_height))
    for x in range(0,belt.width,tile.width):belt.alpha_composite(tile,(x,0))
    belt=belt.crop((0,0,round(length),sprite_height)).rotate(math.degrees(math.atan2(328,352)),Image.Resampling.BICUBIC,expand=True)
    # The strip centre runs between these two anchors in board coordinates.
    board.alpha_composite(belt,(round(250-belt.width/2),round(250-belt.height/2)))
    scale=54/endpoint.width; ep=endpoint.resize((54,round(endpoint.height*scale)),R)
    attach=(.5,.17) if endpoint is lock else (.5,.15)
    for x,y in [(74,414),(426,86)]:board.alpha_composite(ep,(round(x-ep.width*attach[0]),round(y-ep.height*attach[1])))
for row,n in enumerate((40,24)):
    y=116+row*590
    for col,(tile,ep,label) in enumerate(((old,old_lock,'ESKİ'),(chain,lock,'V3 — LACİVERT + KREM HALE'))):
        x=28+col*556
        draw.text((x,y),f'{label}  ·  TABLO {n}',font=font(18),fill=P['cream'])
        board=Image.open(ROOT/f'Assets/Art/Paintings/painting-{n}.jpg').convert('RGBA').resize((500,500),R)
        bd=ImageDraw.Draw(board)
        for k in range(1,6):
            q=round(k*500/6);bd.line((q,0,q,500),fill=(11,42,54,120),width=1);bd.line((0,q,500,q),fill=(11,42,54,120),width=1)
        diagonal(board,tile,ep);preview.alpha_composite(board,(x,y+34))
draw.text((28,1280),'V3 görünen bant: hücrenin yaklaşık %19’u. Kilitler zincir bağlantı noktasından hizalandı.',font=font(16),fill=P['cream'])
preview.convert('RGB').save(OUT/'preview-chain.png',optimize=True)
detail=Image.new('RGBA',(1024,290),P['navy']);dd=ImageDraw.Draw(detail)
dd.text((20,12),'V3 · KOYU / AÇIK ZEMİNDE TEKRAR / KİLİT',font=font(20),fill=P['cream'])
for i in range(6):detail.alpha_composite(chain,(20+i*128,70))
detail.alpha_composite(lock,(820,54))
dd.rectangle((20,164,788,252),fill=P['cream'])
for i in range(6):detail.alpha_composite(chain,(20+i*128,176))
detail.save(OUT/'preview-repeat.png')
assert chain.size==(128,64) and lock.size==(160,160)
assert all(chain.getpixel((0,y))==chain.getpixel((127,y)) for y in range(64))
assert chain.getpixel((32,32))[3]==0, 'Link opening must remain transparent'
assert lock.getpixel((0,159))[3]==0
assert before=={str(p.relative_to(ROOT)):sha(p) for p in protected}
files=[]
for name,use in [('chain-link-v3.png','Tekrarlanan zincir bandı; X repeat, Y clamp'),('chain-lock-v3.png','Zincir uç kilidi; zincir kilidin üst halkasına bağlanır')]:
    im=Image.open(OUT/name)
    files.append(dict(file=name,use=use,size=list(im.size),pivot=[.5,.5],sha256=sha(OUT/name),alphaBBox=im.getchannel('A').getbbox()))
report=dict(status='passed',files=files,palette=P,method='Editable geometry adapted from existing SVG chain/lock family; Python/Pillow supersampled native drawing',haloPixels=4,repeatBorderMismatchedPixels=0,chainInkHeight=ink_height,targetInkFractionOfCell=.19,canvasHeightFractionOfCell=.19*64/ink_height,repeatPitchFractionOfCell=2*.19*64/ink_height,lockAttachmentUnity=[.5,1-27/160],previewSpriteHeight=sprite_height,previewCell=cell,oldAndNewSameCanvasHeight=True,protected=before,protectedMismatches=0,preview='preview-chain.png',runtimeIntegration=False)
(OUT/'manifest.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
(ROOT/'docs/art/reports/task17-validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(json.dumps({k:report[k] for k in ['status','repeatBorderMismatchedPixels','chainInkHeight','canvasHeightFractionOfCell','protectedMismatches']}))
