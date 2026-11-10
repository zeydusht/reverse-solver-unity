# PM → Claude Code (3 Kasım 22:45): yeni başlık/düğme fontu, Baloo 2 ExtraBold (Zeyd onayladı)

**İlk iş:** commit et. PM dosyalarını asla geri alma. PM_40/41 bitmediyse önce onları bitir. Bu işi aynı build'e ekleyebilirsin.

## Hazır dosyalar
- Font: `Assets/Fonts/Baloo2-ExtraBold.ttf`
  - Statik ExtraBold (wght 800) kesimi, Google Fonts'tan.
  - Lisans: SIL OFL, dosyası `Assets/Fonts/OFL-Baloo2.txt`. Ticari kullanım serbest. Lisans dosyası projede kalsın.
- Onaylı görünüm: `docs/qa/2026-11-03-son-kontrol/font_demo_PM.png`, 3. satır. Hedef birebir bu.

## Stil: tek ortak fonksiyon, her yerde
Demodaki değerler (yazı yüksekliği = h):
- **Font:** Baloo 2 ExtraBold. Türkçe karakterler (ğ, ş, ı, İ, ö, ü, ç) içinde var. Font atlasına ekle.
- **Dolgu:** Yazının kendi kutusunda dikey geçiş. Üstte `#FCFADE`, altta `#F0E6A6`. Düz beyaz değil.
- **Kontur:** Kalınlık yazı boyunun ~%6'sı (ince-orta, kalın değil). Renk düğmenin koyu tonu:
  - yeşil `#385018`
  - turkuaz `#0B3E47`
  - kırmızı `#5A1414`
  - krem kart başlığı bandı (turkuaz) `#0B3E47`
- **Gölge:** Kontur rengiyle aynı, aşağı ~%9 kaydırılmış, bulanıklık yok.
- **Boyut:**
  - Tek satırlı düğme yazısı: düğme iç yüksekliğinin ~%50'si.
  - İki satırlıda: üst satır %40, alt satır %27.
  - Uzun metin genişliğe sığdırılsın (FitWidth), en fazla %70'e kadar küçülsün.

## Uygulanacak yerler
1. Ev'deki "Bölüm X" düğmesi. Zor bölümse ikinci satırda "Zor bölüm" / "Hard level" (demodaki 2. sütun).
2. Bütün kart düğmeleri: Sonraki bölüm, Tekrar oyna, Menü, Devam, Yeniden başla, Ana menü, Anladım, Başla, Tekrar dene…
3. Kart başlıkları (Tahta boşaldı!, Durduruldu, tanıtım başlıkları…)
4. Bölümler başlığı ve harita "Oyna" balonu.

Gövde metinleri (açıklama satırları) Nunito'da kalsın. Yalnız başlık ve düğmeler değişiyor.

## Kontrol
- Playwright **webkit** ile iPhone 390×844 ve 375×667 ekran görüntüleri:
  - Ev
  - Bölüm 19 Ev düğmesi
  - kazanma kartı
  - durdurma kartı
  - bir tanıtım kartı
  - İngilizce dilde bir kart
- `docs/qa/2026-11-03-son-kontrol/` klasörüne `webkit_font_*` adıyla kaydet.
- İndirme boyutunu ölç. Font atlası yalnız gereken karakterleri içersin (Latin + Türkçe), boyut artışı ≤ 0,3 MB olsun.
- p20'ye yayınla, OVERNIGHT başına 2 satır, Zeyd'e "bitti" de.
