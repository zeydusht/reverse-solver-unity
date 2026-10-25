"""Reverse Solver jigsaw piece kit generator.
Tab/notch shape is taken from Codex demo (Assets/Art/Review/Pieces/demo-v02-centered.png, red piece left tab),
style (outline, bevel, gloss) imitates the same demo. Outputs 81 combos x 3 layers.
Naming: piece_TOP-RIGHT-BOTTOM-LEFT.png, O=out (tab), F=flat, I=in (notch)."""
import sys, os, json, itertools
import numpy as np
from PIL import Image
from scipy import ndimage as nd
import matplotlib.colors as mc

SRC = sys.argv[1]; OUT = sys.argv[2]
S = 384            # cell size (px, @3x of 128 pt)
P = 128            # padding for tabs on each side
C = S + 2*P        # canvas 640
OUTLINE = 9        # outline thickness px
BEVEL = 22         # bevel width px
RADIUS = 18        # body corner radius
SS = 2             # supersample

# --- extract canonical knob from red piece left tab ---
a = np.array(Image.open(SRC).convert('RGBA')).astype(float)/255
hsv = mc.rgb_to_hsv(a[...,:3]); h = hsv[...,0]*360
red = (((h<22)|(h>340)) & (hsv[...,1]>0.3) & (hsv[...,2]>0.8) & (a[...,3]>0.5))
red = nd.binary_fill_holes(nd.binary_closing(red, iterations=8))
lab, n = nd.label(red); red = lab == (1+int(np.argmax(nd.sum(red, lab, range(1,n+1)))))
BODY_X0, BODY_Y0, BODY_Y1 = 587, 95, 534      # measured red body left edge / top / bottom
BODY_W, BODY_H = 1061-587, BODY_Y1-BODY_Y0
knob = red[:, :BODY_X0+6].copy()               # include a bit of body overlap
ys, xs = np.where(knob)
k = knob[ys.min():ys.max()+1, xs.min():]
cy_src = (ys.min()+ys.max())/2 - BODY_Y0       # knob centre relative to body top
# scale to cell: depth by S/BODY_W, breadth by S/BODY_H (supersampled)
kd = int(round(k.shape[1]*S/BODY_W*SS)); kb = int(round(k.shape[0]*S/BODY_H*SS))
kimg = Image.fromarray((k*255).astype(np.uint8)).resize((kd, kb), Image.LANCZOS)
K = np.array(kimg) > 127                        # knob pointing LEFT, right edge = body edge (+overlap)
overlap = int(round(6*S/BODY_W*SS))
cyc = int(round(cy_src*S/BODY_H*SS))            # centre position along edge
centre = (S*SS)//2                              # use exact centre for symmetry
# make knob symmetric (mirror average) for clean rotations
K = (K | K[::-1, :])

def rounded_rect(n, r):
    m = np.zeros((n, n), bool); m[:] = True
    yy, xx = np.mgrid[0:n, 0:n]
    for cy, cx in [(r, r), (r, n-1-r), (n-1-r, r), (n-1-r, n-1-r)]:
        corner = ((yy < r) if cy == r else (yy > n-1-r)) & ((xx < r) if cx == r else (xx > n-1-r))
        m[corner & ((yy-cy)**2 + (xx-cx)**2 > r*r)] = False
    return m

def stamp(canvas, knob, side, add):
    """place knob on given side of the cell; side in T,R,B,L; add=True tab, False notch"""
    Cs, Ps, Ss = C*SS, P*SS, S*SS
    kk = knob
    # knob canonical points LEFT from left edge; rotate for other sides
    rot = {'L':0, 'T':3, 'R':2, 'B':1}[side]   # np.rot90 counter-clockwise k times
    kk = np.rot90(kk, rot)
    kh, kw = kk.shape
    if side == 'L':
        x0 = Ps - (kw - overlap) if add else Ps - overlap
        if not add: kk = kk[:, ::-1]; x0 = Ps - overlap + 0
        y0 = Ps + centre - kh//2
    if side == 'R':
        x0 = Ps + Ss - overlap if add else Ps + Ss - kw + overlap
        if not add: kk = kk[:, ::-1]
        y0 = Ps + centre - kh//2
    if side == 'T':
        y0 = Ps - (kh - overlap) if add else Ps - overlap
        if not add: kk = kk[::-1, :]
        x0 = Ps + centre - kw//2
    if side == 'B':
        y0 = Ps + Ss - overlap if add else Ps + Ss - kh + overlap
        if not add: kk = kk[::-1, :]
        x0 = Ps + centre - kw//2
    region = canvas[y0:y0+kh, x0:x0+kw]
    if add: region |= kk
    else:
        # notch: carve slightly larger knob so mating tab fits (clearance 2px)
        carve = nd.binary_dilation(kk, iterations=2*SS)
        region &= ~carve
    canvas[y0:y0+kh, x0:x0+kw] = region

def shape_mask(edges):
    Cs, Ps, Ss = C*SS, P*SS, S*SS
    m = np.zeros((Cs, Cs), bool)
    m[Ps:Ps+Ss, Ps:Ps+Ss] = rounded_rect(Ss, RADIUS*SS)
    for side, e in zip('TRBL', edges):
        if e == 'O': stamp(m, K, side, True)
    for side, e in zip('TRBL', edges):
        if e == 'I': stamp(m, K, side, False)
    m = nd.binary_opening(nd.binary_closing(m, iterations=2*SS), iterations=SS)
    return m

def down(x):  # supersampled float -> canvas
    return np.array(Image.fromarray((np.clip(x,0,1)*255).astype(np.uint8)).resize((C, C), Image.LANCZOS)).astype(float)/255

def render(m):
    din = nd.distance_transform_edt(m)
    dout = nd.distance_transform_edt(~m)
    inside = m.astype(float)
    # bevel height & normal
    hgt = np.clip(din/(BEVEL*SS), 0, 1); hgt = 1-(1-hgt)**2
    hgt = nd.gaussian_filter(hgt, 2*SS)
    gy, gx = np.gradient(hgt)
    L = np.array([-0.55, -0.65])  # light from top-left (x,y)
    lam = -(gx*L[0] + gy*L[1]) * (BEVEL*SS) * 1.4
    light = np.clip(lam, 0, 1) * inside
    shadow = np.clip(-lam, 0, 1) * inside
    # rim: light inner band all around (glossy plastic look)
    rim = np.clip(1-np.abs(din-(4.5*SS))/(3*SS), 0, 1) * inside
    rim = nd.gaussian_filter(rim, 0.6*SS)
    # specular streak: short white stroke near top edge (left); no corner dot
    Cs = C*SS; yy, xx = np.mgrid[0:Cs, 0:Cs]
    spec = (np.exp(-(((xx-(P+0.22*S)*SS)/(0.12*S*SS))**2 + ((yy-(P+0.055*S)*SS)/(0.012*S*SS))**2))) * inside
    # thin dark inner line + outer outline
    inner_line = ((din > 0) & (din < 2.2*SS)).astype(float)
    ring = np.clip((OUTLINE*SS - dout)/SS + 0.5, 0, 1) * (~m)
    return dict(inside=down(inside), light=down(light), shadow=down(shadow), rim=down(rim), spec=down(np.clip(spec,0,1)),
                inner=down(inner_line), ring=down(ring))

OUTLINE_RGB = np.array([7, 42, 54])/255
def overlay_rgba(R):
    # transparent overlay that sits ON TOP of the painting/fill
    rgb = np.zeros((C, C, 3)); a = np.zeros((C, C))
    def over(col, al):
        nonlocal rgb, a
        col = np.array(col)
        rgb = col*al[...,None] + rgb*(1-al[...,None]); a = al + a*(1-al)
    over([0.0,0.05,0.10], R['shadow']*0.18)
    over([1,1,1], R['rim']*0.55)
    over([1,1,1], R['light']*0.38)
    over([1,1,1], R['spec']*0.85)
    over(OUTLINE_RGB, R['inner']*0.65)
    over(OUTLINE_RGB, R['ring'])
    return np.dstack([rgb, a])

def compose(base_rgb, R, ov):
    base = np.zeros((C, C, 4)); base[..., :3] = base_rgb; base[..., 3] = R['inside']
    out = ov[..., :3]*ov[..., 3:] + base[..., :3]*base[..., 3:]*(1-ov[..., 3:])
    al = ov[..., 3] + base[..., 3]*(1-ov[..., 3])
    rgb = np.where(al[..., None] > 0, out/np.maximum(al[..., None], 1e-6), 0)
    return np.dstack([rgb, al])

def save(arr, path):
    Image.fromarray((np.clip(arr, 0, 1)*255).astype(np.uint8), 'RGBA').save(path, optimize=True)

if __name__ == '__main__':
    os.makedirs(OUT + '/mask', exist_ok=True); os.makedirs(OUT + '/overlay', exist_ok=True); os.makedirs(OUT + '/flat', exist_ok=True)
    fill = np.array([12, 198, 250])/255     # Codex demo blue
    only = sys.argv[3].split(',') if len(sys.argv) > 3 else None
    combos = [''.join(c) for c in itertools.product('OFI', repeat=4)]
    if only: combos = only
    for c in combos:
        edges = list(c.replace('-', ''))
        m = shape_mask(edges)
        R = render(m)
        name = 'piece_' + '-'.join(edges) + '.png'
        mk = np.dstack([np.ones((C, C, 3)), R['inside']])
        save(mk, f'{OUT}/mask/{name}')
        ov = overlay_rgba(R); save(ov, f'{OUT}/overlay/{name}')
        grad = fill[None, None, :] * (1.05 - 0.12*np.linspace(0, 1, C)[:, None, None])
        save(compose(grad, R, ov), f'{OUT}/flat/{name}')
    json.dump(dict(canvas=C, cell=S, padding=P, cell_rect=[P, P, S, S], pivot=[C/2, C/2],
                   naming='piece_TOP-RIGHT-BOTTOM-LEFT, O=tab out, F=flat, I=notch in',
                   layers=dict(mask='white + alpha shape; clip the painting with it',
                               overlay='outline+bevel+gloss, draw ON TOP of painting',
                               flat='ready colored piece (no painting)'),
                   count=len(combos)), open(f'{OUT}/kit.json', 'w'), indent=2)
    print('done', len(combos))
