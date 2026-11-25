# PM_MESAJ_113: Dalışta U biçimli dönüş (önizleme)

Önizleme. **Push yok, yayın yok.** `pm108-onizleme` dalında devam et.

## Zeyd'in geri bildirimi (PM_112)
- Bungee **kısa** geldi. Kısalmasını istemedi; süre vol 2 ile **tamamen aynı** olsun.
- Çizimi: `docs/art/pm113/ref/zeyd_cizim_u.jpg`. Y ekseni tabloya uzaklık (aşağısı = daha yakın), X ekseni zaman.
  - **Üstteki (istemediği):** gelir, düz bir taban boyunca bekler, uzaklaşır. Yani vol 2.
  - **Alttaki (istediği):** geniş, yuvarlak bir **U**. Düz taban yok, sivri dip de yok.
- Bungee'deki dönüş (0,09–0,13 sn) U değil, **V** oldu. Hız sıfırdan çok dik geçiyor. Gidiş de 0,63 sn'de bitiyor. Bu yüzden kısa hissettiriyor.

## İstenen
Temel: B + vol 2. Değişen tek şey tablonun çevresindeki bölüm.

1. **Süre birebir vol 2:**
   - Toplam **1,60 sn** (±0,01).
   - İlk ve son kare vol 2 ile aynı.
   - En yakın an (U'nun dibi) vol 2'nin bekleme ortasına denk gelsin: Oyna'dan **~0,745 sn** sonra (±0,02).
2. **U biçimi:**
   - Dibin çevresinde uzaklık **parabol** gibi olsun: hız sıfırdan **sabit eğimle**, düz bir çizgi gibi geçsin.
   - Hız sıfıra yaklaşırken yatıklaşmasın (vol 2'deki duruş hissi). Dik de geçmesin (bungee'deki V).
   - Dibin iki yanı yaklaşık simetrik olsun.
   - Sıfırdan tek geçiş olsun. Ani sıçrama, köşe ya da donmuş kare olmasın.
3. **Derinlik:** Dip, tablonun kadrajından **%4 içeride** olsun (bungee 2 ile aynı). Kamera tablonun kadrajına ilk vardığında çizgiler başlasın.
4. **Genişlik varyantları (`&u=1 / 2 / 3`):** Hız +0,3'ten −0,3 ekran boyu/sn'ye geçerken geçen süre:
   - **U1: 0,20 sn**
   - **U2: 0,30 sn**
   - **U3: 0,40 sn**
   - Karşılaştırma: bungee 2'de bu süre ~0,10 sn idi.
   - Genişlik dibin çevresinden alınır; gidişin başı ve geri çekilmenin sonu mümkün olduğunca vol 2'ye benzesin. Toplam her durumda 1,60 sn.
5. **Çizgiler ve ses:** PM_112 kuralları aynen geçerli.
   - Çizgiler tablonun kadrajına ilk varışta başlasın, geri çekilmenin %80'inde bitsin. Tahtaya devir kuralı aynı.
   - Dönüşte sessiz boşluk ve üst üste binme olmasın.
6. Başka hiçbir şeye dokunma (çerçeve, tahta, menü, sesler).

## Çıktılar: `docs/art/pm113/`
- **`u_yan_yana.mp4`:** vol 2 | U1 | U2 | U3. PM_112 ile aynı biçimde: panel başına 390×844, 60 fps, sesli, "SES" işareti.
- **`u_yan_yana_yarim.mp4`:** Aynısı yarım hızda, yalnız varış ve dönüş bölgesi.
- **`u1.mp4`, `u2.mp4`, `u3.mp4`:** Tek tek.
- **`yakinlik.png`:** Zeyd'in çizimiyle aynı düzende bir grafik:
  - X ekseni Oyna'dan sonraki zaman (0–1,60 sn).
  - Y ekseni tabloya uzaklık; aşağısı daha yakın, tablonun kadrajı yatay kesik çizgi.
  - Eğriler: vol 2, U1, U2, U3. Hepsi aynı anda bitmeli.
  - Altında hız grafiği (`hiz.png` gibi). Dip bölgesinde hızın sıfırdan **düz bir çizgi gibi** geçtiği görünmeli.
- **`RAPOR.md`:** Kısa, şu tabloyla:

| | Vol 2 | U1 | U2 | U3 |
|---|---|---|---|---|
| Oyna → tablonun kadrajı (ilk varış) | | | | |
| Oyna → dip | | | | |
| Dip derinliği (%) | | | | |
| Hız +0,3 → −0,3 süresi | | | | |
| Toplam (ölçülen) | | | | |

Raporun sonuna 20 ms'yi aşan kareleri ekle (PM_112'deki gibi).

## Bitiş
- Ref çizimi `docs/art/pm113/ref/` altında dursun.
- Push ve yayın yok. Mesajının en altına kısa özet yaz.
