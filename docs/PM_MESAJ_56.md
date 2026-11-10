# PM_MESAJ_56 — Commit + kilit/zincir/bomba ince ayarları + gerçek zincir fiziği (p21)

Yalnız p21. p20, kök ve pt1'e dokunma. Ağır işleri (kod, betik) `codex exec` ile Codex CLI'ya ver. Sen böl, kısa kontrol et, test et, yayınla.

## 0. Önce kaydet (ZORUNLU, ilk iş)
PM_53, PM_55 ve PM_54 kaynak değişikliklerini anlamlı mesajlarla **commit et** ve `art` dalını GitHub'a **it**. Sonra bu işe başla, bu işin sonunda da aynı şekilde commit + push.

## 1. Kilit
- Boyut **%20 büyüsün**: hücrenin ~%55'i → ~%66'sı. Numara okunur kalsın.
- **Düşüş yarıda kesilmesin:** açıldıktan sonra yerçekimiyle hızlanarak **ekranın en altına kadar** düşsün. **Alt menü şeridinin arkasından** geçsin (sorting: tahtanın ve parçaların önünde, alt şeridin arkasında) ve şeridin arkasında kaybolsun. Dönme (±25°) kalsın. Solma yok ya da yalnız şeridin arkasına girince. Süre mesafeye göre (~0,7–1,0 sn). Kırıntılar aynı.

## 2. Zincir bağlantı noktaları
Zincirli parçaların ortasındaki bağlantı noktaları (renkli halka + işaret) **%60 büyüsün**. İşaret (yıldız, damla, hilal, kalp, yaprak) okunur olsun. Zincir halkasıyla orantı bozulursa halka kalınlığını biraz artır.

## 3. Bomba nabzı
- 3-2-1 nabzı mükemmel, **dokunma**.
- ≥4'te nabız şu an çok az (%3). **Belirgin ama 3-2-1'den az** olsun: ~%5,5 ölçek, ~0,9 sn döngü, yumuşak (sinüs). Hamle anında küçük pump (%3).
- Böylece sayı azaldıkça nabız hızlanıp büyüyor hissi: ≥4 sakin ama görünür, 3-2-1 acil.

## 4. Zincir: gerçek fizik (en önemli iş)
Şimdiki parabol + yaylanma yerine **gerçek ip/zincir simülasyonu**:
- **Verlet ip:** zincir N düğüm (uzunluğa göre ~8–16), iki uç bağlantı noktalarına sabit. Yerçekimi aşağı (ekran), sönüm (damping ~0,98), mesafe kısıtı (5–8 iterasyon). Halka sprite'ları düğümler arasına teğete göre dizilir.
- **Bolluk = ipin gerçek boyu:** gergin mesafe × (1 + bolluk). Kısa zincirlerde bolluk daha fazla. **Çapraz zincirlerde bolluk belirgin artsın** (Zeyd: "mavide bolluk anlaşılmıyor"): çaprazda sarkma gözle net görülmeli. Hedef: duruşta sarkma yatayda ~0,3 hücre, çaprazda ~0,35 hücre, kısa zincirde en az 0,28 hücre. Parametreler tek yerde.
- **Parça tutulup sürüklenince:** bağlantı noktası parmakla hareket eder, zincir **gerçek zamanlı** tepki verir: ileri geri sallarsan zincir sallanır, savrulur, bırakınca sönümlenerek durulur. Kare hızından bağımsız (sabit adım, ör. 120 Hz alt adım).
- **Yanlış hamle sarsıntısı:** şimdiki sallanma iyi; fizikle yap: düğümlere kısa bir dürtü (impuls) ver, zincir doğal şekilde sallanıp dursun. Biraz daha belirgin.
- **Gerildiğinde:** parça zincir boyundan uzağa çekilemiyorsa (kural zaten engelliyor), zincir **gerilip düzleşsin**, hafif titresin. Bırakınca geri sarksın.
- Başka parçaların üstüne binmeme kuralı: sarkma aşağıdaki parçaya biniyorsa sorun değil (zincir parçaların üstünde çiziliyor). Ama zincir tahta çerçevesinin dışına çıkmasın.
- Performans: tahtada 4–6 zincir, telefonda 60 FPS korunmalı. `&fps=1` ile vitrinde ölç.
- Çekiçle kopma: ip ortadan ikiye ayrılır, iki yarı uçlarından sarkıp düşer, sonra kaybolur.

## Teslim
- p21 güncel. Vitrin `p21/?lv=SHOW_ART&lang=tr`.
- Kısa GIF'ler: kilit düşüşü (alt şeridin arkasına), zincir sürükleme-sallama, yanlış hamle sarsıntısı, çapraz zincir duruşu. `docs/art/task29/pm56/`.
- EditMode yeşil, `verify_build` 4/4, PC FPS. Kısa rapor `docs/art/task29/ENTEGRASYON_RAPOR.md`'ye "PM_56" eki. PROJE_DURUMU'na tek satır. Commit + push.
