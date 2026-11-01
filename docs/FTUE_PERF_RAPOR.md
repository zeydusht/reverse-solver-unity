# İlk açılış (FTUE) ve performans incelemesi — PM_MESAJ_28 B (2 Kasım 2026)

Build: p20 önizleme `p20-2026-11-01b` + yeni eğitim. Ölçümler yerel kopyada (`Builds/publish/br`, 127.0.0.1), başsız Chrome, 390×844, dokunmatik simülasyon. Görüntüler `docs/qa/2026-11-02-ftue/` (67), ölçümler `docs/qa/2026-11-02-perf/`. Komutlar: `node Tools/verify_build.js --local Builds/publish/br --only ftue --part 1|2|3` ve `--only perf --net 0|1|2 --fps 0` / `--net 9` (yalnız FPS).

Turun kapsamı: yükleme → isim → profil sorusu → menü → eğitim (Bölüm 1) → Bölüm 2–6 → bombalı kayıp (S50, `Logs/veri/bomb_loss.json`; Bölüm 1–6'da hazır kayıp çözümü yok) → menü sekmeleri, Ayarlar, Bölümler. **Eksik:** kayıp sonrası 1–10 cevabı ve "Bölümler'den tekrar oyna" (2. ve 3. parça 5 dk sınırına takıldı / haritada düğüm bulunamadı, betik Koleksiyon'a bastı). Her bölüm sayfa yenilemesiyle açıldı (betik kolaylığı); gerçek akışta koridor geçişi araya girer.

## 1. Tıkanma listesi

| # | Adım | Ne oldu | Neden sorun | Önerilen çözüm | Önem |
|---|---|---|---|---|---|
| 1 | Kazanma kartı (her bölüm) | En üstteki büyük tuş **"Geç"**, "Sonraki bölüm" onun altında (`09_b1_kazanma`) | Yeni oyuncu ilk tuşa basar; "Geç" yalnız 1–10 sorusunu geçiyor ama "bölümü geç" gibi okunuyor | "Geç"i küçük metin bağlantısı yap ya da 1–10 satırının yanına al; ana tuş "Sonraki bölüm" en üstte | yüksek |
| 2 | 1–10 sorusu | 10 kutu yan yana, her biri ≈ 24 pt | 44 pt altı: parmakla yanlış sayıya basma, veri gürültüsü | 2 satır × 5 (≈ 44 pt) ya da kaydırıcı | yüksek |
| 3 | Bölüm 3 (çivi) | Kart: "Sayı **her hamlende** bir azalır"; yeni spotlight: "Diğer parçalar **çıktıkça** çivi sayacı azalır" | Aynı kural iki farklı cümle; PM'in metni ("birkaç hamle sonra açılır") Codex tarafından oyun koduna göre değiştirildi | Tek cümlede birleş (PM karar) | orta |
| 4 | Bölüm 4 açılışı | "Level 4 · Jiggle'ları kurtar!" kartı, ortada çivi resmi, "Başla" (`34_b5_kart` öncesi, `31_b4_kart`) | Türkçe oyunda İngilizce "Level 4" başlığı; çivi resmi bir şey anlatmıyor; neden bu kart çıktığı belirsiz | Kartın amacını netleştir (zor bölüm uyarısı mı?) ya da kaldır | orta |
| 5 | Eğitim kural kartı | Mini çizimde tik var, **kırmızı çarpı görünmüyor**; oklar soluk (`07_b1_kural_karti`) | "Yolu kapalı = olmaz" mesajı çizimden okunmuyor | Çarpıyı büyüt/çiz, okları koyulaştır | orta |
| 6 | Eğitim adım 1 | Parça zaten sol kenarda; ok kısa, krem α .55, elin arkasında kalıyor | Yön yazıdan ve elden anlaşılıyor ama ok zayıf | Oku daha opak/kalın yap | düşük |
| 7 | Bölüm 3–4 süreleri | 3: 1:00 (betik 32 sn artırdı), 4: 0:55 (betik 12 sn, 2 yıldız) | İlk 5 bölümde süre sıkı; yeni oyuncu kaybedebilir | pt1 verisiyle bak (PM) | orta |
| 8 | Sonuç / tanıtım kartı yazıları | "109 saniye kaldı, 0 kez takıldın", "Ne kadar zordu?…", kart gövdeleri ≈ 9–10 pt | Telefonda zor okunur | Gövdeyi ≥ 13 pt | orta |
| 9 | İsim ekranı | "Playtest sürümü. Adını yaz, sonuçlar bu isimle kaydedilsin." | Sorun değil; yalnız "Playtest" İngilizce | — (metin, PM) | düşük |
| 10 | Kayıp kartı | "Bomba patladı" + 3 boş yıldız + 1–10 | Boş yıldızlar kayıpta kafa karıştırabilir | Kayıpta yıldızları gizle | düşük |
| 11 | Bölümler haritası | Avatar şu anki düğümde, kilitli düğümlerin numarası altta; dokununca oynanacak düğüm belirsiz (betik bulamadı) | Haritadan oynama keşfedilmeyebilir | Şu anki düğümde "Oyna" balonu | düşük |

Çalışan ve sorunsuz görünenler: eğitim adım 1–2 (yön çözümden, el ve yazı doğru: sola / aşağı), örtü ~%60 (ölçüm: tahta üstü parlaklık 119 → 48), kural kartı dokununca kapanıyor, süre eğitim bitene kadar 2:00'de duruyor, çivi (B3) ve bomba (B6) spotlight'ları, Ayarlar anahtarları, sekmeler.

## 2. Okunabilirlik (375×667)
12 pt altı: sonuç kartı istatistik/soru satırları (≈ 9–10 pt), tanıtım kartı gövdesi (≈ 9 pt), Ev'de "Bölümler" etiketi (11 pt), Başarılar koşul satırı (13 pt ama sığmak için ≈ 8–9 pt'ye küçülüyor: "Bombalı 10 bölümü ilk denemede kazan"), profil istatistik etiketleri ("Ortalama kalan süre" küçülüyor), sürüm yazısı (12 pt, kabul). Kontrast: kural kartı okları (krem α .55 krem üstünde) ve eğitim oku düşük; tanıtım kartındaki turuncu ipucu satırı krem üstünde düşük.

## 3. Dokunma alanı (< 44 pt)
1–10 kutuları (≈ 24 pt) · kart kapatma X (36 pt) · Bölümler geri tuşu (38 pt) · Ayarlar anahtarları (65 × 36 pt) · Jiggles avatar daireleri 375'te ≈ 58 pt (uygun) · Bölümler tuşu 44 pt (sınırda).

## 4. Performans

| Ölçüm | Hızlı 4G (9 Mb/s, 60 ms) | Yavaş 4G (1,6 Mb/s, 150 ms) | 3G (0,75 Mb/s, 300 ms) |
|---|---|---|---|
| Açılıştan menüye (CPU ×4) | 17,9 sn | 83,8 sn | 175,6 sn |
| Menüden bölüm 1 oynanabilir | 0,73 sn | 0,85 sn | 0,86 sn |

- Toplam indirme: **15,37 MB** (data 9,63 · wasm 5,18 · yükleyici PNG/JS < 0,5). Tablolar ayrı ve isteğe bağlı iner, bu sayıya dahil değil.
- En büyük 10 dosya: data.unityweb 9,63 MB · wasm.unityweb 5,18 · v3-loader-bg.png 0,20 · loader.js 0,11 · icon-v3-192 0,09 · framework.js 0,07 · icon-192 0,04 (×2) · küçük ikonlar.
- FPS (PC, gerçek GPU, iPhone değil): Bölüm 1 59,9 · Bölüm 30 59,9 · koridor 59,9 · koleksiyon kaydırma 59,9 medyan, **en kötü kare 12 FPS** (≈ 83 ms tek takılma; büyük olasılıkla görünür olan tablonun ilk çözülmesi).
- Bellek tepe: JS yığını 7,2 MB, wasm belleği 114 MB.

### Uygulanan kesin kazanç
- Yeni başlık fontu (Nunito Black, `Fonts/Title`) atlası 2048×1024 → **1024×1024** (örnekleme 72/14 → 56/11; dolgu/boy oranı aynı, kontur görünüşü değişmez). Ham 2,1 MB → 1,05 MB; Brotli'li indirmede yalnız **−79 KB** (data 10,10 → 10,02 MB; SDF zaten iyi sıkışıyor).

### Öneriler (uygulanmadı)
| Öneri | Beklenen kazanç |
|---|---|
| `hud-time-pill(-urgent)`, `hud-moves-pill` (3 × ~300 KB ham): en büyük boyutu 2× ekran ölçüsüne indir + crunch | ~0,5 MB · Yavaş 4G ~2,5 sn |
| `kit-overlay/flat/mask` (~1 MB ham): kullanım ölçüsüne indir / crunch | ~0,4 MB · ~2 sn |
| Menü sahneleri (`t20-scene-*`, `levels-path-repeat`, ~1,4 MB ham) ilk karede gerekmiyor: tablolar gibi sonradan indir | ~1 MB · Yavaş 4G ~5 sn, 3G ~11 sn |
| `Lilita` (524 KB ham) ve `level-cell-*` (4 × 85 KB) yalnız eski (Art kapalı) menüde: kök Build 5 yeni sete geçince build'den çıkar | ~0,4 MB |
| Yükleme ekranında ilerleme yüzdesi + "ilk açılış ~1 dk" metni (3G'de 3 dk boş bekleme) | algılanan süre |
| wasm 5,2 MB: kullanılmayan motor modüllerini kapat (Physics, AI vb. kontrol) | 0,3–0,8 MB (ölçülmeli) |

Toplam gerçekçi hedef: −2,5…3 MB → Yavaş 4G ~70 sn, 3G ~145 sn. Asıl sıçrama için menü sanatının sonradan indirilmesi gerekir.

## 5. Veri — açık sorun (yüksek)
Canlı `?c=ic` testinde (IC_P20T2, IC_P20T3, iki koşu da aynı): Bölüm 1'in **ilk denemesi kazanıldı, 9 hamlesi `moves`'a gitti, ama `plays` satırı gelmedi**; ikinci deneme `attempt 2` olarak geldi (sayaç doğru). İlk denemenin satırı 1–10 cevabını bekliyor ve sayfa kapanınca (gizlenme) gönderilmesi gerekiyor; 27-B build'inde aynı test bu satırı sayfa değişiminde gönderiyordu (IC_P20B, 21:55). Eğitim kodunda bu yola dokunan bir değişiklik bulunamadı; neden çözülmedi. Etki: 1–10'u cevaplamadan sekmeyi kapatan oyuncunun ilk kazanması kaybolabilir. Yayın (pt1 davetleri) öncesi çözülmeli.

## 6. Veri (A5)
Eğitim hamleleri normal `moves`/`plays` satırı olarak gidiyor; eğitimde yanlış parça / yanlış yön session'a hiç gönderilmiyor (jam sayılmaz). `plays`'e "eğitim gösterildi" sütunu eklenmedi — gerekirse PM şeması.
