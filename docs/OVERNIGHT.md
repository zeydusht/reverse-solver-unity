# Gece raporu — 2026-10-06

**Sabah ilk iş: web build'i alınamıyor.** Windows'un Akıllı Uygulama Denetimi (Smart App Control) Unity'nin kendi dosyasını engelliyor:
`WebGLPlayerBuildProgram.Data.dll ... Uygulama Denetimi ilkesi bu dosyayı engelledi (0x800711C7)`.
17:35'teki M2 build'i sorunsuzdu; engel sonradan başladı. Kod ya da proje sorunu değil. Bu yüzden bu gece hiçbir build yayınlanamadı; M3 kodu editörde tamam ama canlıda değil.

## Senin yapacakların (sırayla; biri tutunca dur)
1. **Build engeli** (PM'in sırası):
   1. Bilgisayarı yeniden başlat, Unity'de *Reverse Solver → Build WebGL*'i bir kez dene.
   2. Tutmazsa Unity Hub → Installs → 6000.6.4f1 → Uninstall; aynı sürümü **WebGL Build Support** ile yeniden kur, build'i dene.
   3. Tutmazsa bulut build (GitHub Actions; hazırlık durumu aşağıda, secret'ları senin eklemen gerekiyor).
   4. Başka bir bilgisayar varsa orada dene.
   5. **Smart App Control'ü kapatmak yalnızca son çare: geri dönüşsüz** — kapatınca Windows sıfırlanmadan tekrar açılamaz. Önce PM'e sor.
   - Getireceğin: hangi adımı denedin, build geçti mi, geçmediyse hata metni.
2. Build yayınlanınca iPhone'da: `?debug=1` ile en büyük tahtada (ör. `?lv=40&debug=1`) sürüklerken FPS; L01–L05, L11, L21, L31'i oyna (sürükleme parmağı takip ediyor mu, sayfa kayıyor/yakınlaşıyor mu, engeller okunuyor mu).
3. Actions'ta kuyrukta bekleyen eski re-run'ı (0552040) iptal et.
4. Playtest öncesi 4G (LTE) teyidi: kök adres, 3 açılış, panel değerleri + fast.com.

## Neler bitti
### M3 — kod tamam, build/yayın bekliyor (PM kararı)
- Tahta, parçalar, zincirler, çivi/bomba rozetleri, duvarlar, üst şerit ve bitiş kartı web'in CSS/SVG'sinden birebir taşındı (4 shader). Web görüntüsüyle yan yana: `docs/screens/m3/L40_375x667.png` ve `docs/screens/m3/web/L40_375x667_web.png`. Diğer 23 görüntü (6 seviye × 3 boyut, bitiş kartları) `Builds/screens/m3/` altında (git dışı).
- Hücre boyutu web'in formülüyle aynı: 375×667'de en büyük tahtada **56 pt** (dokunma alanı ≥ 44 pt şartı sağlanıyor); 390×844'te 58, 430×932'de 65.
- **Türkçe karakterler:** TMP'nin hazır fontunda Ğ ğ İ ı Ş ş yoktu (yedek font 350 KB'lık TTF'yi build'e sokacaktı). Yerine 115 karakterlik kendi statik atlasımız (512×512) yapıldı; ğ ü ş ı ö ç İ Ğ Ü Ş I Ö Ç doğru görünüyor: `docs/screens/m3/charset_375x667.png`. TTF build'e girmiyor. **Fontun build boyutuna etkisi ölçülemedi** (build alınamıyor); ilk build'de size-log'dan raporlanacak.
- Renk uzayı Gamma'ya alındı: yarı saydam katmanlar artık tarayıcıdaki CSS gibi karışıyor (Linear'da sönük yıldızlar ve perdeler açık çıkıyordu).
- Sürükleme: gerçek Input System yolundan sanal fareyle test edildi — 15 karede takip hatası **0,00 pt** (yumuşatma yok), kısa sürükleme yerine dönüyor, tam sürükleme parçayı çıkarıyor.
- `?lv=N` ile seviye seçimi; `?debug=1` ile süre paneli, açılış kontrolü ve sürüklerken medyan FPS satırı. iOS'ta iki parmakla ve çift dokunarak yakınlaştırma engellendi.
- Testler: 334 EditMode testi geçiyor.

## PM kararları (ayrıntı `docs/PRODUCT.md`)
- M3 kapanmadı: "kod tamam, build/yayın bekliyor". İlk build'de M3 + M4 birlikte yayınlanacak, iPhone testleri tek seferde.
- Bu gece: M4 kodu ve M5'in Supabase'e dokunmayan ön işleri (PlayerPrefs, kayıt kuyruğu, `level_id` SQL taslağı). Canlı gönderim yok.
- Bulut build (GitHub Actions) bu gece ~1 saatlik zaman kutusuyla hazırlanacak; yalnızca elle tetiklenir, secret repoya girmez.

## Testlerin durumu
334/334 EditMode (editörde). Web build'i yok, iPhone testi yok.

## Önceki gece — 2026-10-05

Kısa özet: boyut çalışması kapandı (11,8 → 8,7 MB gzip, 6,8 MB Brotli), M1 bitti (seviye verisi + kural motoru, 227 test yeşil), ikisi de PM onaylı. **Sabah ilk iş GitHub Pages'i açman lazım; açılmadan hiçbir adres çalışmıyor.**

### Neler bitti

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

### PM kararları (ayrıntı ve gerekçeler `docs/PRODUCT.md`'de)
- Boyut adımları B–F kabul, splash kapalı kalıyor. 6 MB ara hedefi engelleyici değil.
- Sevk edilen sıkıştırma şimdilik **gzip**. Brotli ancak 4G'de medyanda ≥ 0,5 sn hızlıysa seçilecek. İkisi de 10 sn'yi aşarsa boyut işi yeniden açılacak.
- UI Toolkit çıkarılmayacak, URP/uGUI fork'lanmayacak. Input System'den vazgeçilmeyecek.
- Seviye `id`'si kalıcı, `version` oynanış değişince artar.
- M1 kapandı (iPhone kontrolü sende). Sıradaki iş **M2: GameSession** (çivi, bomba, supap, booster'lar, süre, deneme kaydı). İnsan metriklerinin tanımı sabitlendi.
- PhysX modülünü çıkarma fikri backlog'a gitti.

### Testler
- 227 EditMode testi, hepsi geçiyor (`ReverseSolver.Core.Tests`).
- Kurala bilerek bir hata eklendiğinde 160'ı kırıldı. Testler kuralı gerçekten denetliyor; hata geri alındı.
- iPhone Safari'de hiçbir şey test edilmedi.

### Senin yapacakların
1. **GitHub Pages'i aç:** github.com/zeydusht/reverse-solver-unity → Settings → Pages → Source: "Deploy from a branch" → `gh-pages`, `/ (root)` → Save. 1–2 dk sonra https://zeydusht.github.io/reverse-solver-unity/ açılmalı. 404 alırsan Pages ayar sayfasının ekran görüntüsünü getir.
2. **M1 kontrolü:** iPhone Safari'de `/gz/` adresini aç. Süre panelinde "40 seviye okundu, 40 çözülebilir" yazmalı. Ekran görüntüsünü getir.
3. **4G ölçümü:**
   - Wi-Fi ve 5G kapalı olmalı. Önce fast.com'da hız testi yap.
   - `/gz/` ve `/br/` adreslerinin her birini 3 kez aç. Her açılıştan önce Ayarlar → Safari → Geçmişi ve Web Sitesi Verilerini Temizle.
   - Getir: her açılışın panel değerleri (toplam, indirme, JS açma, wasm, motor) ve hız testi sonucu.
4. **Ana ekran:** Paylaş → Ana Ekrana Ekle ile ekle ve oradan aç. Altta farklı renkte bir şerit kalmadığını kontrol et, ekran görüntüsü getir.
5. **Güvenlik duvarı:** Windows Güvenlik Duvarı'nda Python'un "Ortak" ağ iznini kaldır. Yerel sunucular kapalı.

### Açık işler
- Mühendislik: M2 (9 kabul ölçütü, `PRODUCT.md` → "M2 tanımı"). 4G sonucuna göre sıkıştırma kararını uygulamak. M3'te geçici açılış kontrolünü ve süre panelini kaldırmak.
- Backlog: PhysX modülünü çıkarmak, Input Manager'a geçiş (yalnızca 4G 10 sn'yi aşarsa), Unity içinde seviye editörü.

Commit'ler: main `bee34ed`, `806238d`, `e1cf67f` ve bu raporun commit'i. gh-pages `0552040`. Hepsi push'landı.