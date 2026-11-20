# PM_95 RAPOR

**Önemli:** Ben ses dinleyemiyorum. Seçimleri kaynak sayfalarının tarifine, çalgılara, tempoya ve ölçümlere (ses yüksekliği, süre, melodi yönü, döngü dikişi) göre yaptım. Kulakla seçim Zeyd'in.

## 1. Lastik

- `&lastik=2` artık varsayılan.
- 1 ve 1.5 bayrakla duruyor; `0` PM_75'in eski çarpması.

## 2. Dinleme sayfası

Adres: `p23/onizleme/sesler.html`. Kaynağı `docs/art/pm95/onizleme/`. Yayında efekt dosyaları `p23/onizleme/sesler/`, müzik `music/pm95/` altında.

- **Her aday için:**
  - büyük ▶/■ düğmesi;
  - ad, süre, BPM, lisans bağlantısı;
  - **Seç** düğmesi;
  - **Oyunda dene** (o bayrakla `lv=2`).
- **Müzikte "Döngü geçişi":** sondan 6 sn önce başlar, başa dönüşü dinletir. Döngü oyundaki gibi WebAudio döngü noktalarıyla çalar, boşluk olmaz.
- **Efektlerde ▶:** her basış başka bir kaydı biraz farklı perdeyle çalar. **×5** art arda beş kez çalar.
- **Alt şerit:**
  - seçimleri toplar (telefonda hatırlanır);
  - "Seçimlerle oyunda dene" bağlantısı;
  - "Seçimi kopyala" düğmesi.
- **iOS:** ses ilk dokunuşta açılıyor. Safari destekliyorsa sessiz anahtar sayfayı susturmuyor (`audioSession = playback`); yine de sayfada uyarı var.
- **Ses seviyesi:** efektler oyundaki seviyeleriyle çalıyor.
- **Sayfada ayrıca:** karşılaştırma için şimdiki müzik ve PM_86'nın a–e adayları.
- **Kontrol:** yerelde 63 ses dosyasının 63'ü tarayıcıda çözüldü. Canlıda sayfa açılıyor ve müzik çalıyor.

## Müzik (8 yeni aday, `&muzik=f`…`m`)

Hepsi OpenGameArt'tan, CC0. Lisanslar `LISANS.md`'de.

| | Parça | Çalgılar | Tempo | Döngü |
|---|---|---|---|---|
| **f (varsayılan)** | Apple Cider | akustik gitar, flüt, ıslık | 100 | 86 sn, bestecinin döngüsü |
| g | Magic Puzzle Title | yaylılar, mallet, flüt | 100 | 96 sn |
| h | Magic Puzzle In-Game 1 | keman, yaylılar, celesta, klarnet | 110 | 122 sn |
| i | Cozy Puzzle Stage Select | saksafon | 100 | 77 sn |
| j | Magic Puzzle In-Game 2 | yaylılar, celesta, flüt | 120 (biraz hızlı) | 120 sn |
| k | Quaint Town | huzurlu kasaba | ? | 82 sn, parçanın kendi tekrarı |
| l | Coffee House Bump | hafif caz | 100 | 96 sn, parçanın kendi tekrarı |
| m | Lovely Town | piyano, keman | ~85 | 57 sn, parçanın kendi tekrarı |

**Döngüler:**

- f–j bestecilerin kendi döngüleri; tam vuruş sayısına kesildi.
- k, l, m döngü olarak kesilmemiş. Parçanın kendini tekrar ettiği aralığı buldum (`Tools/pm95_loopfind.py`) ve dalga şekliyle örneğine kadar hizaladım. Döngünün sonrasında gelen müzik başın ilk 40 ms'sinin altına karıştırıldı; dikiş parçanın kendi akışı gibi geçiyor.
- 8 dikişin hepsinde tık yok: dikişteki adım parçanın %99,9 adımından küçük.

**Ses yüksekliği:**

- Hepsi −18 LUFS. k ve m'de birkaç tepe hafif bir tepe sınırlayıcıyla −1 dBFS altına alındı.
- Eski müzikler −19,4 LUFS idi; oyundaki müzik kısıklığı (0,35) aynı, yani müzik efektlerin altında kalıyor.

**Varsayılan f neden:** tarife en yakın olan bu. Akustik gitar, flüt ve ıslık var, sıcak bir "Animal Crossing" havası, 100 BPM, besteci tarafından döngü olarak yapılmış. Kazanma jingle'ı a da aynı albümden; müzikle aynı çalgılarla bitiyor.

`&muzik=0` şimdiki eski müziği çalar; a–e de bayrakla duruyor.

## Efektler

Gerçek kayıtlar: Kenney paketleri ve OpenGameArt, hepsi CC0.

- Her aday 1–4 kayıttan oluşuyor. Oyun her seferinde rastgele bir kayıt seçiyor, aynısını art arda çalmıyor ve perdeyi biraz kaydırıyor: çıkış ve çarpmada ±%5, tıkta ±%4, son parçada ±%2. Kazanma jingle'ının perdesi müzikle uyumlu kalsın diye kaydırılmıyor.
- Ses seviyesi, değiştirdiği eski sesin oyunda duyulan seviyesine eşitlendi. Unity eski sesleri tepe-normalize ediyormuş; yenileri normalize edilmeden alındı. Keskin tıklar tepe sınırı yüzünden en çok 6 dB daha yumuşak kaldı.

| Ses | Adaylar | Varsayılan ve neden |
|---|---|---|
| Çıkış (5) | a kart kayması · **b kart kayması + yumuşak pop** · c hafif whoosh + pop · d kumaş hışırtısı · e yumuşak pop | **b**: yapboz parçası kartondur, tahtadan kayan karton sesi + tatlı bir pop tarifin tam karşılığı. 0,14–0,25 sn, yorucu değil. |
| Çarpma (4) | **a yumuşak tok** · b kauçuk bonk · c boing · d tok + hafif boing | **a**: gerçek kayıt, kısa (0,12–0,18 sn), alçak ve tok; sık takılmada sinir bozmaz. Daha "lastik" isteyene d. |
| Son parça (3) | **a pizzicato ta-da** · b çelik davul ta-da · c zil parıltısı | **a**: melodisi yükselerek bitiyor (Re–Mi–Fa#–Sol) ve müziğin akustik havasına uyuyor. |
| Kazanma (3) | **a Apple Cider zafer** · b pizzicato · c çelik davul | **a**: varsayılan müzikle aynı albüm ve çalgılar; 3 sn. |
| Tık (3) | a fare tıkı · **b gerçek düğme tıkı** · c yumuşak tok | **b**: gerçek düğme kaydı, yumuşak ve kısa. |

**Uygulama:**

- Seçilen kayıtlar oyun açılışında yükleniyor; ilk çalışta çözme beklemesi yok.
- Ayrı bir ses kaynağı halkası var: üst üste gelen sesler birbirinin perdesini bozmuyor. Kazanma jingle'ının kendi kaynağı var.
- Müzik, uzun jingle bitene kadar kısık kalıyor.
- Ses ve müzik anahtarları ile sessiz mod eskisi gibi; aynı çalma yolu kullanılıyor.
- Yeni çıkış sesinin kendi "whoosh"u olduğu için fiskede eski "weee" sesi çalmıyor.
- **Bayraklar:** `&sescikis=`, `&sescarpma=`, `&sesbitis=`, `&seskazan=`, `&sestik=` (harf; `0` eski ses; bilinmeyen harf varsayılan). Tarayıcıda denendi: `d`, `0` ve bilinmeyen harf doğru çalışıyor.
- **Build:** veri +0,5 MB.

## Doğrulanmayan

- Seslerin kulağa nasıl geldiği; iPhone'da gecikme ve sessiz anahtar davranışı.
- iOS (App Store) build'i hâlâ eski müziği kullanıyor; değişiklik yalnız web'de. Zeyd seçince iOS'a da konur. `.ogg`'ler `Assets/Audio/Music95`'te hazır.
