"""PM_107 §D4: the game's one gold coin (docs/art/pm107/codex/altin/final_coin.png, Codex: the puzzle piece in relief)
written into every coin file the game draws, each at its own size (Lanczos), so the menu, the win, the chest, the
flight to the counter and the continue panel show the same coin; and Resources/Art/yeni/pm107/altin.png (256) for the
menu's counter (Screens.Coin107). The import settings (.meta) of the old files stay.
    python -I Tools/pm107_altin.py
"""
import os
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources', 'Art')
SRC = os.path.join(ROOT, 'docs', 'art', 'pm107', 'codex', 'altin', 'final_coin.png')
TARGETS = ['yeni/T50/win/coin.png', 'yeni/devam_v2/continue-coin.png', 'yeni/win21/Reward/Flight/coin.png', 'm13-token-coin.png']

coin = Image.open(SRC).convert('RGBA')
# trim to the coin, square, a 3 % margin
bb = coin.getchannel('A').point(lambda a: 255 if a > 8 else 0).getbbox()
c = coin.crop(bb)
side = int(max(c.size) * 1.06)
sq = Image.new('RGBA', (side, side), (0, 0, 0, 0)); sq.paste(c, ((side - c.size[0]) // 2, (side - c.size[1]) // 2), c)
for t in TARGETS:
    p = os.path.join(ART, t)
    w, h = Image.open(p).size
    sq.resize((w, h), Image.LANCZOS).save(p)
    print(f'[altin] {t} {w}x{h}')
out = os.path.join(ART, 'yeni', 'pm107', 'altin.png')
os.makedirs(os.path.dirname(out), exist_ok=True)
sq.resize((256, 256), Image.LANCZOS).save(out)
print(f'[altin] yeni/pm107/altin.png 256x256')
