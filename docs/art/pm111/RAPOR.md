# PM_111 RAPOR: B varsayılan, resimde kalış için 3 varyant (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844, p23 bölüm 1. iPhone'da denenmedi.
- **Araçlar:**
  - kayıt: `Tools/pm108_dalis.js`
  - hız grafiği: `Tools/pm111_plot.py`
  - video: `Tools/pm108_video.py`

## Ne değişti
- **B varsayılan.** `&dalis110=a` ile A'ya dönülür, `&dalis110=0` ile PM_109'a.
- **Resimde kalış:** `&resimbekle=` (0 / 0,07 / 0,145; yoksa şimdiki 0,285 sn).
  - Bayrak artık yalnız menü dalışına uygulanıyor. Koridordan sonraki dalış kendi kalışını (0,227 sn) koruyor.
- **Hız sürekliliği:**
  - Geri çekilme, kalış bittiğinde kameranın o anki hızıyla başlıyor. Kalış 0'da bu hız varış hızıdır; o zaman PM_91'in "değme"si (%2,5) kendiliğinden kısalıyor.
  - Kısa kalışta (0,285 sn'den kısa) geri çekilme yumuşak kalkışlı: ivme de sıfırdan başlıyor (beşinci dereceden eğri). Grafikte dönüş S biçiminde, köşe yok.
  - **Bedeli:** süre aynı (0,78 sn), ama geri çekilmenin tepe hızı 0,80'den 1,0 ekran boyu/sn'ye çıkıyor.
  - Şimdiki (0,285 sn) değişmedi. Onda kalışın sonunda hafif bir köşe var, eskisi gibi.
- **Çizgiler:** Kısa kalışta varış anında başlıyor, geri çekilmenin %80'inde bitiyor (şimdikinde %70). Kalışın sonunda çizilen pay, kalışın süresiyle orantılı; çizim hızı aynı. Çizgiden tahtaya devir (PM_84/86) aynı kuralla, yeni bitişten başlıyor.
- **Ses:**
  - Dalış whoosh'u yine değmede bitiyor; çizgi sesi (c) değmede.
  - Geri çekilme whoosh'u kısa kalışta çizgi sesinin altına giriyor: kalış 0'da %60 ses, 0,15 sn ve üstünde tam ses.
  - Değme ile geri çekilmenin başı arasında ölçülen tepe dört varyantta aynı: efektler −13,3 / −13,7 / −13,3 / −13,6 dBFS, müzikle −11,6 ile −12,8 dBFS arası. Üst üste binip patlama yok.

## Süreler (Oyna → bölüm başlar)

| | Şimdiki | Vol 1 | Vol 2 | Vol 3 |
|---|---|---|---|---|
| Resimde kalış | 0,285 sn | 0 | 0,07 sn | 0,145 sn |
| Yay + yaklaşma | 0,71 sn | 0,71 sn | 0,71 sn | 0,71 sn |
| Geri çekilme | 0,78 sn | 0,78 sn | 0,78 sn | 0,78 sn |
| **Toplam** (ölçülen) | **1,81 sn** | **1,52 sn** | **1,60 sn** | **1,67 sn** |

## Hız (`hiz.png`)
Gidiş, kalış ve geri çekilme işaretli gösteriliyor: + içeri, − dışarı.
- **Varış:** Hız tabloya 0,06 ekran boyu/sn ile varıyor.
  - **Vol 1:** Hız hemen sıfırdan geçip dışarı dönüyor (~0,05 sn).
  - **Vol 2 ve 3:** Kalış boyunca yavaş itiş var, sonra yumuşak kalkış.
- **Sıçrama:** Dönüş bölgesinde iki kare arasında en büyük hız farkı 0,09 ekran boyu/sn. Dördünde de aynı ve gidişin kendi kare farklarıyla aynı düzeyde.
- **Kenar boşluğu:** Vol 1, 2 ve 3'te 0 kare.

## 20 ms'yi aşan kareler (bu PC, Chrome)
Bu turun ikinci yarısında makinede başka işler çalışıyordu (başka bir tarayıcı otomasyonu ve Codex), bu yüzden sayılar yüksek. Şimdiki hâlde de yükseldi.

| Koşu | Yük yokken (ekran kaydı açık) | Yük altında (ekran kaydı kapalı) |
|---|---|---|
| Şimdiki | 4 kare, en uzun 37 ms | 3 kare, en uzun 58 ms |
| Vol 1 | 9 kare, en uzun 41 ms | 4 kare, en uzun 48 ms |
| Vol 2 | 3 kare, en uzun 44 ms | 3 kare, en uzun 53 ms |
| Vol 3 | 8 kare, en uzun 34 ms | 5 kare, en uzun 82 ms |

- **Nerede:** Hep aynı yerler: basışın ilk karesi ve tabloya değme (bölüm kuruluyor). Bazı koşularda geri çekilmede 21–30 ms'lik birkaç kare de var.
- **Kalış 0'da:** Değme karesiyle geri çekilmenin ilk karesi arka arkaya geliyor. Bölüm kuruluşunun uzun karesi artık hareketin içine düşüyor; şimdikinde kalışın durgun anına düşüyordu. Telefonda bakılmalı.

## Önizleme
- **`kalis_yan_yana.mp4`:**
  - Şimdiki | vol 1 | vol 2 | vol 3, 1560×844 (her panel 390×844), 60 fps, 10 sn, 8,4 MB.
  - Dört kez oynuyor; her seferinde bir panelin sesi duyuluyor ("SES" işareti).
- **`kalis_vol1.mp4`, `kalis_vol2.mp4`, `kalis_vol3.mp4`:** Her biri 390×844, 60 fps, sesli, 2,5 sn, 0,6 MB.
- **`hiz.png`:**
  - Üstte basıştan bölümün oturmasına kadar hız.
  - Altta varış ve dönüş yakın planda.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı.
- **Kayıt:** Ekran kaydında birkaç 50–83 ms'lik boşluk kaldı. Hepsi ya varış karesinde ya da bölüm oturduktan sonra.
