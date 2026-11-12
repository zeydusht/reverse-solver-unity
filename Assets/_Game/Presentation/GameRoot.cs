using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReverseSolver.Presentation
{
    /* Boots the game and owns the screen: menu, level list, the board with its
       strip and booster bar, and the cards (intro, booster question, result).
       It runs one GameSession at a time; leaving a level unfinished (restart,
       menu, another level) ends the attempt as a quit row.

       Screen layout (points, y down), the web game's flex column:
         safe top + 4 | strip (10 + 46 + 6) | stage: tray centred |
         prompt 17 + booster bar 97 | 4 + safe bottom
       Cell size: Presentation BoardSizing (Core's vertical reserve, at least
       24 CSS points of horizontal clearance).
       Nothing is drawn below the safe bottom inset (iOS home indicator).

       URL: ?lv=N opens level N directly; ?debug=1 shows the load panel, the FPS
       line and unlocks every level. ?debug=1&set=designs plays the level
       editor's designs instead (designs.json), and lv may then be an id
       (?debug=1&set=designs&lv=D03). Without debug the set parameter is
       ignored; debug sessions never send telemetry. */
    public sealed partial class GameRoot : MonoBehaviour
    {
        // plays.client: the web build or the iOS app (TestFlight)
        public static readonly string Client = Application.platform == RuntimePlatform.IPhonePlayer ? "unity-ios" : "unity-web";
        const float BodyPad = 4f, StripBottom = 6f, BottomReserve = BoosterBar.PromptH + BoosterBar.Height, MaxCell = 78f;
        const int HandOrder = 1400, HudOrder = 3000, BarOrder = 3200, CardOrder = 5000, FlashOrder = 4500, CorridorOrder = 5600, DebugOrder = 6000;

        [SerializeField] TextAsset levels;
        [SerializeField] TextAsset designs;          // level editor output; only with ?debug=1&set=designs
        [SerializeField] TextAsset zeydbot;          // ZeydBot sessions (?set=zeydbot&lv=Z01), moves sent too
        [SerializeField] TextAsset redesign;         // Faz 4 set (R..); ?debug=1&set=redesign, default only with SetFlags.RedesignDefault
        [SerializeField] Material pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial;

        enum Mode { Menu, Playing }

        LevelSet _set;
        AttemptLog _log;
        SaveData _save;
        PlayStats _stats;
        bool _previewWins;
        bool _testMode;
        string _playCohort;
        TelemetryLog _telemetry;
        string _rowId;                 // row_id of the attempt in play
        bool _awaitingName;
        bool _introPlaying, _endingEligible, _endingPlayed;
        TelemetryQueue _introQueue;
        TelemetrySender _introSender;
        readonly ContinueRules _continueRules = new ContinueRules();
        TelemetryQueue _continueQueue;
        TelemetrySender _continueSender;
        ContinuePanel _continuePanel;
        ContinueOffer _continueOffer;
        AttemptRecord _deferredLoss;
        bool _continuePreview, _pageHidden, _continueResultOnReturn;
        bool ContinueEnabled => Art.T54 && TelemetryMeta.Current?.Cohort != "pt1" && !PlayOverride.Active;
        float _nextFlush;
        Progress _progress;
        Mode _mode = Mode.Menu;
        int _index;
        GameSession _session;
        DragModel _drag;
        FlickGesture _flick;
        Vector2 _gestureStart, _gestureLast;
        double _gestureTime;
        int _flickInputs, _dragInputs;
        bool _committedTouch;
        BoosterControls _boosters;
        BoosterInventory _inventory;
        Booster? _trainingUse;
        int _boosterTarget = -1;
        (int x, int y, bool vertical) _boosterJoint;
        bool _tutorialClockHold;
        bool _introOpen;
        bool _designSet;              // playing designs.json (?debug=1&set=designs)
        bool _redesignSet;            // SET40 (redesign.json): 1-10 only with a cohort (PM_MESAJ_25)
        bool _zeydbotSet;             // playing zeydbot.json (?set=zeydbot): every move is recorded and sent
        bool _paintSet;               // PM_MESAJ_18: SET40 v3 shows painting-NN on level N (fetched from Pages)
        bool _showSet;                // lv=SHOW1 (PM_MESAJ_17): the investor demo level alone, no menu, no name gate
        TelemetryQueue _moveQueue;
        TelemetrySender _moveSender;
        float _moveT0, _moveLast;
        int _moveNo;
        float _pressAt;
        bool _newArt;                 // SET40 v3 preview: the corridor menu art (live set keeps the old menu)
        bool _tutorialDone;           // web tutDone: first removal anywhere ends the hand for this visit
        TutorialHand _hand;
        TutorialScript _tutorialScript;
        List<TutorialStep> _tutorialSteps = new List<TutorialStep>();
        int _tutorialAt;
        TutorialSpotlight _spotlight;
        Move _tutorialMove;
        float _tutorialUntil = -1;
        Vector2 _tutorialPress;
        bool TutorialActive => _mode == Mode.Playing && _session != null && !_session.IsOver && _tutorialAt < _tutorialSteps.Count;
        LevelImages _images;          // G1: level pictures, fetched when a level opens
        readonly HashSet<string> _introSeen = new HashSet<string>();

        Camera _cam;
        Transform _world;
        BoardView _board;
        Hud _hud;
        bool UseHud54 => Art.T54 && TelemetryMeta.Current?.Cohort != "pt1";
        BoosterBar _bar;
        Card _card;                  // the one open card: result, intro or question
        string _cardKind;
        Screens.Menu _menu;
        MenuTab _tab = MenuTab.Home;
        bool _levelsMapOpen;
        MenuTabs _tabs;
        CollectionScreen _collection;
        LevelsMap _levelsMap;
        PaintingDetail _paintingDetail;
        MenuSettings _menuSettings;
        JigglesScreen _jiggles;
        AchievementsScreen _achievements;
        ProfileCard _profileCard;
        float _jigglesOffset, _achievementsOffset;
        float _collectionOffset;
        float? _mapOffset;
        bool _menuPointer, _menuDragging, _menuStopTap, _tabAnimating;
        Vector2 _menuPointerStart, _menuPointerLast;
        MenuScroll ActiveMenuScroll => _menuSettings != null || _paintingDetail != null || _profileCard != null || _card != null ? null : _levelsMap?.Scroll ?? _collection?.Scroll ?? _jiggles?.Scroll ?? _achievements?.Scroll;
        CorridorTransition _corridor;  // PM_MESAJ_24: the slide to the next level's painting, over everything
        Screens.LevelList _list;
        TextView _debug;
        TextView _fpsLabel;           // PM_MESAJ_53: ?fps=1, a phone FPS readout independent of debug
        float _fpsLabelAcc, _fpsLabelWorst; int _fpsLabelFrames;
        Vector2Int _screenPx;
        Vector2 _screen;            // points
        Vector4 _safe;              // points: left, top, right, bottom
        float _dpr = 1;
        float _endAt = -1;
        WinCelebration54 _celebration;
        bool _celebrationResult;
        bool _jointsShown;

        readonly List<float> _dragFps = new List<float>();
        float _fpsAcc; int _fpsFrames; float _fpsShown;

        void Start()
        {
            Art.T28 = Host.Current.Query("art") != "eski";
            Music.Start(this);                                 // the menu and the levels share one loop
            Materials.Init(pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial);
            _images = LevelImages.Create();
            _cam = Camera.main;
            _set = PlayOverride.Active ? PlayOverride.Set : ChooseSet();
            string launchCohort = TelemetryMeta.CleanCohort(Host.Current.Query("c")) ?? TelemetryMeta.CleanCohort(Host.Current.Store.Get("rs_cohort"));
            _playCohort = launchCohort; Art.CohortOverride = launchCohort;
            _testMode = LaunchPlan.TestMode(Host.Current.Query("c"), Host.Current.Query("test"));
            string guidedFlag = Host.Current.Query("tut2x2");
            string reset = Host.Current.Query("reset");
            if (reset == "1" && launchCohort == "p21")
                _resetVisit = SaveData.ResetForQuery(Host.Current.Store, reset, launchCohort, P21Onboarding.Prepend(_set).Levels);

            // saved counters and the send queue (M5-pre). Sending is off in the
            // editor, with ?debug=1, and in builds until the switch is turned on.
            System.Func<string> newId = () => System.Guid.NewGuid().ToString("N");
            var host = Host.Current;
            _save = new SaveData(host.Store, newId);
            bool keepPrelude = guidedFlag != "0" && P21Onboarding.HasPreludeProgress(_save);
            _p21Flow = P21Onboarding.Enabled(launchCohort, _redesignSet, PlayOverride.Active, Art.T54, keepPrelude ? "1" : null)
                && P21Onboarding.IsNewFlow(_save);
            _p21Revised = !_p21Flow && P21Onboarding.UseGuided(launchCohort, _redesignSet, PlayOverride.Active, Art.T54, guidedFlag);
            if (_p21Flow) _set = P21Onboarding.Prepend(_set);
            else if (_p21Revised) _set = P21Onboarding.SpaceObstacleLessons(P21Onboarding.ReplaceOpening(_set));
            Debug.Log($"[p21] flow={_p21Flow} levels={_set.Levels.Count}");
            _stats = PlayStats.Load(host.Store);
            _stats.ImportLegacy(_save);
            _log = _save.Restore();
            var queue = new TelemetryQueue(host.Store);
            bool send = host.SendEnabled && !Host.Debug && !Application.isEditor;
            var sender = new TelemetrySender(queue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send);
            var ratingQueue = new TelemetryQueue(host.Store, "ratings");
            var ratings = new TelemetrySender(ratingQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "rpc/set_rating");
            _telemetry = new TelemetryLog(_save, queue, sender, newId, ratingQueue, ratings);
            // PM_MESAJ_25: cohort from the link (?c=pt1), kept on the device for the next visit without it
            string cohort = TelemetryMeta.CleanCohort(host.Query("c"));
            if (cohort != null) { host.Store.Set("rs_cohort", cohort); host.Store.Save(); }
            else cohort = TelemetryMeta.CleanCohort(host.Store.Get("rs_cohort"));
            TelemetryMeta.Current = new TelemetryMeta { Cohort = _testMode ? "ic" : cohort, DeviceId = _save.Install, Platform = PlatformName(), AppVersion = AppVersion() };
            _profileQueue = new TelemetryQueue(host.Store, "profiles");
            _profileSender = new TelemetrySender(_profileQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "player_profiles?on_conflict=cohort,player,device_id");
            if (_zeydbotSet)
            {
                _moveQueue = new TelemetryQueue(host.Store, "moves");
                _moveSender = new TelemetrySender(_moveQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "moves?on_conflict=row_id,move_no");
            }
            _introQueue = new TelemetryQueue(host.Store, "intro");
            _introSender = new TelemetrySender(_introQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "intro_events?on_conflict=row_id");
            _continueQueue = new TelemetryQueue(host.Store, "continue");
            _continueSender = new TelemetrySender(_continueQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "continue_events?on_conflict=row_id");
            _continueSender.Flush();
            if (cohort == "p21")
            {
                _tutorialQueue = new TelemetryQueue(host.Store, "tutorial");
                _tutorialSender = new TelemetrySender(_tutorialQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "tutorial_events?on_conflict=row_id");
                _tutorialSender.Flush();
            }
            if (ContinueEnabled && !_resetVisit)
                _save.SetCoins(int.TryParse(host.Query("coin"), out int coins) && coins >= 0 ? coins : _save.Coins);
            _save.TestMode = _testMode;
            string introParam = host.Query("intro");
            bool hasLv = host.Query("lv") != null, hasSet = host.Query("set") != null;
            bool opening = IntroPlan.ShowOpening(introParam, Host.Debug, _showSet, PlayOverride.Active, hasLv, hasSet,
                cohort == "pt1", _redesignSet, host.Store.Get("rs_intro_shown") != null, _save.Best.Count > 0);
            _endingEligible = IntroPlan.EndingEligible(introParam, Host.Debug, _showSet, PlayOverride.Active, hasLv, hasSet,
                cohort == "pt1", _redesignSet, host.Store.Get("rs_intro_end_shown") != null, _save.Best.ContainsKey(_set.Levels[0].Id));
            _introSender.Flush();
            host.VisibilityChanged += OnVisibility;
            _telemetry.Flush();                               // rows left from an earlier visit

            // Explicit unlock links are for internal testing or debug only; pt1 always keeps progression locks.
            bool unlock = host.Query("unlock") == "1" && cohort != "pt1" && (cohort == "ic" || Host.Debug);
            _progress = new Progress(_log, _set.Levels) { UnlockAll = _testMode || cohort != "pt1" && (unlock || Host.Debug || (_zeydbotSet && !string.IsNullOrEmpty(host.Query("set")))) };
            // PM_MESAJ_26 QA (debug only): ?won=N shows the menus as if the first N levels were won (memory only, never saved)
            if (Host.Debug && int.TryParse(host.Query("won"), out int won))
            {
                _previewWins = true;
                var best = new System.Collections.Generic.Dictionary<string, int>();
                for (int w = 0; w < won && w < _set.Levels.Count; w++) best[_set.Levels[w].Id] = 3;
                _log = new AttemptLog();
                _log.Restore(new System.Collections.Generic.Dictionary<string, int>(), best);
                _progress = new Progress(_log, _set.Levels) { UnlockAll = _testMode || unlock };
            }
            // web name gate: once per browser; the HTML input sits over the canvas
            if (_showSet && string.IsNullOrEmpty(_save.Player)) _save.SetPlayer("SHOW");   // demo visitors are tagged SHOW
            _debugCorridor = Host.Debug && host.Query("corridor") == "1";   // PM_MESAJ_28 B2: the corridor once over the menu, for FPS
            if (Host.Debug && !string.IsNullOrEmpty(host.Query("player"))) _save.SetPlayer(host.Query("player"));   // PM_MESAJ_26 QA: a long name
            bool askName = string.IsNullOrEmpty(_save.Player) && !Host.Debug && !_showSet;
            int requested = PlayOverride.Active ? PlayOverride.Index
                          : !_showSet && TryLevelIndex(host.Query("lv"), out int index) ? index : -1;   // SHOW1: the menu first, its Demo button starts the level
            _awaitingName = askName;
            System.Action<string> nameGiven = name =>
            {
                bool fresh = _save.Best.Count == 0;
                _save.SetPlayer(name); _awaitingName = false;
                if (_mode == Mode.Menu) Layout();
                if (fresh) StartCoroutine(RecoverProgress(name));
            };
            if (opening)
            {
                System.Action play = () => PlayStory("AB", false, () => { MarkIntro("rs_intro_shown"); StartLevel(0); });
                if (askName) { ShowMenu(); host.AskName(name => { nameGiven(LaunchPlan.NormalizePlayerName(name)); play(); }); }
                else play();
            }
            else LaunchPlan.Run(requested, askName, StartLevel, ShowMenu, host.AskName, nameGiven);
            // PM_MESAJ_43: the profile question waits for the first win of level 2 or later (NavigateResult)
            _profileSender.Flush();
        }

        // ---- PM_MESAJ_25: who plays (cohort, device, platform, version; one profile question) --------

        TelemetryQueue _profileQueue;
        TelemetrySender _profileSender;

        void MarkIntro(string key)
        {
            Host.Current.Store.Set(key, "1");
            Host.Current.Store.Save();
        }

        void PlayStory(string parts, bool replay, System.Action done)
        {
            if (_introPlaying) return;
            _introPlaying = true;
            _drag?.Cancel();
            Host.Current.PlayIntro(parts, Texts.IntroWake, Texts.IntroSkip, Texts.IntroEnding, !Art.Muted, value =>
            {
                if (value == "done") { _introPlaying = false; done(); }
                else RecordIntro(value, replay);
            });
        }

        void RecordIntro(string value, bool replay)
        {
            string[] fields = value.Split('|');
            if (fields.Length < 3) return;
            string ev = fields[0] == "shown" ? "intro_shown" : fields[0] == "skip" ? "intro_skipped"
                : fields[0] == "wake" ? "intro_wake_tap" : fields[0] == "fail" ? "intro_failed" : null;
            if (ev == null || !_introSender.Enabled) return;
            if (!double.TryParse(fields[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double sec)
                || double.IsNaN(sec) || double.IsInfinity(sec)) sec = 0;
            string id = System.Guid.NewGuid().ToString("N");
            var m = TelemetryMeta.Current;
            var row = new Core.Json.JsonWriter().BeginObject().Field("row_id", id).Field("player", _save.Player)
                .Field("device_id", m.DeviceId).Field("cohort", m.Cohort).Field("app_version", m.AppVersion)
                .Field("event", ev).Field("part", fields[1]).Key("sec").Raw(sec.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Key("tapped").Value(fields[0] == "wake" && fields.Length > 3 && fields[3] == "1")
                .Key("replay").Value(replay).EndObject().ToString();
            _introQueue.Add(id, row);
            _introSender.Flush();
        }

        static string PlatformName() =>
            Application.platform == RuntimePlatform.IPhonePlayer ? "ios"
            : Application.platform == RuntimePlatform.Android ? "android"
            : Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform ? "mobile-web" : "desktop";

        static string AppVersion() => (Application.platform == RuntimePlatform.IPhonePlayer ? "ios-" : "web-") + BuildStamp.Value;

        string Cohort => TelemetryMeta.Current?.Cohort;
        string ProfileKey => "rs_profile_" + Cohort;

        /* Once per cohort on this device, right after the name: "Do you play puzzle games like this?" */
        void AskProfile()
        {
            if (Cohort == null || _showSet || string.IsNullOrEmpty(_save.Player) || Host.Current.Store.Get(ProfileKey) != null) return;
            OpenCard("profile", new Card(_world, _screen, CardOrder, "Profile") { Compact = true }.Title(Texts.AskProfileTitle)
                .Body(Texts.AskProfile, sizePt: 14, gapAfter: 8)
                .Button("exp_0", Texts.ProfileNever, false)
                .Button("exp_1", Texts.ProfileSometimes, false)
                .Button("exp_2", Texts.ProfileOften, false).Finish());
        }

        void AnswerProfile(int exp)
        {
            Host.Current.Store.Set(ProfileKey, exp.ToString());
            Host.Current.Store.Save();
            var m = TelemetryMeta.Current;
            if (_telemetry.Enabled) _profileQueue.Add(m.Cohort + "|" + _save.Player + "|" + m.DeviceId, ProfileRow.Json(m.Cohort, _save.Player, m.DeviceId, m.Platform, exp));
            _profileSender.Flush();
            CloseCard();
            // PM_MESAJ_40: the question may have covered a level's intro card; it comes back, never swallowed
            if (_mode == Mode.Playing && _introOpen) OpenCard("intro", Screens.Intro(_world, _screen, _session.Level, CardOrder, P21ObstacleLesson));
        }

        /* 1-10 after a level: SET40 with a cohort asks once per level (win or loss, "Skip" allowed); without a cohort
           SET40 does not ask; the ZeydBot set asks after every attempt, as before. */
        bool AskHardNow(string levelId) =>
            _redesignSet ? Cohort != null && Host.Current.Store.Get("rs_hard_" + Cohort + "_" + levelId) == null : _zeydbotSet;

        void HardAsked(string levelId)
        {
            if (!_redesignSet || Cohort == null) return;
            Host.Current.Store.Set("rs_hard_" + Cohort + "_" + levelId, "1");
            Host.Current.Store.Save();
        }

        /* iOS: the levels saved from GitHub Pages at an earlier launch (Platform.RemoteLevels), when they still parse
           as a full set; otherwise the copy inside the app. */
        string RedesignJson()
        {
            string remote = Application.platform == RuntimePlatform.IPhonePlayer ? PlayerPrefs.GetString("rs_remote_redesign", null) : null;
            if (!string.IsNullOrEmpty(remote))
            {
                try { if (LevelParser.Parse(remote).Levels.Count >= 40) return remote; } catch { }
            }
            return redesign.text;
        }

        LevelSet ChooseSet()
        {
            // PM_MESAJ_17: lv=SHOW1 opens the demo level by itself, with the new look and the painting
            // PM_MESAJ_19: ?lang=en|tr sets the language (shared links, QA); otherwise the saved choice, English first
            string lang = Host.Current.Query("lang");
            if (lang == "tr" || lang == "en") Lang.Tr = lang == "tr";
            if (Host.Current.Query("lv") == "SHOW1" || Host.Current.Query("lv") == "SHOW_ART")
            {
                var show = Resources.Load<TextAsset>(Host.Current.Query("lv") == "SHOW_ART" ? "show_art" : "show");
                if (show != null) { _showSet = true; _newArt = Art.On = true; return LevelParser.Parse(show.text); }
            }
            Art.On = false;                                    // static: no domain reload between Play sessions
            _paintSet = false;
            // PM_MESAJ_48: level-agent preview sets (?set=ajan1 …) from Resources/AjanSets/<name>.json; recorded and unlocked like set=zeydbot
            string setName = Host.Current.Query("set");
            if (!string.IsNullOrEmpty(setName) && setName.StartsWith("ajan"))
            {
                var ajan = Resources.Load<TextAsset>("AjanSets/" + setName);
                if (ajan != null) { _zeydbotSet = !Host.Debug; _newArt = Art.On = _paintSet = true; return LevelParser.Parse(ajan.text); }
            }
            switch (SetChoice.For(Host.Debug, Host.Current.Query("set"), HasLevels(designs), HasLevels(redesign), SetFlags.RedesignDefault, HasLevels(zeydbot)))
            {
                case SetChoice.Kind.Zeydbot: _zeydbotSet = true; return LevelParser.Parse(zeydbot.text);
                case SetChoice.Kind.Designs: _designSet = true; return LevelParser.Parse(designs.text);
                // SET40 (PM_MESAJ_5): recorded like the ZeydBot set (moves, 1-10), also when the flag makes it the default
                case SetChoice.Kind.Redesign: _zeydbotSet = !Host.Debug; _redesignSet = true; _newArt = Art.On = _paintSet = true; return LevelParser.Parse(RedesignJson());
                default: return LevelParser.Parse(levels.text);
            }
        }

        /* ZeydBot: every action of the attempt with its time and the time spent before it (thinking). */
        void RecordMove(GameEvent e)
        {
            string kind;
            if (e.Kind == EventKind.PieceRemoved) kind = e.ByHammer ? "hammer" : "exit";
            else if (e.Kind == EventKind.Jammed) kind = "jam";
            else if (e.Kind == EventKind.BoosterUsed && e.Booster != Booster.Hammer) kind = e.Booster.ToString().ToLowerInvariant();
            else return;
            string dir = e.Kind == EventKind.PieceRemoved && !e.ByHammer ? e.Dir.ToString()
                       : e.Kind == EventKind.Jammed ? _drag?.Direction?.ToString() : null;   // jam: the way it was pulled (null: a press on a nail)
            WriteMove(kind, e.Piece, dir);
        }

        /* One moves row; touch rows carry how long the piece was held. Pause / resume and touches do not
           reset the thinking clock: think_ms runs from the last real action. */
        void WriteMove(string kind, int piece, string dir, int? holdMs = null)
        {
            if (_moveQueue == null || _session == null) return;
            float now = Time.unscaledTime;
            var fuses = string.Join(";", System.Linq.Enumerable.Select(_session.Bombs, b => b.Key + ":" + b.Value));
            int no = kind == "start" ? 0 : ++_moveNo;                  // start: move_no 0 (PM_MESAJ_25)
            _moveQueue.Add(_rowId + "-" + no, MoveRow.Json(_rowId, _save.Player, _session.Level.Id, _session.Level.Version, _session.Attempt,
                no, kind, piece, dir, (int)((now - _moveT0) * 1000), (int)((now - _moveLast) * 1000), _session.TimeLeft, fuses, holdMs,
                Cohort == "p21" && (kind == "exit" || kind == "jam" || kind == "tut") ? _moveInput : null));
            bool action = kind != "touch" && kind != "hide" && kind != "show" && kind != "start" && kind != "quit";
            if (action) _moveLast = now;
            // sent on hide, on quit and every 10 rows, so a closed tab loses at most the last few (PM_MESAJ_25)
            if (kind == "hide" || kind == "quit" || no % 10 == 0) _moveSender?.Flush();
        }

        static bool HasLevels(TextAsset file)
        {
            if (file == null) return false;
            var root = Core.Json.JsonReader.Parse(file.text);
            return root.TryGet("levels", out var list) && list.Count > 0;
        }

        /* lv as a number (1-based position) or as a level id. */
        bool TryLevelIndex(string lv, out int index)
        {
            index = 0;
            if (string.IsNullOrEmpty(lv)) return false;
            if (int.TryParse(lv, out int n)) { index = Mathf.Clamp(n, 1, _set.Levels.Count) - 1; return true; }
            for (int i = 0; i < _set.Levels.Count; i++)
                if (_set.Levels[i].Id == lv) { index = i; return true; }
            return false;
        }

        void OnVisibility(bool visible)
        {
            _pageHidden = !visible;
            if (!visible && _deferredLoss != null)
            {
                ResolveContinue("withdrawn", false);
                _continueResultOnReturn = true;
            }
            else if (!visible && _continuePreview) CloseContinue();
            if (_session != null && _session.Continues > 0 && !_session.IsOver)
            {
                if (visible) _continueQueue.Remove(_rowId + ":final");
                else RecordContinueFinal(_session.PreviewQuit(), false);
            }
            if (visible)
            {
                _continueSender?.Flush();
                if (_continueResultOnReturn) { _continueResultOnReturn = false; ShowResult(); }
            }
            if (_zeydbotSet && _mode == Mode.Playing && _session != null && !_session.IsOver) WriteMove(visible ? "show" : "hide", -1, null);
            if (visible) _telemetry.PageVisible(_mode == Mode.Playing ? _session : null, _rowId);
            else
            {
                _telemetry.PageHidden(_mode == Mode.Playing ? _session : null, _rowId);
                if (!_previewWins) _save.Store(_log);
            }
        }

        // ---- flow -------------------------------------------------------------------------

        void LeaveLevel()
        {
            _win21?.Destroy(); _win21 = null; _winPreview21 = false;
            ResetBoosters21();
            _continueResultOnReturn = false;
            if (_deferredLoss != null) ResolveContinue("withdrawn", false);
            CloseContinue();
            _celebration?.Destroy();
            _celebration = null;
            _celebrationResult = false;
            if (_session == null) return;
            if (!_session.IsOver) { WriteMove("quit", -1, null); _session.Quit(); }   // OnEvent logs the quit row
            _session.EventRaised -= OnEvent;
        }

        void ShowMenu()
        {
            CommitPendingHard();
            _rated = null;
            LeaveLevel();
            _session = null;
            if (PlayOverride.Exit != null) { PlayOverride.Exit(); return; }   // back to the level editor
            _mode = Mode.Menu;
            Layout();
            AskProfileIfDue();
        }

        /* PM_MESAJ_43: asked once, after "Next" or "Menu" on the first won level numbered 2 or more. On the next level
           it covers the intro card, which comes back when the question is answered (AnswerProfile). */
        bool _profileDue;
        void AskProfileIfDue()
        {
            if (!_profileDue) return;
            _profileDue = false;
            AskProfile();
        }

        void StartLevel(int index)
        {
            LeaveLevel();
            _index = Mathf.Clamp(index, 0, _set.Levels.Count - 1);
            var level = _set.Levels[_index];
            // G1: this level's picture and the next one's; let go of the rest
            string nextImage = ImageOf(_index + 1);
            _images.KeepOnly(ImageOf(_index), nextImage);
            _images.Get(nextImage);
            CommitPendingHard();
            _rated = null;                                     // answers come only while the result card is open
            _funAnswered = false;
            _askFun = false;
            _hardAnswered = _askHard = false;
            _session = _log.Start(level, Client, new Mulberry32((uint)System.Environment.TickCount));
            _session.TestMode = _testMode;
            _session.ClockFreezesTime = P21Boosters;
            _rowId = _telemetry.NewRowId();
            if (!_previewWins) _save.Store(_log);                                 // the attempt counter, before anything can go wrong
            _trainingUse = null;
            _tutorialClockHold = false;
            _inventory = _redesignSet ? new BoosterInventory(Host.Current.Store, level,
                i => i >= 0 && i + (_p21Flow ? 2 : 0) < _set.Levels.Count && _progress.Won(i + (_p21Flow ? 2 : 0))) : null;
            _inventory?.Apply(_session);
            _session.EventRaised += OnEvent;
            _boosters = new BoosterControls(_session);
            _moveT0 = _moveLast = Time.unscaledTime;
            _moveNo = 0;
            _flickInputs = _dragInputs = 0;
            WriteMove("start", -1, null);                     // PM_MESAJ_25: an attempt that never ends still shows up
            _mode = Mode.Playing;
            _endAt = -1;
            // web loadLevel(i): the intro shows on a level's first start, not on retries
            bool firstVisit = !_introSeen.Contains(level.Id);
            _introOpen = !P21Prelude && !P21OpeningLesson && Screens.HasIntro(level, !_p21Revised || _index != 5) && firstVisit;
            _introSeen.Add(level.Id);
            ResetScriptedTutorial(firstVisit);
            ResetP21Tutorial();
            Layout();
            _board?.Deal();                                    // web render(true): pieces deal in on a fresh start
            TestBooster21();
            TestWin21();
            if (Host.Debug) Debug.Log("[level-ready] " + level.Id);   // PM_MESAJ_28 B2: verify_build --only perf times this
            AskProfileIfDue();
            if (ContinueEnabled && Host.Current.Query("devam") == "1")
            {
                _continuePreview = true;
                _continueOffer = _continueRules.Offer(ContinueKind.Time, 0, level.TimerSeconds, _save.Coins);
                ShowContinue();
            }
            else if (Art.T54 && Host.Current.Query("jigo") == "sevinc") BeginCelebration(false);
        }

        /* Zeyd 2026-10-28: a new browser save (Safari's 7-day clean-up, Chrome or WhatsApp's own browser, another
           phone) loses the progress, but every win is already a `plays` row under the player's name. When the save is
           empty and a name is typed, those rows bring the best stars back (anon read, the schema untouched). */
        System.Collections.IEnumerator RecoverProgress(string name)
        {
            if (_resetVisit) yield break;
            var host = Host.Current;
            if (string.IsNullOrEmpty(host.SupabaseUrl) || string.IsNullOrEmpty(host.AnonKey) || string.IsNullOrEmpty(name)) yield break;
            string url = host.SupabaseUrl + "/rest/v1/plays?select=level_id,stars&result=in.(win,won_after_continue)&player=eq." + UnityEngine.Networking.UnityWebRequest.EscapeURL(name);
            using (var req = UnityEngine.Networking.UnityWebRequest.Get(url))
            {
                req.SetRequestHeader("apikey", host.AnonKey);
                req.SetRequestHeader("Authorization", "Bearer " + host.AnonKey);
                yield return req.SendWebRequest();
                if (req.result != UnityEngine.Networking.UnityWebRequest.Result.Success) { if (Host.Debug) Debug.Log("[recover] " + req.error); yield break; }
                var best = new Dictionary<string, int>();
                try
                {
                    var rows = ReverseSolver.Core.Json.JsonReader.Parse(req.downloadHandler.text);
                    if (rows.Kind == ReverseSolver.Core.Json.JsonKind.Array)
                        foreach (var row in rows.Items)
                            if (row.TryGet("level_id", out var id) && row.TryGet("stars", out var st) && st.Kind == ReverseSolver.Core.Json.JsonKind.Number)
                                best[id.AsString] = System.Math.Max((int)st.AsNumber, best.TryGetValue(id.AsString, out int b) ? b : 0);
                }
                catch (System.Exception) { yield break; }
                int n = _save.MergeBest(best);
                if (Host.Debug) Debug.Log($"[recover] {name}: {best.Count} won levels on the server, {n} restored");
                if (n == 0) yield break;
                _log = _save.Restore();
                _progress = new Progress(_log, _set.Levels) { UnlockAll = _progress.UnlockAll };
                if (_mode == Mode.Menu) Layout();
            }
        }

        void ResetScriptedTutorial(bool firstVisit)
        {
            _tutorialScript ??= TutorialScript.Load();
            int slot = _index + 1 - (_p21Flow ? 2 : 0);
            // Reuse the bomb spotlight on its new teaching board.
            if (_p21Revised && slot == 6) slot = 0;
            else if (_p21Revised && slot == P21Onboarding.BombLesson) slot = 6;
            _tutorialSteps = _redesignSet && !PlayOverride.Active && !P21Prelude && !P21OpeningLesson && !P21StoryLevel
                ? _tutorialScript.ForSlot(slot, firstVisit) : new List<TutorialStep>();
            _tutorialSteps.RemoveAll(step => (step.tur == "booster" || step.tur == "booster_hedef") &&
                (!TutorialScript.BoosterOf(step.booster, out var b) || _inventory == null || _inventory.Trained(b) || !_session.IsUnlocked(b)));
            _tutorialAt = 0;
            _tutorialUntil = -1;
            _session.TutorialMove = false;
        }

        /* Scripted SET40 steps, with the old hand retained for other tutorials. */
        void ShowTutorialStep()
        {
            if (P21StoryLevel) return;
            if (_redesignSet && !PlayOverride.Active && _tutorialScript.ForSlot(_index + 1, true).Count > 0)
            {
                ShowScriptedTutorial();
                return;
            }
            if (_board == null || !Tutorial.Step(_session, out var m)) return;
            // Zeyd 2026-10-28: the hand on level 1's first two moves only, never on level 2 (tutorial 2)
            int done = _session.Level.Pieces.Count - _session.Board.PresentCount;
            if (_session.Level.Tutorial != 1 || done >= 2) return;
            var origin = PlayingBoardPlacement(_screen, _safe, _session.Level, out _);
            _hand = TutorialHand.Show(_board, _board.PieceViewOf(m.Piece), m, HandOrder, new Rect(-origin, _screen));
            if (_session.Level.Tutorial == 1 || done == 0) _bar?.Notice(done == 0 ? Texts.TutorialFirst : Texts.TutorialNext, 2.5f);
        }

        void ShowScriptedTutorial()
        {
            if (_spotlight != null) Destroy(_spotlight.gameObject);
            _spotlight = null;
            bool boosterStep = TutorialActive && (_tutorialSteps[_tutorialAt].tur == "booster" || _tutorialSteps[_tutorialAt].tur == "booster_hedef");
            _bar?.ShowTutorial(TutorialActive && !_introOpen && !boosterStep);
            if (_introOpen || _board == null) return;
            while (TutorialActive)
            {
                var step = _tutorialSteps[_tutorialAt];
                bool valid = step.tur == "kural" || (step.tur == "goster" && TutorialScript.ObstacleOf(_session.Level, step.engel, out _, out _));
                if (step.tur == "surukle")
                    valid = TutorialScript.MoveOf(_session.Level, step, out _tutorialMove) &&
                        (_tutorialUntil >= 0 || (_session.Board.IsPresent(_tutorialMove.Piece) && _session.NailAt(_tutorialMove.Piece) == 0 && _session.Probe(_tutorialMove.Piece, _tutorialMove.Dir).Exit));
                if (step.tur == "hata")
                {
                    valid = _tutorialUntil >= 0 || TutorialScript.ErrorMove(_session, step, out _tutorialMove);
                    if (valid && _tutorialAt + 1 < _tutorialSteps.Count && _tutorialSteps[_tutorialAt + 1].tur == "surukle")
                    {
                        var next = _tutorialSteps[_tutorialAt + 1];
                        _tutorialSteps[_tutorialAt + 1] = new TutorialStep { slot = next.slot, adim = next.adim,
                            tur = next.tur, parca = _tutorialMove.Piece, metinAnahtari = next.metinAnahtari };
                    }
                }
                _session.TutorialMove = valid && step.tur == "hata";
                if (TutorialScript.BoosterOf(step.booster, out var booster))
                {
                    if (step.tur == "booster")
                    {
                        valid = true;
                        // Preview the free use at zero stock without altering persistent inventory.
                        if (_tutorialUntil < 0) _session.FreeUse = booster;
                    }
                    else if (step.tur == "booster_hedef")
                        valid = _tutorialUntil >= 0 || TutorialScript.BoosterTarget(_session, booster, out _boosterTarget, out _boosterJoint);
                }
                if (valid)
                {
                    _spotlight = boosterStep
                        ? TutorialSpotlight.ShowBooster(_world, _screen, _board, step, _bar.RectOf(booster), _boosterTarget, _boosterJoint)
                        : TutorialSpotlight.Show(_world, _screen, _board, step, _tutorialMove);
                    if (_tutorialUntil >= 0)
                    {
                        if (step.tur == "hata") _spotlight.Stuck(); else _spotlight.Success();
                    }
                    return;
                }
                Debug.LogWarning($"Tutorial: skipped slot {_index + 1}, step {step.adim} (missing target or invalid move).");
                _tutorialAt++;
            }
            _bar?.ShowTutorial(false);
        }

        void NextScriptedTutorial()
        {
            if (_drag.Active)
            {
                var limit = _drag.Limit;
                var result = _drag.Release(out int piece, out var dir);
                _board.EndDrag(piece, result, dir, limit.Cells);
            }
            _session.TutorialMove = false;
            _tutorialAt++;
            _tutorialUntil = -1;
            ShowScriptedTutorial();
        }

        // Run before DragModel.Move: even probing a wrong direction must not record a jam.
        bool RejectTutorialDirection(Vector2 pt)
        {
            if (!TutorialActive || _tutorialUntil >= 0 ||
                (_tutorialSteps[_tutorialAt].tur != "surukle" && _tutorialSteps[_tutorialAt].tur != "hata") || !_drag.Active) return false;
            var delta = pt - _tutorialPress;
            if (!TutorialScript.WrongDirection(_tutorialMove.Dir, _drag.Direction, delta.x, delta.y, out var dir)) return false;
            int piece = _drag.Piece;
            _drag.Cancel();
            _board.TutorialShake(piece, dir);
            _spotlight?.Restart();
            return true;
        }

        void MenuAction(Vector2 pt)
        {
            if (Art.On)
            {
                if (_profileCard != null) { ProfileAction(_profileCard.Hit(pt)); return; }
                if (_menuSettings != null) { SettingsAction(_menuSettings.Hit(pt)); return; }
                if (_paintingDetail != null)
                {
                    string action = _paintingDetail.Hit(pt);
                    if (action == "replay") { int index = _paintingDetail.Index; _paintingDetail.Destroy(); _paintingDetail = null; StartLevel(index); }
                    else if (action == "close") { _paintingDetail.Destroy(); _paintingDetail = null; }
                    return;
                }
                int tab = _tabs?.Hit(pt) ?? -1;
                if (tab >= 0)
                {
                    if (_tabAnimating) return;
                    RememberMenuScroll(); _tabAnimating = true;
                    _tabs.Press(tab, () => { _tabAnimating = false; if (_mode != Mode.Menu) return; _tab = (MenuTab)tab; _levelsMapOpen = false; Layout(); });
                    return;
                }
                if (_tabs != null && pt.y >= _tabs.Top - 24) return;
                if (_levelsMap != null)
                {
                    int hit = _levelsMap.Hit(pt);
                    if (hit == -2) { RememberMenuScroll(); _levelsMapOpen = false; Layout(); }
                    else if (hit >= 0) StartLevel(hit);
                    return;
                }
                if (_collection != null)
                {
                    int hit = _collection.Hit(pt);
                    if (hit >= 0)
                    {
                        if (!_progress.Won(hit) && hit == _progress.Next()) StartLevel(hit);
                        else _paintingDetail = new PaintingDetail(_world, _screen, hit, _images, CardOrder, i => _stats.Of(_set.Levels[i].Id)?.BestWinSeconds);
                    }
                    return;
                }
                if (_jiggles != null)
                {
                    int avatar = _jiggles.Hit(pt);
                    if (avatar >= 0) { Art.AvatarIndex = avatar; RememberMenuScroll(); Layout(); }
                    return;
                }
            }
            if (_menu == null) return;
            if (_menu.Settings.Contains(pt)) OpenSettings();
            else if (_menu.Profile.Contains(pt) && Art.On) OpenPlayerProfile();
            else if (_menu.Play.Contains(pt)) StartLevel(_progress.Next());
            else if (_menu.Levels.Contains(pt))
            {
                if (Art.On) { _levelsMapOpen = true; _mapOffset = null; Layout(); }
                else OpenList();
            }
        }

        void RememberMenuScroll()
        {
            if (_collection != null) _collectionOffset = _collection.Scroll.Resting;
            if (_levelsMap != null) _mapOffset = _levelsMap.Scroll.Resting;
            if (_jiggles != null) _jigglesOffset = _jiggles.Scroll.Resting;
            if (_achievements != null) _achievementsOffset = _achievements.Scroll.Resting;
        }

        AchievementProgress[] CurrentAchievements() => Achievements.Compute(_set.Levels, _progress.Won, _stats, _save);

        void OpenPlayerProfile()
        {
            int played = 0, first = 0;
            for (int i = 0; i < _set.Levels.Count; i++)
            {
                string id = _set.Levels[i].Id;
                if (_log.AttemptsOf(id) > 0) played++;
                if (_stats.FirstTry(id, _progress.Won(i), _save)) first++;
            }
            string[] values = { $"{_progress.Next() + 1}/{_set.Levels.Count}", Texts.Count(SavedJiggles()), Texts.Minutes(_stats.TotalSeconds / 60),
                played == 0 ? "—" : $"{first * 100 / played}%", $"{Achievements.Completed(CurrentAchievements())}/9",
                _stats.WinCount == 0 ? "—" : Texts.Seconds(_stats.SumLeftOnWins / _stats.WinCount) };
            _profileCard?.Destroy();
            _profileCard = new ProfileCard(_world, _screen, _save.Player, values, CardOrder);
        }

        void ProfileAction(string action)
        {
            if (action == null) return;
            _profileCard.Destroy(); _profileCard = null;
            if (action == "avatar") { RememberMenuScroll(); _tab = MenuTab.Jiggles; _levelsMapOpen = false; Layout(); }
            else if (action == "name")
            {
                _awaitingName = true;
                Host.Current.AskName(name =>
                {
                    _save.SetPlayer(name); _awaitingName = false;
                    if (_mode == Mode.Menu) { Layout(); OpenPlayerProfile(); }
                });
            }
        }

        void SettingsAction(string id)
        {
            if (id == null) return;
            if (id == "story")
            {
                _menuSettings?.Destroy(); _menuSettings = null;
                CloseCard();
                PlayStory("ABC", true, ShowMenu);
                return;
            }
            if (id == "privacy") { Application.OpenURL(MenuSettings.PrivacyUrl); return; }
            if (id == "contact") { Application.OpenURL(MenuSettings.ContactUrl); return; }
            if (id == "sound") Art.Muted = !Art.Muted;
            else if (id == "music") Music.On = !Music.On;
            else if (id == "haptics") Haptics.On = !Haptics.On;
            else if (id == "tr" || id == "en")
            {
                Lang.Tr = id == "tr"; RememberMenuScroll(); Layout(); OpenSettings(); return;
            }
            _menuSettings.Destroy(); _menuSettings = null;
            if (id != "close") OpenSettings();
        }

        int TotalStars()
        {
            int n = 0;
            for (int i = 0; i < _set.Levels.Count; i++) n += _progress.Stars(i);
            return n;
        }

        /* PM_MESAJ_9 profile box: every piece of a won level is a rescued Jiggle. */
        int SavedJiggles()
        {
            int n = 0;
            for (int i = 0; i < _set.Levels.Count; i++) if (_progress.Won(i)) n += _set.Levels[i].Pieces.Count;
            return n;
        }

        void OpenSettings()
        {
            if (Art.On) { _menuSettings?.Destroy(); _menuSettings = new MenuSettings(_world, _screen, CardOrder); return; }
            OpenCard("settings", new Card(_world, _screen, CardOrder, "Settings") { Compact = true }.Title(Texts.Settings)
            .Button("sound", Art.Muted ? Texts.SoundOff : Texts.SoundOn, false)
            .Button("music", Music.On ? Texts.MusicOn : Texts.MusicOff, false)
            .Button("haptics", Haptics.On ? Texts.HapticsOn : Texts.HapticsOff, false)
            .Button("lang", Texts.Language, false)
            .Button("close", Texts.Close, true).Finish());
        }

        AudioSource _audio;
        void PlaySound(string key)
        {
            var clip = Art.Sound(key);
            if (clip == null || Art.Muted) return;
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _audio.PlayOneShot(clip);
        }

        AudioSource _late;
        void PlaySoundLater(string key, float delay)   // the win fanfare waits for the last piece's long sparkle
        {
            var clip = Art.Sound(key);
            if (clip == null || Art.Muted) return;
            if (_late == null) _late = gameObject.AddComponent<AudioSource>();
            _late.clip = clip;
            _late.PlayDelayed(delay);
        }

        void OnEvent(GameEvent e)
        {
            OnP21TutorialEvent(e);
            OnP21LockedAttempt(e);
            _hud?.OnEvent(e, _session);
            if (_zeydbotSet && e.Kind == EventKind.Jammed && _session.TutorialMove) WriteMove("tut", e.Piece, _drag?.Direction?.ToString());
            else if (_zeydbotSet) RecordMove(e);
            if (e.Kind == EventKind.Jammed || (e.Kind == EventKind.PieceRemoved && !e.ByHammer)) _tutorialClockHold = false;
            if (e.Kind == EventKind.Jammed && TutorialActive && _tutorialSteps[_tutorialAt].tur == "hata" &&
                _tutorialUntil < 0 && e.Piece == _tutorialMove.Piece)
            {
                _tutorialUntil = Time.unscaledTime + 2f;
                _spotlight?.Stuck();
            }
            if (e.Kind == EventKind.TimeAdded) _hud?.ShowTimeBonus(e.Value);
            if (!_testMode && _inventory != null && e.Kind == EventKind.BoosterUsed)
            {
                if (_trainingUse == e.Booster)
                {
                    _inventory.Complete(e.Booster);
                    _session.SetStock(e.Booster, _inventory.Stock(e.Booster));
                    _trainingUse = null;
                    // Core's hammer Remove starts its clock. Hold presentation ticks until a normal move.
                    _tutorialClockHold = true;
                    _tutorialUntil = Time.unscaledTime + .2f;
                    _spotlight?.Success();
                }
                else { _inventory.Spend(e.Booster); _tutorialClockHold = false; }
            }
            if (e.Kind == EventKind.ValveReleased) _bar?.Notice(Texts.NailReleased, 1.6f);   // web says "söktü"; PM: grammar fix
            if (e.Kind == EventKind.PieceRemoved)
            {
                _tutorialDone = true;                          // web clearTutorial() in flyOut and the hammer
                if (_hand != null) { _hand.Remove(); _hand = null; }
                if (TutorialActive && _tutorialSteps[_tutorialAt].tur == "surukle" && e.Piece == _tutorialMove.Piece)
                {
                    _tutorialUntil = Time.unscaledTime + .3f;
                    _spotlight?.Success();
                }
                else if (_session.Level.Tutorial > 0) ShowTutorialStep();
            }
            // Codex sounds (preview art only; silent when Art is off)
            // PM_MESAJ_19: no sound per piece; the jam keeps its knock; the last piece gets the whole sparkle, then the fanfare
            if (e.Kind == EventKind.PieceRemoved)
            {
                if (P21JuiceView.Enabled) { PlaySound("pop"); Haptics.Play(Haptics.Kind.Light); }
                else if (_session.Board.PresentCount == 0) PlaySound("popfull");
                else Haptics.Play(Haptics.Kind.Light);
            }
            else if (P21JuiceView.Enabled && (e.Kind == EventKind.NailFreed || e.Kind == EventKind.JointCut)) PlaySound("piece");
            else if (e.Kind == EventKind.Jammed) { PlaySound("piece"); Haptics.Play(Haptics.Kind.Medium); }
            else if (e.Kind == EventKind.Finished && e.Outcome == Outcome.Win) PlaySoundLater("win", .7f);
            else if (e.Kind == EventKind.Finished && e.Outcome != Outcome.Quit) PlaySound("loss");
            if (e.Kind != EventKind.Finished) return;
            Debug.Log($"[input] flick={_flickInputs} drag={_dragInputs}");
            if (ContinueEnabled && (e.Outcome == Outcome.Time || e.Outcome == Outcome.Bomb))
                _continueOffer = _continueRules.Offer(e.Outcome == Outcome.Time ? ContinueKind.Time : ContinueKind.Bomb,
                    _session.Continues, _session.Level.TimerSeconds, _save.Coins);
            if (_continueOffer != null) _deferredLoss = _session.Record;
            else CommitFinished();
            _drag?.Cancel();
            _boosters?.Disarm();
            if (e.Outcome == Outcome.Quit) return;
            if (e.Outcome == Outcome.Bomb) { Flash(Draw.Hex("#e8674c"), .45f); Haptics.Play(Haptics.Kind.Heavy); }   // bomb: red flash + swell
            if (e.Outcome == Outcome.Win) { Haptics.Play(Haptics.Kind.Success); Music.Duck(.6f, 2f); }   // the fanfare comes at .7 s
            _endAt = Time.unscaledTime + (e.Outcome == Outcome.Win ? .35f : .5f);
            // PM_MESAJ_18: the pieces are out, the painting is free; it stays whole for a second before the card
            if (!P21WinArt && e.Outcome == Outcome.Win && _paintSet && _board != null && _board.Image != null) { ShowFinishedPicture(true); _endAt += 1f; }
        }

        // Only this method commits an outcome; offered losses wait here until a decision.
        void CommitFinished()
        {
            _log.Add(_session.Record);
            if (!_previewWins)
            {
                _stats.Add(_session.Record, _session.Level.TimerSeconds);
                _stats.Save(Host.Current.Store);
            }
            if (!_previewWins) _save.Store(_log);
            if (!_previewWins && P21WinArt && _session.Outcome == Outcome.Win) _save.RecordChestWin21(_session.Level.Number);
            // G8: the attempt that gets the 👍/👎 question waits for the answer (or the card closing)
            // before its row goes; asked once per level, on the win or after the third loss
            string levelId = _session.Level.Id;
            // PM_MESAJ_41 (Zeyd): no end-of-level rating; the row goes with empty zorluk_puani/keyif, set_rating stays unused
            const bool askRatings = false;
            bool ask = askRatings && !_showSet && SetFlags.AskFun && FunAsk.ShouldAsk(_session.Outcome, _save.LossesOf(levelId), _save.WasFunAsked(levelId));
            if (SetFlags.AskFun && (_session.Outcome == Outcome.Time || _session.Outcome == Outcome.Bomb)) _save.RecordLoss(levelId);
            _askFun = ask;
            _askHard = askRatings && _session.Outcome != Outcome.Quit && !_showSet && AskHardNow(levelId);
            if (_askHard) HardAsked(levelId);                       // asked once per level (cohort), answered or skipped
            if (ask) _save.MarkFunAsked(levelId);
            // PM_MESAJ_29: the row goes now, empty answers; the 1-10 and 👍/👎 fill it later (rpc/set_rating)
            _rated = ask || _askHard ? (_session.Record, _rowId) : null;
            _telemetry.Finished(_session.Record, _rowId);
            if (Host.Debug) Debug.Log("[attempt] " + _session.Record);
            if (!_previewWins && ContinueEnabled && _session.Outcome == Outcome.Win)
                _save.SetCoins((int)System.Math.Min(int.MaxValue, (long)_save.Coins + _continueRules.WinCoins));
            if (_session.Continues > 0) RecordContinueFinal(_session.Record, true);
        }

        void ShowContinue()
        {
            CancelCardTap();
            _continuePanel?.Destroy();
            _continuePanel = new ContinuePanel(_world, _screen, _safe, CardOrder + 100,
                _continueOffer, _save.Coins, Host.Current.Query("devamhayir") == "1", _continuePreview);
        }

        void CloseContinue()
        {
            _continuePanel?.Destroy();
            _continuePanel = null;
            _continueOffer = null;
            _continuePreview = false;
        }

        void ResolveContinue(string decision, bool showResult = true)
        {
            if (_continuePreview) { CloseContinue(); return; }
            if (_deferredLoss == null || _continueOffer == null) return;
            var offer = _continueOffer;
            int before = _save.Coins;
            if (decision == "accepted" && before < offer.Price) return;
            if (decision == "accepted")
            {
                _save.SetCoins(before - offer.Price);
                if (!_session.Continue(offer.Seconds, _continueRules.BombFuse)) { _save.SetCoins(before); return; }
                RecordContinueDecision(offer, before, decision);
                _endAt = -1;
                _deferredLoss = null;
                CloseContinue();
                CloseCard();
                RefreshPlaying();
            }
            else
            {
                RecordContinueDecision(offer, before, decision);
                _endAt = -1;
                _deferredLoss = null;
                CloseContinue();
                CommitFinished();
                if (showResult) ShowResult();
            }
        }

        void RecordContinueDecision(ContinueOffer offer, int coinsBefore, string decision)
        {
            if (!_continueSender.Enabled) return;
            string id = System.Guid.NewGuid().ToString("N");
            _continueQueue.Add(id, ContinueRow.Json(id, _rowId, _save.Player, _deferredLoss,
                _deferredLoss.Continues + 1, offer.Kind == ContinueKind.Time ? "time" : "bomb",
                offer.Price, offer.Seconds, coinsBefore, decision));
            if (!_pageHidden) _continueSender.Flush();
        }

        void RecordContinueFinal(AttemptRecord record, bool flush)
        {
            if (!_continueSender.Enabled) return;
            string id = _rowId + ":final";
            _continueQueue.Remove(id);
            _continueQueue.Add(id, ContinueRow.Json(id, _rowId, _save.Player, record, record.Continues,
                "final", 0, 0, _save.Coins, record.Result == "win" ? "won_after_continue" : record.Result));
            if (flush && !_pageHidden) _continueSender.Flush();
        }

        /* The level's picture: its own (G1) or, in SET40 v3, painting-NN for level N (manifest order). */
        string ImageOf(int index) =>
            index < 0 || index >= _set.Levels.Count || _p21Flow && index < 2 ? null : _set.Levels[index].Image ?? (_paintSet ? $"painting-{index + 1 - (_p21Flow ? 2 : 0):00}" : null);

        /* The whole picture over the emptied board, as one flat-edged piece. */
        void ShowFinishedPicture(bool pop)
        {
            var lv = _session.Level;
            float c = _board.Cell;
            var holder = new GameObject("finished").transform;
            holder.SetParent(_board.transform, false);
            holder.localPosition = Draw.W(lv.Width * c / 2, lv.Height * c / 2);
            PicturePiece.Create(holder, _board.Image, new Vector2(lv.Width * c, lv.Height * c), 1250);
            if (pop) Tweens.Run(holder, .3f, t => { if (holder != null) holder.localScale = Vector3.one * Mathf.Lerp(.9f, 1f, 1 - (1 - t) * (1 - t)); });
        }

        // ---- layout -----------------------------------------------------------------------

        void Layout()
        {
            _screenPx = new Vector2Int(Screen.width, Screen.height);
            _dpr = Mathf.Max(1f, Host.Current.PixelRatio);
            _screen = new Vector2(Screen.width, Screen.height) / _dpr;
            _safe = Host.Current.SafeInsets / (Application.isEditor ? _dpr : 1f);
            Build();
            _celebration?.Layout(_screen);
            _win21?.Layout(_screen, WinTargets21());
            if (_continuePreview || _deferredLoss != null && _endAt < 0) ShowContinue();
        }

        /* immediate: screenshots build several screens in one frame, so the
           old world must be gone before the next one is drawn. */
        void Build(bool immediate = false)
        {
            RememberMenuScroll();
            if (_world != null) { if (immediate) DestroyImmediate(_world.gameObject); else Destroy(_world.gameObject); }
            _world = new GameObject("World").transform;
            CancelCardTap();
            _continuePanel = null;
            _card = null; _cardKind = null; _menu = null; _list = null; _board = null; _hud = null; _bar = null;
            _tabs = null; _collection = null; _levelsMap = null; _paintingDetail = null; _menuSettings = null;
            _jiggles = null; _achievements = null; _profileCard = null;
            _menuPointer = _menuDragging = _tabAnimating = false;
            _jointsShown = false;

            _cam.orthographic = true;
            _cam.orthographicSize = _screen.y / 2;
            _cam.transform.position = new Vector3(_screen.x / 2, -_screen.y / 2, -10);
            Background();

            if (_mode == Mode.Menu)
            {
                var menuSafe = _safe;
                if (Art.On)
                {
                    menuSafe.y += 4;   // PM_MESAJ_27 A1: the header from the safe top + 8 pt (its 60 pt tiles centre 34 pt below)
                    var achievements = CurrentAchievements();
                    int completed = Achievements.Completed(achievements);
                    int.TryParse(Host.Current.Store.Get("rs_ach_seen"), out int seen);
                    if (_tab == MenuTab.Trophies && !_levelsMapOpen)
                    {
                        if (!_previewWins && completed != seen) { Host.Current.Store.Set("rs_ach_seen", completed.ToString()); Host.Current.Store.Save(); }
                        seen = completed;
                    }
                    _tabs = new MenuTabs(_world, _screen, _safe, _tab, BarOrder + 100, Mathf.Max(0, completed - seen));
                    float bottom = _tabs.Top - 28;
                    if (_levelsMapOpen) _levelsMap = new LevelsMap(_world, _screen, menuSafe.y, bottom, _set.Levels, _progress, _images, HudOrder, _mapOffset);
                    else if (_tab == MenuTab.Collection) _collection = new CollectionScreen(_world, _screen, menuSafe.y, bottom, _set.Levels, _progress, _images, HudOrder, _collectionOffset);
                    else if (_tab == MenuTab.Jiggles) _jiggles = new JigglesScreen(_world, _screen, menuSafe.y, bottom, _set.Levels, _progress, HudOrder, _jigglesOffset);
                    else if (_tab == MenuTab.Trophies) _achievements = new AchievementsScreen(_world, _screen, menuSafe.y, bottom, achievements, HudOrder, _achievementsOffset);
                    menuSafe.w = _screen.y - _tabs.Top;   // Home: the Level button sits 16 pt above the bar
                }
                if (!Art.On || (!_levelsMapOpen && _tab == MenuTab.Home))
                    _menu = new Screens.Menu(_world, _screen, menuSafe, _p21Flow ? _progress.Next() + 1 : _set.Levels[_progress.Next()].Number, HudOrder, _newArt, _save.Player, SavedJiggles(), TotalStars(), _showSet ? "Demo" : null, _set.Levels[_progress.Next()].Hard);
                // PM_MESAJ_18: the corridor's big frame shows the painting the next level frees
                if (_paintSet && _newArt && _menu != null)
                {
                    var menu = _menu;
                    var pic = _images.Get(ImageOf(_progress.Next()), t => { if (_menu == menu) menu.SetPainting(t); });
                    if (pic != null) _menu.SetPainting(pic);
                }
                MenuArt.Busy = () => this != null && _mode == Mode.Playing;
                MenuArt.Instance.Preload();
            }
            else
            {
                _hud = new Hud(_world, _screen.x, _safe.y + BodyPad, HudOrder, _screen, _safe.y, _session, UseHud54);
                _hud.SetLevel(_p21Flow ? _index + 1 : _session.Level.Number);
                var origin = PlayingBoardPlacement(_screen, _safe, _session.Level, out float cell);
                _board = BoardView.Create(_world, _session, cell, origin);
                // Zeyd 2026-10-26 chose the thick dark navy edge (#06161D, 1.5x rim, 80 % cream band, soft halo) for painted boards
                Shader.SetGlobalFloat("_RSEdge", _showSet || _paintSet ? 1 : 0);
                if (_showSet)
                {
                    var paint = Art.Texture("painting-test-01");
                    if (paint != null) _board.SetImage(paint);
                }
                // PM_MESAJ_18: SET40 v3 levels wear their own painting; until (or unless) it arrives the board is plain
                string image = _showSet ? null : ImageOf(_index);
                if (image != null)
                {
                    var board = _board;
                    var ready = _images.Get(image, t => { if (board != null && _board == board) board.SetImage(t, .25f); });
                    if (ready != null) _board.SetImage(ready);
                }
                if (_session.IsOver && _session.Outcome == Outcome.Win && _paintSet && _board.Image != null) ShowFinishedPicture(false);
                _hand = null;
                bool firstLevel = _index == 0 && !_designSet && !PlayOverride.Active && !_p21Flow && !P21OpeningLesson && !P21StoryLevel;
                if (!TutorialActive && _session.Level.Tutorial == 0 && Tutorial.Hint(_session, firstLevel, _tutorialDone, out var hint))
                    _hand = TutorialHand.Show(_board, _board.PieceViewOf(hint.Piece), hint, HandOrder,
                                              new Rect(-origin, _screen));
                _drag = new DragModel(new SessionDragTarget(_session), cell);
                _bar = new BoosterBar(_world, _screen, _safe.w, BarOrder, UseHud54);
                if (_session.Level.Tutorial > 0 || TutorialActive) ShowTutorialStep();
                if (_introOpen) OpenCard("intro", Screens.Intro(_world, _screen, _session.Level, CardOrder, P21ObstacleLesson));
                else if (_session.IsOver && _endAt < 0 && _celebration == null && _deferredLoss == null) ShowResult();
                RefreshPlaying();
                BuildP21Tutorial();
                BuildBoosters21();
            }

            if (Host.Debug && Host.Current.Query("hud") != "0")   // ?hud=0: clean QA shots
            {
                _debug = TextView.Create(_world, "debug", "", 11, Draw.Hex("#8fb2b3"), false,
                                         TMPro.TextAlignmentOptions.Left, DebugOrder);
                // in the gap between the strip and the tray, clear of the booster bar
                float dy = _safe.y + BodyPad + Hud.TopPad + Hud.Height + 14;
                _debug.transform.localPosition = Draw.W(Hud.Side + 5, dy);
            }
            if (Host.Current.Query("fps") == "1")
            {
                _fpsLabel = TextView.Create(_world, "fps", "FPS --", 13, Palette.Cream, true,
                                            TMPro.TextAlignmentOptions.Left, DebugOrder).Outline(Palette.Navy, .25f);
                _fpsLabel.transform.localPosition = Draw.W(Hud.Side + 5, _safe.y + BodyPad + Hud.TopPad + Hud.Height + 14);
            }
        }

        /* The web game's sizeCell and flex placement: the board's top-left in
           points and the cell size. Safe insets play the role of the body's
           env(safe-area-inset-*) padding. */
        Vector2 PlayingBoardPlacement(Vector2 screen, Vector4 safe, LevelData l, out float cell)
        {
            if (!UseHud54) return BoardPlacement(screen, safe, l, out cell);
            cell = BoardSizing.CellSize(screen, safe, l);
            float top = Hud54.TopBottom(screen, safe.y), bottom = Hud54.ButtonsTop(screen, safe.w);
            if (P21OpeningLesson) bottom -= P21TutorialView.ChatClearance;
            // Include the tray and sealed-edge walls in the clearance.
            float margin = Mathf.Max(18.2f, cell * .25f);
            if (l.Height * cell + 2 * margin > bottom - top)
                cell = Mathf.Floor(Mathf.Min(cell, (bottom - top - 36.4f) / l.Height, (bottom - top) / (l.Height + .5f)));
            return new Vector2(BoardSizing.Left(screen.x, safe, l.Width * cell), (top + bottom - l.Height * cell) / 2);
        }

        public static Vector2 BoardPlacement(Vector2 screen, Vector4 safe, LevelData l, out float cell)
        {
            cell = BoardSizing.CellSize(screen, safe, l);
            var size = new Vector2(l.Width * cell, l.Height * cell);
            float stageTop = safe.y + BodyPad + Hud.TopPad + Hud.Height + StripBottom;
            float stageBottom = screen.y - safe.w - BodyPad - BottomReserve;
            return new Vector2(BoardSizing.Left(screen.x, safe, size.x), (stageTop + stageBottom - size.y) / 2);
        }

        void Background()
        {
            var go = new GameObject("Background");
            go.transform.SetParent(_world, false);
            var mesh = new Mesh { name = "bg" };
            mesh.vertices = new[] { Draw.W(0, 0), Draw.W(_screen.x, 0), Draw.W(_screen.x, _screen.y), Draw.W(0, _screen.y) };
            mesh.uv = new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = Materials.Background;
            r.sortingOrder = -1000;
            // Codex Task 5 game background, cover-fitted over the drawn gradient (preview art only)
            var art = Art.Place(go.transform, "game-background", Vector2.one, -999);
            if (art != null)
            {
                var sz = art.sprite.bounds.size;
                float k = Mathf.Max(_screen.x / sz.x, _screen.y / sz.y);
                art.transform.localScale = new Vector3(k, k, 1);
                art.transform.localPosition = Draw.W(_screen.x / 2, _screen.y / 2);
            }
            var b = new MaterialPropertyBlock();
            b.SetVector("_Screen", _screen);
            r.SetPropertyBlock(b);
        }

        void Flash(Vector4 color, float duration)
        {
            var f = ShapeView.Create(_world, "flash", _screen, 0, FlashOrder).Radius(0).Fill(color);
            f.transform.localPosition = Draw.W(_screen / 2);
            Tweens.Run(f, duration, t => { if (f != null) f.Alpha(.35f * (1 - t)); }, () => { if (f != null) Destroy(f.gameObject); });
        }

        void OpenCard(string kind, Card card)
        {
            CloseCard();
            _card = card;
            _cardKind = kind;
        }

        void CloseCard()
        {
            CancelCardTap();
            _card?.Destroy();
            _card = null;
            _cardKind = null;
        }

        void ShowResult()
        {
            if (P21WinArt && _session.Outcome == Outcome.Win) { ShowWin21(); return; }
            if (_deferredLoss != null) { ShowContinue(); return; }
            CancelCardTap();
            _card?.Destroy();
            _card = null;
            _cardKind = "result";
            _card = Screens.Result(_world, _screen, _session, _index == _set.Levels.Count - 1, CardOrder,
                PlayOverride.Active ? Texts.BackToEditor : string.Format(Texts.SetComplete, _set.Levels.Count),
                _images.Get(_session.Level.Image),          // G1-b: only a picture that really arrived
                _askFun, _funAnswered, _askHard, _hardAnswered, _showSet, _pendingHard);
        }

        /* PM_MESAJ_24: win -> (1-10, 👍/👎) -> Next -> corridor slide -> next level. Only in the painting set;
           the last level has no Next, so its end card is unchanged. The next painting was asked for at StartLevel;
           if it is still on its way the dark aperture waits and it fades in when it lands. */
        bool PlayCorridor(bool toMenu)
        {
            if (!_paintSet || !_newArt || _showSet || PlayOverride.Active || _index + 1 >= _set.Levels.Count) return false;
            if (_session == null || _session.Outcome != Outcome.Win) return false;          // a lost level goes on as before
            int next = _index + 1;
            CorridorTransition c = null;
            float startedAt = Time.unscaledTime;
            var nextPic = _images.Get(ImageOf(next), t => { if (c != null) c.SetNext(t); });
            c = CorridorTransition.Play(_screen, CorridorOrder, _images.Get(ImageOf(_index)), nextPic, () =>
            {
                if (Host.Debug) Debug.Log($"[corridor-end] {Time.unscaledTime - startedAt:0.00} s");
                _corridor = null;
                if (toMenu) ShowMenu(); else StartLevel(next);
            });
            if (c == null) return false;
            if (Host.Debug) Debug.Log($"[corridor] {ImageOf(_index)} -> {ImageOf(next)} {(nextPic != null ? "hazır" : "bekleniyor")}{(toMenu ? " menü" : "")}");
            CloseCard();
            _corridor = c;
            return true;
        }

        // ---- G8: the 👍/👎 answer (SetFlags.AskFun) -------------------------------------------

        (AttemptRecord record, string rowId)? _rated;
        bool _funAnswered, _askFun, _hardAnswered, _askHard;
        int _pendingHard = -1;
        float _hardDue;
        string _pendingResultAction;

        void NavigateResult(string id)
        {
            if (id != "retry" && _index == 0 && _session != null && _session.Outcome == Outcome.Win && _endingEligible && !_endingPlayed)
            {
                _endingPlayed = true;
                CloseCard();
                PlayStory("C", false, () => { MarkIntro("rs_intro_end_shown"); ShowMenu(); });
                return;
            }
            if (id != "retry" && _session != null && _session.Outcome == Outcome.Win && _session.Level.Number >= 2) _profileDue = true;
            if (id == "next") { if (!PlayCorridor(false)) StartLevel(_index + 1); }
            else if (id == "retry") StartLevel(_index);
            else if (!PlayCorridor(true)) ShowMenu();
        }

        void CommitPendingHard()
        {
            if (_pendingHard < 1) return;
            int answer = _pendingHard;
            _pendingHard = -1;
            Rate(answer, null);
            _hardAnswered = true;
        }

        /* An answer on the result card fills the already sent row. */
        void Rate(int? hardness, int? fun)
        {
            if (_rated == null) return;
            var (record, rowId) = _rated.Value;
            if (hardness.HasValue) record.Hardness = hardness;
            if (fun.HasValue) record.Fun = fun;
            _telemetry.Rate(rowId, hardness, fun);
        }

        void OpenPause54()
        {
            OpenCard("pause", new Card(_world, _screen, CardOrder, "Pause")
                .Title(Lang.T("Level ", "Seviye ") + _session.Level.Number)
                .Button("resume", Lang.T("Resume", "Devam"), true)
                .Button("sound", Lang.T("Sound: ", "Ses: ") + (Art.Muted ? Lang.T("Off", "Kapalı") : Lang.T("On", "Açık")), false)
                .Button("menu", Lang.T("Quit", "Çık"), false).Finish());
        }

        void RefreshPlaying()
        {
            if (_hud == null) return;
            _hud.SetLeft(_session.Board.PresentCount);
            _hud.SetTime(_session.TimeLeft);
            _bar.Refresh(_boosters, _session, TutorialActive);
            bool wantJoints = _boosters.Armed == Booster.Scissors && !_session.IsOver;
            if (wantJoints && !_jointsShown) _board.ShowJoints(_boosters.Joints());
            if (!wantJoints && _jointsShown) _board.HideJoints();
            _jointsShown = wantJoints;
        }

        // ---- frame ------------------------------------------------------------------------

        void BeginCelebration(bool result)
        {
            _celebration?.Destroy();
            _celebrationResult = result;
            _celebration = new WinCelebration54(transform, _screen, CorridorOrder);
        }

        void EndCelebration()
        {
            bool result = _celebrationResult;
            _celebration?.Destroy();
            _celebration = null;
            _celebrationResult = false;
            if (result) ShowResult();
        }

        void Update()
        {
            if (_introPlaying) return;
            _continuePanel?.Tick();
            _win21?.Tick();
            _levelsMap?.Tick();
            if (Screen.width != _screenPx.x || Screen.height != _screenPx.y)
            {
                _drag?.Cancel();
                Layout();
            }

            if (PlayOverride.Exit != null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ShowMenu();
                return;
            }
            // PM_MESAJ_40: before the clock runs (intro, card, spotlight, any tutorial step) a jam is a 'tut' row, never a jam
            if (_mode == Mode.Playing && _session != null)
                _session.TutorialMove = P21Guided || TutorialActive || _introOpen || _card != null || _tutorialClockHold && _session.Moves == 0;
            if (!_awaitingName) HandlePointer();
            if (_celebration != null && _celebration.Tick(_screen)) EndCelebration();
            if (_pendingHard > 0 && Time.unscaledTime >= _hardDue)
            {
                CommitPendingHard();
                if (_cardKind == "result")
                {
                    string action = _pendingResultAction;
                    _pendingResultAction = null;
                    if (action == null) ShowResult(); else NavigateResult(action);
                }
            }
            if (Time.unscaledTime >= _nextFlush) { _nextFlush = Time.unscaledTime + 15f; _telemetry.Flush(); _moveSender?.Flush(); _introSender?.Flush(); _tutorialSender?.Flush(); if (!_pageHidden) _continueSender?.Flush(); }

            if (_mode == Mode.Playing)
            {
                TickBoosters21();
                TickP21Tutorial();
                if (TutorialActive && _tutorialUntil >= 0 && Time.unscaledTime >= _tutorialUntil && !BoosterBusy21) NextScriptedTutorial();
                // the clock runs only while the board is actually in play
                if (!_session.IsOver && !P21ClockPaused && !_introOpen && !TutorialActive && !_tutorialClockHold && _card == null && _list == null && _celebration == null && _win21 == null && _continuePanel == null && (!BoosterSelection21 || _selected21 == Booster.Wand) && !BoosterBusy21 && (!P21Boosters || !_pageHidden)) _session.Tick(Time.unscaledDeltaTime);
                if (_endAt > 0 && Time.unscaledTime >= _endAt)
                {
                    _endAt = -1;
                    if (P21WinArt && _session.Outcome == Outcome.Win) ShowWin21();
                    else if (Art.T54 && _session.Outcome == Outcome.Win) BeginCelebration(true);
                    else ShowResult();
                }
                RefreshPlaying();
            }
            if (_debugCorridor && _mode == Mode.Menu && Time.unscaledTime > 2 && _corridor == null)
            {
                _debugCorridor = false;
                float at = Time.unscaledTime;
                _corridor = CorridorTransition.Play(_screen, CorridorOrder, _images.Get(ImageOf(0)), _images.Get(ImageOf(1)), () =>
                {
                    Debug.Log($"[corridor-end] {Time.unscaledTime - at:0.00} s");
                    _corridor = null;
                });
            }
            if (_debug != null) UpdateDebug();
            if (_fpsLabel != null) UpdateFpsLabel();
        }
        bool _debugCorridor;

        /* Focus lost mid-drag: the piece leaves if it was already past the exit
           point, otherwise it springs back (PRODUCT.md K1). */
        void OnApplicationFocus(bool focus)
        {
            if (focus || _drag == null || !_drag.Active) return;
            var lim = _drag.Limit;
            var result = _drag.Abort(out int piece, out var dir);
            _board?.EndDrag(piece, result, dir, lim.Cells);
        }

        void HandlePointer()
        {
            var p = Pointer.current;
            if (p == null) return;
            if (_continuePanel != null)
            {
                Vector2 pos = p.position.ReadValue();
                Vector2 point = ScreenToPoints(pos);
                bool blocked = InputGated(p);
                if (p.press.wasPressedThisFrame && !blocked)
                {
                    string decision = _continuePanel.Hit(point);
                    if (decision != null) ResolveContinue(decision);
                }
                return;
            }
            if (_celebration != null)
            {
                if (p.press.wasPressedThisFrame) _celebration.Skip();
                return;
            }
            Vector2 px = p.position.ReadValue();
            var pt = ScreenToPoints(px);
            bool gated = InputGated(p);

            if (_mode == Mode.Menu && Art.On)
            {
                // PM_MESAJ_27 A2: a touch on a moving list only stops it; under 10 pt of travel is a tap
                var scroll = ActiveMenuScroll;
                float now = Time.unscaledTime;
                if (p.press.wasPressedThisFrame && !gated)
                {
                    _menuPointer = true; _menuDragging = false; _menuPointerStart = _menuPointerLast = pt;
                    _menuStopTap = false;
                    if (scroll != null && scroll.Contains(pt)) { _menuStopTap = scroll.Moving; scroll.Grab(pt.y, now); }
                }
                bool scrolling = _menuPointer && scroll != null && scroll.Held;
                if (scrolling && p.press.isPressed)
                {
                    if (!_menuDragging && Mathf.Abs(pt.y - _menuPointerStart.y) >= 10) _menuDragging = true;
                    scroll.Drag(_menuDragging ? pt.y : _menuPointerStart.y, now);
                    _menuPointerLast = pt;
                }
                if (p.press.wasReleasedThisFrame && _menuPointer)
                {
                    _menuPointer = false;
                    if (scrolling) scroll.Release(_menuDragging ? now : -1);
                    bool cancelled = Touchscreen.current != null && p == Touchscreen.current && Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
                    if (!_menuDragging && !_menuStopTap && !cancelled && Vector2.Distance(pt, _menuPointerStart) < 10) { Haptics.Play(Haptics.Kind.Select); Press(pt); }
                    RememberMenuScroll();
                }
                var mouse = Mouse.current;
                if (mouse != null && scroll != null && !_menuPointer)
                {
                    float wheel = mouse.scroll.ReadValue().y;
                    // browsers report pixels (~100 a notch) or lines (1-3); either way about 60 pt a notch
                    float a = Mathf.Abs(wheel), d = a <= 3 ? a * 40 : Mathf.Min(a, 240) * .5f;
                    if (wheel != 0) scroll.Wheel(-Mathf.Sign(wheel) * d);
                }
                scroll?.Tick(Time.unscaledDeltaTime);
                return;
            }

            if (_committedTouch)
            {
                if (!p.press.isPressed) _committedTouch = false;
                return;
            }
            if (p.press.wasPressedThisFrame) { if (!gated) Press(pt); }
            else if (_drag != null && _drag.Active && (p.press.isPressed || p.press.wasReleasedThisFrame))
            {
                if (TutorialActive && _tutorialUntil >= 0) return;
                if (P21Guided && Time.unscaledTime < _p21AdvanceAt) return;
                if (RejectTutorialDirection(pt)) return;
                var touch = Touchscreen.current;
                bool released = p.press.wasReleasedThisFrame;
                bool cancelled = released && touch != null && p == touch &&
                    touch.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
                double now = Time.unscaledTimeAsDouble, dt = now - _gestureTime;
                var velocity = dt > .0001 ? (pt - _gestureLast) / (float)dt : Vector2.zero;
                _gestureLast = pt; _gestureTime = now;
                var gesture = _flick.Classify(_gestureStart, pt, velocity, released, _board.Cell);
                if (MoveRow.INPUT_COL && Cohort == "p21" && gesture.Direction != Vector2.zero)
                {
                    var d = gesture.Direction.x > 0 ? Dir.R : gesture.Direction.x < 0 ? Dir.L : gesture.Direction.y > 0 ? Dir.D : Dir.U;
                    var lim = _session.Probe(_drag.Piece, d);
                    _moveInput = gesture.State == FlickState.Move && !(released && lim.Exit &&
                        Vector2.Distance(pt, _gestureStart) >= (lim.Cells + DragModel.ExitOvershoot) * _board.Cell * DragModel.ExitFraction) ? "flick" : "drag";
                }
                if (cancelled) { FinishPointerDrag(true); return; }
                if (P21Guided && RejectP21Direction(gesture.Direction)) return;
                if (gesture.Direction != Vector2.zero)
                {
                    // Project before the legacy model chooses its axis.
                    float along = Vector2.Dot(pt - _gestureStart, gesture.Direction);
                    var projected = _gestureStart + gesture.Direction * along;
                    _drag.Move(projected.x, projected.y, now);
                    _board.ShowDrag(_drag);
                    if (FinishP21Jam(released)) return;
                }
                if (gesture.State == FlickState.Move)
                {
                    var limit = _drag.Limit;
                    bool longDrag = released && limit.Exit &&
                        _drag.Offset >= (limit.Cells + DragModel.ExitOvershoot) * _board.Cell * DragModel.ExitFraction;
                    if (longDrag) _dragInputs++; else _flickInputs++;
                    _moveInput = longDrag ? "drag" : "flick";
                    // Advance only the model to its normal release point. The view
                    // retains the finger position and animates the remaining distance.
                    float distance = (limit.Cells + DragModel.ExitOvershoot + 1) * _board.Cell + DragModel.JamSlack;
                    var end = _gestureStart + gesture.Direction * distance;
                    _committedTouch = !released;
                    _drag.Move(end.x, end.y, now);
                    if (FinishP21Jam(released)) return;
                    FinishPointerDrag(false, true);
                }
                else if (released)
                {
                    int piece = _drag.Piece;
                    _drag.Cancel();
                    if (_zeydbotSet) WriteMove("touch", piece, null, (int)((Time.unscaledTime - _pressAt) * 1000));
                    _board.EndDrag(piece, DragRelease.SnappedBack, Dir.U, 0);
                }
            }
        }

        // Host.PixelRatio also sizes the layout; these are CSS px, independent of DPR.
        Vector2 ScreenToPoints(Vector2 pixels) => new Vector2(pixels.x / _dpr, _screen.y - pixels.y / _dpr);

        void BeginGesture(Vector2 point)
        {
            _moveInput = "drag";
            _flick = new FlickGesture();
            _gestureStart = _gestureLast = point;
            _gestureTime = Time.unscaledTimeAsDouble;
            _board.ShowDrag(_drag); _pressAt = Time.unscaledTime;
            Haptics.Play(Haptics.Kind.Select);
        }

        void FinishPointerDrag(bool cancelled, bool autoSlide = false)
        {
            var limit = _drag.Limit;
            int piece; Dir dir;
            var result = cancelled ? _drag.Abort(out piece, out dir) : _drag.Release(out piece, out dir);
            if (_zeydbotSet && result != DragRelease.Exited && piece >= 0)
                WriteMove("touch", piece, null, (int)((Time.unscaledTime - _pressAt) * 1000));
            _board.EndDrag(piece, result, dir, limit.Cells, autoSlide);
        }

        // Task 15 card buttons: the pressed sprite shows for .12 s before the action (_cardTap is the pending one)
        object _cardTap;
        bool _cardFiring;
        // PM_MESAJ_38: after a card opens, closes or changes, presses go nowhere until the finger is up and .25 s passed
        object _gateCard;
        float _gateUntil;
        bool _gateRelease;

        void CancelCardTap()
        {
            if (_cardTap != null) Tweens.Kill(_cardTap);
            _cardTap = null;
        }

        bool InputGated(Pointer p)
        {
            object cards = (object)_continuePanel ?? _card ?? (object)_list?.Card ?? _spotlight;   // PM_MESAJ_40: a spotlight closing gates too
            if (cards != _gateCard)
            {
                _gateCard = cards;
                _gateUntil = Time.unscaledTime + .25f;
                _gateRelease = p.press.isPressed;
            }
            if (_gateRelease && !p.press.isPressed) _gateRelease = false;
            return _cardTap != null || _gateRelease || Time.unscaledTime < _gateUntil;
        }

        void Press(Vector2 pt)
        {
            if (_win21 != null) { _win21.Press(pt); return; }
            if (_corridor != null) { _corridor.Skip(); return; }   // a tap skips the slide

            // level list (over menu or game)
            if (_list != null)
            {
                int hit = _list.Hit(pt);
                if (hit == -2) { _list.Card.Destroy(); _list = null; }
                else if (hit >= 0) StartLevel(hit);
                return;
            }

            if (_card != null && _cardKind == "profile")            // PM_MESAJ_25: over the menu or a level
            {
                string pid = _card.HitButton(pt);
                if (pid != null && pid.StartsWith("exp_")) AnswerProfile(pid[4] - '0');
                return;
            }

            if (_mode == Mode.Menu)
            {
                if (_card != null && _cardKind == "settings")
                {
                    string sid = _card.HitButton(pt);
                    if (sid == "sound") { Art.Muted = !Art.Muted; OpenSettings(); }
                    else if (sid == "haptics") { Haptics.On = !Haptics.On; OpenSettings(); }
                    else if (sid == "music") { Music.On = !Music.On; OpenSettings(); }
                    else if (sid == "lang") { Lang.Tr = !Lang.Tr; CloseCard(); Layout(); OpenSettings(); }
                    else if (sid != null) CloseCard();
                    return;
                }
                if (Art.On && _menuSettings != null && _menuSettings.Press(pt, () => { if (_mode == Mode.Menu) MenuAction(pt); })) return;
                // Task 13: the pressed look shows for a moment before the menu acts
                if (Art.On && (_menuSettings != null || _paintingDetail != null || _profileCard != null || _menu == null || (_tabs?.Hit(pt) ?? -1) >= 0)) { MenuAction(pt); return; }
                if (_menu != null && _menu.Press(pt)) { var at = pt; Tweens.Run(_menu, .12f, _ => { }, () => { if (_mode == Mode.Menu) MenuAction(at); }); return; }
                MenuAction(pt);
                return;
            }

            if (_card != null)
            {
                if (!_cardFiring && _card.Press(pt))
                {
                    // PM_MESAJ_38: one action per tap; the callback does nothing if the card changed meanwhile
                    var key = _cardTap = new object();
                    var card = _card;
                    var at = pt;
                    Tweens.Run(key, .12f, _ => { }, () =>
                    {
                        if (_cardTap != key) return;
                        _cardTap = null;
                        if (_card != card) return;
                        _cardFiring = true;
                        try { Haptics.Play(Haptics.Kind.Select); Press(at); } finally { _cardFiring = false; }
                    });
                    return;
                }
                string id = _card.HitButton(pt);
                if (id == null) return;
                switch (_cardKind)
                {
                    case "intro": _introOpen = false; CloseCard(); ShowScriptedTutorial(); break;
                    case "pause":                                   // the attempt ends as a quit when left (LeaveLevel)
                        if (UseHud54 && id == "sound") { Art.Muted = !Art.Muted; CloseCard(); OpenPause54(); break; }
                        CloseCard();
                        if (id == "restart") StartLevel(_index);
                        else if (id == "menu") ShowMenu();
                        break;
                    case "ask":
                        if (id == "yes") _boosters.Confirm(); else _boosters.CancelAsk();
                        CloseCard();
                        break;
                    case "result":
                        if (id == "hard_skip" || id.StartsWith("hard_") && int.TryParse(id.Substring(5), out int _))
                        {
                            if (id == "hard_skip") { _pendingHard = -1; _pendingResultAction = null; _hardAnswered = true; }
                            else
                            {
                                _pendingResultAction = null;
                                _pendingHard = int.Parse(id.Substring(5));
                                _hardDue = Time.unscaledTime + .3f;
                            }
                            ShowResult();
                            break;
                        }
                        if (id == "fun_up" || id == "fun_down")
                        {
                            // Build 5: the row also waits for the 1-10 answer (both go in one row)
                            int fun = id == "fun_up" ? 1 : -1;
                            Rate(null, fun);
                            _funAnswered = true;
                            ShowResult();
                            break;
                        }
                        if (_pendingHard > 0) _pendingResultAction = id;
                        else NavigateResult(id);
                        break;
                }
                return;
            }

            if (P21ObstacleLesson && _bar.Hit(pt) == Booster.Clock && _session.IsUnlocked(Booster.Clock))
            {
                _bar.Notice(Lang.T("This lesson has no timer.", "Bu derste süre sınırı yok."), 1.5f);
                return;
            }
            if (PressBooster21(pt)) return;
            if (P21StoryLevel && _p21View != null && _p21View.DismissStory(pt))
            {
                _p21StoryDismissed = true; return;
            }
            if (P21Guided)
            {
                PressP21Tutorial(pt);
                return;
            }
            if (TutorialActive)
            {
                if (_tutorialUntil >= 0)
                {
                    if (_tutorialSteps[_tutorialAt].tur == "hata") NextScriptedTutorial();
                    return;
                }
                var step = _tutorialSteps[_tutorialAt];
                if (step.tur == "booster" || step.tur == "booster_hedef")
                { TapTutorialBooster(pt, step); return; }
                if (step.tur != "surukle" && step.tur != "hata") { NextScriptedTutorial(); return; }
                int target = _board.GrabAt(pt, out bool tutorialOutside);
                if (target != _tutorialMove.Piece) { _spotlight?.Restart(); return; }
                _tutorialPress = pt;
                if (_drag.Press(target, pt.x, pt.y, Time.unscaledTimeAsDouble, tutorialOutside))
                { BeginGesture(pt); }
                return;
            }

            if (UseHud54 && _bar.HitAction(pt) == BarAction.Pause) { OpenPause54(); return; }
            if (_hud.RestartRect.Contains(pt))
            {
                if (!Art.On) { StartLevel(_index); return; }
                OpenCard("pause", new Card(_world, _screen, CardOrder, "Pause").Title(Texts.Paused)
                    .Button("resume", Texts.Resume, true).Button("restart", Texts.Restart, false).Button("menu", Texts.MainMenu, false).Finish());
                return;
            }
            if (_hud.LevelRect.Contains(pt)) { OpenList(); return; }

            var b = _bar.Hit(pt);
            if (b.HasValue)
            {
                if (!_session.IsUnlocked(b.Value)) { _bar.LockedNotice(b.Value, _session); return; }
                _boosters.Tap(b.Value);
                if (_boosters.Asking.HasValue)
                    OpenCard("ask", Screens.Ask(_world, _screen, _boosters, _boosters.Asking.Value, CardOrder));
                return;
            }
            if (_session.IsOver) return;

            if (_boosters.Armed == Booster.Scissors)
            {
                var j = _board.JointAt(pt);
                if (j.HasValue) _boosters.TapJoint(j.Value.x, j.Value.y, j.Value.vertical);
                return;
            }
            int piece = _board.PieceAt(pt);
            if (_boosters.Armed == Booster.Hammer)
            {
                if (piece >= 0) _boosters.TapPiece(piece);
                return;
            }
            if (_boosters.BlocksDrag) return;
            // a press just outside the board takes the nearest edge piece (PRODUCT.md K1)
            piece = _board.GrabAt(pt, out bool outside);
            if (piece >= 0 && _session.NailAt(piece) > 0) _board.Clang(piece);     // web: pinned, refuse and say so
            if (piece >= 0 && _drag.Press(piece, pt.x, pt.y, Time.unscaledTimeAsDouble, outside)) { BeginGesture(pt); }
        }

        void TapTutorialBooster(Vector2 pt, TutorialStep step)
        {
            if (!TutorialScript.BoosterOf(step.booster, out var b)) return;
            if (step.tur == "booster")
            {
                if (_bar.Hit(pt) != b) { _spotlight?.Restart(); return; }
                _session.FreeUse = b;
                if (b == Booster.Scissors || b == Booster.Hammer)
                {
                    if (!TutorialScript.BoosterTarget(_session, b, out _boosterTarget, out _boosterJoint))
                    {
                        Debug.LogWarning($"Tutorial: slot {_index + 1} {b} has no target; target step skipped.");
                        _session.FreeUse = null;
                        // No effect is possible: leave training incomplete, skip this attempt's target step.
                        _tutorialAt++;
                        NextScriptedTutorial();
                        return;
                    }
                    // BoosterControls rejects zero stock before arming; lend one only during selection.
                    int stock = _session.Stock(b);
                    _session.SetStock(b, System.Math.Max(1, stock));
                    _boosters.Tap(b);
                    _session.SetStock(b, stock);
                    NextScriptedTutorial();
                }
                else
                {
                    _trainingUse = b;
                    var result = b == Booster.Wand ? _session.UseWand() : _session.UseClock();
                    if (result != CommandResult.Ok)
                    {
                        _trainingUse = null;
                        _bar.Notice(Texts.BoosterPrompt(_boosters), 1.6f);
                        _spotlight?.Restart();
                        Debug.LogWarning($"Tutorial: {b} failed ({result}); tap again to retry.");
                    }
                }
            }
            else
            {
                if (b == Booster.Scissors)
                {
                    var joint = _board.JointAt(pt);
                    if (!joint.HasValue || joint.Value != _boosterJoint) { _spotlight?.Restart(); return; }
                    _trainingUse = b;
                    _boosters.TapJoint(_boosterJoint.x, _boosterJoint.y, _boosterJoint.vertical);
                }
                else
                {
                    if (_board.PieceAt(pt) != _boosterTarget) { _spotlight?.Restart(); return; }
                    _trainingUse = b;
                    _boosters.TapPiece(_boosterTarget);
                }
            }
            RefreshPlaying();
        }

        void OpenList()
        {
            int current = _mode == Mode.Playing ? _index : _progress.Next();
            _list = new Screens.LevelList(_world, _screen, _set.Levels, _progress, current, CardOrder + 100);
        }

        void UpdateFpsLabel()
        {
            float dt = Mathf.Max(Time.unscaledDeltaTime, 1e-4f);
            _fpsLabelAcc += dt; _fpsLabelFrames++;
            _fpsLabelWorst = Mathf.Max(_fpsLabelWorst, dt);
            if (_fpsLabelAcc < 1f) return;
            _fpsLabel.Text = $"FPS {_fpsLabelFrames / _fpsLabelAcc:0}  min {1f / _fpsLabelWorst:0}";
            _fpsLabelAcc = 0; _fpsLabelFrames = 0; _fpsLabelWorst = 0;
        }

        void UpdateDebug()
        {
            float dt = Mathf.Max(Time.unscaledDeltaTime, 1e-4f);
            _fpsAcc += dt; _fpsFrames++;
            if (_fpsAcc >= .5f) { _fpsShown = _fpsFrames / _fpsAcc; _fpsAcc = 0; _fpsFrames = 0; }
            if (_drag != null && _drag.Active)
            {
                _dragFps.Add(1f / dt);
                if (_dragFps.Count > 900) _dragFps.RemoveAt(0);
            }
            string median = "-";
            if (_dragFps.Count > 0)
            {
                var sorted = new List<float>(_dragFps);
                sorted.Sort();
                median = $"{sorted[sorted.Count / 2]:0} (n={sorted.Count})";
            }
            string where = _mode == Mode.Playing ? $"{_session.Level.Id}  hücre {_board.Cell:0} pt" : "menü";
            var q = _telemetry.Queue;
            string net = _telemetry.Enabled ? "açık" : "kapalı";
            string last = _drag?.Last != null ? _drag.Last.ToString() : "-";
            _debug.Text = $"{where}  FPS {_fpsShown:0}  sürüklerken medyan {median}\n" +
                          $"gönderim {net}  kuyruk {q.Rows.Count}  gönderilen {_telemetry.Sender.Sent}  ayrılan {q.Parked}\n" +
                          $"son sürükleme: {last}";
        }

        // ---- editor screenshots -------------------------------------------------------------

        /* Renders a screen at a point size and scale into a PNG. Play mode only.
           state: "" board at start; "win" / "bomb" / "time" result card;
           "charset" a line with every Turkish letter; "menu"; "levels";
           "intro"; "ask" (wand or clock question); "scissors" (joints shown). */
        public string RenderShot(string levelId, int widthPt, int heightPt, int scale, string path, string state = "")
        {
            int idx = -1;
            for (int i = 0; i < _set.Levels.Count; i++) if (_set.Levels[i].Id == levelId) idx = i;
            if (idx < 0 && designs != null && levelId.StartsWith("D"))      // a level-editor design
            {
                _set = LevelParser.Parse(designs.text);
                _designSet = true;
                _progress = new Progress(_log, _set.Levels) { UnlockAll = true };
                for (int i = 0; i < _set.Levels.Count; i++) if (_set.Levels[i].Id == levelId) idx = i;
            }
            if (idx < 0) return "no level " + levelId;

            var rt = new RenderTexture(widthPt * scale, heightPt * scale, 24);
            var prevTarget = _cam.targetTexture;

            LeaveLevel();
            _index = idx;
            _session = _log.Start(_set.Levels[idx], Client, new Mulberry32(1));
            _boosters = new BoosterControls(_session);
            _mode = state == "menu" ? Mode.Menu : Mode.Playing;
            _introOpen = state == "intro";
            ResetScriptedTutorial(true);
            if (state != "" && state != "intro") _tutorialSteps.Clear();
            _endAt = -1;
            switch (state)
            {
                case "win": foreach (var m in SmartPlayer.Play(_session.Level).Moves) _session.TryExit(m.Piece, m.Dir); break;
                case "bomb": foreach (var m in _session.Level.Solution) if (!_session.IsOver) _session.TryExit(m.Piece, m.Dir); break;
                case "time": _session.Tick(_session.TimeLeft); break;
            }

            _screenPx = new Vector2Int(rt.width, rt.height);
            _dpr = scale;
            _screen = new Vector2(widthPt, heightPt);
            _safe = Vector4.zero;
            Build(immediate: true);
            if (_session.IsOver && _card == null) ShowResult();
            // G1: the picture straight from disk, since a shot cannot wait for the download
            var image = _session.Level.Image;
            if (image != null && _board != null && System.IO.File.Exists($"{Application.streamingAssetsPath}/{LevelImages.Folder}/{image}"))
            {
                var pic = new Texture2D(2, 2);
                pic.LoadImage(System.IO.File.ReadAllBytes($"{Application.streamingAssetsPath}/{LevelImages.Folder}/{image}"));
                pic.wrapMode = TextureWrapMode.Clamp;
                _board.SetImage(pic);
            }
            if (state == "levels") OpenList();
            if (state == "ask")
            {
                var b = _boosters.StateOf(Booster.Wand) == BoosterButton.Ready ? Booster.Wand : Booster.Clock;
                _boosters.Tap(b);
                if (_boosters.Asking.HasValue) OpenCard("ask", Screens.Ask(_world, _screen, _boosters, _boosters.Asking.Value, CardOrder));
            }
            if (state == "scissors") { _boosters.Tap(Booster.Scissors); RefreshPlaying(); }
            if (state == "charset")
                TextView.Create(_world, "charset", "Türkçe: ğ ü ş ı ö ç İ  Ğ Ü Ş I Ö Ç", 15, Draw.Hex("#eaf3f2"), order: DebugOrder)
                    .transform.localPosition = Draw.W(widthPt / 2f, _board.Origin.y + _board.Size.y + 40);
            _session.EventRaised += OnEvent;

            _cam.targetTexture = rt;
            _cam.Render();
            var prevActive = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
            RenderTexture.active = prevActive;
            _cam.targetTexture = prevTarget;
            Destroy(tex);
            rt.Release();
            string info = $"{levelId} {state} {widthPt}x{heightPt}: cell {(_board != null ? _board.Cell : 0)} pt";
            _screenPx = Vector2Int.zero;      // forces a normal layout next frame
            return info;
        }
    }
}
