# PM_MESAJ_98: Ses seçimleri; yeni bölüm müziği adayları (Block Out tarzı); duraklatma sesi; UI sesleri duyulur olsun; her dokunuşta ses + titreşim denetimi

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses ve titreşim.

## 1. Zeyd'in seçimleri (23 Kasım): varsayılan yap
| Ses | Seçim |
|---|---|
| Lobi | n (Urban Shop) |
| Bölüm | **c** (şimdilik; Zeyd beğenmedi, 2. madde) |
| Kazanma | b |
| Çıkış | c |
| Çarpma | a |
| Son parça | a |
| Tık | b |

## 2. Bölüm müziği: yeni adaylar
Zeyd hiçbir bölüm adayını beğenmedi. **Hedef tarz:** referans oyun (`docs/art/pm96/ref/ref_bolum.mp3`, Block Out!):
- ~136 BPM;
- enerjiyi büyük ölçüde davul ve perküsyon taşıyor (vurmalı oranı 0,92);
- parlak, neşeli, "arcade puzzle" havası;
- tekrar ettikçe yormayan, sade melodi.

Bizim tarz dokunuşu olabilir (pizzicato, marimba), ama ritim önde.

**İki yoldan aday üret:**
- **A. Kendi bestemiz (en az 4 aday):** PM_96'daki fanfarlar gibi CC0 gerçek çalgı kayıtları (VCSL vb.) + CC0 davul/perküsyon kayıtlarıyla sırala.
  - 130–140 BPM, 4/4.
  - Güçlü ve temiz davul grubu (kick, clap/snare, shaker/hi-hat), bas hattı, 8 ölçülük akılda kalan ama abartısız bir motif, 2 bölümlü yapı (A/B).
  - 60–90 sn kusursuz döngü, −18 LUFS.
  - Kendin dinleyemediğin için:
    - ölçümle doğrula (tempo sabitliği, vurmalı oranı referansa yakın 0,7–0,9, döngü dikişi, parlaklık);
    - midi/nota planını rapora yaz.
- **B. Daha geniş CC0 araması (en az 4 aday):** "upbeat arcade puzzle", "casual game loop", "hyper casual" türlerinde, yine yalnız CC0 / atıfsız ticari kullanım. Vurmalı oranı ≥ 0,6 olmayanları eleme ölçütü yap.

**Dinleme sayfası:**
- `sesler.html`'de "Bölüm (yeni)" bölümü;
- her adayın yanında referansla 10 sn'lik "karşılaştır" düğmesi: önce ref, sonra aday. Ref yalnız yerel önizlemede çalsın; Pages'e yüklenecekse referans dosyasını **koyma**.

## 3. Duraklatma (Durdur paneli)
**Kural** (çoğu casual oyundaki gibi):
- Durdur'a basınca tık sesi + Select titreşimi;
- bölüm müziği durmaz, %35'e iner;
- oyun efektleri (bomba tik-takı vb.) durur;
- Devam'a basınca müzik 0,3 sn'de eski seviyesine döner.

Ayarlar panelinde de aynı.

## 4. UI sesleri neredeyse duyulmuyor
Zeyd: "Butona basmışım basmamışım belli değil. Alt barda bir şeye basıyorum, neredeyse hiç ses duymuyorum."
- **Ses düzeyi:** UI tık sesi müziğin belirgin üstünde olsun (yaklaşık −14 LUFS kısa ses; müzik −18). Telefonda orta seste net duyulsun. Diğer efektleri de bu düzeye göre gözden geçir; çıkış ve çarpma kaybolmasın.
- **Her dokunulabilir öğe ses çalsın:**
  - alt sekmeler;
  - Oyna;
  - Durdur / Devam / Menü / Tekrar;
  - Ayarlar anahtarları;
  - booster seçimi;
  - sandık;
  - kazanma düğmeleri;
  - reklam düğmeleri;
  - Koleksiyon kartları;
  - kapatma (X).
- **Ses tipleri:**
  - panel açılırken kısa yumuşak "vuş";
  - kapanırken ters "vuş";
  - anahtar açma/kapama için ayrı tık;
  - pasif düğmeye basışta boğuk "tok".

  Hepsi CC0 kayıtlı ses.
- **Denetim:** bütün düğmeleri tarayan bir PlayMode testi. Ses ya da titreşimi olmayan dokunulabilir öğe listesi 0 olsun. Liste `docs/TITRESIM_HARITASI.md`'ye ses sütunu olarak eklensin.

## 5. Titreşim notu
- Titreşim **iPhone'da yalnız uygulamada (TestFlight / App Store) çalışır.** iPhone Safari'de web sürümünde titreşim yok (Apple izin vermiyor). Bunu Ayarlar'daki titreşim anahtarının altına küçük bir not olarak **yalnız web'de** göster: "Titreşim uygulamada çalışır".
- iOS build'de her dokunuşta titreşim (PM_86 haritası) kontrol edilsin: iOS kodu yolunda her UI öğesi haptic çağırıyor mu, test et.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (yeni bölüm müzikleri)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (Durdur, UI sesleri)
