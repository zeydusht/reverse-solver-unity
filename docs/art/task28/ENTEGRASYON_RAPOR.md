# PM_MESAJ_52 — Entegrasyon raporu (9 Kasım)

**Durum: build hazır, p21'e YAYINLANMADI.** `verify_build --local` 3/4; kalan `card` (aşağıda). Kaynak değişiklikleri commit edilmedi.

Vitrin: `p21/?lv=SHOW_ART` menüyü açar, **Demo** düğmesi bölümü başlatır (SHOW1 ile aynı akış). Eski görsel: `&art=eski`.
Ekran görüntüleri (390×844, build üzerinde): [başlangıç](entegrasyon/vitrin_baslangic.png), [iki hamle sonra](entegrasyon/vitrin_iki_hamle_sonra.png).

## Ölçümler

| Kontrol | Sonuç |
|---|---|
| EditMode | 598 geçti, 0 kaldı, 2 atlandı (600; yeni `T28ArtTests` 4/4) |
| `verify_build --local` | boot 40/40, k1 12/12, fps GEÇTİ; **card KALDI**: başlık–çerçeve 7 pt, sınır 8 pt. PM_51 build'inde de aynı 7 pt; `PRODUCT.md` açık işi "eski KALDI" diyor |
| FPS (PC, gerçek GPU) | medyan 59,9, p95 59,2, en düşük 58,1 (L40 ve D02). **iPhone değil** |
| Telefonda FPS | **Ölçülmedi**, Zeyd'in telefonu gerekiyor |
| Build boyutu (Brotli) | 16,33 → 17,05 MB (+0,72 MB; data 10,68 → 11,39 MB) |
| Vitrinde iki eylem | Bomba 7→5 ve 3→1, kilit 3→1, sayacı 1 olan kilit açıldı, çekiç zincirli parçayı kaldırdı; istisna 0; `art=eski` ile de istisna 0 |

## Değişenler

- **Görseller:** 61 PNG `Presentation/Resources/Art/yeni/` altına kopyalandı. Eski dosyalar yerinde. `Art.cs` önce `yeni/`, sonra eskiye bakar. HUD ve booster adları ve boyutları birebir uyuştu; rapor edilecek uyumsuzluk yok.
- **Bayrak:** `Art.T28`, `GameRoot` açılışında `art=eski` ile kapanır. Görselleri geri alır; metinler "kilit" kalır.
- **Bomba:** `Badges.cs`, yeni `BombMotion.cs`. 8 kare fitil 11 fps; ≥4'te %3 / 1,2 sn nefes; 3-2-1'de acil PNG, her hamlede %8 pump + titreme. Sayaç etiketi ayrı katman, sallanmıyor.
- **Asma kilit:** `Badges.cs`. Canlı numara; 0'da açılma + düşme kareleri (7 kare, 14 fps), sonra silinir. Kural ve bölüm verisi aynı.
- **Zincir:** yeni `ChainCurve.cs`, `ChainRope.cs`; `ChainView.cs`, `BoardView.cs`. Parabol, en fazla 0,4 hücre sarkma, başka parçaya binince sarkma azalır, halkalar teğete göre, sürüklemede 0,4 sn yaylanma. 5 renk ve işaret.
- **Metin:** çivi→kilit, nail→lock: `Texts.cs`, `Generator.cs`, 4 bölüm JSON'u (yalnız `intro`/`bintro` metni; karşılaştırıldı), `level_texts_en.json`, `docs/i18n/en.md`, `GeneratorTests.cs`. Seviye editörü etiketleri değişmedi.
- **Vitrin:** `Resources/show_art.json`, 6×8.

## PM'e açık konular

1. **Booster "Z1" mi Z2 mi:** mesaj Z1 diyor; `_yeni/Boosters` klasörü, üretim kaydına göre son seçim olan Z2 yüzlerini taşıyor. Klasör kullanıldı. Z1 isteniyorsa görsel değiştirilir (arşivde `_arsiv/Z1-v3`).
2. **Zincir kopması:** kurallarda kopma olayı yok, zincirli parça hep bütün çıkıyor. Kopma kareleri yalnız çekiçle vurulunca oynuyor.
3. **Titreşim:** mesajda "Medium" geçiyor; gerçek harita jam = Medium, bomba = Heavy. Dokunulmadı.
4. **`card` kontrolü:** yayın kuralı KALDI varken yayını yasaklıyor; karar bekliyor.

## Doğrulanmayanlar

- Telefonda FPS ve iPhone Safari'de görünüm.
- Kilit açılma ve zincir kopma karelerinin tek tek görüntüsü: animasyonlar istisnasız çalıştı, ama ekran görüntüsü bittikten sonra alındı.
- Sürüklemedeki yaylanma gözle görülmedi.
- Halkalar nötr PNG'nin çarpımla renklendirilmesi; üretim kaydındaki gradyan eşlemesinin birebir karşılığı değil.
