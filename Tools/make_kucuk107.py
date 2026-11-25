"""PM_107 §D2: the collection's thumbnails. The grid showed every card from its full 1024x1024 painting (downloaded,
decoded and its tones read on the main thread as its row came into view: a 1 s freeze on the first scroll with the
CPU at a phone's pace, the "jumping background"). Each painting's middle, 3:4 as the card shows it, at 210x280 (the
card's 69x92 pt at 3x), JPEG q82: ~15 KB.
    python -I Tools/make_kucuk107.py <paintings-v2 dir of gh-pages>   (writes <dir>/kucuk/painting-NN.jpg)
"""
import glob, os, sys
from PIL import Image

src = sys.argv[1]
out = os.path.join(src, 'kucuk'); os.makedirs(out, exist_ok=True)
n = 0; total = 0
for f in sorted(glob.glob(os.path.join(src, 'painting-*.jpg'))):
    im = Image.open(f).convert('RGB')
    w, h = im.size
    ch = min(h, int(w * 4 / 3)); cw = int(ch * .75)
    im = im.crop(((w - cw) // 2, (h - ch) // 2, (w - cw) // 2 + cw, (h - ch) // 2 + ch)).resize((210, 280), Image.LANCZOS)
    p = os.path.join(out, os.path.basename(f)); im.save(p, quality=82, optimize=True, progressive=False)
    n += 1; total += os.path.getsize(p)
print(f'[kucuk] {n} küçük resim, ortalama {total / max(1, n) / 1024:.1f} KB')
