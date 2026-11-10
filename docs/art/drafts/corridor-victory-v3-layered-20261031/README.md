# Task18 — değiştirilebilir tablo içi

Onaylı `corridor-victory-v2-floor-20261031` sahnesi kullanıldı. Yalnız tablo içi düz canlı **#00FF00** ile dolduruldu. Çerçeve, duvar, mimari, vazo, güneş ışığı ve onaylı zemin değiştirilmedi. Oyun/Unity/Assets dosyalarına yazılmadı; bütün yeni dosyalar bu `docs/art/` klasöründedir.

| Dosya | Çözünürlük | FPS / kare | Süre | Boyut |
|---|---|---|---|---|
| `corridor-empty.mp4` | 780×1688 | 60 / 181 | 3,016667 sn | 1.802.795 bayt ≈ 1,80 MB |
| `corridor-empty-web.mp4` | 540×1168 | 30 / 91 | 3,033333 sn | 701.636 bayt ≈ 0,70 MB |

Hareketin kendisi iki sürümde de **3 saniye**. Son tam kare dâhil dosya süresi bir kare daha uzundur. Her iki video 2.000.000 baytın altındadır. Web yüksekliği H.264 için çift piksele yuvarlanmıştır. H.264/AVC, sessiz, yazılımsal kodlama; standart web video oynatımı için MP4.

`corridor-empty-first.png` ve `corridor-empty-last.png` 780×1688 ham karelerdir; piksel piksel aynıdır. Ham yeşil alanlar tam RGB(0,255,0), alfa veya kenar solması içermez. Diğer bütün ham pikseller onaylı karelerle aynıdır. MP4 kayıplı H.264/YUV420 olduğundan kod çözümünde renkler çok az değişebilir ve renk alt örneklemesi sınır pikselini etkileyebilir; saf RGB istenen referans PNG/mattır. Oyunda görüntüyü takip dörtgenine yerleştirmek, yalnızca tek bir RGB değerine göre chroma-key yapmaktan daha sağlamdır.

## Köşe takip dosyaları

- `frame-track.json`: 60 FPS, **181 kayıt**, 780×1688.
- `frame-track-web.json`: 30 FPS, **91 kayıt**, 540×1168. Web videosu ile kullanılmalıdır.
- `i` sıfırdan başlar. Zaman `i/fps`; son kayıt 3,0 saniyedeki bitiştir.
- `quad` sırası **sol-üst, sağ-üst, sağ-alt, sol-alt**. Koordinat başlangıcı ekranın sol üst köşesi; değerler piksel cinsindedir.
- Köşeler ekran sınırına **kırpılmaz**. Negatif/ekran dışı koordinat, kısmen görünür çerçevenin doğru homografisini korur.
- `visible`: dörtgenin herhangi bir alanı ekranda mı? `partiallyVisible`: bir kısmı dışarıda mı?

Hareketin ortasında **iki komşu tablo aynı anda kısmen görünür**. Bu yüzden her kareye `instances` dizisi de eklendi: `bay-1` çıkan, `bay-2` giren sabit kimliktir. İkisinin de `quad`, `visible`, `partiallyVisible`, `visibleFraction`, `visibility` (`full/partial/hidden`) ve ekranla kesişen `clippedPolygon` bilgisi vardır. Ekranda görünen **her iki instance'a da** tablo yerleştirilmelidir.

Üst düzey `quad`, en büyük görünür alana sahip çerçeveyi verir; hangisi olduğu `frameId` alanındadır. Baskın çerçeve ortada değişir. Kesintisiz nesne takibi için `instances` içindeki sabit kimlikleri kullanın; üst düzey `quad` verisini komşu kareler arasında gelişigüzel interpolate etmeyin. Her kare zaten hesaplanmıştır. Dörtgenler dikdörtgen kutuya indirgenmedi; orijinal eğik tablo içini izler.

İç köşelerin onaylı tek bölümdeki koordinatları: `[[144.5,167.5],[361.5,191.5],[356.5,507.5],[137.5,498]]`. Önceki 496 kaynak piksel mesafe, aynı yön, sabit kadraj ve aynı quintic yumuşatma korunur. Yeni çizim veya imagegen kullanılmadı; yeşil, onaylı görüntü render edildikten sonra yalnız bu iç dörtgenlere ikili maske olarak uygulanır.

## Kontrol / yeniden üretim

`node export.cjs` → `verify.py`. Yerel Chrome profili de bu klasörün altında tutulur. Başka dizine çıktı yazılmaz.

Her iki MP4 tarayıcıda tekrar çözümlendi. Kodlamaya değişmez RGBA piksel anlık görüntüleri verilir; ilk/orta/son görüntüler ayrıca VideoDecoder zaman damgalarından alınır (`qa/*-exact-frame-*.png`). Böylece video elementinin seek sonrası eski görüntüsünü kontrol etme riski olmadan, ortada gerçekten iki kısmi yeşil çerçeve bulunduğu doğrulanır. Onaylı sürümün ilk/orta/son ham kareleriyle, yeşil alan dışındaki pikseller karşılaştırıldı; fark sıfır. Saf yeşil, ilk/son eşitliği, köşe/görünürlük verisi ve web takibinin aynı zamanlardaki ana takip ile eşleşmesi de kontrol edildi. Sonuçlar `qa-report.json` / `export-report.json`; inceleme paftası `qa/checkpoints.png`.
