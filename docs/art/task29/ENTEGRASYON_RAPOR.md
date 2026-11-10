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
