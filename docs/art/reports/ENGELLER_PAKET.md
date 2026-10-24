# Engel konsept paketi · v01

Tarih: 24 Ekim 2026 · Dal: `art` · Üretim aracı: yerleşik image_gen. Bu dosyalar görsel konsept ve dışa aktarım adayıdır; Zeyd'in final çizim onayı ve oyun içi okunabilirlik testi beklenir.

| Engel | Kaynak PNG | Şeffaf 1024×1024 dışa aktarım | Kontrol |
|---|---|---|---|
| Bomba | `Assets/Art/Obstacles/bomb.png` | `Assets/Art/Obstacles/Exports/bomb.png` | Ayrı, yuvarlak koyu gövde/kırmızı bant/altın bilezik ve fitil |
| Çivi | `Assets/Art/Obstacles/nail.png` | `Assets/Art/Obstacles/Exports/nail.png` | Baş ve sivri uç tek siluette |
| Zincir | `Assets/Art/Obstacles/chain.png` | `Assets/Art/Obstacles/Exports/chain.png` | İri, birbirine geçen metal halkalar |
| Duvar | `Assets/Art/Obstacles/wall.png` | `Assets/Art/Obstacles/Exports/wall.png` | Tek şerit kapatan kısa duvar işareti |

Ham ayırma çıktıları `Assets/Art/Obstacles/Sources/` altında tutuldu. Kaynak ana dosyalar değiştirilmedi. Görseller `asset-metrics.json` içindeki boyut/alpha ölçümleriyle ve gerçek koridor fonunda görsel olarak incelendi; dışa aktarımların köşe alpha değeri 0. Çıktılar şeffaf PNG'dir, telefon ekranında ölçü testi değildir.

Kaynak-sheet üretimindeki ajan tam prompt günlüğünü kullanım sınırına çarptığı için teslim edemedi. Bu yüzden burada o promptları kelimesi kelimesine kayıtlıymış gibi göstermiyorum. Stil dayanağı `docs/art/STYLE_CARD.md` v0.2; referans kimliği, üretici kaynakları ve dosya sürümleri yukarıda belirtilmiştir.
