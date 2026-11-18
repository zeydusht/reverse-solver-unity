# PM_MESAJ_88: Dalış sonundaki büyüme/dönme, açılış yükleme ekranı (PM_86 §12 yapılmadı), alt menü anında tepki, koridorda kısa duruşlar

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (19 Kasım 01:32) iPhone'da denedi: "Geçiş daha iyi, kamera çerçeveye güzel gidiyor, resim de ona göre dönüyor, çok iyi." Kalan sorunlar aşağıda.

## 1. Dalışın sonunda resim bir an büyüyüp dönüyor (HATA)
- **Sorun:** Kamera çerçeveye vardıktan sonra ekrandaki resim anlık büyüyor, sonra dönüyor gibi oluyor.
- **Muhtemel sebep** (PM_87 raporu): "tahtanın çerçevesine dönüşüm yalnız çerçeve ekrandan çıkınca başlıyor" ve kazanma dalışında "son %30'da". Perspektifli resimden tahtanın düz resmine geçiş geç ve kısa bir aralığa sıkışmış, gözle fark edilen bir sıçrama yapıyor. Kontrol et ve raporla.
- **İstenen (Zeyd):** resim geldiği gibi kalsın, çizgiler orada çizilmeye başlasın.
  - Kamera resme vardığında ekranda ne görünüyorsa tahta **tam onun üstünde** kurulsun: aynı ölçek, aynı açı, aynı kırpım. Çizgiler o görüntünün üstünde çizilir.
  - Düz tahtaya dönüşüm gerekiyorsa sıkıştırılmasın. Yaklaşmanın tamamına yayılsın (kamera açısı tabloya dik geldikçe resim zaten düzleşiyor) ya da geri çekilmeyle birlikte yumuşakça olsun. Hiçbir karede ani ölçek ya da açı değişimi olmasın.
- **Ölçüm:**
  - varış anı ± 0,3 sn içinde ardışık karelerde resmin ölçek değişimi < %1/kare;
  - dönüş < 0,3°/kare;
  - yön değişimi (önce büyüyüp sonra küçülme gibi) yok.
- Kareler 10 fps önce/sonra `docs/art/pm88/varis_once_sonra.png`. Menü → bölüm ve kazanma → sonraki bölüm, ikisi de.

## 2. Açılış yükleme ekranı (PM_86 §12 YAPILMADI)
PM_86 raporunda bu madde yok; Zeyd hâlâ turuncu çubuklu yükleme ekranını görüyor. PM_86 §12'yi eksiksiz uygula:
- Linke basınca önce açılış videosu (hafif kopya ≤ 3 MB, `faststart`) `index.html` içinde oynasın.
- Unity arkada yüklensin. "Geç" düğmesi yalnız Unity hazır olunca çıksın.
- Geç ya da video bitince doğrudan oyuna geç, boş kare yok.
- Video sessiz başlar, "Sesi aç" düğmesi olur.
- Sonraki açılışlarda turuncu çubuk yerine oyunun stilinde yükleme ekranı (menü resmi + Jigo + ince altın çubuk).
- Ölçüm: linke basış → ilk video karesi; linke basış → Unity hazır; Geç → oyun arası boş kare (hedef 0). 4G kısıtlamasıyla da dene.

## 3. Alt menü: anında tepki, tak tak tak
Zeyd: "Tıklıyorum, geç açılıyor. Videodaki gibi olsun, tak tak tak tıklayayım, smooth olsun."

PM_87 raporunda iki sebep görünüyor:
- **Basış karesi 6–83 ms:** yeni sayfa basınca kuruluyor, eski sayfanın resmi çekiliyor.
- **0,6 sn'lik uzun yumuşak oturuş** yavaş hissettiriyor. Referansta yolun %88'i 0,3 sn'de bitiyor; göz bunu "geldi" diye algılıyor, kalan kuyruk fark edilmiyor.

**İstenen:**
- **Dört sayfa da açılışta kurulu ve canlı** dursun (görünmez değil, ekran dışında). Basınca kurulum yok, resim çekme yok. Basış → ilk hareket **aynı ya da bir sonraki kare** (≤ 16 ms).
- **Süre:** toplam 0,35 sn, ease-out quint ya da expo: yolun ~%90'ı ilk 0,18 sn'de. Basar basmaz sayfa gelmiş gibi hissettirsin.
- **Kesilebilir:** kayış sürerken başka sekmeye basılırsa beklemeden o anki konumdan yeni hedefe dönsün, takılma ve sıçrama yok. Art arda hızlı basışlar (tak tak tak) her biri anında tepki versin.
- Basışta seçim paneli de aynı karede tepki versin (0,15 sn).
- Ağır işler (resim açma vb.) kayış sırasında yapılmasın.
- **Ölçüm** (iPhone Safari'de de dene):
  - basış → ilk hareket (ms);
  - 0,3 sn içinde 4 hızlı basış testinde 20 ms'yi aşan kare sayısı (hedef 0);
  - kayış boyunca 20 ms'yi aşan kare.
- GIF referans | bizim, yarım hız.

## 4. Kazanma → sonraki bölüm: koridorda kısa duruşlar
Zeyd: "Seri geçiş okey ama hafif bir görelim, minnacık."
- **Başta:** bulunduğun tabloyu kayış başlamadan **+0,20 sn** daha göster.
- **Sonda:** yeni tablonun önünde durduktan sonra dalıştan önce **+0,20 sn** dur.
- Kayışın kendisi aynı (1,5 sn, çan eğrisi). Duruşlarda kamera tam donmasın, çok hafif nefes (%0,5 itme) olabilir.
- **Bayraklar:** `&koridorbas=` (varsayılan 0.2), `&koridorson=` (varsayılan 0.2).

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&lang=tr` (açılış videosu + sekmeler + Oyna dalışı)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan → koridor duruşları → dalış)
  - `p23/?c=p23&test=1&lang=tr&lv=3&koridorbas=0.3&koridorson=0.15` (ayar denemesi)
