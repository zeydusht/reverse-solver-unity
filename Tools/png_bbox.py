"""Alpha bounding box of PNGs (no PIL): python Tools/png_bbox.py a.png b.png"""
import zlib, struct, sys
def bbox(p):
    b = open(p, 'rb').read(); i = 8; idat = b''
    while i < len(b):
        n = struct.unpack('>I', b[i:i+4])[0]; t = b[i+4:i+8]; d = b[i+8:i+8+n]
        if t == b'IHDR': w, h, bd, ct = struct.unpack('>IIBB', d[:10])
        if t == b'IDAT': idat += d
        i += 12 + n
    raw = zlib.decompress(idat); bpp = 4; st = w * bpp; prev = bytearray(st); k = 0; xs = []; ys = []
    for y in range(h):
        f = raw[k]; line = bytearray(raw[k+1:k+1+st]); k += 1 + st
        for x in range(st):
            a = line[x-bpp] if x >= bpp else 0; c = prev[x-bpp] if x >= bpp else 0; up = prev[x]
            if f == 1: line[x] = (line[x] + a) & 255
            elif f == 2: line[x] = (line[x] + up) & 255
            elif f == 3: line[x] = (line[x] + ((a + up) >> 1)) & 255
            elif f == 4:
                pa = abs(up - c); pb = abs(a - c); pc = abs(a + up - 2 * c)
                line[x] = (line[x] + (a if pa <= pb and pa <= pc else up if pb <= pc else c)) & 255
        for x in range(w):
            if line[x*4+3] > 128: xs.append(x); ys.append(y)
        prev = line
    return w, h, min(xs), max(xs), min(ys), max(ys)
for p in sys.argv[1:]: print(p.split('/')[-1], bbox(p))
