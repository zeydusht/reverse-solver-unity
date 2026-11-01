"""PM_MESAJ_24: the Task 18 corridor video is a pure horizontal pan (frame 90 = frame 0 moved 431 px, error ~2/255),
so the game slides one still strip instead of playing the video. Out: Assets/_Game/Presentation/Resources/corridor_strip.png
(frame 0 + the next bay, 1644x1688, the green apertures painted dark) and the bay quad/period it prints for CorridorTransition.cs."""
import json, os
import numpy as np
from PIL import Image

SRC = os.path.join(os.path.dirname(__file__), '..', 'docs/art/drafts/corridor-victory-v3-layered-20261031')
OUT = os.path.join(os.path.dirname(__file__), '..', 'Assets/_Game/Presentation/Resources/corridor_strip.png')
first = np.asarray(Image.open(os.path.join(SRC, 'corridor-empty-first.png')).convert('RGB'))
mid = np.asarray(Image.open(os.path.join(SRC, 'qa/background-middle.png')).convert('RGB'))
track = json.load(open(os.path.join(SRC, 'frame-track.json')))['frames']
x = [f['instances'][0]['quad'][0][0] for f in track]
period = x[0] - x[-1]
h, w, _ = first.shape
P = int(round(period)); half = int(round(period / 2))
strip = np.zeros((h, (w + P + 3) // 4 * 4, 3), np.uint8)   # DXT/ASTC blocks: width a multiple of 4
strip[:, :w] = first
strip[:, w:P] = mid[:, w - half:P - half]                 # the gap the first frame does not cover
strip[:, P:P + w] = first
strip[:, P + w:] = strip[:, w:strip.shape[1] - P]           # the corridor repeats every bay
green = (strip[..., 0] == 0) & (strip[..., 1] == 255) & (strip[..., 2] == 0)
strip[green] = (38, 30, 26)                                 # never green, even where a painting is late
Image.fromarray(strip).save(OUT, optimize=True)
q = track[0]['instances'][0]['quad']
print('[strip]', strip.shape[1], 'x', h, 'period', round(period, 2), 'green px', int(green.sum()),
      'mid-green', int(((mid[..., 1] == 255) & (mid[..., 0] == 0)).sum()))
print('[quad]', [[round(a, 1) for a in p] for p in q])
