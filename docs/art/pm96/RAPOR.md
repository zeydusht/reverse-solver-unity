# PM_96 RAPOR

**Önemli:** Ben ses dinleyemiyorum. Seçimleri sayfa tariflerine, çalgılara ve ölçümlere göre yaptım (tempo, vurmalı oranı, parlaklık, döngü dikişi, ses yüksekliği). Kulakla seçim Zeyd'in.

## 1. Zeyd'in seçimleri varsayılan

- **Varsayılanlar:**
  - müzik c (artık lobi müziği);
  - kazanma b;
  - çıkış c, çarpma a, son parça a, tık b.
- Diğer adaylar sayfada ve bayrakla duruyor.
- `&muzik=` eski bayrak; artık lobi müziğini seçer.

## 2. Referans ölçümü (Block Out!, yalnız ölçüm)

Dosyalar `docs/art/pm96/ref/`. Oyuna ve repoya girmedi; yerelde dışlandı.

| | Tempo | Vurmalı oranı | Parlaklık (spektral ağırlık merkezi) |
|---|---|---|---|
| Lobi | 136 | 0,47 | 1550 Hz |
| Bölüm | 136 | **0,92** | 1560 Hz |

Bölüm müziğinin enerjisini neredeyse tamamen davul ve perküsyon taşıyor. Bölüm müziği varsayılanını buna göre seçtim.

## 3. Yeni adaylar

Lisanslar `LISANS.md`'de. Hepsi CC0; atıf notu olan sayfalar alınmadı. Bütün döngüler −18 LUFS ve dikişlerin hepsi tıksız: dikişteki adım parçanın %99,9 adımından küçük.

**Lobi (`&lobi=`), sakin, arkada kalan:**

| | Parça | Ne | Döngü |
|---|---|---|---|
| **c (varsayılan)** | Cozy Puzzle In-Game 2 | Zeyd'in seçimi | 128 sn |
| n | Urban Shop (MintoDog) | trompet, saksafon, sıcak | 83 sn, 92 BPM |
| o | Bunny Dust (Kistol) | piyano, çağdaş klasik; "old money" dokunuşu | 70 sn |
| p | Etirwer (Kistol) | naylon telli gitar, en sakini | 130 sn |
| q | Sci-fi Puzzle Stage Select (MintoDog) | synth, bir bulmaca oyununun bölüm seçim müziği | 64 sn, 105 BPM |

**Bölüm (`&bolummuzik=`), enerjik:**

| | Parça | Ne | Döngü |
|---|---|---|---|
| **r (varsayılan)** | Drive (Zane Little Music) | caz-füzyon, funk; en vurmalı aday (0,30), 136 BPM | 60 sn |
| s | Sci-fi Puzzle In-Game 1 (MintoDog) | synth, bulmaca bölüm müziği, 138 BPM | 139 sn |
| t | Cool Adventure (MintoDog) | synth, en hareketli (saniyede 7,2 vuruş) | 108 sn, 124 BPM |
| u | Cool City (MintoDog) | synth, saksafon | 108 sn, 124 BPM |
| v | Sci-fi Puzzle Title (MintoDog) | parlak synth | 80 sn, 120 BPM |
| w | Urban Theme (MintoDog) | parlak saksafon, bakır | 88 sn, 115 BPM |

**Neden r:** referansla aynı tempo (136) ve adayların en vurmalısı.

**Döngü notu:** Drive'ın bestecisi döngü yapmamış. Parçanın kendini tekrar ettiği 60 sn'yi bir vuruş başından kestim, dikiş 120 ms'lik çapraz geçiş. Tekrarlar elle çalınmış, örneği örneğine aynı değil. Bu yüzden dikişi kulakla bir kez dinlemek iyi olur ("Döngü geçişi" düğmesi).

**Kazanma fanfarı (`&kazanma=`), 1,5–2,5 sn, sayılabilir vuruşlar:**

| | Fanfar | Ne | Vuruş |
|---|---|---|---|
| **b (varsayılan)** | Pizzicato zafer | Zeyd'in seçimi | 7 |
| d | Trop la win (Komiku) | gerçek bir parça, neşeli ve hızlı; 2,5 sn'de solar | 7 |
| e | Marimba geçidi | bizim bestemiz: marimba + glockenspiel merdiveni, büyük Do akoru, zil, timpani | 6 |
| f | Ksilofon zıplayışı | bizim bestemiz: ksilofon + arp; en güçlü vuruşta zil, sonda çan parıltısı | 6 |
| g | Piyano ta-da | bizim bestemiz: yükselen üç akor, bir nefes, büyük Do + boru çanları | 5 |

- **e, f, g:** CC0 gerçek çalgı kayıtlarından (VCSL) bizim melodilerimizle çalındı. Sebep: CC0 kaynaklarda 1,5–2,5 sn'lik, net 5–7 vuruşlu ve parlak biten hazır fanfar yok. Kenney jingle'ları 1,4 sn'den kısa; diğerleri ya uzun ya da 2–3 vuruşlu.
- **Ses düzeyi:** yeniler b'nin ses yüksekliğine eşitlendi (−12,9 LUFS).

## 4. Oyunda iki müzik

- **Menüde lobi, bölümde bölüm müziği.**
  - Dalışta, bölüm resmin içinden çıkarken (geri çekilme) lobi müziği 0,6 sn'de solar, bölüm müziği 0,6 sn'de girer.
  - Menüye dönüşte tersi olur.
  - Lobi müziği kaldığı yerden devam eder; bölüm müziği her bölümde baştan başlar.
- **Kazanma:**
  - fanfar başlarken bölüm müziği 0,15 sn'de kesilir;
  - fanfar bitince lobi müziği 1 sn'de geri gelir;
  - fanfar yüklenemediyse de müzik lobiye geçer.
- **Kayıp ve süre doldu:** bölüm müziği %40'a iner. Devam edilirse geri gelir; tekrar oynayınca ya da menüye dönünce zaten değişir.
- **Anahtarlar:** müzik anahtarı yalnız müziği kapatır. Fanfar bir efekt sayılır; ses anahtarı kapalıysa sessiz çalar ama saati işlemeye devam eder.
- **Bayraklar:** `&lobi=`, `&bolummuzik=`, `&kazanma=`.
  - Harf verilir; `0` eski müzik ya da ses.
  - Eski `&muzik=` ve `&seskazan=` de çalışır.
- **Yükleme:**
  - İlk dokunuşta önce o an çalacak döngü yüklenir, sonra öteki, sonra fanfar.
  - Müzik dosyaları build'de değil, GitHub Pages'ten gelir. Build verisi +0,24 MB: yeni fanfarlar ve vuruş haritası, iOS ve editör için.

## 5. Kazanma ekranı vuruş vuruş

**Vuruş haritası**

- `Tools/make_beatmap96.py` her fanfarın vuruşlarını kendisi çıkarır:
  - spektral akı ile vuruş tespiti;
  - her vuruşu 1 ms'lik ses zarfının en dik yükselişine oturtur;
  - en güçlü 5–7 vuruşu seçer (en az 0,14 sn arayla).
- Sonucu `Resources/Art/audio/pm96/vurus_haritasi.json`'a yazar. Fanfar değişince aracı yeniden çalıştırmak yeter.
- Fanfar başına vuruş sayısı:

| 0 | a | b | c | d | e | f | g |
|---|---|---|---|---|---|---|---|
| 6 | 7 | 7 | 7 | 7 | 6 | 6 | 5 |

**Olaylar**

- 1. vuruş: ışık açılır; Jigo ışıkla belirir.
- 2. vuruş: "Harika!" 1,3×'ten "tak" diye oturur.
  - Harf harf yapılmadı: başlık görselindeki harfler ortak bir dış çizgiyle birbirine bağlı, kesilse dikiş görünürdü.
- Aradaki vuruşlar: her vuruşta bir kurtarılmış Jiggle zıplayarak gelir. En çok 4 Jiggle; kaç tane olduğu boş vuruş sayısına göre.
- En güçlü vuruş: konfeti patlaması, beyaz parlama ve Jigo'nun zıplaması.
- Son vuruş: +50 sayar, sandık çubuğu ve düğmeler gelir.
- Sandıklı bölümde sandık fanfardan sonra gelir.
- Dokunmak her şeyi hemen tamamlar, fanfar sürer.

**Saat**

- Ekranın saati fanfarın **hoparlörde çalan** konumu (WebAudio çıkış zaman damgası).
- Her olay vuruştan 15 ms önce, artı bir kare (ekranın kareyi bir tazeleme sonra göstermesi için) başlar.
- Fanfar, ekranın ilk karesi çizildikten sonra başlar. İlk kare ağır (ekran kuruluyor, resimler GPU'ya gidiyor); ilk denemede 95–300 ms'lik bu takılma ilk vuruşa denk geliyordu. Artık denk gelmiyor.

**Ölçüm (`kazanma_senkron.mp4`, `kazanma_olcum.json`)**

- **Kayıt:**
  - 3. bölüm sl100 çözümüyle kazanıldı; gerçek GPU'lu Chrome, 390×844.
  - Kayıt sayfanın içinden alındı: oyunun tuvali 60 fps, müzik bağlamının sesi.
  - Ses, ölçülen çıkış gecikmesi (49 ms) kadar geciktirildi; yani kaydın sesi hoparlörün çaldığı an.
  - Görüntüde her olay, yerinin değiştiği ilk kare. Ses tarafında fanfar, WAV'ıyla çapraz korelasyonla bulundu (benzerlik 1,00).
- **Fanfar b, 7 olay:**

| | ışık | başlık | Jiggle 1 | Jiggle 2 | konfeti + Jigo | Jiggle 3 | altın |
|---|---|---|---|---|---|---|---|
| kayıtta (çizildiği an) | −39 | −31 | −38 | −33 | −27 | −26 | −30 ms |
| oyunun kendi saatiyle | −31 | −23 | −30 | −26 | −22 | −18 | −24 ms |

- Kayıt kareyi **çizildiği an** alıyor; telefon ekranı onu bir tazeleme (16,7 ms) sonra gösterir. Ekranda olaylar vuruştan **10–23 ms önde, ortalama 15 ms**. Tarif "10–20 ms önde" idi; yalnız ışık 23 ms ile sınırın hemen dışında. Olaylar arasındaki fark 13 ms, bir kareden az.
- **Kayıttaki uzaklık sınırı (≤ 17 ms) kayıtta karşılanmıyor (26–39 ms).** Sebep, bilerek eklenen o bir karelik pay: kayıt kareyi ekrandan bir tazeleme önce alıyor.
  - Kayıtta da ≤ 17 ms isteniyorsa, payı kaldırmak tek sabitlik bir değişiklik.
  - O zaman telefonda olaylar vuruşla aynı anda, ya da birkaç ms sonra gelir. Hangisi istendiğine PM karar versin.
- **Video:** 5,4 sn, 60 fps, sesli, 0,67 MB.
  - Yalnız müzik bağlamının sesi var (bölüm müziği, fanfar, lobi dönüşü).
  - Oyunun efektleri (son parça sesi) ayrı bir ses bağlamında; videoda yok.

**Araçlar**

- `Tools/pm96_record.js`: kayıt. PM_96 müzik dosyalarını yerelden verir.
- `Tools/pm96_measure.py`: ölçüm ve video.

## Doğrulanmayan

- Seslerin kulağa nasıl geldiği; Drive'ın dikişi.
- iPhone Safari'de çıkış gecikmesi ve senkron.
- iOS (App Store) build'i hâlâ tek ve eski müzikte. İki müzik ve fanfar senkronu yalnız web'de; iOS'ta fanfar Unity'nin ses saatiyle çalışır ama müzik geçişi yok.
- Ses kaynakları (`Assets/Audio/Music96`, `Fanfare96`) PM_86/95'teki gibi repoya konmadı; araçlarla yeniden üretilir.
