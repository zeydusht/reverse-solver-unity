# English text draft

Player-facing English equivalents for the current UI constants and level `intro` / `bintro` copy. Proper character names remain **Jiggles** and **Baron**.

## UI constants (`Texts.cs`)

| Source | Turkish | English |
|---|---|---|
| `TutorialFirst` | Bu parçayı it ve kurtar | Push this piece to rescue it. |
| `TutorialNext` | Şimdi bunu kurtar | Now rescue this one. |
| `HardTitle` | Zor bölüm | Hard Level |
| `HardBody` | Bu bölüm zor, haberin olsun. Süre ilk hamlende başlar. | This level is tough, just so you know. The timer starts with your first move. |
| `HardStart` | Başla | Start |

## Level copy (`redesign.json`)

| Level / field | Turkish | English |
|---|---|---|
| 3 `intro.title` | Çivili parça | Nailed Piece |
| 3 `intro.body` | Çivili parçanın sayacı her hamlende 1 azalır. 0 olunca parça serbest kalır. | A nailed piece's counter drops by 1 with every move. At 0, the piece is free. |
| 3 `intro.tip` | Önce diğer parçaları çıkar, çivi kendiliğinden sökülür. | Free the other pieces first; the nail will come off on its own. |
| 5 `bintro.title` | Makas | Scissors |
| 5 `bintro.body` | Makas iki parça arasındaki birleşimi keser ve o kenar dümdüz olur. Kesilen yerde artık hiçbir şey takılmaz, sıkışmış parça serbest kalır. | Scissors cut the connection between two pieces, making that edge straight. Nothing can catch on a cut edge, so the stuck piece is freed. |
| 5 `bintro.tip` | Makasa bas, sonra kesmek istediğin turuncu noktaya dokun. | Tap Scissors, then tap the orange point you want to cut. |
| 6 `intro.title` | Bombalı parça | Bomb Piece |
| 6 `intro.body` | Bu bombayı 23 hamle içinde çıkarmalısın. Sayı her hamlende azalır; sıfıra inerse bölüm biter. | Get this bomb out within 23 moves. The number drops with each move; if it reaches zero, the level ends. |
| 6 `intro.tip` | Bombanın yolunu açan parçaları bul ve önce onları çıkar. | Find the pieces blocking the bomb's path and move them first. |
| 9 `bintro.title` | Değnek | Wand |
| 9 `bintro.body` | Değnek resmi bozmadan tokmakların hangi tarafta olduğunu yeniden karıştırır. Tahta aynı kalır ama hangi parçanın kilitli olduğu tamamen değişir. | The wand reshuffles which sides have tabs without changing the picture. The board stays the same, but which pieces are locked changes completely. |
| 9 `bintro.tip` | Tıkandığında yeni çıkış yönleri açar. | Use it when you're stuck to open up new ways out. |
| 10 `intro.title` | Zincirli çift | Chained Pair |
| 10 `intro.body` | Aynı renkte halka taşıyan iki parça zincirle bağlıdır ve tek parça gibi hareket eder. İkisinin de önü aynı yöne açık olmalı. | Two pieces with matching colored rings are joined by a chain and move as one. Both need an open path in the same direction. |
| 10 `intro.tip` | Zinciri takip et, iki ucunu da serbest bırak. | Follow the chain and free both ends. |
| 14 `intro.title` | Mühürlü kenar | Sealed Edge |
| 14 `intro.body` | Kırmızı şeritli kenar bir duvardır, o yönden hiçbir parça çıkamaz. | An edge with a red stripe is a wall; no piece can leave from that side. |
| 14 `intro.tip` | Çıkış yönleri azaldı, parçaları başka yöne sürükle. | With fewer exits, slide the pieces in another direction. |
| 15 `bintro.title` | Saat | Clock |
| 15 `bintro.body` | Saat geri sayıma 20 saniye ekler. | The clock adds 20 seconds to the countdown. |
| 15 `bintro.tip` | Süre daralınca en hızlı çözüm budur. | When time is running low, this is the quickest fix. |
| 23 `bintro.title` | Çekiç | Hammer |
| 23 `bintro.body` | Çekiç seçtiğin parçayı kilitlerine bakmadan söküp atar. Çivili parçayı da bombalı parçayı da tek hamlede kaldırır. | The hammer removes the piece you choose, ignoring its locks. It can remove a nailed piece or a bomb piece in one move. |
| 23 `bintro.tip` | Çekice bas, sonra kırmak istediğin parçaya dokun. | Tap Hammer, then tap the piece you want to smash. |

## Tutorial

Canonical copy for `Texts.TutorialDrag` / `Texts.TutorialCopy`. Rule line breaks are layout only.

| Key | Turkish | English |
|---|---|---|
| `drag.L` | Parçayı sola sürükle | Drag the piece to the left |
| `drag.R` | Parçayı sağa sürükle | Drag the piece to the right |
| `drag.U` | Parçayı yukarı sürükle | Drag the piece up |
| `drag.D` | Parçayı aşağı sürükle | Drag the piece down |
| `rule` | Tüm parçaları diğer parçalara çarpmadan dışarı çek! | Pull every piece out without hitting other pieces! |
| `nail` | Çivili parçanın sayacı her hamlende 1 azalır. 0 olunca parça serbest kalır. | A nailed piece's counter drops by 1 with every move. At 0, the piece is free. |
| `bomb` | Fitili bitmeden bombayı çıkar! | Free the bomb before its fuse runs out! |
| `chain` | Zincirli parçalar birlikte çıkar. İkisinin de yolunu aç. | Chained pieces leave together. Clear both paths. |
| `wall` | Parçalar kırmızı-beyaz duvardan geçemez. | Pieces can't pass through red-white walls. |

Rule check (P20-D): nail counters decrease on successful removals (with a safety release if all free pieces are pinned); the nail sentence makes that trigger explicit. Bomb fuses also count removals, not seconds. Chained cells are one rigid piece and must clear both corridors in the same direction. Red-white sealed lanes block passage. No Core changes.
## P20-E UI copy

| Key | TR | EN |
|---|---|---|
| `Texts.Level` | Bölüm {0} | Level {0} |
| `Texts.HardBody` | Zor bölüm! Acele etme, önce bak. | Tough level! Look before you pull. |
| `Texts.Play` | Oyna | Play |
| Name screen | Deneme sürümü. Adını yaz, sonuçlar bu isimle kaydedilsin. | Playtest version. Enter your name to save your results. |

P20-E: "every move" means a successful piece removal (hammer included); jams do not decrement nails. Intro and spotlight both use `Texts.NailBody`. The safety valve can also free a nail when every geometrically free piece is pinned.
