# PM_105 RAPOR: zincirli parçada lastik, engel sesleri, tek düğme sesi

PM_104 ve PM_104_EK bu işten önce bitmişti (`docs/art/pm104/RAPOR.md`, `SAFARI_SES.md`).

## 1. Zincirli parçada lastik hatası

### Sebep (kayıtlar ve koddan)

- Zincirli parça kurallarda **tek, katı bir parça**: iki hücresi var, zincir yalnız görüntü (`Contact`: "ilk temas ikisini de durdurur").
- Lastik takılma (PM_93/94) parçanın tamamını ölçekliyordu. Ezilme, uzama ve yana genişleme, iki ucu kapsayan büyük kutunun kenarına göre yapılıyordu.
- Uçlar birbirinden ne kadar uzaksa fırlama o kadar büyüyordu: uzak uç ezilme × aradaki mesafe kadar kayıyordu. Yana genişleme de onu yana itiyordu ("sağa doğru sıçrayarak gidiyor").
- Boşluk büyüdükçe çarpma hızı, dolayısıyla uzama da büyüyor. Zeyd'in "boşluk varken iyice uçuyor" demesi bundan.
- 5. bölümde zincir yok, tek hücreli parçalar: kutu hücrenin kendisi, bu yüzden orada düzgündü.

### Düzeltme

- Zincirli parçada her hücre **kendi ön kenarına göre** eziliyor ve uzuyor. Parçanın tamamı yalnız kayıyor (mesh tepe noktaları; `PieceView.SetCellScale`).
  - Çarpan uç tam eziliyor, öbür uç %30.
  - İkisi aynı mesafeyi gidiyor. Zincir boyu sabit, uç fırlamıyor, yana kaymıyor.
- Gidilen en uzak nokta ilk temas (`Contact.Distance`, iki ucun en yakını). Yer yoksa parça olduğu yerde eziliyor, temasın %8 içine girmiyor (%1).
- Geri sekmede evi aşma en çok %6 (tek hücreli parçada lastik 2: %8).
- Temasta zincir sarsılıyor (ip simülasyonuna tekme) ve "çın + şıkırtı" çalıyor (§2).
- Çıkışta da (tahtadan uçarken) zincirli parça hücre hücre büyüyor.
- Tek hücreli parçalarda hiçbir şey değişmedi (5. bölüm aynı).

### Ölçüm

PlayMode `Pm105ChainKnockTests`, 6 durum. Tahta 6×8, zincir (1,0)–(4,2).

- **Her karede ölçülenler:**
  - temas sınırının geçilip geçilmediği;
  - iki ucun ön kenarları arasındaki fark (fırlama);
  - yana kayma;
  - geri aşma.

| Durum | Temas | En ileri | Uçlar arası fark | Yana | Geri aşma |
|---|---|---|---|---|---|
| boşluksuz, temas yakın | 0 | 0,010 | 0,000 | 0,000 | 0,003 |
| 1 boşluk, temas yakın | 1 | 1,000 | 0,000 | 0,000 | 0,060 |
| 3 boşluk, temas yakın | 3 | 3,000 | 0,000 | 0,000 | 0,060 |
| boşluksuz, zincir ucu yakın | 0 | 0,010 | 0,000 | 0,000 | 0,003 |
| 1 boşluk, zincir ucu yakın | 1 | 1,000 | 0,000 | 0,000 | 0,060 |
| 3 boşluk, zincir ucu yakın | 3 | 3,000 | 0,000 | 0,000 | 0,060 |

(hücre cinsinden)

- **Eski yöntemle aynı durumlar:** uçlar arası fark **0,76 hücre**, aynı tahtada. Bu tahtada uçlar 3 satır arayla.
  - Zeyd'in 28. bölümündeki zincirde uçlar 4–5 satır ayrı; fark orada 1–1,3 hücre olurdu.
- **GIF:** `zincir_once_sonra.gif`.
  - Testin ölçtüğü karelerden çizildi, 4 kat yavaş.
  - Solda önce, sağda sonra; iki durum.
  - Kareler `zincir_kareler.json`'da.

**PM'in "öbür uç yerinden oynamaz" maddesi:** oyunun kuralında iki uç birlikte hareket ediyor. Öbür uç bu yüzden çarpan uçla aynı mesafeyi gidip geri geliyor, kendi başına sıçramıyor. Bu kuralı değiştirmek seviyeleri değiştirirdi; dokunmadım.

## 2. Yeni sesler (CC0, 3'er aday)

Hepsi `Tools/make_sfx105.py` ile üretildi. Lisanslar `LISANS.md`'de. Web'de ham örnek olarak gidiyor (PM_104_EK: Safari çözme yapmıyor).

**Düzeyler**, oyunun kendi sesine göre (en yüksek 400 ms):

| Ses | Düzey |
|---|---|
| zincir şıkırtısı | kaydırmanın 9 dB altı (−24 LUFS) |
| zincir gerilme | çarpma düzeyinde, oyunda 3 dB altında çalıyor |
| kilit dokunma | çarpmanın 2 dB altı |
| kilit açılma | kaydırma düzeyi |
| patlama | çarpmanın 1 dB üstü |
| duvar | çarpmanın 1,5 dB üstü |

| Olay | a | b | c | Oyunda |
|---|---|---|---|---|
| Zincir şıkırtısı | zincir halkaları (rubberduck) | anahtar destesi | toka + metal ufaklık (Kenney) | sürüklerken ve sallanırken |
| Zincir gerilme | ince metal çın + zincir | hafif metal vuruş + toka | metal tını + anahtar | zincirli parça takılınca, çarpmayla birlikte |
| Kilitli parçaya dokunma | kilitli kapı kolu (cough-e) | kilit tıkırtısı | metal mandal | kilitli parçaya basınca |
| Kilit açılma | anahtar + metal düşüş | kapı kilidi açılır + düşüş | asma kilit açılır | kilit açılınca (eskisi kırılma sesiydi; `&seskilitac=0`) |
| Bomba patlaması | yumuşak patlama + pof | kısa bang, oyuncak | konfeti patlaması | bomba patlayınca, 0,45 sn sonra kayıp sesi |
| Kırmızı duvar | ahşap bonk | ahşap vuruş | yumuşak ağır vuruş | kırmızı duvara çarpınca, çarpma yerine |

**Şıkırtının hıza bağlılığı:**

- Zincirin hızı her karede ölçülüyor (ip halkalarının ortalama hızı).
- Zincir her yarım hücre ilerleyişte bir şıkırtı çalıyor; hızlandıkça sıklaşıyor.
- Ses düzeyi saniyede 1,2 hücrede %25, 9 hücrede %100.
- Duran zincir sessiz.

**Patlamanın yumuşaklığı:**

- 150 Hz altı kesildi, tiz uç yumuşatıldı. Patlama yarım saniyeden kısa.
- 120 Hz altındaki enerji payı test ediliyor.

**Dinleme sayfası:**

- Altı yeni bölüm, her adayda "Oyunda dene".
- Bağlantılar: zincir, patlama ve duvar için 31. bölüm; kilit için 3. bölüm.
- Zincirde "Sürükle" düğmesi: oyunun şıkırtı mantığıyla hızlanıp yavaşlayan bir sürüklemeyi çalıyor.
- Varsayılan hepsinde **a**. **Zeyd seçecek.**

## 3. Tek düğme sesi; "çıt" kalktı

**Zeyd'in kaydı (`menu_sesleri.mp4`):**

- 56 ses başlangıcı oyunun dosyalarıyla karşılaştırıldı (çapraz ilinti).
- Menüde ve bölüm düğmesinde çalan, `ui-tick-b`'nin dört kaydı:
  - eşleşme 0,81–1,00;
  - tepe −1 dB, UI'nin en yüksek sesi;
  - her basışta başka kayıt ve ±%4 perde. "Değişerek kötü geliyor" bu.
- Durdur'da çalan `ui-tok-a` (eşleşme 0,93–1,00). Zeyd'in beğendiği ses.

**Yapılan:**

- Bütün düğmeler artık **`ui-tok-a1`**: tek kayıt, perde sabit (1,0), düzey sabit.
  - Oyna ve Bölüm düğmesi;
  - alt sekmeler;
  - Süre doldu panelinde altınla ve reklamla devam;
  - kazanma düğmeleri;
  - sandık;
  - Ayarlar;
  - Koleksiyon kartları;
  - X;
  - booster seçimi;
  - reklam düğmeleri.
- Kendi sesi kalanlar:
  - anahtarlar: anahtar sesi;
  - pasif düğmeler: tok;
  - panel vuşu: tokun 9 dB altında.
- Sandık açılışı, ödül uçuşu ve satın alma onayının ayrı bir sesi yoktu. Dokunuşları artık tok.
- Eski "çıt" dosyaları projeden silindi (`pm98/ui-tick-*` 10 dosya, `pm95/tik-*` 10 dosya). Dinleme sayfasındaki kopyaları ve "Düğme tıkı" bölümü de silindi.
  - UI'nin eski yedeği (`pm72-tick`) artık düğmeden çağrılmıyor.
  - Kodda `ui-tick` geçmiyor (test).
- Tablo: `docs/TITRESIM_HARITASI.md`, ses sütununda dosya adları. "Tık" 0 satırda.

## Doğrulama

(teslimde dolduruldu, aşağıda)
