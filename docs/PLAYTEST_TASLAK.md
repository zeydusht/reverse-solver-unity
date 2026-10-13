# Playtest taslağı (Faz 3, yeni setle) — PM'e öneri

Hazırlayan: mühendislik, 14 Ekim 2026 gecesi.

Bu belge yalnızca bir öneridir: kod yok, karar yok. Her madde ya bir PRODUCT.md kararına ya da PM'in vereceği bir karara bağlı. Playtest tasarımının kararı G8'den sonra PM'de (karar 2026-10-13 ~18:40). PM kararı gelmeden test modu kodu yazılmaz.

## 1. Amaç (PRODUCT.md)

- Playtest, fabrikanın tahminlerini gerçek oyuncularla sınar. Sınanacak tahminler: zorluk (ilk denemede kazanma) ve keyif (1000 sanal oyuncunun keyif tahmini). Karar: 2026-10-13 ~18:40.
- **Başarı ölçütü:** ≥ 30 oyuncu ve her deney seviyesinde ≥ 20 tamamlanmış oyun. Kaynak: "Başarı ölçütleri" tablosu.
- **Keyif doğrulaması:** seviye başına tahmin edilen keyif oranı ile gerçek 👍 oranı karşılaştırılır. Sayısal hedefi PM belirler; veri yokken eşik konmaz.

## 2. Sıralı set mi, test modu mu? (karar: PM)

| | **A. Sıralı set** (oyun sırası R01 → R40, bölüm kilidi açık) | **B. Test modu** (deney seviyeleri oyuncuya rastgele sırada) |
|---|---|---|
| Neyi ölçer | Fabrikanın kurduğu eğriyi, yani oyuncunun gerçekten yaşayacağı oyunu | Seviyenin kendi zorluğunu, sıra ve öğrenme etkisinden arındırılmış olarak |
| Artı | Kod gerekmez (bayrak + set yeter); case study'nin "tahmin → gerçek oyuncu" hikâyesine birebir uyar; tanıtım sırası (3 çivi, 7 mühür, 12 zincir, 20 bomba) korunur | Her seviyeye eşit sayıda oyun düşer; "≥ 20 tamamlanmış oyun" ölçütü her seviyede tutar |
| Eksi | Sonlara doğru oyuncu azalır, bu yüzden 30–40. seviyelerde ≥ 20 oyun zor; öğrenme etkisi zorluğa karışır | Tanıtılmamış engel önce gelebilir ve tanıtım sırası bozulur; yeni kod ve test gerekir (PRODUCT'taki tanım bekliyor); oyuncu deneyimi gerçek oyundan farklı olur |
| Gerekli iş | `SetFlags.RedesignDefault` (yalnızca Zeyd'in "yeni seti canlıya al" komutu) ya da playtest'e özel bir set parametresi | Test modu kodu (rastgele sıra, kilit kuralı; karar 2026-10-06 gece), testleri, yeni bir build |

**Önerim: A.** Gerekçeler:
- Playtest'in amacı fabrikanın 40'lık eğrisini sınamak.
- B'deki "tanıtılmamış engel" sorunu deneyi bozar.
- Seviye başına sayıyı yükseltmek için ayrıca "sonraki 10'a atla" gibi bir yol düşünülebilir. Bu ayrı bir PM kararıdır.

**Açık soru (PM):** A seçilirse playtest linki nasıl açılır?
- Seçenek 1: kök adresin varsayılan seti değiştirilir. Bu Zeyd'in kararıdır.
- Seçenek 2: ayrı bir parametre kullanılır, örneğin `?set=redesign` ama debug olmadan. Bu yeni bir koddur ve gönderim açık kalır.

## 3. "≥ 20 tamamlanmış oyun" sıralı 40'ta nasıl sayılır (öneri)

- **Tamamlanmış oyun:** bir oyuncunun bir seviyedeki ilk denemesi, sonucu ne olursa olsun (kazanç, süre bitmesi, patlama). Bırakma (`quit`) ayrı sayılır.
- **Seviyenin "ilk deneme kazanma" verisi:** her oyuncunun o seviyedeki `attempt = 1` satırı. Unity'de deneme sayacı doğru; web'dekinin aksine. Kaynak: karar 2026-10-05 gece.
- **Beklenen düşüş:** web verisinde 5 oyuncu 40 seviyeyi bitirmedi, satırların yarısı ilk 10'da. Sıralı sette 30 oyuncuyla 30–40. seviyelerde 20'ye ulaşmak zor.
  - Ya oyuncu sayısı artırılır (öneri: 40+),
  - ya ölçüt "bölüm başına" (her 10'luk) toplanır.
  - Karar PM'in.

## 4. 👍/👎 sorusu (G8, bayrak kapalı)

- **Yer:** kazanılan bölümün sonuç kartında "Bu bölümü beğendin mi?" sorusu ve "Beğendim" / "Beğenmedim" düğmeleri, ana düğmenin üstünde. Kod `faz4-g8` dalında hazır.
- **Kaybedilen bölüm:** soru sorulmaz. Neden: kayıptaki duygu zorluk verisinden zaten okunuyor ve soru tekrar denemeyi yavaşlatır.
  - **Açık soru (PM):** kayıpta da sorulsun mu? Tahmin edilen "hayal kırıklığı" ile karşılaştırmak için faydalı olur.
- **Önkoşullar:**
  - Zeyd `docs/sql/004_keyif.sql`'i çalıştırır ve doğrulamayı getirir.
  - `SetFlags.AskFun` Zeyd'in onayıyla açılır.
  - Build alınır, ardından `verify_build.js` çalışır, gerekirse `--send` ile.
  - Cevap verilmezse satır, kart kapanınca cevapsız gider; satır kaybolmaz.

## 5. Analiz planı (veri gelmeden tanımlanır)

Hepsi `docs/sql/003`'teki F1–F5 filtreleriyle yapılır. Playtest başlangıç tarihi F4'e yazılır; gruplama `level_id` + `level_version` ile.

1. **Zorluk tahmini.** Seviye başına sim ilk deneme (redesign.json `ilkDeneme`) ile gerçek ilk deneme kazanma karşılaştırılır:
   - Spearman korelasyonu,
   - ortalama fark d,
   - tür başına fark.
   - Web'de bu karşılaştırma 0,73 vermişti (G4).
2. **Keyif tahmini.** Seviye başına `keyif` tahmini ile gerçek 👍 oranı karşılaştırılır: Spearman ve kalibrasyon grafiği. Sayısal hedefi PM koyar (başarı ölçütü).
3. **Booster'sız kalibrasyon** (backlog, M-K4 kararı):
   - Satırdaki `b_scissors`, `b_wand`, `b_hammer`, `b_clock` sütunları booster kullanımını ayırt ediyor, bu kontrol edildi.
   - Kalibrasyon yalnızca `b_* = 0` olan satırlarla yapılır.
   - Booster'lı satırlar ayrıca raporlanır.
4. **Model varsayımları.** V1–V4 ve s/P sabitleri ilk kez gerçek veriyle karşılaştırılır:
   - ikinci denemede kazanma (ezber, V1),
   - bomba tanıtımı (V3),
   - süre dağılımı (s).
   - Sabitler yalnızca PM kararıyla değişir.
5. **Bırakma.** Bırakma oranı (`quit`) seviye başına ölçülür ve tahmin edilen hayal kırıklığıyla karşılaştırılır.

Sorgu taslakları playtest kararından sonra `docs/sql/005_playtest.sql`'e yazılır. Çalıştırma işi Zeyd'in.

## 6. Oyuncu bulma ve talimat metni (taslak)

> Merhaba! Telefonda oynanan küçük bir bulmaca oyunu geliştiriyorum ve gerçek oyuncuların nasıl oynadığını görmek istiyorum.
> Link: https://zeydusht.github.io/reverse-solver-unity/
> - iPhone'da Safari'de aç (başka telefonlar da olur), bir takma ad yaz ve oyna. Kişisel bilgi istemiyorum.
> - İstediğin kadar oyna; zorlandığın yerde bırakman da veri, utanma :)
> - Bir bölümü bitirince "beğendin mi?" diye soracak, dürüst cevap ver.
> Teşekkürler!

**Açık sorular (PM / Zeyd):**
- Davet kanalı.
- Hedef oyuncu sayısı (≥ 30, öneri 40+).
- Süre (öneri: 1 hafta).
- Oyunculara ödül verilecek mi.

## 7. Zeyd'in adımları (playtest açılmadan önce, sırayla)

1. Build 2 telefon testi, ardından Build 3 yayını. Bu, şu anki tek engel.
2. Onay kuyruğu: 40 slotun her biri için bir seviye onaylanır (R01–R40).
3. `004_keyif.sql`'in çalıştırılması ve doğrulanması.
4. PM'in sıralı set / test modu kararı. A seçilirse "yeni seti canlıya al" komutu (Zeyd).
5. `AskFun` bayrağının açılması; Build 4: G8 + onaylı set.
6. Mühendislik `verify_build.js --local`, `--send` ve canlı kontrolleri yapar.
7. Playtest öncesi 4G teyidi (açık iş).
8. Davet metni gönderilir; playtest başlangıç tarihi `docs/sql/003` F4'e yazılır.
