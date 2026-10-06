# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-06 (M2 kapandı, M3 tanımı onaylandı; kök → `/br/` ve panel düzeltmesi kapandı)

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
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0, M1, M2 bitti (M2 2026-10-06; iPhone'da oturum kontrolü Zeyd'de, engelleyici değil). GitHub Pages yayında, kök → `/br/` (Brotli varsayılan). M3 (tahta görseli + sürükleme) başladı. Kalan: M3–M6, playtest öncesi kısa 4G (LTE) teyidi |
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

Sonuç: 1a, 2–8 karşılandı; 9 iPhone hariç karşılandı. 322 EditMode testi; 40/40 seviyede her adım web'le birebir (37 kazandı, L24/L29/L38 bomba); akıllı oynayıcı 40/40 kazandı (0 patlama/donma/supap); değnek 175/175 uygulandı ve sonrası çözülebilir. Altın senaryo kaynağı `Tools/web_harness.js` + `Tools/make_session_golden.js`. Commit'ler: main a578d4b, d9a11ef; gh-pages 92bcba5.

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

## M3 tanımı (tahta görseli + sürükleme)

Kapsam:
- Presentation'da seviyeyi GameSession'dan kuran tahta: parçalar, eklemler (açık/kapalı), çivi + sayaç, zincirli çift, bomba + fitil, mühürlü kenar. Görsel yön kararına uygun: düz renk, Unlit, ışık/post-processing yok; renk ve biçim web sürümünden.
- Dokunmatik sürükleme (Input System): parça baskın eksende parmağı izler, durma noktasında (M1 `travelLimit`) durur; çıkış eşiği geçilince `TryExit`, geçilmezse yerine döner. Eşikler ve yön kilidi web kodundan alınır; farklılaşırsa rapora yazılır.
- Takılma kuralı (karar 2026-10-06): çivili parçaya basma ve durma noktasının ötesine itme; sürükleme ve yön başına bir kez. Girdi → komut mantığı Unity'siz test edilebilir bir sınıfta.
- Çıkış, çivi sökülmesi, supap, bomba patlaması, kazanma/kaybetme için kısa tween/partikül geri bildirimi (sade; cila M6).
- Geçici HUD: kalan süre, hamle; bitişte sonuç + "tekrar / sonraki". Seviye `?lv=N` ile seçilir (varsayılan 1). Booster'lar M3'te arayüzde yok (M4).
- Ekran: dikey, safe area içinde; ana ekran modundaki alt şerit bölgesine arayüz konmaz (2026-10-05 kararı). 40 seviyenin en büyük tahtası 390×844 ve 430×932 noktada kırpılmadan sığar.
- Süre paneli + açılış kontrolü yalnızca `?debug=1` ile; debug'da FPS/kare süresi.
- Kapsam dışı: menüler, seviye haritası, booster arayüzü, yıldız ekranı (M4); Supabase/PlayerPrefs (M5); ses ve cila (M6).

Kabul ölçütleri:
1. Görsel: Presentation oyun durumunu yalnızca GameSession komut/olaylarıyla değiştirir/izler (kod incelemesi + mümkünse test). Editörde 390×844 ve 430×932'de L01, L05, L11, L21, L31 ve en büyük tahtalı seviyenin ekran görüntüsü rapora eklenir; kırpılma yok, engellerin hepsi ayırt edilebilir.
2. EditMode: girdi denetleyicisi (Unity'siz): baskın eksen seçimi, durma noktasında kenetlenme, çıkış eşiği → `TryExit`, eşik altı → geri dönüş; takılma sayımı kurala göre (aynı sürüklemede aynı yöne ikinci itme sayılmaz, yön değişirse sayılır, çivili parçaya basma sayılır).
3. EditMode/PlayMode: 1a altın senaryolarından en az 5 seviye (L24 dahil) girdi denetleyicisi üzerinden simüle sürüklemelerle oynatılır ve GameSession sonucu altınla aynı çıkar (girdi katmanının hamle kaybetmediğinin kanıtı).
4. Performans: iPhone'da `?debug=1` ile en büyük tahtalı seviyede sürükleme sırasında medyan FPS ≥ 55 (Zeyd okur). Altındaysa PM'e rapor, engelleyici kararı PM verir.
5. Boyut: size-log'a yazılır; toplam indirme 10 MB bütçesinde. Brotli build M2'ye göre +1 MB'ı geçerse (font, doku) PM'e raporlanır.
6. Açılış: debug kapalıyken oyun doğrudan tahtaya açılır; `?debug=1` ile panel 40/40 + oturum 40/40 yazar.
7. Yayın: gh-pages `/br/` ve `/gz/`, kök → `/br/` korunur.
8. iPhone (Zeyd, kök adres): L01–L05 ve L11, L21, L31 oynanır; her biri sonuçlanır (kazanma veya kayıp), yanlış yöne giden/kaybolan sürükleme yok, sayfa kaydırma/yakınlaştırma oyunu bozmuyor, engeller okunuyor.

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
- [ ] 1. M3 (bkz. "M3 tanımı", 8 kabul ölçütü), başladı. Bitince ekran görüntüleri, size-log farkı ve test sayılarıyla PM'e raporla
- [ ] 3. M3'te: `BootLevelCheck` ve süre panelini `?debug=1` arkasına al (silme; karar 2026-10-06)
- [ ] 5. Pages yayın olaylarını kayda geçir (tarih, süre, düzeltme yolu); Netlify tetikleyicisi (A) bu kayıtla sayılır
- [ ] 7. M5 başında: `plays` tablosuna `level_id` sütunu için SQL hazırla, Zeyd'e ver (karar 2026-10-06)
- [ ] 4. Zeyd eski re-run'ı iptal edemeden o çalışırsa ve kök eski iki bağlantılı sayfaya dönerse: boş commit ile gh-pages'i yeniden yayınla

### Zeyd
- [x] GitHub Pages'i aç (gh-pages, / (root)) — açıldı; ilk 404 GitHub runner kuyruğundandı, çözüldü
- [ ] Actions'ta kuyrukta bekleyen eski re-run'ı (commit 0552040) iptal et
- [x] iPhone'da "40 seviye okundu, 40 çözülebilir" (M1 ölçüt 8) — karşılandı 2026-10-06
- [x] gzip/Brotli iPhone karşılaştırması — 5G'de yapıldı, karar verildi 2026-10-06
- [x] Ana ekran modunda alt şerit kontrolü — sorun yok, kapandı 2026-10-06
- [ ] Şimdi (M3'ü engellemez): iPhone'da kök adresi (`https://zeydusht.github.io/reverse-solver-unity/`) yeni özel sekmede bir kez aç; adres çubuğunda `/br/`'ye gittiğini ve panelde "40 seviye okundu, 40 çözülebilir; oturum 40/40 web'le aynı (37 kazandı, 3 bomba)" yazdığını kontrol et (M2 ölçüt 9'un iPhone kısmı). Getir: evet/hayır + panel satırı (ekran görüntüsü yeterli)
- [ ] M3 bitince: iPhone'da M3 ölçüt 4 (FPS, `?debug=1`) ve ölçüt 8 (L01–L05, L11, L21, L31 oynanışı)
- [ ] Playtest öncesi 4G teyidi (M2'yi engellemez): Ayarlar → Hücresel → Hücresel Veri Seçenekleri → Ses ve Veri → LTE; Wi-Fi kapalı; kök adres, her açılış yeni özel sekme, 3 kez; panel değerleri (toplam, JS açma) + başta ve sonda fast.com
- [ ] Yerel sunucu artık gerekmiyor: güvenlik duvarında Python'un "Ortak" ağ iznini kaldır

## Backlog

- Netlify'a geçiş (yedek plan, karar 2026-10-06): yalnızca tetikleyici (A) veya (B) gerçekleşirse. Gerekirse: Zeyd Netlify hesabı + repo bağlantısı, `_headers` ile `Content-Encoding`, decompression fallback kapatma, aynı 4G protokolüyle ölçüm
- Faz 4 girdisi: M2 1b'de akıllı oynayıcı 40/40 kazandı, kazanılamayan seviye yok. Yeniden tasarım kuralı kalır: her seviye 0 patlama/0 donma/0 supapla çözülebilir
- Faz 2: akıllı oynayıcının kısıtlı aramasını (bomba konisi + 1 dolgu) genel çözücüye genişletmek (yeni seviyelerde gerekebilir)
- Unity içinde seviye editörü
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- Eski Input Manager'a geçiş (yalnızca 4G teyidi 10 sn'yi aşarsa yeniden değerlendirilir; iPhone 5G medyanı 1,83 sn)
- Ek boyut adımları (mscorlib/URP küçültme vb.): 4G teyidi hedefi tutarsa yapılmaz
- UI Toolkit'i çıkarmak için URP/uGUI fork'u: reddedildi, yalnızca kayıt için
- 3D fizik modülü (PhysX, URP/uGUI bağımlılığıyla geri geldi) çıkarılabilir mi: ölçülmedi; yalnızca 4G teyidi 10 sn'yi aşarsa
