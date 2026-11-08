# Hikâye eğrisi — SET40 v4 ölçümü (PM_MESAJ_47, 8 Kasım)

**Yöntem:** `python Tools/hikaye_egrisi.py` → Unity batch `HikayeEgrisi.Run` (Z seti için `RunZeyd`). Yol = **SmartPlayer'ın ilk çözümü**: ZeydBot'un sabit bir tercih sırası yok, rastgele seçiyor. SmartPlayer bombaya en kısa yoldan gider, yani bomba payı **en iyi oyuncunun** payıdır. Her adımda her çıkarılabilir parça için `PlayAfter` (bütçe 60k) çalıştırıldı. 40/40 bölüm çözüldü, bütçesi tükenen aday 0. Toplam 1.775 adım, ~1 dk.
Dosyalar: `hikaye_egrisi_set40.csv` (40 satır), `hikaye_egrisi_set40.html` (mini grafikler), `hikaye_kontrol.csv`, `hikaye_benzer10.csv`.

## Ana bulgular
1. **Tuzak neredeyse yalnız bombadan geliyor.** Çıkış kuralı tek yönlü (bir parçayı çıkarmak başka bir parçanın yolunu kapatmaz), yani yanlış hamleyi yalnız fitil yaratıyor. Tuzaklı adım toplam 94. Yalnız 7 bölümde var: 19, 25, 30, 32, 33, 34, 38. İlk hamle tuzak oranı bir bölüm dışında 0 (S32 %67).
2. **Bomba tanıtımı tehdit değil, sayılar doğruluyor.** Bölüm 6 (S45): fitil 23, bölüm 20 adım. Bölüm 7 (S46): fitil 33, 25 adım. İki bölümde de bomba **tamamen yok sayılabiliyor**.
3. **Bölüm 1–9 ve 12 düz azalan eğri:** boğaz yok, en uzun tek seçenekli seri 1. Boğaz ilk kez 10'da çıkıyor. Bölüm 10–40'ta medyan en uzun boğaz 4.
4. **K2'nin "son %30'da seçenek ≥ 3" şartı bizim oyuna uymuyor.** Sonda tahtada az parça kaldığı için seçenek doğal olarak düşüyor. 40 bölümden yalnız 2'si geçiyor (25, 33). Zeyd'in favorisi Z12 de kalıyor.

## Kontrol tablosu (SET40 v4)
| Kural | Kontrol | Geçti | Kaldı | Kalanlar |
|---|---|---|---|---|
| K2 | boğaz ≥ 1 **ve** son %30 ort. seçenek ≥ 3 | 2 | 38 | 25 ve 33 dışında hepsi. Yalnız boğaz şartı: 27 geçer, 13 kalır (1–9, 12, 28, 29, 36) |
| K3 | en uzun boğaz (tipe göre) | — | — | 1–9, 12, 28, 29, 36 = 1 · 10–27 çoğu 2–7 · 30–40 çoğu 4–14 |
| K4 | ilk 3 adımda seçenek ≥ 2, ilk hamle tuzak ≤ %30 | 38 | 2 | 32 (seçenek 1, tuzak %67), 34 (seçenek 1) |
| K5 | ZeydBot süresi gerekli. Vekil: son %30'da seçenek ≥ 3 | 8 | 32 | Vekil zayıf; gerçek ölçüm ZeydBot koşusuyla yapılmalı |
| Bomba D | ilk fitil ≤ bombanın yoldaki çıkış adımı + 2 | 21 | 9 | **6, 7**, 8, 9, 12, 23, 27, 29, 31 (10 bölümde bomba yok) |

## Bölüm yorumları
- **3 (S43):** 20 adım, açılışta en az 7 seçenek var, boğaz ve tuzak yok, eğri düz iniyor. Tanıtım olarak ferah ama eğride "yeni öğe anı" yok: öğretilen şey oyuncuyu hiç zorlamıyor.
- **6 (S45, bomba tanıtımı):** Fitil 23, bölüm 20 adım. Bomba hiç umursanmasa bile oyuncu kazanır, pay hiç 22'nin altına inmiyor. D tarifine göre fitil 3–4 olmalı (bomba 2. adımda çıkabiliyor).
- **10 (Z24):** İlk boğazlı bölüm: 2 boğaz, en uzun 3 adım. Sondaki çığ zayıf (son %30'da ortalama seçenek 1.5). "Bitir/nefes" adımı için fazla dar. K3'ün tanıtım sınırını (0–1) aşıyor.
- **14 (Z21):** Açılışta en az 9 seçenek var. Anahtar an yolun %33'ünde, ortada 4 adımlık tek parça zinciri var. Tanıtım için zincir uzun. Zincir 2'ye inerse K3 tanıtımı geçer.
- **Z12 (Zeyd'in favorisi, zeydbot.json):** 54 adım, açılışta 5 seçenek. Bomba fitili 9, 8. adımda pay 2'ye iniyor: **gerçek tehdit**, D tarifini geçiyor. 3 boğaz var, en uzun 10 adım; son %40'ta seçenek neredeyse hep 1–2. "Zincirleme tek parça" eğride çok net görünüyor. K2 yalnız çığ şartı yüzünden kalıyor, yani sorun kuralda, bölümde değil.

## Eşik önerisi (SET40 dağılımına göre)
- **K2:** Boğaz ≥ 1 şartı kalsın (ilk 9 bölüm hariç tutulur). Çığ şartı **göreli** olsun: son %30'da seçenek/kalan parça ≥ 0,35 (SET40 medyanı 0,42, alt çeyreği 0,32). Zor ve süper zor bölümler çığ şartından muaf olsun (Z12 bunu gösteriyor).
- **K3 (en uzun boğaz):** tanıtım/nefes ≤ 1, normal 2–4, zor ≥ 5 (Z12 = 10). Kitaptaki "zor ≥ 3, normal 1–2" bizim verimize göre fazla düşük.
- **K4:** ilk 3 adımda seçenek tanıtım/normal için ≥ 3 (alt çeyrek 3), zor için ≥ 2. Tuzak ≤ %30 kalsın ama bugün ayırt edici değil, çünkü tuzak yalnız bombadan geliyor.
- **K5:** eşik şimdilik değişmesin. ZeydBot süre ölçümü gelince kalibre edilsin.
- **Bomba D (yeni sert kural):** fitil < bölümün adım sayısı (her zaman). Tanıtımda fitil ≤ bombanın en erken çıkış adımı + 2.
- **K10 / benzerlik:** En benzer 10 eğri çifti çoğunlukla ilk düz bölümler arasında (1–9 0,95; 2–7 0,94; 1–29; 9–29; 9–28; 2–29; 1–28; 7–29) ve 11–16, 10–16. Normalize eğri tek başına düz azalan bölümleri ayıramıyor, siluet benzerliğiyle birlikte kullanılmalı.
