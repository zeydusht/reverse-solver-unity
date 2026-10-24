# Tutorial eli seçenekleri · P0 #4 · v01

İki görsel yön hazırlandı; kullanıcı seçimi bekleniyor. Referans: `Characters/Jiggles-reference-v02.png` yalnız materyal ve ışık için kullanıldı, el anatomisi için değil.

| Aday | Pozlar | PNG'ler | Boyut |
|---|---|---|---|
| A | Petrol-teal manşetli yumuşak krem eldiven | `Assets/Art/TutorialHand/Options/option-a-tap-288.png`, `option-a-swipe-288.png` | Her biri 288×288 RGBA |
| B | Sade ten rengi çıplak el, takısız | `Assets/Art/TutorialHand/Options/option-b-tap-288.png`, `option-b-swipe-288.png` | Her biri 288×288 RGBA |

Tap PNG'sinde aşağı bakan işaret parmağı; swipe PNG'sinde sola uzanan işaret parmağı bulunur. Kaynak iki pozlu sayfalar aynı klasörde `*-source-v01.png`; orijinaller korundu. Alfa arka planı doğrulandı, fazladan küçük parçalar temas sayfası ayrımından kırpıldı. Seçim galerisi: `Assets/Art/TutorialHand/Options/review-options-v01.html`.

## Üretim promptu (iki varyant)

Ortak metin: “Create a two-pose sprite sheet for an ORIGINAL mobile puzzle game's tutorial pointer. Use attached Jiggles reference ONLY for the polished soft animated-feature 3D material and warm gentle lighting, NOT for anatomy. [hand material instructions]. No face, no Jiggle body, no logo. Show two distinct, complete hand-and-short-wrist poses side by side on a truly transparent background, equal scale, no cropping, generous clear gap: left pose index finger pointing straight down in a clear tap gesture, other fingers softly curled; right pose hand in side view, index finger extended horizontally left in a clear swipe/drag gesture, other fingers curled, wrist trailing to the right. Large clear shapes, readable at 96pt / 288px @3x. Keep finger anatomy clean and plausible, exactly one hand per pose, five fingers total per hand, no extra digits. No arrow, no text, no touch ripple, no board, no UI, no glow halo, no shadow floor, no checkerboard, no watermark.”

A için `[hand material instructions]`: “One human cartoon hand only, wearing a simple small deep-teal cuff and smooth warm ivory glove.” B için: “One bare warm-peach cartoon hand only, no glove and no jewelry.” Araç: yerleşik image_gen. Görseller çalışma adayıdır; tutorial kodu veya asset entegrasyonu yapılmadı.
