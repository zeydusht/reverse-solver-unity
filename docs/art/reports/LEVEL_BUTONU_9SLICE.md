# LEVEL X 9-slice tuşu · P0 #3 · v01

İki yazısız renk adayı, her biri 888×138 px şeffaf PNG: `Assets/Art/Menu/Buttons/level-button-teal-option-a-888x138.png` ve `level-button-orange-option-b-888x138.png`. Unity importer `.meta` dilim değerleri sol/alt/sağ/üst: 126/42/126/42 px. Dynamic `LEVEL X` metni görsele gömülü değildir. A/B önizlemesi `Assets/Art/Menu/Buttons/review-options-v01.html`.

Kod ölçü notu: 296×46 pt, @3x=888×138 px; 9-slice köşe en az 42 px. Korunan ilk adaylar `Menu/level-button-*-v01.png` ve `Menu/Exports/` altında kalır; yeni ölçülü seçenekler onları ezmez.

## Tam promptlar

İki varyant promptu aynıydı; yüz rengi talimatı değişti. Ortak prompt: “Use case: reusable mobile game UI sprite for Reverse Solver. Treat the attached teal-gold button only as a style/material reference. Create ONE blank, very wide shallow pill button, exact silhouette ratio about 6.4:1, front-facing orthographic view, on true transparent background. Must be practical for Unity 9-slice at 888x138 pixels: the left and right rounded end caps must each be compact and self-contained, and the long straight top/bottom bevels and broad central face must continue cleanly with no ornaments, texture break, lettering, or icons, so the center can stretch horizontally. Soft premium animated-feature 3D toy material, crisp warm gold outer rim, deep underside bevel and controlled highlights; [FACE] with large calm text-safe center. Symmetrical shape, clean anti-aliased edge, safe transparent margin. No glow outside the edge, no perspective angle, no scene, no shadow cast onto a floor, no checkerboard, no text, logo, symbols, or watermark. Keep object ratio 6.4:1 even if canvas itself is wider.”

Teal `[FACE]`: “rich dark teal enamel face”. Turuncu `[FACE]`: “rich burnt-orange enamel face”. ImageGen çıktılarının nesne oranı hedef 6.4:1'i tam tutturmadığı için alfa sınırından kırpılıp 888×138 hedefine kontrollü ölçeklendi; 9-slice kenarları gerilmeyi köşe kapaklarında sınırlar. Nihai dilim görünümü Unity'de doğrulanmalı. Onay bekleniyor.
