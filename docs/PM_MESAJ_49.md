# PM_MESAJ_49 — ajan2: Zeyd'in ajan1 notlarıyla 6–7'yi yeniden üret

Önce `docs/BOLUM_TASARIM_KITABI.md`'de şunları oku: C tablosunda K5, D'de "Tanıt" satırı, "Normal tip alt sınırı", **"Öğrenme dönemi"**, **"Tahta oranı"** (hepsi 8 Kasım). `ajan1` setine dokunma (karşılaştırma için kalsın). p20/kök/pt1 yasak.

## Zeyd'in notu (ajan1'i oynadı)
1. Tahtalar dikey (3×5, 3×6, 5×7, 4×6). Değişiklik fena değil ama yabancı geldi, biz genelde kare yapıyoruz.
2. Bunlar hâlâ tutorial, unutmayalım.
3. **Süreden kaybetti** → saniyeler artsın. Kayıp bombadan gelmeli, süreden değil.

4. **(19:24 ek) Tanıtımda kimse kaybetmesin.** Oyuncu sadece şunu görsün: "bomba diye bir şey var, sayı bitmeden çıkarmam lazım." Bombalar bir altta, bir üstte, bir ortada olsun.

## Kural değişiklikleri (`tip_varsayilan.json` + eleştirmen)
- **R12 rastgele bot:** tanıtımda UYARI (bilgi). Kitapta karar verildi.
- **R16 final temposu:** her tipte UYARI.
- **Yeni R20 süreden kayıp:** FunSim'de "süre bitti" kayıp oranı. Öğrenme döneminde (bölüm ≤ 10) tanıtım/nefes en fazla %5, normal en fazla %15 → KALDI. Süre (`timer`) bunu sağlayacak şekilde ayarlansın. Eleştirmen ilk düzeltme olarak süreyi artırsın.
- **Yeni R21 tahta oranı:** |en − boy| ≤ 1 (bölüm ≤ 10 için KALDI, sonrası UYARI). Siluet çeşitliliği (R15) köşe kesme/boşluk ile sağlansın. Üretici destekliyorsa maske, desteklemiyorsa şimdilik yalnız kare ve rapora "siluet maskesi yok" notu.
- **Normal tip:** FunSim ilk deneme kazanma ≥ %30. 07 (geliştir) öğrenme döneminde olduğu için **≥ %50** iste.
- Tablo/resim oranını kontrol et: 40 tablonun en/boy oranı ne? Kare dışı tahtada resim kırpılıyor ya da esniyorsa raporda yaz.

## Üret
- **`p21_06` (tanıt) yeni tarif (kitap D "Tanıt" satırı):** 2–3 bomba, farklı konumlar (alt/üst/orta), her biri 1–2 parça açınca ulaşılabilir, fitil ≥ en erken çıkış + 4. FunSim ilk deneme ≥ %90, süreden kayıp ≤ %5. R10/R11 tehdit ve "unutan bot kaybeder" tanıtımda UYARI. Tutorial kartı (intro) metni bombayı anlatmaya devam etsin.
- `p21_06` (tanıt) ve `p21_07` (geliştir, 2 bomba + duvar) kartlarını bu kurallarla güncelle, döngüyü çalıştır.
- Finalistleri **yeni setle** yayınla: `p21/?set=ajan2&lang=tr` (sıra: 06 finalist 1–2, 07 finalist 1–2).
- 07'de 4 adımlık tek parça zinciri iyi bir fikirdi, korunabiliyorsa koru ama FunSim ≥ %50 öncelikli.

## Rapor
`docs/analiz/AJAN_DILIM1_RAPOR.md` altına "ajan2" bölümü (yarım sayfa): ajan1 → ajan2 farkı tablosu (tahta, süre, FunSim kazanma, süreden kayıp %, unutan bot), link. PROJE_DURUMU açık iş 6'ya tek satır. Takılırsan aynı kural: en yakın adayı yine yayınla, en üste "PM KARARI GEREKİYOR:" yaz.
