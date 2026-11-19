# Codex PM_89: Jiggles ekranının rafı ve ödül simgeleri, 3 seçenek

## Sorun
Zeyd: "Jiggles menüsünde alttaki bar yine çok kötü, resmi de kötü." Şimdiki hâli: `docs/art/pm89/jiggles/once.png`. Rafın resmi kahverengi ve basit, stile uymuyor.

## İstenen
**1. Raf resmi:** `shelf.png`, 1080×816 şeffaf PNG; Jiggle'ların durduğu vitrin dolabı / raf.
- Stil: old money mavi (lacivert / kraliyet mavisi) gövde, krem iç yüz, altın süsler ve kenarlar, sıcak iç ışık. Kont'un galerisiyle uyumlu olsun (referans: `docs/art/pm89/cerceve/referans_menu.png`).
- Sade olsun, kalabalık detay olmasın.
- İçi boş kalsın; Jiggle'ları oyun koyuyor (`Assets/_Game/Presentation/Resources/Art` altındaki `jiggle-*-standing` görselleri).
- Yerleşim: oyunun koyduğu yerler resmin ortasına göre verildi; resmin genişliği w, yüksekliği h.
  - Üst raf: 2 Jiggle, x = ∓0,17w, ayaklar y ≈ +0,12h.
  - Alt raf: 3 Jiggle, x = −0,27w / 0 / +0,27w, ayaklar y ≈ +0,56h (ortadan aşağı).
  - Raf tahtaları bu ayak hizalarında olsun; her Jiggle'ın boyu ~0,40h.
- Raf önünde hafif bir ışık havuzu olsun; Jiggle'lar okunur ve sevimli dursun.

**2. Ödül simgeleri:** ilerleme barının üstündeki ara hedefler, her biri 192×192 şeffaf.
- `reward_chest.png`: küçük sandık (500).
- `reward_gift.png`: **teşekkür hediyesi** (1.000, hikâye). Belirgin, sıcak parlayan bir hediye kutusu; içinden mektup ya da küçük kitap ucu görünebilir.
- `reward_medal.png`: büyük madalya (2.000), lacivert-altın.
- `lock_small.png`, 96×96: küçük altın-gri kilit. Simgelerin kilitli hâlini oyun griye çevirecek.

**3. Avatar halkası:** `avatar_ring_gold.png`, 220×220 şeffaf. Seçili avatar için kalın altın halka + hafif dış ışık; ortası şeffaf (çap ~150 px).

**4. Maket:** `onizleme.png`, 390×844 (1×), Python/PIL ile kur:
- üstte "Jiggles" başlığı ve "45 Jiggle kurtarıldı" yer tutucusu;
- raf ve Jiggle'lar;
- "Avatarım" ve 5 yuvarlak avatar: biri seçili, ikisi sönük + kilitli;
- altta ilerleme barı: krem yuva + altın dolgu (`Assets/_Game/Presentation/Resources/Art/yeni/pm86/basari/pm86-ach-track.png` ve `pm86-ach-fill.png` kullan);
- 500 / 1.000 / 2.000 üstünde ödül simgeleri, sayılar okunur.

Görsellere **yazı çizme** (maket hariç).

## Seçenekler
- **A:** Lacivert ahşap vitrin dolabı, kemerli üst, altın kenar çizgileri, iki krem raf.
- **B:** Duvara asılı iki altın konsol raf + arkada lacivert kadife pano, üstte küçük altın taç.
- **C:** Cam kapaklı lacivert vitrin; kapaklar açık, camda hafif yansıma, iç arka yüz krem, raflar altın kenarlı.

## Teslim
Her seçenek X ∈ {A, B, C} için `docs/art/pm89/jiggles/X/` altına yukarıdaki dosyalar. Ayrıca:
- `docs/art/pm89/jiggles/karsilastirma.png`: 3 maket yan yana.
- `docs/art/pm89/jiggles/LOG.md`: seçenek başına 1 satır.

Kontroller: boyutlar, RGBA, şeffaf arka plan.

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
