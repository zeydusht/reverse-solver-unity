# PM_MESAJ_104: Ses seçimleri oyunda gerçekten doğru mu (makas); kaydırmaya swoosh; Durdur ve alt menü için tok ses

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses.

Zeyd (24 Kasım 09:01): "Makasa baktım, istediğim ses değildi gibi. Kaydırma sesi kötü, swoosh gibi olsun. Durdur'a veya alt menüye basınca iğrenç bir ses geliyor, tok bir ses lazım."

## 1. Doğrulama: dinleme sayfası = oyun
`ses.json` PM tarafından kontrol edildi: Zeyd'in bütün seçimleri doğru yazılmış (makas c dahil). Sorun büyük ihtimalle **eşlemede**.

**Kontrol et:**
- Her ses için dinleme sayfasındaki harfin çaldığı dosya ile oyunda o olayda çalan dosya **aynı mı?** Dosya özeti (hash) karşılaştırması.
- Makas animasyonunda başka bir ses daha çalıyor mu (eski ses, kesme anından önceki "uçuş" sesi vb.)?
- Booster'ların hepsinde (makas, değnek, çekiç, saat) aynı kontrol.

**Tablo:** `docs/art/pm104/ses_eslesme.md`:

| Olay | ses.json harfi | sayfadaki dosya | oyunda çalan dosya(lar) | aynı mı |
|---|---|---|---|---|

Uyuşmayan varsa düzelt. Bir olayda iki ses üst üste çalıyorsa yalnız seçileni bırak.

## 2. Kaydırma (parça çıkışı): swoosh
- **Sorun:** Şimdiki (f) kötü.
- **İstenen:** havalı, kısa, yumuşak **swoosh / whoosh**. Hızla geçen bir şeyin hava sesi. Sürtünme, tahta ya da boing yok.
- **En az 6 aday** (CC0 kayıt):
  - kısa (0,12–0,25 sn);
  - yumuşak başlangıç;
  - hafif yükselip sönen;
  - yüksek frekans ağırlıklı (takılma "bonk"undan ayrışsın).
- Hızlı fiskede biraz daha parlak / kısa, yavaş itişte biraz daha yumuşak olabilir (hızla perde/filtre).
- Dinleme sayfasında "Kaydırma (swoosh)" + "10 art arda" düğmesi.

## 3. Durdur ve alt menü sekmeleri: tok ses
- **Sorun:** Durdur düğmesine ve alt menü sekmelerine basınca çıkan ses rahatsız edici.
- **Hangi ses olduğunu bul:** tık b mi, panel "vuş"u mu, sekme swish'i mi, yoksa ikisi üst üste mi? Raporla.
- **İstenen:**
  - kısa, **tok**, tatmin edici bir düğme sesi: yumuşak ahşap ya da plastik "tok/tık", hafif gövdeli, tiz ve cırtlak değil;
  - Durdur, Devam, Menü, alt sekmeler ve Ayarlar düğmeleri bu sesi kullansın;
  - sekme swish'i varsa çok kısık kalsın ya da kaldırılsın; tok ses öne çıksın.
- En az 5 aday, dinleme sayfasında "Düğme (tok)".

## 4. Seçim kolaylığı
Dinleme sayfasının en üstünde "Bugünkü seçim" bölümü: her olay için şu an oyunda çalan ses, oynat düğmesiyle. Zeyd neyin çaldığını tek bakışta duysun.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html`
  - `p23/?c=p23&test=1&lang=tr&lv=23` (makas ve diğer booster'lar)
