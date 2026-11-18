# PM_MESAJ_87: Sekme geçişi referans oyundaki gibi yana kayış, dalışta resim önce kaymasın

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Codex gerekirse: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (18 Kasım 19:48) iki ekran kaydı gönderdi. PM kısalttı ve kareleri çıkardı, hepsi `docs/art/pm87/` içinde:
- `ref_alt_menu.mp4`: başka bir oyunun alt menü ve sekme geçişleri (referans);
- `ref_sekme_kareler.png`: Teams → Home geçişi, 20 fps;
- `ref_alt_cubuk_kareler.png`: alt çubuğun seçim değişimi;
- `bizim_dalis.mp4` ve `bizim_dalis_kareler.png`: bizim menü → bölüm dalışımız, 10 fps.

## 1. Sekme geçişleri: referans oyundaki gibi
Referansta (kareleri incele) olanlar:
- **Sayfalar yan yana dizili**, sırası alt çubuktaki sırayla aynı.
- Sekmeye basınca **iki sayfa birlikte yana kayar**: eski sayfa dışarı itilir, yeni sayfa aynı hızla içeri girer. Sağdaki sekmeye basınca içerik sola akar, soldakine basınca sağa. Sayfa kendi arka planıyla birlikte kayar; ayrı bir solma ya da sonradan gelen arka plan yok.
- Süre kısa: ~0,3 sn. Hızlı başlar, yumuşak oturur (ease-out; çan eğrisine yakın ama sonu daha yumuşak). Atlayarak (1 → 3) geçişte de tek kayış, aradaki sayfa görünmez ya da kısa görünür. Referanstaki gibi.
- **Parmakla kaydırma:** sayfayı sağa/sola sürükleyince içerik parmağı izler. Yarıdan fazla ya da hızlı fiskede yeni sekmeye oturur, değilse geri yaylanır. Dikey kaydırması olan sayfalarda (Koleksiyon, Başarılar) yatay/dikey eksen kilidi olsun.
- **Alt çubuk sabit kalır**, yalnız seçim değişir:
  - seçili sekme daha uzun, üstü yuvarlak, açık renkli panel; ikonu büyük ve yukarı kalkık; altında etiketi;
  - diğerleri yalnız küçük ikon, aralarında ince ayrım çizgileri;
  - seçim değişince panel yeni sekmeye geçer, diğer ikonlar yumuşakça yer değiştirir (0,2 sn);
  - PM_86'daki Codex A görünüşü (renkler, krem çizgiler, ışık) korunur; davranış referansa uyar.
- Basışta Select titreşimi + tık sesi (PM_86 haritası).
- Ana menüye dönüş de aynı kayışla.
- **Ölçüm:** geçiş boyunca 20 ms'yi aşan kare sayısı (hedef 0); kayışta arka planı eksik ya da boş kare 0. GIF: bizim ile referans yan yana `docs/art/pm87/sekme_karsilastirma.gif`.

## 2. Dalış: resim önce kayıyor, sonra kamera gidiyor (HATA)
`bizim_dalis_kareler.png` (10 fps):
- **1. satır, 1 → 2. kare:** "Bölüm 4"e basılır basılmaz çerçevenin içindeki resim değişiyor. Deniz feneri sağa kayıyor, solda çam ağacı beliriyor. Bu sırada oda, çerçeve ve kamera hiç kıpırdamıyor.
- **2. satırın sonunda** (≈1,2 sn sonra) düğme ve HUD soluyor, kamera ancak ondan sonra yürümeye başlıyor.

Yani resim tek başına önden "kayıyor", ardından ölü bir bekleme var.

**İstenen:**
1. **Basış anında resim değişmesin.** Dalışın ilk karesi menünün son karesiyle birebir aynı olsun: resmin kırpımı, ölçeği ve konumu dahil. Muhtemel sebep: menü resmi ile dioramadaki tablo resmi farklı kırpım/UV ya da farklı doku; basınca biri ötekiyle değişiyor. Bul ve raporla.
2. **Resim yalnız kamerayla birlikte hareket etsin**, çerçeveye sabit (PM_86 kuralı). Kamera kıpırdamazken resim de kıpırdamasın.
3. **Ölü bekleme kalksın:** kamera basış anında yumuşakça kalkışa geçsin. Düğme, HUD ve alt çubuk aynı anda ilk 0,3 sn'de solsun. Toplam süre PM_86'daki gibi ya da daha kısa.
4. **Ölçüm:**
   - basıştan kamera hareketine kadar resim bölgesinde kareler arası piksel farkı ≤ 2/255;
   - kamera her karede resimle senkron (köşe farkı ±1 px);
   - basış → ilk kamera hareketi ≤ 1 kare.
5. Kareler önce/sonra `docs/art/pm87/dalis_once_sonra.png`, 10 fps.
6. Kazanma → sonraki bölüm dalışında da aynı kontrol.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&lang=tr` (sekmeler: bas ve kaydır; Oyna → dalış)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan → koridor → dalış)
