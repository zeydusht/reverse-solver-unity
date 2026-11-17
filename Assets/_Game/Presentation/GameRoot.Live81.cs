using System.Collections;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_81: the soft launch's store build.
         content     iOS (and the web with &kaynak=live) reads GitHub Pages' live/ folder: the levels, the settings, the
                     paintings; the device's copy or the app's own when offline (Live, Core.ContentLoader)
         the set     sl100 (100 levels, resolved: no runtime tuning on top); ?set=sl100 on the web
         ads         AppLovin MAX on iOS (R1 time-up +30 s, R2 2x gold, I1 after a win; PM_82: R3, R4); none on the web,
                     &reklam=sahte a fake network to try them; the SDK (and its ATT question) starts after level 2
         telemetry   cohort sl1 sends sl_events (first open, sessions, ATT, ads, crashes) */
    public sealed partial class GameRoot
    {
        bool _started;      // StartGame has run (Update waits for it)
        bool _slSet;        // the set is a resolved live set (sl100)
        bool _adsAsked;     // the ad network was asked to start
        bool _r1Used;       // the R1 ad continue of this attempt

        /* Start: the content first (at most a few seconds on the network when live/ is read), then the game. */
        IEnumerator Start()
        {
            bool ios = Application.platform == RuntimePlatform.IPhonePlayer;
            if (ios || Host.Current.Query("kaynak") == "live") yield return Live.Open(this, ios, ios ? 2.5f : 8f);
            else Live.UseEmbeddedConfig();
            ContinueRules.DefaultStartCoins = Live.Config.StartCoins;
            StartGame();
            _started = true;
            AfterStart81();
        }

        void AfterStart81()
        {
            if (!_testMode) _continueRules = new ContinueRules(Live.Config.ContinuePrices, Live.Config.ExtraSeconds);
            if (Host.Current.Query("reklam") == "sahte")
            {
                // the fake network starts at once; the screens built before it (the menu, a preview) are built again
                Ads.Network = new FakeAdNetwork(this, _screen);
                _adsAsked = true;
                Ads.Network.Init(null);
                if (_mode == Mode.Menu) Layout();
                else { if (_continuePanel != null || _continuePreview) ShowContinue(); _win21?.Layout(_screen, WinTargets21()); }
            }
            if (_testMode && Host.Current.Query("bitis") == "1") ShowFinale81();   // the test link: the finale at once
            if (Cohort == "sl1")
            {
                bool send = Host.Current.SendEnabled && !Host.Debug && !Application.isEditor;
                SlTelemetry.Start(Host.Current, send, _save.Player, () => System.Guid.NewGuid().ToString("N"));
            }
            Debug.Log($"[live] set {_set.Levels.Count} levels, config {Live.Config.Source}, ads {(Ads.Network != null ? Ads.Network.Name : "none")}, cohort {Cohort}");
        }

        /* The live set (Live.Active) or ?set=sl100 (the app's copy); null otherwise. */
        LevelSet LiveSet()
        {
            string json = null;
            if (Live.Active) json = Live.Content?.LevelsJson;
            else if (Host.Current.Query("set") == "sl100" || DefaultSl82()) json = Resources.Load<TextAsset>("Live/levels")?.text;
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                var set = LevelParser.Parse(json);
                _slSet = true;
                return set;
            }
            catch (LevelFormatException e)
            {
                Debug.LogWarning("[live] levels: " + e.Message);
                var own = Resources.Load<TextAsset>("Live/levels");   // a broken download: the app's own copy
                if (own == null) return null;
                _slSet = true;
                return LevelParser.Parse(own.text);
            }
        }

        /* PM_82: p23's default set is sl100 (the app's own copy); ?set=… still opens the others. */
        static bool DefaultSl82()
        {
            if (Host.Current.Query("set") != null || Host.Current.Query("lv") == "SHOW1" || Host.Current.Query("lv") == "SHOW_ART") return false;
            string c = TelemetryMeta.CleanCohort(Host.Current.Query("c")) ?? TelemetryMeta.CleanCohort(Host.Current.Store.Get("rs_cohort"));
            return c == "p23";
        }

        /* The test link's &sonraki=N: the menu's button for level N (the hard colours at a glance). */
        int MenuNext82()
        {
            if (_testMode && int.TryParse(Host.Current.Query("sonraki"), out int n)) return Mathf.Clamp(n - 1, 0, _set.Levels.Count - 1);
            return _progress.Next();
        }

        /* The ad network starts (and MAX asks the ATT question) once level 2 is won and nothing else is on the screen
           (not with the profile question, not over a win screen or a dive). A fake network starts at once. */
        void AdsTick81()
        {
            if (_adsAsked || Ads.Network == null || _progress == null) return;
            bool fake = Ads.Network is FakeAdNetwork;
            if (!fake && (!_progress.Won(1) || _card != null || _profileCard != null || _diving || _win21 != null ||
                _continuePanel != null || _celebration != null || _corridor != null || _awaitingName)) return;
            _adsAsked = true;
            Ads.Network.Init(status => SlTelemetry.Emit("att_status", value: status));
        }

        /* R1: the time-up offer shows "Watch an ad, +30 s" next to the gold button, once per attempt. */
        bool OfferAdContinue81() =>
            !_r1Used && _continueOffer != null && _continueOffer.Kind == ContinueKind.Time && Ads.CanOffer(AdPlacement.R1Continue, true);

        void WatchAdContinue81()
        {
            Ads.ShowReward(AdPlacement.R1Continue, ok =>
            {
                if (!ok || _deferredLoss == null) return;
                _r1Used = true;
                ResolveContinue("ad");
            });
        }

        /* R2: the win screen's "Watch an ad, 2x gold" (+50 -> +100). */
        bool OfferDoubleGold81() => ContinueEnabled && Ads.CanOffer(AdPlacement.R2DoubleGold, true);

        void WatchDoubleGold81(System.Action<bool> done)
        {
            Ads.ShowReward(AdPlacement.R2DoubleGold, ok =>
            {
                if (ok && !_winPreview21) _save.SetCoins((int)System.Math.Min(int.MaxValue, (long)_save.Coins + _continueRules.WinCoins));
                done(ok);
            });
        }

        /* I1 on Next after a win, before the corridor. */
        void AfterWinAd81(System.Action then)
        {
            if (_session == null || _testMode && !(Ads.Network is FakeAdNetwork)) { then(); return; }
            Ads.AfterWin(_session.Level.Number > 0 ? _session.Level.Number : _index + 1, SlTelemetry.FirstSession, SlTelemetry.SessionStart, then);
        }

        /* The device's id: IDFV on iOS (PM_81 §5), the install id elsewhere. */
        string DeviceId81()
        {
#if UNITY_IOS && !UNITY_EDITOR
            string idfv = UnityEngine.iOS.Device.vendorIdentifier;
            if (!string.IsNullOrEmpty(idfv)) return idfv;
#endif
            return _save.Install;
        }

        // ---- PM_81 §6: after the last level ----

        Transform _finale;
        Rect _finaleMenu;

        /* "New paintings are on the way!": Jigo, the Jiggles saved in all, Menu. Nothing is locked; the menu's Play
           brings this back while there is no new level. */
        void ShowFinale81()
        {
            CloseFinale81();
            CancelCardTap(); CloseCard();
            _finale = new GameObject("PM81 finale").transform;
            _finale.SetParent(_world != null ? _world : transform, false);
            int order = CardOrder + 200;
            float k = Mathf.Min(_screen.x / 390, _screen.y / 844), top = (_screen.y - 844 * k) * .5f;
            Vector2 At(float x, float y) => new Vector2(_screen.x * .5f + (x - 195) * k, top + y * k);
            ShapeView.Create(_finale, "shade", _screen * 1.2f, 0, order).Radius(0).Fill(Draw.Rgba(8, 14, 40, .86f)).transform.localPosition = Draw.W(_screen * .5f);
            float cover = Mathf.Max(_screen.x / 390, _screen.y / 844);
            var glow = Art.Place(_finale, "T50/win/win_bg_glow", new Vector2(390, 844) * cover, order + 1);
            if (glow != null) glow.transform.localPosition = Draw.W(_screen * .5f);
            var jigo = Art.Place(_finale, "T50/win/win_jigo_jump", new Vector2(262, 294) * k, order + 3);
            if (jigo != null) jigo.transform.localPosition = Draw.W(At(195, 380));
            TextView.Create(_finale, "title", Lang.T("New paintings are on the way!", "Yeni tablolar yolda!"), 40 * k, Palette.Cream, order: order + 5)
                .Title3D().FitWidth(340 * k, 22 * k).transform.localPosition = Draw.W(At(195, 160));
            TextView.Create(_finale, "saved", string.Format(Lang.T("You saved {0} Jiggles", "{0} Jiggle kurtardın"), SavedJiggles()), 26 * k, Palette.Cream, order: order + 5)
                .Heading(Draw.Hex("#153E75")).FitWidth(330 * k, 14 * k).transform.localPosition = Draw.W(At(195, 572));
            TextView.Create(_finale, "soon", Lang.T("You beat every painting. New ones come soon.", "Bütün tabloları bitirdin. Yenileri çok yakında."), 17 * k, Palette.Cream, order: order + 5)
                .Heading(TextView.RimTeal).FitWidth(330 * k, 11 * k).transform.localPosition = Draw.W(At(195, 616));
            if (Button75.Available)
                _finaleMenu = Button75.Main(_finale, At(195, 716), new Vector2(262, 88) * k, Lang.T("Menu", "Menü"), order + 6).Rect;
            else _finaleMenu = new Rect(At(195, 716) - new Vector2(131, 44) * k, new Vector2(262, 88) * k);
        }

        bool FinaleTap81(Vector2 pt)
        {
            if (_finale == null) return false;
            if (_finaleMenu.Contains(pt)) { CloseFinale81(); ShowMenu(); }
            return true;
        }

        void CloseFinale81()
        {
            if (_finale != null) Destroy(_finale.gameObject);
            _finale = null;
        }

        /* The menu's Play once every level is won (no replays outside the test link): the finale again. */
        bool AllWon81() => !_testMode && _progress != null && _set.Levels.Count > 0 && _progress.Won(_set.Levels.Count - 1);
    }
}
