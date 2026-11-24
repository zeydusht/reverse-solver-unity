# PM_106 RAPOR: Block Out tarzı çıkış sesi, gerçek zincir, lastik her yerde, tek tık, dalış klinki

PM_105 bu işten önce bitmişti (`docs/art/pm105/RAPOR.md`).

## 1. Küçük düzeltmeler

### Ayarlar'da iki tık

- **Sebep:** Ayarlar'a basınca aynı karede iki ses çalıyordu: düğmenin toku (`ui-tok-a1`) ve panelin açılış vuşu (`ui-open`, 9 dB kısık). Zeyd vuşu ikinci bir tık gibi duydu.
- Aynı şey Durdur'da, X'te, Profil'de, bölüm listesinde ve tablo ayrıntısında da vardı.
- **Düzeltme:** Bir dokunuş artık tek ses çalıyor (`UiSounds.Take`). Dokunuşla açılan ya da kapanan panelde vuş çalmıyor. Vuş yalnızca panel kendiliğinden açılıp kapanınca çalıyor.
- **Denetim:** PlayMode dokunuş testi (8 ekran, 1088 dokunuş) her karede kaç UI, geçiş ve engel sesi çaldığını sayıyor; aynı karede iki ses olursa test kalıyor. WebKit testi (`Tools/webkit_buttons.js`) aynı karede iki UI sesi olursa kalıyor.

### Dalış sonu "klink"

- Tabloya değme anında çalan `gecis-ting` kaldırıldı; dalış whoosh'u ve çizgi sesi duruyor.
- Ting dosyası artık yüklenmiyor.
- Dinleme sayfasında da çalmıyor.

## 2. Sürükleme / çıkış sesi: Block Out tarzı

**Önce analiz:** `BLOCKOUT_SES_ANALIZ.md`. Özeti:

- **Sürüklerken:** Block Out'ta ses yok. Ses yalnızca bir şey olunca geliyor.
- **Bırakma:** Parça yerine oturunca 12–16 ms'lik, 395 Hz civarında tahta/plastik bir "tok" çalıyor. Her seferinde aynı kayıt, aynı perde, aynı düzey (±0,24 dB). Oyunun en yüksek sesi bu.
- **Çıkış:** Tok yok. Yerine 0,6 sn'lik kırçıl bir plastik ufalanma geliyor, kapıdaki parçacıklarla aynı karede.
- **Hipotezler:**
  - 1 doğru;
  - 2 yarı doğru ("şak" var, parıltı yok);
  - 3 yanlış (art arda çıkışlarda perde değişmiyor);
  - 4 yanlış (dokunuşta ses yok);
  - 5 ses ve görüntü için doğru, titreşim videoda ölçülemez.
- **Bizim eski sesimiz:** `cikis-f` 0,12 sn'lik bir hava "fıs"ı (3,4 kHz). Gövdesi yok; Block Out'un tersi.

**Uygulama:**

- **Sürüklerken ve kalkışta ses yok.** Yeni adaylarda takılmadan önceki kalkış sesi kapalı.
- **Çıkış sesi** parça tahta kenarını geçtiği karede çalıyor, kıvılcımla aynı anda (eskisi gibi). Ses değişti: tahta "tok" gövde ve kısa bir plastik kırıntı.
- **Her çıkışta aynı:** tek kayıt. Rastgele perde yok, hıza göre perde de yok.
- **Adaylar** (`Tools/make_sfx106.py`, CC0, lisanslar `LISANS.md`):

  | Harf | Ses | Süre |
  |---|---|---|
  | q | Tok + plastik kırıntı (Block Out'un iki sesi bir arada) **varsayılan** | 0,29 sn |
  | r | Yalın tok (Block Out'un bırakma toku) | 0,18 sn |
  | s | Plastik şak + kırıntı | 0,23 sn |
  | t | Pop + parıltı (PM'in 2. hipotezi) | 0,12 sn |
  | u | Tok + uzun ufalanma (Block Out'un çıkışı) | 0,56 sn |

- **Düzey:** Takılma toku düzeyinde (dosya +1 dB, oyun 1 dB kısık çalıyor). Tok −2 dB tepeyi aşmasın diye r 0,6 dB daha kısık.
- **Ardışık perde:** Hipotez 3 yanlış çıktığı için varsayılan kapalı. Zeyd denemek isterse `&cikisperde=1`: her çıkış yarım ton yukarı, 5 adımda başa dönüyor, takılma ya da kayıpta ve her bölümde sıfırlanıyor.
- **Dinleme sayfası:** "Sürükleme / çıkış (Block Out tarzı)" bölümü; her adayda "10 art arda", "Oyunda dene" (`&sescikis=`) ve "Block Out ile" var. "Block Out ile" önce referansın bırakma ve çıkış sesini, sonra adayı çalıyor. Referans yayına konmadığı için bu düğme yalnızca yerel önizlemede çıkıyor.
- **Varsayılan q:** Zeyd'in eski seçimi f, "çok kötü" dediği için değiştirildi. f hâlâ `&sescikis=f` ile açılıyor.
- **iPhone'da deneme:** Yeni adayların hepsi web'de ham örnek olarak da gidiyor, iPhone'da her biri bayrakla denenebilsin diye. Bayrakla açılan eski adaylar AAC kalıyor ve Safari bazılarını çözemiyor.

## 3. Zincir sesi neden zincire benzemiyordu

Ölçüldü:

| Neden | PM_105'teki durum |
|---|---|
| Dosya | Tek bir 0,5 sn'lik sallama kaydından (`chain_01..03`, rubberduck) kesilmiş 0,11 sn'lik **tek vuruş** parçaları. Halkaların birbirine sürtünmesi yok, tek tek "tss". |
| Tını | Enerjinin ağırlık merkezi **8,8 kHz**. Halka gövdesi yok, yalnızca tıs. Üstüne 500 Hz yüksek geçiren süzgeç. |
| Düzey | Dosya −24 LUFS (kaydırmanın 9 dB altı). Oyunda hıza göre %25–100 çalıyordu; 3 hücre/sn'lik bir sürüklemede ~−31 LUFS. Neredeyse duyulmuyordu. |
| Sıklık | Yarım hücrede bir parça, en az 70 ms arayla. Sürekli şıkırtı yerine seyrek tıklar. |
| Dönüştürme (PM_104_EK) | Sebep değil. Ham örnekler dosyanın aynısı (PM_104_EK testi). |
| Perde | ±%6 oynatma. Küçük katkı. |

**Düzeltme:**

- **Gerçek zincir kayıtları:** Freesound, CC0, sayfaları kontrol edildi.
  - d: alienistcog "chain-rattle-small" (varsayılan);
  - e: freemaster2 "Chains 2";
  - f: Kneeling "pickingchain".
- **Döngüler:** Her adayda kaydın en dolu ve en düzgün yerlerinden 3 kesintisiz döngü, 1,1–1,3 sn. 9 kHz üstü kesildi (tıs gitti), halka gövdesi kaldı. Test: enerjinin en az %10'u 4 kHz altında.
- **Çalma (`Sfx105.Bed`):** Zincir hareket ettiği sürece döngülerden biri çalıyor.
  - Biri bitince başka biri 80 ms'de üstüne geçiyor; aynısı üst üste gelmiyor.
  - Her yeni harekette döngünün rastgele bir yerinden başlıyor.
  - Düzey hızla artıyor: 1,2 hücre/sn altında sessiz, 9 hücre/sn'de tam. 50 ms'de açılıp 180 ms'de sönüyor.
  - Perde hızla %±4 değişiyor.
  - Tam hızda düzey takılma tokunun 5 dB altında.
- **Gerilme "çın"ı:** Metal çın ve aynı zincirin kısa bir şıkırtısı. d, e, f adayları var, 2'şer kayıt.
- **Dinleme sayfası:** "Zincir (gerçek zincir kaydı)" bölümü. "Sürükle" düğmesi 2,4 sn'de hızlanıp yavaşlayan bir sürüklemeyi oyunun mantığıyla çalıyor. "Oyunda dene" 31. bölümü açıyor.
- **Eski seçenekler:** a–c adayları `&seszincir=a` ile hâlâ açılıyor.

## 4. Lastik her yerde

### Denetim: hangi yollar lastiksizdi

| Yol | Durum | Düzeltme |
|---|---|---|
| Boşluksuz takılma (komşu parçaya, kırmızı duvara ya da dişe bitişik) | Lastik vardı ama ezilme 0,209'du, lastik 2'nin (0,38) **%55**'i. Zeyd'in "duvara çarpınca lastik yokmuş gibi" dediği bu: parçalar çoğu zaman duvara bitişik. | Bütün takılmalar aynı ezilme (0,38). |
| Zincirli parça, boşluksuz (PM_105'ten sonra) | Yerinde eziliyordu ama aynı %55 ezilmeyle. | Aynı düzeltmeyle %100. Zincir sınırı korunuyor: uçlar birlikte gidiyor, temas geçilmiyor. |
| Kilitli parça, açık bir çıkış yönüne fırlatılınca | Lastik **hiç yoktu**: 0,18 sn'lik düz geri dönüş. | Kilitli parça her yönde lastikle takılıyor. |
| Kilitli parçaya basınca | Yana titreme (dokunma tepkisi, takılma değil). Arkasından gelen lastikle çakışabiliyordu. | Lastik başlayınca titreme duruyor. |
| Boşluklu takılma (her engel) | Lastik 2, tam. | Değişmedi. |
| `&lastik=0` | Eski PM_75 çarpması (yalnızca bayrakla). | Değişmedi. |

### Ölçüm

`Pm106RubberTests` p23'ün kendi bölümleri üzerinde koşuyor (sl100):

- Rastgele 20 bölüm (tohum 106), bölümün çözümünün 0–yarısı oynandıktan sonra birer takılma.
- Her engel türü boşluklu ve boşluksuz birer kez: komşu parça, yandan takılma (diş), kırmızı duvar, zincirli parça, kilitli parça, bombalı parça.
- Takılma oyundaki gibi fırlatılarak çalıştırılıyor. Her karede en çok ezilen hücrenin boyu ölçülüyor.
- Eşik: lastik 2'nin %90'ı (0,342).

Tablo: `lastik_olcum.md`.

**Tahta kenarı:** Bu oyunda tahta kenarı çıkış. Kapalı kenar yalnızca kırmızı duvar, o da ölçüldü.

### Sonuç

32 takılmanın 32'si geçti; büzülme hepsinde 0,380 (lastik 2'nin %100'ü), 6 engel türünün hepsinde.

## Doğrulama

- **EditMode:** 816 test; 814 geçti, 2 atlandı, 0 kaldı. `Pm106Tests` şunları denetliyor:
  - tek ses;
  - ting yok;
  - analiz raporu ve hipotezler;
  - q–u tek kayıt, sabit perde, kalkış yok;
  - perde merdiveni yalnızca bayrakla;
  - zincir döngüleri ve çalma mantığı;
  - web'de ham örnekler;
  - lastik kodu;
  - dinleme sayfası.
- **PlayMode:** Bütün testler yeşil.
  - Dokunuş testi 1088 dokunuş: aynı karede iki ses 0, `ui-tok-a1` ×718, eski tık 0.
  - Lastik testi: 32/32.
  - Zincir testi (PM_105): 6/6.
- **WebKit dokunuş testi** (`webkit_dugmeler.json`, 7 ekran, WebKit 26.5): GEÇTİ.
  - eski çıt 0;
  - aynı karede iki UI sesi 0;
  - her ekranda tok;
  - uyarı 0, sayfa hatası 0.
- **Chrome'da zincir** (31. bölüm, sürükleme):
  - Zincir şıkırtısı çalıyor, döngüler dönüyor.
  - Zincirli parça takılınca `zincirger-d` çarpmayla aynı karede çalıyor.
  - Çıkışta `cikis-q1` çalıyor.
  - **Bulunup düzeltilen:** WebGL'de yeni başlayan ses kaynağı birkaç kare "çalmıyor" göründüğü için döngü 0,1 sn'de bir değişiyordu. Döngünün bitişi artık saatle hesaplanıyor.
  - Bölüm açılırken zincirin sallanması da şıkırtı çaldırıyordu. İlk 3 sn sessiz.
- **`verify_build`:**
  - yerel 4/4;
  - gönderimli 7/7 (TEST_PM106, TEST_PM106B);
  - canlı p23 4/4 (gh-pages cef27f30).
- **Dinleme sayfası = oyun:** `ses_eslesme.md`, 24 olay, uyuşmayan 0.
- **Boyut:** data 37,94 MB Brotli. PM_105'te 36,77 MB'tı.
  - +1,2 MB'ın kaynağı: yeni sesler, özellikle 9 zincir döngüsü, ve iPhone'da denenebilsinler diye bütün PM_106 adaylarının ham örnekleri.
  - Web'deki ham örnekler 2996 KB (70 dosya).
  - Zeyd seçince seçilmeyen adaylar çıkarılırsa yaklaşık 0,8 MB geri gelir.

## Doğrulanmayan

- Seslerin telefonda nasıl duyulduğu; Zeyd dinleyip seçecek.
- Zincir şıkırtısının gerçek parmakla sürüklerken düzeyi. Tam hızda takılma tokunun 5 dB altında.
- Lastiğin gerçek telefonda görünüşü. Ölçüm Unity'de kare kare yapıldı.
