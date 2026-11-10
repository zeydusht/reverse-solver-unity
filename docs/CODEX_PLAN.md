# Codex iş planı

Tarih: 23 Ekim 2026

Kaynaklar: `CLAUDE.md`, `BACKLOG.md`, `docs/OVERNIGHT.md` ve backlog'un işaret ettiği `docs/PM_MESAJ_6.md`.
Öncelikler bu kaynaklardaki bağımlılıklara göre sıralandı. Mevcut kodun tamamlanma durumu doğrulanmadı; aşağıdakiler belgelerde açık kalan işlerdir. OVERNIGHT eski oturumları da içeriyor; eski talepler yeni PM talimatının önüne geçirilmedi.

## Sınırlar

- Tasarım, oyun hissi ve ürün kararlarına dokunulmaz: **Claude bekliyor**.
- Git push, deploy ve dosya silme yapılmaz. SQL canlı veritabanında çalıştırılmaz.
- Başlangıçta BACKLOG.md ve CLAUDE.md zaten değiştirilmişti; mevcut değişiklikler korunur. Mevcut izlenmeyen dosyalara dokunulmaz.

## Öncelikli işler

| Öncelik | İş | Durum / sonraki adım |
|---|---|---|
| P1 | **Codex buldu:** LevelCsv zorluk tablosu dışa aktarma sözleşmesi | **Bitti** — 19/36 sütun, türsüz alanlar, çözüm kanıt sınırı ve BOM sahipliği belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** LevelData paylaşılan veri ve değişmezlik sözleşmesi | **Bitti** — derin değişmezlik garantisi olmadığı, dizi sahipliği ve parça indekslerinin birlikte taşınması gereği belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Board klon sahipliği ve kaldırma önkoşulları | **Bitti** — bağımsız tahta durumu, paylaşılan LevelData ve kural doğrulamayan Remove işlemi belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** LevelStats sayım ve oran sözleşmesi | **Bitti** — tüm iç hücre sınırlarının sayılması, sayaç/toplam ayrımı, doğrulama önkoşulları ve çift sayıya yuvarlama belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** DesignStore taslak sahipliği ve sürüm karşılaştırması sözleşmesi | **Bitti** — canlı referanslar, kopyalayarak düzenleme ve kimlik sayacı sınırları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** LevelDraft kayıtlı çözüm önbelleği sözleşmesi | **Bitti** — liste sahipliği, doğrulama sınırı ve oyun verisi geri alındığında yolun yeniden görünmesi belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** DesignCheck sonuç ve kanıt sınırları sözleşmesi | **Bitti** — sınırlı aramanın kanıt sınırları, rapor alanları ve bağımsız telefon boyutu kontrolü belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** TravelRule geometrik sorgu ve mesafe sözleşmesi | **Bitti** — parça önkoşulları, çıkış/duvar mesafeleri, engel seçimi ve oturum kurallarının kapsam dışında oluşu belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardFive kabul ve çivi seçimi sözleşmesi | **Bitti** — hedef/kabul ayrımı, tek çivi olasılığı, kazı bütçesi ve statik engel konisi belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardEight kabul ve tanı sözleşmesi | **Bitti** — yön tercihi, bomba sayısı, süre kontrolü ve eski tanı sınırları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardSeven kabul ve sınırlı arama sözleşmesi | **Bitti** — hedef/kabul ayrımı, ortak ayarlar, bomba örneklemesi ve bağımlılık ölçümünün sınırları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardSix üretim yollarının kabul ve tanı sözleşmesi | **Bitti** — sınırlı bomba araması, fitil gevşetme, sıralama ve farklı doğrulama kapıları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardDesign üretim ve sıralama sözleşmesi | **Bitti** — süre hesabı, sıfır maliyetli sapmalar, sınırlı çivi çifti araması ve null tanısı belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** FactoryRunner RunRounds yürütme ve kayıt sözleşmesi | **Bitti** — ilk tur kapsamı, eksik slot sayımı, tur sonunda kayıt, sabit FunFirst ve işbirlikli süre sınırı belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** HardFactors yol örnekleme ve eksik ölçüm sözleşmesi | **Bitti** — boş yol, örnekleme aralıkları, eksik kazılar ve hedef kontrollerinin sınırları belgelendi. |
| P1 | **Codex buldu:** FunSim oyuncu/deneme ölçüm sözleşmesi | **Bitti** — oran paydaları, kazanım/hayal kırıklığı örtüşmesi, medyan ve Run önkoşulları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** FunThresholds kabul ve sıralama sözleşmesi | **Bitti** — koşu kaynakları, tolerans, gevşetme önkoşulları ve bant ortası cezası belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** SmartPlayer Play/PlayAfter arama ve sonuç sözleşmesi | **Bitti** — hızlı planın en kısa yol garantisi vermemesi, ortak yedek arama bütçesi ve devam analizinin sonuç kapsamı belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** FactoryPlan dosya okuma ve doğrulama sınırları | **Bitti** — varsayılan plana dönüş, ayrıştırma varsayımları, sıralama ve eksik doğrulamalar belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** FactoryReport seçim ve eksik metrik sözleşmesi | **Bitti** — seçim sırası, eksik skorlar, ilk on hesabı ve dosya okuması belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** SlotSpec takvim ve varsayılan sınır sözleşmesi | **Bitti** — takvim sorgusu ile üretim sınırlarının farkı, tanıtım istisnaları ve çağıranın doğrulama sorumluluğu belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** DifficultyModel süre ve Analyze rapor sözleşmesi | **Bitti** — süre sınırının uygulandığı yer, dışarıdan gelen metrikler, önerilerin yan etkisizliği ve eksik kazıların atlanması belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** MonteCarlo örnekleme ve ölçüm sözleşmesi | **Bitti** — hamle/koşu ağırlıkları, sıfır koşu, sonuç sınıflaması ve CSV tohum alanının gerçek anlamı belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** StructureMetrics ölçüm sözleşmesi | **Bitti** — kısmi yol paydaları, boş yol değerleri ve eksik bomba kazılarının raporlanması belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Calibration Assess/FitTimer sözleşmesi | **Bitti** — yerinde güncelleme, süre geri alma, arama sınırları ve rapor yenileme sorumluluğu belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Curve seviye içi zorluk eğrisi sözleşmesi | **Bitti** — örnekleme, kutu ağırlıkları, düğüm maksimumu, eşit zirveler ve tanımsız eğri belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Search.Run arama sözleşmesi | **Bitti** — sayaçlar, yerel finalist ayıklama, zaman sınırı ve tekrarlanabilirlik kapsamı belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Similarity ve Dedupe API sözleşmesi | **Bitti** — simetriler, yok sayılan alanlar, kesin eşik ve sıraya bağlı ayıklama belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** Generator Make/TryMake API sözleşmesi | **Bitti** — deneme tohumları, sıfır tohum, başarısızlık tanısı ve adayın girdi referansları belgelendi; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** GreedySolver API sözleşmesinin belgelenmesi | **Bitti** — sonuç indeksleri, kısmi/boş tahta, girişin korunması, güncel eklemler ve geometrik çözümün oturum garantisi olmadığı açıklandı; yalnızca yorumlar değişti. |
| P1 | **Codex buldu:** GreedySolver yatay harekette üst/alt kenar takılması testleri | **Bitti** — düz kenar ve iki çıkıntı yönü için 3 yeni + 13 önceki + 40 golden vaka bağımsız .NET/NUnit koşusunda geçti; oyun kodu değişmedi. |
| P1 | **Codex buldu:** GreedySolver geometrik çözümünde çivi/bomba sayaçlarını yok sayma ve sayaçları koruma testleri | **Bitti** — çivi, bomba ve ikisinin birlikte olduğu 3 vaka eklendi; bağımsız .NET/NUnit koşusunda 3 yeni + 4 önceki + 40 golden vaka geçti. |
| P1 | **Codex buldu:** GreedySolver açılan düşük indeksli parçaya geri dönüş testi | **Bitti** — 1 yeni + 7 önceki + 40 golden vaka bağımsız .NET/NUnit koşusunda geçti; oyun kodu değişmedi. |
| P1 | **Codex buldu:** GreedySolver iki hücreli parçada tam koridor ve tek şerit duvar testleri | **Bitti** — 2 yeni + 8 önceki + 40 golden vaka bağımsız .NET/NUnit koşusunda geçti; oyun kodu değişmedi. |
| P1 | **Codex buldu:** GreedySolver yan kenar takılması: düz kenar ve iki çıkıntı yönü testleri | **Bitti** — 3 yeni + 10 önceki + 40 golden vaka bağımsız .NET/NUnit koşusunda geçti; oyun kodu değişmedi. |
| P0 | Eksik işleri ayırmak ve bu planı oluşturmak | **Bitti** — aşağıdaki tamamlanma kaydına bak. |
| P1 | **Codex buldu:** GreedySolver için kısmen boşaltılmış/boş tahta, girişin korunması ve sıkışan kalan parçalar testleri | **Bitti** — oyun kodu değişmeden dört test vakası eklendi; Unity dışında .NET/NUnit ile 4 yeni + 40 mevcut golden vaka geçti. |
| P0 | Güvenli teknik işler için hedefli inceleme aracını çalıştırmak | **Bitti** — Kullanıcının 23 Ekim talimatıyla inceleme doğrudan yapıldı; Codex CLI çağrılmadı. |
| P1 | `docs/sql/008_playtest_analiz.sql`: bölüm başına ilk deneme kazanma oranı, ortanca süre/puan, beğeni ve booster oranı, oyuncunun son bölümü; ddd ve TEST_* hariç | **Bitti** — Mevcut taslak şema/telemetriyle karşılaştırılıp düzeltildi; sürümler ayrıldı, eksik ilk denemeli gruplar ve son oyuncu listesi eklendi. SQL çalıştırılmadı. |
| P1 | SET40 v3 slot dağılımı: öğretici eğri, tanıtımlar 3/6/10/14, zorlar 19/25/30, zirveler 35–40, final 40 | **Claude bekliyor** — seviye tasarımı, zorluk ve oyun hissi. |
| P1 | Tutorial 1–2, tanıtım seviyeleri, S39 yerine yeni zirve ve onaylı zirvelerin yeniden yerleşimi | **Claude bekliyor** — içerik tasarımı; mevcut seviyeler değiştirilmedi. |
| P1 | El animasyonu, adım yazıları, engel tanıtım pencereleri ve zor bölüm ekranı | **Claude bekliyor** — görsel tasarım, yönlendirme ve oyun akışı. Metinleri tek yerde tutma da bu uygulamanın parçası. |
| P1 | Booster açılışlarının tanıtımlarla çakışmayacak şekilde kaydırılması | **Claude bekliyor** — slot seçimi ürün/öğrenme eğrisi kararı. |
| P2 | Build 5: varsayılan set v3, `?set=classic`, beğeni ve kayıt ayarları | **Claude bekliyor** — ürün davranışı değişiyor; v3 onayı ve hazırlığına bağlı. |
| P2 | Yerel EditMode testleri ve build doğrulaması | **Ortam engeli** — EditMode denendi: Unity lisans IPC bağlantısı kurulamadı, test sonucu yok. Mevcut yerel build doğrulaması denendi: Chrome 9333 bağlantısı kurulamadı. Yeni build alınmadı; `--send` kullanılmadı. |
| P2 | SET40 slot tablosu ve güncel sabah raporu; kısa test listesi 1/3/6/10/14/19/40 | **Claude bekliyor** — v3 henüz doğrulanmadı; hazırmış gibi raporlanmayacak. Mevcut OVERNIGHT korunuyor. |
| P2 | Zirve yöntemi kabulünün karar kaydına ve ZeydBot RAPOR'una işlenmesi | **Claude bekliyor** — ürün karar kaydı; PRODUCT.md PM'e ayrılmış. |
| P2 | Build 5 yayını | **Claude bekliyor** — ayrıca kullanıcının bu oturumda push/deploy yasağı var; yapılmayacak. |
| P3 | ZeydBot v2 | **Claude bekliyor** — backlog'a göre setten sonra; model/hedef ve oyun hissi kararı içerir. |
| P3 | App Store / iOS | **Claude bekliyor** — PM tarafından ertelenmiş. |

## Zeyd'in yapacakları

Backlog'daki kullanıcı işleri otomatik yapılmaz: setten 5 zirve + 5 rastgele bölümü puanlama ve onay kuyruğu, CSV/Excel 36 sütun kontrolü, iPhone testi, editör/zorluk paneli kabulü, eski Actions re-run iptali, masaüstü saatlik görevi durdurma, playtest davet kapsamı ve 4G testi. Güncel v3 test listesinin eski v2 listesinin yerini alması **Claude bekliyor**.

## Tamamlanan işler ve doğrulama

1. **Bitti — işlerin önceliklendirilmesi:** Kaynaklardaki açık işler, bağımlılıklar ve kullanıcıya ait kontroller ayrıldı; karar gerektirenler “Claude bekliyor” olarak işaretlendi. Değişen dosya: `docs/CODEX_PLAN.md`.
2. **Bitti — mühendislik durum kaydı:** Planın hazır olduğu ve teknik yürütme engeli backlog'un Mühendislik bölümüne eklendi; önceki kullanıcı değişiklikleri korundu. Değişen dosya: `BACKLOG.md`.

3. **Bitti — doğrudan teknik inceleme:** SQL 001/002/003/005/007, PM_MESAJ_6 ve Core telemetri/deneme alanları karşılaştırıldı; eski CLI engeli kaldırıldı. Değişen dosya: `docs/CODEX_PLAN.md`.
4. **Bitti — playtest SQL düzeltmesi:** Slot + kimlik + sürüm gruplaması, booster’sız oran, saat booster’ı hariç süre, ilk denemesiz gruplar ve son oyuncu listesi eklendi; sorgu çalıştırılmadı. Değişen dosyalar: `docs/sql/008_playtest_analiz.sql`, `docs/CODEX_PLAN.md`.

5. **Bitti — yerel doğrulama girişimi ve engel kaydı:** EditMode Unity lisans IPC hatasıyla sonuç XML’i üretemedi; mevcut yerel build kontrolünde Chrome 9333 bağlantısı açılamadı. `git diff --check` geçti; yeni build alınmadı, SQL çalıştırılmadı. Değişen dosyalar: `docs/CODEX_PLAN.md`, `BACKLOG.md`; günlük: `Logs/codex-editmode-20261023.log`.

## Önceki teknik engel (kalktı)

`CLAUDE.md` uzun okuma/tarama işlerini yerel `codex exec` aracına verip hata durumunda kullanıcıya bildirmeyi ve kendi başına taramaya geçmemeyi emrediyor. İlk çağrı PowerShell script yürütme politikası nedeniyle başarısız oldu. Politikayı değiştirmeden `codex.cmd` denendi; bu kez `Could not find home directory` / `Error finding codex home` hatası alındı. Hiçbir çağrı inceleme görevini başlatamadı.

23 Ekim kullanıcı talimatı bu kısıtı kaldırdı: inceleme ve SQL düzeltmesi doğrudan yapıldı, CLI kullanılmadı. Tasarım işleri Claude bekliyor; SET40 v3 veya Build 5 tamamlanmış sayılmaz.

## Gece turu 1
GreedySolver'ın giriş tahtasını koruması, boş/kısmen boş tahta ve sıkışan parça raporu için 4 test vakası eklendi; bağımsız .NET koşusunda gerçek NUnit doğrulamalarıyla 4 yeni + 40 mevcut golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 2
GreedySolver’ın çivi/bomba sayaçlarından bağımsız geometrik çözümünü ve sayaçları korumasını doğrulayan 3 test vakası eklendi; Unity dışında .NET/NUnit ile 3 yeni + 4 önceki + 40 mevcut golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 3
GreedySolver’ın bir çıkarma sonrası açılan düşük indeksli parçaya geri dönmesini ve çözüm sırasının yasal hamlelerle tahtayı boşaltmasını doğrulayan test eklendi; bağımsız .NET/NUnit ile 1 yeni + 7 önceki + 40 golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 4
İki hücreli parçanın tüm koridor açılmadan çıkarılamamasını ve tek bir şeritteki duvarın parçanın tamamını engellemesini doğrulayan 2 test vakası eklendi; bağımsız .NET/NUnit ile 2 yeni + 8 önceki + 40 golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 5
Yan yana parçaların düz kenarla dikey çıkabilmesini, iki çıkıntı yönünde ise takılı kalmasını doğrulayan 3 test vakası eklendi; bağımsız .NET/NUnit ile 3 yeni + 10 önceki + 40 golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 6
Yatay harekette üst/alt kenar takılmasını, engelleyen parça raporunu ve çözüm sırasını doğrulayan 3 test vakası eklendi; bağımsız .NET/NUnit ile 3 yeni + 13 önceki + 40 golden vaka geçti (Unity EditMode koşulmadı).
Değişen dosyalar: `Assets/_Game/Tests/EditMode/LevelRuleTests.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 1
GreedySolver API sözleşmesi belgelendi; yorumlar çıkarılarak HEAD ile karşılaştırma çalıştırılabilir kodun aynı kaldığını doğruladı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Rules/GreedySolver.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 2
Generator Make/TryMake sözleşmesi belgelendi; yorum ve boşluklar çıkarılarak işlem öncesiyle karşılaştırma çalıştırılabilir kodun aynı kaldığını doğruladı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Generator.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 3
Similarity ve Dedupe sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Similarity.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 4
Search.Run sözleşmesi ve sonuç sayaçları belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Search.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 5
Curve örnekleme ve puanlama sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Curve.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 6
Calibration Assess/FitTimer yan etkileri ve süre seçimi sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Calibration.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 7
StructureMetrics ölçüm sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/StructureMetrics.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 8
MonteCarlo örnekleme ve ölçüm sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/MonteCarlo.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 9
DifficultyModel süre ve Analyze rapor sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/Difficulty/DifficultyModel.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 10
SlotSpec takvim ve varsayılan sınır sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/SlotSpec.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 11
FactoryReport seçim ve eksik metrik sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/FactoryReport.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 12
FactoryPlan dosya okuma, ayrıştırma ve sıra doğrulamasının sınırları belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/FactoryPlan.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 13
SmartPlayer Play/PlayAfter arama bütçesi, hızlı planın sınırları ve sonuç kapsamı belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Solve/SmartPlayer.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 14
FunThresholds kabul/sıralama ve gevşetme sözleşmesi belgelendi; bant ortası cezasının hatalı yorumu düzeltildi. Yorum/boşluk dışı kodun aynı olduğu doğrulandı; git diff --check geçti, Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/Factory.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 15
FunSim ölçüm sözleşmesi ve Run önkoşulları belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/FunSim.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 16
HardFactors yol örnekleme ve eksik ölçüm sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/HardFactors.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.
## Gece turu 17
FactoryRunner RunRounds tur seçimi, kayıt ve süre sınırı sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/FactoryRunner.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 18
HardDesign üretim/sıralama sözleşmesi ve eski süre/fitil yorumları düzeltildi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/HardDesign.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 19
HardSix Try/FromD05 kabul, tanı ve sıralama sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/HardSix.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 20
HardSeven kabul ve sınırlı arama sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/HardSeven.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 21
HardEight kabul, tekrar oynatma ve tanı sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/HardEight.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 22
HardFive kabul, sıralama ve çivi seçimi sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Generation/HardFive.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 23
TravelRule sorgu önkoşulları, mesafe/engel alanları ve oturum kurallarından ayrımı belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Rules/TravelRule.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 24
DesignCheck sonuç ve kanıt sınırları belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/DesignCheck.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 25
LevelDraft.Solution liste sahipliği, doğrulama sınırı ve anahtar eşleşmesi sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/LevelDraft.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.
## Gece turu 26
DesignStore canlı taslak referansları, kopyalayarak düzenleme gereği, sürüm karşılaştırması ve kimlik sayacı sınırları belgelendi; yorum/boşluk dışı kodun aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/DesignStore.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 27
LevelStats sayım kapsamı, anlık görüntü niteliği ve OpenPercent yuvarlama sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Model/LevelStats.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 28
Board klon sahipliği, geometri önkoşulları ve Remove işleminin kural doğrulama sınırı belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Rules/Board.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 29
LevelData paylaşılan veri sahipliği, değişmezlik sınırı ve parça indekslerinin bağımlılıkları belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Core/Model/LevelData.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.

## Gece turu 30
LevelCsv sütun seçimi, analiz maliyeti, eksik tür alanları, çözüm/verdict ilişkisi ve BOM sözleşmesi belgelendi; yorum/boşluk dışı kodun işlem öncesiyle aynı olduğu doğrulandı, git diff --check geçti. Unity çalıştırılmadı.
Değişen dosyalar: `Assets/_Game/Editing/LevelCsv.cs`, `docs/CODEX_PLAN.md`; oyun davranışı değiştirilmedi.
