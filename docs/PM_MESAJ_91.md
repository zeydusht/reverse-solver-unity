# PM_MESAJ_91: Dalış hızı: sonda yavaşlamasın, resimde kalış yarıya insin (ÖNİZLEME, yayın yok)

Kodu kendin yaz. Önce `docs/` commit (yalnız yerel). Bitiş mesajı CLAUDE.md biçiminde, en altta.

Zeyd (19 Kasım 20:54): "Şimdi mükemmel çalışıyor, kusursuz. Sadece hızını düzenleyeceğiz."

**Kapsam çok dar:** yalnız dalış zaman eğrisi ve resimde kalış süresi. Başka hiçbir şeye dokunma (yol, çerçeve, çizgiler, geri çekilme biçimi, koridor, HUD).

**Bu iş ÖNİZLEME:**
- `git push` yok, gh-pages yayını yok, p23'ü güncelleme yok.
- Zeyd video ile bakacak; onaylarsa sonraki mesajda yayınlanır.
- Değişiklikleri ayrı yerel dalda tut (`pm91-onizleme`).

## 1. Ölç (önce)
Şimdiki dalışta (menü → bölüm ve kazanma → sonraki bölüm) şu süreleri ölç ve raporla:
- yay/yaklaşma süresi;
- **resimde kalış süresi:** resim ekranı doldurduğu andan geri çekilmenin başladığı ana kadar;
- geri çekilme süresi.

## 2. Yaklaşma: sonda yavaşlamasın
- **Sorun:** Başta güzel gidiyor ama tabloya yaklaşırken belirgin yavaşlıyor. "Bölünme" hissi veriyor.
- **İstenen:** yumuşak kalkış kalsın (çan eğrisinin başı gibi), ama **sonda belirgin yavaşlama olmasın**. Kamera tabloya hızını koruyarak "dalsın".
- **Eğri:** asimetrik.
  - Hız tepe noktası yolun ~%75–80'inde.
  - Son %10'da hız en fazla %30 düşsün (çok kısa yumuşak iniş).
  - Başlangıç kalkışı aynı.
  - Toplam süre bugünkünden biraz kısa (yavaşlayan kuyruk gittiği için), ama kalkış hissini bozma.
- Hız ve ivme sürekli olsun (sıçrama yok). Varış anında hız sıfır değil; resimde kalışa geçerken PM_79'daki gibi hafif "değme" (minik ölçek darbesi) olabilir, istersen dene.

## 3. Resimde kalış yarıya
Ölçtüğün resimde kalış süresinin **yarısı** olsun, sonra geri çekilme başlasın. Çizgiler mevcut kurala göre devam eder (kalışta başlar, çekilirken biter).

## 4. Önizleme videosu (Zeyd telefonda bakacak)
- Unity'de 60 fps kayıt, 390×844 dikey (Recorder ya da eşdeğeri). Ses yok.
- `docs/art/pm91/dalis_eski.mp4` ve `docs/art/pm91/dalis_yeni.mp4`: menü → bölüm, aynı bölüm.
- `docs/art/pm91/dalis_yan_yana.mp4`: eski | yeni, aynı anda başlasın.
- `docs/art/pm91/kazanma_yeni.mp4`: kazanma → koridor → dalış (yeni).
- Hız grafiği eski/yeni `docs/art/pm91/hiz.png`.
- Her MP4 ≤ 10 MB (H.264, `-preset veryfast`).

## Teslim
- EditMode yeşil (yerel). **Push ve yayın yok.**
- Son mesaj kısa, en altta:
  - ölçülen eski süreler → yeni süreler (tablo);
  - video dosya yolları.
