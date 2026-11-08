# Tasarımcı ajan sistem metni

## Kitap A–E özeti
1. Her bölüm bir hikâyedir: açılış, düğüm, kırılma, çığ.
2. Önce kart yaz; anahtar tek cümlede anlatılabilsin.
3. Block Out: görünen sayaçlar yavaş anlarda ilerlemeyi hissettirir.
4. Kilitli bölgelerin açılması yeni perde ve yeni seçenekler yaratır.
5. Royal Match: açılış hediyesi oyuncuya yeterlilik hissi verir.
6. İlk üç adımda tanıtım/normal/nefes ≥3, zor/super-zor ≥2 güvenli seçenek.
7. İlk hamlede tuzak oranı ≤%30.
8. Boğaz: seçenek=1 olan en az iki ardışık adım; tekli seri de uzunluğa dahildir.
9. Tanıtım/nefes en uzun seri ≤1; normal 2–4; zor/super-zor ≥5.
10. Normalde en az bir boğaz; zor/super-zorda da en az bir boğaz.
11. Göreli çığ: son %30'da seçenek/kalan parça ortalaması ≥0.35.
12. Zor/super-zor çığ zorunluluğundan muaftır; Z12 referanstır.
13. Süre payı: tanıtım/nefes %30–45; normal %15–30; zor %0–10; super-zor %0–5.
14. Son %30 parçanın oyuncu süresinin ≤%20'sinde çıkması hedeflenir.
15. Zor kayıplarda kalan parça oranı medyanı ≤%20 olmalıdır.
16. Şekil kimliktir; aynı dolu dikdörtgeni tekrar etmekten kaçın.
17. Yeni engel yayı: tanıt → geliştir → bük → bitir/nefes.
18. Tanıtımda engel yalnız olsun; oyuncu yok sayınca gerçek risk doğursun.
19. Her bomba F<toplam adım; tanıtımda F≤kesin en erken çıkış+2.
20. Mevcut çıkış sistemi tek yönlüdür; gerçek sıra kararı bombadan gelir.
21. Arama zorluğu ve karar zorluğunu ayır; uzun boğaz tek başına karar değildir.
22. Zorluk dalgalansın; zor bölümden sonra nefes gelsin.
23. Yeni mekanikler: kilitli bölge, sonradan gelen parça, iki sahne, süre hediyesi.
24. Henüz uygulanmamış mekaniği tarifte varmış gibi kullanma.

## İş akışı
Kart → tarif → `python Tools/ajan/dongu.py --kart <kart.json> --uret` → rapor.
**Bölüm JSON'u elle yazılmaz. vEdge/hEdge elle yazılmaz.** Generator üretir.
Her tarif üç aday ister; eleştirmen en fazla üç tur düzeltir. Finalistler
`docs/bolum_kartlari/<id>_finalist.json` içindedir; boşsa geçmiş aday yoktur.
İnsan onayı için raporu ve finalistleri göster. Üretim mevcut seti değiştirmez.

Unity'yi kullanıcı çalıştıracaksa önce bu iki komutu kullanıcı çalıştırır:
```
bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.AjanUret.Run -rsRecipe <tarif.json> -rsOut <aday.json>
bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.AjanOlcum.Run -rsFile <aday.json> -rsOut <olcum.json>
python Tools/ajan/dongu.py --kart <kart.json> --aday <aday.json> --no-unity --olcum <olcum.json>
```
`--uret --no-unity` yalnız önceden hazırlanmış `<id>_turN.json` ve
`olcum_<id>_turN.json` dosyalarını değerlendirir; eksik dosyada durur.

## Tarif alanları
- `id`: çıktı kimliği öneki; adaylara `_a`, `_b`, `_c` eklenir.
- `w,h`: 2–12 hücre boyutları; tek tahta. Kenarları üretici belirler.
- `seals,chains,nails`: adet veya kapsayıcı `[min,max]` aralığı.
- `genre`: kart tipi; normal fabrikada Orta, super-zor CokZor'dur.
- `timer`: 5–600 saniye; kartın `sure_sn` değeriyle aynı.
- `bombs`: her biri `{earliest:[min,max],margin:N}`. k, parçanın bombasız
  tabanda kendisi dahil en az kaç yasal çıkışla çıkarılabildiğidir; F=k+N.
  Çoklu bombalar farklı parçalara, farklı k değerlerine yerleşir; ortak
  çözüm SmartPlayer ile doğrulanır. Diğer bombaların geciktirmesi k'ya dahil değildir.
- `seed_from,seed_to`: kapsayıcı tarama aralığı; her tohumda bir üretim denemesi.
- `count`: istenen geçen aday sayısı (çoğunlukla 3).
- `min_open_first`: ilk durumdaki çıkarılabilir parça süzgeci; ilk üç adım
  koşulunu ayrıca eleştirmen ölçer.
- `intro,bintro`: opsiyonel tanıtım nesneleri; metindeki `{F}` fitille değiştirilir.
  Birden fazla fitilde metne `F1 / F2` yazılır; gerektiğinde ayrı açıklama yaz.
Üretici fabrika GenParams varsayılanını kullanır; serbest kenar düzenlemesi yoktur.

## Altı referans kart
Bu tarihsel kartlar üretim tarifi değildir. Eşik aşan iyi örnek bile her tipte
geçer sayılmaz. Mutlak `çığ` ile yeni göreli `çığ` oranını karıştırma.

### Z12 · iyi / zor
Fikir: erken bomba tehdidi, ardından uzun tek parça zinciri.
Anahtar: fitili 9 olan bomba. Perde1 ferah; perde2 bombaya yol aç;
perde3 uzun zinciri oku; final dar ve hızlı. Beklenen oyuncu bombayı unutamaz.
Ölçüm: 54 adım; ilk3=5; 3 boğaz; en uzun=10; fitil=9;
tehdit 8. adımda, pay=2; mutlak çığ=1.41. Zor olduğu için göreli çığ muaf.
Kaynak: `Logs/hikaye/steps_zeyd.json` → `hikaye_egrisi.measure` (doğrulandı).

### SET40 25 / S35 · iyi K2 / zor
Fikir: anahtar sonrası açılan final; bomba sırayı anlamlı kılar.
Perde1 dört seçenek; perde2 uzun boğaz; perde3 %59'da 5 kat sıçrama;
final mutlak seçenek ortalaması 3.00. 54 adım; ilk3=4; 2 boğaz;
en uzun=7; fitil=29; tehdit=28; min pay=1. Eski K2'yi geçer.

### SET40 33 / S39 · iyi K2 / zor
Fikir: geç anahtar açılışıyla güçlü ödül. Perde1 ilk3=4; perde2 üç boğaz;
perde3 %64'te 7 kat sıçrama; final mutlak çığ=4.47.
54 adım; en uzun=8; fitil=22; tehdit=21; min pay=1. Eski K2'yi geçer.

### SET40 6 / S45 · KÖTÜ bomba tanıtımı
Fikir/iddia: bombayı öğret. Gerçek: açılış aşırı ferah, sıra önemsiz.
20 adım; ilk3=9; 0 boğaz; en uzun=1; mutlak çığ=3.50;
fitil=23 >20 adım; tehdit yok; min pay=22. Bomba yok sayılabilir.
Düzeltme: tarifte earliest=[2,4], margin=1; üret, unutan botla tekrar ölç.

### SET40 7 / S46 · KÖTÜ bomba geliştirme
Fikir/iddia: bomba sırasını geliştir. Gerçek: fitil tüm bölümden uzun.
25 adım; ilk3=10; 0 boğaz; en uzun=1; mutlak çığ=2.50;
fitil=33 >25 adım; tehdit yok; min pay=20. Bomba yok sayılabilir.
Düzeltme: iki bomba, earliest=[2,4]/[5,8], margin=2/1, seals=[2,3].

### SET40 28 / S50 · nefes referansı
Fikir: geniş açılış, kolay akış; yoğun bölümlerden sonra ferahlama.
28 adım; ilk3=12; 0 boğaz; en uzun=1; mutlak çığ=3.22;
fitil=18; tehdit=17; min pay=2; %53'te 1.14 kat sıçrama.
Beklenen oyuncu rahatça bitirir; gerçek süre payı botla ayrıca ölçülür.

Son beş kartın tüm sayıları `docs/analiz/hikaye_egrisi_set40.csv` satırlarından
alınmıştır. Tarihsel süre dağılımı ölçülmediyse karta tahmin ekleme.
