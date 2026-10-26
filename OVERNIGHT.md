# Önizleme 14 (26 Ekim 19:45): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Resimli yapboz testi (Level 7): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46
- Codex Task15: kırmızı rozet, duvar v3, uzun HUD hapları + ikonlar, kısa/uzun kart tuşları, 5 avatar, Jiggles sayacı. Çerçeve ve booster çubuğu eski hâliyle kaldı (Zeyd'in notu).

# Önizleme 12 (26 Ekim 16:40): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Yeni menü (Task13: profil, yıldız, Jiggles rescued, Level + Levels) ve yeni oyun içi set (Task14: HUD, booster tuşları, çerçeve, engeller, duvar, kartlar).
- QA 3 telefon boyutu 8/8, taşma yok; Lilita'da ğ/ş/İ olmadığı için Türkçe metinler Fredoka (+RS Sans yedeği).
- Build 5 yayınlanmadı; kodda Build 5 hâli duruyor, yayın öncesi yeniden alınacak.

# Gece turu (PM_MESAJ_14, 26 Ekim sabahı)
1. Son önizleme 11: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign (Task10 booster, Task11 HUD/kart/menü, kodla kırmızı-beyaz duvar). Kök değişmedi.
2. QA: 40 bölüm kayıtlı çözümle kazanılıyor, slotlar doğru (tutorial 1–2, tanıtım 3/6/10/14, zor 19/25/30/35–40, booster 5/9/15/23). 3 boyutta 24 ekran: docs/qa/2026-10-26/.
3. Bulunan 7 sorun, 6'sı düzeldi: kart tuşları alt kenara değiyordu; kayıpta çift "tekrar" tuşu; listede sıradaki bölüm okunmuyordu; kazanmada 1–10 kayda girmiyordu (👍 satırı erken gönderiyordu); 008 sorgusu yanlış kimlik (R→S); 2048 üstü atlas + kullanılmayan 64 görsel. Kalan: kazanma görselinde ince çizgi (Codex görev 7).
4. Telemetri (TEST_NIGHT, Build 5 adayı): plays + moves 9/9, 👍 ve 1–10 kazanmada, 1–10 kayıpta, pause→Ana menü = quit ✓. Eski a6b testi kökte L02 beklediği için artık uymuyor (ürün hatası değil).
5. Build 5 adayı hazır, YAYINLANMADI: Builds/build5-candidate (kök yeni set, ?set=classic eski, 👍/👎 açık). Yerel verify 6/7 (card = bilinen debug ölçüm hatası). Boyut 9,7 MB (br), PC'de 60 FPS; iPhone FPS ölçülemedi.
6. Not: kodda bayraklar artık AÇIK; bir sonraki gh-pages yüklemesi kökü değiştirir → yalnız "Build 5'i yayınla" ile.
7. Zeyd sabah: 1) önizleme 11'i oyna, 2) "Build 5'i yayınla" kararı, 3) iPhone + 4G testi, 4) CODEX_GOREV 7'yi Codex'e ilet.

# Önizleme 10 (26 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40 — Codex Task7 (v2 bomba/çivi/duvar, altın çerçeve, HUD seti); verify 7/7, kök değişmedi.

# Önizleme 9 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40 — ortalanmış büyük bomba, Fredoka sayaçlar, kalın duvar, altın çerçeve; verify 7/7, kök değişmedi.

# Önizleme 8 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · &lv=S28 — Codex Task6 çivi, bomba, zincir, duvar (+köşe) tahtada; verify 7/7, kök değişmedi.

# Önizleme 7 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign — parçalar Codex Kit81 ile (mask/overlay/flat, 81 kenar kombinasyonu); prosedürel çizim yedek. Verify 7/7, kök değişmedi.

# Önizleme 6 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · zincir+duvar: &lv=S28
- Büyük engel + üstünde sayaç, zincir engelin altında, pause kartı, kırmızı-beyaz duvar. Verify 7/7; kök değişmedi.

# Önizleme 5 (25 Ekim 13:10): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · ilk zor: &lv=S28
- Codex Görev 6 + kalan bağlantılar: zincir, duvar, booster halleri (kilitli/aktif/tükendi/rozet), bölüm hücreleri, makas noktası, parça gövde dokusu, yükleme ve isim ekranı.
- Verify yerel 7/7; kök ve canlı 40 değişmedi. Duvar şeridi ince/gri, okunurluğu Zeyd baksın.

# Önizleme 4 (gh-pages ecc6541, 25 Ekim 02:30) — yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Bitti: PM_MESAJ_8 (sade menü + "Level X" tuşu, tek el, yükleme/isim ekranı) ve PM_MESAJ_9 (profil kutusu "Kurtarılan Jiggles", ayarlar/ses, bölüm başlangıç kartı, yıldız + konfeti).
- Codex CLI ile Görev 4 (parça skin'i) ve 5/5b (89 PNG) yaptırıldı, hepsi bağlandı: üst bar, booster yuvaları, kartlar, düğmeler, tahta, sayaçlar, oyun arka planı.
- Link: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign (menü) · ilk zor + başlangıç kartı: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S28
- Zeyd onaylasın: Codex'in yapboz parça stili (koyu petrol kenar; inceleme: Assets/Art/PieceSkin/review.html), krem kart zemini, Level X tuşu.
- Kalan Claude çizimi ~8: zincir ve duvarın tahtadaki çizimi, booster kilitli/tükendi halleri, bölüm listesi hücreleri, bomba sıcak parlaması, parça dokusu (yalnız kenar rengi uygulandı).
- Verify yerel 7/7; boyut 8,94 MB (oyun arka planı 9 MB → 98 KB küçültüldü). Build 5 kök yayını Zeyd'in "tamam"ını bekliyor.

# Önizleme 2 — Codex görselleri (gh-pages 76ae1dc, 24 Ekim 20:30), yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Engel + booster ikonları, A seçenekleri (düğme, el, tanıtım/zor/kazanma/kaybetme görselleri, kart çerçevesi, tahta altlığı, sesler). A/B tek yerden: `Tools/art_choice.py` → build.
- Aynı linkler (aşağıda). Telefon boyutunda (375×667) bomba sayacı ve booster ikonları okunuyor; çivi kartında artık çekiç değil çivi ikonu.
- Verify: yerel 7/7 (ilk koşu); sonra makinede FPS 30'a sabitlendi (yerel ve canlı aynı, eski set de) → makine durumu, build değil. Boyut 7,38 → 8,46 MB.
- Bağlanmayan: kayıp "kilit" görseli (oyunda o kayıp türü yok), zincir/duvar tahtada çizim kaldı.

# Önizleme — Build 5-önizleme (gh-pages dd11b14, 24 Ekim 18:25): SET40 v3 yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Ana menü (koridor + mavi ikon): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Tutorial 1: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S41 · Çivi (3): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S43 · Bomba (6): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S45
- Zincir (10): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S48 · Duvar (14): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S52
- İlk zor (19, uyarı ekranı): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S28 · Final (40): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40
- Verify: yerel ve canlı 7/7. Not: "card" ölçümü 👍/👎 açıkken (Build 5 bayrakları) yanlış KALDI veriyor; debug yazısı resim çerçevesine biniyor, kart sığıyor. Ölçüm düzeltilmeli.

# Gece raporu — 24 Ekim 2026 (PM_MESAJ_7)

1. **SET40 v3 hazır, Build 5 yerelde (yayınlanmadı).** Varsayılan set v3, eski 40 `?set=classic`, 👍/👎 açık. Zeyd "Build 5'i yayınla" derse kök yayınlanır.
2. **Zeyd'in test listesi:** 1–2 (tutorial eli), 3, 6, 10, 14 (tanıtım kartları), 19 (zor uyarısı + ilk zor), 40 (final), ana menü arka planı ve ikon. Telefonda oynamak için önce yayın kararı gerekiyor (gh-pages'e gönderilmedi).
3. Yeni 12 bölüm (1–3, 5–8, 10–14) fabrikanın üreticisiyle yapıldı, ölçülmedi; 4, 9, 15–40 v2 havuzundan. S39 çıktı, 37 nefes. Tablo: `docs/analiz/zeydbot/SET40.md`.
4. Booster açılışları değişmedi (5/9/15/23); tanıtımlarla (3/6/10/14) çakışma yok.
5. Testler 551/554; kalan 1 hız testi (L40 FunSim 3,4 sn > 2 sn) Codex'in yalnızca yorum içeren hâlinde de kalıyor, kodla ilgisiz.
6. verify_build 6/7: k1 ve fps artık `?set=classic` ile ölçülüyor. "card" kalan ölçüm hatası (debug yazısı resim çerçevesine biniyor, kart 660 ≤ 667 pt sığıyor).
7. Codex: yorumlar commit'lendi (e640763); yeni görevler `docs/CODEX_GOREV.md`; ASSET_IHTIYAC koda göre dolduruldu.
