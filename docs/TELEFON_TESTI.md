# Telefon testi — Build 1 (13 Ekim 2026 gecesi)

**Bu build'de:** L01 öğretici el · K1 kenar parçaları (yan boşluk, kenardan tutma, fiske) · G1 görselli tasarımlar (D01, D02) · M6 efektleri · **canlı veri gönderimi açık**. Yayın: gh-pages `69241ed`, 13 Ekim 01:23.

**Adres:** https://zeydusht.github.io/reverse-solver-unity/
**Oyuncu adın:** her zaman `TEST_ZEYD`. Bu ad analizden çıkarılır.
**Süre:** yaklaşık 30 dakika.
**Getireceklerin:** her madde için geçti/kaldı, kalanların ekran görüntüsü, K1 için 10 denemenin kaçının tek seferde çıktığı, FPS sayıları, iPhone modelin.

> Gece başsız Chrome'da (bilgisayarda) hepsi denendi: el görünüyor, K1 yerleşimi doğru, D01/D02 görselli açılıyor, görsel gelmezse renkler çıkıyor, `TEST_GECE` satırı Supabase'e doğru düştü. Telefonda sen bakmadıkça bunlar iPhone için doğrulanmış sayılmaz.

---

## 1. Yeni oyuncu (yeni özel sekme, debug yok)

1. Safari'de **yeni özel sekme** aç, kök adrese git.
2. İsim kapısı çıkmalı. `TEST_ZEYD` yaz, **Başla**'ya bas.
3. **Oyna · Bölüm 1**'e bas.
4. **L01 eli:**
   - [ ] Parçalar bölüm açılırken küçükten büyüyerek gelmeli. Bu sırada da sürükleyebilmelisin.
   - [ ] Sol alttaki parçanın üstünde beyaz bir el olmalı; solunda turuncu ok, ekranın içinde.
   - [ ] El sola doğru kıpırdamalı, parça hafif turuncu parlamalı.
5. [ ] Gösterilen parçayı sola sürükle: parça çıkar, el kaybolur.
6. [ ] L01'i bitir. Bitiş kartında **Sonraki bölüm** L02'yi açar.

## 2. Gönderim (M5)

1. L01'i bitirdikten sonra en az 20 saniye bekle.
2. Supabase → Table Editor → `plays` → en yeni satır:
   - [ ] `player` = TEST_ZEYD · `client` = unity-web · `level_id` = L01 · `level_version` = 1
   - [ ] `row_id` dolu · `attempt` = 1 · `result` = win · `dur` oynadığın süreye yakın
3. [ ] Sayfayı yenile: isim **sorulmamalı**, menü **Bölüm 2**'yi göstermeli.
4. **Getir:** satırın ekran görüntüsü.

## 3. Kenar parçaları (K1)

1. Adresin sonuna `br/?lv=1&debug=1` ekle. Debug paneline bir kez dokunup kapat.
2. [ ] Tahtanın iki yanında öncekinden belirgin geniş boşluk var (375 pt'de yaklaşık 34 pt).
3. **10 çekiş:** sağ sütundan, sol sütundan, üst sıradan ve alt sıradan parçaları dışarı çek.
   - Parmağını parçanın dış yarısına koymayı da dene.
   - Tahtanın biraz dışına (çerçeveye, duvara) basmayı da dene: en yakın kenar parçası tutulmalı.
   - Kısa ve hızlı bir fiske de dene.
4. Sonra `br/?lv=40&debug=1` (6×7) ile aynısını yap.
5. Her çekişten sonra debug'daki üçüncü satıra bak: `son sürükleme: …`
   - `Exited` = çıktı · `SnappedBack` = geri yaylandı · `Cancel→` = sistem dokunmayı kesti · `(fiske)` = hızlı fiskeyle çıktı
   - Ardından ofset/gereken (pt), hız (pt/s) ve basışın içeriden mi dışarıdan mı olduğu.
6. **Getir:** 10 denemeden kaçı **tek seferde** çıktı (hedef 9 ve üstü). Çıkmayanlarda o satır ne yazıyordu.

## 4. Görselli tasarımlar (G1)

1. `br/?debug=1&set=designs&lv=D01` aç (gün batımı test resmi).
   - [ ] Önce renkli parçalar, kısa süre sonra resim beliriyor.
   - [ ] Resim parçalara bölünmüş gibi; çıkıntılar komşu parçanın resmini sürdürüyor.
   - [ ] Girinti ve çıkıntılar seçilebiliyor.
   - [ ] Çivili parça gri, bomba ve zincir okunuyor; bir parçayı sürükleyince engelleyen parça kırmızı parlıyor.
2. `lv=D02` aç (renkli halkalar; önce tanıtım kartını kapat).
   - [ ] **D02'de girinti ve çıkıntılar seçiliyor mu?** Desen karışık; gözünü yoruyor mu? (PM'in izleme notu)
3. **FPS:** `br/?debug=1&set=designs&lv=D02` açıkken 10–15 saniye sürekli parça sürükle, debug satırındaki "sürüklerken medyan" FPS'i oku. Hedef 60 (en az 55).
4. **Getir:** D01 ve D02 için okunabilirlik notun (iyi / zor / çok zor), FPS sayısı, ekran görüntüleri.

## 5. Kısa kontrol listesi (debug yok, `TEST_ZEYD` ile)

- [ ] Kilitli bir bölüme dokunduğunda açılmıyor.
- [ ] L03'teki tanıtım kartı okunuyor ve kapanıyor.
- [ ] Bitiş kartındaki **Tekrar**, **Sonraki** ve **Menü** çalışıyor.
- [ ] Sürüklerken sayfa kaymıyor ve yakınlaşmıyor.
- [ ] Hiçbir yazı kırpılmıyor.
- [ ] Çivili bir parçaya basınca parça kısa süre sarsılıyor.
- [ ] Çivi sayacı her hamlede kısa süre büyüyüp küçülüyor; çivi sökülünce parça parlıyor.
- [ ] Bomba fitili her hamlede aynı şekilde tıklıyor.

## 6. Güçlendiriciler (`br/?lv=12&debug=1`)

- [ ] Makas: bir eklem noktasına dokun, kenar düzleşir.
- [ ] Değnek: onay kartı çıkar; onaylayınca kenarlar değişir.
- [ ] Çekiç: bir parçaya dokun, parça çıkar.
- [ ] Saat: süreye 20 saniye eklenir.

## 7. Genel izlenim

- Efektler (parçaların gelişi, parlama, sarsılma) göze batıyor mu, oyunu yavaşlatıyor mu?
- L40'ta (`?lv=40&debug=1`) sürüklerken FPS. Hedef 60 (en az 55).

---

## Bu build'de olmayanlar (sonraki build'e)

- "Resmi tamamladın" bitiş kartı (G1-b) — Build 2 yayınlanırsa bu listeye eklenecek.
