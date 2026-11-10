# CODEX_TASK28B — Düzeltme: yeni stil YOK, mevcut stille yeniden çiz

Zeyd'in kararı (9 Kasım): A/B stil örneklerinin **hiçbiri** kabul edilmedi. **Sıfırdan stil önerme, yeni stil arama.** İstek şu: oyunda zaten var olan görselleri (booster, HUD, engel…) **Task26'da çizdiğin görsellerle aynı stilde, aynı kalitede** yeniden çiz. Yani:

- **Stil kaynağı = Task26 çıktıların:** `docs/art/task26/aksesuar.png`, `kahraman-final.png`, `mekan-atolye.png`, `cekim-kareleri-v06/`. Yumuşak 3D animasyon filmi görünümü, sıcak ışık, kadife/oyuncak yüzeyler, altın-turkuaz-krem aile.
- **İçerik kaynağı = mevcut görsel:** her öğenin şekli, rengi, simgesi, işlevi ve dosya boyutu **aynı kalır**. Makas yine makas, sihirli değnek yine değnek, bomba yine aynı bomba. Değişen sadece işçilik: daha temiz hacim, ışık, malzeme ve kontur, bu yüzden "bizim dünyamızdan" görünecek.
- `PALETTE.md` aynen geçerli. Yeni renk, yeni şekil dili, yeni çerçeve tipi yok.
- Denetim raporun (`task28/DENETIM.md`) **sıra ve sorun listesi** olarak kullanılır. A/B örnekleri iptal, `task28/_iptal/` altına taşı.

## Sıra (her grup sonunda dur, Zeyd baksın)
1. **Booster çubuğu** (4 ikon + halka + adet rozeti)
2. **HUD** (süre, kalan parça/hedef rozetleri, duraklat düğmesi)
3. **Engeller:** bomba (sayaç ayrı katman), çivi (raporundaki "sayaç diski gibi görünüyor" sorununu çöz: küçük boyutta çivi olduğu anlaşılsın), zincir/kilit, duvar

## Her grup için teslim
- `docs/art/task28/grupN-once-sonra.png`: **solda mevcut, sağda yeniden çizilmiş**, gerçek telefon boyutunda (64/96 px) ve büyük hâli. Başka seçenek yok, tek öneri.
- Dosyalar mevcut adlarla, ayrı klasörde: `Assets/Art/_yeni/...` (Zeyd onaylamadan oyundaki dosyaların üstüne yazma).
- 3 satır özet, sonra dur.

Başla: Grup 1 (booster çubuğu).
