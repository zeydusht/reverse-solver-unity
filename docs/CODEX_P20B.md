# Codex görevi P20-B — Jiggles, Başarılar, Profil + yerel istatistik deposu

Kaynak istek: `docs/PM_MESAJ_26.md` bölüm 4, 5, 6 ve "Veri kaydı". A parçası (`docs/CODEX_P20A.md`) yapıldı: `Presentation/Ui/MenuTabs.cs` (MenuUi, MenuTabs, MenuScroll), `CollectionScreen.cs`, `LevelsMap.cs`, `MenuSettings.cs`, `MenuPainting.cs` ve `GameRoot` bağlantıları. Aynı yardımcıları ve düzeni kullan. Onaylı görünüm: `Assets/Art/Menu/Task19D/03-jiggles.png, 04-basarilar.png, 05-profil.png`; parçalar `t20-*` (manifest/HANDOFF: `Assets/Art/Menu/Task20/`).

## Dokunma
`Assets/_Game/Levels/*`, `Core/Rules`, `Core/Telemetry`, gönderilen satırlar, `Tools/verify_build.js`, kazanma kartı ve kazanma akışı (ek bildirim yok). Commit atma. `Library/ Logs/ Builds/` okuma.

## 1. İstatistik deposu (Core, saf C#)
- `Core/Persist/PlayStats.cs`: bölüm başına (level id) `Wins`, `FirstTryWin` (bool), `BestWinSeconds` (int?), `MinJamsOnWin` (int?), `MinLeftOnWin` (int?), `HalfLeftWin` (bool: kalan ≥ süre/2 ile kazanıldı mı), ve toplamlar `TotalSeconds` (bütün denemelerin süresi), `SumLeftOnWins`, `WinCount`. `Add(AttemptRecord r, int timerSeconds)` ile güncellenir; `IKeyValueStore`'a `SaveData.Key("stats")` altında JSON (mevcut `JsonWriter` + basit okuyucu; Core'da JSON okuyucu yoksa düz `id:alan=değer;…` biçimi yeter) yazılır. Yalnız win/bomb/time sonuçları sayılır (quit süreye eklenir, galibiyete değil).
- `GameRoot`'ta deneme bittiğinde (`OnEvent`, `_telemetry.Finished` çağrılan yer) `_stats.Add(record, timer)` + kaydet. Debug `?won=N` modunda kaydetme.
- Eski kayıtlar: `FirstTryWin` yoksa `Won && SaveData.LossesOf(id) == 0 && AttemptsOf(id) == 1` say. Süre/sıkışma geçmişi yoksa bilinmiyor say (gösterme).
- EditMode testi: `Tests/EditMode/PlayStatsTests.cs` (ekleme, ilk deneme, en iyi süre, kaydet/yükle).
- Koleksiyon detayındaki "· 0:42" artık `BestWinSeconds`'tan (A'da bırakılan `bestSeconds` parametresi).

## 2. Jiggles sekmesi
- Büyük sayaç: kurtarılan toplam Jiggle (mevcut `SavedJiggles()` ile aynı sayı).
- Raf `t20-jiggles-shelf` + 5 ayakta Jiggle (`t20-jiggle-{blue,pink,red,green,purple}-standing`, pivot alt) + her birinin `t20-color-count-pill` sayacı. Renk kuralı: kazanılmış her bölümün parça sayısı `(slot−1) mod 5` rengine (mavi, pembe, kırmızı, yeşil, mor); slot = set içindeki sıra+1. Kazanılmışlardan hesaplanır (geriye dönük kendiliğinden doğru).
- **Avatarım:** 5 avatar (`t20-avatar-*` + `t20-avatar-ring[-selected]`), dokununca `Art.AvatarIndex` değişir, profil kutusu da değişir. Eski "avatar" kartı (profil kutusuna dokununca açılan) kaldırılır; profil kutusu artık Profil kartını açar.
- **Kilometre taşları** 500 / 1.000 / 2.000: `t20-milestone-*`, ulaşılınca `t20-icon-green-check`. Ödül yok.

## 3. Başarılar sekmesi
- Sayaç "X / 9". 9 satır `t20-achievement-tile` (kaydırılır), madalyon `t20-achievement-*`, ad + koşul metni (TR/EN, `Texts.cs`), ilerleme `t20-progress-track/fill` + "x/n", tamamlanınca `t20-icon-green-check`, ilerleme 0 ise kilitli görünüm (soluk).
- Koşullar (PM tablosu): İlk Tablo 1 kazanma; Koleksiyoncu 10; Müze Sahibi 40; Bomba İmha Uzmanı: bombalı bölümlerden ilk denemede kazanılan 10 (`LevelStats`/seviyenin bomba sayısı > 0); Soğukkanlı: kalan < 3 sn ile bir kazanma; Hatasız: 0 jam ile bir kazanma; Hızlı Parmaklar: `HalfLeftWin` olan 10 bölüm; Zincir Kıran: kazanılmış bölümlerdeki zincirli parça (birden çok hücreli zincir/`chain` verisi seviyede nasıl tutuluyorsa) toplamı 25; Baron'a Kafa Tut: 7 zor slotun (19, 23, 25, 30, 35, 36, 38; A'da nasıl okunduysa) hepsi kazanılmış.
- Hesap saf C#: `Core/Persist/Achievements.cs` (`Compute(levels, progress-benzeri arayüz, PlayStats) → 9 × (done, value, target)`), test `Tests/EditMode/AchievementsTests.cs` (0, 12 ve 40 bölüm senaryoları).
- Rozet: tamamlanan başarı sayısı − son görülen sayı (store'da `rs_ach_seen`); Başarılar sekmesi açılınca görülen = tamamlanan. `MenuTabs`'ın `TrophyBadge` parametresine bağla.

## 4. Profil kartı
- Profil kutusuna dokununca: `t20-modal-panel`, `t20-name-band` içinde isim + `t20-icon-pencil` (dokununca mevcut isim girişi: `GameRoot`'taki ad kapısı/`rs-name` akışı nasıl açılıyorsa), avatar diski.
- 6 istatistik (`t20-stat-tile`): Bölüm (şu anki/40 = `_progress.Next()+1` / sayı), Kurtarılan Jiggle, Toplam süre (dk, `TotalSeconds/60`), İlk denemede % (ilk denemede kazanılan / oynanan bölüm; oynanan = AttemptsOf>0), Başarı x/9, Ortalama kalan süre (sn, `SumLeftOnWins/WinCount`; yoksa "—").
- "Avatarı değiştir" → kartı kapat, Jiggles sekmesine geç.

## Bitince
- Unity'yi çalıştıramıyorsan derlemeyi bana bırak; yalnız sözdizimine dikkat et (C# 9, Unity 6).
- KISA rapor: değişen/yeni dosyalar, varsayımlar (zincir sayımı, zor slot okuma), eksik kalanlar.
