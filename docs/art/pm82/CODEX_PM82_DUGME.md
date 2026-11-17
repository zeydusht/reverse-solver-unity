# Codex PM_82 — Kazanma ekranı alt düğmeleri (3 seçenek)

Bugünkü ekran: `docs/art/pm82/kazanma_dugme/simdiki_kazanma.jpg` (390×844 pt). Ekranın geri kalanı onaylı, ona dokunma. Sorun: "Bölüm N" (Sonraki Seviye) ve "Menü" düğmeleri küçük ve sönük; Menü booster çubuğunun üstüne biniyor.

## İstenen
- Düğmeler daha büyük ve tıklanır görünsün. Ana düğme (yeşil) ekran genişliğinin ~%70'i, en az 60 pt yükseklik; Menü (mavi) daha küçük ikincil düğme, yine en az 56 pt yükseklik.
- Onaylı stil: kalın 3B görünüm, yeşil ana / mavi ikincil, belirgin alt gölge (kalın alt kenar), hafif parlaklık. Basılı hâli: düğme alt kenarına doğru çökmüş (alt kenar incelir, yüz 6–8 px aşağı iner).
- **Yazı yok**: yüz boş; yazıyı oyun kendi 3B fontuyla basıyor.
- 3 farklı seçenek (A, B, C): ör. A = mevcut düğme ailesinin daha kalın ve canlı hâli, B = altın çerçeveli, C = yumuşak hap biçimi ve parlak üst ışık.

## Dosyalar (yalnız bunları yaz)
Her seçenek için `docs/art/pm82/kazanma_dugme/<A|B|C>/`:
- `win-next.png`, `win-next-pressed.png`: 1000×280 RGBA, şeffaf arka plan, 9-dilim uygun (köşe/kenar kalınlığı her yönde en az 70 px aynı, ortası düz uzayabilir).
- `win-menu.png`, `win-menu-pressed.png`: 800×230 RGBA, aynı kurallar (kenarlar en az 60 px).
- `onizleme.png`: 390×844, `simdiki_kazanma.jpg` üzerine bu düğmeler yerleştirilmiş hâli (ana düğme 272×76 pt merkez y≈705, Menü 196×60 pt merkez y≈790; yazılar "Bölüm 6" ve "Menü" kalın krem yazı, koyu dış çizgi), ayrıca basılı hâlleri yan yana küçük.
- `docs/art/pm82/kazanma_dugme/NOTLAR.md`: her seçenek için 2–3 satır.

Görsel üretim aracını kullanabilirsin; kenar temizliği, şeffaflık ve boyut için Python/PIL kullan. Silme yok, oyuna bağlama yok, soru yok.
