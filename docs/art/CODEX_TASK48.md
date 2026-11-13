# CODEX_TASK48 — Stil denetiminden kalan çizimler (PM_65, 14 Kasım gece)

**Durum: Zeyd sabah verecek. Önce Zeyd'in "Zeyd'e sor" maddelerine cevabı alınmalı** (`docs/art/STIL_DENETIM_14KASIM.md`). Onaylı dil: old money mavi + krem, doygun altın/turuncu, hacimli; başlıklar kurdelesiz, kalın 3B yazı (krem dolgu, mavi kontur, altın kalınlık); paneller krem gövde + mavi çerçeve + iç bevel; siyah kontur yok; soluk efekt yok. Referans: T40 HUD, T44 kazanma, `devam_v2/panel`.

Ekran görüntüleri: `docs/art/stil_denetim/` (önce/sonra, 390×844).

## 1. Menü sekme başlıkları (Bölümler, Koleksiyon, Jiggles, Başarılar)
Şu an turkuaz kurdele üstünde canlı yazı. Kurdelesiz başlık için: başlığın arkasına konacak hafif krem/altın ışık plakası (isteğe bağlı) ya da hiçbir şey. Yazı canlı kalır (kod Title3D stilini kullanabilir). 390 pt genişlikte, üst güvenli alanın altında 56 pt yükseklik. `menu_baslik_isik.png` (@3x).

## 2. Ayarlar ve Profil panelleri
Şu an turkuaz çerçeve + turkuaz düğmeler (eski dil). Yeni dilde: krem gövde, mavi lake çerçeve (devam_v2/panel ile aynı), düğmeler old money mavi (ikincil) ve yeşil (birincil), anahtarlar (Ses/Müzik/Titreşim) yeşil-açık / gri-kapalı yuvarlak. Profil istatistik kutuları krem-üstü-açık-mavi. Kapat X kırmızı yuvarlak (devam_v2/close-button ile aynı).
Teslim: `T48/settings/` ve `T48/profile/` katmanları (9-slice kenar paylarını yaz).

## 3. Booster seçim başlık plakası ("Sihirli Makas!" vb.)
Şu an turkuaz plaka. Old money mavi gövde + altın ince kenar, kurdele ucu yok. `T48/booster-title-plate.png`, 260×48 pt, 9-slice.

## 4. Kazanma ekranı "Sonraki Seviye" düğmesi
Şu an düz, koyu yeşil. Diğer ekranlardaki katmanlı parlak yeşil düğmeyle aynı (devam_v2/continue-primary dili), 270×64 pt; "Menü" için old money mavi küçük düğme 120×40 pt.

## 5. Menü üst ve alt şeritleri (yalnız Zeyd "evet" derse)
Menü üst çubuğu ve alt sekme çubuğu turkuaz. Oyun HUD'undaki old money mavi şerit diliyle (üst koyu → alt RM mavisi, altın bordür) eşleşen bir sürüm.

## Teslim
`Assets/Art/_yeni/T48/`, `docs/art/task48/onizleme.png` (önce/sonra), `docs/art/task48/LISTE.md`. 3 satır özet, dur. Kurallar: kurdele/afiş yok, Jigo mutlu/normalde kaş çatmaz, soluk efekt yok, RM'yi birebir kopyalama yok.
