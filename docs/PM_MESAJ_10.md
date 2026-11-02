# PM → Claude Code (25 Ekim 2026, 07:45): Kalan Claude çizimlerini bitir

Zeyd'in isteği: Oyunda Claude çizimi kalmasın. Kalan ~8 öğeyi hemen Codex'e çizdir ve oyuna bağla. Soru sorma, token tasarruflu çalış.

1. **Codex'e ver.** Kalan öğeleri tek bir `codex exec` göreviyle (`-s workspace-write`) Codex'e çizdir. Ölçüleri ve durumları sen ver.
   - **Tahtadaki zincir:** İki parça arasında uzayabilen, tekrar eden halka sprite'ı.
   - **Tahtadaki duvar:** Şerit boyunca tekrar eden sprite. Kapalı şeritte okunur olmalı.
   - **Booster'ın eksik halleri:** Kilitli, seçili/aktif, sayı rozeti, sayısı 0.
   - **Bölüm listesi:** Hücreler (tamamlanan, kilitli, sıradaki), yıldızlar, çerçeve. Kaynak #20.
   - **Parça dokusu/skin:** Zeyd'in onayı alınmadan "bitti" işaretlenmiş. Mevcut haliyle bırak; Zeyd önizleme 4'te bakacak. Kodda Codex skin'i kullanılmıyorsa bağla.
   - **Listede adı geçmeyen ama hâlâ kodla çizilen her şey.** Taramayı tekrar yap, bulduklarını aynı göreve ekle.
2. **Bağla ve yayınla.** Gelen görselleri oyuna bağla. Önizleme 5'i `?set=redesign` adresine yayınla; kök adres değişmeyecek.
3. **Kapanış:**
   - BACKLOG Mühendislik bölümüne tek satır yaz: "Claude çizimi kalmadı" ya da kalanların listesi ve kalma nedeni.
   - OVERNIGHT.md başına önizleme linkini yaz (en fazla 4 satır).
