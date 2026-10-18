"""PM checkpoint 5 (c): after the one-round regeneration of booster-leaning slots, keep at
most three finalists per slot: not leaning on boosters first, then the smaller boosted
minus boosterless fun gap, then no curve exception; a new one replaces an old one only
by ranking ahead of it. Prints old -> kept per slot.

    python Tools/pick_regen.py 16,17,19,32,36,38,40
"""
import json
import re
import sys

P = "Assets/_Game/Levels/redesign.json"
slots = [int(s) for s in sys.argv[1].split(",")]
lines = open(P, encoding="utf-8").read().split("\n")
rows = []
for i, l in enumerate(lines):
    m = re.search(r'"candidate":"([^"]+)"', l)
    if m:
        rows.append((i, m.group(1), json.loads(l.strip().rstrip(","))))
drop = set()
for s in slots:
    fs = [(c, d["fabrika"]) for i, c, d in rows if d["fabrika"]["slot"] == s and d["fabrika"]["status"] == "finalist"]
    def key(x):
        sc = x[1]["scores"]
        return (1 if sc.get("boosteraDayali") else 0, round(sc["keyif"] - sc.get("keyifSiz", sc["keyif"]), 3),
                1 if "istisna" in (x[1]["psych"] or "") else 0, -sc["keyif"])
    fs.sort(key=key)
    keep = fs[:3]
    for c, _ in fs[3:]:
        drop.add(c)
    print(f"slot {s}: {len(fs)} -> " + ", ".join(f"{c} ({'yeni' if not f['aha'] else 'eski'}, fark {(f['scores']['keyif'] - f['scores'].get('keyifSiz', f['scores']['keyif'])) * 100:.0f})" for c, f in keep))
out = [l for l in lines if not any(f'"candidate":"{c}"' in l for c in drop)]
t = re.sub(r",\n(\s*\])", r"\n\1", "\n".join(out))
json.loads(t)
open(P, "w", encoding="utf-8", newline="\n").write(t)
print("çıkan:", sorted(drop))
