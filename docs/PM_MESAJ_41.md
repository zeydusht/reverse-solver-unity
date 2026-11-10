# PM → Claude Code (3 Kasım 22:00): Zeyd'in iPhone notları — son düzeltme, sonra davet (p20)

**İlk iş:** commit et. PM dosyalarını asla geri alma. PM_40 bitmediyse önce onu bitir, sonra bunu yap. İkisini tek build'de yayınlayabilirsin.

## 1. Kart düğmelerindeki yazılar okunmuyor (iPhone Safari)
Zeyd'in iPhone'unda kazanma ve durdurma kartlarındaki düğme yazıları ince ve içi boş kontur gibi görünüyor, okunmuyor. Etkilenen yazılar: "Next level", "Play again", "Menu", "Continue", "Restart", "Main menu". Kart başlıkları ("Board cleared!", "Paused") iyi görünüyor.

- **Bütün kart düğmelerinde** başlıklarla aynı metin stilini kullan (STIL_REHBERI_RM):
  - Nunito Black
  - krem dolgu
  - ince koyu kontur
  - aşağı gölge
  - en az 20 pt
- Sorun büyük ihtimalle PM_39'da yeni kartlar bağlanırken düğmelerde eski ya da farklı bir font ve materyal kalması. Hangi materyal ve fontun kullanıldığını bul. Codex'in bağladığı bütün kartlarda aynı yolu düzelt.
- **WebKit'te doğrula:** Playwright `webkit` kullan, Chromium yetmez. iPhone 390×844'te kazanma, kayıp, durdurma, tanıtım ve isim kartlarının ekran görüntüsünü al. Her düğme yazısı rahat okunmalı.

## 2. Bölüm sonu puanlamayı kaldır (Zeyd kararı)
- Kazanma ve kayıp kartlarından **1–10 kutularını, "Did you like this level?" 👍/👎 sorusunu ve "Puanlamayı geç"i kaldır.** Kart yalnız başlık, süre/takılma satırı ve düğmelerden oluşsun.
- `plays` satırı aynı şekilde bölüm biter bitmez gitsin. `zorluk_puani` ve `keyif` boş kalsın. `set_rating` RPC'si ve kuyruk kodu yerinde kalsın, yalnız çağrılmasın.
- Bilgi için: Zeyd'in telefonunda 1–10 hiç çıkmadı, yalnız 👍/👎 çıktı. Hangi koşulla hangisinin gösterildiğini OVERNIGHT'a tek satırla yaz.

## Bitiş
- p20'ye yayınla. Sürüm etiketi `web-p20-2026-11-03` kalsın.
- Ekran görüntüleri: `docs/qa/2026-11-03-son-kontrol/` (webkit_ önekiyle).
- OVERNIGHT başına 2 satır, sonra Zeyd'e "bitti" de.
