"""Extract the 40 levels from the web game into Assets/_Game/Levels/levels.json.

Level data is copied verbatim from the LEVELS array in ../web-reference/index.html.
Each level gains a stable `id` and a `version`; the file gains a `format` number.

`id` is permanent and never reused: the initial ids L01-L40 come from the web
order, but they do not follow the order if levels are later moved. Bump a
level's `version` for any gameplay-affecting change (not for art or text).
Re-running this script only makes sense while the levels are still the web
originals; after the redesign starts, edit levels.json directly.

usage: python Tools/extract_levels.py [path/to/index.html]
"""
import json, re, sys
from pathlib import Path

FORMAT = 1
root = Path(__file__).resolve().parent.parent
src = Path(sys.argv[1]) if len(sys.argv) > 1 else root.parent / "web-reference" / "index.html"
html = src.read_text(encoding="utf-8")
m = re.search(r"^const LEVELS = (\[.*\]);\s*$", html, re.M)
if not m:
    sys.exit(f"LEVELS array not found in {src}")
levels = json.loads(m.group(1))

out = []
for lv in levels:
    entry = {"id": f"L{lv['lv']:02d}", "version": 1}
    entry.update(lv)
    out.append(entry)

doc = {"format": FORMAT, "source": "zeydusht/reverse-solver index.html LEVELS", "levels": out}
dst = root / "Assets" / "_Game" / "Levels" / "levels.json"
# One level per line: small diffs when a single level changes, still valid JSON.
lines = ",\n".join(json.dumps(l, ensure_ascii=False, separators=(",", ":")) for l in out)
head = json.dumps({k: v for k, v in doc.items() if k != "levels"}, ensure_ascii=False)[:-1]
dst.write_text(head + ',"levels":[\n' + lines + "\n]}\n", encoding="utf-8", newline="\n")
json.loads(dst.read_text(encoding="utf-8"))  # must round-trip
print(f"{len(out)} levels -> {dst.relative_to(root)}")
