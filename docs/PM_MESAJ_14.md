# PM → Claude Code (26 Ekim 2026, 02:00) — GECE TURU: playtest'e hazır son hal

Zeyd uyuyor, sabah bakacak. Soru sorma; belirsiz durumda makul seçimi yap ve raporda yaz.

Codex de bu gece sanat üzerinde çalışıyor; Zeyd ona ayrıca görev verdi. Teslimler `Assets/Art/Gameplay/Task11/` klasörüne ve HANDOFF.md'ye düşecek. Sen yalnız kod tarafını yap; Codex'in klasörlerine yazma.

Token tasarrufu zorunlu:
- Büyük dosyaları baştan okuma.
- En fazla 1 alt ajan kullan.
- Uzun işleri arka planda çalıştır.
- Her adımdan sonra commit at ve BACKLOG Mühendislik bölümünü güncelle.

## Sıra

1. **Booster seti (Task10).**
   - `booster-button`, 4 ikon ve `booster-badge`'i bağla. İkon tuşun yaklaşık %85'ini kaplasın.
   - Altta yazı olmasın.
   - Rozet sağ altta dursun; rakam Fredoka, beyaz, kalın.
   - Kilitli hal: gri ton + kilit ikonu. Sıfır hal: soluk.
   - Task8 ve Task9'u kullanma.

2. **PM_MESAJ_13'ün kalanı** (yapılmadıysa):
   - Bombayı %10 küçült.
   - Duvarı kalın kırmızı-beyaz şeritle, kodla çiz.
   - Üst bar hizasını düzelt.

3. **Codex Task11 geldikçe bağla.** Kontrolü gece boyunca, işler arasında yap.
   - Gelecekler: HUD v3 (level rozeti, haplar, ikonlar), menü "Level X" tuşu, pause kartı tuşları, kazan/kaybet kartı tuşları.
   - Geçici çizimleri kaldır.

4. **QA turu.** Oyunu playtest'e hazır hale getir.
   - **40 bölüm:** Kayıtlı çözümle hepsinin kazanılabildiğini otomatik doğrula. Tutorial, tanıtım, zor uyarısı ve booster tanıtımlarının doğru slotlarda çıktığını kontrol et.
   - **Ekran düzeni:** Üç telefon boyutunda ekran görüntüsü al: 375×667, 390×844, 430×932.
     - Ekranlar: menü, bölüm başlangıcı, oyun, pause, kazanma, kaybetme, bölüm listesi, profil, ayarlar.
     - Taşan, üst üste binen ya da kesilen öğeyi düzelt.
     - Görüntüleri `docs/qa/2026-10-26/` altına kaydet.
   - **Kalan Claude çizimi taraması:** Kodla çizilmiş görünür öğe kaldıysa listele. Bunları Codex'e ayrı görev olarak `docs/CODEX_GOREV.md`'ye yaz; Zeyd sabah iletecek.
   - **Telemetri uçtan uca testi** (TEST_ oyuncu adıyla):
     - plays
     - moves
     - 1–10 puan (kayıpta da)
     - 👍/👎
     - pause → "Ana menü"nün quit olarak düşmesi
   - **Performans:** WebGL build boyutu ve ilk açılış süresi. Doku sıkıştırması ve atlas düzenini kontrol et; 2048 px üstü doku olmasın. Hedef iPhone'da 60 FPS. Ölçüm makinede yapılamıyorsa bunu not et.

5. **Build 5 hazırlığı.** YAYINLAMA.
   - Kök adres yeni seti göstersin; `?set=classic` eski seti açsın; 👍/👎 açık; kayıt açık.
   - Yerel doğrulamayı yap.
   - `docs/sql/008_playtest_analiz.sql` hazır mı, kontrol et. Supabase'de ÇALIŞTIRMA.

6. **Sabah raporu.** OVERNIGHT.md başına en fazla 10 satır:
   - son önizleme linki
   - QA sonucu (kaç sorun bulundu, kaçı düzeldi)
   - kalan sorunlar
   - Zeyd'in sabah yapacakları: 1) önizlemeyi oyna, 2) "Build 5'i yayınla" kararı, 3) iPhone testi + 4G testi

Dokunma:
- canlı kök adres
- Supabase şeması
- seviye tasarımı (40'lık set onaylı)
