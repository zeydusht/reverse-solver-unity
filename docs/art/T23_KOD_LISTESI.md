# Task23 — Kod bağlantı listesi (17 aile)

Durum: çizim teslimi. Oyun kodu, prefab, sahne, Paintings ve mevcut onaylı PNG dosyaları değiştirilmedi. Son kullanıcı talimatı mevcut DENEMELER’deki 17 aileyi nihai seçip Task24’e geçmektir. Eksik kalan 10 aile aşağıda açıkça listelenir.

## Bağlama kuralları

- Her `NIHAI/<aile>/deneme.json` dosyasındaki `assets` kesin dosya listesidir; `logicalPt`, @3x ölçü, `sliceLTRB`, kaynak ve SHA-256 içerir. Unity border sırası L,B,R,T olarak çevrilir. PPU/Canvas ölçeğini mevcut sistemden koru; @3x PNG pikselini mantıksal UI ölçüsü sanma.
- `onizleme`, `ekran-sonra`, `once-sonra`, `olcu-*` runtime sprite değildir. Yazılar Nunito Black ayrı katman; başlık22/23, gövde14–16, puan17/18pt. Resme metin gömme.
- Panel 335pt genişlikte; telefon güvenli alanında ortala. Yeşil tuş684:336 oranını koru; sıkıştırılmış düz şerit yapma. Basılı sprite da aynı ölçekte. Dokunma hedefleri en az44pt.
- 375×667 ve390×844 PNG ölçü kontrolleri grafik kanıtıdır; gerçek Unity akışı, klavye ve cihaz testi tamamlanmış sayılmaz.

| Aile | Sınıf / yer | Değişecek kaynak / anahtar | Nihai |
|---|---|---|---|
| 01-bolumler | `LevelsMap` | Task20/node-* + LevelsMap numara/rozet/temas yerleşimi | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/01-bolumler/NOT.md) |
| 02-kazanma | `Screens.Result / Card` | Task14/card-panel.png → popup-card | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/02-kazanma/NOT.md) |
| 03-kayip-bomba | `Screens.Result / Card` | popup-card + loss-reason-bomb | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/03-kayip-bomba/NOT.md) |
| 04-kayip-sure | `Screens.Result / Card` | popup-card + loss-reason-time | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/04-kayip-sure/NOT.md) |
| 05-durdurma | `Screens.Pause / Card` | popup-card + btn2-primary / btn2-secondary | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/05-durdurma/NOT.md) |
| 06-puan-kutulari | `Card.ChipRow` | ShapeView chip yüzeyi → rating-normal/selected | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/06-puan-kutulari/NOT.md) |
| 07-tanitim-civi | `Screens.Intro / Screens.StartCard` | intro-nail / popup-card; ortak sahne alanı | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/07-tanitim-civi/NOT.md) |
| 08-tanitim-bomba | `Screens.Intro / Screens.StartCard` | intro-bomb / popup-card; ortak sahne alanı | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/08-tanitim-bomba/NOT.md) |
| 09-tanitim-zincir | `Screens.Intro / Screens.StartCard` | intro-chain / popup-card; ortak sahne alanı | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/09-tanitim-zincir/NOT.md) |
| 10-tanitim-duvar | `Screens.Intro / Screens.StartCard` | intro-wall / popup-card; ortak sahne alanı | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/10-tanitim-duvar/NOT.md) |
| 11-booster-makas | `Screens.Intro / Screens.StartCard` | intro-scissors / popup-card; ortak sahne alanı | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/11-booster-makas/NOT.md) |
| 12-booster-degnek | `Screens.Intro / Screens.StartCard` | intro-wand / popup-card; ortak sahne alanı | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/12-booster-degnek/NOT.md) |
| 13-booster-cekic | `Screens.Intro / Screens.StartCard` | intro-hammer / popup-card; ortak sahne alanı | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/13-booster-cekic/NOT.md) |
| 14-booster-saat | `Screens.Intro / Screens.StartCard` | intro-clock / popup-card; ortak sahne alanı | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/14-booster-saat/NOT.md) |
| 15-zor-bolum | `Screens.Intro / Screens.StartCard` | intro-hard / popup-card; ortak sahne alanı | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/15-zor-bolum/NOT.md) |
| 17-isim | `WebGLTemplates/ReverseSolver #rs-name` | name-card/input/rs-name-go;107 kabul yönü | `B`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/17-isim/NOT.md) |
| 18-profil-sorusu | `Profile survey / Card` | popup-card / profil anketi | `A`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/18-profil-sorusu/NOT.md) |

## Davranış ve metin yerleşimi

- **01 Bölümler öncelik:** merkezde bölüm numarası20pt. Kilit20×22pt ayrı sağ alt rozet, bitenlerde yeşil tik. Güncel düğüm altın66pt, biten turkuaz54pt, kapalı koyu54pt; zor çerçeve ayrı. Oyna84×44pt. Düğümler basamak yüzeyine oturmalı; `anchors` örneğini merdiven fazı ve scroll koordinatıyla eşleştir. Ortada büyük kilit eski taslakları kullanma. PM36 kararı.
- **02–04 sonuç:** üç yıldız/istatistik, ana yeşil eylem, isteğe bağlı1–10 puanlama ve Puanlamayı geç korunur. Kayıpta nedeni ve iki satır yardım göster; puanlama kapalıysa paneli9-slice kısalt. UI kodunda var olan sonuç/tekrar/menü olaylarını kullan.
- **05:** aynı boyda üç tuş; Devam yeşil, Yeniden başla/Ana menü turkuaz. Aynı mevcut pause olayları.
- **06:** iki sıra, beş sütun.50×44pt kutular,7pt yatay aralık,50pt satır adımı. Sayılar18pt ayrı metin. Seçili kutuda tik var.
- **07–15:** üstte tek karakter sahnesi, altında en fazla iki kısa satır, altta tek yeşil tuş. `text` dizisindeki koordinatlar335pt kart yereline göredir. Küçük ikon kutusunu ve turuncu ikinci mesajı kaldır. Engellerde ve zor bölümde Baron; booster sahnesinde Jiggle. Sahne PNG’sini9-slice yapma.
- **15 tek uyarı:**19/23/25/30/35/36/38’de yalnız bir Baron kartı.23’ün sonraki çekiç el eğitimi ayrı adım; ikinci Baron kartı oluşturma. Mevcut eğitim eli dosyaları değişmez.
- **17:** WebGL isim alanı gerçek HTML input; Adın placeholder, Başla eylemi, iki kısa açıklama. Playtest sürümü metni yok. Klavye açıkken safe-area ve scroll canlı test edilmeli.
- **18:** isim kartıyla aynı panel; Hiç / Ara sıra / Sık eşit turkuaz cevaplar. Var olan anket kaydı ve ilerleme olayları korunur.

## Bu teslimde tamamlanmayan aileler

16 yükleme;19 ayar anahtarları;20 dil;21 koleksiyon;22 Jiggles;23 HUD;24 booster çubuğu;25 koridor;26 koleksiyon bitişi;27 ev üst barı. Ek kaynak çizimleri var olsa da bunlar nihai teslim değildir. Son kullanıcı kapsamı gereği Task24’e geçildi.

Not: Önce görüntülerinde bazı tanıtımların tam eski modalı bulunmadığından ilgili öğretim/onay ekranı kullanılmıştır; aile NOT dosyalarında sınırlamalar yazılıdır.


## Task25 güncellemesi

-17-isim: gerçek HTML input placeholder rengi #92999E ve opacity1; girilmiş isim #0B2A36 kalır. İki durum NIHAI/17-isim/isim-bos.png ve isim-yazildi.png.
-15-zor-bolum: yeni character-scene.png288×192pt (@3x864×576),9-slice yok. Aynı Baron kolları kavuşturulmuş; arkadaki karışık tahta zor bölüme özel. Panel/metin/tek uyarı davranışı korunur.
-İki kartın Task23 sürümleri kendi T25-oncesi altında; güncel inceleme NIHAI/T25-inceleme.html.
