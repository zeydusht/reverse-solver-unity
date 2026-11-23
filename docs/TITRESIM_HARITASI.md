# Titreşim ve ses haritası (PM_86 §4, PM_98 §4)

Kodla aynı tutulur: `Presentation/Ui/Haptics.cs`, `Presentation/Ui/UiSounds.cs` ve aşağıdaki yerler.

**Kurallar:**

- **50 ms aralık:** hafif titreşimler 50 ms'den sık çalmaz. Heavy, Success ve Error hiç yutulmaz.
- **Anahtarlar:** Ayarlar'daki **Titreşim** kapalıysa titreşimlerin hiçbiri çalmaz; **Ses** kapalıysa seslerin hiçbiri çalmaz.
- Takılma titreşiminin kendi anahtarı var (`&takilmatitresim=`).

**UI sesleri** (PM_98, `Tools/make_ui98.py`, kayıtlı CC0; oyunda duyulan düzey, en yüksek 400 ms):

| Ses | Düzey | Kaynak |
|---|---|---|
| tık | −14 LUFS | `&sestik=` adayı, varsayılan b |
| anahtar | −14 | Kenney gerçek düğme |
| panel açılış / kapanış vuş | −17 | el kaydı swish; kapanış aynı sesin tersi |
| pasif tok | −15 | Kenney tahta vuruşu |

- Müzik −18 LUFS'luk dosyanın 0,35'i, yani yaklaşık −27 LUFS duyulur. Çıkış −19,5, çarpma −22, son parça −9, kazanma fanfarı −13.
- Bir basışta tek ses çalar: anahtar ve pasif tok, düz tıkın yerine geçer.
- Açılış ve kapanış vuşu tıkın yanında çalar. Bir panel aynı karede kapanıp yeniden açılırsa (anahtar sonrası yeniden kurulum) ikisi de çalmaz.

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

**Denetim:** `OpeningFlowTests.EveryTapHasSoundAndTap` (PlayMode).

- **Taranan ekranlar:** menü, Koleksiyon, Jiggle'lar, Ödüller, Ayarlar, bölüm, Durdur, kazanma.
- **Yöntem:** her ekranda 9 × 19 noktalık bir dokunuş ızgarası. Görünen durumu değiştiren her dokunuş (`GameRoot.UiState98`) hem bir UI sesi çalmalı hem titreşim istemeli.
- **Neden iOS'u da kapsar:** titreşim isteği (`Haptics.Asked`) iOS uygulamasının da geçtiği yoldur, platform koduna gitmeden önce çağrılır.
- Sessiz ya da titreşimsiz öğe listesi boş olmalı.

| An | Titreşim | Ses | Kodda |
|---|---|---|---|
| Her düğmeye basış (menü, kart, kazanma, kayıp, devam paneli) | Select | tık | `Haptics.Button()`: GameRoot (menü dokunuşu, kart, kart düğmesi, kayıp, devam), P21WinView.Sink |
| Alt sekmeler, Oyna, Koleksiyon kartları, kapatma (X) | Select | tık (panel açılıyorsa + vuş) | menüde her dokunuş `Haptics.Button()`; Koleksiyon kartı vuşla açılır |
| Durdur düğmesi | Select | tık + açılış vuşu | GameRoot.Press (`BarAction.Pause`) |
| Devam / Tekrar / Menü (Durdur paneli) | Select | tık + kapanış vuşu | kart düğmesi, CloseCard |
| Ses / Müzik / Titreşim / Dil anahtarı | Select | anahtar | `Toggle98`: Durdur paneli, Ayarlar (menü ve kart) |
| Ayarlar açılış / kapanış | Select | tık + vuş / ters vuş | OpenSettings, SettingsAction("close") |
| Profil kartı, bölüm listesi, tablo ayrıntısı açılış / kapanış | Select | tık + vuş / ters vuş | OpenPlayerProfile / ProfileAction, OpenList, PaintingDetail |
| Booster seçimi | Select | tık | GameRoot.Press (bar), SelectBooster21 |
| Kilitli booster | Select | pasif tok | aynı (`LockedNotice`) |
| Altın yetmeyen devam | Select | pasif tok | GameRoot.ResolveContinue |
| HUD: bölüm listesi, yeniden başlat | Select | tık | GameRoot.Press |
| Kazanma: Sonraki / Menü | Select | tık | P21WinView.Sink |
| Kazanma: reklam (2× altın) | Select | tık | P21WinView.Press |
| Sandık dokunuşları | Light, Medium, Heavy (açılış + 0,1 sn sonra Success) | tık | P21WinView.PressChest83 |
| Sandık açıkken dokunuş (ödüller hızlanır) | Select | tık | aynı |
| Parayı ödül sayacına düşürme | Light (5–8 para, 0,1 sn aralıkla) | — | P21WinView.TickChest83 |
| Booster yuvasına düşüş | Medium | — | aynı |
| Parçayı tutma (sürükleme başı) | Select | — (oyun hareketi) | GameRoot.BeginGesture |
| Çekiç vuruşu | Medium | efekt | BoosterFx74.Hammer / HammerSlow |
| Makas kesişi | Medium | efekt | BoosterFx74.Scissors / ScissorsSlow |
| Değnek dönüşümü | Light × 3 dalga | efekt | BoosterFx74.Wand |
| Saat donması | Medium | efekt | BoosterFx74 (saat teması) |
| Kayıp / süre doldu | Error | kayıp sesi; müzik %40 | GameRoot.OnSessionEvent (Finished, kazanma ve bomba dışı) |
| Devam satın alma (altınla) | Success | tık | GameRoot.ResolveContinue |
| Reklam ödülü (devam, çift altın, booster, hediye) | Success | — | Ads.ShowReward |
| Kont mührü (çok zor bölüm başı şeridi) | Heavy | — | GameRoot.Sl82.TickHardRibbon82 |
| Parça çıkışı | Light (bomba parçası Medium) | çıkış (−19,5) | GameRoot.ExitSound (PM_45/46, aynen) |
| Kilit kırılması | Light | kilit | aynen |
| Takılma | Jam (çok kısa, zayıf) | çarpma | aynen |
| Bomba patlaması | Heavy | — | aynen |
| Kazanma (son parça) | Success | son parça, sonra fanfar | aynen |
