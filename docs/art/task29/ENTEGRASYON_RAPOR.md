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
