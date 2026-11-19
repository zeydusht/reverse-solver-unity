"""PM_89 §1: Codex's flat frame (docs/art/pm89/cerceve/<option>/frame.png, 1200 px, opening 150..1050) into the
game's frame89.png (1320 px: the same frame 60 px in, with its soft shadow on the wall down and right, none
inside the opening; the lip carried 2 px into the opening so no hairline shows against the picture). Frame89.cs
slices it at 0 / 390 / 540 / 780 / 930 / 1320.
  python Tools/pm89_frame_import.py [A|B|C]"""
import os, sys
import numpy as np
from PIL import Image, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
opt = sys.argv[1] if len(sys.argv) > 1 else 'A'
src = Image.open(os.path.join(ROOT, 'docs', 'art', 'pm89', 'cerceve', opt, 'frame.png')).convert('RGBA')
assert src.size == (1200, 1200), src.size
a = np.asarray(src).copy()
# the lip 2 px into the opening (rows / columns just inside take the lip's own pixel)
for d in (0, 1):
    a[150 + d, 150:1050] = np.where(a[150 + d, 150:1050, 3:4] == 0, a[149, 150:1050], a[150 + d, 150:1050])
    a[1049 - d, 150:1050] = np.where(a[1049 - d, 150:1050, 3:4] == 0, a[1050, 150:1050], a[1049 - d, 150:1050])
    a[150:1050, 150 + d] = np.where(a[150:1050, 150 + d, 3:4] == 0, a[150:1050, 149], a[150:1050, 150 + d])
    a[150:1050, 1049 - d] = np.where(a[150:1050, 1049 - d, 3:4] == 0, a[150:1050, 1050], a[150:1050, 1049 - d])
frame = Image.fromarray(a, 'RGBA')
M = 60
out = Image.new('RGBA', (1320, 1320), (0, 0, 0, 0))
# the shadow: the frame's alpha, blurred, 10 px right and 16 px down, a deep teal at 45 %
alpha = Image.new('L', (1320, 1320), 0); alpha.paste(frame.getchannel('A'), (M + 10, M + 16))
alpha = alpha.filter(ImageFilter.GaussianBlur(14))
sh = np.asarray(alpha, dtype=np.float64) * .45
sh[M + 150:M + 1050, M + 150:M + 1050] = 0   # none over the picture
shadow = np.zeros((1320, 1320, 4), np.uint8); shadow[..., :3] = (3, 24, 28); shadow[..., 3] = np.clip(sh, 0, 255).astype(np.uint8)
out = Image.alpha_composite(out, Image.fromarray(shadow, 'RGBA'))
layer = Image.new('RGBA', (1320, 1320), (0, 0, 0, 0)); layer.paste(frame, (M, M))
out = Image.alpha_composite(out, layer)
dst = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources', 'Art', 'yeni', 'pm89', 'frame89.png')
os.makedirs(os.path.dirname(dst), exist_ok=True)
out.save(dst)
# the stretches must stay uniform after the shadow (column / row differences in 390..540 and 780..930)
o = np.asarray(out, dtype=np.float64)
worst = max(np.abs(np.diff(o[:, 400:530], axis=1)).mean(), np.abs(np.diff(o[400:530, :], axis=0)).mean(),
            np.abs(np.diff(o[:, 790:920], axis=1)).mean(), np.abs(np.diff(o[790:920, :], axis=0)).mean())
print(f'{dst}: option {opt}, stretch bands mean step {worst:.3f}/255')
