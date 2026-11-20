# PM_MESAJ_96: Ses seçimleri kalıcı; lobi ve bölüm müziği ayrı; referans oyun konseptinde yeni adaylar; kazanma ekranı müzikle vuruş vuruş senkron

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses ve kazanma ekranı zamanlaması.

## 1. Zeyd'in seçimleri: varsayılan yap
| Ses | Seçim |
|---|---|
| Müzik | c |
| Kazanma | b |
| Çıkış | c |
| Çarpma | a |
| Son parça | a |
| Tık | b |

Seçilmeyen adaylar dinleme sayfasında ve bayrakla kalır.

## 2. Referans: Zeyd'in sevdiği oyun (Block Out!)
Dosyalar `docs/art/pm96/ref/`:
- `ref_lobi.mp3`: menü müziği;
- `ref_bolum.mp3`: bölüm içi müzik;
- `ref_kazanma.mp4`: kazanma anı;
- `ref_kazanma_kareler.png`: kazanma, 3 fps.

**Yalnız referans.** Oyuna konmaz, repo dışına çıkmaz, melodi kopyalanmaz.

**PM analizi:**
- **Lobi ve bölüm müziği ayrı.**
  - Lobi müziği daha sakin ve geride, daha az enerjik; menüde gezinirken arka plan.
  - Bölüm müziği belirgin daha enerjik: ~136 BPM (yarı tempo ~68 hissi), parlak, ritmik, kıpır kıpır, motive eden ama yormayan.
- **Kazanma:** bölüm müziği kesilir. Kısa, vuruşlu bir fanfar çalar. Logo harfleri ("BLOCK OUT!") **her vuruşta bir parça** zıplayarak gelir, en güçlü vuruşta havai fişek ve konfeti patlar. Ölçülen vuruşlar (sesin başından): 0,16 · 0,42 · 0,63 · 0,98 · 1,44 · 1,74 sn civarı. Yani ~1,8 sn'de 6 vuruş, her biri ekranda bir olay.

## 3. Yeni adaylar: bu konseptte
Ticari kullanıma atıfsız izinli kaynaklardan (PM_95 kuralları, lisanslar `docs/art/pm96/LISANS.md`). Kusursuz döngü, seviye eşitleme.

| Ne | Kaç aday | Tarif |
|---|---|---|
| **Lobi müziği** | 4 | sakin, sıcak, cozy; 90–110 BPM; "old money" saray dokunuşu olabilir; arka planda kalır |
| **Bölüm müziği** | 6 | enerjik, parlak, oyuncu; 120–140 BPM; vurgulu perküsyon, pluck/marimba/synth bell; referanstaki enerji, bizim tarz; uzun dinlenince yormayan |
| **Kazanma fanfarı** | 4 | 1,5–2,5 sn, net ve sayılabilir vuruşlar (5–7), sonda parlak bitiş |

- Şimdiki seçimler de sayfada kalsın: müzik c lobi/bölüm adayı olarak, kazanma b. Zeyd yeni adaylarla karşılaştıracak.
- Dinleme sayfası `p23/onizleme/sesler.html`'e yeni bölümler: "Lobi", "Bölüm", "Kazanma". Referansı oynatma (yalnız `docs` içinde dursun).
- **Varsayılan:** Zeyd seçene kadar lobi = c (şimdiki), bölüm = en iyi bulduğun yeni aday.

## 4. Oyunda iki müzik
- Menüde lobi müziği, bölümde bölüm müziği.
- Dalışta lobi müziği ~0,6 sn'de solar, bölüm müziği geri çekilmeyle başlar (çapraz geçiş).
- Menüye dönüşte tersi.
- **Kazanma:** bölüm müziği kazanma fanfarı başlarken 0,15 sn'de kesilir (referans gibi). Fanfardan sonra kazanma ekranında lobi müziği yavaşça (1 sn) geri gelir.
- **Kayıp / Süre doldu:** bölüm müziği kısılır (%40), panel kapanınca geri gelir.
- **Bayraklar:** `&lobi=`, `&bolummuzik=`, `&kazanma=`.

## 5. Kazanma ekranı: müzikle vuruş vuruş senkron
- Seçili kazanma fanfarının vuruşlarını otomatik çıkar (onset tespiti, ör. librosa) ve `vurus_haritasi.json`'a yaz (ses başına zaman listesi). Fanfar değişince aracı yeniden çalıştırmak yetsin: animasyon kendini bu haritaya göre kursun.
- **Olayları vuruşlara oturt** (PM önerisi; vuruş sayısına göre uyarla):

| Vuruş | Olay |
|---|---|
| 1 | ışık huzmesi açılır |
| 2 | "Harika!" başlığı ilk vuruşla gelir. Harf harf mümkünse her vuruşta bir hece/harf grubu ("Ha-ri-ka!"), değilse başlık 1,3×'ten 1×'e "tak" diye oturur |
| sonraki | **Jiggle'lar her vuruşta biri** zıplayarak görünür (kurtarılan Jiggle'lar) |
| en güçlü vuruş | konfeti / havai fişek patlaması + Jigo zıplar |
| son vuruş | altın +50 sayacı ve düğmeler gelir |

- Görsel olay, vuruşun **10–20 ms önünde** başlasın (göz ve kulak birlikte algılar).
- Oyuncu dokunursa her şey hemen tamamlanır (mevcut kural), müzik kesilmez.
- Sandık varsa (her 5 bölüm) sandık fanfardan sonra gelir.
- **Ölçüm:** kayıtta her olayın vuruşa uzaklığı ≤ 1 kare (17 ms). Video `docs/art/pm96/kazanma_senkron.mp4` (sesli, 60 fps, ≤ 10 MB).

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (yeni lobi / bölüm / kazanma adayları)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (lobi → bölüm müziği geçişi, kazan → senkron ekran)
