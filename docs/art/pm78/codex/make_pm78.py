"""PIL-only image IO/geometry/compositing; NumPy pixel arithmetic. No game changes."""
from pathlib import Path
import json
import numpy as np
from PIL import Image, ImageChops, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[4]
DOC = ROOT / 'docs/art/pm78/codex'
INPUT = ROOT / 'Assets/Art/_yeni/PM78/girdi'
OUT = ROOT / 'Assets/Art/_yeni/PM78/oda'
OUT.mkdir(parents=True, exist_ok=True)
SIZE = (1408, 1728)
BOX = (640, 256, 1024, 1312)

def rgb(path):
    return Image.open(path).convert('RGB')

def resize_float(a, size):
    return np.stack([np.asarray(Image.fromarray(a[..., c]).resize(size, Image.Resampling.BILINEAR)) for c in range(3)], axis=2)

def seamless(target, source, mask):
    """Harmonic boundary color correction, confined strictly to the supplied mask."""
    target = np.asarray(target, dtype=np.float32)
    source = np.asarray(source, dtype=np.float32)
    m = np.asarray(mask) > 127
    yy, xx = np.where(m)
    x0, x1 = max(0, xx.min()-4), min(m.shape[1], xx.max()+5)
    y0, y1 = max(0, yy.min()-4), min(m.shape[0], yy.max()+5)
    t, s = target[y0:y1, x0:x1], source[y0:y1, x0:x1]
    mm = m[y0:y1, x0:x1]
    fixed = t-s
    # Coarse-to-fine Dirichlet solve, preserving the generated texture/gradients.
    u = None
    for factor in (16, 8, 4, 2, 1):
        wh = (max(4, int(np.ceil(mm.shape[1]/factor))), max(4, int(np.ceil(mm.shape[0]/factor))))
        f = resize_float(fixed, wh)
        pm = np.asarray(Image.fromarray(mm.astype('uint8')*255).resize(wh, Image.Resampling.NEAREST)) > 127
        pm[[0, -1], :] = False
        pm[:, [0, -1]] = False
        if u is None:
            u = f.copy()
        else:
            u = resize_float(u, wh)
        u[~pm] = f[~pm]
        for _ in range(450 if factor == 1 else 220):
            avg = (u[:-2, 1:-1] + u[2:, 1:-1] + u[1:-1, :-2] + u[1:-1, 2:])*.25
            mid = u[1:-1, 1:-1]
            use = pm[1:-1, 1:-1]
            mid[use] = avg[use]
    result = target.copy()
    local = np.clip(np.rint(s+u), 0, 255)
    result[y0:y1, x0:x1][mm] = local[mm]
    return Image.fromarray(result.astype('uint8'), 'RGB')

def mask(name):
    return Image.open(INPUT / f'maske_{name}.png').convert('L').point(lambda v: 255 if v >= 128 else 0).filter(ImageFilter.MaxFilter(7))

def floor_coords(x, y):
    # Align both straight carpet borders to their original lines, including their thickness.
    far = 1378 - .134*x
    upper = 1868 - .445*x
    lower = 2014 - .510*x
    srcupper = 1824 - .385*x
    srclower = 2000 - .480*x
    if y < far:
        sy = y
    elif y < upper:
        sy = far + (y-far)*(srcupper-far)/(upper-far)
    elif y < lower:
        sy = srcupper + (y-upper)*(srclower-srcupper)/(lower-upper)
    else:
        sy = srclower + (y-lower)*.92
    return (x, sy-1056)

def warped_floor(im):
    mesh = []
    for y in range(1056, 1728, 16):
        for x in range(0, 1408, 16):
            x1, y1 = min(x+16, 1408), min(y+16, 1728)
            quad = sum((floor_coords(px, py) for px, py in ((x,y), (x,y1), (x1,y1), (x1,y))), ())
            mesh.append(((x,y-1056,x1,y1-1056), quad))
    return im.transform((1408,672), Image.Transform.MESH, mesh, Image.Resampling.BICUBIC)

def object_fill(im, kind):
    im = im.resize((384,1056), Image.Resampling.LANCZOS)
    mesh = []
    def at(x,y):
        gx, gy = x+640, y+256
        if kind == 'stone':
            gx = float(np.interp(gx, [640,782,854,870,1024], [640,782,843,860,1024]))
        else:
            desired = 1110-(gx-780)*.34
            actual = 1118+(gx-780)*.04
            shift = actual-desired
            amount = float(np.interp(gy, [256,950,desired,1258,1312], [0,0,1,0,0]))
            gy += shift*amount
            if 315 <= gy < 355:
                gy=315+(gy-315)*2.5
            elif 355 <= gy < 450:
                gy=415+(gy-355)*(35/95)
        return gx-640, gy-256
    for y in range(0,1056,8):
        for x in range(0,384,8):
            x1,y1=min(x+8,384),min(y+8,1056)
            q=sum((at(px,py) for px,py in ((x,y),(x,y1),(x1,y1),(x1,y))),())
            mesh.append(((x,y,x1,y1),q))
    result=im.transform((384,1056),Image.Transform.MESH,mesh,Image.Resampling.BICUBIC)
    if kind == 'wall':
        ar=np.asarray(result).copy()
        source=np.asarray(im)
        for y in range(450-256,1040-256):
            for x in range(138,226):
                p=ar[y,x].astype(float)
                if p[0]>1.18*p[1] and p[0]>1.4*p[2]:
                    ar[y,x]=source[y,max(82,x-90)]
        result=Image.fromarray(ar,'RGB')
    return result

def boundary_finish(result, target, m, width=12):
    a=np.asarray(result,dtype=np.float32).copy()
    t=np.asarray(target,dtype=np.float32)
    mm=np.asarray(m)>127
    for y in np.where(mm.any(axis=1))[0]:
        xs=np.where(mm[y])[0]
        left,right=int(xs[0]),int(xs[-1])
        for edge, direction in ((left,1),(right,-1)):
            outside=edge-direction
            near=outside-direction
            gradient=np.clip(t[y,outside]-t[y,near],-4,4)
            for k in range(min(width,len(xs)//2)):
                x=edge+direction*k
                if not mm[y,x]:continue
                weight=(1-k/width)**2
                predicted=t[y,outside]+gradient*min(k+1,4)
                a[y,x]=a[y,x]*(1-weight)+predicted*weight
    return Image.fromarray(np.clip(np.rint(a),0,255).astype('uint8'),'RGB')

def top_extension(im):
    im=im.resize((1024,512),Image.Resampling.LANCZOS)
    tx=[0,22,104,162,688,776,799,972,1024]
    sx=[0,22,104,158,706,781,800,961,1024]
    mesh=[]
    for x in range(0,1024,8):
        x1=min(x+8,1024)
        s0,s1=float(np.interp(x,tx,sx)),float(np.interp(x1,tx,sx))
        mesh.append(((x,0,x1,128),(s0,0,s0,128,s1,128,s1,0)))
    return im.transform((1024,128),Image.Transform.MESH,mesh,Image.Resampling.BICUBIC)

def right_extension(im, desired):
    im=im.resize((768,1344),Image.Resampling.LANCZOS)
    # Local registration of the cropped arch ribs. Shafts below y575 stay vertical.
    rows=[0,128,170,244,254,264,294,295,320,350,380,420,450,480,510,522,530,535,550,558,575,1344]
    positions=[384,384,384,382,375,384,384,380,385,370,384,384,420,436,437,421,420,440,440,420,384,384]
    mesh=[]
    def at(x,y):
        shift=float(np.interp(y,rows,positions))-384
        return x+shift*max(0,1-(x-384)/192),y
    for y in range(0,1344,4):
        for x in range(384,768,16):
            x1,y1=min(x+16,768),min(y+4,1344)
            q=sum((at(px,py) for px,py in ((x,y),(x,y1),(x1,y1),(x1,y))),())
            mesh.append(((x-384,y,x1-384,y1),q))
    return im.transform((384,1344),Image.Transform.MESH,mesh,Image.Resampling.BICUBIC)
    raw=np.asarray(im,dtype=np.float32)
    options=np.arange(328,441)
    rows=np.arange(0,1344,4)
    colors=raw[rows[:,None],options[None,:]]
    cost=((colors-desired[rows,None,:])**2).sum(axis=2)+1.4*(options[None,:]-384)**2
    dp=cost[0].copy()
    parent=np.zeros(cost.shape,dtype=np.int16)
    for i in range(1,len(rows)):
        best=np.full(len(options),np.inf,dtype=np.float32)
        for shift in range(-4,5):
            previous=np.arange(len(options))+shift
            ok=(previous>=0)&(previous<len(options))
            v=np.full(len(options),np.inf,dtype=np.float32)
            v[ok]=dp[previous[ok]]+130*abs(shift)+25*shift*shift
            update=v<best
            best[update]=v[update]
            parent[i,update]=previous[update]
        dp=cost[i]+best
    index=int(dp.argmin())
    path=np.zeros(len(rows),dtype=np.float32)
    for i in range(len(rows)-1,-1,-1):
        path[i]=options[index]-384
        index=int(parent[i,index])
    path=np.interp(np.arange(1344),rows,path)
    path=np.asarray(Image.fromarray(path[:,None].astype('float32')).resize((1,168),Image.Resampling.BILINEAR).resize((1,1344),Image.Resampling.BILINEAR))[:,0]
    mesh=[]
    def at(x,y):
        shift=float(path[min(1343,int(y))])
        return x+shift*(1-(x-384)/384),y
    for y in range(0,1344,8):
        for x in range(384,768,16):
            x1,y1=min(x+16,768),min(y+8,1344)
            q=sum((at(px,py) for px,py in ((x,y),(x,y1),(x1,y1),(x1,y))),())
            mesh.append(((x-384,y,x1-384,y1),q))
    return im.transform((384,1344),Image.Transform.MESH,mesh,Image.Resampling.BICUBIC)

original = rgb(ROOT / 'Assets/_Game/Presentation/Resources/menu_bg.png')
canvas = rgb(DOC / 'genis_uretim.png').transform(SIZE, Image.Transform.AFFINE, (.695,0,0,0,.73,105), Image.Resampling.BICUBIC)
a = np.asarray(canvas, dtype=np.float32).copy()
top = np.asarray(top_extension(rgb(DOC/'ust_uretim.png')),dtype=np.float32)
desired=np.concatenate((top[:128,-1],np.asarray(original,dtype=np.float32)[:1216,-1]),axis=0)
right=np.asarray(right_extension(rgb(DOC/'sag_uretim.png'),desired),dtype=np.float32)
a[:1344,1024:] = right
alternative=np.asarray(canvas,dtype=np.float32)
for y in range(790,1110):
    w=float(np.interp(y,[790,850,1020,1110],[0,1,1,0]))
    a[y,1024:]=a[y,1024:]*(1-w)+alternative[y,1024:]*w
a[:128,:1024] = top[:128]
floor = np.asarray(warped_floor(rgb(DOC / 'zemin_uretim.png').resize((1408,672), Image.Resampling.LANCZOS)), dtype=np.float32)
o = np.asarray(original, dtype=np.float32)
# Match the low frequency palette of each generated extension to the source.
def smooth_profile(v, radius=12):
    shaped=v.reshape((len(v),1,3))
    return np.asarray(Image.fromarray(np.clip(shaped+128,0,255).astype('uint8')).filter(ImageFilter.BoxBlur(radius)),dtype=np.float32).reshape((-1,3))-128
floor_delta=(o[1056-128:,-1]-floor[:608,1024])
red=(o[928:,-1,0]>2*o[928:,-1,1])&(floor[:608,1024,0]>2*floor[:608,1024,1])
valid=np.where(red)[0]
for c in range(3):floor_delta[:,c]=np.interp(np.arange(608),valid,floor_delta[valid,c])
floor_delta=smooth_profile(floor_delta,10)
for x in range(1408):
    y=np.arange(1056,1728,dtype=float)
    yy=np.interp(y,[1056,1378-.134*x,1868-.445*x,2014-.510*x,2200],[1056,1378-.134*1024,1868-.445*1024,2014-.510*1024,2200])
    red_use=(floor[:,x,0]>1.9*floor[:,x,1])&(y>1378-.134*x+14)&(y<1868-.445*x-8)
    for c in range(3):floor[red_use,x,c]+=np.interp(yy[red_use],np.arange(1056,1664),floor_delta[:,c])
floor_im=Image.fromarray(np.clip(floor,0,255).astype('uint8'))
soft=np.asarray(floor_im.filter(ImageFilter.GaussianBlur(1.4)),dtype=np.float32)
for k in range(672):
    z=np.clip((k-180)/400,0,.8)
    floor[k]=floor[k]*(1-z)+soft[k]*z
for y in range(1056,1728):
    alpha = np.clip((y-1100)/100, 0, 1)
    a[y] = a[y]*(1-alpha) + floor[y-1056]*alpha
# Exact edge color matching is restricted to the ADDED side, at most 24 px.
# Material-wide color grading avoids extending edge misregistration into stripes.
top_delta=o[0]-a[127,:1024]
teal=(o[0,:,0]<o[0,:,1])&(a[127,:1024,0]<a[127,:1024,1])
warm=(o[0,:,0]>o[0,:,1]*1.18)&(a[127,:1024,0]>a[127,:1024,1]*1.18)
for material,where in ((teal,a[:128,:1024,0]<a[:128,:1024,1]),(warm,a[:128,:1024,0]>a[:128,:1024,1]*1.18)):
    grade=np.median(top_delta[material],axis=0)
    a[:128,:1024][where]+=grade
bottom_delta=o[-1]-a[1664,:1024]
red=(o[-1,:,0]>2*o[-1,:,1])&(a[1664,:1024,0]>2*a[1664,:1024,1])
marble=(o[-1,:,1]>.58*o[-1,:,0])&(a[1664,:1024,1]>.58*a[1664,:1024,0])
for material,where in ((red,a[1664:,:1024,0]>2*a[1664:,:1024,1]),(marble,a[1664:,:1024,1]>.58*a[1664:,:1024,0])):
    if material.any():a[1664:,:1024][where]+=np.median(bottom_delta[material],axis=0)
delta_top=o[0]-a[127,:1024]
for k in range(24):
    w = (1-k/24)**2 * (1+2*k/24)
    a[127-k,:1024] += delta_top*w
delta = o[-1]-a[1664,:1024]
for k in range(24):
    w = (1-k/24)**2 * (1+2*k/24)
    a[1664+k,:1024] += delta*w
delta = o[:,-1]-a[128:1664,1024]
for k in range(24):
    w = (1-k/24)**2 * (1+2*k/24)
    a[128:1664,1024+k] += delta*w
wide = Image.fromarray(np.clip(np.rint(a),0,255).astype('uint8'), 'RGB')
wide.paste(original, (0,128))
wide.save(OUT / 'oda_genis.png')

objects = ImageChops.lighter(mask('flama'), mask('aplik'))
column = mask('sutun')
fill = wide.copy()
fill.paste(object_fill(rgb(DOC/'flamasiz_uretim.png'),'stone'), BOX[:2])
no_banner = seamless(wide, fill, objects)
no_banner.save(OUT/'oda_flamasiz.png')
fill = no_banner.copy()
fill.paste(object_fill(rgb(DOC/'temiz_uretim.png'),'wall'), BOX[:2])
clean = seamless(no_banner, fill, column)
clean = boundary_finish(clean, no_banner, column)
clean.save(OUT/'oda_temiz.png')

preview = wide.resize((704,864), Image.Resampling.LANCZOS)
ImageDraw.Draw(preview).rectangle((0,64,511,831), outline=(90,220,255), width=1)
preview.save(DOC/'kontrol_genis.png')
sheet = Image.new('RGB', (616,1036), (28,34,38))
draw = ImageDraw.Draw(sheet)
draw.text((8,8), 'oda_flamasiz', fill='white')
draw.text((316,8), 'oda_temiz', fill='white')
sheet.paste(no_banner.crop((700,300,1000,1300)),(4,30))
sheet.paste(clean.crop((700,300,1000,1300)),(312,30))
sheet.save(DOC/'kontrol_temiz.png')

ims = {'oda_genis.png': wide, 'oda_flamasiz.png': no_banner, 'oda_temiz.png': clean}
arrays = {n: np.asarray(i).astype('int16') for n,i in ims.items()}
orig = np.asarray(original).astype('int16')
union = np.maximum(np.asarray(objects), np.asarray(column)) > 127
def maxdiff(a,b,where=None):
    d = np.abs(a-b)
    return int((d[where] if where is not None else d).max(initial=0))
report = {
    'dosyalar': {n: {'boyut_px': list(im.size), 'kip': im.mode, 'bayt': (OUT/n).stat().st_size} for n,im in ims.items()},
    'oda_genis_orijinal_bolge_menu_bg_en_buyuk_piksel_farki': maxdiff(arrays['oda_genis.png'][128:1664,:1024],orig),
    'orijinal_bolgede_izinli_silmeler_disinda_menu_bg_farki': {
        'oda_flamasiz.png': maxdiff(arrays['oda_flamasiz.png'][128:1664,:1024],orig, ~(np.asarray(objects)[128:1664,:1024]>127)),
        'oda_temiz.png': maxdiff(arrays['oda_temiz.png'][128:1664,:1024],orig, ~union[128:1664,:1024])},
    'maske_disinda_dosyalar_arasi_en_buyuk_fark': {
        'genis_flamasiz': maxdiff(arrays['oda_genis.png'],arrays['oda_flamasiz.png'], ~(np.asarray(objects)>127)),
        'flamasiz_temiz': maxdiff(arrays['oda_flamasiz.png'],arrays['oda_temiz.png'], ~(np.asarray(column)>127)),
        'genis_temiz_birlesik_maske': maxdiff(arrays['oda_genis.png'],arrays['oda_temiz.png'], ~union)},
    'magenta_FF00FF_arti_eksi_40_piksel_sayisi': {n:int(np.all(np.abs(a-np.array([255,0,255]))<=40,axis=2).sum()) for n,a in arrays.items()},
    'maske_genisletme_px': 3,
    'maske_genisletme_yontemi': 'PIL MaxFilter(7), beyaz >=128; Chebyshev 3 px',
    'orijinal_bolge': [0,128,1024,1664],
    'not': 'Orijinal bolge sifir fark kosulu oda_genis icindir. Diger iki dosyada sadece istenen silme maskeleri bu bolgeyi degistirir.'
}
checks = [report['oda_genis_orijinal_bolge_menu_bg_en_buyuk_piksel_farki'] == 0,
          all(v == 0 for v in report['orijinal_bolgede_izinli_silmeler_disinda_menu_bg_farki'].values()),
          all(v == 0 for v in report['maske_disinda_dosyalar_arasi_en_buyuk_fark'].values()),
          all(v == 0 for v in report['magenta_FF00FF_arti_eksi_40_piksel_sayisi'].values()),
          all(i.size == SIZE and i.mode == 'RGB' for i in ims.values())]
report['tum_kontroller_gecti'] = all(checks)
(DOC/'dogrulama.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
assert all(checks), report
print(json.dumps(report,ensure_ascii=True))
