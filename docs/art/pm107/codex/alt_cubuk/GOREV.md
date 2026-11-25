# PM_107 §D3: menünün alt sekme çubuğu (Codex görevi)

Sen bir oyun arayüzü sanatçısısın. Reverse Solver'ın (iPhone, dikey 390×844 pt) menüsündeki alt sekme çubuğunu yeniden çiziyorsun. Yalnız görsel üret; kod yazma, oyun dosyalarına dokunma. Bütün çıktılar bu klasöre (`docs/art/pm107/codex/alt_cubuk/`).

## Bugünkü durum
- Ekran görüntüsü: `docs/art/pm107/once_sonra/menu_once.png` (780×1688 px, 2×). Çubuk en altta: 4 sekme (Ev, Koleksiyon, Jiggles, Başarılar). Seçili sekme koyu mavi bir kutu, büyük ikon + etiket; ötekiler yalnız ikon; aralarında ince çizgi.
- Bugünkü ikonlar (referans, aynı konular kalsın): `Assets/_Game/Presentation/Resources/Art/t21-tab-icon-home-selected-large.png`, `...-collection-...`, `...-jiggles-...`, `...-trophy-...`.

## Kalite referansı (kopyalama yok)
`docs/art/pm107/ref/bo_a_3.jpg` ve `bo_a_7.jpg`: Block Out'un alt çubuğu. Oradan yalnız kalite ve katman dilini al:
- seçili sekme, çubuktan yukarı taşan açık renkli, yükselen bir panel; büyük ikon + etiket;
- ötekiler yalnız ikon; aralarında ince ayrım çizgileri;
- derinlik ve gölge; ikonlar zengin, oyuncak gibi ama sade.

Renk, karakter ve yazı **bizim**: "old money" lacivert (#0E2244, #153E75, #234670), krem (#FFF6D8), altın (#E3B54E, kenar #8A6420), ikincil turuncu (#F0A742). Mor yok, Block Out'un renkleri yok. "Daha iyi" = daha temiz ve okunur, daha karmaşık değil.

## İstenen (PM_74 döngüsü)
1. **En az 3 fikir** (A, B, C), birbirinden gerçekten farklı. Örneğin:
   - A: krem seçili panel, altın kenar;
   - B: açık lacivert, altın üst çizgi;
   - C: altın çerçeveli lacivert, iç ışık.

   Her fikir için:
   - `X_onizleme.png`: 780×1688, bugünkü ekran görüntüsünün üstüne yeni çubuk yerleştirilmiş tam ekran önizleme. Çubuğun üstündeki her şey aynı kalsın.
   - `X_strip.png`: çubuğun zemini, 1170×264 px, saydam arka plan; 3× ölçek (390×88 pt).
   - `X_selected.png`: seçili sekmenin paneli, 330×330 px, saydam; panelin üstü çubuktan ~18 pt yukarı taşacak.
   - `X_divider.png`: ayrım çizgisi, 12×180 px, saydam.
   - `X_icon_home.png`, `X_icon_collection.png`, `X_icon_jiggles.png`, `X_icon_trophy.png`: 384×384, saydam. Konular bugünküyle aynı: ev/müze binası, iki tablo, üç Jiggle (bizim yapboz parçası karakterlerimiz, renkleri mavi/pembe/mor), kupa. Kalın, okunur siluet; 48 pt'de de seçilebilsin.
2. **Puanla:** her fikre 1–5 puan ver ve `PUANLAR.md`'ye tablo olarak yaz. Ölçütler:
   - okunurluk (48 pt'de ikon);
   - bizim palete uyum;
   - derinlik ve kalite (Block Out düzeyi);
   - sadelik;
   - seçili sekmenin hemen anlaşılması.

   **Geçme çizgisi:** her ölçütte en az 4. Geçmeyeni bağla, nedenini yaz.
3. **En iyisini iyileştir:** en yüksek puanlıyı en az bir tur daha düzelt. Bulduğun kusurları gider: kenar kiri, saydamlık hatası, renk sapması, ikonların birbirine uymaması. Sonucu `final_*` adlarıyla kaydet (aynı dosya seti + `final_onizleme.png`).
4. **Teknik kontroller:** `KONTROL.md`'ye yaz.
   - **Arka plan:** Saydam üretim yapılamıyorsa düz #FF00FF zeminde üret, Pillow ile ayıkla, kenarda magenta piksel kalmadığını say ve yaz.
   - **Boyutlar:** her PNG istenen boyutta mı?
   - **Seçili panel:** `final_selected` panelinin üst kenarı, saydam olmayan ilk satır, kaçıncı pikselde? Yaz.
5. Kısa bir `SECENEKLER.md` yaz: her fikir 2 satır, hangisi neden seçildi.

İstemlerini `uretim_promptlari.json`'a kaydet. 25 dakikada bitir.
