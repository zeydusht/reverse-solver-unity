# Teknik kontrol — PM_107 §D3

Sonuç: **32/32 istenen PNG doğru boyutta**. A, B, C ve final için sekizer dosya eksiksizdir. 28 ayrık varlığın tamamı gerçek RGBA saydamdır; dört tam ekran önizleme opaktır.

## Üretim ve arka plan

Yerleşik **image_gen.imagegen** kullanıldı; dış API/CLI kullanılmadı. Atlaslar gerçek saydam olarak üretildi. Bu yüzden düz **#FF00FF zeminle üretim ve anahtarlama gerekmedi**. Çıktı atlaslarının 1254×1254 gerçek boyutları üzerinden kırpma yapıldı; teslim dosyaları aşağıdaki kesin boyutlara getirildi.

Pillow 13.0.0 ve NumPy 2.5.4 ile kırpma, alfa temizliği, yeniden örnekleme ve ekran birleştirme yapıldı. Düşük alfadaki renk saçakları temizlendi; alfa=0 piksellerinde RGB de sıfırlandı. Son boyutlandırma sonrası yeniden kenar kontrolü uygulandı. Kod dosyası üretilmedi; teslim görselleri ve bu görevde istenen raporlar yazıldı.

Magenta denetimi iki ayrı eşikle sayıldı:

- Tam anahtar rengi: R=255, G=0, B=255 ve alfa>0.
- Magenta benzeri renk: R≥240, G≤30, B≥240 ve alfa>0.
- Kenar: görünür alfa maskesinin 3×3 erozyonla bulunan dış bir piksel halkası.

**28 saydam PNG'nin tümünde tam magenta=0, magenta benzeri=0, kenarda magenta benzeri=0.** Tam saydam piksellerde kalan renkli RGB=0. Jiggle'ın gerçek mor/pembe gövde renkleri korunmuştur; bu eşikler onları anahtar rengi olarak değerlendirmez.

## Boyutlar — dosya bazında

| Dosya | İstenen px | Ölçülen px | Mod | Sonuç |
|---|---:|---:|---|---|
| [A_onizleme.png](A_onizleme.png) | 780×1688 | 780×1688 | RGBA | Geçti |
| [A_strip.png](A_strip.png) | 1170×264 | 1170×264 | RGBA | Geçti |
| [A_selected.png](A_selected.png) | 330×330 | 330×330 | RGBA | Geçti |
| [A_divider.png](A_divider.png) | 12×180 | 12×180 | RGBA | Geçti |
| [A_icon_home.png](A_icon_home.png) | 384×384 | 384×384 | RGBA | Geçti |
| [A_icon_collection.png](A_icon_collection.png) | 384×384 | 384×384 | RGBA | Geçti |
| [A_icon_jiggles.png](A_icon_jiggles.png) | 384×384 | 384×384 | RGBA | Geçti |
| [A_icon_trophy.png](A_icon_trophy.png) | 384×384 | 384×384 | RGBA | Geçti |
| [B_onizleme.png](B_onizleme.png) | 780×1688 | 780×1688 | RGBA | Geçti |
| [B_strip.png](B_strip.png) | 1170×264 | 1170×264 | RGBA | Geçti |
| [B_selected.png](B_selected.png) | 330×330 | 330×330 | RGBA | Geçti |
| [B_divider.png](B_divider.png) | 12×180 | 12×180 | RGBA | Geçti |
| [B_icon_home.png](B_icon_home.png) | 384×384 | 384×384 | RGBA | Geçti |
| [B_icon_collection.png](B_icon_collection.png) | 384×384 | 384×384 | RGBA | Geçti |
| [B_icon_jiggles.png](B_icon_jiggles.png) | 384×384 | 384×384 | RGBA | Geçti |
| [B_icon_trophy.png](B_icon_trophy.png) | 384×384 | 384×384 | RGBA | Geçti |
| [C_onizleme.png](C_onizleme.png) | 780×1688 | 780×1688 | RGBA | Geçti |
| [C_strip.png](C_strip.png) | 1170×264 | 1170×264 | RGBA | Geçti |
| [C_selected.png](C_selected.png) | 330×330 | 330×330 | RGBA | Geçti |
| [C_divider.png](C_divider.png) | 12×180 | 12×180 | RGBA | Geçti |
| [C_icon_home.png](C_icon_home.png) | 384×384 | 384×384 | RGBA | Geçti |
| [C_icon_collection.png](C_icon_collection.png) | 384×384 | 384×384 | RGBA | Geçti |
| [C_icon_jiggles.png](C_icon_jiggles.png) | 384×384 | 384×384 | RGBA | Geçti |
| [C_icon_trophy.png](C_icon_trophy.png) | 384×384 | 384×384 | RGBA | Geçti |
| [final_onizleme.png](final_onizleme.png) | 780×1688 | 780×1688 | RGBA | Geçti |
| [final_strip.png](final_strip.png) | 1170×264 | 1170×264 | RGBA | Geçti |
| [final_selected.png](final_selected.png) | 330×330 | 330×330 | RGBA | Geçti |
| [final_divider.png](final_divider.png) | 12×180 | 12×180 | RGBA | Geçti |
| [final_icon_home.png](final_icon_home.png) | 384×384 | 384×384 | RGBA | Geçti |
| [final_icon_collection.png](final_icon_collection.png) | 384×384 | 384×384 | RGBA | Geçti |
| [final_icon_jiggles.png](final_icon_jiggles.png) | 384×384 | 384×384 | RGBA | Geçti |
| [final_icon_trophy.png](final_icon_trophy.png) | 384×384 | 384×384 | RGBA | Geçti |

PNG'lerin en büyüğü **1.421.744 bayt**; her dosya 10 MB sınırının altında. Ek inceleme görselleri PNG teslim setinden ayrıdır: `karsilastirma.jpg` 1170×1040, `final_48pt_kontrol.jpg` 780×550.

## Final alfa ölçümleri

| Dosya | Tam saydam piksel | Kısmi alfa pikseli | Tam magenta | Kenarda magenta benzeri |
|---|---:|---:|---:|---:|
| final_strip.png | 4258 | 40206 | 0 | 0 |
| final_selected.png | 14189 | 9094 | 0 | 0 |
| final_divider.png | 1482 | 389 | 0 | 0 |
| final_icon_home.png | 65481 | 16528 | 0 | 0 |
| final_icon_collection.png | 81579 | 13044 | 0 | 0 |
| final_icon_jiggles.png | 87329 | 13957 | 0 | 0 |
| final_icon_trophy.png | 81504 | 31894 | 0 | 0 |

**final_selected.png içinde saydam olmayan ilk satır: y=6 (0 tabanlı; insan sayımıyla 7. satır).** y=0…5 satırlarının tamamında alfa=0. Tuval 330×330 px; iç panel yerleşimi (12,6) başlangıcında 306×318 px. Panel dosyası ikon ve etiket içermez; bunlar önizlemede ayrı katmanlardır.

3× şerit 1170×264 px = 390×88 pt. 2× önizlemede 780×176 px olarak (0,1512) konumunda durur. Panel 220×220 px olarak (0,1472) konumuna yerleştirilir. Geometrik alfa başlangıcı 1472 + 6×2/3 = 1476 px; şeritten **36 px = 18 pt** yukarıdadır. Lanczos'un çok düşük alfalı kenar saçağı y=1474'e kadar uzanır; bu saçak dahil ölçüm **19 pt** olur. Yaklaşık 18 pt çıkıntı hedefi korunur.

## Ekranın korunması

Dört önizleme de kaynak `docs/art/pm107/once_sonra/menu_once.png` üzerine birleştirildi. Ana ekran yeniden üretilmedi.

| Önizleme | Kaynakla aynı üst satırlar | Bu alanda değişen piksel | Navigasyon maskesi dışında değişen piksel |
|---|---|---:|---:|
| A_onizleme.png | y=0…1473, tüm 780 px genişlik | 0 | 0 |
| B_onizleme.png | y=0…1473, tüm 780 px genişlik | 0 | 0 |
| C_onizleme.png | y=0…1473, tüm 780 px genişlik | 0 | 0 |
| final_onizleme.png | y=0…1473, tüm 780 px genişlik | 0 | 0 |

Kontrol maskesi: tam genişlikte y≥1512 şerit alanı ve x<283/y≥1474 eski/yeni yükselen sekme alanı. Eski seçili sekmenin daha geniş altın omzu, yalnız x=212…282/y=1476…1511 içinde ve altın piksellerle sınırlı olarak aynı satırdaki halı örneğiyle temizlendi. Yeni panel dışında kalan ekran içeriğine dokunulmadı.

## Görsel kontrol ve kapsam

[Üç fikir karşılaştırması](karsilastirma.jpg) ve [final 48 pt kontrolü](final_48pt_kontrol.jpg) açılarak incelendi. İkonlar 48×48 px kutularda, 1×=1 pt kabulüyle lacivert, krem ve beyaz üzerinde kontrol edildi. Önizlemede yalnız seçili Ev sekmesi ikon+etiket; diğerleri yalnız ikon. İnce ayırıcılar ayrı katmandır. Etiket oyundaki Baloo2 ExtraBold ile yazıldı; inceleme JPG'lerinde Türkçe glifler için Segoe UI Bold kullanıldı.

Çubuk, panel, müze, tablo çerçeveleri ve kupa lacivert/krem/altın ailesindedir. Görevde istenen mavi/pembe/mor Jiggle üçlüsü korunmuştur. Referans oyunun renkleri veya ikonları kopyalanmadı.

Repo içinde yalnız `docs/art/pm107/codex/alt_cubuk/` altına yazıldı. Oyun dosyaları değiştirilmedi. İstenen 32 PNG'ye ek iki inceleme JPG'si, üç Markdown raporu ve bir istem JSON'u teslim edildi; mevcut GOREV.md korundu.

Süre başlangıcı: 2026-11-24 22:55:29 UTC. Son dosya doğrulaması: 23:12:31 UTC; geçen süre **17 dakika 2 saniye**, 25 dakika sınırı içinde.
