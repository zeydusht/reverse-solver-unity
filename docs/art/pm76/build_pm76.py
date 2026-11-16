"""PM76: reference-aligned layer extraction, sizing, metadata and review boards.

Image synthesis is performed by the built-in imagegen tool; this script only
separates/composites its outputs and the unchanged project references.
Run with Python -B. Writes exclusively to the two PM76 delivery directories.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageOps
import numpy as np
import json, hashlib, shutil

ROOT = Path(__file__).resolve().parents[3]
DOC = ROOT / 'docs/art/pm76'
SRC = DOC / 'sources'
OUT = ROOT / 'Assets/Art/_yeni/PM76'
L = Image.Resampling.LANCZOS
SIZE = (1180, 1770)
QUAD0 = [(349,296),(662,359),(663,832),(348,815)]
FONT = Path('C:/Windows/Fonts/arial.ttf')

def load(path):
    return Image.open(path).convert('RGBA')

def save(im, path):
    path.parent.mkdir(parents=True, exist_ok=True)
    im.save(path, compress_level=6)

def polygon(size, pts):
    m = Image.new('L', size)
    ImageDraw.Draw(m).polygon([tuple(p) for p in pts], fill=255)
    return m

def foreground_colors(im):
    a = np.asarray(im.convert('RGB')).astype(float)
    r,g,b = a[:,:,0], a[:,:,1], a[:,:,2]
    # Gold/stone/red plus foliage; teal wall is excluded. The cobalt vase is
    # included separately in the contiguous lower pillar/plinth silhouette.
    return ((r > 1.12*g) | (g > 1.40*b) |
            ((r>.85*g)&(g>.95*b)&(r>75))) & ((r+g+b)>50)

def a0_mask(im):
    w,h = im.size
    y,x = np.indices((h,w))
    m = foreground_colors(im) & (x<183) & (y<944)
    m |= (x<129) & (y<944)
    low = polygon(im.size, [(0,942),(141,942),(149,950),(149,997),
        (145,1003),(148,1130),(158,1141),(160,1229),(0,1258)])
    return Image.fromarray(np.maximum(m.astype('uint8')*255, np.asarray(low))).filter(ImageFilter.MaxFilter(5))

def corridor_mask(im):
    w,h = im.size
    y,x = np.indices((h,w))
    zones = (x<93) | ((x>687)&(x<970)) | (x>1582)
    m = foreground_colors(im) & zones & (y<1028)
    # Interior stone shafts are solid even in strongly shaded areas.
    m |= (((x>=744)&(x<=871)) | (x<10) | (x>1605)) & (y<1028)
    polys = [
      [(0,1006),(52,1006),(52,1197),(56,1204),(56,1266),(0,1266)],
      [(724,1004),(913,1004),(914,1195),(921,1203),(921,1263),
       (718,1263),(719,1203),(724,1195)],
      [(1592,1004),(1644,1004),(1644,1266),(1589,1266),
       (1589,1204),(1592,1195)]
    ]
    for pts in polys:
        m |= np.asarray(polygon(im.size,pts))>0
    return Image.fromarray(m.astype('uint8')*255).filter(ImageFilter.MaxFilter(5))

def angle_mask(im, cfg):
    w,h=im.size
    y,x=np.indices((h,w))
    edge=cfg['pillar_edge']
    m=foreground_colors(im) & (x<cfg.get('fg_limit',190)) & (y<edge)
    if cfg.get('solid_width',0):
        m |= (x<cfg['solid_width']) & (y<edge)
    m |= np.asarray(polygon(im.size,cfg['lower_outline']))>0
    return Image.fromarray(m.astype('uint8')*255).filter(ImageFilter.MaxFilter(5))

def clear_canvas(bg,quad):
    mask=polygon(bg.size,quad)
    a=np.asarray(bg).copy()
    a[np.asarray(mask)>0,3]=0
    # Zero RGB for fully transparent insert pixels, so mipmaps cannot expose
    # the old painting or charcoal placeholder through the opening.
    a[a[:,:,3]==0,:3]=0
    return Image.fromarray(a)

def harmonize(im,clean,mask):
    """Match imagegen's hidden plate to the unchanged boundary RGB.

    Extend the boundary color correction across each occluded scanline. This
    changes only hidden pixels; imagegen's texture/panelling remains intact.
    """
    a=np.asarray(im.convert('RGB')).astype(float)
    c=np.asarray(clean.convert('RGB')).astype(float)
    m=np.asarray(mask)>0
    correction=np.zeros_like(c)
    w,h=im.size
    occupied=m.any(axis=0)
    starts=np.flatnonzero(occupied & ~np.r_[False,occupied[:-1]])
    ends=np.flatnonzero(occupied & ~np.r_[occupied[1:],False])+1
    kernel=np.exp(-.5*(np.arange(-24,25)/8)**2);kernel/=kernel.sum()
    for gleft,gright in zip(starts,ends):
        dl=np.full((h,3),np.nan);dr=dl.copy();extents={}
        for y in range(h):
            xs=np.flatnonzero(m[y,gleft:gright])+gleft
            if not len(xs):continue
            left,right=int(xs[0]),int(xs[-1])+1
            extents[y]=(left,right)
            li=np.arange(max(0,left-28),max(0,left-12))
            ri=np.arange(min(w,right+12),min(w,right+28))
            li=li[~m[y,li]];ri=ri[~m[y,ri]]
            if len(li):dl[y]=(a[y,li]-c[y,li]).mean(axis=0)
            if len(ri):dr[y]=(a[y,ri]-c[y,ri]).mean(axis=0)
            if np.isnan(dl[y,0]):dl[y]=dr[y]
            if np.isnan(dr[y,0]):dr[y]=dl[y]
        for field in (dl,dr):
            for ch in range(3):
                valid=np.flatnonzero(~np.isnan(field[:,ch]))
                if not len(valid):field[:,ch]=0;continue
                v=np.interp(np.arange(h),valid,field[valid,ch])
                field[:,ch]=np.convolve(np.pad(v,(24,24),mode='edge'),kernel,mode='valid')
        for y,(left,right) in extents.items():
            t=np.linspace(0,1,right-left)[:,None]
            correction[y,left:right]=np.clip(dl[y]*(1-t)+dr[y]*t,-45,45)
    return Image.fromarray(np.clip(c+correction,0,255).round().astype('uint8')).convert('RGBA')

def split(im, mask, clean, quad=None, shadow_limit_y=963):
    # Unmatte antialiasing, cast shadows and candle glow against the generated
    # plate, with exact reconstruction of source RGB after integer compositing.
    a=np.asarray(im).copy()
    support=mask.filter(ImageFilter.MaxFilter(49))
    p=np.asarray(support).copy()
    cutoff=1027 if im.size==(1644,1688) else shadow_limit_y
    # Wall shadows/glow belong to the moving assembly. Keep the unchanged
    # floor and panelling outside the solid object's footprint out of fg.
    p[cutoff:]=np.asarray(mask)[cutoff:]
    support=Image.fromarray(p)
    if im.size==(1644,1688):
        p=np.asarray(support).copy()
        # Protect both original frame bays from the adjacent shadow support.
        p[:,110:668]=0;p[:,974:1558]=0
        support=Image.fromarray(p)
    m=np.asarray(support)>0
    clean=harmonize(im,clean,support)
    bg=a.copy(); bg[m,:3]=np.asarray(clean)[m,:3]; bg[:,:,3]=255
    source=a[:,:,:3].astype(float);base=bg[:,:,:3].astype(float)
    delta=source-base
    amin=np.max(np.where(delta>=0,delta/np.maximum(255-base,1),
                         -delta/np.maximum(base,1)),axis=2)
    core=mask.filter(ImageFilter.MinFilter(5))
    soft=np.asarray(core.filter(ImageFilter.GaussianBlur(.8))).astype(float)/255
    alpha=np.ceil(np.maximum(soft,amin)*255).clip(0,255).astype('uint8')
    alpha[np.asarray(core)==255]=255
    alpha[~m]=0
    aa=alpha.astype(float)/255
    rgb=(source-base*(1-aa[:,:,None]))/np.maximum(aa[:,:,None],1/255)
    fg=np.zeros_like(a);fg[:,:,:3]=rgb.round().clip(0,255).astype('uint8');fg[:,:,3]=alpha
    fg[alpha==0,:3]=0
    check=np.asarray(Image.alpha_composite(Image.fromarray(bg),Image.fromarray(fg)))
    bad=np.any(check[:,:,:3]!=a[:,:,:3],axis=2)
    fg[bad]=a[bad]
    fg,bg=Image.fromarray(fg),Image.fromarray(bg)
    if quad is not None:
        bg=clear_canvas(bg,quad)
    return fg,bg

def adjusted_plate(plate, im, cfg):
    # Piecewise alignment of imagegen's actual wall/wainscot/floor clean plate
    # with the new camera view. Only occluded foreground areas use this image.
    p=np.asarray(plate.convert('RGB')).astype(float)
    target=np.asarray(im.convert('RGB')).astype(float)
    shift=(target[400:650,190:235].mean(axis=(0,1))-
           p[400:650,190:235].mean(axis=(0,1)))
    p=np.clip(p+shift,0,255).astype('uint8')
    ys=np.interp(np.arange(1536),[0,cfg['wall_y'],cfg['floor_y'],1535],
                 [0,963,1178,1535])
    yi=np.clip(np.rint(ys).astype(int),0,1535)
    return Image.fromarray(p[yi]).convert('RGBA')

def homography(src,dst):
    mat=[]; vals=[]
    for (x,y),(u,v) in zip(src,dst):
        mat.extend([[x,y,1,0,0,0,-u*x,-u*y],[0,0,0,x,y,1,-v*x,-v*y]])
        vals.extend([u,v])
    return np.append(np.linalg.solve(np.array(mat),np.array(vals)),1).reshape(3,3)

def gold_mask(im,bounds):
    a=np.asarray(im.convert('RGB')).astype(float)
    r,g,b=a[:,:,0],a[:,:,1],a[:,:,2]
    m=(r>.90*g)&(g>1.17*b)&(r>75)
    region=Image.new('L',im.size)
    ImageDraw.Draw(region).rectangle(bounds,fill=255)
    return Image.fromarray((m & (np.asarray(region)>0)).astype('uint8')*255)

def reference_frame(original):
    alpha=gold_mask(original,[270,215,723,932])
    alpha=alpha.filter(ImageFilter.MaxFilter(3))
    a=np.asarray(alpha).copy()
    a[np.asarray(polygon(original.size,QUAD0))>0]=0
    frame=original.copy();frame.putalpha(Image.fromarray(a))
    save(frame,SRC/'reference-frame-rgba.png')
    return frame

def lock_frame_identity(im, cfg, canonical, wall):
    """The room is imagegen's newly drawn angle; its planar gold frame keeps
    the exact reference ornaments via a calibrated 2.5D wall-plane projection.
    """
    remove=gold_mask(im,cfg['generated_bounds'])
    remove=Image.fromarray(np.maximum(np.asarray(remove),
        np.asarray(polygon(im.size,cfg['generated_quad']))))
    remove=remove.filter(ImageFilter.MaxFilter(35))
    clean=harmonize(im,wall,remove)
    soft=remove.filter(ImageFilter.GaussianBlur(5))
    scene=Image.composite(clean,im,soft)
    inv=np.linalg.inv(homography(QUAD0,cfg['quad']));inv/=inv[2,2]
    frame=canonical.transform(im.size,Image.Transform.PERSPECTIVE,
        tuple(inv.flatten()[:8]),Image.Resampling.BICUBIC)
    shadow=Image.new('RGBA',im.size,(3,19,18,0))
    sha=frame.getchannel('A').filter(ImageFilter.GaussianBlur(6)).point(lambda v:round(v*.45))
    shifted=Image.new('L',im.size);shifted.paste(sha,(5,6));shadow.putalpha(shifted)
    scene=Image.alpha_composite(scene,shadow)
    scene=Image.alpha_composite(scene,frame)
    bounds=frame.getchannel('A').point(lambda v:255 if v>24 else 0).getbbox()
    return scene,list(bounds)

def painting_insert(size,quad,art):
    w,h=art.size
    inv=np.linalg.inv(homography([(0,0),(w,0),(w,h),(0,h)],quad))
    inv=inv/inv[2,2]
    warped=art.transform(size,Image.Transform.PERSPECTIVE,
                         tuple(inv.flatten()[:8]),Image.Resampling.BICUBIC)
    warped.putalpha(polygon(size,quad))
    return warped

def composite(bg,fg,quad=None,art=None):
    base=Image.new('RGBA',bg.size,(0,0,0,0))
    if quad is not None:
        base=painting_insert(bg.size,quad,art)
    base=Image.alpha_composite(base,bg)
    return Image.alpha_composite(base,fg)

def a0_work(im, background=False):
    # The exact requested centered +15% mapping, not a rounded anisotropic
    # resize: x'=1.15*x+1.2, y'=1.15*y+1.8. Pad by replication for full bleed.
    a=np.asarray(im)
    pad=np.pad(a,((3,3),(3,3),(0,0)),mode='edge' if background else 'constant')
    return Image.fromarray(pad).transform(SIZE,Image.Transform.AFFINE,
        (1/1.15,0,3-1.2/1.15,0,1/1.15,3-1.8/1.15),
        Image.Resampling.BICUBIC)

def checker(size):
    y,x=np.indices((size[1],size[0]))
    g=np.where(((x//24+y//24)%2)==0,214,242).astype('uint8')
    return Image.fromarray(np.stack([g,g,g,np.full_like(g,255)],axis=-1))

def tile(im,size):
    out=checker(size)
    p=im.copy();p.thumbnail(size,L)
    out.alpha_composite(p,((size[0]-p.width)//2,(size[1]-p.height)//2))
    return out

def label(draw,xy,text,size=26,fill='#ebedf4'):
    draw.text(xy,text,font=ImageFont.truetype(str(FONT),size),fill=fill)

def board_angles(comps,path,variant):
    tw,th=354,531
    board=Image.new('RGB',(tw*4+60,th+122),'#17232e')
    d=ImageDraw.Draw(board)
    label(d,(18,12),'PM76  /  '+variant+'  /  camera orbit',24)
    for i,(key,im) in enumerate(comps):
        x=12+i*(tw+12)
        board.paste(im.resize((tw,th),L).convert('RGB'),(x,50))
        label(d,(x+14,th+62),key+'   '+['38 deg','25 deg','12 deg','0 deg'][i],24)
    save(board,path)

def board_layers(a0fg,a0bg,a0comp,cfg,cwall,ccomp):
    tw,th=414,622
    board=Image.new('RGB',(tw*3+48,th*2+155),'#17232e')
    d=ImageDraw.Draw(board)
    label(d,(12,12),'PM76  /  pixel-aligned source layers',26)
    groups=[('a0', [a0fg,a0bg,a0comp],['a0_fg','a0_bg','composite']),
            ('corridor',[cfg,cwall,ccomp],['fg','wall','composite'])]
    for row,(name,ims,names) in enumerate(groups):
        y=52+row*(th+63)
        for col,(im,n) in enumerate(zip(ims,names)):
            x=12+col*(tw+12)
            board.paste(tile(im,(tw,th)).convert('RGB'),(x,y))
            label(d,(x+8,y+th+10),name+' / '+n,23)
    save(board,DOC/'katmanlar.png')

def verify_pair(source,fg,bg,quad=None):
    comp=Image.alpha_composite(bg,fg)
    expected=np.asarray(source)
    got=np.asarray(comp)
    exclude=np.asarray(polygon(source.size,quad))>0 if quad else np.zeros(source.size[::-1],bool)
    delta=np.max(np.abs(expected.astype(int)-got.astype(int)),axis=2)
    result={'recomposite_max_error_outside_canvas':int(delta[~exclude].max()),
            'recomposite_changed_pixels_outside_canvas':int(np.count_nonzero(delta[~exclude])),
            'background_alpha_values':sorted(map(int,np.unique(np.asarray(bg.getchannel('A'))))),
            'foreground_alpha_values':sorted(map(int,np.unique(np.asarray(fg.getchannel('A')))))}
    assert result['recomposite_changed_pixels_outside_canvas']==0,result
    return result

def main():
    for p in (DOC,SRC,OUT):p.mkdir(parents=True,exist_ok=True)
    original=load(ROOT/'Assets/_Game/Presentation/Resources/menu_bg.png')
    strip=load(ROOT/'Assets/_Game/Presentation/Resources/corridor_strip.png')
    art=load(ROOT/'Assets/Art/Paintings/painting-01.jpg')
    art=ImageOps.fit(art,(800,1020),method=L,centering=(.5,.5))
    plate=load(SRC/'a0-clean-plate.png').resize(original.size,L)
    cplate=load(SRC/'corridor-clean-plate.png').resize(strip.size,L)
    frame=reference_frame(original)
    frame0_bounds=list(frame.getchannel('A').point(lambda v:255 if v>24 else 0).getbbox())
    wall_path=SRC/'wall-empty.png'
    empty_wall=load(wall_path).resize(original.size,L) if wall_path.exists() else None
    fg0,bg0=split(original,a0_mask(original),plate,QUAD0)
    cfg,cwall=split(strip,corridor_mask(strip),cplate)
    save(cfg,OUT/'koridor/fg.png');save(cwall,OUT/'koridor/wall.png')
    ccomp=composite(cwall,cfg)
    comp0=composite(bg0,fg0,QUAD0,art)
    board_layers(fg0,bg0,comp0,cfg,cwall,ccomp)
    report={'source_a0':verify_pair(original,fg0,bg0,QUAD0),
            'source_corridor':verify_pair(strip,cfg,cwall), 'sets':{}}
    config_path=DOC/'angle_config.json'
    config=json.loads(config_path.read_text(encoding='utf8')) if config_path.exists() else {}
    for variant,folder in [('A','aci'),('B','aci_b')]:
        dest=OUT/folder;dest.mkdir(parents=True,exist_ok=True)
        save(fg0,dest/'a0_fg.png');save(bg0,dest/'a0_bg.png')
        save(a0_work(fg0),dest/'a0_fg_1180x1770.png')
        save(a0_work(bg0,True),dest/'a0_bg_1180x1770.png')
        transform=lambda pt:[round(pt[0]*1.15+1.2,4),round(pt[1]*1.15+1.8,4)]
        metadata={
          'schema_version':1,'coordinate_origin':'top-left','units':'pixels',
          'corner_order':['top_left','top_right','bottom_right','bottom_left'],
          'frame_bounds_order':['left','top','right','bottom'],
          'working_size':list(SIZE),
          'a0_mapping':{'source_size':[1024,1536],'target_size':list(SIZE),
             'scale':1.15,'translation':[1.2,1.8],
             'matrix':[[1.15,0,1.2],[0,1.15,1.8],[0,0,1]],
             'note':'Native a0 files preserve source pixels. Explicit 1180x1770 versions use centered 15% enlargement with replicated outer edge.'},
          'angles':{
            'a0':{'camera_angle_degrees':38,'orbit_right_degrees':0,
              'size':[1024,1536],'fg':'a0_fg.png','bg':'a0_bg.png',
              'working_fg':'a0_fg_1180x1770.png','working_bg':'a0_bg_1180x1770.png',
              'canvas_corners_native':QUAD0,
              'canvas_corners':[transform(p) for p in QUAD0],
              'frame_outer_bounds_native':frame0_bounds,
              'frame_outer_bounds':transform(frame0_bounds[:2])+transform(frame0_bounds[2:])}}}
        comps=[('a0',comp0)]
        report['sets'][variant]={}
        for k in range(1,4):
            key=f'{variant}_a{k}'
            if key not in config:continue
            cfgangle=config[key]
            im=load(SRC/cfgangle.get('source',key+'.png')).resize((1024,1536),L)
            if empty_wall is not None:
                im,cfgangle['bounds']=lock_frame_identity(im,cfgangle,frame,empty_wall)
            fg,bg=split(im,angle_mask(im,cfgangle),adjusted_plate(plate,im,cfgangle),cfgangle['quad'],cfgangle['wall_y'])
            check=verify_pair(im,fg,bg,cfgangle['quad'])
            # Native split is exact; final angle deliverables use 1180x1770.
            fg=fg.resize(SIZE,L);bg=bg.resize(SIZE,L)
            quad=[[round(x*1180/1024,4),round(y*1770/1536,4)] for x,y in cfgangle['quad']]
            # Re-cut at final resolution so the opening matches aci.json exactly
            # and has no remnants of the dark placeholder at its inner edge.
            bg=clear_canvas(bg,quad)
            save(fg,dest/f'a{k}_fg.png');save(bg,dest/f'a{k}_bg.png')
            comp=composite(bg,fg,quad,art)
            comps.append((f'a{k}',comp))
            bounds=cfgangle['bounds']
            metadata['angles'][f'a{k}']={
              'camera_angle_degrees':[25,12,0][k-1],
              'orbit_right_degrees':[13,26,38][k-1],
              'size':list(SIZE),'fg':f'a{k}_fg.png','bg':f'a{k}_bg.png',
              'canvas_corners':quad,
              'frame_outer_bounds':[round(v*1180/1024,4) for v in bounds]}
            metadata['angles'][f'a{k}']['frame_identity']='Original menu_bg frame ornaments projected onto the new camera wall plane; room and foreground drawn by reference-based imagegen.'
            if k==3:
                q=quad
                assert q[0][1]==q[1][1] and q[2][1]==q[3][1]
                assert q[0][0]==q[3][0] and q[1][0]==q[2][0]
                assert abs((q[0][0]+q[1][0])/2-590)<3,'a3 must be centered'
            report['sets'][variant][f'a{k}']=check
        (dest/'aci.json').write_text(json.dumps(metadata,ensure_ascii=False,indent=2),encoding='utf8')
        if len(comps)==4:
            board_angles(comps,DOC/('acilar.png' if variant=='A' else 'acilar_b.png'),variant)
    report['png_files']=[]
    for path in sorted(OUT.rglob('*.png')):
        im=load(path)
        assert path.stat().st_size<10_000_000
        report['png_files'].append({'path':str(path.relative_to(ROOT)).replace('\\','/'),
            'size':list(im.size),'mode':im.mode,'bytes':path.stat().st_size})
    for name in ['Assets/_Game/Presentation/Resources/menu_bg.png',
                 'Assets/_Game/Presentation/Resources/corridor_strip.png',
                 'Assets/Art/Paintings/painting-01.jpg']:
        report.setdefault('reference_sha256',{})[name]=hashlib.sha256((ROOT/name).read_bytes()).hexdigest()
    (DOC/'validation.json').write_text(json.dumps(report,indent=2),encoding='utf8')
    print(json.dumps({'a0_max_pixel_error':report['source_a0']['recomposite_max_error_outside_canvas'],
        'corridor_max_pixel_error':report['source_corridor']['recomposite_max_error_outside_canvas'],
        'angles':{k:list(v) for k,v in report['sets'].items()},'png_count':len(report['png_files'])}))

if __name__=='__main__':
    main()
