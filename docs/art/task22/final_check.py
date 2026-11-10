from pathlib import Path
from collections import Counter
from html.parser import HTMLParser
import json,re
R=Path(__file__).resolve().parents[3];W=R/'docs/art/task22';O=R/'Assets/Art/Redesign_T22'
F=json.loads((W/'findings.json').read_text(encoding='utf8'))
assert len(F)==63 and len({f['id'] for f in F})==63
for filename,key in [('validation.json','findings'),('consolidation.json','mainFindings')]:
 p=W/filename;data=json.loads(p.read_text(encoding='utf8'));data[key]=len(F);p.write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
report=(R/'docs/art/DENETIM_2026-11-03.md').read_text(encoding='utf8')
assert len(re.findall(r'^\| (?:S\d+|U\d+|T\d+|İ\d+) \|',report,re.M))==63
class Parser(HTMLParser):
 def __init__(self):super().__init__();self.ids=[];self.links=[];self.images=0;self.pairs=0;self.articles=0
 def handle_starttag(self,t,attrs):
  a=dict(attrs)
  if 'id' in a:self.ids.append(a['id'])
  for k in ['href','src']:
   if k in a:self.links.append(a[k])
  if t=='img':
   self.images+=1
   if 'once-sonra' in a.get('src',''):self.pairs+=1
  if t=='article':self.articles+=1
page=(O/'inceleme.html').read_text(encoding='utf8')
# A concurrent author appended the same 101–113 families with fuller variant previews.
# Keep that complete block; remove only our duplicate HTML section, never any file.
if '<!-- T22_AI_EK_START -->' in page and '<!-- T22_ALTERNATIVES_START -->' in page:
 page=re.sub(r'<!-- T22_ALTERNATIVES_START -->[\s\S]*?<!-- T22_ALTERNATIVES_END -->','',page)
 (O/'inceleme.html').write_text(page,encoding='utf8')
p=Parser();p.feed(page)
assert p.articles==35 and p.pairs==36,(p.articles,p.images,p.pairs)
assert len(p.ids)==len(set(p.ids)),'Duplicate DOM ids'
for href in p.links:
 if href.startswith('#'):assert href[1:] in p.ids,href
 else:assert (O/href).exists(),href
C=json.loads((W/'temizlik-final-ozet.json').read_text(encoding='utf8'));D=json.loads((W/'teslim-ozet.json').read_text(encoding='utf8'))
assert abs(C['potentialMB']-D['cleanupPotentialMB'])<.000001
log=(R/'docs/art/GECE_LOG.md').read_text(encoding='utf8')
assert 'Sabah teslim özeti' in log and '63 kayıt' in log
result=dict(finalFindings=len(F),severity=dict(Counter(f['severity'] for f in F)),reviewFamilies=p.articles,beforeAfter=p.pairs,previewImages=p.images,candidates=150,cleanupPotentialMB=C['potentialMB'],status='PASS')
(W/'final-check.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(result,ensure_ascii=False))
