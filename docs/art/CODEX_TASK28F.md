# CODEX_TASK28F — Grup 3 düzeltmeleri (Zeyd, 9 Kasım)

Grup 3'ü Zeyd inceledi. Kurallar `CODEX_TASK28E.md` ile aynı (oyuncak malzeme, kendi rengi, 64 px okunurluk).

## Bomba ✅ onaylandı + animasyon kareleri
Siyah bomba onaylı (playtest'te bomba ile çivi karışıyordu, siyah çözüyor). Ek kareler:
- **Fitil kıvılcımı döngüsü:** 6–8 kare, şeffaf PNG, yalnız fitil ucu ve kıvılcım ayrı katman (gövde sabit). Döngü dikişsiz.
- **Acil hâl (3-2-1):** aynı bomba, gövdede kırmızımsı sıcak parıltı + daha büyük kıvılcım. Ayrı PNG.
- Şişme/titreme hareketini **Claude Code kodla yapacak**, sen sadece kareleri ver. Sayaç etiketi ayrı katman kalır.

## Çivi ❌ → ASMA KİLİT (yeni engel görünümü)
Çivi kaldırıldı. Yerine **asma kilit**: parçanın üstünde tombul, parlak bir asma kilit, gövdesinde **büyük numara alanı** (krem etiket, rakam ayrı katman).
- Renk: diğer engellerden ayrı. Öneri **parlak gümüş/çelik gövde + pirinç kulp**. 64 px'te kilit olduğu anlaşılsın.
- Kareler: kapalı, **açılma** (kulp yukarı fırlar, 3–4 kare), açık kilidin düşüşü (2–3 kare). Sıfıra inince açılıp düşer.

## Zincir 🔄 yeni tasarım
Kilit simgesi artık asma kilitte olduğu için zincirde kilit yok.
- Zincirli her parçanın **tam ortasında yuvarlak bir bağlantı noktası** (vantuz/perçin gibi, tombul, parlak). Zincir iki parçanın bağlantı noktaları arasında gerilir.
- **5 renk çifti:** altın, turkuaz, mor, mercan (`PALETTE.md` Task17) + **yeşil** (`#70C942`/`#368730`). Bağlantı noktasının üstünde renkten bağımsız işaret: yıldız, damla, hilal, kalp, **yaprak**.
- **A/B:** **A dümdüz gergin**, **B hafif sarkık** (yumuşak kavis). İkisini kısa (komşu parça) ve uzun (3 hücre) mesafede göster, yanında diğer parçalar olsun ki sarkmanın üste binip binmediği görülsün.
- Zincir halkaları tekrar eden tek bir halka sprite'ı olsun (uzunluğa göre çoğaltılsın, Claude Code yerleştirir).
- Zincir kopma anı: 3–4 karelik kopma/kaybolma.

## Duvar — DOKUNMA
Eski duvar aynen kalır. Yeni çizimi arşive.

Teslim: `task28/grup3-v2.png` (bomba kareleri, kilit kareleri, zincir A/B × kısa/uzun, 64/96/264 px), dosyalar `Assets/Art/_yeni/Obstacles/`. 3 satır özet, dur.
