# PM → Claude Code (2 Kasım 2026, 19:45) — Eğitim karartması: tek maske, yalnız hedef (p20)

Zeyd p20'yi telefonda denedi (ekran görüntüsü: bölüm 10, zincir spotlight'ı). İki hata var. Davetler henüz gitmedi, p20'ye yayınla.

## Hata 1: Yatay açık şeritler (tüm eğitim adımlarında)
- Karartma birden çok dikdörtgenden yapılmış.
- Dikdörtgenlerin birleştiği yerlerde ekran boyunca **yatay açık bantlar** kalıyor: tahtanın üst ve alt kenarı hizasında, HUD'dan halıya kadar. Bazı yerlerde karartma iki kat binmiş.

**Düzeltme:**
- Ekranı kaplayan **tek bir** koyu örtü kullan (%60, her yerde aynı ton).
- Aydınlık bölgeler bu örtüden **delik** olarak açılsın: stencil, maske dokusu ya da shader. Dikdörtgen parçalarla örtü kurma.
- Delik kenarları 6–8 pt yumuşak geçişli olsun, köşeler yuvarlak (hücre köşe yarıçapı).
- Örtü HUD'u, booster çubuğunu ve alt alanı da kaplasın. Yalnız booster adımında booster tuşu delik olur.
- Ekran kenarında, tahta kenarında ve güvenli alanlarda dikiş görünmesin. 3 ekran boyunda kontrol et.

## Hata 2: Bölüm 10 zincir: yalnız TEK çift
- Şu an iki parçayı kapsayan dikdörtgen ve çıkış yolları açılmış. 3 zincirin parçaları ve fazladan hücreler aydınlık.
- **Doğrusu:** yalnız **bir** zincirli çiftin **iki parçasının hücreleri** ve aralarındaki **zincir çizgisi** aydınlık.
  - Her parça kendi şekliyle delik olsun (hücre + yuvarlak köşe).
  - Zincir, iki kilit arasında ince bir şerit olarak delik açsın.
  - Başka hücre, şerit ya da çıkış yolu açılmasın.
- **Hangi çift:** ekranda en kolay görünen (zincir çizgisi diğer zincirlerle en az kesişen) çift. Eşitse ilk çift.

## Diğer spotlight'lar (aynı kural: yalnız gereken şekiller)
- **Bölüm 1 sürükleme adımları:** hedef parça + çıkış yolu (parçadan tahta kenarına şerit). Yol tek, ince ve parçayla aynı genişlikte olsun. Bitişik satır ya da sütun açılmasın.
- **Çivi:** çivili parça + sayaç rozeti.
- **Bomba:** bomba parçası + fitil rozeti.
- **Duvar:** duvar şeridi.
- **Booster:** tuş adımında yalnız tuş. Hedef adımında yalnız hedef hücre(ler).

## Kontrol
- Bölüm 1 (iki adım), 3, 6, 10, 14, 5 (makas iki adım) için 390×844 ve 375×667 ekran görüntüsü → `docs/qa/2026-11-02-spot/`.
- Her görüntüde iki şeye bak:
  - Örtünün tonu her yerde aynı mı? Yatay ya da dikey bant var mı? Piksel parlaklığını 5 noktadan ölç.
  - Aydınlık alan yalnız listelenen şekiller mi?
- OVERNIGHT.md başına 2 satır.
