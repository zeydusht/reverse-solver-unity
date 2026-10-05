# Gece raporu — 2026-10-05

Kısa özet: boyut çalışması kapandı (11,8 → 8,7 MB gzip, 6,8 MB Brotli), M1 bitti (seviye verisi + kural motoru, 227 test yeşil), ikisi de PM onaylı. **Sabah ilk iş GitHub Pages'i açman lazım; açılmadan hiçbir adres çalışmıyor.**

## Neler bitti

### Boyut çalışması
Toplam indirme (indirilen 4 dosya, sıkıştırılmış). Ham değerler `Builds/size-log.csv`'de.

| Adım | wasm | data | Toplam |
|---|---|---|---|
| A başlangıç | 9,02 MB | 2,66 MB | 11,81 MB |
| B kullanılmayan paketler | 8,50 | 2,52 | 11,15 |
| D URP: post-processing, HDR, ışık, lens flare kapalı; Unlit | 8,50 | 2,41 | 11,04 |
| E Unity splash kapalı | 8,50 | 2,35 | 10,98 |
| C High stripping + IL2CPP Optimize Size | 7,43 | 1,86 | 9,42 |
| F wasm Code Optimization: DiskSizeLTO (Unity varsayılanı BuildTimes'tı) | 6,65 | 1,86 | 8,64 |
| G Brotli (aynı kod) | 5,16 | 1,47 | 6,81 |
| **M1 gzip** (şu an yayında) | 6,66 | 1,87 | **8,67** |
| **M1 Brotli** (şu an yayında) | 5,16 | 1,49 | **6,84** |

- Brotli 1,8 MB daha küçük ama JS ile açması bilgisayarda 1,55 sn, gzip 0,27 sn. Başa baş bağlantı hızı ≈ 11 Mbit/s. Kararı senin 4G ölçümün verecek.
- İndirmenin neredeyse tamamı kod (wasm + IL2CPP metadata). UI Toolkit (~1,5 MB IL) çıkarılamıyor: Input System paketi ona bağımlı.
- Ana ekran alt şeridi: sayfa arka planı build sırasında kamera rengine (#314D79) eşitleniyor. iPhone'da doğrulanmadı.

### M1: seviye verisi ve kural motoru
- `Assets/_Game/Levels/levels.json`: web'deki 40 seviye birebir, her birinde kalıcı `id` (L01–L40) ve `version: 1`.
- Core (saf C#, Unity'siz): JSON okuyucu, `LevelParser`, `LevelStats`, `TravelRule` (web kuralının birebir portu), `GreedySolver`.
- Web oyununun kendi kodu Node'da çalıştırılarak 10.612 altın değer üretildi; C# portu hepsinde aynı.
- High stripping'li gerçek web build'i bu bilgisayarda (başsız Chrome) "40 seviye okundu, 40 çözülebilir (51 ms)" yazdı.

## PM kararları (ayrıntı ve gerekçeler `docs/PRODUCT.md`'de)
- Boyut adımları B–F kabul, splash kapalı kalıyor. 6 MB ara hedefi engelleyici değil.
- Sevk edilen sıkıştırma şimdilik **gzip**. Brotli ancak 4G'de medyanda ≥ 0,5 sn hızlıysa seçilecek. İkisi de 10 sn'yi aşarsa boyut işi yeniden açılacak.
- UI Toolkit çıkarılmayacak, URP/uGUI fork'lanmayacak. Input System'den vazgeçilmeyecek.
- Seviye `id`'si kalıcı, `version` oynanış değişince artar.
- M1 kapandı (iPhone kontrolü sende). Sıradaki iş **M2: GameSession** (çivi, bomba, supap, booster'lar, süre, deneme kaydı). İnsan metriklerinin tanımı sabitlendi.
- PhysX modülünü çıkarma fikri backlog'a gitti.

## Testler
- 227 EditMode testi, hepsi geçiyor (`ReverseSolver.Core.Tests`).
- Kurala bilerek bir hata eklendiğinde 160'ı kırıldı. Testler kuralı gerçekten denetliyor; hata geri alındı.
- iPhone Safari'de hiçbir şey test edilmedi.

## Senin yapacakların
1. **GitHub Pages'i aç:** github.com/zeydusht/reverse-solver-unity → Settings → Pages → Source: "Deploy from a branch" → `gh-pages`, `/ (root)` → Save. 1–2 dk sonra https://zeydusht.github.io/reverse-solver-unity/ açılmalı. 404 alırsan Pages ayar sayfasının ekran görüntüsünü getir.
2. **M1 kontrolü:** iPhone Safari'de `/gz/` adresini aç. Süre panelinde "40 seviye okundu, 40 çözülebilir" yazmalı. Ekran görüntüsünü getir.
3. **4G ölçümü:**
   - Wi-Fi ve 5G kapalı olmalı. Önce fast.com'da hız testi yap.
   - `/gz/` ve `/br/` adreslerinin her birini 3 kez aç. Her açılıştan önce Ayarlar → Safari → Geçmişi ve Web Sitesi Verilerini Temizle.
   - Getir: her açılışın panel değerleri (toplam, indirme, JS açma, wasm, motor) ve hız testi sonucu.
4. **Ana ekran:** Paylaş → Ana Ekrana Ekle ile ekle ve oradan aç. Altta farklı renkte bir şerit kalmadığını kontrol et, ekran görüntüsü getir.
5. **Güvenlik duvarı:** Windows Güvenlik Duvarı'nda Python'un "Ortak" ağ iznini kaldır. Yerel sunucular kapalı.

## Açık işler
- Mühendislik: M2 (9 kabul ölçütü, `PRODUCT.md` → "M2 tanımı"). 4G sonucuna göre sıkıştırma kararını uygulamak. M3'te geçici açılış kontrolünü ve süre panelini kaldırmak.
- Backlog: PhysX modülünü çıkarmak, Input Manager'a geçiş (yalnızca 4G 10 sn'yi aşarsa), Unity içinde seviye editörü.

Commit'ler: main `bee34ed`, `806238d`, `e1cf67f` ve bu raporun commit'i. gh-pages `0552040`. Hepsi push'landı.
