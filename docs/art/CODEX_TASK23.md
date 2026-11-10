# Codex Task23 — Gece 2. tur: PM'in eleştirisi + tek nihai aday + eksik ekranlar

Task22 çok hızlı bitti, iyi. Ama PM tek tek baktı, bazıları **geri gitmiş**. Bu tur kalitede. Acele etme, her öğeye en az 2 deneme yap, en iyisini seç. Kurallar Task22 ile aynı: yalnız `Assets/Art/Redesign_T23/` ve `docs/art/`, silme yok, kod yok, soru yok, saat başı `GECE_LOG.md`.

## PM kararları (Task22 adaylarına)

| Aile | Karar | Gerekçe ve yapılacak |
|---|---|---|
| 01 / 101 kazanma | **101 yönü kabul** (turkuaz yumuşak panel) | Türkçe metinler ve "Puanlamayı geç" iyi. **01'in düz lacivert başlık bandı kötü**, kullanılmayacak. 101'i nihai yap. Kayıp (102/03/04) ve durdurma (103/05) aynı aileden. |
| 06–14 / 104 tanıtım + zor | **İKİSİ DE RED** | Mevcut kartlardaki **Baron illüstrasyonu kaldırılmış**. Kart boş ve sıkıcı kalmış, karakter sıcaklığı gitmiş. Royal Match kartlarında hep karakter sahnesi var. **Yeni yön:** mevcut kart düzeni (üstte karakter sahnesi + altında kısa metin + yeşil tuş) kalsın. Yalnız şunlar düzelsin: metin en fazla 2 kısa satır, turuncu ikinci metin satırı kalksın (tek mesaj), küçük ikon kutusu kalksın, çerçeve 101 ailesine geçsin. Her engel ve booster için kendi sahnesi olsun: Baron (engel) ya da Jiggle (booster), tek karakter. Mevcut Baron sahneleri iyiyse onları koru. |
| 15 / 106 yükleme | **15 kabul** | Önceki ekran simsiyahtı, büyük iyileşme. 106 ile birleştir, en iyisini nihai yap. Yüzde göstergesi yeri olsun. |
| 16 / 107 isim | **107 kabul** | Royal Match diline çok yakın. "Playtest sürümü" yazısı yok, iyi. |
| 17 / 108 profil sorusu | 107 ile aynı aileden nihai yap. | |
| 18 / 109 ayar anahtarı | **109 kabul** (tik / çarpı) | |
| 19 dil seçici | 109 ile tutarlı nihai. | |
| 20 / 110 harita | **"Oyna" balonu kabul** (yeşil tuş balon). **Düğüm RED** | Şu anki düğüm koyu yeşil olunca parlak altın olan mevcut düğümden daha az göze çarpıyor. Şu anki düğüm **altın** kalsın. Biten düğümler turkuaz, kilitliler koyu. Yalnız kilitli düğümün kilidini büyüt. |
| 21 / 111 koleksiyon | Nihai tek aday seç (Sıradaki şeridi + kilitli hücre). | |
| 22 jiggles | **22 kabul** (krem kapsül içinde çubuk) | 500 / 1.000 / 2.000 okunur. |
| 02 / 113 puan kutuları | 101 ailesine uyan nihai tek set. | |

## Yeni: eksik kalan ekranlar (Task22'de yoktu)
1. **Oyun içi HUD:** bölüm rozeti, hamle hapı, süre hapı, durdur. Onaylı Task14/15 setini Royal Match rehberine göre **cilala**, yerleşimi değiştirme. Daha tok kontur, daha okunur rakamlar. Önce/sonra.
2. **Booster çubuğu:** dört booster tuşu + stok rozetleri. Stok 0 iken gri hal.
3. **Koridor geçişindeki** yazı ve tuş, varsa "Devam" ya da atla ipucu.
4. **40. bölüm bitiş kartı:** "Koleksiyon tamamlandı" kutlaması, 101 ailesi.
5. **Ev ekranı üst barı:** kare avatar, isim plaketi, Jiggle sayacı ve dişli. Royal Match `IMG_2440` ile yan yana kıyasla, eksikse cilala.

## Teslim
- `Assets/Art/Redesign_T23/NIHAI/<aile>/`: her aile için **tek nihai aday**, oyunun kullanacağı PNG'ler (@3x, şeffaf, 9-slice notu) + `once-sonra.png` + `NOT.md` (hangi dosyanın yerine geçecek).
- `Assets/Art/Redesign_T23/inceleme.html`: yalnız nihai adaylar, numaralı. Zeyd "hepsi olsun" ya da "şunlar hariç" diyecek kadar net olsun.
- `docs/art/T23_KOD_LISTESI.md`: bu çizimler oyuna bağlanırken Claude Code'un yapması gerekenler. Hangi sprite hangi `Ui` sınıfında değişir, metin yerleşimi.
- Sabah özeti `GECE_LOG.md` sonuna: kabul edilen aileler, kalan sorunlar.
