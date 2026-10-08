# Telefon testi — L01 el + K1 + G1 + M5 build'i

Bu listeyi build yayınlandıktan sonra kullan. Claude yayını haber verdiğinde başla. Süre yaklaşık 25 dakika.

**Adres:** https://zeydusht.github.io/reverse-solver-unity/
**Oyuncu adın:** her zaman `TEST_ZEYD`. Bu ad analizden çıkarılır.
**Getireceklerin:** her madde için geçti ya da kaldı. Kalan maddelerin ekran görüntüsü. K1 için 10 denemeden kaçının çıktığı. iPhone modelin.

---

## 1. Yeni oyuncu (yeni özel sekme, debug yok)

1. Safari'de yeni bir **özel sekme** aç ve kök adrese git.
2. İsim kapısı çıkmalı. `TEST_ZEYD` yaz, **Başla**'ya bas.
3. Menüde **Oyna · Bölüm 1**'e bas.
4. **L01 eli:**
   - Sol alttaki parçanın üstünde beyaz bir el olmalı.
   - Elin solunda turuncu bir ok olmalı; ok ekranın içinde, tahtanın kenarında durmalı.
   - El sola doğru kıpırdamalı ve parça hafif turuncu parlamalı.
   - Parçalar bölüm açılırken küçükten büyüyerek gelmeli. Bu sırada da sürükleyebilmelisin.
5. Gösterilen parçayı sola sürükle. Parça çıkmalı ve el kaybolmalı.
6. L01'i bitir. Bitiş kartında **Sonraki bölüm**'e bas, L02 açılmalı.

## 2. Gönderim (M5)

1. L01'i bitirdikten sonra en az 20 saniye bekle.
2. Supabase → Table Editor → `plays` tablosunda en yeni satırı aç. Şunlar olmalı:
   - `player` = TEST_ZEYD
   - `client` = unity-web
   - `level_id` = L01
   - `level_version` = 1
   - `row_id` dolu
   - `attempt` = 1
   - `dur` oynadığın süreye yakın
   - `result` = win
3. Sayfayı yenile. İsim tekrar **sorulmamalı**. Menü **Bölüm 2**'yi göstermeli.
4. **Getir:** satırın ekran görüntüsü.

## 3. Kenar parçaları (K1)

1. Adresin sonuna `br/?lv=1&debug=1` ekle.
2. Debug paneline bir kez dokunup kapat.
3. Tahtanın iki yanında artık daha geniş boşluk olmalı: 375 pt genişlikte yaklaşık 34 pt.
4. Toplam **10 çekiş** yap: L01'de sağ sütundan, sol sütundan, üst sıradan ve alt sıradan parçaları dışarı çek.
   - Parmağını parçanın dış yarısına koymayı da dene.
   - Tahtanın biraz dışına basmayı da dene; en yakın kenar parçası tutulmalı.
5. Sonra `br/?lv=40&debug=1` (6×7) ile aynı denemeleri tekrarla.
6. Her çekişten sonra debug'daki üçüncü satıra bak: `son sürükleme: …`
   - `Exited` = çıktı, `SnappedBack` = geri yaylandı, `Cancel→` = sistem dokunmayı kesti.
   - Ardından ofset/gereken (pt), hız (pt/s) ve basışın içeriden mi dışarıdan mı olduğu gelir.
7. **Getir:** 10 denemeden kaçı **tek seferde** çıktı? Çıkmayanlarda bu satır ne yazıyordu?
   - Hedef: 10'da en az 9.

## 4. Görselli tasarım (G1) — yalnızca Claude "G1 bu build'de" dediyse

1. `br/?debug=1&set=designs&lv=D01` adresini aç.
2. Önce renkli parçalar görünmeli, kısa süre sonra resim belirmeli.
3. Resim parçalara bölünmüş gibi görünmeli; çıkıntılar komşu parçanın resmini sürdürmeli.
4. Girinti ve çıkıntılar seçilebiliyor mu? Parçaları birbirinden ayırt edebiliyor musun?
5. Bir parçayı sürükle. Engelleyen parçanın kırmızı parlaması ve çivili parçanın gri görünümü doğru mu?
6. D02 ve D03'ü de aç (`lv=D02`, `lv=D03`).
7. **Getir:** okunabilirlik için kendi notun (iyi / zor / çok zor) ve ekran görüntüleri.

## 5. Kısa kontrol listesi (debug yok, `TEST_ZEYD` ile)

- [ ] Kilitli bir bölüme dokunduğunda açılmıyor.
- [ ] L03'teki tanıtım kartı okunuyor ve kapanıyor.
- [ ] Bitiş kartındaki **Tekrar**, **Sonraki** ve **Menü** düğmeleri çalışıyor.
- [ ] Sürüklerken sayfa kaymıyor ve yakınlaşmıyor.
- [ ] Hiçbir yazı kırpılmıyor.
- [ ] Çivili bir parçaya basınca parça sarsılıyor (yeni).
- [ ] Çivi sayacı her hamlede kısa süre büyüyüp küçülüyor; çivi sökülünce parça parlıyor (yeni).

## 6. Güçlendiriciler (`br/?lv=12&debug=1`)

- [ ] Makas: bir eklem noktasına dokun, kenar düzleşmeli.
- [ ] Değnek: onay kartı çıkmalı; onaylayınca kenarlar değişmeli.
- [ ] Çekiç: bir parçaya dokun, parça çıkmalı.
- [ ] Saat: süreye 20 saniye eklenmeli.

## 7. Genel izlenim

- Efektler (parçaların gelişi, parlama, sarsılma) göze batıyor mu, oyunu yavaşlatıyor mu?
- Sürüklerken FPS'e bak (`?debug=1`, L40). Hedef 55 ve üstü.
