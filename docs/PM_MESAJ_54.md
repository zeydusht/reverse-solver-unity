# PM_MESAJ_54 — HUD şeritleri, Jigo tepkileri, devam teklifi, kazanma ekranı (p21)

Önce PM_MESAJ_55 bitmiş ve Codex **TASK40** bitmiş olsun. **Görsel kaynağı: `docs/art/task40/FINAL_LISTE.md`** (HUD31–40'tan final dosyalar; başlık = yarı gömülü krem kutu + kadife lacivert, dışa simetrik dalgalı flama; ikonlar HUD36; saat kahverengi; düğme 5 = Durdur). Hepsi **yalnız p21**; p20, kök ve pt1'e dokunma.

## 0. Çalışma şekli: ağır işi Codex CLI yapsın (ZORUNLU)
Claude limiti çok az, Codex'te limit var. Bu yüzden:
- Kod yazma, betik, görsel işleme, toplu dosya işleri → **`codex exec`** ile Codex CLI'ya ver (bu bilgisayarda kurulu, 0.166.0). Her alt iş için kısa ve net bir talimat yaz: hangi dosyalar, ne değişecek, hangi test geçmeli.
- Senin işin: işi bölmek, Codex'in çıktısını **kısaca** gözden geçirmek, EditMode + `verify_build` çalıştırmak, küçük düzeltmeleri kendin yapmak, yayınlamak, rapor yazmak.
- Uzun logları okuma; yalnız özet ve hata satırlarına bak.
- Görseller: `Assets/Art/_yeni/HUD32/` (değişenler) + `HUD31/`, `Jigo_v3/`, `Obstacles_v3/`, `Devam_v2/` (TASK31) + sevinç `Jigo/` (TASK29). Hangi dosyanın nereden geleceği `docs/art/task32/` özetinde. Renk/katman şartnamesi `docs/art/CODEX_TASK31.md`. Gelmediyse **yer tutucuyla** çalış, sonra dosyaları değiştir.

## 1. Çekiç: maviye dön (hemen)
`docs/art/task28/_arsiv/Z2-v3-mavi-cekic/booster-hammer-*.png` dosyaları pembe çekicin yerine geçsin (bütün durumlar). Diğer boosterlar aynı kalır.

## 2. Üst ve alt şerit düzeni — Royal Match ölçüsünde (ZORUNLU)
Yerleşim **birebir** `docs/art/HUD_ANALIZ_RM.md` (pt ölçüleri): üst şerit ekranın %19'u + altın/turuncu bordür + 3–4 pt gölge; sol "Parça" ve sağ "Süre" panelleri 113×118 pt; ortada 127 pt kemerli Jigo penceresi; alt şerit %8,6, **5 yuva** (4 booster + **Ayarlar**), tuşlar 62 pt ve şeritten %25 taşar.
- **Pause ve LV rozeti üstten kalkar**; 5. yuvadaki **Durdur** tuşu (pause ikonu) oyunu durdurur ve menü açar: üstte **Seviye N**, altında Devam / Ses / Çık. Süre durur.
- Panel başlıkları düz şerit üstünde, okunur boyda; rakamlar **tam ortalı**, gövdenin ~%34'ü (çok büyük olmasın).
- Rakamlar ve yazılar canlı metin (TR/EN), kalın konturlu. Süre ≤10 sn'de mercan + nabız.
- Tahta küçülmez; güvenli alan (çentik, home çubuğu).
- Bittiğinde RM ekranı ile bizimkini aynı ölçekte yan yana koyan bir karşılaştırma görüntüsü üret (`docs/art/task30/karsilastirma.png`); hizalar tutmuyorsa düzelt.
- `&art=eski` eski düzeni göstermeye devam etsin.

## 3. Jigo tepki durumları (madalyonda)
Durum makinesi, öncelik yukarıdan aşağı:
- **korku:** süre ≤3 sn **ya da** herhangi bir bomba 1'de
- **tedirgin:** süre ≤10 sn
- **mutlu (kısa, ~0,6 sn):** parça çıkarılınca, sonra eski hale döner
- **normal:** diğer her an
- Kaybedince **üzgün**, kazanınca kazanma ekranı.
Durum değişimi 0,15 sn yumuşak geçiş. Süre eşikleri tek yerde parametre olsun.

## 3b. Kilit (B: Kont'un mührü) ve zincir
- Kilit görseli `Obstacles_v3`. Açılma kareleri, ardından **düşüş kodla**: yerçekimi ivmesi (hızlanarak), hafif rastgele dönme (±25°), ~0,45 sn, son %30'da solma; mum kırıntısı parçacıkları. Akıcı olmalı, kasmamalı (Zeyd: "düşerken kasıyor").
- Zincir: p20'deki (art=eski) zincirin halkası, mevcut sarkma ve yaylanma koduyla. Kısa zincirlerde sarkma PM_55 kuralı (max 0,22 hücre / %10); Zeyd hâlâ "kısalar daha bol olabilir" diyorsa alt sınırı 0,28 hücreye çıkar.
- Kilit boyutu hücrenin ~%55'i.

## 4. Kazanma ekranı
Mevcut kazanma akışına büyük Jigo sevinç animasyonu + konfeti ekle. Süre ~2 sn, dokunmayla geçilebilir. Sonraki akış (koridor/tablo) aynı kalır.

## 5. Devam teklifi (kaybedince)
**Önce ölç, sonra yap:** şunları raporla, sonra devam et:
- Oyunda altın (coin) bakiyesi var mı? Bölüm başına kaç altın kazanılıyor?
- 40 bölümün süre limitleri (en kısa / medyan / en uzun) ve Joker kaç saniye veriyor?

Panel düzeni (TASK31 §9): kurdele "Süre doldu", ortada büyük üzgün Jigo, altında "+30 sn", büyük yeşil **satın al** tuşu (içinde altın ikonu + fiyat), sağ üstte kırmızı **X** (vazgeç). "Hayır, teşekkürler" metin tuşu bayrakla açılabilsin (`&devamhayir=1`), varsayılan kapalı; karar Zeyd'de.

Kural (PM kararı):
- **Süre bitince kaybedince** panel çıkar: "+30 sn" karşılığı altın. Fiyat aynı bölüm denemesinde artar: **900 → 1900 → 2900**, sonra teklif yok. Yeni denemede 900'e döner.
- Bomba patlayarak kaybedince teklif: bombayı sıfırla (sayaç 5'e döner) + 30 sn yok; fiyat aynı merdiven. (Tek panel, metin değişir.)
- Bölüm süresi 60 sn'nin altındaysa ekleme **+20 sn** olsun (parametre). Raporda öner.
- Altın yetmiyorsa "Devam" pasif görünür (mağaza yok, şimdilik yalnız gösterim).
- Altın sistemi yoksa: profilde basit bakiye ekle; her kazanılan bölüm +50 (parametre), başlangıç 1000. Yalnız p21'de.

Telemetri (çok önemli, tahmin modelimiz bozulmasın):
- Her devam olayını kaydet: bölüm, deneme no, kaçıncı devam, fiyat, sonuç.
- Devamla kazanılan oyun **ayrı işaretlensin** (`won_after_continue`). `v_attempts`/`v_pred_vs_actual` ilk-deneme kazanma hesabında bunları kazanma saymasın. Gerekirse `docs/sql/015_continue.sql` yaz, **uygulama** (PM uygular).

## Teslim
- p21 güncel. Linkler: normal oyun `p21/?lang=tr`, vitrin `p21/?lv=SHOW_ART&lang=tr`.
- Test bayrakları: `&jigo=tedirgin|korku|uzgun|mutlu|sevinc` (hali zorla göster), `&devam=1` (devam panelini zorla aç), `&coin=5000`.
- EditMode yeşil, `verify_build` 4/4.
- Rapor `docs/art/task29/ENTEGRASYON_RAPOR.md`: ölçümler (altın, süreler), hangi işleri Codex yaptı, eksikler, ekran görüntüleri (normal, tedirgin, korku, devam paneli, kazanma). PROJE_DURUMU'na tek satır.
