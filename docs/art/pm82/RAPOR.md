# PM_82 RAPOR: ilk 100 bölüm, harita yok, rehberli 1. bölüm, isteğe bağlı reklam

p23 varsayılanı artık **sl100**. Eski 40'lık set `&set=redesign` ile açılıyor. `live/`'a **yayınlanmadı** (PM onayı bekliyor).

## 1. İlk 100 bölüm (sl100)
- **Plan:**
  - dersler: 3 kilit, 7 zincir, 11 bomba, 21 duvar;
  - birleşimler: 31, 41 … 91;
  - booster açılışları: 6 makas, 13 çekiç, 18 saat, 25 değnek;
  - zorluk: 4 ile bitenler zor, 9 ile bitenler çok zor, x0'lar nefes.
- **Nasıl seçildi:** zorluk modeli v2 (sınama hatası ±10,7) ve hedefe göre seçim.
  - Aday havuzu:
    - projedeki bütün bölümler: p22'nin oynanmış hâli, redesign v2–v4, web, ZeydBot, fabrika finalistleri, ajan setleri;
    - bunların bir engel türü çıkarılmış hâlleri (çivi, duvar ya da bomba çıkınca kayıtlı çözüm bozulmaz);
    - üreticinin yeni kare tahtaları (5×5, 6×6, 7×7).
  - Toplam 3 415 aday. Her slot için:
    - kurallara uyanlar (açılmış engeller, ders/birleşim, boyut);
    - model v2'nin ilk deneme tahminini hedefe getiren süre (60–120 sn, zor/çok zorda ≤150 sn);
    - en iyisi: hedefe uzaklık, kare tahta, çeşitlilik.
  - Çeşitlilik kuralları:
    - bir tahta bir kez;
    - komşu iki bölüm aynı boyutta değil;
    - komşu iki bölümde aynı bomba deseni yok.
  - Araç: `Tools/sl100/Design.cs` (Core/Generation'ı Unity'siz derleyen dotnet aracı) + `Tools/sl100/finalize.py`.
- **Ajan döngüsü yerine:** CLAUDE.md "subagent açma" diyor. Tasarımcı ajanın işini (hedefe göre en az 8 aday, doğrulama, banda en yakın) bu araç yaptı.
  - Sabit slotlar dışında her slotta en az 66 uygun aday vardı.
  - Sabit slotlar: 1 = p20'nin S41'i (rehberli), 2 = p22'nin S42'si (engelsiz, ≤16 parça: yalnız 2 aday), 3/7/11/21 = p22'nin dersleri aynen.
- **Sonuç:** 100 bölümün **100'ü hedef bantta**.

  | | Sayı |
  |---|---|
  | Kare tahta | 74 |
  | p22 tahtası (pt1 verisi olan) | 30 (1'i aynen, 29'unun süresi değişti: sürüm +1) |
  | Üreticinin yeni tahtası | 47 |
  | Eski havuzdan (fabrika, web, ZeydBot, p22'nin engeli çıkarılmışı) | 23 |
  | Süre aralığı | 49–140 sn |

- **Çıktılar:**
  - bölüm tablosu `docs/analiz/PM82_100_BOLUM.md`;
  - eğri `docs/analiz/pm82_egri.png`;
  - tahminler `docs/analiz/v2/pred_sl100.csv`.
- **Kimlikler:** p22 tahtası kimliğini korur (sürüm, oynanış değiştiyse +1). Diğerleri kalıcı `SL001`…`SL100` (yeniden kullanılmaz).

## 2. Harita yok, düz ilerleme
- Oyuncu menüsünde yalnız "Oyna · Bölüm N" (harita ve bölüm listesi yalnız `&test=1`'de).
- Bitmiş bölüme dönülmüyor. Kaybedince aynı bölüm tekrar oynanıyor.
- **Zor bölüm:** kırmızı düğme + "Zor bölüm".
- **Çok zor bölüm:** koyu kırmızı düğme + Kont mührü + "Çok zor bölüm".
- Bölüm başında aynı renkte kısa şerit (~1,8 sn, dokunmak gerekmiyor). Eski "Başla" kartının yerine geçti.

## 3. Rehberli 1. bölüm
- Tahta p20'nin 1. bölümü (S41, 3×3). Eski eldivenli el geri geldi: büyük, hareket yönünde altın iz, eylem yapılana kadar döngü.
- Jigo krem sohbet kutusunda konuşuyor, yazı harf harf akıyor. Ekran kararmıyor, sıradaki parça altın parlıyor. Süre ve kayıp yok.
- **Adımlar** PM'in tablosundaki gibi:
  1. Merhaba;
  2. sol alt aşağı;
  3. sağ alt sağa: S41'de gerçekten takılıyor, yön değiştirmeye gerek kalmadı;
  4. temas noktasında kırmızı parıltı;
  5. aşağı;
  6. serbest oyun;
  7. son parçada "Bütün Jiggle'lar kurtuldu!", sonra "Harika!".
- **Kurallar:**
  - yanlış parça sallanır, Jigo "Önce bunu dene!" der;
  - 4 sn hareketsizlikte el geri gelir;
  - serbest kısımda ve 2. bölümde 6 sn hareketsizlikte açık bir parça parlar.
- PlayMode testi 7 adımı, yanlış dokunuşu ve hareketsizlik ipucunu sırayla geçiyor.

## 4. İsteğe bağlı reklam
- **R3:** booster 0 olunca yuvada küçük kırmızı video işareti; dokununca reklam → +1, günde en çok 5.
- **R4:** menüde "Hediye" sandığı; reklam → 100 altın, günde 3. Reklam hazır değilse görünmez.
- İkisi de `live/config.json`'dan aç/kapa; telemetride `r3_booster` / `r4_hediye`.

## 5. Kazanma ekranı düğmeleri
- Codex 3 seçenek çizdi:
  - **A** (klasik, kalın 3B, derin alt kenar): **9/10**;
  - **B** (altın çerçeve): **8/10**;
  - **C** (hap): **7/10**.
- A ve B `docs/art/pm82/kazanma_dugme/` altında; C `elenen/` içinde. **A bağlandı:**
  - Sonraki ekranın ~%70'i, 76 pt;
  - Menü 196×60 pt;
  - basınca çöken yüz.
- Altın satırı, sandık çubuğu ve düğmeler biraz yukarı alındı, Menü booster çubuğunun üstüne binmiyor.

## Tablolar (Codex TASK51)
- **41–90 hazır**, stil 1–40 ile aynı. 91–100 Codex'te.
- Eksik tablo olan bölüm 1–40'tan birini gösteriyor; çökme yok.

## Doğrulanmayan
- Model v2 tahmini gerçek oyuncu verisi değil (sl100'ün yeni bölümleri hiç oynanmadı).
- iPhone'da his.
