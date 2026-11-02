# PM → Claude Code (25 Ekim 2026, 13:35) — Zeyd'in önizleme 5 geri bildirimi

Raporda "Claude çizimi kalmadı" yazıyor ama bu doğru değil. Tahtadaki parçalar hâlâ eski prosedürel çizimde. Zincir ve duvar da Zeyd'in beklediği gibi değil.

Bundan sonra çizimleri Zeyd, Codex uygulamasına kendisi verecek. Sen yalnız **kod** tarafını yap. Codex'in yeni görselleri `docs/art/HANDOFF.md`'ye düştükçe onları oyuna bağla.

Soru sorma. Token tasarrufu zorunlu.

## Şimdi yap (görsel beklemeden)

1. **Engel boyutu.**
   - Çivi ve bomba hücrenin yaklaşık %80'ini kaplasın. Şu an küçük kalıyorlar.
2. **Sayaç yeri.**
   - Kalan hamle sayısını görselin TAM ÜSTÜNE yaz:
     - Çivide: çivi başının ortasına.
     - Bombada: bombanın ortasındaki yuvarlak alana.
   - Rakam büyük ve kalın olsun, koyu bir dış çizgisi olsun. Hücrenin yaklaşık %40'ı yüksekliğinde.
   - Ayrı küçük rozet daireyi kaldır.
3. **Katman sırası.** Alttan üste şöyle olmalı:
   - Parça
   - Zincir çizgisi
   - Engel (çivi/bomba)
   - Sayaç

   Zincir bir engelin üstünden geçmesin, altından geçsin.
4. **Durdurma ve ana menü.**
   - Oyun içinde bir durdurma tuşu olsun. Mevcut pause görselini kullan.
   - Tuşa basınca açılan kartta şu seçenekler olsun: "Devam", "Yeniden başla", "Ana menü".
   - Ana menüye dönünce bölüm sayılmasın; telemetriye "quit" olarak düşsün.
5. **Duvar okunurluğu.**
   - Codex'in kırmızı-beyaz şeridi gelene kadar duvarı geçici olarak kırmızı-beyaz şeritle çiz. Eski haldeki gibi.
6. Önizleme 6'yı `?set=redesign` adresine yayınla. BACKLOG Mühendislik bölümüne 2 satır ekle.

## Codex görselleri gelince

- **Yapboz parçası seti.** 81 kombinasyon gelecek: şekil maskesi + kenar overlay + düz dolgu, `piece_O-F-I-F` adlandırması. Parçaları bu sprite'larla çiz:
  - Tablo resmi maskeyle kırpılsın.
  - Overlay en üste gelsin.
  - Prosedürel çizim yedek olarak kalsın.
  - Çok hücreli parça varsa HANDOFF'taki notu uygula.
- **Çivi başı, bomba, zincir, duvar ve pause kartı:** yeni görsellerle değiştir. Yukarıdaki boyut, sayaç ve katman kuralları aynen geçerli.
