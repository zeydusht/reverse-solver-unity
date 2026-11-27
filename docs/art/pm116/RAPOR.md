# PM_116 RAPOR: dalışın sonundaki ani solma (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok. PM_115'in üstünde.

- **Ölçüm ortamı:** gzip build'i (Brotli bellekten alınamadı, bkz. PM_115), başsız Chrome, gerçek GPU, 390×844 (2x).
- **Değişmeyenler:** U1, 1,60 sn, kenar çizimi, PM_115 düzeltmeleri, sesler, çerçeve. Bayraksız oyun aynı.

## §1 Teşhis

**Ne soluyor:** Parça shader'ının oyun görünümü ("look"). Dalışta görünüm 0'ken parça yalın resim; görünüm 1'e çıkınca şunlar ekleniyor:

| Katman | Etki |
|---|---|
| Kontrast | Resim **%12 griye** yaklaştırılıyor |
| Işık geçişi | Üstte %12 açma, altta %10 koyultma |
| Parlama | Sol üstte **%25'e varan** beyaz |
| Kit katmanı | Koyu kontur, görünümün 0,6'sıyla |
| Kabartma | ±0,16 |
| Kenarlar | Krem şerit, lacivert hat ve gölge |

**Ne zaman:** PM_86'dan beri görünüm, çizgiler bittikten sonra geliyor. PM_111 çizgileri geri çekilmenin %80'inde bitirdiği için görünüm, dalışın son ~0,17 sn'sine (10 kare) sıkıştı.

**Ölçüm (tahtanın ortası):**
- Görünümün kendi payı: geri çekilmede doygunluk 10,9 puan, parlaklık 2,7 puan düşüyor (şimdiki − canlı farkı).
- Şimdikinde bu düşüşün tamamı son 10 karede: karede 1,7–1,9 doygunluk puanı.
- Grafikteki 62'den 51'e iniş ise görünümden değil. Kamera geri çekildikçe tahtanın daha az doygun kısımları kadraja giriyor; canlıda da aynısı var.

**Kim ekledi, neden:**
- G1, 13 Ekim 2026: resmi parçaya çizen ilk iş. Commit notu: "gradient and highlight are softened and contrast eased 12% so the cut reads".
- Aynı gün PRODUCT kararı: "Okunabilirlik oyunun özüdür… çıkıntı ve girintiler her görselde seçilebilmeli, gerekirse dolgu kontrastı %10–15 düşürülür".
- Önizleme 15, 26 Ekim 2026: kenarlar (lacivert hat, krem şerit, kabartma, gölge).
- PM_86, 18 Kasım 2026: görünümü çizgilerden sonraya koydu ("their own look, shadow and badges after the cuts").

**İki cümleyle:** Solma bilinçli eklenmiş: resim, çıkıntı ve girintiler her resimde okunsun diye %12 griye çekilmiş, üstüne yumuşak ışık geçişi ve parlama konmuş (G1, PRODUCT 13 Ekim). Ani olması ise tasarım değil: PM_86 görünümü çizgilerden sonraya koymuş, PM_111 çizgileri uzatınca görünüm son 0,17 sn'ye sıkışmış. Bunun için bir tasarım gerekçesi bulunamadı.

## §2 İki seçenek

### `&renk=canli`
- **Ne değişiyor:** Oyunda gri çekme, ışık geçişi ve parlama yok; parçalar resmin kendi renginde.
- **Kalanlar:** Kabartma, kenarlar, gölge, kit konturu ve rozetler aynen duruyor.
- **Dalışta:** Bu kalan görünüm geri çekilmenin tamamına yayılıyor. Karede en çok %3 (şimdikinde %15).
- **Son renk:** Doygunluk 51,1, parlaklık 69,5 (şimdiki 40,2 / 66,8).

### `&renk=yavas`
- **Oyun:** Görünüm bugünkü gibi.
- **Dalışta:**
  - Görünüm dipten başlıyor ve geri çekilmenin tamamına yumuşak bir eğriyle yayılıyor; oyunun ilk karesinde tamamlanıyor.
  - Kabartma ve gölge de aynı sürede geliyor.
  - Solma karede en çok 0,4–0,45 doygunluk puanı; şimdikinde 1,7–1,9.

### Ölçütler

| Ölçüt | Şimdiki | Canlı | Yavaş |
|---|---|---|---|
| Solmanın kendisi, karede 2× komşu ortalamasını geçen sıçrama | son 10 karede yığılıyor (karede 1,7–1,9) | solma yok | **yok** (2 koşunun birinde 0,15'lik bir tane; ölçüm tabanında) |
| Tüm görüntü (kamera dahil), 2× komşu sıçraması | 1,32 / 0,81 / 1,12 sn'de küçük tepeler | 1,22–1,36 sn'de küçük tepeler | 1,10–1,49 sn'de küçük tepeler |
| Oyunun ilk karesine geçişte piksel farkı | yumuşakça 0'a | yumuşakça 0'a | yumuşakça 0'a |

- **Tüm görüntüdeki tepeler:** Bunlar 0,2–0,5 puanlık, üç hâlde de ve bayrağa bakmadan çıkan tepeler. Aynı anlarda üst ve alt çubuklar kayarak giriyor ve Jigo'nun yazısı yazılıyor; koşudan koşuya yer değiştiriyorlar.
- **Solmanın kendi payı:** Yavaş − canlı farkı, kamera ve içerik çıkınca kalan solmadır. Yavaşta bu farkta sıçrama yok.

## Okunabilirlik (şimdiki | canlı)
`oyun_ici_karsilastirma.png`: bölüm 1, 7 (zincir), 14 (kilit, bomba), 19, 39 (duvar), 3 (kilit dersi), 21 (duvar dersi). 390×844.
- **Not:** PM "7 (bomba)" yazmıştı, ama 7. bölüm zincir dersi. Bomba 14, 19 ve 39'da var; bomba dersi 11. bölümde ve resimsiz, renk ayarı onu etkilemiyor.

| Öğe | Canlıda |
|---|---|
| Kilit sayısı | Altın kilit, kırmızı rozet: iki hâlde de net |
| Bomba sayısı | Net; koyu bomba koyu gökte (39) beyaz-altın rakam halkası sayesinde okunuyor |
| Zincir (turuncu) | Net |
| Zincir (camgöbeği) | **Zayıflıyor:** 39 ve 19'da koyu camgöbeği gökyüzünün üstünde halkalar zemine biraz karışıyor. Şimdikinde de zayıf, canlıda biraz daha |
| Jiggles | İki hâlde de net |
| Parça kenarları | Krem şerit ve lacivert hat iki hâlde de net, açık resimlerde de (7, 14) |
| Tutulan parça | Kalkma ve gölge iki hâlde de görünüyor |

- **Düzeltme:** Yapmadım. Tek zayıf nokta camgöbeği zincir, ve o şimdikinde de zayıf.
  - Gerekirse yalnız camgöbeği zincire ince açık bir hat eklenebilir, renkleri yeniden soldurmadan.
  - Bu yeni bir build ister. PM'in seçimine bırakıyorum.

## Çıktılar
- **`renk_yan_yana.mp4`:** şimdiki | canlı | yavaş. 390×844, 60 fps, sesli. Üç kez oynuyor, her seferinde bir panelin sesi.
- **`renk_yan_yana_yarim.mp4`:** yarım hız, dip → oyunun başı (0,70–1,85 sn).
- **`oyun_ici_karsilastirma.png`:** yukarıdaki bölümler, şimdiki | canlı.
- **`renk.png`:** geri çekilmede kare kare ortalama doygunluk ve parlaklık, üç hâl üst üste. Şimdikinde 1,47–1,59 sn'deki dik düşüş görülüyor.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı. Renkler telefon ekranında farklı görünebilir.
