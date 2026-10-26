#!/bin/bash
# Preview builds publish to gh-pages but must not change the root while Build 5 waits for Zeyd:
#   bash Tools/preview_mode.sh on   -> flags off, new icon only at ?set=redesign (do NOT commit this state)
#   bash Tools/preview_mode.sh off  -> back to the committed Build 5 state
F=Assets/_Game/Presentation/SetFlags.cs; T=Assets/WebGLTemplates/ReverseSolver/index.html
if [ "$1" = on ]; then
  python - "$F" "$T" <<'PY'
import sys, re
f, t = sys.argv[1], sys.argv[2]
s = open(f, encoding='utf-8').read()
s = s.replace('RedesignDefault = true;', 'RedesignDefault = false;').replace('AskFun = true;', 'AskFun = false;')
open(f, 'w', encoding='utf-8', newline='').write(s)
s = open(t, encoding='utf-8').read()
s = s.replace('if (!/[?&]set=(classic|zeydbot|designs|levels)\b/.test(location.search)) {', 'if (/[?&]set=redesign/.test(location.search)) {')
open(t, 'w', encoding='utf-8', newline='').write(s)
PY
else
  git checkout -- $F $T
fi
grep -c "RedesignDefault = true" $F; grep -c "set=redesign" $T
