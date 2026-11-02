# PM → Claude Code (1 Kasım 2026) — p20 menü revizyonu (Zeyd telefonda oynadı)

Yayın: `p20/`. `p19/` (pt1) donduruldu, kök ve iOS dokunulmaz. Veri kaydı kuralları (PM_25/26) aynen geçerli.

İki bölüm var:
- **A:** şimdi, mevcut görsellerle.
- **B:** Codex Task21 parçaları gelince (`Assets/Art/Menu/Task21/`). Klasör yoksa A'yı bitir, B'yi bekle.

## A. Şimdi yap

### A1. Yerleşim (Ev ekranı ve tüm sekmeler)
- **Alt sekme çubuğu ekranın en altına yapışık.** Şu an yukarıda duruyor ve altında resim görünüyor.
  - Çubuğun zemini ekranın alt kenarına kadar insin.
  - İkonlar `safe-area-inset-bottom` kadar yukarıda kalsın.
  - Telefonda tarayıcı adres çubuğu olsa da olmasa da doğru dursun: `100dvh`/visualViewport, iOS Safari ve Android Chrome. Masaüstü 9:16 alanında da en altta.
- **Üst bar en üstte.** Profil, sayaç ve dişli `safe-area-inset-top` + 8 pt'den başlasın, şu an çok aşağıda.
- **Level tuşu daha aşağıda.** Alt çubuğun hemen üstünde, arada ~16 pt.
- Ortadaki tablo kazanılan alanla büyüsün (oranı korunarak).

### A2. Kaydırma (Koleksiyon, Başarılar, Bölümler, Jiggles)
Şu an parmakla sürükleyince kayıyor, ama fırlatınca (flick) devam etmiyor. Telefon hissi gerek:
- **Atalet:** Parmak bırakılınca hızla devam etsin, yavaşlayarak dursun.
  - Hız parmağın son ~80 ms'deki hızından alınır.
  - Yavaşlama: Unity ScrollRect `decelerationRate ≈ 0,135`, ya da eşdeğeri: güçlü bir fırlatma ~0,8–1,2 sn kaysın.
- **Kenar esnemesi:** Uçlarda hafif esneyip geri gelsin (elastic, ~0,1).
- Kaydırırken dokunuş tıklama sayılmasın. Ancak parmak 10 pt'den az hareket ettiyse tıklamadır.
- WebGL'de dokunma ve fare tekerleği ikisi de çalışsın. 60 FPS'i koru.
- Test: telefonda dokunmatik ile (`verify_build` dokunma simülasyonu) hızlı fırlatma → liste en az 1 ekran boyu ilerlemeli.

### A3. Yazı stili: "Level X" ve başlıklar (Royal Match gibi okunur)
Referans: `Assets/Art/References/WhatsApp Image 2026-10-23 at 18.34.52 (8).jpeg` → "Level 3469" tuşu. Bizim "Level 13" soluk.

- **Font:** Türkçe harfleri (ğ ş İ ı ç ö ü Ğ Ş Ç Ö Ü) eksiksiz olan, kalın ve yuvarlak bir **OFL** font bul.
  - Adaylar: Titan One, Baloo 2 ExtraBold, Lilita One (ğ ş İ yok, eleme), Fredoka Bold (şu an).
  - 3 adayı aynı "Level 13 / Bölüm 13 / Başarılar / ĞİŞ" örneğiyle `docs/qa/font-karsilastirma.png`'ye koy, glif kontrolünü yaz.
  - **En kalın ve en okunur olanı varsayılan yap**, Zeyd sonra değiştirebilir.
- **Stil (TextMeshPro malzemesi):**
  - Dolgu: krem-beyazdan açık sarıya dikey degrade (`#FFFFFF` → `#FFF1B0`).
  - **Kalın koyu kontur:** yeşil tuşta `#1E4D05`, turkuaz ve lacivert zeminde `#0B2A36`, yazı boyunun ~%12'si.
  - Altta koyu gölge: 2–3 pt aşağı, %50 opak.
- Bu stil şunlara uygulanır: Level tuşu, ekran başlıkları (pankartlar), seçili sekme etiketi, sayaç sayıları, "X Jiggle kurtarıldı" yazısı, kart tuşları.
- Krem kart içindeki küçük açıklama yazıları kontursuz lacivert kalsın.

### A4. Jiggles ekranı
- **Renk sayaçları kalksın.** Raftaki 5 Jiggle'ın altındaki sayılar (60, 64…) oyuncuya anlamsız geldi, `(slot−1) mod 5` kuralı görünmüyor. Raf yalnız 5 Jiggle'ı göstersin, sayı yok.
- **"313 Jiggle kurtarıldı":** A3 stiliyle, ortada, daha büyük (Jiggle ikonu solda). Okunsun.
- "Avatarım" seçimi B'de yeni halkalarla değişecek. Şimdilik avatarı halkanın içine **%78** sığdır ve maskele, taşmasın.

### A5. Ayarlar
Yazılar A3 fontu ile. Gizlilik ve Bize yaz tuşları B'de değişecek.

### A6. Hata ayıklama yazısı
Ekran görüntülerinde sol üstte "menü FPS 60 sürüklerken medyan…" yazısı görünüyor. Normal açılışta **asla** görünmesin (yalnız `?debug=1`). Canlıda kontrol et.

## B. Codex Task21 gelince
- **Alt çubuk v3:** Yazısız ikonlar, yalnız seçili sekmenin etiketi (A3 stili). Seçili ikon büyür, hafif yukarı taşar (0,15 sn tween).
- **Sol üst:** kare avatar çerçevesi + isim plaketi (isme göre genişler) + yeni sayaç hapı.
- **Avatarım:** yeni halkalar, maske, seçili altın halka + tik.
- **Kilometre taşları:** 3 madalyon + noktalı yol (yatay çubuk kalkar).
- **Ayarlar:** krem satırlar, yan yana ikonlu Gizlilik ve Bize yaz tuşları.

## Kontrol
- **Ekran görüntüleri:** `docs/qa/2026-11-01-p20b/`, 375×667, 390×844, 430×932, TR/EN.
  - Alt çubuk en altta, altında resim yok.
  - Üst bar en üstte.
  - Hata ayıklama yazısı yok.
- **Kaydırma:** Fırlatma testinin sayıları (kaç piksel, kaç sn) OVERNIGHT'a.
- **Veri:** `?c=ic` canlı veri kontrolü, `app_version` `web-p20-…`.
- OVERNIGHT.md başına: p20 linki + A/B durumu + sorunlar.
