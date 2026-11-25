# PM_107 gece raporu (25 Kasım)

p23'te. `live/`'a yayın yok. Supabase'e yazılmadı (`verify_build --send` bu gece yapılmadı).

## A. Ses seçimleri
- **ses.json:** Zeyd'in tablosu yazıldı:
  - kazanma tınısı pizz;
  - çıkış u;
  - çekiç c;
  - saat b;
  - çizgi c;
  - düğme c;
  - zincir f, zincir gerilme f;
  - geri kalanı aynı.
- **Dinleme sayfası:** Varsayılanlar (`KEYS`) aynı tabloya getirildi. "Bugünkü seçim" oyunun kendi dosyalarını çalıyor (miksaj kopyaları).
- **Lisanslar:** `SES_LISANSLARI.md` güncel: pizz (VSCO 2 CE + VCSL), çıkış u, zincir f, düğme c, çekiç c, saat b, çizgi c.
- **iOS ve web:**
  - iOS: yalnız seçilenlerin miksaj kopyaları build'e giriyor (`IosBuild.Unused`).
  - Web: PM_106 adaylarının ham örnekleri artık taşınmıyor.
  - Veri 37,93'ten 37,79 MB'a indi.

## B. Zincir sesi kayarken de
- **Sebep:**
  - Çıkan zincirli parçanın zinciri, çıkış başında ölçümden siliniyordu.
  - Bölüm başındaki 3 sn'lik sessizlik ilk sürüklemede de sürüyordu.
  - Kazanılan son hamlede (oturum bitince) şıkırtı kesiliyordu.
- **Düzeltme:**
  - Çıkan parçanın zinciri, parça yok olana kadar ölçülüyor.
  - Sessizlik ilk sürüklemede bitiyor.
  - Son çıkış da şıkırdıyor.
  - Takılmada gerilme sesi (f) ayrıca çalıyor.

## C. Miksaj
Ayrıntı: `MIKS_RAPORU.md`.

- **Kanallar:** Mix107: müzik, efekt, arayüz, zincir, saat, fanfar.
- **Kısma:**
  - Saat çalışırken efektler −5 dB, müzik −3 dB.
  - Fanfarda efektler −6 dB.
  - Patlamada müzik −4 dB.
- **Ses sınırı:** Aynı ses en çok 2 kez üst üste.
- **Sınırlayıcı:** Web'de ana çıkışta.
- **Dosyalar:** 42 seçili dosyanın miksaj kopyası. Tik-tak −29,6'dan −21,5 LUFS'e, dalış −33'ten −24'e çıktı. Kayma ve çıkışın tizi yumuşadı.
- **Dalış:** Menüden Oyna'da hava sesi yaklaşmanın başından başlıyor, kamera hızıyla yükseliyor, varışta sönüyor. Kazanma → koridor → dalış da öyle.
- **7 senaryo kaydı (önce → sonra):**
  - Duyulmayan ses: saatte 11 → 0. Öteki senaryolarda zaten 0'dı, 0 kaldı.
  - Tepe aşımı: bölümde 2 → 0, zincirde 1 → 0, bombada 2 → 0, kazanmada 2 → 0, menüde 1 → 0.
  - Kayıtlar ve seviye grafikleri `miks/`'te.

## D. Görsel uyum
Ayrıntı: `DENETIM.md`; önce/sonra görüntüleri `once_sonra/`'da.

- **D1 denetim:** 19 ekran gezildi. Bulgular önemleriyle listelendi.
- **D2 koleksiyon:**
  - **Detay:** Yeni paletle yeniden kuruldu: krem panel, 3B başlık, altın çerçeve, yeşil düğme. Turkuaz kalmadı.
  - **Kartlar:** "Bölüm N" lacivert plaka üstünde kalın 3B yazı; çerçeveler altın.
  - **Sıçrama:** İşlemci telefon hızına indirilince, ilk kaydırmada 1 sn'lik donma ölçüldü. Sebep: her kart tam boy 1024² tabloyu çözüyordu. Düzeltme: 210×280 küçük resimler (16 KB; gh-pages `paintings-v2/kucuk/`) ve karede en çok bir resim çözme. Sonra en uzun kare 100 ms (`kare_sonra.json`).
- **D3 alt çubuk:**
  - Codex 3 seçenek üretti (PM_74). A (krem galeri) iyileştirildi, 24/25 puan.
  - C elendi: sadelik 3, seçili sekmenin anlaşılması 3.
  - Seçili sekme altın kenarlı krem panel, ikonlar sadeleşti.
  - `codex/alt_cubuk/`.
- **D4 altın:**
  - Codex 3 seçenek üretti. Final, yapboz kabartmalı altın (18/20). C elendi: 24 pt okunurluk 3.
  - Her yerde aynı altın: menü, kazanma, uçan altın, devam paneli, sandık.
  - Menüde sayaç kutusu yok: altın + kalın sayı.
  - `codex/altin/`.
- **D5 rozet:** Menünün sol üstünde Jiggle avatarı, isim ve altında "Bölüm N".
- **D6 kendi bulduklarım:**
  - Booster ders kartının ipucu satırı büyüdü (14 → 16 pt, 3 satır).
  - Başarılar'da uzun koşul ilerleme çubuğuna taşmıyor (tek satır, gerekirse küçülür).
  - Saat ve makasta, booster'ın kendi sesinin altında eski tık çalıyordu (bir basışa iki ses); kalktı.
  - Jiggles'ta seçili olmayan avatarlar eski turkuaz diskteydi; lacivert disk ve altın halka.
  - Kazanma ve sandık ekranındaki altın sayacının kutusu kalktı; menüdeki gibi altın + kalın sayı.
  - "Süre doldu" panelinde "Hayır" düğmesi altın satırına biniyordu; ayrıldı.

## Dokunulmayanlar (Zeyd onaylı)
Dalış ve koridorun görüntüsü, çerçeve, lastik, kazanma ekranı yerleşimi, HUD.

## Doğrulanmayan
- **iPhone:** Hoparlörde dinleme ve iPhone'da koleksiyon kaydırma hissi yapılmadı. İşlemci yavaşlatılarak (6×) ölçüldü.
- **Sandık ekranı:** Sesi ayrıca kaydedilmedi; nedeni `MIKS_RAPORU.md`'de.
