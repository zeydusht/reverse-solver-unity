"""Reverse Solver 'Level X' play button (Royal-Match-like generic style). Procedural, 9-slice friendly.
Rim uses docs/art/PALETTE.md gold scale; body is candy green. Text is NOT baked (rendered in Unity)."""
import sys
import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont
W, H = 600, 216; SS = 4
def hexc(h): h=h.lstrip('#'); return np.array([int(h[i:i+2],16) for i in (0,2,4)])/255.0
def rr_mask(w, h, r, inset=0, dy=0):
    m = Image.new('L', (w*SS, h*SS), 0); d = ImageDraw.Draw(m)
    d.rounded_rectangle([inset*SS, (inset+dy)*SS, (w-inset)*SS-1, (h-inset+dy)*SS-1], radius=r*SS, fill=255)
    return np.array(m.resize((w, h), Image.LANCZOS)).astype(float)/255
def vgrad(h, top, bot, y0=0, y1=None):
    y1 = h if y1 is None else y1
    t = np.clip((np.arange(h)-y0)/max(1,(y1-y0)), 0, 1)[:, None, None]
    return top*(1-t) + bot*t
def over(dst, rgb, a):
    a = a[..., None]; dst[..., :3] = rgb*a + dst[..., :3]*(1-a); dst[..., 3:] = a + dst[..., 3:]*(1-a)
def build(pressed=False):
    PAD = 14; w, h = W, H
    out = np.zeros((h, w, 4))
    dy = 6 if pressed else 0
    # drop shadow
    sh = rr_mask(w, h, 62, PAD-2, dy=8 if not pressed else 4)
    sh = np.array(Image.fromarray((sh*255).astype(np.uint8)).filter(ImageFilter.GaussianBlur(6))).astype(float)/255
    over(out, np.zeros((h, w, 3))+hexc('#3A1C02')*0, sh*0.45)
    # outline (dark gold)
    o = rr_mask(w, h, 62, PAD, dy); over(out, np.broadcast_to(hexc('#7A3A00'), (h, w, 3)), o)
    # gold rim gradient
    rim = rr_mask(w, h, 58, PAD+4, dy)
    over(out, vgrad(h, hexc('#FFE27A'), hexc('#CE5D01'), PAD+dy, h-PAD+dy).repeat(w, 1), rim)
    # rim mid band
    rim2 = rr_mask(w, h, 54, PAD+8, dy)
    over(out, vgrad(h, hexc('#FBBA0E'), hexc('#F38A03'), PAD+dy, h-PAD+dy).repeat(w, 1), rim2)
    # inner dark edge of body
    be = rr_mask(w, h, 46, PAD+16, dy); over(out, np.broadcast_to(hexc('#2E7A12'), (h, w, 3)), be)
    # green body
    body = rr_mask(w, h, 42, PAD+19, dy)
    top, bot = (hexc('#7FD43A'), hexc('#3E9E1B')) if not pressed else (hexc('#6CBF2E'), hexc('#358A16'))
    over(out, vgrad(h, top, bot, PAD+19+dy, h-PAD-19+dy).repeat(w, 1), body)
    # glossy upper half
    gl = rr_mask(w, h, 38, PAD+24, dy) * (np.arange(h)[:, None] < (h/2 + dy - 4))
    gl = np.array(Image.fromarray((gl*255).astype(np.uint8)).filter(ImageFilter.GaussianBlur(1.2))).astype(float)/255
    over(out, np.broadcast_to(hexc('#B8F06E'), (h, w, 3)), gl*0.55)
    # thin top highlight line
    hl = rr_mask(w, h, 40, PAD+21, dy) - rr_mask(w, h, 38, PAD+23, dy+2)
    hl = np.clip(hl, 0, 1) * (np.arange(h)[:, None] < h*0.45 + dy)
    over(out, np.broadcast_to(np.ones(3), (h, w, 3)), hl*0.7)
    return Image.fromarray((np.clip(out, 0, 1)*255).astype(np.uint8), 'RGBA')
def label(img, text, font, dy=0):
    im = img.copy(); d = ImageDraw.Draw(im)
    f = ImageFont.truetype(font, 92)
    bb = d.textbbox((0, 0), text, font=f, stroke_width=7)
    x = (W-(bb[2]-bb[0]))//2 - bb[0]; y = (H-(bb[3]-bb[1]))//2 - bb[1] - 4 + dy
    d.text((x, y+5), text, font=f, fill=(20, 70, 10, 200), stroke_width=7, stroke_fill=(20, 70, 10, 200))
    d.text((x, y), text, font=f, fill=(255, 255, 240, 255), stroke_width=7, stroke_fill=(30, 92, 14, 255))
    return im
if __name__ == '__main__':
    out = sys.argv[1]; font = sys.argv[2]
    n = build(False); p = build(True)
    n.save(f'{out}/menu-play-button.png'); p.save(f'{out}/menu-play-button-pressed.png')
    prev = Image.new('RGBA', (W*2+60, H+40), (150, 110, 80, 255))
    prev.alpha_composite(label(n, 'Level 7', font), (20, 20)); prev.alpha_composite(label(p, 'Level 7', font, 6), (W+40, 20))
    prev.save(f'{out}/preview.png')
