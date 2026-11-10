# PM_MESAJ_62 — PM_60 geri bildirimi (Zeyd oynadı, 11 Kasım gece) (p21)

Zeyd: "Başlangıç için çok iyi animasyonlar." iPhone'da 0.6.0 (7) de denendi: fiske akıcı, titreşim güzel, kombo patlaması çok iyi. Aşağıdakiler düzelecek. Yalnız p21. Çizim gerekenleri **Codex'e** ver (`codex exec` ya da `docs/art/CODEX_TASK46.md` yazıp raporla). Dal değiştirmeden önce `docs/`'u commit et.

## 1. Test modu (Zeyd için)
`&test=1` (yalnız `c=ic` ya da `c=p21` ile): bütün bölümler açık, bütün booster'lar açık ve sınırsız (99), altın 99999. Kayıtlar `cohort=ic` gider, pt1/model verisine karışmaz. Linki rapora yaz.

## 2. Booster'lar
- **Kilitli booster'a dokununca:** "Seviye N'de açılır" balonu (şu an hiçbir şey olmuyor, Zeyd çekici bulamadı).
- **Makas:** biraz yavaş ve okunur: toplam ~1,0 → ~1,4 sn (gidiş 0,45, kesim 0,3, çıkış 0,45).
- **Değnek:** seçim modunda **süre DURMASIN**, akmaya devam etsin.
- **Saat: yeniden.** Kopuk/yamalı kol kalkıyor. Yeni akış:
  1. Jigo madalyonda **düğmeyi indirir**, **fırçasını kılıç gibi** sağa uzatır (madalyondan hafifçe taşarak, gövdesiyle birlikte eğilir; kol ayrı parça gibi görünmesin, tek parça poz kareleri).
  2. Fırça ucu **Süre paneline dokunur**: dokunduğu yerde ışıltı + panel **donar** (buz/ayaz kaplaması, rakamlar mavi-beyaz).
  3. Süre panelinin altında **10 sn'lik bar** dolar (ya da boşalır; görünür sayım). Süre bu sırada durur.
  4. Bar bitince buz kırılır, Jigo fırçayı geri çeker, düğmeyi alır.
  - Dondurma süresi **10 sn** (parametre).
  - Gerekli kareler (fırça uzatma 4–6 kare tek parça, geri çekme, buz kaplama + kırılma) → **Codex**.

## 3. Kilit okunurluğu
Kilit **%15 daha büyük**; sayı **beyaz**, kalın koyu kontur, daha büyük punto (koyu kırmızı mühür üstünde uzaktan okunmalı). Telefonda 64 pt hücrede kontrol et.

## 4. Kombo
Herhangi bir **yanlış hamle (takılma)** komboyu **hemen sıfırlar**.

## 5. Kazanma ekranı
- Yürüyen Jiggle'lar **kalkıyor**. Yerine sade: Jiggle ikonu + "**30 Jiggle kurtarıldı!**" (sayı canlı, küçük "pop").
- İki düğme: **Sonraki Seviye** (büyük, yeşil) ve **Menü** (küçük, ikincil).
- Ekran düzeni daha temiz: başlık, Jigo, sandık ilerlemesi, Jiggle sayısı, düğmeler. Boşluklar dengeli.

## 6. Jigo (madalyon) animasyonu
- **Göz kırpma azalsın:** rastgele 4–7 sn'de bir.
- **Durum geçişleri yumuşak:** normal → mutlu → normal arası ani kare atlaması değil, 0,2–0,3 sn yumuşak geçiş (crossfade + hafif ölçek/esneme).
- **Akıcılık:** şu an 30 FPS gibi kesik. Kare animasyonun üstüne kodla sürekli hafif nefes/esneme (sinüs ölçek, hafif sallanma) ekle; gerekirse Codex'ten normal ve mutlu için daha sık kare (12–24 fps) ve normal↔mutlu **geçiş kareleri** iste. Hedef: RM'deki kral gibi canlı ve akıcı.

## 7. Bölüm haritası (seviye listesi)
- Kendi bölümün ekranda görünmüyorsa (yukarı/aşağı kaydırınca) kenarda **soluk, küçük bir hap**: "Seviye N ↓/↑". Dokununca kendi bölümüne kayar.
- Ek olarak küçük **"en alta / en üste"** tuşu.

## 8. Kenar payı
PM_58'de 24 pt yapıldı, iOS'ta da var. Zeyd hâlâ kılıfa çarpma hissederse 28 pt'ye çıkarılabilir (parametre hazır olsun, şimdilik 24).

## Teslim
p21 güncel, `&test=1` linki, EditMode yeşil (Perf hariç), `verify_build` 4/4. GIF: yeni saat, makas, kazanma ekranı, Jigo geçişi, harita hapı. Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_62" eki, PROJE_DURUMU'na tek satır, commit + push.
