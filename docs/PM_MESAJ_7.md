# PM → Claude Code (24 Ekim 2026, 17:15): uygulamayı playtest'e hazırla

Zeyd yaklaşık 2 saat yok. Ona soru sorma. Belirsiz bir durumda makul seçimi yap ve kararı rapora yaz.

## Durum

- **SET40 v3 yapılmadı.** PM_MESAJ_6 limit yüzünden çalışmadı. `CODEX_PLAN.md`'de SET40 v3, tutorial, tanıtım ve Build 5 işleri "Claude bekliyor" olarak duruyor.
- **Codex'in yaptıkları:**
  - Sanat paketi: `docs/art/HANDOFF.md`
  - `docs/sql/008_playtest_analiz.sql` (çalıştırılmadı)
  - 30 "gece turu": `Assets/_Game/...` altındaki C# dosyalarına yalnızca yorum ekledi, davranış değişmedi.
- **Codex'in engeli:** Unity lisans IPC ve Chrome 9333 nedeniyle yerel test ve build alamıyor. Build ve test senin işin.

## Sıra (her adımdan sonra commit et ve BACKLOG.md'nin Mühendislik bölümünü güncelle)

1. **Codex'in yorum değişikliklerini kontrol et.** `git diff` ile yalnızca yorum ve boşluk değiştiğini doğrula; davranışı değiştiren bir şey varsa geri al. Ardından tek commit at: "docs: Codex API sözleşme yorumları". Testler yeşil olmalı.
2. **SET40 v3.** PM_MESAJ_6'nın 2. ve 3. bölümleri aynen geçerli:
   - Zeyd'in slot planı: tutorial 1–2; tanıtımlar 3, 6, 10, 14; zorlar 19, 25, 30; zirveler 35–40; final 40.
   - Booster açılışları tanıtım bölümleriyle çakışmayacak.
   - El animasyonu, tanıtım penceresi ve zor bölüm uyarısı en basit haliyle yapılacak.
   - Tanıtım pencerelerinde ŞİMDİLİK oyundaki mevcut engel grafiklerini kullan. Codex'in engel ve booster konseptleri henüz onaylı değil.
   - Hikâye dili `docs/HIKAYE.md`'den gelir: Jiggles, Baron. Metinler tek dosyada dursun, İngilizce sürüme hazır olsun.
3. **Onaylı sanatı entegre et (yalnızca bu ikisi):**
   - **İkon:** `Assets/Art/Icon/Exports/2-market-mavi-{1024,512,256}.png` dosyalarını WebGL/PWA ikonu, favicon ve manifest olarak kullan.
   - **Menü arka planı:** `Assets/Art/Menu/corridor-v04-source.png` statik olarak, bölüm seçimi ya da başlangıç ekranında. Üstüne mevcut "Oyna" tuşunu koy; yazı dinamik kalsın. iPhone portre güvenli alanını koru. Texture'ı sıkıştır, uzun kenarı en fazla 2048 piksel olsun.
   - Zoom geçişi, animasyon, koşu kareleri ve sayaç playtest'ten SONRA gelecek. Bunlara dokunma.
4. **Build 5 hazırlığı:** PM_MESAJ_6'nın 4. bölümü. Varsayılan set v3 olacak; `?set=classic`, 👍/👎 ve kayıt açık. Yerel testi yap ama gh-pages'e **gönderme**. Önizlemeyi yalnızca `?set=redesign` yoluna koy.
5. **Sabah teslimi:** OVERNIGHT.md başına en fazla 8 satır yaz. Zeyd'in test listesi: 1, 3, 6, 10, 14, 19, 40 ve ana menü/ikon görünümü.

## Codex'e iş paslama (token tasarrufu, ZORUNLU)

Codex kod yazabilir ama Unity çalıştıramıyor. Karar gerektirmeyen, tanımı net mekanik işleri ona ver:

- **Nasıl:** `docs/CODEX_GOREV.md` dosyasına numaralı görev yaz. Her görevde şunlar olsun: amaç, dokunacağı dosyalar, kabul ölçütü, "push/deploy yok".
- **Uygun işler:**
  - görsel export'ları (yeniden boyutlandırma, sıkıştırma, atlas)
  - metin dosyası doldurma, İngilizce çeviri taslağı
  - Unity'siz .NET testleri
  - SQL taslağı
- **Uygun olmayan işler:** seviye tasarımı, zorluk, oyun hissi ve ürün kararları. Bunlar senin işin.
- Codex'e iletimi Zeyd yapar ("CODEX_GOREV'i oku ve yap"). Sonucu Codex aynı dosyada "Bitti" olarak işaretler. Sen sonucu git diff ile kontrol edip birleştirirsin.
- **Çakışmayı önlemek için:** Codex yalnızca `docs/art/`, `Assets/Art/` ve CODEX_GOREV.md'de adı geçen dosyalara dokunur. Aynı dosyada ikiniz aynı anda çalışmayın.

## Token kuralları

- PRODUCT.md'yi grep ile oku; karar kaydına en fazla 3 satır ekle.
- Büyük dosya ve logları baştan okuma; son 20 satıra bak.
- En fazla 1 alt ajan kullan.
- Uzun koşuları arka planda çalıştır.
- Rapor en fazla 1 sayfa olsun.
- Kesilirsen kaldığın adımdan devam et.
