# Görev: kenar parçaları (K1) ve seviyeye görsel (G1)

Yazan: Zeyd'in PM'i (claude.ai), 2026-10-08 akşam. Zeyd bu dosyayı onayladı ve "bizimkiler hazırlasın" dedi.
Okuma sırası: 0 → K1 → G1 → Build. Kurallar en altta, hepsi geçerli.

---

## 0. Önce yarım kalan iş

- Son commit `a7b1433` (10/8 00:54). Ondan sonra L01 öğretici el yazılmaya başlanmış ama **commit edilmemiş**. 00:56–01:01 arasında değişen dosyalar:
  - `Core/Session/Tutorial.cs`
  - `Presentation/Board/TutorialHand.cs`
  - `Tests/EditMode/TutorialTests.cs`
  - `BoardView.cs`
  - `GameRoot.cs`
- Kalp atışı ve OVERNIGHT.md 00:50'den beri güncellenmedi. Oturum muhtemelen limit ya da izin sorusunda durdu.
- Yapılacaklar:
  1. `git status` ile durumu gör.
  2. Testleri çalıştır, öğretici eli bitir ve ayrı bir commit at.
  3. OVERNIGHT.md'deki "Kaldığım yer"i güncelle.
- Sonra `product-manager`'a bu dosyadaki **K1** ve **G1**'i `docs/PRODUCT.md`'ye ekletip şu sırayı onaylat:
  1. L01 el
  2. K1
  3. G1
  4. Build ve yayın
  5. Kademe 2'nin kalanı

---

## K1 — Kenardaki parçaları çıkarmak zor (Zeyd'in iPhone gözlemi)

**Sorun:** Sağ ve sol sütundaki parçaları ekranın dışına çekmek zor.

- Bugün yan boşluk yaklaşık 19–20 pt. Hücre boyutu `(W − 38) / w` ile hesaplanıyor, tepsi ekran kenarına 1–2 pt kalıyor.
- Dış sütundaki bir parçanın dışarı çıkması için ≈0,48 hücre (59 pt hücrede ≈28 pt) çekilmesi gerekiyor.
- Parmak parçanın dış yarısından tutarsa bu mesafe ekran kenarını, yani çerçeveyi aşıyor. Bu durumda dokunma kaybolabilir ya da iptal gelebilir; iptal gelince parça geri yaylanıyor.

**Yapılacaklar:**

1. **Önce ölç.**
   - `?debug=1` satırına son sürüklemenin sonucunu yaz: `Exited`, `SnappedBack` ya da `Cancel`, ofset / gereken ofset (pt) ve basılan nokta tahtanın içinde mi dışında mı.
   - Böylece Zeyd'in testinde hangisinin olduğunu göreceğiz:
     - (a) iptal mi geliyor,
     - (b) mesafe mi yetmiyor,
     - (c) parmak hiç parçayı tutmuyor mu (boşluğa basıyor).
2. **Yan boşluk.** `GameRoot.BoardPlacement`'ta her iki yana `G = clamp((W − 44·w) / 2, 19, 34)` pt boşluk bırak.
   - Hesaplanan sonuçlar:

     | Ekran | Tahta | Hücre (eski → yeni) | Boşluk (eski → yeni) |
     |---|---|---|---|
     | 393 pt | 6×6 | 59 → 54 | 20 → 34 |
     | 375 pt | 6×7 | 56 → 51 | 22 → 34 |
     | 430 pt | 6×6 | 65 → 60 | 20 → 35 |

   - Bugünkü 40 seviyenin hiçbiri 44 pt'nin altına inmiyor.
   - Editördeki "hücre 44 pt'nin altında" uyarısı aynı fonksiyonu kullansın; hesap tek bir yerde dursun.
   - 34 bir başlangıç değeri; son değeri PM onaylar.
3. **Dışa taşan tutma alanı.**
   - Tahtanın dışında, dış kenara en fazla 24 pt mesafeye basmak, en yakın dış sıra ya da sütundaki parçayı tutsun. Mühür duvarına basmak da aynı işi görsün.
   - Tutulan parça her yöne normal şekilde sürüklenir.
4. **Fiske (hızlı kaydırma).** Bırakma anında şu üç koşul birlikte sağlanırsa parça çıksın:
   - parmağın çıkış yönündeki hızı ≥ 900 pt/s,
   - parça o yönde çıkabiliyor,
   - ofset çıkış mesafesinin en az %20'si.

   Mevcut %42 kuralı aynen kalıyor. Sabitler `DragModel`'de dursun. Fiske jam saymaz.
5. **Dokunma iptali.**
   - Sistem dokunmayı iptal ederse (kenar hareketi, odak kaybı) ve çıkış koşulu o anda sağlanmışsa parça çıkmış sayılsın.
   - Sağlanmamışsa parça geri yaylansın. İptal jam saymaz.
6. **Web'den bilinçli sapma.** Fiske, iptalde çıkış, dışa taşan tutma alanı ve yan boşluk web sürüklemesinde yok.
   - PM bunu karar kaydına gerekçesiyle yazsın. Gerekçe: Zeyd'in iPhone gözlemi; Unity playtest'i henüz başlamadı, yani bütün Unity verisi yeni yerleşimle toplanacak.
   - Kural motoru değişmiyor. `travel_golden` ve `session_golden` testleri olduğu gibi geçmeli.

**Kabul ölçütleri (K1):**

- EditMode testleri yeşil. Yeni testler:
  - fiske ile parça çıkıyor,
  - yavaş ve kısa çekiş çıkmıyor,
  - iptalde eşiğin üstündeyse çıkıyor, altındaysa geri yaylanıyor,
  - dışarıdaki dokunuş en yakın kenar parçasını tutuyor,
  - hiçbir seviyede hücre 44 pt'nin altına inmiyor,
  - jam sayımı eskisi gibi.
- Ekran görüntüleri: 375×667, 390×844 ve 430×932 boyutlarında L01 ile en büyük tahta (6×7). Boşluk görülsün.
- Zeyd'in iPhone testi: L01'de ve bir 6×7 seviyede sağ sütundan, sol sütundan, üst sıradan ve alt sıradan parçalar çekilir. Toplam 10 denemenin en az 9'u tek seferde çıkmalı. Debug satırındaki sonuçlar not edilir.

---

## G1 — Her seviyeye ayrı görsel (görselli yapboz)

**Zeyd'in isteği:** Seviye editöründe, tahtanın göründüğü ana ekrana bir görsel yükleyebilmek. Parçalar o görselin parçaları gibi görünsün; her seviyenin kendi görseli olsun.

**Kapsam:** Şimdilik yalnızca tasarımlar (`designs.json`, debug adresi).

- `levels.json`'daki 40 canlı seviyeye görsel koymak bir görsel kimlik kararıdır. Bu, PRODUCT.md'deki "Faz 3 bitmeden yeni görsel kimlik yok" kararına dokunur.
- PM bunu Zeyd'e karar olarak sunsun. Mühendislik canlı sete uygulamasın.

**Veri:**

- `designs.json`'a isteğe bağlı bir alan: `"image": "D03.jpg"`.
- Bu bir görünüş alanı, sürüm artmaz (LEVEL_EDITOR.md'deki sürüm kuralı).
- `art` alanı (piksel resim adı) ayrı kalır.
- `LevelParser` ve `LevelData`'ya `Image` eklenir; null olabilir.
- `levels.json` bayt bayt aynı kalmalı.

**Editör (Seviye bölümü):**

- **Görsel seç…** düğmesi: `EditorUtility.OpenFilePanel` ile png, jpg ya da jpeg seçilir.
- **Görseli kaldır** düğmesi ve küçük bir önizleme.
- İçe alırken:
  - Görsel tahta oranına (w:h) göre ortadan kırpılır (cover).
  - Hücre başına yaklaşık 150 px olacak şekilde ölçeklenir; en uzun kenar en fazla 1024 px.
  - JPG kalitesi yaklaşık 80; sonuç `Assets/StreamingAssets/LevelImages/<id>.jpg`'ye yazılır.
- En uzun kenarı 2048 px'e küçültülmüş bir kaynak kopyası `Assets/_Game/Levels/ImageSources/<id>.<uzantı>`'da tutulur (build'e girmez). Tahta boyutu değişince görsel bu kopyadan yeniden kırpılır.
- Zeyd'in seçtiği orijinal dosyaya hiç dokunulmaz.
- Görsel tahtada hemen görünür. Kaydedince `designs.json`'a yazılır.
- İsteğe bağlı (vakit kalırsa): "kaydır / yakınlaştır" ile kırpma ayarı.

**Çizim (`Piece.shader` + `PieceView`):**

- Görsel isteğe bağlı bir doku olarak verilir. uv2'de hücrenin (x, y) konumu, property block'ta `_BoardCells` (w, h) ve `_Image` bulunur. Fragment, tahta UV'sini `(hücre·100 + q) / (wh·100)` ile hesaplar (y ters).
- Çıkıntılar komşu hücrenin resminden devam eder; gerçek yapboz gibi görünür.
- **Okunabilirlik oyunun özüdür.** Beyaz ve koyu kenar şeritleri kalır.
  - Dolgu: görsel, üzerinde mevcut gradyan ve vurgunun hafifletilmiş hali.
  - Çıkıntı ve girintiler her görselde seçilebilmeli. Gerekirse dolgunun kontrastı %10–15 düşürülür ya da dikiş şeridi kalınlaştırılır.
- Çivili gri görünüm, engelleyen parçanın parlaması, kaldırma büyümesi ve zincir aynen çalışmalı.
- Görsel yoksa ya da henüz yüklenmediyse bugünkü renk ve piksel resim görünür.

**Yükleme ve boyut:**

- Görseller ilk indirmeye girmez.
  - Seviye açılınca `StreamingAssets/LevelImages/<id>.jpg` `UnityWebRequestTexture` ile istenir.
  - Gelene kadar renkler görünür; gelince 0,25 sn'lik bir geçişle görsel belirir.
  - Sonraki seviyenin görseli arka planda önceden indirilir.
  - Önceki seviyenin dokusu bellekten bırakılır.
- `size-log`: wasm ve data boyutu değişmemeli. Görsel dosyalarının boyutları rapora yazılsın; görsel başına hedef ≤ 200 KB.
- Ağ hatası ya da 404 olursa oyun renklerle sorunsuz oynanır, hata kutusu çıkmaz.

**Test görselleri:**

- Telifli görsel kullanma. 2–3 test görselini kodla üret: renkli gradyanlar ve şekiller.
- Bunları D01–D03'e koyup ekran görüntülerini al. Gerçek görselleri Zeyd koyacak.

**PM önerisi (G1-b, PM karar verir, şimdilik yapma):**

- Kazanınca bitiş kartında görselin tamamı küçük bir resim olarak gösterilsin ("resmi tamamladın" ödülü).
- Alternatif tasarım: resim parçaların altında dursun, parçalar çıktıkça ortaya çıksın.

**Kabul ölçütleri (G1):**

1. Editörde bir tasarıma görsel seçilir ve tahtada hemen görünür. Kaydedince `designs.json`'da `image` alanı olur. Unity kapatılıp açıldığında görsel yerindedir.
2. Görseli kaldırınca renkler geri döner. Görsel değişikliği sürümü artırmaz.
3. Çıkıntı ve girintiler görselli tahtada okunur. Editörden ve 390×844 oyundan ekran görüntüleri PM'e gider.
4. **Oyna ▶** ile oyunda görsel görünür. Sürükleme, çivi grisi, bomba, zincir ve parlama doğru çalışır.
5. Web build'inde `?debug=1&set=designs&lv=D01` telefonda görselli açılır. İlk indirme boyutu değişmez.
6. Görsel gelmezse oyun renklerle oynanır.
7. EditMode testleri:
   - parser `image` alanını okur ve yazar,
   - sürüm artmaz,
   - `levels.json` değişmez.
8. `docs/LEVEL_EDITOR.md`'ye ekran görüntüleriyle "Görsel ekle" bölümü eklenir.

---

## Build ve yayın

- L01 el, K1 ve G1 bitince Brotli ve gzip build'lerini al. `Builds/publish`'ten `gh-pages`'e yayınla.
- `size-log`'a bak, PM'e rapor ver ve OVERNIGHT.md'yi güncelle.
- Zeyd'e telefon test listesi hazırla:
  - K1 ölçütündeki kenar çekme testi,
  - D01'in görselli hali,
  - L01'deki el.
- Canlı veri gönderimi: **yalnızca Zeyd bu oturumda kendi mesajıyla açıkça onay verdiyse** M5 akışını (aç, build, yayınla, `TEST_` satırıyla doğrula) bu build'e kat. Onay yoksa kapalı kalır.

## Değişmeyen kurallar

- Şunlara dokunma:
  - `levels.json`
  - `Tests/EditMode/Golden/`
  - `../web-reference`
  - Supabase şeması
- `git push --force`, `reset --hard` ya da geçmişi yeniden yazmak yok.
- Repoya Supabase anon anahtarı dışında hiçbir anahtar girmez.
- Debug oturumları hiçbir zaman gönderilmez. Windows güvenlik ayarlarına dokunulmaz.
- Commit mesajları İngilizce olur. Her mesajın sonunda "Zeyd'in yapacakları" ve "Açık işler" bölümleri bulunur.
