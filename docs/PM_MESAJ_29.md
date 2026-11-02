# PM → Claude Code (2 Kasım 2026) — Davet öncesi son düzeltmeler (p20)

FTUE raporun çok iyiydi. PM ve Zeyd kararları aşağıda. Yayın `p20/`. p19, kök, `redesign.json`, bölümler ve iOS dokunulmaz.

**Sıra: 1 → 2 → 3 → 4.** 1 bitmeden davet gitmez.

## 1. VERİ: ilk denemenin kaybolması (en önemli)

PM Supabase'de yeni bir fonksiyon açtı: **`rpc/set_rating`** (migration 012).
- **Kullanım:** `POST /rest/v1/rpc/set_rating` gövde `{"p_row_id": "...", "p_zorluk": 7, "p_keyif": 1}`. Biri boş olabilir.
- **Ne yapar:** Yalnız boş alanları doldurur. Yalnız son 6 saatteki satıra uygulanır. Dönüş `true`/`false`.

Akış şöyle değişecek:
1. **Bölüm biter bitmez** (kazanma / bomba / süre / çıkış) `plays` satırını **hemen** gönder. `zorluk_puani` ve `keyif` boş (null) gider. 1–10 cevabını bekleme.
2. Oyuncu 1–10'u ya da 👍/👎'yu seçince `set_rating` çağır (aynı `row_id`).
3. Gönderilemeyen satırlar ve cevaplar yerel kuyrukta dursun, bir sonraki açılışta tekrar gitsin. `row_id` benzersiz olduğu için tekrar zarar vermez.

**Test:**
- (a) Kazan, 1–10'u cevaplamadan sekmeyi kapat → `plays` satırı var mı?
- (b) Kazan, 7 ver → satırda 7 mi?
- (c) Kayıp → 3 ver → satırda 3 mü?

Canlı `?c=ic` ile yap, Supabase'den sorgula, OVERNIGHT'a yapıştır.

## 2. Ev ekranı: kırmızı halı (PM_28'in 0. bölümü yapılmadı)

Gece dosyayı okuduğunda bu bölüm henüz eklenmemişti, şimdi geçerli. PM_28 bölüm 0'ı aynen uygula:
- Koridor kadrajı eski haline dönsün: lambri ve **kırmızı halı görünsün**. Tablo ekranın ~%40–45'i.
- Level tuşu halının üstünde dursun.
- Seçili sekme ikonu çubuktan en fazla %25 taşsın. 375×667'de tuşlar birbirine değmesin (≥ 12 pt).

## 3. FTUE kararları (raporun tablosuna göre)

| # | Karar |
|---|---|
| 1 | **Kazanma ve kayıp kartı:** En üstte büyük tuş "Sonraki bölüm" (kayıpta "Tekrar dene"). 1–10 satırı onun altında. "Geç" küçük, sade metin bağlantısı olarak en altta. |
| 2 | **1–10:** 2 satır × 5, her kutu ≥ 44 pt. Seçilince kutu dolar, 0,3 sn sonra kaydedilir (yanlış dokunuşu düzeltme fırsatı). |
| 3 | **Çivi metni tek cümle** (kart ve spotlight aynı). TR: "Çivili parçanın sayacı her hamlende 1 azalır. 0 olunca parça serbest kalır." EN: "A nailed piece's counter drops by 1 with every move. At 0, the piece is free." Kod kuralı buysa (her çıkış = 1 hamle) bu metin doğru; değilse koda uygun yaz ve OVERNIGHT'a not düş. |
| 4 | **"Jiggle'ları kurtar!" bölüm başı kartı TÜM bölümlerden kalkar** (Zeyd: "her levelden önce niye yazıyor, direkt başlasın"). Bölüm doğrudan başlar. Kart yalnız şu durumlarda kalır: eğitim/tanıtım bölümleri (1, 3, 6, 10, 14 ve booster bölümleri 5, 9, 15, 23) ve zor bölüm uyarısı (19, 23, 25, 30, 35, 36, 38). Başlıklar her yerde "Bölüm X" (TR) / "Level X" (EN). |
| 5 | **Kural kartı çizimi:** Kırmızı çarpı büyük ve net, oklar koyu ve opak (α 1). |
| 6 | **Eğitim oku:** Opak (α ≥ 0,9), daha kalın, elin önünde. Kısa olsa da ucundaki ok başı net görünsün. |
| 7 | **Bölüm 3–4 süreleri: dokunma.** Tahminler kilitli. pt1 verisi karar verecek. |
| 8 | **Yazı boyları:** Kart gövdeleri, sonuç istatistikleri ve tanıtım metinleri ≥ 13 pt. Başarılar koşul satırı sığmazsa 2 satıra insin, 12 pt altına küçülmesin. "Bölümler" etiketi 12 pt. |
| 9 | "Playtest sürümü" → TR "Deneme sürümü". |
| 10 | **Kayıp kartında yıldızları gizle.** |
| 11 | **Bölümler haritası:** Şu anki düğümün üstünde küçük "Oyna" balonu (yeşil). Haritadan tekrar oynamayı test et. |
| — | **Dokunma alanları:** X 44 pt, geri tuşu 44 pt, anahtarlar en az 44 pt yükseklikte dokunma alanı (görsel aynı kalabilir). |

## 4. Performans (önerilerinden 5'ini uygula)

1. `hud-time-pill(-urgent)`, `hud-moves-pill`: en büyük boyutu ekranda kullanılan boyutun 2 katına indir + sıkıştır.
2. `kit-overlay/flat/mask`: kullanım ölçüsüne indir + sıkıştır.
3. **Menü sahneleri sonradan insin** (`t20-scene-*`, `levels-path-repeat`): ilk karede gerekmiyor, tablolar gibi arka planda insin. İnene kadar düz renk zemin.
4. `Lilita` ve `level-cell-*` yeni sette kullanılmıyorsa build'den çıkar. Kök Build 5 ayrı build olduğu için etkilenmez. Emin değilsen bırak, not düş.
5. **Yükleme ekranı:** ilerleme yüzdesi + "İlk açılış biraz sürebilir…" yazısı (TR/EN).

wasm modül kapatma **yok** (riskli, sonra).

Uyguladıktan sonra aynı 3 ağ koşulunda yeniden ölç. Önce/sonra tablosu OVERNIGHT'a.

## Bitince
- OVERNIGHT.md başına: p20 linki + 1'in test sonuçları + önce/sonra yükleme süreleri + sorunlar.
- Sürüm etiketini yenile: `web-p20-2026-11-02`.
- 5 dk kuralı geçerli. Unity build'i hariç: o arka planda koşar.
