# PM_MESAJ_86: Hız eğrileri, çizgi→tahta birebir, sandık, titreşim, müzik, alt menü, Başarılar, dalış hatası

Kodu kendin yaz. p23 üzerinde. **PM_85 bitmediyse önce onu bitir.** Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (18 Kasım 16:25): "Daha iyi olmuş, hoşuma gitti." Aşağıdakiler eklenecek.

## 1. Koridor kayışı: sabit hız değil, çan eğrisi hız
**Sorun:** 1,5 sn'lik kayış sabit hızda gidiyor gibi hissettiriyor.

**İstenen:** hız grafiği çan eğrisi (normal dağılım gibi):
- başta yavaş: bulunduğun tabloyu bir an daha gösterir;
- hızlanarak ortada en yüksek hıza çıkar;
- yavaşlayarak yeni tablonun önüne süzülür ve oturur.

Süre aynı (1,5 sn).
- **Eğri:** konum için smootherstep (6t⁵ − 15t⁴ + 10t³) ya da eşdeğeri. Hız başta ve sonda 0'a teğet, ortada tepe; ivmede sıçrama yok. "Slow in / slow out" animasyon ilkesi.
- **Ortada hafif his:**
  - tepe hızda çok hafif hareket bulanıklığı;
  - ön plan katmanlarında küçük parallax.

  Abartma.
- Hız grafiği önce/sonra `docs/art/pm86/koridor_hiz.png`, GIF normal + yarım hız.

**Bütün geçişler için aynı ilke (denetim):** projede **doğrusal (linear) tween kalmasın**. Panel açılış/kapanışları, menü, ödül uçuşları, HUD girişleri, kamera, sayaçlar: hepsini tara. Doğrusal olanları uygun yumuşak eğriye çevir. Bilerek sabit hızda olan (dönen ışık huzmesi gibi) kalabilir. Listeyi raporla.

## 2. Çizgiden tahtaya geçiş: %100 birebir
**Sorun:** Geri çekilirken çizgiler doğru çiziliyor ama geçiş anında anlık bir renk/ton değişimi oluyor, hafif uzaklaşıyor, sonra asıl tahta geliyor. Zeyd'in istediği: gözle hiçbir değişiklik fark edilmesin, milisaniye bile olmasın; "yapboz oluşmuş" hissi.

**Kural:** devir anında renk, çizgi kalınlığı, ton, gölge ve konum birebir aynı.

**Olası sebepleri kontrol et:**
- renk uzayı (gamma / linear);
- farklı malzeme ya da shader;
- doku filtreleme / mipmap;
- render ölçeği;
- çizgi katmanıyla parça kenarı arasında kalınlık farkı;
- gölge ya da ayrışmanın tek karede başlaması;
- devir karesinde ölçek sıçraması.

**En sağlam yol:** ayrı çizgi katmanı olmasın. Çizgiler baştan itibaren gerçek parçaların kendi kenar çizgisi olsun (aynı malzeme); yalnız görünürlük maskesi çizilerek açılsın. Parçaların 1–2 px ayrışması ve gölgesi devirden **sonra** yumuşakça (≥ 0,2 sn) gelsin.

**Ölçüm:**
- devirden önceki ve sonraki karede piksel farkı: kanal başına ≤ 2/255 (ayrışma başlamadan);
- ardışık karelerde ortalama parlaklık sıçraması yok;
- ölçek ve konum sürekli.

Fark görselleri `docs/art/pm86/devir_fark.png`. Kazanma → sonraki bölüm ve menü → bölüm, iki geçişte de.

## 3. Sandık: ekranın her yerine dokunma
- Sandık açılırken ekranın **herhangi bir yerine** dokunmak sayılsın (3 dokunuş: sallan, sallan, açıl). Sandığın üstüne dokunmak şart değil.
- Ödüller uçarken dokunmak animasyonu hızlandırsın, ödül kaybolmasın.
- Ekranın altında küçük "Dokun!" yazısı nabız atsın (ilk sandıkta).

## 4. Titreşim haritası (her yerde)
Oyunlarda dokunulan her şey bir his verir. Bütün etkileşimleri tara, eksikleri ekle. Kurallar:
- 50 ms aralık kuralı;
- Ayarlar'daki titreşim anahtarına uyulsun;
- iOS dışında sessizce yok sayılsın.

| An | Titreşim |
|---|---|
| Her düğmeye basış | Select |
| Sandık 1. ve 2. dokunuş | Light, sonra Medium (artan) |
| Sandık açılış | Heavy + 0,1 sn sonra Success |
| Ödül parası sayaca düştükçe | Light (en çok 8, aralıklı) |
| Booster seçimi | Select |
| Booster vuruşu (çekiç, makas kesişi) | Medium |
| Değnek dönüşümü | Light × 3 dalga |
| Saat donması | Medium |
| Kayıp / Süre doldu | Error |
| Devam satın alma / reklam ödülü | Success |
| Kont mührü kırılması (çok zor bölüm başı) | Heavy |
| Mevcut parça, jam, bomba, kazanma | aynen (PM_45 / 46) |

Tabloyu `docs/TITRESIM_HARITASI.md`'ye yaz (kodla aynı kalsın).

## 5. Müzik: yeni döngü
- **Sorun:** şimdiki müzik ritimsel ama sıkıcı, Zeyd artık beğenmiyor.
- **Referans:** Zeyd bir oyundan örnek ses ya da video gönderecek. PM `docs/art/pm86/muzik_ref/` klasörüne koyacak. Gelmediyse: hafif, sıcak, oyuncu, akustik ağırlıklı (pizzicato yaylı, marimba ya da kalimba, yumuşak ritim). Tempo 90–110 BPM, sinir bozmayan, uzun dinlenebilir. "Old money" saray havası hafif dokunuş olabilir.
- **Kaynak:** Codex müzik üretmiyor. Sentetik ses de reddedildi (PM_51).
  - Ticari kullanıma izin veren lisanslı kütüphanelerden **5 aday** bul (ör. Pixabay Music). Her birinin lisans linkini ve koşulunu `docs/art/pm86/MUZIK_LISANS.md`'ye yaz. Atıf gerektiren ya da "yalnız kişisel kullanım" olanları alma.
  - Adayları kusursuz döngüye getir: tıklama yok, 60–120 sn.
  - Ses seviyesini efektlerin altına ayarla.
  - Bayrakla seçilebilsin: `&muzik=a…e`.
  - Uygun aday çıkmazsa raporla. PM, Zeyd'le Suno gibi bir yapay zekâ müzik aracını konuşacak.
- **Ayarlar:** Müzik ve Ses ayrı anahtarlar olsun (yoksa ekle). Reklam ve video sırasında müzik kısılsın, uygulama arka plana gidince dursun.

## 6. Küçük eksikler denetimi (PM'in eklediği)
Bilinen oyunlarda olup bizde eksik olabilecek küçük şeyleri tara; düzeltilmesi kolay olanları yap, gerisini raporla:
- Uygulama arka plana gidince ya da bildirim/telefon gelince süre durur, dönünce Durdur paneli açık gelir.
- Her düğmede basma animasyonu (çökme) + tık sesi + titreşim.
- Altın ve booster sayaçları değişince kısa sayma animasyonu.
- Sessiz modda (iPhone yan anahtarı) müzik çalmasın, efektler kurala uysun.
- Çok hızlı çift dokunuşta iki kez tetiklenen düğme kalmasın (kazanma, sandık, reklam).
- İnternet yokken reklam tuşları görünmesin, hata mesajı çıkmasın.
- Bölüm başı uyarı şeridi (zor / çok zor) dokununca hemen kapansın.

## 7. Alt menü sekmeleri: arka plan resmi geç geliyor (HATA)
- **Sorun:** Ana menü dışındaki 3 sekmeden birine (ör. en sağdaki Başarılar) basınca:
  - üst kısım ve ortadaki içerik hemen geliyor;
  - arka plan resminin kalan kısmı bir an sonra beliriyor.
  
  Ana menüye dönüşte sorun yok.
- Sebebi bul: geç yüklenen doku / Addressables / async yükleme, ilk karede kapalı gelen Image, ya da farklı arka plan nesnesi. Raporla.
- **Çözüm:** 3 sekmenin arka planı ve içeriği açılışta önceden yüklensin ve bellekte kalsın. Sekmeye geçiş tek karede tam ekran olsun.
- **Ölçüm:** sekme geçişinde arka planı eksik kare sayısı 0. Üç sekme × 5 geçiş, PlayMode testi.

## 8. Alt menü (sekme çubuğu) yeniden tasarım
Zeyd artık beğenmiyor. Seçilen sekme yalnız bir yöne açılıyor, ayrım çizgileri yok, seçili sekme yeterince belli değil.
- Codex 3 seçenek çizer (referans: Royal Match alt çubuğu). Stil: old money mavi + krem, kalın 3B yazı, altın vurgu.
- **İstenenler:**
  - sekmeler arasında belirgin krem ayrım çizgileri;
  - seçilen sekme **ortası sabit kalarak iki yana eşit genişler**;
  - ikon yukarı kalkar ve büyür, altında etiketi belirir;
  - seçili sekmede sıcak ışık / parlama, diğerleri hafif sönük.
- **Animasyon:** 0,25 sn, hafif geri tepmeli yumuşak bitiş (ease-out-back, abartısız). Diğer sekmeler aynı anda daralır. Hiçbir şey zıplamaz. Basışta Select titreşimi + tık sesi.
- Puanla, en iyi 2'yi `docs/art/pm86/alt_menu/` klasörüne koy, en iyisini bağla. GIF normal + yarım hız.

## 9. Başarılar ekranı görselleri yeniden çizilsin
Şimdiki çubuklar, kutular ve rozetler kötü duruyor.
- Codex yeniden çizsin: daha **sade**, bizim stilde. Krem panel + mavi çerçeve, altın vurgular, kalın 3B başlık, okunur ilerleme çubuğu (krem yuva içinde altın dolgu), basit ve temiz rozetler.
- Renk paleti aynı kalabilir. Karmaşık sahne, dolap gibi ağır arka plan gerekmez; güzel ve düzenli dursun.
- 3 seçenek, puanla, en iyisini bağla. Önce/sonra ekran görüntüsü `docs/art/pm86/basarilar/`.
- Yalnız görsel. Yeni başarı özelliği ekleme (v1.1).

## 10. Menü → bölüm dalışı: daha kısa ve çan eğrisi hız
- **Şimdi:** yay + yaklaşma yaklaşık 3 sn, yavaş yavaş gidiyor gibi.
- **Yeni:** yay + yaklaşma **tek çan eğrisi** (smootherstep), toplam yaklaşık **2,2 sn** (bugünkünden ~%25 kısa, oranlar korunur). Başta yumuşak kalkış, ortada en hızlı, tabloya yaklaşırken yumuşak yavaşlama.
- Resimde kalış ve geri çekilme PM_85 kuralları (≤ 0,4 sn, çekilme çizgilerle üst üste).
- **Bayrak:** `&dalishiz=` (çarpan, varsayılan 1).
- Hız grafiği `docs/art/pm86/dalis_hiz.png`.

## 11. Dalışta tablonun resmi çerçeveden bağımsız dönüyor (HATA)
- **Sorun:** Menüden tabloya giderken duvarlar, çerçeve ve zemin yürüyen kameraya doğru uyuyor. Ama çerçevenin içindeki resim ayrı hareket ediyor: bükülüyor, sağa sola dönüyor, açılıp kapanıyor gibi; kamerayla senkron değil.
- **Muhtemel sebep:** resim ayrı bir katman ya da sprite. Kendi dönüşü, kendi perspektif hesabı ya da farklı parallax/derinlik çarpanı var; ya da çerçevenin köşeleriyle her karede eşlenmiyor. Bul ve raporla.
- **Çözüm:** resim çerçeveye **katı bağlı** olsun: aynı düzlem, aynı derinlik, aynı dönüşüm (alt nesne). Ya da her karede çerçevenin iç 4 köşesinden hesaplanan aynı izdüşümle çizilsin. Resim kendi başına hiçbir dönüş ya da ölçek yapmasın.
- **Ölçüm:** dalış boyunca her karede resmin 4 köşesi çerçevenin iç köşeleriyle ±1 px. GIF önce/sonra.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan: çan eğrisi kayış + birebir devir)
  - `p23/?c=p23&test=1&lang=tr&lv=10` (sandık: her yere dokun)
  - `p23/?c=p23&test=1&lang=tr&muzik=a` (… `e`'ye kadar)
  - `p23/?c=p23&lang=tr` (menü: alt çubuk, sekmeler, Başarılar, Oyna → dalış)
