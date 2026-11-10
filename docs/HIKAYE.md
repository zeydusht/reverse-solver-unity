# Reverse Solver — Hikâye ve görsel kimlik (taslak, 22 Ekim 2026)

Durum: Konuşma aşamasında. Henüz kod yazılmayacak. Playtest'ten önce yalnızca ikon ve menü arka planı yapılacak; geri kalanı playtest'ten sonra.

## Kararlaştırılanlar (Zeyd)

- **Karakterler:** Yapboz parçaları sürü halinde takılan, minyon tadında sevimli karakterler.
  - Yalnızca fikir alınıyor; minyonların görünüşü (sarı kapsül gövde, gözlük, tulum) kullanılmayacak.
- **Kötü adam:** Yapbozları tamamlıyor, çerçeveletip şatosunun duvarına asıyor. Parçalar sonsuza kadar hapis kalmak istemiyor.
- **Oyuncu:** Şatonun duvarlarındaki tabloları indirip yapbozu söküyor ve parçaları kurtarıyor. "Reverse Solver" adı buradan anlam kazanıyor: kötü adam yapbozu tamamlıyor, sen söküyorsun.
- **Menü:** Kötü adamın uzun koridoru, duvarda bir tablo, altında "Oyna" tuşu. Tuşa basınca kamera tabloya yaklaşıyor ve bölüm başlıyor.
- **Bölümler:** Her bölüm farklı bir resimli yapboz. Mekanik aynı kalıyor.

## PM önerileri (Zeyd onayı bekliyor)

- **Koridor = bölüm haritası.** Çözülen tablolar boş çerçeve olarak kalır, sıradaki tablo parlar.
- **Engeller = kötü adamın güvenlik önlemleri:**
  - Çivi tabloyu çakar.
  - Zincir çerçeveyi kilitler.
  - Bomba "dokunan patlasın" tuzağıdır.
  - Duvar kapalı şerittir.
- **Zor bölümler = değerli eserler:** Bölüm 19, 25, 30 ve 35–40 altın çerçeveli olur. Bölüm 40 kötü adamın kendi portresidir (final).
- **Okunabilirlik:** Parça sınırları ve engeller resmin üstünde net görünmeli. Bunun playtest'te ölçülmesi gerekiyor.
- **Görsel tutarlılık için üretim sistemi (Ruby Games yöntemi):**
  1. Tek bir stil kartı.
  2. Her karakter için tek bir referans kart.
  3. Her tablo için aynı kalıp: tema / sahne / ortam.

## Kararlar (Zeyd, 22 Ekim 21:02)

- **Kötü adam:** Uzun boylu, siyah giyinen, kibirli ama komik görünen bir koleksiyoncu. Gru'dan esinleniyor.
  - Gru'nun ayırt edici özellikleri kullanılmayacak: kel kafa + uzun burun + çizgili atkı kombinasyonu.
- **Kurtarılan parçalar** oyuncunun sürüsüne katılır.
- **Ses:** Bir parça itildiğinde kısa, tatlı, sevimli bir "veey" sesi çıkar.
  - Ses bize ait olmalı (kendi kaydımız ya da lisanslı ses). Minyonların sesi kullanılmayacak.
- **İsimler** İngilizce olacak.
- **Profil ekranı:** "Şu ana kadar kurtardığın parça sayısı" gösterilir. Bu, toplam ilerlemeyi gösteren bir meta sayaç.
- **Okunabilirlik:** Parça kenarları belirgin çizilir.

## Açık sorular

1. Tablolarda ne var? (sonra netleşecek)
2. Kötü adamın adı. Adaylar:
   - Baron Von Glue
   - Count Grimframe
   - Mortimer Gluegrave
   - Lord Lockwood

## Kararlar (Zeyd, 22 Ekim 23:46)

- **Parçacıkların adı:** Jiggles. Jigsaw kelimesinden geliyor; kıpır kıpır, sevimli bir isim.
  - PM notu: Mağazada aynı ya da çok benzer adlı bir oyun olup olmadığı yayından önce kontrol edilecek.
- **1000 parça ödülü:** Jiggles bir araya gelip oyuncuya bir hediye verir ve "Bizi kurtardığın için teşekkürler!" der. Ara kilometre taşları da olabilir (100 / 500 / 1000).

## Sıradaki adım

Sorular netleşince şunlar yazılacak:
- stil kartı
- 3 karakter kartı: parçacık, kötü adam, koridor

Bunlar Higgsfield'da üretilecek.

## Kararlar (Zeyd, 8 Kasım)

- **Açılış animasyonu** (`docs/art/CODEX_TASK26.md`): çayır → Baron Jiggles'ı toplar, birleştirir, dondurur → çerçeveletip duvara asar (kısa görüntü) → koridor dolusu tablo → **oyuncu tek bir Jiggle'ı dokunarak uyandırır** → "Free them all!" ve 1. bölüm.
- **Kahraman = uyanan Jiggle.** Oyuncu bu Jiggle'dır. Herkesi kurtarmak için tabloları sırayla açar. Sıradan Jiggles'tan tek bir ayırt edici öğeyle ayrılır.
- PM notları (sonra konuşulacak): koridor haritasında kahraman Jiggle sıradaki tabloya yürüyebilir. Eğitimdeki eldivenli el kahramanın eli mi olacak, yoksa sadece arayüz ipucu olarak mı kalacak, karar verilecek. Kahramanın adı açık.
- **Kötü adamın adı: Kont** (Zeyd, 8 Kasım). "Baron Von Glue" ve diğer adaylar çalışma adıydı. Görsel referans aynı: `Assets/Art/Characters/Baron-reference-v01.png`. Oyundaki "Baron" metinleri (ör. zor bölüm kartı) Kont olarak değişecek.
- **Açılış videosu tam senaryosu:** `docs/art/CODEX_TASK26_V06.md` (11 çekim, mekân/zaman/aksesuar sürekliliği, geçişler). Kalkan: düğme (Zeyd). Kont Jiggles'ı mavi uçlu cila fırçasıyla dondurur, tamir kesesini (fırça + yüksük + düğme) tablonun arkasına asar; tablo inince kese düşer, kahraman Kont'un kendi fırçasıyla savaşır. Şato vadinin sağ arkasında tepede, net görünür. Video AI ile üretilecek (Codex kareler + Higgsfield).
