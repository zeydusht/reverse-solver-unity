# PM_MESAJ_93: Takılmada lastik gibi esneyen parça + Koleksiyon'dan tekrar oynayınca menü bozuluyor (HATA)

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. **Kapsam dar: yalnız bu iki madde.**

Dosyalar `docs/art/pm93/` içinde:
- `takilma_ref.mp4`: Zeyd'in kaydı; sol üstteki parçayı tekrar tekrar sağa itiyor;
- `bug_menu_bozuk.png`: hata ekranı.

## 1. Takılma animasyonu: lastik / oyuncak hissi
Zeyd (20 Kasım 01:04): "Parça sağa gidip çarpıp geri geliyor, bu animasyon güzel olmuş. Geliştirelim. Lastikmiş gibi: gidiyor, duvara çarpıyor, iyice büzülüyor, sonra açılarak geri sekip yerine oturuyor. Oyuncak gibi bir cisim."

Şimdiki hâl (PM_75): parça temas noktasına kadar kayıyor, %6 sıkışma, ~80 ms durma, yaylı dönüş. Yeni hâl:

**Boşluklu takılma (parça bir mesafe gidip çarpıyor):**
1. **Gidiş:** gittiği yönde hafif uzar (gidiş ekseninde %6–8 uzama, diğer eksende hafif incelme). Hacim korunuyor gibi görünsün.
2. **Çarpma:** temas anında belirgin büzülme. Çarpma ekseninde **%18–22 basılma**, diğer eksende **%10–12 genişleme** (squash). Büzülme çarpılan kenara yaslanır, karşı taraf sıkışır; pivot temas kenarı.
3. **Açılma ve geri sekme:** büzülmeden açılırken geri itilir. Dönüşte ters yönde hafif uzar, yuvasına gelince **1 küçük aşma** (yuvayı ~%4 geçip geri) ve 1–2 sönen salınımla oturur. Yay benzetimi, sönümlü.
4. **Süre:** toplam ~0,45 sn (gidiş mesafeye bağlı). Fazla uzun olmasın: art arda itince yeni hareket animasyonu kesip başlayabilsin.

**Boşluksuz takılma (parçanın hemen yanında engel var):**
- Parça itilen yöne **çok az** hareket eder (hücrenin ~%8'i), engele değip büzülür (%10–12 basılma), sonra yaylanarak geri gelir ve küçük bir salınımla oturur. "Bouncy" ama kısa (~0,3 sn).

**Ortak kurallar:**
- Resim parçayla birlikte esner (aynı doku, eşit dağılımla). Tırnaklar ve kenar çizgisi bozulmasın, kopma ya da yırtılma görünmesin.
- Ses: çarpmada "tok" sesi (mevcut). Titreşim: çarpmada Light (mevcut kural).
- Kırmızı parıltı yalnız temas anında, kısa (mevcut).
- Zincirli parça: zincir boyu kuralı aynen (PM_63). Esneme zincirin sınırında olur.
- Bomba ve kilit görselleri parçayla birlikte esner, ayrı kalmaz.
- **Ayar bayrakları:** `&lastik=` (esneme çarpanı, varsayılan 1; 0 = eski PM_75 hâli), `&lastiksure=` (sn).
- GIF yarım hız: boşluklu ve boşluksuz takılma, `docs/art/pm93/lastik/`. 60 fps, 20 ms'yi aşan kare 0.

## 2. HATA: Koleksiyon'dan bölümü tekrar oynayıp menüye dönünce menü bozuk
**Zeyd'in adımları:**
1. Koleksiyon sekmesi.
2. 2. bölüm → Tekrar oyna.
3. Oynadı.
4. Oyundan çıkıp ana menüye döndü.

**Sonuç** (`bug_menu_bozuk.png`):
- arka plan **ikiye bölünmüş**: sol yarı koridor, sağ yarı başka bir salon (merdivenli);
- ortada bir sonraki bölümün tablosu **çerçevesiz**, eğik, havada duruyor;
- altında siyah bir dikey şerit;
- "Bölüm 5" düğmesi ve alt çubuk normal.

**Muhtemel sebep:** Koleksiyon'dan girişte dalış/diorama, koridor ya da sekme sayfası durumu sıfırlanmıyor. Menüye dönüşte eski kamera pozu, yarım kalmış sekme kayışı ve dioramanın tablo katmanı üst üste kalıyor. Bul ve raporla.

**Düzeltme:**
- Menüye her dönüşte (oyundan, Koleksiyon'dan, kazanma/kaybetmeden, Durdur → Menü'den) menü sahnesi temiz başlangıç durumuna kurulsun: kamera menü pozu, diorama kapalı, sayfalar doğru konumda, çerçeve + resim birlikte.
- Koleksiyon'dan tekrar oynanan bölüm de dalışla girsin. Dönüşte sıradaki bölümün ("Bölüm 5") tablosu menüde normal görünsün.

**PlayMode testi** (hepsinden sonra menü ekranının referans görüntüyle aynı olduğunu doğrula):
- Koleksiyon → bölüm → kazan → Menü;
- Koleksiyon → bölüm → Durdur → Menü;
- Koleksiyon → bölüm → kaybet → Menü;
- normal bölüm → Menü.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr&lv=2` (takılma: parçayı engele it)
  - `p23/?c=p23&test=1&lang=tr&lv=2&lastik=0` (eski hâl, karşılaştırma)
  - `p23/?c=p23&lang=tr` (Koleksiyon → tekrar oyna → menüye dön)
