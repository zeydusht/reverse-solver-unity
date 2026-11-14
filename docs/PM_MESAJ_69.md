# PM_MESAJ_69: Zorluk özellikleri v2 (motor) + pt1 replay. Bölüm DEĞİŞTİRME.

Zeyd onayı (14 Kasım). Kodu kendin yaz. Bu görev yalnız **ölçüm**: bölüm, p22 ya da yayın değişmez. Önce `docs/` commit.

Arka plan: `docs/analiz/ZORLUK_MODELI_v2.md`. Özet:
- pt1 zorluğunun %73'ünü **bomba sıkılığı + bombanın erken gitmesi + sn/hamle** açıklıyor.
- `level_features.fuse_slack_*` yanlış ölçülüyor. S50 kayıtlı pay 1, oyuncular ≥12 payla alıyor; S17 kayıtlı 2, gerçek 7.
- Asıl eksik: **bomba doğal oynayışta zamanında çıkıyor mu.** Örnek: S31 ile S32 aynı pay/boyut, %72'ye %11.

**Zeyd'in zorluk çerçevesi.** İki ayrı zorluk ekseni var, ölçümler bunu ayırsın:
- **A. Kaçırma (bomba):** tahtada **çok boş hamle** varken bomba tehlikeli. Oyuncu düşünmeden görünen parçaları çıkarır, fitili yakar. Pay 2 + bol boş hamle ≈ kesin kayıp. Ölçü: fitil penceresindeki **çeldirici** (bombanın yolunda olmayan açık hamle) baskısı.
- **B. Bulma (kilit, zincir, duvar):** bu engeller seçenekleri **1–2'ye düşürür** ve doğru hamleyi gizler.
  - Kilit tutuyor, oyuncu kilidi açacak parçayı göremiyor.
  - Zincir iki ayrı çıkabilecek parçayı birbirine bağlıyor.
  - Duvar boş görünen parçanın tek yönünü kapatıyor.
  
  Bunlar iyi zorluk ve görsel olarak da güzel.
  
  pt1'de kaybettirmediler, çünkü süre boldu. Ama takılmayı ve hamle başı süreyi artırdılar: kazananlarda takılma/çıkış ve sn/çıkış için R² 0,70 ve 0,55; en güçlü etki duvarda. Ölçü: **dar anlar** (open_n ≤ 2) ve o anda tek doğru hamlenin **gizli** olması.

## 1. Her bölüm için yeni özellikler (bütün havuz: 1–40 + kullanılmayan bütün bölümler, her sürüm)
Motorun gerçek kurallarıyla (DragModel, Probe, kilit, zincir, duvar, bomba sayacı: her çıkış −1) hesapla:
1. `slack_true[b]`: her bomba için fitil − (o bombalı parçayı çıkarmak için gereken **en az** çıkış). BFS/A* ile, bombayı önceliklendiren çözücü. `slack_true_min`, `slack_true_mean`.
2. `choke_n`: pay_true ≤ 1 olan bomba sayısı (boğaz sayısı).
3. **Doğal oyuncu Monte Carlo** (her bölüm 2000 oyun, sabit tohum), üç politika:
   - `rand`: o an çıkabilen parçalardan rastgele.
   - `greedy_edge`: kenara en yakın / en kısa yol önce.
   - `bomb_aware`: %50 olasılıkla bombaya giden yoldaki parçayı seç, yoksa rastgele.
   Her politika için `p_survive` (hiç bomba patlamadan bitirme), `nat_slack_med` (bombalı parçanın çıktığı an ile fitil arasındaki fark, medyan) ve `first_choke_ex` (ilk patlamanın kaçıncı çıkışta olduğu).
4. **Açık hamle profili:** çözüm boyunca her adımda
   - `open_n`: çıkabilen parça sayısı;
   - `open_on_path`: bunların kaçı en yakın bombanın yolunda.
   Özet: `open_min`, `open_mean`, `bottleneck_n` (open_n = 1 olan adım sayısı), `distractor_ratio` (ilk bomba çıkana kadar ortalama (open − on_path)/open).
   - **Çeldirici baskısı (eksen A):** her bomba için, fitil penceresi boyunca yoldaki adım başına ortalama çeldirici sayısı, yani `distract_per_step[b]`. Ayrıca `distract_vs_slack[b]` = çeldirici ortalaması / (pay_true + 1).
   - **Dar an (eksen B):** `narrow_n`, yani doğal oyuncu yolunda open_n ≤ 2 olan adım sayısı, ve `narrow_hidden_n`, bu anlarda tek geçerli hamle görsel olarak serbest görünmüyorsa. "Görsel serbest" = en az bir kenarı açık ve kilit/zincir/duvar yok.
   - **Engel payı:** her adımda çıkamayan her parça için onu tutan neden: kilit / zincir / duvar / başka parça / tırnak. Özet: her engelin `blocked_share_lock`, `blocked_share_chain`, `blocked_share_wall` (o engelin kapattığı hamle oranı) ve `narrow_by_lock|chain|wall` (dar anların hangi engelden kaynaklandığı).
5. **Takılma tuzağı:** `jam_bait_n`, yani bir yönde açık kenarı olup o yönde takılan parça sayısı (başta ve çözüm ortasında). `dir_ambiguity` = açık yönü >1 görünen ama yalnız 1'i geçerli parça oranı.
6. Var olanlar da kalsın: `pieces`, `locks`, `chains`, `walls`, `timer_s`, `sec_per_move`, `open_edge_pct`.

Çıktı:
- Supabase'de yeni tablo `level_features_v2` (level_id, level_version, set_name, slot ve yukarıdakiler), migration ile.
- Aynısı `docs/analiz/v2/level_features_v2.csv` olarak.
- `level_features`'a dokunma.

## 2. pt1 replay
`moves` tablosundaki pt1 hamlelerini (cohort pt1, bütün denemeler) motorda baştan oynat. Her deneme için:
- `replay_ok`: aynı sonuç ve aynı çıkış sayısı mı (değilse nedeni).
- Bomba ölümlerinde `off_path_exits` (patlayan bombanın yolunda olmayan çıkış sayısı) ve `on_path_exits`.
- Her takılma için neden: kilit / zincir / tırnak / duvar / başka parça.
- `open_n` ortalaması (oyuncunun gördüğü açık hamle sayısı).
- **Hamle düzeyi tablo** `replay_pt1_steps`. Her hamle için:
  - `open_n` ve `on_path_n`,
  - dar an mı, gizli mi,
  - o adımda seçenekleri kısan engel(ler),
  - oyuncunun `think_ms` değeri ve bir sonraki çıkışa kadar takılma sayısı.
  
  Böylece "kilit/zincir/duvar dar an yaratınca insan kaç sn düşünüyor, kaç kez takılıyor" ölçülecek. Eksen B'nin gerçek maliyeti bu olacak.

Çıktı: tablo `replay_pt1` (player, level_id, level_version, attempt + yukarıdakiler) ve `docs/analiz/v2/replay_pt1.csv`. Replay uyum oranını raporla (hedef ≥ %95).

## 3. Kontrol
- S17, S18, S31, S32, S47, S08, S50 için tabloyu rapora yaz.
- Beklenti:
  - S31 ve S17'de `p_survive` ve `nat_slack_med` yüksek; S32, S18, S47 ve S08'de düşük.
  - S47 ve S08'de (engelsiz, açık tahta) `distract_per_step` yüksek.
  - Z25, S29 ve Z23 (çok duvar/kilit) `narrow_hidden_n` yüksek.
  
  Değilse söyle, uydurma.
- EditMode testi: `slack_true` küçük elle kurulmuş 2 tahtada doğru.

## Teslim
Yeni tablolar ve CSV'ler, rapor `docs/analiz/v2/PM69_RAPOR.md` (özellik tanımları, süreler, replay uyum oranı, kontrol tablosu), `PROJE_DURUMU.md`'ye tek satır, commit + push. Sonra **dur**. Modeli PM kuracak.
