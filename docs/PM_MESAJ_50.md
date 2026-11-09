# PM_MESAJ_50 — Açılış hikâye videosu: kurgu + oyuna bağlama (p21)

Kaynak: `docs/art/task26/` (`CODEX_TASK26_V06.md` = senaryo, `CEKIM_LISTESI.md` = geçişler, `KLING_PROMPTLARI.md`). Klipler `docs/art/task26/video/` içinde: `1.mp4` … `11.mp4` (Kling, 1080×1920, 24 fps; 1–3 5 sn, diğerleri 3 sn). `9.mp4` Zeyd'den gelecek, yoksa yer tutucu kullan. Kötü adamın adı **Kont**. p20/kök/pt1'e dokunma, her şey p21.

## 1. Kurgu (`Tools/intro/kurgu.py`, ffmpeg)
- **Bölüm A (ilk açılış):** 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8. Hedef toplam ~18–21 sn. 1–3'ü en canlı 2,5–3 sn'ye kırp (kırpma noktalarını parametre yap). Geçişler: 1→2 düz kesme, 3 sonu zaten kararıyor, 4 sonu 0,3 sn kararma, 6→7 kısa çapraz geçiş, diğerleri düz kesme.
- **Bölüm B (dokunmadan sonra):** yalnız 9. Son karesi 1. bölümün tahtasına geçiş noktası.
- **Bölüm C (1. bölüm kazanılınca):** 10 → 11.
- Çıktı: `Assets/Intro/introA.mp4`, `introB.mp4`, `introC.mp4`. H.264, 720×1280 (web için), CRF ~26, `-preset veryfast`, sessiz video + ayrı ses. Toplam boyut ≤ 8 MB hedef. Ayrıca inceleme için 1080p tek dosya `docs/art/task26/video/intro_onizleme.mp4` (A + B + C arka arkaya).

## 2. Ses
Mevcut ses kütüphanesinden (PM_45 ses haritası, `docs/sounds`): hafif çayır ambiyansı (A başı), gölgede alçak tını, çuval hışırtısı, kapı tok sesi, son parçanın "tık"ı, 8'de göz kırpış tınısı, C'de "weee" + eşya düşme sesleri + kahraman pozunda kısa müzik vurgusu. Yoksa basit sentetik yer tutucu yap ve raporda listele.

## 3. Oyuna bağlama (p21)
- İlk açılışta (hiç oynanmamışsa): introA tam ekran oynar. **Atla** düğmesi (sağ üst, 1 sn sonra görünür).
- introA son karesinde **donar**, "Uyandır! / Tap to wake!" düğmesi çıkar (oyun metni, i18n). Dokunma veya 5 sn → introB → 1. bölüm başlar.
- 1. bölümün resmi **Tablo #1** (`docs/art/task26/cekim-kareleri-v06/tablo-01.png`, renk/dizilim aynı). Yoksa sırada en yakın tablo; raporda yaz.
- 1. bölüm kazanılınca (yalnız ilk kez): introC → menü koridoru. Son yazı oyun metni olarak: **"Save the Jiggles!" / "Jiggles'ı kurtar!"**.
- Ayarlar'a "Hikâyeyi tekrar izle".
- Oyundaki "Baron" metinleri (zor bölüm kartı, i18n) → **Kont** / "the Count".
- Veri: `intro_shown`, `intro_skipped` (hangi saniyede), `intro_wake_tap` (dokundu mu, kaç sn) olaylarını mevcut telemetriye ekle.

## 4. Teslim
p21'de yayın + link (ilk açılışı görmek için `&intro=1` ile zorla). `docs/art/task26/video/KURGU_RAPOR.md`: süreler, kırpma noktaları, boyutlar, eksik sesler. PROJE_DURUMU'na tek satır.
