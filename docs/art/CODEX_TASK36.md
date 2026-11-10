# CODEX_TASK36 — İkon yerleşimi, kahverengi saat, başlık kutusu süsleri

Zeyd TASK35'i **daha iyi** buldu. Başlık kutusunda **"yarı gömülü" varyant seçildi**. Okunurluk iyi.

## 1. İkonlar yeşil diskin DIŞINA taşmasın (düzeltme)
RM'de hiçbir ikon yeşil diskin dışına taşmıyor. Hepsi altın halkanın içinde kalıyor. (PM'in "uçlar halkaya binebilir" notu yanlıştı, iptal.)
- **Kural:** her ikonun bütün pikselleri yeşil diskin içinde kalır; diskle ikon arasında her yönde en az ~%4 çap boşluk. Bunu otomatik kontrol et, `docs/art/task36/olcum.md`'ye yaz.
- **Makas:** biraz küçülsün ve **sola** kaysın (ucu sağdan taşıyor).
- **Sihirli değnek:** biraz küçülsün ve **sağ üst çapraza** kaysın (sol alttaki mor kısım taşıyor).
- **Çekiç:** biraz küçülsün ve **sağ üste** kaysın (sol alt taşıyor).
- **Saat:** boyutu ve yeri iyi, dokunma.
- Optik ağırlık merkezi tuşun merkezine yakın kalsın.

## 2. Saat rengi: siyah → kahverengi
Siyah booster gibi durmadı. Kasa (eski yeşil bölüm) **sıcak, ahşap kahverengi** olsun, hacimli: ışık `#D08A4A` → `#A8642E` → `#7A4019` → kenar `#4E2610`, üst solda parlama. Ziller ve ayaklar sarı, kadran krem kalır. İsteğe bağlı ikinci ton: biraz daha kırmızımsı kahve (`#B0542A` ailesi). İki örnek göster.

## 3. Başlık kutusu (yarı gömülü) + kenar süsü: A/B/C
Seçilen yarı gömülü kutu kalır. Kenarlarına süs dene, hepsi mavi ailesinde ve oyunun malzemesine uygun:
- **A) Flama uçları:** kutunun iki yanından çıkan, ucu V kesik küçük kurdele kuyrukları (arkada, koyu mavi).
- **B) Aşağı inen üçgen bayraklar:** iki köşeden aşağı sarkan küçük üçgen flamalar (mavi + ince altın kenar).
- **C) Altın köşe süsü:** kutunun iki yanında küçük altın kıvrım/kartuş uçları (tablo çerçevesindeki süsle akraba).
Hepsi başlığı **büyütmeden**, okunurluğu bozmadan. Aynısı Süre doldu panelinin başlığında.

## Teslim
`docs/art/task36/onizleme.png`: 4 tuş önce/sonra (yeşil disk sınırı çizili), saat 2 kahverengi, başlık A/B/C oyun ekranında. Dosyalar `Assets/Art/_yeni/HUD36/`. 3 satır özet, dur.
