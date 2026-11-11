# PM_62 kod işleri (Unity, `Assets/_Game/`)

Mimari: Core ← Presentation ← Platform. Core saf C# (Unity API yok). Presentation oyun durumunu yalnız Core olaylarıyla izler. Altın değer testlerini (`Golden/*.json`) yeniden üretme. `Library/ Logs/ Builds/ *.csproj` okuma. Görseller `Assets/Art/_yeni/T46/` altına başka bir Codex tarafından çiziliyor (liste `docs/art/task46/LISTE.md`); kod bu yolları kullansın, dosya henüz yoksa mevcut kareye düşsün (çökmesin).

1. **Test modu** URL `&test=1` yalnız `c=ic` ya da `c=p21` ile geçerli: bütün bölümler açık, bütün booster'lar açık ve sayısı 99 (azalmaz), altın 99999. Telemetri kayıtları `cohort=ic` gider. Başka cohort'ta bayrak yok sayılır. Mevcut URL bayraklarını (`&coin=`, `&devam=` vb.) nasıl okunuyorsa öyle oku.
2. **Kilitli booster'a dokununca** booster düğmesinin üstünde kısa balon: "Seviye N'de açılır" (TR; EN "Unlocks at level N"), ~1,5 sn sonra söner. N = açılış seviyesi.
3. **Makas** animasyonu: gidiş 0,45 sn, kesim 0,3 sn, çıkış 0,45 sn (toplam ~1,2–1,4 sn). Sabitler adlandırılmış alan olsun.
4. **Değnek** seçim modunda oyun süresi DURMASIN (şu an duruyorsa duraklatmayı kaldır).
5. **Saat booster yeniden** (eski kopuk kol çizimi/kodu kalksın): Jigo madalyonda `T46/clock/extend` kareleri → uzanmış `hold` döngüsü; fırça ucu Süre paneline değince `spark` + `ice_overlay` panele oturur, süre rakamları mavi-beyaz renge geçer; panelin altında 10 sn bar (`freeze_bar_bg/fill`) görünür sayımla boşalır; süre bu sırada durur (Core'daki mevcut dondurma mantığını kullan, süreyi parametre `FreezeSeconds = 10` yap); bitince `ice_break` + `retract`, Jigo normale döner.
6. **Kilit okunurluğu**: kilit görseli %15 büyük; kilit sayısı beyaz, kalın koyu kontur (TMP outline), punto büyük.
7. **Kombo**: herhangi bir yanlış hamle (takılma/bloklanan çekiş) komboyu anında sıfırlar.
8. **Kazanma ekranı**: yürüyen Jiggle'lar kalksın. Yerine Jiggle ikonu + "30 Jiggle kurtarıldı!" (sayı 0'dan sayarak artar, bitince küçük pop ölçek). Düğmeler: **Sonraki Seviye** (büyük, yeşil) ve **Menü** (küçük, ikincil). Dikey düzen: başlık, Jigo, sandık ilerlemesi, Jiggle sayısı, düğmeler; eşit boşluk. EN metin: "30 Jiggles rescued!".
9. **Jigo madalyon**: göz kırpma rastgele 4–7 sn aralıkla. Durum geçişleri (normal↔mutlu vb.) 0,25 sn crossfade + hafif ölçek esnemesi; `T46/jigo/normal_to_happy`, `happy_to_normal` varsa onları oynat. Kare animasyonun üstüne sürekli kod nefesi: sinüs ölçek ±%2 (~2,2 sn), hafif dönme ±1,5°. `T46/jigo/normal_loop`, `happy_loop` varsa onları ve 12–24 fps kullan.
10. **Bölüm haritası**: oyuncunun mevcut bölümü görünür alanda değilse kenarda soluk küçük hap "Seviye N ↓" ya da "↑" (EN "Level N"); dokununca o bölüme yumuşak kayar. Ek olarak küçük "en üste / en alta" tuşu.
11. **Kenar payı**: 24 pt kalır ama tek bir parametre (ör. `SafeMarginPt`) üzerinden; 28'e çevirmek tek satır olsun.

Değişikliklerden sonra derlemeyi kontrol et: `bash Tools/unity_batch.sh tests` (EditMode; Perf testleri hariç değerlendirilir). Kırılan test varsa düzelt. Sonunda KISA özet: değişen dosyalar (tek satırda) + test sayıları + yapılamayan madde varsa nedeni.
