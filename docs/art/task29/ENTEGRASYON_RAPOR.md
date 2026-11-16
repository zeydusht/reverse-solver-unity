# PM_MESAJ_54 — Entegrasyon raporu (10 Kasım)

**Durum: p21'de yayında.** gh-pages `b6370160`, yalnız `p21/` değişti. p20, kök ve pt1'e dokunulmadı. Kaynak değişiklikleri commit edilmedi.

- Normal oyun: `https://zeydusht.github.io/reverse-solver-unity/p21/?lang=tr`
- Vitrin: `https://zeydusht.github.io/reverse-solver-unity/p21/?lv=SHOW_ART&lang=tr` (menüde Demo)
- Test bayrakları: `&jigo=tedirgin|korku|uzgun|mutlu|sevinc`, `&devam=1`, `&coin=5000`, `&devamhayir=1`

## Önce ölç (bölüm 5)

| Soru | Sonuç |
|---|---|
| Altın var mı? | Yoktu. Jiggle toplamı harcanabilir para değil. Eklendi: başlangıç 1000, kazanılan her bölüm +50 |
| 40 bölümün süresi (SET40 v4) | En kısa 50 sn, medyan 65 sn, en uzun 170 sn; 8 bölüm 60 sn'nin altında |
| Joker (saat) | +20 sn |

**Öneri (+20 sn kuralı):** 60 sn altındaki bölümlerde +20 sn kalsın. En kısa bölüm 50 sn; +30 sn sürenin %60'ı olur, +20 sn %40 ve Joker'le aynı.

**PM'in bakması gereken:** 900'lük ilk devamdan sonra bakiye 100'e düşer; ikinci devam için 16 bölüm kazanmak gerekir. Tekrar oynanan bölüm de +50 verir (mesajdaki "her kazanılan bölüm" olduğu gibi uygulandı).

## Ölçümler

| Kontrol | Sonuç |
|---|---|
| EditMode | 624 geçti, 0 kaldı, 2 atlandı (626) |
| `verify_build` yerel | 4/4 |
| `verify_build` canlı p21 | 4/4; PC'de medyan 59,9 FPS |
| `--send --player TEST_PM54` | 5/7; KALDI yalnız bilinen eski `a6b` ve `zeydsend` |
| Vitrin, canlı, `art=eski` dahil | İstisna 0 |
| Build (Brotli) | 17,04 → 18,89 MB (+1,85 MB) |
| Telefonda FPS | Ölçülmedi, Zeyd'de (`&fps=1`) |

`verify_build` kontrolleri görselsiz setleri kullandığı için yeni HUD'u ölçmüyor. Yeni HUD ayrıca vitrinde ekran görüntüsü ve oynanışla denendi.

## Yapılanlar

- **Çekiç:** final listedeki `HUD36` mavi çekiç, katmanlı gövdeyle.
- **Üst/alt şerit:** ölçüler `HUD_ANALIZ_RM.md`; 5. yuva Durdur. Durdur menüsü: Seviye N, Devam, Ses, Çık. Süre ≤10 sn'de mercan + nabız. `art=eski` eski düzen. [Ölçü karşılaştırması](../task30/karsilastirma.png): çizgiler üst şerit, panel, pencere ve alt tuşlara oturuyor. Yalnız ölçüler alındı; görseller ve karakter bizim.
- **Jigo:** korku, tedirgin, mutlu (0,6 sn), normal, üzgün; 0,15 sn geçiş; eşikler `JigoMood`'da tek yerde.
- **Kilit:** Kont'un mührü, açılma kareleri + kodla düşüş (yerçekimi, ±25°, 0,45 sn, son %30 solma) + mum kırıntıları. Boyut hücrenin %55'i.
- **Zincir:** eski zincirden çıkan halka (`Obstacles_v3`). Sarkma PM_55 kuralı; alt sınır 0,22'de bırakıldı (0,28 için Zeyd'in yeni geri bildirimi yok).
- **Kazanma:** büyük Jigo, ışınlar, konfeti, ~2 sn, dokunmayla geçiliyor; sonrası aynı.
- **Devam:** süre veya bomba kaybında panel; 900 → 1900 → 2900, sonra teklif yok; yeni denemede 900. 60 sn altı bölümde +20 sn. Bomba: sayaç 5'e döner, süre eklenmez. Altın yetmezse tuş pasif.

## Telemetri (tahmin modeli)

- `plays` tablosuna yeni sütun eklenmedi; olmayan sütun satırı reddettirirdi. Devamla kazanma `result = 'won_after_continue'` olarak gidiyor. `v_level_actual`, `v_pred_vs_actual`, `v_model_score` yalnız `'win'` saydığı için bunları kazanma saymıyor; SQL uygulanmasa da model korunuyor.
- Kayıp, oyuncu karar verene kadar ertelenir. Sayfa kapanırsa kayıp yine yazılır, teklif geri çekilir.
- Devam olayları ayrı `continue_events` tablosuna gider: bölüm, deneme, kaçıncı devam, fiyat, sonuç. **`docs/sql/015_continue.sql` yazıldı, uygulanmadı (PM uygular).** Uygulanana kadar bu satırlar gönderilemez, `plays` etkilenmez.
- Ayrıntı: [JOB_C_REPORT.md](JOB_C_REPORT.md).
- Yıldızlar eklenen süreyle şişmez; devamla kazanmada hesap özgün süreye göre yapılır. Devamla kazanma ilk deneme kazancı sayılmaz.

## İş bölümü

Codex: keşif ve ölçümler, A (HUD, Durdur, Jigo), B (kilit, zincir, bomba kıvılcımı, kazanma), C (devam, altın, telemetri, SQL, testler). Claude Code: bölme, inceleme, tarayıcı denemeleri, düzeltmeler, build, yayın, bu rapor.

Tarayıcı denemesinde bulunup düzeltilenler: High stripping `SpriteMask`'i build'den atıp HUD'u kırıyordu, Jigo kareleri kemer şekline önceden kırpıldı; krem üstüne krem rakamlar başlıkların konturlu stiline geçirildi.

## Sapmalar ve eksikler

- Vitrin bölümü (6×8) 375×667'de 1,6 pt taşıyordu; yalnız bu durumda hücre 50 → 49. Oyun setinde küçülme hesaplanmadı.
- Çentikli ekranlarda üst şerit %19'u biraz aşabilir (sabit paneller + güvenli alan).
- `&jigo=sevinc` önizlemesi tanıtım kartının arkasında kalıyor. Kazanma ekranı gerçek bir kazanmayla (SHOW1) doğrulandı.
- **Doğrulanmadı:** telefonda FPS ve iPhone Safari görünümü; kilit düşüşünün akıcılığı (istisnasız çalıştı, kare kare izlenmedi); gerçek süre kaybı (panel `&devam=1` ile, satın alma gerçek bomba kaybıyla denendi); `continue_events` gönderimi (tablo yok).

## Ekran görüntüleri

[Normal](ekran/normal.png) · [Tedirgin](ekran/tedirgin.png) · [Korku](ekran/korku.png) · [Mutlu](ekran/mutlu.png) · [Üzgün](ekran/uzgun.png) · [Devam, süre](ekran/devam_time.png) · [Devam, bomba](ekran/bomb_panel.png) · [Durdur](ekran/durdur.png) · [Kazanma](ekran/kazanma.png) · [375×667](ekran/normal_375.png)

## PM_56/57 eki (10 Kasım)

**Durum: p21'de yayında**, gh-pages `82a62ff1`, yalnız `p21/` değişti. Kaynak `art` dalında commit + push.

| Kontrol | Sonuç |
|---|---|
| EditMode | 641 geçti, 0 kaldı, 2 atlandı (ip testleri 16 yeni, altın tamamlama 1 yeni) |
| `verify_build` yerel / canlı p21 | 4/4 / 4/4 |
| Vitrin FPS (zincirli parça sürüklenirken, PC gerçek GPU) | medyan 59,9, en düşük 58,5 |
| Vitrin kayıtları | istisna 0 |
| Build (Brotli) | 18,89 MB (değişmedi) |

**PM_56**
- Kilit hücrenin ~%66'sı. Açılınca yerçekimiyle hızlanıp ekranın altına düşüyor, alt şeridin arkasından geçip kayboluyor (sıra 2600, HUD 3000). ±25° dönme, solma yok.
- Bağlantı noktaları %60 büyük, halka %15 kalın.
- Bomba ≥4: %5,5 sinüs nabız, 0,9 sn; hamlede %3 pump. 3-2-1 değişmedi.
- Zincir: Verlet ip (`Board/RopeSim.cs`, ayarlar `ChainTuning`'de tek yerde). 8–16 düğüm, 120 Hz sabit adım, 20 kısıt turu, uçlara bağ (esneme yok), yerçekimi 18 hücre/sn² (~1 Hz salınım), sönüm 0,98/60 Hz. Duruş sarkması yatay 0,30, çapraz 0,35, kısa en az 0,28 hücre; kurulumda ölçülüp boy ayarlanıyor. Sürüklerken sallanıp duruluyor; yanlış hamlede dürtü; gerilince düz + hafif titreme; tahta çerçevesinin içinde kalıyor; çekiçte ortadan ayrılıp düşüyor.
- GIF'ler: [kilit düşüşü](pm56/kilit_dusus.gif) · [sürükleme-sallama](pm56/zincir_surukle_salla.gif) · [yanlış hamle](pm56/yanlis_hamle_sarsinti.gif) · [çapraz duruş](pm56/capraz_zincir_durus.gif)

**PM_57**
- Altın: başlangıç 5000; bakiyesi 5000'in altında olan eski oyuncu bir kereliğine 5000'e çıkıyor (`coins_topup57` işareti). Vitrinde panel 5000 gösterdi, 900'lük devam alındı, sayaç 5'e döndü.
- **`continue_events` gönderimi çalışmıyor:** istek gidiyor, Supabase **401** dönüyor (edge log 12:00 UTC). Sebep: istemci `?on_conflict=row_id` ile yazıyor (yeniden denemede çift satır olmasın diye, `plays` ile aynı), Postgres bunun için `row_id`'ye SELECT yetkisi istiyor; anon'da yalnız INSERT var. Çözüm `docs/sql/016_continue_select.sql` (tek satır, sütun düzeyinde yetki, RLS yüzünden hiçbir satır okunamaz). **Uygulanmadı, Zeyd'in "çalıştır" onayı gerekiyor.** 011 (`intro_events`) dosyasına da aynı satır eklendi. Ayrıca: vitrin (`SHOW_ART`) hiç satır yazmıyor; sınama gerçek bir sette yapılmalı.
- Açılış sesi C: A'nın kendi sesleri temel, üstüne B'den CC0 efektler (A yüksekken kısılıyor), altta −18 dB müzik kutusu; Kont sahnesinde gerilim, sonda neşeli kapanış. Oyunda varsayılan C (A/B araçla yeniden üretilebilir). Önizleme `p21/onizleme/intro_ses_C.mp4`. Lisanslar `docs/sounds/LISANS.md`.

**Doğrulanmadı:** telefonda FPS ve iPhone Safari; `continue_events` satırı (016 bekliyor); intro C'yi kulakla dinlemedim (Codex karışımı ölçtü).

## PM_58 eki (10 Kasım akşam): fiske + kenar payı

**Durum: p21'de yayında**, gh-pages `db178a29`, yalnız `p21/`.

| Kontrol | Sonuç |
|---|---|
| `verify_build` yerel / canlı p21 | 4/4 / 4/4 |
| Fiske, klasik set (L01, L40; çözümün ilk 5'er hamlesi, 0,45 hücre / 50 ms) | 10/10 parça çıktı, istisna 0 (yerel ve canlı) |
| Fiske, vitrin | 10 fiske, istisna 0 (yerel ve canlı) |
| EditMode | 655 geçti, 1 kaldı, 2 atlandı. Yeni fiske testleri (29) geçti. Kalan `FunSimV5Tests.HardestLevelStillUnderTwoSeconds` |

**Süre testi:** yalnız Core'u ölçüyor, PM_58'de Core'a dokunulmadı. Sonuç koşudan koşuya 2,3 ile 5,3 sn arasında oynuyor (sınır 2 sn); PM_57'nin tam koşusunda geçmişti. Makine hızına bağlı bir test, PM'in kararına bırakıldı (sınırı gevşetmek ya da başka makinede ölçmek).

**Fiske** (`Board/FlickGesture.cs`)
- Ölü bölge 8 pt'nin altı. Yön baskın eksene kilitli; iki eksen arasındaki fark ≤%20 ise hamle yok, parça geri döner.
- Eşik min(0,3 hücre, 18 pt); hız ≥600 pt/sn ya da parmak kalkınca tetiklenir.
- Hamle eski sürüklemeyle aynı kod yolundan işleniyor: kural, sayaçlar ve takılma aynı. Parça hızlanarak kendi kayıyor; zincir aynı fizikle tepki veriyor. Uzun sürükleme de çalışıyor.
- Telemetri: `moves` tablosunda serbest metin ya da json alanı yok, sütun eklenmedi. Fiske/sürükleme sayısı yalnız günlükte (`[input] flick=N drag=M`). Tabloya yazılsın mı, **PM kararı**.

**Kenar**
- Tahtanın yatay payı en az 24 pt: `G = min(34, max(24, (W − 44·sütun)/2))`, gerekirse hücre küçülüyor. 390 pt genişlikte 8 sütunlu tahta 19 → 24 pt.
- Kenar hücreler: tahtanın 12 pt dışına kadar dokunma kenar hücreye sayılıyor.
- Sayfa: tuvalde `touch-action: none`, `overscroll-behavior: none`, tuvaldeki dokunmalarda `preventDefault`. İki parmakla yakınlaştırma her yerde engelli; HTML katmanları (isim kapısı, intro) dokunmayı alıyor.
- **iPhone'da sınanmadı:** Safari'nin kenardan geri kaydırması sistem hareketi; sayfa tarafından tamamen engellenemeyebilir.

GIF'ler: [fiskeyle çıkış](pm58/fiske_cikis.gif) · [kenar parça (zincirli)](pm58/kenar_parca.gif) · [yanlış yön, takılma](pm58/yanlis_yon_takilma.gif)

## PM_59 eki (10 Kasım gece): yeni onboarding

**Durum: p21'de yayında**, gh-pages `be7722ee`, yalnız `p21/`. Yeni oyuncu linki: `p21/?set=redesign&c=p21&reset=1&lang=tr` (`reset=1` ilerlemeyi, tutorial/intro bayraklarını ve altını sıfırlar; yalnız `c=p21`).

| Kontrol | Sonuç |
|---|---|
| EditMode (Perf hariç) | 666 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel / canlı p21 | 4/4 / 4/4 |
| Tutorial baştan sona (yerel), vitrinde kilit ve kart | istisna 0; canlıda yeni akış açılıyor |

**PM_58 kararları:** `HardestLevelStillUnderTwoSeconds` → `[Category("Perf")]`. `unity_batch.sh tests` onu dışarıda bırakıyor, `tests Perf` ayrı koşuyor. `MoveRow.INPUT_COL = false`: kapalıyken JSON birebir eskisi gibi; açılınca `input=flick|drag` gider.

**Bölüm 1–2:** yalnız `c=p21` + redesign setinde ve yeni oyuncuda (hiç bölüm bitirmemiş ya da tutorial'a başlamış), setin önüne iki yeni bölüm ekleniyor: `P21_T01` (2×2 tutorial) ve `P21_E01` (3×3, engelsiz, 180 sn). 40 bölümlük dosyalar ve kimlikler değişmedi; mevcut oyuncular eski akışta kalıyor.
- Tutorial: çayır resmi, süre durdurulmuş, kayıp yok. Hedef dışı parçaya dokunma = titreme, hamle sayılmıyor. Adımlar spesifikasyondaki gibi: C↓ → B← takılma + açıklama → B→ → A → D → kazanma.
- **Sapma (adım 3):** C ve B çıktıktan sonra A dört yönde de çıkabiliyor, yani "yanlış yön" denemesi mümkün değil; her yön kabul ediliyor. Gerçek bir yanlış yön için başka bir yerleşim gerekir, PM kararı.
- Adım telemetrisi `tut_step` → `tutorial_events`. **`docs/sql/017_tutorial_events.sql` yazıldı (`row_id` için SELECT dahil), uygulanmadı.**
- İlk 3 bölümde 1,5 sn üst bant "Bütün parçaları tahtadan çıkar!". Parça sayacı her çıkışta "pop" yapıyor. 2. bölümde balon "Önce önü açık parçaları çıkar."

**Kartlar ve kilit** (p21 + yeni görsel): bomba, kilit ve zincir kartlarında yeni TR/EN metin, anahtar kelimeler kalın, daha büyük punto; bomba kartında 3→2→1 animasyonu. Kilitli parçayı itince kilit sallanıyor, sayacın çevresinde mercan halka, hamle sayılmıyor; oturumda ilk 2 kez "Kilit N hamle sonra açılacak." balonu.

**Yolda düzeltilen:** Codex'in elle yazdığı 23 `.meta` dosyası Unity'de okunamadı; görseller build'e girmedi (düz mavi parçalar, el, ok ve balon yok). Meta dosyaları silindi, Unity yeniden üretti.

**Doğrulanmadı:** iPhone; 2. bölümde ilk deneme kazanma oranı ≥%95 (çok sıralı ve çözülebilir olduğu testle doğrulandı, oyuncuyla ölçülmedi); `tutorial_events` gönderimi (017 bekliyor).

GIF'ler: [tutorial 4 adım](pm59/tutorial_4_adim.gif) · [kilit uyarısı](pm59/kilit_uyarisi.gif) · [bomba kartı](pm59/bomba_karti.gif)

## PM_60 eki (11 Kasım gece): tutorial geri alındı, booster gösterimleri, haz, kazanma ve sandık

**Durum: p21'de yayında**, gh-pages `cd5d6e0b`, yalnız `p21/`. (PM_61 iOS işi için arada durduruldu, `pm60-wip` dalında devam edildi.)

| Kontrol | Sonuç |
|---|---|
| EditMode (Perf hariç) | 689 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel | 4/4; PC FPS medyan 59,9 |
| `verify_build` canlı p21 | 4/4 (Pages dağıtımı bitmeden koşulan ilk deneme 2/4 verdi; eski ve yeni dosyalar karışıktı) |
| Tarayıcı denemeleri (hikâye kutusu, 4 booster, kombo, kazanma, sandık) | istisna 0 |
| `moves.input` (`TEST_PM60`, S43) | 4 fiske hamlesi `input=flick` ile tabloya düştü. `drag` satırı görülmedi (son uzun sürükleme hamle olarak kaydedilmedi), doğrulanmadı |
| Build (Brotli) | 19,35 → 26,22 MB (+6,9 MB) |

**Boyut:** İlk build 34,3 MB'dı. Yeni efekt kareleri sıkıştırılmamış RGBA32 olarak giriyor. Booster ve kazanma kareleri en fazla 384 px'e, büyük arka planlar 768 px'e, haz kareleri 512 px'e indirildi; ekrandaki boyutları (en çok ~260 pt) için yeterli. Boyut yine PM_59'dan 6,9 MB fazla; dokuların sıkıştırılması ayrı bir iş (iOS Safari'de DXT yok).

**1. Tutorial:** 2×2 ve kolay bölüm kapalı (`&tut2x2=1` ile açılıyor). 1. bölüm eskisi. Büyük el ve oklar kaldırıldı. Alt şeridin üstünde krem/mavi sohbet kutusu, köşede Jigo, yazı daktilo gibi akıyor. "Bu Jiggle'ları kurtaralım! Önü açık bir parçayı kaydır." Önü açık 1–2 parça altın nabızla parlıyor (en çok 3 kez). Son parçada "Bütün parçaları çıkarınca resim kurtulur!" Dokununca kapanıyor. 1. bölümde kayıp yok. Hedef bandı, kart metinleri ve kilit uyarısı PM_59'dan kaldı.

**2. Booster'lar** (`&booster=cekic|makas|degnek|saat`):
- Ortak seçim modu: tahta dışı kararıyor, üstte ikon + ad ("Mavi Çekiç!", "Sihirli Makas!", "Sihirli Değnek!") + talimat. Seçili tuş parlıyor, aynı tuşa tekrar dokunmak iptal ediyor.
- Çekiç: kavisle uçuyor, vuruyor, patlama + sarsıntı, parça düşüyor (denendi).
- Makas: kural değişmedi. **Makasın yaptığı:** iki ayrı parça arasındaki tek çıkıntılı birleşimi düzleştiriyor; zinciri kesmiyor, parça kaldırmıyor. Seçim modunda kesilebilir birleşimler turuncu noktayla gösteriliyor. Vitrinde (SHOW_ART) hiç uygun birleşim yok; bölüm 12'de denendi: makas uçuyor, kesiyor, çıkıntı düzleşiyor.
- Değnek, **kural değişikliği:** tahta 4 bölgeye ayrılıyor, oyuncu bölge seçip "Değiştir!" diyor, yalnız o bölgedeki çıkıntı yönleri değişiyor (parça yerleri aynı). Değişecek şeyi olmayan bölge gri, seçilemiyor. Vitrinde dört bölge de gri; bölüm 12'de denendi, çalışıyor.
- Değnek dengesi (FunSim, 40 bölüm, eşleştirilmiş 1000 oyuncu): tüm tahta → bölge: ilk deneme kazanma %41,5 → %41,8, keyif %53,3 → %53,8, hayal kırıklığı %45,2 → %44,6, 3 denemede kazanma %59,5 → %60,0. Fark yok denecek kadar küçük. Ayrıntı `pm60/wand_compare.txt`.
- Saat: seçim modu yok. Jigo kolunu Süre paneline uzatıyor, süre **duruyor** (20 sn, tek parametre), altta boşalan bar.

**3. Haz:** parça çıkışında patlama + parça renginde konfeti + pop, kilit/zincirde metal parça + kıvılcım, kombo (1,2 sn içinde art arda çıkış) ve övgü rozetleri (en çok 3 hamlede bir). Sesler mevcut kütüphaneden. **Kombo rozeti doğrulanmadı:** denemede ikinci çıkış ~1,6 sn sonra geldi.

**4. Kazanma ve sandık** (`&kazan=1`, `&sandik=1`): "Harika!", büyük Jigo, bölmeli sandık ilerlemesi ("2 bölüm sonra sandık!"), "44 Jiggle kurtarıldı!" ve yürüyen Jiggle'lar, "Sonraki Seviye". Sandık 3, 5, 10 ve sonra her 5 bölümde. 3 dokunuşla açılıyor, ödüller tek tek çıkıyor: +100 altın (5000 → 5100) ve 1 rastgele booster. Durdur menüsü eski orta panel.

**Küçük kusur:** makas seçim modunda alt şeridin üstünde eski ipucu metni soluk olarak yineleniyor.

**Doğrulanmadı:** iPhone, kombo rozeti, `drag` telemetrisi, sesler (başsız tarayıcıda dinlenmedi).

GIF'ler: [çekiç](pm60/cekic.gif) · [makas](pm60/makas.gif) · [değnek](pm60/degnek.gif) · [saat](pm60/saat.gif) · [patlama](pm60/kombo_patlama.gif) · [kazanma](pm60/kazanma.gif) · [sandık](pm60/sandik.gif) · [hikâye kutusu](pm60/hikaye_kutusu.png)

## PM_62 — Zeyd'in PM_60 geri bildirimi (11 Kasım)

**Durum: p21'de yayında**, gh-pages `738806cb`, yalnız `p21/`.

**Test linki:** https://zeydusht.github.io/reverse-solver-unity/p21/?c=p21&test=1&lang=tr — bütün bölümler açık, booster'lar açık ve 99 (azalmaz), altın 99999. Kayıtlar `cohort=ic`. Bayrak yalnız `c=ic` ya da `c=p21` ile çalışır.

| Madde | Yapılan |
|---|---|
| 1 Test modu | `LaunchPlan.TestMode`; `Progress.UnlockAll`, `Session.TestMode` (stok 99, harcanmaz), `SaveData.TestMode` (altın 99999, kaydedilmez), telemetri `cohort=ic` |
| 2 Kilitli booster | Dokununca "Seviye N'de açılır" balonu (~1,5 sn) |
| 2 Makas | gidiş 0,45 + kesim 0,3 + çıkış 0,45 = 1,2 sn (`P21BoosterView` sabitleri) |
| 2 Değnek | Seçim modunda süre akıyor |
| 2 Saat | Yeni akış: tek parça fırça uzatma (T46, 6 kare), ışıltı, buz kaplama + mavi-beyaz rakam, 10 sn bar, buz kırılma, geri çekme. `FreezeSeconds = 10` |
| 3 Kilit | %15 büyük, beyaz sayı + kalın koyu kontur, büyük punto |
| 4 Kombo | Takılma (`Jammed`) komboyu anında sıfırlar |
| 5 Kazanma | Yürüyen Jiggle'lar kalktı; ikon + "N Jiggle kurtarıldı!" (sayarak, pop), Sonraki Seviye (yeşil, büyük) + Menü (küçük) |
| 6 Jigo | Göz kırpma 4–7 sn; 0,25 sn crossfade + esneme geçiş, T46 geçiş kareleri; sinüs nefes ±%2 ve ±1,5° sallanma; döngüler 12→20 fps |
| 7 Harita | Kendi bölümün görünmüyorsa "Seviye N ↑/↓" hapı (dokununca kayar) + en üste/en alta tuşu |
| 8 Kenar payı | 24 pt, tek parametre (`SafeMarginPt`) |

Çizimler Codex TASK46 (`Assets/Art/_yeni/T46/`, 65 kare, `docs/art/task46/onizleme.png`). Kod Codex + Claude kontrolü.

| Ölçüm | Sonuç |
|---|---|
| EditMode | 697 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel | 4/4; PC FPS medyan 59,9 |
| `--send --player TEST_PM62` | 5/7; KALDI yalnız bilinen eski `zeydsend` ve `a6b` (PM_54'ten beri aynı) |
| `verify_build` canlı p21 | 4/4 |
| Build (Brotli) | 26,22 → 30,82 MB (+4,6 MB, T46 kareleri) |

**Doğrulanmayan:** iPhone'da görünüm ve akıcılık; kilit sayısının 64 pt hücrede uzaktan okunurluğu (Zeyd telefonda bakmalı); Jigo'nun "RM'deki kral kadar canlı" olup olmadığı.
GIF'ler: [saat](pm62/saat.gif) · [makas](pm62/makas.gif) · [kazanma](pm62/kazanma.gif) · [Jigo geçişi](pm62/jigo_gecis.gif) · [harita hapı](pm62/harita_hap.gif)

## PM_64 — Codex devri birleştirildi → p22 önizleme + PM_63 + kombo merdiveni (14 Kasım gece)

**Durum: p22'de yayında** (gh-pages `8efbc22e`, yeni klasör `p22/`; p20, p21, kök, pt1 değişmedi). Kaynak dal `pm64`.
- Oyun: https://zeydusht.github.io/reverse-solver-unity/p22/?c=p22&lang=tr
- Test: https://zeydusht.github.io/reverse-solver-unity/p22/?c=p22&test=1&lang=tr

### 14 dalın birleştirme notları
Dallar `art` (PM_62) üstüne sıralı bir zincirdi; `codex/14` hepsini içeriyordu, çakışma çıkmadı. Kararlarla çelişenler bayrağa alındı.

| Dal | Ne getiriyor | PM_56–62 ile |
|---|---|---|
| 01 onboarding | 2×2 rehberli + 3×3 açılış, 3–10'da süre tabanı | **Çelişki** (Zeyd 2×2'yi reddetti) → varsayılan PM_60 hikâye bölümü; Codex'inki `&tut=2x2` |
| 02 controls | Fiskede eksen kilidi, yan güvenli alan ≥24 pt | Uyumlu (PM_58) |
| 03 engel dersleri | Kilit 3, zincir 10, duvar 14, bomba 20; dersler süresiz; 20'den önceki bombalar kalkar | Uyumlu ("yay") → **varsayılan açık**, `&dersler=0` kapatır |
| 04 parça çıkışı | Patlama tahta kenarında, yavaş oyuncuya yarıda övgü, "weee" | Uyumlu |
| 05 booster animasyonları | Değnek onayı dar ekranda, süresiz derste saat harcanmaz | Uyumlu |
| 06 HUD | Süresizde "Sınırsız", kazanınca mutlu Jigo, devam fiyatları | Uyumlu (PM_54) |
| 07 Jigo kıyafeti | Yalnız sanat dosyası | Oyuna bağlanmadı |
| 08 Kont | Yalnız sanat dosyası | Oyuna bağlanmadı |
| 09 açılış storyboard | Yalnız sanat dosyası | Oyuna bağlanmadı; onaylı intro videosu kalır |
| 10 HUD/booster envanteri | Belge | — |
| 11 PlayMode açılış testi | `OpeningFlowTests` | 2×2'yi varsayıyor; varsayılan değişti, **PlayMode bu gece koşulmadı** |
| 12 Jiggle sayacı | Kurtarma toplamı sürüm değişince korunur | Uyumlu |
| 13 | Doğrulama listesi | — |
| 14 Jigo/booster cilası | Sabit portre (şader penceresi), düğme kalkanlı saat, makas ~2,1 sn, hedef seçerken süre akar, zaman aşımı harcamadan iptal | **Çelişki** → varsayılan PM_62: canlı portre (nefes, geçiş kareleri), fırçalı saat, makas 1,2 sn. Codex'inkiler `&portre=sabit`, `&saat=kalkan` (sabit portreyi de açar), `&makas=yavas`. Süre akışı ve iptal herkes için açık |

### Bayrak listesi (p22)
`&test=1` (yalnız `c=ic|p21|p22`) · `&tut=2x2` · `&dersler=0` · `&saat=kalkan` · `&makas=yavas` · `&portre=sabit` · mevcutlar: `&booster=`, `&jigo=`, `&devam=1`, `&coin=`, `&reset=1`, `&fps=1`.
p22, kodda p21 ile aynı oyun ailesi (`TelemetryMeta.P21Line`); kayıtlar `cohort=p22` gider, test modunda `ic`.

### PM_63 — zincirli parça takılması
- **Sebep:** zincirli çift çoğunlukla yandan "kayma" kuralıyla duruyor (yumuşak engel). Yumuşak engelde sürükleme payı hücrenin %30'uydu, parça yarım hücreye yakın kayıp boşluk açıyordu. Kural motoru zaten geçersiz sayıyordu; yalnız görünüm kayıyordu.
- **Düzeltme (`DragModel`):** hiç ilerleyemeyen zincirli çift artık normal parçadaki gibi küçük pay alıyor; geçerlilik `Probe` ile harekete başlamadan biliniyor. Takılma payı %70'e indi (0,07 → 0,049 hücre), fiskede de aynı. Geri yaylanma 0,18 sn (en çok iki salınım). Zincire yalnız küçük dürtü (mevcut `Kick`).
- **Test:** `ChainedPairThatCannotMoveStaysInItsSlot`: 40 seviyede 168 takılan zincirli hamle (110'u yan engel); hepsinde parça yerinde, jam sayılıyor, hamle sayılmıyor.

### Kombo merdiveni (T47)
x2 Güzel · x3 Harika · x4 Süper · x5+ Muhteşem (EN Nice/Great/Super/Amazing). Çıkış kenarında patlama (küçük/orta/büyük), rozet ~300 ms sonra (giriş 5 kare, kademeye göre bekleme, çıkış 4 kare, ~650 ms). x4+ çerçeve ışık dalgası, şok halkası, ekran kenarı ve flaş; x5+ ikinci küçük patlama. Tahta sarsıntısı kademeye göre (0,75–3 px). Tahtanın altında 1,2 sn sayaç; takılma komboyu sıfırlar ve sayacı kırar. Puan yazısı yok. Eski "x2" rozeti ve kombo övgüsü kalktı; yavaş oyuncu için yarı yol övgüsü kaldı.
- Not: yazı tiplerinde "×" glifi yok; rakam "x2" diye yazılıyor.

### Ölçümler
| | Sonuç |
|---|---|
| EditMode | 711 geçti, 0 kaldı, 2 atlandı (Perf dahil geçti) |
| `verify_build` yerel | 4/4, PC FPS medyan 59,9 |
| `--send --player TEST_PM64` | 5/7; KALDI yalnız bilinen eski `zeydsend` ve `a6b` |
| `verify_build` canlı p22 | 4/4 (L40 en düşük 2,5 FPS tek anlık, medyan 59,9) |
| Build (Brotli) | 30,82 → 36,11 MB (+5,3 MB, çoğu T47 kareleri, 384/512 px'e sınırlandı) |

GIF'ler: [kombo x2→x5+](pm64/kombo_x2_x5.gif) · [zincirli takılma önce/sonra](pm64/zincir_takilma_once_sonra.gif) · [saat: fırça ve kalkan](pm64/saat_firca_ve_kalkan.gif)

**Doğrulanmayan:** iPhone'da görünüm ve akıcılık; PlayMode testleri (`OpeningFlowTests` 2×2 varsayıyor); `&tut=2x2` akışı oynanmadı.
**Zeyd'e sor:** (1) p21 ve p22 aynı tarayıcı kaydını paylaşıyor (aynı alan adı); `c=p22` ile açınca cohort cihazda p22 olarak kalır, p21 linki `c` içermiyorsa p22 sayılır. (2) Bombalar 20. bölümden önce kalkıyor (codex/03); bu p22 varsayılanı.

## PM_65 — Stil denetimi ve düzeltme (14 Kasım gece)

**Durum: p22 güncellendi** (gh-pages `e5cd7c5b`; p20, p21, kök, pt1 değişmedi). Tablo: [`docs/art/STIL_DENETIM_14KASIM.md`](../STIL_DENETIM_14KASIM.md), görüntüler `docs/art/stil_denetim/` (önce/sonra). Codex'e kalan çizimler: [`docs/art/CODEX_TASK48.md`](../CODEX_TASK48.md) (çalıştırılmadı).

**Eski el nereden geliyordu:** 1. bölümün yeni akışı (sohbet kutusu + altın parlama) yalnız `c=p21/p22`'de açılıyordu. Test linki `c=ic` ya da cohort'suz link eski "Tutorial 1"e düşüyor, eldivenli el + büyük sarı ok + karartma çıkıyordu. Booster dersleri (5, 9, 15, 23) ve sandık ekranı da eldiven gösteriyordu.

**Düzeltilenler (kod):**
- `ic` ve cohort'suz giriş de p21/p22 oyunu (`TelemetryMeta.P21Line`, `P21Onboarding.Enabled`); eski el (`TutorialHand`) bu oyunda hiç açılmaz.
- `TutorialSpotlight`: p21/p22'de eldiven ve büyük ok yok, yazı Baloo; "Sıfır / olunca" satır bölme hatası düzeldi.
- Bütün T23 kartları (engel, booster, zor bölüm, Durdur): kurdeleli turkuaz panel yerine onaylı krem gövde + mavi çerçeve (`devam_v2/panel`, 9-slice 72 px), başlık kurdelesiz 3B yazı (`TextView.Title3D`: krem dolgu, mavi kontur, altın gölge).
- Sandık: eldivenli dokunma eli kaldırıldı.
- EN "Chest in 1 level!"; sürüm etiketi ve `app_version` `p22-2026-11-14`.

| Ölçüm | Sonuç |
|---|---|
| EditMode | 711 geçti, 0 kaldı, 2 atlandı (son düzeltmeden sonra yeniden koşuldu) |
| `verify_build` yerel | 4/4 (L40'ta tek anlık 3,3 FPS düşüşü, medyan 59,9) |
| `--send` (A6b, M5, zeydsend) | M5 geçti; `a6b` ve `zeydsend` bilinen eski KALDI (değişmedi) |
| `verify_build` canlı p22 | 4/4 |
| Build (Brotli) | 36,12 MB |

**Zeyd'e sor:** menü kurdeleli başlıkları ve turkuaz menü şeritleri; Ayarlar/Profil yeni dile geçsin mi; T45 Durdur paneli bağlansın mı (şimdi krem panel ama turkuaz Ses/Çık düğmeleri); booster seçim başlık plakası.
**Doğrulanmayan:** iPhone'da görünüm; `&tut=2x2` akışı ve PlayMode testleri.

## PM_66 — Zeyd'in p22 kararları (14 Kasım)

**Durum: p22 güncel** (gh-pages `b9187df4`; p20, p21, kök, pt1 değişmedi).

### 1. Takılma: bütün parçalar
Hiç ilerleyemeyen her parça (normal, zincirli, bombalı; sert ya da yumuşak engel) yalnız 0,049 hücre kıpırdar, 0,18 sn'de geri oturur. Yumuşak engeldeki %30 pay tamamen kalktı; ilerleyebilen parça sert engel payıyla (0,07) durur. Kilitli parça zaten sürüklenmez. Test `AnyPieceThatCannotMoveStaysInItsSlot`: 40 bölümde **4692 takılan hamle** (3008'i yan engel, eskiden %30 kayıyordu); hepsinde parça yuvasında, jam sayılıyor, hamle sayılmıyor.

### 2. Saat
Varsayılan Codex'in sol el + düğme kalkanı akışı. Portre canlı kalır; kalkan uçarken portre kısa süre kalkansız yüze geçer, sonra canlı portreye döner. PM_62 fırçası `&saat=firca`, sabit portre `&portre=sabit`.

### 3. Değnek — ölçüm ve yeni davranış
**Eski davranış ölçümü** (40 bölüm, 191 rastgele oyun ortası durum, 764 çeyrek denemesi): değnek 693 kullanımın **hiçbirinde** yeni serbest parça doğurmadı (%0). Sebep: kuralda yan komşudan kayma düz kenarlara ve parçanın ev hücresi kenarına bağlı; değnek yalnız iki parça arasındaki çıkıntıları değiştiriyordu, serbestliği belirleyen kenarlar ise çoğunlukla parça ile boş hücre arasında.
**Yeni:** değnek çeyrekteki parça–boş hücre kenarlarını da değiştirebilir (çeyrek sınırı değişmez) ve yalnız **en az 1 parçayı serbest bırakan**, çözülebilir düzeni seçer (en çok parça açanı). Yoksa çeyrek gri; hiç çeyrek yoksa "Değnek şu an bir parçayı açamıyor. Birkaç hamle sonra dene." uyarısı, booster harcanmaz. Ders (9. bölüm, bedava kullanım) eski davranışla çalışır, yoksa dolu tahtada ders kilitlenirdi.
**Yeni ölçüm:** kullanımların %100'ü ≥1 parça açıyor (47/47, ortalama 1,04; 45'inde 1, 2'sinde 2). Ama durumların yalnız **%24'ünde** (45/191) en az bir çeyrek kullanılabiliyor (oyunun başında 11/82, ortasında 20/71, sonunda 14/38).
**Görsel:** çeyrekteki parçalar havalanıp bir tur dönerek yeni çıkıntılarıyla oturur (zincirliler yalnız havalanır); serbest kalanlar iki kez altın parlar; alt şeritte "N parça serbest!".

### 4. Bomba sırası (değişmedi, bilgi)
p22'de 20. bölümden önce bombası kaldırılan bölümler (11 bölüm, 16 bomba):

| Bölüm | Kimlik | Bomba |
|---|---|---|
| 6 | S45 | 1 (bomba tanıtım kartı da kalkıyor) |
| 7 | S46 | 1 |
| 8 | S47 | 2 |
| 9 | S20 | 1 |
| 11 | S21 | 1 |
| 12 | S51 | 2 |
| 13 | S34 | 1 |
| 16 | Z04 | 1 |
| 17 | S18 | 1 |
| 18 | S22 | 1 |
| 19 | S28 | 4 |

Bomba dersi 20'de (S24, 6×7, 1 bomba). Ders öne alınırsa (ör. 7) yalnız `P21Onboarding.BombLesson` değişir.

### 5. Makas ipucu
Seçim modunda alt şeridin üstündeki ikinci soluk ipucu satırı kalktı (seçim görünümü kendi başlığını ve ipucunu gösteriyor).

### 6. Durdur: T45 paneli
Krem panel + 3B "Seviye N" başlık, büyük yeşil Devam, yuvarlak Ses/Müzik/Titreşim (açık/kapalı görselleri, dokununca değişir), küçük kırmızı Çık; arka plan opak lacivert (tahta görünmez). Codex TASK48 görselleri gelince bağlanacak.

| Ölçüm | Sonuç |
|---|---|
| EditMode | 712 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel / canlı p22 | 4/4 / 4/4, PC FPS medyan 59,9 |
| Build (Brotli) | 36,35 MB (+0,2 MB, T45 düğmeleri) |

GIF'ler: [takılma önce/sonra](pm66/takilma_butun_parcalar_once_sonra.gif) · [değnek](pm66/degnek_serbest_parca.gif) · [saat (kalkan, canlı portre)](pm66/saat_kalkan_canli_portre.gif) · [Durdur T45](pm66/durdur_t45.gif)

**PM'e soru:** değnek artık her kullanımda parça açıyor ama oyunun %76'sında hiç kullanılamıyor (gri). Seçenekler: (a) böyle kalsın; (b) hiçbir çeyrek açamıyorsa bütün tahtada dene; (c) değnek çeyrekteki çıkıntıları düzleştirsin (kayma serbestleşir, çok daha güçlü ama kural değişikliği).
**Doğrulanmayan:** iPhone; değnek dersinin (9. bölüm) baştan sona oynanması; Durdur'daki Müzik/Titreşim düğmelerinin telefonda etkisi.

## PM_67 — TASK48 bağlandı + değnek (b) + bomba dersi 7 (14 Kasım)

**Durum: p22 güncel** (gh-pages `87aca776`; p20, p21, kök, pt1 değişmedi). Build 38,01 MB (+1,7 MB, T48 katmanları; paneller oyundaki `devam_v2/panel` ile paylaşıldı, maket ve örnek başlık build'e girmedi).

### 1. TASK48 görselleri
- **Menü:** üstte old money mavi şerit (altın alt bordür) + mavi ad plakası; altta mavi sekme şeridi, etkin sekmede mavi lake yüz, ayırıcılar. Sekme başlıkları (Bölümler/Levels, Koleksiyon/Collection, Jiggles, Başarılar) kurdelesiz 3B yazı + krem-altın ışık. Haritadaki "En üste/En alta" ve Koleksiyon/Başarılar sayaçları turkuaz plaka yerine mavi plaka; menü afiş yazıları Baloo + lacivert kontur.
- **Ayarlar:** krem panel + mavi çerçeve, 3B başlık, krem satırlar, mavi simge diskleri, yeşil/gri anahtar + topuz, dil tepsisi, mavi Gizlilik/Bize yaz ve yeşil Hikâye düğmeleri (basılı hâlleri), kırmızı X, sürüm `p22-2026-11-14d`.
- **Profil:** krem panel, 3B başlık, açık mavi avatar diski, ad alanı + mavi düzenle düğmesi, krem-açık mavi istatistik kutuları, yeşil "Avatarı değiştir", kırmızı X.
- **Kazanma:** krem kart, altın hüzme + 3B "Harika!", büyük mutlu Jigo, krem tepside altın çerçeveli yuvalar + sandık, koyu mavi "N Jiggle kurtarıldı!" + Jiggle ikonu, katmanlı yeşil **Sonraki Seviye** ve mavi **Menü**. Sandık ekranı aynı (lacivert).
Görüntüler (TR/EN): `pm67/tr_menu`, `tr_harita`, `tr_koleksiyon`, `tr_ayarlar`, `tr_profil`, `tr_kazanma` ve `en_*` karşılıkları.

### 2. Değnek (b)
Hiçbir çeyrek parça açamıyorsa seçim, bütün tahta + "**Bütün tahtayı değiştir!**" onayıyla açılır (ipucu: "Hiçbir bölge parça açmıyor: bütün tahta değişir"); bütün tahtada da ≥1 parça açan çözülebilir düzen seçilir. O da yoksa kısa uyarı ("Değnek şu an parça açamıyor."), booster harcanmaz.
**Ölçüm** (aynı 191 rastgele durum): değneğin kullanılabildiği durumlar **%24 → %30** (45 → 57). Bütün tahta 12 durumda devreye girdi, ortalama **1,00** parça açtı; çeyrek kullanımlarında ortalama 1,04. Oyunun başında (ilk %20) hâlâ 13/82'de kullanılabiliyor: başlangıç tahtaları çok sıkı, yan kaymayı belirleyen kenarlar düz. Görüntü: `pm67/degnek_butun_tahta` (6. bölüm, dört hamle sonra).

### 3. Bomba dersi 7
Ders 7'de (S46 yerine 4×4, tek bomba, sayaç = parça sayısı, süresiz, kayıp yok; tanıtım kartı `pm67/tr_bolum7_bomba_karti`). 1–6'da bomba yok. 8 ve 9: tek bomba (en uzun sayaçlısı), sayaç ≥ parça + 4. 10'dan sonra bombalar özgün hâliyle geri geldi; 20 (S24) normal bölüm. Kilit 3, zincir 10, duvar 14 aynı.

**FunSim, 1–20 ilk deneme kazanma** (96 sanal oyuncu, Kolay profil):

| Bölüm | Kimlik | Bomba | İlk deneme | |
|---|---|---|---|---|
| 2 | S42 | 0 | 1,00 | |
| 3 | S43 | 0 | 1,00 | ders |
| 4 | S06 | 0 | 0,89 | |
| 5 | S44 | 0 | 0,98 | |
| 6 | S45 | 0 | 1,00 | |
| 7 | S46 (ders) | 1 | 1,00 | ders |
| 8 | S47 | 1 | 0,96 | yumuşak |
| 9 | S20 | 1 | 0,98 | yumuşak |
| 10 | Z24 | 0 | 0,91 | ders |
| 11 | S21 | 1 | 0,31 | |
| 12 | S51 | 2 | 0,38 | |
| 13 | S34 | 1 | 0,19 | |
| 14 | Z21 | 0 | 0,86 | ders |
| 15 | S13 | 0 | 0,66 | |
| 16 | Z04 | 1 | 0,45 | |
| 17 | S18 | 1 | 0,23 | |
| 18 | S22 | 1 | 0,06 | |
| 19 | S28 | 4 | 0,00 | |
| 20 | S24 | 1 | 0,00 | |

Dersler ve yumuşak bölümler ≥%85 (test `FirstTwentyFirstWinTable` bunu korur). **Uyarı:** 11'den sonra bombalar geri gelince ilk deneme kazanma sert düşüyor (11: %31, 13: %19, 17: %23, 18: %6, 19–20: %0). pt1'de 8: %63, 9: %38 duvarı vardı; şimdi duvar 11–13 ve 16–20'ye kayıyor.

| Ölçüm | Sonuç |
|---|---|
| EditMode | 713 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel / canlı p22 | 4/4 / 4/4, PC FPS medyan 59,9 |

**PM'e soru:** 11–20'deki bomba duvarı (FunSim %0–45) için ne yapalım: 11–13'e de yumuşak bomba (sayaç cömert) mi, yoksa bu bölümleri ayarlamak mı?
**Doğrulanmayan:** iPhone; değnek dersinin (9. bölüm) baştan sona oynanması; kazanmadan sandık ekranına geçiş (sandık ekranı eski lacivert).

## PM_68 — 11–20 bomba duvarı + sandık ekranı (14 Kasım)

**Durum: p22 güncel** (gh-pages `455f3429`; p20, p21, kök, pt1 değişmedi). Build 38,01 MB.

### 1. Kalibrasyon
pt1 gerçek veri (Supabase `v_attempts`, cohort pt1, attempt=1, slot 1–20; bölüm başına 12–51 oyuncu). Araç `Assets/_Game/Editor/Pm68Tune.cs` (`unity_batch.sh method ReverseSolver.EditorTools.Pm68Tune.Run`): özgün 1–20 (pt1'in oynadığı hâli), 160 sanal oyuncu, tür × hız × bomba dikkati × beceri ızgarası (160 birleşim), oyuncu sayısıyla ağırlıklı mutlak hata.
**En iyi:** Kolay, hız ×1,2, bomba dikkati +0,4, beceri +0,1 (`FunSim.Calibrated`). **Ağırlıklı ortalama mutlak hata 0,112, düz 0,153.**

| Bölüm | Kimlik | pt1 | Kalibre sim | Fark |
|---|---|---|---|---|
| 1 | S41 | 0,98 | 1,00 | +0,02 |
| 2 | S42 | 0,94 | 1,00 | +0,06 |
| 3 | S43 | 0,95 | 1,00 | +0,05 |
| 4 | S06 | 0,75 | 0,76 | +0,01 |
| 5 | S44 | 1,00 | 0,97 | −0,03 |
| 6 | S45 | 0,97 | 1,00 | +0,03 |
| 7 | S46 | 0,97 | 0,99 | +0,02 |
| 8 | S47 | 0,63 | 0,54 | −0,09 |
| 9 | S20 | 0,38 | 0,79 | +0,41 |
| 10 | Z24 | 0,74 | 0,77 | +0,03 |
| 11 | S21 | 0,29 | 0,28 | −0,02 |
| 12 | S51 | 0,71 | 0,64 | −0,07 |
| 13 | S34 | 0,60 | 0,33 | −0,28 |
| 14 | Z21 | 0,94 | 0,73 | −0,21 |
| 15 | S13 | 0,94 | 0,48 | −0,46 |
| 16 | Z04 | 0,43 | 0,54 | +0,11 |
| 17 | S18 | 0,14 | 0,38 | +0,24 |
| 18 | S22 | 0,69 | 0,09 | −0,60 |
| 19 | S28 | 0,00 | 0,00 | 0,00 |
| 20 | S24 | 0,33 | 0,00 | −0,33 |

Simülasyon 1–10'da ve bazı bölümlerde iyi, ama tek tek bölümlerde büyük sapma var (9, 15, 18, 20). Bu yüzden ayarlar **gerçek veriye dayalı tahminle** seçildi: *tahmin = pt1 gerçek + (ayarlı sim − pt1'in oynadığı hâlin simi)*. Simülasyon yalnız değişikliğin etkisini ölçüyor. pt1'in o slotta görmediği bölümde (19'a gelen S50) yalnız sim.

### 2–3. Sonuç (p22, 1–20)
| Bölüm | Önce | Sonra | pt1 gerçek | p22 önce (sim) | Tahmin sonra | Değişiklik |
|---|---|---|---|---|---|---|
| 1–6 | — | — | 0,75–1,00 | | | yok (PM_67) |
| 7 | S46 ders | aynı | 0,97 | 1,00 | | yok |
| 8–9 | yumuşak | aynı | 0,63 / 0,38 | 0,94 / 0,97 | | yok |
| 10 | Z24 ders | aynı | 0,74 | 0,77 | | yok |
| 11 | S21 | S21 v2 | 0,29 | 0,28 | **0,62** | bomba +3, süre +10 |
| 12 | S51 | aynı | 0,71 | 0,64 | 0,71 | yok |
| 13 | S34 | aynı | 0,60 | 0,33 | 0,60 | yok |
| 14 | Z21 ders | aynı | 0,94 | 0,73 | 0,94 | yok |
| 15 | S13 | S13 v2 | 0,94 | 0,48 | **0,67** | süre −15 (fazla kolaydı) |
| 16 | Z04 | Z04 v2 | 0,43 | 0,54 | **0,63** | süre +20 (zor kartı yoktu → hedefe çekildi) |
| 17 | S18 | S18 v3 | 0,14 | 0,38 | **0,57** | bomba +3, süre +20 |
| 18 | S22 | aynı | 0,69 | 0,09 | 0,69 | yok |
| 19 | S28 (4 bomba, zor) | **S50 v3** (1 bomba) | 0,00 | 0,00 | **0,68** (sim) | S28 ↔ S50 |
| 20 | S24 | S24 v2 | 0,33 | 0,00 | **0,58** | bomba +6 |
| 39 | S40 | **S28 v2** (zor) | — | | sim 0,46 | bomba +4; 28–38 bir slot öne kaydı |

Tahta düzenlerine dokunulmadı; yalnız bomba sayacı ve süre. Değişen her bölüm yeni sürüm (telemetri karışmaz); kimlikler aynı (kayıt/sayım uyumlu). 11–20'de zor bölüm kalmadı (en çok 2 kuralı sağlanıyor; art arda yok). 37 S38 ve 39 S28 zor, arada 38 normal. Uygulama `P21Onboarding.TuneSecondTen` (yalnız ders düzeni açıkken, yani p22). Test `SecondTenHitsTheTargetsOnTheCalibratedSim` aralıkları korur.

### 4. Sandık ekranı
Kazanma kartıyla aynı dil: krem panel + mavi çerçeve (`devam_v2/panel`), altın hüzme ve 3B "Sandık!/Chest!", sandığın arkasında altın ışık, koyu mavi "Sandığa dokun!", ödüller açıldıktan sonra katmanlı yeşil **Topla/Collect** (sonraki bölüm) ve mavi **Menü**. Yeni çizim gerekmedi (T48 parçaları); CODEX_TASK49 yazılmadı. Görüntüler `pm68/tr_sandik`, `tr_sandik_odul`, `tr_sandik_topla`, `en_sandik`, `en_sandik_topla`.

| Ölçüm | Sonuç |
|---|---|
| EditMode | 714 geçti, 0 kaldı, 2 atlandı |
| `verify_build` yerel / canlı p22 | 4/4 / 4/4 |

**Doğrulanmayan:** iPhone; tahminler gerçek oyuncuyla doğrulanmadı (p22'nin kendi verisi gelince karşılaştırılmalı); 39'daki S28 sim %46, zor bölüm hedefinin (%35–45) bir puan üstünde.

## PM_70 — Zorluk modeli v2 üreticide + p22'de 11–40 ayarı (14 Kasım)

p22 `0d63cc99` (gh-pages), sürüm `p22-2026-11-14f`. EditMode 721 geçti / 0 kaldı (2 atlandı). `verify_build`: yerel 4/4, canlı 4/4. `--send` koşulmadı (PM: Supabase'e yazma yok). Tahtalar değişmedi; yalnız bomba (fitil, bomba sayısı), süre, kilit ve zor kartı. Bölüm kimlikleri aynı, değişen her bölümün sürümü +1.

**Model v2 kodda.** `Generation/DifficultyModelV2.cs`:
- `PWin1`: ortalama oyuncunun ilk denemede kazanma şansı.
- `JamPerExit`: çıkış başına takılma (düşündürme).
- `Rules`: tasarım kuralları.
- `CheckCurve`: hedef eğri.

Test `ModelV2ReproducesThePoolScores`: `pool_scored.csv`'deki 250 `p_pred` ±0,01 içinde.

Süresiz bölümde sn/parça 10 sayılıyor. pt1'in en bol süresi 13,3 idi; yoksa ln(0) çıkıyor ve tahmin 0 oluyordu.

**Üretici.** `Factory.RunSlot` adayları model v2 ile değerlendiriyor (`FactoryOptions.ModelV2` = açık, 1000 oyun):
- İlk deneme eşiği model v2'den, tasarım kuralları da ret nedeni.
- Sıralama model v2'nin ilk denemesine göre.
- Raporda yeni sütunlar `v2Ilk`, `v2Dusundurme`, `v2Bogaz`. FunSim'in `ilkDeneme` sütunu yan sütun olarak kaldı.
- `FactoryRunner` yeniden kontrolü de aynı.
- **Bant ayarı:** model v2 ölçeğinde tanıtım ≥ %85, nefes ≥ %75 (PM_68/70 eğrisi). FunSim için konan tanıtım ≥ %95 eşiğinde 3×3–4×5 tanıtım bölümleri %94 çıkıp reddediliyordu. Kolay, orta, zor ve çok zor bantları aynı kaldı. Bunlar FunSim ölçeğinde konmuştu; model v2 ile yeniden bakılmalı. **Zeyd'e sor / PM.**

**Kurallar** `docs/kit/KIT_NOTLARI.md`'ye yazıldı:
- boğaz ≤ 1 (zor bölümde ≤ 2);
- açık tahtada sıkı bomba yok;
- sn/parça 1,7–2,3;
- 21–40'ta düşündürme ≥ 0,5.

**Roller:**
- Zor: 21, 23, 33, 35, 40.
- Nefes: 22, 24 (bombasız) ve 39.
- Diğerleri normal; 1–10 ve 14 değişmedi.

Rol kararları:
- **13 S34 normal yapıldı**, zor değil. Zor bandına (,35–,45) inmek için payı 1–2'ye düşürmek gerekiyor. Tahta açık (adım başına 5,9 çeldirici), bu da "açık tahtada sıkı bomba" kuralını bozuyor. PM'in iki seçeneğinden ikincisi.
- **15 S13'e bomba eklendi.** Süre kuralı içinde (≥ 1,7 sn/parça) bombasız bir bölüm modelde her zaman ≥ %85. 1,03 sn/parçayla %80 veriyordu.
- **39 S28 nefes oldu.** 40 zor; art arda iki zor olmasın.

Bombası azaltılan bölümler:
- 21 Z23 ve 38 S40'ta 2 bomba kaldı (4'ten ve 3'ten).
- 31 S31, 35 S29, 36 Z25 ve 39 S28'de 3 bomba kaldı (4'ten).

Bütün bombaları tutan seçenekler hedefe ve boğaz kuralına aynı anda uymadı.

Eklenen kilitler (düşündürme kuralı için): 23, 28, 30 ve 33'te birer kilit.

**Sonuç** (model v2, 2000 oyun; `fun` = PM_68'deki pt1'e kalibre FunSim'in ilk deneme tahmini, yan sütun):

| slot | bölüm | rol | p_win1 önce → sonra | düşündürme | boğaz | fun | değişiklik |
|---|---|---|---|---|---|---|---|
| 11 | S21 v2→v2 | normal | 0,67 → **0,67** | 0,62 → 0,62 | 0 → 0 | 0,61 | — |
| 12 | S51 v2→v2 | normal | 0,58 → **0,58** | 0,38 → 0,38 | 0 → 0 | 0,64 | — |
| 13 | S34 v1→v2 | normal | 0,42 → **0,60** | 0,53 → 0,53 | 0 → 0 | 0,57 | bomba 6:21 → 6:24; süre 65 → 67 sn |
| 14 | Z21 v3→v3 | ders | 0,88 → **0,88** | 0,54 → 0,54 | 0 → 0 | 0,73 | — |
| 15 | S13 v2→v3 | normal | 0,80 → **0,66** | 0,70 → 0,67 | 0 → 0 | 0,78 | bomba — → 10:15; süre 35 → 70 sn |
| 16 | Z04 v2→v3 | normal | 0,49 → **0,66** | 0,70 → 0,70 | 0 → 0 | 0,71 | bomba 13:22 → 13:26; süre 80 → 67 sn |
| 17 | S18 v3→v4 | normal | 0,62 → **0,64** | 0,61 → 0,61 | 0 → 0 | 0,81 | süre 70 → 74 sn |
| 18 | S22 v1→v2 | normal | 0,59 → **0,71** | 0,67 → 0,67 | 0 → 0 | 0,24 | bomba 19:21 → 19:24; süre 70 → 67 sn |
| 19 | S50 v3→v3 | normal | 0,65 → **0,65** | 0,42 → 0,42 | 0 → 0 | 0,68 | — |
| 20 | S24 v2→v3 | normal | 0,49 → **0,65** | 0,49 → 0,49 | 0 → 0 | 0,33 | süre 55 → 81 sn |
| 21 | Z23 v1→v2 | **zor** | 0,24 → **0,40** | 0,97 → 0,97 | 4 → 0 | 0,21 | bomba 19:9,31:18,32:11,37:22 → 19:10,31:19 (2/4 bomba); süre 150 → 115 sn |
| 22 | S19 v1→v2 | nefes | 0,90 → **0,92** | 0,58 → 0,58 | 0 → 0 | 0,59 | süre 50 → 57 sn |
| 23 | S27 v1→v2 | **zor** | 0,53 → **0,43** | 0,43 → 0,92 | 0 → 0 | 0,21 | bomba 3:6,23:8 → 3:3,23:5; süre 60 → 77 sn; kilit +8:24 |
| 24 | S14 v1→v2 | nefes | 0,84 → **0,88** | 0,53 → 0,53 | 0 → 0 | 0,68 | süre 50 → 60 sn |
| 25 | S35 v1→v2 | normal | 0,06 → **0,59** | 0,66 → 0,66 | 4 → 0 | 0,51 | bomba 18:33,19:33,24:32,25:29 → 18:36,19:36,24:35,25:32; süre 105 → 124 sn |
| 26 | S17 v2→v3 | normal | 0,56 → **0,66** | 0,60 → 0,60 | 0 → 0 | 0,64 | bomba 21:19 → 21:20; süre 55 → 57 sn |
| 27 | S10 v2→v3 | normal | 0,51 → **0,60** | 0,51 → 0,51 | 0 → 0 | 0,57 | süre 55 → 68 sn |
| 28 | S52 v2→v3 | normal | 0,68 → **0,59** | 0,45 → 0,60 | 0 → 0 | 0,68 | bomba 17:14 → 17:15; süre 60 → 46 sn; kilit +19:24 |
| 29 | S37 v1→v2 | normal | 0,28 → **0,61** | 0,85 → 0,85 | 4 → 0 | 0,05 | bomba 20:20,25:52,35:46,37:44 → 20:21,25:53,35:48,37:45; süre 135 → 101 sn |
| 30 | S08 v2→v3 | normal | 0,62 → **0,65** | 0,48 → 0,79 | 0 → 0 | 0,65 | kilit +16:24 |
| 31 | S31 v1→v2 | normal | 0,69 → **0,64** | 1,08 → 1,08 | 4 → 0 | 0,08 | bomba 23:30,25:28,30:33,35:32 → 23:31,30:34,35:33 (3/4 bomba); süre 170 → 92 sn |
| 32 | S39 v1→v2 | normal | 0,12 → **0,56** | 0,77 → 0,77 | 4 → 1 | 0,14 | bomba 19:36,25:36,32:22,37:32 → 19:48,25:48,32:22,37:44; süre 115 → 98 sn |
| 33 | S32 v1→v2 | **zor** | 0,06 → **0,40** | 0,41 → 0,60 | 0 → 0 | 0,26 | bomba 26:18,31:27,36:17,37:18 → 26:26,31:32,36:25,37:26; süre 110 → 100 sn; kilit +50:24 |
| 34 | S36 v1→v2 | normal | 0,41 → **0,55** | 1,12 → 1,12 | 3 → 0 | 0,13 | bomba 7:35,29:15,46:23 → 7:36,29:16,46:24; süre 80 → 85 sn |
| 35 | S29 v1→v2 | **zor** | 0,21 → **0,38** | 1,03 → 1,08 | 3 → 0 | 0,18 | bomba 7:7,24:21,47:17,57:43 → 7:8,24:22,47:18 (3/4 bomba); süre 110 → 133 sn |
| 36 | Z25 v1→v2 | normal | 0,57 → **0,55** | 1,40 → 1,40 | 4 → 0 | 0,17 | bomba 25:28,30:31,31:30,32:29 → 25:29,31:31,32:30 (3/4 bomba); süre 145 → 93 sn |
| 37 | S38 v1→v2 | normal | 0,18 → **0,56** | 1,45 → 1,45 | 3 → 1 | 0,33 | bomba 8:29,29:17,30:8 → 8:34,29:22,30:8; süre 70 → 90 sn |
| 38 | S40 v1→v2 | normal | 0,41 → **0,57** | 0,87 → 0,87 | 3 → 1 | 0,01 | bomba 14:40,15:26,43:16 → 14:42,15:26 (2/3 bomba); süre 80 → 106 sn |
| 39 | S28 v2→v3 | nefes | 0,75 → **0,83** | 0,71 → 0,70 | 0 → 0 | 0,46 | bomba 18:37,24:38,30:23,32:12 → 18:38,24:39,30:24 (3/4 bomba); süre 130 → 119 sn |
| 40 | S33 v1→v2 | **zor** | 0,08 → **0,33** | 0,82 → 0,82 | 4 → 0 | 0,12 | bomba 8:8,10:31,38:27,47:44 → 8:10,10:33,38:29,47:46; süre 105 → 115 sn |

Eğri ve kurallar (test `P22ElevenToFortyHitsTheCurveAndTheRules`):
- 11–20 normal ,58–,71; zor yok.
- 21–30 normal ,59–,66; zor ,40 / ,43; ardından nefes ,92 / ,88.
- 31–40 normal ,55–,64; zor ,40 / ,38 / ,33; nefes yalnız 39 (,83).
- Boğaz en çok 1, sn/parça 1,7–2,3, 21–40 düşündürme ≥ ,51.

**Doğrulanmayanlar ve riskler:**
- **FunSim ile model v2 21–40'ta çok ayrışıyor.** FunSim'in tahmini birçok bölümde ,01–,33. Model v2 pt1'e birini dışarıda bırakarak ±10,7 puan hatayla oturmuş; FunSim'in 21–40 kalibrasyonu yok. Bu yüzden karar model v2'ye göre verildi, ama gerçek veri gelmeden ikisi de tahmin.
- Modelin S47/S08 tipi bölümlerde iyimser olduğu biliniyor (PM: ±25 puan). 20 S24, 27 S10, 30 S08 ve 19 S50 bu tipte; gerçekte daha zor çıkabilir.
- Zincir dar anı modelde zorluğu azaltıyor. Tek veri setine dayanıyor; p22 verisiyle sınanmalı.
- iPhone'da denenmedi.

## PM_72 — Görsel toparlama: tutorial, menü şeridi, kombo yerine tatmin anı, test linki, T50 seçenekleri (15 Kasım)

Görüntüler `docs/art/task29/pm72/` klasöründe:
- `once/`: canlı p22, `0d63cc99`
- `sonra/`: yeni build
- yan yana karşılaştırmalar: `menu_once_sonra.png`, `bolum1_once_sonra.png`

Tur aracı: `Tools/pm72_tour.js`, bölüm verisi `Pm72Tour.Dump` ile üretiliyor. Turda kayıtlı çözümle bölümler kazanılıyor.

### 0. Sorunların kaynağı

| Sorun | Nereden geldi |
|---|---|
| 1. bölümde eski el, büyük ok, kararma | 3 ayrı kaynak var, aşağıda. |
| Ana menüde üst mavi şerit | PM_67 (`ff85c438`): TASK48 `top-strip` ve mavi `profile-pill` menüye bağlandı. |
| Kombo yazıları, rozetler, çerçeve dalgası, şok halkası | PM_64 (`fbd86994`): T47 kombo merdiveni (`ComboLadderView`). |
| Test linkinde booster 99, altın 99 999, ilerleme normal kayda yazılıyor, telemetri `ic` kohortuyla gidiyor | PM_6x test modu yalnız stoğu ve kilitleri açıyordu. |

1. bölümdeki el, ok ve kararmanın üç kaynağı:
1. **`7530365c`** (12 Kasım, Codex devri, "Rework the guided opening"): `keepPrelude` eklendi. Kayıtta eski öğretici bölümü (`P21_T01`) olan cihaz, `reset=1` olmayan her açılışta eski açılış akışına (`Prepend`) dönüyordu. O akışta `Hand/drag` eli ve `Path/straight` okları var.
2. **PM_64** (`355a0324`): 2×2 açılışı `&tut=2x2` arkasına aldı, ama `keepPrelude` yolunu bıraktı. `&tut=2x2` bir kez açılınca `P21_T01` kaydı oluşuyor ve eski akış o cihazda kalıcı oluyor.
3. **PM_65**: yalnız `TutorialSpotlight` eldivenini ve okları `Art.P21Gameplay` ile kapattı. Ama:
   - eski açılışın kendi eli (`P21TutorialView.Show`) korunmasız kaldı;
   - kararma (`Veil`) hiç korunmadı;
   - eski el `art=eski` bayrağıyla da açılabiliyordu.

PlayerPrefs her build yolunda ayrı tutuluyor. Zeyd'in p22 kaydında böyle bir `P21_T01` var. Test linki `reset=1` içermediği için eski akış geri geldi.

### 1. Tutorial: yalnız PM_60 akışı

- **Açılış:** `_p21Flow` ve `_p21Revised` artık hep kapalı. `&tut=2x2`, `&tut2x2=1` ve kayıttaki `P21_T01` hiçbir şeyi değiştirmiyor.
- **Eski el kaldırıldı:**
  - `GameRoot` içindeki iki `TutorialHand.Show` çağrısı,
  - `P21TutorialView.Show` içindeki el ve ok,
  - `TutorialSpotlight` eldiveni: `Glove = false`, bayrağa bakmıyor.
- **Büyük ok:** kural kartı dışında hiç çizilmiyor.
- **Kararma:** `TutorialSpotlight.Darken = false`. `Veil` hiçbir akışta çizilmiyor.
- **Kalıcı test:** PlayMode `LevelOneIsTheStoryOpeningOnEveryLink`.
  - Linkler: `c=p22`, `c=p22&reset=1`, `c=p22&test=1`, kohortsuz, `&tut=2x2`, `&tut2x2=1`.
  - Her linkte cihazda eski `P21_T01` kazancı var.
  - 1. bölümde bulunmaması gerekenler: `TutorialHand`, `TutorialSpotlight`, `Veil`, `FingerPivot`, `ArrowShaft`, `ArrowHead`, `Swipe path`, `guide`, `P21 tutorial`, el ya da eldiven sprite'ı. Hiçbiri yok.
  - Bulunması gerekenler: "P21 story" sohbet kutusu açık ve altın parlama bir parçada. İkisi de var.
- `Tools/unity_batch.sh ptests` eklendi (PlayMode, grafikli batch). PlayMode 4/4.
  - Eski 2×2 açılış testi kaldırıldı, çünkü o akış artık yok.
  - Makas testi uçuşun 1 saniyeden kısa olduğu durumda takılıyordu. Kesimden sonra saatin aktığını bekleyecek şekilde düzeltildi.

### 2. Ana menü üst şeridi

- Menüde şerit yok.
- Oyuncu adı, Jiggle sayacı ve dişli, arka planın üstünde küçük, yarı saydam (%78) krem haplarda. Kenarları ince altın, yazılar lacivert.
- Dişli simgesi `settings-normal`'den kesildi: `Resources/Art/pm72-gear.png`.
- Alt sekme şeridi aynı kaldı.
- Karşılaştırma: `menu_once_sonra.png`.

### 3. Kombo yerine her çıkışta küçük tatmin anı

**Kaldırılanlar:**
- `ComboLadderView` (kombo yazıları, rozetler, çerçeve ışık dalgası, şok halkası, sayaç)
- yarı yol övgüsü
- konfeti
- `Resources/Art/yeni/T47`, 220 dosya. Build'e artık kopyalanmıyor.
- `juice21` içindeki `combo`, `great`, `super`, `perfect`, `confetti` ve `pop`

| An | Görüntü | Ses | Titreşim |
|---|---|---|---|
| Her çıkış | Parçanın tahtadan ayrıldığı yerde küçük, opak altın kıvılcım. 0,25 sn, ~0,95 hücre. Kareleri `Tools/make_pm72_spark.py` çiziyor. | `pm72-tick` (70 ms pop) | hafif |
| Bombalı parça çıkışı | Daha büyük kıvılcım (1,45 hücre, 0,32 sn), bomba ikonu grileşip küçülerek söner (0,38 sn). | `pm72-defuse` (pop + kısa tıslama) | orta |
| Kilit açılınca | Küçük metal kıvılcım | `pm72-unlock` (kırılma) | hafif |
| Son parça | Mevcut parıltı | `popfull`, sonra fanfar | başarı (notification success), sonra kazanma ekranı |
| Takılma | — | mevcut tık | çok kısa, zayıf; kendi anahtarı var |

Sesleri `Tools/make_pm72_sounds.py` üretiyor.

**Titreşim altyapısı:**
- **iOS uygulaması:** `RSHaptics.mm`, UIImpactFeedbackGenerator ve UINotificationFeedbackGenerator. Takılma için 0,4 şiddetli hafif impact eklendi.
- **Web, Android:** `navigator.vibrate`, jslib `RS_Vibrate`. Süreler: hafif 12 ms, orta 22, başarı 30, takılma 8 ms. Seçim tıkı web'de yok.
- **iPhone Safari:** web titreşimi yok. Orada yalnız görüntü ve ses var.
- **Ayarlar:** "Titreşim" anahtarı hepsini kapatıyor.
- **Takılma titreşimi:** ayrı anahtar, varsayılan açık. Linkle açılıp kapanıyor: `&takilmatitresim=0` ya da `=1`. Ayar cihazda kalıyor.
- Kareler: `sonra/cikis_kivilcim_kareler.jpg`, `sonra/bombali_parca_cikis_kareler.jpg`.

### 4. Test linki (`&test=1`)

- **Bölümler:** 1–40 açık (mevcut `UnlockAll`).
- **Booster:** her biri 999 (`GameSession.TestStock`).
- **Altın:** 999 999 (`SaveData.TestCoins`).
- **Devam:** ücretsiz, fiyat 0.
- **Kayıt:** `TestHost` cihaz kaydını okuyor; ad ve ayarlar oradan geliyor. Bütün yazmalar bellekte kalıyor, test ilerlemesi normal kayda yazılmıyor. Test için `reset=1` de normal kayda dokunmuyor.
- **Ad kutusu:** test linkinde çıkmıyor, ad "TEST" (yalnız bellekte).
- **Telemetri:** test modunda hiç gönderilmiyor (transport yok). pt1 ve p22 verisi kirlenmiyor.
- **Normal link:** etkilenmiyor.
- **EditMode testleri** (`Pm72Tests`):
  - bellek kaydı cihaza yazmıyor,
  - stok ve altın değerleri,
  - titreşim anahtarları.
- **Not:** `&sandik=1` önizlemesi de artık test altınını gösteriyor.

### 5. T50: kazanma ekranı ve harita düğmeleri

- Codex iki ayrı `codex exec` çağrısıyla çalıştı (`timeout 1800`, `< /dev/null`). İkisi de ilk denemede bitti, tarza uygun; yeniden deneme gerekmedi.
- Kazanma: 30 parça. Harita: 39 parça. Her öğe için A/B/C önizlemesi var.
- Seçim sayfası: `docs/art/task50/SECENEKLER.md`.
- Oyuna bağlanmadı.

### Ölçümler

- **EditMode:** 724/0, 2 atlandı.
- **PlayMode:** 4/4.
- **Build:** Brotli toplam 36,5 MB (önce 38). T47 ve kullanılmayan efektler çıktı.
- **verify_build:** yerel 4/4, canlı 4/4.
- **Doğrulanmadı:**
  - iPhone'da his (titreşim yok, ses ve kıvılcım),
  - Android'de titreşim,
  - iOS uygulamasında haptic (native build yapılmadı).

### Fark edilen ama bu işin dışında kalanlar

- Bomba tanıtım kartında ikon "2" gösteriyor, metin "16 hamle" diyor (bölüm 7).
- "Süre doldu" devam panelinin başlığı kurdele/afiş biçiminde. PM_65 ölçütüne göre kurdele olmamalı.

## PM_73 — T50 seçimleri, kayma izi, nefes anı, iki küçük hata (16 Kasım)

Zeyd'in seçimi: kazanma ekranı B, harita düğmeleri C.
- p22 build `p22-2026-11-16a` (gh-pages `a15a03d2`).
- Görüntüler ve GIF'ler `docs/art/task29/pm73/` klasöründe:
  - `sonra/`: yeni ekranlar.
  - Yan yana karşılaştırmalar: `kazanma_once_sonra.png`, `harita_once_sonra.png`, `sure_doldu_once_sonra.png`, `sandik_once_sonra.png`.

### 1. Kazanma ekranı: T50 B
**Parçalar:** `Tools/make_pm73_art.py`, `Assets/Art/_yeni/T50/kazanma/B` kaynağını `Resources/Art/yeni/T50/win` klasörüne kopyalıyor.
- Düğme yüzlerindeki gömülü Türkçe yazı silindi. Yazılar oyunun fontuyla (Baloo) çiziliyor, bu yüzden İngilizce de çalışıyor.
- Işık resmi 472×1024'e indirildi.
- Bütün parçalar 4'ün katı boyutunda, sıkıştırılabiliyor.

**Ekran (`P21WinView.LayoutB`):**
- Kart yok. Gölgelenmiş tahtanın üstünde mavi ışık, altın huzmeler ve konfeti.
- 3B "Harika!", İngilizcede "Great!" (Codex'in resimleri).
- Zıplayan Jigo.
- "+50" oyunun fontuyla (krem dolgu, mavi kontur) ve yanında altın simgesi. Bozuk "÷" görünümü giderildi.
- "+50" gerçek ödül: `WinCoins` her kazanmada 50. Ödül yoksa satır gizleniyor.
- Beş bölmeli sandık çubuğu. İki sandık arasındaki ilerleme orantılı doluyor; en son dolan bölme hafifçe büyüyüp yerine oturuyor.
- Yeşil "Sonraki Seviye" / "Next Level" ve mavi "Menü" / "Menu".

**Giriş animasyonu, toplam 1,15 sn:**
1. Işık açılır (0–0,3 sn).
2. Başlık 1,3×'ten 1×'e oturur (0,12–0,45).
3. Jigo zıplar ve hafifçe ezilerek iner (0,3–0,78).
4. Altın 0'dan 50'ye sayılır, çubuk görünür (0,6–0,92).
5. Düğmeler aşağıdan gelir (0,82–1,15).

Dokunmak girişi hemen tamamlıyor. Giriş bitince Jigo hafifçe sallanıyor.

**Sandık ekranı:** aynı dile geçti. Krem panel yerine mavi ışık, 3B "Sandık!" ve aynı düğmeler.

**T48 kaldırıldı:** kazanma kartı koddan çıktı, `Resources/Art/yeni/T48/win` silindi. `T46BuildArt` artık yalnız T48'in menü, profil ve ayar klasörlerini kopyalıyor.

### 2. Harita: T50 C (yapboz düğmeleri)
`LevelsMap.NodeC` beş durumu çiziyor:
- **Tamamlandı:** mavi gövde, altın kenar, tik.
- **Şu anki:** 1,25× (82 pt), hale. Hafif nefes alıyor: 1,0 ↔ 1,04, 2 sn.
- **Kilitli:** gri gövde, asma kilit.
- **Zor:** bordo gövde, Kont mührü.
- **Engel dersi:** 3 kilit, 7 bomba, 10 zincir, 14 duvar rozeti.

Rozetler:
- 24 pt.
- Türü gösteren rozetler (Kont, engel) sağ üstte, durumu gösterenler (tik, kilit) sağ altta; üst üste binmiyorlar.

Diğer ayrıntılar:
- Bölüm numaraları Baloo fontunda.
- **Jigo işareti:** düğme yüksekliğinin %70'i. Kenar boşluğu kırpıldı. 1,5 sn'de bir aşağı yukarı sallanıyor.
- Düğmeler arasında altın noktalı yol var, her 10 pt'de bir nokta.
- Eski "Oyna" baloncuğu ve avatar kaldırıldı. Dokunma alanı 66 pt.

### 3. Kayma izi (`ExitTrail`)
Doğru yönde atılan parça çıkarken arkasında iz bırakıyor. Takılmada ve çekiçte iz yok.

**`&iz=serit` (varsayılan):**
- Parça genişliğinde bir şerit. İlk üçte birde tam genişlikte, sonra incelip solan "uçurtma kuyruğu".
- Rengi parçanın altındaki resmin baskın tonu. Bu ton resim yüklenirken 16×16 ızgaradan bir kez okunuyor.
- Bombalı parçada iz altın.
- Ucunda iki altın kıvılcım var.

**`&iz=hayalet`:** parçanın soluk kopyaları, 45 ms arayla en çok 3 tane.

**Ortak kurallar:**
- **Ömür:** her iz noktası 0,22 sn yaşıyor; parça gidince iz 0,22 sn içinde siliniyor.
- **Uzunluk:** bırakma hızına göre 0,6–2,6 hücre; hızlı fiske uzun, yavaş itiş kısa iz bırakıyor.
- **Komşular:** iz, parçanın kendi başlangıç hücresinin arkasına geçmiyor. Bütün parçaların altında çiziliyor (sıra 98), komşu parçalar her zaman üstte kalıyor.
- **Sınır:** ekranda en çok 3 iz.
- **Ayarlar:** `ExitTrail.LengthCells`, `MinCells`, `Opacity`, `Life`.

**Düzeltilen sorun:**
- **Belirti:** ilk sürümde iz görünmüyordu.
- **Ölçüm:** tarayıcı konsoluna yazdırılan tanı satırları ve piksel ölçümüyle sebep bulundu: iz çiziliyordu, ama baş kısmı parçanın altında kalıyor, kuyruğu da hızla şeffaflaşıyordu.
- **Düzeltme:** ilk üçte birlik bölümün opaklığı ve genişliği artırıldı, renk canlandırıldı.
- Şerit artık bir sprite, nesneleri tahtayla birlikte önceden yaratılıyor. Oyun sırasında yeni nesne yaratılmıyor.

**GIF'ler:** `iz_serit.gif`, `iz_hayalet.gif`.

### 4. Nefes anı (`BoardView.FlyOut`)
**Ne zaman:**
- Bombalı parça çıkarken,
- bir kilidi açan çıkışta,
- zincirli parçada,
- son parçada.

**Nasıl:**
- Parça tahtanın kenarını geçtiği anda yalnız o parçanın saati 90 ms boyunca %20 hıza iniyor. Son parçada bu süre 160 ms.
- Son %30'luk kısımda parça yeniden hızlanıyor.
- **Etkilenmeyenler:** oyun saati, girdi ve diğer parçalar durmuyor.
- **Kıvılcım, ses, titreşim:** aynı kenar anında geliyor.
  - Kıvılcım ya da bomba sönme efekti patlıyor.
  - Ses ve titreşim de o anda çalıyor (`BoardView.ExitBurst`). PM_72'de bunlar parmak bırakıldığı anda çalıyordu, artık kıvılcımla aynı anda.
  - Kilit sesi, kilidi açan çıkışın kenar anına taşındı.
  - Son parçanın başarı titreşimi de kenar anına taşındı.
- **Kazanma ekranı:** son parçanın uçuşu bitmeden açılmıyor (`FlyingUntil`).

**Bayraklar:**
- `&nefes=onemli` (varsayılan).
- `&nefes=hep`: her çıkışta 50 ms; önemli çıkışlarda kendi süreleri.
- `&nefes=yok`.

**GIF'ler:**
- `nefes_bomba_once_sonra.gif`: bölüm 7'nin bombalı parçası.
- `nefes_son_parca_once_sonra.gif`: bölüm 1'in son parçası ve kazanma ekranı.
- 30 fps. Sağ tarafta parça kenarda 3–5 kare daha uzun kalıyor.

### 5. İki küçük hata
- **7. bölüm bomba kartı:** simge bombanın gerçek sayacını gösteriyor (16) ve metindeki sayıyla aynı. Birden çok bomba varsa en küçük sayaç gösteriliyor. Simge artık 3-2-1 saymıyor; 1,6 sn'de bir yerinde tıklıyor.
- **"Süre doldu" / "Bomba patladı" paneli:** kurdele kaldırıldı. Başlık krem panelde 3B yazı (krem dolgu, mavi kontur, altın derinlik).

### Ölçümler
- **EditMode:** 733/0, 2 atlandı.
  - Yeni `Pm73Tests`: giriş ≤ 1,2 sn, rozet ≥ 22, Jigo payı, nefes süreleri, iz sınırları, bomba kartı.
  - Yeni `Pm71Tests`.
- **PlayMode:** 4/4.
- **Build:** Brotli 35,86 MB. PM_72'de 36,48 MB idi; T48 kazanma parçaları çıktı.
- **verify_build:**
  - Yerel 4/4.
  - `--send --player TEST_PM73`: 5/7. KALDI'lar yalnız PM_54'ten beri bilinen eski `zeydsend` ve `a6b`.
  - Canlı 4/4.

**Doğrulanmadı:**
- iPhone'da his (iz, nefes, giriş animasyonu).
- Android titreşimi.

## PM_71 — Veriyle düzeltme + 8 bölüme ince ayar (16 Kasım)

### 1. `PWin1Blended`
`DifficultyModelV2.PWin1Parts` model, veriyle ve birleşik değeri birlikte veriyor. `PlayData` sınıfı:
- **pt1 verisi:** `docs/analiz/v2/irt_level_d.csv` ve oynanan sürümler `redesign.json`. Sürümlerin 40'ı da tutuyor.
- **Kohort parametre:** ileride `irt_level_d_<kohort>.csv` aynı yoldan okunacak.

Formül:
- veriyle = σ(−d_obs + logit p_model(yeni) − logit p_model(pt1 sürümü))
- birleşik = σ((1−w)·logit p_model + w·logit veriyle), w = n/(n+10)

Davranış:
- Verisi olmayan bölümde birleşik değer `PWin1` ile aynı.
- **Kullananlar:**
  - üretici (`Estimate.Of`, `PWin1Model` ve `PWin1Data` yan alanlarıyla),
  - eğri testi (`P22CurveTests`),
  - `Pm71Tune.Report`.
- **Ayar aracı:** `Pm71Tune.Search` ve `Eval`. Birleşik hedefi modelin hedefine çeviriyor: birleşik logit = model logit − w(d_obs + logit p_pt1).

**Testler:**
- S38 v2 için birleşik değer 0,391 (PM: 0,39).
- Verisi olmayan bölümde `Blended == PWin1`.
- Kohort parametresi çalışıyor.

**Eğri kontrolüne ek:** bant artık iki basamağa yuvarlanmış değerle karşılaştırılıyor; PM'in tabloları da böyle okunuyor.
- Sebep: 13 (0,547), 19 (0,754) ve 31 (0,651) bandın 0,005 içinde. PM tablosunda üçü de bantta, "diğer bölümler kalsın" dendiği için dokunulmadı.

### 2. İnce ayar (`P21Onboarding.DataTuning`)
- PM_70'in bölümleri üzerine uygulandı. Tahtalar değişmedi, her bölümün sürümü +1.
- Sıra: fitil → süre → bomba taşıma. Engel eklenmedi.

| slot | bölüm | rol | model | veriyle | birleşik | değişiklik |
|---|---|---|---|---|---|---|
| 11 | S21 v3 | normal | ,67 → ,73 | ,46 → ,52 | ,53 → ,59 | fitil 19 → 22 |
| 26 | S17 v4 | normal | ,66 → ,31 | ,97 → ,88 | ,87 → ,61 | fitil 20 → 16 |
| 27 | S10 v4 | normal | ,60 → ,31 | ,97 → ,90 | ,85 → ,63 | bomba 8:17 → 15:19 (açık tahtada yalnız fitil kısmak "sıkı bomba" kuralını bozuyordu) |
| 30 | S08 v4 | normal | ,65 → ,71 | ,35 → ,41 | ,52 → ,58 | fitil 18 → 20, süre 70 → 63 sn |
| 32 | S39 v3 | normal | ,56 → ,47 | ,76 → ,69 | ,66 → ,57 | bomba 19 fitil 48 → 41, süre 98 → 100 |
| 33 | S32 v3 | zor | ,40 → ,33 | ,57 → ,50 | ,47 → ,40 | bomba 37 fitil 26 → 24 |
| 36 | Z25 v3 | normal | ,55 → ,44 | ,81 → ,73 | ,67 → ,57 | bomba 31 fitil 31 → 30 |
| 37 | S38 v3 | normal | ,56 → ,70 | ,19 → ,30 | ,39 → ,54 | bomba 30: 8 → 9 (pt1'deki ölümler); 29: 22 → 17; 8: 34 → 32 |

**S38 neden üç bomba birden değişti:** yalnız 30'u gevşetmek bölümü 0,93'e çıkarıyordu.

**Eğri:**
- 11–40'ta eğri ve kurallar birleşik değere göre temiz: art arda zor yok, 31–40'ta 3 zor, nefes bölümleri yerinde.
- `Rules.Check` hiçbir bölümde ihlal göstermiyor.

**Doğrulanmadı:** gerçek oyuncu sonucu. p22 verisi gelince tablo `irt_level_d_p22.csv` ile güncellenecek.

## PM_74 — Gece cilası: hatalar, yerleşim, booster'lar, resme dalış, kayıp ekranları (16 Kasım)

Yeni önizleme `p23/` (p22 yedek). Ayrıntı ve önce/sonra görüntüler: `docs/art/pm74/RAPOR.md`, denetim `docs/art/pm74/DENETIM.md`.

**Hatalar**
- Test linkinde booster dersi bitmiyordu (değnek, saat, makas, çekiç): `OnEvent` test modunda envanteri yazmasın diye ders ilerlemesini de atlıyordu. Ders adımı artık her modda bitiyor, envanter test modunda yine yazılmıyor. PlayMode `BoosterLessonOnTheTestLinkGivesInputBack` (9. ve 15. bölüm).
- Seçimde tahtanın dışına tek dokunuş iptal ediyor (`PressBooster21`; rehberli ders seçimi hariç).
- Kilit kartı "N hamle" → "üstündeki sayı kadar hamle"; "gövde" → "parça"; TR'de "Seviye" → "Bölüm".

**Yerleşim ve HUD**
- `Hud54BoardPlacement`: çerçeve bandı (18,2) + 12 pt hava her yanda; yalnız 8 sütunlu iki bölümde hücre 42 → 41.
- Jigo canlı portresi `JigoPortrait` gölgelendiricisiyle kemere kırpılıyor (gölgelendirici artık sprite rengini de çarpıyor).
  - Boy %72,5; yüksük kemerin 3 pt altında.
  - Açık göz tek kare; kırpma 3–6 sn'de bir.
- Parça kutusu yalnız sayı (%44 yükseklik). Başlıklar kurdelesiz krem hap; `&hud=kurdele` eski başlık.
- Menü: normal oyuncuda tek "Oyna · Bölüm N"; Bölümler yalnız test linkinde.

**Booster'lar (`BoosterFx74`)**
- Codex (xhigh) parçaları: `Assets/Art/_yeni/PM74`. `Tools/make_pm74_art.py` bunları `Resources/Art/yeni/pm74`'e alıyor.
- Her booster için üç fikir; varsayılan A, `&cekicfx` / `&makasfx` / `&degnekfx` / `&saatfx` = b|c ile diğerleri. `&fx=eski` PM_62/66 animasyonları.
- Zamanlama sözleşmesi `P21BoosterView.HitSeconds` / `DurationSeconds`.
- Kamera sarsıntısı seviye değişiminde geri alınıyor (`FxGuard74`).
- Saat kuralı aynı (10 sn durdurur). Kutunun altında geri sayan "+N sn" rozeti eski donma çubuğunun yerini aldı.

**Resme dalış (`Dive74`)**
- Menü → bölüm: tabloya kamera yakınlaşması, bölüm kurulur, dağıtma atlanır (`SkipDeal`), kamera tahtadan geri çekilir, HUD ve çubuk kayar.
- Kazanma → sonraki: biten resim sola, yenisi ortaya, dalış.
- Dalışta dokunuş yok sayılır. `&gecis=yok` kapatır.

**Kayıp ve devam (`LossView74`)**
- T50 B dilinde; `ContinuePanel` p21 hattında ona devrediyor, kayıp `ShowResult`'tan açılıyor.

**Diğer**
- İsim ekranı CSS: krem panel, mavi çerçeve, Baloo alt kümesi `TemplateData/pm74-baloo.ttf` (22 KB).
- Sandık altın sayacı başlığın altında.
- Kazanma "Menü" dokunma alanı ≥ 44 pt.
- Tık sesi −4 dB.
- Durdur yazıları 15 pt.
- `WebBuild -rsBrotliOnly`.
- Tur aracında `eval` ve `--safe top,bottom` (güvenli alan taklidi).

**İlk açılış ve yükleme**
- `&perf=1`: bölüm açılışının süre dökümü ve sonraki 3 kare, `[perf]` satırları. Resim çözme süresi de yazılır.
- İlk açılıştaki ~300 ms'lik kare ilk kullanım maliyetiydi. `WarmLevelOnce` menüde bir kez, sonraki bölümün HUD/tahta/çubuğunu `SortingGroup` (-32000) altında 2 kare çizip siliyor. Sonuç: ilk kare 32 ms, dalış en kötü 58,5 FPS. `&isinma=0` kapatır.
- PlayMode `Board` yardımcısı ısınma tahtasını atlıyor.
- Yükleme ekranında eski ikon bir an görünüyordu; ikon ilk boyamadan doğru dosyayla geliyor.
- Menü "Oyna" düğmesi `Pulse74` ile nefes alıyor (1,6 sn, %3).
- Galeri anında kazanma ışığı (`T50/win/win_bg_glow`) duruyor.
- Çekiç kırılmasında `pm72-defuse` sesi.
- Durdur kartının arkasında mavi ışık (örtü bir sıra alta indi).
- Booster seçim ikonu başlık rozetinin solunda (`&secim=eski` eski düzen).

**Doğrulanmadı:** iPhone'da his, FPS ve titreşim.
