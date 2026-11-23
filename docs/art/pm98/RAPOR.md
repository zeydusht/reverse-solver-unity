# PM_98 RAPOR

**Önemli:** ben ses dinleyemiyorum. Bütün seçim ve ayarlar ölçümle yapıldı (tempo, vurmalı oranı, parlaklık, döngü dikişi, ses yüksekliği). Kulakla karar Zeyd'in.

## 1. Seçimler varsayılan

- **Müzik:** lobi n (Urban Shop), bölüm c (şimdilik), kazanma b.
- **Efektler:** çıkış c, çarpma a, son parça a, tık b.
- Hepsi bayrakla değiştirilebilir: `&lobi= &bolummuzik= &kazanma= &sescikis= …`.

## 2. Yeni bölüm müziği adayları (`&bolummuzik=A…G`)

Ölçü referansla aynı: vurmalı oranı = HPSS ile ayrılan vurmalı enerjinin payı, ilk 60 sn. Referans `ref_bolum`: 136 BPM, vurmalı oranı **0,92**, parlaklık 1560 Hz.

| | Ad | Ne | Tempo (ölçülen) | Vurmalı | Parlaklık | Döngü |
|---|---|---|---|---|---|---|
| **A** | Marimba Pop | bizim beste; marimba motifi, arp, dört vuruşlu davul + el çırpma | 136 (136) | **0,80** | 1850 Hz | 70,6 sn |
| **B** | Ksilofon Arcade | bizim; ksilofon + glockenspiel, kırık davul, tef | 138 (136*) | 0,78 | 2470 Hz | 69,6 sn |
| **C** | Kalimba Zıplama | bizim; kalimba, arp, dört vuruşlu | 132 (129*) | 0,81 | 1970 Hz | 72,7 sn |
| **D** | Piyano Koşu | bizim; glockenspiel motifi, piyano vuruşları, kırık davul | 136 (136) | 0,79 | 2650 Hz | 70,6 sn |
| E | Cat in Space (mutantleg) | CC0, tuhaf ve zıplayan | 120 | 0,63 | 2290 Hz | 32 sn |
| F | Danza D' Bots II (lushogames) | CC0, funk, elektronik | 110 | 0,57 (eşik altı) | 1430 Hz | 87 sn |
| G | Upbeat Beat (centurionofwar) | CC0, retro beat | 120 | 0,55 (eşik altı) | 1860 Hz | 40 sn |

\* Ölçülen tempodaki küçük fark ölçüm aracının tempo ızgarasından geliyor. Besteler sabit tempoda üretildi.

**A–D: kendi bestelerimiz** (`Tools/make_level98.py`, nota planı `BESTE.md`).

- **Yapı:** 4/4, 40 ölçü.
  - A: 8 ölçülük motif iki kez.
  - B: cevap motifi; tef her arka vuruşta, ek bas davul.
  - A′: motifin 8 ölçüsü.
  - Her 8 ölçünün son vuruşunda trampet geçişi.
- **Davul:** gerçek kayıtlar (VCSL): bas davul + cajon, trampet, el çırpma, hi-hat, shaker, tef.
- **Bas:** bizim sentezimiz, sekizlik.
- **Ezgi:** marimba, ksilofon, glockenspiel, arp, kalimba, piyano kayıtları.
- **Ritim önde:** melodi kanalının seviyesi, vurmalı oranı 0,80 olacak şekilde araçta otomatik ayarlandı. Sonuç 0,78–0,81, istenen 0,7–0,9 aralığında.
- **Parlaklık:** zil sesleri kısılarak A ve C referansa yaklaştı (1850 / 1970 Hz). B ve D'de ksilofon ve glockenspiel ana çalgı olduğu için daha parlak (2470 / 2650 Hz).
- **Döngü:** yuvarlak çizim; son ölçünün kuyrukları başa eklenir. Dikiş adımları parçanın olağan adımının çok altında (0,0008–0,003 < 0,12). −18 LUFS.
- **Tempo:** sabit (vuruş aralığı sapması A ve D'de 0 ms).

**E–G: CC0 araması** (`Tools/make_music98.py`, `LISANS.md`).

- OpenGameArt'ta 532 CC0 sayfası tarandı (arcade, edm, dance, drums, beat, funk, house, casual, hyper casual …). PM_96 havuzuyla yaklaşık 330 parça ölçüldü.
- **0,6 eşiğini geçen ve temposu uyan yalnız E var.**
  - Next to You (0,70) da eşiği geçiyor ama 92 BPM'lik duygusal bir parça; elendi.
  - Gerçek CC0 oyun müziklerinde bu oran neredeyse hiç 0,5'i geçmiyor. Referansın 0,92'si ancak kendi bestemizle yakalanabildi.
- F ve G sayfada "eşik altı, karşılaştırma için" diye duruyor.
- E'nin ve G'nin döngüsü kısa (32 / 40 sn); parçaların kendisi o kadar.

**Dinleme sayfası** (`p23/onizleme/sesler.html`):

- En üstte "Bölüm (yeni)" bölümü.
- **Karşılaştır (ref → aday):** önce referansın 10 sn'si, sonra adayın 10 sn'si.
  - Referans yayına konmadı. Düğme yalnız repoyu yerelde açınca görünür; Pages'te görünmez.
  - Sayfa referansı bulamazsa düğmeyi göstermiyor.
- Yerelde 7 adayın 7'si tarayıcıda çözüldü.

## 3. Duraklatma

- **Durdur'a basış:** tık + Select titreşimi + panel açılış vuşu.
- **Durdur ya da Ayarlar paneli açıkken:**
  - müzik durmaz, 0,15 sn'de %35'e iner;
  - oyunun kendi sesleri durur (`AudioListener.pause`);
  - UI sesleri ayrı kaynaktan çalar, duyulur.
- **Devam:** müzik 0,3 sn'de eski seviyesine döner.
- Menüdeki Ayarlar paneli de aynı kuralda.
- Durum her karede panellerin kendisinden okunuyor; hangi yoldan kapanırsa kapansın takılı kalmaz.

## 4. UI sesleri duyulur oldu

**Ölçülen sorun:**

- Tık sesi oyunda yaklaşık **−32 LUFS**'ta duyuluyordu: −25,5'lik kayıt 0,45 seviyede çalıyordu.
- Müzik ise yaklaşık −27 LUFS'ta: −18'lik dosya 0,35 seviyede. Yani tık, müziğin 5 dB altındaydı.

**Şimdi** (en yüksek 400 ms ile ölçüldü):

- tık −14…−15 LUFS (≈ 18 dB yükseldi);
- anahtar −14…−15;
- panel vuşları −17;
- pasif tok −15.

**Efektler:**

- çıkış artık tam seviyede, −19,5;
- çarpma −22;
- son parça −9;
- fanfar −13.

Hiyerarşi: UI ve ödül sesleri önde, sık duyulan çıkış onların altında, müzik en altta.

**Yeni ses tipleri** (kayıtlı CC0; `Tools/make_ui98.py`):

- panel açılışı: kısa yumuşak swish;
- kapanış: aynı sesin tersi;
- anahtar: gerçek düğme;
- pasif düğme (kilitli booster, yetmeyen altın): tahta "tok".

**Kapsananlar:**

- alt sekmeler, Oyna;
- Durdur, Devam, Menü, Tekrar;
- Ayarlar anahtarları ve dil;
- booster seçimi, kilitli booster;
- HUD düğmeleri;
- sandık dokunuşları;
- kazanma düğmeleri ve reklam (2× altın);
- devam paneli, Koleksiyon kartları, profil ve liste panelleri, kapatma (X).

**Denetim** (`OpeningFlowTests.EveryTapHasSoundAndTap`):

- **Ekranlar:** menü, Koleksiyon, Jiggle'lar, Ödüller, Ayarlar, bölüm, Durdur, kazanma.
- **Yöntem:** her ekranda 8 × 17 noktalık dokunuş ızgarası; toplam **1088 dokunuş**. Bunların **152'si** ekranı değiştirdi (panel, sekme, anahtar, bölüm, booster …).
- **Sonuç:** sesi ya da titreşimi olmayan: **0**.
- Liste `docs/TITRESIM_HARITASI.md`'de "Ses" sütunuyla.

## 5. Titreşim notu

- Ayarlar'da ve Durdur panelinde titreşim anahtarının altında, **yalnız web'de**, "Titreşim uygulamada çalışır" yazıyor.
- **iOS:** her UI öğesinin titreşim çağrısı, yukarıdaki testte `Haptics.Asked` ile doğrulandı. Bu çağrı, iOS uygulamasının da geçtiği yol; platform koduna (`RS_Haptic`) gitmeden önce çalışır.

## Doğrulanmayan

- Seslerin ve bestelerin kulağa nasıl geldiği.
- iPhone'da gerçek titreşim; TestFlight build'i bu işte alınmadı.
- iOS build'inde iki müzik ve panel kısması hâlâ yok; orada tek ve eski döngü çalıyor.
