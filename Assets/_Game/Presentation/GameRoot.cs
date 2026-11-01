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
       Cell size: Core BoardLayout (the web's sizeCell with a wider side gap,
       PRODUCT.md K1).
       Nothing is drawn below the safe bottom inset (iOS home indicator).

       URL: ?lv=N opens level N directly; ?debug=1 shows the load panel, the FPS
       line and unlocks every level. ?debug=1&set=designs plays the level
       editor's designs instead (designs.json), and lv may then be an id
       (?debug=1&set=designs&lv=D03). Without debug the set parameter is
       ignored; debug sessions never send telemetry. */
    public sealed class GameRoot : MonoBehaviour
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
        TelemetryLog _telemetry;
        string _rowId;                 // row_id of the attempt in play
        bool _awaitingName;
        float _nextFlush;
        Progress _progress;
        Mode _mode = Mode.Menu;
        int _index;
        GameSession _session;
        DragModel _drag;
        BoosterControls _boosters;
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
        LevelImages _images;          // G1: level pictures, fetched when a level opens
        readonly HashSet<string> _introSeen = new HashSet<string>();

        Camera _cam;
        Transform _world;
        BoardView _board;
        Hud _hud;
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
        bool _menuPointer, _menuDragging, _tabAnimating;
        Vector2 _menuPointerStart, _menuPointerLast;
        MenuScroll ActiveMenuScroll => _menuSettings != null || _paintingDetail != null || _profileCard != null || _card != null ? null : _levelsMap?.Scroll ?? _collection?.Scroll ?? _jiggles?.Scroll ?? _achievements?.Scroll;
        CorridorTransition _corridor;  // PM_MESAJ_24: the slide to the next level's painting, over everything
        Screens.LevelList _list;
        TextView _debug;
        Vector2Int _screenPx;
        Vector2 _screen;            // points
        Vector4 _safe;              // points: left, top, right, bottom
        float _dpr = 1;
        float _endAt = -1;
        bool _jointsShown;

        readonly List<float> _dragFps = new List<float>();
        float _fpsAcc; int _fpsFrames; float _fpsShown;

        void Start()
        {
            Music.Start(this);                                 // the menu and the levels share one loop
            Materials.Init(pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial);
            _images = LevelImages.Create();
            _cam = Camera.main;
            _set = PlayOverride.Active ? PlayOverride.Set : ChooseSet();

            // saved counters and the send queue (M5-pre). Sending is off in the
            // editor, with ?debug=1, and in builds until the switch is turned on.
            System.Func<string> newId = () => System.Guid.NewGuid().ToString("N");
            var host = Host.Current;
            _save = new SaveData(host.Store, newId);
            _stats = PlayStats.Load(host.Store);
            _stats.ImportLegacy(_save);
            _log = _save.Restore();
            var queue = new TelemetryQueue(host.Store);
            bool send = host.SendEnabled && !Host.Debug && !Application.isEditor;
            var sender = new TelemetrySender(queue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send);
            _telemetry = new TelemetryLog(_save, queue, sender, newId);
            // PM_MESAJ_25: cohort from the link (?c=pt1), kept on the device for the next visit without it
            string cohort = TelemetryMeta.CleanCohort(host.Query("c"));
            if (cohort != null) { host.Store.Set("rs_cohort", cohort); host.Store.Save(); }
            else cohort = TelemetryMeta.CleanCohort(host.Store.Get("rs_cohort"));
            TelemetryMeta.Current = new TelemetryMeta { Cohort = cohort, DeviceId = _save.Install, Platform = PlatformName(), AppVersion = AppVersion() };
            _profileQueue = new TelemetryQueue(host.Store, "profiles");
            _profileSender = new TelemetrySender(_profileQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "player_profiles?on_conflict=cohort,player,device_id");
            if (_zeydbotSet)
            {
                _moveQueue = new TelemetryQueue(host.Store, "moves");
                _moveSender = new TelemetrySender(_moveQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "moves?on_conflict=row_id,move_no");
            }
            host.VisibilityChanged += OnVisibility;
            _telemetry.Flush();                               // rows left from an earlier visit

            // every level open for the test links (set= in the address), not when the flag makes SET40 the playtest default
            _progress = new Progress(_log, _set.Levels) { UnlockAll = Host.Debug || (_zeydbotSet && !string.IsNullOrEmpty(host.Query("set"))) };
            // PM_MESAJ_26 QA (debug only): ?won=N shows the menus as if the first N levels were won (memory only, never saved)
            if (Host.Debug && int.TryParse(host.Query("won"), out int won))
            {
                _previewWins = true;
                var best = new System.Collections.Generic.Dictionary<string, int>();
                for (int w = 0; w < won && w < _set.Levels.Count; w++) best[_set.Levels[w].Id] = 3;
                _log = new AttemptLog();
                _log.Restore(new System.Collections.Generic.Dictionary<string, int>(), best);
                _progress = new Progress(_log, _set.Levels);
            }
            // web name gate: once per browser; the HTML input sits over the canvas
            if (_showSet && string.IsNullOrEmpty(_save.Player)) _save.SetPlayer("SHOW");   // demo visitors are tagged SHOW
            if (Host.Debug && !string.IsNullOrEmpty(host.Query("player"))) _save.SetPlayer(host.Query("player"));   // PM_MESAJ_26 QA: a long name
            bool askName = string.IsNullOrEmpty(_save.Player) && !Host.Debug && !_showSet;
            int requested = PlayOverride.Active ? PlayOverride.Index
                          : !_showSet && TryLevelIndex(host.Query("lv"), out int index) ? index : -1;   // SHOW1: the menu first, its Demo button starts the level
            _awaitingName = askName;
            LaunchPlan.Run(requested, askName, StartLevel, ShowMenu, host.AskName,
                           name => { bool fresh = _save.Best.Count == 0; _save.SetPlayer(name); _awaitingName = false; if (_mode == Mode.Menu) Layout();   // the profile box shows it
                                   if (fresh) StartCoroutine(RecoverProgress(name));
                                   AskProfile(); });
            if (!askName) AskProfile();
            _profileSender.Flush();
        }

        // ---- PM_MESAJ_25: who plays (cohort, device, platform, version; one profile question) --------

        TelemetryQueue _profileQueue;
        TelemetrySender _profileSender;

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
            if (Host.Current.Query("lv") == "SHOW1")
            {
                var show = Resources.Load<TextAsset>("show");
                if (show != null) { _showSet = true; _newArt = Art.On = true; return LevelParser.Parse(show.text); }
            }
            Art.On = false;                                    // static: no domain reload between Play sessions
            _paintSet = false;
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
                no, kind, piece, dir, (int)((now - _moveT0) * 1000), (int)((now - _moveLast) * 1000), _session.TimeLeft, fuses, holdMs));
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
            if (_zeydbotSet && _mode == Mode.Playing && _session != null && !_session.IsOver) WriteMove(visible ? "show" : "hide", -1, null);
            if (visible) _telemetry.PageVisible(_mode == Mode.Playing ? _session : null, _rowId);
            else
            {
                SendPendingFun();                                 // a closing tab must not lose the won row
                _telemetry.PageHidden(_mode == Mode.Playing ? _session : null, _rowId);
                if (!_previewWins) _save.Store(_log);
            }
        }

        // ---- flow -------------------------------------------------------------------------

        void LeaveLevel()
        {
            if (_session == null) return;
            if (!_session.IsOver) { WriteMove("quit", -1, null); _session.Quit(); }   // OnEvent logs the quit row
            _session.EventRaised -= OnEvent;
        }

        void ShowMenu()
        {
            SendPendingFun();
            LeaveLevel();
            _session = null;
            if (PlayOverride.Exit != null) { PlayOverride.Exit(); return; }   // back to the level editor
            _mode = Mode.Menu;
            Layout();
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
            SendPendingFun();                                  // G8: a won row never waits past the next level
            _funAnswered = false;
            _askFun = false;
            _hardAnswered = _askHard = false;
            _session = _log.Start(level, Client, new Mulberry32((uint)System.Environment.TickCount));
            _rowId = _telemetry.NewRowId();
            if (!_previewWins) _save.Store(_log);                                 // the attempt counter, before anything can go wrong
            _session.EventRaised += OnEvent;
            _boosters = new BoosterControls(_session);
            _moveT0 = _moveLast = Time.unscaledTime;
            _moveNo = 0;
            WriteMove("start", -1, null);                     // PM_MESAJ_25: an attempt that never ends still shows up
            _mode = Mode.Playing;
            _endAt = -1;
            // web loadLevel(i): the intro shows on a level's first start, not on retries
            _introOpen = Screens.HasIntro(level) && _introSeen.Add(level.Id);
            Layout();
            _board?.Deal();                                    // web render(true): pieces deal in on a fresh start
        }

        /* Zeyd 2026-10-28: a new browser save (Safari's 7-day clean-up, Chrome or WhatsApp's own browser, another
           phone) loses the progress, but every win is already a `plays` row under the player's name. When the save is
           empty and a name is typed, those rows bring the best stars back (anon read, the schema untouched). */
        System.Collections.IEnumerator RecoverProgress(string name)
        {
            var host = Host.Current;
            if (string.IsNullOrEmpty(host.SupabaseUrl) || string.IsNullOrEmpty(host.AnonKey) || string.IsNullOrEmpty(name)) yield break;
            string url = host.SupabaseUrl + "/rest/v1/plays?select=level_id,stars&result=eq.win&player=eq." + UnityEngine.Networking.UnityWebRequest.EscapeURL(name);
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

        /* SET40 v3 tutorial: the hand on the next solution move and its step text. */
        void ShowTutorialStep()
        {
            if (_board == null || !Tutorial.Step(_session, out var m)) return;
            // Zeyd 2026-10-28: the hand on level 1's first two moves only, never on level 2
            int done = _session.Level.Pieces.Count - _session.Board.PresentCount;
            if (_session.Level.Tutorial != 1 || done >= 2) return;
            var origin = BoardPlacement(_screen, _safe, _session.Level, out _);
            _hand = TutorialHand.Show(_board, _board.PieceViewOf(m.Piece), m, HandOrder, new Rect(-origin, _screen));
            if (_session.Level.Tutorial == 1 || done == 0) _bar?.Notice(done == 0 ? Texts.TutorialFirst : Texts.TutorialNext, 2.5f);
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
                    if (hit >= 0) _paintingDetail = new PaintingDetail(_world, _screen, hit, _images, CardOrder, i => _stats.Of(_set.Levels[i].Id)?.BestWinSeconds);
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
            if (_collection != null) _collectionOffset = _collection.Scroll.Offset;
            if (_levelsMap != null) _mapOffset = _levelsMap.Scroll.Offset;
            if (_jiggles != null) _jigglesOffset = _jiggles.Scroll.Offset;
            if (_achievements != null) _achievementsOffset = _achievements.Scroll.Offset;
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
            if (_zeydbotSet) RecordMove(e);
            if (e.Kind == EventKind.ValveReleased) _bar?.Notice(Texts.NailReleased, 1.6f);   // web says "söktü"; PM: grammar fix
            if (e.Kind == EventKind.PieceRemoved)
            {
                _tutorialDone = true;                          // web clearTutorial() in flyOut and the hammer
                if (_hand != null) { _hand.Remove(); _hand = null; }
                if (_session.Level.Tutorial > 0) ShowTutorialStep();
            }
            // Codex sounds (preview art only; silent when Art is off)
            // PM_MESAJ_19: no sound per piece; the jam keeps its knock; the last piece gets the whole sparkle, then the fanfare
            if (e.Kind == EventKind.PieceRemoved) { if (_session.Board.PresentCount == 0) PlaySound("popfull"); }
            else if (e.Kind == EventKind.Jammed) { PlaySound("piece"); Haptics.Play(Haptics.Kind.Light); }
            else if (e.Kind == EventKind.Finished && e.Outcome == Outcome.Win) PlaySoundLater("win", .7f);
            else if (e.Kind == EventKind.Finished && e.Outcome != Outcome.Quit) PlaySound("loss");
            if (e.Kind != EventKind.Finished) return;
            _log.Add(_session.Record);
            if (!_previewWins)
            {
                _stats.Add(_session.Record, _session.Level.TimerSeconds);
                _stats.Save(Host.Current.Store);
            }
            if (!_previewWins) _save.Store(_log);
            // G8: the attempt that gets the 👍/👎 question waits for the answer (or the card closing)
            // before its row goes; asked once per level, on the win or after the third loss
            string levelId = _session.Level.Id;
            bool ask = !_showSet && SetFlags.AskFun && FunAsk.ShouldAsk(e.Outcome, _save.LossesOf(levelId), _save.WasFunAsked(levelId));
            if (SetFlags.AskFun && (e.Outcome == Outcome.Time || e.Outcome == Outcome.Bomb)) _save.RecordLoss(levelId);
            _askFun = ask;
            _askHard = e.Outcome != Outcome.Quit && !_showSet && AskHardNow(levelId);
            if (_askHard) HardAsked(levelId);                       // asked once per level (cohort), answered or skipped
            if (ask) { _save.MarkFunAsked(levelId); _pendingFun = (_session.Record, _rowId); }
            else if (_askHard) _pendingFun = (_session.Record, _rowId);   // waits for the 1-10 answer
            else _telemetry.Finished(_session.Record, _rowId);
            if (Host.Debug) Debug.Log("[attempt] " + _session.Record);
            _drag?.Cancel();
            _boosters?.Disarm();
            if (e.Outcome == Outcome.Quit) return;
            if (e.Outcome == Outcome.Bomb) { Flash(Draw.Hex("#e8674c"), .45f); Haptics.Play(Haptics.Kind.Heavy); }   // bomb: red flash + swell
            if (e.Outcome == Outcome.Win) { Haptics.Play(Haptics.Kind.Success); Music.Duck(.6f, 2f); }   // the fanfare comes at .7 s
            _endAt = Time.unscaledTime + (e.Outcome == Outcome.Win ? .35f : .5f);
            // PM_MESAJ_18: the pieces are out, the painting is free; it stays whole for a second before the card
            if (e.Outcome == Outcome.Win && _paintSet && _board != null && _board.Image != null) { ShowFinishedPicture(true); _endAt += 1f; }
        }

        /* The level's picture: its own (G1) or, in SET40 v3, painting-NN for level N (manifest order). */
        string ImageOf(int index) =>
            index < 0 || index >= _set.Levels.Count ? null : _set.Levels[index].Image ?? (_paintSet ? $"painting-{index + 1:00}" : null);

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
        }

        /* immediate: screenshots build several screens in one frame, so the
           old world must be gone before the next one is drawn. */
        void Build(bool immediate = false)
        {
            RememberMenuScroll();
            if (_world != null) { if (immediate) DestroyImmediate(_world.gameObject); else Destroy(_world.gameObject); }
            _world = new GameObject("World").transform;
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
                    menuSafe.y = Mathf.Max(44, menuSafe.y);
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
                    menuSafe.w = _screen.y - bottom;
                }
                if (!Art.On || (!_levelsMapOpen && _tab == MenuTab.Home))
                    _menu = new Screens.Menu(_world, _screen, menuSafe, _set.Levels[_progress.Next()].Number, HudOrder, _newArt, _save.Player, SavedJiggles(), TotalStars(), _showSet ? "Demo" : null);
                // PM_MESAJ_18: the corridor's big frame shows the painting the next level frees
                if (_paintSet && _newArt && _menu != null)
                {
                    var menu = _menu;
                    var pic = _images.Get(ImageOf(_progress.Next()), t => { if (_menu == menu) menu.SetPainting(t); });
                    if (pic != null) _menu.SetPainting(pic);
                }
            }
            else
            {
                _hud = new Hud(_world, _screen.x, _safe.y + BodyPad, HudOrder);
                _hud.SetLevel(_session.Level.Number);
                var origin = BoardPlacement(_screen, _safe, _session.Level, out float cell);
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
                bool firstLevel = _index == 0 && !_designSet && !PlayOverride.Active;
                if (_session.Level.Tutorial == 0 && Tutorial.Hint(_session, firstLevel, _tutorialDone, out var hint))
                    _hand = TutorialHand.Show(_board, _board.PieceViewOf(hint.Piece), hint, HandOrder,
                                              new Rect(-origin, _screen));
                _drag = new DragModel(new SessionDragTarget(_session), cell);
                _bar = new BoosterBar(_world, _screen, _safe.w, BarOrder);
                if (_session.Level.Tutorial > 0) ShowTutorialStep();
                if (_introOpen) OpenCard("intro", Screens.Intro(_world, _screen, _session.Level, CardOrder));
                else if (_session.IsOver && _endAt < 0) ShowResult();
                RefreshPlaying();
            }

            if (Host.Debug)
            {
                _debug = TextView.Create(_world, "debug", "", 11, Draw.Hex("#8fb2b3"), false,
                                         TMPro.TextAlignmentOptions.Left, DebugOrder);
                // in the gap between the strip and the tray, clear of the booster bar
                float dy = _safe.y + BodyPad + Hud.TopPad + Hud.Height + 14;
                _debug.transform.localPosition = Draw.W(Hud.Side + 5, dy);
            }
        }

        /* The web game's sizeCell and flex placement: the board's top-left in
           points and the cell size. Safe insets play the role of the body's
           env(safe-area-inset-*) padding. */
        public static Vector2 BoardPlacement(Vector2 screen, Vector4 safe, LevelData l, out float cell)
        {
            float innerH = screen.y - safe.y - safe.w;
            cell = BoardLayout.CellSize(screen.x, innerH, l.Width, l.Height);
            var size = new Vector2(l.Width * cell, l.Height * cell);
            float stageTop = safe.y + BodyPad + Hud.TopPad + Hud.Height + StripBottom;
            float stageBottom = screen.y - safe.w - BodyPad - BottomReserve;
            return new Vector2((screen.x - size.x) / 2, (stageTop + stageBottom - size.y) / 2);
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
            _card?.Destroy();
            _card = null;
            _cardKind = null;
        }

        void ShowResult() =>
            OpenCard("result", Screens.Result(_world, _screen, _session, _index == _set.Levels.Count - 1, CardOrder,
                PlayOverride.Active ? Texts.BackToEditor : string.Format(Texts.SetComplete, _set.Levels.Count),
                _images.Get(_session.Level.Image),          // G1-b: only a picture that really arrived
                _askFun, _funAnswered, _askHard, _hardAnswered, _showSet));

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

        (AttemptRecord record, string rowId)? _pendingFun;
        bool _funAnswered, _askFun, _hardAnswered, _askHard;

        /* The held row goes out, with the answer when there is one. */
        void SendPendingFun(int? answer = null)
        {
            if (_pendingFun == null) return;
            var (record, rowId) = _pendingFun.Value;
            _pendingFun = null;
            if (answer.HasValue) record.Fun = answer;          // a 👍/👎 given earlier stays on the record
            _telemetry.Finished(record, rowId);
        }

        void RefreshPlaying()
        {
            if (_hud == null) return;
            _hud.SetLeft(_session.Board.PresentCount);
            _hud.SetTime(_session.TimeLeft);
            _bar.Refresh(_boosters, _session);
            bool wantJoints = _boosters.Armed == Booster.Scissors && !_session.IsOver;
            if (wantJoints && !_jointsShown) _board.ShowJoints(_boosters.Joints());
            if (!wantJoints && _jointsShown) _board.HideJoints();
            _jointsShown = wantJoints;
        }

        // ---- frame ------------------------------------------------------------------------

        void Update()
        {
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
            if (!_awaitingName) HandlePointer();
            if (Time.unscaledTime >= _nextFlush) { _nextFlush = Time.unscaledTime + 15f; _telemetry.Flush(); _moveSender?.Flush(); }

            if (_mode == Mode.Playing)
            {
                // the clock runs only while the board is actually in play
                if (!_session.IsOver && !_introOpen && _card == null && _list == null) _session.Tick(Time.unscaledDeltaTime);
                if (_endAt > 0 && Time.unscaledTime >= _endAt) { _endAt = -1; ShowResult(); }
                RefreshPlaying();
            }
            if (_debug != null) UpdateDebug();
        }

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
            Vector2 px = p.position.ReadValue();
            var pt = new Vector2(px.x / _dpr, _screen.y - px.y / _dpr);    // layout points, y down

            if (_mode == Mode.Menu && Art.On)
            {
                if (p.press.wasPressedThisFrame)
                {
                    _menuPointer = true; _menuDragging = false; _menuPointerStart = _menuPointerLast = pt;
                }
                var scroll = ActiveMenuScroll;
                if (_menuPointer && p.press.isPressed && scroll != null && scroll.Contains(_menuPointerStart))
                {
                    if (!_menuDragging && Mathf.Abs(pt.y - _menuPointerStart.y) >= 8) _menuDragging = true;
                    if (_menuDragging) scroll.Move(pt.y - _menuPointerLast.y);
                    _menuPointerLast = pt;
                }
                if (p.press.wasReleasedThisFrame && _menuPointer)
                {
                    _menuPointer = false;
                    bool cancelled = Touchscreen.current != null && p == Touchscreen.current && Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
                    if (!_menuDragging && !cancelled && Vector2.Distance(pt, _menuPointerStart) < 8) Press(pt);
                    RememberMenuScroll();
                }
                return;
            }

            if (p.press.wasPressedThisFrame) Press(pt);
            else if (p.press.isPressed && _drag != null && _drag.Active)
            {
                _drag.Move(pt.x, pt.y, Time.unscaledTimeAsDouble);
                _board.ShowDrag(_drag);
            }
            if (p.press.wasReleasedThisFrame && _drag != null && _drag.Active)
            {
                var lim = _drag.Limit;
                // a touch the system took away (edge swipe, notification) ends as a cancel, not a release
                var touch = Touchscreen.current;
                bool cancelled = touch != null && p == touch &&
                                 touch.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
                int piece; Dir dir;
                var result = cancelled ? _drag.Abort(out piece, out dir) : _drag.Release(out piece, out dir);
                if (_zeydbotSet && result != DragRelease.Exited && piece >= 0)
                    WriteMove("touch", piece, null, (int)((Time.unscaledTime - _pressAt) * 1000));
                _board.EndDrag(piece, result, dir, lim.Cells);
            }
        }

        bool _cardPressed;   // Task 15 card buttons: the pressed sprite shows before the action

        void Press(Vector2 pt)
        {
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
                // Task 13: the pressed look shows for a moment before the menu acts
                if (Art.On && (_menuSettings != null || _paintingDetail != null || _profileCard != null || _menu == null || (_tabs?.Hit(pt) ?? -1) >= 0)) { MenuAction(pt); return; }
                if (_menu != null && _menu.Press(pt)) { var at = pt; Tweens.Run(_menu, .12f, _ => { }, () => { if (_mode == Mode.Menu) MenuAction(at); }); return; }
                MenuAction(pt);
                return;
            }

            if (_card != null)
            {
                if (!_cardPressed && _card.Press(pt))
                {
                    _cardPressed = true;
                    var at = pt;
                    Tweens.Run(_card, .12f, _ => { }, () => { if (_card != null) Press(at); _cardPressed = false; });
                    return;
                }
                string id = _card.HitButton(pt);
                if (id == null) return;
                switch (_cardKind)
                {
                    case "intro": _introOpen = false; CloseCard(); break;
                    case "pause":                                   // the attempt ends as a quit when left (LeaveLevel)
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
                            if (_pendingFun != null && id != "hard_skip") _pendingFun.Value.record.Hardness = int.Parse(id.Substring(5));
                            SendPendingFun();
                            _hardAnswered = true;
                            ShowResult();
                            break;
                        }
                        if (id == "fun_up" || id == "fun_down")
                        {
                            // Build 5: the row also waits for the 1-10 answer (both go in one row)
                            int fun = id == "fun_up" ? 1 : -1;
                            if (_askHard && !_hardAnswered && _pendingFun != null) _pendingFun.Value.record.Fun = fun;
                            else SendPendingFun(fun);
                            _funAnswered = true;
                            ShowResult();
                            break;
                        }
                        SendPendingFun();
                        if (id == "next") { if (!PlayCorridor(false)) StartLevel(_index + 1); }
                        else if (id == "retry") StartLevel(_index);
                        else if (!PlayCorridor(true)) ShowMenu();          // PM_MESAJ_24B: after a win the menu comes through the corridor too
                        break;
                }
                return;
            }

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
            if (piece >= 0 && _drag.Press(piece, pt.x, pt.y, Time.unscaledTimeAsDouble, outside)) { _board.ShowDrag(_drag); _pressAt = Time.unscaledTime; }
        }

        void OpenList()
        {
            int current = _mode == Mode.Playing ? _index : _progress.Next();
            _list = new Screens.LevelList(_world, _screen, _set.Levels, _progress, current, CardOrder + 100);
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
