# Jigo HUD portresi ve düğme kalkanı

Portre, kemerin içini maskeleyen `JigoPortrait` shader’ıyla sabit durur. Göz kırpması 0,40 saniye sürer ve iki göz kısa süre tam kapalı kalır. Yüz ifadesi değişebilir. Sağ el ve fırça saat hareketi boyunca aynı pozda kalır.

Unity’nin `Assets/_Game/Presentation/Resources/Art/yeni/hud/` klasöründeki katmanlar

- `jigo-portrait-open.png` · Düğme kalkanı ve tutuşu ayrılmış, gözleri açık gövde.
- `jigo-portrait-closed.png` · Aynı portrede gözler tam kapalı. Shader bu karenin yalnız göz çevresini kullanır.
- `jigo-shield-grip.png` · T42’nin mevcut `free-grip` katmanı. Jigo’nun sol elinin parmakları.

Dört delikli düğme için mevcut `boosters21/Clock/Layers/button-shield.png` kullanılır. Kalkan 0,62 saniyede Süre paneline ulaşır. Temasla buz ve panelin altında 10 saniyelik çubuk görünür.

## ImageGen istemleri

İki portre yerleşik ImageGen ile, şeffaf arka plan seçilerek düzenlendi. İlk düzenlemenin kaynağı `Assets/Art/_yeni/T46/jigo/normal_loop/01.png`. İkinci düzenlemenin kaynağı gözleri açık yeni portre.

### Açık gözler

> Precise object edit of this exact Jigo game HUD bust. The only change is REMOVE the ivory four-hole button shield and remove the exposed turquoise fingers that curl around the right edge of that shield (at the lower right of the image). Reconstruct the underlying turquoise jigsaw torso and left upper arm naturally behind the removed button. Keep the anatomical left upper arm relaxed at his side and its hand below the bottom crop, leaving clear space for a separately animated left hand and shield. Match the exact original frontal face and smile, eye positions and open eyelids, silver thimble helmet, right arm and blue-tipped brush on image left, gold ferrule, turquoise jigsaw silhouette, toy-textured 3D light and materials, and square orthographic crop. His right hand and brush stay exactly in the original position. Keep the bust crop and margins as close as possible to the reference; do not zoom out to full body. True transparent background, no frame, no shadows outside the silhouette, no text.

### Kapalı gözler

> Precise in-place eyelid edit, one PNG. In this exact square front-facing Jigo portrait, fully close both eyes in a natural relaxed blink: the turquoise eyelids meet in two slender dark gently curved lines, with zero white, iris or pupil visible. Put the closed lines within the original open-eye outlines and retain realistic turquoise eyelid lighting. Preserve the exact camera, framing, anatomy, contour, head, position, brows, mouth, cheeks, body, right hand, brush and hat. Match the original image outside the two eyes as closely as possible. Keep the transparent background and the missing shield. No other pose change, no zoom, no text.
