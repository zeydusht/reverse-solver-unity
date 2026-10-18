"""PM checkpoint 5 (2026-10-18) on redesign.json: (b) Orta early time-out relaxed before
round 3 and failing the strict 5 % cap -> out; (a) curve exceptions (fit < 0.6 on
O/Z/ÇZ): one per slot, only while the slot has fewer than two other accepted finalists,
fit >= 0.40, the highest fun kept and labelled; the rest out."""
import json
import re

P = "Assets/_Game/Levels/redesign.json"
PSYCH = re.compile(r'"psych":(null|"(?:[^"\\]|\\.)*")')
lines = open(P, encoding="utf-8").read().split("\n")
recs = {}
for i, l in enumerate(lines):
    m = re.search(r'"candidate":"([^"]+)"', l)
    if m:
        recs[m.group(1)] = (i, json.loads(l.strip().rstrip(",")))
drop, label = set(), {}
for c, (i, d) in recs.items():
    sc = d["fabrika"]["scores"]
    if sc.get("erkenGevsetildi") and d["fabrika"]["genre"] == "orta" and sc["erkenSure"] > 0.05 + 1e-9:
        drop.add(c)
by = {}
for c, (i, d) in recs.items():
    by.setdefault(d["fabrika"]["slot"], []).append(c)
for s, cs in by.items():
    live = [c for c in cs if c not in drop]
    f = lambda c: recs[c][1]["fabrika"]
    exc = [c for c in live if f(c)["genre"] in ("orta", "zor", "cokzor") and f(c)["scores"].get("uyum", 1) < 0.6]
    normal = [c for c in live if c not in exc]
    exc.sort(key=lambda c: -f(c)["scores"]["keyif"])
    for k, c in enumerate(exc):
        u = f(c)["scores"]["uyum"]
        if k == 0 and len(normal) < 2 and u >= 0.40:
            label[c] = f"KABUL (eğri istisnası, uyum {u:.2f}; PM kontrol noktası 5)"
        else:
            drop.add(c)
for c, v in label.items():
    i = recs[c][0]
    lines[i] = PSYCH.sub(lambda _: '"psych":' + json.dumps(v, ensure_ascii=False), lines[i], count=1)
out = [l for l in lines if not any(f'"candidate":"{c}"' in l for c in drop)]
t = "\n".join(out)
t = re.sub(r",\n(\s*\])", r"\n\1", t)
json.loads(t)
open(P, "w", encoding="utf-8", newline="\n").write(t)
print("out:", sorted(drop))
print("istisna:", sorted(label))
