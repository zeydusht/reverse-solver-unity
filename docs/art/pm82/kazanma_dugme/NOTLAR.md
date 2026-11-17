# PM82 — Kazanma ekranı düğmeleri

## A — Canlı klasik
- Koyu yeşil / lacivert kalın alt kenar, canlı yeşil / mavi yüz ve yumuşak üst parlaklık; mevcut düğme ailesine yakın, daha dolgun görünüm.
- Basılı durumda yüz 7 px aşağı iner; alt kenar aynı tabana doğru incelir ve renk çok hafif koyulaşır.
- Önizleme: [A/onizleme.png](A/onizleme.png).

## B — Altın çerçeve
- Yeşil ve mavi boş yüzleri çevreleyen altın bevel, krem üst ışık ve turuncu alt çerçeve; daha belirgin ödül hissi.
- Basılı durumda yüz ve çerçeve 7 px aşağı iner; koyu alt taşıyıcı kenar incelir.
- Önizleme: [B/onizleme.png](B/onizleme.png).

## C — Parlak hap
- Yumuşak hap biçimi, canlı yeşil / gök mavisi yüz ve geniş, sütlü üst ışık; koyu alt kenarla kalın 3B görünüm.
- Basılı durumda yüz 7 px aşağı iner; yuvarlak uçlar korunur ve alt kenar incelir.
- Önizleme: [C/onizleme.png](C/onizleme.png).

## Ortak teknik bilgiler
- Her klasörde `win-next.png`, `win-next-pressed.png`: **1000×280 RGBA**; `win-menu.png`, `win-menu-pressed.png`: **800×230 RGBA**. Gerçek şeffaflık, temiz kenarlar, tamamen şeffaf piksellerde sıfır RGB; sprite yüzlerinde yazı veya simge yok.
- 9-dilim sınırları her yönde eşit: **next = 128 px**, **menu = 108 px** (sol / sağ / üst / alt). Normal ve basılı hâller aynı sınırları kullanır. Yatay ve dikey orta şeritler düz ve tekrarlanabilirdir; bu değerler PNG metaverisinde de bulunur.
- `onizleme.png`: **390×844**. Ana düğme **272×76**, merkez **(195,705)**; Menü **196×60**, merkez **(195,790)**. “Bölüm 6” ve “Menü” yalnız önizlemelerde, projedeki Baloo2 ExtraBold ile krem dolgu ve koyu dış çizgiyle yazıldı.
- Basılı hâller önizlemenin en altında “Basılı” etiketiyle yan yana küçük gösterilir. Eski ana düğmenin yeni sınırların dışında kalan izi, yalnız eski düğme bölgesinde komşu koridor renkleriyle temizlendi; ekranın üst bölümü aynen korundu.
- Yerleşik görsel üretim aracıyla üç temel çift üretildi; PIL / NumPy ile ayırma, kenar ve alfa temizliği, simetri, orta alan düzleştirme, kesin boyutlandırma ve basılı hâl türetme uygulandı. Kaynak JPG değişmedi; oyuna bağlama yapılmadı.
- Kontrol: 12 sprite için boyut, RGBA / alfa, dört dış kenarda şeffaflık, eşit 9-dilim orta şeritleri ve 10 MB altı dosya boyutu doğrulandı; üç önizleme görsel olarak incelendi.

<details>
<summary>Son görsel üretim istemleri (A / B / C)</summary>

### A

```text
Use case: stylized-concept. Asset type: production-ready blank mobile puzzle game UI button sprites, design option A. Reference image is for STYLE only: colorful plush 3D casual puzzle game victory screen. Do not recreate the screenshot. Generate a transparent PNG sprite sheet containing EXACTLY TWO isolated horizontal blank buttons, one above the other, generous transparent gap. Top button saturated lime-green face, bottom button royal-blue face. Both are wide rounded rectangles, approximately 4:1 aspect, perfect straight-on orthographic view, horizontal and symmetric, with modest rounded corners, bold thick dark colored outline and a substantial deep darker lower extrusion (15% of total height). More vivid thicker evolution of a classic cartoon game button. Clean sculpted bevel, subtle light upper rim, smooth satin highlight along upper quarter. Flat unobstructed center. Top and bottom each occupy about 90 percent of sheet width, all edges and entire shadow visible. Matching geometry and lighting in both. Green outer rim is dark forest green, blue outer rim is dark navy, NOT gold. Style: clean polished 3D mobile game UI, readable at small scale, no texture grain. Genuine transparent background. NO text, letters, numbers, logos, symbols, icons, sparkles, surrounding objects, panels, decorative marks, labels or watermark. No perspective, no ground plane, no huge glow or cast shadow. Preserve empty button faces for game-rendered text. This output is just two unpressed base sprites; pressed variants will be made later.
```

### B

```text
Use case: stylized-concept. Asset type: production-ready blank mobile puzzle game UI button sprites, design option B, gold rim. Generate a transparent PNG sprite sheet containing EXACTLY TWO isolated horizontal blank buttons, one above the other, generous transparent gap. Top button bright emerald and apple-green face; bottom button saturated medium royal blue face. Both are wide rounded rectangles, approximately 4:1 aspect, perfect straight-on orthographic view, horizontal symmetric silhouette. Each has a polished thick warm yellow gold bevel frame with a narrow cream top glint, a slightly orange lower rim, and a substantial darker colored lower extrusion. Gold frame should occupy about 7 percent of total button height, the lower extrusion 15 percent; crisp dark outer contour. Rich friendly 3D casual puzzle game aesthetic, not realistic metal, not ornamental. Broad clean empty colored face with very subtle upper satin gloss and softly beveled inner corners. Same geometry and lighting for both, rounded rectangle corners not pill ends. Top and bottom each occupy 90 percent sheet width, full outline visible. Genuine transparent background. NO text, letters, numbers, logos, symbols, icons, sparkles, ornaments, labels, scene or watermark. No perspective or ground plane or huge shadow. All face interiors blank for game-rendered text. Produce only two unpressed base sprites.
```

### C

```text
Use case: stylized-concept. Asset type: production-ready blank mobile puzzle game UI button sprites, design option C, soft candy pill. Generate a transparent PNG sprite sheet containing EXACTLY TWO isolated horizontal blank buttons, one above the other, generous transparent gap. Top button vivid fresh mint-lime green face; bottom button rich azure blue face. Both are wide horizontal capsules about 4:1 aspect with FULLY rounded pill ends; perfect straight-on orthographic view, horizontal symmetric silhouette, whole outline visible. Plump soft candy 3D mobile game style with a bold thick darker colored lower extrusion, a crisp dark green / navy outside edge, smooth bevel, a luminous gently curved but HORIZONTALLY EVEN upper highlight occupying upper quarter, slightly milky gloss. Keep the large center uncluttered and smooth; avoid harsh specular spots. Lower rim substantially darker, 15% of total button height, to convey an elevated clickable button. Same geometry and lighting in both sprites. Top and bottom occupy 90 percent of sheet width. Genuine transparent background. NO text, letters, numbers, logos, symbols, icons, gold frame, sparkles, decorative marks, labels, panels, surroundings or watermark. No perspective, ground plane, blurry glow or large external shadow. Keep blank face for separately game-rendered text. Produce only two unpressed base sprites.
```

</details>

