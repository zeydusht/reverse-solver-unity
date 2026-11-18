# PM_MESAJ_84: Rehberin devamı (okla ipucu, yalnız 1–3), son parça mesajı, dalışta çizgiden tahtaya kesintisiz geçiş

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (18 Kasım 14:08): "Şu an daha iyi tutorial." Rehber PM_83 hâliyle kalıyor (karartma + kayan ok, Jigo kutusu üstte). Aşağıdakiler eklenecek.

## 1. Rehberin devamı: karartmasız ok ipucu
Amaç: oyuncu mekaniği kavrasın, "hangisi boşta?" sorusunun cevabını ilk bölümlerde görsün.

**1. bölüm, serbest kısım (adım 6'dan sonra):**
- "Kalanları sen kurtar!" dedikten sonra serbest kısmın **ilk hamlesinde** (bölümün 3. hamlesi) bir parça daha gösterilir:
  - önü açık bir parça altın parlar (bugünkü sarı parlama, Zeyd beğendi);
  - üstünde gideceği yöne kayan ok;
  - **karartma yok**;
  - Jigo üst kutuda: "Bu parçayı oynatabilirsin!" (EN: "You can move this one!").
- Oyuncu başka bir doğru parçayı çıkarırsa da kabul. İpucu kalkar, kızma yok.

**İpucu (bölüm 1, 2 ve 3):**
- Oyuncu **5 sn** hareketsiz kalırsa önü açık bir parça parlar + ok.
- Daha dikkat çekici olsun:
  - parlama nabız gibi (1,0 ↔ 1,06 ölçek, 0,8 sn);
  - ok gidiş yönünde 2 kez ileri-geri sallanır;
  - ilk gösterimde Jigo kutuda kısa "Bu parçayı oynatabilirsin!".
- Oyuncu dokununca ipucu hemen kalkar. Sonraki hareketsizlikte yine çıkar.
- Seçilen parça: sonraki doğru hamle (çözümün devamını bozmayan, takılmayan). Bomba varsa önce bombayı çözen parça.

**4. bölüm ve sonrası: ipucu YOK.** Ne parlama ne ok; zor ve çok zor bölümlerde de yok. Hangi parçanın boşta olduğunu bulmak oyunun kendisi; kopya vermiyoruz.
- Sınır `live/config.json` → `ipucu_son_bolum = 3`.
- PM_82'deki "2. bölümde 6 sn hareketsizlikte parlama" bu kuralla değişir.
- Booster dersleri ve engel kartları aynen kalır.

## 2. HATA: "Bütün Jiggle'lar kurtuldu!" erken çıkıyor
1. bölümde 8. parça çıkınca yazıyor, oysa 9 parça var.
- Mesaj **son parça tahtadan tamamen çıktıktan sonra** gelsin (çıkış animasyonu bitince). Sonra "Harika!".
- Sayaçta bir eksik ya da erken tetik var: sebebini bul ve raporla.
- Aynı hatanın başka bölümlerde de olup olmadığını kontrol et: kazanma tetiği her bölümde son parçanın çıkışından sonra mı?
- PlayMode testi: 1. bölüm 9/9, başka bir bölüm ve tek parçası kalmış tahta.

## 3. Dalış: resimdeki çizgiler doğrudan gerçek tahtaya dönüşsün
**Şimdiki sorun:** Resme yaklaşınca yapboz çizgileri doğru çıkıyor. Geri çekilirken çizgiler kayboluyor, kısa bir boşluk oluyor, sonra gerçek tahta geliyor.

**İstenen (PM onaylıyor, Zeyd'in fikri doğru):**
- Hiç boşluk olmasın. Çizilen çizgiler **gerçek tahtanın kendi parça kenarları** olsun.
- **Yöntem:**
  - gerçek tahtayı dalışın başında resmin tam üstüne, aynı konum ve ölçekte, görünmez kur;
  - çizgiler bu tahtanın parça sınırlarından çizilsin;
  - geri çekilirken ayrı bir çizgi katmanı kalkıp tahta gelmesin; aynı nesne ölçeklenerek oyun konumuna otursun.
- Parçaların hafif ayrışması (gölge, 1–2 px) çekilmenin sonunda aynen kalır.
- **Ölçüm:**
  - geçişin her karesinde ya çizgiler ya tahta görünür; hiçbiri görünmeyen kare sayısı **0**;
  - çizgi ile parça kenarı arasındaki fark ≤ 1 px;
  - 20 ms'yi aşan kare sayısını raporla (hedef 0).
- GIF'ler `docs/art/pm84/` (normal + yarım hız), önce/sonra.
- Kazanma → sonraki bölüm dalışında da aynı kural.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&reset=1&lang=tr` (rehber + ipucu)
  - `p23/?c=p23&test=1&lang=tr&lv=4` (ipucu yok)
  - `p23/?c=p23&test=1&lang=tr&lv=2` (dalış ve 5 sn ipucu)
