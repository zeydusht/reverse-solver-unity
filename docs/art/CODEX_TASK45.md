# CODEX_TASK45 — Kalan ekranları yeni HUD diline taşı

HUD, kazanma ve sandık yeni dilde (old money mavi + krem, doygun altın/turuncu, hacimli, 3B başlık yazısı, siyah kontur yok; örnekler `T44`, `HUD31–40`). Bazı ekranlar hâlâ eski stilde. Hepsini aynı dile getir. **Önce mevcut hâllerine bak** (p21'de oyna ya da `Presentation/` altındaki ilgili görselleri ve kodu bul) ve önizlemede **önce/sonra** göster.

## 1. Durdur paneli (ortadaki büyük panel KALIYOR)
Tahtayı tamamen kapatan ortadaki panel. Yeni dilde: krem gövde, mavi çerçeve, 3B başlık "Durdu", üstte "Seviye N" küçük rozet. Tuşlar: **Devam** (büyük yeşil), **Ses** / **Müzik** / **Titreşim** (yuvarlak, açık/kapalı), **Çık** (kırmızı, küçük). Arka plan tam karartma.

## 2. Kaybetme ekranı (devamı reddedince ya da teklif bitince)
Üzgün Jigo (onaylı `Jigo_v3` üzgün), 3B başlık "Olmadı!" (ya da "Süre bitti" / "Bomba patladı" alt satırı canlı), **"Tekrar Dene"** büyük yeşil düğme, küçük "Menü" düğmesi. Moral bozmayan, sevimli ton.

## 3. Engel tanıtım kartları (bomba, kilit, zincir)
Yeni metinler PM_59/60'ta: kısa, net. Kart: krem gövde, mavi çerçeve, 3B başlık ("Bomba!", "Kilit!", "Zincir!"), ortada engelin büyük görseli + **mini animasyon kareleri**:
- Bomba: sayı 3 → 2 → 1 ve parça çıkınca bomba söner (patlamaz). 6–8 kare.
- Kilit (Kont mührü): sayı 2 → 1 → 0 ve kilit açılıp düşer.
- Zincir: iki parça birlikte kayar.
"Anladım!" yeşil düğmesi.

## 4. 1. bölüm anlatım parçaları
- Alttaki **sohbet kutusu** (alt şeridin hemen üstünde): krem, mavi çerçeve, sol köşede küçük Jigo büstü (normal ve konuşan iki kare), metin canlı. T41 balonundan daha zarif ve ince; ekranın %18'inden fazla yer kaplamasın.
- Önü açık parçayı gösteren **altın sarısı parlama halkası**: parçanın kenarını saran yumuşak altın ışık, nabız döngüsü 6 kare. Parçanın resmini kapatmasın.

## 5. "Bütün parçaları tahtadan çıkar!" bandı
Bölüm başında üstten inip çıkan ince bant: yeni dilde, 3B küçük yazı, parça ikonu.

## Teslim
`docs/art/task45/onizleme.png` (her ekran önce/sonra, 390×844), dosyalar `Assets/Art/_yeni/T45/`, `docs/art/task45/LISTE.md`. 3 satır özet, dur. Zeyd'in kuralları: kurdele/afiş başlık yok, Jigo mutlu/normalde kaş çatmaz, soluk efekt yok, RM'yi birebir kopyalama yok (ölçü ve katman dili serbest).
