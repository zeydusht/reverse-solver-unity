# PM_MESAJ_106: Sürükleme sesi (Block Out hissi: önce araştır, sonra üret); zincir sesi gerçek zincir olsun; lastik her yerde; Ayarlar'da çift tık; dalış sonu "klink" kalksın

Kodu kendin yaz. p23 üzerinde. **PM_105 bitmediyse önce onu bitir.** Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (24 Kasım 19:12).

## 1. Küçük düzeltmeler
- **Ayarlar:** menüde Ayarlar'a basınca **iki tık** sesi geliyor. Tek tık olsun (Durdur sesi). Aynı anda çalan iki ses tetikleyicisini bul (düğme + panel açılışı gibi), birini kaldır. Bütün düğmelerde "aynı karede iki ses" denetimi: test bunu yakalasın.
- **Dalış sonu:** tabloya varınca çalan "klink / ting" sesi kalksın. Dalış whoosh'u kalır.

## 2. Sürükleme sesi: Block Out hissi
Zeyd: "Sürükleme sesi hâlâ çok kötü. Block Out'takine benzer olabilir mi? Önce hissiyatı neden iyiydi araştır, netleştir, sonra ona uygun ses üret."

**Adım A, araştırma (rapor önce):**
- Referans `docs/art/pm106/ref_blockout_oynanis.mp4` (sesli Block Out oynanışı, ~52 sn). Yalnız referans, oyuna girmez.
- Kare kare izle. Her parça hamlesinde **ne zaman hangi ses çıkıyor**:
  - dokunma anı;
  - sürükleme sırasında;
  - parça kapıya / çıkışa vardığında;
  - parça yok olurken;
  - art arda çıkışlarda sesin değişip değişmediği (perde yükseliyor mu);
  - titreşim / görsel efektle hizası.
- Müziği ayır (HPSS / spektral çıkarma) ve olay seslerinin tınısını tarif et: süre, parlaklık, gövde, saldırı.
- Rapor `docs/art/pm106/BLOCKOUT_SES_ANALIZ.md`: **"Neden iyi hissettiriyor"** maddeleri.

  PM'in kontrol etmeni istediği hipotezler:
  - (1) ses sürüklerken değil, **sonuçta** (çıkış anında) çalıyor;
  - (2) çıkış sesi kısa, tok ve tatmin edici bir "pop/şak" + parıltı;
  - (3) art arda çıkışlarda perde hafifçe yükseliyor;
  - (4) dokunma anında çok hafif bir "tık";
  - (5) ses, görsel patlama ve titreşim aynı karede.

  Hangileri doğru, hangileri yanlış: kanıtla.

**Adım B, uygulama:**
- Analize göre bizim sürükleme/çıkış ses tasarımımızı kur. Bizim mekanik farklı (parça tahtadan çıkıyor) ama his aynı olsun.
- CC0 kayıtlardan ve gerekirse katmanlayarak (kısa pop + hafif parıltı) **en az 5 aday**.
- Ardışık çıkış perdesi (hipotez 3 doğruysa): her çıkışta yarım ton yukarı, 4–5 adımda döner; kayıp/takılmada sıfırlanır.
- Dinleme sayfası: "Sürükleme / çıkış (Block Out tarzı)" + referansla karşılaştırma (yerel) + "10 art arda".
- Oyunda `&sescikis=` ile denenebilsin.

## 3. Zincir sesi zincire benzemiyor
- **Sebebi bul:** dosya mı yanlış (sentetik, çok kısa, filtrelenmiş), dönüştürme mi bozdu (PM_104_EK), çok mu kısık / perde mi kaymış? Raporla.
- **Gerçek zincir kaydı** (CC0: metal zincir şıkırtısı, halka sürtünmesi):
  - sürüklerken ve sallanırken hareket hızına göre şiddet;
  - en az 2–3 sürekli şıkırtı varyasyonu, tekrar etmeyen;
  - gerilmede kısa metal "çın".
- 3 aday, dinleme sayfasında.

## 4. Lastik her yerde
Zeyd: "Zincirde lastiği kaldırdınız mı? Duvara çarpınca lastik yokmuş gibi geldi. İleri bölümlerde yok mu?"
- **Kural:** lastik (squash + geri sekme, `lastik=2`) **bütün takılmalarda** aynı. Ne tür engel olursa olsun: komşu parça, tahta kenarı, kırmızı duvar, zincir gerilmesi, kilitli parça, bombalı parça. Bütün bölümlerde (1–100).
- PM_105'teki zincir düzeltmesi lastiği kapatmış olabilir. Kontrol et ve geri getir (zincir sınırı kuralı korunarak).
- **Denetim:** hangi kod yolları lastiksiz "eski takılma" animasyonuna düşüyor? Hepsini lastiğe bağla.
- **Test:** rastgele 20 bölümde ve her engel türünde birer takılma. Her birinde büzülme oranı ≥ lastik 2 değerinin %90'ı. Tablo raporda.

## Teslim
- EditMode + PlayMode yeşil, WebKit ses testi hatasız, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (yeni sürükleme ve zincir adayları)
  - `p23/?c=p23&test=1&lang=tr&lv=31` (zincir sesi + lastik)
  - `p23/?c=p23&test=1&lang=tr&lv=21` (kırmızı duvar + lastik)
  - `p23/?c=p23&lang=tr` (Ayarlar tek tık)
