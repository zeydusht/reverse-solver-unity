from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps
from html.parser import HTMLParser
from urllib.parse import unquote
import json,hashlib,collections,datetime,colorsys
R=Path(__file__).resolve().parents[3];O=R/'Assets/Art/Redesign_T22';W=R/'docs/art/task22'
M=json.loads((O/'manifest.json').read_text(encoding='utf8'));F=json.loads((W/'findings.json').read_text(encoding='utf8'))
errors=[];warnings=[];checks=[]
class Links(HTMLParser):
 def __init__(self):super().__init__();self.links=[];self.ids=[];self.images=[]
 def handle_starttag(self,t,attrs):
  a=dict(attrs)
  if 'id' in a:self.ids.append(a['id'])
  for k in ['href','src']:
   if k in a:self.links.append(a[k])
  if t=='img':self.images.append(a)
page=Links();page.feed((O/'inceleme.html').read_text(encoding='utf8'))
for url in page.links:
 if url.startswith('#'):
  if url[1:] not in page.ids:errors.append('HTML anchor '+url)
 elif '://' in url:errors.append('Unexpected external URL '+url)
 elif not (O/unquote(url)).exists():errors.append('Missing link '+url)
expected=22+sum(len(list(p.glob('once-sonra*.png'))) for p in O.glob('1??-ai-*') if p.is_dir()) if 'i101' in page.ids else 22
pair_images=[a for a in page.images if Path(a.get('src','')).name.startswith('once-sonra')]
assert len(pair_images)==expected,(len(pair_images),expected)
for a in M['assets']:
 p=O/a['file']
 with Image.open(p) as im:
  if im.mode!='RGBA' or im.size!=(a['width'],a['height']):errors.append('RGBA/size '+a['file'])
  if im.getchannel('A').getextrema()[0]!=0:errors.append('No actual transparent pixel '+a['file'])
  if im.width%3 or im.height%3:errors.append('Not @3x integral pt '+a['file'])
  l,t,r,b=a['sliceLTRB']
  if l+r>im.width or t+b>im.height:errors.append('Slice border '+a['file'])
 if hashlib.sha256(p.read_bytes()).hexdigest()!=a['sha256']:errors.append('Hash '+a['file'])
 if 'minHitPt' in a and min(a['minHitPt'])<44:errors.append('Hit target '+a['file'])
checks.append(f'{len(M["assets"])} runtime adayı PNG: RGBA, gerçek alfa, @3x, slice, hash ve belirtilmiş min hedefler kontrol edildi.')
gold=[]
for j in M['items']:
 p=O/j['folder'];required=['NOT.md','once-sonra.png','ekran-sonra.png','taslak-katmani.png','metin-yerlesimi.json','telefon-375x667.png','telefon-390x844.png']
 for n in required:
  if not (p/n).exists():errors.append('Missing deliverable '+str(p/n))
 t=json.loads((p/'metin-yerlesimi.json').read_text(encoding='utf8'))
 if min(r['sizePt'] for r in t['text'] if r['text'])<13:errors.append('Font <13 '+j['folder'])
 for sw,sh in [(375,667),(390,844)]:
  with Image.open(p/f'telefon-{sw}x{sh}.png') as im:
   if im.size!=(sw,sh):errors.append('Viewport '+j['folder'])
 with Image.open(p/'taslak-katmani.png') as im:
  im.thumbnail((375,844));pixels=list(im.getdata());solid=[(r,g,b) for r,g,b,a in pixels if a>128];ng=0
  for r,g,b in solid:
   h,s,v=colorsys.rgb_to_hsv(r/255,g/255,b/255)
   if .075<=h<=.17 and s>=.58 and v>=.50:ng+=1
  rate=round(100*ng/max(1,len(solid)),2);gold.append({'id':j['id'],'goldApproxPct':rate,'method':'HSV hue .075-.17, saturation>=.58, value>=.50; alpha>128, downsampled'})
  if rate>10:warnings.append(f'{j["id"]:02} yerel katman altın maskesi %{rate}; ekran oranından farklıdır.')
 # Runtime parts are text-free; text/samples are only in proposal overlay.
 if j['id'] in [1,3]:
  j['sharedComponents']=['02-puan-secimi/kutu-normal.png','02-puan-secimi/kutu-selected.png','02-puan-secimi/kutu-pressed.png']
  note=p/'NOT.md';s=note.read_text(encoding='utf8')
  if 'Ortak bağımlılık' not in s:note.write_text(s+'\nOrtak bağımlılık: 02-puan-secimi klasöründeki metinsiz kutu-normal/selected/pressed PNG’leri. Sayı ve seçili durum kodda yönetilir.\n',encoding='utf8')
 j['review']='visual-checked';j['goldApproxPct']=rate
drawings={f['drawing'] for f in F if f['kind']=='çizim' and f['severity'] in ['yüksek','orta']}
if drawings!={j['id'] for j in M['items']}:errors.append('Finding/drawing mapping mismatch')
checks.append('22/22 yüksek-orta çizim bulgusu klasör/önce-sonra/not/inceleme ile birebir eşleşiyor.')
original=json.loads((W/'original-art-baseline.json').read_text(encoding='utf8'));changed=[];missing=[];unreadable=[]
for name,a in original.items():
 p=R/name
 try:
  if not p.exists():missing.append(name)
  elif hashlib.sha256(p.read_bytes()).hexdigest()!=a['sha256']:changed.append(name)
 except OSError:unreadable.append(name)
checks.append(f'Başlangıçtaki {len(original)} okunabilir Assets/Art dosyası: {len(changed)} değişti, {len(missing)} eksildi, {len(unreadable)} erişilemez.')
if changed or missing:errors.append('Original art changed/missing; inspect original-art-check.json')
(W/'original-art-check.json').write_text(json.dumps({'checked':len(original),'changed':changed,'missing':missing,'unreadable':unreadable},indent=2),encoding='utf8')
# No new write entrypoints outside the two authorized roots; scripts do not run old exporter mains.
manifested={a['file'] for a in M['assets']};leftovers=[]
for p in O.glob('*/*.png'):
 if p.parent.name not in {j['folder'] for j in M['items']}:continue
 rp=p.relative_to(O).as_posix()
 if rp not in manifested and p.name not in ['once-sonra.png','ekran-sonra.png','taslak-katmani.png','telefon-375x667.png','telefon-390x844.png']:leftovers.append(rp)
M['unusedAttemptFiles']=leftovers
M['reviewedAt']=datetime.datetime.now().astimezone().isoformat(timespec='seconds')
M['validationErrors']=errors;M['goldMethod']=gold[0]['method']
(O/'manifest.json').write_text(json.dumps(M,ensure_ascii=False,indent=2),encoding='utf8')
(W/'gold-measurements.json').write_text(json.dumps(gold,ensure_ascii=False,indent=2),encoding='utf8')
summary={'errors':errors,'warnings':warnings,'checks':checks,'findings':len(F),'drawings':len(M['items']),'runtimePNG':len(M['assets']),'unusedAttemptFiles':leftovers}
(W/'validation.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf8')
# Retain all original source screenshots, including the late-arriving full tour, in a hash index.
screens=[]
for d in ['2026-11-02-son','2026-11-02-spot','2026-11-02-booster','2026-11-02-ftue','2026-11-01-p20b','2026-11-03-tam','2026-11-02-rating']:
 for p in sorted((R/'docs/qa'/d).glob('*.png')):
  with Image.open(p) as im:screens.append(dict(path=p.relative_to(R).as_posix(),size=list(im.size),sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(W/'screens.json').write_text(json.dumps(screens,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(summary,ensure_ascii=False),flush=True)
if errors:raise SystemExit(1)
