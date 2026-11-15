# PM_MESAJ_74: GECE İŞİ (6–7 saat): oyunu cilala, mükemmel UI

Zeyd uyuyor, sabaha kadar çalış. **Erken bitirme.** Liste bitince denetim listesinden bir sonraki maddeye geç. Zeyd'in sözü: "Çok iyi bir iş istiyoruz. Karmaşık hale getirme, smooth işler yap. Vasat şeyi onaylamayın, en iyisi olana kadar çizdirin."

## Güvenlik (işi batırma)
- **Yeni klasör `p23/`.** p22, p21, p20, kök ve pt1'e dokunma. p22 yedek olarak kalır.
- Yeni dal `pm74`. Her aşama sonunda commit + p23'e yayın + `verify_build` p23.
- Bir değişiklik bir şeyi bozarsa o değişikliği geri al, raporla, devam et. Takılıp durma.
- Force push yok. Repo dışında silme yok. Supabase'e yazma yok.
- Dal değiştirmeden önce `docs/` commit.
- Her aşamada EditMode ve PlayMode yeşil kalsın.

## Kalite kuralı (çizim ve animasyon)
- **Codex:** `codex exec` en güçlü modelle ve en yüksek düşünme ayarıyla. Zeyd "extra high" istedi; `codex --help` ve config'e bak, desteklenen en yüksek `model_reasoning_effort` değerini kullan. Komut `< /dev/null`, `timeout 1800`.
- **Her görsel ya da animasyon için döngü:**
  1. En az 3 farklı fikir.
  2. Oyuna koyup 390×844 ekran görüntüsü ya da GIF al.
  3. Aşağıdaki ölçütle puanla. En iyisini en az 2 tur daha iyileştir.
  4. Ölçütün hepsini geçmeyen hiçbir şeyi oyuna bağlama; o durumda eskisi kalsın.
  5. En iyi 2 seçeneği Zeyd için sakla.
- **Ölçüt:**
  - onaylı tarz (PM_65 listesi: old money mavi, krem panel + mavi çerçeve, altın, kalın 3B yazı, kurdele yok, opak ve doygun);
  - tek bakışta anlaşılır;
  - "wow" anı var;
  - kaba kenar ve taşma yok;
  - 60 fps;
  - süre: booster ≤ 1,5 sn, geçişler ≤ 0,8 sn;
  - sonda yumuşak yavaşlama, ani durma yok.
- Örnek seviye: Royal Match booster'ları (`docs/art/booster ref/kareler/BOOSTER_REF_ANALIZ.md`).

## Aşama 0: Denetim (≈30 dk)
Test linkiyle (`&test=1`) ve normal linkle bütün akışı 390×844'te gez. Görüntüleri `docs/art/pm74/once/` klasörüne koy.

Gezilecekler:
- açılış, menü, bölüm 1, 3, 7, 10, 14, 20, 30, 40;
- her booster;
- kazanma, sandık, kaybetme, süre doldu, Durdur, Ayarlar, Profil.

`docs/art/pm74/DENETIM.md`'ye bütün sorunları yaz (ekran · sorun · önem 1–3). Zeyd'in listesi aşağıda; senin bulacakların da en az bunlar kadar önemli. PM önerileri, kontrol et:
- güvenli alan (çentik, alt çizgi);
- dokunma alanları ≥ 44 px;
- TR/EN taşan yazı;
- animasyonların hızlanma/yavaşlama eğrileri tutarlı mı;
- ses seviyeleri dengeli mi;
- yükleme süresi;
- menü → bölüm → kazanma → sonraki bölüm kaç dokunuş (hedef: her adımda 1);
- kaybetme ve devam panelleri yeni kazanma ekranıyla aynı dilde mi;
- eski görsel ya da metin kalıntısı (çivi, Baron, turkuaz parça).

## Aşama 1: Hatalar
1. **Değnek:** test modunda değnek kullanılınca hiçbir şeye tıklanamıyor. Muhtemelen seçim modu kapanmıyor ya da stok çok olduğu için tekrar açılıyor. Düzelt:
   - her kullanımdan sonra seçim modu kapansın, oyun normal girdiye dönsün;
   - seçim modunda iptal için tek dokunuş olsun;
   - stok sayısı davranışı değiştirmesin;
   - PlayMode testi ekle.
2. Denetimde bulduğun diğer hatalar (önem 1 önce).

## Aşama 2: Yerleşim ve HUD
1. **Tahta kenarları:** tahta ekran kenarlarına yapışık, "kaba". Her yanda nefes payı bırak (yanlarda ≥ 12 px, üst ve altta HUD ile arada boşluk). Tahta ölçeği buna göre küçülsün, parça dokunma alanı ≥ 44 px kalsın.
2. **Jigo penceresi (ortadaki kemerli çerçeve):** Jigo çerçevenin dışına taşıyor, tam içinde olsun (maske ya da kırpma; kafa ve yüksük dahil). Ayrıca sürekli göz kırpıyormuş gibi görünüyor: göz kırpma seyrek olsun (3–6 sn'de bir, rastgele), titreme ya da kare atlama olmasın. Boşta hafif nefes alma yeterli.
3. **Sol üst Parça kutusu:** içindeki yapboz ikonu kalksın, yalnız kalan parça **sayısı** büyük ve ortalı dursun ("Parça" başlığı kalsın).
4. **Ana menü:** normal oyuncuda "Bölümler / Levels" bölümü ve sekmesi olmasın. Tek büyük buton: **"Oyna · Bölüm N"**. Bölüm listesi yalnız `&test=1`'de görünsün.

## Aşama 3: Booster'lar ve saat (Codex ile, kalite döngüsü)
Her biri için kalite döngüsünü uygula.
1. **Saat:** mevcut animasyon "inanılmaz kötü". Baştan tasarla: anlaşılır (süre eklendiğini tek bakışta göster), sevimli, kısa. Ör. büyük saat Süre kutusuna uçar, kadranı geri sarar, "+30 sn" kutuda sayılır. Codex'e en az 3 fikir çizdir.
2. **Makas:** şu an "geliyor, kesiyor, gidiyor", %40'ı eksik. Hedef: makas zincire **gerçekten gelsin**, ağzı açılsın, keskin bir "şak"la kapansın, zincir halkaları kopup saçılsın, kısa sarsıntı ve titreşim olsun, makas dönerek çıksın. ≤ 1,5 sn.
3. **Çekiç ve değnek:** aynı ölçüt. Çekiçte vuruş anında ekran sarsıntısı + kıvılcım + parça çatlaması. Değnekte sihir izi + parçaların yer değiştirmesinin okunur olması.
4. Saklanan alternatifler `docs/art/pm74/booster/` klasöründe, GIF'li.

## Aşama 4: Resme dalma geçişi (yeni)
Zeyd: "Oyna dedikten sonra kamera ekrandaki resme yaklaşsın, bölüm açılsın; açılınca uzaklaşarak oyun ekranına geçelim, resmin içine dalalım."
- **Menü → bölüm:** menüdeki tablo resmine yakınlaşma (0,5 sn, ease-in) → resmin içi bölümün yapboz tahtasına dönüşür → hafif uzaklaşarak oyun ekranı oturur (0,4 sn, ease-out). HUD en son kayarak gelir.
- **Kazanma → Sonraki bölüm:** aynı dalış. Tamamlanan resim küçülüp galeriye asılır, kamera yeni tabloya dalar.
- Gerekirse Codex'e çizdir: tablo çerçevesi, geçiş maskesi (yapboz kenarlı ya da çerçeve içi ışık), parıltı.
- **Kalite:** takılma yok, 60 fps. Bayrak `&gecis=yok` ile kapatılabilsin. Süre ≤ 1 sn.

## Aşama 5: Genel cila (kalan bütün süre)
`DENETIM.md`'deki maddeleri öneme göre bitir. Öncelik: Zeyd'in gözüne ilk çarpacak şeyler (menü, HUD, tahta, kazanma, booster'lar). Her bitende listeyi işaretle.
- **İlke:** sade, akıcı, karmaşık değil. Yeni özellik ekleme; olanı mükemmel yap.

## Teslim (sabah)
- p23 canlı, son hâl.
- `docs/art/pm74/RAPOR.md`: tek sayfa, önce/sonra görüntü ve GIF'lerle; her maddenin durumu; saklanan alternatifler; Zeyd'e kalan seçimler.
- `ENTEGRASYON_RAPOR.md`'ye "PM_74", `PROJE_DURUMU`'na tek satır, commit + push.
- **Son mesaj CLAUDE.md'deki kısa biçimde, en altta. "Oyna" linkleri:**
  - `https://zeydusht.github.io/reverse-solver-unity/p23/?c=p23&test=1&lang=tr`
  - `https://zeydusht.github.io/reverse-solver-unity/p23/?c=p23&reset=1&lang=tr`
