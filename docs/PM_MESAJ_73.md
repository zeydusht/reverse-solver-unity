# PM_MESAJ_73: T50 seçimlerini bağla + kayma izi + nefes anı + 2 küçük hata (p22), sonra PM_71

Kodu kendin yaz. Yalnız p22. Önce `docs/` commit. Bitiş mesajı CLAUDE.md'deki kısa biçimde.

Zeyd seçimi (15 Kasım 21:36): **kazanma ekranı B**, **harita düğmeleri C**.

## 1. Kazanma ekranı: T50 seçenek B
`Assets/Art/_yeni/T50/kazanma/B/`. Kartsız; mavi ışıklı arka plan + altın konfeti, büyük mutlu Jigo, 3B "Harika!", +50 altın, 5 bölmeli sandık çubuğu, yeşil "Sonraki Seviye", mavi "Menü".
- **Düzelt:** "+50"deki artı işareti bozuk ("÷" gibi). Görsel değil, oyunun yazı tipiyle yaz: Baloo 2 ExtraBold, krem dolgu, mavi kontur.
- **Giriş animasyonu:** ışık huzmesi açılır → başlık 1,3×'ten 1×'e oturur → Jigo zıplar → altın sayılır → butonlar gelir. Toplam ≤ 1,2 sn, dokununca hemen tamamlansın.
- EN metinleri: "Great!", "Next Level", "Menu".
- Eski kazanma ekranı (T48) p22'de kalmasın.

## 2. Harita bölüm düğmeleri: T50 seçenek C (yapboz parçası)
`Assets/Art/_yeni/T50/harita/C/`.
- **Durumlar:** tamamlandı, şu anki, kilitli, zor (Kont mührü), ders (engel rozeti).
- Bölüm numarası oyunun yazı tipiyle.
- **Düzelt:** şu anki bölümün üstündeki Jigo işareti çok küçük. Düğmenin ~%70'i boyunda olsun ve hafifçe sallansın (bob, 1,5 sn).
- Rozetler (tik, kilit, Kont, engel) en az 22 px görünsün.
- Şu anki düğme hafif nefes alsın (1,0 ↔ 1,04, 2 sn).
- Yollar: altın noktalı çizgi.

## 3. Kayma izi (PM_72'de eklenmişti, yapılmadı)
Doğru yönde atılan parça tahtadan çıkarken arkasında kısa iz bıraksın:
- **Şekil:** parça genişliğinde, arkaya doğru incelen ve solan şerit (uçurtma kuyruğu). Rengi parçanın resminin baskın tonu, ucu altın kıvılcımlı. Ömrü ~0,18–0,25 sn.
- **Hız:** hızlı fiskede uzun, yavaş itişte kısa.
- **Özel durumlar:** takılmada iz yok. Bombalı parçada iz altın.
- **Sınırlar:** iz komşu parçaları kapatmasın, ekranda en çok 3 iz.
- **Bayraklar:** `&iz=serit` (varsayılan) ve `&iz=hayalet` (parçanın 2–3 solan kopyası). `ExitTrail` ayarları: uzunluk, opaklık, ömür.
- **GIF:** iki stilin kısa GIF'i `docs/art/task29/pm73/`.

## 4. Nefes anı (Zeyd'in fikri): önemli anlarda kayma bir an yavaşlasın
Kayma sırasında hareket kısa bir an yavaşlayıp devam etsin; göz önemli şeye gitsin.

PM kararı: **her çıkışta değil, yalnız önemli anlarda.** Normal çıkışlarda oyuncu art arda hızlı oynuyor, her seferinde yavaşlarsa oyun ağır hissettirir.
- **Ne zaman:**
  - bombalı parça çıkarken;
  - kilidi açan çıkış;
  - zinciri çözen çıkış;
  - **son parça**.
- **Nasıl:**
  - Parça tahtanın kenarını geçtiği anda parçanın hızı ~90 ms boyunca %20'ye iner.
  - Aynı anda kıvılcım ya da bomba sönme efekti patlar ve titreşim gelir.
  - Sonra hızlanıp çıkar.
  - Son parçada süre ~160 ms, ardından kazanma ekranı.
- **Kapsam:** yalnız o parçanın animasyonu yavaşlar. Oyun saati, girdi ve diğer parçalar durmaz; oyuncu arada başka parçaya dokunabilir.
- **Bayrak:** `&nefes=onemli` (varsayılan), `&nefes=hep` (her çıkışta 50 ms, Zeyd karşılaştırsın), `&nefes=yok`.
- **GIF:** bombalı parça ve son parça için önce/sonra GIF'i.

## 5. İki küçük hata (PM_72'de bulundu)
- **7. bölüm bomba kartı:** simge "2" gösteriyor, metin "16 hamle" diyor. İkisi de bombanın gerçek sayacını göstersin.
- **"Süre doldu" devam paneli:** başlık kurdele/afiş biçiminde. Kurdelesiz 3B yazıya çevir (PM_65 ölçütü).

## 6. Sonra PM_71
Yukarıdakiler bitip p22'ye çıktıktan sonra `docs/PM_MESAJ_71.md`'yi uygula: veriyle düzeltme + 8 bölüme ince ayar.

## Teslim
- p22 güncel, EditMode yeşil, `verify_build` p22 4/4.
- Önce/sonra görüntüleri ve GIF'ler `docs/art/task29/pm73/`.
- `ENTEGRASYON_RAPOR.md`'ye "PM_73" ve "PM_71" ekleri, `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa biçimde.
