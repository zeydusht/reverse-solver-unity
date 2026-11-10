# Royal Match booster referansı: kare kare analiz (PM, 10 Kasım)

Kaynak: `docs/art/booster ref/*.mov` (Zeyd'in ekran kayıtları, 1170×2532, 60 fps). Kare paftaları bu klasörde (`*_genel.png` 2 fps genel bakış; `hammer/arrow/cannon/jester.png` yürütme anı, 12–15 fps; süre damgası her karede).

## Ortak kalıp (BÜTÜN boosterlar)
1. **Booster'a dokun → seçim modu.** Tahta dışındaki her şey (HUD, arka plan, alt şerit) ~%70 karartılır; **tahta aydınlık kalır**. Ekranın üstünde **büyük booster ikonu + adı** ("Royal Hammer!") + tek satır talimat ("Tap any object to smash it!"). Alt şeritte seçilen tuş **parlar ve kıvılcımlanır**, diğerleri sönükleşir. Oyun burada bekler; aynı tuşa tekrar basınca iptal.
2. **Hedefe dokun → karartma ~0,1 sn'de kalkar**, booster **alt şeritteki tuşundan çıkıp** hedefe uçar, işini yapar, ekrandan çıkar.
3. **Hedefsiz booster (Jester/karıştırma):** karartmada **tahta da kararır**, ortada büyük yeşil **onay düğmesi** ("Shuffle!"). Onaylanınca çalışır.
4. Her vuruşta: büyük parlama, parçalar havaya saçılır ve döner, toz/kıymık, kısa tahta sarsıntısı.

## Zamanlamalar (dokunuştan itibaren)
| Booster | Akış | Süre |
|---|---|---|
| **Çekiç** | Tuştan kavisle hedefe uçar, uçarken ~2 kat büyür, altında gölge (0–0,4 sn) → hedefin üstünde yukarı kalkar (0,27–0,4) → **iner ve vurur** (~0,07 sn) → büyük beyaz-sarı yıldız patlaması + tahta sarsıntısı (0,47–0,67) → çekiç geri seker, dönerek sağ alta düşüp ekrandan çıkar (0,67–1,0) | **~1,0 sn** |
| **Ok** (satır) | Dokunuştan ~0,27 sn sonra satırın sol kenarında belirir → mavi izle **çok hızlı** sağa uçar (~0,27 sn) → satırdaki nesneler yukarı fırlar, döner, kıymık → ok sağda durur, dönerek aşağı düşer (0,8–1,1) → kırıntı yağmuru 1,4'e kadar | **~1,3 sn** |
| **Top** (sütun) | Top sütunun altından yükselir, nişan alır (0–0,9 sn, yavaş "gerilim") → **ateş**: alevli ışın sütun boyunca yukarı (0,9–1,35) → nesneler patlar, saçılır → top geri teper, aşağı iner | **~1,7 sn** |
| **Jester** (karıştır) | Onay → şapka soldan uçarak ortaya gelir (0,1–0,4), hafif zıplar → nesneler havaya kalkıp şapkanın etrafında karışır (0,7–1,0) → yerlerine oturur → şapka pembe parıltı iziyle sağa çıkar (1,2–1,5) | **~1,5 sn** |

## Ayarlar (durdur)
Dişliye basınca ekran kararır, dişlinin **üstünde dikey bir sütun** halinde yuvarlak tuşlar açılır: Çıkış (kırmızı), Müzik, Ses, Titreşim (yeşil). Tekrar basınca kapanır. Bizdeki Durdur menüsü için iyi model: küçük, oyunu kapatmayan.

## Bize uyarlama
- **Seçim modu** bütün boosterlarımızda aynı: karartma (tahta aydınlık), üstte büyük ikon + Türkçe ad + tek satır ("Kırmak istediğin parçaya dokun"), tuş parlar, tekrar basınca iptal.
- **Çekiç** = Royal Hammer akışı birebir (yukarıdaki zamanlama).
- **Makas** = ok gibi hızlı ve keskin: tuştan çıkar, açılıp kapanarak ("çıt çıt") seçilen iki parça arasındaki bağa gider (~0,3 sn), keser (~0,2 sn, kırpıntı), aşağı kayıp düşer (~0,35 sn). Toplam ~1,0 sn.
- **Değnek** = Jester mantığı + bölge seçimi: karartmada tahta 4 bölgeye ayrılır, oyuncu bölgeye dokunur, **onay düğmesi** ("Değiştir!"), değnek uçar, bölgenin ortasına dokunur, o bölgedeki parçalar havalanıp yer değiştirir, değnek parıltıyla çıkar. ~1,5 sn.
- **Saat** = RM'de karşılığı yok. Jigo kol animasyonu (CODEX_TASK42 §2). Seçim modu gerekmez (hedefsiz), onay da gerekmez; basınca hemen başlar.
- Animasyonlar uzun olmasın: 1,0–1,7 sn. Oyuncu beklemekten sıkılmamalı.
