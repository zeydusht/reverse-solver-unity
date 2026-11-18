# PM_85 RAPOR: rehber eli (D + A + yeni ok), kısa bölüm geçişi

## 1. Rehber eli, varsayılan
**Görünüş:** p20'nin eldiveni (`&el=d` çizimi). Basarken eldivenin "basılı" çizimi kullanılıyor.

**Hareket** (`&el=a`'nın hareketi; döngü yaklaşık 3,8 sn):

| Süre | Ne olur |
|---|---|
| 0,45 sn | Sağdan, hafif aşağıdan gelir; parçanın üstünde çapraz (24°) durur, parmak ucu parçada. |
| 0,12 sn | Basar: basılı eldiven, el biraz küçülür, parmağın altında krem halka açılır. |
| 0,25 sn | Bekler. |
| 0,7 sn | Bastırarak sürükler: parçanın yarı saydam hayaleti elle birlikte kayar, gerçek parça yerinde kalır. |
| 0,15 sn | Durur. |
| 0,3 sn | Kalkar, kaybolur. |
| 1,8 sn | Bekleme, sonra tekrar. |

**Ok:**
- Düz, kalın, krem şerit; ince altın kenarlı. Noktalı ve ince değil.
- Parçanın ortasından gidiş yönünde tahtanın dışına (aydınlık yolun sonuna) kadar uzanır.
- Önce ok yolu açar: parçadan çıkış yönüne doğru 0,3 sn'de uzanarak çizilir, sonra el gelir.
- Karartmalı adımlarda (2–5) ve karartmasız ipucunda (PM_84) aynı ok. İpucunda ok 2 kez ileri-geri sallanıyor.

**Codex'in 3 seçeneği** (`docs/art/pm85/ok/`):

| Seçenek | Ne | Puan | Not |
|---|---|---|---|
| **B** | krem + ince altın kenar | **9** | Kenar, eldivenin manşetindeki altın halkayla eşleşiyor; karartmasız ipucunda tablonun üstünde en net okunuyor. **Bağlandı.** |
| A | düz krem | 8 | Eldivenle en yumuşak uyum, ama açık renkli tablolarda kenarı kayboluyor. |
| C | turkuaz kenar, sivri uç | 7 | Soğuk duruyor, eldivenden çok manşete benziyor. |

**Bayraklar:** `&el=a`, `&el=b`, `&el=c`, `&el=d` test için duruyor; bayrak yoksa yeni birleşim çıkıyor.

**GIF:** `docs/art/pm85/el_ok.gif` (390×844, 2. adım).

## 2. Bölümden bölüme geçiş
| Adım | Önce | Şimdi (ölçülen) |
|---|---|---|
| Koridor kayışı | 3,6 sn (0,25 açılış + 0,35 bekleme + 3,0 kayış) | **1,50 sn** (0,12 açılış + 0,08 bekleme + 1,3 kayış) |
| Yaklaşma | 0,8 sn | **0,6 sn** |
| Resimde kalış | 0,8 sn | **0,4 sn** |
| Geri çekilme | 1,0 sn | **0,8 sn** |
| **Toplam (kazanma → oyun)** | ≈ 6,2 sn | **3,31 sn** (koridor 1,50 + dalış 1,81) |

- **Koridor:**
  - İlk 0,2 sn kazanılan tablo görünür.
  - Hız ilk %30'da yumuşakça artar, sonra eşit akar, son %30'da yumuşakça sıfıra iner.
  - Zıplama ve geri sekme yok: EditMode testi kayışın hiç geri gitmediğini ve hızın kareler arasında sıçramadığını kontrol ediyor.
  - Durunca yaklaşma hemen başlıyor.
- **Resimde kalış:** çizgiler resim tam ekran olur olmaz başlıyor (PM_84: çizgiler tahtanın kendi kenarları).
- **Geri çekilme:**
  - Çizgiler çizilirken başlıyor.
  - Çizgiler çekilmenin %70'inde tamamlanıyor.
  - HUD son 0,3 sn'de geliyor.
- **Menü → bölüm:** resimde kalış 0,4 sn, geri çekilme 0,8 sn, çizgiler çekilirken sürüyor. Yay (uçuş 1,8 sn) ve yaklaşma (0,8 sn) aynı. Toplam 3,81 sn.
- **Dokunma kuralı aynı:** 1. dokunuş 2×, 2. dokunuş bitirir.
- **Bayraklar:**
  - `&koridor=` (sn, varsayılan 1.5).
  - `&resimbekle=` (sn, varsayılan 0.4).
  - Deneme: `&koridor=1.2&resimbekle=0.25` → koridor 1,22 + dalış 1,65 = 2,87 sn.

**20 ms'yi aşan kare** (oyunun kendi sayacı, PC, gerçek GPU):

| Bölüm | Kare | 20 ms'yi aşan |
|---|---|---|
| Koridor | 90 | 0 (en uzun 17 ms) |
| Dalış | 108 | 1 (44 ms) |

Dalıştaki tek kare, yaklaşma dururken bölümün kurulduğu kare.

**Kenar boşluğu:** koridor ve dalış kodunda kenarı açan bir değişiklik yok. `&dalisbosluk=1` ile yeniden sayılmadı.

**Dosyalar:**
- Hız grafiği: `docs/art/pm85/hiz_gecis.png` (kazanma → sonraki, dalış kısmı).
- GIF'ler: `docs/art/pm85/gecis_once_sonra.gif` ve `docs/art/pm85/gecis_yarim_hiz.gif`.

## Doğrulanmayan
- iPhone'da kısa geçişin hissi.
- Elin hızı.
