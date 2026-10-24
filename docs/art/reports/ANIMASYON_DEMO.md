# Koridor ve koşu animatiği · v01

Tarih: 24 Ekim 2026 · Üretim aracı: yerleşik image_gen, kompozisyon/tempo: HTML canvas tween ve sprite kareleri. Önizleme: `Assets/Art/AnimationDemo/index.html`; poster: `Assets/Art/AnimationDemo/poster-v01.png`.

## Kısa hareket

2,8 saniyelik döngüde seçilen `Koridor v04` görseli hafif küçülerek kameranın geri alındığı hissini verir. İki ayrı turkuaz Jiggle koşu pozu sırayla oynar ve öne doğru büyür. İki ayrı Baron koşu pozu küçük ölçekte arkadan takip eder. Üstteki örnek seviye rozeti “BÖLÜM 12 KURTARILDI”dan “BÖLÜM 13”e geçer. Bu bir sunum animatiğidir; `index.html` yerel tarayıcıda açılır, “Yeniden oynat” ile yeniden başlatılır. Tarayıcı destekliyorsa “WebM indir” düğmesi 2,8 sn kaydı indirir. Poster tek karedir; çalışma videosu HTML canvas'ta oynar.

## Varlıklar

- Kaynak poz sayfaları: `sources/jiggle-run-sheet.png`, `sources/baron-run-sheet.png`.
- Arka plandan ayrılmış şeffaf poz sayfaları: `sources/jiggle-run-alpha-sheet-v01.png`, `sources/baron-run-alpha-sheet-v01.png`.
- Kullanıma ayrılmış dört poz: `sprites/jiggle-run-01.png`, `jiggle-run-02.png`, `baron-run-01.png`, `baron-run-02.png`. Her dosyanın gerçek boyutu dosya adından değil PNG başlığından okunur; sırasıyla 560×741, 562×741, 548×795, 501×797.
- Yeniden poster çıkarmak için `build-demo.ps1` çalıştırılır; karakterler ayrı sprite PNG'lerinden kompoze edilir.

## Kesim promptları

Jiggles: “Edit the attached two-pose character sheet. Extract ONLY the two complete turquoise puzzle-piece creature characters and remove the entire background, glow, and floor. Preserve their exact shapes, face, colors, material, and limbs. Arrange the two full-body run poses side by side with generous transparent space between them, same scale and baseline, no cropping, on a truly transparent alpha background. Do not add anything or change the character design. Keep the two poses distinct as source frames for a tiny animation. Output a transparent PNG.”

Baron tam promptu: “Edit the attached two-pose character sheet. Extract ONLY the two complete running Baron characters and remove the entire background, glow, and floor. Preserve exact design, face, expression, black coat, gold trim and burgundy lining. Arrange the two full-body run poses side by side with generous transparent space between them, same scale and baseline, no cropping, on a truly transparent alpha background. Do not add anything or change the character design. Keep the two poses distinct as source frames for a tiny animation. Output a transparent PNG.” İki sonuçta da köşe alpha=0 ve gerçek şeffaflık görüldü.

## Sınırlar

Bu önizleme tek parça koridor PNG'sini kaydırıp ölçekler; ana tablonun kendi düzleminde yana gidip komşu küçük çerçeveye dönüşmesini ve yeni bir tablo görselinin ana çerçevede belirmesini katman bazında yapmaz. Poster tek karedir, GIF/video değildir; animasyon HTML canvas içinde oynar. Üretimde bu geçiş için duvar/çerçeve/çerçeve içi tablo ve karakter katmanlarıyla sahne kamerası hazırlanmalıdır. Jiggles/Baron koşu görseli kullanıcı incelemesi bekler; referans kartlarının yerine geçmez.
