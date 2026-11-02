# P20-G kısa rapor

Uygulandı; commit ve yayın yapılmadı. Core, Platform, bölüm dosyaları ve telemetri gönderim akışı değiştirilmedi.

## Dosyalar

`Assets/_Game/Presentation/`: `BoosterInventory.cs` (+ `.meta`), `GameRoot.cs`, `TutorialScript.cs`, `Resources/tutorial.json`, `Board/TutorialSpotlight.cs`, `Ui/BoosterBar.cs`, `Ui/Hud.cs`, `Ui/Texts.cs`.

`Assets/_Game/Tests/EditMode/TutorialScriptTests.cs`, `OVERNIGHT.md`, bu rapor.

## Hedefler ve TR / EN metinler

| Booster | Seçim ve gerçek hedef | Metinler |
|---|---|---|
| Makas, bölüm 5 | Çözüm sırasındaki ilk kesilebilir eklem; yoksa herhangi bir kesilebilir eklem. Mevcut veride parça 3, dikey eklem (3,0); iki hücre aydınlık. | Makası seç / Tap the scissors; Parçaları ayırmak için eklemi kes / Tap the joint to cut it |
| Değnek, bölüm 9 | Hedef yok; tuştan tüm eklemleri yeniden düzenler, onay kartı açılmaz. | Değneği seç / Tap the wand |
| Saat, bölüm 15 | Hedef yok; +20 saniye, sayaç üstünde 0,8 saniye yükselip sönen yazı. | Saate dokun / Tap the clock; +20 sn / +20 sec |
| Çekiç, bölüm 23 | Çözüm sırasındaki ilk çivili parça; yoksa çözümün ilk mevcut parçası. Mevcut veride parça 28. | Çekici seç / Tap the hammer; Parçayı çıkarmak için ona dokun / Tap a piece to knock it out |

Envanter ve eğitim bayrakları: `Host.Current.Store`, **`rs_boosters`**. Başlangıç sabiti **`BoosterStart = 3`**. Başarılı ücretsiz eğitim sonrası bir kez 3 verilir; normal kullanımlar hemen kaydedilir, bölüm değişince dolmaz. Kayıtsız eski ilerlemede geçilmiş açılış bölümleri için başlangıç stoğu verilir.

Çoklu spotlight: zincirli mantıksal parçanın iki hücresi, aradaki zincir ve çözüm yönündeki iki çıkış şeridi; çivi/bomba parçası ve sayaçları; duvar şeritlerinin tamamı. Örtüşen delikler birleşim olarak çıkarılır.

## Kontrol ve kalanlar

- Gerçek Core + hedef/envanter kodu, geçici .NET harness: **34 kontrol geçti** (ücretsiz etkiler, Used/stok, olay, tek seferlik stok, kalıcılık, eski ilerleme, hedefsiz atlama).
- Maske birleşimi/örtüşme/kırpma: **10.000 nokta geçti**. Değişen 8 C# dosyası: **C# 9 sözdizimi hatası yok**.
- EditMode testleri genişletildi; Unity'de çalıştırılmadı. Unity derlemesi, ekran görüntüleri ve canlı Supabase doğrulaması kullanıcıya bırakıldı.
- **PM kararı bekliyor:** Değnekte hedef adımı yok. Hiç hedef bulunmayan veride hedef adımı atlanır ve eğitim bayrağı/stok ödülü başarılı etki gerçekleşene kadar verilmez (mevcut bölüm 5/23'te hedef var).
- Core'un ücretsiz çekiç `Remove` işlemi `ClockStarted` açıyor. Core'a dokunmadan Presentation Tick'i ilk normal hamleye kadar bekletildi; Core bayrağının düzeltilmesi Core sahibine bırakıldı.
- 5/9/15/23'te henüz eğitilmemiş oyuncular aynı ücretsiz yardımı alır; eğitim `moves` olayını üretir, `plays.b_*` sayılmaz. Canlı gönderim bu görevde doğrulanmadı.
