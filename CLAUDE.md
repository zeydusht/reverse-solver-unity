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

- Seviye verisi JSON olarak durur; ileride her seviyede `id` + `version` olacak ve engel sayıları seviyeden hesaplanabilecek.
- Managed stripping High: `Assets/_Game/link.xml` Core ve Platform'u tamamen korur. EditMode testleri stripping'siz çalışır, stripping hatasını ancak web build yakalar.

## Build

- Unity menüsü **Reverse Solver → Build WebGL** → `Builds/WebGL` (git dışında). Web ayarlarını `WebBuild.ApplySettings` uygular: gzip + decompression fallback (GitHub Pages `Content-Encoding` göndermez), özel şablon `Assets/WebGLTemplates/ReverseSolver`, High stripping, IL2CPP Optimize Size.
- Her build boyutları `Builds/size-log.csv`'ye ekler ve `Builds/WebGL-report.txt`'ye wasm/data dökümünü yazar. Boyut değişikliklerini bununla ölç.
- Sayfa arka planı build sırasında `Game` sahnesindeki kameranın rengine eşitlenir (iOS ana ekran modundaki alt şerit için).
- Bu makinede tam IL2CPP build'i ~10 dakika sürer; yalnızca data değişen build'ler saniyeler sürer.

## Kurallar

- Supabase'in yalnızca publishable/anon anahtarı repoya girebilir; `service_role` ya da başka gizli anahtar asla. Push'tan önce kontrol et.
- Commit mesajları İngilizce.
