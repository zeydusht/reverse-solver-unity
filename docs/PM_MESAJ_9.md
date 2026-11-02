# PM → Claude Code (25 Ekim 2026) — PM_MESAJ_8 BİTİNCE başla

Zeyd'in kararı: Oyundaki her görsel Codex'in stilinde olacak. İki stilin karışması kötü görünüyor.

Ona soru sorma. Token tasarrufu zorunlu: web araştırması yapma, bildiklerinle çalış.

## 1) Kalan "Claude çizimi" taraması (önce bu)

Oyunda ekrana gelen her şeyi tara: menü, oyun, kartlar, sonuç ekranları, booster'lar, sayaçlar, ikonlar, yükleme, isim ekranı. Kodla çizilmiş ya da Codex'e ait olmayan her görseli listele. Örnekler: durdurma tuşu, ok, X, yıldız, dairesel sayaç, ilerleme çubuğu, toast, gölge, kart köşeleri.

Listeyi `docs/art/ASSET_IHTIYAC.md` dosyasına **"Kalan Claude çizimleri"** başlığıyla yaz. Her satırda şunlar olsun:

- öğe
- ekran
- kod dosyası
- ölçü (pt ve @3x px)
- durumlar (normal / basılı / kapalı)
- 9-slice gerekiyor mu

## 2) Rakip karşılaştırması: klasik mobil bulmaca oyununda olup bizde olmayanlar

Referans oyunlar: Royal Match, Toon Blast, Gardenscapes. Şu ekranları karşılaştır: ana menü, üst bar, oyun ekranı HUD'u, kazanma ve kaybetme akışı, ayarlar, profil.

Bizde eksik olanları `docs/art/EKSIKLER.md` dosyasına tablo halinde yaz. Sütunlar:

- özellik
- rakipte nasıl
- bizde var mı
- **playtest öncesi mi, sonrası mı**
- iş tahmini

Örnekler:

- sol üstte profil kutusu (avatar = bir Jiggle, altında "Kurtarılan Jiggles: X")
- ayarlar dişlisi (ses, müzik, titreşim)
- üst bar düzeni
- kazanınca yıldız ve ödül animasyonu
- kaybedince "tekrar dene" akışı
- bölüm başlangıç kartı ("Level X, hedef")
- alt navigasyon

**Kapsam kuralı:** Playtest öncesine yalnız ucuz ve oynanışı etkilemeyen UI girer: profil kutusu, ayarlar, kazanma/kaybetme cilası, bölüm başlangıç kartı.

Şunlar **playtest sonrasına** yazılır, kod yazılmaz: coin, can (lives), mağaza, günlük ödül, etkinlikler. Para ve ekonomi sistemleri ayrı bir ürün kararıdır.

## 3) Codex'e görev yaz

1. ve 2. maddelerdeki çizimleri `docs/CODEX_GOREV.md` dosyasına "5. bölüm" olarak yaz. Görevler Codex'in tek okumada çizebileceği kadar net olmalı. Her kalemde şunlar olsun:

- ne
- nerede duracak
- şekil (kare / yuvarlak / 9-slice)
- içinde ne var (ör. "kare çerçeve, içinde turkuaz Jiggle yüzü, altında sayı alanı")
- ölçü (@3x)
- durumlar

## 4) Uygula

Playtest öncesi işaretlenen özellikleri kodla. Codex'in görselleri gelene kadar geçici bir görsel kullan, gelince değiştir. Önizlemeyi `?set=redesign`'da yenile; kök adres değişmez.

## 5) Rapor

BACKLOG'un Mühendislik bölümüne en fazla 5 satır yaz. Zeyd'e kısa bir özet ver:

- kaç "Claude çizimi" kaldı
- playtest öncesi eklenecek özellikler
- Codex'e gidecek görev (CODEX_GOREV 5. bölüm)
