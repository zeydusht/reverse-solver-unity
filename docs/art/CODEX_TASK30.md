# CODEX_TASK30 — HUD'u Royal Match ölçüsünde yeniden çiz + kilit + zincir + bomba kıvılcımı

Zeyd TASK29 önizlemesine baktı: **olmadı**. Şerit ince, haplar küçük, madalyon küçük, Jigo içine sıkışmış, turkuaz düz ve soluk; katman, gradyan ve gölge yok. Suç brifteydi (küçültün demiştik). Yeni kural: **boyut, yerleşim ve katman yapısı Royal Match ile birebir.** Renk ailesi, karakter ve yazılar bizim.

## 0. Önce kendin ölç (zorunlu)
- Referans: `docs/art/Reference/rm_oyun_ekrani.png`. PM ölçümü: `docs/art/HUD_ANALIZ_RM.md`.
- Görseli kendin de ölç (Python/PIL ile şerit sınırları, katman kalınlıkları, renkler). PM tablosuyla farkın varsa `docs/art/task30/olcum.md`'ye yaz. Tersine mühendisliği yazmadan çizime geçme.

## 1. Üst şerit (yeniden)
`HUD_ANALIZ_RM.md` §1 birebir: şerit ekranın %19'u, alt bordür (açık sarı → altın → turuncu) + **altında 3–4 pt yumuşak gölge**.
- Sol panel "Parça", sağ panel "Süre": RM Target/Moves ile **aynı boy** (113×118 pt), aynı katman sırası (açık mavi → lacivert → turuncu → ışık → altın → koyu bant → bej bevel → krem gövde), üstte kurdele. Yazılar ve rakamlar ayrı katman.
- Orta: kemerli pencere 127 pt genişlik, 5 katman çerçeve, iç zemin gece→camgöbeği gradyanı.
- **Pause yok** (alta Ayarlar'a gidiyor).
- Şerit rengi: **3 aday** (A petrol-turkuaz, B mavimsi, C yeşilimsi; renkler analiz §4). Hepsi gradyan, düz renk yok.

## 2. Alt şerit (yeniden)
§2 birebir: şerit %8,6, **5 yuva** (4 booster + Ayarlar), tuş çapı 62 pt, şeritten çapın %25'i kadar yukarı taşar, her tuşun altında koyu yuva/çukur.
- Tuş katmanları: turuncu halka → altın halka → açık yeşil ışık → yeşil gövde (üst açık, alt koyu) → iç disk → ikon. Altta gölge. Basılı / kilitli / tükenmiş hâlleri de.
- Rozet: kırmızı küre gradyan + açık kontur, rakam ayrı katman.
- Ayarlar tuşu: aynı halka, mavi gövde, dişli ikon.
- İkonlar: makas, yıldız asa, **mavi çekiç** (`task28/_arsiv/Z2-v3-mavi-cekic`), saat. İkonları yeniden çizme, yalnız yeni tuşlara yerleştir.

## 3. Jigo büstü (yeniden)
TASK29'daki portreler 96 pt'de birbirinin aynısı görünüyor ve tam boy figür pencereye sıkışmış.
- **Büst**: baş + omuz + yüksük + fırça ucu. Yüz pencerenin büyük kısmını kaplar, **dışarı taşmaz**.
- İfadeler **abartılı** ve uzaktan ayırt edilir: normal (gülümser, göz kırpar), tedirgin (kaşlar yukarıda, ter damlası, gözler yana), korku (ağız açık, gözler kocaman, yüksük titrer), üzgün (yüksük göze düşmüş, dudak titrer), mutlu (kocaman gülümseme, fırça havada). Her hâl 4–6 kare.
- Görünüş `task26/kahraman-final.png` ile birebir (turkuaz, gümüş yüksük, mavi uçlu fırça, düğme kalkan).
- Sevinç (tam boy, 8 kare) ve devam paneli TASK29'dan **kalabilir**, yalnız devam paneline yeni katmanlı stil uygula.

## 4. Asma kilit — yeniden (Zeyd: "oyunla hiç uyuşmadı")
Gri çelik kilit kalkıyor. Kilit kalacak ama oyunun malzemesine uyacak: tombul, oyuncak gibi, katmanlı, palet altını. **3 aday**:
- A) **Altın oyuncak kilit:** altın gövde, kalın lacivert kontur, krem numara plakası, parlak kulp.
- B) **Kont'un mühürlü kilidi:** altın kilit, gövdede kırmızı balmumu mühür (Kont'un işareti), numara mührün üstünde. Hikâyeye bağlanır.
- C) **Ahşap + altın kilit:** sıcak ahşap gövde, altın kulp ve köşeler, krem numara.
Hepsinde: açılma (kulp fırlar, 3–4 kare) + düşme (2–3 kare). 64 pt'de net okunur.

## 5. Zincir — eski görünüme dön
Zeyd eski zinciri daha çok beğendi: şimdiki **çok ince, seçilmiyor**.
- Eski zincirden (Task17) **tek halka sprite'ı** çıkar. Eski kalınlık ve renk çiftleri korunur; bugünkünden en az 1,6 kat kalın.
- Bağlantı noktaları kalsın (5 renk + işaret), ama zincirle aynı kalınlık dilinde.

## 6. Bomba kıvılcımı
Fitil ateşi telefonda neredeyse görünmüyor. Kıvılcımı **2–2,5 kat büyüt**, sıcak ışık halesi ekle (sarı→turuncu). Acil hâlde (3-2-1) daha da büyük ve parlak. 8 kare döngü.

## Teslim
- Önizleme: `docs/art/task30/onizleme.png`. Mutlaka şunlar olsun:
  - Royal Match ekranı ile bizim 390×844 ekranımız **yan yana** (aynı ölçek; şerit, panel, portre ve tuş hizaları çizgiyle gösterilsin),
  - 3 şerit rengi adayı (aynı ekran 3 kez),
  - Jigo'nun 5 hâli büst olarak 96 pt'de,
  - 3 kilit adayı, eski ve yeni zincir yan yana, büyük kıvılcım.
- Dosyalar: `Assets/Art/_yeni/HUD30/`, `Assets/Art/_yeni/Jigo/` (eskinin üstüne yazma, `Jigo_v2/`), `Assets/Art/_yeni/Obstacles_v2/`.
- `docs/art/task30/uretim.md`, 3 satır özet, **dur**. Zeyd seçmeden entegrasyon yok.
