# PM_MESAJ_52 — Entegrasyon raporu (9 Kasım)

**Durum: p21'de yayında (9 Kasım, PM_MESAJ_53).** Aşağıdaki ölçümler PM_52 build'ine aittir; yayınlanan sürümün sonuçları en sondaki ektedir.

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

## Ek — PM_MESAJ_53 yayını (9 Kasım)

**Sürüm:** kaynak `4621ec21` (`art` dalı, itilmedi), yayın `680cacc6` (gh-pages, yalnız `p21/` değişti). p20, kök ve pt1'e dokunulmadı.

| Kontrol | Sonuç |
|---|---|
| EditMode | 598 geçti, 0 kaldı, 2 atlandı |
| `--local` | **4/4**; başlık–çerçeve 9 pt, kart altı 593 pt (önceki ile aynı). `art=eski` ile `card` da GEÇTİ, 9 pt |
| Canlı p21, bayraksız | **4/4**; PC'de medyan 59,9 FPS |
| Canlı vitrin `?lv=SHOW_ART` ve `&art=eski` | İkisi de açıldı, istisna 0 |
| `--send --player TEST_PM53` | 5/7. KALDI: `a6b` (PRODUCT.md'de eski KALDI) ve `zeydsend` (`zorluk_puani` bekliyor; oyun sonu puanı PM_41 ile kaldırıldı). İkisi de bu işten değil; kontrol beklentileri güncellenmeli |

**Card:** Ölçülen boşluk tanıtım kartında değil, kazanma kartında ("Picture complete!" ile resim çerçevesi) çıkıyor. Resim 2 pt aşağı alındı, altındaki boşluk 2 pt azaltıldı; yazı, kart boyu ve renk aynı.

**Kayıtlar (canlı p21, 390×844 tahta bölgesi):**
- [Kilit açılma](entegrasyon/kilit_acilma.gif): açılma ve düşme yaklaşık 0,5 sn, kareler görüldü.
- [Zincir yaylanma](entegrasyon/zincir_yaylanma.gif): uzun zincirde sürükleme ve bırakma.
- [Çekiçle kopma](entegrasyon/zincir_kopma_cekic.gif): yarım halkalar orta noktada ayrılıp yaklaşık 0,2 sn'de kayboluyor.

**FPS:** Vitrinde `&fps=1` sol üstte, HUD'un altında "FPS x min y" gösterir (saniyede bir güncellenir; varsayılan kapalı). Telefonda ölçüm Zeyd'de.

## Ek — PM_55 (9 Kasım, vitrin geri bildirimi)

**Yayın:** gh-pages `6017ad93`, yalnız `p21/` değişti. Kaynak değişiklikleri çalışma alanında, commit edilmedi. EditMode 598 geçti, 0 kaldı, 2 atlandı. `verify_build` yerelde ve canlıda 4/4. Canlı vitrin `?lv=SHOW_ART` ve `&art=eski` istisna 0. Telefonda FPS ölçülmedi.

Önce/sonra: [önce](entegrasyon/vitrin_baslangic.png), [sonra](entegrasyon/vitrin_pm55_sonra.png).

| Madde | Yapılan |
|---|---|
| 1. Bomba | 3-2-1'de sürekli nabız (%8, 0,6 sn), üstüne hamle pump'ı ve titreme. ≥4'te %3 / 1,2 sn aynı. Fitil ateşi 1,8 kat, kıvılcım noktası sabit kalacak şekilde; arkasında sarı→turuncu additive hale |
| 2. Zincir sarkması | max(0,22 hücre, boyun %10'u), üst sınır 0,45 hücre; dikeyde yön çarpanı korundu, dikey düz kalıyor. Yaylanma aynı |
| 3. Yüksek durma | Neden: `ChainedBoost` tüm parçayı gölgesiyle komşularının üstüne çiziyordu. Yeni görselde kaldırıldı; zincir ve bağlantı noktaları parçaların üstünde, duvarların ve öğretici elin altında. Parça sürüklenirken zinciri de yükselir |
| 4. Zincir kalınlığı | Halka 1,6 kat, aralık 0,352 hücre; 2 hücreye kadar zincirlerde 0,28 hücre, yani daha çok halka |
| 5. Kilit | Dokunulmadı |

**Doğrulanmayan:** Telefonda görünüm ve FPS. Hale rengi PC tarayıcısında görüldü, iPhone Safari'de görülmedi.
