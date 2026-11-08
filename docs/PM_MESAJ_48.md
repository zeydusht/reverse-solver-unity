# PM_MESAJ_48 — Bölüm ajanı altyapısı + ilk dikey dilim (bomba yayı 6–7)

Önce `docs/BOLUM_TASARIM_KITABI.md` (v0.2: özellikle 0, C, D, D2, F) ve `docs/analiz/HIKAYE_EGRISI_RAPOR.md`'yi oku.
**Dokunma:** p20, kök, `levels/redesign.json`, pt1 verisi. Her şey p21'de ve ayrı bir sette.

## Amaç
Bölüm tasarım ajanını uçtan uca bir kez çalıştırmak: kart → bölüm → ölçüm → eleştiri → düzeltme → Zeyd'e oynanabilir aday. İlk iş, bomba tanıtım yayını (bölüm 6 = Tanıt, 7 = Geliştir) kitaptaki D tarifine göre yeniden yapmak.

## 1. Kart dosyası (makinece kontrol edilebilir)
`Tools/ajan/kart_sema.json` + örnek `docs/bolum_kartlari/`. Kart = kitaptaki şablon (bölüm 0) + ölçülebilir iddialar:
`tip, sure_sn, anahtar, ilk3_min_secenek, en_uzun_bogaz_min/max, bogaz_sayisi_min, cig_orani_min (göreli), bomba_tehdit_adimi_max, bomba_fitil_max, yeni_oge, sekil_notu, beklenen_oyuncu (metin)`.
Eşikler kitabın C tablosundan ve bomba sert kurallarından gelir, tipe göre varsayılan doldurulur.

## 2. Ajan talimatları
- `Tools/ajan/BOLUM_AJANI.md`: tasarımcı ajanın sistem metni. Kitabın A–E özeti + **6 örnek kart**: Z12 (iyi/zor), 25 ve 33 (K2'yi geçen), 6 ve 7 (kötü örnek: bomba tehdit değil, sebebiyle), 28 veya 29 (nefes). Örneklerin eğri sayıları `hikaye_egrisi_set40.csv`'den alınır. Görev: önce kart, sonra kartı sağlayan 3 aday bölüm JSON'u (mevcut seviye formatı).
- `Tools/ajan/ELESTIRMEN.md`: eleştirmen ajan. Girdi: kart + ölçüm. Çıktı: GEÇTİ/RED + madde madde sebep + düzeltme önerisi ("boğaz 1 adım, kart 2–4 diyor; şu parçayı şuraya taşı" gibi somut).

## 3. Döngü aracı `Tools/ajan/dongu.py`
`python Tools/ajan/dongu.py --kart docs/bolum_kartlari/p21_06.json --aday <dosya>`:
1. Çözülebilirlik (fabrika çözücüsü).
2. `hikaye_egrisi` ölçümü.
3. Kart iddiaları ile ölçümün karşılaştırılması.
4. Benzerlik: eğri + siluet (siluet benzerliği yoksa basit bir doluluk maskesi benzerliği ekle). Mevcut 40 bölüm ve diğer adaylarla karşılaştırılır.
5. ZeydBot / oyuncu botları ile kazanma oranı ve kalan süre dağılımı (kitap K6, K7). Mevcut bot kodunu kullan.
Çıktı: `docs/bolum_kartlari/<id>_rapor.md` (1 sayfa: kart, eğri mini grafiği, kontrol tablosu, bot sonucu, eleştirmen kararı).
Ajan en fazla 3 tur düzeltir. 3. turda geçemeyen aday "RED" ile kalır.

## 4. İlk dikey dilim: bomba yayı
- **p21_06 (Tanıt):** bomba tek başına, başka engel yok, şekil 1–9'dan farklı. Bomba en erken 2–4. adımda çıkabilsin, fitil ≤ en erken çıkış + 2. Bombayı unutan oyuncu **kaybetsin**, hatırlayan rahat kazansın (tanıtım payı %30–45). Açılış ≥ 3 seçenek. En uzun boğaz ≤ 1.
- **p21_07 (Geliştir):** 2 bomba + bildik bir öğe (duvar). Fitiller farklı uzunlukta, sırayı oyuncu seçmeli (en az 1 adımda tuzak > 0). Normal tip eşikleri.
- Her biri için 3 aday → döngü → en iyi 1–2 finalist.
- Finalistleri p21 önizlemede **ayrı sette** yayınla (ör. `p21/?set=ajan1&lang=tr`). Zeyd'e linki ve 2 cümlelik "ne hissetmeli" notunu ver.

## 5. Rapor
`docs/analiz/AJAN_DILIM1_RAPOR.md` (en fazla 1 sayfa): kaç tur sürdü, eleştirmen neleri reddetti, finalistlerin eğrileri ve eski 6–7 ile karşılaştırması, Zeyd'in oynayacağı link.
`docs/PROJE_DURUMU.md` açık iş 6'ya tek satır ekle.

## Kabul
- Ajan araçları tekrar çalıştırılabilir (yeni bir kartla aynı komut çalışıyor).
- Finalistlerde kitap kontrolleri GEÇTİ, bot sonucunda "bombayı yok sayan bot" kaybediyor.
- `git status`: p20/kök/redesign.json değişmemiş.
