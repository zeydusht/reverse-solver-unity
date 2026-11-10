from pathlib import Path
import sys, math, json, hashlib
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2]
# Reuse only the native geometry helpers; never run the base asset exporter.
source=(ROOT/'docs/art/build-task17.py').read_text(encoding='utf-8')
exec(compile(source.split('# Render several periods')[0], 'task17-geometry-helpers', 'exec'))
base_palette=P.copy()
approved=[p for p in OUT.glob('*.png') if p.name in ['chain-link-v3.png','chain-lock-v3.png','preview-chain.png','preview-repeat.png']]
before={str(p.relative_to(ROOT)):sha(p) for p in approved+protected}
def blend(a,b,q):return '#'+''.join(f'{round(x*(1-q)+y*q):02X}' for x,y in zip(color(a),color(b)))
families=[
 ('altin','Altın','Yıldız','#FBBA0E','#F38A03','#CE5D01','#FFF1C2'),
 ('turkuaz','Turkuaz','Damla','#1C7F8C','#1C7F8C','#0F4F5A',blend('#1C7F8C','#F8EBD0',.45)),
 ('mor','Mor','Ay','#AF77E8','#7546B1','#3D2858','#AF77E8'),
 ('mercan','Mercan','Kalp','#E8574A','#E8574A','#94160F',blend('#E8574A','#F8EBD0',.35)),
]
def disc(im,b,stops):
    m=Image.new('L',im.size);ImageDraw.Draw(m).ellipse(box(b),fill=255)
    g=gradient((im.width,round((b[3]-b[1])*S)+1),stops);f=Image.new('RGBA',im.size);f.paste(g,(0,round(b[1]*S)))
    im.alpha_composite(Image.composite(f,Image.new('RGBA',im.size),m))
def glyph(im,kind,fill):
    m=Image.new('L',im.size);d=ImageDraw.Draw(m)
    if kind=='altin':
        pts=[]
        for i in range(10):
            a=-math.pi/2+i*math.pi/5;r=23 if i%2==0 else 10
            pts.append(((80+math.cos(a)*r)*S,(106+math.sin(a)*r)*S))
        d.polygon(pts,fill=255)
    elif kind=='turkuaz':
        pts=[(80,81)]
        for i in range(49):
            a=math.pi+i*math.pi/48
            pts.append((80+18*math.cos(a),109-18*math.sin(a)))
        d.polygon([(x*S,y*S) for x,y in pts],fill=255)
    elif kind=='mor':
        d.ellipse(box((57,82,103,128)),fill=255)
        d.ellipse(box((72,76,113,117)),fill=0)
    else:
        pts=[]
        for i in range(160):
            a=i*math.tau/160;x=16*math.sin(a)**3
            y=13*math.cos(a)-5*math.cos(2*a)-2*math.cos(3*a)-math.cos(4*a)
            pts.append(((80+x*1.4)*S,(104-y*1.4)*S))
        d.polygon(pts,fill=255)
    rim=m.filter(ImageFilter.MaxFilter(17))
    layer=Image.new('RGBA',im.size,base_palette['cream']);layer.putalpha(rim);im.alpha_composite(layer)
    layer=Image.new('RGBA',im.size,fill);layer.putalpha(m);im.alpha_composite(layer)
assets={};records=[]
for name,label,shape,bright,mid,shade,light in families:
    P.update(dark=shade,navy=mid,gold=bright,mid=mid,shade=shade,light=light)
    strip=canvas((128*7,64))
    for period in range(-1,8):
        steel(strip,period*128+96,32,80,24);steel(strip,period*128+32,32)
    chain=halo(strip).crop((384,0,512,64))
    for y in range(64):
        a=chain.getpixel((0,y));b=chain.getpixel((127,y));v=tuple(round((x+z)/2) for x,z in zip(a,b))
        chain.putpixel((0,y),v);chain.putpixel((127,y),v)
    lock=canvas((160,160))
    ring(lock,(49,29,111,100),13,[(0,light),(.24,bright),(.65,mid),(1,shade)])
    steel(lock,80,27,91,44)
    disc(lock,(27,57,133,155),[(0,bright),(.5,mid),(1,shade)])
    disc(lock,(30,56,130,149),[(0,light),(.2,bright),(.58,mid),(1,shade)])
    disc(lock,(40,67,120,140),[(0,base_palette['navy']),(1,base_palette['dark'])])
    ImageDraw.Draw(lock).arc(box((33,59,127,146)),195,315,fill=light,width=3*S)
    glyph(lock,name,bright if name in ('altin','mor','mercan') else light)
    lock=halo(lock)
    for role,im in [('link',chain),('lock',lock)]:
        filename=f'chain-{role}-v3-{name}.png';im.save(OUT/filename,optimize=True)
        assert im.size==((128,64) if role=='link' else (160,160))
        assert im.mode=='RGBA' and im.getpixel((0,0))[3]==0 if role=='lock' else im.mode=='RGBA'
        records.append(dict(file=filename,color=name,shape=shape if role=='lock' else None,size=list(im.size),pivot=[.5,.5],sha256=sha(OUT/filename)))
    assert all(chain.getpixel((0,y))==chain.getpixel((127,y)) for y in range(64))
    assert chain.getpixel((32,32))[3]==0
    assert ImageChops.difference(chain.getchannel('A'),Image.open(OUT/'chain-link-v3.png').getchannel('A')).getbbox() is None
    assert ImageChops.difference(lock.getchannel('A'),Image.open(OUT/'chain-lock-v3.png').getchannel('A')).getbbox() is None
    assets[name]=(chain,lock)
P.update(base_palette)
font=lambda n:ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf',n)
preview=Image.new('RGBA',(1120,1060),P['navy']);d=ImageDraw.Draw(preview)
d.text((28,18),'ZİNCİR V3 · DÖRT RENK, DÖRT ŞEKİL',font=font(27),fill=P['cream'])
d.text((28,61),'Aynı tahtada iki farklı çift · krem hale · hücrenin yaklaşık %19 kalınlığı',font=font(16),fill=P['cream'])
for i,(name,label,shape,*_) in enumerate(families):
    x=28+i*274;chain,lock=assets[name]
    preview.alpha_composite(lock,(x+60,96));d.text((x+28,266),f'{label} · {shape}',font=font(18),fill=P['cream'])
    for j in range(2):preview.alpha_composite(chain.resize((112,56),R),(x+j*112,307))
def route(board,chain,lock,a,b):
    h=18;tile=chain.resize((h*2,h),R);length=math.dist(a,b)
    strip=Image.new('RGBA',(math.ceil(length)+tile.width,h))
    for x in range(0,strip.width,tile.width):strip.alpha_composite(tile,(x,0))
    angle=-math.degrees(math.atan2(b[1]-a[1],b[0]-a[0]))
    belt=strip.crop((0,0,round(length),h)).rotate(angle,Image.Resampling.BICUBIC,expand=True)
    cx=(a[0]+b[0])/2;cy=(a[1]+b[1])/2
    board.alpha_composite(belt,(round(cx-belt.width/2),round(cy-belt.height/2)))
    ep=lock.resize((64,64),R)
    for x,y in (a,b):board.alpha_composite(ep,(round(x-32),round(y-27/160*64)))
for col,(painting,names) in enumerate([(40,('altin','turkuaz')),(24,('mor','mercan'))]):
    x=28+col*556;y=430
    d.text((x,y-36),f'TABLO {painting} · '+('ALTIN + TURKUAZ' if col==0 else 'MOR + MERCAN'),font=font(20),fill=P['cream'])
    board=Image.open(ROOT/f'Assets/Art/Paintings/painting-{painting}.jpg').convert('RGBA').resize((500,500),R)
    bd=ImageDraw.Draw(board)
    for k in range(1,6):
        q=round(k*500/6);bd.line((q,0,q,500),fill=(11,42,54,120));bd.line((0,q,500,q),fill=(11,42,54,120))
    route(board,*assets[names[0]],(65,352),(432,85))
    route(board,*assets[names[1]],(70,85),(432,352))
    preview.alpha_composite(board,(x,y))
d.text((28,966),'Eşleşme: renk + kilit şekli. İki uçta aynı işaret bulunur.',font=font(19),fill=P['cream'])
d.text((28,1000),'Yıldız / damla / ay / kalp · onaylı v3 kalınlığı, dış hale ve bağlantı noktası korunur.',font=font(16),fill=P['cream'])
preview.convert('RGB').save(OUT/'preview-chain-colors.png',optimize=True)
assert before=={str(p.relative_to(ROOT)):sha(p) for p in approved+protected}
report=dict(status='passed',files=records,haloPixels=4,repeatBorderMismatchedPixels={n:0 for n,*_ in families},approvedAlphaGeometryUnchanged=True,protected=before,protectedMismatches=0,shapeEncoding={'altin':'star','turkuaz':'drop','mor':'crescent','mercan':'heart'},palette='docs/art/PALETTE.md — Task17 colored pairs',preview='preview-chain-colors.png',sameBoardPairCounts=[2,2],lockAttachmentUnity=[.5,.83125],runtimeIntegration=False)
(OUT/'manifest-colors.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
(ROOT/'docs/art/reports/task17-colors-validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('8 RGBA assets exported; all 4 repeat seams match; approved v3 and prior assets unchanged.')
