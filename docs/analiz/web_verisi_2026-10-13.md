# Web oyuncu verisi — zorluk modeliyle karşılaştırma (13 Ekim 2026)

**Soru (PRODUCT.md Faz 2, B1–B2):** Zorluk modelinin `s` (parça başına saniye) sabiti gerçek oyuncu süreleriyle tutuyor mu? Yapı ölçüleri insanlar için zorluğu gösteriyor mu?

**Kısa cevap:** Ortalamada evet, ama veri çok az. Hiçbir sabit değiştirilmedi.

- `s` ortalamada isabetli. Kazananların medyan süresi / gövde, modelin `s`'inin **0,98 katı** (18 seviye). Güçlendirici kullanılmayan oyunlarda 0,95.
- Yapı ölçüleri zorluğun yönünü gösteriyor. `s_yapı` ile kazanma oranı arasında Spearman **−0,67** (24 seviye): okuma yükü yüksek seviyeler daha sık kaybediliyor.
- Seviye seviye süre tahmini zayıf. `s_veri` ile `s_yapı` arasında yalnızca **+0,40** (18 seviye).
- **PM'in durma koşulu tuttu.** n ≥ 10 olan seviye sayısı **1** (yalnızca L01), eşik 10. Bu yüzden aşağıdaki her sayı **yalnızca yön göstergesidir**; sabit ayarı için playtest verisi gerekir.

## Veri

- **Kaynak:** Supabase `plays`, `client = 'web'`. Yalnızca okundu (anon SELECT); Supabase'de hiçbir şey değişmedi.
- **Ham satırlar:** 183 satır, 6 oyuncu, 1 Eylül – 7 Ekim. `TEST_` adlı oyuncu yok, saat güçlendiricisi hiç kullanılmamış.
- **Eski süreli satırlar çıkarıldı (50 satır).** Web oyunu 6 Eylül'e kadar bugünkünden uzun sürelerle oynanmış (ör. L40: 225 sn → 65 sn). Bu, satırdaki `dur + left_s` toplamının bugünkü süreye eşit olmamasından anlaşılıyor. Kalan satırlar bugünkü süreyle oynanmış oyunlar; bu satırların bugünkü yerleşimle oynandığı varsayıldı.
- **Kalan:** 133 satır, 5 oyuncu. Bir oyuncu satırların %45'ini (60/133) oynamış.
- **Bilinen sınırlar:**
  - Web'in deneme sayacı hep 1 yazıyor, "quit" satırı hiç yazılmıyor. Bu yüzden deneme sayısı ve bırakma oranı kullanılmadı.
  - Satırların 69'unda makas, değnek ya da çekiç kullanılmış; bunlar süreyi kısaltır. Duyarlılık için güçlendiricisiz alt küme ayrıca hesaplandı.
  - Süre ve kalan süre yalnızca **kazananlardan** hesaplandı (hayatta kalan yanlılığı: zorlananlar süre dolunca ya da patlayınca listeye girmez).
- **Repoya giren:** yalnızca seviye düzeyinde toplamlar. Oyuncu adı ve ham satır repoya girmedi.

## Model ile veri, tür bazında (yalnızca kazananlar)

Tür, `40_seviye_raporu.md`'deki kuraldan geliyor.

| Tür | Kazanma (satır) | Kalan süre — veri | Kalan süre — model | s_veri / s_yapı (güçlendiricisiz) |
|---|---|---|---|---|
| Tanıtım | 21 | %53 | %55 | 0,93 |
| Kolay | 14 | %60 | %65 | 1,05 |
| Orta | 40 | %64 | %67 | 0,86 |
| Zor | 26 | %33 | %23 | 0,84 |
| Çok zor | 7 | %10 | %19 | 1,10 |

Kolay, orta ve tanıtım türlerinde model ile veri 2–5 puan içinde. Zor seviyelerde oyuncular modelin beklediğinden hızlı, çok zorda daha yavaş. Çok zorda yalnızca 7 kazanma var.

## Sıra korelasyonları (Spearman; yön göstergesi)

| İnsan ölçüsü | Yapı ölçüsü | ρ | Seviye |
|---|---|---|---|
| s_veri (kazananların medyan süresi / gövde) | hamle başına okuma | +0,11 | 18 |
| s_veri (kazananların medyan süresi / gövde) | ilk hamlede serbest (k₀) | +0,22 | 18 |
| s_veri (kazananların medyan süresi / gövde) | darboğaz | +0,35 | 18 |
| s_veri (kazananların medyan süresi / gövde) | s_yapı | +0,40 | 18 |
| s_veri (kazananların medyan süresi / gövde) | gövde | -0,17 | 18 |
| kazanma oranı | hamle başına okuma | -0,47 | 24 |
| kazanma oranı | ilk hamlede serbest (k₀) | -0,12 | 24 |
| kazanma oranı | darboğaz | -0,47 | 24 |
| kazanma oranı | s_yapı | -0,67 | 24 |
| kazanma oranı | gövde | -0,27 | 24 |
| kazanma oranı | bomba payı (hepsi) | +0,05 | 5 |
| kazanma oranı | bomba payı (L26/L30/L34 hariç) | -0,20 | 4 |

- Kazanma oranı en çok `s_yapı` (−0,67), okuma (−0,47) ve darboğaz (−0,47) ile ilişkili: model doğru şeyleri ölçüyor.
- Bomba payının kazanmayla ilişkisi yok (+0,05, 5 seviye). Az seviye ve az satır olduğu için hiçbir şey söylemiyor.
- Kazananların süresi gövdeyle ilişkisiz (−0,17). Parça sayısı zaten `s`'nin paydasında.

## Sabit önerileri (PM'e; uygulanmadı)

1. **`s` = 0,6 + 0,16 × okuma korunsun.** Ortalamada tutuyor (0,98). Veriye en küçük kareler uydurması `0,72 + 0,11 × okuma` veriyor, ama 18 seviye ve 5 oyuncuyla bu ayrım anlamlı değil.
2. **P ve yıldız eşikleri değişmesin.** Orta ve kolayda kalan süre modelle 2–5 puan içinde.
3. **Zor ve çok zor için playtest beklenmeli.** Zorda oyuncular modelden hızlı (veri %33, model %23), çok zorda yavaş (%10 ve %19). İkisi birlikte, P'nin zor 1,3 / çok zor 1,15 ayrımının fazla keskin olabileceğini düşündürüyor. Bir karar için seviye başına en az 10 satır gerekir.
4. **Tanıtım seviyeleri (L11, L21, L31) için öneri ile mevcut süre çok farklı** (öneri 70–75 sn, mevcut 45 sn). Ama web oyuncularında L11'de kazananlar %20, L31'de %53 süre bırakıyor. Tanıtım için P = 2,2'nin bu seviyelerde cömert kaldığı görülüyor; tanıtım seviyesinde yeni engel öğretilirken süre baskısının istenip istenmediği bir ürün kararı.

## Seviye tablosu

Sütunlar:
- **satır:** bugünkü süreyle oynanmış satırlar.
- **s_veri:** kazananların medyan süresi / gövde.
- **kalan (veri):** kazananların medyan kalan süre oranı.
- **kalan (model):** 1 − gövde × s / süre.

| Seviye | Gövde | Süre | Satır | Oyuncu | Kazanma | Kazanan medyan sn | s_veri | s_yapı | Kalan (veri) | Kalan (model) |
|---|---|---|---|---|---|---|---|---|---|---|
| L01 | 20 | 60 | 10 | 5 | %90 | 27 | 1,35 | 1,35 | %55 | %55 |
| L02 | 25 | 85 | 6 | 5 | %100 | 25,5 | 1,02 | 1,07 | %70 | %69 |
| L03 | 25 | 70 | 6 | 5 | %100 | 24,5 | 0,98 | 0,97 | %65 | %65 |
| L04 | 25 | 80 | 6 | 5 | %100 | 23,5 | 0,94 | 1,06 | %71 | %67 |
| L05 | 30 | 95 | 5 | 4 | %100 | 27 | 0,90 | 1,03 | %72 | %68 |
| L06 | 30 | 90 | 5 | 4 | %100 | 30 | 1,00 | 1,01 | %67 | %66 |
| L07 | 30 | 100 | 5 | 4 | %100 | 27 | 0,90 | 1,09 | %73 | %67 |
| L08 | 36 | 115 | 4 | 4 | %100 | 36,5 | 1,01 | 1,04 | %68 | %67 |
| L09 | 36 | 115 | 3 | 3 | %100 | 44 | 1,22 | 1,05 | %62 | %67 |
| L10 | 36 | 115 | 3 | 3 | %100 | 28 | 0,78 | 1,05 | %76 | %67 |
| L11 | 29 | 45 | 4 | 3 | %75 | 36 | 1,24 | 1,06 | %20 | %32 |
| L12 | 29 | 40 | 3 | 3 | %100 | 29 | 1,00 | 1,02 | %28 | %26 |
| L13 | 35 | 50 | 3 | 3 | %100 | 35 | 1,00 | 1,04 | %30 | %27 |
| L14 | 34 | 50 | 3 | 3 | %100 | 32 | 0,94 | 1,13 | %36 | %23 |
| L15 | 34 | 55 | 5 | 3 | %60 | 43 | 1,26 | 1,19 | %22 | %26 |
| L16 | 34 | 55 | 4 | 4 | %100 | 41,5 | 1,22 | 1,23 | %25 | %24 |
| L17 | 33 | 55 | 3 | 3 | %100 | 43 | 1,30 | 1,27 | %22 | %24 |
| L18 | 29 | 45 | 2 | 2 | %100 | 29 | 1,00 | 1,07 | %36 | %31 |
| L19 | 34 | 50 | 2 | 2 | %100 | 33,5 | 0,99 | 1,10 | %33 | %25 |
| L20 | 40 | 55 | 4 | 2 | %50 | 49 | 1,23 | 1,12 | %11 | %19 |
| L21 | 30 | 45 | 4 | 2 | %50 | 31,5 | 1,05 | 1,16 | %30 | %23 |
| L22 | 36 | 55 | 3 | 2 | %67 | 33,5 | 0,93 | 1,28 | %39 | %16 |
| L23 | 35 | 50 | 2 | 2 | %100 | 32,5 | 0,93 | 1,19 | %35 | %17 |
| L24 | 34 | 55 | 5 | 2 | %20 | 33 | 0,97 | 1,29 | %40 | %20 |
| L25 | 29 | 45 | 1 | 1 | %100 | 32 | 1,10 | 1,27 | %29 | %18 |
| L26 | 35 | 50 | 2 | 1 | %50 | 36 | 1,03 | 1,21 | %28 | %15 |
| L27 | 34 | 50 | 2 | 1 | %50 | 46 | 1,35 | 1,24 | %8 | %16 |
| L28 | 34 | 55 | 2 | 1 | %50 | 45 | 1,32 | 1,34 | %18 | %17 |
| L29 | 39 | 60 | 2 | 1 | %50 | 42 | 1,08 | 1,28 | %30 | %17 |
| L30 | 35 | 55 | 1 | 1 | %100 | 33 | 0,94 | 1,25 | %40 | %21 |
| L31 | 30 | 45 | 2 | 1 | %100 | 21 | 0,70 | 1,06 | %53 | %29 |
| L32 | 36 | 55 | 2 | 1 | %100 | 26 | 0,72 | 1,20 | %53 | %22 |
| L33 | 35 | 50 | 3 | 2 | %100 | 28 | 0,80 | 1,13 | %44 | %21 |
| L34 | 35 | 55 | 4 | 2 | %25 | 38 | 1,09 | 1,34 | %31 | %15 |
| L35 | 34 | 60 | 2 | 1 | %50 | 54 | 1,59 | 1,46 | %10 | %17 |
| L36 | 29 | 50 | 2 | 2 | %100 | 32 | 1,10 | 1,18 | %36 | %31 |
| L37 | 35 | 55 | 2 | 2 | %50 | 24 | 0,69 | 1,22 | %56 | %22 |
| L38 | 35 | 60 | 1 | 1 | %100 | 32 | 0,91 | 1,29 | %47 | %25 |
| L39 | 40 | 70 | 4 | 2 | %25 | 63 | 1,57 | 1,42 | %10 | %19 |
| L40 | 39 | 65 | 1 | 1 | %100 | 64 | 1,64 | 1,35 | %2 | %19 |

Aynı tablo: `web_verisi_2026-10-13.csv` (`;` ayraçlı, Türkçe Excel'de açılır).
