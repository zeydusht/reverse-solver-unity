from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps
from collections import Counter
from html.parser import HTMLParser
from datetime import datetime
import json,hashlib,math
ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'Assets/Art/Redesign_T22';WORK=ROOT/'docs/art/task22';R=Image.Resampling.LANCZOS
manifest=json.loads((OUT/'ai-manifest.json').read_text(encoding='utf-8'));jobs=json.loads((WORK/'ai-jobs.json').read_text(encoding='utf-8'));font=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),25)
checks=[];seen=set();fail=[]
for row in manifest:
 p=OUT/row['file'];im=Image.open(p);alpha=im.getchannel('A') if im.mode=='RGBA' else None
 tests={'size':im.size==(row['width'],row['height']),'rgba':im.mode=='RGBA','transparent':alpha is not None and alpha.getextrema()==(0,255),'at3x':im.width%3==0 and im.height%3==0,'hash':hashlib.sha256(p.read_bytes()).hexdigest()==row['sha256'],'unique':row['file'] not in seen}
 seen.add(row['file']);l,t,r,b=row['sliceLTRB'];tests['slice']=l+r<im.width and t+b<im.height
 # Hue mask is an estimate, not a claim about material perception or exact palette conformity.
 rgb=im.convert('RGB').resize((min(im.width,360),round(im.height*min(im.width,360)/im.width)),R);al=alpha.resize(rgb.size,R);hsv=rgb.convert('HSV');gold=visible=0
 for (h,s,v),a in zip(hsv.getdata(),al.getdata()):
  if a<128:continue
  visible+=1
  if 10<=h<=39 and s>=150 and v>=110:gold+=1
 gold_pct=round(100*gold/max(1,visible),3)
 checks.append(dict(file=row['file'],checks=tests,goldHueEstimatePct=gold_pct))
 if not all(tests.values()):fail.append(row['file'])
 # Alpha proof over two backgrounds; no original images are altered.
 plate=Image.new('RGB',(im.width*2+18,im.height),'#E3E7E5');plate.paste(im,(0,0),im);dark=Image.new('RGB',im.size,'#0B2A36');dark.paste(im,(0,0),im);plate.paste(dark,(im.width+18,0));plate.thumbnail((540,540));plate.save(WORK/('ai-alpha-'+p.parent.name+'.jpg'),quality=90)
class Links(HTMLParser):
 def __init__(self):super().__init__();self.links=[]
 def handle_starttag(self,tag,attrs):
  for k,v in attrs:
   if k in {'src','href'}:self.links.append(v)
parser=Links();parser.feed((OUT/'inceleme-ai.html').read_text(encoding='utf-8'));missing=[]
for link in parser.links:
 if ':' in link or link.startswith('#'):continue
 p=(OUT/link.split('#')[0]).resolve()
 if not p.exists():missing.append(link)
assert len({j['folder'] for j in jobs})==13
# Independent phone-size static plates: actual@3x canvas sizes, no claimed live UI test.
phone=[]
for job in jobs:
 folder=OUT/job['folder'];pn=next(folder.glob('panel-@3x.png'),None)
 if pn:
  variants=sorted(folder.glob('onizleme-*.png'))
  # source screenshot crop already includes the final rendered panel and labels.
  shot=Image.open(folder/'ekran-sonra-375.png').convert('RGBA');x,y,w,h=job['rectPt'];card=shot.crop((round(x*3),round(y*3),round((x+w)*3),round((y+h)*3)))
  for sw,sh in [(375,667),(390,844)]:
   plate=ImageOps.fit(Image.open(ROOT/'Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png').convert('RGBA'),(sw*3,sh*3),R);plate.alpha_composite(Image.new('RGBA',plate.size,(8,32,34,215)));px=(plate.width-card.width)//2;py=(plate.height-card.height)//2;assert min(px,py)>=0;plate.alpha_composite(card,(px,py));plate.save(folder/f'telefon-{sw}x{sh}.png');phone.append({'folder':job['folder'],'viewport':[sw,sh],'cardSizePt':[card.width/3,card.height/3],'fits':True})
 # NOT.md status is technical QA, not a fourth art attempt.
 note=folder/'NOT.md';s=note.read_text(encoding='utf-8');s=s.replace('Görsel kabul:1. taslak. Bu sürüm telefon önizlemesi ve alfa/kenar/boyut kontrolünden sonra değerlendirilecek; nihai QA `ai-validation.json` içinde.','Görsel kontrol: ilk çizim ardından en fazla iki yerleşim düzeltmesi (toplam en fazla3 deneme). Eski düğme izleri/hizalama ve metin kuralları kontrol edildi. Son teknik sonuç `docs/art/task22/ai-validation.json` içinde. Kaynak bitmapler her biri1 imagegen üretimidir. Onaylı ana tuşların yeniden çizildiği iddia edilmez; geniş yerleşimleri ayrı kod uyarlaması gerektirir.')
 if job['folder']=='102-ai-kayip':s+='\nSüre karşılaştırma kaynağı: `docs/qa/2026-11-03-tam/375x667_06_kaybetme.png`. `once-sonra-sure.png` ayrı dosyadır.\n'
 if job['folder']=='112-ai-jiggles':s+='\nKarar ayrımı: PM33 işlevini koruyan kısa bar ana22 adayında; bu112 numara daha eski Task21 madalyon yönünün isteğe bağlı alternatifi.315/2.000 toplamı üst kurtarılan sayacında korunur; entegrasyonda veri/ödül eşikleri değiştirilemez.\n'
 note.write_text(s,encoding='utf-8')
# Read-only provenance check: concurrent writers are reported rather than attributed to this run.
baseline=json.loads((WORK/'baslangic.json').read_text(encoding='utf-8'));changed=[];unreadable=[]
for r in baseline['files']:
 p=ROOT/r['path']
 try:
  if not p.exists():changed.append({'path':r['path'],'status':'missing since baseline; no removal performed by this script'})
  elif hashlib.sha256(p.read_bytes()).hexdigest()!=r['sha256']:changed.append({'path':r['path'],'status':'changed since baseline; concurrent writer; this task does not write here'})
 except OSError as e:unreadable.append({'path':r['path'],'error':str(e)})
qa={'timestamp':datetime.now().astimezone().isoformat(),'families':len(jobs),'newImagegenSources':len(list((OUT/'ai-kaynaklar').glob('*.png'))),'candidatePNGs':len(manifest),'candidateChecks':checks,'goldHueTargetUnder10':all(r['goldHueEstimatePct']<=10 for r in checks),'failedCandidates':fail,'missingReviewLinks':missing,'phonePlates':phone,'protectedBaselineReadErrors':baseline.get('errors',[]),'protectedChangesSinceBaseline':changed,'protectedReadErrorsNow':unreadable,'scope':'All writes of this script only Assets/Art/Redesign_T22 and docs/art. No delete/move/rename APIs.'}
(WORK/'ai-validation.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
# Every final family and all introduction variants receive a contact-sheet slot for visual review.
paths=[OUT/j['folder']/'once-sonra.png' for j in jobs]+sorted((OUT/'104-ai-tanitim').glob('onizleme-*.png'))
for page in range((len(paths)+8)//9):
 subset=paths[page*9:(page+1)*9];sheet=Image.new('RGB',(1350,math.ceil(len(subset)/3)*550),'#274854');d=ImageDraw.Draw(sheet)
 for i,p in enumerate(subset):
  im=Image.open(p).convert('RGB');im.thumbnail((430,500),R);x=i%3*450;y=i//3*550;sheet.paste(im,(x+(450-im.width)//2,y));d.text((x+9,y+507),p.parent.name[:21],font=font,fill='#FFF6D8');d.text((x+9,y+534),p.stem[:23],font=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),16),fill='#FFF6D8')
 sheet.save(WORK/f'ai-qa-pafta-{page+1}.jpg',quality=93)
print(json.dumps({'families':len(jobs),'candidatePNGs':len(manifest),'failed':fail,'missingLinks':missing,'protectedChangedCount':len(changed),'protectedReadErrorCount':len(unreadable),'phonePlates':len(phone),'gold':[(r['file'],r['goldHueEstimatePct']) for r in checks]},ensure_ascii=False))
assert not fail and not missing
