# PM_MESAJ_89: Dalış sonunda resim uzayıp kısalıyor (en-boy uyumsuzluğu), süre barı kalksın (buzlu bar yalnız saat booster'ında), Jiggles ekranı alt kısmı ve resmi

Kodu kendin yaz. p23 üzerinde. **PM_88 bitmediyse önce onu bitir.** Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (19 Kasım 09:43) iPhone kaydı gönderdi, PM kare kare inceledi. Dosyalar `docs/art/pm89/` içinde:
- `zeyd_dalis_19kasim.mp4`: kayıt;
- `dalis_10fps.png`: bütün dalış;
- `geri_cekilme_20fps.png`: sorunlu an.

Zeyd: "Son gidişe kadar çok iyi, orada bir kısalıp uzuyor."

## 1. Dalışın sonunda resim uzayıp kısalıyor (HATA)
**PM gözlemi** (`geri_cekilme_20fps.png`):
- Çerçeve ekranı doldurduğunda resim **dar ve dikey** bir kırpım: kapı ekranın büyük kısmını kaplıyor, soldaki saksı ve sağdaki sarı duvar görünmüyor.
- Geri çekilirken resmin iki yanından yeni içerik açılıyor (saksı, sarı duvar). Tahta, çerçevedeki resimden daha geniş en-boy oranında (4×5 tahta, çerçeve açıklığı ~0,625 dikey).
- Kapının en-boy oranı da çerçevede ve tahtada farklı görünüyor. Yani ya kırpım değişiyor ya da resim çerçevede yatayda sıkıştırılmış (eşit olmayan ölçek). İkisi birden de olabilir.
- Gözün gördüğü: resim "kısalıp uzuyor".

**Önce ölç ve raporla:**
- çerçeve açıklığının en-boy oranı;
- bu bölümün tahta en-boy oranı;
- resmin çerçevede ve tahtada X ve Y ölçeği (kapının en/boy oranıyla doğrula);
- görünen kırpım dikdörtgenleri.

**Kural:**
- Dalış boyunca resim **yalnız eşit ölçek + kaydırma** ile değişsin; X ve Y ölçeği her karede aynı.
- Çerçevede görünen bölge **tahtanın resmiyle birebir aynı bölge** olsun; geri çekilirken yeni içerik açılmasın.

**Çözüm yolu (PM önerisi, en temizini seç ve gerekçesini yaz):**
- **A, tercih:** Çerçeve açıklığı her bölümde **tahtanın en-boy oranına** uysun. Çerçeve 9-dilim (köşeler sabit, kenarlar uzar) ile ölçeklensin. Menüde ve koridorda da bu bölümün tablosu kendi oranında görünsün (kare tahta → kareye yakın çerçeve). Böylece çerçevedeki resim = tahtanın resmi.
- **B:** Çerçeve sabit kalır; tahtanın resmi çerçevenin gösterdiği kırpım olur. Tahta oranı değişmediği için genelde uymaz; büyük ihtimalle olmaz.

**Ölçüm:**
- her karede resmin X/Y ölçek oranı 1,00 ± 0,01;
- görünen kırpım dikdörtgeni varıştan oyuna kadar sabit;
- önce/sonra 20 fps kareler `docs/art/pm89/geri_cekilme_once_sonra.png`.

Kare olmayan 3 tahtada da (ör. 4×5, 5×6) dene. Menü → bölüm ve kazanma → sonraki bölüm, ikisi de.

## 2. Süre panelinin altındaki yeşil bar kalksın
- Süre panelinin altındaki yeşil bar **tamamen** kalksın.
- **Saat booster'ı** kullanılınca (Jigo saati dondurduğunda) panelin altında **buzlu bar** çıksın:
  - buz mavisi, hafif kırağı dokusu, üstünde küçük buz kristalleri parıltısı;
  - donma süresi boyunca (10 sn) soldan sağa boşalır;
  - bitince çatlayıp kırılarak kaybolur (küçük buz parçacıkları + çatırtı sesi + Light titreşim).
  - Süre panelinin kendisi donarken hafif buz kenarı alır, çözülünce gider.
- Codex buzlu bar ve kırılma için 3 seçenek çizer, puanla, en iyisini bağla. GIF `docs/art/pm89/buzlu_bar/`.

## 3. Jiggles ekranı: alt kısım ve resim kötü
Zeyd: "Jiggles menüsünde alttaki bar yine çok kötü, resmi de kötü."
- **Alt kısım** ("Avatarım" sırası + "45 / 2.000" ilerleme barı + 500 / 1.000 işaretleri):
  - Bar, Başarılar ekranındaki yeni bar diliyle aynı: krem yuva, altın dolgu, kalın 3B sayı.
  - Ara hedefler küçük ödül simgesi (sandık ya da hediye) olsun, sayılar okunur.
  - 1.000'deki teşekkür hediyesi (hikâye) belirgin olsun.
  - Avatar sırası: yuvarlaklar eşit aralıklı, seçili avatar altın çerçeve + hafif ışık; kilitli olanlar sönük + küçük kilit.
- **Resim** (Jiggle'ların durduğu raf / dolap):
  - Codex yeniden çizsin: bizim stil (old money mavi, krem, altın, sıcak ışık), Kont'un galerisiyle uyumlu.
  - Jiggle'lar rafta okunur ve sevimli dursun.
  - Sade; kalabalık detay yok.
- Codex 3 seçenek, puanla, en iyi 2'yi `docs/art/pm89/jiggles/` klasörüne koy, en iyisini bağla. Önce/sonra ekran görüntüsü 390×844.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&lang=tr` (Oyna → dalış; Jiggles sekmesi)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan → dalış)
  - `p23/?c=p23&test=1&lang=tr&lv=18&booster=saat` (buzlu bar)
