using ReverseSolver.Core;

namespace ReverseSolver.Presentation
{
    /* Player-facing text follows the language chosen in settings. */
    public static class Texts
    {
        public static string Win21 => Lang.T("Great!", "Harika!");
        public static string Chest21 => Lang.T("Chest!", "Sandık!");
        public static string TapChest21 => Lang.T("Tap the chest!", "Sandığa dokun!");
        public static string NextLevel21 => Lang.T("Next Level", "Sonraki Seviye");
        public static string ChestIn21(int n) => Lang.T($"Chest in {n} levels!", $"{n} bölüm sonra sandık!");
        public static string Rescued21(int n) => Lang.T($"{n} Jiggles rescued!", $"{n} Jiggle kurtarıldı!");

        public static string JuicePraise(int tier) => tier == 0 ? Lang.T("Great!", "Harika!")
            : tier == 1 ? Lang.T("Super!", "Süper!") : Lang.T("Perfect!", "Mükemmel!");
        public static string RegionShuffle => Lang.T("Shuffle!", "Değiştir!");
        static bool P21BoosterArt => Art.T54 && TelemetryMeta.Current?.Cohort == "p21";
        public static string ClockFreeze => Lang.T($"Freeze the timer for {GameSession.ClockBonusSeconds} seconds.", $"Süreyi {GameSession.ClockBonusSeconds} saniye dondurur.");
        public static string BoosterTitle21(Booster b) => b == Booster.Hammer ? Lang.T("Blue Hammer!", "Mavi Çekiç!")
            : b == Booster.Scissors ? Lang.T("Magic Scissors!", "Sihirli Makas!") : Lang.T("Magic Wand!", "Sihirli Değnek!");
        public static string BoosterHint21(Booster b) => b == Booster.Hammer ? Lang.T("Tap a piece to smash it", "Kırmak istediğin parçaya dokun")
            : b == Booster.Scissors ? Lang.T("Tap a tabbed seam to cut it", "Kesmek istediğin birleşime dokun")
            : Lang.T("Choose a region, then confirm", "Bir bölge seç, sonra onayla");
        public static string P21StoryStart => Lang.T("Let's rescue these Jiggles! Slide a piece with an open path.", "Bu Jiggle'ları kurtaralım! Önü açık bir parçayı kaydır.");
        public static string P21StoryLast => Lang.T("Clear every piece to free the picture!", "Bütün parçaları çıkarınca resim kurtulur!");
        public static string ObstacleIntro(string scene) => scene switch
        {
            "bomb" => Lang.T("Clear the bomb piece <b>before</b> its number runs out. It drops by 1 each move; at 0 it blows!", "Bombalı parçayı sayı <b>bitmeden</b> çıkar. Her hamlede sayı 1 azalır; 0 olursa patlar!"),
            "nail" => LockIntro(0),
            "chain" => Lang.T("Two chained pieces move <b>together</b>.", "Zincirli iki parça <b>birlikte</b> hareket eder."),
            _ => TutorialCopy(scene)
        };
        public static string LockIntro(int moves) => string.Format(Lang.T("A locked piece can't move for <b>{0} moves</b>. Play other pieces; at 0 the lock opens.", "Kilitli parça <b>{0} hamle</b> boyunca hareket etmez. Başka parçaları oyna; sayı 0 olunca kilit açılır."), moves > 0 ? moves.ToString() : "N");
        public static string LockWarning(int remaining) => string.Format(Lang.T("The lock opens in {0} moves.", "Kilit {0} hamle sonra açılacak."), remaining);

        // Source copy: docs/i18n/en.md, Tutorial.
        public static string TutorialDrag(Dir dir) => dir switch
        {
            Dir.L => Lang.T("Drag the piece to the left", "Parçayı sola sürükle"),
            Dir.R => Lang.T("Drag the piece to the right", "Parçayı sağa sürükle"),
            Dir.U => Lang.T("Drag the piece up", "Parçayı yukarı sürükle"),
            _ => Lang.T("Drag the piece down", "Parçayı aşağı sürükle")
        };
        public static string TutorialErrorDrag(Dir dir) => dir switch
        {
            Dir.L => Lang.T("Drag this piece to the left", "Bu parçayı sola çek"),
            Dir.R => Lang.T("Drag this piece to the right", "Bu parçayı sağa çek"),
            Dir.U => Lang.T("Drag this piece up", "Bu parçayı yukarı çek"),
            _ => Lang.T("Drag this piece down", "Bu parçayı aşağı çek")
        };
        public static string TutorialCopy(string key) => key switch
        {
            "tap_scissors" => Lang.T("Tap the scissors", "Makası seç"),
            "tap_wand" => Lang.T("Tap the wand", "Değneği seç"),
            "tap_clock" => Lang.T("Tap the clock", "Saate dokun"),
            "tap_hammer" => Lang.T("Tap the hammer", "Çekici seç"),
            "target_scissors" => Lang.T("Tap the joint to cut it", "Parçaları ayırmak için eklemi kes"),
            "target_hammer" => Lang.T("Tap a piece to knock it out", "Parçayı çıkarmak için ona dokun"),
            "rule" => Lang.T("Pull every piece out\nwithout hitting other pieces!", "Tüm parçaları diğer parçalara\nçarpmadan dışarı çek!"),
            "stuck" => Lang.T("Stuck! Pull pieces out without hitting other pieces.", "Takıldı! Parçaları başka parçalara çarpmadan dışarı çekmelisin."),
            "nail" => NailBody,
            "bomb" => Lang.T("Free the bomb before its fuse runs out!", "Fitili bitmeden bombayı çıkar!"),
            "chain" => Lang.T("Chained pieces leave together. Clear both paths.", "Zincirli parçalar birlikte çıkar. İkisinin de yolunu aç."),
            "wall" => Lang.T("Pieces can't pass through red-white walls.", "Parçalar kırmızı-beyaz duvardan geçemez."),
            "scissors" => TutorialCopy("tap_scissors") + "\n" + ScissorsHint,
            "hammer" => TutorialCopy("tap_hammer") + "\n" + HammerHint,
            "wand" => P21BoosterArt ? Lang.T("Choose a region, then confirm.\nOnly its connections change.", "Bir bölge seç, sonra onayla.\nYalnız o bölgenin bağlantıları değişir.") : Lang.T("Shuffle the tabs.\nSame picture, new connections.", "Çıkıntıları yeniden dizer.\nResim aynı kalır, kilitler değişir."),
            "clock" => TutorialCopy("tap_clock") + "\n" + (P21BoosterArt ? ClockFreeze : ClockDone),
            _ => ""
        };
        public static string MyAvatar => Lang.T("My avatar", "Avatarım");
        public static string ProfileTitle => Lang.T("Profile", "Profil");
        public static string ChangeAvatar => Lang.T("Change avatar", "Avatarı değiştir");
        public static string Count(int n) => n.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo(Lang.Tr ? "tr-TR" : "en-US"));
        public static string SavedTotal(int n) => Lang.T($"{Count(n)} Jiggles saved", $"{Count(n)} Jiggle kurtarıldı");
        public static string Milestone(int i) => Lang.T(new[] { "Frame", "Badge", "Portrait" }[i], new[] { "Çerçeve", "Rozet", "Portre" }[i]);
        public static string Minutes(int n) => Lang.T($"{n} min", $"{n} dk");
        public static string Seconds(int n) => Lang.T($"{n} sec", $"{n} sn");
        public static string ProfileStat(int i) => Lang.T(
            new[] { "Level", "Saved Jiggles", "Total time", "First try", "Achievements", "Average time left" }[i],
            new[] { "Bölüm", "Kurtarılan Jiggle", "Toplam süre", "İlk denemede", "Başarı", "Ortalama kalan süre" }[i]);
        public static string AchievementName(int i) => Lang.T(
            new[] { "First Painting", "Collector", "Museum Owner", "Bomb Expert", "Cool Head", "Flawless", "Fast Fingers", "Chain Breaker", "Defy the Count" }[i],
            new[] { "İlk Tablo", "Koleksiyoncu", "Müze Sahibi", "Bomba İmha Uzmanı", "Soğukkanlı", "Hatasız", "Hızlı Parmaklar", "Zincir Kıran", "Kont'a Kafa Tut" }[i]);
        public static string AchievementCondition(int i) => Lang.T(
            new[] { "Win 1 level", "Win 10 levels", "Win 40 levels", "Win 10 bomb levels on your first try", "Win with less than 3 seconds left", "Win with no jams", "Win 10 levels with at least half the time left", "Save 25 chained pieces in won levels", "Win all 7 hard levels" }[i],
            new[] { "1 bölüm kazan", "10 bölüm kazan", "40 bölüm kazan", "Bombalı 10 bölümü ilk denemede kazan", "Kalan süre 3 sn altında kazan", "Hiç sıkışmadan kazan", "10 bölümü en az yarı süre kalırken kazan", "Kazanılan bölümlerde 25 zincirli parça kurtar", "7 zor bölümün hepsini kazan" }[i]);
        public static string Tab(int index) => index == 0 ? Lang.T("Home", "Ev") : index == 1 ? Lang.T("Collection", "Koleksiyon") : index == 2 ? "Jiggles" : Lang.T("Achievements", "Başarılar");
        public static string CollectionNext => Lang.T("Next", "Sıradaki");
        public static string CollectionLevel => Lang.T("Level {0}", "Bölüm {0}");
        public static string CollectionEarned => Lang.T("Earned in Level {0}", "Bölüm {0}'de kazanıldı");
        public static string CollectionReplay => Lang.T("Play again", "Tekrar oyna");
        public static string CollectionDetail => Lang.T("Painting details", "Tablo detayı");
        public static string SettingsRow(int index) => index == 0 ? Lang.T("Sound", "Ses") : index == 1 ? Lang.T("Music", "Müzik") : Lang.T("Vibration", "Titreşim");
        public static string SettingsLanguage => Lang.T("Language", "Dil");
        public static string IntroWake => Lang.T("Tap to wake!", "Uyandır!");
        public static string IntroSkip => Lang.T("Skip", "Atla");
        public static string IntroEnding => Lang.T("Save the Jiggles!", "Jiggles'ı kurtar!");
        public static string ReplayStory => Lang.T("Watch the story again", "Hikâyeyi tekrar izle");
        public static string SettingsPrivacy => Lang.T("Privacy", "Gizlilik");
        public static string SettingsContact => Lang.T("Contact us", "Bize yaz");
        public static string TutorialFirst => Lang.T("Push this piece to save it", "Bu parçayı it ve kurtar");
        public static string TutorialNext => Lang.T("Now save this one", "Şimdi bunu kurtar");
        public static string HardTitle => Lang.T("Hard level", "Zor bölüm");
        public static string HardBody => Lang.T("This one is a bit harder. Take your time, look first.", "Bu bölüm biraz daha zorlu. Acele etme, önce bak.");
        public static string HardStart => Lang.T("Start", "Başla");
        public static string Saved => Lang.T("Jiggles saved: {0}", "Kurtarılan Jiggles: {0}");
        public static string Settings => Lang.T("Settings", "Ayarlar");
        public static string SoundOn => Lang.T("Sound: On", "Ses: Açık");
        public static string SoundOff => Lang.T("Sound: Off", "Ses: Kapalı");
        public static string Close => Lang.T("Close", "Kapat");
        public static string MusicOn => Lang.T("Music: On", "Müzik: Açık");
        public static string MusicOff => Lang.T("Music: Off", "Müzik: Kapalı");
        public static string HapticsOn => Lang.T("Vibration: On", "Titreşim: Açık");
        public static string HapticsOff => Lang.T("Vibration: Off", "Titreşim: Kapalı");
        public static string Language => Lang.T("Language: English", "Dil: Türkçe");
        public static string Level => Lang.T("Level {0}", "Bölüm {0}");
        public static string Play => Lang.T("Play", "Oyna");
        public static string NailBody => Lang.T("Each move lowers the lock counter.\nAt zero, the piece is free.", "Her hamlede kilit sayacı azalır.\nSıfır olunca parça serbest kalır.");
        public static string Goal => Lang.T("Save the Jiggles!", "Jiggle'ları kurtar!");
        public static string Avatar => Lang.T("Choose your Jiggle", "Jiggle'ını seç");
        public static string Paused => Lang.T("Paused", "Durduruldu");
        public static string Resume => Lang.T("Continue", "Devam");
        public static string Restart => Lang.T("Restart", "Yeniden başla");
        public static string MainMenu => Lang.T("Main menu", "Ana menü");
        public static string MenuHint => Lang.T("Drag each piece the right way to clear the board.", "Her parçayı doğru yönde sürükleyip tahtayı boşalt.");
        public static string PlayLevel => Lang.T("Play · Level {0}", "Oyna · Bölüm {0}");
        public static string Levels => Lang.T("Levels", "Bölümler");
        public static string Player => Lang.T("Player", "Oyuncu");
        public static string NamePrompt => Lang.T("Your name", "Adın");
        public static string TestLevels => Lang.T("Pick any level to test.", "Test için istediğin bölüme atla.");
        public static string UnlockHint => Lang.T("Beat a level to unlock the next one.", "Bir bölümü geçince sonraki açılır.");
        public static string SetComplete => Lang.T("Well done! All {0} levels cleared", "Tebrikler, {0} bölüm bitti");
        public static string PictureComplete => Lang.T("Picture complete!", "Resmi tamamladın");
        public static string BoardCleared => Lang.T("Board cleared!", "Tahta boşaldı!");
        public static string BombExploded => Lang.T("The bomb exploded", "Bomba patladı");
        public static string TimeUp => Lang.T("Time's up", "Süre doldu");
        public static string WinStats => Lang.T("{0} seconds left, {1} jams.", "{0} saniye kaldı, {1} kez takıldın.");
        public static string BombHint => Lang.T("Clear the bomb's path before the fuse runs out. The hammer removes it in one move.", "Fitil bitmeden bombanın yolunu açman gerekiyordu. Çekiç onu tek hamlede söker.");
        public static string TimeHint => P21BoosterArt ? Lang.T("{0} pieces left. ", "{0} gövde kaldı. ") + ClockFreeze : Lang.T("{0} pieces left. The clock booster adds 20 seconds.", "{0} gövde kaldı. Saat booster'ı 20 saniye ekler.");
        public static string NextLevel => Lang.T("Next level", "Sonraki bölüm");
        public static string Retry => Lang.T("Try again", "Tekrar dene");
        public static string Congratulations => Lang.T("Well done!", "Tebrikler!");
        public static string AskFun => Lang.T("Did you like this level?", "Bu bölümü beğendin mi?");
        public static string Liked => Lang.T("Liked", "Beğendim");
        public static string Disliked => Lang.T("Disliked", "Beğenmedim");
        public static string Thanks => Lang.T("Thanks!", "Teşekkürler!");
        public static string AskHard => Lang.T("How hard was it? 1 very easy · 10 very hard", "Ne kadar zordu? 1 çok kolay · 10 çok zor");
        public static string AskHardLost => Lang.T("Your rating still counts! How hard was it? (1–10)", "Kaybettin ama puanın önemli: ne kadar zordu? (1–10)");
        public static string RatingSaved => Lang.T("Saved, thanks!", "Kaydedildi, teşekkürler!");
        public static string SkipRating => Lang.T("Skip", "Geç");
        // PM_MESAJ_25: one question per cohort, after the name
        public static string AskProfileTitle => Lang.T("One question", "Tek bir soru");
        public static string AskProfile => Lang.T("Do you play puzzle games on your phone? (like Candy Crush or Royal Match)", "Telefonda bulmaca oyunları oynar mısın? (Candy Crush, Royal Match gibi)");
        public static string ProfileNever => Lang.T("Never", "Hiç");
        public static string ProfileSometimes => Lang.T("Sometimes", "Ara sıra");
        public static string ProfileOften => Lang.T("Often", "Sık");
        public static string Menu => Lang.T("Menu", "Menü");
        public static string Demo => Lang.T("This is a demo level.", "Bu bir demo bölüm.");
        public static string PlayAgain => Lang.T("Play again", "Tekrar oyna");
        public static string NewBooster => Lang.T("New booster: {0}", "Yeni booster: {0}");
        public static string GotIt => Lang.T("Got it", "Anladım");
        public static string Use => Lang.T("Use", "Kullan");
        public static string Cancel => Lang.T("Cancel", "Vazgeç");
        public static string NailReleased => Lang.T("The lock opened", "Kilit kendiliğinden açıldı");
        public static string BackToEditor => Lang.T("Back to editor", "Editöre dön");
        public static string Scissors => Lang.T("SCISSORS", "MAKAS");
        public static string Wand => Lang.T("WAND", "DEĞNEK");
        public static string Hammer => Lang.T("HAMMER", "ÇEKİÇ");
        public static string Clock => Lang.T("CLOCK", "SAAT");
        public static string LockedLevel => Lang.T("LEVEL {0}", "{0}. BÖLÜM");
        public static string UnlocksAt => Lang.T("Unlocks at level {0}", "{0}. bölümde açılıyor");
        public static string ScissorsHint => Lang.T("Tap the joint you want to cut", "Kesmek istediğin birleşim noktasına dokun");
        public static string HammerHint => Lang.T("Tap the piece you want to break", "Kırmak istediğin parçaya dokun");
        public static string WandFailed => Lang.T("No new layout found. Your wand wasn't used.", "Değnek yeni bir dizilim bulamadı, hakkın harcanmadı");
        public static string ClockDone => Lang.T("+20 seconds", "+20 saniye");
        public static string AskWand => Lang.T("Use a wand?", "Değnek kullanılsın mı?");
        public static string WandBody => Lang.T("Shuffle all the tabs without changing the picture. Which pieces are locked together will change completely. You have {0} left.", "Tahtadaki tüm çıkıntılar yeniden dizilir. Resim değişmez ama hangi parçaların birbirine kilitli olduğu tamamen değişir. Elinde {0} tane kaldı.");
        public static string AskClock => Lang.T("Use a clock?", "Saat kullanılsın mı?");
        public static string ClockBody => Lang.T("Add 20 seconds to the timer. You have {0} left.", "Geri sayıma 20 saniye eklenir. Elinde {0} tane kaldı.");

        public static string BoosterName(Booster b) => b == Booster.Scissors ? Scissors : b == Booster.Wand ? Wand : b == Booster.Hammer ? Hammer : Clock;

        public static (string title, string body) BoosterQuestion(BoosterControls bar, Booster b)
        {
            var question = bar.Question(b);
            if (Lang.Tr) return question;
            // Core owns the stock; its question ends with the remaining count.
            var count = System.Text.RegularExpressions.Regex.Match(question.body, @"(\d+) tane kaldı\.$");
            return (b == Booster.Wand ? AskWand : AskClock,
                    count.Success ? string.Format(b == Booster.Wand ? WandBody : ClockBody, count.Groups[1].Value) : question.body);
        }

        /* Core keeps its original messages; translate them only at the drawing boundary. */
        public static string BoosterPrompt(BoosterControls bar)
        {
            string prompt = bar.Prompt;
            if (prompt == BoosterControls.ScissorsHint) return ScissorsHint;
            if (prompt == BoosterControls.HammerHint) return HammerHint;
            if (prompt == BoosterControls.WandFailed) return WandFailed;
            if (prompt == BoosterControls.ClockDone) return ClockDone;
            foreach (var b in BoosterExt.All)
                if (prompt == string.Format("{0}. bölümde açılıyor", bar.UnlockLevel(b)))
                    return string.Format(UnlocksAt, bar.UnlockLevel(b));
            return prompt;
        }
    }
}
