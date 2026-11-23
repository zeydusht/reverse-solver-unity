# PM_MESAJ_102: Ses düzeltmeleri: fanfar g temel; lastik sesi sabit ve net; booster sesleri gerçek kayıt; koridor whoosh eksik

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: **yalnız ses** (kazanma ekranı zamanlaması fanfara göre yeniden oturabilir).

Zeyd (23 Kasım 17:55).

## 1. Kazanma fanfarı: temel g ("Piyano ta-da")
- **Yanlış anlaşılma:** Zeyd'in beğendiği fanfar **g (Piyano ta-da, 5 vuruş)**. PM_100'de b'nin ritmi temel alınmış.
- **Doğrusu:**
  - g'nin melodisi, ritmi ve süresi temel;
  - g'nin piyano hâli kalır;
  - **aynı melodi ve ritimle bizim tarzda tını çeşitleri:** marimba + glockenspiel, pizzicato yaylılar + çan, ksilofon + el çırpma, müzik kutusu / celesta.
- **Vuruş haritası** g'den üretilsin. Kazanma ekranı 5 vuruşa göre:
  1. ışık + "Harika!";
  2. Jigo;
  3. +50 altın;
  4. Sonraki Seviye;
  5. Menü + konfeti.
- **Varsayılan:** g piyano. `&kazanmaton=` ile diğerleri. b ve b'den türetilenler sayfada "eski" olarak kalsın.

## 2. Lastik çarpma sesi: sabit ve net
- **Sorun:** Bazen kısık bazen yüksek. Sabit değil, hâlâ az duyuluyor.
- **Sebep olabilir:** rastgele seçilen kayıtların ses yüksekliği farklı, ya da rastgele seviye/perde oynaması fazla. Bul ve raporla.
- **Düzeltme:**
  - bütün varyasyonlar aynı ses yüksekliğine ve tepe seviyesine eşitlensin;
  - rastgelelik yalnız ±%3 perde, **ses seviyesi rastgele değil**;
  - seviyesi kayma sesinin **2–3 dB üstünde**;
  - boşluksuz takılmada da aynı seviye.
- **Ölçüm:** 20 takılmanın kaydında tepe seviyeleri arasındaki fark ≤ 1 dB.

## 3. Kayma sesi
Net ve iyi. **1 dB kıs.**

## 4. Booster sesleri: gerçek kayıtlar, oyuna uyarlanmış
CC0 gerçek kayıtlar. Animasyonun ilgili karesine otursun. Her biri için 3 aday, dinleme sayfasında "Booster sesleri".

| Booster | Ses | An |
|---|---|---|
| **Makas** | gerçek makas kesme sesi, 1–2 "şık" | makasın kapandığı karede; mevcut "iğrenç" ses kalksın |
| **Değnek** | büyülü parıltı, yükselen çıngırak / shimmer | dönüşüm anında küçük "puf" |
| **Çekiç** | vuruş + **kırılma** (seramik / ahşap çatlaması) | darbe karesinde, parça düşerken hafif tıkırtı |
| **Saat** | hafif saat tik-takı (donma boyunca kısık) + donarken buz çatırtısı | buzlu bar kırılırken buz kırılma sesi |

Hepsi oyun efekt seviyesinde. Sert ya da korkutucu değil, oyuncak tadında.

## 5. Geçiş sesleri: koridor kayışında whoosh yok
- **Sorun:** Kazandıktan sonra duvar sağdan sola kayarken whoosh çalmıyor. Yalnız tabloya yaklaşırken çalıyor.
- **İstenen:** koridor kayışında whoosh. Kayışın hız eğrisini izlesin: kalkışta artar, ortada en yüksek, oturuşta söner. Başlangıcı kayışın ilk hareket karesinde. Sonra dalış sesi (mevcut).
- **Bütün geçişleri denetle:** menü → bölüm, kazanma → koridor → bölüm, bölüm → menü, sekme kayışı. Her birinde sesin doğru saniyede çaldığını gösteren tablo (olay zamanı vs ses başlangıcı).
- PlayMode testi: geçiş olayları sırasında ses olayı tetikleniyor mu.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (g tınıları, booster sesleri)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (lastik sesi, koridor whoosh, kazanma g)
  - `p23/?c=p23&test=1&lang=tr&lv=23` (booster'lar)
