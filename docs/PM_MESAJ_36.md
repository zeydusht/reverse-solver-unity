# PM → Claude Code (3 Kasım 2026, 01:20) — Zeyd'in 4 notu (p20)

**İlk iş:** commit et, PM dosyalarını geri alma. Zeyd'in kendi Codex'i `Assets/Art/Redesign_T23/` altında çalışıyor, oraya dokunma.

## 1. Eğitim eli: ELDİVENLİ el, arkadan
- Şu anki `hand-back-v1` çıplak ve ten rengi. Zeyd **önceki beyaz eldivenli eli** istiyor, yalnız **arkadan görünen** hali:
  - Eldivenin sırtı ve dikiş çizgileri görünsün.
  - İşaret parmağı yukarı, diğer parmaklar kıvrık.
  - Bilekte eldiven manşeti olsun.
- Kendi Codex'ine çizdir: `Assets/Art/Tutorial/hand-glove-back-v1.png` + `-press`.
  - Referans: eski eldivenli el (Codex eldiveni, 28 Ekim'deki tek el, `Resources/Art` altındaki eski hand-swipe).
  - Rengi, konturu ve malzemesi aynı olsun, sadece açı değişsin.
- Boyut: tahtada bir hücreden biraz büyük (~1,3 hücre). Şu anki el küçük kalıyor.
- Hiç dönmesin (mevcut kural). Tüm eğitimlerde bu el.

## 2. Zor bölümlerde Baron iki kez çıkıyor
- Zor bölümlerde (19, 23, 25, 30, 35, 36, 38) yalnız **tek** kart çıksın: **zor bölüm uyarısı** (Baron).
- Başka bir bölüm başı ya da tanıtım kartı da gösteriliyorsa kaldır.
- **23 hem zor hem çekiç tanıtımı:** önce zor uyarısı (Baron), sonra çekiç booster eğitimi (el), toplam iki adım. Aynı Baron iki kez olmasın.
- Nedenini OVERNIGHT'a yaz (hangi iki kart üst üste geliyordu).

## 3. Ev sol üst profil hizası
Şimdi avatar kutusu büyük, isim plaketi küçük ve ayrı, sayaç farklı hizada. Royal Match `IMG_2440` gibi düzenle:
- Avatar kutusu 60–64 pt kare. Avatar çerçevenin **tam ortasında**, iç boşluk eşit.
- İsim plaketi avatarın sağına **yapışık**: sol ucu avatar çerçevesinin arkasından çıkar, ~8 pt üst üste biner.
  - Plaketin yüksekliği avatarın ~%55'i.
  - Dikey merkezi avatarın merkeziyle aynı.
  - İsim yazısı A3 stilinde ≥ 13 pt. Uzunsa küçülsün, en az 11 pt.
- Jiggle sayacı ve dişli aynı dikey merkezde.
- Sayacın ikonu hapın solundan taşsın (mevcut). Arada eşit boşluk.
- 375×667, 390×844 ve 430×932'de, kısa ve uzun isimle ekran görüntüsü al.

## 4. Bölümler merdiveni düğümleri
Zeyd düğümleri beğenmedi. Codex Task23 harita düğümlerini yeniden çiziyor (`Assets/Art/Redesign_T23/NIHAI/` altında). Hazır olunca onları bağla. Hazır değilse bu maddeyi şimdilik atla, OVERNIGHT'a "Task23 bekleniyor" yaz. Kod tarafında hazırlık:
- Kilitli düğümde numara düğümün **içinde** dursun, kilit küçük rozet olarak sağ altta. Şu an numara düğümün altında kalıyor.
- Düğüm merdiven basamağına otursun, havada durmasın.
- Bitenlerde küçük yeşil tik rozeti.

## Kontrol
- `?c=ic&unlock=1`: bölüm 1 eğitimi (el), bölüm 19 ve 23 (kart sayısı), Ev üst bar.
- Ekran görüntüleri → `docs/qa/2026-11-03-dort/`.
- OVERNIGHT.md başına 2 satır.
