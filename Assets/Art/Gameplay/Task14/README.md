# Task14 final @3x export

Zeyd revizyon 2 tam ekran taslağını onayladı. 61 şeffaf RGBA üretim PNG'si bu klasörün kökünde hazırdır. Hepsi @3x; sayı alanları ve kart/tuş metinleri boş bırakıldı.

- HUD: level rozeti, ikonlu hamle/süre hapları, acil süre ve durdur; normal/basılı.
- Booster: boş zemin, dört bağımsız ikon, boş adet rozeti, kilit; her booster için normal/basılı/kilitli/0-adet birleşik seçenekler.
- Tahta: ortası şeffaf 1125×1125 altın çerçeve, 300 px 9-slice.
- Engeller: merkezli bomba 384×384, üstten çivi başı 384×384, zincir 96×48 + kilit 144×144; kırmızı-krem duvar repeat 96×48 + köşe 96×96.
- Kartlar: pause, kazanma, nötr/bomba/süre kaybetme, başlangıç; ortak panel, üç renk tuş ve basılıları; dolu/boş yıldız ve ayrı kaybetme sebebi görselleri.
- Font: LilitaOne-Regular.ttf + LilitaOne-OFL.txt. ı/ç/ö/ü var, ğ/ş/İ yok; Türkçe metinlerde fallback gerekir.

manifest.json her dosyanın kullanım, boyut, sol/sağ/üst/alt 9-slice, normalize pivot ve SHA-256 bilgisini içerir. Nominal UI ölçüsü PNG boyutu / 3. Köşe pivotu (0.25,0.25), diğerleri (0.5,0.5).

Final inceleme: preview-export.png, preview-booster-states.png, preview-cards.png, preview-seams.png. Kaynaklar ve eski ekranlar Source içinde korunur. Source ve preview dosyaları runtime atlası değildir. Onaylı preview-ingame.png değiştirilmeden saklandı.

Kullanım: docs/art/TASK14_EXPORT.md. Teslim: docs/art/HANDOFF.md. Font: docs/art/TASK14_FONT.md. Kontrol: docs/art/reports/task14-export-validation.json. Prompt/araç: docs/art/templates/task14-final-export-prompts.md. Tekrar export: docs/art/export-task14.ps1.

Zincir/duvar repeat RGBA kenarları ve köşe bağlantıları piksel piksel eşleşir. 252 korunan kaynak değişmedi. C# dosyalarına dokunulmadı; oyun entegrasyonu yapılmadı. Bu tur yazımları yalnız Task14 ve docs/art içindedir.