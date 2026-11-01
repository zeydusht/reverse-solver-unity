# Task21 — Claude Code art handoff

42 yeni yazısız RGBA runtime PNG ve 6 inceleme PNG'si. Yalnız art üretildi; C#, sahne, prefab, importer ve font dosyaları değiştirilmedi. Task20'nin bütün manifest hash'leri yeniden doğrulandı. Royal Match görüntüleri yalnız yapı/ölçü referansıdır; onlardan piksel, ikon, karakter veya logo alınmadı. Yeni ikonlar ve çerçeveler built-in image_gen ile üretildi; düz renkli çubuk, daire maskeleri ve noktalı yol ölçülü raster geometrisidir. Prompt seti: `docs/art/task21/prompts.json`; kaynaklar aynı dizinin `sources/` altındadır.

## Import

Task20 biçimindeki `manifest.json` bir dizidir: gerçek piksel width/height, `sliceLTRB` SOL/ÜST/SAĞ/ALT kaynak piksel, normalize pivot, UI scale=3 ve pixelsPerUnit=3. Unity border sırası SOL/ALT/SAĞ/ÜST'tür; dönüştürün. Single, FullRect, alpha transparency, clamp/bilinear, sıkıştırmasız veya yüksek kaliteli RGBA, maxSize 2048; orijinal ölçüleri koruyun. Task21 mevcut Resources importer yoluna dahil değildir; manifest ayarlarını bağlayan taraf uygulamalıdır. isRuntime:false PNG'ler oyuna import edilmez. UGUI kullanılırsa Canvas PPU oranı ayrıca eşleştirilir.

## Öncelik ve yerleşim

`STIL_REHBERI_RM.md` Task21 metnindeki çelişkilerden üstündür: profil 64 değil 90pt; normal sekme ikonları çubuk yüksekliğinin yaklaşık %60'ı. Referans ekran 375×812pt, @3x 1125×2436. Kesin sprite/metin kutuları `docs/art/task21/preview-layout.json` içindedir; bütün önizlemeler bu katmanlarla yeniden kuruldu.

- Çubuk üstü y726, yüksekliği86pt, altı812: cihazın alt güvenli alanı DAHİL. Farklı cihazda ekran altına bağlayın, alt alanı 9-slice ile uzatın; çubuğu safe-area içine taşımayın. İlk10px tek altın şerit, geri kalanı düz #10707F. Yuvalar şeridin altından en alta uzar; seçili #1A9AAE, ayırıcı #0B4A55 3px. Kabarcık, dekoratif çerçeve veya kabartma yok.
- Dört sekme için ağırlıklar seçili2, diğerleri1: genişlikler150/75/75/75pt; seçime göre yerleri kayar. RM'nin beş sekmeli390/195px örneğini dört sekmeye aynen uygulamayın. Dört durum `Preview/04-sekme-durumlari.png` içinde. Geçişte yuva genişliklerini ve ikon merkezlerini birlikte enterpole edin.
- Normal ikon147px=49pt, seçili184px≈61.33pt (%25.17 büyüme); selected-large yeni çizim değil aynı silüetin yüksek çözünürlüklü varyantıdır. Normal üst729, seçili üst693; seçili etiket merkezi765pt. Yalnız seçili etiket yazılır. 778–812pt ev göstergesi alanında ikon/metin yoktur. tab-badge boş kırmızı rozettir, gerekirse sayı ayrı yazılır.
- Profil dış kutu [8,44,90,90]pt; y44 durum çubuğunun hemen altıdır. Avatar, ayrı koyu isim plaketi, sayaç ve ayar tuşu ortak merkez y89. Portre48pt ve daire53pt, çerçeve altındadır; portre çerçeveye değmez. İsim plaketin içinde, kare içinde yazmaz. Sayaç ikon46pt, krem hap30pt; ikon soldan taşar, sayı sağdaki boş alana yazılır. Profil çerçevesinin merkezi şeffaftır, portrait üzerine çizilir.
- Ana yeşil CTA genişlik206.25pt (%55), kutusu [84.375,613,206.25,90]; çubuğa23pt (69px) boşluk. Yazı krem #FFF6D8, koyu yeşil #1E4D05 kontur ve aşağı gölge. Sekme başlıklarında pankart yoktur.
- Avatar seçimi: bütün katmanlar180px ortak tuval. Halka dış180, iç132px; avatar içerik sınırı103px (%78.03). avatar-ring-mask beyaz alfa dairedir. Disk→maskeli avatar-round→halka→seçiliyse ayrı check-badge sırasıyla çizilir. Tik48px, halkanın sağ altında; avatar sprite'ını yeniden180px içerik boyuna büyütmeyin. Normal turkuaz ve seçili altın halka merkezleri aynıdır.
- Jiggles500/1.000/2.000: ayrı madalyon merkezleri67/187/307pt, y615; arada70pt kısa noktalar, uzun ray yok. 1.248 örneğinde ilk ikisi normal, 2.000 soluk+kilitli. Eşik ve ödül isimleri oyun metnidir. Kilitli varyantta kilit resme dahildir, tekrar kilit eklemeyin.
- Ayarlar paneli krem ağırlıklı; her satır ayrı krem9-slice karo. Soldaki dark-turquoise disc ve merkez sembol ayrı katmanlar. Gizlilik/Bize yaz iki54pt kare tuştur, altlarında bağımsız küçük etiketler; yatay uzun tuş yok. settings-icon-btn-pressed aynı tuvalde aşağı offset + Y daralmasıdır; ikon da aynı basma dönüşümünü izlemelidir. Toggle ve dil seçiciler Task20'den gelir. Kırmızı X panelin sağ üst köşesinden taşar.

## Bağımlılıklar ve metin

Değişmeyen sahneler, oyuncu portreleri/aktörler, raf, tablo çerçevesi, yeşil CTA, ayar dişlisi, toggle, dil seçici ve X Task20'dendir; kopya dosya üretilmedi. Kullanılan adlar ve SHA-256 `docs/art/task21/dependencies.json` içindedir. Gerçek Ev tablosu `Assets/Art/Gameplay/Task15/painting-test-01.png`; Task20 hero-picture-mask ile kırpılır. Eski nav-bar/slot/bubble, milestone-rail, yatay settings tuşları ve avatar ringleri yerine Task21 parçalarını kullanın.

Önizlemede mevcut OFL Nunito Black kullanıldı, Türkçe ğşİıĞŞçÇöÖüÜ glifleri doğrulandı; font dosyasına yazılmadı. Bütün dinamik yazılar/etiketler oyunda çizilir; runtime PNG'lere gömülü değildir. Sayaç sayıları lacivert, kontur/gölgesiz; başlık ve seçili etiketler krem, lacivert kontur ve aşağı gölge. Projedeki nihai font seçimi PM27 akışında bağlanır.

## Dosyalar

| PNG | px | slice L/T/R/B | kullanım |
|---|---:|---|---|
| tab-icon-home-normal.png | 165×147 | [0, 0, 0, 0] | home sekme ikonu; normal |
| tab-icon-home-selected-large.png | 206×184 | [0, 0, 0, 0] | home sekme ikonu; selected-large |
| tab-icon-collection-normal.png | 184×147 | [0, 0, 0, 0] | collection sekme ikonu; normal |
| tab-icon-collection-selected-large.png | 230×184 | [0, 0, 0, 0] | collection sekme ikonu; selected-large |
| tab-icon-jiggles-normal.png | 238×147 | [0, 0, 0, 0] | jiggles sekme ikonu; normal |
| tab-icon-jiggles-selected-large.png | 297×184 | [0, 0, 0, 0] | jiggles sekme ikonu; selected-large |
| tab-icon-trophy-normal.png | 158×147 | [0, 0, 0, 0] | trophy sekme ikonu; normal |
| tab-icon-trophy-selected-large.png | 197×184 | [0, 0, 0, 0] | trophy sekme ikonu; selected-large |
| tabbar-bg.png | 1125×258 | [1, 10, 1, 1] | Tam ekran alt çubuk, alt güvenli alan dahil |
| tab-slot.png | 225×248 | [1, 1, 1, 1] | Normal yuva; 75pt |
| tab-slot-selected.png | 450×248 | [1, 1, 1, 1] | Seçili yuva; 150pt |
| tab-divider.png | 3×248 | [0, 1, 0, 1] | Yuva ayırıcı |
| tab-badge.png | 57×57 | [0, 0, 0, 0] | Boş kırmızı bildirim rozeti |
| profile-square-frame.png | 270×270 | [0, 0, 0, 0] | 90pt kare profil çerçevesi |
| profile-name-plate.png | 360×126 | [45, 40, 45, 40] | Ayrı koyu isim plaketi |
| counter-pill-v2.png | 420×120 | [54, 39, 54, 39] | Krem sayaç; soldaki ikon ayrı |
| avatar-ring.png | 180×180 | [0, 0, 0, 0] | Avatar seçimi avatar-ring |
| avatar-ring-selected.png | 180×180 | [0, 0, 0, 0] | Avatar seçimi avatar-ring-selected |
| check-badge.png | 48×48 | [0, 0, 0, 0] | Avatar seçimi check-badge |
| avatar-ring-mask.png | 180×180 | [0, 0, 0, 0] | 132px iç çap daire alfa maskesi |
| avatar-blue-round.png | 180×180 | [0, 0, 0, 0] | İç çapın %78 boyunda maskeli avatar |
| avatar-pink-round.png | 180×180 | [0, 0, 0, 0] | İç çapın %78 boyunda maskeli avatar |
| avatar-red-round.png | 180×180 | [0, 0, 0, 0] | İç çapın %78 boyunda maskeli avatar |
| avatar-green-round.png | 180×180 | [0, 0, 0, 0] | İç çapın %78 boyunda maskeli avatar |
| avatar-purple-round.png | 180×180 | [0, 0, 0, 0] | İç çapın %78 boyunda maskeli avatar |
| milestone-medal-frame.png | 156×156 | [0, 0, 0, 0] | Bağımsız ulaşılmış madalyon |
| milestone-medal-frame-locked.png | 156×156 | [0, 0, 0, 0] | Soluk ve kilitli bağımsız madalyon |
| milestone-medal-badge.png | 156×156 | [0, 0, 0, 0] | Bağımsız ulaşılmış madalyon |
| milestone-medal-badge-locked.png | 156×156 | [0, 0, 0, 0] | Soluk ve kilitli bağımsız madalyon |
| milestone-medal-portrait.png | 156×156 | [0, 0, 0, 0] | Bağımsız ulaşılmış madalyon |
| milestone-medal-portrait-locked.png | 156×156 | [0, 0, 0, 0] | Soluk ve kilitli bağımsız madalyon |
| milestone-dots.png | 210×18 | [0, 0, 0, 0] | Kısa noktalı bağlantı; yatay ray yok |
| settings-panel.png | 987×1500 | [45, 45, 45, 45] | Ayarlar settings-panel |
| settings-row.png | 897×195 | [38, 38, 38, 38] | Ayarlar settings-row |
| settings-icon-btn.png | 162×162 | [35, 35, 35, 35] | Ayarlar settings-icon-btn |
| settings-icon-btn-pressed.png | 162×162 | [35, 35, 35, 35] | Ayarlar settings-icon-btn; basılı |
| icon-privacy.png | 102×114 | [0, 0, 0, 0] | Ayarlar icon-privacy |
| icon-mail.png | 114×96 | [0, 0, 0, 0] | Ayarlar icon-mail |
| settings-icon-disc.png | 132×132 | [0, 0, 0, 0] | Ayarlar settings-icon-disc |
| settings-symbol-sound.png | 72×72 | [0, 0, 0, 0] | Koyu daire üstünde bağımsız sound sembolü |
| settings-symbol-music.png | 72×72 | [0, 0, 0, 0] | Koyu daire üstünde bağımsız music sembolü |
| settings-symbol-vibration.png | 72×72 | [0, 0, 0, 0] | Koyu daire üstünde bağımsız vibration sembolü |

Doğrulama: `docs/art/task21/validation.json`. Önizleme: `onizleme.png`; tam boy üç ekran ve dört sekme durumu `Preview/` içinde.
