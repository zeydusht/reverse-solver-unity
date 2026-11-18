# Titreşim haritası (PM_86 §4)

Kodla aynı tutulur (`Presentation/Ui/Haptics.cs` ve aşağıdaki yerler).

**Kurallar:**
- 50 ms aralık: hafif titreşimler 50 ms'den sık çalmaz. Heavy, Success ve Error hiç yutulmaz.
- Ayarlar'daki **Titreşim** anahtarı kapalıysa hiçbiri çalmaz.
- Takılma titreşiminin kendi anahtarı var (`&takilmatitresim=`).

**Platformlar:**
- iOS: gerçek dokunsal geri bildirim (`Plugins/iOS/RSHaptics.mm`).
- Web: `navigator.vibrate`. Android Chrome'da titrer; iPhone Safari'de web titreşimi olmadığı için sessizce yok sayılır. Select web'de hiç çalmaz.

| An | Titreşim | Kodda |
|---|---|---|
| Her düğmeye basış (menü, kart, kazanma, kayıp, devam paneli, alt çubuk) | Select + tık sesi | `Haptics.Button()`: GameRoot (menü dokunuşu, kart, kayıp, devam), P21WinView.Sink |
| Parçayı tutma (sürükleme başı) | Select | GameRoot.BeginGesture |
| Sandık 1. dokunuş | Light | P21WinView.PressChest83 |
| Sandık 2. dokunuş | Medium | aynı |
| Sandık açılış | Heavy + 0,1 sn sonra Success | aynı (`Haptics.PlayLater`) |
| Ödül parası sayaca düştükçe | Light (5–8 para, 0,1 sn aralıkla) | P21WinView.TickChest83 |
| Booster yuvasına düşüş | Medium | aynı |
| Booster seçimi | Select | GameRoot.Boosters21 |
| Çekiç vuruşu | Medium | BoosterFx74.Hammer / HammerSlow |
| Makas kesişi | Medium | BoosterFx74.Scissors / ScissorsSlow |
| Değnek dönüşümü | Light × 3 dalga | BoosterFx74.Wand |
| Saat donması | Medium | BoosterFx74 (saat teması) |
| Kayıp / süre doldu | Error | GameRoot.OnSessionEvent (Finished, kazanma ve bomba dışı) |
| Devam satın alma (altınla) | Success | GameRoot.ResolveContinue |
| Reklam ödülü (devam, çift altın, booster, hediye) | Success | Ads.ShowReward |
| Kont mührü (çok zor bölüm başı şeridi) | Heavy | GameRoot.Sl82.TickHardRibbon82 |
| Parça çıkışı | Light (bomba parçası Medium) | GameRoot.ExitSound (PM_45/46, aynen) |
| Kilit kırılması | Light | aynen |
| Takılma | Jam (çok kısa, zayıf) | aynen |
| Bomba patlaması | Heavy | aynen |
| Kazanma (son parça) | Success | aynen |
