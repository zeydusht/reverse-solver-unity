# Task13 — ana menü üretim promptları

Araç: yerleşik image_gen; CLI/API kullanılmadı. Tarih:26 Ekim2026. Tüm yeni çizimler tek adaydır ve Zeyd incelemesindedir. Onaylı Level v2 dosyaları korunup birebir kopyalandı. Sprite dışa aktarımı ve kompozisyon: docs/art/build-menu-task13.ps1, render-menu-task13.ps1. Son kullanım ölçüleri manifest.json içinde.

## Ortak sanat dayanağı

Asset type: final production mobile puzzle menu sprite, one asset on genuine transparent alpha.
Image 1 is the APPROVED material/lighting master: our chunky green Level button. Match its premium softly painted 3D toy rendering, layered warm gold bevel, broad soft highlights, dark colored contact edges, tactile depth and warm light. Keep similar soft rounded proportions and rendering richness. Image 2 is a MENU UI STYLE/READABILITY reference: emulate the polish and scale of the menu widgets, but no copied characters, crowns, shields or branded symbols. Use original game-specific motifs.
Gold ramp #FBBA0E / #F38A03 / #CE5D01 with restrained #FFF1C2 shine. Teal #1C7F8C / #0F4F5A, cream #F8EBD0; dark edge #0B2A36, never hard black ink. Strong readable silhouettes at phone size. No flat vector look; no ornamental filigree, no etched detail or busy texture. No letters, digits, watermark or baked checkerboard. One isolated complete item, frontal orthographic view, transparent padding on all sides.

Ana girdiler: UI/PlayButton/menu-play-button-v2.png (onaylı materyal/ışık); References/WhatsApp Image2026-10-23 at18.34.52.jpeg (menü düzeni/okunurluk); portre ve Jiggles ikonunda Characters/Jiggles-reference-v02.png (kimlik). Sonraki çizimlerde Task13 settings-button/counter-pill yeni takım referansı olarak da verildi.

## profile-frame

Proje kaynağı: Assets/Art/Menu/Task13/Source/profile-frame.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Draw only a chunky rounded-SQUARE avatar picture frame, not a circle. Reference menu top-left avatar frame quality. Outer rim thick warm gold, slightly pillowy square corners (~18% side), inner deep teal bevel; central square opening is FULLY TRANSPARENT so a separate portrait can be inserted later. No portrait inside. At final 216x216 sprite size, frame occupies about204x204 with inner square opening about154x154. Small soft shadow underneath. Gold not silver.

## profile-avatar

Proje kaynağı: Assets/Art/Menu/Task13/Source/profile-avatar.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Image 3 is the approved Jiggles character identity reference and MUST determine the character. Draw one friendly turquoise Jiggle face/upper body portrait, front view, head fills the frame with centered puzzle tab visible at top, brown eyes and eyebrows, wide happy smile exactly this family. No hair, glasses, clothes or accessories. No hands crossing puzzle connectors. Keep canonical square puzzle body with distinct centered tabs; not a capsule. No frame or background. This is an avatar bust, not a whole-body standing figure. Intended inside154px square opening of216px avatar frame.

## counter-pill

Proje kaynağı: Assets/Art/Menu/Task13/Source/counter-pill.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Draw a blank compact horizontal resource counter capsule, width:height exactly 3.4:1. Warm cream softly convex face, thick rounded golden-orange bevel along the outside, restrained dark-teal contact shadow, corners about30% of height, tiny upper highlight. Very clean broad blank face for dark runtime numbers. No icon well, no asymmetric slot, no tokens and no numbers. Final intended360x108 px, 9-slice with rounded ends. Similar to reference's cream coin/life/star counter backplates, adapted to our gold.

## settings-button

Proje kaynağı: Assets/Art/Menu/Task13/Source/settings-button.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Draw one complete round SETTINGS button: thick layered gold rim, inset rich deep teal softly convex center, large simple chunky CREAM six-to-eight-tooth gear with a deep teal circular center hole. Gear fills about65% interior, softly beveled toy object, not thin line icon. No filigree or ornament. Match the reference menu's readable circular gear button, adapted to our gold+teal+cream materials. Final168x168.

## profile-box

Proje kaynağı: Assets/Art/Menu/Task13/Source/profile-box.png

Matched counter-pill source formatted as480x156 blank profile strip; separate avatar overlays left.

## token-coin

Proje kaynağı: Assets/Art/Menu/Task13/Source/token-coin.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One chunky round GOLD COIN resource token, frontal very slight three-quarter depth, a simple raised original JIGSAW PIECE emblem centered on face (no crown, no face, no letter). Thick rounded milled-free rim, two broad soft gold facets, sculpted but only large details. Rich warm gold ramp from approved button. A large readable coin filling a120x120 square sprite. No button surrounding the coin.

## token-heart

Proje kaynağı: Assets/Art/Menu/Task13/Source/token-heart.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One plump glossy CORAL HEART resource icon, smooth rounded toy heart, saturated coral #E8574A with warm deeper coral underside and restrained cream soft specular, strong large silhouette, no face, no infinity sign, no numbers, no frame. Match reference's heart icon size/rendering but our own simple heart shape. Final120x120.

## token-star

Proje kaynağı: Assets/Art/Menu/Task13/Source/token-star.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One chunky GOLD FIVE-POINT STAR resource token, plump gently beveled points and subtle wide sculpted facets, golden yellow on upper-left to warm golden-orange underneath. Cream only small soft highlights. No badge, face, letters, numbers or surrounding frame. Soft 3D toy rendering, not a thin flat star. Final120x120.

## token-jiggle

Proje kaynağı: Assets/Art/Menu/Task13/Source/token-jiggle.png

Approved new avatar source reused at120x120 for rescued Jiggle token.

## nav-home

Proje kaynağı: Assets/Art/Menu/Task13/Source/nav-home.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One isolated HOME navigation icon, original collector-castle entrance: a broad squat CREAM stone arch, thick warm GOLD door frame/cornice, large TEAL double doors, one small gold step. A compact welcoming palace gateway silhouette, soft chunky toy-like relief, 3D but nearly frontal. Not Royal Match's three-tower purple castle, no roof tower, no crown. Use only cream+teal+gold. Keep large simple architecture with no fine brick lines, no letters or frame. Final180x180.

## nav-collection

Proje kaynağı: Assets/Art/Menu/Task13/Source/nav-collection.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One isolated PAINTING COLLECTION navigation icon: two slightly overlapping small chunky GOLD picture frames, front frame shows a very simple teal-and-cream landscape silhouette (single hill and small cream sun), back frame peeks at upper-left. Thick smooth rounded edges, no ornate curl carving. Toy 3D raised icon, near frontal with slight tilt. No crown, text or letters, no enclosing button. Only gold+teal+cream. Final180x180.

## nav-jiggles

Proje kaynağı: Assets/Art/Menu/Task13/Source/nav-jiggles.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Image 3 is approved Jiggles character identity. One isolated RESCUED FRIENDS navigation icon showing a compact group of THREE little friendly TURQUOISE Jiggle puzzle faces: large central front smiling face, two smaller matching teal faces peeking from left/right behind. Keep approved centered top puzzle tabs, brown eyes, smiles and gently square bodies. No whole bodies or legs, no hair/clothes/glasses, no yellow capsules. Group is about as wide as tall, clear at60px. Teal identity with restrained cream facial areas; no new clothing or crown. No enclosing button or backdrop. Final180x180.

## nav-trophy

Proje kaynağı: Assets/Art/Menu/Task13/Source/nav-trophy.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One isolated ACHIEVEMENT trophy navigation icon, original broad squat GOLD cup with two chunky curled handles, short thick stem and small dark TEAL pedestal. One broad sculpted front facet, NO crown, NO emblem, NO engraving, NO stars or letters on cup. Very rounded tactile toy appearance and soft highlights, readable at60px. Near frontal, gently warm lit, no enclosing button or backdrop. Only gold+teal+cream shine. Final180x180.

## progress-track

Proje kaynağı: Assets/Art/Menu/Task13/Source/progress-track.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One EMPTY menu progress bar trough with a richly rounded thick warm GOLD rim and deep DARK TEAL inset empty channel. Very long horizontal shape, actual visible width:height about8.5:1 (1020x120 target), slightly squared rounded endcaps. Blank continuous channel all the way across; no markers, icons, numbers, labels, handles or existing fill. Gold rim smoothly lit like our Level button. The channel is opaque dark teal (not transparent); outside is transparent. Minimal small soft shadow. Keep the whole item visible with transparent margins.

## progress-fill

Proje kaynağı: Assets/Art/Menu/Task13/Source/progress-fill.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One standalone vivid TEAL glossy progress FILL strip, long horizontal capsule width:height about13:1, target996x72, no surrounding frame, no dark trough, no icons/text/markers. This is the fill insert for our menu progress bar and will be clipped by the game. Rounded ends, bright teal center (#1C7F8C with a lighter teal upper diffuse reflection) and deep teal lower edge #0F4F5A, thin cream reflective highlight at upper edge. Padded transparent surroundings. Match our gold/teal toy material rendering.

## bottom-nav

Proje kaynağı: Assets/Art/Menu/Task13/Source/bottom-nav.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One wide main-menu BOTTOM NAVIGATION RAIL backplate, width:height4.75:1, target1170x246. A broad chunky dark TEAL horizontal base with a brighter teal upper bevel and a continuous warm GOLD raised top border. Full-width rail rather than a floating pill; left/right edges nearly straight, lower corners only gently rounded, flat lower edge. Reference is the broad blue bottom bar in Image2, adapt it to our teal+gold palette and our approved toy material. Keep ALL internal area blank: no icons, text, selected tab, dividers or labels. Small soft underside shadow. One isolated complete rail on transparent canvas.

## nav-active

Proje kaynağı: Assets/Art/Menu/Task13/Source/nav-active.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One BLANK raised SELECTED TAB plate for bottom navigation, rounded rectangle width:height1.14:1, target252x222. Plump TEAL inset toy face with a slim but chunky warm GOLD edge, quiet shadow underneath. Teal center brighter than our dark bottom rail, soft upper reflection; keep broad center fully blank for an icon and label to be composited later. Corners softly squared, not a circle or shield. No text, letters, icons, engravings, ornament or symbols. Original simple navigation tile.

## plus-button

Proje kaynağı: Assets/Art/Menu/Task13/Source/plus-button.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Draw one small round ADD button: plump vivid TEAL circular disc edged with warm GOLD, raised simple CREAM plus sign with thick softly rounded arms and a tiny soft shadow. Toy-like materials consistent with the approved Level button; not a flat glyph. No number, text, surrounding counter, rim ornament or extra symbol. Final84x84.

## notification-badge

Proje kaynağı: Assets/Art/Menu/Task13/Source/notification-badge.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: Draw one small round NOTIFICATION NUMBER BADGE backplate: plump coral #E8574A disc, fine warm cream perimeter, broad simple front face left EMPTY for a runtime number, tiny soft upper-left reflection and underside shadow. No golden rim, no digit, no exclamation mark, no lettering or icon. A clean smooth toy-like circular badge, final72x72.

## levels-button

Proje kaynağı: Assets/Art/Menu/Task13/Source/levels-button.png

(Yukarıdaki ortak sanat tanımı uygulanır.)
REQUEST: One complete rounded-SQUARE gold-rimmed TEAL shortcut button, same frame material as our approved Level button and same thickness as settings button. Center icon is a simple original2x2 grid of four chunky cream puzzle tiles, each with one centered small tab, large gaps, very readable. No numbers or letters, no crown, no pattern. Four tiles are sculpted in cream+gold highlights. Final192x192, soft shallow lower shadow.

## profile-box-pressed

Proje kaynağı: Assets/Art/Menu/Task13/Source/profile-box-pressed.png

Use case: precise-object-edit. Make ONLY the pressed state of this exact blank gold-rimmed CREAM profile strip. Keep width/height, shape, border widths, corners, golden rim, blank cream face and dark teal contact keyline exactly. Cream face sinks subtly3px at480x156 final dimensions, cream shading becomes only8% deeper, small lower shadow shortens. Gold rim color stays the same approved gold, not brown. No portrait or letters, no number or icon, no new details. True transparent surroundings.

## settings-button-pressed

Proje kaynağı: Assets/Art/Menu/Task13/Source/settings-button-pressed.png

Use case: precise-object-edit. Edit this exact settings button into its PRESSED state only: same outer size, same gold rim, exact same gear and circular geometry, unchanged colors/material family; cream gear and teal face sink subtly about3px at final168px size, shading8% deeper, lower shadow shorter. Keep warm gold rim intact. No new marks or ornaments. Truly transparent outside. Preserve the normal button's design exactly; this is an interaction state, not a variant.

## plus-button-pressed

Proje kaynağı: Assets/Art/Menu/Task13/Source/plus-button-pressed.png

Use case: precise-object-edit. Make ONLY the pressed state of this exact round gold/teal/cream PLUS button. Preserve exact size, circular silhouette, gold rim, teal center and thick cream plus shape; plus and face sink2px at84px final size with shading only8% deeper, shorter lower shadow. Don't redesign, add marks or change the color family. Outside stays genuinely transparent.

## profile-frame-pressed

Proje kaynağı: Assets/Art/Menu/Task13/Source/profile-frame-pressed.png

Use case: precise-object-edit. Edit this exact square avatar FRAME into its PRESSED state only: same square silhouette, exact same chunky gold and teal rims, corners, border widths and transparent central hole; soften/shorten the lower shadow and depress the teal bevel by3px at final216px size, shading about8% deeper. No character, no opaque center, no new symbols. Preserve the normal frame identity. Truly transparent outside AND inside.

