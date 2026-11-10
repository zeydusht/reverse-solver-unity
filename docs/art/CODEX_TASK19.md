# Codex Task19 — Menü ekranları: tam ekran tasarım (mockup) seti

Zeyd: "Menüyü geliştir, her ekranı benim stilimle yap." Bu turda **tam ekran tasarım** yapıyoruz. Parça parça export'u Zeyd ekranları onayladıktan sonra Task20'de alacağız. Önce görünüşte anlaşalım.

## Kurallar (değişmez)

- Yalnız `Assets/Art/Menu/Task19/` ve `docs/art/` altına yaz. C#, `Assets/_Game`, Unity sahnelerine dokunma. Oyun kodu şu an başka bir işte (veri sistemi) değişiyor.
- Stil kaynakları:
  - `docs/art/STYLE_CARD.md`
  - `docs/art/PALETTE.md` (ZORUNLU; tablo dışı altın/sarı yok)
  - Onaylı Level tuşu `menu-play-button-v2`
  - Task13 menü seti (profil kutusu, sayaç hapı, ayarlar, 4 sekme ikonu, alt şerit, seçili sekme)
  - Task14 kartları
  - Task15 yeşil/turkuaz tuşlar ve 5 Jiggle avatarı
  - Onaylı koridor (petrol duvar, altın oymalı çerçeveler, kırmızı halı)
- **Önce mevcut onaylı parçaları kullan**, yeniden çizme. Eksik olanı aynı dilde çiz.
- Yeşil yalnız ana eylem tuşunda. İkincil tuşlar turkuaz. Kart zemini krem `#F8EBD0`. Yazı ve kontur lacivert `#0B2A36`.
- Yazı fontu: Türkçe metinlerde **Fredoka** (ğ, ş ve İ harfleri var). Lilita One yalnız İngilizce etiketlerde.
- Ekonomi yok: **para, kalp ve mağaza gösterme.** Sayaçlar yalnız ⭐ yıldız ve Jiggle (kurtarılan).
- Ölçü: telefon 375×812 pt → **1125×2436 px @3x**.
  - Üstte 132 px, altta 102 px güvenli alan boş kalır (çentik ve ev çubuğu).
  - Alt sekme şeridi bütün sekme ekranlarında aynı yerde durur.
- Metinler tasarımda görünsün ama ayrı katman olsun. Oyunda dinamik yazılacaklar: isim, sayılar, bölüm no.
- Gerçekçi içerik kullan: oyundaki gerçek tablo görsellerinden küçük örnekler ve gerçek bölüm sayıları (40 bölüm). Uydurma Lorem metin olmasın.

## Ekranlar (her biri ayrı PNG)

### 1. Ana ekran (Ev sekmesi) — `01-ev.png`
Şu anki menünün toparlanmış hali, oyuncunun her açılışta gördüğü yer.
- Arka plan: onaylı koridor. Ana duvar bloğunda **sıradaki bölümün tablosu** büyük durur, sağ blokta önceki tablo küçük görünür (STYLE_CARD "güncel menü seçimi").
- **Sol üst:** profil kutusu (avatar + oyuncu adı + küçük "Bölüm 12/40").
- **Sağ üst:** ayarlar dişlisi.
- **Üst orta:** iki sayaç hapı: ⭐ toplam yıldız, Jiggle ikonu + kurtarılan Jiggle sayısı.
- **Orta-alt:** büyük yeşil **"Bölüm 12"** tuşu (onaylı Level tuşu). Üstünde ince ilerleme çubuğu ("12/40 tablo").
- **Sol:** küçük yuvarlak **"Bölümler"** tuşu (`levels-button`), bölüm listesini açar.
- **Alt:** sekme şeridi, Ev seçili.

### 2. Koleksiyon (Tablolar sekmesi) — `02-koleksiyon.png` + `02b-tablo-detay.png`
Oyunun ödülü tablolar. Burası oyuncunun "müzesi".
- Başlık: **"Koleksiyon"** + "12 / 40".
- Petrol duvar zemininde **3 sütun ızgara**, her hücrede küçük altın çerçeve. Aşağı kaydırılır, 40 hücre.
  - Kazanılmış: tablo görünür, altında küçük ⭐⭐⭐.
  - Sıradaki: hafif parlayan çerçeve + "Sıradaki" etiketi.
  - Kilitli: koyu lacivert siluet, ortada kilit, altında bölüm numarası.
- Zor bölümlerin (19, 23, 25, 30, 35, 36, 38) çerçevesinde küçük bir **kırmızı köşe işareti** olsun. Oyuncu ilerideki zorlukları görsün.
- **Tablo detayı (`02b`):** Tabloya dokununca krem kart açılır: büyük tablo, tablonun adı (örn. "Kıyı Kasabası"), "Bölüm 7'de kazanıldı · ⭐⭐⭐ · 0:42", turkuaz "Tekrar oyna" tuşu, kapat X'i.

### 3. Jiggles sekmesi — `03-jiggles.png`
Hikâye: her bölümde yapboz parçaları içinde sıkışmış Jiggle'ları kurtarıyoruz. Bu ekran "kurtardığın aile".
- Üstte büyük sayaç: Jiggle ikonu + **"1.248 Jiggle kurtarıldı"**.
- Ortada sahne: koridorun bir köşesinde, basamaklı altın/ahşap bir **sergi rafı** üzerinde 5 renk Jiggle (mavi, pembe, kırmızı, yeşil, mor). Onaylı Jiggles referansı ve Task15 avatarları; ayrı uzuv kuralı geçerli.
- Her Jiggle'ın altında küçük sayaç: o renkten kurtarılan sayı.
- Altta **"Avatarım"** satırı: 5 yuvarlak avatar, seçili olanın etrafında altın halka. Profil avatarı buradan seçilir.
- **Kilometre taşı çubuğu:** "500 Jiggle → yeni çerçeve" gibi 3 durak, altın rozet ikonları. Görsel ödül, ekonomi değil.

### 4. Başarılar (Kupa sekmesi) — `04-basarilar.png`
- Başlık: **"Başarılar"** + "5 / 16".
- Dikey liste, her satır krem kart: solda altın madalyon ikon, ortada başlık ve kısa açıklama, altında ilerleme çubuğu ("7/10"), sağda durum.
- Durumlar: **tamamlandı** (dolu madalyon + tik), **devam ediyor** (çubuk), **kilitli** (gri-lacivert madalyon).
- Örnek başarılar (bunları kullan):

| Başarı | Açıklama |
|---|---|
| İlk Tablo | İlk bölümü bitir |
| Koleksiyoncu | 10 tablo topla |
| Müze Sahibi | 40 tabloyu topla |
| Bomba İmha Uzmanı | 10 bombalı bölümü ilk denemede geç |
| Soğukkanlı | Bir bölümü 3 saniyeden az süre kala bitir |
| Hatasız | Bir bölümü hiç sıkışmadan bitir |
| Yıldız Avcısı | 30 bölümde 3 yıldız al |
| Zincir Kıran | 25 zincirli parçayı çıkar |
| Baron'a Kafa Tut | Tüm zor bölümleri geç |

  Madalyon ikonları her başarı için ayrı ve okunur olsun: tablo, bomba, kum saati, yıldız, zincir, Baron şapkası vb.

### 5. Profil kartı — `05-profil.png`
Profil kutusuna dokununca açılan kart (koridor arka planı kararmış).
- Büyük avatar (altın çerçeve) + oyuncu adı + küçük kalem ikonu (adı değiştir).
- İstatistik ızgarası (2×3 kutu, ikon + sayı + etiket):
  - Bölüm 12/40
  - ⭐ 31
  - Kurtarılan Jiggle 1.248
  - Toplam süre 18 dk
  - İlk denemede geçilen %75
  - Başarı 5/16
- Turkuaz "Avatarı değiştir" tuşu, kapat X'i.

### 6. Ayarlar kartı — `06-ayarlar.png`
Oyunda var (ses, müzik, titreşim, dil). Şimdi görselini yapıyoruz.
- Krem kart, başlık **"Ayarlar"**.
- Üç satır, her biri ikon + etiket + **aç/kapa anahtarı**: Ses, Müzik, Titreşim. Açık hal yeşil, kapalı hal lacivert-gri; ikisini de çiz.
- **Dil:** iki seçenekli düğme "Türkçe | English", seçili olan dolu.
- Alt kısımda küçük turkuaz bağlantı tuşları: "Gizlilik", "Bize yaz". En altta küçük gri sürüm yazısı "v0.5.1".

### 7. Bölümler listesi — `07-bolumler.png`
"Bölümler" tuşuyla açılan, aşağı kaydırılan dikey yol.
- Koridor temasında aşağıdan yukarı ilerleyen bir yol: altın düğümler, aralarında kırmızı halı şeridi.
- Düğüm türleri:
  - **Bitmiş:** yeşil-altın, numara, altında ⭐ sayısı.
  - **Şu anki:** büyük, parlayan, üstünde küçük avatar.
  - **Kilitli:** lacivert, kilit.
  - **Zor bölüm:** kırmızı kenarlı, küçük Baron yüzü ikonu.
- Her 10 bölümde bir yolun yanında küçük bir tablo çerçevesi (koleksiyon kilometre taşı).
- Üstte geri oku ve başlık "Bölümler".

### 8. Alt sekme şeridi — `08-sekme-durumlari.png`
Task13 şeridi + dört ikon, 4 sekmenin her biri seçili haliyle (4 yatay şerit üst üste).
- Seçili sekme: yükseltilmiş altın kabartma + ikon büyür + altında etiket.
- Etiketler: Ev / Koleksiyon / Jiggles / Başarılar.
- Bildirim noktası örneği: Başarılar ikonunda kırmızı rozet (yeni başarı).

## Teslim

1. 9 PNG, her biri 1125×2436: `01`–`07` + `02b` + `08`.
2. `Assets/Art/Menu/Task19/inceleme.html`: hepsi yan yana telefon çerçevesinde, altında birer satır açıklama. Zeyd buradan bakacak.
3. `docs/art/TASK19_PARCALAR.md`: her ekranda **yeni çizilmesi gereken parçaların listesi** (Task20 export'u için). Ad, ölçü @3x, 9-slice gerekir mi, hangi ekranlarda tekrar kullanılıyor. Mevcut onaylı parçayla karşılananları "mevcut: dosya adı" diye işaretle.
4. Bitince 3 satır yaz: hangi dosyalar hazır, hangi kararlar Zeyd'den bekleniyor (en fazla 3 soru).
