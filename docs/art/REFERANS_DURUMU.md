# Görsel referansların güncel durumu

24 Ekim 2026 güncellemesi: Bu belge geçmiş denemeler ile aktif referansları ayırır. Seçilmemiş olmak, reddedilmiş olmak değildir.

Son ikon kararı: **Review/Ikon/2-market-mavi-v01.png onaylıdır ve korunmalıdır.** Son kırmızı/sarı/yeşil revizyon aktif seçimden çıkarıldı; arşiv olarak yerinde durur. Onaylı dosya selected-review-20261023-v02.zip içinde doğrulanmış yedekte de vardır.

## Korunan temel görseller

| Dosya (Assets/Art/ altında) | Kullanıcı kararı / kullanım |
|---|---|
| Ornekler/01-jiggles-v01.png, 02-koridor-v01.png | İlk beğenilen güçlü adaylar; korunur, güncel üretimde stil adayı 2 önceliklidir |
| Ornekler/StilAdaylari/Jiggles/2.png | Seçilen çalışma stili ve yüz ailesi |
| Ornekler/StilAdaylari/Koridor/2.png | Seçilen stilin ortam referansı; nihai menü onayı değil |
| Ornekler/StilAdaylari/Jiggles/2-ortali-v01.png | Kullanıcının beğendiği merkezli yapboz geometrisi |
| Ornekler/Baron/3.png | Onaylı Baron; sağ alt çatık kaşlı yüz ana öfke referansı |
| Characters/Jiggles-reference-v02.png | Güncel çalışma kartı; karakter kimliğini değiştirmez |
| Characters/Baron-reference-v01.png | Onaylı tasarımdan güncel çalışma kartı |
| Ornekler/Ikon/4.png | Beğenilen ikon yönünün önceki sürümü; korunur |
| Ornekler/Ikon/4-uc-jiggles-v02.png | Güncel üç Jiggles/gradyan revizyonu; nihai onay bekliyor |
| Review/Koridor/2.png | Zeyd'in seçtiği ilk menü sahnesi; korunur |
| Review/Koridor/2-tek-cerceve-v03.png | Kullanıcı isteğine göre yalnız büyük çerçeve kalan yeni revizyon; değerlendirme bekliyor |
| Review/Koridor/2-ortali-yan-blok-v04.png | Zeyd'in seçtiği güncel koridor: ana blokta büyük çerçeve, sağ komşu blokta küçük çerçeve. Üretim kopyası `Menu/corridor-v04-source.png` olarak korunur. |
| Review/Koridor/2-ortali-yan-blok-v05.png | Sonraki alternatif; Zeyd v04'ü seçtiği için aktif üretim tabanı değildir, korunur. |

Güncel ikon revizyonu Review/Ikon/2-market-mavi-v01.png; önceki Review/Ikon/1-safe-area.png korunur. Review/Koridor/2.png ve bu iki Review ikonu ayrıca backups/selected-review-20261023-v02.zip içinde yedeklendi; üç dosyanın SHA-256 değerleri doğrulandı, yolları ayrı manifestte kayıtlı.

Diğer eski adaylar ve kullanıcı yüklemeleri silinmedi. Onay durumları kesin değildir; yeni üretime otomatik referans olarak verilmezler. Saçlı Jiggles demolarından nihai seçim yok; saçsız ve kıyafetsiz güncel çalışma kartı kullanılır. Minions görseli yükleme olarak korunur, aktif üretim referansı değildir.

## Doğrulanmış yedek ve temizlik

- Tam görsel yedeği: backups/art-before-parallel-20261023-v01.zip.
- 50 dosyanın yolu, boyutu ve SHA-256 değeri: backups/art-before-parallel-20261023-v01-manifest.json.
- ZIP içindeki her dosyanın hash'i özgün dosyayla karşılaştırıldı. Temizlik öncesi 10 temel/güncel dosyanın hash'i ayrıca doğrulandı.
- Yalnız kesin iptal/hatalı üç dosya aktif alandan kaldırıldı: Characters/Jiggles-reference-v01.png (fazla uzuv); Ornekler/JigglesTatlilik/4-merakli-v01.png (iptal ifade); Ornekler/JigglesTatlilik/5-tulum-v01.png (iptal kıyafet).
- Bu dosyalar ZIP'te korunur. Geçmiş karar/prompt kayıtlarında eski yolların bulunması tarihsel kayıttır; üretim referansı değildir.
- Yedek proje içindedir; harici disk/bulut yedeği değildir.

## Paralel çalışma alanları

| Ajan | Yazma alanı | Görev |
|---|---|---|
| ikon | Assets/Art/Review/Ikon/; reports/IKON_PARALLEL.md | Güncel ikonun güvenli alan revizyonu ve boyut önizlemesi |
| koridor | Assets/Art/Review/Koridor/; reports/KORIDOR_PARALLEL.md | Seçilen stilde iki statik menü kompozisyonu |
| jiggles | Assets/Art/Review/Jiggles/; reports/JIGGLES_PARALLEL.md | Ayrı uzuvlar ve merkezli geometriyle poz kontrolü |
| ana yönetici | docs/art/ ortak belgeler; BACKLOG.md yalnız Art | Yedek, temizlik, koruma, kalite kontrolü ve teslim |

Yeni Review çıktılarının hiçbiri otomatik olarak kullanıcı onaylı veya aktif ana referans sayılmaz. Karakter tasarımını, stili veya oyun kodunu değiştirmeden değerlendirme için sunulurlar.

İlk paralel grup tamamlandı: ikon 1-safe-area.png, koridor 1-bordo-v02.png / 2.png, Jiggles 1-poz-kontrol.png. Koridor 1.png ilk sürümdür; mavi halısı nedeniyle bordo v02 incelemede tercih edilir. Bütün yeni sürümler korunur. İkon ajanı yedeği ikinci kez bağımsız doğruladı: kalan 47 özgün dosya değişmemiştir. 24 Ekim üretim dosyaları, seçili kaynakların üzerine yazılmaz ve `Assets/Art/ProductionReview.html` içinden incelenir.
