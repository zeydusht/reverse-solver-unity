from pathlib import Path
from html.parser import HTMLParser
from datetime import datetime
import json,csv,os
R=Path(__file__).resolve().parents[2];W=R/'docs/art/task22';O=R/'Assets/Art/Redesign_T22'
class Extract(HTMLParser):
 def __init__(self):super().__init__();self.active=False;self.value=''
 def handle_starttag(self,t,a):
  if t=='pre' and dict(a).get('id')=='result':self.active=True
 def handle_endtag(self,t):
  if t=='pre':self.active=False
 def handle_data(self,d):
  if self.active:self.value+=d
browser=[]
for n,expected in [('browser-test-dom.html',375),('browser-test-390-dom.html',390)]:
 e=Extract();e.feed((W/n).read_text(encoding='utf-8-sig'));row=json.loads(e.value);assert row['selectionWorks'] and row['uniqueCheckboxes'] and row['noHorizontalOverflow'];assert row['checkboxCount']==35 and row['viewport']==expected;browser.append(row)
(W/'browser-behavior.json').write_text(json.dumps({'timestamp':datetime.now().astimezone().isoformat(),'tests':browser,'desktopScreenshot':'inceleme-1200.png','directAnchorMobileScreenshot':'inceleme-mobile-390.png is blank, not accepted as evidence','mode':'isolated Chrome headless; local review only, not live Unity/game'},ensure_ascii=False,indent=2),encoding='utf-8')
p=W/'ai-validation.json';qa=json.loads(p.read_text(encoding='utf-8'));qa['newImagegenSources']=len(list((O/'ai-kaynaklar').glob('*.png')));qa['goldHueTargetUnder10']=all(r['goldHueEstimatePct']<=10 for r in qa['candidateChecks']);assert qa['goldHueTargetUnder10'];p.write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
con=W/'consolidation.json';c=json.loads(con.read_text(encoding='utf-8'));c['independentCrossCheckRows']=41;c['browserBehaviorEvidence']='browser-behavior.json';c['AIImagegenCallsIncludingNodeRevision']=11;con.write_text(json.dumps(c,ensure_ascii=False,indent=2),encoding='utf-8')
# Snapshot newly produced QA files after the frozen cleanup pass. They are retained evidence,
# not silently appended to the safe-to-remove total. Do not follow inaccessible directories.
with (W/'temizlik-final-dosyalar.csv').open(encoding='utf-8-sig',newline='') as f:known={r['path'] for r in csv.DictReader(f)}
new=[];err=[]
for base in ['Assets/Art','docs/art']:
 for dp,dn,fn in os.walk(R/base,onerror=lambda e:err.append(str(e))):
  for n in fn:
   p=Path(dp)/n;rel=p.relative_to(R).as_posix()
   if rel in known or rel.endswith('teslim-sonrasi-qa-envanter.csv'):continue
   try:new.append({'path':rel,'bytes':p.stat().st_size,'MB':round(p.stat().st_size/1e6,6),'group':'Kalmalı','gameUse':'T22 aday/QA kaydı; entegrasyon yapılmadı','reason':'Dondurulmuş taramadan sonra oluşturulmuş teslim veya tarayıcı QA kanıtı; bu görevde saklandı, kazanım toplamına eklenmedi.'})
   except OSError as e:err.append(str(e))
with (W/'teslim-sonrasi-qa-envanter.csv').open('w',encoding='utf-8-sig',newline='') as f:
 writer=csv.DictWriter(f,fieldnames=['path','bytes','MB','group','gameUse','reason']);writer.writeheader();writer.writerows(new)
report=R/'docs/art/TEMIZLIK_ONERI.md';s=report.read_text(encoding='utf-8');marker='## Son QA sırasında eklenen dosyalar'
if marker not in s:
 s+='\n'+marker+'\n\nDondurulmuş00:51 taramasından sonra oluşan aday sürümü, masaüstü ekran görüntüsü, davranış testi ve izole tarayıcı profilleri [ek CSV](task22/teslim-sonrasi-qa-envanter.csv) içinde dosya başına boyutla listelendi. Bunlar teslim/QA kanıtı olarak **kalmalı** sınıfında tutuldu;25,595 MB güvenli önbellek ve1598,622 MB koşullu potansiyel toplamları artırılmadı. Bu ek de silme önerisinin uygulandığı anlamına gelmez.\n'
 report.write_text(s,encoding='utf-8')
with (R/'docs/art/GECE_LOG.md').open('a',encoding='utf-8') as f:
 f.write('\n## '+datetime.now().astimezone().isoformat(timespec='minutes')+' — teslimi kapatan ek doğrulama\n')
 f.write('- Birleşik inceleme35 aile/36 önce-sonra/150 PNG ile PASS. Chrome yerel davranış testinde375 ve390 px içerik genişliği:1 ve101 ortak seçim,35 benzersiz kutu, yatay taşma yok. Masaüstü ekran görüntüsü görsel olarak incelendi. Kanıt: `task22/browser-behavior.json`.\n')
 f.write('- AI20 PNG alfa/@3x/hash/slice doğrulaması geçti.11 imagegen çağrısı (10 ilk kaynak +1 düğüm revizyonu), son düğüm altın tahmini%7,745; bütün son AI sprite tahminleri%10 altında. Kaynak ve önceki düğüm PNGsi korundu.\n')
 f.write('- karar: Genel Assets başlangıç karşılaştırmasında4 PerformanceTestRunInfo/Settings JSON/meta artık görünmüyor; bunlar izinli çalışma alanımızın dışında ve oturum komutlarıyla silinmedi/değiştirilmedi. Oyun dosyaları geri alınmadı. Onaylı sanat3082 dosya kontrolünde0 değişen/0 eksik.\n')
 f.write('- Son teslim sayıları değişmedi:63 ana bulgu (10 yüksek/38 orta/15 düşük),22 ana çizim +13 alternatif; inceleme `Assets/Art/Redesign_T22/inceleme.html`; güvenli cache25,595 MB, dış arşiv1573,027 MB, koşullu potansiyel1598,622 MB, uygulanan temizlik0 MB. Ek41 kontrol satırı ana bulgulara tekrar eklenmedi.\n')
print(json.dumps({'browserWidths':[r['viewport'] for r in browser],'candidatePNGs':150,'goldUnder10':qa['goldHueTargetUnder10'],'postSnapshotFiles':len(new),'postSnapshotMB':sum(r['bytes'] for r in new)/1e6,'status':'PASS'},ensure_ascii=False))
