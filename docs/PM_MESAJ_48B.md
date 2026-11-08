# PM_MESAJ_48B — Ajan döngüsü düzeltmesi (p21_06 9 adayda da RED)

Durum: `p21_06_turlar.md` 3 turda 9 adayın hepsini reddetti, finalist boş. **Sorun adaylarda değil kurallarda: kurallar birbiriyle çelişiyor** (PM hatası). Aşağıdakileri düzelt, sonra PM_48'e kaldığın yerden devam et (06 → 07 → yayın → rapor).

## 1. Bomba tanıtım tarifi (kitap D güncellendi, oku)
- Eski: `bomba_fitil_max 2`, `margin 1`, `earliest [2,4]`. Sonuç: ilk hamle tuzak %80, FunSim ilk deneme kazanma %21. Bu tanıtım değil ceza.
- Yeni: `earliest [3,5]`, **`margin 2` (fitil = en erken çıkış + 2, tam)**, `bomba_fitil_max` = earliest+2. `bomba_tehdit_adimi_max` 5.
- Yeni kontroller (tanıtım tipi): ilk hamle tuzak = 0. `bot_bomba_unutan` kaybeder. `rastgele bot` kazanma %15–50. **FunSim ilk deneme kazanma ≥ 0,65.**

## 2. Güvenilmeyen ölçümler RED vermesin, UYARI versin
- **R16 final temposu:** kitapta K5'in eşiği "ZeydBot süresiyle kalibre edilecek" diye bırakılmıştı. Şimdilik **UYARI**. Eşiği aynı ölçümle SET40'ın 40 bölümünden kalibre et (tanıtım/normal için SET40 medyanı) ve raporda göster.
- **R14 kalan süre:** ZeydBot örneği n=2 (kazanma %0,5) güvenilmez. Kural: **n < 20 ise UYARI**, karar FunSim/oyuncu modeli kalan süre dağılımıyla verilsin. Tanıtım aralığını genişlet: medyan 0,25–0,50.
- Bir kontrolün ölçüm kaynağı yetersizse (örnek az, model kalibre değil) her zaman UYARI, asla RED.

## 3. Adaylar gerçekten farklı olsun
Turlar arası düzeltme şu an çoğunlukla `seed`/`timer` itmesi. Adaylar birbirinin kopyası (maske benzerliği 1,000). Kural: aynı turdaki 3 adayın maske benzerliği < 0,90 olsun (farklı siluet/boyut). Eleştirmen düzeltmesi sayı itmenin yanında **yerleşim** önerisi de versin ("bombayı 2. sıradan 3. sıraya al" gibi).

## 4. Tıkanınca durma, raporla
3 turda GEÇTİ yoksa: hangi kuralların birbiriyle çeliştiğini tespit et (ör. "R07 ve R11 aynı anda sağlanamıyor çünkü …"), **en yakın adayı "aday" etiketiyle yine de p21 setine koy** ve raporda yaz. Sessizce bekleme. Yeni bir PM kararı gerekiyorsa `docs/analiz/AJAN_DILIM1_RAPOR.md`'nin en üstüne "PM KARARI GEREKİYOR:" diye tek satır yaz ve dur.

## 5. Sonra
PM_48 adım 4–5 (p21_07, p21'de `set=ajan1` yayını, `AJAN_DILIM1_RAPOR.md`, PROJE_DURUMU satırı). Kabul şartları aynı.
