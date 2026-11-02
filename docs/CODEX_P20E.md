# Codex görevi P20-E — FTUE kararları (PM_MESAJ_29 bölüm 3)

Kaynak: `docs/PM_MESAJ_29.md` bölüm 3 (oku; tablo birebir uygulanır). Rapor: `docs/FTUE_PERF_RAPOR.md` bölüm 1–3.

## Dokunma
`Assets/_Game/Levels/*`, `Core/` (kural/telemetri), `Platform/`, `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`, `MenuTabs.cs` sekme ikon ölçüleri, `Screens.cs` Ev ekranı `Task13` yerleşimi (yeni değişti). Telemetri akışı yeni değişti: `GameRoot.Rate(...)`, `_rated`, `_telemetry.Finished` yerinde kalsın, 1–10 cevabı `Rate(n, null)` ile gider. Bölüm süreleri (tablo #7) değişmez. Commit atma. `Library/ Logs/ Builds/` okuma. Geçici dosyalar `%TEMP%`'e.

## İşler (tablo numaraları)
1. **Sonuç kartı** (kazanma + kayıp; `Screens` sonuç kartı / `GameRoot.ShowResult`): sıra yukarıdan aşağı: büyük ana tuş ("Sonraki bölüm"; kayıpta "Tekrar dene") → 1–10 satırı → en altta küçük sade metin bağlantısı "Geç" (`hard_skip`, buton zemini yok, ≥ 44 pt dokunma kutusu). Diğer tuşlar (Menü vb.) mevcut gibi kalır, ana tuşun altında.
2. **1–10:** 2 satır × 5, her kutu ≥ 44 × 44 pt (375 genişlikte sığmalı). Dokununca kutu dolu renge geçer, **0,3 sn sonra** `hard_N` işlenir; bu sürede başka kutuya dokunulursa seçim ona geçer ve süre baştan başlar (son seçilen kaydedilir, tek kayıt).
3. **Çivi metni:** kart (`Texts` tanıtım kartı) ve spotlight (`Texts.TutorialCopy("nail")`) aynı cümle: TR "Çivili parçanın sayacı her hamlende 1 azalır. 0 olunca parça serbest kalır." EN "A nailed piece's counter drops by 1 with every move. At 0, the piece is free." Önce Core'da çivi sayacının neyle azaldığını kontrol et (her çıkış mı, takılma da mı). "Her hamle = her çıkış" ise bu metin aynen; değilse koda uygun yaz ve raporda söyle. `docs/i18n/en.md` aynı.
4. **Bölüm başı kartı** ("Level X · Jiggle'ları kurtar!"): yalnız şu slotlarda çıkar: tanıtım/eğitim 1, 3, 6, 10, 14; booster 5, 9, 15, 23; zor bölüm uyarısı 19, 23, 25, 30, 35, 36, 38. Diğerlerinde bölüm doğrudan başlar (kart yok, süre hemen değil, mevcut "kart kapanınca başlar" kuralının kart yokken karşılığı: ilk kare). Başlık her yerde `Texts.Level` ("Bölüm X" / "Level X"); karttaki İngilizce sabit "Level" kalmasın. Zor bölüm kartının gövdesi yoksa kısa metin ekle: TR "Zor bölüm! Acele etme, önce bak." EN "Tough level! Look before you pull." (Texts + en.md). Slot listesi tek yerde sabit dizi olsun.
5. **Kural kartı çizimi** (`TutorialSpotlight` kural kartı): kırmızı çarpı büyük ve net (en az tik kadar), oklar koyu ve opak (α 1).
6. **Eğitim oku** (`TutorialSpotlight`): α ≥ 0,9, daha kalın (şerit ~1,5×), elin ÖNÜNDE (sorting order elden büyük), ok başı kısa okta da net (uç boyu sabit en az ~18 pt, gövde kısa olabilir).
8. **Yazı boyları:** sonuç kartı istatistik/soru satırları, tanıtım kartı gövdesi, kart gövdeleri ≥ 13 pt. Başarılar koşul satırı (`AchievementsScreen`): sığmazsa 2 satıra kırılsın, 12 pt altına küçülmesin. (Ev'deki "Bölümler" etiketi zaten 12 yapıldı.)
9. İsim ekranı: TR "Playtest sürümü" → "Deneme sürümü" (EN aynı kalabilir). en.md güncelle.
10. **Kayıp kartında yıldızlar gizli** (boş yıldız çizilmez; yerleşim boşluğu kapansın).
11. **Bölümler haritası** (`LevelsMap` / harita ekranı): şu anki düğümün üstünde küçük yeşil "Oyna" balonu (`Texts` "Oyna"/"Play"); balona ya da düğüme dokununca o bölüm başlar (bugün nasıl başlıyorsa). Balon dokunma kutusu ≥ 44 pt.
— **Dokunma alanları:** kart kapatma X ≥ 44 pt kutu, Bölümler geri tuşu 44 pt kutu, Ayarlar anahtarları dokunma kutusu ≥ 44 pt yükseklik (görsel aynı).

## Test
Var olan EditMode testleri bozulmasın. Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6).

## Bitince
KISA rapor: değişen dosyalar, çivi kuralı sonucu, 4. maddenin slot listesi nerede, varsayımlar, eksikler.
