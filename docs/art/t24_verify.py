from pathlib import Path
from PIL import Image
import json,hashlib,re
from datetime import datetime
ROOT=Path(__file__).resolve().parents[2];WORK=ROOT/'docs/art/task24';OUT=ROOT/'Assets/Art/Paintings_v2'
NOTES={
11:'İkinci denemede kesim çizgileri kaldırıldı. Örnekler yaprak, dal, ışıklı gövde, patika ve çayır tonlarıyla ayrılıyor.',
12:'İkinci denemede Jiggle küçültüldü; posta kutusu odak kaldı. Altı örnekte kapı tokmağı, çatı, pencere, çayır ve gökyüzü sınırları var.',
13:'Yakın plan ve kuşbakışı kitap kompozisyonu panoramalardan ayrılıyor. Sayfa, cilt, sepet, örtü ve gölgeler örnekleri ayırıyor.',
14:'İkinci denemede20 ile yinelenen uçurtma kaldırıldı; küçük tahıl çuvalı eklendi. Bulut kuşakları ve renkli yamaçlar örneklerde okunuyor.',
15:'İki alt kaya parçası yakın mor tonlarda; farklı diyagonal düzlemler ve ışık şeritleri seçiliyor. Su/yaprak/kıyı örnekleri belirgin.',
16:'Örnekler iri mantar lamelleri, turkuaz gövde, sarı ışık ve ön yaprak siluetleriyle ayrılıyor; küçük mantar ormanı gürültüsü yok.',
17:'Sepet şeritleri geniş; iki sepet parçasında farklı bant yönü ve ışık var. Elma, gövde ve mavi uzaklık örnekleri kolay ayrılıyor.',
18:'İlk testte üç pervaz parçası çok benzerdi. İkinci denemede üç büyük yaprak gölgesi eklendi; aynı örnekler farklı gölge kenarları taşıyor.',
19:'Çapraz tek kütük köprü ve geniş su tonları. Örneklerde dal, saz, gölge ve kıyı ayrı; bir koyu alt parça daha zor ama ton sınırı var.',
20:'Tek çam, kırmızı uçurtma ve lavanta Jiggle. Örneklerde bulut, iğne yaprak kütlesi, uçurtma ve yamaç farklı;14teki tekrar kaldırıldı.',
21:'İlk gökyüzü parçası fazla sakindi. İkinci denemede geniş bulut kuşakları; aynı altı parçada açıklık, bulut, kablo ve kabin sınırları ayrılıyor.',
22:'Sıcak koltuk ve serin karlı pencere birbirinden net. Örnekler koltuk kenarı, pencere, lamba ayağı ve döşeme ışığıyla ayrılıyor.',
23:'İlk buz yüzeyi fazla tekdüzeydi. İkinci denemede üç geniş yansıma-gölge kuşağı ve tek büyük bulut eklendi; aynı örnekler yeniden incelendi.',
24:'Kızak kırmızı tek odak; yumuşak kar gölgeleri. Altı örnekte kar sıçraması, kızak kenarı ve farklı bulut yönleri var.',
25:'Tek iri çan ile basit taşıyıcı; örneklerde ip, çan içi, ahşap, bulut ve dağ ışığı ayrılıyor.',
26:'Büyük kozalak pulları korunuyor, ince kozalak gürültüsü yok. Örneklerde sepet sapı, dal, kar ve karakter kenarı farklı.',
27:'Geniş dağ düzlemleri, pirinç dürbün ve az nesne. Altı örnekte kar-kaya sınırı ve bulut tonları var; yakın mavi tonlar daha zor parça oluşturuyor.',
28:'Katı buzul mağarası; sıvı su değildir. Örnekler turkuaz mağara kemeri, sıcak uzak zirve ve mor kar katmanlarıyla ayrılıyor.',
29:'Tek somun ön planda; penceredeki sıcak/soğuk ayrımı güçlü. Bir sıcak duvar örneği şekil yerine geniş ışık geçişiyle ayırt ediliyor.',
30:'Tek kızıl ağaç ve kıvrımlı buz kıyısı. Rastgele örnekler dal yönü, çam silueti ve pembe-mavi gökyüzüyle ayrılıyor.',
31:'Tek sıcak mantar fener ve serin orman. İki koyu örnek yakın tonlu; farklı yaprak/gövde sınırları mevcut, düz siyah alan yok.',
32:'İlk ışık gündüz gibiydi; ikinci deneme ay ışığı ve kitabın kendi sıcak ışığıyla gece oldu. Örneklerde sayfa kıvrımı, halı ve masa kenarı var.',
33:'Tek balon büyük siluet; altı örnek balon şeritleri, bulut ve tepe ışık bantlarıyla ayrılıyor.',
34:'Tek büyük nilüfer. Üç sol örnekte saz gölgeleri benzer koyu palette ama açı, açık aralık ve arka plan tonu farklı; yoğun çiçek yok.',
35:'İlk testte sağ duvar örneği tekdüzeydi. İkinci denemede geniş amber ışık ve mor gölge sınırı eklendi; aynı duvar parçası tekrar kontrol edildi.',
36:'Tek bulut evi; geniş bulutlar sakin. Bir alt bulut örneği nesne yerine pembe-mavi ton geçişi taşıyor; bu örnek daha zor olabilir.',
37:'İnsan karakteri yok; uyuyan taş dev yalnız iki oyuk göz ve gülüşlü stilize kaya. Örneklerde iri yosun, kök ve farklı kaya ışıkları ayrılıyor.',
38:'Tek kestane arabası ve sıcak fener. Altı örnek duvar, tente, araba ve iri döşeme sınırlarıyla ayrılıyor.',
39:'İlk sürüm fazla günbatımıydı ve iki üst örnek benziyordu. İkinci deneme indigo gece ve geniş gümüş bulut kuşakları; aynı örnekler yeniden incelendi.',
40:'Final şatosu tek büyük ışıklı pencereyle odak. Son ışık/şafak geçişi, tek kısa merdiven; örneklerde kaya düzlemi, sis ve basamak sınırları ayrılıyor.'}
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def run():
 mpath=OUT/'manifest-v2.json';m=json.loads(mpath.read_text(encoding='utf-8'));assert len(m['paintings'])==30;assert sorted(r['level'] for r in m['paintings'])==list(range(11,41));assert len({r['titleTR'] for r in m['paintings']})==30
 qa=json.loads((WORK/'parcalanma-testleri.json').read_text(encoding='utf-8'));checks=[]
 for r in m['paintings']:
  n=r['level'];s=OUT/r['source'];g=OUT/r['game'];assert Image.open(s).size==(1536,1536);assert Image.open(g).size==(1024,1024);assert g.stat().st_size<=300000;assert sha(s)==r['sourceSha256'] and sha(g)==r['gameSha256'];assert len(list((WORK/'parcalar'/f'{n:02}').glob('*.png')))==42
  q=next(x for x in qa if x['level']==n);assert len(q['sample'])==6;q['visualReview']='reviewed';q['reviewNote']=NOTES[n];r['qa']={'grid':[6,7],'sampleCount':6,'sampleSeed':2400+n,'visualReview':'reviewed','note':NOTES[n]};checks.append({'level':n,'sourceSize':[1536,1536],'gameSize':[1024,1024],'gameBytes':g.stat().st_size,'pieces':42,'sampleReviewed':6})
 m['batches']=[{'batch':i+2,'room':room,'firstLevel':a,'lastLevel':a+9,'preview':f'once-sonra-{a}-{a+9}.png'} for i,(a,room) in enumerate([(11,'Orman ve kır salonu'),(21,'Kış ve dağ salonu'),(31,'Gece ve büyü salonu')])];m['visualReviewComplete']=True;m['gameplayDeviceTested']=False
 mpath.write_text(json.dumps(m,ensure_ascii=False,indent=2),encoding='utf-8');(WORK/'parcalanma-testleri.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
 h=OUT/'inceleme.html';links=re.findall(r'(?:href|src)="([^"]+)"',h.read_text(encoding='utf-8'));broken=[x for x in links if not (h.parent/x).exists()];assert not broken,broken
 baseline=json.loads((WORK/'paintings-baseline.json').read_text(encoding='utf-8'));changed=[p for p,v in baseline.items() if not (ROOT/p).exists() or sha(ROOT/p)!=v];assert not changed,changed
 report={'time':datetime.now().astimezone().isoformat(timespec='seconds'),'paintings':checks,'totalPieces':1260,'randomPiecesVisuallyReviewed':180,'oldPaintingsFilesUnchanged':len(baseline),'oldPaintingsChanges':changed,'brokenReviewLinks':broken,'gameplayDeviceTested':False,'sourceNormalization':'Generated originals1254×1254 normalized to1536×1536 with LANCZOS; no crop, no detail invention. Originals and rejected variants retained.'};(WORK/'teslim-dogrulama.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
 md=['# Task24 — Kalite ve teslim raporu','','30 tablo11–40 tamamlandı. Önce11/21/31 pilotu karşılaştırıldı;11 ve21 ikinci denemeden sonra yeterli bulundu, kalan27 aynı yönde çizildi. Karar kullanıcı onayı değil, sanatçı öz değerlendirmesidir.','','## Çizim ve teknik teslim','','- Üretim: yerleşik image_gen; imagegen becerisi. Her tablo ayrı üretildi. Ortak istem `task24/prompt-final.txt`; tam tablo istemleri ve düzeltme istemleri `Paintings_v2/manifest-v2.json` ve `task24/generations.json` içinde.','-30 PNG kaynak1536×1536;30 JPG1024×1024, her biri en fazla300000 bayt. Araç çıktıları1254×1254 olduğundan PNG kaynaklar LANCZOS ile1536ya büyütüldü; doğal1536 ayrıntı iddia edilmiyor. İşlenmemiş çıktılar ve önceki denemeler `Denemeler/` altında korunur.','- Her resim6 sütun×7 satırla gerçekten42 parçaya kesildi: toplam1260 dosya `docs/art/task24/parcalar/` altında. Her resimden sabit tohumla rastgele6 parça seçildi;180 örnek görsel olarak incelendi. Bu inceleme bütün42 parçanın eşit kolaylıkta olduğu anlamına gelmez.','-11/12/14/18/21/23/32/35/39 için ikinci deneme yapıldı.18 pervaz,23 buz,35 duvar ve39 gökyüzü parça örneklerinin yetersizliğinden sonra düzeltildi.','- Üç salon karşılaştırma şeridinde üst sıra eski, alt sıra yeni. `inceleme.html` içinde30 çift numaralı ve ayrı parça testi bağlantıları var.','- Su bütçesi:11–20 yalnız15 ve19;21–30 donmuş göl23 ve yarı donmuş göl30 (28 katı buzul);31–40 yalnız34. Merdiven yalnız40ta. Her salonun10 konusu farklı;14teki uçurtma tekrarı kaldırıldı.','','## Sınırlar','','Gerçek6×7/8×8 oyun tahtası, telefon oynanışı veya Unity import/atlas testi yapılmadı. Büyük sakin gökyüzü, kar ve gölge parçaları doğal olarak daha zor kalabilir; özellikle15/27/31/34/36 için notlar aşağıda. Karakter boyları sahneye göre değişiyor; yakın planlarda hedef8–11%ten büyükler. Bazı sahneler gece/şafak sınırında;32 ve39 belirgin geceye düzeltildi.40 son ışığın final şatosudur.','','## Tek tek inceleme','','| No | Karar / bulgu |','|---|---|']
 md += [f'| {n} | {NOTES[n]} |' for n in range(11,41)];md+=['','## Koruma','','Mevcut Paintings klasöründeki '+str(len(baseline))+' dosya başlangıç SHA-256 değerleriyle aynı. Mevcut tabloların üzerine yazılmadı. Kod/manifest bağlantısı yapılmadı; silme, taşıma, yeniden adlandırma yok. Yeni manifest yalnız Paintings_v2ye aittir.','']
 (ROOT/'docs/art/T24_KALITE_RAPORU.md').write_text('\n'.join(md),encoding='utf-8')
 with (ROOT/'docs/art/GECE_LOG.md').open('a',encoding='utf-8') as log:
  log.write('\n\n## '+datetime.now().astimezone().isoformat(timespec='minutes')+' — Task24 sabah teslimi\n\n11/21/31 pilotundan sonra kalan27 çizildi: toplam30 yeni tablo,39 üretim/düzeltme.1536px PNG,1024px JPG≤300000bayt, TR/EN manifest, üç eski/yeni şerit ve30 çiftlik inceleme hazır.1260 parça kesildi,180 rastgele örnek görsel incelendi;9 resim ikinci denemeyle düzeltildi. Mevcut Paintings dosyalarının tamamı hash kontrolünde aynı. Gerçek cihaz/oynanış testi yapılmadı.\n\nTask23 son kapsamı mevcut17 aile: NIHAI/inceleme/kod listesi tamam. Koruma karşılaştırmasında4617 dosya aynı,7 dosya oturumda başka faaliyetle değişmiş (iki hand-glove PNG ve TutorialHand/TutorialSpotlight/LevelsMap/Screens/Texts); bu çalışmaların komutları bu dosyalara yazmadı, geri alınmadı. Liste task23/koruma-sonuc.json içinde; eksik dosya yok.\n')
 print('PASS:30 paintings,1260 tiles,180 reviewed samples; all links valid; old Paintings unchanged; JPEG max',max(r['gameBytes'] for r in m['paintings']))
if __name__=='__main__':run()
