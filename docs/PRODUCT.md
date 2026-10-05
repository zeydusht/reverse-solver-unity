# Reverse Solver (Unity) — Ürün Dokümanı

Ürün kararlarının tek kaynağı bu dosya. Kararlar, ölçümler ve açık işler burada tutulur; `product-manager` subagent'ı her değerlendirmeden sonra günceller.

Son güncelleme: 2026-10-05

## Hedef

Reverse Solver'ı Unity'de iPhone'da (Safari, dikey) oynanır hale getirmek, oyuncu verisiyle doğrulanmış bir zorluk modeli kurmak ve 40 seviyeyi bu modele göre baştan tasarlamak. Sonuç: iş başvurularında kullanılacak, veriye dayalı seviye tasarımını anlatan bir case study.

## Başarı ölçütleri

| Ölçüt | Hedef |
|---|---|
| İlk açılış süresi (iPhone, 4G, önbellek boş) | ≤ 10 sn |
| İlk açılışta toplam indirme (ara hedef) | ≤ 6 MB |
| Seviye doğrulama | 40 seviyenin tamamı otomatik testte çözülebilir |
| Playtest | ≥ 30 oyuncu, her deney seviyesinde ≥ 20 tamamlanmış oyun |
| Zorluk modeli | Modelde kullanılmayan seviyelerde tahmin edilen zorluk ile gerçek deneme sayısı arasında korelasyon ≥ 0,7 |

## Fazlar

| Faz | İçerik | Durum |
|---|---|---|
| 1. Taşıma | M0–M6: oyun Unity'de çalışır, iPhone'da oynanır | M0 bitti; boyut çalışması sürüyor |
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

## Ölçümler

| Tarih | Build | Toplam indirme | Toplam süre | Ayrıntı |
|---|---|---|---|---|
| 2026-10-05 | M0, boş sahne, gzip | 11,2 MB | 9,07 sn (aynı Wi-Fi, önbellek boş, Safari) | İndirme 8,13 sn · JS açma 0,35 sn · wasm 0,22 sn · motor 0,28 sn |

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
- [ ] Boyut çalışması: kullanılmayan paketler, Managed Stripping High, IL2CPP Optimize Size, URP özellikleri ve shader varyantları, gzip/Brotli karşılaştırması (her adımın etkisi tabloyla)
- [ ] Ana ekran alt şeridi: arka plan rengini kamera rengine eşitle
- [ ] Yedek: commit + push (gizli anahtar kontrolüyle)
- [ ] M1: seviye JSON'u, kural motoru, 40 seviyenin çözüm testleri

### Zeyd
- [ ] Yeni build'i iPhone'da önbelleği temizleyip ölç (Safari → site verisini sil), süre panelini ve ana ekran şeridini kontrol et
- [ ] GitHub'da push'un geldiğini doğrula
- [ ] Testler bitince yerel sunucuyu kapattır, güvenlik duvarında Python'un "Ortak" ağ iznini kaldır

## Backlog

- Unity içinde seviye editörü
- Supabase telemetrisinin Unity'ye taşınması (Faz 2)
- 4G üzerinde gerçek ilk açılış ölçümü (GitHub Pages yayınından sonra)
