# PM → Claude Code (25 Ekim 2026, 23:05): Önizleme 8'den gelen geri bildirim

Zeyd seviye 40'ın ekran görüntüsünü gönderdi. Ona soru sorma. Token tasarrufu zorunlu.

## Şimdi yap (kod)

1. **Bomba ortalaması ve boyutu.**
   - Bomba hücrenin altına kaymış ve küçük görünüyor. Sebebi sprite'ın üstündeki fitil: görselin merkezi gövdenin merkezi değil.
   - Pivotu **gövdenin merkezine** al (fitil hariç). Codex'in v2 görselinde gövde tuvalin tam ortasında olacak; o gelene kadar mevcut görseli yukarı ofsetle.
   - Gövde çapı hücrenin yaklaşık **%85**'i olsun.
2. **Sayaç okunurluğu.** Şu an küçük, ince ve kontrastı zayıf.
   - Font: kalın ve yuvarlak bir oyun fontu. Lisansı serbest (OFL) olsun: **Fredoka** (SemiBold/Bold) ya da **Baloo 2** (ExtraBold). WebGL'e göm.
   - **Bomba:** Rakam krem dairenin içine sığacak en büyük boyutta (dairenin yaklaşık %70'i). Renk koyu lacivert (#0B2A36). Kontur yok.
   - **Çivi:** Rakam çivi başının ortasında, koyu lacivert (#0B2A36). Altın üstünde okunması için 2 px açık krem dış çizgi olsun. Boyutu başlığın yaklaşık %55'i.
   - İki basamaklı sayılarda (26, 37, 40) rakamlar sıkışmasın; genişliğe göre ölçekle.
3. **Zincir uç kilidi:** Hücrenin yaklaşık **%45**'i kadar büyüt (şu an çok küçük). Zincir halkaları aynen kalsın, Zeyd beğendi.
4. **Duvar şeridi:** Kalınlaştır. Şerit kalınlığı hücrenin yaklaşık %18'i olsun, çapraz çizgiler eski haldeki gibi kalın ve seyrek olsun. Codex'in v2'si gelene kadar mevcut tile'ı ölçekle.
5. **Tahta çerçevesi rengi:** Tahtanın arkasındaki sarı çerçeve, menü koridorundaki tablo çerçevesiyle aynı **altın** tonunda olsun. Codex yeni çerçeve çizene kadar geçici olarak renk tonunu (tint) eşitle.
6. Önizleme 9'u `?set=redesign` adresine yayınla. BACKLOG'a 2 satır ekle.

## Codex'ten gelecek (Zeyd iletiyor)

Codex'in v2 dosyaları gelince bağla ve HANDOFF'a oyun içindeki adlarıyla ekle:

- `bomb-v2-384.png`
- `wall-repeat-v2`
- `board-frame-gold-v1`
- HUD seti
  - `hud-level-badge`
  - `hud-pill`
  - `hud-pause` (`normal` ve `pressed`)
