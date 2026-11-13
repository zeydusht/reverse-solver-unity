# PM_MESAJ_65 — Gece işi: stil denetimi + düzeltme (PM_64'ten sonra, Zeyd'i BEKLEMEDEN)

Zeyd uyuyor. PM_64 bitince **onay beklemeden** buna geç. Kodu kendin yaz, `codex exec` kullanma. Yalnız p22'de çalış (p20, p21, kök, pt1'e dokunma). Bitince dur ve rapor yaz.

## Amaç
Oyunda **onaylı tarzımıza uymayan** her şeyi bul ve düzelt. Zeyd'in örneği: **1. bölüme eski eldivenli el (tutorial eli) gelmiş, Zeyd sevmiyor.** Eski el ve büyük oklar hiçbir akışta çıkmamalı (PM_60 kararı). İpucu yalnız: altta Jigo'lu krem-mavi sohbet kutusu + parçada altın parlama.

## Onaylı tarz (ölçüt)
- HUD: old money mavi üst/alt şerit (üst koyu → alta RM mavisine geçiş), altın/turuncu bordür + gölge; Parça/Süre krem kutuları, yarı gömülü başlık + kadife lacivert sarkan flamalar; ortada kemerli Jigo penceresi; alt şeritte 4 yeşil katmanlı booster + Durdur (mavi). Rakamlar ortalı, gövdenin ~%34'ü. Seviye numarası HUD'da yok.
- Booster ikonları: TASK36 hâli (kontursuz, doygun, yeşil diskin içinde), mavi çekiç, kahverengi saat.
- Engeller: siyah bomba (nabız), **Kont mühürlü kilit** (beyaz sayı), p20 tarzı kalın zincir + Verlet fizik, duvar aynı.
- Başlıklar: kurdele/afiş YOK, kalın 3B yazı (krem dolgu, mavi kontur, altın kalınlık).
- Paneller: krem gövde, mavi çerçeve, iç bevel. Kartlar (bomba/kilit/zincir) T45 dili.
- Jigo: onaylı `Jigo_v3` (turkuaz, gümüş yüksük, mavi uçlu fırça, düğme kalkan). Normal/mutluda **kaş çatmaz**.
- Efektler: opak, doygun, büyük; soluk sis yok.
- Durdur: ortadaki büyük panel (tahtayı kapatır).
- Yazı tipi: Baloo 2 ExtraBold / Fredoka; LilitaOne yalnız İngilizce etiketler.

## Yöntem
1. Bütün akışı ekran görüntüsüyle gez (390×844, TR ve EN): açılış videosu → menü/koridor → harita → bölüm 1, 2, 3 (kilit dersi), 10 (zincir), 14 (duvar), 20 (bomba), bir zor bölüm → her booster seçim modu ve animasyonu → kombo x2–x5 → kazanma → sandık → süre doldu / bomba devam paneli → kaybetme → Durdur → ayarlar → profil/Jiggle ekranı → tanıtım kartları. Bayraklar: `&test=1`, `&booster=…`, `&kazan=1`, `&sandik=1`, `&devam=1`, `&jigo=…`.
2. Her ekranı yukarıdaki ölçüte göre işaretle. Özellikle ara: **eski el/ok**, eski turkuaz/krem HUD parçaları, eski çivi görseli ya da "çivi/nail" metni, eski Baron adı (artık **Kont**), kurdele başlıklar, soluk efektler, eski booster ikonları, pembe çekiç, kaş çatan Jigo, ortalanmamış rakamlar, taşan ikonlar, Türkçe/İngilizce karışık metin, yazım hataları.
3. **Kodla düzeltilebilenleri düzelt** (eski görseli onaylı olanla değiştir, gizle, metni düzelt). Onaylı bir görseli yoksa ve çizim gerekiyorsa `docs/art/CODEX_TASK48.md`'ye madde ekle (**Codex'i çalıştırma**, Zeyd sabah verir).
4. Emin olmadığın tarz kararlarını değiştirme; listeye "Zeyd'e sor" diye yaz.

## Teslim
- `docs/art/STIL_DENETIM_14KASIM.md`: tablo (ekran | sorun | ekran görüntüsü | düzeltildi mi / Codex'e mi / Zeyd'e sor). Önce/sonra görüntüleri `docs/art/stil_denetim/`.
- p22 güncel, EditMode (Perf hariç) yeşil, `verify_build` p22 4/4. `ENTEGRASYON_RAPOR.md`'ye "PM_65" eki, PROJE_DURUMU'na tek satır, commit + push. Sonra **dur**.
