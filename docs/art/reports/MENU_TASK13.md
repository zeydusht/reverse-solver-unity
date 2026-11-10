# Task13 teslim ve kontrol kaydı

26 Ekim 2026 — `art` dalı. Teslim klasörü: `Assets/Art/Menu/Task13/`.

- 28 üretim sprite'ı: tamamı belirtilen ölçülerde, köşe alfa kontrolü 0, yazı/sayı gömülmemiş şeffaf PNG.
- 24 kaynak PNG: 22 image_gen üretimi; iki kaynak takım tutarlılığı için avatar/sayaç çiziminden tekrar kullanıldı. Çerçeve+portre birleşimleri ayrıca verildi.
- 6 inceleme PNG'si: 390×844 menü, 1170×2532 @3x menü, toplu set, üst bar, gezinme, kontrol/durum sayfaları. Telefon kompozisyonu ve toplu sayfa görsel olarak incelendi.
- Onaylı Level normal/basılı kopyaları canonical v2 ile SHA-256 bakımından aynı; onaylı koridor ve karakter kaynakları korunuyor.
- index.html içindeki statik yerel dosya bağlantılarında eksik yok; font kaynağı mevcut. Galeride manifest ile aynı 28 dosya tanımlı.
- Otomatik tarayıcı ekran görüntüsü bu ortamda Chrome GPU/renderer başlatma sorunu nedeniyle alınamadı. Bu nedenle HTML etkileşimlerinin tarayıcı üzerinden otomatik doğrulandığı iddia edilmez. Teslim PNG'leri GDI kompozisyonlarıdır; canlı oyun ekran görüntüsü değildir. Deneme için açılan tarayıcı ve geçici profil kapatılıp temizlendi.
- HANDOFF, ASSET_IHTIYAC dosya/grup durumları, STYLE_CARD, REFERANS_DURUMU, DECISIONS ve BACKLOG'un Art bölümü güncel. Yeni set Zeyd değerlendirmesinde; Level v2 zaten onaylı.

Yerleşik image_gen prompt seti: `docs/art/templates/menu-task13-prompts.md`. Oyun koduna entegrasyon bu teslimin dışında; `Assets/_Game` dosyaları değiştirilmedi.
