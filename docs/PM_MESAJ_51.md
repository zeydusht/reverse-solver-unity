# PM_MESAJ_51 — Açılış videosu düzeltmeleri (Zeyd izledi, 9 Kasım)

Zeyd: yerleştirme ve akış güzel (özellikle C: düşüş + kahramanın kuşanması). Üç sorun var. p21'de düzelt, `Tools/intro/kurgu.py` parametreleriyle.

## 1. Sesler (en önemli: "inanılmaz kötü")
Sentetik yer tutucuları **tamamen kaldır**. Sırayla dene ve Zeyd'e karşılaştırma ver:
- **A) Kling'in kendi ses izi:** kliplerin içinde ses var (rapor: −22…−27 dB). Her klibin kendi sesini normalize edip kurguda kullan. Sahneyle birebir senkron olduğu için çoğu zaman en doğal sonuç bu olur.
- **B) Lisansı temiz hazır ses efektleri:** yalnız **CC0** ya da ticari kullanıma açık kaynaklar (Kenney.nl, Pixabay Sound Effects, Freesound'da yalnız CC0). Kapı, çuval hışırtısı, ayak sesi, fırça, mum sönmesi, cam tınısı, sihirli ışıltı. Her dosyanın kaynağı ve lisansı `docs/sounds/LISANS.md`'ye.
- **Müzik:** A bölümüne hafif, masalsı bir altyapı (CC0 ya da ticari kullanıma açık; pizzicato yaylılar / celesta tarzı). Gölgede gerilime döner, 8'de neredeyse sessizlik, C'de neşeli kapanış. Müzik efektlerin altında kalsın (yaklaşık −18 dB).
- "weee" ve oyun içi seslerimiz kalır.
- Teslim: `intro_ses_A.mp4` (Kling sesi) ve `intro_ses_B.mp4` (kütüphane + müzik), Zeyd seçsin. Karışık da olabilir.

## 2. 1 → 2 geçişi ("baştan başlıyormuş gibi")
Klip 1 Kling'de kamerayı ileri itiyor, klip 2 ise geniş kadrajdan başlıyor. Bu yüzden 2 başa sarmış gibi görünüyor.
- Klip 1'i **ilk ~3 sn** olarak kullan (kırpma 0 → 3, en canlı kısım).
- Klip 1'in son karesine kadraj olarak **en çok benzeyen** klip 2 karesini otomatik bul (kare benzerliği), 2'yi oradan başlat. Arada 0,4–0,5 sn çapraz geçiş.
- Benzer kare yoksa: 1'in sonunda hafif yakınlaşma, 2'de aynı ölçekten başlayan dijital zoom ile eşle. Gölgenin gelişi geçişi örter.
- A'nın toplamı yine 18–21 sn.

## 3. 1. bölümün resmi
Eski 1. bölüm resmine **geri dön**. Tablo #1 zorunlu değil. 9 → tahta geçişi 0,3 sn kararma ile kalsın.

## Teslim
p21 güncel + `docs/art/task26/video/intro_onizleme_v2.mp4` (sesli) + iki ses seçeneği. `KURGU_RAPOR.md`'ye kısa ek. Zeyd telefondan izleyecek.
