# PM → Claude Code (3 Kasım 2026, sabah): harita düğümlerini bağla + 2 küçük düzeltme + PM_35'in kalan denetimi (p20)

**İlk iş:** commit et. PM dosyalarını asla geri alma. `Assets/Art/Redesign_T23/` klasörü Codex'in, içindeki dosyaları **değiştirme**, yalnız oradan kopyala.

## 1. Bölümler haritası: yeni düğümler (Codex Task23 nihai)
- Kaynak klasör: `Assets/Art/Redesign_T23/NIHAI/01-bolumler/`
  - Ölçüler ve 9-slice bilgileri: `NOT.md` ve `deneme.json`
  - Kıyas görseli: `once-sonra.png`
- Sprite'ları oyunun kendi sanat klasörüne kopyala, `LevelsMap` başındaki sprite anahtarlarını bunlara çevir:
  - node-current
  - node-complete + complete-check
  - node-locked + lock-badge
  - node-hard-locked + hard-marker
  - node-contact-shadow
  - play-bubble
- Numara Nunito Black ile düğümün içinde kalsın. PNG'de metin yok.
- **Zor bölüm düğümü:**
  - Kırmızı kenar kalsın, `hard-marker` (Baron rozeti) da görünsün.
  - Kilitliyken kilit rozeti sağ altta, Baron rozeti üstte; çakışmasınlar.
  - Zor bölüm şu anki bölümse altın yüzey + kırmızı kenar + Baron rozeti.
- Şu anki düğümdeki eski geniş sarı parıltı kalksın. Yeni altın jeton zaten yeterince belirgin.
- Düğümler basamağa otursun (temas gölgesi).
- **Dokunma alanları:** düğümde en az 54×54 pt, Oyna balonunda en az 84×44 pt.

## 2. Zor bölüm kartı (19, 23 vb.)
- Ortadaki **bomba ikonu kutusunu kaldır.** Zor bölümün bombayla ilgisi yok, yanlış ipucu veriyor.
- Metin başlığı tekrar ediyor. Şöyle olsun:
  - TR: "Acele etme, önce bak."
  - EN: "Take your time, look first."
- Başlık "Zor bölüm" ve Baron sahnesi aynen kalsın.

## 3. PM_35'ten kalan: davranış denetimi (yapılmamış)
`docs/UX_KOD_DENETIM.md` oluştur. **Düzeltme yapma**, yalnız tablo hazırla (ekran / sorun / önem / öneri). Kontrol edilecekler:
- Çift tıklayınca kart iki kez açılıyor mu?
- Her kartta X ya da geri tuşu çalışıyor mu?
- Kart açıkken arkadaki tahtaya tık geçiyor mu? (PM_33'te otomatik oynayıcının kart kapatma tıkları tahtaya düşmüştü, buna özellikle bak.)
- Sekme geçişinde takılma var mı?
- Uzun isim ya da İngilizce metinde taşma var mı?
- Sesler doğru mu?

Ek olarak Codex denetiminden iki metin/kod maddesine bak (`docs/art/DENETIM_2026-11-03.md`):
- **T03 "to the up":** İngilizce dilbilgisi hatası. Düzelt; küçük metin hatası olduğu için izinli.
- **U02 kart yüksekliği taşma riski:** 375×667'de kontrol et, yalnız raporla.

## Kontrol
- `?c=ic&unlock=1` ile:
  - Haritanın baş, orta ve son kısmı; 375×667 ve 390×844
  - Bölüm 19 ve 23 kartı
- Ekran görüntülerini `docs/qa/2026-11-03-sabah/` klasörüne koy.
- 5 dk kuralı geçerli (Unity build hariç).
- p20'ye yayınla. Kök ve p19'a dokunma.
- Bitince OVERNIGHT.md başına 2 satır yaz.
