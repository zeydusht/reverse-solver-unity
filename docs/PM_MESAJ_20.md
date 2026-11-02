# PM → Claude Code (29 Ekim 2026, 02:40) — Zincir görünürlüğü

Zeyd şu an `deneme1` adıyla 40 bölümü oynayıp puanlıyor. Bitene kadar önizlemeyi YAYINLAMA; oyunu ortasında değiştirmeyelim. "Bitti" diyene kadar yalnız yerelde çalış.

## Sorun

Resimli tahtada zincir neredeyse görünmüyor. Halkalar ince ve küçük, gri rengi de detaylı resmin içinde kayboluyor.

## Yap

1. **Hemen (kodla, geçici):**
   - Zincir kalınlığını yaklaşık 2 katına çıkar.
   - Halkaları koyu lacivertle (#0B2A36) boya.
   - Çevresine 3 px krem (#F8EBD0) dış kontur ekle.
   - Kilitlerin altında ve zincir boyunca hafif gölge olsun.
   - Bölüm 40, 24 ve SHOW1'de ekran görüntüsü al.
2. **Codex'in Task17'si gelince** (`Assets/Art/Gameplay/Task17/`):
   - `chain-link-v3` ve `chain-lock-v3` ile değiştir.
   - Halka döşemesi zincirin uzunluğuna göre kesintisiz olsun, dikiş görünmesin.
   - Sürüklerken kilit ve zincir parçayla birlikte hareket etsin. Bu daha önce düzeltildi; tekrar kontrol et.
3. **Katman sırası:** parça < zincir < engel < sayaç.
4. **Yayın:** Zeyd "bitti" dedikten sonra önizlemeyi `?set=redesign` adresine yayınla. OVERNIGHT.md başına 2 satır yaz.

iOS zip'ini hâlâ üretme.
