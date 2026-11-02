# PM → Claude Code (25 Ekim 2026, 01:20) — Önizleme 2 geri bildirimi

Zeyd önizleme 2'yi oynadı. Ona soru sorma; belirsiz kalan yerde makul olanı seç ve raporla. Token tasarrufu zorunlu.

## A) Şimdi düzelt (sanat beklemeden)

1. **Ana menü sadeleşsin.** Koridor arka planının üstündeki şu öğeleri kaldır:
   - "Reverse Solver" başlığı
   - "Her parçayı doğru yönde sürükleyip tahtayı boşalt" açıklaması
   - gri-mavi yarı saydam panel
   - sarı amblem

   Ekranda koridor ve tek bir Oyna tuşu kalsın.
2. **Oyna tuşu** Dream Games / Royal Match menüsündeki gibi olsun:
   - Ekranın alt kısmına, alt güvenli alanın hemen üstüne yerleştir.
   - Şimdikinden dar ve uzun yap; büyük ve tombul görünsün. Yaklaşık 200×72 pt, yazı "Level X".
   - Codex'in yeni tuşu gelene kadar mevcut A tuşunu bu ölçülerle kullan.
3. **Tutorial eli:** İki el üst üste çıkıyor.
   - Senin vektör elini (ok + parlama) tamamen kaldır; yalnız Codex'in çizdiği el kalsın.
   - Çizilmiş el yatay duruyor; **dikey** dursun (parmak yukarıyı gösterir, bilek aşağıda). Döndürme yerine Codex'in dikey export'u gelecek. Gelene kadar sprite'ı 90° döndür.
4. **Eksik 6 ekranı bağla.** Zeyd #20–25'i onayladı: bölüm listesi, duraklatma/çıkış, booster onay, booster tanıtım, isim sorma, yükleme ekranı. Kaynaklar: `docs/art/ASSET_IHTIYAC.md` ve `HANDOFF.md`. Codex bunları SVG olarak verdi; PNG'ye dönüştürmek gerekirse bunu Codex'e görev olarak ver.
5. **Önizleme 3'ü** `?set=redesign` adresine yayınla. Kök adres değişmesin. BACKLOG'u kısa güncelle.

## B) Zeyd'in kararı: oyundaki her görsel Codex'in olacak

Kaynak kodla çizilmiş (vektör, placeholder) hiçbir görsel kalmayacak. Bunun için:

1. **UI envanteri** (kısa tut, ~30 dk). Oyunda kodla çizilen her görsel öğeyi `docs/art/ASSET_IHTIYAC.md`'ye #26 ve sonrası olarak ekle. Her satırda şunlar olsun:
   - öğe adı
   - hangi ekranda
   - kod dosyası
   - ölçü (pt ve @3x px)
   - 9-slice gerekip gerekmediği

   Envantere en az şunlar girmeli:
   - üst bar: level, hamle ve süre panelleri
   - booster yuvası ve kutusu
   - genel butonlar (birincil, ikincil, kapat)
   - kart ve popup zeminleri
   - **oyun ekranı arka planı**
   - zincir ve duvarın tahtadaki çizimi
   - sayaç rozetleri (çivi ve bomba sayısı)
   - yıldızlar, ilerleme çubuğu
   - toast ve uyarı balonları
2. **Yapboz parçası "skin" formatı.** Parça şekilleri prosedürel üretiliyor; her parçanın tırnakları farklı. Bu yüzden Codex sabit parça PNG'leri çizemez. Codex'in çizebileceği bir format tanımla ve `docs/CODEX_GOREV.md`'ye yaz. Öneri:
   - gövde dokusu (tileable veya düz dolgu)
   - kenar kontur rengi ve kalınlığı
   - iç bevel, highlight ve gölge
   - seçili ve sürüklenen parça hali
   - kilitli (çivili) hali

   Sen bunu shader veya mesh stiliyle uygula. Kural: parça kenarları **belirgin** olmalı. Üstüne her bölümde farklı tablo resmi gelecek; mevcut resim kaplama sistemiyle uyumlu olmalı.
3. Envanteri bitirince Codex'e tek bir görev yaz: "#26+ hepsini çiz". Ölçüleri ve formatları sen ver.
4. Codex'in yeni görselleri geldikçe oyuna bağla ve her partiden sonra önizlemeyi yenile.

## C) Bu iş akışında Build 5 bekliyor

Görsel yenileme bitip Zeyd "tamam" demeden kök adrese yayın yapma.
