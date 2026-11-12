# Reverse Solver açılış storyboard’u

Yedi dikey kare. İlk dört kare onaylı Task26 görsellerinden, son üçü bu kurgu için üretildi. Aktif kurgu yaklaşık 17 saniye. Uyandırma dokunuşunda bekleme bu süreye dahil değil.

| Kare | Süre | Kadraj ve aksiyon |
|---|---:|---|
| [01 · Dondurma](../task26/cekim-kareleri-v06/05-bitis.png) | 3 sn | Kont mum ışığındaki atölyede dört Jiggle’ı tepsiye dizer. Mavi cilalı fırça değdiğinde Jiggle’lar kısa bir ışıltıyla hareketsiz kalır. |
| [02 · Çerçeve](../task26/cekim-kareleri-v06/06-baslangic.png) | 2 sn | Kont altın çerçeveye son dokunuşu yapar. Sırıtıp eserini kaldırır. Tamir kesesini arkadaki askıya takar. |
| [03 · Galeri](../task26/cekim-kareleri-v06/07-baslangic.png) | 2,5 sn | Galerinin uzunluğunu gösteren geniş açı. Kont tabloyu diğerlerinin yanına asar, çerçeveyi hizalar ve uzaklaşır. |
| [04 · Uyanış](../task26/cekim-kareleri-v06/08-bitis.png) | 2 sn + dokunma | Karanlıkta tabloda yalnız sol üstteki mavi Jiggle kıpırdar. Oyuncunun dokunuşuyla gözleri açılır. |
| [05 · Düşüş](acilis-kareleri/05-dusus.png) | 2 sn | Geniş alçak açı. Jigo çerçeveden halıya düşer, kese de askıdan çıkar. Arkadaşları tabloda kalır. |
| [06 · Kese](acilis-kareleri/06-kese.png) | 2,5 sn | Halı hizasında yakın çekim. Jigo keseden lacivert kumaşı çeker. Yüksük, düğme ve mavi uçlu fırça önünde durur. |
| [07 · Karar](acilis-kareleri/07-karar.png) | 3 sn | Jigo yüksüğü, kumaşı, düğme kalkanı ve fırça mızrağını kuşanır. Dolu çerçevelere bakar ve kurtarmak için ilk adımı atar. Kameranın ışıklı çerçeveye yaklaşması oyuna geçişi hazırlar. |

## Süreklilik

Çayır tablosunun sol üst yuvası 05–07 boyunca boş. Koridorun petrol mavisi duvarı, altın çerçevesi ve bordo halısı her kesmede aynı. 06’da görünen donanım 07’de giyilir. Açık ve koyu galeriyi bağlamak için 04’ten 05’e geçerken en yakın aplik yeniden yanar.

İlk dört kaynak 1080×1920, yeni RGB storyboard kareleri 941×1672. Yeni karelerde 9:16 kadraj, karakter silueti ve tablodaki üç arkadaş görsel olarak kontrol edildi. [Jigo föyü](JIGO.md) · [Kont föyü](KONT.md)

## ImageGen istemleri

Yeni kareler yerleşik ImageGen ile üretildi. Galeri ve uyanış için Task26 kareleri, giyilen donanım için Jigo föyü referans oldu.

### 05 · Düşüş

```text
One cinematic storyboard key frame, vertical 9:16 portrait with the visual action centered in a safe mobile crop. Continue the supplied Reverse Solver shots in the same royal puzzle-game 3D animated film look, exactly the same petrol-blue wall, cream stone plinth, gilded rectangular meadow portrait, warm brass sconces, dim moonlight and burgundy runner. Low wide shot. One small turquoise Jiggle with the identical rounded square puzzle shape and brown eyes, no costume yet, is clearly airborne outside the painting, comically tipping forward in mid-fall toward the carpet. His hands and feet extend in surprise. The gilded frame still hangs on the left wall above him, gently tilted. A small empty top-left gap reveals meadow behind the character's former place, while the three coral, lavender and green companions remain flat and motionless in the picture. The small brown leather repair pouch tumbles separately a little behind him. No Count in frame, no glass shards or danger, no words, no UI. Strong visual separation between the gold frame and the turquoise body, toy material, amber bounce light, quiet blue shadows. Show floor and frame in one readable shot.
```

### 06 · Kese

```text
One cinematic storyboard key frame, vertical 9:16 portrait with a safe centered mobile crop. Same dim royal castle gallery as the supplied Reverse Solver reference, petrol-blue wall, cream stone skirting, burgundy runner with restrained gold border, amber sconce and a touch of moonlight. Very low carpet-height medium close-up. On the floor the little canonical turquoise jigsaw creature Jigo has just landed, eyes wide and curious, still wearing no clothes. With one hand he lifts the corner of a short folded dark marine tabard with cream edging from an open small brown leather repair pouch. Spread on the runner and unmistakably readable are a silver dimpled thimble, a single creamy FOUR-hole button and the gold-ferruled paintbrush with a pointed cobalt paint tuft. These are the same props as the supplied character sheet but loose on the carpet. Keep hero turquoise with the correct raised tab, rounded shape and short limbs. Upper background softly shows the same gold frame out of focus. One hero only. Soft tactile rounded 3D family-animation render, warm/cool light, restrained spectacle, clear narrative without text, no logos, no UI.

Keep this exact carpet-height close-up with Jigo kneeling, lifting dark cloth from an open bag, silver thimble, four-hole button and blue paintbrush on the carpet. Preserve all foreground elements pixel-for-pixel as much as possible. Make only one small edit in the blurred background: in the big gold painting at the upper left, the blue painted character at the TOP LEFT should disappear. That little spot should instead be soft out-of-focus green grass, with only the other three coral, purple and green painted characters visible. The real large foreground blue Jigo must remain in the same kneeling pose. Do not recompose, change the shot, show a fall or show a different angle. Vertical 9:16.
```

### 07 · Karar

```text
One cinematic final opening storyboard key frame, vertical 9:16 portrait with safe centered staging. In the same Reverse Solver gallery at night, recognizable petrol-blue walls, cream stone arches, warm brass sconces, ornate gilded frames and burgundy runner. Very low three-quarter rear-side tracking angle at toy height. Foreground left-center the little turquoise jigsaw hero Jigo is newly dressed exactly like the supplied costume sheet, silver dimpled thimble helmet, short dark marine tabard with cream edging and leather belt, four-hole ivory button shield in anatomical left hand and the same cobalt-tipped brush-spear in anatomical right. Show part of his face in profile with a tiny determined warm expression as he turns toward the deep row of gold frames filled with other colorful trapped Jiggles. A warm patch of light on the first occupied frame leads the eye onward down the gallery. Hero remains small beside the tall setting. Same soft rounded premium mobile puzzle-game animated-film materials, cinematic depth, amber light and petrol-blue shadows, no extra weapons, no typography, no logos, no UI. A quiet visual decision to rescue his friends, no fighting.

Precise object edit, same exact portrait 9:16 key frame and art. Edit only the INSIDE of the nearest large ornate gold meadow painting in the upper LEFT. The turquoise puzzle creature has already jumped out of this painting. REMOVE the turquoise blue creature from the top-left position inside this one painting, including its turquoise body, face, eyes and any blue silhouette. In that precise position reveal the painted green hillside and grass with a subtle empty puzzle-shaped gap, as in the second reference image. Keep the coral top-right, lavender lower-left and green lower-right creatures unchanged. Maintain the gold frame, perspective, depth of field, palette and lighting. Preserve the foreground living turquoise Jigo exactly as it is, and preserve every object and all other frames. No new subjects, no text.
```
