# PM_MESAJ_60 — Tutorial geri al + booster gösterimleri + haz efektleri + kazanma/sandık (p21)

Yalnız p21. Ağır işleri `codex exec` ile Codex CLI'ya ver; sen böl, kontrol et, test et, yayınla.

## 0. Önce
- `moves.input` sütunu Supabase'te **açıldı**. `INPUT_COL` bayrağını aç: her hamlede `flick` / `drag` gönder. Bir test oyuncusuyla (`TEST_PM60`) satır düştüğünü doğrula.
- PM_59 bitmediyse kaldığı yer önemli değil, aşağıdaki §1 geçerli.

## 1. Tutorial: 2×2 ve yeni 2. bölüm GERİ ALINIYOR
Zeyd oynadı, beğenmedi (el, metin kutusu, ilk iki bölüm). Yapılacak:
- 1. ve 2. bölüm **PM_59 öncesi hâline** döner (eski 1. bölüm ve 2. bölüm). 2×2 bölümü ve tutorial akışı kaldırılır (kod dalda kalabilir, bayrakla kapalı: `&tut2x2=1` ile açılabilsin).
- Eski 1. bölüme **hafif hikâye anlatımı:** ekranın altında, alt şeridin hemen üstünde açılan **sohbet kutusu** (krem, mavi çerçeve, köşede küçük Jigo büstü; metin canlı). Büyük el ve oklar yok.
  - Bölüm başı: kutu "Bu Jiggle'ları kurtaralım! Önü açık bir parçayı kaydır." ve önü açık **1–2 parça altın sarısı parlar** (yumuşak nabız). Oyuncu parlayanı çıkarınca bir sonraki önü açık parça parlar (en fazla 3 kez).
  - Son parçada: "**Bütün parçaları çıkarınca resim kurtulur!**" (Zeyd bu cümleyi beğendi).
  - Kutu dokununca kapanır; oyun beklemez, süre işler ama 1. bölümde kayıp yok.
- PM_59'dan **kalanlar**: "Bütün parçaları tahtadan çıkar!" bandı (ilk 3 bölüm), net engel kartı cümleleri, kilitli parçaya hamlede sallanma + uyarı. Bunlar kalsın.

## 2. Booster gösterimleri
Görseller: `T41/Boosters_FX`, `T42` (+`docs/art/task42/LISTE.md`). Zamanlama ve akış: `docs/art/booster ref/kareler/BOOSTER_REF_ANALIZ.md` (RM'den ölçüldü).
- **Ortak seçim modu:** booster'a dokun → tahta dışı ~%70 karartma, tahta aydınlık; üstte büyük ikon + Türkçe ad ("Mavi Çekiç!") + tek satır talimat; seçili tuş parlar ve kıvılcımlanır; aynı tuşa tekrar dokun = iptal (booster harcanmaz).
- **Çekiç (~1,0 sn):** tuştan kavisle hedefe uçar ve büyür (gölgeli) → kalkar → vurur → büyük patlama + tahta sarsıntısı (~0,1 sn, 4–6 pt) → parça seker, döner, düşer → çekiç geri seker ve dönerek ekrandan çıkar.
- **Makas (~1,0 sn):** iki parça arasındaki bağı seçtir; makas açılıp kapanarak gider, keser (kırpıntı), aşağı kayıp kaybolur. **Makasın kuralı** şu an ne yapıyorsa o (ne kestiğini raporla); görsel akış ona uysun.
- **Değnek (~1,5 sn):** seçim modunda tahta 4 bölgeye ayrılır; oyuncu bölge seçer → "Değiştir!" onayı → değnek uçar, bölgenin ortasına dokunur, **yalnız o bölge** karışır/değişir. Kural değişikliği: bölge dışı aynı kalır. Bölge çok küçük/boşsa seçilemez. Denge için raporda: eski (tüm tahta) ve yeni etkinin FunSim karşılaştırması.
- **Saat:** seçim modu yok; basınca Jigo animasyonu (TASK42 §2: düğmeyi bırak, saati al, kolu pencereden Süre paneline uzat, tut). Süre **durur**; Süre panelinin altında boşalan bar. Süre: şu anki saat booster'ının değeri (+20 sn) kadar **dondurma** (parametre). Bar bitince Jigo geri çekilir.

## 3. Haz (juice)
Görseller: `T43` (güçlü patlama ve engel efektleri), `T41/Juice`.
- Parça çıkışı: patlama + konfeti (parça rengi), kısa "pop" ölçek, hafif titreşim (mevcut harita).
- Engel kırılması (kilit açılması, zincir kopması): metal parça + kıvılcım.
- Arka arkaya hızlı çıkışlarda kombo rozeti (x2, x3) ve aferin rozetleri ("Harika!", "Süper!", "Mükemmel!"); fazla sık olmasın (en çok 3 hamlede bir).
- Ses: mevcut kütüphaneden uygun "pop", "puf", "tık"; yoksa CC0 ekle, lisansı `docs/sounds/LISANS.md`.

## 4. Kazanma ekranı ve sandık
Görseller: `T44` (onaylı).
- Kazanma: 3B "Harika!", büyük mutlu Jigo, bölmeli sandık ilerlemesi ("2 bölüm sonra sandık!"), "N Jiggle kurtarıldı!" + sağa yürüyen sıralı Jiggle'lar, **"Sonraki Seviye"** düğmesi. Geçiş hızlı (≤0,5 sn), araya uzun ekran koyma.
- Sandık: 3., 5., 10. bölüm ve sonra her 5 bölümde bir. "Sandık!" + "Sandığa dokun!" + el; 1. dokunuş sallanır, 2. dokunuş sert sallanır + ışık sızar, 3. dokunuş açılır. Ödüller tek tek büyük çıkar, ortada durur, yerlerine uçar (altın sayaca, booster tuşuna). Ödül: +100 altın + 1 rastgele booster (parametre).
- Durdur menüsü: **eski ortadaki panel kalır** (T43 sütunu kullanılmaz).

## Teslim
p21 güncel. Test bayrakları: `&booster=cekic|makas|degnek|saat` (seçim moduna doğrudan gir), `&kazan=1` (kazanma ekranı), `&sandik=1`. EditMode yeşil (Perf hariç), `verify_build` 4/4, PC FPS. GIF'ler: 4 booster, kombo, kazanma, sandık → `docs/art/task29/pm60/`. Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_60" eki, PROJE_DURUMU'na tek satır, commit + push.
