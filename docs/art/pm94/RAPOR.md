# PM_94 RAPOR

Ölçümler yerel build'de alındı: Chrome, gerçek GPU, 390×844, 60 fps. iPhone'da denenmedi.

## 1. Boşluklu takılmada gidiş

- Parça ilk kareden hızla çıkıyor. Hız parmağın hızı, ama en az bir hücreyi 0,07 sn'de, iki hücreyi 0,11 sn'de geçecek kadar.
- Temasa kadar hız %30 artıyor. Başta da sonda da yavaşlama yok.
- Ölçüm: bölüm 4'teki itmede gidiş eskiden ~0,13 sn sürüyordu, şimdi 0,059 sn (3–4 kare).
- Uzama ilk karede geliyor (0,025 sn'de tamam) ve çarpmada büzülmeye dönüyor.
- Geri sekme ve oturma 1. seviyede PM_93 ile aynı.
- Boşluksuz takılmanın kısa itmesi değişmedi.

## 2. Seviyeler (`&lastik=`, varsayılan 1.5)

| Seviye | Basılma | Yana genişleme | Yuvayı geçme | Salınım | Toplam süre (1 hücre / 2 hücre) |
|---|---|---|---|---|---|
| 1 | %20 | %12 | hücrenin %4'ü | 1 | 0,43 / 0,47 sn |
| 1.5 | %29 | %15 | %6 | 1 | 0,43 / 0,47 sn |
| 2 | %38 | %19 | %8 | 2 (ikincisi ~%3) | 0,45 / 0,49 sn |

- Toplam süre her seviyede ≤ 0,5 sn; uzun boşluklarda gidiş kısaltılıyor.
- Boşluksuz takılmada basılma aynı oranda artıyor: %11 / %16 / %21. Toplam süre 0,33 / 0,33 / 0,35 sn.
- Çarpan kenar engelin kenarında sabit kalıyor; gövde o kenara doğru eziliyor ve engelin içine geçmiyor.
- Resim, tırnak, kilit ve bomba parçayla birlikte esniyor (tek transform).
- Ara değerler de çalışıyor (ör. `&lastik=1.75`).

## Kare süreleri (`&lastiklog=1`, bölüm 4, her seviyede 6 itme)

- **Boşluksuz:** 20 ms'yi aşan kare 0 (her seviyede).
- **Boşluklu:** her itmede temas anında tek bir ~30 ms kare var. Aynı kare PM_93'te ve eski PM_75 çarpmasında da vardı (açık iş, kapsam dışı).

## GIF

`lastik_1_15_2.gif`: yarım hız, soldan sağa 1, 1.5, 2. Önce boşluklu itme, sonra boşluksuz itme.
