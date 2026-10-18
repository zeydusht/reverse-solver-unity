# Zorluk metrikleri — Zeyd'in oyun testinden (18 Ekim 2026)

Bu tablo Zeyd'in D04 ve D05 oyunlarından çıktı. Fabrikanın, Zorluk panelinin ve ajanların ölçtüğü **birincil zorluk ölçüsü** budur. Eski "keyif tahmini" ikincildir.

Zeyd'in gözlemi: "Bir parça 3–4 parçaya bağlıysa ve bunlardan çok varsa oyun zorlaşır. Sadece bir şeye bağlı olanı bulman gerekir. O tür parçaların sayısı 1–2 ise oyuncuyu yönetebiliriz."

| # | Metrik | Ne ölçer | Çok zor hedefi | D05'te ne oldu (Zeyd) |
|---|---|---|---|---|
| M1 | **Açılış hamlesi (k₀)** | İlk anda çıkarılabilen parça sayısı. **En önemli metrik.** | **1–2** | 6–7 parça hemen çıkıyordu |
| M2 | **Serbest parça tavanı** | Çözümün ilk %70'inde herhangi bir anda çıkarılabilen en çok parça | **≤ 3**; 10–20 parçanın aynı anda serbest kalması yok | Ortada çok parça birden açıldı |
| M3 | **Bağlılık derecesi** | Bir parçayı kaç komşunun kilitlediği (dik çıkıntılarla) | Parçaların çoğu **≥ 3 komşuya** bağlı; yalnız 1 şeye bağlı ("çözülecek") parça her an en fazla 1–2; hiç kenarı düz kare yok | Düz kareler her yöne gidiyordu |
| M4 | **Seri çıkış yok** | Aynı şeritten aynı yöne art arda çıkan parça | **≤ 2**; yan yana 4–5 parçanın tek itişle boşalması yok | Bombanın sağındaki 4–5 parça yukarı itince boşaldı |
| M5 | **Bomba merkezde, hamle hakkı en kısa yol + 1** | Bombanın kenara uzaklığı; fitil ile en kısa kazma yolu farkı | Kenara uzaklık **≥ 2 hücre**; kazma yolu ≥ 8; **fitil = en kısa yol + 1** | Bomba kenardaydı, kolay çıktı |
| M6 | **Çivi kilidi** | Bombanın yolunu kesen parçayı çivili bir parçanın tutması | Bombanın engelini çivili parça tutuyor; sayaç, o ana kadar yapılması **zorunlu** hamle sayısına eşit (boş hamleye yer yok); çivi kenarda değil (≥ 2 hücre içeride) | Çivi iyi tutuyordu ama açılınca hiç düşündürmedi |
| M7 | **Zincir yön çatışması** | Zincirli iki parçanın çıkabileceği yönler | A yalnızca {a, b}, B yalnızca {c, d} yönünden çıkabiliyor ve bu yönler ortak değil; ikisi birbirinin yolunu kapatıyor; uçlar yan yana değil, aynı satır ya da sütunda değil | Zincirler yan yana, tek itişle çıktı |
| M8 | **Duvar kolay yönde** | Mühürlü şerit, bir parçanın **kolay** çıkış yönünü mü kapatıyor | Kolay yönü duvarla kapanıp zor yöne zorlanan parça **≥ 3**; duvar şeridi ≥ 3 | 2 duvarın önüne neredeyse hiç parça gitmedi |
| M9 | **Kenar etkisi** | Bomba, çivi ve zincirlerin kenara uzaklığı | Hepsi **≥ 2 hücre** içeride | Engeller kenarda olunca kolaylaştı |
| M10 | **Süre** | D05'teki pay iyiydi (Zeyd bitirdiğinde 10 sn kalmıştı) | Süre payı 1,15 korunur; süre **ilk hamleyle** başlar | — |

## Adillik (değişmez)

- Bölüm çözülebilir olmalı; supap kullanılmamalı; kayıtlı çözüm kazanmalı.
- Gizli kural yok.
- "Psikoloji" bu adımda önemsenmiyor: amaç, metriklerin gerçekten zorluk yaratıp yaratmadığını görmek.
