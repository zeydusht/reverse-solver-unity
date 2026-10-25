# Codex görevleri (Claude Code yazar, Codex yapar)

Kurallar: her görevde **push/deploy yok**, Unity çalıştırmak gerekmez. Yalnızca görevde adı geçen dosyalara ve `docs/art/`, `Assets/Art/` altına dokun. Bitince görevin başlığına **Bitti** yaz ve 1–2 satır not bırak; Claude Code git diff ile kontrol edip birleştirir.

## 1. İngilizce metin taslağı — Bitti
- **Amaç:** Oyuncuya görünen yeni metinlerin İngilizce karşılığı.
- **Dosyalar:** okunur: `Assets/_Game/Presentation/Ui/Texts.cs`, `Assets/_Game/Levels/redesign.json` (`intro`, `bintro` alanları). Yazılır: yalnızca yeni `docs/i18n/en.md`.
- **Kabul:** Texts.cs'deki her sabit ve redesign.json'daki her intro/bintro için `Türkçe | English` satırı içeren tablo; karakter adları `docs/HIKAYE.md`'deki gibi (Jiggles, Baron); kod dosyaları değişmez.

Not: 5 UI sabiti ve 8 bölümdeki intro/bintro başlık, açıklama ve ipuçları `docs/i18n/en.md` tablosuna eklendi. Yalnızca dokümantasyon değişti; Görev 2–3 onay bekliyor.

## 2. Tutorial eli PNG export'u (P0 #4 onaylanınca)
- **Amaç:** Onaylı el çiziminden oyun export'u.
- **Dosyalar:** `Assets/Art/Tutorial/Exports/hand-tap.png`, `hand-swipe.png`.
- **Kabul:** 288×288 px, şeffaf arka plan, parmak ucu görselin sol üst çeyreğinde ve `docs/art/HANDOFF.md`'de pivot noktası (px) yazılı; her dosya ≤ 60 KB.

## 3. Engel tanıtım kart görselleri export'u (P0 #5 onaylanınca)
- **Dosyalar:** `Assets/Art/Intro/Exports/{nail,bomb,chain,wall,hard}.png`.
- **Kabul:** 888×592 px, PNG, her biri ≤ 250 KB; HANDOFF.md'ye satır eklenmiş.

## 4. Zeyd'in kararı (25 Ekim): oyundaki HER görsel baştan Codex'in olacak — PM yazdı

Claude Code UI envanterini `docs/art/ASSET_IHTIYAC.md`'ye #26 ve sonrası olarak ekleyecek; tam ölçüler orada olacak. O liste gelene kadar aşağıdakilere hemen başlayabilirsin. Stil kartına ve @3x kuralına uy.

- **4a. Oyna tuşu (yeni).** Dream Games / Royal Match menüsündeki gibi büyük, tombul, dikeyde yüksek, yatayda kısa bir tuş. Yaklaşık 200×72 pt (600×216 px @3x), 9-slice. Yazı ayrı bir katman olacak ("Level X"). Normal ve basılı olmak üzere 2 durum.
- **4b. Tutorial eli DİKEY.** Parmak yukarıyı göstersin, bilek aşağıda dursun. Dokun ve kaydır için 2 kare, her biri 288×288 px, şeffaf. Parmak ucunun piksel koordinatını HANDOFF.md'ye yaz.
- **4c. Yapboz parçası skin kiti.** Parça şekilleri prosedürel olduğu için sabit PNG çizme. Format Claude Code'dan gelecek; o gelene kadar 3×3'lük örnek bir parça setinde görsel yönü çiz:
  - gövde dolgusu
  - belirgin kenar konturu
  - iç bevel, highlight ve gölge
  - seçili, sürüklenen ve çivili (kilitli) hali

  Üstüne tablo resmi gelecek; kenarlar resmin üstünde de okunmalı. 2 yön çiz.
- **4d. Üst bar (HUD).** Level, hamle ve süre panelleri. Süre azalınca "acil" hali. Sayılar dinamik metin olarak kalacak.
- **4e. Booster yuvası ve kutusu.** Makas ikonunun etrafındaki kutu dahil. Durumlar: boş, dolu, kilitli, sayı rozeti.
- **4f. Oyun ekranı arka planı.** Bölüm oynanırken arkada duran sahne: Baron'un şatosu, sakin ve okunabilirliği bozmayan. 1125×2436 px, portre.
- **4g. Tahtada zincir ve duvar.** Şu an kodla çiziliyor; oyun içi sprite olarak yeniden çiz. Zincir iki parça arasında uzayabilmeli (tileable halka), duvar şerit boyunca tekrarlanabilmeli.
- **4h. Genel UI kiti.**
  - birincil, ikincil ve kapat butonları
  - kart ve popup zemini
  - çivi ve bomba sayaç rozetleri
  - yıldız
  - ilerleme çubuğu
  - toast/uyarı balonu

**Teslim:**
- A/B yapma; her kalem için tek, iyi bir seçenek çiz. Yalnızca 4c'de 2 yön çiz.
- Bitenleri ASSET_IHTIYAC.md'de ✅ olarak işaretle ve HANDOFF.md'ye satır ekle.
- Yalnızca `docs/art/` ve `Assets/Art/` içinde çalış.
- Token tasarruflu çalış.

## 4. Yapboz parçası skin'i (format; Görev 5'in parçası) — Bitti
Not: `Assets/Art/PieceSkin/` içinde skin.json, 256 px tekrarlanabilir gövde ve üç tablo × dört durumlu 4×4 inceleme teslim edildi.
40/56/76 pt tarayıcı kontrolleri ve teknik sözleşme `docs/art/reports/PIECE_SKIN.md` içinde; oyun entegrasyonu yapılmadı.
Parça şekilleri her bölümde koddan üretilir (tırnaklar farklı), bu yüzden parça PNG'si çizilmez. Bunun yerine bir **skin** verilir; oyun bunu her parçanın şekline uygular.
- **Dosyalar:** `Assets/Art/PieceSkin/skin.json` + `body-256.png`.
- **`body-256.png`:** 256×256 px, kenardan kenara tekrarlanabilir gövde dokusu; düz dolgu isteniyorsa tek renk PNG. Üstüne bölümün tablo resmi %~75 opaklıkla biner, doku hafif olmalı (kontrast ≤ %15).
- **`skin.json` alanları:** `outline` (renk, kalınlık pt; 1,5–3), `bevel` (iç kenar genişliği pt, highlight rengi + opaklık, shadow rengi + opaklık), `dropShadow` (renk, y-ofset pt, blur pt), `selected` (outline rengi, glow rengi + yarıçap pt), `dragging` (ölçek 1,0–1,1, gölge y-ofset pt), `nailed` (doygunluk 0–1, karartma 0–1, outline rengi).
- **Kural:** parça kenarları her tablo resminin üstünde **belirgin** olmalı; aynı skin 40–76 pt hücrede okunmalı.
- **Kabul:** `review.html`'de üç örnek tablo resmi üzerinde 4×4 tahta mockup'ı; 4 hal (normal, seçili, sürüklenen, çivili) yan yana.

## 5. #26–49'un hepsini çiz (+ #20–25 parçaları, dikey el) — Bitti
Not: 89 yazısız PNG, dosya bazında HANDOFF ve 9-slice manifesti teslim edildi; #49 skin korundu.
İnceleme: `Assets/Art/UI/Task5/review.html`; ölçü/alpha/tile kontrolleri temiz, Unity entegrasyonu ve push/deploy yapılmadı.
- **Amaç:** Oyunda kodla çizilen görsel kalmasın. Liste, ölçüler ve 9-slice bilgisi: `docs/art/ASSET_IHTIYAC.md` #26–49 (#49 = Görev 4).
- **Format:** her öğe @3x PNG (ölçü tablodaki px), şeffaf arka plan; 9-slice olanlarda HANDOFF'a sınır px yaz; çok halli öğeler ayrı dosya (`-normal`, `-locked`, `-out`, `-hot`…). Sayılar ve yazılar çizilmez (oyun yazar).
- **#20–25:** SVG ekranlardaki parçaları ayrı PNG ver (kart zemini 1020 px 9-slice, altın birincil düğme, ikincil düğme, bölüm hücresi 4 hal, ilerleme çubuğu iz+dolgu, isim giriş kutusu).
- **Tutorial eli:** dikey export (parmak yukarı, bilek aşağı), 288×288, parmak ucu üst ortada; pivot px HANDOFF'a.
- **Dosyalar:** `Assets/Art/UI/…`, `Assets/Art/Gameplay/…` altında; HANDOFF.md'ye her dosya bir satır.
- **Kabul:** #26–49 her satırın Durum'u "🎨 hazır"; tek seçenek (A/B yok, PM kararı); push/deploy yok.

### 5b. Kalan Claude çizimleri + playtest öncesi yeni UI (PM_MESAJ_9) — Bitti
Not: Profil/avatar, ayarlar, başlık, hedef, konfeti/ışıma ve kalan ikon/düğme durumları aynı PNG setinde.
Türkçe destekli Nunito TTF için normal/kalın ağırlık ve açık lisans önerisi HANDOFF.md içinde.
Hepsi @3x PNG, şeffaf, tek seçenek; yazı ve sayılar çizilmez. Liste: `ASSET_IHTIYAC.md` "Kalan Claude çizimleri" + aşağıdakiler.
| Ne | Nerede | Şekil | İçinde | Ölçü @3x | Durumlar |
|---|---|---|---|---|---|
| Profil kutusu | Ana menü sol üst (güvenli alanın altında) | 9-slice yatay kutu (köşe 36 px) | solda kare avatar yuvası; sağda iki satırlık yazı alanı (isim, "Kurtarılan Jiggles: X") | 480×156 px | normal / basılı |
| Avatar | Profil kutusunun içi | kare, köşesi yuvarlak | turkuaz Jiggle yüzü, gülümser | 132×132 px | normal |
| Ayarlar dişlisi | Ana menü ve oyun sağ üst | yuvarlak düğme | altın dişli | 138×138 px | normal / basılı |
| Ayarlar paneli satırı + anahtar | Ayarlar kartı | 9-slice satır (köşe 30 px) + aç/kapa anahtarı | solda ikon yeri, sağda anahtar | satır 888×150; anahtar 156×90 | açık / kapalı |
| Ses ikonu | Ayarlar satırı | kare | hoparlör | 90×90 | açık / kapalı (çizgili) |
| Bölüm başlangıç kartı başlığı | Kart üstü | 9-slice şerit | "Level X" yazılacak kurdele/şerit | 888×180 (köşe 60) | normal / zor (altın) |
| Hedef satırı | Başlangıç kartı | kare | küçük Jiggle silueti (hedef ikonu) | 120×120 | normal |
| Konfeti | Kazanma kartı | 6 küçük parça | turuncu, turkuaz, krem şeritler/kareler | her biri 36×36 | normal |
| Yıldız patlaması | Kazanma kartı | kare | yıldız dolarken çıkan ışık halkası | 252×252 | 1 kare |
| Kilit, üst bar ikonları, 1–10 sayı düğmesi, isim kutusu, telefonu çevir ikonu | bkz. "Kalan Claude çizimleri" tablosu | tabloda | tabloda | tabloda | tabloda |
| Yazı tipi önerisi | Her yer | — | Türkçe karakterli, açık lisanslı, Codex stiline uyan bir TTF (yalnızca dosya adı + lisans, çizim yok) | — | normal / kalın |

## 6. Son eksikler (PM_MESAJ_10) — Bitti
Not: 4 yazısız PNG, Görev 5 biçiminde manifest ve review.html teslim edildi; ölçü/alpha/boyut kontrolleri temiz.
HANDOFF güncellendi; yalnız izinli sanat klasörlerinde çalışıldı, push/deploy yapılmadı.
Stil: Görev 5 ile aynı (Assets/Art/UI/Task5, Gameplay/Task5). Yazı/sayı çizilmez. Dosyalar `Assets/Art/Gameplay/Task6/`; `manifest.json` Görev 5'teki alanlarla (name, width, height, path, slice).
| Ne | Nerede | Şekil | İçinde | Ölçü @3x | Durumlar |
|---|---|---|---|---|---|
| booster-active.png | Alt bar, seçili booster | kare (booster-normal ile aynı çerçeve) | normal yuvanın parlayan, altın kenarlı hali | 180×180 | aktif |
| joint-target.png, joint-target-pulse.png | Makas seçiliyken kesilebilecek dikiş noktası (tahtada) | yuvarlak | turuncu/altın hedef noktası, ortası dolu | 66×66 | normal / nabız (biraz büyük, daha parlak) |
| loader-bg.png | WebGL yükleme ekranı arka planı | tam ekran | koridor ya da oyun arka planının sade, bulanık, koyu hali (ikon ve çubuk üstüne gelecek) | 750×1334, ≤ 250 KB | — |
