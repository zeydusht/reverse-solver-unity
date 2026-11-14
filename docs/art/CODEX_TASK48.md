# CODEX_TASK48 — Stil denetiminden kalan çizimler (PM_65, 14 Kasım gece)

**Durum: Zeyd onayladı (14 Kasım 10:05).** Madde 1, 2, 5 (menü, Ayarlar/Profil, menü şeritleri) EVET. **Madde 3 İPTAL** (plaka zaten mavi, PM kontrol etti). Madde 4 aşağıdaki 6. madde içinde. Onaylı dil: old money mavi + krem, doygun altın/turuncu, hacimli; başlıklar kurdelesiz, kalın 3B yazı (krem dolgu, mavi kontur, altın kalınlık); paneller krem gövde + mavi çerçeve + iç bevel; siyah kontur yok; soluk efekt yok. Referans: T40 HUD, T44 kazanma, `devam_v2/panel`.

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

## 6. Kazanma ekranı: baştan (Zeyd: "tarzımızla inanılmaz alakasız")
Şu anki ekran: düz koyu lacivert kart, düz yeşil düğme, sade yazılar (`docs/art/stil_denetim/16_kazanma_tr.jpg`). Yeni dil:
- Kart: **krem gövde + mavi lake çerçeve + iç bevel** (devam_v2/panel ve Süre doldu paneliyle aynı aile); arkada yumuşak altın ışık hüzmesi ve birkaç yıldız.
- Başlık "Harika!" kurdelesiz 3B yazı (mevcut stil iyi, kalabilir), üstte; ortada büyük mutlu Jigo (`Jigo_v3`, kaş çatmaz).
- Sandık ilerlemesi: krem kartın içinde, yuvalar altın çerçeveli, dolanlarda altın yıldız; sağda sandık ikonu.
- "N Jiggle kurtarıldı!": küçük Jiggle ikonu + krem üstüne koyu mavi kalın yazı.
- Düğmeler: **Sonraki Seviye** büyük, katmanlı parlak yeşil (booster tuşlarıyla aynı katman dili: altın+turuncu halka, üst açık alt koyu yeşil gövde); **Menü** küçük, old money mavi katmanlı.
- Hepsi HUD ile aynı malzeme: oyuncu "aynı oyun" hissetsin. 390×844 maket + katmanlar `T48/win/`.

## Teslim
`Assets/Art/_yeni/T48/`, `docs/art/task48/onizleme.png` (önce/sonra), `docs/art/task48/LISTE.md`. 3 satır özet, dur. Kurallar: kurdele/afiş yok, Jigo mutlu/normalde kaş çatmaz, soluk efekt yok, RM'yi birebir kopyalama yok.
