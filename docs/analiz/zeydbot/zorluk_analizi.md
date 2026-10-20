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

## v2 (20 Ekim): "düşündüren zor" ile "cezalandıran zor" — Z12 neden tekti

Kaynak: `bolum_yol.csv` (SmartPlayer yolu; `yaniltici` = kayabilen ama çıkamayan parça, `ZeydAnalysis.Misleading`), `ham_moves.csv` (ilk denemeler: touch = dokunup çıkaramama, jam = takılma).

| Bölüm | Puan | Yolda tuzaklı hamle | Hamle başına seçenek | Yanıltıcı parça / adım | Zeyd: dokunma / çıkış | Takılma | Süre / çözüm yolu |
|---|---|---|---|---|---|---|---|
| **Z12** | 9 | 0 | 2,5 | **3,80** | **0,46** | **11** | 2,31 |
| Z13 | 9 | 24 | 3,7 | 1,65 | 0,21 | 4 | 1,94 |
| Z18 | 9 | 3 | 2,1 | 0,02 | 0,11 | 4 | 2,50 |
| Z07 | 8 | 22 | 3,4 | 0,61 | 0,18 | 4 | 2,04 |
| Z08 | 7 | 17 | 2,5 | 1,02 | 0,11 | 6 | 2,41 |
| Z23 | 5 | 0 | 1,7 | 0,61 | 0,09 | 4 | 2,78 |
| Z25 | 4 | 0 | 1,8 | 0,56 | 0,06 | 2 | 2,69 |
| Z26 | 6 | 2 | 1,4 | 0,06 | 0,07 | 2 | 3,15 |
| Z29 | 6 | 17 | 3,7 | 0,07 | 0,11 | 1 | 2,13 |

**Z12'nin farkı tuzak değil, aramaydı.** Çözüm yolunda kalan parçaların ortalama %23'ü "serbest görünüp çıkamayan" parçalar; diğer 29 bölümde bu oran en fazla %9. Zeyd de Z12'de hamle başına 0,46 kez dokunup çıkaramadı (ikinci sırada 0,26) ve 11 kez takıldı (en yüksek). Z12'deki 20 "tek doğru" hamlenin hepsi zorunlu (tek oynanabilir parça). Zor olan seçmek değil, o parçayı bulmaktı.

**Z23 / Z25 / Z26 neden Z12 gibi gelmedi:**
- Hamle başına 1,4–1,8 seçenek var ve yanıltıcı parça az. Oynanabilir parça hemen göze çarpıyor, aranacak bir şey yok.
- Süre daha bol: çözüm yolunun 2,7–3,2 katı. Z12'de 2,3 katı. Zeyd üçünde de süre baskısı hissetmedi (82–86 s kaldı).

**Z29 neden 6:** Tuzak dolu (17), yani Z13 gibi bir "cezalandıran" bölüm. Ama yanıltıcı parça neredeyse yok. Zeyd bu kez tuzağa düşmedi (öğrenme etkisi), 64 s kala kazandı.

**Tasarım kuralı (hipotez, n = 1 + 4 karşıt örnek):**
- Düşündüren zor = çok yanıltıcı parça + zorunlu tek hamle zinciri + tuzak yok + dar süre (yolun ~2,3 katı).
- Cezalandıran zor = çok tuzak.

Z31–Z35 bu kurala göre seçildi (adım başına 1,8–2,4 yanıltıcı parça). Z12'ye (3,8) ulaşamadılar, ama oturum 3'ün zorlarından (0,06–0,61) çok yukarıdalar.
