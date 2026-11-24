# PM_104 RAPOR: sesler oyunda doğru mu, kaydırmaya swoosh, düğmelere tok

Önce ek iş yapıldı: iPhone Safari ses hatası (`SAFARI_SES.md`).

## 1. Dinleme sayfası = oyun

Tablo: `ses_eslesme.md`. Araç `node Tools/ses_eslesme.js`: sayfanın çaldığı dosya ile oyunun çaldığı dosyayı özetle (SHA-1) karşılaştırıyor.

**Önce:** 13 olaydan 2'si farklıydı.
- **Kaydırma f:**
  - sayfa PM_99'daki sürümü çalıyordu;
  - oyun PM_100'de süzülüp ses düzeyi ayarlanmış sürümü çalıyor.
- **Çarpma a:**
  - sayfa PM_95'in eski kayıtlarını çalıyordu;
  - oyun PM_102'nin eşitlenmiş dört kaydını çalıyor.
- **Makas c dosyası aynıydı.**

**Üst üste çalan sesler** (koddan):

| Olay | Seçilen ses | Üstüne çalan | Düzeltme |
|---|---|---|---|
| Makas | makas c (bahçe makası "şık") | **eski "piece" toku**: kesilen eklem olayında her zaman çalıyordu | makasın kendi sesi varken eski tok çalmıyor |
| Çekiç | çekiç a (vuruş + kırıntı) | **kaydırma sesi f**: çekiçle silinen parça "çıkış" sayılıyordu | çekiçle silinen parçada kaydırma sesi yok (bomba parçasının sönme sesi kalıyor) |
| Değnek | değnek a | yok | — |
| Saat | saat a (tik-tak, buz, kırılma) | yok | — |

**Zeyd'in "makas seçtiğim ses değil" demesi:** büyük ihtimalle makas c'nin üstüne çalan eski tok yüzünden. Safari'de c'nin çözülemeyip sessiz kalması da etkili olmuş olabilir (PM_104_EK'te düzeldi).

**Sonra:** sayfa artık her olayda oyunun kendi dosyasını çalıyor. Araç oyunun çaldığı bütün dosyaları sayfanın klasörüne kopyalıyor ve "Bugünkü seçim" listesini yazıyor. Yeniden ölçüm `ses_eslesme.md`'de.

## 2. Kaydırma: swoosh adayları (k–p)

`&sescikis=k…p`. Hepsi CC0 kayıt (`LISANS.md`), dörder kayıt:

| | Ad | Kaynak | Uzunluk | Parlaklık (ağırlık merkezi) |
|---|---|---|---|---|
| k | Bambu swoosh, parlak | qubodup | 0,22 sn | 3,1–3,9 kHz |
| l | Bambu swoosh, kısa | qubodup | 0,16 sn | 3,6–4,5 kHz |
| m | Bambu swoosh, geniş | qubodup | 0,21–0,25 sn | 3,2–4,0 kHz |
| n | Hava whoosh | OpenGameArt "Air whoosh" | 0,20 sn | 5,9 kHz |
| o | İnce hava taraması | Kenney Interface | 0,20 sn | 3,3–7,8 kHz |
| p | Bambu swoosh, yükseltilmiş | qubodup (+3 yarım ton) | 0,18 sn | 3,0–3,8 kHz |

- Her birinde:
  - 20–30 ms yumuşak başlangıç ve 60 ms sönüş;
  - 700 Hz altı kesildi (sürtünme ve tahta sesi yok; takılmadaki "bonk" 358–525 Hz, ondan ayrışıyor);
  - ses yüksekliği bugünkü f ile aynı (−15 LUFS);
  - takılma için kendi kısa kalkış sesleri (45 ve 25 ms), PM_100'deki gibi.
- **Hıza göre:**
  - hızlı fiske (saniyede 35 kare ve üstü) %4 yüksek perde ve +1 dB;
  - yavaş itiş (saniyede 5 kare ve altı) %4 alçak ve −1 dB;
  - arası yumuşak geçiş.
  - Bütün kaydırma sesleri için geçerli, bugünkü f dahil.
- Dinleme sayfası:
  - "Kaydırma (swoosh)" bölümü;
  - her adayda "10 art arda" düğmesi;
  - karşılaştırma için bugünkü f.
- Varsayılan değişmedi (f). **Zeyd seçecek.**

## 3. Durdur ve alt menü: tok ses

**Bugün çalan:**
- **Durdur:** iki ses aynı anda.
  - düğme tıkı b: tepe −1 dB, en yüksek UI sesi;
  - panel açılış "vuş"u.
- **Alt sekmeler:** düğme tıkı b ile sekme kayış swish'i üst üste.

**Yapılan:**
- Yeni "tok" düğme sesi; şu düğmeler artık onu çalıyor:
  - Durdur, Devam, Menü;
  - alt sekmeler;
  - Ayarlar (açma ve kapama).
- Tok çalarken:
  - panelin vuşu 9 dB kısık;
  - sekme swish'i 12 dB kısık.
- Anahtarlar (ses, müzik, titreşim) kendi seslerinde kaldı.
- Diğer düğmeler tık b'de.

**7 aday** (`&sesdugme=a…g`, `0` eski tık), hepsi en çok 0,12 sn, UI tıkı düzeyinde (−16 LUFS), 5 kHz üstü yumuşatılmış, 200 Hz altı kesik (telefon hoparlörü çalamaz):

| | Ad | Kaynak |
|---|---|---|
| a | Yumuşak ahşap (**şimdilik varsayılan**) | Kenney Impact Sounds |
| b | Ahşap, dolgun | Kenney Impact Sounds |
| c | Yumuşak, boğuk | Kenney Impact Sounds |
| d | Plastik tok | Kenney Interface Sounds |
| e | Plastik seçim | Kenney Interface Sounds |
| f | Yumuşak menü toku | Lokif GUI Sound Effects |
| g | Küçük tahta | Kenney Impact Sounds |

- Varsayılan a, `ses.json`'da (`"dugme": "a"`). **Zeyd seçecek.**
- Dinleme sayfasında "Düğme (tok)" bölümü, "×5" ile art arda beş basış.

## 4. Bugünkü seçim

- Dinleme sayfasının en üstünde. Şu an oyunda çalan her ses tek satırda, ▶ ile çalıyor (oyunun kendi dosyası):
  - üç müzik;
  - kaydırma, kalkış, çarpma;
  - tıklar ve tok;
  - panel, anahtar, pasif düğme;
  - bomba, kilit;
  - dört booster;
  - beş geçiş.
- Liste `bugun.json`'dan geliyor. Araç her yayında onu oyunun listesinden yeniden yazıyor.

## Doğrulama

- EditMode testleri (PM_10x 27/27; tamamı aşağıda):
  - booster olaylarında tek ses;
  - swoosh adaylarının uzunluğu, yumuşak başlangıcı ve ses düzeyi;
  - hıza göre perde;
  - tok'un tık yerine geçmesi, anahtarın kalması, panel sesinin kısılması;
  - yalnız seçilenlerin yüklenmesi ve adayların iOS dışında kalması;
  - sayfanın oyunun dosyalarını bayt bayt aynı çalması.
- PlayMode 17/17.

## Doğrulanmayan

- Seslerin telefonda nasıl duyulduğu (Zeyd dinleyecek).
- Hıza göre perde, gerçek parmakla denenmedi; testi formül üzerinden.
