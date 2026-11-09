# PM_MESAJ_53 — PM_52'yi p21'e yayınla (+ card 1 pt düzeltmesi)

PM_52 raporu okundu (`docs/art/task28/ENTEGRASYON_RAPOR.md`). PM kararları:

## Kararlar (açık konular)
1. **Booster Z2** doğru: `_yeni/Boosters` (Z2) kalsın. Değiştirme.
2. **Zincir kopması** yalnız çekiçle: doğru, kural değişmez.
3. **Titreşim**: mevcut harita kalsın (jam Medium, bomba Heavy). Dokunma.
4. **`card` kontrolü**: bu işten değil ama yayını engelliyor. **Düzelt:** zor bölüm / tanıtım kartında başlık ile çerçeve arasındaki boşluğu 7 pt → **≥ 8 pt** yap (yalnız boşluk; yazı boyu, kart boyutu, renk aynı). Hem `art=yeni` hem `art=eski`de geçsin.

## Yap
1. Card düzeltmesi → `verify_build --local` **4/4**.
2. EditMode yeşil kalsın.
3. Kaynak değişikliklerini commit et (PM_51 + PM_52 + bu düzeltme), **yalnız p21'e** yayınla. p20, kök, pt1'e dokunma.
4. Canlı p21'de `verify_build` 4/4 ve vitrin `?lv=SHOW_ART` + `&art=eski` istisnasız açılsın.
5. Vitrinde kilit açılma, zincir kopma (çekiç) ve sürükleme yaylanmasını **kısa ekran kaydı/GIF** olarak al: `docs/art/task28/entegrasyon/` (raporda "gözle görülmedi" denenler).

## Teslim
- Zeyd'e iki link (Türkçe):
  - Vitrin: `https://zeydusht.github.io/reverse-solver-unity/p21/?lv=SHOW_ART&lang=tr` (menüde **Demo**)
  - Eski görsel karşılaştırma: aynı link + `&art=eski`
- `ENTEGRASYON_RAPOR.md`'ye kısa ek (card sonucu, yayın sürümü, kayıtlar).
- `PROJE_DURUMU.md`'de PM_52 satırını "p21'de yayında" yap. Tek satır.
- Telefonda FPS'i Zeyd ölçecek: vitrinde ekranın köşesinde FPS sayacı `&fps=1` ile görünsün (zaten yoksa ekle, varsayılan kapalı).
