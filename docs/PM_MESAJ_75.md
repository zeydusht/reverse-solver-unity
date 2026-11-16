# PM_MESAJ_75: Zeyd'in p23 testi (16 Kasım sabah): düzeltmeler

Kodu kendin yaz. **p23** üzerinde devam (p22 yedek kalır). Önce `docs/` commit. Kalite kuralı ve Codex kullanımı PM_74 ile aynı: en az 3 fikir, puanla, en iyisini iyileştir, ölçütü geçmeyeni bağlama, `xhigh`. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd'in videosu: `docs/art/pm75/zeyd_video_takilma.mp4`. PM kopyalayacak; yoksa tarifi aşağıda.

## 1. Değnek hâlâ kullanılamıyor (HATA)
Test linkinde değnek "parça açamıyor" diyor. Kural:
- Tahtada ≥ 2 parça varken değnek **her zaman** bir şey yapabilmeli: çeyrek seçimi → olmazsa "Bütün tahtayı değiştir" → o da olmazsa en az 1 parçayı serbest bırakan herhangi bir çözülebilir yeniden dizilim (arama bütçesini artır).
- Gerçekten imkânsızsa (ör. 1 parça kaldı) booster düğmesi **önceden** soluk/kilitli görünsün; basınca uyarı değil kısa açıklama çıksın.
- Neden bulamadığını raporla: Zeyd'in denediği bölüm(ler)de ölç. Değnekli PlayMode testi 9, 15, 23 ve rastgele 10 bölüm.

## 2. Takılma gerçekçi olsun: parça çarptığı yere kadar gitsin
Video: sağ üstteki dikdörtgen parçanın altı boş, yalnız sol alttaki komşunun yapboz çıkıntısı aşağı yolu kapatıyor. Parça aşağı atılınca yerinde titreyip kırmızı oluyor. Zeyd'in isteği: "Aşağı inebilir; çıkıntıya değdiği yerde takılsın. Sağa ve yukarı nasıl hareket ediyorsa aşağı da öyle hareket etsin, gerçekçi olsun."
- Takılmada parça, o yönde **ilk temas noktasına kadar** gerçekten kaysın. Mesafe geometriden hesaplanır (`TravelRule.Limit` / tırnak ve doluluk). Boşluk bir hücreden büyükse o kadar uzağa gider.
- Temas anında küçük sıkışma (squash %6), "tok" sesi, hafif titreşim. ~80 ms dursun, sonra yaylı geri dönüşle yuvasına otursun.
- **Kurallar:**
  - Temas mesafesi ~0 ise (hemen bitişik) bugünkü küçük sarsıntı kalsın.
  - Zincirli parçalarda PM_63 kuralı geçerli: zincir boyu kadar.
  - Kırmızı renk yalnız temas anında kısa bir parıltı olsun, sürekli değil.
- EditMode testi: temas mesafesi hesabı 3 elle kurulmuş durumda doğru.

## 3. Geçişler: koridor geri gelsin, dalış yavaş ve açılı olsun
Zeyd: "Bölümden bölüme geçerken koridorda soldan sağa kayıp diğer resme geçiyorduk; o gitmiş. Menüye dönünce de gitmiş."
- **Koridor geçişini geri getir** (PM_74 öncesi hâli):
  - kazanma → sonraki bölüm: koridorda bir sonraki tabloya kayış;
  - bölümden menüye dönüş.
- **Dalış** yalnız tablonun önüne gelindiğinde, bölüme girerken çalışsın:
  - menüden "Bölüm N";
  - koridor kayışının sonunda.
- **Dalışın yeni hâli.** Zeyd: "Çok hızlı, dümdüz geliyor, ışınlanmış gibi."
  1. Tablo şu an yandan, açılı görünüyor. Kamera önce yana kayarak tablonun **tam karşısına** gelsin (hafif dönüş/perspektif hissi, 0,5 sn, ease-in-out).
  2. Sonra tabloya yaklaşsın; ekran tamamen resim olsun (0,6 sn).
  3. **0,5 sn bekle**: ekran sadece resim.
  4. Çerçeve geri gelmeye başlarken (uzaklaşma, 0,6 sn) resmin üstünde yapboz kesik çizgileri ve çıkıntıları belirsin. Çizgi hızla çizilir gibi, parça parça, Codex'le efekt çizilebilir. Sonunda oyun tahtası oturur.
  5. HUD ve booster çubuğu en son kayarak gelsin.
  - Toplam ≈ 2,2–2,5 sn. Dokununca hızlansın: ikinci dokunuş geçişi bitirir.
  - GIF: `docs/art/pm75/dalis.gif`, `koridor_sonraki.gif`, `koridor_menu.gif`.

## 4. Booster'lar (Zeyd: "Hızlı olmasına gerek yok, yavaş yavaş daha güzel görünüyor.")
- **Makas:**
  1. Gelir, iki parçanın arasındaki yapboz çıkıntısının üstüne yerleşir.
  2. Ağzı **yavaşça** açılır, çıkıntının arasına girer, **görünür şekilde** keser (~0,5 sn kesiş). Şimdiki hızlı "tık tık" olmasın.
  3. Çıkıntı kopar, parça düzelir.
  4. Makas **geri dönüp kaybolmasın**, booster yuvasına geri uçup otursun.
  - Toplam ~2 sn olabilir.
- **Çekiç:**
  1. Şu an çok dönüyor ve nereye vurduğu belli değil. Çekiç parçanın üstüne gelsin, 3B görünüşle **başı (yuvarlak/vuran yüzü) aşağı** dursun.
  2. Yukarı kalksın (hazırlık), **balyoz gibi aşağı insin**, vurduğu nokta net görünsün: çatlak o noktadan yayılsın, kırılma + kıvılcım + sarsıntı + titreşim.
  3. Sonra yukarı çıkıp yerine dönsün. Dönme yok ya da çok az.
- **Değnek:** 1. madde düzelince animasyonu kontrol et, aynı yavaş ve okunur ölçüt.
- **Saat:** Zeyd "fena olmamış" dedi. Ek: **Süre kutusunun altına ince süre çubuğu** (kalan süre oranı; azaldıkça yeşil → sarı → kırmızı). Saat kullanılınca çubukta duraklama da görünsün.

## 5. Ana menü üstü (Zeyd: "Niye öyle karışık?")
- **Sorunlar:**
  - profil fotoğrafı kaymış;
  - haplar yarı saydam, arka plan içinden görünüyor;
  - ayarlar düğmesi belli olmuyor;
  - Jiggle seçme alanı karışık.
- **Yeniden tasarla**, şu anki onaylı dile göre:
  - **İsim çubuğu:** solda yuvarlak profil/Jigo resmi tam ortalı, krem gövde, mavi kenar, opak; isim Baloo. Codex'le güzel bir isim plakası çizdir.
  - **Ayarlar:** sağ üstte tek, net düğme. Yuvarlak mavi düğme + beyaz dişli, katmanlı 3B, ≥ 48 pt.
  - Opak olsun, saydamlık yok. Üst alanda en çok 3 öğe: isim plakası, altın, ayarlar.
- 3 seçenek çiz ve oyun içinde görüntüle. En iyisini bağla, 2'sini sakla.

## 6. "Bölüm N" düğmesi ve kazanma butonları
- **Ana menü düğmesi:**
  - Yazı yalnız **"Bölüm N"** olsun ("Oyna ·" kalksın).
  - Kendi kendine hareket etmesin (nefes animasyonu kalksın); yalnız basınca küçülme.
- **Kazanma ekranı:** ekran güzel bulundu, yalnız butonlar beğenilmedi. "Sonraki Bölüm" butonu ana menüdeki "Bölüm N" düğmesiyle **aynı stil** olsun. Yazı "Bölüm N+1" olabilir. "Menü" küçük ve aynı ailede. Kaybetme ve devam ekranlarındaki butonlar da aynı aileye çekilsin.

## Teslim
- p23 güncel, EditMode ve PlayMode yeşil, `verify_build` p23 4/4.
- Önce/sonra ve GIF'ler `docs/art/pm75/`, rapor `docs/art/pm75/RAPOR.md`, `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `https://zeydusht.github.io/reverse-solver-unity/p23/?c=p23&test=1&lang=tr`
  - `https://zeydusht.github.io/reverse-solver-unity/p23/?c=p23&reset=1&lang=tr`
