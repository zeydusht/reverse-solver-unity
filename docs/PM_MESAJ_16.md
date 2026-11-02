# PM → Claude Code (26 Ekim 2026, 17:00) — Önizleme 12 geri bildirimi

Zeyd oynadı: "Oyun güzel bir konumda, akıyor." Bomba, çivi, durdur tuşu ve booster çubuğu onaylandı.

Kurallar:
- Soru sorma.
- Token tasarrufu zorunlu.
- Her adımda commit at.

Ekran görüntüleri: `docs/qa/feedback-2026-10-26/` (01-menu, 02-start-card, 03-pause, 04-drag-lock-bug, 05-ingame).

Bu iş iki kısımdan oluşuyor:
- **A kısmı yalnız kod.** Şimdi yap.
- **B kısmı Codex'in görsellerini bekliyor.** Codex `Assets/Art/Gameplay/Task15/` klasörüne yazacak. Görseller gelince bağla.

## A — Şimdi yap (kod)

### 1. HATA: Sürüklerken kilit kayboluyor (04-drag-lock-bug)

- Zincirli parçalardan biri sürüklenirken uç kilidi görünmez oluyor. Kilit, parçayla birlikte hareket etmeli. Zincir de iki uç arasında her karede yeniden çizilmeli.
- Aynı görüntüde, sürüklenen parçanın yanında kırmızı bir parça artığı görünüyor ("7" benzeri şekil). Muhtemelen duvar ya da maske artığı; bul ve düzelt.
- Bu davranışı test et:
  - zincirli parça sürüklenirken
  - bırakıldıktan sonra
  - geçersiz hamlede geri dönerken

### 2. Ana menü (01-menu)

- **"Jiggles rescued" yazısını ve 581/1000 çubuğunu tamamen kaldır.**
- **Yıldız sayacını Jiggles sayacına çevir.** Sağ üstteki yıldız + 41 kutusunda yıldız yerine Jiggle ikonu olsun (geçici olarak mevcut Jiggle avatarının küçük hali). Sayı = toplam kurtarılan Jiggles (581). Yıldız sayısı menüde görünmesin; veri olarak kayıtta kalsın.
- **Profil kutusu:** "Jiggles: 581" satırını kaldır. Oyuncu adı (ddd) kutunun hem yatayda hem dikeyde tam ortasında dursun.
- **Avatar seçimi:**
  - Avatara dokununca küçük bir kart açılsın. Kartta 5 Jiggle avatarı ızgara halinde dizilsin.
  - Seçilen avatar işaretlensin ve cihazda saklansın (localStorage). Supabase'e dokunma.
  - Codex'in avatarları gelene kadar mevcut avatarın renk tint'li 5 kopyasını kullan.

### 3. Oyun içi küçük düzeltmeler (05-ingame)

- **LV rozeti:** "LV" ve sayı (40) rozetin içinde tam ortalı değil, ortala. 1, 2 ve 3 haneli sayıları test et.
- **Mevcut kırmızılar geçicidir:** booster adet rozeti ve duvar. B kısmında yeni kırmızı tonla değişecek. Şimdilik dokunma.

### 4. Yayın

Önizleme 13'ü `?set=redesign` adresine yayınla. BACKLOG'a 2 satır yaz.

## B — Codex'in Task15'i gelince

`docs/art/HANDOFF.md`'deki Task15 satırlarına bak. Gelecek parçalar:

1. **Yeni kırmızı skala.** `PALETTE.md` güncellenecek. Bu ton tüm kırmızılara uygulanacak: booster adet rozeti, duvar şeridi, gerekirse kaybetme kartı vurguları.
2. **Duvar v3:** daha parlak ve kalın kırmızı-beyaz şerit (repeat + köşe). Koddan çizilen şeridi kaldırıp görsel tile'ı kullan.
   - Kalınlık hücrenin ~%20'si.
   - Kenar boyunca kesintisiz uzanmalı, köşede düzgün birleşmeli.
3. **Yeni HUD hapları:** daha uzun (dikeyde dolgun) haplar. Yeni ikonlar: yapboz parçası ve saat. Hapı ve ikonu yeni görselle değiştir; sayılar aynı fontla kalsın.
4. **Yeni kart tuşları:** daha kısa ve daha uzun boylu tuşlar. Tuş yazısı okunur olmalı (koyu zemin + krem yazı ya da altın zemin + lacivert yazı).
   - Hangi kartlarda değişecek: başlangıç kartındaki "Başla", pause kartındaki Devam / Yeniden başla / Ana menü, kazanma ve kaybetme kartı tuşları.
   - Tuş genişliği kartın en fazla ~%65'i olsun ve ortalansın.
5. **5 Jiggle avatarı:** mavi, pembe, kırmızı, yeşil, mor. Avatar seçim kartına ve sağ üstteki Jiggles sayacına bağla.
6. **Resimli yapboz testi:** `painting-test-01.png` (kare). Bu resmi bir bölümün parça yüzeyi olarak kullan: her parça resmin kendi hücresine düşen kısmını göstersin (Kit81 maskesi aynı kalsın).
   - Yalnız `?set=redesign&painting=1` ile açılsın. Normal set düz mavi kalsın.
   - Önce Level 7'de dene.

B bitince Önizleme 14'ü yayınla. Ayrıca `&painting=1` linkini OVERNIGHT.md'nin başına yaz.

Build 5'i yayınlama.
