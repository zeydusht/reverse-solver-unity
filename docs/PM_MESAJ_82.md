# PM_MESAJ_82: İlk 100 bölüm, yeni tanıtım sırası, harita yok, rehberli 1. bölüm, isteğe bağlı reklam

Kodu kendin yaz. p23 üzerinde (PM_81'in `pm81` dalından devam). Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

**Önce PM_81'i bitir, sonra bunu yap.** Bu mesaj PM_81'deki çakışan maddeleri geçersiz kılar (özellikle "bitmiş bölümler tekrar oynanabilsin" ve harita).

Zeyd (17 Kasım 19:25) kısaca şunları istedi:
- Artık biten bir oyun değil, canlı bir oyun yapıyoruz. Zorluk ara ara gelsin: zor ve çok zor bölümler.
- Oyuncu bölüm haritasını görmesin, istediği bölüme dönemesin. 1'den başlayıp düz oynasın.
- Eski 1. bölüm rehberi çok iyiydi: el göstersin, Jigo konuşsun.
- Oyuncu isterse kendi isteğiyle reklam izleyebilsin.

## 1. İlk 100 bölümün planı (PM kararı)

### Engel tanıtımları
Her biri tek engelli, süre baskısız ders bölümü. Kart + mini animasyon mevcut hâliyle.

| Bölüm | Engel |
|---|---|
| 3 | kilit |
| 7 | zincir |
| 11 | bomba |
| 21 | duvar |

Sonrası yeni engel yok, **birleşimler** her 10 bölümde bir, x1'lerde:
- 31: bomba + zincir
- 41: kilit + bomba
- 51: duvar + bomba
- 61: zincir + duvar
- 71: kilit + zincir + bomba
- 81: duvar + kilit
- 91: dördü birden (hafif)

Birleşim bölümleri tanıtım gibi yumuşak başlar, sonraki bölümler geliştirir ve büker (Bölüm Tasarım Kitabı: tanıt → geliştir → bük → bitir).

### Booster açılışları
Her biri bir gösterimle açılır ve 3 ücretsiz verilir.

| Bölüm | Booster | Neden |
|---|---|---|
| 6 | makas | |
| 13 | çekiç | ilk zor bölüm 14'ten hemen önce |
| 18 | saat | ilk çok zor bölüm 19'dan hemen önce |
| 25 | değnek | en zayıf booster, en son |

### Zorluk ritmi
Sort Express düzeni: **4 ile bitenler zor, 9 ile bitenler çok zor.** İlk zor 14, ilk çok zor 19.

| Tip | Bölümler | İlk deneme kazanma hedefi (model v2, ortalama oyuncu) |
|---|---|---|
| Öğrenme | 1–13 | ≥ %90; kayıp yalnız yeni öğeden, süreden kayıp ≤ %5 |
| Normal | diğerleri | 20'ye kadar %75–85, 100'e doğru yavaşça %65–75 |
| Zor | 14, 24, 34 … 94 | %35–50 |
| Çok zor | 19, 29, 39 … 99 | %20–30, hiçbiri %15'in altında olmasın |
| Nefes | çok zordan sonraki x0 (20, 30 … 100) | ≥ %85, kısa ve keyifli |

- Bölüm süresi hedefi 60–120 sn. Zor ve çok zor bölümler de 150 sn'yi geçmesin.
- Ders bölümleri (3, 7, 11, 21) ve birleşim tanıtımları (31, 41 …) asla zor slotuna denk gelmez; plan buna göre kuruldu.
- **Mevcut zor bölümler** (eski 21/23/33/35/40 vb.) çöpe gitmez. Uyanlar zor ya da çok zor slotlara taşınır; bitiş zorluğu gerekmez.
- Çeşitlilik:
  - aynı bomba çözüm deseni üst üste 2 bölümde olmasın;
  - tahta silueti ve boyutu değişsin;
  - tahta çoğunlukla kare olsun (Zeyd kuralı).

### Bölümleri hangi sistem üretecek
Elimizdeki en iyi tahmin **zorluk modeli v2**:
- PM_69 ölçüleri + PM_70 üretici + PM_71 pt1 düzeltmesi;
- sınama hatası ±10,7 puan; bot v1 ±13,5, eski model ±13,8.

Akış (PM_48 ajan döngüsü + PM_70 üretici):
1. Her slot için Claude tasarımcı ajanı, hikâye kartı ve kitap kurallarıyla **en az 8 aday** tasarlar. Hedefe göre tasarlar, rastgele üretmez.
2. Motor çözülebilirliği doğrular; tek çözüme kilitli bölüm olmasın.
3. FunSim (pt1 kalibrasyonu) ve model v2 ilk deneme oranını tahmin eder.
4. Hedef banda en yakın ve çeşitlilik kuralını geçen aday seçilir. Bantta aday yoksa 8 aday daha üret; yine yoksa en yakını al ve raporda işaretle.
5. **1–40 da bu plana göre yeniden düzenlenir.** Uyan mevcut bölüm kalır (pt1 verisi olanlar değerli), uymayan yer değiştirir ya da yeniden üretilir.

Çıktılar:
- Yeni set `sl100`.
- Rapor `docs/analiz/PM82_100_BOLUM.md`: bölüm · tip · engeller · tahmini ilk deneme · süre · kaynak (eski / yeni).
- 100 bölümlük zorluk eğrisi grafiği `docs/analiz/pm82_egri.png`.
- Tahminler `docs/analiz/v2/pred_sl100.csv`. Supabase'e PM yükler, sen yazma.

### Tablolar
41–100 için Codex'e 60 tablo çizdir (üç parti: 41–60, 61–80, 81–100). Stil: mevcut 40 tablo. Önce 41–60, gerisi yetişmezse geçici tekrar tablo (PM_81 kuralı).

## 2. Harita yok, düz ilerleme
- Bölüm haritası oyuncuya görünmez. Menüde tek büyük "Oyna · Bölüm N" düğmesi.
- Zor bölümde düğme **kırmızı** + "Zor Bölüm", çok zor bölümde koyu kırmızı + Kont mührü + "Çok Zor Bölüm". Bölüm başında da aynı uyarı kartı kısa görünsün.
- Bitmiş bölüme dönülemez. Kaybedince aynı bölüm tekrar oynanır.
- Test linkinde (`&test=1`) bölüm seçimi açık kalsın (geliştirme için).
- Harita kodu silinmesin, bayrakla kapalı kalsın (`&harita=1`).

## 3. Rehberli 1. bölüm (eski el geri, Jigo konuşur)
- Tahta: **p20'deki eski 1. bölüm** (git geçmişinden).
- El: eski **eldivenli el** animasyonu (p20). Daha açıklayıcı yap: büyük, net, hareket yönünde kısa altın iz, eylem yapılana kadar döngüde.
- Jigo alttaki krem sohbet kutusunda konuşur. Yazılar harf harf akar; dokununca tamamlanır.
- Ekran **kararmaz** (PM_72 hatası). Sıradaki parça altın parlar, diğerleri normal.
- Süre yok, kayıp yok.

| Adım | Jigo (TR) | Jigo (EN) | El / oyun |
|---|---|---|---|
| 1 | "Merhaba, ben Jigo! Kont arkadaşlarımı bu tabloya hapsetti." | "Hi, I'm Jigo! The Count trapped my friends in this painting." | — (dokununca devam) |
| 2 | "Bu parçayı aşağı kaydır, bir Jiggle'ı kurtar!" | "Slide this piece down to save a Jiggle!" | sol alt parçada aşağı kaydırma |
| 3 | "Harika, bir Jiggle kurtuldu! Şimdi şunu sağa itmeyi dene." | "Great, you saved a Jiggle! Now try pushing this one right." | sağ alt parçada sağa itme, **bilerek takılır** |
| 4 | "Olmadı! Yanındaki parçaya çarptı. Takılan parça çıkamaz." | "Oops! It bumped into its neighbor. A stuck piece can't get out." | temas noktası kısa kırmızı parıltı |
| 5 | "Bunu hiçbir yere çarpmadan aşağı itmen lazım." | "Push it down, where nothing blocks it." | aynı parçada aşağı |
| 6 | "Süpersin! Kalan Jiggle'ları sen kurtar!" | "Awesome! Now save the rest yourself!" | el kaybolur, serbest oyun |
| 7 | son parçada: "Bütün Jiggle'lar kurtuldu!" | "You saved all the Jiggles!" | sonra mevcut "Harika!" kazanma ekranı |

- Rehberli adımda yanlış parçaya dokunulursa parça hafif sallanır, Jigo "Önce bunu dene!" der. Ceza yok.
- 4 sn hareketsizlikte el yeniden gösterilir.
- Serbest kısımda 6 sn hareketsizlikte önü açık bir parça altın parlar.
- Eski tahtada "sağa" takılmıyorsa takılan yönü kullan (ör. yukarı), metni ona göre değiştir ve raporla.
- 2. bölüm: rehbersiz, kolay; tek ipucu: 6 sn hareketsizlikte açık parça parlar.
- PlayMode testi: 7 adım sırayla, yanlış dokunuş, hareketsizlik ipucu.

## 4. İsteğe bağlı reklam (ödüllü)
PM_81'deki R1, R2 ve geçiş reklamı aynen kalır. Ek olarak:
- **R3, booster bittiğinde:** booster sayısı 0 olunca yuvada küçük video simgesi. Dokununca reklam → +1 o booster. Günde en çok 5 (config).
- **R4, menüde "Hediye":** menüde küçük hediye kutusu, reklam izlenince 100 altın. Günde 3 (config). Hazır değilse görünmez.
- İkisi de `live/config.json`'dan açılıp kapanır; telemetride yerleşim adı `r3_booster` / `r4_hediye`.

## 5. Görsel düzeltmeler (Zeyd liste hâlinde verecek, ilki şimdi)
**Kazanma ekranı alt düğmeleri** ("Sonraki Seviye" ve "Menü") küçük ve sönük. Ekranın geri kalanı onaylı, ona dokunma.
- Düğmeler daha büyük ve tıklanır görünsün: en az 60 pt yükseklik; Sonraki Seviye genişliğin ~%70'i, Menü daha küçük ikincil.
- Onaylı stil: 3B kalın yazı, yeşil ana düğme / mavi ikincil, alt gölge, basınca çökme animasyonu.
- Codex 3 seçenek çizer, sen puanla, en iyi 2'yi `docs/art/pm82/kazanma_dugme/` klasörüne koy, en iyisini bağla.

Zeyd yeni görsel sorun buldukça PM aynı biçimde ekleyecek.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- p23 varsayılan set `sl100`, harita kapalı, yeni 1. bölüm. `live/`'a **yayınlama** (PM onayı bekler).
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&reset=1&lang=tr` (sıfırdan: rehber + düz ilerleme)
  - `p23/?c=p23&test=1&lang=tr` (bölüm seçimi açık)
  - `p23/?c=p23&test=1&lang=tr&lv=19` (ilk çok zor)
