# CODEX TASK 18 — Koridor kazanma animasyonu: tablosuz (değiştirilebilir tablo) sürüm

Zeyd `corridor-victory-v2-floor-20261031` taslağını onayladı. Bu stille devam ediyoruz, yeniden çizme. Tek değişiklik: **tablo videoya pişmeden gelmeli**, oyun her seviyede kazanılan tabloyu üstüne koyacak.

## Üret

Klasör: `docs/art/drafts/corridor-victory-v3-layered-20261031/`

1. `corridor-empty.mp4` — aynı 3 sn, 60 FPS, aynı kamera hareketi. **Altın çerçevenin içi düz canlı yeşil (#00FF00)** dolsun (resim yok). Çerçeve, duvar, zemin, vazo, ışık aynı kalsın. Yeşil alan çerçevenin iç kenarına tam otursun, taşma ve kenar solması olmasın.
2. `frame-track.json` — her kare için (181 kare) yeşil alanın 4 köşesinin piksel koordinatı (sol-üst, sağ-üst, sağ-alt, sol-alt) ve çerçevenin görünürlüğü (kamera kaydıkça kısmen ekran dışına çıkıyor mu). Biçim: `{"fps":60,"width":780,"height":1688,"frames":[{"i":0,"quad":[[x,y],[x,y],[x,y],[x,y]],"visible":true}, ...]}`.
3. `corridor-empty-first.png` ve `corridor-empty-last.png` — ilk ve son kare (döngü kontrolü için).
4. Kısa not (`README.md`): video süresi/boyutu. Hedef: web'de oynatmak için mümkünse **2 MB altı** (gerekirse 30 FPS, 540 genişlik sürümü de ver: `corridor-empty-web.mp4` + ayrı `frame-track-web.json`).

## Kural
- Yalnız `docs/art/` altına yaz. Oyuna, Unity'ye, `Assets/` içine dokunma.
- Yeşil alan dışında hiçbir şeyi değiştirme (zemin revizyonu dahil aynı).
- Bitince hangi dosyaların hazır olduğunu 3 satırda yaz.
