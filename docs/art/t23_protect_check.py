from pathlib import Path
import json,hashlib,re
r=Path(__file__).resolve().parents[2];b=json.loads((r/'docs/art/task23/koruma-baslangic.json').read_text(encoding='utf-8'));diff=[];missing=[];checked=0
for x in b['files']:
 p=r/x['path']
 if not p.exists():missing.append(x['path'])
 elif hashlib.sha256(p.read_bytes()).hexdigest()!=x['sha256']:diff.append(x['path'])
 else:checked+=1
out={'unchanged':checked,'changed':diff,'missing':missing};(r/'docs/art/task23/koruma-sonuc.json').write_text(json.dumps(out,indent=2),encoding='utf-8')
h=r/'Assets/Art/Redesign_T23/inceleme.html';links=re.findall(r'(?:href|src)="([^"]+)"',h.read_text(encoding='utf-8'));bad=[x for x in links if not (h.parent/x).exists()]
print('unchanged',checked,'changed',len(diff),'missing',len(missing),'broken links',bad);print('changed paths',diff[:12]);print('missing paths',missing[:12]);assert not bad
