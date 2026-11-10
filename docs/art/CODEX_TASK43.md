# CODEX_TASK43 — Haz rötuşu + Durdur menüsü + ödül sandığı + "bir tane daha" görselleri

Bağlam: `docs/PT1_GERIBILDIRIM.md` (B ve G). Oyuncular bölümü kazanıp çıkıyor; "bir tane daha" isteği yok. Stil: onaylı HUD (old money mavi + krem, doygun altın/turuncu, hacimli, siyah kontur yok), Jigo `Jigo_v3`. RM referansı: `docs/art/booster ref/kareler/`.

## 1. Parça çıkışı + engel dağılması: daha güçlü (TASK42 rötuşu)
Çekiç darbesi kadar güçlü olsun: **opak**, yarı saydam altın sis değil. Merkezde beyaz-sarı parlak çekirdek, savrulan **dolu renkli** yıldızlar ve konfeti, kalın şok halkası, 2–3 karelik "pop". Engel dağılmasında metal parçalar ve kıvılcım **büyük ve net**. Oyun ekranında, gerçek boyutta önce/sonra.

## 2. Durdur menüsü (RM ayarlar modeli)
`ayarlar_genel.png`: dişliye basınca üstünde **dikey yuvarlak tuş sütunu** açılıyor. Bizde Durdur tuşunun üstünde:
- Tuşlar (yuvarlak, booster tuşuyla aynı katman dili): **Çık** (kırmızı), **Müzik**, **Ses**, **Titreşim** (yeşil; açık/kapalı iki hâl: kapalıda çapraz çizgi).
- Sütunun üstünde küçük krem rozet: **"Seviye 19"** (metin canlı).
- Arka plan karartması. Açılma: tuşlar sırayla aşağıdan yukarı "pop" (4 kare yeter, kod da yapabilir).

## 3. Ödül sandığı (3., 5., 10. bölüm ve sonrası)
- Sandık: kapalı, sallanma (beklerken), açılma (kapak fırlar, ışık huzmesi, 6–8 kare), içinden saçılan ödüller.
- Ödül ikonları: altın yığını (+100), booster ikonları (mevcut 4 ikon, küçük), "x1" rozeti.
- "Ödül!" başlık rozeti ve "Topla" düğmesi (yeşil).
- Kazanma ekranında **ilerleme çubuğu**: "Sandığa 2 bölüm kaldı" (çubuk + sandık ikonu ucunda).

## 4. Merak kancası: sonraki tablo
- Kazanma ekranında **"Sıradaki tablo"** kartı: altın çerçeveli, üstü **kumaşla örtülü** tablo (kumaş kenarından resmin bir köşesi görünür), köşede Kont'un küçük mührü. Kumaşın kalkma animasyonu (4–6 kare).
- Kont'un **siluet teaser'ı**: kartın arkasından gölge olarak bakan Kont (komik, kibirli), 2 poz.

## 5. Kurtarılan Jiggle'lar sürüye katılıyor
- Kazanma ekranının alt kısmı için **sürü sahnesi**: Jigo önde, arkasında kalabalık Jiggle sürüsü (5 renk), yeni kurtarılanlar koşarak katılıyor (koşu kareleri TASK41'de var; burada kalabalık arka plan + "+4" rozeti).
- Sürü büyüdükçe kalabalık görünsün: 3 yoğunluk seviyesi (az / orta / çok).

## Teslim
`docs/art/task43/onizleme.png`: patlama önce/sonra (oyun ekranında), Durdur menüsü açık hâli, sandık akışı, yeni kazanma ekranı maketi (sandık çubuğu + sıradaki tablo + sürü), Kont siluet. Dosyalar `Assets/Art/_yeni/T43/`, `docs/art/task43/LISTE.md`. 3 satır özet, dur.
