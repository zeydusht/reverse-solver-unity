# Codex PM_86: Alt menü (sekme çubuğu), 3 seçenek

## Sorun
Ana menünün altındaki 4 sekmeli çubuk (Ev, Koleksiyon, Jiggles, Başarılar) Zeyd'e artık güzel gelmiyor:
- sekmeler arasında ayrım çizgisi yok;
- seçili sekme yeterince belli değil;
- seçili sekme yalnız bir yöne açılıyor.

Şimdiki hâli: `docs/art/pm86/alt_menu/simdiki.png` (dört ekran yan yana; çubuk en altta).

## İstenen
Stil:
- Referans: Royal Match'in alt çubuğu (yalnız fikir; onun çizimini, renklerini, ikonlarını kopyalama).
- Bizim stil: "old money" koyu lacivert/mavi + krem, altın vurgu, kalın hacimli (3B) görünüm, yumuşak gölgeler.
- Oyunun mevcut parçalarıyla uyumlu olsun: `Assets/_Game/Presentation/Resources/Art/T48/menu/` (bottom-strip, tab-selected, tab-divider) ve `Assets/_Game/Presentation/Resources/Art/yeni/pm75/menu/A_coin_pill.png`.

Parçalar (her seçenek için):
1. **Çubuk zemini** (`strip.png`, 1170×240 şeffaf PNG):
   - Ekranın tam altına yaslanan koyu mavi şerit; üst kenarında ince altın ya da krem kenar çizgisi.
   - Yatayda dokusuz ve desensiz; oyun bunu genişliğe göre esnetecek (9-dilim: sol/sağ kenarların 60 px'i sabit kalır).
2. **Seçili sekme yüzü** (`selected.png`, 420×270 şeffaf PNG):
   - Çubuktan biraz yüksek, üst köşeleri yuvarlatılmış, daha açık ve sıcak mavi.
   - İçten sıcak ışık / parlama, kenarında altın çizgi.
   - 9-dilim: köşelerin 70 px'i sabit kalır, orta kısım esnetilecek; ortada belirgin bir desen olmasın.
3. **Ayrım çizgisi** (`divider.png`, 24×200 şeffaf PNG): krem, ince ama belirgin dikey çizgi; uçları yumuşakça kaybolur, hafif gölge.
4. **Seçili sekmenin arkasındaki ışık** (`glow.png`, 400×300 şeffaf PNG): sıcak altın-krem yumuşak ışık lekesi, ikonun arkasında durur.

Seçenekler:
- **A:** düz koyu lacivert şerit, seçili yüz parlak kraliyet mavisi, kalın altın kenar.
- **B:** şerit lacivert, üstünde ince krem bant; seçili yüz krem-altın çerçeveli, içi parlak mavi.
- **C:** şerit lacivertten koyu teal'e hafif geçiş; seçili yüz yuvarlak kemer biçiminde, içi sıcak ışıklı.

Her seçenek X ∈ {A, B, C} için `docs/art/pm86/alt_menu/X/` altına yukarıdaki 4 PNG ve `onizleme.png` yaz:
- `onizleme.png` 390×844 maket: ekranın altında çubuk; 4 sekme eşit dört parçada, aralarında krem çizgiler.
- Seçili sekme (Jiggles, soldan 3.) iki yana eşit genişlemiş, yüzü ve ışığı arkasında.
- İkonu yukarı kalkmış ve büyümüş, altında "Jiggles" etiketi.
- Diğer üç sekme hafif sönük, etiketsiz.
- İkonlar: `Assets/_Game/Presentation/Resources/Art/t21-tab-icon-home-normal.png`, `t21-tab-icon-collection-normal.png`, `t21-tab-icon-jiggles-selected-large.png`, `t21-tab-icon-trophy-normal.png`.
- Üstte koyu bir menü sahnesi yeterli.

Ayrıca:
- `docs/art/pm86/alt_menu/karsilastirma.png`: üç maketin alt 220 px'i yan yana.
- `docs/art/pm86/alt_menu/LOG.md`: 3 satır.

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok. Yazı ya da logo çizme (etiketler oyunda yazılıyor; maketteki etiket yalnız gösterim için).
