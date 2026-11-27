# PM_115 RAPOR: dalışta kalan iki kusur (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm ortamı:** yerel build, başsız Chrome, gerçek GPU, 390×844. p23, bölüm 1. iPhone'da denenmedi.
- **Bayrak:** `&pm115=0`, PM_114'ü olduğu gibi gösteriyor.
- **Build:** Bu makinede bellek (8 GB) yetmediği için Brotli paketi alınamadı. Önizleme ve ölçümler gzip build'inden (`/gz/`). Oyun iki pakette de aynı; fark yalnız sıkıştırmada.
- **Değişmeyenler:** U1, 1,60 sn, kenar çizimi, sesler, çerçeve.

## §1 Dönme ve kayma (0,27 sn)
- **Ne yapıldı:**
  - Uçuşun son 0,11 sn'sinde (6–7 kare) yalnız yana kayma ve onunla gelen dönme yumuşakça sıfıra iniyor.
  - Yol ve ileri hız aynı. Geri dönme yok.
  - Toplam dönme ve kayma korunuyor (−6,2° / +2,8 pt; PM_114'te −6,1° / +2,8 pt).
  - Yaklaşma tablonun tam karşısından başlıyor.
- **Nasıl:** Pencerenin başında da sonunda da hız ve ivme sürekli. Toplamı korumak için kayma pencerenin ilk yarısında biraz hızlanıyor (dönme 1,0'dan 1,28°/kareye), sonra sıfıra iniyor.
- **Ölçüt:** Karedeki dönme ve kayma değişimi, komşu karelerin ortalamasının en çok 2 katı olmalı. Önce/sonra 6'şar koşu.

| | PM_114 | PM_115 |
|---|---|---|
| Dönme, 0,27 sn civarında | 1,0°/kare → 0, tek karede. Komşuların **5–177 katı** | 1,28 → 1,10 → 0,67 → 0,15 → 0. **1,7 kat** |
| Kayma, 0,27 sn civarında | 0,6 pt/kare → 0, tek karede. **4–95 kat** | 0,64 → 0,59 → 0,38 → 0,07 → 0. **1,9–2,0 kat** |
| Pencerenin başı (0,20 sn) | — | 2,1 kat (0,24°/kare, gözle görülmez) |

- **Sınırda kalan:** Kayma sınırda (2,0 kat). Pencereyi 0,12 sn yaparsak oranı biraz düşer; yeni build gerektiriyor, PM isterse yapılır.

## §2 Dipte takılma (0,72–0,85 sn)
- **Neden render texture değil:** PM'in önerisi resmi bir kez görüntüye çizip 0,67–1,42 sn arasında onu göstermekti. Bu aralıkta her karede iki şey değişiyor:
  - Kesim çizgileri kalemle çiziliyor.
  - Resim kamerayla eğilip büzülüyor.

  Bir kez çizilen görüntü bu yüzden yanlış olur.
- **Yerine:** Dalışta parça shader'ı, ekranda etkisi zaten sıfır olan hesapları atlıyor:
  - Parça görünümü 0'ken (dip, yalın resim) kabartma, gölge ve kit katmanı atlanıyor. Bunların hepsi görünümle çarpılıyor, yani sıfırla çarpılıyordu.
  - Çizgisi henüz gelmemiş yerlerde kesim halkası atlanıyor (24 doku okuması).
  - Oyun içi parça çizimi değişmedi: atlama yalnız dalışta açık.
- **Devir:** Bu yolda devir yok. Görüntü tanım gereği aynı; ölçtüm de:
  - Dipte aynı saatteki kareler: eski–yeni farkı (ortalama 1,0–8,2) aynı build'in iki koşusu kadar (1,0–8,7). Fark kare zamanlamasından.
  - Oyunun son karesi: eski–yeni 0,14, eski–eski 0,14 (JPEG gürültüsü).

**Uzun kareler, dip (0,69–0,90 sn):**

| Koşu | PM_114: 20 ms üstü | PM_114: 24 ms üstü (30 FPS'e düşüş) | PM_115: 20 ms üstü | PM_115: 24 ms üstü |
|---|---|---|---|---|
| 2x, kayıtsız, 6 koşu | 5 kare (1 koşuda, 25–29 ms) | 5 | **1** (20 ms) | **0** |
| 2x, ekran kaydıyla, 6 koşu | 13 kare (4 koşuda, en çok 36 ms) | 10 | **9** (hepsi 20–22 ms) | **0** |
| 3x, kayıtsız, 3 koşu | 6 kare (2 koşuda, 25–27 ms) | 5 | **0** | **0** |

Dipte ortalama kare süresi: kayıtla 17,8 → 16,4 ms, kayıtsız 17,2 → 16,3 ms.

- **Kayıtsız ve 3x:** Ölçüt tuttu (%80 az ya da hiç).
- **Kayıtla 2x:** Ölçüt tam tutmadı. 20 ms üstü kare 13'ten 9'a indi (%31). Kalanların hepsi 20–22 ms. 30 FPS düşüşü (24 ms üstü, art arda 5 kare) hiçbir koşuda yok.
  - Ekran kaydı da ekran kartından pay alıyor. Daha fazlası için oyun içi parça çizimine dokunmak gerekir; bu PM_115'in kapsamı dışında.

## Çıktılar
- **`once_sonra.mp4`:** PM_114 | PM_115. Panel başına 390×844, 60 fps, sesli. İki kez oynuyor, her seferinde bir panelin sesi. İki kayıt da aynı build'den (PM_114 `&pm115=0`).
- **`once_sonra_yarim.mp4`:** yarım hız, iki bölge art arda: 0,15–0,40 sn (dönme) ve 0,65–0,95 sn (dip).
- **`u1_son.mp4`:** PM_115 tek başına, 2,5 sn, sesli.
- **`sicrama.png`:** PM_114'teki gibi, iki sütun. Gri dönme çizgisi 0,27 sn'de artık tek karede düşmüyor, 4–5 karede iniyor.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı. Dipteki yük 3x'te ölçüldü, ama gerçek telefon GPU'su yerine masaüstü GPU'suyla.
