# Codex Task19C — Pilot revizyonu: daha az altın, oyunun GÜNCEL hali, daha iyi alt çubuk

Zeyd Task19B pilotuna (Koleksiyon + Başarılar) "fena değil" dedi. Üç düzeltme istedi. Task19B'deki tüm kurallar (Royal Match stil referansı, kopyalama yasağı, klasör, palet, font, güvenli alan) aynen geçerli.

## 1. Altın çok fazla → Royal Match oranı

Task19B'de ekranda 10–15 süslü altın çerçeve var: her tablo, her satır, sütunlar, başlık. Göz yoruluyor. Royal Match'te (`Assets/Art/References/`) altın **az ve vurgu** olarak kullanılıyor: paneller renkli, altın yalnız ince kenar/şerit.

Kurallar:
- **Ekran başına en fazla 1 büyük süslü altın öğe.** Ya başlık pankartı ya kahraman tablo, ikisi birden değil.
- **Oran:** yüzeyin yaklaşık %60'ı sahne + krem/turkuaz zemin, %30'u turkuaz/lacivert paneller, **altın en fazla %10** (ince kenar, küçük vurgu).
- **Koleksiyon ızgarası:**
  - Tablo çerçeveleri sade olsun: ince koyu ahşap ya da turkuaz çerçeve, köşede küçük altın nokta. Süslü oymalı altın yok.
  - Kilitli hücreler: düz lacivert + kilit.
  - Hücreler arası boşluk biraz daha geniş.
- **Başarılar satırları:**
  - Krem karo + turkuaz kenar. Satırda altın çerçeve yok.
  - Yalnız madalyonun dış halkası ince altın.
  - "Tamamlandı" tiki yeşil.
- **Arka plan sahnesi:**
  - Sütunlar ve duvarlar altın değil: petrol duvar, krem taş, ahşap.
  - Altın yalnız küçük süs (şamdan, ince korniş).
  - Sahne panelin arkasında hafif bulanık/sakin kalsın, paneli bastırmasın.

## 2. Oyunun GÜNCEL hali (eski Task13 menüsü artık geçerli değil)

Önizleme 12'den bu yana menü değişti. **Ekranlara bak:**
- Güncel menü: `docs/qa/2026-10-29-p19/tr_390x844_2_menu.png`
- Ayarlar: `docs/qa/2026-10-29-p19/tr_390x844_5_ayarlar.png`
- Oyun ve kazanma: `docs/qa/2026-10-31/show_390x844_2_tahta.png`, `show_390x844_3_kazanma.png`

Oyunda şu an olanlar:

| Yer | Güncel durum |
|---|---|
| Sol üst | **Profil kutusu:** Jiggle avatarı + oyuncu adı. Adın altında "Jiggles: 128" yazısı **yok**. |
| Üst orta | **Tek sayaç:** Jiggle ikonu + kurtarılan Jiggle sayısı. |
| Sağ üst | **Ayarlar dişlisi.** |
| Orta | Koridor duvarında büyük tablo (sıradaki bölümün tablosu). |
| Alt | Büyük yeşil **"Level 1" / "Bölüm 1"** tuşu. |
| Ayarlar kartı | Alt alta turkuaz tuşlar: "Ses: Açık", "Müzik: Açık", "Titreşim: Açık", "Dil: Türkçe" + yeşil "Kapat". |

Oyunda şu an **olmayanlar** (tasarımda bunlara göre davran):
- **Yıldız sayacı kaldırıldı.** Menüde, profilde, koleksiyonda ve başarılarda yıldız sayacı ya da "⭐⭐⭐" satırı gösterme.
  - Koleksiyon hücresinin altında yıldız yerine bölüm numarası olsun.
  - Profildeki "Yıldız" kutusunun yerine "Kalan süre ortalaması" gelsin.
  - Başarılar'daki "Yıldız Avcısı" yerine: **"Hızlı Parmaklar — 10 bölümü sürenin yarısı dolmadan bitir"**.
- "Jiggles rescued 128/500" ilerleme çubuğu **yok**.
- Alt sekme çubuğu ve "Bölümler/Levels" tuşu oyunda **henüz yok**. Bunlar bu tasarımla gelecek yeni öğeler, eklemeye devam et.
- Ekonomi yok: para, kalp, mağaza yok.

**`01-ev.png` güncel menünün aynısı olsun.** Tek fark: alttaki yeni sekme çubuğu ve küçük "Bölümler" tuşu. Profil kutusu, Jiggle sayacı, dişli, tablo ve yeşil tuş güncel ekrandaki yerlerinde ve görünüşlerinde kalır.

## 3. Alt sekme çubuğu daha iyi

Mevcut Task13 şeridini sadece kopyalama, Royal Match'teki alt çubuğa bakarak yeniden çiz:
- **Çubuk:** tam genişlik, koyu turkuaz/lacivert. Üstte ince altın şerit, başka altın yok.
- **Sekmeler:** 4 eşit bölme, aralarında ince ayırıcı çizgi.
- **Seçili sekme:**
  - Çubuktan yukarı taşan, açık turkuaz parlak yuvarlak kabarcık (altın değil).
  - İkon %30 büyük.
  - Etiket ikonun ALTINDA, kabarcığın dışında, her zaman tam okunur.
- **Seçili olmayanlar:** daha küçük ikon, hafif soluk. Etiket kalın krem yazı, koyu kontur.
- **İkonlar:** 4 sekme ikonunu (Ev, Koleksiyon, Jiggles, Başarılar) aynı setten görünecek şekilde yeniden çiz. 3D, tok, aynı ışık yönü.
- **Bildirim rozeti:** kırmızı, kalın, krem rakam (Başarılar'da "1").

## Bu turun teslimi (yalnız 4 dosya)
Klasör: `Assets/Art/Menu/Task19C/`

1. `01-ev.png`: güncel menü + yeni sekme çubuğu + Bölümler tuşu.
2. `02-koleksiyon.png`: az altın, yıldızsız.
3. `04-basarilar.png`: az altın, yıldızsız ("Hızlı Parmaklar").
4. `08-sekme-cubugu.png`: 4 seçili hal + seçili olmayan ikon seti.
5. `pilot-karsilastirma.png`: solda güncel oyun menüsü ekranı, ortada Task19B, sağda Task19C (aynı ekranlar). Altın azaldı mı, tek bakışta görülsün.

Bitince 2 satır yaz. Zeyd onaylarsa kalan ekranlar (02b, 03, 05, 06, 07) bu dille yapılacak.
