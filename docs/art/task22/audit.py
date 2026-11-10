from pathlib import Path
import json,datetime,collections
R=Path(__file__).resolve().parents[3];W=R/'docs/art/task22';J=json.loads((W/'jobs.json').read_text(encoding='utf8'))
for j,file,crop in [(J[0],'a_kart.png',[50,165,700,1190]),(J[1],'a_kart.png',[88,630,665,888]),(J[2],'c_kayip_kart.png',[50,165,700,1170])]:
 j['source']='docs/qa/2026-11-02-rating/'+file;j['crop']=crop
J[1]['issue']='Güncel iki sıralı kutular doğru boyutta; düz ince çizgili yüzey, kabartmalı eylem tuşlarından kopuk. Seçim, basılı ve boş durumlar aynı aileye alınmalı.'
J[2]['issue']='Güncel kartta puan sorusu açıldığında bomba nedeni görseli kayboluyor; ağır altın kabuk ve uzun açıklama başlıkla yarışıyor.'
J[21]['issue']='Son ekran uzun ve altın ağırlıklı çizgiyi gösteriyor; küçük eşik rakamları ve dolgu düşük kontrastlı. PM_MESAJ_33 ilerleme işlevi korunarak kısa turkuaz kapsül öneriliyor.'
(W/'jobs.json').write_text(json.dumps(J,ensure_ascii=False,indent=2),encoding='utf8')
findings=[]
for j in J:
 findings.append(dict(id=f'S{j["id"]:02}',round=1,screen=j['title'],element=j['title'],problem=j['issue'],severity=j['priority'],suggestion=f'{j["id"]:02} numaralı yeni @3x set; ortak Nunito Black, krem/turkuaz kabuk, ince altın vurgu.',kind='çizim',evidence=j['source'],drawing=j['id']))
extras=[
 (1,'S23','Ev / 4 sekme','Başlık ve ikon ölçekleri','Onaylı Task21 yalın alt çubuk referansı ile son menüde seçili büyük ikon dili genel olarak uyumlu; Ev tablosunun büyük altını istisna ve onaylı sanat.','düşük','Onaylı Ev tablosunu yeniden boyama; altın oranını yeni UI ile sınırla.','kod','Assets/Art/Menu/Task21/Preview/04-sekme-durumlari.png'),
 (1,'S24','HUD / booster','Onaylı oyun içi set','HUD ve dört booster tok nesne diliyle uyumlu; yeni kart çalışması bunların paletini/çerçevesini değiştirmemeli.','düşük','Task14/15 onayını koru; yalnız yazı ailesinin kodda tutarlılığını kontrol et.','kod','docs/qa/2026-11-02-ftue/08_b1_serbest.png'),
 (2,'U01','Kazanma / kayıp','1–10 hedefleri','FTUE kareleri eski tek sıra; güncel rating kareleri iki sıra. Card.ChipRow h=44, cw=(279-24)/5=51 pt: güncel hedef uygundur.','düşük','Eski tek sıra hatasını kapatılmış say; çizim işi S02 yüzey/durum iyileştirmesidir.','kod','Assets/_Game/Presentation/Ui/Card.cs::ChipRow'),
 (2,'U02','Kazanma / kayıp','Kartın toplam yüksekliği','Card.Finish yüksekliği ekran güvenli alanına sınırlamıyor. Uzun Türkçe/İngilizce kayıp metni ve soru birlikte kısa telefonda taşma riski taşıyor.','yüksek','375×667 için içerik yüksekliğini güvenli alana sınırla veya soru bölgesini kaydır; 390×844 karşılaştır.','kod','Assets/_Game/Presentation/Ui/Card.cs::Finish; docs/qa/2026-11-02-rating/c_kayip_kart.png'),
 (2,'U03','Kazanma','Yan yana ikincil eylemler','Compact kartta InnerWidth=279, bw=135.5, bh=66.56, etkin yükseklik=53.25 pt; güncel ButtonPair 44 pt üstünde.','düşük','44 pt altı diye raporlama; yazı ve çevre boşluğunu S01 taslağındaki gibi artır.','kod','Assets/_Game/Presentation/Ui/Card.cs::ButtonPair'),
 (2,'U04','Ayarlar / profil / detay / harita','Kapatma ve geri','Görsel X 36 pt, geri 38 pt olsa da kod hedefi 44 pt genişletiyor; yalnız ekran ölçerek hata ilan edilemez.','düşük','44×44 genişletilmiş hedefi koru; ekran kenarında kırpılmadığını cihazda doğrula.','kod','Assets/_Game/Presentation/Ui/MenuSettings.cs::Add; ProfileCard.cs; CollectionScreen.cs; LevelsMap.cs'),
 (2,'U05','Ayarlar','Anahtar dokunması','65×36 görselin gerçek hedefi Add içinde 65×44; dil segmentleri 95×50. Dokunma ölçüsü geçiyor.','düşük','S18 yeni görsel boyutunu hedefle eşleştir; satırın tamamını dokunulur yapmak isteğe bağlıdır.','kod','Assets/_Game/Presentation/Ui/MenuSettings.cs'),
 (2,'U06','Ayarlar','Sürüm satırı','BuildStamp 12 pt; istenen 13 pt eşiğinin altında.','düşük','İkincil sürüm metnini 13 pt yap veya test bilgisini üretim ekranından kaldır.','kod','Assets/_Game/Presentation/Ui/MenuSettings.cs::BuildStamp'),
 (2,'U07','Koleksiyon','Sıradaki etiketi','CollectionScreen next-ribbon 20 pt yüksek, Banner 12 pt: 13 pt altı; fotoğrafın üstünü de kapatıyor.','orta','S21 şeridi 14 pt, 26 pt yükseklik; fotoğrafın üstündeki güvenli alana yerleştir.','kod','Assets/_Game/Presentation/Ui/CollectionScreen.cs'),
 (2,'U08','Jiggles','Eşik etiketleri','JigglesScreen eşik fontu 12 pt ve #857D69; parlak taş/altın üzerinde kontrast zayıf.','orta','S22 koyu krem zemin üstünde 13–14 pt eşik etiketleri; ilerleme hedefi ve eşikleri koru.','kod','Assets/_Game/Presentation/Ui/JigglesScreen.cs'),
 (2,'U09','Başarılar','Uzun açıklamalar','375 karede Bomba İmha Uzmanı alt açıklaması ince/küçük; 390 daha rahat. Kodda autosize fontSizeMin=12 ve Max=13 pt: 13 altına düşme olasılığı doğrulandı.','orta','Açıklamaları iki kısa satıra aç; asgari 13 pt, ilerleme çubuğuna 8 pt boşluk.','kod','Assets/_Game/Presentation/Ui/AchievementsScreen.cs; docs/qa/2026-11-03-tam/tr_375x667_won12_5_basarilar.png'),
 (2,'U10','Profil','Uzun oyuncu adı','Zeynep Yılmazoğlu örneği isim kutusuna çok sıkı oturuyor; kalem ve isim alanı yarışıyor.','orta','İsim alanına sınır ve ellipsis koy; tam adı düzenleme görünümünde göster, 13 pt altına düşme.','kod','docs/qa/2026-11-01-p20b/tr_375x667_won12_6_profil.png'),
 (2,'U11','Koleksiyon / Jiggles / Başarılar','Alt içerik kesilmesi','375 kısa telefonda son satırlar alt çubuk altında kesiliyor; kaydırma olanağı yalnız dolaylı anlaşılıyor.','orta','Alt güvenli alan ve scroll içerik yüksekliğini doğrula; son satır tam görülebilmeli.','kod','docs/qa/2026-11-02-son/375x667_koleksiyon.png; 375x667_jiggles.png'),
 (2,'U12','Yükleme','Boş bekleme','01_yukleme tek karede tamamen boş. Bunun kısa mı kalıcı mı olduğu tek kareden ölçülemez.','yüksek','S15 yükleme katmanı; indirme ilerlemesi varsa gerçek değere bağla, yoksa belirsiz animasyon; sahte yüzde yok.','kod','docs/qa/2026-11-02-ftue/01_yukleme.png'),
 (2,'U13','Oyun HUD','Duraklatma','Hud.RestartRect 46×46 pt; BoosterBar slotları 60×60 pt. Güncel dokunma boyutları 44 pt üstünde.','düşük','Bu hedefleri koru; küçük görünen ikon yüzünden gereksiz yeniden çizim isteme.','kod','Assets/_Game/Presentation/Ui/Hud.cs; BoosterBar.cs'),
 (2,'U14','İsim girişi','Klavye açık yerleşim','Kaynak yalnız kapalı klavye. 375×667 klavye açıldığında alan ve Başla tuşunun görünürlüğü doğrulanamıyor.','yüksek','Yazılım klavyesinde aktif alanı görünür tut; boş/çok uzun ad durumlarını doğrula.','kod','docs/qa/2026-11-02-ftue/02_isim.png'),
 (2,'U15','Oyun HUD','LV etiketi','Hud.cs içindeki LV metni 9 pt; 13 pt eşiğinin altında. Büyük bölüm sayısı okunur, küçük LV ek bilgi.','düşük','LV yerine bağlama uygun kısa başlık 13 pt veya yalnız büyük bölüm sayısı; mevcut HUD sanatını yeniden boyama.','kod','Assets/_Game/Presentation/Ui/Hud.cs::lv'),
 (2,'U16','Booster çubuğu','Geçici prompt','BoosterBar prompt fontu 12.5 pt; seçili araç talimatı ve geçici durum mesajı eşik altında. Gizli 9pt adlar Art.On durumunda gösterilmez, onları aktif hata sayma.','orta','Aktif talimatı asgari 13 pt yap ve iki satır için yer ayır; HUD/booster onaylı bitmapleri aynı kalsın.','kod','Assets/_Game/Presentation/Ui/BoosterBar.cs::_prompt'),
 (2,'U17','Yeni tam tur','Debug üst yazısı','3 Kasım karelerinde FPS/hücre/telemetri debug metni HUD altını kaplıyor. Bu test kaydı olabilir; üretimde göründüğü kanıtlanmadı.','orta','PM/Claude üretim bayrağında debug metninin kapalı olduğunu doğrulasın; UI tasarımı bu metne göre küçültülmesin.','kod','docs/qa/2026-11-03-tam/375x667_03_oyun.png'),
 (2,'U18','Başarılar','Sıfır ilerlemede tüm kartı soldurma','AchievementsScreen sıfır ilerlemede tüm SpriteRenderer renklerini .7 ile çarpıyor; krem yüzey de kararıyor. Okunurluk ve kilit durumu birbirine karışıyor.','orta','Yalnız ikon/ilerlemeyi pasifleştir; açıklama ve krem panel kontrastını koru.','kod','Assets/_Game/Presentation/Ui/AchievementsScreen.cs::a.Value == 0'),
 (3,'T01','Ev / bölüm başlangıcı','Level / Bölüm','Eski Türkçe FTUE Evinde Level görülüyor; yeni 3 Kasım TR Ev karesinde Bölüm 13 olarak düzelmiş. Yeni EN kayıtta Level doğru dilde.','düşük','Kapanmış eski metin farkı; güncel Bölüm / Level yerelleştirmesini koru.','metin','docs/qa/2026-11-03-tam/tr_375x667_won12_1_ev.png'),
 (3,'T02','Tanıtım / süre kaybı','booster / gövde terimleri','Yeni booster ve gövde/eklem terimleri ilk oyuncuya belirsiz; diğer ekranlarda parça deniyor.','orta','Yardımcı araç / parça / birleşim noktası terimlerini sözlükte birleştir.','metin','Assets/_Game/Presentation/Ui/Texts.cs::TimeHint,NewBooster,ScissorsHint'),
 (3,'T03','İngilizce eğitim','Yön grameri','TutorialErrorDrag: to the up / to the down doğal İngilizce değil.','orta','Drag this piece up / down olarak düzelt.','metin','Assets/_Game/Presentation/Ui/Texts.cs::TutorialErrorDrag'),
 (3,'T04','Zincir öğretimi','İki dilin karışması','booster/b10_0a İngilizce, yeni spot/b10_1 Türkçe. Bunlar farklı dil kayıtları; aynı oturumda karışım kanıtı değiller.','düşük','Dil seçimini oturum metadatasında kaydet; İngilizce kayıtları Türkçe hata sanma.','kod','docs/qa/2026-11-02-booster/b10_0a.png; docs/qa/2026-11-02-spot/375x667_b10_1.png'),
 (3,'T05','Ayarlar','English etiketi','Türkçe ayarda English dilin kendi adı olarak bilinçli ve geçerli; hata değil.','düşük','Türkçe / English özadları korunabilir.','metin','docs/qa/2026-11-02-ftue/65_ayarlar.png'),
 (3,'T06','Kazanma / harita','Ana eylem ailesi','Yeni rating kartında yeşil ana eylem doğru; harita Oyna balonu ise düz #3e9a51 ile farklı yeşil.','orta','S20 yeşil balonu paletteki ana eylem ailesine bağla; diğer tüm ana eylemler aynı font/outline/gölgeyi kullansın.','kod','Assets/_Game/Presentation/Ui/LevelsMap.cs::PlayBubble'),
 (3,'T07','Ayarlar / diğer kartlar','Başlık malzemesi','Ayarlar Task21 doğrudan krem başlık kullanırken eski oyun kartları süslü pankartta; uygulama iki dönemi birleştiriyor.','orta','S01/03–17 yeni ortak kart başlık alanını kullan; sekme sayfalarında Task21 yalın başlık.','kod','docs/qa/2026-11-02-ftue/65_ayarlar.png; 37_b6_kart.png'),
 (3,'T08','Tablo detayı','Tablo başlığı ve kazanım metni','Detay ekranı Türkçe ve 3:4 çerçeve ailesi tutarlı; yeniden çizim gerektiren kanıt yok.','düşük','Tablo oranı ve mevcut resim piksellerini koru; uzun başlıklar iki satırda 13 pt üstü kalmalı.','kod','docs/qa/2026-11-01-p20b/tr_390x844_won12_3_tablo_detay.png'),
 (3,'T09','Oyun kartları / menü','Başlık fontu yolu','Card.Finish başlığı Create/Outline ile kuruyor; menü Title/Banner ailesiyle aynı yazı yolu kullanılmıyor. Yeni Nunito Black mockup kendi başına runtime fontunu düzeltmez.','orta','Kart başlıklarını ortak TitleFont/Banner stilinden üret; ana tuş konturu koyu yeşil, turkuaz başlık konturu lacivert olmalı.','kod','Assets/_Game/Presentation/Ui/Card.cs::Finish; Assets/_Game/Presentation/Ui/MenuArt.cs'),
 (3,'T10','Koleksiyon / Jiggles / Başarılar / harita','Sekme başlık pankartları','Stil rehberi sekme sayfasında başlığı doğrudan zeminde isterken son tam turda altın kıvrımlı pankart hâlâ gösteriliyor.','orta','MenuUi.Title içinde mevcut pankart spriteını kapat, aynı Nunito Black başlığı doğrudan zemine al. Yeni bitmap gerekmez.','kod','docs/qa/2026-11-03-tam/tr_375x667_won12_2_koleksiyon.png; tr_375x667_won12_4_jiggles.png'),
 (4,'İ01','İsim → profil sorusu','İlk oyun öncesi kesinti','Ad sonrasında hemen üç yanıtlı soru çıkıyor; oyuncu henüz ne oynayacağını bilmiyor.','orta','Araştırma kohortu gereğini koruyarak tek kısa cümle ve tek dokunuşla ilerleme; isteğe bağlı atlama PM kararıdır.','metin','docs/qa/2026-11-02-ftue/02_isim.png; 03_profil_sorusu.png'),
 (4,'İ02','Eğitim → ilk bölüm','Kural kartı','İlk iki sürüklemeden sonra soyut kare/ok şeması çıkıyor; gerçek parçayla ilişki kurmak ek zihinsel yük.','orta','Mevcut tahtadaki tek vurgulu örnek ve “Diğer parçalara çarpmadan dışarı çek” cümlesi; eğitim eli bu görevin dışında.','kod','docs/qa/2026-11-02-ftue/07_b1_kural_karti.png'),
 (4,'İ03','İlk 1–5 bölüm','Tekrarlanan sonuç sorusu','FTUE B1/B2/B3/B4/B5 sonunda aynı zorluk ölçeği oyunu sürekli kesiyor; araştırma amacı belli değil.','orta','Kohort ihtiyacını PM değerlendirsin; soruyu isteğe bağlı ve sonraki bölümden ikincil tut.','kod','docs/qa/2026-11-02-ftue/09_b1_kazanma.png;13_b2_kazanma.png;16_b3_kazanma.png;33_b4_kazanma.png;36_b5_kazanma.png'),
 (4,'İ04','Bölüm 3','Çivi kuralı','Her hamlede sayaç azalması ile çivili parçayı sürükleme arasındaki ilişki uzun kartta geç öğreniliyor.','orta','S06 öneri metni: Diğer parçaları çıkar. Çivinin sayacı 0 olunca bu parça serbest kalır.','metin','docs/qa/2026-11-02-ftue/14_b3_kart.png;15_b3_spot.png'),
 (4,'İ05','Bölüm 5','Makasın etkisi','Makası seç / birleşimi kes iki adımı spot kareleri açıklıyor; kartta “eklem” sözcüğü yabancı.','orta','S10 kısa iki adım ve birleşim noktası terimi; görsel stok rozeti ayrı kalsın.','metin','docs/qa/2026-11-02-spot/375x667_b5_1.png;375x667_b5_2.png'),
 (4,'İ06','İlk kayıp','Neden ve çıkış','Güncel ratingli kayıpta neden görseli atlanıyor; soru geniş alanı kaplıyor.','yüksek','S03/04 neden ikonunu compact durumda da koru; Tekrar dene ilk eylem ve büyük yeşil kalsın.','kod','Assets/_Game/Presentation/Ui/Screens.cs::Result; docs/qa/2026-11-02-rating/c_kayip_kart.png'),
 (4,'İ07','Koridor geçişi','Atlama keşfi','Görüntülerde yazı/tuş yok. Kod SkipAfter=.3, Slide=3 saniye; dokunarak atlanabilir olduğu görünmüyor.','düşük','İlk geçişte kısa “Geçmek için dokun” metni düşünülebilir; mevcut olmayan düğme için çizim üretilmedi.','metin','Assets/_Game/Presentation/Ui/CorridorTransition.cs; docs/qa/2026-11-01-corridor/375x667_lv1_menu_b_orta.png'),
 (4,'İ08','40. bölüm bitişi','Terminal akış kanıtı','Sonuç için kod isLast durumunda menü eylemi üretiyor; won40 menü kayıtları mevcut ama güncel 40 kazanma modalı yok. lv40_kart adlı kare gerçekte tahtayı gösteriyor.','yüksek','PM/Claude tam turunda 40 kazanma → terminal etiket → menü → 40/40 koleksiyon karesi alsın; Sonraki bölüm olmamalı. Görülmeyen sonuca görsel hata yakıştırılmadı.','kod','Assets/_Game/Presentation/Ui/Screens.cs::Result; docs/qa/2026-11-01-p20b/tr_390x844_won40_2_koleksiyon.png'),
 (4,'İ09','Ev → dört sekme','Yalnız ikon sekmeler','Seçili olmayan sekmeler yazısız; bu onaylı Task21 tasarım kararı. İlk keşif yardımı olmadan koleksiyon/başarı ayrımı öğrenilmeli.','düşük','İlk ziyarette kısa açıklama veya erişilebilir isim; alt çubuğa sürekli yeni etiket ekleme.','kod','Assets/Art/Menu/Task21/Preview/04-sekme-durumlari.png'),
]
for rnd,idx,screen,el,problem,sev,sol,kind,ev in extras:findings.append(dict(id=idx,round=rnd,screen=screen,element=el,problem=problem,severity=sev,suggestion=sol,kind=kind,evidence=ev))
(W/'findings.json').write_text(json.dumps(findings,ensure_ascii=False,indent=2),encoding='utf8')
text='''# Task22 — 3 Kasım 2026 dört ayrı denetim turu

Bu rapor ekran görüntüsü ve salt okunur kod incelemesidir; canlı oyun, Unity ve build çalıştırılmadı. İlk kaynak havuzu 173 PNG idi. Son/spot/FTUE/booster/p20b karelerine ek olarak **2 Kasım rating** ve gece oluşan **3 Kasım tam tur** bulundu. Süre kaybı, pause, zor giriş ve ayarlar yeni tam turdan; kazanma en yeni geçerli rating kaydından değerlendirildi. Tam turdaki 05_kazanma dosyası gerçekte eğitim sahnesidir. Eski tek sıralı ölçek güncel hata sayılmadı. Güncel kaynak hash/boyut listesi: [screens.json](task22/screens.json). Kod satırları eşzamanlı Claude çalışması nedeniyle değişebilir; sembol adları esas alınır.

## Yöntem ve kanıt sınırı

- Tur 1 yalnız stil: Task19D/20/21 ile özgün Royal Match referanslarında malzeme, kontur, yazı ve altın yoğunluğu karşılaştırıldı. Referans pikselleri teslim çizimlerinde kullanılmadı.
- Tur 2 yalnız UX: 375×667 ve 390×844 kaynaklar ayrı açıldı; 375 kareler küçültülmeden telefon ölçeğinde incelendi. Kodda asgari hedefler hesaplandı. Görsel boyut, gerçek dokunma hedefi değildir.
- Tur 3 yalnız tutarlılık: aynı öğeler yan yana ve Türkçe/İngilizce p20b örnekleri karşılaştırıldı; bazı iki dil kareleri ayrı oturumlardır.
- Tur 4 yalnız ilk oyuncu: FTUE 01–16,31–40 ve61–67 sırasıyla izlendi; B1–5'in kural/sonuç tekrarları, kayıp, sekmeler, koridor ve terminal kodu incelendi. Statik kareler animasyon/süre/hit testinin çalıştığını kanıtlamaz.
- Kesin font adı raster görüntüden çıkarılamaz. Yeni taslaklar Nunito Black TTF kullanır. 13 pt / 44 pt altında olduğu kodla bilinenler ile görsel şüpheler ayrıdır.
- Altın ≤%10 yeni UI tasarım hedefidir; onaylı Ev tablosunun altın çerçevesi ve resim içindeki sarı pikseller UI yeniden boyama izni değildir. Teslimde gold-mask oranları teknik QA'da sezgisel ölçü olarak verilir, algısal doğruluk iddiası yoktur.

## Ekran kapsamı: dört turda da kontrol edilenler

| Ekran / kaynak | Stil | UX 375 / 390 | Tutarlılık | İlk oyuncu |
|---|---|---|---|---|
| Yükleme FTUE01 | S15 | U12; tek kare | Marka dili | Boş bekleme |
| İsim FTUE02 | S16 | U14; klavye karesi yok | Ana eylem | İ01 |
| Profil sorusu FTUE03 | S17 | Yanıtlar okunur; canlı hedef yok | Ortak kabuk | İ01 |
| Ev FTUE04/61; p20b Ev | Onaylı çerçeve korunur | Dar üst plaket | T01, S23 | İ09 |
| Dört sekme / alt çubuk p20b1/2/4/5 | S23; Task21 | U11 | Dört seçili durum | İ09 |
| Koleksiyon son/koleksiyon_siradaki | S21 | U07/U11 | Krem/turkuaz | Kilit ve sonraki ayrımı |
| Tablo detayı p20b3 | T08 | 375/390 resim oranı doğru | T08 | Tekrar oyna anlaşılır |
| Jiggles son/jiggles | S22 | U08/U11 | Son PM33 işlevi korunur | Hedef / eşikler |
| Başarılar p20b5 | Yeni çizim gerekmedi | U09/U11 | 16/9 kayıt farkı ayrı veri | Uzun açıklama |
| Profil p20b6 | Yeni çizim gerekmedi | U10 | İsim/sayaç | Avatar değiştir açık |
| Ayarlar yeni tam tur08 / FTUE65 / p20b7 | S18/S19 | U04/U05/U06 | T05/T07 | Anahtar durumları |
| Harita son/harita / p20b8 | S20 | U04; düğüm66, balon70×44 | T06 | Oyna ve kilit |
| Oyun HUD / booster FTUE08/spot | S24 | U13 | Onaylı14/15 | Stok/kilit |
| Çivi FTUE14 / spot3 | S06 | Açıklama yoğun | T02 | İ04 |
| Bomba FTUE37 / spot6 | S07 | Tek kural hedefi | T02 | Fitil önceliği |
| Zincir spot10 | S08 | 375/390 iki satır | T04 | İki yol aynı anda |
| Duvar spot14 | S09 | 375/390 tek satır | Kırmızı-beyaz | Geçilemez kenar |
| Makas FTUE34 / spot5 | S10 | Kart metni yoğun | T02 | İ05 |
| Çekiç booster9 | S11 | Güncel bağlam karesi | Dört booster ailesi | Tek parça hedefi |
| Saat booster15 | S12 | Güncel bağlam karesi | Dört booster ailesi | +20 saniye |
| Değnek booster23 | S13 | Güncel bağlam karesi | Dört booster ailesi | Resim değişmez |
| Zor bölüm yeni tam tur02 / kod | S14 | Güncel giriş kartı | Ortak uyarı | Ayrı mekanik değildir |
| Kazanma / 1–10 rating a,b | S01/S02 | U01/U02/U03 | T06 | İ03 |
| Bomba kaybı rating c | S03 | U02 | Ortak sonuç | İ06 |
| Süre kaybı yeni tam tur06 | S04 | 375/390 | Ortak sonuç | İ06 |
| Durdurma yeni tam tur04 / güncel kod | S05 | 375/390 | Ortak sonuç | Devam ilk eylem |
| Koridor Kasım1 / kod | Yeni çizim yok; mevcut metin yok | 3s atlanabilir kod | İ07 | İ07 |
| 40 bitişi won40 menü / Result kodu | Modal görülmedi | İ08 | Terminal eylem | İ08 |

'''
for rnd,name in [(1,'Stil turu'),(2,'UX turu'),(3,'Tutarlılık turu'),(4,'İlk kez oynayan gözü')]:
 text+=f'## Tur {rnd} — {name}\n\n| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |\n|---|---|---|---|---|---|---|\n'
 for f in findings:
  if f['round']!=rnd:continue
  cells=[f['id'],f['screen'],f['element'],f['problem']+' Kanıt: `'+f['evidence']+'`',f['severity'],f['suggestion'],f['kind']]
  text+='| '+' | '.join(x.replace('|',' / ').replace('\n',' ') for x in cells)+' |\n'
 text+='\n'
counts=collections.Counter(f['severity'] for f in findings)
text+='## Çizim kapanış eşlemesi\n\n22 yüksek/orta çizim bulgusunun her biri 01–22 numaralı klasör, şeffaf @3x parçalar, önce/sonra ve NOT.md ile eşleşir. Tek inceleme sayfası: [inceleme.html](../../Assets/Art/Redesign_T22/inceleme.html). S08/S09/S11/S12/S13 kaynakları modal değil en yeni ilgili öğretim bağlamıdır; bu açıkça etiketlenir. S04/S05/S14/S18/S19 gece gelen 3 Kasım kaynağına güncellendi.\n\n'
text+=f'Toplam {len(findings)} kayıt: {counts["yüksek"]} yüksek / {counts["orta"]} orta / {counts["düşük"]} düşük. Düşük kayıtların bir bölümü “uygun / koru / doğrulanamadı” kontrol sonucudur; bunlar yeni hata sayılmaz. Yüksek ve orta çizim işi: {len(J)}.\n'
(R/'docs/art/DENETIM_2026-11-03.md').write_text(text,encoding='utf8')
with (R/'docs/art/GECE_LOG.md').open('a',encoding='utf8') as f:f.write('\n## '+datetime.datetime.now().astimezone().isoformat(timespec='minutes')+' — dört turun kayıtları\n- Stil, UX, tutarlılık ve ilk oyuncu turları ayrı tablolarla DENETIM_2026-11-03.md içine yazıldı; tüm istenen ekranlar kapsam matrisinde.\n- karar: Sonradan bulunan 2 Kasım rating kareleri en güncel kazanma/kayıp kanıtı olarak alındı. Tek sıra ölçek sorunu kapalı; yeni çizim yalnız yüzey/durum ailesini iyileştiriyor.\n- karar: Jiggles kodunda PM_MESAJ_33 ilerleme işlevini açıkça istiyor. Daha eski Task21 madalyon yönüyle çelişkiyi işlevi koruyarak çözüyorum: yeni kısa kapsül + okunur eşikler; veri/ödül sistemi değişmeyecek.\n- karar: 40. bölüm final modalı görülmediği için uydurma önce/sonra üretilmeyecek; terminal akışın eksik görsel kanıtı yüksek kod/QA işi olarak PM’ye yazıldı.\n')
print(json.dumps(dict(total=len(findings),counts=counts,drawings=len(J)),ensure_ascii=False))
