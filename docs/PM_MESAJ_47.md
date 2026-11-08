# PM_MESAJ_47 — Hikâye eğrisi ölçümü (Bölüm Tasarım Kitabı, Adım 1)

Önce `docs/BOLUM_TASARIM_KITABI.md`'nin **0, C ve F** bölümlerini oku. Bu iş yalnız **ölçüm/analiz**: oyun koduna, p20'ye, köke ve seviye dosyalarına dokunma.

## Amaç
Her bölümün "hikâyesini" sayıyla görmek: çözüm boyunca oyuncunun kaç anlamlı seçeneği var, nerede tek parçaya sıkışıyor, ne zaman açılıyor. Kitaptaki K1–K7, K10 eşiklerini SET40 v4 üzerinde kalibre edeceğiz.

## Yapılacaklar
1. **`Tools/hikaye_egrisi.py`** (fabrikanın mevcut çözücüsünü kullan, yeni çözücü yazma). Her bölüm için çözüm yolu boyunca, her adımda:
   - `secenek`: o anda çıkarılabilen VE çıkarılınca bölüm hâlâ çözülebilir kalan parça sayısı.
   - `tuzak`: çıkarılabilen ama çözümü kilitleyen parça sayısı.
   - Engel durumu: bomba fitil payı (patlamaya kalan − o ana kadarki adım), zincir/çivi açık mı.
   Çözüm yolu olarak ZeydBot'un tercih sırasını kullan. Yoksa çözücünün ilk çözümünü kullan ve hangisi olduğunu yaz.
2. Bölüm başına özet metrikler → `docs/analiz/hikaye_egrisi_set40.csv`:
   `bolum, adim_sayisi, ilk3_min_secenek, ilk_hamle_tuzak_orani, bogaz_sayisi (secenek=1 olan ardışık ≥2 adımlık bloklar), en_uzun_bogaz, anahtar_adim (seçeneğin en çok sıçradığı adım, % olarak), sicrama_kat, cig_orani (son %30 parçadaki ortalama seçenek), bomba_min_pay, bomba_tehdit_adimi (pay ≤2 olduğu ilk adım), egri_imzasi (eğri 10 noktaya örneklenmiş)`.
3. **Benzerlik:** `level_similarity.py`'ye eğri imzası benzerliği ekle (ör. normalize Öklid). En çok birbirine benzeyen 10 bölüm çiftini listele.
4. **Rapor → `docs/analiz/HIKAYE_EGRISI_RAPOR.md`** (kısa, Türkçe):
   - 40 bölümün tek sayfalık küçük grafikleri (`docs/analiz/hikaye_egrisi_set40.html`, her bölüm için mini çizgi; kalın çizgi = seçenek, kırmızı nokta = tuzak).
   - Kitaptaki K2, K3, K4, K5 kurallarını ve bomba tanıtımı kontrolünü (D tarifi: tehdit ilk N hamle + 2 içinde mi) **geçen/kalan** tablosu.
   - Özellikle 3, 6, 10, 14 (tanıtım) ve Zeyd'in favorisi Z12 için 2'şer cümle yorum.
   - Eşik önerisi: SET40'ın dağılımına göre K2–K5 için önerilen sayılar.
5. Bittiğinde `docs/PROJE_DURUMU.md` "Açık işler 6"ya tek satır sonuç ekle.

## Kabul
- CSV 40 satır, HTML tarayıcıda açılıyor.
- Rapor en fazla 1,5 sayfa.
- Hiçbir seviye/oyun dosyası değişmemiş (`git status` ile göster).
