# PM_MESAJ_79: Yay onaylandı: varsayılan yap, 2× hızlı ve kesintisiz

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd (17 Kasım 04:19): "Şimdi çok iyi oldu, istediğim açı tam buydu. Sadece biraz yavaş; 2× gibi daha hızlı ve daha smooth içeri dalsın. Tablonun karşısına geçince durmasın, direkt devam etsin. Resme gelince 1–2 sn durup geri çekiliyor; bekleme milisaniye olsun, görsel olarak 'içine girdik' gibi olsun."

## 1. Varsayılan
PM_78'deki yay (orijinal menü dioramasıyla) artık **varsayılan** menü → bölüm geçişi. `&gecis=yay` bayrağı gerekmesin; eski hâl `&gecis=eski` ile açılsın.

## 2. Yeni zamanlama (≈ 2× hızlı, tek akış)
| Adım | Önce | Sonra |
|---|---|---|
| Yay (Zeyd'in 4 karesi) | 3,6 sn | **1,8 sn** |
| Tablonun karşısında duruş | ~0,2 sn + dalış başlangıcı | **yok**. Yayın sonu ile yaklaşmanın başı tek hareket, hız hiç sıfıra inmez |
| Yaklaşma, ekran resim olur | 1,1 sn | **0,55 sn** |
| Sadece resim | 0,5 sn (+ nefesler) | **0,12 sn**, adım arası nefesler kalkar |
| Uzaklaşırken yapboz çizgileri | 1,0 sn | **0,6 sn** |
| HUD + booster çubuğu | 0,4 sn | **0,3 sn**, çizgilerin son %30'uyla üst üste |

- **Hareket eğrisi:** yay + yaklaşma **tek eğri**. Başta yumuşak kalkış, yaklaşmanın sonuna doğru hızlanan "dalış" hissi (ease-in ağırlıklı); resme değdiği an kısa bir parlama ya da hafif ölçek darbesi, sonra 0,12 sn bekleme. Ardından geri çekilme ease-out. Kareler arası hız sürekli olsun (konum + hız sürekliliği, sıçrama yok).
- Toplam ≈ 3,3 sn. Dokunma kuralı aynı: 1. dokunuş 2×, 2. dokunuş bitirir.
- **Ayar bayrakları** (Zeyd hızlı denesin):
  - `&yayhiz=` çarpan, varsayılan 1, örnek 0.8 ya da 1.2;
  - `&resimbekle=` sn, varsayılan 0.12.
- Kenar boşluğu denetimi her karede 0. Akıcılık: geçiş boyunca 20 ms'yi aşan kare sayısını raporla.

## Teslim
- p23 güncel, testler yeşil, `verify_build` p23 4/4. GIF'ler `docs/art/pm79/` (normal + yarım hız). `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr`
  - `p23/?c=p23&test=1&lang=tr&yayhiz=1.2`
  - `p23/?c=p23&reset=1&lang=tr`
