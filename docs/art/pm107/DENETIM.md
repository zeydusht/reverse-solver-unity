# PM_107 §D1: görsel uyum denetimi

## Nasıl yapıldı
- **Görüntüler:** Bütün ekranlar 390×844'te (2×) gezildi. Araç `Tools/gif_capture.js`; bayraklar `&kazan=1`, `&sandik=1`, `&devam=1`, `&devamhayir=1`, `&bitis=1`, `&reklam=sahte` ve ders/engel kartlı bölümler (lv 3, 6, 7).
- **Ölçüt:** Yeni palet "old money" lacivert (#0E2244, #153E75, #234670), krem (#FFF6D8), altın (#E3B54E) ve kalın 3B yazı. Eski palet T20/T21'in turkuazı (#0B3A44, #1FA3B8 tonları).
- **Önem:**
  - **1:** göze batan, oyuncunun hep gördüğü;
  - **2:** fark edilen;
  - **3:** küçük.

## Bulgular

| Ekran | Sorun | Önem | Durum |
|---|---|---|---|
| Koleksiyon detayı | Eski turkuaz: panel kenarı, başlık plakası, isim bandı, "Tekrar oyna" düğmesi, tablo çerçevesi (t20 parçaları). Zeyd'in söylediği. | 1 | Düzeldi: Ayarlar'ın diliyle yeniden kuruldu. Krem panel, 3B başlık, kırmızı kapat, lacivert paspartu ve altın çerçeve, yeşil düğme. |
| Koleksiyon | Kartların altındaki "Bölüm N" ince krem yazı, aydınlık galeride okunmuyor. | 1 | Düzeldi: lacivert plaka üstünde kalın 3B yazı, altın kenar. |
| Koleksiyon | Kart çerçeveleri eski turkuaz (t20 portrait-frame). | 1 | Düzeldi: altın çerçeve; sıradaki tablo açık altın. |
| Koleksiyon | "Sıradaki" şeridi eski koyu turkuaz (#0B3A44). | 2 | Düzeldi: lacivert. |
| Koleksiyon | Arka planın "kayıyor, sıçrıyor" hissi. | 1 | Düzeldi (D2). Sayfalar tek parça kayıyor; asıl sebep ilk kaydırmadaki donma: her kart tam boy 1024² tabloyu indirip çözüyordu. Telefon hızında (Chrome, işlemci 6× yavaş; `Tools/pm107_kare.js`) aşağı kaydırmada en uzun kare 1000 ms idi, şimdi 100 ms. Kartlar 210×280 küçük resimden (16 KB, `paintings-v2/kucuk/`, `Tools/make_kucuk107.py`); karede en çok bir resim çözülüyor. |
| Menü üst çubuk | Oyuncu rozeti yalnız isim; Block Out'taki gibi bölüm bilgisi yok. | 2 | Düzeldi (D5): ismin altında "Bölüm N". |
| Menü üst çubuk | Altın sayacı kutu içinde; her ekranda başka bir altın görseli (menü yıldızlı, kazanma, devam, uçan altın). | 2 | Düzeldi (D4): Codex'in yapbozlu altını her yerde; menüde kutusuz altın + kalın sayı. |
| Alt sekme çubuğu | Seçili sekme düz koyu mavi kutu; derinlik az. | 2 | Düzeldi (D3): altın kenarlı krem seçili panel, sadeleşmiş ikonlar (Codex A, 24/25). |
| Başarılar | "Bombalı 10 bölümü ilk denemede kazan" iki satıra taşıp ilerleme çubuğuna değiyor. | 2 | Düzeldi: tek satır, gerekirse küçülür. |
| Booster ders kartı (Makas) | İpucu satırı 14 pt'de iki satıra sıkışıp çok küçük kalıyor. | 2 | Düzeldi: 16 pt, en çok 3 satır. |
| Süre doldu | "Hayır" düğmesi çok küçük görünüyor (dokunma alanı 44 pt, yeterli). | 3 | Bırakıldı (dokunma alanı doğru). |
| Süre doldu (`&devam=1` testi) | Panelin içinden alttaki kilit kartının yazısı görünüyor. | 3 | Yalnız test bayrağında: gerçek oyunda kart, süre dolmadan kapanmış oluyor. Bırakıldı. |
| Ayarlar | Uyumlu (T48). | – | – |
| Profil | Uyumlu (krem panel, altın halka, lacivert yazı). | – | – |
| Jiggles | Uyumlu. | – | – |
| Durdur | Uyumlu. | – | – |
| Kazanma, sandık | Uyumlu. Altın D4 ile yeni. | – | – |
| Engel kartları (kilit, zincir) | Uyumlu. | – | – |
| "Yeni tablolar yolda!" | Uyumlu. | – | – |
| Reklamla devam | Uyumlu (mavi "Reklam izle +30 sn"). | – | – |
| Yükleme ekranı | Uyumlu (PM_88). | – | – |
| Booster (saat, makas) | Booster'ın kendi sesinin altında eski "tık" da çalıyordu (bir basışa iki ses). | 2 | Düzeldi: booster'ın sesi varken eski tık çalmıyor. |

Önce/sonra görüntüleri `docs/art/pm107/once_sonra/` altında.
