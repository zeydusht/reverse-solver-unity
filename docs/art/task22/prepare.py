from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps
import json,hashlib,datetime,os
ROOT=Path(__file__).resolve().parents[3]
WORK=ROOT/'docs/art/task22'
OUT=ROOT/'Assets/Art/Redesign_T22'
OUT.mkdir(exist_ok=True)
def log(t):
 with (ROOT/'docs/art/GECE_LOG.md').open('a',encoding='utf8') as f:f.write('\n## '+datetime.datetime.now().astimezone().isoformat(timespec='minutes')+'\n'+t+'\n')
baseline={}
for p in (ROOT/'Assets/Art').rglob('*'):
 if p.is_file() and 'Redesign_T22' not in p.parts:
  try:baseline[p.relative_to(ROOT).as_posix()]={'size':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
  except (OSError,PermissionError):pass
(WORK/'original-art-baseline.json').write_text(json.dumps(baseline,indent=2),encoding='utf8')
sources=[]
for d in ['2026-11-02-son','2026-11-02-spot','2026-11-02-booster','2026-11-02-ftue','2026-11-01-p20b','2026-11-03-tam','2026-11-02-rating']:
 for p in sorted((ROOT/'docs/qa'/d).glob('*.png')):
  with Image.open(p) as im:sources.append({'path':p.relative_to(ROOT).as_posix(),'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
(WORK/'screens.json').write_text(json.dumps(sources,ensure_ascii=False,indent=2),encoding='utf8')
# Fixed evidence crops, in screen pixels. All drawing work has an actual screen source.
F='docs/qa/2026-11-02-ftue/'
M='docs/qa/2026-11-01-p20b/'
S='docs/qa/2026-11-02-son/'
jobs=[
 (1,'kazanma','Kazanma kartı',F+'09_b1_kazanma.png',(35,235,340,650),'Assets/Art/Gameplay/Task14/card-win.png','Yüksek','Yaldızlı kalın panel menüdeki krem/turkuaz aileden ayrışıyor; dört eylem aynı ağırlıkta.'),
 (2,'puan-secimi','1–10 zorluk kutuları',F+'09_b1_kazanma.png',(35,365,340,445),'Assets/_Game/Presentation/Ui/Card.cs::ChipRow','Yüksek','Eski ekranda tek sıra minik kutular; iki sıranın da seçili/basılı durumları için ortak hacimli sprite gerekiyor.'),
 (3,'kayip-bomba','Bomba kayıp kartı',F+'40_kayip.png',(35,180,340,570),'Assets/Art/Gameplay/Task14/card-lose-bomb.png','Yüksek','Kayıp nedeni küçük; soru eylemi yeniden denemenin önüne geçiyor; kalın altın çevre içeriği eziyor.'),
 (4,'kayip-sure','Süre kayıp kartı','docs/qa/2026-10-26-p18/375x667_06_kaybetme.png',(25,150,350,600),'Assets/Art/Gameplay/Task14/card-lose-time.png','Orta','Ortak kayıp kabuğu ve büyük saat nedeni gerekiyor; güncel süre kayıp karesi bulunmadığından eski kaynak açıkça işaretli.'),
 (5,'durdurma','Durdurma kartı','docs/qa/2026-10-26-p18/375x667_04_pause.png',(25,150,350,550),'Assets/Art/Gameplay/Task14/card-pause.png','Orta','Eski düz/ince eylem yüzleri ve ağır altın kabuk güncel menü ailesine uymuyor.'),
 (6,'tanim-civi','Çivi tanıtımı',F+'14_b3_kart.png',(35,70,340,610),'Assets/Art/ObstacleIntros/OptionB/nail-888x592.png','Orta','Büyük eski karakter sahnesi, tekrar ikon ve küçük açıklama; öğretilecek nesne başrolde değil.'),
 (7,'tanim-bomba','Bomba tanıtımı',F+'37_b6_kart.png',(35,110,340,630),'Assets/Art/ObstacleIntros/OptionB/bomb-888x592.png','Orta','Resim, ikon ve çok küçük açıklama üç ayrı odak oluşturuyor; nesne ve kural tek panelde birleşmeli.'),
 (8,'tanim-zincir','Zincir tanıtımı','docs/qa/2026-11-02-spot/375x667_b10_1.png',(10,80,365,580),'Assets/Art/ObstacleIntros/OptionB/chain-888x592.png','Orta','Spotlight metni okunuyor; ayrı tanıtım ailesinde iki bağlı nesnenin ilişkisi büyük ve renk dışı bir bağla da anlatılmalı.'),
 (9,'tanim-duvar','Duvar tanıtımı','docs/qa/2026-11-02-spot/375x667_b14_1.png',(10,80,365,580),'Assets/Art/ObstacleIntros/OptionB/wall-888x592.png','Orta','Duvarın geçilemezliği ayrı kartta net nesne silüetiyle anlatılmalı; süslü çerçeve yerine bariyer odaklı sunum.'),
 (10,'booster-makas','Makas tanıtımı',F+'34_b5_kart.png',(35,200,340,600),'Assets/_Game/Presentation/Ui/Screens.cs::Intro / booster-scissors','Orta','Makas ikonu açıklamaya göre çok küçük; üst başlık uzun, talimat ve ipucu aynı yoğunlukta.'),
 (11,'booster-cekic','Çekiç tanıtımı','docs/qa/2026-11-02-booster/b9_1_tus.png',(10,80,365,600),'Assets/_Game/Presentation/Ui/Screens.cs::Intro / booster-hammer','Orta','Dört booster tanıtımının ortak büyük nesne alanı yok; çekiç etkisi kısa bir görsel hiyerarşi gerektiriyor.'),
 (12,'booster-saat','Saat tanıtımı','docs/qa/2026-11-02-booster/b15_1_tus.png',(10,80,365,600),'Assets/_Game/Presentation/Ui/Screens.cs::Intro / booster-clock','Orta','Saat öğretimi aynı kabukta büyük nesne ve kısa kural ile eşleşmeli; stok/süre birbirine karışmamalı.'),
 (13,'booster-degnek','Değnek tanıtımı','docs/qa/2026-11-02-booster/b23_1_tus.png',(10,80,365,600),'Assets/_Game/Presentation/Ui/Screens.cs::Intro / booster-wand','Orta','Son booster için de aynı nesne ölçeği ve tek yeşil eylem ailesi gerekir.'),
 (14,'zor-bolum','Zor bölüm uyarısı','docs/qa/2026-10-28-paint40-live/375x667_lv19.png',(20,100,355,600),'Assets/Art/HardWarning/option-b-jiggle-warning-888x592.png','Orta','Zor durum ince uyarı işaretiyle anlatılmalı; büyük altın/karakter sahnesi oyunun kendi nesnesiyle sadeleşmeli.'),
 (15,'yukleme','Yükleme',F+'01_yukleme.png',(0,0,375,667),'Assets/Art/MissingScreens/loading-1125x2001.svg','Yüksek','Ekran tamamen koyu ve boş; marka, bekleme işareti ve anlamlı yükleme geri bildirimi görünmüyor.'),
 (16,'isim','İsim girişi',F+'02_isim.png',(20,215,355,480),'Assets/Art/MissingScreens/name-gate-1125x2001.svg','Yüksek','Düz ince çerçeve ve düz turkuaz ana eylem onaylı menüden kopuk; alan/etiket hiyerarşisi zayıf.'),
 (17,'profil-sorusu','Profil sorusu',F+'03_profil_sorusu.png',(30,150,345,570),'Assets/_Game/Presentation/Ui/Card.cs / profile question','Orta','Üç yanıtın tümü ağır altın çerçeveli; kabuk ve seçim yüzleri sakin menü kartına uymalı.'),
 (18,'ayar-anahtari','Ayar anahtarları',F+'65_ayarlar.png',(70,240,315,410),'Assets/Art/Menu/Task20/toggle-on-track.png + toggle-off-track.png + toggle-knob.png','Orta','Açık/kapalı yalnız düğmenin yeri ve rengiyle ayrılıyor; küçük krem düğme satır içinde kayboluyor.'),
 (19,'dil-secici','Dil seçici',F+'65_ayarlar.png',(90,390,315,465),'Assets/Art/Menu/Task20/language-left-selected.png + language-right-selected.png','Orta','Seçili dilin açık-koyu farkı zayıf; iki segment aynı kabuk içinde daha net ayrılmalı.'),
 (20,'harita','Harita düğümleri ve Oyna',S+'375x667_harita.png',(35,340,330,600),'Assets/Art/Menu/Task20/node-current.png + node-complete.png + node-locked.png','Orta','Altın dolu aktif düğüm ve küçük düz yeşil Oyna balonu farklı malzeme; normal/aktif/kilitli tek nesne ailesi olmalı.'),
 (21,'koleksiyon','Sıradaki şeridi ve kilitli hücre',S+'375x667_koleksiyon_siradaki.png',(10,230,245,350),'Assets/Art/Menu/Task20/portrait-placeholder.png + portrait-frame.png','Orta','Sıradaki küçük yazı resme yapışık; boş/kilitli karolar düz koyu blok gibi, durum hiyerarşisi zayıf.'),
 (22,'jiggles','Jiggles ilerleme',S+'375x667_jiggles.png',(20,480,355,610),'Assets/Art/Menu/Task20/progress-track.png + progress-fill.png; Task21 milestone medals','Orta','Son görüntü uzun altın yatay çubuğa dönmüş; Task21 ayrı madalyon kararına uymuyor.'),
]
data=[]
for n,slug,title,source,crop,target,priority,issue in jobs:
 p=ROOT/source
 with Image.open(p) as im:
  w,h=im.size
  # Evidence coordinates are expressed on 375x667 reference; portrait 390x844 inputs adapt proportionally.
  c=[round(v*(w/375 if i%2==0 else h/667)) for i,v in enumerate(crop)]
  c=[max(0,min(v,w if i%2==0 else h)) for i,v in enumerate(c)]
 data.append(dict(id=n,slug=slug,title=title,source=source,crop=c,target=target,priority=priority.lower(),issue=issue,folder=f'{n:02d}-{slug}'))
(WORK/'jobs.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
log('- Stil turu kaynakları ve 22 bağımsız çizim işi kaydedildi. Task19D/20/21 ve özgün stil referansları görsel olarak karşılaştırıldı.\n- karar: Onaylı nesne ve tuş bitmaplerini koruyan yerel UI çizim/export yöntemi kullanılacak; yeni karakter üretilmeyecek, Royal Match pikselleri hiçbir çıktıya alınmayacak.\n- karar: Zincir/duvar ve bazı booster kartlarının yeni modal görüntüsü yok; en yeni ilgili öğretim ekranı öncesi olarak kullanılacak ve NOT.md içinde “modal öncesi değil, ilgili öğretim bağlamı” diye belirtilecek. Eski süre/pause kareleri tarihleriyle işaretlenecek.')
print(json.dumps({'baseline_files':len(baseline),'screens':len(sources),'drawing_jobs':len(data)}))
