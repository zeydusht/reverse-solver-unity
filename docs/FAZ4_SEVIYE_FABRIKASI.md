# Faz 4 — Seviye fabrikası ve 40 seviyenin yeniden tasarımı

Yazan: Zeyd'in PM'i (claude.ai), 2026-10-13 18:30.

Zeyd'in isteği, özetle:

> Bir sürü seviye üretilsin. Monte Carlo ile defalarca çözülsün; istenen zorluk yüzdelerine uyanlar kalsın, gerisi silinsin. Seviyeler oyuncu psikolojisi grafiklerine göre tasarlansın. Ayrı bir "psikolog" bunları elesin: farklı oyunculardan oluşan 1000 kişi oynasa 900'ü keyif alsın. Ajanlar işi birbirine paslasın, en sonda gerçekten keyif veren bölümler Zeyd'e sunulsun. Zeyd oynasın, onayladığı seviye canlıya alınabilir olsun. App Store şimdilik yok.

---

## Zeyd'in kararları (PM bu dosyayla kayda geçirir)

1. **App Store ertelendi.** `docs/M7_IOS.md` backlog'a gider. Herkese açık yayın konusu yeni set ve playtest bittikten sonra yeniden açılır.
2. **Sıra değişiyor: önce Faz 4, sonra playtest.**
   - Playtest eski 40 seviyeyle değil, fabrikanın ürettiği **yeni setle** yapılır.
   - Eski setin karşılaştırma verisi web verisinden gelir.
   - Gerekçe: playtest, fabrikanın tahminlerini (zorluk ve keyif) gerçek oyuncularla doğrulayan deney olur. Case study'nin ana hikâyesi bu.
3. **Seviyeyi canlıya yalnızca Zeyd'in onayı alır.**
   - Fabrika, ajanlar ve PM yalnızca aday sunar.
   - Yeni setin oyunda varsayılan olması yalnızca Zeyd'in açık komutuyla olur ("yeni seti canlıya al").
4. **Unity'yi yeniden başlatma ve batchmode izni kalıcı.** Koşullar 2026-10-13 gece planındakilerle aynı.

---

## "Dopamin" ve "keyif" ne demek (dürüstlük notu)

- Dopamini ölçemeyiz. Ölçtüğümüz şey bir **keyif tahmini**:
  - eğitim setindeki psikoloji kurallarına göre yazılmış bir model,
  - bu modelle oynayan, farklı huylarda 1000 sanal oyuncu.
- "1000 kişiden 900'ü keyif aldı" bu modelin çıktısıdır. **Gerçek oyuncunun yerini tutmaz.**
- Model playtest'te doğrulanır. Her seviyenin sonunda oyuncuya kısa bir 👍/👎 sorusu sorulur (G8). Tahmin edilen keyif oranı ile gerçek 👍 oranı karşılaştırılır.
- Tutmayan kural düzeltilir. Bu karşılaştırma case study'nin sonucudur.
- Raporlarda "dopamin" değil, **"keyif tahmini"** yazılır.

---

## Fabrikanın akışı

```
Sıra planı (40 slot) ──► Üretici ──► Zorluk elemesi ──► Süre/fitil/çivi ayarı ──► Keyif simülasyonu (1000 sanal oyuncu)
                              ▲                                                            │
                              │                         ┌──────────────────────────────────┘
                              │                         ▼
                     Tasarımcı ajan ◄──── ret + gerekçe ──── Psikolog ajan
                              │                         │ kabul
                              │                         ▼
                              └──────────────► PM: 40'lık sıra kontrolü ──► Onay kuyruğu ──► Zeyd oynar: onay / ret + not
                                                                                                │ onay
                                                                                                ▼
                                                                                        "Canlıya hazır" (R01–R40)
```

**Sayı işini kod yapar, ajanlar karar verir.**

- Üretme, Monte Carlo ve simülasyon editör-only C# kodudur: hızlı, deterministik ve tohumlu.
- Ajanlar (Claude Code subagent'ları) raporları okur, kuralları uygular, ret gerekçesi yazar ve üreticinin ayarlarını değiştirtir.
- Dış yapay zekâ servisi ve API anahtarı kullanılmaz.

---

## Sıra planı (40 slot)

Varsayılan plan eğitim setinden alınıyor. Zeyd isterse değiştirir.

**Yeni şeyler hangi seviyede gelir:**

| Seviye | Yeni şey |
|---|---|
| 3 | Sayılı çivi |
| 5 | Makas |
| 7 | Mühürlü kenar |
| 9 | Değnek |
| 12 | Uzak zincir |
| 15 | Saat |
| 20 | Bomba |
| 23 | Çekiç |

**Engel kuralları:**
- Engellerin sırası "öğret → pekiştir → zorla → birleştir".
- Tek yenilik kuralı: bir seviyede en fazla bir yeni şey.
- Tanıtım seviyesinde süre payı ≥ 2, bomba 2–3 hamle uzakta, bomba payı 10.

**Testere dişi:**
- İki zor seviye arka arkaya gelmez.
- Çok zor seviyeden sonra mutlaka nefes seviyesi gelir.
- Bölüm sonları (10, 20, 30, 40) zirvedir.

**İlk denemede kazanma hedefleri:**
- 1–10. seviyelerin ortalaması ≥ %85.
- 40 seviyenin ortalaması ≈ %72.

**Her slotun tanımı:** tür, yeni şey, izin verilen engeller, tahta ve gövde aralığı, `PRODUCT.md` → "Zorluk modeli"ndeki hedef bantlar. Plan `Assets/_Game/Levels/fabrika_plan.json` dosyasında durur ve PM onaylar.

---

## Mühendislik işleri

Hepsi editör-only, build'e girmez. Önerilen assembly `ReverseSolver.Generation`; Editing'in yanında, Core'a bağlı.

**G1. Üretici**
- **Çözülebilirlik yapı gereği gelir.**
  - Eğitim setindeki "dolu tahtayı soy" yöntemi: bütün dikişler kilitli başlar, koridoru boş bir parça çıkarılır ve yalnızca gereken dikişler düzlenir.
  - Kaydedilen sıra `solution` alanıdır.
  - Sonra slota göre engeller eklenir: çivi ve sayacı, uzak zincir, mühür, bomba (çekirdekte ya da kazma yolunda).
- **Doğrulama:** her aday SmartPlayer ve DesignCheck'ten geçer. 0 patlama, 0 donma, 0 supap.
- **Arama:** her slot için ayar uzayında arama yapılır (rastgele arama, ardından tepe tırmanma ya da evrimsel arama). Ayarlar: açık dikiş oranı, k₀, çekirdek derinliği, engel yerleri, zincir uçları.
- **Tekrar yok:** birbirine çok benzeyen tahtalar ayıklanır, hem slot içinde hem 40'lık sırada. Benzerlik ölçüsünü mühendislik seçer.
- **Testler:**
  - 1000 aday %100 çözülebilir,
  - aynı tohum aynı tahta,
  - kısıtlara uyuyor (boyut, gövde, engel),
  - hız ölçülüp rapora yazılır (aday/sn).

**G2. Zorluk elemesi ve kalibrasyon**
- Mevcut `StructureMetrics`, `MonteCarlo` ve `DifficultyModel` kullanılır.
- Bandın dışındaki aday elenir.
- Süre, fitil ve çivi sayacı 8. modülün formülleriyle konur: `T = ⌈g·s·P⌉₅`, fitil = en kısa kazma yolu + pay, çivi sayacı ≤ %40.
- Eğitim setinin "haksız zorluk: yapma listesi" otomatik kontrol olarak çalışır:
  - bomba payı 0,
  - süre payı 1,1'in altında,
  - süre 75 sn'den uzun,
  - 44 pt'den küçük hücre,
  - tek yenilik kuralı.

**G3. Seviye içi eğri**
- Monte Carlo yollarında ilerlemeye göre hamle başına okuma eğrisi çıkarılır.
- Bu eğri 4. modüldeki üç perdeyle karşılaştırılır:

  | Perde | İlerleme | Beklenen |
  |---|---|---|
  | Açılış | ilk %20 | kolay, k₀ türün bandında |
  | Düğüm | %30–70 | zirve |
  | Final | %70–100 | okuma düşer, baskıyı saat ve derin bomba verir |

- Çıktılar: **eğri uyumu puanı** ve zirve anı (`PRODUCT.md`'de tanımlanır, backlog maddesi kapanır).
- Rapora küçük bir eğri grafiği girer.

**G4. Keyif simülasyonu ("psikolog modeli")**
- **Sanal oyuncu nüfusu:** tohumlu 1000 oyuncu; her birinin parametreleri bir dağılımdan çekilir.
  - **Hız:** hamle başına süre ≈ a + b × bakış sayısı. Başlangıç değeri eğitim setindeki `0,6 + 0,16 × okuma`. Kişiden kişiye log-normal yayılır; σ bir varsayımdır ve rapora yazılır.
  - **Arama becerisi:** serbest parçayı bulana kadar kaç parçaya baktığı. Rastgele bakan ile iyi tarayan arasında.
  - **Bomba dikkati:** bombanın yolunu önceleme olasılığı.
  - **Sabır:** ilerlemeden geçen süreye toleransı.
  - **Tekrar isteği:** kayıptan sonra yeniden deneme olasılığı. Near-miss'te artar, erken patlamada düşer.
- Her sanal oyuncu seviyeyi en fazla 3 deneme oynar. Kaydedilenler:
  - sonuç,
  - kalan süre,
  - kalan parça,
  - en uzun takılma,
  - patlamanın kaçıncı hamlede olduğu,
  - "aha" anı: uzun bir takılmanın ardından birden çok parçanın serbest kalması.
- **Keyif kuralları:** isimli sabitler, her biri kaynağıyla (eğitim setinin 4–8. modülleri).
  - **Keyifli sayılır:**
    - türün bandında kalan süreyle kazanmak,
    - near-miss kazanmak (sürenin %10'undan azı kalmışken),
    - near-miss kaybetmek (3 ya da daha az parça kala) ve 3 deneme içinde kazanmak.
  - **Hayal kırıklığı sayılır:**
    - tanıtım seviyesinde kaybetmek,
    - 3 denemede kazanamamak,
    - ilk 5 hamlede patlamak,
    - çok parça kala süreyi kaçırmak,
    - sabır eşiğini aşan takılma.
  - **Sıkıcı sayılır:** nefes ve tanıtım dışındaki seviyelerde hiç düğüm yaşamadan, sürenin %60'ından fazlası kalmışken kazanmak.
- **Çıktılar:**
  - keyif oranı (1000 oyuncudan kaçı keyifli),
  - hayal kırıklığı oranı,
  - sıkıcı oranı,
  - ilk denemede kazanma,
  - 3. denemeye kadar kazanma,
  - near-miss oranı,
  - medyan oyuncunun yıldızı.
- **Hedef (Zeyd):** keyif oranı **≥ %90**, hayal kırıklığı **≤ %5**, ayrıca türün kazanma ve near-miss bantları. PM eşikleri slot türüne göre ince ayarlayabilir. "1000'de 900" ölçütü korunur.
- **Testler:** elle kurulmuş tahtalarla yön testleri.
  - Bariz kolay tahta, zor slotta "sıkıcı" çıkmalı.
  - Süresi imkânsız dar tahta "hayal kırıklığı" çıkmalı.
  - Near-miss'e ayarlı tahta near-miss bandına düşmeli.
  - Aynı tohum aynı sonucu vermeli.
  - 1000 oyuncu × 3 deneme süresi ölçülüp yazılır. Hedef: bir aday için ≤ 2 sn; değilse rapora yaz ve oyuncu sayısını aşamalı yap (önce 200, finalistlere 1000).

**G5. Ajanlar** (`.claude/agents/`; yalnızca okur ve karar verir; dosyaları ana Claude yazar)

- **`seviye-tasarimci`:**
  - Slotun tarifini eğitim setinin 6–7. modüllerinden kurar.
  - Üreticinin ayarlarını seçer, finalist raporlarını okur.
  - Her seviyenin "tek büyük fikir / aha" cümlesini yazar.
  - Ret gelirse ayarı değiştirip yeniden ürettirir.
- **`oyuncu-psikologu`:**
  - Keyif simülasyonunun raporunu okur; eşikleri, eğri uyumunu ve haksız zorluk listesini uygular.
  - Kabul ya da ret verir. Ret gerekçesi somut olur, örneğin "ilk 5 hamlede patlama %12, bomba çok yakın".
  - Etik çizgiyi korur: near-miss tasarımla kurulur, sonuç gizlice değiştirilmez.
- **Döngü:** slot başına en fazla 3 tur. Olmazsa slot "zor slot" olarak PM'e gider. PM ya bandı gevşetir ya da tarifi değiştirir.
- **`product-manager` (mevcut):** 40'lık sırayı kontrol eder.
  - testere dişi,
  - arka arkaya iki zor seviye olmaması,
  - nefes seviyeleri,
  - 1–10 ortalaması,
  - tekrar eden tahta olmaması.
  - Her slot için 2–3 finalisti onay kuyruğuna koyar.
- Ajan kararları `docs/fabrika/KARARLAR.md`'ye yazılır: tarih, slot, karar ve gerekçe.

**G6. Fabrika koşucusu**
- İki yoldan çalışır: batchmode komutu ve editörde "Seviye fabrikası" penceresi. Tek slot ya da 40 slotun hepsi çalıştırılabilir; ilerleme ve kalan süre görünür.
- **Çıktılar:**
  - Bütün adaylar ve raporlar `Builds/fabrika/<koşu>/` altına gider (git dışı).
  - Finalistler `Assets/_Game/Levels/redesign.json`'a gider (commit'li, `levels.json` biçiminde). Her finaliste aday kimliği, tohum, durum (aday / finalist / onaylı / ret), skorlar ve ajan notları yazılır.
- `docs/fabrika/RAPOR.md` her koşuda yenilenir:
  - slot başına finalistler ve sayıları,
  - 40'lık sıra grafiği (tür rengi, hamle başına okuma, tahmini ilk deneme kazanma ve keyif oranı).

**G7. Onay kuyruğu (Zeyd'in ekranı)**
- Editörde "Onay" sekmesi: slotlar sırayla, her slotun finalistleri bir rapor kartıyla gösterilir.
  - Kartta: ana sayılar, eğri grafiği, tasarımcının "aha" cümlesi ve psikoloğun notu.
  - Düğmeler: **Oyna ▶**, **Onayla**, **Reddet + not**.
- Onaylanan seviye kalıcı bir kimlik alır (`R01`–`R40`) ve "onaylı" olur. Reddedilen seviyenin notu tasarımcı ajana gider; o slot bir sonraki koşuda yeniden üretilir.
- Telefonda oynama: `…/br/?debug=1&set=redesign&lv=R05` ya da finalist kimliğiyle. Bunun için build ve yayın gerekir; PM uygun anda yaptırır.
- Zeyd onaylı bir seviyeyi editörde değiştirirse normal sürüm kuralı geçerli olur.

**G8. Canlıya ve playtest'e hazırlık (hazırlanır ama açılmaz)**
- **Set seçimi:** varsayılan set `redesign` yapılabilir hale getirilir, ama bayrak kapalı kalır. Açılması yalnızca Zeyd'in komutuyla.
  - `levels.json` dokunulmadan kalır (web referansı ve Golden testleri için).
  - Telemetri yeni kimliklerle (`R..`) gider, eski veriyle karışmaz.
- **Keyif sorusu:** bölüm sonunda tek dokunuşla 👍/👎 (bayrak arkasında, kapalı).
  - Gerekli Supabase değişikliği için SQL `docs/sql/004_keyif.sql` olarak hazırlanır. Boş bırakılabilen tek bir sütun eklenir.
  - **SQL'i Zeyd çalıştırır.** Mühendislik Supabase şemasına dokunmaz.
  - Bayrak, SQL çalıştırılıp doğrulandıktan sonra Zeyd'in onayıyla açılır.
- `Tools/verify_build.js` `redesign` setini de kontrol edecek şekilde güncellenir.

---

## Kabul ölçütleri (Faz 4 fabrika)

1. G1–G4 testleri yeşil (Unity Test Runner ya da batchmode). `levels.json`, Golden ve D03 değişmez.
2. 40 slotun her birinde en az 2 finalist var.
   - Her finalist: çözülebilir; bandın içinde; haksız zorluk listesi temiz; keyif ≥ %90 ve hayal kırıklığı ≤ %5 (ya da PM'in gerekçeli eşiği).
   - Tutmayan slotlar rapor edilir.
3. 40'lık sıra testere dişi kurallarına uyar. 1–10'un tahmini ilk deneme kazanma ortalaması ≥ %85.
4. Her finalistin ajan kararı ve gerekçesi `KARARLAR.md`'de, sayıları `RAPOR.md`'de.
5. Onay kuyruğunda Zeyd bir finalisti oynayıp onaylayabilir. Onaylanan `redesign.json`'da `R..` kimliğiyle "onaylı" olur.
6. Canlıya alma ve 👍/👎 bayrakları kapalı. SQL dosyası hazır, çalıştırılmadı.
7. **Bağımsız inceleme:** işi yapmamış bir alt ajan, fabrikanın 5 rastgele finalistini oyun kurallarıyla yeniden kontrol eder (çözülebilirlik, süre, fitil) ve keyif simülasyonunu tohumla yeniden üretir.

---

## Çalışma kuralları

- Önceki gece kuralları aynen geçerli:
  - her adım testler yeşil → commit → push ile biter,
  - 45 dk kutusu,
  - Zeyd'e soru yok, kararları PM verir,
  - `levels.json`, Golden, `web-reference` ve Supabase şemasına dokunulmaz,
  - force push yok,
  - repoya anon anahtar dışında anahtar girmez.
- Sıra: G1 → G2 → G3 → G4 → G5 → G6 → 40 slot koşusu → G7 → G8 → bağımsız inceleme → rapor. Gerekirse birden çok gece sürer; PM her gecenin sonunda nerede kalındığını yazar.
- Bekleyen Build 3 (kart boşluğu ve açılış testi) Zeyd'in telefon sonuçlarıyla alınır, fabrikayı beklemez. Fabrika build'e giren koda dokunmaz; G8'deki bayraklar kapalı kalır.
- **Zeyd'e sunum:** onay kuyruğu hazır olunca OVERNIGHT.md'nin başına yaz:
  - kaç slot hazır,
  - Zeyd'in nasıl oynayıp onaylayacağı,
  - tahmini süre.
