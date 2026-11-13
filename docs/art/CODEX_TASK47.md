# CODEX_TASK47 — Kombo efektleri: Block Blast'tan ders çıkar ve çiz

Zeyd: "Bizimkinde kombolar çok düz. Art arda çıkarınca Block Blast'taki gibi patlama efektleri olsun."
Referans video: `docs/art/reff/ScreenRecording_11-11-2026 11-28-02_1.MP4` (Block Blast, büyük dosya). Bizim mevcut kombo: `T43` patlamaları + `T41/Juice` (x2, x3 rozetleri, Harika/Süper/Mükemmel).

## 1. Önce izle ve ölç (zorunlu, ders çıkar)
- `ffmpeg` ile videoyu karelere ayır. Önce 1–2 fps genel pafta, sonra kombo anlarında 15–30 fps yakın paftalar. Paftalar `docs/art/task47/ref/`.
- `docs/art/task47/DERS.md` yaz, şu sorulara kısa ve ölçülü cevap ver:
  - Kombo seviyesi arttıkça (1., 2., 3., 4.+) neler **büyüyor**: yazı boyu, renk, patlama boyutu, parçacık sayısı, ekran sarsıntısı, ışık?
  - Kombo yazısı nerede çıkıyor, nasıl giriyor ve çıkıyor (ölçek, döndürme, süre ms)?
  - Ekran sarsıntısı, flaş, renk dalgası var mı; ne kadar sürüyor?
  - Skor sayısı nasıl uçuyor?
  - "Wow" hissini veren 3 ana şey ne?
  - Bize uymayacak olanlar (telif ya da stil olarak birebir kopyalanmayacak şeyler)?

## 2. Sonra çiz (bizim stilimizde)
Stil: onaylı HUD dili (old money mavi + krem, doygun altın/turuncu, hacimli, siyah kontur yok), parçalar resimli yapboz parçası. **Birebir kopya yok.** Block Blast'in yapısını ve ritmini al, görseller bizim.
- **Kombo merdiveni (4 kademe):** x2, x3, x4, x5+. Her kademede rozet büyür, renk ısınır (altın → turuncu → kırmızı-pembe → gökkuşağı parıltı), parçacık ve ışık artar. Rozet giriş/çıkış kareleri.
- **Kombo yazıları** (TR/EN, metin canlı olacaksa arka plan + stil örneği): "Güzel!" / "Harika!" / "Süper!" / "Muhteşem!" (kademe kademe büyüyen).
- **Çıkış patlaması kademeli:** aynı patlama kombo seviyesine göre 3 güç seviyesinde (küçük, orta, büyük; büyükte şok halkası + ekran kenarında renkli parlama).
- **Tahta ışık dalgası:** yüksek komboda tahta çerçevesi boyunca dolaşan parlak ışık (6–8 kare) ve hafif ekran flaşı katmanı.
- **Kombo sayacı:** küçük yanan çubuk ya da alev, sonraki çıkışa kalan süreyi gösterir (1,2 sn). Yanlış hamlede söner (kırılma 3 kare).
- Her şey **opak, doygun, büyük**; soluk sis yok.

## Teslim
- `docs/art/task47/onizleme.png`: 390×844 oyun ekranında kombo x2 → x5 dizisi (aynı ekran 4 kez), yakın planlar, Block Blast'tan aldığımız 3 dersin yanında bizim karşılığımız.
- `docs/art/task47/DERS.md`, `docs/art/task47/LISTE.md` (dosya, kare, fps, kod tarafının yapacağı ölçek/sarsıntı/süre değerleri; Claude Code bunu uygulayacak).
- Dosyalar `Assets/Art/_yeni/T47/`. **.meta dosyası yazma** (Unity üretiyor). 3 satır özet, dur.
