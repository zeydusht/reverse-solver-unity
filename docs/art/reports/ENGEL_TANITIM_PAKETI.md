# Engel tanıtım sahneleri · A/B

P0 #5 için dört engelin her birine iki sahne yaklaşımı hazırlandı. Görseller yazısızdır; kart başlığı, metni ve butonu dinamik UI katmanında kalır.

| Seçenek | Dosyalar | Görsel yön |
|---|---|---|
| A | `Assets/Art/ObstacleIntros/OptionA/{nail,bomb,chain,wall}.png` | Baron orta planda tehdidi gösterir; karakter ve engel dengeli okunur. |
| B | `Assets/Art/ObstacleIntros/OptionB/{nail,bomb,chain,wall}.png` | Engel yakın planda baskın, Baron kenarda; küçük kartta engel daha hızlı seçilir. |

Her çıktı 888×592 PNG sahne kartıdır; aynı kadraj ve aynı engel kimliği korundu. İlk 768×512 kırpımlar ve kaynak tabloları da seçenek klasörlerinde saklıdır. İnceleme: `Assets/Art/ObstacleIntros/review-options-v01.html`.

Üretim promptunun ortak yönü: premium animasyon filmi hissinde yumuşak 3D; seçilmiş Baron kimliği (süpürülmüş patlıcan rengi saç, çatık kaş, siyah-altın ceket, teal yelek); mevcut çivi/bomba/zincir/duvar konseptlerinin biçimini koruma; sade ve sıcak tiyatral kale ışığı; her panelde tek Baron ve tek tehdit; alt bantta sonradan UI metni koymak için boş alan; yazı, sayı, logo, watermark, kan/gore ve çerçeve yok.

A'da Baron her karede orta planda, tehdidi el hareketiyle sunar; bomba patlamaz. B'de Baron üst gövdesi kenara çekilir ve tehdit ön planda büyür. Dört sahne 3:2 ızgarada ayrı üretilip 768×512 kartlara ayrıldı.
