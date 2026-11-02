# PM → Claude Code (21 Ekim 2026, 16:25) — PM_MESAJ_5 EKİ (SET40 v2'ye dahil)

## Zeyd'in geri bildirimi (AYNEN, özet)

> "Bomba, çivi ya da zincirli parça hep en ortada duruyor. Mesela upuzun bir zincirli parça da olabilir, bir uçtan öbür uca veya ortaya giden. Ya da en uçta, üstünde 4 yazan bomba hiç zorlamıyor, ne yapacağım belli (skillful isem; değilsem ekrana bakmadan başlayıp ölebilirim). Homojenlik iyi. Puzzle'ı zorlaştırmanın yöntemini bulduk aslında: bir bombayı çıkarmak için birbirine bağlı o bir sürü parçayı hangi sırayla çözeceğini kullanıcıya sordurmak."

## Kararlar

### 1. Asıl zorluk kaynağı (tasarım ilkesi)

Zorluk, bombayı serbest bırakmak için birbirine kilitli parçaları **hangi sırayla** çözeceğini bulmaktan gelir. Engelin tahtadaki konumu zorluk yaratmaz.

### 2. M17 — Bomba bağımlılık zinciri (YENİ, zirvelerde birincil)

- **Derinlik:** Bomba çıkmadan önce belirli bir sırayla çıkarılması gereken parça sayısı.
- **Sıra belirsizliği:** Mümkün görünen sıralar çok, işe yarayanlar az. Bunu çözücüyle ölç: rastgele geçerli sıraların yüzde kaçı bombayı fitil bitmeden çıkarıyor.
- **Hedef (zirve):** Derinlik ≥ 6, sıra başarı oranı düşük, M13 anahtar anları bu zincirin içinde.

### 3. Engel konumu çeşitlensin; M9 ve M11 gevşer

- Engeller hep merkezde toplanmasın. Kenar, köşe ve merkez arasında dağılsın; set içinde konum dağılımını ölç.
- **Kenarda bomba serbest**, ama yalnızca çıkışı derin bir bağımlılık zinciriyle kilitliyse (M17). "Kenarda, fitil 4, önü boş" türünden, okuyunca çözülen bomba YASAK. Kural: Fitil ≥ en kısa yol + 2 ve zincir derinliği < 3 olan bomba zirvede bulunamaz.
- **Uzun zincirler:** Uçtan uca ya da kenardan merkeze giden uzun zincirli parça bağlantıları kullan. Bir zincir, tahtanın iki uzak bölgesini birbirine bağlasın.

### 4. Oyuncu tipi notu (onboarding)

Zeyd şöyle diyor: "Skillful değilsem ekrana bakmadan başlayıp ölebilirim."

İlk bombalı bölümler (tanıtım slotları) oyuncuya "önce bak, sonra hamle yap" alışkanlığını öğretmeli:
- Bomba tanıtım bölümünde fitil cömert olsun.
- Zincir derinliği 2–3 olsun ve tek doğru sıra açıkça görünsün.

Süre ilk hamleyle başlıyor. Bunu tanıtım bölümünün ipucu metninde söyle: "Acele etme, süre ilk hamlende başlar."

## Teslim

- PM_MESAJ_5 teslimine ekle: SET40.md'de her zirve için M17 (derinlik, sıra başarı oranı) ve engel konumu (merkez, kenar, köşe) yazılsın.
- Token kuralları aynen geçerli.
