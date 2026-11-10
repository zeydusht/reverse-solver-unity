# Task22 — bağımsız dört tur / AI malzeme eki

Kayıt: 2026-11-03T00:46:33+03:00. Bu ek, eşzamanlı ana raporun verilerini ezmeden bağımsız incelemeyi saklar.

Kaynak:2 Kasım FTUE/son/spot/booster ve rating, menü detayları için1 Kasım p20b. Başlangıçta olmayan `2026-11-03-tam` sonradan geldi;375/390 süre kaybı, durdurma, zor giriş, Ayarlar ve bölüm listesi ayrıca incelendi. Bu durumlarda eski26 Ekim kareleri yeni kaynakla değiştirildi. Canlı oyun web aracıyla açılamadı. Bu ekran görüntüsü + salt okunur kod incelemesidir; oynama, gerçek cihaz dokunma/klavye veya animasyon testi yapıldığı iddia edilmez.

375×667 ve390×844 kaynakları ayrı karşılaştırıldı. Ekran dosyaları bazı durumlarda @2x; fiziksel piksel pt diye kabul edilmedi. Kodun mantıksal ölçüleri kullanıldı. Altın oranı için illüstrasyonların sarıları ile UI altını karıştırılmamalı; yeni sprite üzerindeki palet alanı doğrulama raporunda, mevcut ekranın tüm sarı pikselini kesin UI yüzdesi diye raporlamıyoruz.

Task19D, Task20/Preview ve Task21/onizleme, stil referanslarıyla karşılaştırıldı. RM görselleri yalnız incelemede kullanıldı; teslim bitmaplerine kopyalanmadı. 101–113 numaraları ayrı AI aday ailesidir; ana listedeki1–22 ile karışmaz.

## 1. Stil turu

| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |
|---|---|---|---|---|---|---|
| AI-01 | Sonuç / kazanma | Panel kenarı | Kalın altın kıvrımlar krem içeriğin önüne geçiyor; Task21 turkuaz/krem panelinden ayrışıyor. | yüksek | İnce altın çizgili turkuaz kabuk; 101. | çizim |
| AI-02 | Bomba ve süre kaybı | Ortak kart | Kalın altın kabuk ve küçük neden ikonu;3 Kasım tam tur süre kaybı ve2 Kasım rating bombasıyla doğrulandı. | orta | Aynı hafif kabuk, büyük neden ikonu; 102. | çizim |
| AI-03 | Durdurma | Panel | 3 Kasım tam turdaki durdurma aynı ağır altın kabuğu koruyor; güncel kare bulundu. | orta | Ortak aile, 103; mevcut Devam yeşili korunur. | çizim |
| AI-04 | 8 tanıtım | Kart ve odak | Çivi/bomba eski sahneleri, küçük tekrar ikonları ve yoğun yazı ayrı odaklar. Zincir/duvar/diğer booster modal QA eksik. | orta | Nesne merkezli ortak sunum; 104 içinde sekiz ayrı taslak. | çizim |
| AI-05 | Zor bölüm | Uyarı | 3 Kasım tam tur Level19 Tough level başlangıcı ortak kartta dört küçük engel ikonu ve küçük uyarı satırı taşıyor; uyarı hiyerarşisi zayıf. | orta | Mevcut onaylı Baron görseliyle sade uyarı; 105. | çizim |
| AI-06 | Yükleme | Kimlik / durum | 01_yukleme karesi boş koyu yüzey; tek kare ilerlemenin gerçekten takıldığını kanıtlamaz. | yüksek | Marka, mevcut avatar ve durum alanı önerisi; 106. | çizim |
| AI-07 | İsim girişi | Panel ve başla | Web formu düz sistem fontlu; düz turkuaz Başla ana eylem yeşiliyle uyumsuz. | yüksek | Aynı turkuaz/krem panel, mevcut yeşil CTA; 107. | çizim |
| AI-08 | Profil sorusu | Panel | İlk girişte ağır altın kart ilk isim formundan da sonraki menülerden de ayrışıyor. | orta | Ortak kabuk ve üç eşit ikincil seçim; 108. | çizim |
| AI-09 | Ayarlar | Anahtar / dil | ON/OFF yalnız konum ve renkle ayrılıyor; dil seçiminin kabartma yoğunluğu anahtarlarla farklı. | orta | Tik/çarpı taşıyan iki durum ve ortak dil yüzeyi; 109. | çizim |
| AI-10 | Harita | Oyna balonu / güncel düğüm | Düz yeşil Oyna balonu onaylı hacimli düğümün üstünde başka aile; çok altın güncel düğümle yarışıyor. | orta | Yeşil kabartmalı balon ve dar altınlı güncel düğüm; 110. | çizim |
| AI-11 | Koleksiyon | Sıradaki / kilitli hücre | Küçük düz şerit ve koyu boş kilitli karo, tablo çerçevelerinin malzemesiyle ayrışıyor. | orta | Turkuaz etiket ve çerçeveli kilit durumu; 111. | çizim |
| AI-12 | Jiggles | Uzun ilerleme çubuğu | Son ekranında uzun parlak ray; Task21 üç bağımsız madalyon isteğiyle çelişiyor. | orta | Mevcut raf/avatarları koruyan üç madalyon; 112. | çizim |
| AI-13 | Sonuç puanı | Düz seçim yüzleri | Güncel iki sıralı kutular okunur fakat kart/tuş malzemesine göre düz kalıyor. | orta | Boş hacimli kutu, seçili durum ve ayrı dinamik rakamlar; 113. | çizim |
| AI-14 | Kartlar | Başlık fontu | Card.Title/Finish, TextView.Banner yerine Create/Outline kullanıyor; Nunito aile uygulaması menüyle aynı değil. | orta | PM kod işi: başlıkları TitleFont/Banner üzerinden birleştir. | kod |
| AI-15 | Ev / 4 sekme / HUD | Onaylı parçalar | Task21 alt çubuk ve Task14/15 HUD incelemede korunabilir; yeni genel set çizmek için kanıt yok. | düşük | Yeni çizim açma; palet ve gerçek cihaz yerleşimini doğrula. | kod |

## 2. UX turu — 375×667 ve 390×844

| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |
|---|---|---|---|---|---|---|
| AI-16 | Sonuç puanı | Dokunma ölçüsü | Eski FTUE tek sıra minik; güncel Card.ChipRow iki sıra ve 375 için51×44, 390 için54×44 pt. Güncel hata değil. | düşük | Mevcut iki sıra korunmalı; 113 yalnız malzeme önerisi. | kod |
| AI-17 | Koleksiyon | Sıradaki yazısı | CollectionScreen.cs şerit yüksekliği20 ve yazı12 pt; alt sınır13 pt altında. | orta | En az13 pt; taslakta14 pt, şerit26 pt. | kod |
| AI-18 | Jiggles | 500 / 1.000 eşikleri | JigglesScreen.cs eşik Label boyu12 pt ve #857D69; açık arka planda düşük vurgu. | orta | 14 pt lacivert krem rozet ve bağımsız kilometre taşları. | kod |
| AI-19 | Başarılar | Koşul açıklaması | AchievementsScreen autosize 12–13 pt; bazı uzun koşullar13 altına inebilir. | orta | 13 pt altını kapat, satırı büyüt veya metni kısalt; yeni karo çizmek gerekmez. | kod |
| AI-20 | Ayarlar | Sürüm bilgisi | BuildStamp 12 pt. İşlevsel bir eylem değil, düşük önem. | düşük | Sürümü13 pt yap veya destek detayında göster. | kod |
| AI-21 | Ayarlar / profil / tablo / harita | Kapat ve anahtar hedefleri | Görsel36 pt olması hedef36 demek değil: kod kapatma ve toggle hedefini en az44 pt genişletiyor. | düşük | Mevcut hedef büyütmesini koru; doğrulanan bir hata olarak sayma. | kod |
| AI-22 | Sonuç kartı | Eylem yoğunluğu | Eski karede Geç ana eylemin önünde; yeni kod ana eylemi önce koyuyor, Skip aşağıda44 pt hedef. | düşük | Güncel dizilim korunmalı; 101 yeni kabuğu bu sırayla gösterir. | kod |
| AI-23 | İsim girişi | Klavye / küçük ekran | Statik kare klavye açıkken formun görünürlüğünü kanıtlamaz. Web formu fixed merkezli. | orta | PM cihaz testi: klavyede alan ve Başla görünür, kaydırılabilir en az44 pt hedef. | kod |
| AI-24 | Oyun / booster | Sıfır stok | Booster QA sıfır stok rozetlerini gösteriyor; durum simgesi küçük ve kilit ile boş stok birbirine yakın. | orta | Koda/metne: Kilitli ile stok yok için ayrı erişilebilir açıklama; onaylı ikonları yeniden boyama. | metin |
| AI-25 | Profil kartı | Uzun ad / istatistik | Uzun ad ve en uzun istatistik başlığı aynı satırda sıkışabilir; 375 kare kırpılma riski gösteriyor. | orta | 23 pt başlık için alt sınır13 pt ve ellipsis; tam ad erişilebilir ayrıntıda. | kod |
| AI-26 | Süre kaybı390 | Uzun İngilizce açıklama | 3 Kasım390x844_06_kaybetme metni kartın sağ sınırını aşıyor;375 karesinde iki satıra sığıyor. | yüksek | PM kod/QA: wrap ve InnerWidth/build sürümünü karşılaştır, her iki dilde güvenli içerik genişliği sınırla. | kod |
| AI-27 | Eski bölüm listesi375 | Dikey taşma | 3 Kasım375x667_07_bolum_listesi kırpıntısında üst başlık ekranın tepesine dayanıyor;40 hücreli modal uzun. | orta | PM kod/QA: debug/eski liste erişim yolunu doğrula; ulaşılabilir ise dikey kaydırma veya maksimum modal yüksekliği. | kod |

## 3. Tutarlılık turu

| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |
|---|---|---|---|---|---|---|
| AI-28 | Ev / bölüm başlangıcı | Level / Bölüm | TR FTUE Evde Level1; bölüm başlangıcında Level4 görülüyor. Dil ayarıyla çelişen metin. | orta | TR Bölüm {n}, EN Level {n}; tek yerelleştirme anahtarı. | metin |
| AI-29 | Ayarlar | Türkçe / English | İki dilin kendi adıyla gösterilmesi dil karışıklığı hatası sayılmaz. | düşük | Mevcut endonimleri koru; seçili durum 109. | metin |
| AI-30 | Ekran başlıkları | Pankart kullanımı | Son menülerde her sayfada altın pankart; stil rehberi sekme başlığını doğrudan zeminde ister. | orta | PM kod işi: mevcut yazıyı Banner ile doğrudan zemine al; yeni bitmap gerekmiyor. | kod |
| AI-31 | Sayaçlar | Krem içindeki rakam | Koleksiyon sayacı beyaz konturlu; rehber krem hapta düz lacivert rakam der. | düşük | Sayaç metin renderını düz lacivert yap; bitmap aynı kalabilir. | kod |
| AI-32 | Başarılar | Kilitlenmiş satır | Sıfır ilerlemede bütün sprite renderları .7 tint alıyor; krem pano da soluyor. | orta | Yalnız simgeyi/ilerlemeyi pasifleştir; açıklama kontrastını koru. | kod |
| AI-33 | Harita / Koleksiyon | Sonraki içerik | Oyna yeşil, Sıradaki koyu küçük etiket: ikisi de sonraki hedef ama ağırlıkları farklı. | düşük | 111 etiketi yalnız durum;110 balonu eylem. Etiket dokunma düğmesi gibi sunulmasın. | metin |

## 4. İlk kez oynayan gözü

| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |
|---|---|---|---|---|---|---|
| AI-34 | Yükleme → isim | İlk izlenim | Koyu boş kareden test formuna geçişte oyun kimliği ve neden ad istendiği zayıf. | orta | 106/107 taslağı; adı sonuçları kaydetmek için istediğini tek cümleyle anlat. | metin |
| AI-35 | İsim → profil sorusu | Oyun öncesi iki durak | İsimden sonra zorunlu görünen deneyim sorusu ilk oyunu geciktiriyor. | orta | PM ürün/kod kararı: soruyu atlanabilir yap veya ilk galibiyet sonrası sor. | kod |
| AI-36 | Eğitim → ilk hamle | Kural | FTUE hamleler ve kural kartı adımları görselde anlaşılır. El başka görevin sorumluluğunda. | düşük | Eğitim eline değişiklik yok; süre/tıklama davranışı canlı test edilmedi. | kod |
| AI-37 | İlk5 bölüm | Kural yükü | Bölüm3 çivi ve bölüm5 makas açıklamalarında uzun küçük paragraflar; hızlı öğrenmeyi yavaşlatıyor. | orta | 104 taslağında tek kural ve tek eylem; ayrıntıyı ikinci dokunuşa bırakmak PM kararı. | metin |
| AI-38 | İlk kayıp | Yeniden deneme | Eski bombalı kayıp görselinde yeniden deneme yok; yeni kod retry ekliyor. | düşük | Eski ekran hatasını güncel diye açma;102 yeni taslakta Tekrar dene yeşil. | kod |
| AI-39 | Menü sekmeleri | İkon anlamı | Seçili olmayan sekmeler etiketsiz; rehber bunu ister ama ilk kullanıcı ikon adını bilmeyebilir. | düşük | İlk açılışta tek seferlik metin ipucu; alt çubuğu yeniden çizme. | kod |
| AI-40 | Koridor | Atlama | CorridorTransition.SkipAfter=.3; karelerde görünür atlama ipucu yok. | orta | PM kod/metin işi:13pt Devam etmek için dokun ipucu; koşullu kart/tuş çizimi açma. | metin |
| AI-41 | 40.bölüm bitişi | QA kanıtı | 375x667_lv40_kart dosyasının içeriği HUD LV1 ve eğitim eli;40 sonu kanıtı değil. won40 menü karesi bitiş kartını kanıtlamaz. | yüksek | PM QA: gerçek40 kazanma + galeri + menü zincirini tekrar çek. Kodda isLast etiketi var; gerçekleştiği varsayılmadı. | kod |

## Kapsama ve sınırlar

İncelendi: yükleme, isim/profil sorusu, Ev, dört sekme ve alt çubuk, Koleksiyon ve tablo detayı, Jiggles/ilerleme, Başarılar, profil kartı, Ayarlar, harita, HUD ve dört booster, tanıtımların mevcut kareleri, sonuç/rating/kayıp, eski durdurma, koridor kareleri.

Eksik güncel görsel kanıt: zincir/duvar ve üç booster için ayrı tanıtım modalı, gerçek40.bölüm bitişi. Süre/durdurma/zor giriş/Ayarlar için3 Kasım kareleri artık var. Yeni tam turun05_kazanma dosyası öğretim ekranı, lv40 dosyası oyun tahtasıdır; isimleri final sonucu kanıtı sayılmaz. Eksik modal kanıtı NOT.md dosyalarında bağlam etiketiyle gösterilir.

Bağımsız bulgu satırları: 41 — yüksek 5, orta 25, düşük 11. Düşük önem satırlarının bir bölümü doğrulanmış uygunluk/eskilik notudur; yeni hata sayısı gibi yorumlanmamalı. Ana raporla aynı konu tekrarlarını toplayıp şişirmeyin.

13 yüksek/orta çizim satırının karşılığı101–113;102 iki kayıp,104 sekiz tanıtım varyantı içerir. Yalnız yeni aday dosyaları var; hiçbir oyun dosyasına bağlantı yapılmadı.
