# BÖLÜM TASARIM KİTABI — Reverse Solver

Sürüm 0.1 · 8 Kasım 2026 · Sahibi: PM. Okuyanlar: bölüm ajanı, Claude Code (fabrika), Zeyd.

> **Ana fikir (Zeyd):** Bölüm bir hikâyedir. Oyuncuyu X'te başlatıp Y'de bitiren bir yol. Rastgele bir parça yığını değildir.

Bu kitap üç şey yapar:
1. Referans oyunlardan (Block Out, Royal Match) **hikâye kartlarını** toplar.
2. Kartlardan **ölçülebilir kurallar** çıkarır (her kuralın fabrikada bir kontrolü var).
3. Bölüm ajanının **nasıl çalışacağını** tanımlar (kart → bölüm → simülasyon → ZeydBot → Zeyd).

Kural: kitaba eklenen her kural ya bir karttan ya da pt1/playtest verisinden gelir. Kaynağı yazılır, kaynaksız kural eklenmez.

---

## 0. Hikâye kartı şablonu (her yeni bölüm için zorunlu)

Ajan bölüm JSON'u yazmadan **önce** bu kartı doldurur. Fabrika kartın iddialarını simülasyonla kontrol eder.

```
KART: <bölüm no> · <tip: tanıtım | normal | nefes | zor | super-zor> · <tek cümlelik fikir>
SÜRE: <sn> · hedef bitiş payı: <ör. normal 15–30 sn, zor 0–8 sn>
ANAHTAR: bölümün omurgası olan tek şey (ör. "ortadaki 3x2 parça", "sağdaki kilitli bölge", "fitili 6 olan bomba")
PERDE 1 — açılış: ilk görüntü, ilk hamlede kaç anlamlı seçenek var, oyuncu ne hissediyor
PERDE 2 — düğüm: hangi anda tek parça ilerletiyor (zincir kaç halka), engel nerede gerçek tehdit
PERDE 3 — kırılma: anahtar çözülünce kaç parça serbest kalıyor
FİNAL — çığ: son %30 parça yaklaşık kaç sn'de çıkıyor
YENİ ÖĞE: (varsa) bu bölüm neyi öğretiyor / büküyor
ŞEKİL: tahta silueti (önceki 5 bölümden farklı olmalı)
BEKLENEN OYUNCU: iyi oyuncu / ortalama / yeni başlayan ne yapar, nerede takılır
```

---
## Bölüm A — Block Out 273–278 hikâye kartları

Kaynak: `Level Design/kareler/blockout/` (21 temas sayfası, 2 sn aralık). Block Out: süreli, renkli blokları aynı renk kapıdan dışarı kaydırma. Bizim oyuna en yakın tür: **çıkarma + süre + engel**.

Oyunun temel araçları (kareden okunan):
- **Sayaçlı kapı:** her kapının üstünde "kaç blok daha geçecek" sayısı. Sıfırlanınca kapı kaybolur ya da renk değiştirir → çıkış haritası bölüm içinde değişir.
- **Örtülü panel (kilitli bölge):** tahtanın bir kısmı mor perdeyle kapalı, üstünde sayaç (ör. 7). Başka yerden N blok çıkınca perde kalkar, altındaki yeni bölüm görünür. **Zeyd'in "kilitli bölge" fikrinin birebir aynısı, 6 bölümün 4'ünde var.**
- **Zincir çubuğu:** büyük bir bloğu tutan sayaçlı kilit (273'te 6).
- **Yön kilitli blok:** yalnız ↕ ya da ↔ kayabilen blok (274, 277).
- **Süre hediyesi:** blok içinde +5 / +10 sn, çıkınca süreye eklenir.
- **Roket:** bloklar çıktıkça dolan sayaç, dolunca tahtaya kendiliğinden ateş eder (ödül hissi).

---

### BO-273 · Normal · 1:25 · 25 sn kala kazandı ("PERFECT")
- **Tahta:** dolu, 4 yan kapı + alt kapı. Ortada camgöbeği dev blok, alt kapıya bakıyor ama **6 sayaçlı zincir çubuğu** onu tutuyor.
- **1. perde, sıkışma (0:04–0:30):** yaklaşık 25 sn tahta neredeyse değişmiyor. Oynanabilir hamle az, oyuncu arıyor, küçük "10"/"5" etiketli parçaları oynatıyor.
- **2. perde, besleme (0:30–0:54):** sol taraf açılıyor, her çıkış zincir sayacını düşürüyor (6→1). Oyuncu neyi beslediğini görüyor.
- **3. perde, kırılma (0:56):** zincir patlıyor, dev blok aşağıdan çıkıyor, tahta ikiye bölünüyor.
- **Final, çığ (1:04–1:22):** kalan bloklar art arda çıkıyor, roketler ateşleniyor. Son %40'ı 20 sn'de bitiyor.
- **Ders:** bölümün **omurgası tek bir anahtar engel**. Önceki her şey sayacı beslemek, sonrası ödül/çığ.
- **Bize çevirisi:** "Anahtar parça" bölümü. Ortada büyük bir parça var, zincir sayacı N parça çıkınca açılıyor. Önce dar alanda N parça, sonra büyük parça ve arkası çözülüyor.

### BO-274 · **Hard Level** · 1:35 · ~4 sn kala kazandı (+5 sn hediye ile)
- **Tahta:** görünen alan yaklaşık %25 (sol üst köşe). Geri kalanı **3 örtülü panel** kaplıyor (sayaç 7, 10, 10).
- **1. perde, küçük oda (1:36–2:20):** az blok, dar alan, renk kapıları sayaçlı. Yaklaşık 45 sn sürüyor, yavaş ve düşündürücü.
- **2. perde, ilk açılış (2:22):** 7'lik panel kalkıyor, altından **sıkışık yeni bir bölge** ve ↕ kilitli turuncu blok çıkıyor.
- **3. perde, ikinci açılış (2:30):** uzun panel de kalkıyor, sağda üst üste yığılmış renkler ve camgöbeği blok çıkıyor.
- **Final (3:14–3:24):** camgöbeği çıkıyor, son bloklar roketle temizleniyor. **Süre 0:04.**
- **Ders:** **açılışlar = perdeler.** Her açılış "ne yapabilirim" listesini sıfırlıyor ve oyuncuya yeni bir mini bulmaca veriyor. Süre, iyi oyuncunun bile son saniyede bitireceği şekilde ayarlanmış. +5 sn hediye blok içinde saklı (kurtarma valfi).
- **Bize çevirisi:** Zeyd'in "kilitli bölge"si için ilk şablon: 3 bölge, açılma sırası sayaçla belli. Zor bölüm hedefi: iyi oyuncu 0–5 sn kala bitirsin.

### BO-275 · Normal (uzun) · 1:50 · ~13 sn kala kazandı
- **Tahta:** tamamen dolu duvar. Giriş yalnız üstten (kırmızı kapı 14, beyaz kapı 14) ve yandan küçük kapılardan.
- **1. perde, kazı (3:32–4:22, yaklaşık 50 sn = sürenin yarısı):** yalnız en üstteki tek hücrelik sarı/mor parçalar hareket edebiliyor. Kapı sayaçları yavaş düşüyor (14→9).
- **2. perde, kapı bitişi (4:14 ve 4:24):** kırmızı kapı sayacı bitince kapı patlıyor, yeni bir yan kapı ön plana çıkıyor, orta kısım çöküyor.
- **3. perde, renk değişimi (4:58–5:18):** alt kapılar yeşile/maviye dönüyor, kalan renkler hızla çıkıyor.
- **Ders:** "kazı" bölümü. Uzun ve yavaş başlangıç, çünkü **her hamle sayaçta görünür ilerleme** demek. Ortada çıkış haritası değişiyor ve tempo kırılıyor.
- **Risk:** 50 sn az ilerleme sıkıcı olabilir. Sayaçlar olmasa öyle olurdu.
- **Bize çevirisi:** tahtada ilerleme her zaman görünmeli (kalan parça sayısı, bölge sayacı). Yavaş açılışa ancak böyle izin verilir.

### BO-276 · Normal (nefes) · 1:20 (+10 sn, +5 sn) · ~25 sn kala kazandı
- **Tahta:** **merdiven/üçgen şekilli**, alışılmadık. Solda küçük bir küme, sağda **3 uzun panel (6, 6, 6)**.
- **1. perde (5:52–6:04):** 14 sn içinde 3 panel birlikte 6→1'e iniyor ve **aynı anda açılıyor**. Erken gelen büyük bir sürpriz.
- **2. perde (6:06–6:56):** açılan bölgede orta zorlukta bir düğüm var. Köşedeki mavi-sarı T parça üst kapıyı kapatıyor, en sona kalıyor.
- **Final (6:58–7:06):** roketler + son parçalar.
- **Ders:** iki zor bölümden (274, 275) sonra gelen **nefes bölümü**. Erken açılış oyuncuya "hediye" gibi geliyor. Tahta şekli bölüme kimlik veriyor.
- **Bize çevirisi:** zorluk eğrisinde zor bölümlerden sonra kısa, ferah, şekliyle akılda kalan bir bölüm. Erken açılış iyi bir açılış kancası.

### BO-277 · Normal (merdiven) · 1:45 · ~34 sn kala kazandı ("PERFECT")
- **Tahta:** üçte ikisi örtülü. Üst üste 3 panel (14, 9, 5), çalışma alanı alttaki ince şerit.
- **Akış:** paneller **küçükten büyüğe sırayla** açılıyor: 5 (7:32), 9 (7:54), 14 (8:16). Her kat 20–25 sn sürüyor.
- **Her kat bir yenilik getiriyor:** 2. katta ↔ kilitli mavi blok, 3. katta ↕ kırmızı blok ve sarı yatay sürgü.
- **Final (8:24–8:40):** yalnız mavi bloklar kalıyor, tek kapıdan hızla çıkıyor.
- **Ders:** **"merdiven" yapısı.** İlerleme öngörülebilir, her kat bir öğe ekliyor. Tanıtım bölümleri için ideal kalıp.
- **Bize çevirisi:** yeni engel tanıtımı için şablon: 1. kat engelsiz ısınma, 2. kat engel tek başına ve tehditli, 3. kat engel + bildik bir öğe.

### BO-278 · Zor · 1:30 · **SÜRE BİTTİ** → "+30 sn, 900 jeton" → sonra bitti
- **Tahta:** üstte küçük panel (2), altta büyük panel (9), arada sıkışık bir yığın. Tek çıkış sol üstteki turuncu kapı. Ortada mavi "boş kalıp" hücreleri var.
- **1. perde (8:50–8:52):** ilk saniyede küçük panel açılıyor (hediye).
- **2. perde, uzun düğüm (9:00–10:20, yaklaşık 80 sn):** büyük panel sayacı yalnız 9→8'e iniyor. Oyuncu aynı birkaç parçayı oynatıp duruyor.
- **3. perde, geç çözülme (10:24–10:46):** düğüm çözülüyor, sayaç 22 sn'de 7→1'e iniyor, kapılar renk değiştiriyor, süre dondurma kullanılıyor. **Yine de 0:00.**
- **Sonuç:** "Out of Time +30 sec" teklifi geliyor, satın alınıyor, bölüm bitiyor.
- **Ders:** zor bölüm **"neredeyse başardım" kaybına** göre ayarlanmış. Asıl çözüm (düğüm) geç bulunuyor, bulununca her şey hızlanıyor. Para kazanma burada devreye giriyor.
- **Bize çevirisi:** bizde satın alma yok ama duygu aynı olmalı. Kaybeden oyuncu "biraz daha süre olsa bitiyordu" demeli, "imkânsız" dememeli. Simülatör ölçütü: kaybedenlerin kalan parça oranı düşük olmalı.

---

### Block Out'tan çıkan 8 kural
1. **İlerleme hep görünür.** Kapı, panel, zincir: her engelin sayacı var. Oyuncu yavaş anlarda bile neyi beslediğini biliyor.
2. **Örtülü bölge = perde.** 6 bölümün 4'ünde var. Açılış sırası sayaç büyüklüğüyle belirleniyor. Her açılış yeni bir mini bulmaca.
3. **Tek anahtar.** Her bölümün bir omurgası var (zincir, en büyük panel, tek kapı). Bölümün hikâyesi o anahtarın etrafında dönüyor.
4. **Sıkışık başla, çığla bitir.** İlk %20–50 yavaş ve düşündürücü, son %30–40 10–20 sn'de akıyor. Bitiş hep hızlı ve ödüllü (roket, patlama).
5. **Süre bütçesi bölüm tipine göre.** Normal: 13–35 sn kala biter. Zor: 0–5 sn kala ya da kayıp. Süre hediyeleri (+5/+10) blokların içine saklanıyor.
6. **Zorluk dalgası:** N(273) → **Z**(274) → N-uzun(275) → nefes(276) → N(277) → **Z**(278). Zor bölüm yaklaşık her 3–4 bölümde bir ve hep arkasından nefes bölümü geliyor.
7. **Çıkış haritası bölüm içinde değişir.** Kapı biter, renk değiştirir, yenisi açılır. Aynı tahta ikinci yarıda başka bir bulmacaya dönüşüyor.
8. **Şekil kimliktir.** Merdiven, U, haç: her bölümün tahta silueti farklı. "Birbirine benzeyen bölüm" sorununa doğrudan çözüm.

---

## Bölüm B — Royal Match 3473–3477 hikâye kartları

Kaynak: `Level Design/kareler/rm/` (18 temas sayfası). Royal Match süreyle değil **hamle bütçesiyle** çalışır. Bizdeki "süre"nin karşılığı hamle sayısı. Tür bize uzak ama **hedef yönetimi, gizli hedef ve zorluk etiketi** dersleri doğrudan uyuyor.

Her bölümün başında **Butler's Gift** ve seçili güçlendiriciler tahtaya patlıyor: oyuncu her bölüme bir "açılış hediyesi"yle başlıyor.

---

### RM-3473 · Normal · 31 hamle · **8 hamle kala** kazandı
- **Hedef:** 4 tür (halka 40, ayıcık 6, yumurta 12, kemik 15).
- **Tahta:** solda 6 büyük buz bloğu (altında ayıcıklar gizli), ortada katmanlı kurabiye/pati karoları, altta kilitli dolap kapakları (altında halkalar).
- **1. perde (31→27):** hediye patlamaları ve büyük kombo, tahtanın yarısı ilk 4 hamlede açılıyor. "Güçlüyüm" hissi.
- **2. perde (27→13):** buzlar kırılınca ayıcıklar görünüyor ve tek tek düşürülüyor. Hedefler paralel azalıyor.
- **Final (13→8):** sol alt köşedeki son ayıcık ve son dolap, tahtanın en kör noktası. Son hedef bilerek oraya konmuş.
- **Ders:** son hedef **en ulaşılmaz köşeye** saklanıyor, bölümün "son düğümü" o oluyor. Normal bölüm rahat bir payla bitiyor (%25 hamle artıyor).

### RM-3474 · Normal · 33 hamle · **18 hamle kala** kazandı
- **Hedef:** mor halı 24, çiçek 24, vazo 28, ayıcık 16.
- **Tahta:** dolu ama simetrik, 4 TV kutusu (içinde ayıcık), halı yayılma mekaniği.
- **Akış:** 2:16–2:22 arasında üst üste kombolar (disko + pervane). Halı patlayarak yayılıyor, 3 hedef birlikte çöküyor.
- **Ders:** iki zor bölümün arasındaki **ödül bölümü**. Kombo zinciri oyuncuya "usta" hissi veriyor. Bütçenin yarısı artıyor.

### RM-3475 · **Super Hard** · 34 hamle · **3 hamle kala** kazandı
- **Hedef:** kuş 16, ördek 18, balkabağı 12.
- **Tahta:** **haç şekli**. Merkez küçük, kollar zincir kafesli pembe sütunlar ve iksir rafları. Köşelerde kazanlar var.
- **1. perde (34→30):** dolap ve zincir sütunlar kırılıyor, haç açılıyor. Hızlı ve ödüllü.
- **2. perde (30→10), damla:** balkabakları kazanlardan **damla damla** doğuyor. Oyuncu hızlansa bile hedef ancak üretildikçe toplanabiliyor. Tempo tasarımcının elinde.
- **Final (10→3):** son 1–2 balkabağı 3 hamle kala geliyor.
- **Ders:** **damla hedef = gerilim kontrolü.** Hedefin bir kısmı baştan tahtada değil, sonradan doğuyor. Gerilim sona kadar korunuyor, "erken çözdüm, gerisi angarya" anı olmuyor. Super Hard etiketi oyuncuyu baştan uyarıyor.

### RM-3476 · Normal (sıkı) · 30 hamle · **1 hamle kala** kazandı
- **Hedef:** altın ördek 2, çuval 14, mavi su karosu 7.
- **Tahta:** T biçimli su kanalları, kenarlarda çuval sütunları, altta **iki büyük ördek küveti** (dolunca altın ördek heykeline dönüşüyor).
- **1. perde (30→20):** su kanalları ve çuvallar. Ekran kalabalık, ilerleme yavaş.
- **2. perde (20→16):** sağ küvet doluyor ve 6:04'te heykele dönüyor. **İlk büyük ödül anı** bölümün ortasında.
- **Final (16→1):** sol küvet daha derin ve daha kör. Son karo/ördek son hamlede geliyor.
- **Ders:** **iki eş hedef, farklı zorlukta.** Birincisi öğretiyor ("böyle doluyor"), ikincisi sınıyor. Kolay olan orta noktada ödül olarak bitiyor, zor olan finali taşıyor.

### RM-3477 · Normal (iki ekran) · 33 hamle · **son hamlede (0)** kazandı
- **1. ekran:** tuğla duvar ve **dinamit sayacı 25**. Mor raflardaki sarımsakları temizle, sayaç düşsün. Hediye kombolarıyla 4 hamlede 25→0, duvar yıkılıyor.
- **2. ekran (kamera kayıyor):** yeni tahta ve **yeni hedefler** (bowling lobutu 18, mor yaratık 12, bal 36). Hedefler tamamen değişiyor.
- **2. ekran içi:** bal (turuncu katman) temizlendikçe metal **çekmeceler açılıyor**, içinden lobut kutuları çıkıyor (8:22). Gizli hedef ortada açığa çıkıyor.
- **Final:** son lobut ve son yaratık 0. hamlede düşüyor.
- **Ders:** **"sahne değişimi".** Bölüm iki ayrı bulmaca. 1. ekran kısa bir ısınma ve "duvarı yıktım" ödülü, 2. ekran asıl sınav. Ortadaki çekmece açılışı ikinci bir mini perde.

---

### Royal Match'ten çıkan 7 kural
1. **Her bölüm hediye patlamasıyla açılıyor.** İlk 2–4 hamle oyuncuyu güçlü hissettiriyor, sıkıntı sonra geliyor.
2. **Son hedef en kör köşede.** Final kendiliğinden bir "son düğüm" oluyor.
3. **Gizli hedef ortada açılıyor:** buzun altında ayıcık, kutuda ayıcık, çekmecede lobut. Tahta oynadıkça "yeni bilgi" veriyor.
4. **Damla hedef:** bir kısım hedef sonradan doğuyor, gerilim sona kadar sürüyor.
5. **Pay bölüm tipine göre:** normal %25–55 artıyor (8, 18 hamle), sıkı/zor 0–3 hamle artıyor. 5 bölümün 3'ü son 3 hamlede bitti.
6. **Zorluk etiketi** ("Hard", "Super Hard") oyuncunun beklentisini ayarlıyor. Zorluğu "haksızlık" değil "meydan okuma" yapıyor.
7. **Sahne/ekran değişimi** (3477) ve **eş hedef ikilisi** (3476): bir bölümde iki küçük hikâye.

Yazılı kaynaklar da aynı ritmi söylüyor: Royal Match'in ilk 100 bölümünde 2–3 öğretme bölümü → zor sıçrama → bazen super hard tepe → bonus döngüsü var. Normal bölüm ~1,2, zor ~1,6, super hard ~2,5 deneme. Hamle bütçesi 30–38'den 23–27'ye düşüyor (Gamigion).

---

## C. Reverse Solver kuralları (iki oyunun ortak dersi → bizim ölçüt)

Her kuralın yanında fabrikanın ölçeceği **kontrol** var. "Seçenek" = o anda çıkarılabilir ve çözüm yolunu bozmayan parça sayısı (çözücü hesaplar).

| # | Kural | Kaynak | Fabrika kontrolü |
|---|---|---|---|
| K1 | **Tek anahtar.** Her bölümün bir omurgası var, kartta yazılı. | BO-273, 278 · RM-3473 | Anahtar parça çözüm sırasının %30–70 arasında açılıyor. Açılınca seçenek sayısı en az 2 katına çıkıyor. |
| K2 | **Hikâye eğrisi.** Seçenek sayısı adım adım çizilince düz olmamalı: dar → genişle → dar (düğüm) → çığ. | BO genel | Eğride en az 1 "boğaz" (seçenek = 1, en az 2 ardışık adım) ve en az 1 "çığ" (son %30'da seçenek en az 3) var. |
| K3 | **Zincirleme tek parça (Zeyd'in favorisi).** Çok parça görünür ama ilerletebilen yalnız bir tanesi; bu birkaç kez arka arkaya. | Zeyd, Z12 · BO-275 kazı | Zor bölümde art arda seçenek = 1 olan adım sayısı en az 3. Normalde 1–2. Tanıtımda 0–1. |
| K4 | **Açılış hediyesi.** İlk 2–3 hamle kolay ve tatmin edici. Zorluk sonra gelir. | RM hepsi · BO-276, 278 | İlk 3 adımda seçenek en az 2. İlk hamlede "yanlış" (kilitleyen) seçenek oranı en fazla %30 (zor hariç). |
| K5 | **Çığla bitir.** Son parçalar hızla, art arda çıkar. | BO-273, 274 · RM-3474 | ZeydBot simülasyonunda son %30 parça toplam sürenin en fazla %20'sinde çıkıyor. |
| K6 | **Süre payı tipe göre.** | BO süreleri · RM hamle payları | Medyan oyuncu botuyla kalan süre: tanıtım/nefes %30–45, normal %15–30, zor %0–10, super-zor kazananların yarısı 5 sn'den az. |
| K7 | **Neredeyse kaybı.** Zor bölümde kaybeden "az kaldı" demeli. | BO-278 | Kaybeden bot denemelerinde kalan parça oranının medyanı en fazla %20. |
| K8 | **İlerleme hep görünür.** Kilitli bölge, zincir, bomba: sayaç ekranda. | BO genel | Kod/görsel kontrolü: her sayaçlı engelin sayısı HUD'da veya üzerinde. |
| K9 | **Gizli/geç gelen bilgi.** Orta ve ileri bölümlerin yarısında tahta oynadıkça yeni bir şey açılmalı (kilitli bölge, alttan gelen parça). | BO-274/277 · RM-3473/3477 | Set başına: 20. bölümden sonra bölümlerin en az %40'ında "açılan alan" var (p21 mekaniği gelince). |
| K10 | **Şekil kimliktir.** | BO-276, 277 · RM-3475 | Ardışık 5 bölümde tahta silueti benzerliği eşik altında (`level_similarity.py`'ye siluet + eğri benzerliği eklenecek). |
| K11 | **Zorluk dalgası.** Zor bölümden sonra nefes bölümü. | BO 274→276 · RM döngüsü | 40'lık sette: her 4–5 bölümde 1 zor, her 10'da 1 super-zor (bölüm 9, 19, 29, 39 civarı), zor/super-zorun hemen ardından nefes. |
| K12 | **Zorluk etiketi.** Oyuncu zor bölüme girerken bunu bilmeli. | BO "Hard Level" · RM "Super Hard" | UI: zor/super-zor bölüm kartında rozet (Baron kartı zaten var, PM_36). |

---

## D. Yeni engel tanıtımı: 4 adımlı tarif (tanıt → geliştir → bük → bitir)

Nintendo'nun kishōtenketsu yapısı (tanıt, geliştir, bük, bitir) ile BO-277'nin "merdiven" yapısı birleşimi. Yeni engel **tek bölümde değil 4 bölümlük bir yayda** öğretilir:

| Yay bölümü | Ne olur | Kart notu |
|---|---|---|
| **Tanıt** (ör. 6) | Engel tek başına. Ekranda başka zorluk yok. İlk 3 hamle içinde **gerçek tehdit**. | Bomba: fitil ≤ çözüm yolundaki ilk N hamle + 2 (PROJE_DURUMU 6c). Oyuncu bombayı yok sayarsa **kaybedebilmeli**. |
| **Geliştir** (7) | Aynı engel 2–3 kez veya daha büyük ölçekte. Bildik bir öğeyle birlikte. | Bomba + duvar. Fitiller farklı uzunlukta, sırayı oyuncu seçer. |
| **Bük** (8–9) | Engel beklenmedik bir rolde: tehdit değil **anahtar**. | Ör. bomba patlayınca bir duvarı yıkıyor ve yol açıyor. Ya da çivi tam anahtar parçayı tutuyor. |
| **Bitir / nefes** (10) | Engel geri planda, bölüm ferah. | Sonraki yeni öğeye kadar engel "bilinen alet" olur. |

**Mevcut sorun (Zeyd, 8 Kasım):** bomba tanıtımında fitil 25, oyuncu her şeyi çıkarıyor, bomba hâlâ 4'te → tehdit yok. Bu tarifle tanıtım bölümü **bombayı unutan oyuncuyu kaybettirmeli**. Çivi tanıtımı da aynı şekilde "Bük" adımıyla netleşir: çivi = "bu parça şimdilik çıkmaz, önce şunu çöz".

---

## E. Bize gelecek yeni mekanikler (referanslardan kanıtlı)

1. **Kilitli bölge (öncelik 1).** Block Out'ta 6 bölümün 4'ünde var. Bölge sayaçlı perdeyle örtülü, N parça çıkınca açılıyor. Kullanım biçimleri: tek büyük bölge (274), aynı anda açılan üçlü (276), merdiven sırası (277), erken açılış hediyesi (278). → p21 prototipi.
2. **Alttan/sonradan gelen parça (damla).** RM-3475 kazan mantığı: hedefin bir kısmı baştan tahtada değil. Gerilimi sona kadar taşır.
3. **İki sahneli bölüm.** RM-3477: kısa ısınma tablosu bitince kamera yeni tabloya kayıyor. Hikâyemizle uyumlu: "Baron'un tablosunun arkasından ikinci tablo çıktı".
4. **Süre hediyesi parçası.** BO +5/+10 sn: bir parça çıkınca süre ekler. Zor bölümlerde kurtarma valfi olur, K6'yı ayarlamayı kolaylaştırır.

---

## F. Bölüm ajanı bu kitapla nasıl eğitilir (plan)

"Eğitim" = modeli yeniden eğitmek değil. Ajana **doğru bağlam + zorunlu kart + otomatik eleştiri döngüsü** vermek. Ajan oynayarak öğrenemez ama **her bölümün ölçülmüş sonucunu** okuyarak kitabı büyütebilir.

**Adım 1 — Ölçüm aracı (önce bu, PM_MESAJ_47):** fabrika çözücüsüne "hikâye eğrisi" çıktısı eklenir: adım başına seçenek sayısı, boğaz sayısı, anahtar anı, çığ oranı, bomba tehdit payı. SET40 v4'ün 40 bölümü ölçülür. Böylece "bazı bölümler birbirine benziyor" ve "tanıtım zayıf" sorunları **sayıyla** görünür. Kitabın kuralları gerçek verimizle kalibre edilir (K2–K6 eşikleri tahmin, bununla düzelir).

**Adım 2 — Kendi kartlarımız:** en iyi 5 bölümümüz (Z12 ve Zeyd'in sevdikleri) ile en zayıf 5'imiz için kart yazılır, eğrileri eklenir. Ajanın few-shot örnekleri bunlar olur: "iyi örnek" ve "kötü örnek + neden". Referans oyun kartları ilham, kendi kartlarımız **ölçü** olur.

**Adım 3 — Ajan talimatı (`Tools/ajan/BOLUM_AJANI.md`):** sistem metni = bu kitabın A–E bölümleri (kısaltılmış) + 6 örnek kart. Ajanın görevi sırayla:
1. Kart yaz.
2. Kartı karşılayan bölüm JSON'u üret (3 aday).
3. Fabrikaya gönder.

**Adım 4 — Doğrulama döngüsü (otomatik):**
- Fabrika çözülebilirlik + hikâye eğrisini ölçer.
- **Eleştirmen ajan** kartla eğriyi karşılaştırır: "kart 'perde 2'de 3 halkalı zincir' diyor, eğride boğaz 1 adım → RED, sebep: …".
- Ajan en fazla 3 tur düzeltir. Geçenleri ZeydBot oynar (K6, K7 süre/kayıp kontrolü). Benzerlik kontrolü (K10).
- Çıktı: bölüm başına tek sayfa "kart + eğri + bot sonucu".

**Adım 5 — Zeyd onayı:** finalistleri oynar, 1 cümle not yazar ("tehdit yok", "çok güzel düğüm"). Not kartın altına eklenir.

**Adım 6 — Gerçek veriyle kapanış:** p21'de yayınlanan bölümlerin pt verisi (`v_pred_vs_actual`, bırakma noktaları) karta "veri ne dedi" satırı olarak yazılır. Tutmayan kural düzeltilir, işe yarayan yeni bir desen kural olarak eklenir. Kitap böyle büyür. Ajan her yeni üretimde güncel kitabı okur.

**İlk hedef:** 3/6/10/14 tanıtım bölümlerini D tarifine göre yeniden yapmak (p21). Ardından kilitli bölge prototipiyle 4 bölümlük bir yay.

---

## Kaynaklar
- Ekran kayıtları: `Level Design/ScreenRecording_11-08-2026 01-42-26_1.MP4` (Royal Match), `…01-52-13_1.MP4` (Block Out). Kareler `Level Design/kareler/`.
- [Gamigion — Royal Match level design insights](https://www.gamigion.com/royal-match-level-design-insights/) (zorluk döngüsü, deneme sayıları, hamle bütçeleri)
- [MCV/Develop — Nintendo's level design secrets in four steps](https://www.mcvuk.com/development/video-nintendos-level-design-secrets-in-four-steps) (kishōtenketsu: tanıt-geliştir-bük-bitir)
- [Naavik — Royal Match deep dive](https://naavik.co/deep-dives/royal-match/)
