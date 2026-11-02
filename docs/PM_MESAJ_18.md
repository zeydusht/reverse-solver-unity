# PM → Claude Code (28 Ekim 2026, 13:45) — 40 tablo her bölümde

Codex Task16 bitti. 40 tablo resmi `Assets/Art/Paintings/` klasöründe:
- dosyalar `painting-01.jpg` … `painting-40.jpg` (1024 px, her biri ~300 KB)
- bölüm ve tema bilgisi `manifest-collection.json` içinde

Zeyd her bölümde kendi tablosunu görmek istiyor. Kurallar:
- Soru sorma.
- Token tasarrufu yap.
- Her adımda commit at.

## Yap

1. **Her bölüm kendi tablosuyla.**
   - SET40 v3'te bölüm N, `painting-NN` resmini kullansın. Eşleşme manifest'teki sıraya göre.
   - Bu, yeni setin varsayılan görünümü olsun; `painting=1` parametresi gerekmesin.
   - Düz mavi görünüm yalnız `?set=classic` ile ve resim yüklenemezse yedek olarak kalsın.
   - SHOW1 Akdeniz resmiyle aynen kalsın.
2. **Kenarlar.** Mevcut resimli mod ayarlarını kullan: kalın koyu lacivert kenar, krem ışık şeridi, bevel, sürüklenen parçanın gölgesi. Resimler çok detaylı. Tutorial bölümlerinde (1 ve 2) ve 3 rastgele zor bölümde (ör. 19, 30, 40) şunları kontrol et:
   - kenarlar
   - bomba, çivi ve zincir sayaçları
   - kırmızı-beyaz duvar

   Okunmayan varsa kenarı ya da engel konturunu güçlendir. Resmi değiştirme.
3. **Yükleme.**
   - Resimleri level'lar gibi GitHub Pages'ten çek ve önbelleğe al.
   - Bölüm açılırken sıradaki bölümün resmini önceden yükle.
   - Web build boyutu büyümesin; resimler build'in içine gömülmesin.
   - Resim gelmezse düz mavi set açılsın, oyun beklemesin.
4. **Ana menü.** Koridordaki büyük tablo, oyuncunun sıradaki bölümünün resmini göstersin; "bu tabloyu kurtar" hissi versin. Mevcut altın çerçeve aynen kalsın.
5. **Kazanma anı (basit).** Kazanınca kart açılmadan önce tamamlanmış resim 1 saniye tam görünsün (parçalar kurtarıldı → tablo özgür).
6. **Yayın.**
   - Önizleme 18'i `?set=redesign` adresine yayınla.
   - Kök adresteki Build 5'e dokunma; playtest sürüyor. Kök için Zeyd'in "kökü güncelle" onayını bekle.
   - Linki ve 3 telefon boyutundaki QA sonucunu OVERNIGHT.md başına yaz.
7. **iOS.** Bir sonraki iOS sürümüne (0.5.1) dahil et; titreşim işiyle birlikte.
