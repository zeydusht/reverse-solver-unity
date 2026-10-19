# ZeydBot verisi — analiz notları

## Oyuncu kimliği (Zeyd, 2026-10-19)

Telefonda isim kapısına "ddd" yazılmış ve cihazda kalmış. Analizde:

- `plays.session = 'ad844d4abb404f5f97b5d9e1c28b23f7'` ve bu oturumdan **sonraki** `player = 'ddd'` satırları **TEST_ZEYD** sayılır.
- `moves` satırları `row_id` ile `plays`'e bağlanır; aynı kural geçerlidir.
- Yalnız ilk deneme (`attempt = 1`) analize girer; tekrarlar ayrıca işaretlenir.

## Gönderim

- Hamleler `POST /rest/v1/moves?on_conflict=row_id,move_no`, `Prefer: resolution=ignore-duplicates,return=minimal` ile gider (oyunun TelemetrySender'ı; plays ile aynı başlık). UPDATE politikası yok, merge yapılmaz.
- 006b'den önce gelen hamle POST'ları 401 aldı (SELECT politikası yoktu). 4xx alan satırlar oyunda "kenara ayrılır" (parked, en çok 100), yeniden gönderilmez; 006b'den önce oynanan hamleler büyük olasılıkla bu yüzden kayıp. `plays` satırları bu sorundan etkilenmedi (ayrı kuyruk).

## Z01'den önceki Z denemeleri neden plays'e yok (19 Ekim)

Zaman çizelgesi:

- 18:57 — Build 5z gh-pages'e push edildi (`0e6c633`).
- 19:12 ve 19:22 — mühendisliğin canlı kontrolü: site hâlâ **Build 4f**'i sunuyordu (zeydbot seti yok).
- 19:27 — site Build 5z'yi sunmaya başladı; canlı kontrol 7/7.

Build 4f'de `set=zeydbot` tanınmıyor: oyun canlı 40'ı yükler, `lv=Z0x` bulunamaz → menü / L seviyeleri. Bu aralıkta açılan Z linkleri **Z bölümü oynatmadı**; oynanan bir şey olduysa `plays`'e `level_id = 'L..'` olarak gitti (gönderim açık, debug yok). Sonrasında da Safari eski `index.html` / loader'ı önbellekten verebilir.

Kontrol (salt okuma):

```sql
select created_at, player, session, level_id, attempt, result
from plays
where created_at >= timestamptz '2026-10-19 18:50+03'
  and player in ('ddd', 'TEST_ZEYD')
order by created_at;
```

Zeydbot setinde gönderim kapalı DEĞİL: `?set=zeydbot` debug'sız açılır, `send = SendEnabled && !debug && !editor` → açık; plays satırları gider, hamleler ayrı kuyrukla `moves`'a.
