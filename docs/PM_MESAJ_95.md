# PM_MESAJ_95: Lastik 2 varsayılan; müzik ve kaydırma sesleri "gerçek oyun" kalitesinde (dinleme sayfasıyla seçim)

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız aşağıdakiler.

Zeyd (20 Kasım 17:37): "Lastik 2 iyi olmuş, onu koyalım. Müzikleri halledecektiniz. Kaydırma ses efektini de değiştirin, bunlar çok robotik. Gerçek oyun gibi daha iyisi lazım."

## 1. Lastik
`&lastik=2` varsayılan olsun. 1 ve 1.5 bayrakla kalır.

## 2. Ses kalitesi: yeni adaylar + dinleme sayfası
Zeyd telefonda hızlıca dinleyip seçecek. **Dinleme sayfası:** `p23/onizleme/sesler.html`.
- Her aday için büyük oynat/durdur düğmesi, adı, süresi, lisansı.
- Müzik döngüsü ve efektler ayrı bölümlerde.
- iOS Safari'de çalışsın: ilk dokunuşta ses kilidi açılsın.
- Oyunun stilinde (krem kart, mavi başlık), sade.

### Müzik (PM_86'daki a–e adayları seçilmedi; daha iyileri lazım)
- **Tarz:** cozy / casual mobil bulmaca oyunu (Royal Match, Gardenscapes havası). Sıcak, neşeli, akustik ağırlıklı (pizzicato yaylı, marimba, ukulele, yumuşak perküsyon, hafif flüt/klarinet); "old money" saray dokunuşu olabilir. 90–115 BPM, uzun süre dinlenince sinir bozmayan, melodisi tekrar ettiğinde rahatsız etmeyen.
- **En az 8 yeni aday.** Ticari kullanıma atıfsız izin veren kaynaklardan: Pixabay Music, CC0 kütüphaneler. Her birinin lisans linki ve koşulu `docs/art/pm95/LISANS.md`'ye. "Yalnız kişisel kullanım", atıf zorunlu ya da Content ID'li parçalar **alınmaz**.
- Her adayı kusursuz döngüye getir: tıklama yok, geçiş fark edilmesin, 60–120 sn.
- Ses seviyesi eşitlensin (müzik ~−18 LUFS, efektlerin altında).
- PM_86'nın a–e'si de sayfada kalsın (karşılaştırma).
- **Ayrı:** kazanma jingle'ı için 3 aday (kısa, 1,5–3 sn, sevinçli).

### Kaydırma ve parça sesleri (şimdikiler robotik)
Gerçek kayıtlı (foley) sesler, sentetik değil. Kaynak: CC0 paketler (ör. Kenney ses paketleri, Pixabay sound effects, Freesound'da yalnız CC0). Lisanslar aynı dosyaya.

| Ses | Ne zaman | Aday sayısı | Tarif |
|---|---|---|---|
| **Kaydırma / çıkış** | parça tahtadan çıkarken | 5 | kısa "whoosh" + yumuşak "pop" ya da tahta/karton sürtünmesi; tatlı, yorucu değil |
| **Çarpma** | lastik takılma | 4 | yumuşak, kauçuk "boing" / "bonk", lastik 2 hissine uygun; kısa, hafif tok |
| **Son parça / tablo kurtuldu** | son parça | 3 | parıltılı "ta-da" |
| **Düğme tık** | her düğme | 3 | yumuşak, tok UI tık |

**Uygulama:**
- Her sesin 3–4 hafif varyasyonu ya da ±%5 rastgele perde: art arda çıkışlarda makineli tüfek gibi aynı ses olmasın.
- Sesler önceden yüklensin, gecikme < 30 ms.
- Ses ayarı (Ses / Müzik anahtarları) ve sessiz mod kuralları aynen.
- Oyunda bayrakla denenebilsin: `&muzik=`, `&sescikis=`, `&sescarpma=`, `&sesbitis=`, `&sestik=` (aday harfi).

**Varsayılan:** sen en iyi bulduğunu varsayılan yap ve gerekçesini yaz. Zeyd sayfadan seçince kalıcı yaparız.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (dinle ve seç)
  - `p23/?c=p23&test=1&lang=tr&lv=2` (lastik 2 + yeni varsayılan sesler oyunda)
