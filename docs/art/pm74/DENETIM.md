# PM_74 denetimi (16 Kasım 2026, gece)

p22 build'i (PM_73 hâli) 390×844'te gezildi: test linki (`&test=1`), normal link (`&reset=1`, hikâye dahil), TR ve EN, iPhone ana ekran güvenli alanı taklidi (üst 47, alt 34 px). Görüntüler `once/` klasöründe (numaralı).

Önem: **1** = hata ya da yanlış bilgi, oyuncuyu durdurur · **2** = göze batan tarz ya da tutarlılık sorunu · **3** = cila.
Durum: ✅ bitti (p23) · ⏳ sürüyor · — yapılmadı (sebebi yanında).

| # | Ekran | Sorun | Önem | Durum |
|---|---|---|---|---|
| 1 | Booster dersleri (test linki) | Değnek (9), saat (15), makas (5), çekiç (23) dersinde booster kullanılınca ders adımı bitmiyor; o adım bütün dokunuşları yutuyor, parça, booster ve Durdur çalışmıyor (`20_degnek_sonrasi_donuk`). Sebep: test modunda envanter yazılmasın diye konan koşul ders ilerlemesini de atlıyordu. | 1 | ✅ |
| 2 | Booster seçimi | Seçimden çıkmak için yalnız aynı booster'a yeniden dokunmak gerekiyor; tahtanın dışına dokunmak hiçbir şey yapmıyor. | 1 | ✅ tahta dışına tek dokunuş iptal eder |
| 3 | Bölüm 30 kartı (TR/EN) | "Kilitli parça **N hamle** boyunca…": kilitlerin sayıları farklı olunca metinde "N" kalıyor (`12_…`, `33_…`). | 1 | ✅ |
| 4 | Kaybetme (süre) | "35 **gövde** kaldı": eski terim. | 1 | ✅ "parça" |
| 5 | Durdur, kazanma, harita, booster kilidi | TR'de "Seviye 40", "Sonraki Seviye", "Seviye 6'da açılır"; menü ve kartlar "Bölüm" diyor. | 2 | ✅ hepsi "Bölüm" |
| 6 | Bölüm 40 (8 sütun) ve geniş tahtalar | Tahta ekran kenarına yapışık (yanlarda ~4 px), duvar şeritleri ekran kenarında (`15_…`). | 2 | ✅ çerçeve dışında her yanda ≥ 12 px (8 sütun ~13 px, 6–7 sütun ~16 px), HUD ile arada 12 px |
| 7 | HUD, Jigo penceresi | Yüksük kemerli çerçevenin üstünden, düğme alttan taşıyor; göz kırpma sık. | 2 | ✅ kemer kırpması + yüksük kemerin 3 pt altında; açık göz tek kare, 3–6 sn'de bir kırpma |
| 8 | HUD, Parça kutusu | Sayının yanında küçük yapboz ikonu; sayı ortalı değil. | 2 | ✅ yalnız sayı, %30 büyük |
| 9 | Menü | Normal oyuncuda "Bölümler" düğmesi var (tek büyük "Oyna" isteniyor). | 2 | ✅ tek "Oyna · Bölüm N"; Bölümler yalnız `&test=1` |
| 10 | Saat booster | Animasyon anlaşılmıyor: Jigo'nun düğmesi Süre kutusuna gidiyor gibi görünüyor (`19_…`). | 2 | ✅ uçan çalar saat, buzlanan kutu, "+10 sn" rozeti (3 fikir) |
| 11 | Makas, çekiç, değnek | Vuruş anı zayıf; seçimde booster ikonu Jigo penceresinin üstünde asılı duruyor (`16_…`, `17_…`). | 2 | ✅ vuruş anları yeniden (her biri 3 fikir); seçim ikonu aynı kaldı |
| 12 | İsim sorusu (ilk açılış) | Eski T23 tarzı: turkuaz kurdeleli başlık. Onaylı tarz krem panel + mavi çerçeve, kurdele yok (`29_…`). Yeni oyuncunun ilk gördüğü ekran. | 2 | ✅ krem panel, mavi çerçeve, Baloo kabartma başlık |
| 13 | Durdur | Arka plan düz lacivert, tahta tamamen kayboluyor; küçük kart, ikon altı yazılar 9 px (`21_…`). Yeni kazanma ekranıyla aynı dilde değil. | 2 | ✅ yazılar 15 pt; kart ve opak arka plan bilerek kaldı (durunca tahta görünmesin) |
| 14 | Kaybetme kartı | Küçük krem kart, minik saat ikonu, Jigo yok; yeni kazanma ekranıyla aynı dilde değil (`28_…`). | 2 | ✅ kazanma ekranı dilinde |
| 15 | Süre doldu (devam) | Kartlı; kazanma kartsız. "+30 sn" satırı saat ikonuyla hizasız. | 3 | ✅ kartsız, kazanma dilinde |
| 16 | Sandık | Altın hapı (999 999) HUD'daki Süre kutusunun üstüne biniyor (`34_…`). | 3 | ✅ başlığın altında |
| 17 | Kazanma | "Menü" düğmesi ~36 px yüksek (< 44). | 2 | ✅ dokunma alanı ≥ 44×150 pt (görünüm aynı) |
| 18 | Harita | "En üste / En alta" düğmeleri ~28 px yüksek (< 44). | 3 | — harita artık yalnız test linkinde |
| 19 | Ayarlar, Profil | Kapatma (X) ~36 px, profil kalem ikonu ~22 px görünüyor. Ayarlar alt satırı yazıları 9 px ("Hikâyeyi tekrar izle"). | 3 | — dokunma alanları zaten ≥ 44 px (kodda); yazı boyu kaldı |
| 20 | Jiggles sekmesi | "0 / 2.000" çubuğu ve 500/1.000 işaretleri ne olduğu anlaşılmıyor. | 3 | — kapsam dışı (yeni özellik gerektirir) |
| 21 | Ses dengesi | En sık çalan "tık" (her çıkış) en yüksek ses (100 ms tepe −14 dB); kilit açılma sesi en kısık (−20 dB), bomba sesi −12 dB. Tık kulağı yorar, kilit kaybolur. | 3 | ✅ tık −4 dB, bomba −1,5 dB |
| 22 | Animasyon eğrileri | Merkezde 5 eğri var (Settle, FlyOut, Deal, Tick, Ease); yeni ekranlar kendi eğrisini kullanıyor. Yeni yazılanlar tek tip ease-out kullanacak. | 3 | ✅ PM_74 işlerinin hepsi kübik ease-in/out |
| 23 | Yükleme | Yerelde motor hazır 3,2 sn (Brotli 35,9 MB); canlıda ağ eklenir. | 3 | — ölçüldü, değişiklik yok |
| 24 | Dokunuş sayısı | Menü → bölüm 1, kazanma → sonraki bölüm 1 dokunuş. Ders/zor bölümlerde +1 kart dokunuşu ("Anladım", "Başla"). Hedefte. | — | ✅ hedefte |
| 25 | Güvenli alan | Çentik ve alt çizgi taklidinde HUD ve booster çubuğu güvenli alanda (`37_…`, `38_…`). | — | ✅ sorun yok |
| 26 | TR/EN taşma | Gezilen ekranlarda taşan yazı yok. | — | ✅ |
| 27 | Eski kalıntı | Çivi, Baron, turkuaz parça görülmedi; kalıntılar 4, 5 ve 12. maddelerde. | — | ✅ |
| 28 | HUD | Parça/Süre başlıklarında mavi kurdele uçları (onaylı tarz: kurdele yok). | 2 | ✅ krem hap (seçenek: lacivert, eski kurdeleli) |
| 29 | Menü → bölüm, kazanma → sonraki | Geçiş yok / koridor 3,9 sn | 2 | ✅ resme dalış ≈ 1 sn |
