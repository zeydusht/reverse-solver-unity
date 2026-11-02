# PM → Claude Code (31 Ekim 2026) — SET40 v4: 9 bölümü ayarla ve önizlemede yayınla

Zeyd `docs/SET40_V4.md` tablosunu onayladı. Kök adrese (Build 5) ve iOS zip'ine DOKUNMA. Yalnız önizleme.

## 1. Ayarlanacak 9 bölüm (tabloda `*`)

Hepsinde hedef: bombasız/rahat oyunu bomba ve fitil baskısıyla hedef puana çekmek. Oynanışı değiştiği için her birinin `version`'ı artar, `id` korunur.

| Slot | Bölüm | Değişiklik | Hedef puan |
|---|---|---|---|
| 8 | S47 | 2 bombanın fitili kısalır (29/35 → ~16/20) | 3 |
| 9 | S20 | değnek bölümü; fitil 12 → 10 | 3 |
| 12 | S51 | fitil 15 → ~10 | 3–4 |
| 17 | S18 | +1 bomba | 4–5 |
| 26 | S17 | +1 bomba (nefes bölümü) | 3–4 |
| 27 | S10 | +1 bomba (nefes bölümü) | 3–4 |
| 28 | S50 | +1 bomba (nefes bölümü) | 3 |
| 29 | S52 | +1 bomba (nefes bölümü) | 3 |
| 31 | S08 | +1 bomba (nefes bölümü) | 3–4 |

Not: 26–29 ve 31 slotları "nefes": zor bölümler arasında 3–4 puan, çok kolay (1) olmasın.

## 2. Doğrulama (SHOW1'de yaptığımız gibi)

- Her ayarlanan bölümde fitilleri FunSim ile tara, ilk denemede kazanma oranı hedef puana uyana kadar ayarla.
- SmartPlayer ve 40/40 açılış kontrolü. Çözülemeyen bölüm olmasın.
- Her bölümün bot sonucunu `docs/SET40_V4.md` tablosundaki "ZeydBot tahmini" sütununa yaz (`ölçülmedi` yerine).
- Yeni bölümler arasında benzerlik kontrolü (`Tools/level_similarity.py`), eşik %60.

## 3. Setin yazılması

1. `redesign.json` v4: tablodaki sıraya göre 40 slot. Yeni gelenler: Z04, Z21, Z23, Z24, Z25, S21, S39 (S34 ve S22 öne alınır). Çıkanlar: S48, S49, S26, S25, S30, S11, S23. Değişen bölümlerin `version`'ı artar, id'ler korunur.
2. Tanıtım kartları: zincir kartı S48 → Z24, duvar kartı S52 → Z21. Booster tanıtımları (slot 5, 9, 15, 23) aynı slotta kalır.
3. Zor uyarısı kartı: slot 19, 23, 25, 30, 35, 36, 38 (SET40_V4.md "Ne değişiyor").
4. 40 tablonun sırası slotlara göre kalır.
5. Seviye numaralarını (UI'da "Seviye N") 1–40 slot sırasına göre göster.

## 4. Yayın

- Önizleme: `p19/?set=redesign` linki aynı kalsın veya yeni klasör aç (`p20/`), hangisi daha güvenliyse. Zeyd'e linki yaz.
- Canlıda doğrula (40/40 açılış, 3 seviye elle: 8, 17, 31).
- Bozulan yer olursa geri al, `OVERNIGHT.md`'ye yaz.
- `OVERNIGHT.md` başına 3 satır: yayın linki, ayarlanan 9 bölümün bot sonuçları, sorunlar.

## Yapma
- Kök adresi güncelleme ("kökü güncelle" Zeyd'den gelmeden).
- iOS zip üretme, Unity derlemesi alma. Seviyeler Pages'ten iner, yeni iOS build gerekmez.
