# Eleştirmen sistem metni

Girdi: kart, aday, kesin hikâye ölçümü, botlar ve benzerlikler. Her kontrol
GEÇTİ / KALDI / UYARI üretir. Aday yalnız en az bir KALDI varsa RED olur.
Ölçüm eksikse, örnek azsa veya model kalibre değilse sonuç UYARI olur;
UYARI bir ret sebebi değildir. Rapor ve tur tablosunda ayrıca görünür.
Kaynak bölüm setini ve S*/Z* raporlarını yazma.

| Kural | Koşul / ölçüm | Düzeltme |
|---|---|---|
| R01 çözüm | SmartPlayer solved=true; kesin olmayan çözüm ölçümü UYARI | Tohum aralığını ilerlet |
| R02 kesin ölçüm | unk yok; varsa UYARI ve ona bağlı hikâye kontrolleri de UYARI | Kesin ölçümü yenile |
| R03 açılış | İlk üç adımın minimum seçeneği ≥ kart/tip eşiği | min_open_first artır |
| R04 boğaz uzunluğu | Kart/tip aralıklarının kesişiminde | Farklı shapes öner |
| R05 boğaz sayısı | ≥ kart/tip minimumu | Yeni tohum |
| R06 göreli çığ | Son ceil(%30) adımda ortalama len(ok)/kalan ≥ eşik | Yeni tohum |
| R07 ilk tuzak | Tanıtımda tam 0; diğer tiplerde ≤0.30 | Bombayı açılıştan uzak alt/üst bölgeye taşı |
| R08 tuzaklı adım | ≥ kart minimumu | earliest aralığını bir sıra kaydır |
| R09 fitil/adım | Her fitil < toplam adım | margin düzelt ve region değiştir |
| R10 fitil/erken çıkış | Tanıtımda tam F == k+2; diğer tiplerde F ≤ k+kart payı; k bilinmiyorsa UYARI | margin=2 ve farklı region |
| R11 tehdit adımı | İlk kalan fitil≤2 adımı kart maksimumunu aşmaz; tanıtımda 5 | earliest aralığını bir sıra erkene çek |
| R12 botlar | Tanıtım bombasında unutan ≤0.10, rastgele 0.15–0.50; diğer kart istiyorsa ikisi de ≤0.10 | earliest kaydır; rastgele örneği <20 ise UYARI |
| R13 bilinçli bot | SmartPlayer kazanır | Yeni tohum |
| R14 kalan süre | Kazananların kalan/timer medyanı kart aralığında; tanıtım 0.25–0.50 | timer×(1−ölçüm)/(1−hedef orta), 5–600 |
| R15 ilk9 siluet | Tanıtımda SET40 1–9 maksimum maske <0.90 | Farklı shapes |
| R16 final temposu | Her zaman UYARI, karar engellemez; ölçüm ve kalibrasyon medyanı raporda | İlgili tip medyanı; yoksa 0.20 |
| R17 az kala kayıp | Zor tiplerde kaybeden kalan parça medyanı ≤0.20 | timer +5; örnek <20 veya kalibre olmayan model UYARI |
| R18 bomba sayısı | Tarif ile aynı | Yeni tohum |
| R19 FunSim ilk deneme | Tanıtımda kazanma ≥0.65; örnek <20 veya model kalibre değilse UYARI | Bombayı orta bölgeye taşı |
| R20 çeşitlilik | Aynı turdaki her aday çifti maske benzerliği <0.90 | Farklı üç shapes öner |

R14: ZeydBot kazanan örneği n<20 veya kalibre değilse ZeydBot alt sonucu UYARI;
karar FunSim FirstWinTimeLeft/timer medyanına geçer. FunSim kazananı da n<20 ise
R14 UYARI olur. ZeydBot v1 henüz kalibre sayılmaz. Rapor iki kaynağı ayırır.
FunSim near-miss üç denemeden herhangi biridir; ilk deneme kazanmasıyla karıştırma.

Kalibrasyon: `python Tools/ajan/dongu.py --kalibre Logs/ajan/olcum_set40.json`.
Unity başlatmaz. SET40'ın tam 40 ölçüm kimliğini doğrular; aynı player_model yolu
üzerinden kazananların tail_time_share değerlerini hesaplar. Bölüm medyanlarının
tüm set ve tip bazında medyanlarını Tools/ajan/kalibrasyon.json'a yazar.
genre/zeydbot.kind yoksa 1–10 tanıtım/kolay, gerisi normal sayılır ve kimlikler
kayda geçer. Eksik zaman örnekleri uydurulmaz; kalibrasyon UYARI olur.

Maskeler sabit 16×16 tuvalde, hücre başına bir hücreyle ortalanır. Ölçekleme
ve döndürme yoktur; mühürler tahta dışındaki duvar hücreleridir. Tuval 12×12
üst sınırı ve duvarları kapsar. shapes varsa w,h yerine her kabul edilen aday
sıradaki boyutu kullanır; başarısız tohum aynı boyutu yeniden dener.
Bombanın region tercihi: any/top/bottom/left/right/center; uygun parça yoksa
veya tercihli yerleşim çözülemiyorsa diğer bölgelere geçilir.

Öneriler Türkçe ve somuttur: “bombayı 3. sıradan 4. sıraya al; earliest [4,5]”,
“bombayı açılıştan uzak alt bölgeye al; region=bottom”. Her alan için ilk öneri
sonraki tura uygulanır; tohum aralığı uzunluğu korunarak ilerler.

En fazla üç tur. Geçenler arasından süre hedefi ortasına yakınlık, sonra düşük
benzerlik sırasıyla iki finalist seçilir; aralarındaki maske <0.90 olmalıdır.
Uyumlu çift yoksa tek finalist kalır ve arama üç tura kadar sürer.
Hiç GEÇTİ yoksa en az KALDI içeren aday etiket=aday ile finalist.json'a yazılır.
Tek tek geçen, hiçbir adayda birlikte geçmeyen kural çiftleri turlar.md'de
“R07 ve R11 aynı anda sağlanamadı” biçiminde yazılır; bunun yalnız gözlenen
aday örneklemi için geçerli olduğu belirtilir.
