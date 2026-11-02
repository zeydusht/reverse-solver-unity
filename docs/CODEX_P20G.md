# Codex görevi P20-G — Booster eğitimi, envanter, çoklu spotlight (PM_MESAJ_30)

Kaynak: `docs/PM_MESAJ_30.md` (oku; kararlar orada). Belirsiz kalan her noktada en basit seçeneği uygula ve raporda "PM kararı bekliyor" diye yaz.

## Dokunma
`Assets/_Game/Levels/*` (redesign.json dahil), `Core/` (aşağıdaki hazır API dışında değişiklik yok; Core'u ben değiştirdim), `Platform/`, telemetri akışı (`GameRoot.Rate`, `_rated`, `_telemetry.Finished`), `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`. Süre kuralı: süre ilk normal hamlede başlar (`ClockOnFirstMove`), bunu değiştirme. Commit atma. `Library/ Logs/ Builds/` okuma. Geçici dosyalar `%TEMP%`'e.

## Hazır Core API (GameSession)
- `SetStock(Booster b, int n)`: stok envanterden (oturum başında, ilk komuttan önce).
- `FreeUse` (Booster?): atanırsa o booster'ın bir sonraki kullanımı ücretsiz: stok düşmez, `Used`/`Record.b_*` artmaz, süre başlamaz; `Used` olayı yine çıkar (moves satırı normal gider). Kullanılınca kendiliğinden null olur. Stok 0 iken de çalışır.
- Booster açılış bölümleri veride: `Level.Unlock` (redesign: makas 5, değnek 9, saat 15, çekiç 23); `session.IsUnlocked(b)`.

## 1. Envanter (`BoosterStart = 3`)
- Yalnız redesign setinde (`_redesignSet`); diğer setlerde bugünkü bölüm tablosu kalır.
- Tek sabit `BoosterStart = 3` (Presentation, ör. `BoosterInventory`). Yerel kayıt (`Host.Current.Store`, kalıcı anahtar ör. `rs_boosters`; profil/ilerleme gibi).
- Her `StartLevel`'da açılmış her booster için `session.SetStock(b, envanter[b])`; açılmamış olanlar kilitli kalır (bugünkü gibi).
- Envanter kaydı yoksa: booster'ın eğitimi daha önce yapıldıysa ya da oyuncu açılış bölümünü geçmişse (ilerleme > açılış bölümü) stok `BoosterStart`; değilse 0 (eğitim kullanımı ücretsiz olduğu için 0 sorun değil).
- Eğitim kullanımı bitince o booster'ın stoğu `BoosterStart` olur (bir kez).
- Normal kullanımda (`EventKind.BoosterUsed` / Used olayı, `FreeUse` değilken) envanter 1 azalır, hemen kaydedilir.
- Rozet stok sayısını gösterir; stok 0: tuş gri + rozet "0" (BoosterBar'daki mevcut Out durumu).
- Booster onay kartı ("… kullanılsın mı?", `BoosterControls` ask) varsa eğitim kullanımında gösterilmez.

## 2. Yönlendirilmiş ilk kullanım (bölüm 5, 9, 15, 23 = açılış bölümleri)
- Ne zaman: o booster'ın eğitimi daha önce tamamlanmadıysa (kayıtta booster başına bayrak), bölümün tanıtım kartı kapandıktan sonra (kart yoksa açılışta), süre başlamadan. Tamamlanınca bayrak kaydedilir; tekrar oynamada gösterilmez. Eğitim sırasında `session.Tick` çalışmaz (mevcut `TutorialActive` mantığı).
- Adımlar `tutorial.json`'a (mevcut biçim) eklensin: `{ "slot": 5, "adim": 1, "tur": "booster", "booster": "scissors", "metinAnahtari": "tap_scissors" }` ve hedef adımı `{ ..., "tur": "booster_hedef", "booster": "scissors", "metinAnahtari": "target_scissors" }`.
- **Tuş adımı:** örtü %60, yalnız ilgili booster tuşu aydınlık (yuvarlatılmış delik), el tuşa dokunma döngüsü, altta Banner yazı (PM tablosu, TR/EN: Makası seç / Tap the scissors, Değneği seç / Tap the wand, Saate dokun / Tap the clock, Çekici seç / Tap the hammer). Yalnız o tuş tepki verir; başka dokunuş hiçbir şeye gitmez. Dokununca `session.FreeUse = b` ve booster seçilir (bugünkü "armed" akışı).
- **Hedef adımı** — koddaki gerçek etkiye göre:
  - **Makas** (`UseScissors(x, y, vertical)`): iki parça arasındaki çıkıntılı bir eklemi düzleştirir. Hedef: kayıtlı çözümde en erken çıkan parçanın kesilebilir (`IsCuttable`) bir eklemi; yoksa herhangi bir kesilebilir eklem; hiç yoksa hedef adımı atlanır (raporla). Aydınlık: eklemin iki hücresi. Metin: TR "Parçaları ayırmak için eklemi kes" EN "Tap the joint to cut it" (koda göre düzelt). Yalnız o eklem tepki verir.
  - **Çekiç** (`UseHammer(piece)`): parçayı doğrudan çıkarır. Hedef: çivili parça (`session.Nails` > 0) varsa çözümde en erken kullanılan, yoksa çözümün ilk parçası. Metin TR "Parçayı çıkarmak için ona dokun" EN "Tap a piece to knock it out".
  - **Değnek** (`UseWand()`): hedef almaz, tüm eklemleri karıştırır → hedef adımı yok; tuşa dokunmak (ve varsa onay) yeterli. En basit seçenek, raporda "PM kararı bekliyor".
  - **Saat** (`UseClock()`): hedef yok; dokununca süre artar, sayaç üstünde kısa "+20 sn" efekti (yoksa ekle: 0,8 sn yukarı kayıp sönen yazı).
- Etki gerçekten uygulanır, örtü 0,2 sn'de kalkar, normal oyun. Süre ilk normal hamlede başlar.

## 3. Çoklu spotlight
- `TutorialSpotlight` örtüsü: birden çok dikdörtgen ve dairenin birleşimi delik olsun (ör. ekranı yatay bantlara bölüp her bantta deliklerin dışını koyu dikdörtgenlerle doldur; daire = birkaç ince dikdörtgen ya da yuvarlatılmış kare yaklaşımı yeter).
- `goster` adımları:
  - **chain** (bölüm 10): zincirle bağlı iki parçanın tüm hücreleri + aradaki zincir + ikisinin çıkış yolu (çözümdeki yönde tahta kenarına kadar şerit).
  - **nail**: çivili parça + sayacı. **bomb**: bomba parçası + fitil sayacı. **wall**: duvar şeridinin tamamı.

## Test
- EditMode: `TutorialScriptTests`'e booster adımları için kontrol (her `booster_hedef` makas/çekiç adımında hedef var ya da adım atlanıyor; açılış bölümleri 5/9/15/23).
- Var olan testler bozulmasın. Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6).

## Bitince
KISA rapor: değişen dosyalar, her booster için hedef seçimi ve metin, "PM kararı bekliyor" listesi, envanter anahtarı.
