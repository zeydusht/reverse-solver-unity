# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-05

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
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0 bitti; boyut çalışması kapanışta (Brotli + Pages 4G ölçümü + UI Toolkit zaman kutusu), ardından M1 |
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
- [ ] 1. EditMode testlerini yeniden çalıştır; boyut çalışmasını commit + push et (28b2c54 dahil, anahtar kontrolüyle)
- [ ] 2. Brotli build: boyut + bilgisayarda JS açma süresi (gzip ile aynı yöntemle)
- [ ] 3. GitHub Pages yayını: gzip (F) ve Brotli build'leri iki ayrı adreste (ör. `/gz/`, `/br/`), süre paneli açık
- [ ] 4. UI Toolkit (UIElementsModule) neden giriyor; çıkarılabiliyorsa çıkar ve ölç. Zaman kutusu en fazla yarım gün; sonuç ne olursa olsun raporla ve bırak
- [ ] 5. M1: seviye JSON'u, kural motoru, 40 seviyenin çözüm testleri; JSON okumanın High stripping'le web build'de çalıştığı doğrulanır

### Zeyd
- [ ] GitHub'da yeni push'un (boyut çalışması) geldiğini doğrula
- [ ] Pages yayınından sonra iPhone'da 4G ile (Wi-Fi kapalı) iki adresi ölç: her biri için önbelleği temizle, 3 kez aç, süre paneli değerlerini getir
- [ ] Ana ekran modunda (Paylaş → Ana Ekrana Ekle) alt şeritte farklı renk kalmadığını kontrol et, ekran görüntüsü getir
- [ ] Yerel sunucu artık gerekmiyor: güvenlik duvarında Python'un "Ortak" ağ iznini kaldır

## Backlog

- Unity içinde seviye editörü
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- Eski Input Manager'a geçiş (yalnızca 4G ölçümü 10 sn'yi aşarsa yeniden değerlendirilir)
- Ek boyut adımları (mscorlib/URP küçültme vb.): 4G ölçümü hedefi tutarsa yapılmaz
