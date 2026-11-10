# Codex Task19B — Menü ekranları yeniden: ONAYLI STİLDE (önce 2 ekranlık pilot)

Zeyd Task19'u reddetti: "Bu bizim görsel stilimiz değil." Haklı. Task19 ekranları düz ve karanlık bir web paneli gibi duruyor. Onaylı oyun ekranlarımız ise parlak, tok ve kabartmalı.

## Ölçü çubuğu: bunlara benzemeli
- `Assets/Art/Menu/Task13/preview-menu.png` (onaylı ana menü)
- `Assets/Art/Gameplay/Task15/preview-pause.png` (onaylı Durduruldu kartı)
- `Assets/Art/Gameplay/Task14/` kartları ve tuşları
- İşçilik ve yoğunluk referansı: `docs/art/references-menu-contact-sheet.png` (Royal Match ekranları).
  - Bunlar yalnız **işçilik ve yerleşim** dersi içindir.
  - Karakter, logo, amblem ve sahne kopyalanmaz.
  - Bizim Jiggles'ımız, Baron'umuz ve koridorumuz kullanılır.

## Task19'da yanlış olanlar → doğrusu

| Task19'da | Olması gereken |
|---|---|
| Her sekme koyulaştırılmış (veil) koridorun üstünde, büyük boş lacivert alanlar | Her sekmenin **kendi parlak, sıcak ışıklı sahnesi** olsun. Örnekler: koleksiyon için müze/galeri odası, Jiggles için oyun odası/sahne, başarılar için kupa salonu. Koyu boş alan bırakma. Ekran Royal Match yoğunluğunda dolu olsun. |
| Başlık: düz yazı + ince altın çizgi | Başlık kalın, **kabartmalı bir altın/turkuaz pankart (ribbon) plaketinin** içinde. Kalın krem yazı, lacivert kontur ve alt gölge olsun. Onaylı "Durduruldu" başlığı gibi. |
| Liste satırları: düz krem dikdörtgen, küçük ince yazı | Her satır **kabartmalı bir karo** olsun: altın kenar, iç gölge, hafif üst parlama. Madalyon ve ikonlar 3D oyuncak hacminde. Yazı Fredoka Bold, **en az 15 pt** (@3x 45 px). |
| Çizgi/şerit gibi ince öğeler (bölüm yolu, ilerleme çizgisi) | Tok hacimli öğeler kullan. Bölüm yolu **zemin üstünde kalın halı/taş yol**, düğümler **büyük 3D jeton/düğme**. İlerleme çubukları Task13 `progress-track` / `progress-fill` kalınlığında. |
| Soluk, düşük doygunluk | `PALETTE.md` renkleri tam doygunlukta, sıcak ana ışıkla. Onaylı menüdeki parlaklık. |
| Kod ile düz çizilmiş (vektör gibi) yüzeyler | Level tuşu v2'de olduğu gibi: onaylı parçalar referans verilerek **görsel üretimle** sahne ve paneller. Sonra gerçek yazılar katman olarak bindirilir. Yeni bir parça, yanına konduğu onaylı parçadan ayırt edilemesin. |

Ek kurallar:
- Task19'daki tüm kurallar (klasör, palet, font, güvenli alan, ekonomi yok) aynen geçerli.
- **Sekme etiketi her zaman okunur olmalı.** Seçili sekmede etiket çerçevenin arkasında kalmaz.

## Zeyd kararları (Task19'dan)
1. Yerleşim fikri ve Jiggles rafı kalıyor. **Görünüş yeniden yapılacak.**
2. Koleksiyon sayacı yalnız kazanılan tabloları sayar: örnekte **11/40**.
   - Ev'deki ilerleme çubuğu da 11/40 olur.
   - Profil kutusundaki "Bölüm 12/40" kalır.
3. Biten bölüm düğümleri **turkuaz-altın**, yeşil yok.
   - Şu anki düğüm: altın dolgulu, parlak, büyük.
   - Kilitli düğüm: gri-lacivert, büyük kilit.
   - Zor bölüm: kırmızı kenar + Baron yüzü (19'u örnek göster).

## Bu tur: PİLOT (yalnız 2 ekran)
Önce stilde anlaşalım, 9 ekranı boşa çizmeyelim.

1. `02-koleksiyon.png`
   - Galeri odası sahnesi.
   - Kalın pankart başlık "Koleksiyon".
   - Sağ üstte "11 / 40" hapı (Task13 `counter-pill`).
   - Kabartmalı altın çerçeveli tablo ızgarası: kazanılmış, sıradaki ve kilitli hücreler.
2. `04-basarilar.png`
   - Kupa salonu sahnesi.
   - Pankart başlık "Başarılar".
   - Kabartmalı karo satırlar ve 3D madalyonlar.

İkisinin yanına `pilot-karsilastirma.png` koy. Solda onaylı `preview-menu.png` + `preview-pause.png`, sağda yeni iki ekran. Aynı oyundan çıkmış gibi görünmeliler.

Klasör: `Assets/Art/Menu/Task19B/`. Task19 dosyalarını silme (arşiv).

Bitince 2 satır yaz. Zeyd pilotu onaylarsa aynı stilde kalan 7 ekran (01, 02b, 03, 05, 06, 07, 08) gelecek turda yapılacak.
