# PM_107 §D4: altın (coin) görseli (Codex görevi)

Sen bir oyun arayüzü sanatçısısın. Reverse Solver'ın (iPhone, dikey) altın parasını yeniden çiziyorsun. Yalnız görsel üret; kod yazma, oyun dosyalarına dokunma. Bütün çıktılar bu klasöre (`docs/art/pm107/codex/altin/`).

## Bugünkü durum
- Menüde: `docs/art/pm107/once_sonra/menu_once.png`, sol üstte isim, ortada yıldızlı altın + sayı bir kutuda.
- Kazanmada: `docs/art/pm107/once_sonra/kazanma_once.png`, "+50" yanında küçük altın.
- Devam panelinde: `docs/art/pm107/once_sonra/sure_doldu_once.png`, yeşil düğmede altın + fiyat, altında "999999 altın".
- Bugünkü dosyalar (her yerde başka bir altın, birleşmesi isteniyor):
  - `Assets/_Game/Presentation/Resources/Art/yeni/T50/win/coin.png`
  - `Assets/_Game/Presentation/Resources/Art/yeni/devam_v2/continue-coin.png`
  - `Assets/_Game/Presentation/Resources/Art/yeni/win21/Reward/Flight/coin.png`

## Kalite referansı (kopyalama yok)
`docs/art/pm107/ref/bo_a_7.jpg` ve `bo_b_6.jpg`: Block Out'un sol üstündeki altın.

- Parlak, kalın kenarlı, okunur bir para.
- Sayı yanında düz yazı: **sayaç kutusu ve "+" yok**.

Renk ve tasarım **bizim**: sıcak altın (#E3B54E, #F6D36B, koyu kenar #8A6420), "old money" zarafeti. Ortada sade bir işaret olabilir: yıldız, ya da bizim yapboz parçası silueti. Karikatür ama sade, 3B ve kalın.

## İstenen (PM_74 döngüsü)
1. **En az 3 fikir** (A, B, C), gerçekten farklı. Örneğin:
   - A: klasik yıldızlı, kalın kenar;
   - B: yapboz parçası kabartmalı;
   - C: hafif eğik 3B, kalın yan kenar görünür.

   Her fikir için:
   - `X_coin.png`: 512×512, saydam. 24 pt'de de okunur olmalı: kalın koyu dış kenar, net parıltı.
   - `X_onizleme.png`: üç ekran görüntüsünün her birine yeni altını yerleştirip yan yana dizdiğin tek görsel. Menüdeki sayaç kutusu ve "+" kaldırılmış; altın + düz sayı, kalın krem yazı ve koyu kontur.
2. **Puanla:** 1–5 puan, `PUANLAR.md`. Ölçütler:
   - 24 pt'de okunurluk;
   - parlaklık ve kalite (Block Out düzeyi);
   - bizim palete ve stile uyum;
   - sadelik.

   **Geçme çizgisi:** her ölçütte en az 4. Geçmeyeni bağla, nedenini yaz.
3. **En iyisini iyileştir:** en az bir tur daha. Kenar kiri, saydamlık ve renk sapmasını gider. `final_coin.png` (512×512), `final_coin_64.png` (64×64, küçükte okunurluk için elle keskinleştirilmiş) ve `final_onizleme.png`.
4. **Teknik kontroller:** `KONTROL.md`'ye yaz.
   - **Arka plan:** Saydam üretim yapılamıyorsa düz #FF00FF zeminde üret, Pillow ile ayıkla, magenta piksel kalmadığını say.
   - **Merkez:** Para merkezde mi? Saydam olmayan alanın sınır kutusu ve merkezi.
   - **Boyut:** Boyutlar doğru mu?
5. Kısa bir `SECENEKLER.md` yaz: her fikir 2 satır, hangisi neden seçildi.

İstemlerini `uretim_promptlari.json`'a kaydet. 25 dakikada bitir.
