# Codex PM_86: Başarılar ekranı görselleri, 3 seçenek

## Sorun
Başarılar ekranındaki satır kutuları, ilerleme çubukları ve rozetler kötü duruyor. Şimdiki hâli: `docs/art/pm86/basarilar/once.png`.

## İstenen
Daha sade, bizim stilde:
- krem panel, mavi çerçeve, altın vurgular;
- hacimli ama temiz;
- okunur ilerleme çubuğu: krem yuva içinde altın dolgu;
- basit, temiz, tek bakışta okunan rozetler.

Renk paleti: koyu lacivert `#0E2244`, kraliyet mavisi, krem `#F8EBD0`, altın. Karmaşık sahne gerekmez. Referans parçalar:
- `Assets/_Game/Presentation/Resources/Art/T48/menu/`
- `Assets/_Game/Presentation/Resources/Art/yeni/T50/win/coin.png`

Satırda yazılar oyunda yazılıyor (başlık lacivert kalın, altında koşul, çubuğun ortasında "3/10"); görsellere **yazı çizme**.

## Parçalar (her seçenek)
1. `tile.png`, 1100×300 şeffaf PNG: satır kutusu.
   - Krem iç yüz, mavi kalın çerçeve, köşeler yuvarlak, hafif alt gölge.
   - 9-dilim: köşelerin 80 px'i sabit, orta kısım düz renk (esnetilecek).
2. `track.png`, 600×60: ilerleme yuvası. Koyu krem-kahve iç gölgeli oluk, kenarı ince lacivert. 9-dilim: uçlardaki 30 px sabit.
3. `fill.png`, 600×44: altın dolgu. Parlak altın, üstte ince parlama. 9-dilim: uçlardaki 22 px sabit.
4. `lock.png` ve `check.png`, 160×160: küçük kilit (gri-mavi) ve tamamlandı işareti (yeşil-altın rozet içinde tik).
5. Rozetler, 3 örnek (`badge_first-picture.png`, `badge_bomb.png`, `badge_baron-hat.png`), 256×256 şeffaf:
   - Yuvarlak madalyon: lacivert zemin, altın kenar, ortada tek sade ikon.
   - `first-picture`: küçük çerçeveli tablo. `bomb`: bomba. `baron-hat`: Kont'un silindir şapkası.
   - Hepsi aynı madalyon dilinde.
6. `onizleme.png`, 390×844 maket:
   - Üstte "Başarılar" başlığı ve "2/9" sayacı yer tutucu olarak.
   - Altında 5 satır: rozet solda, başlık + koşul ortada, çubuk altta; biri tamamlanmış (dolu çubuk, tik), biri yarım, biri kilitli.
   - Arka plan: düz koyu mavi ya da hafif desenli, sade.

## Seçenekler
- **A:** krem kutu + kalın kraliyet mavisi çerçeve, altın ince iç çizgi; madalyonlar lacivert + altın.
- **B:** krem kutu, ince altın çerçeve + dışta lacivert gölge; madalyonlar kremden altına parlayan.
- **C:** lacivert kutu + krem iç bant (koyu satır), altın çubuk; madalyonlar krem zeminli.

Her seçenek X ∈ {A, B, C} için `docs/art/pm86/basarilar/X/` altına. Ayrıca:
- `docs/art/pm86/basarilar/karsilastirma.png`: 3 maket yan yana.
- `docs/art/pm86/basarilar/LOG.md`: 3 satır.

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
