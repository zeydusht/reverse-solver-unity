# PM → Claude Code (26 Ekim 2026, 15:00) — Yeni görsel set: menü (Task13) + oyun içi (Task14)

Zeyd iki seti de onayladı. Bu işten sonra oyunun görünümü playtest için son hali alacak.

Kurallar:
- Soru sorma.
- Token tasarrufu zorunlu.
- Her adımda commit at.

## Kaynaklar

- **Menü:** `Assets/Art/Menu/Task13/` (önizleme: `preview-menu.png`)
- **Level tuşu:** `Assets/Art/UI/PlayButton/` (v2 dosyaları)
- **Oyun içi:** `Assets/Art/Gameplay/Task14/` (önizleme: `preview-ingame.png`, 64 px detaylar: `preview-details.png`)
- **Font önerisi:** `docs/art/TASK14_FONT.md`
- **Ölçüler ve 9-slice değerleri:** `docs/art/HANDOFF.md`, Codex'in Task13 ve Task14 satırları
- **Renkler:** `docs/art/PALETTE.md`

## Yap

### 1. Ana menü (`preview-menu.png`'ye birebir)

- Sol üstte profil kutusu:
  - Jiggle avatar
  - oyuncu adı
  - "Jiggles: X" (X = toplam kurtarılan parça)
- Yıldız sayacı: toplam kazanılan yıldız.
- Ayarlar tuşu.
- Ortada koridor ve tablo.
- "Jiggles rescued X / 500" ilerleme çubuğu. Hedefler 100 / 500 / 1000; bir hedefe ulaşılınca çubuk bir sonraki hedefe geçer.
- Büyük "Level X" tuşu.
- Yanında "Levels" tuşu; bölüm listesini açar.
- **Alt gezinme çubuğu (Home / Gallery / Jiggles / Awards) GİZLİ kalsın.** O ekranlar playtest'ten sonra yapılacak.

### 2. Oyun içi (`preview-ingame.png`'ye birebir)

- **Üst bar:** level rozeti, hamle, süre (süre azalınca "acil" hali), durdur tuşu.
- **Booster çubuğu:**
  - yeni tuş zemini ve 4 ikon
  - sağ altta adet rozeti
  - kilitli hali
  - adet 0 iken görünen hali
- **Tahta:**
  - Yeni çerçeve.
  - Yeni bomba, çivi ve zincir (halka + uç kilidi).
  - Yeni kırmızı-beyaz duvar şeridi.
  - Bomba pivotu gövdenin merkezinde olsun.
  - Sayaçlar görseldeki sayı alanının ortasına gelsin, iki haneli sayılar da sığsın.
  - Çivi, bomba ve zincir hücre içinde 64 px'te de okunmalı. `preview-details.png` ile karşılaştır.
- **Kartlar:** pause (Devam / Yeniden başla / Ana menü), kazanma (3 yıldız), kaybetme (bomba ve süre sebebi), bölüm başlangıcı. Tuşlar Level tuşu ailesinden.
- **Yapboz parçaları (Kit81) ve oyun arka planı aynen kalsın.**

### 3. Font

`TASK14_FONT.md`'deki öneriyi uygula. Fontun lisansı OFL olmalı ve **Türkçe karakterleri** içermeli: ğ, ş, İ, ı, ç, ö, ü. Bu harfler yoksa:
- O font yalnız İngilizce etiketlerde kullanılsın ("Level X" için LilitaOne).
- Türkçe metinler Fredoka ile kalsın.

### 4. Eski dosyalar

Task8–12 arasındaki eski UI görsellerini kullanma. Yeni setle değiştir. Kodda geçici tint ya da ofset kalmasın.

### 5. QA

- 3 telefon boyutunda ekran görüntüsü al: 375×667, 390×844, 430×932.
- Kontrol edilecek ekranlar: menü, oyun, pause, kazanma, kaybetme.
- Taşma ya da üst üste binme varsa düzelt.

### 6. Yayın ve kayıt

- Önizleme 12'yi `?set=redesign` adresine yayınla.
- HANDOFF'a oyun içindeki adlarla satır ekle.
- BACKLOG'a 2 satır yaz.
- OVERNIGHT.md'nin başına link ve 3 satırlık özet koy.

Build 5'i yayınlama; o adım Zeyd'in onayından sonra.
