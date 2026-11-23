# PM_MESAJ_97: AI Level Design vizyonu: DENETİM ve durum haritası (yalnız okuma, oyun koduna dokunma)

**PM_96 bitmeden başlama.** Bu iş tamamen **okuma + rapor**. Oyun kodu, bölümler, Supabase, p23 ve `live/`: hiçbirine dokunma. Yayın yok. Yalnız `docs/ai-level-design/` altına dosya yazılır, sonra `docs/` commit + push.

Bitiş mesajı CLAUDE.md biçiminde, en altta.

## Bağlam
- Zeyd'in uzun vadeli vizyon belgesi: `docs/ai-level-design/AI_Level_Design_Vision.md` (PM kaydetti). Önce baştan sona oku.
- Bu belgedeki Phase 1–3'ün önemli kısmı zaten yapıldı ama dağınık:
  - PM_25 botlar + Monte Carlo;
  - PM_47/48 ajan döngüsü;
  - PM_69 zorluk özellikleri v2;
  - model v2 (IRT, pt1 kalibrasyonu, `docs/analiz/ZORLUK_MODELI_v2.md`);
  - PM_70 üretici + model;
  - PM_71 pt1 düzeltmesi;
  - PM_82 sl100 seçimi;
  - FunSim;
  - ZeydBot;
  - `moves.input` hamle kaydı (10 Kasım'dan beri).
- Amaç: vizyondaki her parçanın **şu an gerçekte ne durumda** olduğunu kanıtla ortaya koymak ve kalanlar için ölçülebilir, aşamalı bir plan çıkarmak. Varsayımla değil, koda ve veriye bakarak.

## Çıktı 1: `docs/ai-level-design/DURUM_HARITASI.md`
Vizyondaki her başlık için bir satır (A–E bileşenleri, Monte Carlo metrikleri, loss/reward, human feedback, overfitting, kalibrasyon, fun factor, taşınabilirlik):

| Vizyon parçası | Bugün var mı? (yok / kısmen / var) | Nerede (dosya, sınıf, araç) | Nasıl çalışıyor (1–2 cümle) | Kanıt (test, rapor, sayı) | Eksik |
|---|---|---|---|---|---|

Özellikle net cevapla:
1. **ZeydBot gerçekten öğrenilmiş bir model mi, yoksa kurallı/ağırlıklı bir algoritma mı?** Kodda ne var: parametreler nasıl elde edildi, eğitim adımı var mı?
2. **Monte Carlo metrikleri:** vizyondaki listeden (win, first-try win, ortalama hamle, near-miss, son hamlede kazanma, ölü uç, çözüm yolu çeşitliliği…) hangileri bugün ölçülüyor, hangileri yok?
3. **Model v2'nin gerçek isabeti:**
   - pt1'de tahmin ile gerçek (MAE, ±);
   - hangi bölüm tiplerinde yanılıyor.
4. **Hamle bazlı veri:**
   - `moves.input` ve hamle kayıtları Supabase'te ne kadar var (oyuncu ve hamle sayısı);
   - oyuncu davranış modeli (Beginner / Average / Expert) için yeterli mi?
   - **Okumak serbest, yazmak yasak.**
5. **Zeyd'in öznel geri bildirimleri** ("sıkıcı", "son hamlede çözdüm", "çok iyi"…) bir yerde kayıtlı mı? Reward model için kullanılabilir veri seti var mı?
6. **Bugünkü seçimin kalite fonksiyonu:**
   - sl100 seçimi aslında hangi "kalite fonksiyonuna" göre yapıldı;
   - vizyondaki Loss formülüne ne kadar yakın?

## Çıktı 2: `docs/ai-level-design/GELISIM_PLANI.md`
Kalan işler için aşamalı plan. **Her öneri için** (vizyon §14):
1. matematiksel temel;
2. algoritma;
3. gereken veri (bugün var mı, soft launch ile mi gelecek);
4. beklenen fayda (hangi metrik ne kadar iyileşir);
5. nasıl test edilir (train/val/test ayrımı, görülmemiş bölüm, gerçek oyuncu karşılaştırması).

Kurallar:
- **Sırf AI olsun diye neural network veya RL önerme.** Veri az: önce basit modeller (lojistik regresyon, IRT, gradient boosting, Bayesçi kalibrasyon). Her karmaşık yöntem için "neden daha basiti yetmez" yaz.
- **Overfitting ve reward hacking riskleri** her öneri için ayrı satır.
- **Sırala:**
  - (a) soft launch'tan önce yapılabilecek ve riski sıfır olanlar (yalnız ölçüm ve rapor);
  - (b) soft launch verisi (`sl1` kohortu, hamle kayıtları) gelince yapılacaklar;
  - (c) daha fazla veri gerektirenler.
- Her öneriye saat tahmini.
- **Soft launch'ı geciktirecek hiçbir şey önerme.** Yayından önceki tek iş, gerekiyorsa telemetride eksik bir alanın eklenmesi olabilir (ör. near-miss, son hamle). Onu da yalnız öner, yapma.

## Teslim
- İki dosya `docs/ai-level-design/` altında. `docs/` commit + push. Oyun kodunda değişiklik yok.
- Son mesaj kısa: en önemli 5 bulgu + (a) listesinden ilk 3 öneri.
