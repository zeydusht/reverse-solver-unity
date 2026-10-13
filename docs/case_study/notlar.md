# Case study notları

Bu dosya, `docs/PRODUCT.md`'deki 5. aşamada (case study) yazılacak portfolyo dokümanının ham malzemesidir. Bulguları kaynaklarıyla birlikte topluyor: karar tarihi, commit ya da analiz dosyası. Yeni analiz, ham veri ya da oyuncu adı içermez. Sayılar kaynaklarındaki gibidir; yeniden hesaplanmadı.

Son güncelleme: 13 Ekim 2026.

## 1. Veri kalitesi: web telemetrisi düzeltmeleri

Model ancak veriye güvenilirse işe yarar. Web oyununun verisinde bulunan sorunlar ve her birine verilen karar:

| Bulgu | Etki | Karar | Kaynak |
|---|---|---|---|
| Web'in süresi `timer − kalan` diye hesaplanıyordu, bu yüzden saat booster'ı süreyi bozuyordu. | Süre ölçüsü yanlış. | Unity'de süre, oturumun saydığı aktif saniye. | PRODUCT karar 2026-10-06 (a) |
| Web her denemede `attempt = 1` yazıyor ve hiç "quit" satırı yazmıyor. | Deneme sayısı ve bırakma oranı web verisinden okunamaz. | Web satırları deneme ve bırakma analizine girmez (filtre F5). | `Supabase/001`, `docs/sql/003` F5 |
| README'deki "patlayan bomba 0, donma 0, supap 0" garantisi kayıtlı çözüm için doğru değil. | Belgeye dayalı varsayım çürüdü. | Garanti doğrulanana kadar "doğrulanmamış" sayıldı. | PRODUCT karar 2026-10-06 |
| Web oyunu 6 Eylül'e kadar bugünkünden uzun sürelerle oynanmış (ör. L40'ta 225 sn → 65 sn). Bu, satırdaki `dur + left_s` toplamının bugünkü süreye eşit olmamasından anlaşıldı. | 183 satırın 50'si bugünkü seviyelerle oynanmadı. | 2026-09-06 14:00 UTC öncesi web satırları kalıcı olarak dışarıda (F2); 133 satır, 5 oyuncu kalıyor. | `docs/analiz/web_verisi_2026-10-13.md`, PRODUCT karar 2026-10-13 ~07:45, `docs/sql/003` F2 |
| Build 1'de bağlantıyla açılan bölümün (`?lv=N`) süresi, oyuncu adını yazarken de işliyordu. | O satırların süre ölçüsü bozuk. | Düzeltme Build 2'de yayına çıktı (`d9a08e8`); Build 2 öncesi unity-web satırları dışarıda (F4). Canlı ölçüm: kapıda 22 sn beklendi, L02 tam süreyle başladı. | PRODUCT A6b, `docs/sql/003` F4 |
| Test oyunları canlı tabloya satır yazıyor. | Gerçek oyuncu verisine karışıyor. | `TEST_` adlı satırlar silinmez, her sorguda filtrelenir (F1). | PRODUCT karar 2026-10-07 gece |
| Debug raporu çekişi yanlış yazıyordu: bölümü bitiren çekişte "Exited 0/165". | Girdi teşhisi yanıltıcıydı. | Rapor, çekişin değerlerini bölüm bitmeden alıyor. | `d813ba6` |

**Anlatılacak nokta:** Her filtre, karar tarihiyle birlikte tek bir yerde duruyor (`docs/sql/003`); satırlar hiç silinmedi. Bu sayede analiz geri alınabilir ve tekrarlanabilir.

## 2. Girdi düzeltmesi (K1): zorluk sanılan girdi hatası

- **Gözlem:** Zeyd iPhone'da sağ ve sol sütundaki parçaları çıkarmakta zorlandı. Parmak çerçeveye taşınca dokunma kayboluyor ya da iptal geliyor, parça geri yaylanıyordu. Oyuncu tahtayı çözmüş olduğu halde "takılmış" görünüyordu. Bu, süre ve deneme verisini zorluktan bağımsız bozar. Kaynak: PRODUCT karar 2026-10-08 akşam.
- **Çözüm:** Kural motoru değişmedi (altın değer testleri aynen geçti). Değişen yalnızca girdi: fiske, dokunma iptalinde eşiği geçmiş çekişi sayma, tahtanın dışına taşan 24 pt tutma alanı ve yan boşluk `G = clamp((W − 44·w)/2, 19, 34)`. Kaynak: `79bb93d`.
- **Ölçüm:** Bilgisayarda (başsız Chrome, dokunma olayları) 12 kenar çekişinin 12'si tek seferde çıktı; üçü tahtanın dışından tutuldu, üçü fiskeyle yapıldı. iPhone ölçütü (10 çekişte 9) Zeyd'in telefon testinde. Kaynak: `71f57ba`, `docs/screens/k1/K1_pulls_pc_375x667.png`.

## 3. Zorluk modeli: yapıdan süreye

- **Yapısal ölçüler** çözücünün yolu boyunca ölçülüyor: hamle başına okuma, ilk hamlede serbest parça sayısı (k₀), darboğaz (10+ parça kalmışken tek seçenek kalan hamleler), bomba payı (fitil − en kısa kazma yolu). Rastgele bir oyuncunun 200 oyunluk Monte Carlo'su da ekleniyor. Kaynak: `ff04e80`, `8577211`, `2bb3208`.
- **Model:** `T = ⌈gövde · s · P⌉₅`, en fazla 75 sn. Formülde `s = 0,6 + 0,16 × okuma`; P türe göre değişiyor. Fitil = en kısa kazma yolu + türün payı. Çivi sayacı hiçbir zaman gövdenin %40'ını geçmiyor. Kaynak: PRODUCT "Zorluk modeli (başlangıç değerleri)".
- **Web verisiyle ilk karşılaştırma** (yalnızca yön göstergesi; 10'dan fazla oyun oynanmış tek seviye var). Kaynak: `docs/analiz/web_verisi_2026-10-13.md`.
  - `s` ortalamada isabetli: kazananların medyan süresi, modelin `s`'inin 0,98 katı (18 seviye).
  - Seviye seviye süre tahmini zayıf: Spearman +0,40.
  - Yapı ölçüleri zorluğun yönünü doğru gösteriyor. Kazanma oranıyla Spearman: `s_yapı` −0,67, okuma −0,47, darboğaz −0,47 (24 seviye).
- **Bilinçli olarak yapılmayan:** Sabitler 5 oyuncunun verisine göre yeniden ayarlanmadı (oyunculardan biri satırların %45'ini oynamış). Veriye uydurulan `0,72 + 0,11 × okuma` yalnızca rapor notu olarak kaldı. Kaynak: PRODUCT karar 2026-10-13 ~07:45.
- **Modelin kendi hatası:** Tanıtım türü bombaya izin vermiyordu, oysa L21 bombayı tanıtan seviye. Tür tablosu ile bant çelişiyordu. Düzeltildi: Tanıtım ve Nefes'te bomba payı bandı 8+. Kaynak: `9978688`.

## 4. 40 web seviyesi model gözüyle

Kaynak: `docs/analiz/40_seviye_raporu.md`.

- **İlk 10 seviyenin süresi formülün yaklaşık iki katı.** Web'de kazananlar bu seviyelerde sürenin %62–76'sını kullanmadan bitiriyor. Model ile veri aynı şeyi söylüyor: süre baskısı yok.
- **19 seviyede önerilen süre mevcuttan en az %20 farklı.** Canlı 40 seviyeye dokunulmadı; bu bulgu Faz 4'teki yeniden tasarımın girdisi.
- **Türe göre web kazanma oranı:** Tanıtım %84, Kolay %100, Orta %91, Zor %72, Çok zor %50. Türle aynı yönde.

## 5. Yöntem: telefonsuz ve tekrarlanabilir doğrulama

- **Kural motoru web kodunun birebir portu.** Web kodu Node'da çalıştırılıyor ve altın değerler üretiliyor (`travel_golden`, `session_golden`). Kural değişirse bu testler kırılıyor. Kaynak: `Tools/make_travel_golden.js`, `Tools/make_session_golden.js`.
- **Her build kendini doğruluyor.** Web build'i açılırken 40 seviyeyi çözüp web'in oturum sonuçlarıyla karşılaştırıyor: "40/40 web'le aynı". Kaynak: `Platform/BootLevelCheck.cs`, `Platform/BootCheck/`.
- **Testler gerçekten hatayı yakalıyor mu (mutasyon kontrolü):** Bağlantılı bölüm düzeltmesi (A6b) koddan geri alınınca açılış testlerinden 2'si kırıldı. Önceki test bu durumda da geçiyordu. Kaynak: `4e64340`.
- **Tek komutla build doğrulaması:** `Tools/verify_build.js` açılış kontrolünü, K1 çekişlerini, FPS'i ve kart yerleşimini sınıyor. Gerçek oyuncu tablosuna yalnızca açık bir bayrak ve `TEST_` adıyla yazıyor. Canlı Build 2'de kart kontrolü, başlık ile resim çerçevesi arasındaki 3,5 pt boşluğu yakaladı; düzeltme `a80868f`. Kaynak: `29e3ff0`, `f11b215`.
- **Başsız Chrome'da GPU farkı:** Yazılımla çizim saniyede yaklaşık 3 kare verdi; bu hızla ne bölüm kazanılabiliyor ne de FPS ölçülebiliyor. Gerçek GPU'yla saniyede 60 kare. Kaynak: `2cc3836`.

## 7. Faz 4: seviye fabrikası (13–14 Ekim 2026)

Sayılar modelin çıktısıdır. **Keyif tahmini** 1000 sanal oyuncunun sonucudur ve gerçek oyuncunun yerini tutmaz; playtest'teki 👍/👎 bunu sınayacak.

### Üretim
- **"Dolu tahtayı soy":** Bütün dikişler kilitli başlar. Tahtadan parçalar çıkarılırken yalnızca gerçekten takılan dikişler düzlenir.
  - Düzlemek hiçbir hamleyi geçersiz kılmaz, bu yüzden çıkış sırası seviyenin çözümü olur.
  - 1000 adayın 1000'i oyunun kendi kurallarıyla çözüldü.
  - Kaynak: `b4df3d6`.
- **İlk sürüm açılışta zirve yapıyordu:**
  - Kenardaki parçalar dışarı çıkarken kenar boyunca dikişleri açıyor ve ilk hamlede çok parça serbest kalıyordu.
  - "Boşalan hücrelerden kaymayı" tercih eden ve oyunun ortasını sıkı tutan düğmeler eklendi (Sweep, Knot/Ease). Örneğin slot 14'te eğri uyumu 0,50'den 0,73'e çıktı.
  - Kaynak: `1cddd9b`, `884e130`.

### Keyif simülasyonu web verisiyle
- Simülasyonun ilk denemede kazanma oranı ile gerçek web kazanma oranı arasında Spearman 0,73 çıktı (24 seviye). Ortalamalar 0,79 (sim) ve 0,82 (web).
- Sim bombalı seviyelerde sert.
- Kaynak: `2217bc3`, PRODUCT karar 2026-10-13 ~23:10.

### Fabrikanın kendi çelişkileri (ve nasıl çözüldü)
1. **Süre formülü ile ilk deneme bandı.**
   - Bandı geçen Zor adaylar sim'de ilk denemede %73–85 kazanılıyordu, oysa hedef bant %35–65.
   - Karar: band korundu; Orta/Zor/Çok zor'da süre her aday için simülasyonla seçiliyor (P ≥ 1,0).
   - Kaynak: kontrol noktası 4, `b234794`.
2. **Bant ile erken süre bitmesi.**
   - Sim'de kaybeden oyuncu neredeyse hep "çok parça kalmışken süre bitti" diye kaybediyor. Zor bant ve %10 erken süre tavanı aynı anda tutmadı.
   - Tavan Zor için %25'e, Çok zor için %35'e çıktı.
   - Kaynak: kontrol noktası 4-b, `544d51f`.
3. **Çok zor bombaları.**
   - Bomba payı 2 olunca sim oyuncularının çoğu patlattı (ilk denemede %0–4 kazanma).
   - Fabrikada Çok zor için bomba payı 3–5 yapıldı ve bomba zorunlu olmaktan çıktı.
   - Kaynak: `544d51f`.
4. **Sim ile web arasındaki fark (M-K4).**
   - Zor seviyelerde fark (d) −18 puan çıktı: sim, web'den kötümser.
   - Bu fark yorumlanamadı:
     - web satırlarının yaklaşık yarısında booster kullanılmış, sim ise booster'sız oynuyor;
     - Zor'da her seviyede 3–5 satır var.
   - Bant kaydırılmadı. Kalibrasyon playtest'e kaldı ve booster'sız satırlarla yapılacak.

### Ajanların bulduğu ama kodun bulamadığı
- **Tasarımcı ajan:**
  - Kart metinlerinde redesign takvimiyle çelişen atıflar buldu: çivi kartı "9. bölümde çekiç" diyordu, oysa çekiç 23'te.
  - Booster kartlarında Türkçe karakter yoktu.
  - Nefes slotlarındaki bombaların hepsi ilk hamlede çıkıyordu; beş slot aynı fikri işliyordu.
  - Bazı finalistlerde çivi süstü: sayaç, parçanın sırası gelmeden çok önce bitiyordu.
  - Hepsi kurala çevrildi: Nefes'te kazma ≥ 2, slot 3–4'te "çivi önemli", kartlar düzeltildi.
  - Kaynak: `544d51f`, PRODUCT karar 2026-10-14 gece (2).
- **Psikolog ajan:**
  - Near-miss'i kendi talimatına göre eleme saydı ve 60 finalistten 3'ünü reddetti.
  - PRODUCT'ta near-miss yalnızca sıralama ölçütü. PM bu çelişkiyi çözdü: o üç finalist sıralamanın sonunda kaldı.
  - Anlatılacak nokta: ajanın kararı ürün kuralıyla çeliştiğinde ürün kuralı kazanır.

## 6. Henüz eksik olanlar

Case study yazılmadan önce gerekenler:

- iPhone ölçümleri: K1'de 10 çekişte 9, FPS, 4G'de açılış.
- Playtest verisi. `level_id` + `level_version` bazında deneme, süre, başarısızlık ve bırakma; filtreler F1–F5.
- Modelin sabitlerinin playtest verisiyle ayarı.
- Faz 4: yeniden tasarlanan seviyeler ve bunların modelle ve veriyle karşılaştırılması.
