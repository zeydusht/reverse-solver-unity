# Task14 export — Claude Code teslim notu

Zeyd tam ekran Task14 revizyon 2'yi onayladı. Final sprite'lar `Assets/Art/Gameplay/Task14/` kökünde; ham çizimler ve önceki taslaklar `Source/` içinde korunur. Entegrasyon yapılmadı; C# dosyasına dokunulmadı.

- `manifest.json`: her PNG'nin boyutu, kullanım yeri, @3x ölçeği, `sliceLTRB` ve pivotu. HANDOFF aynı veriyi dosya başına bir satır olarak içerir.
- Border sırası **sol / sağ / üst / alt**, birim kaynak PNG pikselidir. `0/0/0/0` sprite için 9-slice kullanılmaz. Pivot Unity'nin sol-alt başlangıcından normalize edilir; çoğu `(0.5,0.5)`, duvar köşesi dirsek merkezi `(0.25,0.25)`.
- UI nominal yerleşim ölçüsü PNG boyutu / 3'tür. @3x metadata PNG'ye fiziksel DPI yazılması anlamına gelmez. HUD hapları X yönünde esner; yükseklik 168 px sabit tutularak içerideki ikonların oranı korunur.
- `board-frame-gold.png` ortası gerçekten şeffaftır. Border 300 px, köşe süslerini korur. Görselin dışında 12 px teknik boşluk vardır. Duvar şeridi bu frame'e gömülü değildir.
- Tüm üretim PNG'leri gerçek alpha içerir; metin/sayı gömülmez. PALETTE.md'nin dokuz ana rengi kullanılır; ışık ve yumuşak degradeler bu renklerin karışımlarıdır. Final RGB değerleri palet renklerinin oluşturduğu renk hacmine sınırlandı; keskin renk basamakları eklenmedi.

## HUD ve booster

HUD hamle/süre ikonları hapın içindedir; ikon için ayrı yuva yoktur. LV, level sayısı, hamle ve süre TMP ile yazılır. Acil süre yüzeyi mercan; üzerinde krem sayaç kullanılır.

Booster için iki entegrasyon yolu aynı görünümü sağlar:

1. `booster-button*.png` zemin + 240×240 bağımsız ikon + 96×96 adet rozeti/kilit kaplaması. İkon canvas'ı butonda `(12,12)` konumuna oturur; görünür ikon 150 px, iç dairenin yaklaşık %70'i. Adet rozetinin 264×264 birleşik örnekteki konumu `(180,180)`, boyutu 78×78 px'tir.
2. `booster-<scissors|wand|hammer|clock>-<normal|pressed|locked|zero>.png` hazır 264×264 birleşikler. Bunların üstüne tekrar ikinci bir ikon/rozet bindirilmez. Adet rakamı her durumda ayrı TMP katmanıdır.

`zero` dosyası 0 rakamı içermez; soluk kullanılabilirlik görünümü + boş rozettir. Oyunun 0'ı yazması ve tıklama/satın alma davranışını belirlemesi gerekir. `locked` dosyası kilit içerir. Basılı durum aynı canvas/pivotu korur, 6 px aşağı hareket ve hafif gövde sıkışmasıyla hazırlanmıştır.

## Engeller ve tekrar

- Bomba 384×384; gövde merkezi `(192,192)`, çapı yaklaşık 292 px. Kısa fitil üst boşluktadır. Krem etiket gövdenin yaklaşık %60'ı. İki haneli sayı güvenli kutusu: üst-sol başlangıçlı `(127,142,130,100)`; Lilita One yaklaşık 108 px veya daha küçük. Sayı bitmap'te yoktur.
- Çivi 384×384; yalnız yukarıdan görülen baş. Sayı güvenli kutusu `(108,126,168,132)`; yaklaşık 120 px veya daha küçük metin.
- Zincir tekrar 96×48, kilit 144×144. Tekrar halkası X yönünde; kilit ayrı hücre merkezine bağlanır.
- Duvar tekrar 96×48; köşe 96×96. Köşe yukarıdan girer ve sağa çıkar. Diğer yönler sprite rotasyonuyla elde edilir. Zincir ve duvar düz parçasının ilk/son RGBA sütunları aynıdır; köşe sağ/üst bağlantıları düz parçaya eşlendi. `preview-seams.png` altı tekrar gösterir.
- Tekrar sprite'larını X yönünde Tiled olarak kullanın; yatay bütün resmi uzatarak halka/çizgi oranlarını bozmayın. Köşeyi döndürürken belirtilen pivotu kullanın. Atlas kenar taşması için en az 2–4 px extrusion/padding ayarlanır; oyun atlası bu teslimde değiştirilmedi.

## Kartlar ve tuş eşlemesi

Kart başlığı teal üst alana, açıklama gövdenin boş krem alanına ayrı TMP ile yerleşir. Başlık/ikon bölgesi 552 px üst slice içinde korunur. Tuşlar kart PNG'lerine gömülü değildir.

| Kart | Tuşlar | Önerilen konumlar (PNG pikseli, üst-sol başlangıç) |
|---|---|---|
| Pause, 969×1320 | Devam: primary; Yeniden başla: secondary; Ana menü: danger | 600×216 tuşlar; x=184; y=510 / 744 / 978 |
| Kazanma, 969×1200 | Devam: primary | x=184, y=852; 600×216 |
| Kaybetme, 969×1200 | Tekrar dene: primary; isteğe bağlı secondary | Ana tuş x=184, y=852; 600×216 |
| Bölüm başlangıcı, 969×1200 | Başla: primary | x=184, y=852; 600×216 |

Üç yıldız kazanma kartına yerleştirilmiş örnektir. Animasyon/dinamik yıldız sayısı için `card-panel.png` + ayrı `star-full.png` / `star-empty.png` kullanılır; hazır `card-win.png` üzerine yıldızlar yeniden bindirilmez. Kaybetmede nötr `card-lose.png`, bomba veya süre ikonlu iki hazır varyant ya da ayrı `loss-reason-*` kaplamaları kullanılabilir. Başlangıç kartının rozetindeki bölüm numarası boştur.

## Font ve inceleme

`LilitaOne-Regular.ttf` + `LilitaOne-OFL.txt` aynı klasördedir. Doğrudan cmap kontrolü: ı/ç/ö/ü var; ğ/ş/İ yok. TMP Türkçe metinleri için fallback gerekir. Ayrıntı: `docs/art/TASK14_FONT.md`.

Önizlemeler: `preview-export.png`, `preview-booster-states.png`, `preview-cards.png`, `preview-seams.png`. Önizlemeler ve `Source/` oyun atlasına alınmaz. Onaylı eski `preview-ingame.png` inceleme dayanağı olarak korundu; bu dosyadaki rakamlar üretim sprite'larına gömülmüş değildir.

Kaynak ve araç kaydı: `docs/art/templates/task14-final-export-prompts.md`; paketleme: `docs/art/export-task14.ps1`; doğrulama: `docs/art/reports/task14-export-validation.json`.
