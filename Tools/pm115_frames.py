"""PM_115: each capture's frame times from its log (Tools/pm108_dalis.js, &dalislog=1: one [cerceve] line a frame on
the way in, one [dl] line a frame from the hand over; the console's own time stamps): the frames over 20 ms at the
bottom (the dive's clock .69 .. .90 s, after the build and the reveal frames) and over the whole dive.
usage: python -I Tools/pm115_frames.py <capture> [<capture> ...]"""
import re, sys
num = lambda s: float(s.replace(',', '.'))
for cap in sys.argv[1:]:
    fr = []
    for line in open(cap + '/log.txt', encoding='utf8', errors='replace'):
        m = re.match(r'^([\d.]+) \[(cerceve|dl)\] ([\d.,-]+) ', line)
        if m and (not fr or abs(num(m.group(3)) - fr[-1][1]) > 1e-6): fr.append((float(m.group(1)), num(m.group(3))))
    dts = [(fr[i][0] - fr[i - 1][0], fr[i][1]) for i in range(1, len(fr))]
    bottom = [round(d) for d, c in dts if .69 < c <= .90 and d > 20]
    allx = [round(d) for d, c in dts if d > 20]
    print(f'[frames] {cap.replace(chr(92), "/").split("/")[-1]}: bottom {len(bottom)} {bottom}, whole dive {len(allx)} (max {max([d for d, _ in dts]):.0f} ms)')
