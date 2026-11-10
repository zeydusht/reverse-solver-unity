# Codex Task20 — Menü ekranları: üretim parçalarının export'u (Task19D onaylandı)

Zeyd Task19D'nin 9 ekranını onayladı. Şimdi oyuna bağlanabilecek parçaları çıkarıyoruz. Kaynak liste: `docs/art/TASK19_PARCALAR.md`.

## Önce 3 küçük düzeltme (kaynak ekranlarda da düzelt)

1. **07 Bölümler:**
   - 10. bölüm tablo işareti 12'nin yanında "10" yazısıyla duruyor. Bu, iki tane 10 varmış gibi görünüyor. Tablo işaretini 10 numaralı düğümün yanına taşı, altına numara yazma.
   - 19 numaralı zor bölüm kilitli olmalı (şu anki bölüm 12). Kırmızı kenar + Baron yüzü + kilit birlikte görünsün.
2. **04 Başarılar:** "10 bombalı bölümü ilk denemede geç" sağ kenara değiyor. Metin için sağda en az 24 px boşluk bırak, gerekirse yazıyı küçült.
3. **03 Jiggles:** "Avatarım" başlığı avatar halkalarına değiyor. Arada en az 16 px boşluk olsun.

## Export kuralları (Task14/15 ile aynı)

- **Konum:** `Assets/Art/Menu/Task20/` altına şeffaf @3x PNG'ler.
- **Yazı yok:** yazılar ve sayılar resmin içinde olmasın. Oyun bunları kendisi yazacak. Yalnız ikonlar ve süsler resimde kalır.
- **Basılı hal:** her tuş ve sekme için normal + basılı (`-pressed`).
- **9-slice:** paneller, karolar, ilerleme çubukları, sekme yuvası ve pankart başlık genişleyebilmeli. Kenar ölçülerini manifestte yaz.
- **Tablo hücreleri:** tablo görseli resmin içine gömülmesin. Çerçeve ortası şeffaf, oyun tabloyu altına koyacak. Kırpma oranı 3:4.
- **Arka plan sahneleri:** Ev (mevcut koridor), Koleksiyon (galeri), Jiggles (raf odası), Başarılar (kupa salonu), Bölümler (merdiven ve halı).
  - Panel olmadan tam sahne, en uzun kenar en fazla 2048 px (STYLE_CARD).
  - Bölümler yolu dikey tekrar eden bir parça olsun: `levels-path-repeat`, alt-üst kenarları birleşecek.
- **Alt sekme çubuğu:**
  - Çubuk zemini (9-slice)
  - Yuva normal / seçili
  - 4 ikon × (normal, seçili)
  - Bildirim rozeti
- **Düğümler:** biten, şu anki, kilitli, zor-kilitli, zor-açık ve parlama halkası. Hepsi sayısız; numarayı oyun yazacak.
- **Ayarlar:** anahtar aç/kapa (zemin + topuz ayrı), Türkçe|English seçici (sol seçili / sağ seçili).
- **Manifest:** `manifest.json` ve `HANDOFF.md`'ye her dosya için bir satır: dosya, oyunda nerede, ölçü, 9-slice kenarları, pivot.
- **Önizleme:** `preview-kontak.png` = parçalardan yeniden birleştirilmiş 9 ekran. Task19D ekranlarıyla aynı görünmeli, fark olursa nedenini yaz.

## Yapma
- C#, `Assets/_Game`, sahne ve prefab dosyalarına dokunma. Bağlamayı Claude Code yapacak.
- Onaylı Task13/14/15 dosyalarının üstüne yazma. Yeni dosya adı kullan.

Bitince 3 satır yaz: kaç parça çıktı, manifest yeri, önizleme farkı var mı.
