"""Writes the factory agents' notes into redesign.json: the designer's FİKİR lines into
"aha", the psychologist's verdict into "psych" ("KABUL" unless a RET line names the id).

    python Tools/apply_agent_notes.py Builds/fabrika/ajan/tasarimci_*.txt Builds/fabrika/ajan/psikolog_*.txt

Only the "aha" and "psych" fields of the matching finalists change (one level per line).
"""
import json
import re
import sys

PATH = "Assets/_Game/Levels/redesign.json"
aha, ret = {}, {}
for f in sys.argv[1:]:
    for line in open(f, encoding="utf-8"):
        m = re.match(r"FİKİR: (C\d\d-[0-9a-f]{8}): (.+)", line.strip())
        if m: aha[m.group(1)] = m.group(2).strip()
        m = re.match(r"RET: (C\d\d-[0-9a-f]{8}): (.+)", line.strip())
        if m: ret[m.group(1)] = "RET: " + m.group(2).strip()
out, n_aha, n_psych = [], 0, 0
for line in open(PATH, encoding="utf-8").read().split("\n"):
    m = re.search(r'"candidate":"(C\d\d-[0-9a-f]{8})"', line)
    if m:
        cid = m.group(1)
        if cid in aha:
            line = re.sub(r'"aha":(null|"(?:[^"\\]|\\.)*")', lambda _: '"aha":' + json.dumps(aha[cid], ensure_ascii=False), line, count=1); n_aha += 1
        verdict = ret.get(cid, "KABUL (psikolog, tur 2)" if cid in aha else None)
        if verdict:
            line = re.sub(r'"psych":(null|"(?:[^"\\]|\\.)*")', lambda _: '"psych":' + json.dumps(verdict, ensure_ascii=False), line, count=1); n_psych += 1
    out.append(line)
text = "\n".join(out)
json.loads(text)
open(PATH, "w", encoding="utf-8", newline="\n").write(text)
print(f"aha {n_aha}, psych {n_psych}, ret {sum(1 for k in ret if k in text)}")
