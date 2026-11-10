# Level button v2 — referans restorasyonu

Araç: yerleşik image_gen. Kaynak: Assets/Art/Reference/level-button-reference.png.
Seçim: ikinci normal restorasyon + ona bağlı basılı durum. Son PNG dışa aktarımında görünen şekil 660×300 px, tuval 684×336 px olarak ayarlandı. Önizlemede referansın özgün 247×155 oranı korundu. Önizleme yazı dolgusu referanstan ölçülen #F9F1BA.

## İlk restorasyon

Use case: precise-object-edit / background-extraction.
Image 1 is the EDIT TARGET. Extract and faithfully restore ONLY the gold-rimmed GREEN Level button that occupies the lower center of this screenshot. This is a precise restoration of that particular button, not a redesign or a general style reference.
Remove the dog, burgundy bone-pattern panel, x2 tag, all surrounding scenery, sparkles, chain, and ALL lettering. Restore the small obscured right edge by matching the existing left edge. Deliver one standalone empty button on a genuinely transparent background, with its small soft shadow.
CRITICAL INVARIANTS: Preserve the source button's actual thick, chunky silhouette and exact width-to-height proportions (roughly 2.2:1, emphatically NOT the skinny 3:1 button). Preserve its rather squared rounded corners, thick layered gold/orange rim with dark brown fine outline, deep inset dark-green edge, rounded inner glossy green face, warm soft painted 3D lighting and small beveled highlights. The GREEN body should look exactly like the reference: saturated yellow-green upper surface, deeper vivid green lower surface, subtle soft horizontal gloss division, soft upper-edge highlight. There is NO white line running horizontally across the middle. Preserve the reference's green tones, not mint/lime pastel or teal.
Gold highlights #FBBA0E, midtone #F38A03, shadow #CE5D01; sparing #FFF1C2 shine. Green interior matched to reference.
No text, no digits, no labels, no icons, no ornaments, no dog, no bone board, no x2. One button, frontal view, centered, ample transparent margin. Render at high resolution with clean anti-aliased alpha edges, preserving shape proportions rather than stretching.

## Oran düzeltmesi — seçilen normal kaynak

Use case: precise-object-edit.
Image 1: edit target, the restored empty green Level button. Image 2: original reference, authoritative for proportions and color.
Change ONLY the SHAPE PROPORTIONS of Image 1 to match the substantially taller, thicker original button in Image 2. The previous result is still too wide and skinny. Visible gold-rim silhouette MUST have width:height exactly 2.2:1, e.g. 660 pixels wide by 300 pixels tall. Render on a square transparent canvas with generous transparent padding; don't use a panoramic canvas. The silhouette's height is 45.45% of its width.
Keep Image 1's gold rim, green lighting, inset inner-face, highlights, shadow and paint quality. Match the original's squared rounded corners: corner radius is about 15% of the visible total button height, not pill-shaped.
Match original vivid yellow-green/medium green upper part and dark saturated green lower area, with a softly edged upper glossy green inset rectangle and dark-green perimeter. Preserve gold highlight #FBBA0E / middle #F38A03 / dark #CE5D01. No white horizontal line across center.
No text, numerals, labels, characters, background, badges, or decorations. Genuine transparent PNG. One empty button.

## Basılı durum

Use case: precise-object-edit.
Edit target: this empty gold/green Level button.
Create ONLY its pressed interaction state. Keep the silhouette, width/height proportions, corner shapes, every outline, every material, every highlight placement, and transparent surroundings identical. Depress the green inner face slightly, about 4 px downward at a final visible size of 660x300 px, and darken its green shading by only 8%. Keep the gold frame unchanged. The pressed state must still be vivid green and golden, not desaturated or muddy. No text, digits, badge, background or new elements. One button on genuine transparency.

