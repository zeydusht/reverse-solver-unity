# PM_MESAJ_78: Yay, Zeyd'in çizdiği yoldan ve ORİJİNAL menü resmiyle

Kodu kendin yaz. p23 üzerinde (p22 yedek). Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd (17 Kasım): "Yay muhabbetini çok iyi anlamış (önce sağa, sonra ileri). Ama oradaki resim hoşuma gitmedi. Bizim asıl ana menü resmimizde yapsın."

Zeyd yolu kendisi çizdi: "Kamera Yolu Stüdyosu" (`https://claude.ai/artifact/PdS8tq4SkdDa3xpbRQtLGG`). Notu: "Bu çizdiğim yolda ama aralarda durmadan gitsin." Veri aşağıda; dosyası `docs/art/pm78/zeyd_yol.json`.

## 1. Codex'in PM_77 odasını kaldır, orijinal menü resmiyle diorama kur
- PM_77'de Codex'in yeni çizdiği oda (duvar, zemin, sütun) **beğenilmedi**, kullanma. Menünün bugünkü resmi (PM_74 sonrası menü arka planı) değişmeyecek; başlangıç karesi **birebir** o olsun.
- **Codex'in işi yeni oda çizmek değil, orijinal resmi tamamlamak.** Orijinal menü resmini ver ve şunu iste: aynı oda, aynı renkler, aynı çizim tarzı; tablo, çerçeve, kemer, sütun, flama, aplik ve halı birebir aynı. Çıktı:
  - karşıdan (tablonun tam önünden) görünüş,
  - ayrı şeffaf katmanlar: duvar + tablo, zemin + halı, sağdaki kemer/sütun, flama, aplik.
  
  Orijinal resimde görünen her şey aynı kalsın; yalnız görünmeyen kısımlar (yanlar, karşıdan bakınca açılan yerler) tamamlansın.
- **Kabul testi:**
  1. Dioramayı başlangıç pozundan (aşağıdaki 1. kare) çiz.
  2. Orijinal menü resmiyle yan yana koy, bir de üst üste %50 saydam karşılaştırma yap (`docs/art/pm78/baslangic_karsilastirma.png`).
  3. Tablo, çerçeve, renkler ve kemer üst üste oturmalı.
  4. Tutmuyorsa o çizimi reddet ve yeniden çizdir; en çok 4 tur. Yine tutmazsa dur, en iyi 2'yi raporla.
- Başlangıçta menüden dioramaya geçerken **çapraz geçiş yok**. Menü arka planı zaten bu dioramanın başlangıç görüntüsü olsun; Zeyd "karışıyor" dedi.

## 2. Yol: Zeyd'in kareleri, durmadan tek akış
Stüdyonun ölçüsü:
- birim metre;
- `x` sağa +, `z` duvardan uzaklık;
- tablo merkezi x=0, y=1,75; tablo 1,25 × 2,0;
- kamera yüksekliği 1,5;
- `yaw` 0° = duvara dik, + = sağa;
- `fov` dikey görüş açısı.

Kendi diorama ölçüne çevirmek için tablo genişliğini ölçek al.

| Kare | x | z | kafa | fov | süre (öncekinden) |
|---|---|---|---|---|---|
| 1 (başlangıç) | −1,454 | 2,499 | tabloya kilitli (30,2°) | 52 | — |
| 2 | −1,004 | 2,517 | tabloya kilitli (21,8°) | 52 | 1,2 sn |
| 3 | −0,474 | 2,405 | tabloya kilitli (11,1°) | 52 | 1,2 sn |
| 4 | −0,024 | 2,122 | tabloya kilitli (0,7°) | 52 | 1,2 sn |

- **Toplam 3,6 sn, arada durma yok.** Kareler arasında hız sıfıra düşmesin. Tek akış: yalnız başta yumuşak kalkış, sonda yumuşak duruş. Global smootherstep ile zaman eğrisi, yol üzerinde Catmull-Rom (centripetal).
- **Kafa** her an tablo merkezine bakar: konumdan hesapla, kareler arası açı ara değerlemesi yapma.
- Stüdyodaki önizleme aynı formülle oynuyor, kontrol için ona bak.
- **4. kareden sonra** PM_77'deki dalış adımları aynen:
  - yaklaşma 1,1 sn,
  - sadece resim 0,5 sn,
  - yapboz çizgileri 1,0 sn,
  - HUD 0,4 sn.
  
  4. kare ile dalış arasında durma olmasın: yay biterken yaklaşma başlasın, hız kesintisiz.
- Kenar boşluğu denetimi (`&dalisbosluk=1`) her karede 0.

## 3. Yayın
- Yeni yay `&gecis=yay` bayrağında kalsın. Zeyd onaylayınca varsayılan olur.
- Kazanma → sonraki bölüm: PM_77'deki eski koridor + dalış, dokunma.
- GIF'ler `docs/art/pm78/`:
  - `yay.gif` (normal), `yay_yavas.gif` (yarım hız);
  - `baslangic_karsilastirma.png`;
  - Codex katmanları ve kontrol görüntüleri.

## Teslim
- p23 güncel, testler yeşil, `verify_build` p23 4/4.
- `RAPOR.md`, `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr&gecis=yay`
  - `p23/?c=p23&test=1&lang=tr`
