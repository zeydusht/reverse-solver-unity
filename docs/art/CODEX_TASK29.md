# CODEX_TASK29 — Oyun içi HUD şeritleri + Jigo (kahraman) halleri + Devam ve Kazanma ekranları

Zeyd, 9 Kasım. Referans fikir: Royal Match'teki gibi tahtayı üstte ve altta şeritle çerçevelemek (odak artar). **Birebir kopya YOK:** kendi paletimiz (`docs/art/PALETTE.md`), kendi stilimiz, kendi karakterimiz. Kurallar TASK28 ile aynı: oyuncak malzeme, 64 px'te okunurluk, metin ve rakamlar ayrı katman (yazıyı PNG'ye gömme).

Kahramanın adı artık **Jigo**. Görünüş referansı: `docs/art/task26/kahraman-final.png` (turkuaz yapboz gövde, gümüş yüksük miğfer, mavi uçlu fırça, düğme kalkan). Videodaki görünüşle **birebir aynı** olmalı.

## 1. Üst şerit (HUD)
- Ekranın en üstünde tam genişlik **turkuaz şerit** (`#1C7F8C` / gölge `#0F4F5A`). Düz mavi sıkıcı kaçmasın: alt kenarında **altın bordür** (`#FBBA0E`/`#F38A03`/`#CE5D01`) + hemen üstünde **ince krem çizgi** (`#F8EBD0`). Hafif kumaş ya da ahşap doku olabilir.
- **Ortada madalyon:** Jigo'nun portresinin oturacağı yuvarlak, altın çerçeveli pencere (içi açık turkuaz ya da krem).
- **Sol:** kalan parça sayısı hapı. **Sağ:** süre hapı. Pause küçük yuvarlak tuş (sağ üst köşe). Haplar bugünkünden **~%20 daha küçük**. Hap içi krem, rakam ayrı katman.
- Seviye no (LV) küçük rozet olarak sol kenara.
- Şerit yüksekliği: 390×844 ekranda ~110–130 px (madalyon şeritten aşağı biraz taşabilir).

## 2. Alt şerit (booster)
- Aynı turkuaz + altın bordür (bordür üst kenarda). Boosterlar bu şeridin üstüne oturur.
- Şerit yüksekliği ~120 px. Güvenli alan (iPhone home çubuğu) için alt boşluk düşün.
- **Çekiç:** pembe çekiç kalkıyor, **mavi çekiç** geri geliyor (`docs/art/task28/_arsiv/Z2-v3-mavi-cekic/`). Yeni çizim gerekmez; yalnız yeni şeritle birlikte önizlemede göster.

## 3. Jigo portre halleri (madalyon için, büst: baş + omuz + fırça ucu)
Her hal **4–6 karelik dikişsiz döngü**, şeffaf PNG, 256×256 kaynak, 96 px'te okunur:
1. **normal:** kameraya bakar, arada göz kırpar, hafif nefes.
2. **tedirgin** (süre ≤10 sn): kaşlar kalkık, gözler sağa sola, ter damlası, fırçayı sıkar.
3. **korku** (süre ≤3 sn ya da bomba 1'de): gözler kocaman, yüksük titrer, kalkanı yüzüne siper eder.
4. **üzgün** (kaybedince): yüksük gözüne düşer, dudak titrer.
5. **mutlu** (bir parça çıkarınca kısa tepki, 3 kare): gülümser, fırçayı kaldırır.

## 4. Kazanma (sevinç) ekranı
- Büyük Jigo **tam boy** sevinç: zıplar, fırçayı havaya kaldırır, kalkanı sallar. 6–8 kare.
- Arkada konfeti ve ışık huzmesi parçaları ayrı katmanlar (Claude Code oynatır).

## 5. Devam teklifi paneli (kaybedince)
- Krem kart, altın çerçeve. Ortada büyük saat ikonu ve **"+30"** (rakam ayrı katman). Altında altın para ikonu ve fiyat alanı (rakam ayrı katman: 900 / 1900 / 2900).
- Yanda küçük **üzgün Jigo** (3. bölümdeki kareleri kullan).
- Tuşlar: yeşil "Devam" (onaylı yeşil), turkuaz "Vazgeç". Metin yok, yalnız tuş gövdesi.
- Altın para ikonu yoksa çiz: 64 px'te okunur, palet altını.

## Teslim
- Dosyalar: `Assets/Art/_yeni/HUD29/` (şeritler, madalyon, haplar), `Assets/Art/_yeni/Jigo/` (hal klasörleri: `normal/`, `tedirgin/`, `korku/`, `uzgun/`, `mutlu/`, `sevinc/`), `Assets/Art/_yeni/Devam/`.
- Önizleme paftası: `docs/art/task29/onizleme.png`. 390×844 oyun ekranı mock'u: üst şerit + tahta (vitrin tahtası gibi) + alt şerit + mavi çekiç; yanında Jigo'nun 5 hali ve devam paneli.
- Üretim notu: `docs/art/task29/uretim.md` (prompt, kaynak, boyut listesi).
- 3 satır özet yaz ve **dur**. Zeyd onaylamadan Claude Code entegre etmez.
