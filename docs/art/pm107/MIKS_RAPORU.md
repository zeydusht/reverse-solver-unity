# PM_107 §C: ses miksajı raporu

Zeyd'in istediği: "Bir şey kulağımı tırmalamasın, ne çok olsun ne az; net ve temiz duyulsun. Saati durdurdum, parçaları kaydırırken tik takı hiç duymadım."

## Ne neden değişti

| Sorun (önce) | Sebep (ölçülen) | Ne yaptım |
|---|---|---|
| Saat açıkken tik-tak duyulmuyordu | Tik-tak dosyası −29,6 LUFS, kayma −15,5 LUFS. Tik-tak kaymanın 14 dB altındaydı, tamamen örtülüyordu. | Tik-tak −21,5 LUFS'e çıktı. Saat çalışırken kayma, çıkış ve öteki efektler 5 dB, müzik 3 dB kısılıyor (Mix107: Saat kanalı hiç kısılmaz). Kayma artık tik-takın 0,5 dB altında. |
| Dalışta neredeyse ses yok, sonda yalnız "fışt" | Whoosh −33 LUFS ve yalnız son 0,6 sn'de çalıyordu (yaklaşma 2–3 sn). | Yaklaşmanın başından kamera hızını izleyen bir hava sesi var: koridorun dokusu, hız eğrisi 30u²(1−u)², varışta sıfır. Sondaki whoosh −24 LUFS'e çıktı ve %55 düzeyde, havanın ucu gibi çalıyor. Kazanma → koridor → dalış da aynı yoldan geçiyor. |
| Kayma ve çıkış tizde tırmalıyordu | Seçilen çıkış (u) ile zincirin tizinde sert enerji vardı; saldırı ani. | Kayma, çıkış, zincir ve whoosh'larda 7 kHz'den yukarısı −4 dB (yüksek raf süzgeci), 4 ms yumuşak saldırı. Bütün dosyalarda 1 ms giriş ve 5 ms çıkış sönümü (tık yok). |
| Hızlı fiske serisinde sesler üst üste biniyordu | Aynı ses sınırsız sayıda üst üste çalabiliyordu. | Aynı ses aynı anda en çok 2 kez (Mix107.Admit). Web'de her ses bağlamının çıkışında sınırlayıcı: eşik −4 dBFS, oran 20, saldırı 2 ms. |
| Fanfar sırasında efektler | Fanfar ve efektler aynı düzeydeydi. | Fanfar çalarken efektler ve zincir 6 dB, düğmeler 3 dB kısık. Kazanma ekranından çıkınca hemen geri gelir. |
| Patlama ve kayıp | Müzik yalnız kayıpta kısılıyordu. | Bomba patlayınca müzik 1,6 sn boyunca 4 dB daha aşağı iner (kaybın kendi kısmasının üstüne). |
| Seviyeler dağınıktı | Dosyalar −15…−33 LUFS arasındaydı; birbirine göre ayarlanmamıştı. | Her seçili ses hedef düzeyine getirildi (aşağıdaki tablo). Tepeler −1,5 dBFS'in altında, sert kırpma yok. |

## Kanallar (Mix107, tek yer)

| Kanal | İçinde | Saat açık | Fanfar | Patlama |
|---|---|---|---|---|
| Müzik | lobi ve bölüm döngüsü | −3 dB | (bölüm müziği zaten kesik) | −4 dB, 1,6 sn |
| Efekt | kayma/çıkış, çarpma, engeller, booster sesleri | −5 dB | −6 dB | |
| Arayüz | düğme toku, panel vuşu, geçişler, dalış havası | | −3 dB | |
| Zincir | zincirin kesintisiz şıkırtısı | | −6 dB | |
| Saat | tik-tak | kısılmaz | | |
| Fanfar | kazanma fanfarı | | | |

Kısmalar yumuşak geçer: inişte saniyede 80 dB, çıkışta saniyede 20 dB; pompalama olmaz.

`&miks=0` hepsini kapatır: eski dosyalar, kısma yok, ses sınırı yok, dalış havası yok. `&limiter=0` sınırlayıcıyı kapatır.

## Senaryo kayıtları (önce → sonra)

Her kayıt oyunun kendi çıkışından (Chrome, gerçek GPU) alındı: efektler ve müzik ayrı; `Tools/pm107_miks.js`.

- **Önce:** `&miks=0&limiter=0`. Seçimler aynı, miksaj eski.
- **Ölçüm:** `Tools/pm107_miks_olc.py`.
- **Duyulmayan ses:** Aynı 400 ms'de çalan en yüksek şeyin (başka bir ses ya da müzik) 6 dB'den fazla altında kalan ses.
- **Tepe aşımı:** Efekt ile müzik toplamının −1 dBFS'i aştığı 10 ms'lik dilimler.

| Senaryo | ses sayısı (önce / sonra) | duyulmayan | tepe aşımı | tepe dBFS | efekt en yüksek LUFS | müzik LUFS | seviye |
|---|---|---|---|---|---|---|---|
| Bölüm oynanışı (10 çıkış, 3 takılma) | 15 / 15 | 0 → **0** | 2 → **0** | -0.93 → -1.5 | -14.3 → -15.1 | -35.3 → -34.6 | [grafik](miks/bolum/seviye.png) |
| Saat booster'ı açıkken kayma | 75 / 76 | 11 → **0** | 0 → **0** | -2.03 → -2.59 | -16.8 → -17.4 | -34.8 → -36.8 | [grafik](miks/saat/seviye.png) |
| Zincirli bölüm | 18 / 16 | 0 → **0** | 1 → **0** | -0.7 → -2.21 | -17.0 → -17.4 | -35.0 → -34.2 | [grafik](miks/zincir/seviye.png) |
| Bomba + kayıp | 23 / 23 | 0 → **0** | 2 → **0** | -0.8 → -1.66 | -17.0 → -17.4 | -36.1 → -35.3 | [grafik](miks/bomba/seviye.png) |
| Kazanma fanfarı (4. bölüm baştan sona) | 44 / 60 | 0 → **0** | 2 → **0** | -0.15 → -2.5 | -14.3 → -15.1 | -36.0 → -35.3 | [grafik](miks/kazanma/seviye.png) |
| Menü gezintisi (düğmeler) | 15 / 15 | 0 → **0** | 1 → **0** | -0.99 → -1.13 | -18.3 → -16.5 | -36.2 → -35.5 | [grafik](miks/menu/seviye.png) |
| Menüden Oyna: dalış | 7 / 37 | 0 → **0** | 0 → **0** | -2.79 → -2.69 | -21.3 → -19.3 | -36.2 → -35.4 | [grafik](miks/dalis/seviye.png) |

Ses kayıtları `miks/<senaryo>/efektler.webm` ve `muzik.webm`. Önce kayıtları `miks/once/<senaryo>/` altında.

## Dosya düzeyleri (Tools/make_miks107.py, Resources/Art/audio/pm107)

En yüksek 400 ms'lik dilim, LUFS. Hedefler:

- kayma / çıkış −17;
- çarpma −17,5;
- düğme −20;
- panel vuşu −23;
- zincir −22,5;
- zincir gerilme −17;
- patlama −16;
- tik-tak −21,5;
- booster'lar −18;
- koridor −25;
- dalış −24.

Hafif geçiş işaretleri (çizgi, geri, HUD, sekme) olduğu gibi, müziğin altında kaldı.

| Dosya | önce LUFS | sonra LUFS | tepe dBFS | yumuşatma |
|---|---|---|---|---|
| bomba-a1 | -17.7 | **-17.5** | -3.0 → -2.9 |  |
| booster-cekic-kirinti-c | -25.6 | **-21.5** | -3.0 → -1.5 | evet |
| booster-cekic-vurus-c | -17.6 | **-18.0** | -3.0 → -3.4 |  |
| booster-degnek-donusum-a | -18.6 | **-18.0** | -4.1 → -3.5 |  |
| booster-makas-kesim-c | -17.7 | **-18.0** | -3.0 → -3.4 |  |
| booster-saat-donma-b | -21.6 | **-18.0** | -3.0 → -1.6 |  |
| booster-saat-kirilma-b | -19.6 | **-18.0** | -5.9 → -4.3 |  |
| booster-saat-tiktak-b | -29.6 | **-21.5** | -11.9 → -3.8 |  |
| carpma-a1 | -16.8 | **-17.6** | -1.5 → -2.5 |  |
| carpma-a2 | -16.2 | **-17.5** | -1.5 → -2.9 |  |
| carpma-a3 | -16.5 | **-17.5** | -1.5 → -2.7 |  |
| carpma-a4 | -16.3 | **-17.5** | -1.5 → -2.8 |  |
| cikis-u1 | -15.5 | **-17.0** | -2.0 → -3.0 | evet |
| duvar-a1 | -15.6 | **-17.0** | -2.0 → -3.4 |  |
| duvar-a2 | -15.6 | **-17.0** | -2.0 → -3.4 |  |
| duvar-a3 | -15.4 | **-17.0** | -2.0 → -3.6 |  |
| gecis-cizgi-c | -39.3 | **-40.4** | -19.5 → -20.9 | evet |
| gecis-dalis-a | -33.0 | **-24.0** | -18.2 → -8.9 | evet |
| gecis-geri-a | -34.0 | **-34.5** | -20.2 → -20.7 | evet |
| gecis-hud-a | -32.0 | **-32.1** | -6.8 → -6.9 | evet |
| gecis-koridor-a | -28.0 | **-25.0** | -15.3 → -12.3 | evet |
| gecis-sekme-a | -32.0 | **-32.0** | -9.9 → -9.9 | evet |
| kilitac-a1 | -18.0 | **-18.5** | -3.1 → -3.6 |  |
| kilitac-a2 | -18.0 | **-18.5** | -2.0 → -2.7 |  |
| kilitdokun-a1 | -18.5 | **-18.5** | -2.0 → -2.4 |  |
| kilitdokun-a2 | -18.5 | **-18.5** | -2.0 → -2.4 |  |
| kilitdokun-a3 | -18.5 | **-18.5** | -2.0 → -2.4 |  |
| patlama-a1 | -15.5 | **-16.0** | -2.0 → -2.7 |  |
| ui-close1 | -22.4 | **-23.0** | -2.7 → -3.4 | evet |
| ui-close2 | -22.2 | **-23.0** | -2.5 → -3.3 | evet |
| ui-dull1 | -21.0 | **-21.1** | -1.0 → -1.9 |  |
| ui-dull2 | -21.0 | **-21.1** | -1.0 → -1.9 |  |
| ui-open1 | -22.4 | **-23.0** | -2.7 → -3.4 | evet |
| ui-open2 | -22.2 | **-23.0** | -2.5 → -3.3 | evet |
| ui-toggle1 | -20.5 | **-21.1** | -1.0 → -2.1 |  |
| ui-toggle2 | -20.9 | **-21.1** | -1.0 → -2.0 |  |
| ui-tok-c1 | -19.2 | **-20.0** | -3.0 → -3.8 |  |
| zincir-f1 | -21.7 | **-22.5** | -6.8 → -6.5 | evet |
| zincir-f2 | -21.5 | **-22.5** | -2.0 → -2.6 | evet |
| zincir-f3 | -22.1 | **-22.5** | -2.0 → -2.8 | evet |
| zincirger-f1 | -16.5 | **-17.0** | -2.0 → -2.7 |  |
| zincirger-f2 | -16.5 | **-17.0** | -2.0 → -2.7 |  |

## Doğrulanmayan

- **Telefon:** Ölçümler bilgisayarda Chrome'da yapıldı; iPhone hoparlöründe dinleme Zeyd'de. Bilgisayarda doğru duyulan dengenin telefonda da korunması bekleniyor, çünkü hedefler göreli. Telefon hoparlörü 200 Hz altını keser; çarpma ve kaymanın gövdesi 300 Hz üstünde.
- **Sandık ekranı:** Ayrıca kaydedilmedi. `&sandik=1` kazanma ekranını ilk dokunuştan önce açıyor; webde ses ilk dokunuşla açıldığı için o anda ses yok. Gerçek sandık bölümü 3 ve 5 ise ders ya da kayıt aracının oynayamadığı bölümler. Sandığın sesleri efekt ve fanfar kanallarında, aynı kısma kurallarıyla.
- **iOS uygulaması:** Sınırlayıcı yalnız web'de (WebAudio). iOS'ta kanallar ve kısmalar aynı; sınırlayıcının yerini dosyaların −1,5 dBFS tepe sınırı ve 2 ses sınırı tutuyor.
