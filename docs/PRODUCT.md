# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-05 (gece)

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
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0 bitti; boyut çalışmasının mühendislik kısmı kapandı (8,64 MB gzip). M1 başladı. 4G ölçümü (Zeyd) gelince sıkıştırma kararı kesinleşir |
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

## M1 tanımı (seviye verisi ve kural motoru)

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
- [ ] 1. M1 (bkz. "M1 tanımı", 8 kabul ölçütü). Bitince M1 build'ini gh-pages `/gz/`'ye koy (süre paneli + "40 seviye okundu"), size-log'a yaz, PM'e raporla
- [ ] 2. 4G ölçümü gelince karar kuralına göre sıkıştırmayı kesinleştir (Brotli seçilirse main'de varsayılanı değiştir)

### Zeyd
- [ ] GitHub Pages'i aç (Settings → Pages → Deploy from a branch → gh-pages, / (root))
- [ ] Pages açılınca iPhone'da 4G ile (Wi-Fi kapalı, 5G kapalı) iki adresi ölç: her biri için önbelleği temizle, 3 kez aç, süre paneli değerlerini + bir hız testi sonucunu getir
- [ ] Ana ekran modunda (Paylaş → Ana Ekrana Ekle) alt şeritte farklı renk kalmadığını kontrol et, ekran görüntüsü getir
- [ ] Yerel sunucu artık gerekmiyor: güvenlik duvarında Python'un "Ortak" ağ iznini kaldır

## Backlog

- Unity içinde seviye editörü
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- Eski Input Manager'a geçiş (yalnızca 4G ölçümü 10 sn'yi aşarsa yeniden değerlendirilir)
- Ek boyut adımları (mscorlib/URP küçültme vb.): 4G ölçümü hedefi tutarsa yapılmaz
- UI Toolkit'i çıkarmak için URP/uGUI fork'u: reddedildi, yalnızca kayıt için
