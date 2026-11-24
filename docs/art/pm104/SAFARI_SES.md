# PM_104_EK RAPOR: iPhone Safari'de "EncodingError: Decoding failed"

## Neden

Hata, Safari'nin bir ses dosyasını çözememesi (WebAudio `decodeAudioData`).

- **Unity, web build'inde her sesi AAC'ye (.m4a) çeviriyor.** Kaynak WAV ya da OGG olsa da öyle. Tarayıcı da açılışta bu AAC'leri tek tek çözüyor.
  - Eski build'de oyun açılışta **100 AAC** çözdürüyordu.
  - Bunların 40'ı 4 KB'tan küçük, en kısa efektlerdi:
    - parça kalkışı 0,025–0,045 sn;
    - tıklar 0,03 sn;
    - geçiş 0,01 sn.
  - Bu kadar kısa bir ses bir AAC çerçevesinden (1024 örnek) bile kısa. Safari bu dosyaları çözemiyor.
- **Uyarı penceresi nereden geliyor:**
  - Unity'nin ses kodu `decodeAudioData`'yı geri çağrıyla kullanıyor.
  - Safari bir de reddedilen bir promise döndürüyor ve onu kimse yakalamıyor.
  - Unity'nin "unhandledrejection" dinleyicisi bunu "An error occurred running the Unity content…" penceresine çeviriyor.
  - Aynı açık bizim müzik ve açılış kodumuzda da vardı.
- OGG yok: web'e giden dosyalar arasında Ogg Vorbis hiç çıkmadı (build verisinde "OggS" 0).
- "Makas seçtiğim ses değil" (PM tahmini): makas c de kısa bir AAC'ydi. Safari çözemeyince sessiz kalıyordu. Eşleme kontrolü PM_104 §1'de.

## Web'e giden sesler (önce → sonra)

| Ne | Kaynak | Web'de önce | Web'de şimdi |
|---|---|---|---|
| Oyunun çaldığı 51 efekt ve arayüz sesi (Zeyd'in seçimleri) | WAV 16 bit, 44,1 kHz (3'ü 22,05 kHz), çoğu tek kanal | Unity AAC | **Ham örnek** (16 bit, kendi hızı ve kanalı), çözme yok |
| Eski sesler (Art/audio kökü, 11 dosya; yedek olarak çalanlar) | WAV 16 bit | Unity AAC | **Ham örnek** |
| 153 aday (yalnız bayrakla açılır) | WAV | Unity AAC | Unity AAC (değişmedi) |
| Lobi ve bölüm müziği | MP3 128 kbps, 44,1 kHz, stereo | MP3 | MP3 (Safari'nin kendi biçimi) |
| Kazanma fanfarı (g, marimba) | WAV 16 bit, 44,1 kHz, stereo | WAV | WAV |
| Açılış filmi sesi (3 parça) | MP3 96 kbps, 44,1 kHz, stereo | MP3 | MP3 |
| Dinleme sayfası | WAV / MP3 | WAV / MP3 | WAV / MP3 |

**Nasıl:**
- Unity'nin ses ayarında web'e özel biçim yok: PCM, MP3 ve diğerleri denendi, hepsini reddediyor.
- Bu yüzden web build'i sırasında seçili seslerin ham örnekleri ayrıca yazılıyor. Oyun web'de sesi bunlardan kuruyor ve tarayıcıya hiçbir şey çözdürmüyor. Bu dosyalar build bitince siliniyor, iOS'a girmiyor.
- **Ses düzeyleri ve uzunluklar değişmedi:** aynı örnekler, aynı hız. Test her dosyada uzunluğu ve düzeyi (RMS) Unity'nin kendi klibiyle karşılaştırıyor.
- **Döngü dikişi:** efektlerin döngüsü yok. Müzik döngüleri MP3'te kalıyor; dikişi `Music.jslib` önceki gibi kendisi buluyor (başlangıç sessizliği ve döngü uzunluğu).
- **Boyut:** web data (Brotli) 34 977 619 → 36 257 746 B (**+1,25 MB**). Toplam indirme 40,9 → 42,1 MB.

## iOS uygulaması

Değişmedi:
- iOS'ta sesleri Unity'nin kendi motoru (FMOD) çözüyor (Vorbis), WebAudio yok; bu hata orada olamaz.
- Ham örnek klasörü iOS build'inde yok: iOS build'i başlarken siliniyor.
- PM_103'teki iOS build'i ve oyun içi müzik testi geçmişti.

## Hata yönetimi

- **Sayfa koruması** (`index.html`, Unity'den önce yüklenir):
  - her `decodeAudioData` çağrısının promise'i yakalanıyor;
  - ses hatası olan "unhandledrejection" ya da "error" olayı Unity'ye ulaşmadan durduruluyor;
  - Unity yapılandırmasının `errorHandler`'ı ses hatasını "işlendi" sayıyor;
  - Unity'nin doğrudan ses uyarısı ("Web Audio API is not supported") pencere yerine konsola gidiyor.
- **Çözülemeyen ses sessiz geçiyor.** Oyun dosya adını bildiriyor: günlükte `[audio] decode fail <dosya>`, telemetride `audio_decode_fail` (değer: dosya).
  - Unity klibi yüklerken dosya adını sayfaya veriyor. Müzik ve açılış kendi dosya adını veriyor.
- **Telemetri:**
  - `audio_decode_fail` soft launch olaylarına (`sl_events`, `docs/sql/019_soft_launch.sql`) eklendi.
  - Bu tablo canlıda henüz kurulu değil. SQL dosyası PM ve Zeyd onayıyla çalıştırılınca yazılmaya başlar.
  - Olaylar yalnız mağaza kohortundan (sl1) gidiyor.

## Test

Playwright **WebKit** (Safari'nin motoru) bu makinede (Windows) Web Audio'yu hiç desteklemiyor: `AudioContext` yok. Çözme davranışı WebKit'te ölçülemedi. WebKit'te ve gerçek GPU'lu Chrome'da ölçülenler (`node Tools/webkit_audio_check.js <adres> [--chrome <chrome.exe>]`):

| Koşu | Çözülen ses | Çözülemeyen | Uyarı penceresi | Sayfa hatası |
|---|---|---|---|---|
| Eski build, Chrome | 100 AAC + 2 MP3 + 1 WAV | 0 (Chrome hepsini çözer) | 0 | 0 |
| **Yeni build, Chrome** | **0 AAC** + 2 MP3 + 1 WAV | 0 | 0 | 0 |
| Yeni build, `&sestest=bozuk` (her çözme başarısız) | 0 | 3 (müzikler, fanfar) | **0** | **0** |
| Yeni build, bozuk + aday sesler (`&sesmakas=b&sescikis=a`) | 0 | 14 (adaylar + müzik) | **0** | **0**; oyun 16 dosyayı adıyla bildirdi |
| Eski p23, WebKit (Windows, Web Audio yok) | — | — | **1** ("Web Audio API is not supported") | 0 |
| **Yeni p23, WebKit** | — | — | **0** (konsola gitti) | **0** |
| Yeni p23, dinleme sayfası, Chrome | 19 dosya (4'ü bulunamadı) | 0 | 0 | 0 |
| Yeni p23, dinleme sayfası, WebKit | Web Audio yok, çözülemedi | — | 0 | **0** |

- `verify_build`: yerel 4/4, gönderimli 7/7 (TEST_PM104), canlı p23 4/4 (gh-pages 6ec60da2).
- Testler: EditMode 790 geçti, 2 atlandı; PlayMode 17/17. Yeni testler:
  - her seçili sesin ham örneği var, uzunluğu ve düzeyi aynı;
  - adaylar dışarıda;
  - klasör yalnız build sırasında var;
  - sayfa koruması yerinde;
  - telemetri olayı tanımlı;
  - çözülemeyen klip bir kez bildiriliyor.
- Bayraksız oyunda tarayıcıya çözdürülen Unity sesi kalmadı. Safari'nin çözebileceği/çözemeyeceği bir AAC yok.
- Tarayıcının çözdüğü yalnız MP3 ve WAV: Safari'nin kendi biçimleri.
- Her çözme bozulsa bile uyarı çıkmıyor, oyun devam ediyor.

## Doğrulanmayan

- **Gerçek iPhone Safari'de dinlenmedi.** Bu makinede Safari yok; Windows WebKit'te Web Audio yok. Zeyd'in telefonda p23'ü açıp oynaması gerekiyor: uyarı çıkmamalı, makas c ve kalkış sesi duyulmalı.
- Bayrakla açılan aday seslerden bazıları Unity'nin geç yüklemesi yüzünden adsız ("6937 B") bildiriliyor. Bayraksız oyunda bu durum yok.
