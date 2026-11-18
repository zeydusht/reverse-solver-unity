# PM_83 RAPOR: rehber eli, sandık ekonomisi, altın, Oyna düğmesi, tablolar 41–100

## 1. Rehber (1. bölüm)
**Git geçmişindeki eller** (`&el=` ile açılır, GIF'ler `docs/art/pm83/el/`):

| Bayrak | El | Tarih | Ne |
|---|---|---|---|
| `&el=a` (**varsayılan**) | Codex TASK41 eli | 10 Kasım (PM_59'da bağlandı; Zeyd 10 Kasım 15:10'da onayladı) | Beyaz eldiven, turkuaz manşet; 5 karelik sürükleme; parmak ucu parçaya basar, yol boyunca kayar, kalkar. TASK41 maketinde de karartma + aşağı kesik ok vardı. |
| `&el=b` | PM_60 rehberi | 11 Kasım | El yok; parça altın parlar, Jigo konuşur (PM_72'de geri dönülen sürüm). |
| `&el=c` | `codex/01-onboarding` | 12 Kasım | El yok; parçadan çıkış yönüne üç altın nokta + küçük ok. |
| `&el=d` | p20 eldivenli el | 3 Kasım (PM_36; PM_82'de bağlanan) | Eldivenin sırtı, yerinde ileri-geri iter. |

PM_82'deki "p20 eli" `d`'ydi. Zeyd'in tarif ettiği karartma + aşağı oklu el, TASK41'in onaylı maketi (`docs/art/task41/onizleme.png`, "Tutorial" karesi) ve p20'nin karartması (PM_31–33, "Karartma ve yazı güzel oldu"). Varsayılan bu yüzden `a` + karartma + kayan ok.

**Karartma (bütün `el` seçeneklerinde aynı):**
- Rehberli adımlarda (2–5) ekran %60 kararır; yalnız oynanacak parça ve gideceği yol açık (p20'nin tek maskeli örtüsü, yumuşak kenarlı delikler).
- Yolda TASK41'in kesik çizgili altın oku gidiş yönüne doğru kayar (1,2 sn döngü).
- Adım 4'te (takılma) ok kalkar, karartma kalır.
- Serbest oyunda (adım 6) karartma 0,25 sn'de tamamen kalkar. Diğer bölümlerde hiç yok.

**Jigo'nun kutusu:** üst şeridin hemen altında, Jigo solda. Tahta kutunun payı kadar aşağı iner (ekran yetmezse payın sığanı kadar; hücreler 44 pt'nin altına inmez).

**Metinler ve 7 adım:** PM_82'deki gibi. PlayMode testi 7 adımı, karartmayı (adım 2'de var, adım 6'da yok), TASK41 elini ve kutunun tahtanın üstünde durmasını kontrol ediyor.

## 2. Sandık ekonomisi
**Kural** (Core `ChestEconomy`, sayılar `live/config.json`'da):

| Anahtar | Değer |
|---|---|
| `sandik_her_n` | 5 |
| `sandik_altin` | [100, 150, 200] |
| `sandik_altin_sinir` | [20, 50] |
| `sandik_booster_bolum` | 10 |
| `sandik_booster` | 1 |
| `kont_altin_kat` | 2 |
| `kont_booster` | 2 |
| `zor_bonus` | 100 |

- Normal sandık 5, 10, 15 …; Kont sandığı çok zor bölümlerde (19 … 99; koyu kırmızı, kapakta Kont mührü); zor bölümde (14 … 94) sandık yok, kazanma ekranında "Zor bölüm bonusu +100".
- 3. bölümdeki eski sandık (ve açılmamış değnek) bu sette artık yok.
- Booster yalnız o bölüme kadar açılmışlardan (makas 6, çekiç 13, saat 18, değnek 25). Ağırlık 1/(1+elde)², yani en az olan en sık gelir. Bir sandıktaki ikinci booster, birinciyi elde sayar.

**Akış:**
1. Sandık kazanma ekranının içinde, Jigo'nun yerinde.
2. "Sandığa dokun!" yazısı; 3 dokunuşta sallanır, ışık sızar, açılır.
3. 5–8 para tek tek üstteki altın sayacına uçar; sayaç her parada tıklayarak artar (dokunsal tık). Ardından booster ikonları yuvalarına uçar.
4. "Topla" yok. Sandık bitince "Sonraki Seviye" ve "Menü" gelir; oyuncu kendisi geçer.

**Ekonomi testi:** `docs/art/pm83/EKONOMI.md` (1000 sanal oyuncu, sandıklar oyunun kendi kodundan).

| Bölüm sonu | Altın (ort.) | Elde booster (ort.) | Sandıktan altın | Sandıktan booster |
|---|---|---|---|---|
| 10 | 5579 | 3,2 | 200 | 1 |
| 25 | 5401 | 12,8 | 750 | 6 |
| 50 | 5640 | 16,0 | 2400 | 17 |
| 100 | 6060 | 20,6 | 6400 | 37 |

100 bölümde 29 sandık (20 normal, 9 Kont). Altın 5000 civarında dengede kalıyor: devama harcanan, kazanılanı karşılıyor. Booster birikiyor (100'de elde ortalama 20).

## 3. Altın para
Codex 3 seçenek çizdi (`docs/art/pm83/altin/`):

| Seçenek | Motif | Puan | Not |
|---|---|---|---|
| **A** | yıldız | **9/10** | 24 px'te en net okunan |
| B | Jigo yüzü | 8/10 | özgün ama küçükte yüz kayboluyor |
| C | taç | 7/10 | `elenen/` içinde |

**A bağlandı.** Her yerde aynı para:
- menüdeki altın sayacı (yeni hap, "+" yok);
- kazanma ekranı (+50 ve zor bölüm bonusu);
- sandık uçuşu ve üst sayaç;
- devam paneli;
- hediye (R4) sandığının üstü.

Eski dosyalar `docs/art/pm83/altin/eski/` içinde. Mağaza ekranı oyunda yok.

## 4. "Oyna · Bölüm N" düğmesi
Harita kısayolu varken olduğu boyutta (228 pt'ye kadar, 112:228 oran), ekranın ortasında. Zor ve çok zor renkleri, Kont mührü aynen.

## 5. Tablolar 41–100
- **Adlar:** `docs/art/pm83/TABLO_ADLARI.md`. Her 10 tablo bir salon: Kum, Mercan, Hasat, Bulut, Zümrüt, Kont'un Salonu. Oyunda koleksiyonda tabloya dokununca görünür.
- **Jiggle'lar:** Codex mevcut tabloları düzenleyerek ekledi (yeniden çizim yok). Her tabloda tek Jiggle (100'de birkaç), listedeki renkte (turkuaz, mercan, mor, sarı, pembe), tablonun nesnesiyle bir iş yapıyor.
  - 41–60'ta boyları %9–11 kaldı; 61–100 için %13–17 istendi.
- **Karşılaştırma:** `docs/art/pm83/tablo_karsilastirma.png` (1–40'tan 3 tablo, yenilerle yan yana) ve `docs/art/pm83/tablolar_41_100.jpg` (60'ı birden).
- p23'te yeni tablolar yayında; `live/`'a yayınlanmadı.

## Achievements
Yapılmadı (PM kararı: v1.1).

## Doğrulanmayan
- iPhone'da his: karartmanın koyuluğu, para uçuşunun hızı.
- Ekonomi testindeki oyuncu modeli varsayım; gerçek veri değil.
