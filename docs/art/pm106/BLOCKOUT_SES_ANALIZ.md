# PM_106 §2A: Block Out'ta sürükleme sesi neden iyi hissettiriyor

Kaynak: `ref_blockout_oynanis.mp4`, 52 sn, 60 kare/sn, 44,1 kHz. Yalnızca referans; oyuna girmez.

## Nasıl ölçüldü

- **Dokunma ve bırakma anları:** Seçili parçanın beyaz çerçevesi kare kare izlendi. Çerçevenin belirdiği kare dokunma, kaybolduğu kare bırakma anı.
  - 73 hamle bulundu.
  - Elle de kontrol edildi: 5,8–6,8 sn ve 30,0–31,3 sn aralıkları 30 kare/sn'de tek tek izlendi.
- **Ses olayları:** 4 ms'lik ses zarfında ani yükselişler (≥14 dB) arandı. Her olay için tepe düzeyi, süresi, saldırısı ve spektrumu ölçüldü.
- **Ayırma:** Müzik ve olaylar HPSS (harmonik/vuruşlu ayırma) ile ayrıldı.
- **Aynı sample mı:** Olaylar birbirleriyle çapraz ilintiyle karşılaştırıldı. Perde farkı için 0,84–1,20 arası yeniden örnekleyip spektrum eşleşmesine bakıldı.
- **Betikler:** scratchpad'de; ham sayılar aşağıda.

## Ne zaman hangi ses çıkıyor

| An | Ses | Ölçüm |
|---|---|---|
| Dokunma (parça seçilir) | **yok** | Dokunuşlarda bırakma tıkının imzası yok (eşleşme 0,28; rastgele anlarda da 0,28). Ses düzeyi dokunmadan sonra yükselmiyor (ortanca −5,9 dB, yani düşüyor). |
| Sürükleme sırasında | **yok** | Sürükleme karelerinde ortanca düzey −63 dB, tıkın 50 dB altı. Parça hızlı ya da yavaş gitse de sessiz. |
| Bırakma: parça yeni yerine oturur | **"tok"**, oyunun en yüksek sesi | 44 hamlenin 44'ünde. Bırakma karesinden 0–50 ms sonra (ortanca 36 ms, 2 kare). Tepe −8,7 dB, sapma ±0,24 dB. |
| Bırakma: parça yerinden oynamadan geri döner | **yok** | 18 hamle; geçersiz ya da boş hamle sessiz. |
| Çıkış: parça kapıya varır, ufalanır | **"kırçıl ufalanma"**, tık yok | 11 çıkış hamlesi. Ses parça kapıya değdiği karede başlıyor. Kapıda parçacıklar çıkıyor, sayaç aynı karede bir azalıyor. Tepe −20 dB, 0,35–0,7 sn. |
| Art arda çıkışlar | **aynı ses, aynı perde** | 15 çıkış sesinin hepsinde perde oranı 1,000; spektrum eşleşmesi 0,91–0,97. Düzey −27 ±0,4 dB; 3 çıkış 3 dB daha dolgun (uzun parçalar). |
| Bölüm sonu | kısa tınılı jingle, sonra tok bir vuruş | 48,8 sn (melodik, 870 Hz) ve 51,2 sn. |

**Müzik:** Kayıtta müzik ya hiç yok ya da çok kısık. Olaylar arasında düzey −60…−70 dB, yani tıkın 50 dB altı. Harmonik bileşen yalnız bölüm sonundaki jingle'da yükseliyor. Olay sesleri müzikle yarışmıyor.

## Seslerin tınısı

### Bırakma toku

- **Kayıt:** Her seferinde aynı kayıt.
  - 9 örnek birbirine 0,98–0,99 ilintili.
  - Düzey farkı en çok 0,3 dB.
  - Perde oynatması yok.
- **Saldırı:** 4 ms'den kısa.
- **Gövde:** −20 dB'ye 12–16 ms'de iniyor. Arkasında çok kısık (−30…−40 dB) ~200 ms'lik bir çınlama var.
- **Enerji:** 395 Hz'de toplanıyor. Kısmi tonlar 123, 216, 335, 456 ve 519 Hz.
- **Karakter:** Tahta ya da kalın plastiğin "tok"u, Lego parçasının yuvaya oturması gibi. Tiz parıltı yok, bas gümbürtü de yok.
- **Bizim seslerle karşılaştırma:**
  - Zeyd'in Durdur'da beğendiği `ui-tok-a1` bu sese çok yakın: saldırı 3 ms, enerji 476 Hz civarında, 57 ms.
  - Bizim şimdiki çıkış sesi `cikis-f` başka bir şey: 0,12 sn hava "fıs"ı. Saldırı 18 ms, ağırlık merkezi 3,4 kHz, gövdesi yok.

### Çıkış ufalanması

- **Süre:** Yaklaşık 0,6 sn.
- **İki kısım:**
  1. İlk 20–50 ms'de ağırlık merkezi ~1,4 kHz olan bir çıtırtı.
  2. +0,2 sn'de ikinci, daha tok bir kırıntı (~1 kHz). Enerjinin %40–50'si 300 Hz altında, %30'u 3–8 kHz arasında.
- **Karakter:** Tonal değil, gürültülü ve tanecikli. Plastik parçacıkların dökülmesi ya da ufalanması gibi.
- **Düzey:** Tepede toktan 11 dB kısık. 400 ms'lik ortalamada yalnızca 2 dB kısık (−26 ve −24 dB): uzun olduğu için dolgun duyuluyor.

## PM'in hipotezleri

| # | Hipotez | Sonuç | Kanıt |
|---|---|---|---|
| 1 | Ses sürüklerken değil, sonuçta çalıyor | **Doğru** | Sürükleme 73 hamlede de sessiz (−63 dB). Ses yalnızca parça yerine oturunca (44/44) ya da çıkınca (11/11) çalıyor. Boş hamle sessiz (18/18). |
| 2 | Çıkış sesi kısa, tok, tatmin edici bir "pop/şak" + parıltı | **Yarı doğru** | "Tok/şak" var ama bırakmanın sesi: 12–16 ms, 395 Hz. Çıkışın kendisi 0,6 sn'lik kırçıl bir ufalanma. Parıltı yok: tonal tiz bileşen ölçülmedi, tiz kısım gürültü. |
| 3 | Art arda çıkışlarda perde hafifçe yükseliyor | **Yanlış** | 15 çıkışta perde oranı 1,000. Art arda 3–5 çıkışlı iki seride (41–43 sn, 44–48 sn) de değişmiyor. |
| 4 | Dokunma anında çok hafif bir "tık" | **Yanlış** | Dokunuşta ses yok (yukarıdaki iki ölçüm). |
| 5 | Ses, görsel patlama ve titreşim aynı karede | **Ses ve görüntü için doğru; titreşim ölçülemez** | Tok, oturma karesinden 0–3 kare sonra. Ufalanma, kapıdaki parçacıklar ve sayaç aynı karede. Titreşim videoda görünmüyor. |

## Neden iyi hissettiriyor

1. **Sessizlik ödülü belirgin kılıyor.** Sürükleme sırasında hiç ses yok. Ses yalnızca bir şey *olduğunda* geliyor: parça yerine oturduğunda ya da çıktığında. Boş hamle sessiz. Ses "hamle geçerli" onayına dönüşüyor.
2. **Tok kısa, tok ve hep aynı.** 12–16 ms'lik, 400 Hz civarında tahta ya da plastik bir "tok". Her hamlede aynı kayıt, aynı perde, aynı düzey: ritmik, öngörülebilir, yormuyor. Tiz değil, kulağı tırmalamıyor. Bas değil, telefonda kaybolmuyor.
3. **Oyunun en yüksek sesi bu.** Müzik yok ya da 50 dB altta. Tok her şeyin üstünde net duyuluyor.
4. **Çıkışın kendi sesi var.** Çıkışta tok çalmıyor. Yerine daha uzun, gürültülü, tanecikli bir ufalanma geliyor, kapıdaki parçacıklarla aynı karede. "Yerine oturdu" ile "gitti" kulakta ayrı.
5. **Ses ve görüntü aynı anda.** Tok oturmanın, ufalanma parçacıkların karesinde. Ses görüntünün sonucu gibi duyuluyor.
6. **Seri ödülü perdeyle verilmiyor.** Art arda çıkışlar aynı sesle. Tatmin ritimden geliyor, tırmanan perdeden değil.

## Bizim oyuna çevirisi (§2B'nin tasarımı)

Bizde parça sürüklenip bırakılınca ya tahtadan çıkıyor ya da bir engele takılıp lastikle geri dönüyor. Takılmanın sesi zaten "tok" (çarpma a, Zeyd beğendi). Çevirisi:

| Block Out | Bizde |
|---|---|
| Sürüklerken ses yok | Aynı; zaten yok. Kalkış sesi (`kalkis`) yeni adaylarda kapalı. |
| Dokunuşta ses yok | Aynı; zaten yok. Kilitli parça kendi sesini korur. |
| Yerine oturma toku | Bizde "yerine oturma" yok. Hamlenin sonucu ya çıkış ya takılma. Takılmanın toku zaten var. |
| Çıkış ufalanması (tok yok) | **Çıkış sesi:** parça tahta kenarını geçtiği karede, kıvılcım ve nefesle aynı karede. **Tahta/plastik tok gövde** (Block Out'un en tatmin edici sesi) ile kısa bir **plastik kırıntı** kuyruğu. "Hava fısı" yok. |
| Hep aynı kayıt, aynı perde | Yeni adaylarda tek kayıt. Rastgele perde yok, hıza göre perde de yok. |
| Seri perdesi yok | Varsayılan kapalı. Zeyd denemek isterse `&cikisperde=1`: her çıkış yarım ton yukarı, 5 adımda başa döner, takılma ya da kayıpta sıfırlanır. |

### Adaylar

Hepsi CC0 kayıtlardan, tek kayıt.

- **q:** Block Out tarzı: tahta tok + plastik kırıntı.
- **r:** Yalın tok: yalnızca Block Out'un bırakma toku gibi, kuyruksuz.
- **s:** Plastik şak + kırıntı: fiş ya da jeton çarpışması.
- **t:** Pop + parıltı: PM'in 2. hipotezi.
- **u:** Ufalanma: tok + Block Out çıkışı gibi uzun kırıntı.

## Ham sayılar

| Ölçüm | Değer |
|---|---|
| Hamle (dokunma → bırakma) | 73 |
| Toklu hamle | 44 |
| Çıkış | 11 |
| Sessiz (boş hamle) | 18 |
| Tok sayısı (kayıtta) | 54; tepe −8,73 dB, sapma 0,24 dB |
| Tok gecikmesi (bırakma karesinden) | ortanca 36 ms |
| Çıkış sesi | 12 olay; tepe ortalaması −20,2 dB |
| Sürüklemede düzey | ortanca −62,6 dB |
| Tokun zarfı | ortanca −12,6 dB |

**Referans kesitleri** `referans/` klasöründe; yalnızca yerel karşılaştırma için, yayına konmaz:

- `blockout_birakma.wav`
- `blockout_cikis.wav`
- `blockout_seri.wav`
- `blockout_5sn.wav`
