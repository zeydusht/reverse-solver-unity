# Titreşim ve ses haritası (PM_86 §4, PM_98 §4, PM_105 §3)

Kodla aynı tutulur: `Presentation/Ui/Haptics.cs`, `Presentation/Ui/UiSounds.cs` ve aşağıdaki yerler.

**Kurallar:**

- **50 ms aralık:** hafif titreşimler 50 ms'den sık çalmaz. Heavy, Success ve Error hiç yutulmaz.
- **Anahtarlar:** Ayarlar'daki **Titreşim** kapalıysa titreşimlerin hiçbiri çalmaz; **Ses** kapalıysa seslerin hiçbiri çalmaz.
- Takılma titreşiminin kendi anahtarı var (`&takilmatitresim=`).

**UI sesleri** (kayıtlı CC0; oyunda duyulan düzey, en yüksek 400 ms):

| Ses | Dosya | Düzey | Kaynak |
|---|---|---|---|
| düğme toku | `pm104/ui-tok-a1` (tek kayıt) | −16 LUFS | Kenney Impact, yumuşak ahşap (PM_104; Zeyd Durdur'da beğendi) |
| anahtar | `pm98/ui-toggle1-2` | −14 | Kenney gerçek düğme |
| panel açılış / kapanış vuş | `pm98/ui-open1-2`, `ui-close1-2` | −17 (tokun altında −26) | el kaydı swish; kapanış aynı sesin tersi |
| pasif tok | `pm98/ui-dull1-2` | −15 | Kenney tahta vuruşu |

- **PM_105 §3:** bütün düğmeler **aynı tok**u çalar: tek dosya (`ui-tok-a1`), rastgele perde yok, hep aynı düzey. Eski "çıt" (tık b: `pm98/ui-tick-*`, `pm95/tik-*`; dört kayıt, ±%4 perde, −1 dB tepe) projeden silindi; hiçbir kod yolu onu çalamaz. `&sesdugme=a…g` tok adayını değiştirir; seçilen her düğmede çalar.
- Müzik −18 LUFS'luk dosyanın 0,35'i, yani yaklaşık −27 LUFS duyulur.
- Bir basışta tek ses çalar: anahtar ve pasif tok, düz tokun yerine geçer.
- Açılış ve kapanış vuşu tokun yanında 9 dB kısık çalar (sekme kayışı 12 dB kısık). Bir panel aynı karede kapanıp yeniden açılırsa (anahtar sonrası yeniden kurulum) ikisi de çalmaz.
- **İstisnalar (kendi sesi):** sandık açılışı, ödül uçuşu ve satın alma onayında ayrı bir ses yok; dokunuşları tok çalar, onay Success titreşimidir.

**Duraklatma (PM_98 §3):**

- Durdur ya da Ayarlar paneli açıkken müzik %35'e iner, oyunun kendi sesleri durur (`AudioListener.pause`). UI sesleri ayrı kaynaktan çaldığı için duyulur.
- Panel kapanınca müzik 0,3 sn'de eski düzeyine döner.
- Durum her karede panellerin kendisinden okunur: `GameRoot.LateUpdate`.

**Platformlar:**

- **iOS:** gerçek dokunsal geri bildirim (`Plugins/iOS/RSHaptics.mm`).
- **Web:** `navigator.vibrate`.
  - Android Chrome'da titrer.
  - iPhone Safari'de web titreşimi yok; sessizce yok sayılır. Select web'de hiç çalmaz.
  - Ayarlar'da ve Durdur panelinde titreşim anahtarının altında yalnız web'de "Titreşim uygulamada çalışır" yazar.

**Denetim:** `OpeningFlowTests.EveryTapHasSoundAndTap` (PlayMode; PM_105: çalan her dosya kaydedilir, `ui-tick` hiç çalmaz) ve `Tools/webkit_buttons.js` (WebKit; menü, sekmeler, bölüm, Durdur, süre doldu paneli, kazanma, sandık; çalan dosya listesi).

- **Taranan ekranlar:** menü, Koleksiyon, Jiggle'lar, Ödüller, Ayarlar, bölüm, Durdur, kazanma.
- **Yöntem:** her ekranda 9 × 19 noktalık bir dokunuş ızgarası. Görünen durumu değiştiren her dokunuş (`GameRoot.UiState98`) hem bir UI sesi çalmalı hem titreşim istemeli.
- **Neden iOS'u da kapsar:** titreşim isteği (`Haptics.Asked`) iOS uygulamasının da geçtiği yoldur, platform koduna gitmeden önce çağrılır.
- Sessiz ya da titreşimsiz öğe listesi boş olmalı.

| An | Titreşim | Ses | Kodda |
|---|---|---|---|
| Her düğmeye basış (menü, kart, kazanma, kayıp, devam paneli) | Select | `ui-tok-a1` | `Haptics.Button()`: GameRoot (menü dokunuşu, kart, kart düğmesi, kayıp, devam), P21WinView.Sink |
| Alt sekmeler, Oyna, Koleksiyon kartları, kapatma (X) | Select | `ui-tok-a1` (panel açılıyorsa + `ui-open` −9 dB) | menüde her dokunuş `Haptics.Button()`; Koleksiyon kartı vuşla açılır |
| Durdur düğmesi | Select | `ui-tok-a1` + `ui-open` (−9 dB) | GameRoot.Press (`BarAction.Pause`) |
| Devam / Tekrar / Menü (Durdur paneli) | Select | `ui-tok-a1` + `ui-close` (−9 dB) | kart düğmesi, CloseCard |
| Ses / Müzik / Titreşim / Dil anahtarı | Select | `ui-toggle` | `Toggle98`: Durdur paneli, Ayarlar (menü ve kart) |
| Ayarlar açılış / kapanış | Select | `ui-tok-a1` + `ui-open` / `ui-close` (−9 dB) | OpenSettings, SettingsAction("close") |
| Profil kartı, bölüm listesi, tablo ayrıntısı açılış / kapanış | Select | `ui-tok-a1` + `ui-open` / `ui-close` (−9 dB) | OpenPlayerProfile / ProfileAction, OpenList, PaintingDetail |
| Booster seçimi | Select | `ui-tok-a1` | GameRoot.Press (bar), SelectBooster21 |
| Kilitli booster | Select | `ui-dull` | aynı (`LockedNotice`) |
| Altın yetmeyen devam | Select | `ui-dull` | GameRoot.ResolveContinue |
| HUD: bölüm listesi, yeniden başlat | Select | `ui-tok-a1` | GameRoot.Press |
| Kazanma: Sonraki / Menü | Select | `ui-tok-a1` | P21WinView.Sink |
| Kazanma: reklam (2× altın) | Select | `ui-tok-a1` | P21WinView.Press |
| Sandık dokunuşları | Light, Medium, Heavy (açılış + 0,1 sn sonra Success) | `ui-tok-a1` | P21WinView.PressChest83 |
| Sandık açıkken dokunuş (ödüller hızlanır) | Select | `ui-tok-a1` | aynı |
| Parayı ödül sayacına düşürme | Light (5–8 para, 0,1 sn aralıkla) | — | P21WinView.TickChest83 |
| Booster yuvasına düşüş | Medium | — | aynı |
| Parçayı tutma (sürükleme başı) | Select | — (oyun hareketi) | GameRoot.BeginGesture |
| Çekiç vuruşu | Medium | efekt | BoosterFx74.Hammer / HammerSlow |
| Makas kesişi | Medium | efekt | BoosterFx74.Scissors / ScissorsSlow |
| Değnek dönüşümü | Light × 3 dalga | efekt | BoosterFx74.Wand |
| Saat donması | Medium | efekt | BoosterFx74 (saat teması) |
| Kayıp / süre doldu | Error | `loss` (kayıp); müzik %40 | GameRoot.OnSessionEvent (Finished, kazanma ve bomba dışı) |
| Devam satın alma (altınla) | Success | `ui-tok-a1` (süre doldu panelinde altınla ve reklamla devam da) | GameRoot.ResolveContinue |
| Reklam ödülü (devam, çift altın, booster, hediye) | Success | — | Ads.ShowReward |
| Kont mührü (çok zor bölüm başı şeridi) | Heavy | — | GameRoot.Sl82.TickHardRibbon82 |
| Parça çıkışı | Light (bomba parçası Medium) | `pm100/cikis-f1-4` (bomba parçası `pm100/bomba-a1`) | GameRoot.ExitSound (PM_45/46, aynen) |
| Kilit açılması | Light | `pm105/kilitac-a1-2` (anahtar + düşüş; yoksa `pm100/kilit-a1`) | aynen |
| Takılma | Jam (çok kısa, zayıf) | `pm102/carpma-a1-4`; kırmızı duvarda `pm105/duvar-a1-3`; zincirli parçada + `pm105/zincirger-a1-3` (−3 dB) | aynen |
| Kilitli parçaya dokunma | — (sallanma) | `pm105/kilitdokun-a1-3` | GameRoot.Press (`Clang`) |
| Zincirli parça sürüklenirken / sallanırken | — | `pm105/zincir-a1-4` (hıza göre sıklık ve düzey, durunca yok) | GameRoot.Rattle105 (`BoardView.ChainMotion`) |
| Bomba patlaması | Heavy | `pm105/patlama-a1`, 0,45 sn sonra `loss` | GameRoot.OnSessionEvent (Finished, bomba) |
| Kazanma (son parça) | Success | son parça, sonra fanfar (`Music/kazanma-g-marimba`) | aynen |
