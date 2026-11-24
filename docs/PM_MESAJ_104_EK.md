# PM_MESAJ_104_EK: ACİL: iPhone Safari'de "EncodingError: Decoding failed" (ses çözülemiyor)

**PM_104'ün 1. maddesinden önce yap.** Kapsam: yalnız ses dosyaları ve ses yükleme.

## Hata
Zeyd'in iPhone'unda (Safari, p23) oyun sırasında tekrar tekrar uyarı çıkıyor:

```
An error occurred running the Unity content on this page. See your browser JavaScript console for more info. The error was: EncodingError: Decoding failed
```

## PM tahmini
Bu, tarayıcının bir ses dosyasını çözememesi (WebAudio `decodeAudioData`). **iPhone Safari bazı ses biçimlerini çözemez**, özellikle **OGG Vorbis** ve bazı Opus/WebM dosyaları. Kenney ve OpenGameArt seslerinin çoğu OGG.

Sonuçlar:
- Çözülemeyen ses çalmıyor. Yerine başka ya da eski ses çalıyor olabilir. **Zeyd'in "makas seçtiğim ses değil" demesinin sebebi büyük ihtimalle bu.**
- Unity hatayı yakalanmamış istisna olarak görüp sayfada uyarı gösteriyor.

## Yapılacaklar
1. **Biçimleri tara:** web'e yüklenen bütün sesleri ve müzikleri listele: biçim, kodek, örnekleme hızı.
2. **Hepsini Safari uyumlu biçime çevir:**
   - efektler ve müzik için **AAC (.m4a)** ya da **MP3**;
   - tutarlı örnekleme hızı (44,1 kHz);
   - döngü dikişleri korunsun: MP3 başındaki sessizlik dikişi bozabilir, döngüler için AAC + doğru gapless ayarı ya da WAV/PCM (küçük efektler).

   Ses düzeyleri değişmesin.
3. **iOS uygulaması:** Unity'nin iOS içe aktarma ayarlarında da aynı dosyalar çalıyor mu kontrol et (Unity içinde sıkıştırma biçimi). Uygulama tarafında bu hata olmamalı ama doğrula.
4. **Hata yönetimi:** bir ses çözülemezse **oyun uyarı göstermesin**. Hata yakalansın, telemetriye `audio_decode_fail` (dosya adı) yazılsın, o ses sessiz geçsin. Unity'nin `alert` penceresi hiçbir ses hatasında çıkmasın.
5. **Safari testi:** Playwright **WebKit** ile p23'ü aç. Bütün sesleri tetikleyen akış: menü, bölüm, çıkış, takılma, 4 booster, kazanma, sandık, Durdur, sekmeler. `decodeAudioData` hatası 0 olsun. Dinleme sayfası da WebKit'te 0 hata.
6. **Eşleme** (PM_104 §1) bu düzeltmeden sonra yapılsın.

## Teslim
- Rapor `docs/art/pm104/SAFARI_SES.md`: hangi dosyalar çözülemiyordu, neye çevrildi, WebKit test sonucu.
- `verify_build` p23 4/4, commit + push. Sonra PM_104'e devam.
