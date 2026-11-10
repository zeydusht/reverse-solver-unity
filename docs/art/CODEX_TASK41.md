# CODEX_TASK41 — Onboarding, booster gösterimleri ve "haz" efektleri (görseller)

Arka plan: `docs/PT1_GERIBILDIRIM.md`. Oyuncular mekaniği ve booster'ları anlamadı, oyunu "durağan" buldu. Bu görev yalnız **görsel üretim**; entegrasyonu Claude Code yapacak. Stil: onaylı HUD (old money mavi + krem, doygun altın/turuncu, hacimli boyama, siyah kontur yok), Jigo `Jigo_v3`. Her şey şeffaf PNG; animasyonlar kare dizisi (8–12 fps) ya da parça parça katman (kodla hareket edecekse).

## 1. Tutorial
- **Yönlendiren el** (eldivenli el, mevcut stilde): dokun, basılı tut, sürükle kareleri (4–6).
- **Sürükleme yolu oku:** kesik çizgili, ucu ok başlı, parlayan krem/altın (düz, kavisli; 4 yön).
- **Takılma göstergesi:** iki parçanın çarpıştığı yerde kısa kırmızımsı "tık" parlaması + küçük çarpı.
- **Doğru hamle göstergesi:** yeşil ışıltı + tik.
- **Tutorial konuşma balonu:** krem balon, mavi çerçeve, Jigo'nun küçük büstü köşede (konuşan karakter). Metin canlı.
- **2×2 tutorial resmi:** basit, sevimli, 4 parçalık resim (çayır + Jigo ya da tek büyük Jiggle yüzü). 1024×1024.

## 2. Booster gösterimleri
- **Makas:** ekrana gelen büyük makas (açık, kapalı, kesme 3 kare); kesilecek yerde parlayan kesik çizgi; kesim anında kumaş/kağıt kırpıntısı.
- **Sihirli değnek:** büyük değnek (sallanma 3 kare); yıldız tozu izi; tahtanın **4 bölgesi** için seçim çerçevesi (köşeli, parlayan, seçili/seçili değil) ve karartma katmanı; değnek dokununca o bölgede ışık patlaması (6 kare).
- **Çekiç:** büyük çekiç (kalkış, iniş, çarpma 3 kare); çarpma yıldızı/şok dalgası (4 kare); parça çatlak katmanı (2 kademe).
- **Saat:** Jigo'nun **saat tutan kolu** (madalyon penceresinden uzanan kol + elinde saat; uzanma 3 kare, tutma döngüsü 2 kare); saatin üstünde **boşalan süre barı** (çerçeve + dolgu ayrı, krem/mavi + altın kenar).

## 3. Haz (juice) efektleri
- **Parça çıkış patlaması:** renkli konfeti + yıldız + küçük "puf" bulutu (6 kare), parça rengine boyanabilir nötr sürüm.
- **Çatlama:** engel (kilit, zincir) kırılırken parça parça dağılma (5 kare).
- **Aferin rozetleri:** "Harika!", "Süper!", "Mükemmel!" (TR) / "Great!", "Super!", "Perfect!" (EN), kalın font, altın/turuncu, hafif eğik; metin ayrı katman olacaksa yalnız arka plan rozeti.
- **Kombo sayacı arka planı:** "x2, x3" için küçük patlama rozeti.
- **Kazanma "Sonraki" düğmesi:** büyük yeşil düğme (normal/basılı), sağa ok.
- **Kurtarılan Jiggle sayacı:** Jiggle ikonu + sayı rozeti; kazanınca Jiggle'ların sürüye koşması için 3–5 küçük Jiggle koşma karesi (renk varyantları).

## 4. Kilit uyarısı
Kilitli parça kaydırılmaya çalışınca: kilit sallanma 3 kare + sayının etrafında kırmızımsı vurgu halkası.

## Teslim
`docs/art/task41/onizleme.png` (hepsi tek pafta, gruplar başlıklı; tutorial ve booster sahneleri için 390×844 küçük mock'lar). Dosyalar `Assets/Art/_yeni/T41/` alt klasörlerde (`Tutorial/`, `Boosters_FX/`, `Juice/`). `docs/art/task41/LISTE.md`: dosya, boyut, kare sayısı, fps önerisi. 3 satır özet, dur.
