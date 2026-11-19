# PM_93 RAPOR

Ölçümler yerel build'de alındı: Chrome, gerçek GPU, 390×844, 60 fps. iPhone'da denenmedi.

## 1. Takılma: lastik

`&lastik=` (varsayılan 1, 0 = PM_75'in eski hâli), `&lastiksure=` (temastan sonraki süre, sn), `&lastiklog=1` (her takılmada bir ölçüm satırı).

### Boşluklu takılma

1. **Gidiş:** parça temasa kayıyor ve hızıyla orantılı uzuyor. Gidiş ekseninde en çok %7 uzuyor, diğer eksende hacmi koruyarak %3,4 inceliyor.
2. **Çarpma:** gidiş ekseninde %20 basılıyor, diğer eksende %11,8 genişliyor. Pivot temas kenarı: o kenar engelde kalıyor, karşı taraf sıkışıyor.
3. **Geri sekme:** parça büzülmeden açılırken geri itiliyor. Dönüşte gidiş ekseninde yine hızıyla orantılı uzuyor.
   - Sönümlü bir yay benzetimiyle yerine oturuyor.
   - Yuvasını hücrenin ~%4'ü kadar geçiyor, 1–2 sönen salınımla duruyor.
4. **Süre:** temastan sonra 0,36 sn. Gidiş mesafeye bağlı (0,08–0,28 sn). Toplam 0,44–0,64 sn; ölçülen bir hücrelik boşlukta 0,44–0,53 sn.

### Boşluksuz takılma

- Parça hücrenin %8'i kadar itiliyor, %11 basılıyor, yaylanarak geri geliyor ve küçük bir salınımla oturuyor.
- Toplam 0,33 sn.

### Ortak kurallar

- Resim, tırnaklar, kenar çizgisi, kilit ve bomba rozetleri parçayla birlikte esniyor (hepsi tek transform).
- Ses, titreşim ve kırmızı parıltı eskisi gibi: boşluklu takılmada temasta, boşluksuzda bırakmada.
- Zincirli parça zincirin sınırında esniyor; temas mesafesi zincir kuralından geliyor (PM_63).
- Yeni bir dokunuş animasyonu o anda kesiyor.

### Kare süreleri (`&lastiklog=1`, takılma başına)

| Durum | Takılma | 20 ms'yi aşan kare |
|---|---|---|
| Boşluksuz, bölüm 4 | 6 | **0** (en uzun 18 ms) |
| Boşluklu, bölüm 4 | 6 | 1'er (30 ms) |
| Eski PM_75 çarpması, aynı boşluklu itme | 6 | 1'er (29–31 ms) |
| Bölüm 2 (ipucu açık), boşluklu ve boşluksuz | 6+6 | 1'er |

- **Boşluklu takılmadaki tek uzun kare eski çarpmada da aynen var.** Hep "3 ms + 30 ms" çifti olarak geliyor ve ekran kaydı, titreşim ya da günlük kapatılınca da kalıyor.
  - Lastik animasyonundan değil, temas anındaki ortak işten geliyor; büyük ihtimalle "tok" sesi.
  - Kapsam dışı olduğu için dokunulmadı.
- **Bölüm 2'deki uzun kare** 1–3. bölümlerin ipucu/rehber sisteminden geliyor. Bölüm 4'te aynı boşluksuz itme temiz.

### GIF'ler (yarım hız, `lastik/`)

- `bosluklu.gif` ve `bosluksuz.gif`: yeni hâl.
- `bosluklu_once_sonra.gif` ve `bosluksuz_once_sonra.gif`: PM_75 ile PM_93 yan yana.
- Bölüm 4: boşluklu takılmada sol üst parça bir boş hücreden geçip sağdaki parçaya çarpıyor.

## 2. Hata: Koleksiyon'dan tekrar oynayınca menü bozuluyor

**Sebep:**

- Menünün dört sekmesi yan yana kurulu sayfalar (PM_88).
- Koleksiyon'dan "Tekrar oyna" bölümü dalışsız açıyordu. Menüye dönüşte menü, son sekme olan Koleksiyon'da yeniden kuruluyordu.
- Ev sayfası arkada önceden kuruluyor; o sırada sayfa bir ekran genişliği solda duruyor.
- Ev'in odası (duvar + çerçeve, dalışın ilk görünümü) yerini dünya koordinatından alıyordu. Bu yüzden oda bir ekran sola kayıyordu:
  - sağda oyunun arka planı açıkta kalıyordu;
  - tablo yerindeydi ama çerçevesizdi;
  - altında koyu bir şerit görünüyordu.

**Düzeltme:**

- Oda artık yerini Ev sayfasının kendi koordinatından alıyor. Ev sayfası hangi sekmedeyken kurulursa kurulsun doğru yerde.
- Koleksiyon'dan "Tekrar oyna":
  - sayfalar anında Ev'e geçiyor;
  - çerçeveye o bölümün tablosu konuyor;
  - normal dalışla giriliyor;
  - dönüş Ev'e, sıradaki bölümün tablosuna.
  - Resim 2 sn içinde gelmezse bölüm dalışsız açılıyor.

**PlayMode testi (`MenuIsWholeAfterEveryWayBack`):**

- Akışlar:
  - Koleksiyon → bölüm → kazan → Menü (kazanma ekranının Menü'sü, koridorla);
  - Koleksiyon → bölüm → Durdur → Menü;
  - Koleksiyon → bölüm → kaybet → Menü;
  - normal bölüm → Menü;
  - hatanın kendi yolu: Koleksiyon'dayken menünün yeniden kurulması, sonra Ev.
- Her birinden sonra menü taze açılmış menüyle karşılaştırılıyor:
  - sekme, sayfa konumu, kamera, odanın ekrandaki yeri birebir aynı;
  - piksel farkı %0,1.
- Düzeltme geri alınınca test hatanın yolunda kalıyor (oda 559 pt kaymış).

Tarayıcıda uçtan uca da denendi: Koleksiyon → Bölüm 2 → Tekrar oyna → dalış → Durdur → Çık. Menü Ev'de, Bölüm 5 tablosu çerçevesiyle normal.
