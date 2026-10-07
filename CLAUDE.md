# Reverse Solver (Unity)

Web bulmaca oyunu Reverse Solver'ın Unity 6.6 (URP 2D) sürümü. Hedef: iPhone Safari'de dikey oynanan Web build, GitHub Pages'te yayın. Web sürümü referans olarak `../web-reference` klasöründe (Unity projesinin dışında; Assets'e kopyalanmaz).

## Çalışma akışı

- **Ürün kararlarının tek kaynağı `docs/PRODUCT.md`.** Hedefler, başarı ölçütleri, karar kaydı, ölçümler ve açık işler orada. Ona aykırı iş yapma; bir iş bir kararla çelişiyorsa önce PM'e sor.
- **`product-manager` subagent'ını çağır** ve raporunu ona ver:
  - bir milestone ya da önemli bir iş bittiğinde,
  - ürün kararı gereken bir soru çıktığında (kapsam, öncelik, görsel yön, hedef değerler),
  - Zeyd bir ölçüm veya test sonucu getirdiğinde.
  Rapor somut olsun: ne yapıldı, ölçülen sayılar, neyin doğrulandığı ve neyin doğrulanmadığı.
- **Ürün sorularını Zeyd'e sormadan önce PM'e sor.** Zeyd'e sadece PM'in Zeyd'e bıraktığı kararları sor. Teknik uygulama kararları mühendisliğindir (ana Claude).
- **Her mesajın sonunda** son PM çıktısındaki **"Zeyd'in yapacakları"** ve **"Açık işler"** bölümlerini göster.
- PM yalnızca `docs/PRODUCT.md`'yi düzenler; kod ve ayar değişiklikleri ana Claude'dadır.

## Mimari

Bağımlılık tek yönlü: **Core ← Presentation ← Platform**.

| Assembly | Klasör | Not |
|---|---|---|
| `ReverseSolver.Core` | `Assets/_Game/Core` | Saf C#, `noEngineReferences: true`. Oyun kuralları, seviye modeli, oturum, telemetri modeli. Unity API'si, `UnityEngine.Random` ya da motor saati kullanılmaz; zaman ve rastgelelik dışarıdan verilir. Editörde ekransız çözücü ve Monte Carlo bunun üzerinde çalışır. |
| `ReverseSolver.Presentation` | `Assets/_Game/Presentation` | Görsel katman ve girdi (Input System, uGUI + TextMeshPro). Oyun durumunu yalnızca Core'un olaylarıyla izler, doğrudan değiştirmez. |
| `ReverseSolver.Platform` | `Assets/_Game/Platform` | Web'e özgü: Supabase gönderimi, PlayerPrefs, jslib köprüleri. |
| `ReverseSolver.Editor` | `Assets/_Game/Editor` | Build ve editör araçları. |
| `ReverseSolver.Core.Tests` | `Assets/_Game/Tests/EditMode` | EditMode testleri. `CoreIsolationTests` Core'un Unity'ye bağlanmadığını korur. |

- Seviye verisi `Assets/_Game/Levels/levels.json` (`format` 1). Her seviyede kalıcı `id` (yeniden kullanılmaz, sıra değişince değişmez) ve `version` var; oynanışı etkileyen her değişiklikte `version` artar. Engel sayıları `LevelStats` ile seviyeden hesaplanır, dosyada tutulmaz.
- Kural motoru (`TravelRule`, `GreedySolver`) web oyununun kodunun birebir portu. Değiştirirsen `Tools/make_travel_golden.js` ile web kodundan üretilen altın değer testleri (`Golden/travel_golden.json`) bunu yakalar; web kuralı bilinçli olarak değişmedikçe altın dosyayı yeniden üretme.

## Araçlar (`Tools/`, Unity dışında)

- `python Tools/extract_levels.py` — `../web-reference/index.html`'deki seviyeleri `levels.json`'a çıkarır. Yalnızca seviyeler hâlâ web'deki haliyken anlamlı; yeniden tasarım başladıktan sonra `levels.json` doğrudan düzenlenir.
- `node Tools/make_travel_golden.js` — web kural kodunu Node'da çalıştırıp altın değerleri üretir.
- `Tools/web_harness.js` — web oyununun tüm betiğini sahte DOM ile Node'da çalıştırır (oturum kuralları, booster'lar, saat); `Math.random` tohumlu Mulberry32'dir (C# `Mulberry32` ile aynı). Web davranışı hakkında bir soru varsa tahmin etmek yerine bununla ölç.
- `node Tools/make_session_golden.js` — GameSession altın senaryolarını (`Golden/session_golden.json`) ve web build'indeki açılış kontrolünün kompakt kopyasını (`Platform/BootCheck/`) üretir.
- `SmartPlayer` (Core) — çivilere uyan, bombaya en kısa yoldan giden oynayıcı; README garantisinin (40/40, bomba/donma/supap 0) kontrolü ve Faz 2 çözücüsünün temeli.
- Managed stripping High: `Assets/_Game/link.xml` Core ve Platform'u tamamen korur. EditMode testleri stripping'siz çalışır, stripping hatasını ancak web build yakalar.

## Build

- Unity menüsü **Reverse Solver → Build WebGL** → `Builds/WebGL` (Brotli, sevk edilen) ve **Build WebGL (gzip backup)** → `Builds/WebGL-gz` (git dışında). Web ayarlarını `WebBuild.ApplySettings` uygular: Brotli + decompression fallback (GitHub Pages `Content-Encoding` göndermez), özel şablon `Assets/WebGLTemplates/ReverseSolver`, High stripping, IL2CPP Optimize Size, wasm DiskSizeLTO.
- Her build boyutları `Builds/size-log.csv`'ye ekler ve `Builds/WebGL-report.txt`'ye wasm/data dökümünü yazar. Boyut değişikliklerini bununla ölç.
- Sayfa arka planı build sırasında `Game` sahnesindeki kameranın rengine eşitlenir (iOS ana ekran modundaki alt şerit için).
- Bu makinede tam IL2CPP build'i ~5–10 dakika sürer; yalnızca data değişen build'ler saniyeler sürer.
- Build'i telefonsuz doğrulamak için: `Builds/WebGL`'i `127.0.0.1`'de yayınla, Chrome'u `--headless=new --remote-debugging-port=9222 --use-angle=swiftshader --enable-unsafe-swiftshader` ve geçici `--user-data-dir` ile aç, sayfanın süre panelini CDP üzerinden oku. (`--virtual-time-budget`/`--dump-dom` işe yaramaz: indirmeler bitmeden döner.) iPhone Safari doğrulamasının yerini tutmaz.
- **gh-pages'e `Builds/publish/br` ve `Builds/publish/gz`'den yayınla**, `Builds/WebGL*`'den değil: Unity'nin artımlı build'i sonraki build başka klasöre gidince öncekinin `Build/` dosyalarını siliyor; `WebBuild` her başarılı build'i `Builds/publish`'e kopyalıyor.
- Gerçek editör günlüğü `Logs/Editor.log` (proje içinde); `%LOCALAPPDATA%` altındaki Editor.log bu kurulumda güncellenmiyor.
- Yayın: `gh-pages` dalı. `/br/` Brotli (varsayılan), `/gz/` gzip (yedek); kök `index.html` `/br/`'ye yönlendirir (playtest linki). Her yeni build ikisine de konur. Büyük push'ta `git -c http.postBuffer=524288000 push` gerekir.

## Kurallar

- Supabase'in yalnızca publishable/anon anahtarı repoya girebilir; `service_role` ya da başka gizli anahtar asla. Push'tan önce kontrol et.
- Commit mesajları İngilizce.
