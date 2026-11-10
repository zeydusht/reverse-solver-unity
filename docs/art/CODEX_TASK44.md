# CODEX_TASK44 — TASK43 düzeltmeleri (Zeyd geri bildirimi)

TASK43'ten **onaylanan**: güçlü parça çıkışı ve engel efektleri (T43 "sonra" hâlleri), sandığın kendisi (mavi + altın sandık ve açılma kareleri), "Sonraki" düğmesinin gövdesi. Gerisi aşağıdaki gibi değişiyor.

## İptal
- **Durdur menüsü (dikey tuş sütunu) İPTAL.** Eski büyük, ortadaki Durdur paneli kalıyor. Sebep: sütun tahtayı açıkta bırakıyor, oyuncu süreyi durdurup hamlesini düşünebiliyor. Ortadaki panel tahtayı kapatıyor.
- **"Sıradaki tablo" kartı ve Kont gölgesi İPTAL.**
- Kalabalık **sürü yoğunluğu** sahneleri iptal.

## 1. Jigo kaşları
"Mutlu" ve "normal" Jigo **asla kaş çatmaz**. Onaylı `Jigo_v3` mutlu ve normal karelerini kullan, yeniden çizme. Kazanma ekranındaki büyük Jigo da sıcak, içten gülümsemeli, kaşlar yuvarlak ve yukarıda.

## 2. Kazanma ekranı (yeniden)
- **Başlık "Harika!":** kurdele/afiş şekli kalkıyor (ucuz duruyor). Yerine kalın, büyük, 3B yazı: krem-sarı dolgu, kalın old money mavi kontur, altta altın kalınlık, hafif yukarı kavis, arkasında yumuşak ışık patlaması ve birkaç yıldız. Yazı canlı metin olacaksa, yazı stili örneği + arka ışık ayrı PNG.
- **Sandık ilerlemesi:** ince bar yerine **bölmeli ilerleme**: sandığa kadar kalan bölüm sayısı kadar yuva (ör. 3 yuva), dolanlarda altın yıldız/tik, sonunda sandık ikonu. Altında kısa ve okunur yazı ("2 bölüm sonra sandık!"). Yazı stili HUD ile aynı (krem, koyu kontur).
- **Kurtarılan Jiggle'lar:** başlık "**4 Jiggle kurtarıldı!**" (sayı canlı). Kompozisyon: **Jigo önde**, arkasında **sıra halinde** onu takip eden Jiggle'lar, hepsi **sağa doğru yürüyor** (yan/üç çeyrek görünüş, yürüme döngüsü 4–6 kare). Görünen en fazla 5–6 Jiggle; rastgele yığın yok.
- **Düğme:** "**Sonraki Seviye**" (metin canlı).

## 3. Ödül ekranı (yeniden)
- "Ödül!" kurdele afişi kalkıyor. Başlık §2'deki gibi 3B yazı: "Sandık!" ya da "Ödülün hazır!".
- "Aç" düğmesi yok. Sandığın altında yanıp sönen **"Sandığa dokun!"** yazısı + TASK41'deki yönlendiren el.
- Etkileşim: 1. dokunuş sandık sallanır, 2. dokunuş daha güçlü sallanır ve kapak aralığından ışık sızar, 3. dokunuş sandık açılır (onaylı kareler). Her aşama için kare: sallanma-1, sallanma-2 + ışık sızıntısı.
- **Ödüller büyük:** sandıktan tek tek fırlar, ekranın ortasında **büyük** (her biri ~90–110 pt) parlayarak durur, altında miktar ("+100", "x1"). Sonra ait oldukları yere uçar: altın üstteki altın sayacına, boosterlar alttaki tuşlarına. "Topla" düğmesi kalabilir.

## Teslim
`docs/art/task44/onizleme.png`: yeni kazanma ekranı, ödül ekranı 3 aşama + ödüllerin büyük hâli, Jigo mutlu/normal kontrolü. Dosyalar `Assets/Art/_yeni/T44/`. 3 satır özet, dur. **Teslimden önce kendi önizlemeni Zeyd'in bu notlarına karşı kontrol et** (kaş, kurdele yok, sıralı yürüyüş, büyük ödüller).
