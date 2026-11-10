# PM_MESAJ_58 — Kenar boşluğu + fiske ile çıkarma (p21)

Kaynak: `docs/PT1_GERIBILDIRIM.md` (D ve E). Oyuncuların en büyük şikâyeti: kenar parçaları telefon kılıfına ve tarayıcı kenarına denk geliyor, parçayı çıkarmak için çok uzun sürüklemek gerekiyor. Yalnız p21. Ağır işleri `codex exec` ile Codex CLI'ya ver; sen böl, kontrol et, test et, yayınla.

## 1. Fiske (flick) ile hamle
- Parçaya dokunup **kısa bir kaydırma** yeterli olsun: parmak bir yönde ≥ **0,3 hücre** (ya da ≥ 18 pt, hangisi küçükse) gittiğinde ve hız eşiği aşıldığında (ör. ≥ 600 pt/sn) ya da parmak kalktığında **hamle o yönde işlenir**: parça kendiliğinden kayar, yol açıksa tahtadan çıkar, değilse takılır (jam). Kural ve sayım (hamle, jam, kilit/bomba sayaçları) değişmez.
- Yön: baskın eksene kilitlen (4 yön). Eksenler arası fark küçükse (≤ %20) hamle yapma, parçayı geri bırak.
- **Ölü bölge:** < 8 pt hareket = dokunma sayılır, hamle yok (yanlış tetiklemeyi önle).
- Eski uzun sürükleme de çalışmaya devam etsin (sürüklerken parça parmağı izler, bırakınca aynı kural).
- Parçanın kendiliğinden kayışı akıcı ve tatmin edici: hızlanarak kayar, çıkarken hafif uzama ve "puf" (efektler sonra gelecek, şimdilik basit).
- Zincir fiziği sürüklemede olduğu gibi fiskede de tepki versin.
- Telemetri: her hamlede `input = flick|drag` (moves tablosuna yeni sütun EKLEME; mevcut bir serbest metin/json alanı varsa oraya, yoksa raporla, PM karar verir).

## 2. Kenar boşluğu
- iOS Safari'de **soldan/sağdan kaydırma = geri/ileri sayfa** hareketi oyunu bozabilir. Oyun alanında tarayıcı kenar hareketlerini engelle (`touch-action: none`, `overscroll-behavior: none`, gerekiyorsa pointer olaylarında `preventDefault`). iPhone'da test edilemiyorsa raporla.
- Tahtanın yatay **güvenli payı**: ekran kenarından tahtadaki ilk hücreye en az **24 pt** (şu an ~18–20). Gerekirse hücre biraz küçülür; tahta yine en büyük olacak şekilde ortalanır. Üst/alt şeritler değişmez.
- Kenar hücredeki parça: dokunma alanı hücrenin **içine doğru** biraz genişlesin (kenara yakın parmak hatası için); dışa taşma yok.

## 3. Test
- EditMode: fiske eşikleri (ölü bölge, yön kilidi, eşik altı/üstü) için birim testleri.
- Vitrin + normal set: fiske ile 10 hamle, istisna 0. `verify_build` 4/4.
- Kısa GIF: fiske ile parça çıkışı, kenar parçası, yanlış yön (takılma). `docs/art/task29/pm58/`.

## Teslim
p21 güncel, rapor `docs/art/task29/ENTEGRASYON_RAPOR.md`'ye "PM_58" eki, PROJE_DURUMU'na tek satır, commit + push.
