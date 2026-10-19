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
        public const string Client = "unity-web";
        const float BodyPad = 4f, StripBottom = 6f, BottomReserve = BoosterBar.PromptH + BoosterBar.Height, MaxCell = 78f;
        const int HandOrder = 1400, HudOrder = 3000, BarOrder = 3200, CardOrder = 5000, FlashOrder = 4500, DebugOrder = 6000;

        [SerializeField] TextAsset levels;
        [SerializeField] TextAsset designs;          // level editor output; only with ?debug=1&set=designs
        [SerializeField] TextAsset zeydbot;          // ZeydBot sessions (?set=zeydbot&lv=Z01), moves sent too
        [SerializeField] TextAsset redesign;         // Faz 4 set (R..); ?debug=1&set=redesign, default only with SetFlags.RedesignDefault
        [SerializeField] Material pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial;

        enum Mode { Menu, Playing }

        LevelSet _set;
        AttemptLog _log;
        SaveData _save;
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
        bool _zeydbotSet;             // playing zeydbot.json (?set=zeydbot): every move is recorded and sent
        TelemetryQueue _moveQueue;
        TelemetrySender _moveSender;
        float _moveT0, _moveLast;
        int _moveNo;
        float _pressAt;
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
            Materials.Init(pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial);
            _images = LevelImages.Create();
            _cam = Camera.main;
            _set = PlayOverride.Active ? PlayOverride.Set : ChooseSet();

            // saved counters and the send queue (M5-pre). Sending is off in the
            // editor, with ?debug=1, and in builds until the switch is turned on.
            System.Func<string> newId = () => System.Guid.NewGuid().ToString("N");
            var host = Host.Current;
            _save = new SaveData(host.Store, newId);
            _log = _save.Restore();
            var queue = new TelemetryQueue(host.Store);
            bool send = host.SendEnabled && !Host.Debug && !Application.isEditor;
            var sender = new TelemetrySender(queue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send);
            _telemetry = new TelemetryLog(_save, queue, sender, newId);
            if (_zeydbotSet)
            {
                _moveQueue = new TelemetryQueue(host.Store, "moves");
                _moveSender = new TelemetrySender(_moveQueue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "moves?on_conflict=row_id,move_no");
            }
            host.VisibilityChanged += OnVisibility;
            _telemetry.Flush();                               // rows left from an earlier visit

            _progress = new Progress(_log, _set.Levels) { UnlockAll = Host.Debug || _zeydbotSet };
            // web name gate: once per browser; the HTML input sits over the canvas
            bool askName = string.IsNullOrEmpty(_save.Player) && !Host.Debug;
            int requested = PlayOverride.Active ? PlayOverride.Index
                          : TryLevelIndex(host.Query("lv"), out int index) ? index : -1;
            _awaitingName = askName;
            LaunchPlan.Run(requested, askName, StartLevel, ShowMenu, host.AskName,
                           name => { _save.SetPlayer(name); _awaitingName = false; });
        }

        LevelSet ChooseSet()
        {
            switch (SetChoice.For(Host.Debug, Host.Current.Query("set"), HasLevels(designs), HasLevels(redesign), SetFlags.RedesignDefault, HasLevels(zeydbot)))
            {
                case SetChoice.Kind.Zeydbot: _zeydbotSet = true; return LevelParser.Parse(zeydbot.text);
                case SetChoice.Kind.Designs: _designSet = true; return LevelParser.Parse(designs.text);
                case SetChoice.Kind.Redesign: return LevelParser.Parse(redesign.text);
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
            WriteMove(kind, e.Piece, e.Kind == EventKind.PieceRemoved && !e.ByHammer ? e.Dir.ToString() : null);
        }

        /* One moves row; touch rows carry how long the piece was held. Pause / resume and touches do not
           reset the thinking clock: think_ms runs from the last real action. */
        void WriteMove(string kind, int piece, string dir, int? holdMs = null)
        {
            if (_moveQueue == null || _session == null) return;
            float now = Time.unscaledTime;
            var fuses = string.Join(";", System.Linq.Enumerable.Select(_session.Bombs, b => b.Key + ":" + b.Value));
            _moveQueue.Add(_rowId + "-" + (++_moveNo), MoveRow.Json(_rowId, _save.Player, _session.Level.Id, _session.Level.Version, _session.Attempt,
                _moveNo, kind, piece, dir, (int)((now - _moveT0) * 1000), (int)((now - _moveLast) * 1000), _session.TimeLeft, fuses, holdMs));
            bool action = kind != "touch" && kind != "pause" && kind != "resume";
            if (action) _moveLast = now;
            if (kind == "pause") _moveSender?.Flush();
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
            if (_zeydbotSet && _mode == Mode.Playing && _session != null && !_session.IsOver) WriteMove(visible ? "resume" : "pause", -1, null);
            if (visible) _telemetry.PageVisible(_mode == Mode.Playing ? _session : null, _rowId);
            else
            {
                SendPendingFun();                                 // a closing tab must not lose the won row
                _telemetry.PageHidden(_mode == Mode.Playing ? _session : null, _rowId);
                _save.Store(_log);
            }
        }

        // ---- flow -------------------------------------------------------------------------

        void LeaveLevel()
        {
            if (_session == null) return;
            if (!_session.IsOver) _session.Quit();          // OnEvent logs the quit row
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
            string nextImage = _index + 1 < _set.Levels.Count ? _set.Levels[_index + 1].Image : null;
            _images.KeepOnly(level.Image, nextImage);
            _images.Get(nextImage);
            SendPendingFun();                                  // G8: a won row never waits past the next level
            _funAnswered = false;
            _askFun = false;
            _hardAnswered = false;
            _session = _log.Start(level, Client, new Mulberry32((uint)System.Environment.TickCount));
            _rowId = _telemetry.NewRowId();
            _save.Store(_log);                                 // the attempt counter, before anything can go wrong
            _session.EventRaised += OnEvent;
            _boosters = new BoosterControls(_session);
            _moveT0 = _moveLast = Time.unscaledTime;
            _moveNo = 0;
            _mode = Mode.Playing;
            _endAt = -1;
            // web loadLevel(i): the intro shows on a level's first start, not on retries
            _introOpen = Screens.HasIntro(level) && _introSeen.Add(level.Id);
            Layout();
            _board?.Deal();                                    // web render(true): pieces deal in on a fresh start
        }

        void OnEvent(GameEvent e)
        {
            if (_zeydbotSet) RecordMove(e);
            if (e.Kind == EventKind.ValveReleased) _bar?.Notice("Çivi kendiliğinden söküldü", 1.6f);   // web says "söktü"; PM: grammar fix
            if (e.Kind == EventKind.PieceRemoved)
            {
                _tutorialDone = true;                          // web clearTutorial() in flyOut and the hammer
                if (_hand != null) { _hand.Remove(); _hand = null; }
            }
            if (e.Kind != EventKind.Finished) return;
            _log.Add(_session.Record);
            _save.Store(_log);
            // G8: the attempt that gets the 👍/👎 question waits for the answer (or the card closing)
            // before its row goes; asked once per level, on the win or after the third loss
            string levelId = _session.Level.Id;
            bool ask = SetFlags.AskFun && FunAsk.ShouldAsk(e.Outcome, _save.LossesOf(levelId), _save.WasFunAsked(levelId));
            if (SetFlags.AskFun && (e.Outcome == Outcome.Time || e.Outcome == Outcome.Bomb)) _save.RecordLoss(levelId);
            _askFun = ask;
            if (ask) { _save.MarkFunAsked(levelId); _pendingFun = (_session.Record, _rowId); }
            else if (_zeydbotSet && e.Outcome != Outcome.Quit) _pendingFun = (_session.Record, _rowId);   // waits for the 1-10 answer
            else _telemetry.Finished(_session.Record, _rowId);
            if (Host.Debug) Debug.Log("[attempt] " + _session.Record);
            _drag?.Cancel();
            _boosters?.Disarm();
            if (e.Outcome == Outcome.Quit) return;
            if (e.Outcome == Outcome.Bomb) Flash(Draw.Hex("#e8674c"), .45f);   // bomb: red flash + swell
            _endAt = Time.unscaledTime + (e.Outcome == Outcome.Win ? .35f : .5f);
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
            if (_world != null) { if (immediate) DestroyImmediate(_world.gameObject); else Destroy(_world.gameObject); }
            _world = new GameObject("World").transform;
            _card = null; _cardKind = null; _menu = null; _list = null; _board = null; _hud = null; _bar = null;
            _jointsShown = false;

            _cam.orthographic = true;
            _cam.orthographicSize = _screen.y / 2;
            _cam.transform.position = new Vector3(_screen.x / 2, -_screen.y / 2, -10);
            Background();

            if (_mode == Mode.Menu)
            {
                _menu = new Screens.Menu(_world, _screen, _safe, _set.Levels[_progress.Next()].Number, HudOrder);
            }
            else
            {
                _hud = new Hud(_world, _screen.x, _safe.y + BodyPad, HudOrder);
                _hud.SetLevel(_session.Level.Number);
                var origin = BoardPlacement(_screen, _safe, _session.Level, out float cell);
                _board = BoardView.Create(_world, _session, cell, origin);
                if (_session.Level.Image != null)
                {
                    var board = _board;
                    var ready = _images.Get(_session.Level.Image, t => { if (board != null && _board == board) board.SetImage(t, .25f); });
                    if (ready != null) _board.SetImage(ready);
                }
                _hand = null;
                bool firstLevel = _index == 0 && !_designSet && !PlayOverride.Active;
                if (Tutorial.Hint(_session, firstLevel, _tutorialDone, out var hint))
                    _hand = TutorialHand.Show(_board, _board.PieceViewOf(hint.Piece), hint, HandOrder,
                                              new Rect(-origin, _screen));
                _drag = new DragModel(new SessionDragTarget(_session), cell);
                _bar = new BoosterBar(_world, _screen, _safe.w, BarOrder);
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
                PlayOverride.Active ? "Editöre dön" : $"Tebrikler, {_set.Levels.Count} bölüm bitti",
                _images.Get(_session.Level.Image),          // G1-b: only a picture that really arrived
                _askFun, _funAnswered, _zeydbotSet, _hardAnswered));

        // ---- G8: the 👍/👎 answer (SetFlags.AskFun) -------------------------------------------

        (AttemptRecord record, string rowId)? _pendingFun;
        bool _funAnswered, _askFun, _hardAnswered;

        /* The held row goes out, with the answer when there is one. */
        void SendPendingFun(int? answer = null)
        {
            if (_pendingFun == null) return;
            var (record, rowId) = _pendingFun.Value;
            _pendingFun = null;
            record.Fun = answer;
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

        void Press(Vector2 pt)
        {
            // level list (over menu or game)
            if (_list != null)
            {
                int hit = _list.Hit(pt);
                if (hit == -2) { _list.Card.Destroy(); _list = null; }
                else if (hit >= 0) StartLevel(hit);
                return;
            }

            if (_mode == Mode.Menu)
            {
                if (_menu.Play.Contains(pt)) StartLevel(_progress.Next());
                else if (_menu.Levels.Contains(pt)) OpenList();
                return;
            }

            if (_card != null)
            {
                string id = _card.HitButton(pt);
                if (id == null) return;
                switch (_cardKind)
                {
                    case "intro": _introOpen = false; CloseCard(); break;
                    case "ask":
                        if (id == "yes") _boosters.Confirm(); else _boosters.CancelAsk();
                        CloseCard();
                        break;
                    case "result":
                        if (id.StartsWith("hard_") && int.TryParse(id.Substring(5), out int hard))
                        {
                            if (_pendingFun != null) _pendingFun.Value.record.Hardness = hard;
                            SendPendingFun();
                            _hardAnswered = true;
                            ShowResult();
                            break;
                        }
                        if (id == "fun_up" || id == "fun_down")
                        {
                            SendPendingFun(id == "fun_up" ? 1 : -1);
                            _funAnswered = true;
                            ShowResult();
                            break;
                        }
                        SendPendingFun();
                        if (id == "next") StartLevel(_index + 1);
                        else if (id == "retry") StartLevel(_index);
                        else ShowMenu();
                        break;
                }
                return;
            }

            if (_hud.RestartRect.Contains(pt)) { StartLevel(_index); return; }
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
