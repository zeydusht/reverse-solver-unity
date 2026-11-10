# Codex Task19D — Task19C onaylandı: 3 düzeltme + kalan 5 ekran

Zeyd Task19C'yi beğendi: altın oranı, yıldızsız düzen ve Ev yerleşimi tamam. Task19B/19C kuralları aynen geçerli: Royal Match stil referansı, kopyalama yasağı, altın ≤ %10, yıldız yok, ekonomi yok, güncel oyun durumu.

## Düzeltmeler (Task19C'deki 4 ekrana)

1. **Tablolar dikey olmalı.** Koleksiyon ızgarasında tablolar yatay kırpılmış. Oyunda tablo dikey görünüyor (menüdeki çerçeve ve tahta dikey).
   - Hücreleri **dikey 3:4** yap. Tablo görselini (`Assets/Art/Paintings/painting-XX.jpg`, 1024×1024 kare) ortadan dikey kırp, menüdeki çerçeveyle aynı oranda.
   - 3 sütun kalsın. Hücreler uzayacağı için ekranda 4 tam sıra + 5. sıranın başı görünsün; kaydırma hissi versin.
   - Ayrıntı ekranındaki (02b) büyük tablo da dikey olacak.
2. **Alt çubuğun kendisi eski kalmış.** Kabarcık ve ikonlar iyi, ama altlarındaki şerit hâlâ Task13'ün düz turkuaz bandı. Royal Match'teki alt çubuğa bak (`Assets/Art/References/`, alt kısımlar) ve şeridi yeniden çiz:
   - Kalın, hacimli bir çubuk: üst kenarı açık renk parlak bevel, alt kısmı koyu.
   - Her sekme ayrı **gömülü bir yuva**: hafif iç gölgeli, aralarında belirgin ayırıcı.
   - **Seçili sekmenin yuvası** komple açık turkuaz ve parlak olsun, sadece ikon kabarcığı değil. Yuva hafif yukarı kabarsın.
   - Renk koyu lacivert-turkuaz. Üstte tek ince altın şerit, başka altın yok.
   - 4 seçili hali `08-sekme-cubugu.png`'de yeniden göster. Aynı yeni çubuk bütün ekranların altında olacak.
3. **Başarılar metin taşması.** "Bomba İmha Uzmanı" ve "Hızlı Parmaklar" açıklamaları 3 satıra çıkıp ilerleme çubuğuna değiyor. Açıklama en fazla 2 satır olsun:
   - Bomba İmha Uzmanı: "10 bombalı bölümü ilk denemede geç" (tek satır, küçült)
   - Hızlı Parmaklar: "10 bölümü yarı sürede bitir"
   - Gerekirse satır yüksekliğini biraz artır. İlerleme çubuğu metne değmesin.
4. Küçük not: profil kutusunda uzun isim kesiliyor ("Zeynep Yılma"). Kutuyu biraz genişlet ya da isim 14 karakteri geçince yazı küçülsün; tasarımda uzun isimle göster.

## Kalan 5 ekran (aynı yeni dille)

Görevler Task19'daki tanımlara göre (`docs/art/CODEX_TASK19.md`). Yıldız ve ekonomi kuralları geçerli.

- `02b-tablo-detay.png`
  - Dikey büyük tablo, tablo adı, "Bölüm 7'de kazanıldı · 0:42".
  - Turkuaz "Tekrar oyna" tuşu, köşede kırmızı yuvarlak X.
- `03-jiggles.png`
  - Raf sahnesi, "1.248 Jiggle kurtarıldı", 5 renk sayaçları.
  - Avatar seçimi, 3 duraklı kilometre taşı.
- `05-profil.png`
  - Profil kartı, 6 istatistik: Bölüm, Kurtarılan Jiggle, Toplam süre, İlk denemede %, Başarı, Ortalama kalan süre. Yıldız yok.
- `06-ayarlar.png`
  - Ses, Müzik, Titreşim aç/kapa anahtarları.
  - Türkçe | English seçimi.
  - Gizlilik ve Bize yaz tuşları, sürüm yazısı.
- `07-bolumler.png`
  - Bölüm yolu. Düğümler turkuaz-altın (bitmiş), parlak altın (şu anki), gri-lacivert + kilit (kilitli), kırmızı kenar + Baron (zor, 19 örnek).
  - Kalın halı/taş yol, yanlarda koridor dekoru.
  - Her 10 bölümde küçük dikey tablo çerçevesi.

## Teslim
Klasör: `Assets/Art/Menu/Task19D/`

- 9 ekranın tam seti: 01, 02, 02b, 03, 04, 05, 06, 07, 08. Düzeltilenler dahil, hepsi aynı yeni alt çubukla.
- `inceleme.html` (9 ekran yan yana) + `kontak.png` (tek görselde 9 ekran).
- `docs/art/TASK19_PARCALAR.md` listesini yeni tasarıma göre güncelle (Task20 export'u buna göre yapılacak).

Bitince 2 satır yaz.
