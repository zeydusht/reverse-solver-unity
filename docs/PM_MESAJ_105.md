# PM_MESAJ_105: Zincirli parçada lastik hatası; zincir / kilit / bomba / duvar sesleri; kötü düğme sesinin her yerden kaldırılması

Kodu kendin yaz. p23 üzerinde. **PM_104 / PM_104_EK bitmediyse önce onları bitir.** Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd'in iPhone kayıtları `docs/art/pm105/` içinde:
- `zincir_lastik.mp4`, `zincir_lastik_2.mp4`: zincirli parça;
- `zincir_kareler_6fps.png`: kareler;
- `menu_sesleri.mp4`: menü ve düğme sesleri.

## 1. HATA: zincirli parçada lastik takılma bozuk
**Zeyd:** "Üstteki parça lastik gibi çarpıyor ama alttaki, zincirli parça, sıçrıyor. Sağa doğru sıçrayarak gidiyor. Yanında boşluk varken iyice sıçrıyor, uçuyor. 5. bölümde lastik çok düzgün çalışıyordu."

**PM gözlemi** (kareler):
- Zincirin bir ucundaki parça itilince takılma animasyonu yerine **zincirin öbür ucundaki parça** ya da itilen parça çok uzağa fırlıyor, sonra geri dönüyor.
- Esneme ve geri sekme bir parçaya uygulanıyor, zincirle bağlı öteki parçaya uygulanmıyor ya da iki kural (PM_63 zincir boyu sınırı + PM_93/94 lastik gidişi) çakışıyor.
- Boşluk büyüdükçe fırlama artıyor: lastik gidiş mesafesi zincir sınırını dikkate almadan, ilk temas noktasına göre hesaplanıyor gibi.

**Kural:**
- Zincirli parçanın gidebileceği en uzak nokta **min(ilk temas, zincir boyu)**. Lastik gidişi, büzülme ve geri sekme bu sınıra göre.
- **Zincir gerildiğinde** "çarpma" zincirin gerilmesi olur:
  - parça zincir boyunca gider;
  - zincir gerilir (Verlet ip gerginliği);
  - parça gerilme yönünde hafif büzülür, lastik gibi geri seker.
  - Zincirin öbür ucundaki parça **yerinden oynamaz** (en fazla çok hafif titreme).
- Fırlama, aşma ya da ışınlanma yok. Aşma ≤ %6 (PM_94 lastik 2 kuralı).
- **Test:** EditMode / PlayMode, 6 durum:
  - zincirli parça × (boşluksuz / 1 boşluk / 3 boşluk) × (zincir sınırı daha yakın / temas daha yakın);
  - her karede parça konumu sınırı geçmiyor;
  - öbür uç sabit.
- GIF önce/sonra `docs/art/pm105/zincir_once_sonra.gif`.

## 2. Yeni sesler (CC0 kayıt; 3'er aday, dinleme sayfasında)

| Olay | Ses |
|---|---|
| Zincirli parça sürüklenirken / sallanırken | zincir şıkırtısı; hareket hızına göre şiddeti; durunca söner |
| Zincir gerilip takılınca | kısa metal "çın" + şıkırtı |
| Kilitli parçaya dokununca (henüz açılmamışken) | kilit tıkırtısı ("kilitli" hissi, kısa) |
| Kilit açılınca | anahtar dönme sesi + kilidin düşüş sesi (varsa kontrol et; yoksa ekle) |
| Bomba patlayınca (kayıp) | patlama sesi: oyuncak tadında, korkutucu değil; ardından kayıp paneli |
| Kırmızı duvara çarpınca | tok "duvara çarpma" (bonk), lastik çarpmadan biraz daha sert |

Hepsi efekt seviyesinde, tıkla tutarlı. Safari uyumlu biçimde (PM_104_EK).

## 3. Kötü düğme sesi ("çıt çıt") her yerden kalksın
**Zeyd:**
- "Durdurma butonunun sesi hoşuma gitti." → bu ses **tek düğme sesi** olsun.
- "Menüde en aşağıdaki bölüm düğmesine tıklıyorum, hep o çıt çıt ses değişerek kötü geliyor."
- "Süre bitti, para verip uzattım; o butonun sesi de kötü."

**Yapılacak:**
- Projedeki **bütün** dokunulabilir öğelerin sesini tara (PM_98 testi ve listesi):
  - Oyna / Bölüm düğmesi;
  - alt sekmeler;
  - Süre doldu panelindeki altınla devam ve reklamla devam;
  - kazanma düğmeleri;
  - sandık;
  - Ayarlar;
  - Koleksiyon kartları;
  - X / kapat;
  - booster seçimi;
  - reklam düğmeleri.
- Hepsi **Durdur düğmesinin sesini** kullansın. Rastgele perde/varyasyon **yok** (aynı ses, aynı seviye).
- Eski "çıt" sesinin dosyası projeden kaldırılsın; başka bir kod yolu onu çalamasın.
- **İstisnalar** (kendi sesi kalır): sandık açılışı, ödül uçuşu, satın alma onayı (Success sesi; tık + kısa "kaching" olabilir, kötü çıt değil).
- **Tablo** `docs/TITRESIM_HARITASI.md` ses sütunu: her öğe → ses dosyası. "Çıt" sesi 0 yerde.
- **WebKit testi:** bütün düğmeler tetiklenir, çalan dosya listesi kaydedilir; eski dosya hiç çalmaz.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr&lv=31` (zincir + bomba)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kilit)
  - `p23/?c=p23&lang=tr` (menü düğme sesleri)
  - `p23/onizleme/sesler.html` (yeni sesler)
