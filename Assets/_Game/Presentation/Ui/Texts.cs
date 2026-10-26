namespace ReverseSolver.Presentation
{
    /* New player-facing texts (SET40 v3, PM_MESAJ_6 §3.4): kept in one place so an English set can
       replace this class later. Level cards (intro / bintro) carry their own text in the level file. */
    public static class Texts
    {
        public const string TutorialFirst = "Bu parçayı it ve kurtar";
        public const string TutorialNext = "Şimdi bunu kurtar";
        public const string HardTitle = "Zor bölüm";
        public const string HardBody = "Bu bölüm zor, haberin olsun. Süre ilk hamlende başlar.";
        public const string HardStart = "Başla";
        // PM_MESAJ_9: profile box, settings, level start card
        public const string Saved = "Kurtarılan Jiggles: {0}";
        public const string Settings = "Ayarlar";
        public const string SoundOn = "Ses: Açık", SoundOff = "Ses: Kapalı", Close = "Kapat";
        public const string Level = "Level {0}";
        public const string Goal = "Tüm Jiggles'ı kurtar";
        public const string Avatar = "Jiggle'ını seç";
        public const string Paused = "Durduruldu", Resume = "Devam", Restart = "Yeniden başla", MainMenu = "Ana menü";
    }
}
