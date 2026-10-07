# Gece planı — 2026-10-07 → 2026-10-08

Bu dosya bu gecenin görev tanımı. Zeyd'in isteğiyle, sohbetteki PM'i (Claude) ile hazırlandı. Gece boyunca birden fazla oturum aynı yerden devam edecek: önce VS Code'daki oturum, kullanım limiti dolarsa limit yenilenince Claude Desktop'taki saatlik görevin açtığı yeni oturumlar. Hepsi bu dosyayı ve `docs/OVERNIGHT.md`'deki **Kaldığım yer** bölümünü okuyarak çalışır.

Zeyd uyuyor. Sabaha kadar ona soru sorulmaz, onay beklenmez (AskUserQuestion kullanma).

## Durum: bugün olanlar (PRODUCT.md'de henüz yok)

- WebGL build engeli çözüldü: Zeyd, Windows Akıllı Uygulama Denetimi'ni (Smart App Control) kapattı. PRODUCT.md ve OVERNIGHT.md'deki "geri dönüşsüz" notu eskidi; Nisan 2026 güncellemesinden (KB5083769) beri yeniden açılabiliyor. Yeniden açmak Zeyd'in ileride vereceği bir karar; bu gece konu değil.
- M3 + M4 (+ M5-ön, gönderim kapalı) build'i gh-pages'te yayında (10/7 14:14).
- Zeyd canlı adreste iPhone'da oynadı: "oyun çalışıyor, süper"; sürüklerken **60 FPS**. M3 ölçüt 4 (≥ 55) karşılandı. M3 ölçüt 8 ve M4 ölçüt 5'in kontrol listesi madde madde raporlanmadı.
- Zeyd `docs/sql/002_plays_level_id_row_id.sql`'i Supabase'de telefondan çalıştırdı (10/7 akşam). Doğrulama sorgusunun çıktısını getirmedi.
- **Zeyd'in kararı, bu gecenin ana işi: seviye editörü.** Kendi sözleriyle: "bana dizayn edebileceğim bir alan ayarlasınlar, gerekirse Unity'den bana bir kısım açsınlar; ben orada yapbozun her parçasını görebileyim ve yerleştirme usulü level design edebileyim." Backlog'daki "Unity içinde seviye editörü" maddesi öne alınır. PM itiraz edebilir ama kararı kayda geçirir.

## Gecenin sırası

### 0. Gözetimsiz çalışmaya hazırlık (ilk oturum, ~15 dk)

1. **İzinler:** gece izin sorusu kimse yanıtlamayacağı için oturumu sabaha kadar kilitler. Bu projede bugüne kadar onaylanan komutları ve gece gerekecekleri `.claude/settings.local.json`'daki `permissions.allow` listesine ekle: git (status, log, diff, add, commit, push, gh-pages yayını), python, node, Unity MCP araçları, build doğrulaması için başsız Chrome ve yerel sunucu, Supabase'e anon anahtarla curl. **Eklenmeyecekler:** `rm -rf`, `git push --force`, `git reset --hard`, `git clean`, geçmişi yeniden yazan komutlar, Windows ayarları.
2. **Kalp atışı:** `Logs/overnight-heartbeat.txt` (git dışında olduğundan emin ol). Her adımın başında ve sonunda, en az 20 dakikada bir satır ekle:
   `2026-10-08 01:12 | ÇALIŞIYOR | <ne yapıyorsun> | ~<beklenen süre> dk`
   Uzun adımdan (IL2CPP build, tam test koşusu) önce mutlaka yaz. Diğer oturumlar başka bir oturumun çalışıp çalışmadığını buradan anlar.
3. **Kaldığım yer:** `docs/OVERNIGHT.md`'nin en üstüne bu gecenin bölümünü aç; önceki gecelerin raporları altta kalsın. En üstte her zaman güncel bir **Kaldığım yer** bölümü olsun:
   - son tamamlanan adım,
   - yarım kalan iş,
   - sıradaki somut adım,
   - commit'lenmemiş değişiklik var mı.

   Her adımdan sonra güncelle. Limit her an kesebilir.
4. Commit + push.

### 1. PM: durumu güncelle, M3 ve M4'ü değerlendir

`product-manager`'ı çağır, yukarıdaki "Durum"u rapor olarak ver. PM'den beklenenler:

- Akıllı Uygulama Denetimi kaydını düzeltmesi.
- Faz tablosunu ve açık işleri bugünkü duruma getirmesi.
- M3 ve M4'ün kapanıp kapanmayacağına karar vermesi. Önerim: 60 FPS ve Zeyd'in genel oynanış onayıyla kapat. Madde madde iPhone kontrollerini sabahki tek telefon oturumuna kısa bir liste olarak ekle; Zeyd zaten M5 test satırı için telefonda oynayacak.
- Seviye editörünün backlog'dan çıkıp sıradaki işler arasına alınması (Zeyd'in kararı).

### 2. M5: gönderimi canlıya aç

PRODUCT.md'deki "gönderimi canlıya açma kuralı"na göre. Zeyd (1)'i yaptı ama çıktıyı getirmedi; eksik doğrulamayı sen yap:

1. **SQL'in çalıştığını kanıtla**, yalnızca okuma isteğiyle: anon anahtarla `GET /rest/v1/plays?select=level_id,row_id,client,level_version&limit=1`.
   - Sütun yoksa PostgREST 400 döner. O zaman dur: gönderimi açma, sabah listesine "SQL tam çalışmamış" yaz ve hata metnini ekle, PM'e bildir, 3. adıma geç.
   - 200 (boş liste dahil) sütunların var olduğunu gösterir.
2. 200 ise: gönderim bayrağını aç ve build al (Brotli + gzip). Ardından size-log'a yaz, gh-pages `/br/` + `/gz/` yayınla, kök → `/br/` korunsun.
3. **Gece doğrulaması** (önce PM'e sor, onaylarsa): canlı adresi başsız Chrome'da `?debug=1` **olmadan** aç, oyuncu adı `TEST_GECE` olsun, L01'i bir kez oynat. Satırı yine anon anahtarla okuyarak doğrula: `client`, `level_id`='L01', `level_version`=1 ve `row_id` dolu; deneme no ve süre mantıklı. RLS okumayı engelleyip boş dönerse bu doğrulamayı Zeyd'e bırak. `TEST_GECE` satırlarının analizden çıkarılacağını PRODUCT.md'ye not et.
4. PM'e rapor ver. Kuraldaki son adım PM onayı; Zeyd'in sabah kontrolü listeye girer.

### 3. Seviye editörü (bu gecenin ana işi)

Önce PM'e "Seviye editörü tanımı"nı yazdır. Aşağıdaki taslak Zeyd'in isteği ve sohbetteki PM'in önerisi. PM son haline getirir, kabul ölçütlerini kesinleştirir ve PRODUCT.md'ye ekler. Sonra yap.

**Amaç:** Zeyd, koda ya da JSON'a dokunmadan, oyundaki görünümün aynısını görerek seviye kurar. Çözülebilirliği anında görür, kaydeder ve telefonda oynar. Faz 3'ün deney seviyeleri ve Faz 4'teki 40 seviyenin yeniden tasarımı bu araçla yapılacak.

**Nerede:** Unity'de, menüden açılır (**Reverse Solver → Seviye Editörü**).
- Teknik yol mühendisliğin kararı; tek şart: tahtada görülen parça oyundaki parçanın aynısı olacak (şekil, girinti/çıkıntı, renk, engel ikonları).
- Önerim: oyunun kendi çizim katmanını (BoardView/PieceView) kullanan, Play modunda çalışan bir editör sahnesi. Böylece "Oyna" düğmesi tasarımı anında gerçek oyunda açar.
- Telefonda düzenleme kapsam dışı (backlog).

**Kapsam:**
- **Başlangıç:** genişlik × yükseklik seçerek yeni seviye, ya da var olan bir seviyeyi kopya olarak açma.
- **Tahta, her parça ayrı:**
  - parça seçilince vurgulanır; yanda kenarları, rengi ve engeli görünür;
  - "Parçaları ayır" görünümü: parçalar aralıklı çizilir, her parçanın şekli tek tek görülür.
- **Yerleştirme (palet → tahta, sürükle-bırak):**
  - *Parça şablonları:* kenar kombinasyonlarına göre hazır parça şekilleri. Bir hücreye bırakınca o hücrenin kenarları şablona göre ayarlanır, komşular kendiliğinden uyar. Tek tek kenar düzenleme de olsun: iki parça arasındaki kenara tıklayınca kenar türü (`vEdge`/`hEdge` değerleri) döner. Uyumsuz komşu kenar hiçbir yoldan oluşamaz.
  - *Engeller:*
    - çivi (sayaçlı);
    - bomba (fitilli);
    - duvar (mühürlü kenar);
    - zincir: komşu hücreleri tek parça yapar, gerekirse ayırır.
    Sayı alanı tıklanınca düzenlenir; silgi ya da sağ tıkla kaldırılır. Web kuralının izin vermediği birleşimlere izin verilmez.
  - *Renk fırçası:* hücreleri seviyenin paletiyle boyar (seviyenin resmi). Palet düzenlenebilir.
- **Seviye ayarları:** süre, booster stokları ve açılış bölümleri, tanıtım kartı (başlık, metin, ipucu; Türkçe), resim adı. Engel tanıtım planı 3, 7, 12 ve 20. seviyeler (Zorluk deneyi ilkeleri); tanıtım kartları buna göre kurulabilmeli.
- **Geri al / yinele** (en az 50 adım). Kaydetmeden çıkarken uyarı.
- **Canlı kontrol paneli** (her değişiklikten sonra, ≤ 1 sn):
  - Çözülebilir mi (mevcut çözücülerle)? Tasarım kuralı: 0 patlama, 0 donma, 0 supapla çözülebilmeli. Çözülemiyorsa nedeni ve takıldığı parça.
  - Çözüm uzunluğu, engel sayıları (`LevelStats`), tahta boyutu.
  - Çözümü tahtada adım adım oynatma (ileri/geri).
  - Uyarı: tahta 375×667'de sığmıyor ya da hücre 44 pt'nin altına düşüyor.
- **Kaydetme:**
  - Tasarımlar ayrı bir set dosyasına gider (ör. `Assets/_Game/Levels/designs.json`), aynı formatta.
  - Her tasarımın kalıcı `id`'si olur (ör. D01, D02…; sıra değişse de değişmez, silinen id yeniden kullanılmaz) ve bir `version`'ı. Oynanışı etkileyen değişiklikte `version` kendiliğinden artar; yalnızca renk ya da metin değişince artmaz. Kayıtlı çözüm çözücüden üretilir.
  - Web'den gelen 40 seviye (`levels.json`) referanstır. Editör onları yalnızca "kopya olarak aç"ar, üzerine yazmaz; altın testler ve web karşılaştırması bunlara bağlı.
  - Oyunda hangi setin varsayılan olacağı ayrı bir PM kararı (playtest öncesi); bu gece değişmez.
- **Telefonda deneme:**
  - Tasarım seti web build'ine girer ama normal oyuncu görmez.
  - `?debug=1` ile tasarımlar listelenir ve doğrudan açılabilir (ör. `?debug=1&set=designs&lv=D03`; parametre adları mühendisliğin).
  - Debug oturumları gönderilmediği için telemetri kirlenmez.
- **Dışa aktarma:** tüm seviyelerin (web + tasarım) özellik tablosu CSV olarak.
  - Sütunlar: id, version, w, h, parça, zincir, çivi, bomba, duvar, süre, çözüm uzunluğu; varsa Monte Carlo değerleri.
  - Türkçe Excel'de doğrudan açılsın: `;` ayırıcı, ondalık virgül, UTF-8 BOM.
  - Faz 4 zorluk tablosunun girdisi.
- **Kılavuz:** `docs/LEVEL_EDITOR.md`, Türkçe, kısa, ekran görüntülü. Nasıl açılır, parça nasıl yerleştirilir, engel nasıl eklenir, nasıl kaydedilir, telefonda nasıl denenir. En başta "İlk seviyeni kur (10 dk)" adlı adım adım bir tur.
- **İsteğe bağlı** (vakit kalırsa, PM sıralar):
  - Hızlı Monte Carlo panelde: ör. 200 tohumla rastgele/akıllı oynayıcının kazanma oranı ve ortalama geçerli hamle sayısı. Faz 2'nin "ekransız çözücü + Monte Carlo" işinin ilk adımı.
  - "Çözülebilir rastgele tahta üret" düğmesi; Zeyd üstünden düzenler.

**Kabul ölçütleri** (taslak; PM kesinleştirir):
1. Mühendislik şu akışı kendisi baştan sona yapar ve her adımın ekran görüntüsünü `docs/screens/editor/`'e koyar: menüden editörü aç, 5×6 boş tahta kur, paletten parça ve engel yerleştir, kaydet, "Oyna" ile aynı seviyeyi oyunda oyna. Koda ya da JSON'a dokunmadan.
2. **Gidiş-dönüş:** 40 web seviyesinin her biri editörde açılıp değiştirilmeden kaydedilince anlamca aynı veri çıkar (EditMode testi).
3. Editörün çözülebilirlik sonucu, aynı seviyede GameSession + SmartPlayer sonucuyla aynı (EditMode testi; 40 web seviyesi + en az 3 tasarım).
4. **Tutarsız tahta oluşamaz:** uyumsuz komşu kenar, komşu olmayan zincir yok; sığmayan tahta uyarı verir (test).
5. **`version` kuralı:** oynanışı etkileyen değişiklik +1; yalnızca renk ya da metin değişince aynı kalır (test).
6. **Korunanlar:**
   - `levels.json` ve `Golden/` dosyaları değişmez.
   - Mevcut testlerin hepsi geçer.
   - Core Unity'ye bağlanmaz (`CoreIsolationTests`).
   - Editör kodu web build'ine girmez.
   - Build boyutu yalnızca tasarım verisi kadar artar (size-log).
7. Tasarım seti web build'inde `?debug=1` ile açılır; debug olmadan görünmez. Canlıda başsız Chrome ile doğrulanır.
8. CSV Türkçe Excel'de doğru sütunlarla açılır.
9. Kılavuz hazır.
10. *iPhone (Zeyd, sabah):* bir tasarımı telefonda `?debug=1` adresinden oynar.

### 4. Vakit kalırsa

Editör kapandıysa PM'in sırasıyla devam et:
- M6'nın playtest öncesi zorunlu kısmı (L01 öğretici el), ya da
- editörün isteğe bağlı maddeleri.

## Kurallar (her oturum için)

- **Kararlar:**
  - Ürün soruları önce PM'e gider.
  - PM'in Zeyd'e bıraktığı kararlar sabah listesine yazılır; o iş bekletilmez, başka işe geçilir.
  - Bir sorun 45 dakikadan uzun sürüyorsa pragmatik çözümü seç ya da PM'e götür, gerekçeyi yaz, devam et.
- **Commit:**
  - Her adım bitince: testler yeşil → commit (İngilizce mesaj) → push → **Kaldığım yer** güncellenir.
  - Derlenmeyen ya da testi kırık kod commit'lenmez.
  - Push öncesi anahtar taraması yapılır; repoya yalnızca anon/publishable anahtar girer.
- **Dokunulmayacaklar:**
  - `levels.json` ve `Golden/` dosyaları;
  - web reposu;
  - Supabase şeması (yalnızca okuma ve 2. adımdaki tek test oyunu serbest);
  - Windows güvenlik ayarları;
  - `git push --force`, `reset --hard`, geçmişi yeniden yazmak.
- **Unity:**
  - Unity açık olmalı (MCP).
  - Unity yanıt vermiyorsa ya da bir iletişim kutusu yolu kesiyorsa 10 dakikadan fazla uğraşma: Unity'siz yapılabilecek işe geç (kılavuz, Core kodu, rapor) ve durumu sabah listesine yaz.
  - Unity'yi zorla kapatma.
- **Bellek 8 GB:** aynı anda tek build çalışsın. İşi biten başsız Chrome'u ve yerel sunucuyu kapat.

## Yeni oturum başlarken (limit sonrası devam)

1. `Logs/overnight-heartbeat.txt`'nin son satırına bak. Son satır `ÇALIŞIYOR` ise ve üzerinden en büyük(25 dk, yazılan süre + 10 dk) geçmediyse başka bir oturum çalışıyor demektir. Hiçbir şey yapma; "Başka oturum çalışıyor, çıkıyorum." yaz ve bitir.
2. Saat 2026-10-08 10:00'ı geçtiyse ya da `docs/OVERNIGHT.md`'de `GECE BİTTİ` yazıyorsa hiçbir şey yapma ve bitir.
3. Değilse:
   1. Kalp atışını yaz.
   2. Şunları oku: `CLAUDE.md`, bu dosya, `docs/OVERNIGHT.md` (önce **Kaldığım yer**) ve PRODUCT.md'nin ilgili bölümleri.
   3. `git status` + `git log -5` ile yarım iş var mı bak. Commit'lenmemiş değişiklik varsa: derleniyor ve testler geçiyorsa commit'le, değilse düzelt.
   4. Sıradaki adımdan devam et.

## Gecenin sonu ve sabah raporu

Plan bitince ya da saat 09:30'u geçince yeni adım başlatma. Şunları yap:
1. `docs/OVERNIGHT.md`'nin en üstünü sabah raporu olarak toparla.
2. PM'i son kez çağır.
3. En üste `GECE BİTTİ` yaz.
4. Commit + push.

Rapor biçimi:
1. **Sabah ilk iş** (Zeyd için, en fazla 5 madde, adım adım; her birinde neye bakacağı ve ne getireceği). Muhtemel maddeler: Desktop'taki saatlik görevi duraklatmak, kılavuzdaki "İlk seviyeni kur" turu, M5 test satırı kontrolü, iPhone kısa kontrol listesi.
2. Neler bitti: milestone başına, ölçülen sayılar, doğrulanan ve doğrulanmayan.
3. PM kararları: kısa; ayrıntı PRODUCT.md'de.
4. Testler, commit'ler, yayın.
5. Açık işler.
