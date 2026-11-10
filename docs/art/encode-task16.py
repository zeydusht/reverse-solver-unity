"""Export an opaque 1024px game JPG at quality 85 with optimized coding."""
import sys
import json
from io import BytesIO
from pathlib import Path

sys.dont_write_bytecode = True
sys.path.insert(0, str(Path(__file__).resolve().parent / '.task16-python'))
from PIL import Image, ImageFilter

source, destination = sys.argv[1:3]
with Image.open(source) as image:
    game = image.convert('RGB').resize((1024, 1024), Image.Resampling.LANCZOS)
    for radius in (0, .2, .3, .4, .5, .6, .7, .8, .9, 1.0, 1.1, 1.2):
        filtered = game if radius == 0 else game.filter(ImageFilter.GaussianBlur(radius))
        encoded = BytesIO()
        filtered.save(encoded, format='JPEG', quality=85, subsampling=2,
                      optimize=True, progressive=True)
        if encoded.tell() <= 300_000:
            Path(destination).write_bytes(encoded.getvalue())
            print(json.dumps({'prefilterRadius': radius, 'bytes': encoded.tell()}))
            break
    else:
        raise RuntimeError('Cannot meet 300000 bytes with quality 85 and <=1.2px prefilter')
