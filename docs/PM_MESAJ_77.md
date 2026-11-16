# PM_MESAJ_77: Geçişler: eski koridoru geri getir, dalışı yavaş ve adım adım yap

Kodu kendin yaz. p23 üzerinde (p22 yedek). Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd (16 Kasım 22:58): "Bu niye böyle geçiş yapmaya başladı? Eskisi daha düzgündü. Şu anki çok garip, karışıyor. Niye bu kadar hızlı, acele ediyor? Yavaş yavaş geçsin. Karşısına geçme 2–3 resimle deneniyor ama hiç o etkiyi vermiyor. Adım adım, tane tane olsun."

## 1. Kazanma → sonraki bölüm: ESKİ koridor kayışı geri gelsin
- PM_74 öncesindeki koridor kayışını **birebir geri getir**: soldan sağa bir resimden diğerine kayış (PM_62/64 hâli). Eğrisi, süresi ve görseli aynı olsun; git geçmişinden al.
- PM_76'nın yeni parallax kayışını bu geçişte **kullanma**. Bölümden menüye dönüşte de eski hâl.
- Eski kayışın sonunda yeni tablo zaten tam karşıda duruyor. Oradan **yay yok**, doğrudan 3. maddedeki dalış:
  - yavaşça yaklaş,
  - sadece resim, 0,5 sn,
  - çizgiler ve yapboz oluşur,
  - uzaklaşarak oyuna girilir,
  - HUD gelir.

## 2. Menü → bölüm: yay çizerek tablonun karşısına geçme (önce yalnız bunu mükemmel yap)
**Şimdiki sorun:** 2–3 çizilmiş açı arasında geçiş, yay etkisi vermiyor ve çok hızlı.

**Önce yalnız bu adımı yap ve onaylat.** Dalış ve diğer adımlara ancak bu net ve anlaşılır olunca geç.
- **Yöntem: 3B diorama.** Az sayıda çizim arasında geçiş yetmiyor.
  - Codex'in tam karşıdan çizdiği koridor görselini katmanlara ayır: arka duvar + tablo, zemin, sütunlar, saksılar, perdeler.
  - Her katmanı Unity'de gerçek derinlikte bir düzleme koy: perspektif kamera ya da 2D'de ölçek + parallax hesabıyla eşdeğeri.
  - Kamera tablo merkezli bir çember üstünde **gerçekten** dönsün: yan açıdan tam karşıya. Böylece her karede doğru perspektif ve parallax olur, ara resim gerekmez.
- **Başlangıç karesi:** menünün bugünkü açılı görünümüne olabildiğince yakın olsun. Gerekirse menü arka planı da bu dioramanın o açıdan görüntüsü olur, böylece kesinti olmaz. Katman eksikse Codex'e çizdir: aynı oda, şeffaf, katman katman.
- **Kenar boşluğu yok:** her karede otomatik kontrol.
- **Hız:** yay **1,6–2,0 sn**, ease-in-out. Başta yumuşak hızlanma, sonda yumuşak durma, tablonun tam karşısında kısa (~0,2 sn) duruş.
- **Teslim (adım 1):** yalnız yay hareketinin 60 fps GIF'i, normal hız + yarım hız (`docs/art/pm77/yay.gif`, `yay_yavas.gif`). p23'te `&gecis=yay` bayrağıyla açık. 3. maddeyi yine yap, ama raporda yay için "Zeyd onayı bekliyor" yaz.
- Diorama tutmazsa, yani aynı oda hissi kaybolursa, **yedek yol:** Codex'e en az 10 ara açı çizdir. Aynı tutarlılık testiyle reddet ya da kabul et. Kareler arasında hareket telafili geçiş yap. İki yolu yan yana GIF'le karşılaştır.

## 3. Dalış ve oyuna giriş: tane tane, acelesiz
Her adım belirgin olsun, birbirine **binmesin**; adım aralarında kısa nefes (0,1–0,2 sn).

| Adım | Süre |
|---|---|
| a. Tablonun karşısında yavaşça yaklaşma, ekran resim olur | 1,0–1,2 sn, ease-in-out |
| b. Sadece resim | 0,5 sn |
| c. Uzaklaşırken yapboz çizgileri ve çıkıntılar belirir (Zeyd bunu beğendi, aynen kalsın, yalnız hızı) | 1,0 sn |
| d. HUD üstten, booster çubuğu alttan gelir | 0,4 sn |

- Toplam menüden: yay ~1,8 + dalış ~3 ≈ 5 sn. Kazanmadan: eski koridor + dalış.
- Dokununca hızlansın: ilk dokunuş 2×, ikincisi bitirir. İlk kez oynayan acele etmiyor; tekrar eden geçebilir.
- Önce/sonra GIF'leri: `docs/art/pm77/`.

## Teslim
- p23 güncel, EditMode ve PlayMode yeşil, `verify_build` p23 4/4. `RAPOR.md`, `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta. "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr` (kazanma → eski koridor + dalış)
  - `p23/?c=p23&reset=1&lang=tr`
  - `p23/?c=p23&test=1&lang=tr&gecis=yay` (yalnız yay adımı, Zeyd onayı için)
