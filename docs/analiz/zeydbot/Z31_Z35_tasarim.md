# Z31–Z35 — tasarımcı ajan + fabrika (PM_MESAJ_2, 2026-10-20)

**Zeyd oynamadan önce okumasın.** Yöntem: tasarımcı ajan (`seviye-tasarimci`) önce Z12 ve Z13'ü inceleyip tarifi yazdı (`Z12_TARIFI.md`). Sonra taslakları bir taban tahta ve işlem listesi olarak niyetle kurdu (`Tools/zeydbot_draft.py`). Fabrika her taslağı doğruladı (`ZeydDesign.Check`: çözülebilirlik, supap, M1–M12, rastgele oyuncu, adım adım seçenek/doğru/yanıltıcı). 4 tur, 16 taslak; işlem listeleri `Logs/zdesign/tur1..4.json`, raporlar `Logs/zdesign/`.

## Turlar

| Tur | Taslak | Sonuç |
|---|---|---|
| 1 | T31a–T35a | Yalnız Z12'nin aynaları yapıyı taşıdı (yanıltıcı 3,5–3,8). Mühür ve çivi eklemek yanıltıcı üretmedi (0,11–1,57). |
| 2 | T31b–T35b | Z12'nin 3 yönelimi (x, y, xy) hedefi tuttu. Aynalar yapıyı değiştirmiyor. Başka tabana dikiş taşıma çözülemedi. |
| 3 | T36c–T39c | **T37c (Z23):** "kayar ama çıkamaz" satırı yanıltıcıyı 0,61'den 2,46'ya çıkardı. Z25 denemeleri tıkandı (zincir). |
| 4 | T40d–T41d | Ek dikiş kaldırmanın etkisi olmadı (2,46). T41d = T37c'nin aynası, süre 118. |

## Seçilen 5

| Bölüm | Taslak | Taban | Fikir / aha (ajanın notu) |
|---|---|---|---|
| Z31 | T32b | Z12, 180° dönüş, geç bomba | Bombalar 20., 22. ve 24. hamlede eşit aralıkla biter, sonra baskıyı yalnız süre taşır. Aha: köşede serbest görünen parçayı bulmak (çıkış #15 civarı). |
| Z32 | T37c | Z23, orta satır iki yandan mühürlü | Ortadaki satırın parçaları yana kayar ama çıkamaz, çıkış hep yukarı/aşağı. Aha: "kayan ama çıkmayan" parçaları ayırt etmek. |
| Z33 | T31b | Z12, dikey ayna, erken bomba | İlk beş hamlede fitili 6 olan bombayı kurtarmak, sonra uzun zorunlu zincir. Aha: üst sıralar boşalınca kayan parçayı bulmak (çıkış #9). |
| Z34 | T41d | Z23, yatay ayna, süre 118 | Z32'nin fikri aynalı tahtada ve daha dar sürede. |
| Z35 | T33b | Z12, yatay ayna, bomba ve çivi taşındı | Çivili parça 17. hamlede açılır. Aha: o ana kadar yanıltıcı parçalarla oyalanıp çivinin sayacını fark etmek. |

## Fabrika ölçümleri (SmartPlayer yolu)

| Bölüm | Çözülür, supap 0 | Rastgele | M11 | M12 | Tuzaklı hamle | Zorunlu | Ort. seçenek | Yanıltıcı / adım | Süre / yol |
|---|---|---|---|---|---|---|---|---|---|
| Z12 (referans) | ✓ | %0 | 4/4 | %54 | 0 | 20 | 2,48 | 3,80 | 2,31 |
| Z31 (T32b) | ✓ | %0 | 4/4 | %54 ✓ | 0 | 20 | 2,46 | 3,81 | 2,31 |
| Z32 (T37c) | ✓ | %0 | 4/4 | %67 ✗ | 0 | 20 | 2,19 | 2,46 | 2,31 |
| Z33 (T31b) | ✓ | %0 | 4/4 | %54 ✓ | 0 | 20 | 2,48 | 3,52 | 2,31 |
| Z34 (T41d) | ✓ | %0 | 4/4 | %67 ✗ | 0 | 20 | 2,19 | 2,46 | 2,19 |
| Z35 (T33b) | ✓ | %1 | 4/4 | %54 ✓ | 0 | 20 | 2,37 | 3,48 | 2,31 |

## Sapmalar (dürüst not)

- **Z31, Z33 ve Z35 Z12 türevi.** Tahta aynı, yalnız döndürülmüş ya da aynalanmış; bombalar ve çivi taşındı. Zeyd tanıyabilir. Tanırsa "Z12 gibi" cevabı yöntemi değil hafızayı ölçer. PM'in ölçütü (5'in 3'ü "Z12 gibi") bu yüzden Z32 ve Z34'e ayrıca bakılarak okunmalı: bunlar Z12 türevi değil.
- **Z32 ve Z34 aynı tabandan** ve ikisinde de M12 %67 (hedef ≤ %60). Z23'ün çözümü dikey ağırlıklı. Ajan M12'yi tutan tek tabanda (Z25) mekanizmayı uygulayamadı.
- **Z32/Z34'ün yanıltıcısı 2,46**, tarifin hedefinin (≥ 2,5) hemen altında.
- **Tur sayısı:** Bölüm başına 6 tur yerine 4 tur yapıldı. Fabrika ölçümü taslak başına 6–10 dk, bot yolları 20–40 dk sürüyor.
- **ZeydBot v1 bu tip için geçerli bir ölçüt değil.** Z12'nin kendisine 4,4 puan ve %67 kazanma veriyor. Seçim tarifin ölçütleriyle yapıldı; v1 tahmini yine de donduruldu (`tahmin_Z31_Z35.md`).
- Başlangıç payı 54 parçada çoğunlukla "ölçülemedi": kazı araması bütçeyi aşıyor. Fitiller Z12'den geliyor; Z32'de pay 1 ölçüldü.
