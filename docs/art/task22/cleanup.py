"""Read-only cleanup analysis. Enumerates only Assets/Art and docs/art;
text usage index is restricted to Assets. Never deletes/moves/renames files.
"""
from pathlib import Path
import os,re,json,csv,hashlib,collections,datetime
R=Path(__file__).resolve().parents[3];W=R/'docs/art/task22'
def rel(p):return p.relative_to(R).as_posix()
errors=[];files=[];directories=[]
for root in [R/'Assets/Art',R/'docs/art']:
 for current,ds,fs in os.walk(root,onerror=lambda e:errors.append({'path':str(e.filename),'error':str(e)})):
  ds[:]=[d for d in ds if d not in ['Library','Logs','Builds']]
  directories.append(Path(current))
  for name in fs:
   p=Path(current)/name
   # Prior inventory outputs are excluded from their own evidence index, but included in inventory.
   try:files.append((p,p.stat().st_size))
   except OSError as e:errors.append({'path':str(p),'error':str(e)})
print(f'inventory {len(files)} files; {len(errors)} inaccessible directories',flush=True)
exts={'.cs','.json','.unity','.prefab','.asset','.meta','.mat','.controller','.anim','.overridecontroller','.txt','.md','.manifest','.html','.js','.uxml','.uss','.shader','.cginc','.asmdef','.asmref','.ps1','.py','.cjs','.svg','.shadervariants','.rendertexture','.spriteatlas','.playable','.inputactions'}
texts={};asset_files=[];guidrefs=collections.defaultdict(set);namerefs=collections.defaultdict(set);res_hash=collections.defaultdict(list)
for current,ds,fs in os.walk(R/'Assets',onerror=lambda e:errors.append({'path':str(e.filename),'error':str(e)})):
 ds[:]=[d for d in ds if d not in ['Library','Logs','Builds','Redesign_T22']]
 for name in fs:
  p=Path(current)/name;rp=rel(p);asset_files.append(rp)
  if p.suffix.lower() in exts:
   try:
    s=p.read_text(encoding='utf8',errors='replace');texts[rp]=s
    for guid in set(re.findall(r'(?<![0-9a-fA-F])[0-9a-fA-F]{32}(?![0-9a-fA-F])',s)):guidrefs[guid.lower()].add(rp)
    # Includes resource keys, file names, paths, and dynamically prefixed t20/m13 names.
    for token in set(re.findall(r'[A-Za-z0-9_][A-Za-z0-9_./\\-]{2,}',s)):
     token=token.replace('\\','/').strip('./').lower()
     for key in [token,token.rsplit('/',1)[-1]]:
      namerefs[key].add(rp)
      if '.' in key:namerefs[key.rsplit('.',1)[0]].add(rp)
      if re.match(r'^(t\d+|m\d+)-',key):namerefs[re.sub(r'^(t\d+|m\d+)-','',key)].add(rp)
   except OSError as e:errors.append({'path':rp,'error':str(e)})
  if ('Resources' in p.parts or 'StreamingAssets' in p.parts) and p.suffix.lower() not in exts and p.suffix.lower()!='.meta':
   try:res_hash[hashlib.sha256(p.read_bytes()).hexdigest()].append(rp)
   except OSError as e:errors.append({'path':rp,'error':str(e)})
print(f'usage index {len(texts)} Assets text files; {sum(map(len,res_hash.values()))} runtime content files',flush=True)
# Authoring references distinguish historical source from unreferenced caches.
docrefs=collections.defaultdict(set)
for p,size in files:
 if 'docs/art' not in rel(p) or '/task22/' in rel(p) or p.name in ['GECE_LOG.md','DENETIM_2026-11-03.md','TEMIZLIK_ONERI.md'] or p.suffix.lower() not in exts or size>5_000_000:continue
 try:
  s=p.read_text(encoding='utf8',errors='replace')
  for token in set(re.findall(r'[A-Za-z0-9_][A-Za-z0-9_./\\-]{2,}',s)):
   tok=token.replace('\\','/').strip('./').lower();docrefs[tok].add(rel(p));docrefs[tok.rsplit('/',1)[-1]].add(rel(p))
 except OSError:pass
rows=[];cache_roots={'.chrome-task6','.task15-pip-cache','.task15-python','.task16-python'}
sha_cache={}
for idx,(p,size) in enumerate(files):
 rp=rel(p);name=p.name.lower();stem=p.stem.lower();ownmeta=rp if p.suffix=='.meta' else rp+'.meta';guid=None
 meta=texts.get(ownmeta,'')
 m=re.search(r'^guid:\s*([0-9a-fA-F]{32})',meta,re.M)
 if m:guid=m.group(1).lower()
 gu=sorted(guidrefs.get(guid,set())-{ownmeta}) if guid else []
 nr=set(namerefs.get(name,set()))|set(namerefs.get(stem,set()))|set(namerefs.get(rp.lower(),set()))
 nr={x for x in nr if x not in [rp,ownmeta]}
 runtime_refs=sorted(x for x in nr|set(gu) if not x.startswith('Assets/Art/'))
 author_refs=sorted(x for x in nr|set(gu) if x.startswith('Assets/Art/'))
 dr=sorted((set(docrefs.get(name,set()))|set(docrefs.get(rp.lower(),set())))-{rp})
 copies=[];sha=''
 if rp.startswith('Assets/Art/') and p.suffix.lower() not in ['.meta','.md','.json','.html']:
  try:sha=hashlib.sha256(p.read_bytes()).hexdigest();copies=res_hash.get(sha,[])
  except OSError as e:errors.append({'path':rp,'error':str(e)})
 resource_path='Resources' in p.parts
 cache=next((x for x in cache_roots if x in p.parts),None)
 live=bool(runtime_refs or copies or resource_path)
 if live:
  group='Kalmalı';reason='Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var.'
 elif 'Redesign_T22' in p.parts or '/task22/' in rp or (rp.startswith('docs/art/') and len(p.relative_to(R/'docs/art').parts)==1):
  group='Kalmalı';reason='Task22 teslimi; henüz onaysız yeni aday, bu görevin silme önerisine girmez.'
  if rp.startswith('docs/art/') and len(p.relative_to(R/'docs/art').parts)==1:
   reason='Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı.'
 elif any(x in rp for x in ['/References/','/Reference/','/TutorialHand/','/Tutorial/','/Task20/','/Task21/','/Task14/','/Task15/','/Task17/','/Paintings/']):
  group='Kalmalı';reason='Onaylı aktif set, kaynak referans, tablo veya eğitim varlığı; runtime başvurusu bulunmasa da koruma kararı.'
 elif cache:
  # Python trees have unreadable children; hold the entire environment to avoid claiming a complete byte total.
  if cache in ['.task15-python','.task16-python']:
   group='Kalmalı';reason='Araç ortamının alt dizinleri erişim reddi verdi; boyut/kullanım bütünü doğrulanamadı. Yeniden kurulabilir ama şimdilik beklet.'
  else:
   group='Güvenle silinebilir';reason='Yeniden üretilebilir tarayıcı/pip önbelleği; Assets runtime ad/GUID/hash kullanım kanıtı yok. Art araçları yeniden çalıştırılırsa cache/bağımlılık yeniden kurulur.'
 elif author_refs or dr:
  group='Kalmalı' if (p.suffix.lower() in ['.py','.ps1','.json','.ttf','.otf'] or any(x in rp for x in ['Task20/','Task21/','Task14/','Task15/','Task17/','Paintings/','References/','Reference/','TutorialHand/','Tutorial/'])) else 'Arşive taşınabilir'
  reason='Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. '+('Üretim/referans kaynağı korunmalı.' if group=='Kalmalı' else 'Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli.')
 elif any(x in rp for x in ['/Task19/','/Task19B/','/Task19C/','/Task19D/','/drafts/','/backups/','/Review/','/Ornekler/','/task19c/','/task19d/','/corridor-victory/']):
  group='Arşive taşınabilir';reason='Eski taslak/onay geçmişi; Assets içinde ad/GUID/runtime hash kullanım kanıtı bulunmadı. Kaynak/onay geçmişi nedeniyle silme önerilmiyor.'
 else:
  group='Kalmalı';reason='Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez.'
 rows.append(dict(path=rp,bytes=size,MB=round(size/1_000_000,6),group=group,gameUse='kanıt var' if live else 'runtime kanıtı bulunmadı',guid=guid or '',guidReferences='; '.join(gu),assetNameReferences='; '.join(sorted(nr)),runtimeReferences='; '.join(runtime_refs),runtimeHashCopies='; '.join(copies),authoringReferences='; '.join(author_refs+dr),sha256=sha,reason=reason))
 if idx%2000==0:print(f'checked {idx}/{len(files)}',flush=True)
# A .meta tracks its asset: never propose separate cleanup of one side.
by={r['path']:r for r in rows}
for row in rows:
 if row['path'].endswith('.meta') and row['path'][:-5] in by:
  owner=by[row['path'][:-5]]
  if row['gameUse']!='kanıt var':
   row['group']=owner['group'];row['reason']='Eş varlıkla birlikte değerlendirilir: '+owner['reason']
for row in rows:
 if row['path'].endswith('.meta') and (R/row['path'][:-5]).is_dir():
  children=[r for r in rows if r['path'].startswith(row['path'][:-5]+'/')]
  if any(r['group']=='Kalmalı' for r in children):row['group']='Kalmalı';row['reason']='Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez.'
errors=list({e['path']:e for e in errors}.values())
fields=list(rows[0])
with (W/'temizlik-dosyalar.csv').open('w',newline='',encoding='utf-8-sig') as f:w=csv.DictWriter(f,fields);w.writeheader();w.writerows(rows)
(W/'temizlik-erisim-hatalari.json').write_text(json.dumps(errors,ensure_ascii=False,indent=2),encoding='utf8')
# Per-directory inventory totals are recursive and explicitly nonadditive.
dirs=[]
for p in directories:
 rp=rel(p);children=[r for r in rows if r['path'].startswith(rp+'/')];n=len(children);size=sum(r['bytes'] for r in children)
 groups=collections.Counter(r['group'] for r in children);usage=sum(r['gameUse']=='kanıt var' for r in children)
 inaccessible=any(str(p).lower() in e['path'].lower() for e in errors)
 dirs.append(dict(path=rp,files=n,bytes=size,MB=round(size/1e6,6),gameEvidenceFiles=usage,safeFiles=groups['Güvenle silinebilir'],archiveFiles=groups['Arşive taşınabilir'],keepFiles=groups['Kalmalı'],partial=inaccessible))
for e in errors:
 p=Path(e['path'])
 try:rp=rel(p)
 except ValueError:continue
 if rp not in {d['path'] for d in dirs}:dirs.append(dict(path=rp,files='',bytes='',MB='',gameEvidenceFiles='',safeFiles=0,archiveFiles=0,keepFiles='',partial=True))
with (W/'temizlik-klasorler.csv').open('w',newline='',encoding='utf-8-sig') as f:w=csv.DictWriter(f,list(dirs[0]));w.writeheader();w.writerows(dirs)
tot=collections.Counter();counts=collections.Counter()
for r in rows:tot[r['group']]+=r['bytes'];counts[r['group']]+=1
summary={'scannedAt':datetime.datetime.now().astimezone().isoformat(),'files':len(rows),'folders':len(dirs),'AssetsTextFiles':len(texts),'runtimeHashFiles':sum(map(len,res_hash.values())),'totalsBytes':dict(tot),'counts':dict(counts),'accessErrors':len(errors),'safeMB':tot['Güvenle silinebilir']/1e6,'archiveMB':tot['Arşive taşınabilir']/1e6,'potentialMB':(tot['Güvenle silinebilir']+tot['Arşive taşınabilir'])/1e6,'actualRecoveredMB':0}
(W/'temizlik-ozet.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf8')
text='''# Task22 — Temizlik önerisi (hiçbir işlem uygulanmadı)

Tarama kapsamı yalnız **Assets/Art/** ve **docs/art/**. Kullanım araması yalnız **Assets/** metinleri, .meta GUID'leri ve Resources/StreamingAssets içerik eşitliğidir. Library/, Logs/, Builds/ taranmadı. Git, Unity ve build çalıştırılmadı. Silme/taşıma/yeniden adlandırma yapılmadı.

## Kanıt ve güven sınırı

- GUID kullanımı kendi .meta satırı çıkarılarak Assets içindeki tüm okunabilir kod/prefab/sahne/manifest/metinlerde arandı. Kodla ad yükleme ve t20-/m13- gibi runtime takma adları da tarandı.
- Resources içindeki bir kopya kaynakla SHA-256 eşitse kaynak **kalmalı**. GUID ile kullanılmıyor diye kopyanın üretim kaynağı silinebilir sayılmadı.
- Yalnız manifest/belge bağlantısı canlı oyunda kullanım kanıtı değildir; ayrı authoringReferences alanıdır. Dinamik ad birleştirme ve dış build araçları tümüyle kanıtlanamaz; şüpheli dosya kalmalı grubuna alındı.
- Task19/19B/19C ve iptal adayı taslaklar otomatik silinebilir sayılmadı: onay/yeniden üretilebilirlik kesin değilse arşiv önerildi. Referanslar, eğitim eli, onaylı aktif setler korunur.
- .task15-python ve .task16-python altlarında erişim reddi var. Bu ortamlar **kalmalı / ölçüm eksik**; bilinmeyen baytlar toplam kazanca dahil değil. Önbelleklerin silinmesi gelecekte art scriptleri için cache/bağımlılık kurulumunu gerektirebilir.
- MB = 1.000.000 bayt. Dosya toplamı çakışmaz. Klasör CSV'si recursive olduğundan satırları toplanmaz. Listeler mevcut tarama anının fotoğrafıdır; Claude eşzamanlı çalıştığı için uygulama öncesi yeniden kontrol gerekir.

## Her dosya ve her klasör

Tam döküm: [temizlik-dosyalar.csv](task22/temizlik-dosyalar.csv) — her dosya için yol, boyut, grup, oyun kullanımı, GUID, GUID/ad başvuruları, Resources hash eşleri ve gerekçe. [temizlik-klasorler.csv](task22/temizlik-klasorler.csv) — her erişilebilir klasörün boyutu ve grupları. [Erişim hataları](task22/temizlik-erisim-hatalari.json) erişilemeyen dizinleri ayrıca gösterir. Bu CSV'ler raporun ayrılmaz ekidir; burada aşağıda okunur karar özeti bulunur.

'''
for group in ['Güvenle silinebilir','Arşive taşınabilir','Kalmalı']:
 text+='## '+group+'\n\n| Yol | Boyut (MB) | Neden |\n|---|---:|---|\n'
 # Aggregate each top-level art category / immediate docs subdirectory without overlap.
 buckets=collections.defaultdict(list)
 for row in rows:
  if row['group']!=group:continue
  parts=row['path'].split('/')
  key='/'.join(parts[:3]) if row['path'].startswith('Assets/Art/') else '/'.join(parts[:3])
  buckets[key].append(row)
 for key,rr in sorted(buckets.items(),key=lambda kv:-sum(x['bytes'] for x in kv[1])):
  size=sum(x['bytes'] for x in rr);why=rr[0]['reason'];live=sum(r['gameUse']=='kanıt var' for r in rr)
  text+=f'| `{key}` ({len(rr)} dosya; bu gruba düşen kısmı) | {size/1e6:.3f} | {why} Oyun kullanım kanıtlı dosya: {live}. |\n'
 text+=f'\nGrup toplamı: **{tot[group]/1e6:.3f} MB**, {counts[group]} dosya. Ayrı dosyaların gerçek grup ve gerekçesi CSV’de.\n\n'
text+=f'''## Kazanç hesabı

- Güvenle silinebilir, yeniden kurulabilir önbellek: **{summary['safeMB']:.3f} MB**.
- Arşive taşınabilir tarihçe: **{summary['archiveMB']:.3f} MB**. Aynı diskte başka klasöre taşımak alan kazandırmaz; ancak dış arşivleme sonrası ayrı onaylı kaldırma ile bu miktar kazanılır.
- **Toplam olası alan kazanımı: {summary['potentialMB']:.3f} MB** (güvenli cache + dış arşivleme senaryosu; erişilemeyen Python içeriği hariç).
- **Bu görevde gerçekte kazanılan: 0 MB.** Hiçbir dosya silinmedi veya taşınmadı.

PM dosya listesini kontrol edip Zeyd onayladığında ayrı bir görev uygulanabilir. Bu rapor silme yetkisi veya silme komutu içermez.
'''
(R/'docs/art/TEMIZLIK_ONERI.md').write_text(text,encoding='utf8')
print(json.dumps(summary,ensure_ascii=False),flush=True)
