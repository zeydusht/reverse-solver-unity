# ZeydBot — zorluk analizi (Z01–Z20, gece 2026-10-20)

Kaynak: `denemeler.csv`, `hamleler.csv`, `bolum_ozellik.csv` (ZeydAnalysis); korelasyonlar `zorluk_korelasyon.txt` (`Tools/zeydbot_korelasyon.py` ile üretildi, aynı hesap `Tools/zeydbot_model.py` içindeki `spearman`). Puan = Zeyd'in 1–10 cevabı (18 bölüm; Z01, Z02 puansız). Yalnız ilk deneme; Z03 deneme 2 (quit, 0 hamle) atıldı. Z01 ve Z07 "kör değil" olabilir, ayrıca işaretli.

## En çok açıklayan 4 özellik

| Özellik (sade adı) | Puanla sıra ilişkisi (ρ) | İlk denemede kaybetmeyle ρ | Sürenin ne kadarını harcadığıyla ρ |
|---|---|---|---|
| Bombanın en derin kazısı (bombaya kaç hamlede ulaşılır) | +0,92 | +0,45 | +0,57 |
| Başlangıçtaki en dar bomba payı (fitil − en kısa kazı) | −0,92 | −0,43 | −0,64 |
| Rastgele oyuncunun kazanma oranı | −0,92 | −0,55 | −0,54 |
| Çözüm yolunda "tek doğru seçenek" kalan hamle sayısı | +0,83 | +0,42 | +0,45 |

**Dürüst uyarı:** bunların hepsi büyük ölçüde "tür"ü (kolay/orta/zor) ölçüyor. Parça sayısı bile ρ = +0,94 veriyor, çünkü zor bölümlerde 54 parça, kolaylarda 25–35 parça var. 18 bölümle tür içindeki farkı güvenle ayıramıyoruz.

## Tür içinde ne görülüyor

- **Kolay (7 bölüm):** hepsi 1–2 puan aldı, hiç kayıp yok. Bombasız bölümler Zeyd için "boş".
- **Orta (6 bölüm):** ilk denemede kazanılan dört bölüm 2–3 puan aldı. 4–6 puan alan iki bölüm (Z16: 6, Z17: 4) **ilk denemede bombayla kaybedilenler**. Z16'nın başlangıç payı 1, diğer ortaların 2. **Orta bandı (4–7) ancak bir kez yanma ihtimaliyle doluyor.**
- **Zor (6 bölüm):** 7–9 puan. Bunlardan 3'ü ilk denemede kaybedildi (Z07 ve Z13 bomba, Z12 süre).
- **Zeyd'in sözü veriyle uyumlu:** "zorluk bombadan geliyor, fitil hep uçta". Puanı 4 ve üstü olan 8 bölümün hepsinde başlangıç payı 0–2. Bombasız hiçbir bölüm 2'nin üstüne çıkmadı.

## Z12 neden "keyifli zor", Z13 ve Z18 neden "sadece sıkı zor"

| | Z12 | Z13 | Z18 |
|---|---|---|---|
| Puan / ilk deneme | 9 / süre bitti (125 s) | 9 / bomba (38 s) | 9 / kazandı (101 s) |
| Çözüm yolunda tuzaklı hamle (yanlış seçince kurtuluş yok) | **0** | 24 | 3 |
| "Tek doğru" hamle sayısı | 20 | 12 | 10 |
| Hamle başına ortalama seçenek | 2,5 | 3,7 | 2,1 |
| Bomba payı / en derin kazı | 1 / 23 | 1 / 32 | 0 / 51 |
| Zeyd'in ortalama / en uzun düşünmesi | **3,4 s** / 24,5 s | 1,5 s / 15,1 s | 1,9 s / 19,6 s |

**Z12'nin sırrı (hipotez, n = 1):** her hamlede doğru seçenek tek ve fitil dar, ama tuzak yok. Yanlış bir parçayı oynamak oyunu bitirmiyor, yalnızca zaman kaybettiriyor. Zeyd o yüzden durup düşündü: ortalama düşünmesi diğer zorların iki katı. Süreyle kaybetti, ikinci denemede 83 s'de çözdü. "Buldum be" hissi, cezayla değil düşünerek bulunan çözümden geliyor.

**Z13** tersine tuzaklarla dolu (24 hamlede yanlış seçim çözümü bitiriyor). Zeyd 16 doğru hamleden sonra tuzağa düştü ve 38 s'de bombayla kaybetti. Bu bir "düşün" bölümü değil, "cezalandır" bölümü. **Z18** ise uzun bir kazı (51 hamle, pay 0, 4 çivi): zor, ama sıkı bir iş gibi.

Tasarım için çıkarım: Z12 tipi = **dar fitil + tek doğru yol + tuzak yok (ya da çok az) + süre baskısı**. Tuzaklı zor, keyiften çok ceza hissi veriyor olabilir. Bu tek bir bölümden çıkan bir hipotez; oturum 3 ve Z31–Z38 ile sınanmalı.
