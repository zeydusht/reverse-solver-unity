# Codex Task21 — Menü revizyonu (Zeyd oynadı): alt çubuk, profil, avatar halkaları

Zeyd p20'yi telefonda oynadı. Genel olarak beğendi, ama şu parçalar yeniden çizilecek. Royal Match referansı ve kopyalama yasağı geçerli: `Assets/Art/References/`, özellikle `...18.34.52 (8).jpeg` ve `...18.34.52.jpeg` ana ekranlar. Klasör: `Assets/Art/Menu/Task21/`. Yalnız art, kod yok. Manifest/HANDOFF satırları Task20 formatında.

**ÖNCE OKU: `docs/art/STIL_REHBERI_RM.md`.** Zeyd'in yeni yakın çekimleri: `Assets/Art/References/1.11 ref/`. Alt çubuk için `IMG_2437` ve `IMG_2441`, üst bar için `IMG_2440` ve `IMG_2444`. Aşağıdaki maddeler rehberle çelişirse **rehber geçerli**. Özellikle alt çubuk düz renkli olacak, kabartma yok. Seçili yuva 2 kat geniş olacak.

## 1. Alt sekme çubuğu v3 (en önemlisi)
Royal Match'teki gibi (rehber bölüm 1):
- **Çubuk ekranın en altına oturur.** Zemini alt kenara kadar uzansın; iPhone ev çubuğunun altında da çubuk rengi görünsün.
  - Çubuk zemini dikeyde 9-slice, alt kısmı uzatılabilir.
  - İkonlar ev çubuğunun üstünde kalır.
- **Yazı yok.** Seçili olmayan sekmeler yalnız ikon. İkonlar çubuğun **içinde**, yuvalara tam ortalı, büyük ve net (yuva yüksekliğinin ~%70'i).
- **Seçili sekme:**
  - İkon büyür ve çubuktan biraz yukarı taşar.
  - Altında, çubuğun içinde tek etiket (Royal Match'teki "Home" gibi). Kalın beyaz-krem yazı, koyu kontur. Etiket metni oyun basacak, sen yalnız yerini ve boşluğunu tasarla.
- **Renk (düz, degrade/kabartma yok):**
  - Zemin `#10707F`, seçili yuva `#1A9AAE`. Seçili yuva **iki kat geniş**, sekme değişince genişlik kayar.
  - Yuvalar arası 2–3 px `#0B4A55` ayırıcı.
  - Üstte tek ince altın şerit (`#F38A03`→`#FBBA0E`).
  - İkonların etrafında çerçeve ya da daire yok.
- **İkonlar:** 4 ikon, mevcut set (Ev, Koleksiyon, Jiggles, Kupa). Daha büyük ve daha net silüet, kalın koyu dış kontur.
  - Kupa ikonu Royal Match'teki gibi parlak ve okunur.
- **Çıktı:**
  - `tabbar-bg` (9-slice)
  - `tab-slot`, `tab-slot-selected`
  - 4 ikon × (normal, seçili-büyük)
  - `tab-badge` (kırmızı rozet)

## 2. Sol üst profil (avatar)
Bizimki küçük ve köşeye sıkışmış. Royal Match'teki gibi:
- **Kare, büyük, altın-turkuaz çerçeveli avatar kutusu.** 64 pt, @3x 192 px. Avatar tam ortada, çerçeveye değmeden.
- Avatarın sağında ayrı bir **isim plaketi**. 9-slice, isim uzunluğuna göre genişler. Koyu zemin, krem yazı yeri.
- Jiggle sayaç hapı aynı ailede: ikon solda taşan, sayı alanı geniş.
- **Çıktı:** `profile-square-frame`, `profile-name-plate` (9-slice), `counter-pill-v2` (9-slice).

## 3. "Avatarım" seçim halkaları (Jiggles ekranı)
Şu an avatarlar halkadan taşıyor ve çok büyük.
- Yuvarlak halka. Avatar **halkanın içinde, iç çapın ~%78'i**. Taşma yok, maske ile kırpılmış.
- Normal halka: turkuaz. Seçili halka: kalın altın + sağ altta küçük yeşil tik rozeti.
- **Çıktı:** `avatar-ring`, `avatar-ring-selected`, `avatar-ring-mask` (daire maske), `check-badge`. 5 avatarı yeniden kırpıp hazırla: `avatar-<renk>-round.png`, halka içine oturur boyutta.

## 4. Yatay uzun çubukları kaldır
Zeyd uzun yatay barları sevmiyor.
- **Jiggles kilometre taşları (500/1.000/2.000):** Yatay çubuk yerine 3 ayrı madalyon, aralarında noktalı kısa yol. Ulaşılan madalyon parlak, ulaşılmayan soluk + kilit.
  - Çıktı: `milestone-medal-{frame,badge,portrait}` (normal/kilitli), `milestone-dots`.
- **Ayarlar:** "Gizlilik" ve "Bize yaz" uzun yatay tuşlar yerine **yan yana iki küçük kare-yuvarlak tuş**, ikonlu (kalkan ve zarf), altında küçük yazı.
  - Çıktı: `settings-icon-btn` (normal/basılı), `icon-privacy`, `icon-mail`.
  - Ayar kartı satırları: her satır krem bir karo, ikon solda koyu turkuaz daire. Kart daha az mavi, daha çok krem olsun.

## Teslim
- Tüm PNG'ler + manifest + HANDOFF.
- `onizleme.png`: Ev, Jiggles ve Ayarlar ekranları bu parçalarla yeniden kurulmuş. Ev'de alt çubuk ekranın en altında, üst bar en üstte.
- Bitince 2 satır yaz.
