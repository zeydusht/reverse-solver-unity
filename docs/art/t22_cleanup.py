"""Read-only inventory, conservative reference evidence, no removal operations."""
from pathlib import Path
import os, json, csv, re, hashlib
from collections import defaultdict, Counter
from datetime import datetime
ROOT=Path(__file__).resolve().parents[2]; WORK=ROOT/'docs/art/task22'
errors=[];files=[];dirs=[]
def walk(base):
 def err(e): errors.append(str(e))
 for dp,dn,fn in os.walk(base,onerror=err):
  p=Path(dp);dirs.append(p)
  for n in fn:
   q=p/n
   try: files.append((q,q.stat().st_size))
   except OSError as e:errors.append(str(e))
for base in ['Assets/Art','docs/art']:walk(ROOT/base)
text_ext={'.cs','.prefab','.unity','.asset','.mat','.json','.txt','.md','.py','.ps1','.js','.yaml','.yml','.html','.asmdef','.meta'}
guidrefs=defaultdict(set);namerefs=defaultdict(set);corpus=[]
for base in ['Assets','Tools','docs/art']:
 for dp,dn,fn in os.walk(ROOT/base):
  dn[:]=[n for n in dn if n not in {'.deps','.task15-python','.task16-python','.task15-pip-cache','.chrome-task6','Redesign_T22','task22','__pycache__'}]
  for n in fn:
   p=Path(dp)/n
   if p.suffix.lower() not in text_ext or p.name in {'TEMIZLIK_ONERI.md','GECE_LOG.md','DENETIM_2026-11-03.md'} or p.name.startswith('t22_'):continue
   try:
    if p.stat().st_size>8_000_000:continue
    txt=p.read_text(encoding='utf-8-sig',errors='replace'); rel=p.relative_to(ROOT).as_posix();corpus.append((rel,txt))
    if p.suffix=='.meta':continue
    for g in set(re.findall(r'\b[0-9a-f]{32}\b',txt)):guidrefs[g].add(rel)
    for token in set(re.findall(r'[\w.-]+',txt)):
     if len(token)>3:namerefs[token.casefold()].add(rel)
   except OSError as e:errors.append(str(e))
runtime_hash=defaultdict(list)
for dp,dn,fn in os.walk(ROOT/'Assets'):
 if 'Resources' not in Path(dp).parts and 'StreamingAssets' not in Path(dp).parts:continue
 for n in fn:
  p=Path(dp)/n
  if p.suffix.lower() in {'.png','.jpg','.jpeg','.wav','.json'}:
   try:runtime_hash[hashlib.sha256(p.read_bytes()).hexdigest()].append(p.relative_to(ROOT).as_posix())
   except OSError as e:errors.append(str(e))
tool_text='\n'.join(t for r,t in corpus if r.startswith('Tools/') or r.endswith(('.py','.ps1')))
rows=[]
for p,size in files:
 rel=p.relative_to(ROOT).as_posix(); guid='';meta=p.with_name(p.name+'.meta');gr=[]
 if p.suffix=='.meta':meta=p
 if meta.exists():
  try:
   m=re.search(r'^guid:\s*([a-f0-9]{32})',meta.read_text(encoding='utf-8-sig',errors='replace'),re.M)
   if m:guid=m[1];gr=sorted(guidrefs.get(guid,[]))
  except OSError as e:errors.append(str(e))
 nr=sorted(set(namerefs.get(p.name.casefold(),set())) | set(namerefs.get(p.stem.casefold(),set())))
 nr=[s for s in nr if s!=rel and s!=rel+'.meta']
 rt=[]
 if p.suffix.lower() in {'.png','.jpg','.jpeg','.wav','.json'}:
  try:rt=runtime_hash.get(hashlib.sha256(p.read_bytes()).hexdigest(),[])
  except OSError as e:errors.append(str(e))
 category='Kalmalı'; reason='Belge/kaynak; onay ve dinamik yükleme belirsizliğinde koru.'
 cache=next((x for x in p.parts if x in {'.chrome-task6','.task15-pip-cache','__pycache__'}),None)
 if cache and not gr and not rt and not nr:
  category='Güvenle silinebilir';reason='Yeniden üretilebilir araç önbelleği; taranan ad/GUID ve çalışma zamanı kopya kanıtı yok.'
 elif rt or gr:
  reason='Çalışma zamanı birebir içerik kopyası veya Assets GUID referansı var; kaynak olarak koru.'
 elif '/References/' in rel or '/Reference/' in rel or 'TutorialHand' in rel or '/Redesign_T22/' in rel:
  reason='Referans/eğitim eli/yeni onaysız T22 adayı; görev kapsamı gereği koru.'
 elif any(x in p.parts for x in ['Task19','Task19B','Task19C','drafts','backups','task19c','task19d','task20','task21']):
  category='Arşive taşınabilir';reason='Geçmiş taslak veya inceleme kaynağı; aktiflik kanıtı yok fakat referans/üretim değeri var. Ad eşleşmeleri kanıt dosyasında.'
 elif '.task15-python' in p.parts or '.task16-python' in p.parts:
  reason='Mevcut üretim betiklerinin Python/Pillow bağımlılığı; salt önbellek sayılmadı.'
 elif nr:
  reason='Ad/kök ad eşleşmesi mevcut; manifest/betik veya dinamik anahtar olasılığı nedeniyle koru.'
 if 'Resources' in p.parts or 'StreamingAssets' in p.parts:reason='Çalışma zamanı dizininde; dinamik yükleme için koru.'
 rows.append(dict(path=rel,bytes=size,MB=round(size/1_000_000,6),group=category,guid=guid,guid_refs='; '.join(gr),name_refs='; '.join(nr[:30]),runtime_hash_matches='; '.join(rt),reason=reason))
with (WORK/'temizlik-dosyalar.csv').open('w',encoding='utf-8-sig',newline='') as f:
 w=csv.DictWriter(f,fieldnames=list(rows[0]));w.writeheader();w.writerows(rows)
folder_rows=[]
for p in dirs:
 rel=p.relative_to(ROOT).as_posix();rs=[r for r in rows if r['path'].startswith(rel+'/')]
 folder_rows.append(dict(path=rel,bytes=sum(r['bytes'] for r in rs),MB=round(sum(r['bytes'] for r in rs)/1_000_000,6),files=len(rs),groups=dict(Counter(r['group'] for r in rs))))
(WORK/'temizlik-klasorler.json').write_text(json.dumps(folder_rows,ensure_ascii=False,indent=2),encoding='utf-8')
(WORK/'temizlik-okuma-hatalari.json').write_text(json.dumps(errors,ensure_ascii=False,indent=2),encoding='utf-8')
totals={g:sum(r['bytes'] for r in rows if r['group']==g)/1_000_000 for g in ['Güvenle silinebilir','Arşive taşınabilir','Kalmalı']}
md=['# Task22 — Temizlik önerisi (silme ve taşıma yapılmadı)','',f'Tarama: {datetime.now().astimezone().isoformat(timespec="seconds")}. MB = 1.000.000 bayt. {len(rows)} okunabilir dosya, {len(folder_rows)} klasör.','',
'Dosya başına yol, boyut, GUID, Assets GUID referansları, ad/kök ad eşleşmeleri ve Resources/StreamingAssets ile SHA-256 eşitliği: [tam CSV](task22/temizlik-dosyalar.csv). Her klasörün toplamı: [klasör JSON](task22/temizlik-klasorler.json). [Okuma hataları](task22/temizlik-okuma-hatalari.json).', '',
'Ad eşleşmesi kullanımın kesin kanıtı değildir; belgeler ve kaynak manifestleri de eşleşebilir. GUID kendi .meta dosyasından sayılmaz. Kaynaklar oyuna kopyalanabildiğinden Resources/StreamingAssets dosyalarıyla içerik eşitliği ayrıca tarandı. `Art.cs`, `MenuArt.cs` ve `Tools/art_choice.py` dinamik yükleme/kopyalama yollarını gösterir. Eski Task klasörü olması silinebilir sayılması için yeterli değildir.', '',
'Onay bilgisi kesin olmayan sanat kaynakları güvenle silinebilir grubuna alınmadı. Python paketleri mevcut üretim araçlarına gerekli olabildiği için korundu. Erişilemeyen dizinler için boyut/kullanım iddiası yok; aşağıdaki toplamlar yalnız okunabilen dosyalardır.','']
for g in totals:
 md+=['## '+g,'','| Yol / alt kapsam | Boyut (MB) | Neden |','|---|---:|---|']
 buckets=defaultdict(list)
 for r in rows:
  if r['group']!=g:continue
  parts=r['path'].split('/'); key='/'.join(parts[:4]) if parts[0]=='Assets' else '/'.join(parts[:3])
  buckets[key].append(r)
 for k,rs in sorted(buckets.items(),key=lambda x:-sum(r['bytes'] for r in x[1])):
  reasons=' '.join(dict.fromkeys(r['reason'] for r in rs))
  md.append(f'| `{k}` ({len(rs)} dosya; bu gruba giren alt kapsam) | {sum(r["bytes"] for r in rs)/1_000_000:.3f} | {reasons} |')
 md+=['',f'Bu grup: **{totals[g]:.3f} MB**.','']
md += [f'Koşullu temizlik kazanımı: **{totals["Güvenle silinebilir"]:.3f} MB**. Arşiv grubu **{totals["Arşive taşınabilir"]:.3f} MB**; aynı disk içinde arşive taşımak disk alanı kazandırmaz. İki toplam birbirine eklenmedi.','', 'Bu liste PM incelemesi içindir; dosya silme/taşıma işlemi yapılmadı. Canlı/build kullanımını kesinleştirmeden sanat kaynakları kaldırılmamalı.']
(ROOT/'docs/art/TEMIZLIK_ONERI.md').write_text('\n'.join(md)+'\n',encoding='utf-8')
(WORK/'temizlik-ozet.json').write_text(json.dumps({'files':len(rows),'folders':len(folder_rows),'totalsMB':totals,'errors':len(errors)},ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({'files':len(rows),'totalsMB':totals,'errors':len(errors)},ensure_ascii=False))
