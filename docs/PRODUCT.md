# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-07 gece (M3 ve M4 kapandı: iPhone 60 FPS + Zeyd'in genel onayı; M5-ön 1–7 kabul, gönderimi canlıya açma ve TEST_GECE doğrulaması onaylandı; seviye editörü tanımı eklendi ve sıradaki ana iş)

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
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0–M4 bitti (M3 ve M4 2026-10-07: iPhone 60 FPS + Zeyd'in genel onayı; madde madde kontroller sabahki telefon oturumunda, engelleyici değil). GitHub Pages yayında, kök → `/br/`. M5-ön ölçüt 1–7 kabul (2026-10-07 gece); 002 SQL çalıştı (anon okumayla doğrulandı). **Şu an:** gönderimi canlıya açma (bayrak → build → yayın → TEST_GECE doğrulaması → PM onayı). Kalan: M5-ön ölçüt 8 (canlıda kalıcılık + bıraktı), M6 (L01 öğretici el zorunlu + cila), playtest öncesi 4G (LTE) teyidi. M6 seviye editörünün arkasında (karar 2026-10-07 gece) |
| 2. Ölçüm altyapısı | Telemetri (client, level_version), test modu (rastgele sıra), ekransız çözücü + Monte Carlo | Kısmen: telemetri (client, level_id, level_version, row_id) M5 ile canlıya açılıyor. Test modu ve Monte Carlo bekliyor (editörün isteğe bağlı Monte Carlo paneli ilk adım olabilir) |
| 3. Deney | Deney seviyeleri, playtest ile veri toplama | Hazırlık: **seviye editörü** (araç; Faz 3 deney seviyeleri ve Faz 4 yeniden tasarımı bununla yapılır) 2026-10-07 gece başladı, bkz. "Seviye editörü tanımı" |
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
| 2026-10-06 | **Hosting: GitHub Pages'te kalınır, Netlify yedek plan (Zeyd'in kararı).** 2026-10-05 hosting kararını teyit eder; o karardaki "Netlify'a gerek yok" ifadesi "Netlify yedek plan" olarak güncellenir. Netlify'a geçiş yalnızca aşağıdaki tetikleyicilerden biri gerçekleşirse PM'e gelir; kendiliğinden yapılmaz | Pages çalışıyor (ilk 404 GitHub runner kuyruğundandı, yönlendirme commit'iyle yayın başarılı). Netlify'ın artısı: `Content-Encoding` ayarlanabildiği için tarayıcı dosyaları kendisi açar (JS açma ve Brotli açma maliyeti kalkar) ve deploy GitHub runner'ına bağlı değildir. Bu artılar henüz ölçümle gerekli görünmedi; ölçmeden geçiş yok |
| 2026-10-06 | **Netlify tetikleyicileri (biri yeterli).** (A) Güvenilirlik: 2026-10-06'dan itibaren 30 gün içinde 2 yayın olayı, ya da playtest süresince 1 olay. Olay = push'tan sonra canlı adres 30 dk içinde güncellenmez/404 verir ve "pages build and deployment" yeniden tetiklemesi (boş commit) 30 dk içinde düzeltmez. (B) Performans: 4G ölçümünde kazanan sıkıştırmanın medyan toplam süresi > 10 sn **ve** medyan JS açma çıkarıldığında ≤ 10 sn'ye iniyor; ya da medyan JS açma toplamın ≥ %20'si veya ≥ 1,5 sn. (B) tetiklenirse sıra: önce Netlify denemesi (kod değişikliği yok), boyut işi (Input Manager, backlog boyut adımları) ancak Netlify de 10 sn'yi tutturamazsa açılır. Geçiş, Netlify'da aynı protokolle ölçülen medyan toplam süre Pages'tekinden ≥ 0,5 sn daha iyiyse (B) ya da (A) olayından sonra kesinleşir | Ölçülebilir eşikler; ilk 404 (2026-10-05) olay sayılmaz, nedeni bilinip yöntemi kayda geçti. ≥ 0,5 sn eşiği sıkıştırma kuralıyla aynı gürültü payı. Playtest linki değişirse oyunculara yeni link gerekir; bu yüzden geçiş tercihen playtest başlamadan yapılır |
| 2026-10-06 | **Varsayılan sıkıştırma Brotli (Zeyd'in kararı).** main'de varsayılan Brotli; kök adres `/br/`'ye yönlendirir (sorgu parametreleri korunur); `/gz/` yedek olarak kalır | iPhone Safari, Pages, önbellek boş, 3+3 ölçüm: medyan toplam Brotli 1,83 sn, gzip 2,64 sn; fark 0,81 sn ≥ 0,5 sn kuralı. Ölçüm 5G'de yapıldı (protokol 4G idi); ancak Brotli'nin avantajı indirmeden gelir ve ağ yavaşladıkça büyür, JS açma maliyeti (iPhone'da ~0,46 sn, PC'deki 1,55 sn tahmininden çok düşük) ağdan bağımsızdır. Yani 4G'de sonucun gzip lehine dönmesi beklenmez; karar 4G'yi beklemeden kesinleşir |
| 2026-10-06 | **Sıkıştırma için 4G karşılaştırması kapandı; yerine playtest öncesi tek bir 4G teyidi.** Yalnızca kök adres (Brotli), iPhone'da 5G kapalı (LTE), 3 açılış, önbellek boş. Sonuç başarı ölçütünü ("4G ≤ 10 sn") belgelemek içindir; M2–M5'i engellemez, playtest başlamadan önce gelir. Medyan > 10 sn çıkarsa 2026-10-05 gece kuralındaki boyut işi ve Netlify tetikleyicisi (B) yeniden değerlendirilir | Başarı ölçütü 4G'de tanımlı ve 5G onu kanıtlamaz; ama karşılaştırma (iki adres, dönüşümlü) artık gereksiz, tek adres yeterli. 6,4 MB'ın 10 sn'yi aşması için ağın ~6 Mbit/s altına düşmesi gerekir; risk düşük, teyit ucuz |
| 2026-10-06 | M1 ölçüt 8 karşılandı: iPhone'da her açılışta "40 seviye okundu, 40 çözülebilir". Ana ekran modundaki alt şerit sorunu kapandı (Zeyd doğruladı) | Zeyd'in iPhone kontrolü |
| 2026-10-06 | Boyut işi kapalı kalır: Input Manager'a geçiş, ek boyut adımları ve PhysX denemesi yapılmaz (4G teyidi > 10 sn çıkmadıkça) | Medyan toplam 1,83 sn, hedefin çok altında. Ölçmeden optimizasyon yok |
| 2026-10-06 | **Netlify tetikleyicisi (B) düzeltildi; bu ölçümde tetiklenmedi.** Yeni tanım: (B) yalnızca kazanan sıkıştırmanın medyan toplam süresi ≥ 7 sn iken değerlendirilir; o durumda tetiklenir: medyan JS açma toplamın ≥ %20'si veya ≥ 1,5 sn, ya da toplam > 10 sn ve JS açma çıkarıldığında ≤ 10 sn'ye iniyor. Toplam < 7 sn ise (B) yoktur. Ölçüm kaynağı: playtest öncesi 4G teyidi (yoksa en son iPhone ölçümü) | Eski tanımın yüzde koşulu, toplam süre çok kısayken anlamsız biçimde tutuyordu (0,46 / 1,83 ≈ %25). Netlify'ın en fazla kazandırabileceği JS açma süresidir (0,46 sn); geçiş kuralı ≥ 0,5 sn iyileşme ister, yani bu veride Netlify denemesi kural gereği geçişi zaten haklı çıkaramazdı. Tetikleyicinin amacı 10 sn hedefini korumak; 7 sn eşiği hedefe ~3 sn pay bırakır |
| 2026-10-06 | **M2 ölçüt 1 ikiye ayrıldı (1a bağlayıcı, 1b rapor).** 1a: çivilere uyan basit oynayıcının web harness'taki hamle listesi altın senaryo; GameSession gerçek komut yoluyla 40/40 web'le birebir aynı sonucu verir (patlamalar dahil). Testlere çivi kontrolünü atlayan yol eklenmez. 1b: akıllı oynayıcı (çivi uyumlu, bombaya sınırlı aramayla) README garantisini ölçer; hedef 40/40, tutmayan seviye rapor verisi. Ölçüt 9'daki açılış kontrolü "40/40 web'le aynı" olarak değişir | Bulgu: kayıtlı çözüm üretecin soyma sırası; çivili parçaları oynatıyor (oyunda imkânsız) ve web kodunda 8 seviyede (L22, L24, L25, L28, L29, L30, L35, L38) bomba patlatıyor. Eski ölçüt web'in kendisinin de geçemeyeceği bir şeyi istiyordu. Taşıma doğruluğunun kanıtı "web'le aynı davranış"tır, "kazandı" değil. Gerçek komut yolu testin oyuncunun yapabileceği şeyi sınamasını sağlar. Akıllı oynayıcı Faz 2 çözücü/Monte Carlo'nun temeli |
| 2026-10-06 | **1b'de tutmayan seviye: M2'yi engellemez; rapora ve Faz 4 tasarım girdisine yazılır.** Arama sınırına takılan ("doğrulanamadı") ile gerçekten kaybettiren (aramada hiçbir yol yok) ayrı raporlanır. İkincisi seviye verisi sorunudur: seviyeler zaten Faz 4'te yeniden tasarlanacak ve playtest Faz 3'teki deney seviyeleriyle yapılacağından şimdi düzeltilmez. Yeniden tasarımda kural: her seviye akıllı oynayıcıyla 0 patlama/0 donma/0 supapla çözülebilir olmalı | Taşıma fazında seviye değiştirmek web verisiyle karşılaştırmayı bozar (2026-10-05 kararı). M1'deki açgözlü çözücü kararıyla aynı mantık |
| 2026-10-06 | README'deki "patlayan bomba 0, donma 0, supap 0" garantisi kayıtlı çözüm için doğru değil; garanti 1b sonucu gelene kadar "doğrulanmamış" sayılır. Kayıtlı çözüm yalnızca M1 ölçüt 1 (engelsiz çıkış geçerliliği) için kanıt olarak kalır | Web harness bulgusu; case study'de veri kalitesi notu olarak kullanılabilir |
| 2026-10-06 | Süre panelindeki "tarayıcı yerel açtı mı" satırı mühendislikçe düzeltilebilir (yalnızca Unity dosyalarına bakacak şekilde); en fazla ~30 dk, M2'yi bekletmez. Önceki "yok sayılır" kararı karar verisi açısından geçerli kalır | 4G teyidi hâlâ bu paneli kullanacak; yanıltıcı satır Zeyd'in okumasını karıştırmasın. Panel M3'te kalkıyor, bundan fazla emek değmez |
| 2026-10-06 | Kök adres `/br/`'ye yönlendiriyor (gh-pages fb5ca44, sorgu parametreleri korunuyor, canlıda doğrulandı); main'de varsayılan Brotli + panel "yerel açtı mı" satırı düzeltildi (bc053ab). İkisi de kapandı | 2026-10-06 Brotli kararının ve panel kararının uygulanması |
| 2026-10-06 | **M2 kapandı.** Kabul 1a ve 2–8 karşılandı; 9 iPhone hariç karşılandı (PC'de başsız Chrome'da iki build de "oturum 40/40 web'le aynı"). iPhone'da oturum satırı Zeyd'in kök adres kontrolüne eklendi; M3'ü engellemez | 322 EditMode testi; 40/40 seviyede her adım web'le birebir (37 kazandı, L24/L29/L38 bomba, web'le aynı). M1'deki gerekçeyle aynı: stripping'li build Chrome'da doğruysa Safari'de farklı sonuç için sebep yok |
| 2026-10-06 | **1b sonucu: README garantisi Unity portunda doğrulandı** (akıllı oynayıcı 40/40 kazandı, patlama 0, donma 0, supap 0; arama yalnızca L24/L29/L38'de, 28–213 düğüm). 2026-10-06 "doğrulanmamış" kararı kapanır; Faz 4 backlog'una kazanılamayan seviye girmez. Arama kısıtlı (bomba konisi + 1 dolgu hamlesi); Faz 2 çözücüsü bunu genelleştirir, şimdilik yeterli | Ölçüldü; arama sınırına takılan seviye yok, kısıtlı aramanın eksikliği bu veride sonucu etkilemedi |
| 2026-10-06 | **Web'den bilinçli farklar kabul edildi:** (a) süre = oturumun saydığı aktif saniye (web'deki timer − kalan, saat booster'ıyla bozuluyordu); (b) bırakma dahil her deneme bir kayıt; (c) takılma GameSession'da yalnızca sayılır, kuralı Presentation uygular: çivili parçaya basma ve sürüklemeyi durma noktasının ötesine itme, sürükleme ve yön başına bir kez. Case study'de "web telemetrisi düzeltmeleri" olarak anlatılır; web verisiyle karşılaştırmada süre ve deneme alanları bu farkla okunur | 2026-10-05 gece insan metrikleri kararının uygulanışı; oynanışı değiştirmiyor, yalnızca ölçümü düzeltiyor |
| 2026-10-06 | **Supabase `level_id` sütunu M5'te eklenir** (şimdi değil). M5'in ilk adımı: mühendislik SQL'i hazırlar (boş bırakılabilir metin sütun; web oyunu etkilenmez), Zeyd çalıştırır, sonra gönderim kodu yazılır. M5 kabulüne eklenir: Unity satırlarında `level_id` + `level_version` + `client` dolu | Şu an gönderim yok; sütunu erken eklemek bir şey kazandırmaz, Zeyd'e şimdi iş çıkarır. Karar (level_id + version eşleşmesi) değişmedi, yalnızca zamanlaması M5 |
| 2026-10-06 | **M3 = tahta görseli + sürükleme** (bkz. "M3 tanımı"). Menüler, booster arayüzü ve HUD cilası M4'te. Seviye seçimi M3'te yalnızca `?lv=N` sorgu parametresi ve bitişte basit "tekrar / sonraki" ile | Playtest yolundaki sıradaki darboğaz: oyuncunun tahtayı görüp parmakla oynayabilmesi. Sürükleme hissi ve takılma kuralı telemetri verisini doğrudan etkiler, menülerden önce doğrulanmalı |
| 2026-10-06 | **Süre paneli ve açılış kontrolü M3'te silinmez; yalnızca `?debug=1` ile görünür, varsayılan kapalı** (2026-10-05 "M3'te kalkar" kararını değiştirir). Debug modunda ayrıca FPS / kare süresi gösterilir | 4G teyidi ve M3 performans ölçümü bu panele dayanıyor; oyuncu görmez, ölçüm kaybolmaz. Boyut maliyeti ihmal edilebilir |
| 2026-10-06 | M2 ölçüt 9'un iPhone kısmı karşılandı: kök adres `/br/`'ye gidiyor, panelde "oturum ... web'le aynı" (Zeyd). M1 ve M2'nin iPhone ölçütleri tamamen kapandı; kök yönlendirmesi iPhone'da doğrulandı | Zeyd'in iPhone kontrolü |
| 2026-10-06 | **M3 görsel yön (Zeyd'in kararı): web görünümü birebir.** Renk paleti, parça şekilleri ve engel ikonları web sürümüyle aynı. Yeni görsel kimlik ayrı bir iş, sonraya (backlog). 2026-10-05 görsel yön kararını daraltır | Taşıma fazında görünüm değişmesin; web verisiyle karşılaştırma ve playtest öncesi risk azalır |
| 2026-10-06 | **M3 okunabilirlik (Zeyd'in kararı):** engeller iPhone SE genişliğinde ayırt edilir; dokunma alanı ≥ 44 pt (görsel küçükse dokunma alanı görselden büyük olabilir); sürüklerken sayfa kaymaz ve yakınlaşmaz | Telefonda doğru görünüm; yanlış dokunma telemetriyi (takılma, deneme) bozar |
| 2026-10-06 | **M3 efektleri minimum (Zeyd'in kararı):** parça parmağı gecikmesiz izler (sürükleme sırasında yumuşatma/lerp yok, aynı karede parmak konumu); bırakınca kısa oturma animasyonu; bomba ve kazanma için basit geri bildirim. Diğer efektler (çıkış, çivi sökülmesi, supap partikülleri vb.) M6 cilalama adımına. M3 tanımındaki "kısa tween/partikül" maddesi buna göre daraltıldı | Playtest yolunu tıkamayan kozmetik iş sonraya; sürükleme hissi veri kalitesini etkiler, efekt etkilemez |
| 2026-10-06 | **M3 ekran görüntüsü ölçütüne iPhone SE eklendi: 375×667 pt (SE 2./3. nesil), bağlayıcı.** 320 pt (SE 1. nesil) eklenmez | Güncel iOS'u çalıştıran en dar ve en kısa iPhone 375×667; hem genişlik (engel okunabilirliği) hem yükseklik (tahta + HUD sığması) için en kötü durum. 320 pt'lik cihazlar güncel iOS almıyor, playtest kitlesinde beklenmez; eklemek ölçeği gereksiz küçültür |
| 2026-10-06 gece | **Build engeli için Zeyd'e önerilen sıra: (0) bilgisayarı yeniden başlat + tek build denemesi → (B) Unity 6000.6.4f1'i WebGL modülüyle kaldırıp yeniden kur → (C) GitHub Actions bulut build → (D) başka bilgisayar → (A) Akıllı Uygulama Denetimi'ni kapatmak yalnızca son çare ve yalnızca Zeyd'in açık kararıyla.** ~~A GERİ DÖNÜŞSÜZDÜR~~ — **2026-10-07 düzeltme (Zeyd): yanlıştı.** Nisan 2026 güncellemesinden (KB5083769) beri SAC kapatıldıktan sonra tekrar açılabiliyor; bkz. 2026-10-07 kararı. Mühendislik güvenlik ayarlarına dokunmaz | Engel kod/proje değil, makine (17:35 M2 build'i sorunsuzdu; hata `WebGLPlayerBuildProgram.Data.dll` 0x800711C7). Ucuz ve geri alınabilir adımlar önce. C uzun vadede de değerli (makineden bağımsız, tekrarlanabilir build) ama lisans adımı ve Unity 6.6 imajı belirsizliği var; A hızlı ama kalıcı güvenlik kaybı, playtest takvimi bunu henüz gerektirmiyor |
| 2026-10-06 gece | **M3 kapanmaz: durum "kod tamam, build/yayın bekliyor".** Editörde karşılananlar: ölçüt 1 (ekran görüntüleri 375×667/390×844/430×932, en büyük tahtada hücre 56 pt ≥ 44), 1b'nin kod kısmı, 2, 3 (334 EditMode testi). Build'e bağlı olanlar açık: 5 (size-log), 6'nın web kısmı, 7 (yayın), 4 ve 8 (iPhone). Editörde Input System sanal fareyle sürükleme testi ek kanıt olarak kabul edilir ama iPhone ölçütlerinin yerine geçmez | Stripping hatasını yalnızca web build yakalar; dokunma, sayfa kaydırma/yakınlaşma ve FPS yalnızca Safari'de görülür. Bunlar M3'ün asıl riskleri; build'siz kapatmak ölçmeden kapatmak olur |
| 2026-10-06 gece | **M4 kodlamasına geçilir (editörde derlenip test edilerek); M5'ten bu gece yalnızca Supabase'e dokunmayan kısımlar** (PlayerPrefs ilerleme, deneme kayıt kuyruğu/serileştirme, `level_id` SQL taslağı). Supabase'e gerçek gönderim kodu SQL çalıştırılıp bir web build alınana kadar yazılmaz/denenmez. Build'siz birikim sınırı: M3 + M4. Build hâlâ alınamıyorsa M4'ten sonra yeni kapsam açılmaz; PM'e gelinir. İlk build alındığında M3 + M4 birlikte yayınlanır ve iPhone testleri tek seferde yapılır | Playtest yolunu beklemek boşa zaman; M4 Presentation işi, build engelinden bağımsız. Ama yayınlanmamış kod biriktikçe stripping/Safari hatasının kaynağını bulmak zorlaşır; sınır bunu tutar. Editörden canlı `plays` tablosuna gönderim, sütun yokken veri kirletir |
| 2026-10-06 gece | **Mühendislik bu gece C'yi hazırlar** (zaman kutusu ~1 saat): GitHub Actions workflow'u (yalnızca `workflow_dispatch`, gh-pages'e otomatik yayın yok; çıktı artifact olarak indirilir) + Zeyd için secret talimatı. Önce game-ci'de 6000.6.4f1 (WebGL) imajı olup olmadığına bakılır; yoksa durur ve raporlar, sürüm değiştirilmez. Repoya secret/lisans dosyası girmez. CI build'i gh-pages'e ancak yerel build'le aynı ayarları (gzip/Brotli, şablon, High stripping, size-log) ürettiği doğrulanınca bağlanır | Secret'sız çalışmaz ama hazır durması sabah Zeyd'in tek adımla denemesini sağlar; B tutmazsa A'ya gitmeden alternatif olur. Otomatik yayın, doğrulanmamış build'i playtest linkine koyabilir |
| 2026-10-06 | **"Cilalama adımı" = M6 (ses ve cila), playtest öncesi.** "Yeni görsel kimlik" Faz 3 veri toplama bitene kadar yapılmaz; en erken Faz 4 yeniden tasarımıyla birlikte (backlog) | Deney sırasında görünüm değişirse oyuncu verisi iki görsel koşula bölünür; zorluk modeli karışır |
| 2026-10-06 gece | **M4 kapanmaz: durum "kod tamam, build/yayın bekliyor"** (M3 ile aynı). Editörde 1, 2, 3 karşılandı (BoosterControls + 12 test; bıraktı kaydı testi; 9 ekran × 3 boyut, kırpılma yok, booster 60 pt, kart düğmeleri 46 pt, bölüm hücresi 375'te ≈49 pt, makas noktası dokunma hedefi 44 pt, alt şeritte arayüz yok). 4 açık: size-log, yayın, iPhone. Taslak ölçütler bu haliyle kesinleşti; iPhone'a M4 kontrolleri eklendi (bkz. M4 tanımı) | Gerekçe M3 kararıyla aynı: stripping ve Safari dokunma davranışı yalnızca web build'de görülür |
| 2026-10-06 gece | **Bölüm kilidi onaylandı:** önceki bölüm kazanılınca sonraki açılır; ilk bölüm hep açık; `?debug=1` hepsini açar. Kalıcılık M5. Faz 3 test modunda (rastgele sıra) kilit kuralı yeniden ele alınır | Web'in "her bölüme atlanabilir" davranışı playtest için yapılmıştı; normal oyunda sıralı ilerleme bölüm sırasıyla tanıtılan engellerin öğretimini korur. Debug açığı test ihtiyacını karşılar |
| 2026-10-06 gece | **M4'te mühendisliğin verdiği arayüz kararları onaylandı:** ana menü (logo, başlık, alt başlık, "Oyna · Bölüm N", "Bölümler"; `?lv=N` menüyü atlar); tanıtım kartı yalnızca bölümün ilk başlatılışında, açıkken süre işlemez (web'le aynı); bitiş kartına "Menü" düğmesi (web'den fark, taslak istiyordu); booster adları sabit büyük harf (WebGL kültür verisi riski). L01 öğretici eli → M6 (playtest öncesi zorunlu, L01 ilk deneyimi ve takılma verisini etkiler); oyuncu adı ekranı → M5-ön | Hepsi oynanışı değiştirmiyor; "Menü" düğmesi bırakma kaydını üretir, telemetri tanımına uygun. Öğretici el ilk açılış deneyiminin parçası, playtest'ten önce şart ama build engelini çözmez |
| 2026-10-06 gece | **Zeyd'in kararı: M5'in Zeyd'e bağlı olmayan kısmı şimdi yapılır; Supabase gönderim kodu yazılır ve sahte HTTP ile test edilir; SQL Zeyd tarafından çalıştırılana kadar canlıda gönderim kapalı kalır.** Bu, 2026-10-06 gece "gerçek gönderim kodu SQL + web build'den önce yazılmaz/denenmez" kararının gönderim kısmını ve "M4'ten sonra yeni kapsam açılmaz" sınırını değiştirir. PM notu: sınırın amacı (yayınlanmamış kod birikince stripping/Safari hatasının kaynağını bulmanın zorlaşması) geçerli; bu yüzden M5-ön'den sonra build alınmadan yeni kapsam açılmaz (M6 dahil), PM'e gelinir | Zeyd'in açık talimatı. Riski azaltan koşullar: gönderim varsayılan kapalı, editör ve testler canlıya hiç istek atmaz, canlıya açılış ayrı bir adım (SQL → bayrak → build → tek test satırı doğrulaması). Canlı tablo kirlenmez |
| 2026-10-06 gece | **Gönderimi canlıya açma kuralı:** (1) Zeyd SQL dosyasını Supabase'de çalıştırır ve doğrulama sorgusunun çıktısını getirir; (2) mühendislik bayrağı açar ve build alır; (3) Zeyd bir bölüm oynar, Supabase'de satırı kontrol eder (`client`='unity', `level_id`, `level_version` dolu, deneme no ve süre mantıklı); (4) PM onayı. Bayrak build'e gömülüdür; URL parametresiyle açılamaz. `?debug=1` oturumları hiçbir zaman gönderilmez. Kapalıyken üretilen kayıtlar sonradan gönderilmez | URL ile açılabilen gönderim, sütun yokken 400 hatası ve kirli veri riski taşır. Kapalı dönemdeki kayıtlar geliştirme/test oyunlarıdır; playtest verisine karışmamalı |
| 2026-10-06 gece | **Kayıt tekrarı olmayacak:** ağ hatasında yeniden deneme aynı denemeyi iki satır yapmamalı. Mühendislik her kayda istemcide benzersiz kimlik verir; gerekiyorsa SQL dosyası bu kimlik için ek sütun + tekil kısıt içerir (Zeyd tek seferde çalıştırır) | Deneme sayısı zorluk modelinin ana metriği; çift satır modeli doğrudan bozar |
| 2026-10-07 | **SAC notu düzeltildi (Zeyd):** Akıllı Uygulama Denetimi, Nisan 2026 güncellemesinden (KB5083769) beri kapatıldıktan sonra tekrar açılabiliyor; 2026-10-06 gece kaydındaki "geri dönüşsüz" ifadesi yanlıştı. Zeyd SAC'ı kapattı, yerel build engellenmiyor. **Bulut build (C) yedek olarak kalır, acil değil;** Zeyd'in secret eklemesi gerekmez. SAC'ı tekrar açmak Zeyd'in kararı; açarsa ve build yeniden engellenirse PM'e gelinir | Zeyd'in düzeltmesi. Engel makine ayarıydı, kod değil; yerel build 921 sn'de sorunsuz bitti |
| 2026-10-07 | **M3+M4+M5-ön build'i kabul edildi (build-tarafı).** Brotli 7,15 MB (M2'ye göre +290 KB, +%4,2; +1 MB eşiğinin altında), gzip 9,04 MB (10 MB bütçesinde). Font net etkisi ~60–80 KB (statik 512×512 Alpha8 atlas, 115 glif, TMP hazır TTF'i build dışı); artışın kalanı TMP/uGUI kodu ve M3/M4 ekranları. Boyut işi açılmaz. Karşılanan: M3 ölçüt 5, 6, 7; M4 ölçüt 4; M5-ön ölçüt 8'in "gönderim kapalı" kısmı (canlıda Supabase isteği 0). M3/M4 iPhone ölçütleri (M3 4, 8; M4 5) açık | Ölçüldü (size-log, headless Chrome canlı adreslerde 40/40 + oturum 40/40, istisna 0, Türkçe karakterler doğru). Headless FPS (5–6) ve süreler yazılım GPU'su nedeniyle karar verisi değil |
| 2026-10-07 | **Brotli bütçe uyarısı: 7,15 MB'ta 4G riski hâlâ düşük.** 10 sn'yi aşmak için ağın ~6,5 Mbit/s altına düşmesi gerekir. 4G teyidi playtest öncesi aynen kalır; M6 sonrası build'de alınır (son boyutla) | 5G medyanı 1,83 sn idi; +290 KB bunu anlamlı değiştirmez. Teyidi son build'le yapmak bir ölçümü boşa harcamaz |
| 2026-10-07 | **Sıra: (1) M5-ön raporu PM'e (önce), (2) M6 başlar; iPhone'da çıkan M3/M4 hatası M6'nın önüne geçer.** M5-ön kodu (1f3e2fc) bu build'de ama ölçüt 1–7'nin kanıtı PM'e raporlanmadı; ölçüt 8'in kalıcılık ve sayfa kapanışı kısmı canlı build'de doğrulanmadı. M6 kapsamı: L01 öğretici el (zorunlu) + M3'te ertelenen minimum dışı efektler (çıkış, çivi sökülmesi, supap) + ses. Her M6 adımı build + yayınla biter (yayınlanmamış kod birikmez) | Build engeli kalktı; 2026-10-06 gece "M5-ön'den sonra build olmadan yeni kapsam yok" sınırının koşulu sağlandı. Ama M5-ön PM'ce kapanmadan M6 açmak doğrulanmamış iş biriktirir. iPhone hataları (dokunma, FPS) veri kalitesini doğrudan etkiler, cila etkilemez |
| 2026-10-07 | **iPhone test protokolü:** kilit, ilerleme ve isim kapısı kontrolleri `?debug=1` OLMADAN ve yeni özel sekmede (temiz oyuncu) yapılır; `?debug=1` yalnızca FPS ve booster kontrolü için (kilitleri açar, gönderimi kapatır). Kalıcılık kontrolü normal sekmede yenileyerek | Debug modu kilitleri açtığı ve M5-ön kalıcılığı devrede olduğu için, karışık sırada yapılan test yanlış "kilit çalışmıyor" ya da "ilerleme kayboldu" sonucu verir |
| 2026-10-07 gece | **SAC kaydı kontrol edildi:** PRODUCT.md'de "geri dönüşsüz" ifadesi yalnızca 2026-10-06 gece kaydında üstü çizili ve düzeltme notuyla duruyor (tarihsel kayıt, doğru). `docs/OVERNIGHT.md`'deki eski rehberde (madde 5) hâlâ "geri dönüşsüz" yazıyor; mühendislik orada eski metni silmeden altına düzeltme notu ekler | Karar kaydı değiştirilmez, düzeltilir; eski gece raporu okuyanı yanıltmasın |
| 2026-10-07 gece | **M3 ve M4 kapandı.** M3 ölçüt 4 karşılandı (Zeyd, iPhone, canlı adres: sürüklerken 60 FPS ≥ 55). M3 ölçüt 8 ve M4 ölçüt 5 Zeyd'in genel onayıyla ("oyun çalışıyor, süper") kapatılır; madde madde liste raporlanmadığı için kısa kontrol listesi sabahki telefon oturumuna eklenir (bkz. açık işler). O listede çıkan hata yeni bir hata kaydıdır: veri kalitesini etkiliyorsa (dokunma, kilit, kayıt) editörden ve M6'dan önce düzeltilir, kozmetikse M6'ya | Ölçümlü tek ölçüt (FPS) karşılandı; Zeyd baştan sona oynadı ve sorun görmedi. Kalan maddelerin build-tarafı kanıtı headless Chrome'da var (40/40, sürükleme, booster kilitleri, L40 stokları). Zeyd zaten M5 test satırı için telefonda oynayacak; ayrı bir oturum açmak playtest yolunu boşuna bekletir |
| 2026-10-07 gece | **Zeyd'in kararı: seviye editörü backlog'dan çıkar, bu gecenin ana işi ve M6'nın önünde.** PM itiraz etmez. Sıra: (1) gönderimi canlıya açma (kısa, zaten başladı), (2) seviye editörü (bkz. "Seviye editörü tanımı"), (3) M6. 2026-10-07 "sıra" kararındaki "M5-ön raporundan sonra M6" bu kararla değişir. Koşul: L01 öğretici eli playtest öncesi zorunlu kalır; playtest tarihi konuşulmaya başladığında M6 editörün isteğe bağlı maddelerinin önüne geçer | Editör playtest yolunun dışında değil, üzerinde: playtest Faz 3 deney seviyeleriyle yapılacak (2026-10-06 kararı) ve deney seviyeleri ancak bu araçla, Zeyd'in kendisi tarafından kurulabilir. M6 cila + öğretici el, deney seviyeleri hazır olmadan playtest'i başlatamaz. Editör Unity editöründe çalıştığı için yayındaki oyunu riske atmaz |
| 2026-10-07 gece | **M5-ön ölçüt 1–7 kabul edildi.** Kanıt: 365 EditMode testi geçiyor (TelemetryTests: kalıcı kuyruk, gönderici yeniden deneme/ayırma, PlayRow 19 sütun altın JSON); bayrak kapalıyken ve `?debug=1`'de sahte HTTP çağrısı 0, canlıda Supabase isteği 0; anahtar taraması temiz (yalnızca publishable); 002 SQL çalışmış (anon okuma: `level_id`, `row_id`, `client`, `level_version` sütunları var, eski satırlarda `level_id` backfill'i doğru — lv 40 → "L40", `row_id` boş). Ölçüt 8'in kalıcılık ve sayfa kapanışı kısmı TEST_GECE akışında, gönderim açık build'de doğrulanır; M5-ön o raporla kapanır | Test ve canlı ölçümle kanıtlandı. Unique kısıt anon okumayla görülemez; ilk `on_conflict=row_id` insert'ünün kabulü dolaylı kanıttır (kısıt yoksa 400/42P10) |
| 2026-10-07 gece | **Gönderimi canlıya açma onaylandı; kuraldaki Zeyd adımı (3) gece için mühendisliğin başsız Chrome doğrulamasıyla yer değiştirir.** Sıra: bayrak aç → Brotli + gzip build → size-log → gh-pages `/br/` + `/gz/` (kök → `/br/` korunur) → canlı kök adreste `?debug=1` OLMADAN, oyuncu adı `TEST_GECE`, L01 bir kez oynanır → anon okumayla doğrulanır. **Geçme koşulları:** `client`='unity', `level_id`='L01', `level_version`=1, `row_id` dolu ve benzersiz, deneme no = 1, süre > 0 ve gerçek oyun süresiyle tutarlı, sonuç doğru, oyuncu adı TEST_GECE; 1 deneme = tam 1 satır (aynı `row_id` ile ikinci satır yok). Ek: aynı sekmede sayfa yenilenince isim ve L01 kazanımı korunur (ölçüt 8 kalıcılık); L02'de yarım denemede sekme kapatılınca/gizlenince tek "bıraktı" satırı gelir (ölçüt 8 sayfa kapanışı); `?debug=1` ile bir deneme oynanır ve satır gelmez. **Başarısızlıkta:** bayrak kapatılır, build alınıp yayınlanır (sabaha kadar yanlış veri toplanmaz), hata sabah raporuna. RLS okumayı engellerse doğrulama Zeyd'e kalır. Son adım PM onayı (rapor bana gelir); Zeyd'in iPhone satırı sabah listesinde | Zeyd uyuyor; kanıt aynı (canlı build + gerçek tablo), anon okuma mümkün. Ölçüt 8 aynı akışta kapanır, ayrı build gerekmez. Yanlış veri zorluk modelini doğrudan bozar, bu yüzden geri dönüş yolu baştan tanımlı |
| 2026-10-07 gece | **Test ve geliştirme satırları analizden çıkarılır:** (a) oyuncu adı `TEST_` ile başlayan tüm satırlar (TEST_GECE dahil); Zeyd kendi test oyunlarında `TEST_ZEYD` adını kullanır; (b) playtest başlangıç tarihinden önceki tüm `client`='unity' satırları. Playtest başlangıç tarihi playtest açılırken bu dosyaya yazılır. Satırlar silinmez, sorguda filtrelenir | Gönderim artık canlıda açık: playtest'ten önce kök adresi açan herkes (Zeyd, arkadaşlar) satır üretir. Silmek Supabase şemasına/verisine dokunmak olur; filtre geri alınabilir ve case study'de "veri temizliği" olarak anlatılır |
| 2026-10-07 gece | **Seviye editörü tanımı ve öncelik sırası onaylandı** (bkz. "Seviye editörü tanımı"): E1–E5 + E7 bu gecenin zorunlu kısmı (Zeyd sabah ilk seviyesini kurup telefonda oynayabilsin), E6 ve ikinci kademe "vakit kalırsa", Monte Carlo ve rastgele üretici üçüncü kademe. Teknik yol mühendisliğin (Play modunda BoardView'i kullanan editör sahnesi uygun); tek şart parçaların oyundakiyle aynı görünmesi | Sabahki değer "Zeyd tek başına bir seviye kurup telefonda oynayabiliyor mu"dur; kalan her şey bu akışı güzelleştirir. Bir gecede bitmeyebilir; yarım kalan bir zorunlu parça, tamamlanmış bir isteğe bağlı parçadan değerlidir |

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
| 2026-10-06 | M1, gzip, Pages `/gz/` doğrudan, **iPhone Safari, 5G (4G DEĞİL)**, fast.com 540 Mbps, her açılış yeni özel sekme | 8,2 MB | **medyan 2,64 sn** (2,41 / 2,66 / 2,64) | JS açma ~0,26 sn. Her açılışta 40/40. Sıkıştırma kararı verisi |
| 2026-10-06 | M1, Brotli, Pages `/br/` doğrudan, aynı koşullar (5G) | 6,4 MB | **medyan 1,83 sn** (2,19 / 1,66 / 1,83) | JS açma ~0,46 sn (toplamın ~%25'i; PC tahmini 1,55 sn'den çok düşük). Her açılışta 40/40. gzip'ten 0,81 sn hızlı → Brotli varsayılan |
| 2026-10-06 | M2, Brotli (gh-pages 92bcba5, `/br/`, kök) | 6,86 MB (+15 KB vs M1 Brotli) | — (PC başsız Chrome; iPhone'da ölçülmedi) | "40 seviye okundu, 40 çözülebilir; oturum 40/40 web'le aynı (37 kazandı, 3 bomba)". Stripped Core'da GameSession, SmartPlayer, AttemptRecord, Mulberry32 var |
| 2026-10-06 | M2, gzip (`/gz/`) | 8,69 MB | — | Aynı kontrol, aynı sonuç. 10 MB bütçesinde |
| 2026-10-06 | M2 testleri ve raporlar | — | — | 322 EditMode testi (227 + 95). 1a: 40/40 her adım web'le birebir. 1b: akıllı oynayıcı 40/40 kazandı, 0 patlama/donma/supap. Ölçüt 8: değnek 35 seviye × 5 tohum = 175/175 uygulandı, sonrası 175/175 açgözlü çözücüyle çözülebilir |
| 2026-10-06 gece | M3 editör, sanal fare (Input System) | — | — | Takip hatası 0,00 pt; kısa sürükleme geri dönüyor, tam sürükleme çıkarıyor (31742aa). iPhone ölçütlerinin yerine geçmez |
| 2026-10-06 gece | M4 editör (ee11e3d) | — (build yok) | — | 346 EditMode testi. 375×667'de: booster düğmesi 60 pt, kart düğmeleri 46 pt, bölüm listesi hücresi ≈49 pt, makas eklem noktası dokunma hedefi 44 pt. 9 ekran × 3 boyut kırpılmasız. Sanal fare sürükleme yeni akışta da geçti |
| 2026-10-07 | M3+M4+M5-ön, Brotli (gh-pages fdab901 `/br/`, kök) | **7.147.435 B (7,15 MB; +290 KB / +%4,2 vs M2)** | — (iPhone'da ölçülmedi); yerel build 921 sn | loader 118.567 · framework 73.946 · wasm 5.320.924 (+158 KB) · data 1.633.998 (+130 KB). Font: atlas "RS Sans SDF" 512×512 Alpha8, 115 glif, ham 262.144 B → Brotli 58.180 B; font metadata 11.880 B; TMP SDF-Mobile shader 10.372 B; TMP hazır TTF (350.200 B, Brotli ~156.580 B) build dışı. Unity.TextMeshPro.dll stripping sonrası 277.504 B IL |
| 2026-10-07 | M3+M4+M5-ön, gzip (`/gz/`) | 9.042.175 B (9,04 MB; +350 KB vs M2) | — ; build 60 sn | loader 48.540 · framework 84.923 · wasm 6.877.732 · data 2.030.980 (atlas gzip 77.968 B). 10 MB bütçesinde |
| 2026-10-07 | M3+M4+M5-ön, canlı kök adres, **iPhone Safari** (Zeyd) | — | — | Sürüklerken **60 FPS** (M3 ölçüt 4 ≥ 55 karşılandı). Genel oynanış: "oyun çalışıyor, süper". Madde madde kontrol listesi raporlanmadı |
| 2026-10-07 gece | Supabase 002 SQL doğrulaması (anon, salt okuma, mühendislik) | — | — | `select=level_id,row_id,client,level_version&limit=1` → HTTP 200, `{"level_id":"L01","row_id":null,"client":"web","level_version":1}`. `order=lv.desc&limit=2` → 200, lv 40 → `level_id` "L40" (backfill doğru). Eski web satırlarında `row_id` null (beklenen). Unique kısıt (`plays_row_id_key`) anon ile görülemez; ilk `on_conflict=row_id` insert'üyle dolaylı doğrulanacak |
| 2026-10-07 gece | M5-ön testleri | — | — | 365 EditMode testi geçiyor (TelemetryTests dahil); bayrak kapalı/debug'da HTTP çağrısı 0; anahtar taraması temiz |
| 2026-10-07 | Canlı doğrulama, headless Chrome 375×667, SwiftShader (iPhone DEĞİL) | — | FPS 5–6, süreler anlamsız (yazılım GPU) | Kök: isim kapısı, Türkçe doğru, Supabase isteği 0, istisna 0. `/br/?lv=1&debug=1`: 40/40 + oturum 40/40 (37 kazandı, 3 bomba), L01'de sürükleme ile parça çıktı (20→19), debug satırı booster şeridiyle çakışmıyor, booster kilitleri 3./6./9./12. bölüm. `/gz/?lv=40&debug=1`: aynı kontrol, L40 engelleri ve booster stokları 5/3/7/10 doğru |

## M1 tanımı (seviye verisi ve kural motoru) — KAPANDI 2026-10-05

Sonuç: kabul 1–8 karşılandı (8: iPhone'da 2026-10-06). 227 EditMode testi; travelLimit 10.612 web altın değerinde birebir; açgözlü çözücü 40/40 web'le aynı sonuç ve sıra; LevelStats 40/40; README bölüm planı doğrulandı (çivi 5, zincir 11, bomba 21, mühür 31; tüm zincirler uzak). Geçici `BootLevelCheck` ve süre paneli M3'te kalkacak.

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

## M2 tanımı (GameSession: oyun oturumu) — KAPANDI 2026-10-06

Sonuç: 1a ve 2–9 karşılandı (9'un iPhone kısmı 2026-10-06, Zeyd). 322 EditMode testi; 40/40 seviyede her adım web'le birebir (37 kazandı, L24/L29/L38 bomba); akıllı oynayıcı 40/40 kazandı (0 patlama/donma/supap); değnek 175/175 uygulandı ve sonrası çözülebilir. Altın senaryo kaynağı `Tools/web_harness.js` + `Tools/make_session_golden.js`. Commit'ler: main a578d4b, d9a11ef; gh-pages 92bcba5.

Kapsam:
- Core'da saf C# `GameSession`: bir seviyenin tek denemesi. Zaman `Tick(dt)` ile, rastgelelik `IRandom` ile dışarıdan verilir; motor saati/`UnityEngine.Random` yok.
- Hamle sırası web'le aynı: çıkarma → çivi sayaçları azalır → emniyet supabı → bomba fitili azalır → kazanma kontrolü.
- Engeller: sayılı çivi, zincirli çift (tek gövde), bomba, mühürlü kenar (M1 kuralı üzerinden).
- Booster'lar: makas, değnek, çekiç, saat (+20 sn); stok ve kullanılamayacağı durumlar web'le aynı.
- Sonuçlar: kazandı / süre doldu / bomba patladı / bıraktı. Bittikten sonra hiçbir hamle ve booster kabul edilmez.
- Olaylar: Presentation'ın dinleyeceği olaylar (parça çıktı, sayaç değişti, supap, booster, süre, sonuç). Oturum durumu dışarıdan yalnızca komutlarla değişir.
- Deneme kaydı modeli (gönderim değil): `level_id`, `level_version`, `client`, deneme no, süre, sonuç, kalan süre, takılma sayısı, booster başına kullanım, yıldız. Tanımlar karar kaydında (2026-10-05 gece). Deneme sayısı ve yıldız oturuma dışarıdan verilir/oturumdan okunur; kalıcı saklama M5.
- Açılış kontrolünün genişletilmesi: web build'inde 40 seviyenin altın senaryosu (ölçüt 1a) GameSession ile oynatılır, süre panelinde sonuç yazar (stripping'in oturum kodunu bozmadığının dinamik kanıtı). M3'te panelle birlikte kalkar.
- Kapsam dışı: görsel, girdi, Supabase/PlayerPrefs (M3–M5), Monte Carlo (Faz 2), yeni seviye/süre tasarımı (Faz 4).

Kabul ölçütleri:
1. (2026-10-06'da ikiye ayrıldı; eski hali kayıtlı çözümün çivi/bomba farkında olmaması nedeniyle geçersiz, bkz. karar kaydı.)
   - **1a. Web'le birebir oturum (bağlayıcı).** 40 seviyenin her biri için, çivilere uyan basit oynayıcının (serbest + çivisiz parçalar arasından: bomba serbestse onu, değilse bombayı tıkayanı, yoksa ilkini) web kodunda (`Tools/web_harness.js`) ürettiği hamle listesi altın senaryodur. GameSession aynı listeyi **gerçek komut yolundan** oynar (teste özel çivi atlama yolu yok) ve web'le birebir aynı sonucu verir: her adımda sonuç/sayaçlar/supap/fitil ve son sonuç (L24, L29, L38'deki bomba patlaması dahil). 40/40 eşleşme şart.
   - **1b. README garantisi (rapor, engelleyici değil).** Core'da "akıllı oynayıcı": çivilere uyar, bombaya sınırlı aramayla en kısa yoldan gider. Hedef 40/40 kazandı, patlayan bomba 0, donma 0, supap 0. Tutmayan her seviye test hatası değil, rapor verisidir: seviye, neden (bomba/donma/supap), arama sınırı. Zaman kutusu: ~yarım gün; arama sınırına takılan seviye "doğrulanamadı" olarak raporlanır.
2. EditMode, işlem sırası: sınır durumları için senaryo testleri: aynı hamlede çivinin serbest kalması, bombanın 0'a inmesi, bombalı parçanın kendisinin çıkarılması, son parçanın çıkarıldığı hamlede fitilin 0'a inmesi. Beklenen sonuç web koduyla belirlenir: mümkünse M1'deki gibi web kodu Node'da çalıştırılarak altın senaryo üretilir; mümkün değilse her beklenti web kodunda ilgili satıra referansla yazılır ve bu yöntem rapora yazılır.
3. EditMode, emniyet supabı: tetiklendiği yapay bir tahtada en uzun bekleyen çiviyi söker; web'le aynı seçim.
4. EditMode, booster'lar: her biri için etki, stok düşümü ve reddedildiği durumlar. Değnek deterministik `IRandom` ile: aynı tohum aynı tahta; farklı tohumlar farklı tahta; web'in değiştirmediği kenarlara (düz/tahta kenarı vb.) dokunmaz. Saat süreye tam 20 sn ekler.
5. EditMode, sonuçlar: süre dolması (sınır değerinde dahil), bomba, kazanma, bırakma; her denemede tam olarak bir kayıt; bitişten sonra komutlar reddedilir ve olay üretmez.
6. EditMode, deneme kaydı: alanlar doğru doluyor (yukarıdaki tanımlarla); deneme no artıyor; bırakma kaydı üretiliyor; yıldız web formülüyle aynı.
7. EditMode, determinizm: aynı seviye + tohum + komut/Tick dizisi → birebir aynı olay listesi ve kayıt (tekrar oynatma ve Monte Carlo için).
8. Rapor (engelleyici değil): 40 seviyede değnek sonrası açgözlü çözücünün çözebildiği oran (ör. seviye başına birkaç tohum).
9. `CoreIsolationTests` ve mevcut testler geçer. Web build size-log'a yazılır (10 MB bütçesi); genişletilmiş açılış kontrolü web build'inde 1a altın senaryolarını oynar ve "40/40 web'le aynı" gösterir (2026-10-06 değişikliği; eski "40/40 kazandı" ifadesi kayıtlı çözüme dayanıyordu).

## M3 tanımı (tahta görseli + sürükleme) — KAPANDI 2026-10-07

Sonuç: 1, 1b (kod), 2, 3 editörde; 5 (Brotli +290 KB, eşik altında), 6 (canlıda 40/40 + oturum 40/40), 7 (gh-pages fdab901, kök → `/br/`) karşılandı. 4: iPhone'da sürüklerken 60 FPS (Zeyd). 8: Zeyd'in genel onayıyla kapandı; madde madde kontrol sabahki telefon oturumunda (engelleyici değil, karar 2026-10-07 gece).

Kapsam:
- Presentation'da seviyeyi GameSession'dan kuran tahta: parçalar, eklemler (açık/kapalı), çivi + sayaç, zincirli çift, bomba + fitil, mühürlü kenar. Görsel yön kararına uygun: düz renk, Unlit, ışık/post-processing yok; renk paleti, parça şekilleri ve engel ikonları web sürümüyle birebir (karar 2026-10-06).
- Dokunmatik sürükleme (Input System): parça baskın eksende parmağı izler, durma noktasında (M1 `travelLimit`) durur; çıkış eşiği geçilince `TryExit`, geçilmezse yerine döner. Eşikler ve yön kilidi web kodundan alınır; farklılaşırsa rapora yazılır.
- Takılma kuralı (karar 2026-10-06): çivili parçaya basma ve durma noktasının ötesine itme; sürükleme ve yön başına bir kez. Girdi → komut mantığı Unity'siz test edilebilir bir sınıfta.
- Parça parmağı gecikmesiz izler (sürüklemede yumuşatma yok). Efektler minimum: bırakınca kısa oturma animasyonu, bomba patlaması ve kazanma için basit geri bildirim; sonuç HUD'da yazılı. Diğer efektler M6 cilalama adımında.
- Dokunma alanı ≥ 44 pt (görsel küçükse dokunma alanı genişletilir); sürüklerken sayfa kaymaz/yakınlaşmaz.
- Geçici HUD: kalan süre, hamle; bitişte sonuç + "tekrar / sonraki". Seviye `?lv=N` ile seçilir (varsayılan 1). Booster'lar M3'te arayüzde yok (M4).
- Ekran: dikey, safe area içinde; ana ekran modundaki alt şerit bölgesine arayüz konmaz (2026-10-05 kararı). 40 seviyenin en büyük tahtası 375×667 (iPhone SE), 390×844 ve 430×932 noktada kırpılmadan sığar.
- Süre paneli + açılış kontrolü yalnızca `?debug=1` ile; debug'da FPS/kare süresi.
- Kapsam dışı: menüler, seviye haritası, booster arayüzü, yıldız ekranı (M4); Supabase/PlayerPrefs (M5); ses ve cila (M6).

Kabul ölçütleri:
1. Görsel: Presentation oyun durumunu yalnızca GameSession komut/olaylarıyla değiştirir/izler (kod incelemesi + mümkünse test). Editörde 375×667, 390×844 ve 430×932'de L01, L05, L11, L21, L31 ve en büyük tahtalı seviyenin ekran görüntüsü rapora eklenir; kırpılma yok, engellerin hepsi ayırt edilebilir (375×667 bağlayıcı). Rapora en büyük tahtada 375×667'deki en küçük parça boyutu ve dokunma alanı (pt) yazılır; dokunma alanı ≥ 44 pt.
1b. Girdi: sürüklemede parça aynı karede parmak konumunda (yumuşatma yok); sayfa `touch-action`/viewport ile kaydırma ve yakınlaştırmaya kapalı (kod incelemesi; iPhone'da ölçüt 8).
2. EditMode: girdi denetleyicisi (Unity'siz): baskın eksen seçimi, durma noktasında kenetlenme, çıkış eşiği → `TryExit`, eşik altı → geri dönüş; takılma sayımı kurala göre (aynı sürüklemede aynı yöne ikinci itme sayılmaz, yön değişirse sayılır, çivili parçaya basma sayılır).
3. EditMode/PlayMode: 1a altın senaryolarından en az 5 seviye (L24 dahil) girdi denetleyicisi üzerinden simüle sürüklemelerle oynatılır ve GameSession sonucu altınla aynı çıkar (girdi katmanının hamle kaybetmediğinin kanıtı).
4. Performans: iPhone'da `?debug=1` ile en büyük tahtalı seviyede sürükleme sırasında medyan FPS ≥ 55 (Zeyd okur). Altındaysa PM'e rapor, engelleyici kararı PM verir.
5. Boyut: size-log'a yazılır; toplam indirme 10 MB bütçesinde. Brotli build M2'ye göre +1 MB'ı geçerse (font, doku) PM'e raporlanır.
6. Açılış: debug kapalıyken oyun doğrudan tahtaya açılır; `?debug=1` ile panel 40/40 + oturum 40/40 yazar.
7. Yayın: gh-pages `/br/` ve `/gz/`, kök → `/br/` korunur.
8. iPhone (Zeyd, kök adres): L01–L05 ve L11, L21, L31 oynanır; her biri sonuçlanır (kazanma veya kayıp), yanlış yöne giden/kaybolan sürükleme yok, parça parmağın gerisinde kalmıyor, sürüklerken sayfa kaymıyor/yakınlaşmıyor, engeller okunuyor.

## M4 tanımı (menüler, booster arayüzü, HUD) — KAPANDI 2026-10-07

Sonuç: 1, 2, 3 editörde; 4 (size-log, yayın) karşılandı. 5: Zeyd'in genel onayıyla kapandı; madde madde kontrol sabahki telefon oturumunda (engelleyici değil, karar 2026-10-07 gece).

Kapsam: ana menü ve seviye listesi (40 seviye, kilit/yıldız gösterimi; kalıcılık M5'te, şimdilik oturum içi), booster arayüzü (makas, değnek, çekiç, saat; stok ve reddedilen durumlar GameSession'dan), bitiş ekranı (sonuç, yıldız, tekrar/sonraki/menü), HUD'un web görünümüne getirilmesi. Görsel yön web birebir; efektler minimum (M6'ya). `?lv=N` ve `?debug=1` korunur.

Kabul ölçütleri (2026-10-06 gece kesinleşti):
1. Presentation booster'ları yalnızca GameSession komutlarıyla kullanır; her booster için arayüz → komut → olay yolu EditMode testinde (Unity'siz denetleyici). — KARŞILANDI
2. Menüye dönüş ve yeniden başlatma "bıraktı" kaydı üretir (M2 tanımı); test. — KARŞILANDI
3. 375×667, 390×844, 430×932 ekran görüntüleri: menü, seviye listesi, booster'lı HUD, bitiş ekranı; kırpılma yok, dokunma alanları ≥ 44 pt, alt şerit bölgesinde arayüz yok. — KARŞILANDI (editör)
4. Mevcut testler geçer; size-log, yayın M3 ile birlikte (M3 ölçüt 5–7 ile aynı biçimde). — testler geçiyor; build bekliyor
5. iPhone (Zeyd, M3 ölçüt 8 ile aynı oturumda): kök adres menüye açılır; "Oyna" L01'i açar; L01 kazanılınca L02 açılır; bölüm listesinde kilitli bölüme dokunulamaz; L12'de dört booster da kullanılır (makas eklem noktasına dokunma, değnek onay kartı, çekiç, saat +20 sn); tanıtım kartı L03'te görünür, okunur, kapatılır; bitiş kartındaki Tekrar / Sonraki / Menü çalışır; hiçbir yazı/düğme kırpılmıyor, Türkçe karakterler doğru.

## M5-ön tanımı (kalıcılık, oyuncu adı, kapalı gönderim) — 2026-10-06 gece, Zeyd'in kararıyla

Durum (2026-10-07 gece): ölçüt 1–7 KABUL (365 test, HTTP 0, anahtar taraması temiz, SQL çalışmış ve anon okumayla doğrulanmış). Ölçüt 8'in "gönderim kapalı" kısmı 2026-10-07'de karşılandı; kalıcılık ve sayfa kapanışı kısmı gönderim açık build'de TEST_GECE akışında doğrulanır (karar 2026-10-07 gece), M5-ön o raporla kapanır.

Amaç: Zeyd SQL'i çalıştırıp build alınınca gönderimin tek bayrakla açılabilmesi. Canlı Supabase'e hiçbir istek atılmaz.

Kapsam:
- Kalıcılık (PlayerPrefs, Platform'da; Core'da depolama arayüzü): `level_id` bazında deneme sayacı, en iyi yıldız, kazanılmış/açık bölümler, oyuncu adı. Anahtar şeması sürümlü. Bölüm sırası değişse de veri `level_id`'ye bağlı kalır.
- Oyuncu adı ekranı (web'in name gate'i): ilk açılışta bir kez, web'deki kurallarla (uzunluk/boşluk); menüden değiştirilebilirse web'deki gibi.
- Kayıt kuyruğu: her deneme kaydı kalıcı kuyruğa yazılır (sayfa yeniden yüklenince kaybolmaz), her kayda istemcide benzersiz kimlik; üst sınır (ör. 200 kayıt, en eskisi düşer).
- Sayfa kapanışı = bıraktı (M2 tanımında M5'e bırakılan): `pagehide`/`visibilitychange` ile açık deneme bir kez "bıraktı" kaydı olarak kuyruğa yazılır (jslib).
- Gönderim: Supabase REST insert, yalnızca anon anahtar; HTTP arayüz arkasında. Yeniden deneme: ağ/5xx hatasında artan bekleme ile; kalıcı hata (4xx) kuyruğu tıkamaz, kayıt ayrılır ve sayılır. Gönderim bayrağı build'e gömülü, varsayılan KAPALI; kapalıyken HTTP nesnesi hiç çağrılmaz ve kuyruk gönderilmez. `?debug=1` oturumları ve editör asla gönderilmez. Debug panelinde: kuyrukta / gönderildi / reddedildi sayıları, bayrak durumu.
- SQL dosyası (`docs/sql/` altında): `plays` tablosuna `level_id` (boş bırakılabilir metin) ve gerekiyorsa kayıt kimliği sütunu + tekil kısıt; tekrar çalıştırılabilir (`if not exists`); web oyununu etkilemez; sonunda Zeyd için bir doğrulama sorgusu ve dosyanın başında adım adım çalıştırma talimatı.
- Kapsam dışı: bayrağı açmak, canlıya istek, Supabase'de herhangi bir değişiklik (Zeyd'in işi), öğretici el ve cila (M6).

Kabul ölçütleri:
1. EditMode (sahte depolama): deneme sayacı, en iyi yıldız (yalnızca artar), kilit ve oyuncu adı "yeniden açılış" sonrası korunur; bozuk/eksik/eski sürüm veri çökertmez, varsayılana döner; anahtarlar `level_id`'ye bağlı.
2. EditMode: deneme no kalıcı sayaçtan gelir; yeniden açılışta kaldığı yerden artar (web'deki deneme sayacı hatası tekrar etmez).
3. EditMode (sahte HTTP): başarı → kuyruktan düşer; ağ/5xx → kuyrukta kalır, yeniden denenir; 4xx → ayrılır, sonraki kayıtlar gönderilir; aynı kayıt iki kez gönderilse bile aynı benzersiz kimliği taşır; kuyruk sınırı çalışır.
4. EditMode: gönderilen JSON, `plays` tablosunun sütunlarıyla birebir (altın JSON testi): `client`='unity', `level_id`, `level_version`, deneme no, süre, sonuç, kalan süre, takılma, booster kullanımları, yıldız, oyuncu adı; web'in sütun adları ve tipleri.
5. EditMode: bayrak kapalıyken ve `?debug=1`'de hiçbir yoldan HTTP çağrısı yok (sahte HTTP çağrı sayısı 0).
6. Repoda yalnızca anon/publishable anahtar; `service_role` yok (push öncesi tarama).
7. SQL dosyası hazır ve mühendislikçe gözden geçirilmiş (sütun adları web'in `plays` şemasıyla tutarlı); Zeyd'e adım adım talimat.
8. Build'e bağlı (açık kalır, M3+M4 build'iyle birlikte): stripped build'de kalıcılık sayfa yenilemede korunur, sayfa kapanışı bıraktı kaydı kuyruğa düşer (debug panelinde görülür), gönderim kapalı (ağ sekmesinde Supabase isteği yok).

Bu işten sonra build alınmadan yeni kapsam açılmaz (M6 dahil); PM'e gelinir.

## Seviye editörü tanımı — 2026-10-07 gece, Zeyd'in kararıyla (sıradaki ana iş)

**Amaç:** Zeyd, koda ya da JSON'a dokunmadan, oyundaki görünümün aynısını görerek seviye kurar; çözülebilirliği anında görür, kaydeder, Unity'de ve telefonda oynar. Faz 3 deney seviyeleri ve Faz 4'teki 40 seviyenin yeniden tasarımı bu araçla yapılır.

**Nerede:** Unity'de, menüden (**Reverse Solver → Seviye Editörü**). Teknik yol mühendisliğin; tek şart: tahtadaki parça oyundakinin aynısı (şekil, girinti/çıkıntı, renk, engel ikonları) — en kolay yolu oyunun kendi BoardView/PieceView'ını kullanmak. Telefonda düzenleme kapsam dışı (backlog).

**Değişmeyen kurallar:**
- Web'den gelen 40 seviye (`levels.json`) ve `Golden/` referanstır; editör onları yalnızca "kopya olarak aç"ar, üzerine yazmaz.
- Tasarımlar ayrı set: `Assets/_Game/Levels/designs.json`, aynı format. Kalıcı `id` (D01, D02…; silinen id yeniden kullanılmaz), `version`.
- **`version` kuralı:** oynanışı etkileyen değişiklik +1 (boyut, kenarlar/şekil, zincir, çivi ve sayacı, bomba ve fitili, mühür, süre, booster stokları/açılışları); etkilemeyen aynı kalır (renk/palet, resim adı, tanıtım kartı metni, kayıtlı çözüm). Kaydedilmemiş ara değişiklikler tek kayıtta en fazla +1. Henüz hiç kaydedilmemiş yeni tasarım `version` 1 ile başlar.
- Kayıtlı çözüm çözücüden üretilir, elle girilmez.
- Tasarım kuralı: 0 patlama, 0 donma, 0 supapla çözülebilir. Editör kurala uymayanı kaydetmeyi engellemez ama açıkça uyarır (deney için bilerek bozuk tahta gerekebilir).
- Oyunda hangi setin varsayılan olacağı ayrı bir PM kararı (playtest öncesi); bu gece normal oyun değişmez.
- Debug oturumları hiçbir zaman gönderilmez; gönderim artık canlıda açık olduğu için tasarım oyunlarında bu ayrıca doğrulanır.

### Öncelik sırası

**Kademe 1 — bu gece zorunlu** (sabahki hedef: Zeyd tek başına bir seviye kurar, Unity'de oynar, telefonda açar):
- **E1. Core: düzenlenebilir seviye modeli + kurallar + testler.** Tahtayı yalnızca geçerli durumda tutan düzenleme işlemleri (kenar ayarla, şablon uygula, engel ekle/kaldır, zincir kur/ayır, boyut), `version` kuralı, designs.json okuma/yazma, id ataması, çözülebilirlik sorgusu (GameSession + SmartPlayer; sonuç: çözülebilir / çözülemez + neden + parça / doğrulanamadı-arama sınırı). Saf C#, Unity'siz. Ölçüt 2, 3, 4, 5 burada kapanır.
- **E2. Editör sahnesi ve yerleştirme.** Menüden açılır; yeni tahta (genişlik × yükseklik) ya da var olan seviyeyi kopya olarak açma; parça seçimi + yan panelde kenarlar/renk/engel; palet → tahta sürükle-bırak: parça şablonları, kenara tıklayınca kenar türü döner, çivi (sayaçlı), bomba (fitilli), mühürlü kenar, zincir; sayı alanı tıklanınca düzenlenir; silgi/sağ tık ile kaldırma; renk fırçası (seviyenin mevcut paletiyle); süre ayarı; geri al / yinele (≥ 50 adım); kaydetmeden çıkarken uyarı.
- **E3. Canlı kontrol paneli (temel).** Her değişiklikten sonra ≤ 1 sn: çözülebilir mi (+ neden ve takıldığı parça vurgulu), çözüm uzunluğu, engel sayıları (`LevelStats`), boyut, sığma uyarısı (375×667'de sığmıyor ya da hücre < 44 pt).
- **E4. Kaydet / Oyna.** designs.json'a kaydet (id, version kendiliğinden), "Oyna" tasarımı aynı oyun sahnesinde açar; oyundan editöre dönüş tasarımı kaybetmez.
- **E5. Web'de debug seti.** Tasarım seti web build'ine girer; yalnızca `?debug=1` ile listelenir ve doğrudan açılır (parametre adları mühendisliğin). Build + yayın + başsız Chrome doğrulaması.
- **E7. Kılavuz (kısa).** `docs/LEVEL_EDITOR.md`, Türkçe, ekran görüntülü; en başta "İlk seviyeni kur (10 dk)". Yalnızca çalışan özellikleri anlatır; kademe 2 eklendikçe güncellenir. Kademe 1 bitmeden de, o ana kadar çalışan akışla yazılır (sabah Zeyd elinde ne varsa onu kullanabilsin).

**Kademe 2 — vakit kalırsa, bu sırayla:**
1. **E6. CSV dışa aktarma** (web + tasarım): id, version, w, h, parça, zincir, çivi, bomba, mühür (duvar), süre, çözüm uzunluğu; `;` ayırıcı, ondalık virgül, UTF-8 BOM. Faz 4 zorluk tablosunun girdisi.
2. Çözümü tahtada adım adım oynatma (ileri/geri).
3. "Parçaları ayır" görünümü.
4. Seviye ayarlarının kalanı: booster stokları ve açılış bölümleri, tanıtım kartı (başlık, metin, ipucu), resim adı, palet düzenleme.

**Kademe 3 — sonraki oturumlar (Faz 2 ile birleşir):** panelde hızlı Monte Carlo (ör. 200 tohum, rastgele/akıllı oynayıcının kazanma oranı, ortalama geçerli hamle sayısı; CSV'ye de girer), "çözülebilir rastgele tahta üret".

**Gecenin kuralı:** her E adımı testler yeşil → commit → push ile biter. E5 build + yayınla biter. Kademe 1'in bir parçası 45 dk'dan fazla takılırsa daha basit çözüm seçilir (ör. sürükle-bırak yerine "araç seç + hücreye tıkla"); yerleştirme yöntemi Zeyd'in "yerleştirme usulü" isteğini karşıladığı sürece mühendisliğin kararı.

### Kabul ölçütleri

1. **Uçtan uca akış (mühendislik):** menüden editörü aç → 5×6 boş tahta → paletten parça şablonları + en az bir çivi, bomba, mühür ve zincir yerleştir → panel "çözülebilir" der → kaydet → "Oyna" ile aynı seviyeyi oyunda kazan. Koda ya da JSON'a dokunmadan; her adımın ekran görüntüsü `docs/screens/editor/`'de.
2. **Gidiş-dönüş (EditMode):** 40 web seviyesinin her biri editör modeline yüklenip değiştirilmeden yazılınca anlamca aynı veri çıkar (ayrıştırılmış model eşit; alan sırası/boşluk serbest). designs.json için de yaz → oku → aynı.
3. **Çözülebilirlik tutarlılığı (EditMode):** editörün sonucu aynı seviyede GameSession + SmartPlayer sonucuyla aynı: 40 web seviyesi + en az 3 tasarım; tasarımlardan en az biri bilerek çözülemez (olumsuz durum da sınanır). "Doğrulanamadı (arama sınırı)" "çözülemez"den ayrı raporlanır.
4. **Tutarsız tahta oluşamaz (EditMode):** her düzenleme işlemi için test + rastgele düzenleme dizisi testi (ör. 1000 işlem, sabit tohum): her adımdan sonra uyumsuz komşu kenar yok, zincir yalnızca komşu hücreler arasında, web kuralının izin vermediği engel birleşimi yok. Sığmayan tahta uyarı verir (test).
5. **`version` kuralı (EditMode):** listedeki her oynanış değişikliği +1, her görsel/metin değişikliği 0; kaydetmeden yapılan birden çok değişiklik tek kayıtta +1; silinen id yeniden kullanılmaz.
6. **Korunanlar:** `levels.json` ve `Golden/` değişmez (git diff boş); mevcut testlerin hepsi geçer; `CoreIsolationTests` geçer; editör kodu ve editör sahnesi web build'ine girmez (build raporunda yok); Brotli build artışı designs.json + debug-set yükleyicisi kadar (> 50 KB ise PM'e rapor). Normal oyun (debug'sız) 40 seviye, değişmeden.
7. **Web debug seti (başsız Chrome, canlı):** `?debug=1` ile tasarımlar listelenir ve bir tasarım oynanıp sonuçlanır; debug olmadan tasarım izi yok (liste 40, tasarım parametresi yok sayılır); tasarım oyununda Supabase isteği 0.
8. **Canlı panel hızı:** en büyük tahtada (40 seviyenin en büyüğü ve 1. ölçütteki tasarım) bir düzenlemeden sonra panel ≤ 1 sn'de güncellenir (ölçülüp rapora yazılır).
9. **Kılavuz:** `docs/LEVEL_EDITOR.md` var; "İlk seviyeni kur (10 dk)" turu 1. ölçütteki akışı ekran görüntüleriyle anlatır; anlatılan her özellik çalışıyor.
10. *(Kademe 2)* CSV: biçim testi (BOM, `;`, ondalık virgül, başlık satırı, 40 + tasarım satırı). Türkçe Excel'de açılış Zeyd'in kontrolü.
11. *(Zeyd, sabah)* Kılavuzdaki turu kendisi yapar ve bir tasarımı iPhone'da `?debug=1` adresinden oynar. Getirir: kaç dakika sürdü, nerede takıldı, eksik bulduğu şey.

Editör kademe 1 + ölçüt 1–9 ile kapanır; 11 Zeyd'in kabulüdür. Kademe 2–3 ayrı işler olarak açık işlerde kalır.

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
- [x] iPhone ölçümü (5G) değerlendirildi: varsayılan Brotli, tetikleyici (B) tetiklenmedi (2026-10-06)
- [x] M2: 322 test, 1a 40/40 web'le birebir, 1b 40/40 kazandı (main a578d4b, d9a11ef; gh-pages 92bcba5) — kapandı 2026-10-06
- [x] Varsayılan Brotli + kök → `/br/` (main bc053ab, gh-pages fb5ca44, canlıda doğrulandı) — kapandı 2026-10-06
- [x] Panel "yerel açtı mı" satırı düzeltildi (bc053ab) — kapandı 2026-10-06
- [x] C hazırlığı: workflow (`workflow_dispatch`, game-ci, imaj `unityci/editor:ubuntu-6000.6.4f1-webgl-3`), `WebBuild.BuildFromCommandLine`, `docs/CLOUD_BUILD.md` (55b861a). Doğrulanmadı: Zeyd'in secret'ları ve ilk çalıştırma gerekiyor; Unity'nin yeni lisans sistemi nedeniyle Personal lisans adımı belirsiz
- [x] M4 kodu (ee11e3d): 346 test, ölçüt 1–3 editörde karşılandı
- [x] M3+M4+M5-ön build (yerel, 2026-10-07): Brotli 7,15 MB, gzip 9,04 MB; gh-pages fdab901 `/br/` + `/gz/`, kök → `/br/`; canlıda 40/40 + oturum 40/40, Supabase 0. `Builds/publish`'ten yayın düzeltmesi (28117a8)
- [x] `BootLevelCheck` ve süre paneli `?debug=1` arkasında — canlıda doğrulandı 2026-10-07
- [x] M3 ve M4 kapandı (2026-10-07 gece; 60 FPS + Zeyd'in genel onayı)
- [x] M5-ön ölçüt 1–7 kabul; 002 SQL anon okumayla doğrulandı (2026-10-07 gece)
- [ ] 1. **(şu an) Gönderimi canlıya açma:** bayrak aç → Brotli + gzip build → size-log → gh-pages `/br/` + `/gz/` → TEST_GECE doğrulaması (geçme koşulları ve geri dönüş karar kaydında, 2026-10-07 gece) → rapor PM'e. M5-ön ölçüt 8 aynı raporla kapanır
- [ ] 2. **Seviye editörü kademe 1:** E1 → E2 → E3 → E4 → E5, E7 paralel/sonda (bkz. "Seviye editörü tanımı"). Her E adımı commit + push; bitince rapor PM'e (ölçüt 1–9 kanıtı)
- [ ] 3. Seviye editörü kademe 2 (vakit kalırsa, sırayla): E6 CSV, çözüm oynatma, parçaları ayır, seviye ayarlarının kalanı
- [ ] 4. `docs/OVERNIGHT.md` eski rehberindeki "SAC geri dönüşsüz" maddesinin altına düzeltme notu (KB5083769)
- [ ] 5. M6 (editör kademe 1'den sonra; sabah listesinde çıkan veri-kalitesi hataları önce): L01 öğretici eli (playtest öncesi zorunlu), ardından çıkış/çivi sökülmesi/supap efektleri ve ses. Her adım build + yayınla biter; boyut size-log'da
- [ ] 6. Pages yayın olaylarını kayda geçir (tarih, süre, düzeltme yolu); Netlify tetikleyicisi (A) bu kayıtla sayılır
- [ ] 7. Zeyd eski re-run'ı iptal edemeden o çalışırsa ve kök eski iki bağlantılı sayfaya dönerse: boş commit ile gh-pages'i yeniden yayınla

### Zeyd
- [x] GitHub Pages'i aç (gh-pages, / (root)) — açıldı; ilk 404 GitHub runner kuyruğundandı, çözüldü
- [ ] Actions'ta kuyrukta bekleyen eski re-run'ı (commit 0552040) iptal et
- [x] iPhone'da "40 seviye okundu, 40 çözülebilir" (M1 ölçüt 8) — karşılandı 2026-10-06
- [x] gzip/Brotli iPhone karşılaştırması — 5G'de yapıldı, karar verildi 2026-10-06
- [x] Ana ekran modunda alt şerit kontrolü — sorun yok, kapandı 2026-10-06
- [x] iPhone'da kök → `/br/` ve panelde "oturum ... web'le aynı" (M2 ölçüt 9) — karşılandı 2026-10-06
- [x] M3 görsel yön kararı (web birebir, okunabilirlik, minimum efekt) — verildi 2026-10-06
- [x] WebGL build engeli — Zeyd Akıllı Uygulama Denetimi'ni kapattı (KB5083769'dan beri tekrar açılabiliyor); yerel build geçti 2026-10-07. Bulut build secret'ları gerekmez (yedek)
- [x] iPhone'da genel oynanış + sürüklerken 60 FPS (M3 ölçüt 4) — 2026-10-07
- [x] 002 SQL çalıştırıldı (çıktı getirilmedi; mühendislik anon okumayla doğruladı) — 2026-10-07
- [ ] **(sabah, tek telefon oturumu ~20 dk)** (a) Gönderim teyidi: yeni özel sekmede kök adres, ad `TEST_ZEYD`, L01'i bir kez oyna; Supabase → Table Editor → `plays`'te en yeni satır: `client`=unity, `level_id`=L01, `level_version`=1, `row_id` dolu, deneme no 1, süre makul. (b) Kısa kontrol listesi (aynı sekme, debug'sız): L01 kazanınca L02 açılıyor; bölüm listesinde kilitli bölüme dokunulamıyor; L03 tanıtım kartı okunuyor/kapanıyor; bitiş kartında Tekrar/Sonraki/Menü; sayfayı yenile → ad ve ilerleme duruyor; sürüklerken sayfa kaymıyor/yakınlaşmıyor; yazı kırpılması yok. (c) `?debug=1` ile L12: dört booster (makas eklem noktası, değnek onay kartı, çekiç, saat +20). Getir: (a) satırın ekran görüntüsü, (b)(c) her maddeye geçti/kaldı, kalan için ekran görüntüsü
- [ ] **(sabah) Seviye editörü kabulü (ölçüt 11):** `docs/LEVEL_EDITOR.md`'deki "İlk seviyeni kur" turu; tasarımı iPhone'da `?debug=1` adresinden oyna. Getir: süre, takıldığın yer, eksik bulduğun şey
- [ ] Kendi test oyunlarında oyuncu adı olarak `TEST_ZEYD` kullan (analizden çıkarılır; karar 2026-10-07 gece)
- [ ] Desktop'taki saatlik gece görevini sabah duraklat/kapat
- [ ] Playtest öncesi 4G teyidi (M6 sonrası son build'le; M2'yi engellemez): Ayarlar → Hücresel → Hücresel Veri Seçenekleri → Ses ve Veri → LTE; Wi-Fi kapalı; kök adres, her açılış yeni özel sekme, 3 kez; panel değerleri (toplam, JS açma) + başta ve sonda fast.com
- [x] Güvenlik duvarında Python'un "Ortak" ağ izni kaldırıldı — 2026-10-06

## Backlog

- Netlify'a geçiş (yedek plan, karar 2026-10-06): yalnızca tetikleyici (A) veya (B) gerçekleşirse. Gerekirse: Zeyd Netlify hesabı + repo bağlantısı, `_headers` ile `Content-Encoding`, decompression fallback kapatma, aynı 4G protokolüyle ölçüm
- Faz 4 girdisi: M2 1b'de akıllı oynayıcı 40/40 kazandı, kazanılamayan seviye yok. Yeniden tasarım kuralı kalır: her seviye 0 patlama/0 donma/0 supapla çözülebilir
- Faz 2: akıllı oynayıcının kısıtlı aramasını (bomba konisi + 1 dolgu) genel çözücüye genişletmek (yeni seviyelerde gerekebilir)
- M6 cilalama (playtest öncesi): çıkış, çivi sökülmesi, supap partikül/tween'leri, ses, genel his. M3'te yalnızca minimum efekt (karar 2026-10-06)
- Yeni görsel kimlik (Zeyd'in kararı: ayrı iş): Faz 3 veri toplama bitene kadar yapılmaz; en erken Faz 4 yeniden tasarımıyla. Case study görselleri için de değerlendirilebilir
- ~~Unity içinde seviye editörü~~ → sıradaki işlere alındı (Zeyd'in kararı, 2026-10-07 gece); bkz. "Seviye editörü tanımı"
- Seviye editörü kademe 3: panelde hızlı Monte Carlo, "çözülebilir rastgele tahta üret" (Faz 2 ile)
- Telefonda seviye düzenleme (editör yalnızca Unity'de)
- Oyunda varsayılan set kararı (web 40 / tasarım seti / deney seti): playtest öncesi PM kararı
- Bulut build (GitHub Actions, 55b861a, doğrulanmadı): yedek; yalnızca yerel build yeniden engellenirse ya da makine değişirse. Zeyd'in secret adımı o zaman
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- Eski Input Manager'a geçiş (yalnızca 4G teyidi 10 sn'yi aşarsa yeniden değerlendirilir; iPhone 5G medyanı 1,83 sn)
- Ek boyut adımları (mscorlib/URP küçültme vb.): 4G teyidi hedefi tutarsa yapılmaz
- UI Toolkit'i çıkarmak için URP/uGUI fork'u: reddedildi, yalnızca kayıt için
- 3D fizik modülü (PhysX, URP/uGUI bağımlılığıyla geri geldi) çıkarılabilir mi: ölçülmedi; yalnızca 4G teyidi 10 sn'yi aşarsa
