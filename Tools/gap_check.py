"""PM_76: no uncovered edge in the dive. Frames captured with &dalisbosluk=1 (the dive's backdrop is magenta):
counts magenta pixels in a 3 px band along the screen's edges, per frame; prints the worst frame and the total.
    python -I Tools/gap_check.py <capture dir>"""
import json, os, sys
from PIL import Image


def magenta(p):
    r, g, b = p[:3]
    return r > 190 and b > 190 and g < 90


def main():
    d = sys.argv[1]
    files = sorted(f for f in os.listdir(d) if f.startswith("f") and f.endswith(".jpg"))
    worst, worst_file, total, frames_with = 0, None, 0, 0
    for f in files:
        im = Image.open(os.path.join(d, f)).convert("RGB")
        w, h = im.size
        px = im.load()
        n = 0
        for x in range(w):
            for y in list(range(3)) + list(range(h - 3, h)):
                n += magenta(px[x, y])
        for y in range(3, h - 3):
            for x in list(range(3)) + list(range(w - 3, w)):
                n += magenta(px[x, y])
        total += n
        frames_with += n > 0
        if n > worst:
            worst, worst_file = n, f
    print(json.dumps({"frames": len(files), "frames_with_gap": frames_with, "worst_pixels": worst, "worst_frame": worst_file, "total": total}))


if __name__ == "__main__":
    main()
