# PM → Claude Code (26 Ekim 2026, 00:45) — Önizleme 10 geri bildirimi

Zeyd sayıları beğendi. Ona soru sorma. Token tasarrufu zorunlu.

## Şimdi yap (kod)

1. **Bomba:** Gövdeyi %10 küçült. Hücrenin yaklaşık %85'i yerine %76'sı olsun.

2. **Duvar:** Eski kalın kırmızı-beyaz şeride geri dön. Codex tile'ı yerine kodla çiz:
   - Şerit kalınlığı hücrenin yaklaşık %20'si.
   - Kalın, seyrek çapraz çizgiler: kırmızı #D9332B, beyaz #FFFFFF.
   - İnce koyu kontur.
   - Kenar boyunca kesintisiz uzansın, köşelerde düzgün birleşsin.

3. **Booster çubuğu** (`Assets/Art/Reference/booster-bar-layout-reference.jpg`, YALNIZ yerleşim referansı):
   - Tuşların altındaki yazıları (MAKAS, DEĞNEK, ÇEKİÇ, SAAT) kaldır.
   - Adet rozeti tuşun **sağ altına** gelsin. Rakam, sayaçlardaki Fredoka fontuyla, beyaz ve kalın yazılsın. Rozet kırmızı, 2 px açık kontur.
   - İkon yuvarlak tuşun içini doldursun (tuşun ~%80'i), taşmasın.
   - Codex'in yeni booster tuşu ve ikonları gelince onları kullan.

4. **Üst bar (HUD) hizası:**
   - Mevcut parçalar kaymış. LV yazısı rozetten taşıyor. Hap içindeki asimetrik "kalkan" yuvası ikonla hizalı değil.
   - Geçici çözüm: yuvayı kullanma, ikonu hap içinde solda ortala.
   - Üç öğenin (rozet, iki hap, durdur) dikey merkezi aynı çizgide olsun, aralarındaki boşluk eşit olsun.
   - Durdur tuşu iyi, aynen kalsın.
   - Codex'in HUD v3'ü gelince onunla değiştir.

5. Önizleme 11'i `?set=redesign` adresine yayınla. BACKLOG'a 2 satır ekle.

## Codex'ten gelecek (Zeyd iletiyor)

`Assets/Art/Gameplay/Task8/` klasörüne gelecek dosyalar:

- `booster-button.png`
- `booster-badge.png`
- `booster-scissors.png`, `booster-wand.png`, `booster-hammer.png`, `booster-clock.png`
- `hud-level-badge-v3.png`
- `hud-pill-v3.png`

Gelince bağla ve HANDOFF'a oyun adlarıyla ekle.
