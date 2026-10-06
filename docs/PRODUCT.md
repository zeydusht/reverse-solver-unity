# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-06 (GitHub Pages yayında, 4G ölçüm protokolü, M2 başlatıldı)

## Hedef

Reverse Solver'ı Unity'de iPhone'da (Safari, dikey) oynanır hale getirmek, oyuncu verisiyle doğrulanmış bir zorluk modeli kurmak ve 40 seviyeyi bu modele göre baştan tasarlamak. Sonuç: iş başvurularında kullanılacak, veriye dayalı seviye tasarımını anlatan bir case study.

## Başarı ölçütleri

| Ölçüt | Hedef |
|---|---|
| İlk açılış süresi (iPhone, 4G, önbellek boş) | ≤ 10 sn |
| İlk açılışta toplam indirme (ara hedef; engelleyici değil, bkz. karar 2026-10-05) | ≤ 6 MB |
| Seviye doğrulama | 40 seviyenin tamamı otomatik testte çözülebilir |
| Playtest | ≥ 30 oyuncu, her deney seviyesinde ≥ 20 tamamlanmış oyun |
| Zorluk modeli | Modelde kullanılmayan seviyelerde tahmin edilen zorluk ile gerçek deneme sayısı arasında korelasyon ≥ 0,7 |

## Fazlar

| Faz | İçerik | Durum |
|---|---|---|
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0 ve M1 bitti (M1'in iPhone kontrolü Zeyd'de, engelleyici değil). GitHub Pages yayında (2026-10-06). M2 (GameSession) 2026-10-06'da başlatıldı. 4G ölçümü (Zeyd) gelince sıkıştırma kararı kesinleşir |
| 2. Ölçüm altyapısı | Telemetri (client, level_version), test modu (rastgele sıra), ekransız çözücü + Monte Carlo | Bekliyor |
| 3. Deney | Deney seviyeleri, playtest ile veri toplama | Bekliyor |
| 4. Model ve tasarım | Zorluk tablosu, engel tanıtımı ve 40 seviyenin yeniden tasarımı | Bekliyor |
| 5. Case study | Portfolyo dokümanı | Bekliyor |

## Karar kaydı

| Tarih | Karar | Gerekçe |
|---|---|---|
| 2026-10-05 | Unity 6.6 (Supported), URP + 2D Renderer | Built-in RP Unity 6.5'ten beri deprecated, yeni projelere önerilmiyor |
| 2026-10-05 | Hedef platform: iPhone Safari'de dikey Web build; TestFlight yok | Mac yok; web linki playtest için en kolay dağıtım |
| 2026-10-05 | Hosting: GitHub Pages + decompression fallback | Ölçümde JS ile açma 0,35 sn; darboğaz değil, Netlify'a gerek yok |
| 2026-10-05 | Görsel yön: düz renk, sade çizim. 2D ışık ve post-processing yok, sprite'lar Unlit. "Juice" partikül ve tween ile | Web sürümünün tarzı korunuyor; build boyutu |
| 2026-10-05 | Ana ekran modundaki alt şerit: sayfa arka planı kamera rengiyle aynı; o bölgeye arayüz konmaz | Layout düzeltmesi tutmadı, playtest için yeterli çözüm |
| 2026-10-05 | Seviyeler web sürümünden olduğu gibi taşınır; süreler yeni zorluk modeliyle belirlenecek | Tüm seviyeler yeniden tasarlanacak |
| 2026-10-05 | Telemetri düzeltilmiş gelir; Supabase `plays` tablosuna `client` ve `level_version` eklendi (SQL çalıştırıldı) | Web ve Unity verisi karıştırılmadan karşılaştırılabilsin |
| 2026-10-05 | Repo: github.com/zeydusht/reverse-solver-unity (public). Web reposuna dokunulmaz | Yedek, Pages ücretsiz, portfolyoda görünür |
| 2026-10-05 | Kaldırılan paketler: VisualScripting, collab-proxy, iet-framework, Welcome klasörü | Kullanılmıyor; boyut ve derleme süresi |
| 2026-10-05 | Boyut adımları B–F kabul edildi: 2D animation/aseprite/psdimporter/spriteshape/tilemap/2d tooling/timeline ve ~15 modül kaldırıldı; URP'de HDR, PP, 2D ışık vb. kapalı; Managed Stripping High + IL2CPP Optimize Size; wasm Code Optimization = DiskSizeLTO | Ölçülen kazanç 11,81 → 8,64 MB (−%27); görsel yön kararıyla uyumlu |
| 2026-10-05 | Unity splash ekranı kapalı | Unity 6 Personal'da izinli; oyuncu için 2–3 sn boş bekleme yerine doğrudan oyun. Yükleme çubuğu şablonda kalır |
| 2026-10-05 | Input System kalır; eski Input Manager'a geçilmez | Kazancı ölçülmemiş (~0,8 MB IL, wasm payı belirsiz); eski sistem Unity 6'da geri planda, dokunmatik girdi ve mimari kararı değiştirir. Ancak Pages 4G ölçümü 10 sn'yi aşarsa yeniden açılır |
| 2026-10-05 | 6 MB ara hedef engelleyici değil. Asıl ölçüt "iPhone, 4G, önbellek boş ≤ 10 sn". Boyut çalışması şu adımlarla kapanır: Brotli ölçümü, GitHub Pages yayını + 4G ölçümü, UI Toolkit'i çıkarma denemesi (en fazla yarım gün). Sonra M1 | Ölçmeden optimizasyon yok: gerçek 4G süresi bilinmeden boyut kovalamak playtest'i geciktirir. Önceki 8,13 sn indirme yerel Python sunucusundan; 4G'yi temsil etmiyor |
| 2026-10-05 | Brotli yalnızca toplam açılış süresini düşürürse kullanılır (indirme kazancı > JS açma maliyeti, iPhone'da ölçülerek) | Pages `Content-Encoding` göndermediği için Brotli JS ile açılır ve gzip'ten yavaştır |
| 2026-10-05 | Boyut bütçesi: sonraki milestone'larda her build size-log'a yazılır; toplam indirme 10 MB'ı geçerse PM'e raporlanır | M1+ ile font, seviye verisi ve kod eklenecek; kazanılan pay geri gitmesin |
| 2026-10-05 (gece) | Boyut çalışmasının mühendislik kısmı kapandı; M1'e geçiş onaylandı. 4G ölçümü M1'i beklemeden paralel gelir | Kalan tek iş Zeyd'in ölçümü; M1 saf Core işi, sıkıştırma kararından bağımsız. Playtest yolunu bekletmenin anlamı yok |
| 2026-10-05 (gece) | UI Toolkit çıkarılmaz; URP/uGUI fork'u yapılmaz | Modül Input System'in doğrudan bağımlılığı; tek yol fork (bakım yükü) ya da Input System'i bırakmak (reddedildi). Zaman kutusu ~30 dk'da kapandı |
| 2026-10-05 (gece) | main'de varsayılan sıkıştırma gzip kalır; Brotli 4G ölçümüne kadar yalnızca `/br/` adresinde | Önceki karar: Brotli ancak iPhone'da toplam süreyi düşürürse. PC'de JS açma 0,27 → 1,55 sn; başa baş ≈ 11 Mbit/s, 4G bu eşiğin iki yanında olabilir. iPhone'da Brotli açma ölçülmedi |
| 2026-10-05 (gece) | 4G sonrası karar kuralı: iki adresin medyan toplam süresi karşılaştırılır; Brotli ancak ≥ 0,5 sn daha hızlıysa seçilir, aksi halde gzip. İkisi de > 10 sn ise boyut işi yeniden açılır (Input Manager ve backlog'daki boyut adımları PM'e gelir) | Gürültü payı; gzip daha basit ve cihaz CPU'suna daha az bağımlı |
| 2026-10-05 (gece) | M1 kapsamı ve kabul ölçütleri (bkz. "M1 tanımı") onaylandı. Bağlayıcı çözülebilirlik kanıtı kayıtlı çözüm testidir; açgözlü çözücünün çözemediği seviye hata değil, raporlanacak veridir | Başarı ölçütü "40 seviye otomatik testte çözülebilir"; açgözlü çözücü tanımı gereği eksik arama yapar, Monte Carlo/Faz 2 için temel |
| 2026-10-05 (gece) | Seviye kimliği: konumdan bağımsız, kalıcı, asla yeniden kullanılmayan `id` (ör. `L01`–`L40` başlangıçta web sırasından türetilir ama sıra değişince değişmez). Oynanışı etkileyen her değişiklik `version`'ı artırır; görsel/metin değişikliği artırmaz | Test modunda rastgele sıra ve yeniden tasarımda telemetri `level_id` + `level_version` ile karışmadan eşleşsin |
| 2026-10-05 (gece) | M1 kapandı. Kabul 1–7 karşılandı (227 EditMode testi; 10.612 web altın değeri birebir; mutasyon kontrolü 160/227 kırdı). Ölçüt 8 (iPhone'da "40 seviye okundu") Zeyd'in Pages adımına bağlandı, M2'yi engellemez | High stripping'li gerçek web build'i Chrome'da 40/40 okudu ve çözdü (51 ms); iPhone'da farklı sonuç beklemek için sebep yok. Kalan risk Safari'ye özgü ve Pages açılınca zaten görülecek |
| 2026-10-05 (gece) | M2 = GameSession (bkz. "M2 tanımı"). Saf C#; zaman `Tick(dt)`, rastgelelik `IRandom` ile dışarıdan; web'deki işlem sırası korunur. Görsel/girdi, Supabase gönderimi ve PlayerPrefs M2'de yok | Playtest yolundaki sıradaki darboğaz oyun kuralları; ekransız oturum Faz 2'deki Monte Carlo ve tekrar oynatmanın da temeli |
| 2026-10-05 (gece) | İnsan metriklerinin tanımı M2'de sabitlenir: **deneme** = bir seviyenin her başlatılışı (yeniden başlat dahil); **bıraktı (quit)** = sonuçsuz biten deneme (yeniden başlat, menüye dönüş; sayfa kapanışı M5'te); **süre** = oturumun saydığı aktif oyun süresi; **takılma** web'deki tanımla aynı, kodda belgelenir. Her deneme tam olarak bir kayıt üretir | "İnsan metrikleri veri toplanmadan önce tanımlanır" ilkesi; web'deki telemetri hataları (deneme sayacı, eksik quit satırı) bu tanımlarla düzeltilir |
| 2026-10-05 (gece) | Booster davranışı (stok, etki, kullanılamayacağı durumlar) web'le aynı taşınır; değnek sonrası tahtanın çözülebilir kalıp kalmadığı yalnızca ölçülüp raporlanır, davranış değiştirilmez | Taşıma fazında oynanışı değiştirmek web verisiyle karşılaştırmayı bozar; sorun çıkarsa Faz 4 tasarım kararı |
| 2026-10-05 (gece) | 3D fizik modülü (PhysX) boyut fırsatı backlog'a; 4G ölçümüyle birlikte değerlendirilir | Ölçülmedi; boyut işi yalnızca 4G > 10 sn ise yeniden açılır (karar kuralı) |
| 2026-10-06 | Playtest linki kök adres: `https://zeydusht.github.io/reverse-solver-unity/`. Kök şimdilik `/gz/`'ye yönlendirir (sorgu parametreleri korunur); 4G kararından sonra kazanan sıkıştırmaya çevrilir. `/gz/` ve `/br/` alt adresleri kalır (Zeyd'in kararı) | Oyuncuya tek, değişmeyen bir link; sıkıştırma değişse de link aynı kalır |
| 2026-10-06 | İlk Pages 404'ü bizim dosyalarımızdan değil, GitHub'ın "deploy" işine runner atamamasından (15 dk kuyruk, iptal). Kod/ayar değişikliği gerekmez. Yayın bozulursa önce Actions'taki son "pages build and deployment" çalıştırmasına bakılır; gerekirse boş commit ile yeniden tetiklenir | Kanıt GitHub API'de: build 6 sn'de başarılı, deploy hiç başlamadı. `.nojekyll` ilk günden vardı |
| 2026-10-06 | 4G karşılaştırması `/gz/` ve `/br/` adresleri **doğrudan** açılarak yapılır (kök değil). Kök ayrıca bir kez açılıp yönlendirmenin iPhone'da çalıştığı kontrol edilir | İki adres aynı sayfa yapısında olsun; kökün yönlendirme adımı yalnızca gzip'e eklenip karşılaştırmayı bozmasın. Kazanan hangisiyse kök ona yönlenecek, yönlendirme maliyeti iki seçenekte de aynı |
| 2026-10-06 | Süre panelindeki "tarayıcı yerel açtı mı" satırı yok sayılır; karar verisi değildir | Pages CDN'i loader.js gibi metin dosyalarını kendisi gzip'liyor, satır bu yüzden "evet" diyor. Unity dosyaları yine JS ile açılıyor (JS açma satırı bunu gösteriyor). Panel M3'te kalkıyor, düzeltmeye değmez |
| 2026-10-06 | M2'yi PM başlatır; Zeyd'in ayrıca "devam" demesi gerekmez. Zeyd durdurmak isterse durur | M2 kapsamı ve kabul ölçütleri 2026-10-05 gece onaylandı; açık ürün sorusu yok. Zeyd'in gece talimatı geceye özeldi. Zeyd'in bekleyen işlerinin (4G, iPhone kontrolleri) hiçbiri M2'yi engellemiyor; M2 saf Core işi, sıkıştırma kararından bağımsız. Beklemek playtest yolunu boşuna geciktirir |

## Ölçümler

| Tarih | Build | Toplam indirme | Toplam süre | Ayrıntı |
|---|---|---|---|---|
| 2026-10-05 | M0, boş sahne, gzip | 11,2 MB | 9,07 sn (aynı Wi-Fi, önbellek boş, Safari) | İndirme 8,13 sn · JS açma 0,35 sn · wasm 0,22 sn · motor 0,28 sn |
| 2026-10-05 | Boyut A: başlangıç (Medium strip, OptimizeSpeed), gzip | 11,81 MB | — | wasm 9,02 · data 2,66 |
| 2026-10-05 | Boyut B: kullanılmayan paketler/modüller kaldırıldı | 11,15 MB (−0,66) | — | wasm 8,50 · data 2,52 |
| 2026-10-05 | Boyut D: URP özellikleri kapalı, Unlit, Global Light 2D yok | 11,04 MB (−0,11) | — | wasm 8,50 · data 2,41 |
| 2026-10-05 | Boyut E: splash kapalı | 10,98 MB (−0,05) | — | wasm 8,50 · data 2,35 |
| 2026-10-05 | Boyut C: Stripping High + IL2CPP Optimize Size | 9,42 MB (−1,57) | JS açma ~0,33 sn (bilgisayar) | wasm 7,43 · data 1,86 · açma: wasm 235 ms, data 88 ms, framework 4 ms |
| 2026-10-05 | Boyut F: wasm Code Optimization DiskSizeLTO | 8,64 MB (−0,78) | — (iPhone'da ölçülmedi) | wasm 6,65 · data 1,86 · 8.641.034 bayt. Data'nın ~%95'i global-metadata.dat; shader/doku < 0,5 MB. En büyük IL: UIElementsModule 1,54 MB, mscorlib 1,32, InputSystem 0,79, URP ~0,87 |
| 2026-10-05 | Boyut G: F + Brotli (fallback açık) | 6,81 MB (−1,83 vs F) | JS açma 1,55 sn (bilgisayar, 7 tekrar medyan) | wasm 5,16 · data 1,47 · loader 118,6 KB · açma: wasm 1161 ms, data 347 ms. Aynı yöntemle F: 0,27 sn (wasm 200, data 62). Başa baş ≈ 1,4 MB/s (≈ 11 Mbit/s). iPhone'da ölçülmedi |
| 2026-10-05 | M1, gzip (gh-pages `/gz/`) | 8,67 MB (+26 KB vs F) | Açılış kontrolü 51 ms (Chrome/Windows, başsız) | wasm +10 KB · data +16 KB. "40 seviye okundu, 40 çözülebilir". 10 MB bütçesinde |
| 2026-10-05 | M1, Brotli (gh-pages `/br/`) | 6,84 MB | — | Aynı kod; 4G karşılaştırması için |
| 2026-10-06 | M1, gzip, canlı GitHub Pages (kök → `/gz/`) | wasm 6.663.005 B (Content-Encoding yok, beklendiği gibi) | 4,85 sn (PC, başsız Chrome, internet, tek ölçüm, yazılımsal grafik; 4G DEĞİL) | İndirme 1,69 · JS açma 1,63 (iş süresi) · motor 1,47. "40 seviye okundu, 40 çözülebilir". Yalnızca yayının çalıştığının kanıtı; karar verisi değil |
| 2026-10-06 | M1, Brotli, canlı GitHub Pages (`/br/`) | — | 7,55 sn (aynı koşullar, tek ölçüm; 4G DEĞİL) | İndirme 3,07 · JS açma 3,27. 40/40. Karar verisi değil |

## M1 tanımı (seviye verisi ve kural motoru) — KAPANDI 2026-10-05

Sonuç: kabul 1–7 karşılandı, 8 Zeyd'de (Pages açılınca). 227 EditMode testi; travelLimit 10.612 web altın değerinde birebir; açgözlü çözücü 40/40 web'le aynı sonuç ve sıra; LevelStats 40/40; README bölüm planı doğrulandı (çivi 5, zincir 11, bomba 21, mühür 31; tüm zincirler uzak). Geçici `BootLevelCheck` ve süre paneli M3'te kalkacak.

Kapsam:
- `levels.json`: web'deki 40 seviye birebir; her seviyede `id` ve `version: 1`; dosyanın üstünde format sürümü.
- Core'da bağımlılıksız JSON okuyucu + `LevelParser` (Unity `JsonUtility` yok), seviye modeli, `LevelStats` (engel sayıları), `travelLimit`'in birebir portu, açgözlü çözülebilirlik çözücüsü.
- Geçici açılış kontrolü: oyun açılışta seviyeleri okur ve süre panelinde "40 seviye okundu" (veya hata) gösterir.
- Kapsam dışı (M2+): çivi/bomba sayaçları, booster'lar, süre, GameSession, görsel ve girdi.

Kabul ölçütleri:
1. EditMode: 40 seviyenin kayıtlı çözümü her adımda geçerli bir çıkış hamlesi ve sonunda tahta boş.
2. EditMode: `travelLimit` web'le eşdeğer: web referans kodundan alınmış en az birkaç altın değer (farklı engel durumlarını kapsayan) birebir tutar.
3. EditMode: engel sayıları web verisiyle tutarlı (40 seviye).
4. EditMode: açgözlü çözücü 40 seviyede çalışır; çözdüğü/çözemediği seviyeler rapora yazılır. Web'deki çözücünün portuysa sonuçları web'le aynı olmalı.
5. EditMode: format sürümü bilinmeyen ya da `id` tekrarlı/eksik dosya açık bir hatayla reddedilir.
6. `CoreIsolationTests` geçer; Core'da Unity bağımlılığı yok.
7. Web build: stripped Core dll'de parser türleri var (statik kanıt) ve build size-log'a yazıldı (10 MB bütçesi).
8. iPhone'da `/gz/` sürümünde "40 seviye okundu" görülür (Zeyd).

## M2 tanımı (GameSession: oyun oturumu)

Kapsam:
- Core'da saf C# `GameSession`: bir seviyenin tek denemesi. Zaman `Tick(dt)` ile, rastgelelik `IRandom` ile dışarıdan verilir; motor saati/`UnityEngine.Random` yok.
- Hamle sırası web'le aynı: çıkarma → çivi sayaçları azalır → emniyet supabı → bomba fitili azalır → kazanma kontrolü.
- Engeller: sayılı çivi, zincirli çift (tek gövde), bomba, mühürlü kenar (M1 kuralı üzerinden).
- Booster'lar: makas, değnek, çekiç, saat (+20 sn); stok ve kullanılamayacağı durumlar web'le aynı.
- Sonuçlar: kazandı / süre doldu / bomba patladı / bıraktı. Bittikten sonra hiçbir hamle ve booster kabul edilmez.
- Olaylar: Presentation'ın dinleyeceği olaylar (parça çıktı, sayaç değişti, supap, booster, süre, sonuç). Oturum durumu dışarıdan yalnızca komutlarla değişir.
- Deneme kaydı modeli (gönderim değil): `level_id`, `level_version`, `client`, deneme no, süre, sonuç, kalan süre, takılma sayısı, booster başına kullanım, yıldız. Tanımlar karar kaydında (2026-10-05 gece). Deneme sayısı ve yıldız oturuma dışarıdan verilir/oturumdan okunur; kalıcı saklama M5.
- Açılış kontrolünün genişletilmesi: web build'inde 40 seviyenin kayıtlı çözümü GameSession ile oynatılır, süre panelinde sonuç yazar (stripping'in oturum kodunu bozmadığının dinamik kanıtı). M3'te panelle birlikte kalkar.
- Kapsam dışı: görsel, girdi, Supabase/PlayerPrefs (M3–M5), Monte Carlo (Faz 2), yeni seviye/süre tasarımı (Faz 4).

Kabul ölçütleri:
1. EditMode, README garantileri: 40/40 seviyenin kayıtlı çözümü GameSession'da çivi/zincir/bomba/mühürle oynanır ve **kazandı** ile biter; patlayan bomba 0, donma 0, emniyet supabı 0 kez.
2. EditMode, işlem sırası: sınır durumları için senaryo testleri: aynı hamlede çivinin serbest kalması, bombanın 0'a inmesi, bombalı parçanın kendisinin çıkarılması, son parçanın çıkarıldığı hamlede fitilin 0'a inmesi. Beklenen sonuç web koduyla belirlenir: mümkünse M1'deki gibi web kodu Node'da çalıştırılarak altın senaryo üretilir; mümkün değilse her beklenti web kodunda ilgili satıra referansla yazılır ve bu yöntem rapora yazılır.
3. EditMode, emniyet supabı: tetiklendiği yapay bir tahtada en uzun bekleyen çiviyi söker; web'le aynı seçim.
4. EditMode, booster'lar: her biri için etki, stok düşümü ve reddedildiği durumlar. Değnek deterministik `IRandom` ile: aynı tohum aynı tahta; farklı tohumlar farklı tahta; web'in değiştirmediği kenarlara (düz/tahta kenarı vb.) dokunmaz. Saat süreye tam 20 sn ekler.
5. EditMode, sonuçlar: süre dolması (sınır değerinde dahil), bomba, kazanma, bırakma; her denemede tam olarak bir kayıt; bitişten sonra komutlar reddedilir ve olay üretmez.
6. EditMode, deneme kaydı: alanlar doğru doluyor (yukarıdaki tanımlarla); deneme no artıyor; bırakma kaydı üretiliyor; yıldız web formülüyle aynı.
7. EditMode, determinizm: aynı seviye + tohum + komut/Tick dizisi → birebir aynı olay listesi ve kayıt (tekrar oynatma ve Monte Carlo için).
8. Rapor (engelleyici değil): 40 seviyede değnek sonrası açgözlü çözücünün çözebildiği oran (ör. seviye başına birkaç tohum).
9. `CoreIsolationTests` ve mevcut testler geçer. Web build size-log'a yazılır (10 MB bütçesi); genişletilmiş açılış kontrolü web build'inde 40/40 kazandı gösterir.

## Zorluk deneyi ilkeleri (Faz 2–4)

- Yeni engeller sırayla tanıtılır: 3, 7, 12 ve 20. seviyeler.
- Her engel için zorluğa ve süreye etkisi ölçülür; sonuç "zorluk tablosu"nda tutulur (engel ağırlığı, ek süre, seviye başına tahmin ve gerçek değer).
- Her deney koşulu için en az 2–3 farklı seviye olur; tek seviyede engelin etkisi yerleşimin etkisinden ayrılamaz.
- Deney seviyeleri test modunda her oyuncuya rastgele sırayla sunulur (öğrenme ve bırakma etkisini dengelemek için).
- Önce tekli engellerin ana etkisi: aynı engelden 1, 2 ve 4 adet (doğrusallık kontrolü). İkili ve üçlü etkileşimler ancak veri yeterse.
- Monte Carlo yapısal zorluğu ölçer (çözüm uzunluğu, geçerli hamle sayısı, rastgele oynayanın çözme olasılığı). Model, bu özellikler + engel sayılarıyla insan metriklerini tahmin eder.
- İnsan metrikleri veri toplanmadan önce tanımlanır: deneme sayısı, süre, başarısızlık oranı, bırakma oranı.
- Model, kurulurken kullanılmayan seviyelerde doğrulanır.
- Teknik ön koşullar: seviye verisinde `id` + `version`; engel sayıları seviyeden hesaplanabilir; oyun mantığı saf C# (editörde ekransız çözücü çalışabilsin).

## Açık işler

### Mühendislik
- [x] Boyut çalışması adım A–F (paketler, URP, splash, stripping, Optimize Size, DiskSizeLTO): 11,81 → 8,64 MB
- [x] Ana ekran alt şeridi: arka plan kamera rengine (#314D79) eşitlendi (iPhone doğrulaması Zeyd'de)
- [x] Yedek: ilk push (a9fb333) yapıldı, gizli anahtar taraması temiz
- [x] EditMode testleri (1/1) + commit/push (bee34ed, 28b2c54), anahtar taraması temiz
- [x] Brotli build ve PC ölçümü (G: 6,81 MB, JS açma 1,55 sn)
- [x] gh-pages dalı: `/gz/` (F), `/br/` (G), kök sayfa, .nojekyll (Pages repoda açılınca yayında)
- [x] UI Toolkit zaman kutusu: çıkarılamaz (Input System bağımlılığı), kapandı
- [x] M1: 227 EditMode testi, gh-pages `/gz/` ve `/br/` M1 build'i (e1cf67f; gh-pages 0552040)
- [x] GitHub Pages yayını doğrulandı (gh-pages bb41921, 13:35 success): kök 200 (→ `/gz/` yönlendirme), `/gz/` 200, `/br/` 200; canlı adreste 40/40
- [ ] 1. M2 (bkz. "M2 tanımı", 9 kabul ölçütü), 2026-10-06'da başlatıldı. Bitince build'i gh-pages `/gz/`'ye koy (kök yönlendirmesini koru), size-log'a yaz, PM'e raporla
- [ ] 2. 4G ölçümü gelince karar kuralına göre sıkıştırmayı kesinleştir (Brotli seçilirse main'de varsayılanı değiştir ve kök yönlendirmesini `/br/`'ye çevir)
- [ ] 3. M3'te: geçici `BootLevelCheck` ve süre panelini kaldır
- [ ] 4. Zeyd eski re-run'ı iptal edemeden o çalışırsa ve kök eski iki bağlantılı sayfaya dönerse: boş commit ile gh-pages'i yeniden yayınla

### Zeyd
- [x] GitHub Pages'i aç (gh-pages, / (root)) — açıldı; ilk 404 GitHub runner kuyruğundandı, çözüldü
- [ ] Actions'ta kuyrukta bekleyen eski re-run'ı (commit 0552040) iptal et
- [ ] iPhone'da kök adresi aç: `/gz/`'ye gittiğini ve süre panelinde "40 seviye okundu, 40 çözülebilir" yazdığını kontrol et (M1 ölçüt 8), ekran görüntüsü getir
- [ ] 4G ölçümü: `/gz/` ve `/br/` doğrudan, sırayla dönüşümlü, her açılıştan önce önbellek temizliği, her adres 3 kez; panel değerleri + başta ve sonda hız testi (protokol PM çıktısında)
- [ ] Ana ekran modunda (Paylaş → Ana Ekrana Ekle) alt şeritte farklı renk kalmadığını kontrol et, ekran görüntüsü getir
- [ ] Yerel sunucu artık gerekmiyor: güvenlik duvarında Python'un "Ortak" ağ iznini kaldır

## Backlog

- Unity içinde seviye editörü
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- Eski Input Manager'a geçiş (yalnızca 4G ölçümü 10 sn'yi aşarsa yeniden değerlendirilir)
- Ek boyut adımları (mscorlib/URP küçültme vb.): 4G ölçümü hedefi tutarsa yapılmaz
- UI Toolkit'i çıkarmak için URP/uGUI fork'u: reddedildi, yalnızca kayıt için
- 3D fizik modülü (PhysX, URP/uGUI bağımlılığıyla geri geldi) çıkarılabilir mi: ölçülmedi; 4G ölçümüyle birlikte değerlendirilir
