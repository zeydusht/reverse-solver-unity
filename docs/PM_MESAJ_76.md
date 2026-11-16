# PM_MESAJ_76: Geçişler pürüzsüz olsun (yay çizen kamera + bütün geçişlerde akıcılık)

Kodu kendin yaz. p23 üzerinde devam (p22 yedek). Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd (16 Kasım akşam): "Yapmak istediğim özellik büyük ölçüde olmuş: kameranın tabloya yaklaşması, yapbozun oluşması, çizgiler, HUD'un yukarıdan ve aşağıdan gelmesi çok iyi. **Ama geçişler çok robotik, akıcılık sıfır.**"

## 1. Tablonun karşısına geçiş: duvar dönmesin, kamera yay çizsin
**Şimdiki sorun:** Kamera tablonun karşısına geçerken resim düzlemsel olarak döndürülüyor ("duvar bize doğru dönüyor"). Kenarlarda boşluk kalıyor, his kötü.

**İstenen:** Tablo bir çemberin merkezi, kamera bu çemberin üstünde yay çizerek sağa doğru ilerleyip tablonun **tam karşısına** gelsin. Sonra tabloya yaklaşsın. Ardından gelen bölüm, yani çizgilerin çıkışı, yapboz oluşumu ve HUD'un gelişi, olduğu gibi kalsın.

**Yöntem: çizilmiş bakış açıları + katmanlı parallax (2,5B).** Dönen düz resim olmayacak.
1. **Codex aynı koridoru, aynı duvarı ve aynı tabloyu birebir çizsin:**
   - **Karşıdan görünüm:** kamera tablonun tam önünde.
   - **2–3 ara açı:** bugünkü açıdan karşıya doğru adım adım.

   Bugünkü görünüm başlangıç kare olarak **hiç değişmeden** kalsın. Yeni kareler onunla aynı oda olsun: aynı sütunlar, bitkiler, ışık, renkler, zemin deseni, aynı tablo ve çerçeve. Tutarlılık için bugünkü görüntüyü referans olarak ver, "aynı sahne, kamera yay üstünde X derece döndü" diye tarif et.
2. **Katmanlar:** her kare için ön plan (sütun, saksı, perde) ayrı şeffaf PNG; duvar ve tablo ayrı; zemin ayrı. Kamera yay çizerken ön plan hızlı, duvar yavaş kaysın (parallax). Ara açılar arasında yumuşak geçiş, ön plan katmanları geçişi gizlesin.
3. **Kenar boşluğu asla olmasın:** her katman ekrandan en az %15 büyük çizilsin. Otomatik kontrol: geçişin her karesinde ekran kenarında arka plan rengi (boşluk) piksel sayısı 0.
4. **Hareket eğrisi:**
   - yay ve yaklaşma tek kesintisiz hareket, ease-in-out;
   - yay ~0,9 sn, yaklaşma ~0,7 sn;
   - yay biterken yaklaşma başlasın (çakışmalı); arada duraklama yok, hız kesintisiz.
5. **Tutarlılık testi:** Codex kareleri yan yana koy (`docs/art/pm76/acilar.png`). Tablo, çerçeve ya da renkler birbirini tutmuyorsa o kareyi reddet, yeniden çizdir. Zeyd "birebir aynısı olması lazım" dedi. En iyi 2 deneme saklansın.
6. Aynı yöntem **koridor kayışında** da kullanılsın: bölümden bölüme ve menüye dönüşte katmanlı parallax kayış, aynı eğri ailesi.

## 2. Bütün geçişler: akıcılık turu
Her geçişi listele, 60 fps GIF ile kaydet, önce/sonra koy (`docs/art/pm76/`):
- açılış → menü
- menü → bölüm (yay + dalış + yapboz + HUD)
- kazanma ekranının gelişi
- kazanma → sonraki bölüm (koridor + dalış)
- bölüm → menü
- panellerin açılıp kapanması (Durdur, Ayarlar, kaybetme, devam, sandık)
- booster seçim modu giriş/çıkış

Kurallar:
- **Doğrusal hareket yok:** hepsi ease-in-out ya da yaylı (spring). Ani başlama ya da durma yok.
- **Kesintisiz hız:** bir hareket bitip diğeri başlarken hız sıfıra düşmesin, ard arda adımlar ~%20–30 üst üste binsin. Bilerek konan bekleme (dalıştaki 0,5 sn "sadece resim" anı) hariç.
- **Sıçrama yok:** geçiş başlamadan önce gereken görseller ve gölgelendiriciler yüklensin. Ölçüm `&perf=1`: geçiş boyunca hiçbir kare 20 ms'yi aşmasın (PC).
- **Tutarlı süreler:**
  - paneller 0,3–0,4 sn;
  - ekran geçişleri 0,6–1,0 sn;
  - dalış toplamı ≤ 2,6 sn;
  - dokununca hızlansın (ikinci dokunuş bitirir).
- **Ufak hareket ayrıntıları:** hızlı kaymada hafif hareket bulanıklığı ya da iz, yerine oturan öğede küçük yaylanma (overshoot %3–5).
- Sonuç tablosu: geçiş · önce süre/en kötü kare · sonra süre/en kötü kare · eğri.

## 3. Değnek kararı (PM_75'te soruldu)
Ölçüm: tırnak değiştirmek durumların ancak %25'inde parça açabiliyor, bölüm başında hiç. PM önerisi, **varsayılan olarak uygula**:
- Değnek önce bugünkü gibi tırnakları yeniden dizmeyi denesin.
- Olmazsa **seçilen bölgede, en çok parçayı tutan parçanın çıkıntılarını düzleştirsin** (parça dikdörtgene döner, en az 1 parça serbest kalır).
- Böylece değnek her zaman işe yarar. Doluluktan (önünde parça olan) çıkamayan durumda bile tırnak düzleşince en az bir komşu açılacak şekilde seçim yap.
- Bayrak `&degnek=eski` ile eski davranış.
- Animasyon: çıkıntılar ışıkla eriyip düzleşsin, okunur ve yavaş.

## Teslim
- p23 güncel, EditMode ve PlayMode yeşil, `verify_build` p23 4/4.
- GIF'ler ve `docs/art/pm76/RAPOR.md`, `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri (`p23/?c=p23&test=1&lang=tr`, `p23/?c=p23&reset=1&lang=tr`).
