# PM_MESAJ_85: Rehber eli (D eli + A hareketi + yeni ok), daha kısa bölüm geçişi

Kodu kendin yaz. p23 üzerinde. **PM_84 bitmediyse önce onu bitir.** Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (18 Kasım 14:44) el seçeneklerine baktı: üçünün karışımını istiyor.

## 1. Rehber eli: D'nin görünüşü + A'nın hareketi + yeni ok
- **Görünüş:** `&el=d` (p20 eldivenli el) çizimi.
- **Hareket:** `&el=a` (TASK41) hareketi. Adım adım:
  1. El sağdan gelir, parçanın üstünde çapraz durur.
  2. Parmak ucuyla parçaya basar: küçük basma, parmak hafif ezilir, altında küçük halka.
  3. Kısa bekler (~0,25 sn).
  4. Bastırarak gidiş yönüne sürükler: el ve parça birlikte kayar (parçanın hayaleti, gerçek parça yerinde).
  5. Sonda kısa durur, kalkar, kaybolur.
  6. Döngü: ~1,8 sn bekleme, sonra tekrar.
- **Ok:** `&el=b`'deki ok fikri (yolu gösteren ok) ama yeniden çizilecek.
  - Noktalı ve ince ok **olmayacak**.
  - Düz, kalın, şerit hâlinde tek bir ok. Rengi krem, kenarları eldivenle aynı çizgi ve gölge dilinde, eldivenle uyumlu.
  - Parçanın gideceği yol boyunca uzanır, ucu gidiş yönünde.
  - **Önce ok yolu açar:** ok parçadan çıkış yönüne doğru uzanarak çizilir (~0,3 sn), sonra el gelir ve sürükler.
  - Karartmalı adımlarda (2–5) ve karartmasız ipucunda (PM_84) aynı ok.
- Codex oku 3 seçenek çizer, puanla, en iyisini bağla. 390×844 GIF'ler `docs/art/pm85/`.
- Bu birleşim varsayılan olur. `&el=a–d` bayrakları test için kalabilir.

## 2. Bölümden bölüme geçiş daha kısa
Zeyd: "Çok iyi geliyordu ama 3 bölüm oynadıktan sonra 'niye bu kadar uzun' diye düşünmeye başladım."

| Adım | Şimdi | Yeni |
|---|---|---|
| Koridor kayışı (kazanma → sonraki tablo) | ~3 sn | **1,5 sn** |
| Yaklaşma | — | 0,6 sn |
| Resimde kalış | ~1,5 sn | **≤ 0,4 sn** |
| Geri çekilme | — | 0,8 sn |
| **Toplam (kazanma → oyun)** | | **≈ 3,3 sn** (bugün belirgin daha uzun) |

- **Koridor kayışı, 1,5 sn:**
  - başta ~0,2 sn bulunduğun tabloyu göster;
  - sonra yumuşak hızlanma, hızlı ve akıcı kayış;
  - yeni tablonun önünde yumuşakça dur ve otur. Zıplama, sekme ve "zınk" yok (ease-in-out, son %25 yumuşak yavaşlama).
- **Yaklaşma:** durduktan hemen sonra, arada bekleme yok.
- **Resimde kalış:** çizgiler resim tam ekran olur olmaz başlar (PM_84: çizgiler gerçek tahtanın kenarları).
- **Geri çekilme:** çizgiler çizilirken başlar; çizgiler çekilmenin %70'inde tamamlanır; HUD son 0,3 sn'de.
- Menü → bölüm geçişindeki resimde kalış da aynı kurala uysun: ≤ 0,4 sn, çizgiler çekilirken sürer. Yay aynı.
- Dokunma kuralı aynı: 1. dokunuş 2×, 2. dokunuş bitirir.
- **Ayar bayrakları:** `&koridor=` (sn, varsayılan 1.5), `&resimbekle=` (sn, varsayılan 0.4).
- Hız grafiği ve GIF (normal + yarım hız) `docs/art/pm85/`. Kenar boşluğu 0. 20 ms'yi aşan kare sayısını raporla (hedef 0).

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&reset=1&lang=tr` (yeni el + ok)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan, kısa geçişi gör)
  - `p23/?c=p23&test=1&lang=tr&lv=3&koridor=1.2&resimbekle=0.25` (daha hızlı deneme)
