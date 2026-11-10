"""User-authorized pixel copying: original first bay; no resizing or projection change."""
import sys,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw
im=Image.open(root/'original-panorama.png').convert('RGB')
tile=im.crop((145,0,641,971))
tile.save(root/'copied-first-bay.png')
pair=Image.new('RGB',(992,971))
pair.paste(tile,(0,0));pair.paste(tile,(496,0))
pair.save(root/'copy-before-seam-repair.png')
pair.crop((400,750,592,971)).save(root/'floor-join-input.png')
# Reference edit region shown only on a review image, not on source artwork.
mark=pair.copy();d=ImageDraw.Draw(mark);d.rectangle((424,0,568,970),outline='#ff00ff',width=3)
mark.save(root/'seam-region-reference.png')
print('Copied source x145..641 twice; period496; complete original pillar retained. No resizing.')
