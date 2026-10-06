using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReverseSolver.Presentation
{
    /* Boots the game: reads the levels, lays the screen out in points, runs one
       GameSession at a time and turns pointer input into DragModel calls.

       Screen layout (points, y down), the web game's flex column:
         safe top + 4 | strip (10 + 46 + 6) | stage: tray centred |
         prompt 17 + booster bar 97 (bar from M4) | 4 + safe bottom
       Cell size is the web game's sizeCell: min((W - 38) / w, (H - 262) / h, 78).

       URL: ?lv=N opens level N; ?debug=1 shows the load panel and the FPS line. */
    public sealed class GameRoot : MonoBehaviour
    {
        public const string Client = "unity-web";
        const float BodyPad = 4f, StripBottom = 6f, BottomReserve = 17f + 97f, MaxCell = 78f;
        const int HudOrder = 3000, CardOrder = 5000, FlashOrder = 4500, DebugOrder = 6000;

        [SerializeField] TextAsset levels;
        [SerializeField] Material pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial;

        LevelSet _set;
        readonly AttemptLog _log = new AttemptLog();
        int _index;
        GameSession _session;
        DragModel _drag;

        Camera _cam;
        Transform _world;
        BoardView _board;
        Hud _hud;
        EndCard _card;
        TextView _debug;
        Vector2Int _screenPx;
        Vector2 _screen;            // points
        float _dpr = 1;
        float _endAt = -1;

        readonly List<float> _dragFps = new List<float>();
        float _fpsAcc; int _fpsFrames; float _fpsShown;

        void Start()
        {
            Materials.Init(pieceMaterial, shapeMaterial, solidMaterial, backgroundMaterial);
            _cam = Camera.main;
            _set = LevelParser.Parse(levels.text);
            int lv = int.TryParse(Host.Current.Query("lv"), out int n) ? n : 1;
            StartLevel(Mathf.Clamp(lv, 1, _set.Levels.Count) - 1);
        }

        // ---- levels ---------------------------------------------------------------------

        void StartLevel(int index)
        {
            if (_session != null && !_session.IsOver) _session.Quit();   // restart / next: a quit row
            if (_session != null) _session.EventRaised -= OnEvent;
            _index = Mathf.Clamp(index, 0, _set.Levels.Count - 1);
            var level = _set.Levels[_index];
            _session = _log.Start(level, Client, new Mulberry32((uint)System.Environment.TickCount));
            _session.EventRaised += OnEvent;
            _endAt = -1;
            Layout();
        }

        void OnEvent(GameEvent e)
        {
            if (e.Kind != EventKind.Finished) return;
            _log.Add(_session.Record);
            if (Host.Debug) Debug.Log("[attempt] " + _session.Record);
            if (_drag != null) _drag.Cancel();
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
            var safe = Host.Current.SafeInsets / (Application.isEditor ? _dpr : 1f);

            if (_world != null) Destroy(_world.gameObject);
            _world = new GameObject("World").transform;
            _card = null;

            _cam.orthographic = true;
            _cam.orthographicSize = _screen.y / 2;
            _cam.transform.position = new Vector3(_screen.x / 2, -_screen.y / 2, -10);

            Background();

            _hud = new Hud(_world, _screen.x, safe.y + BodyPad, HudOrder);
            _hud.SetLevel(_session.Level.Number);

            var origin = BoardPlacement(_screen, safe, _session.Level, out float cell);
            _board = BoardView.Create(_world, _session, cell, origin);
            _drag = new DragModel(new SessionDragTarget(_session), cell);

            if (Host.Debug)
            {
                _debug = TextView.Create(_world, "debug", "", 11, Draw.Hex("#8fb2b3"), false,
                                         TMPro.TextAlignmentOptions.Left, DebugOrder);
                _debug.transform.localPosition = Draw.W(Hud.Side + 5, _screen.y - safe.w - 14);
            }

            if (_session.IsOver) ShowCard();
            RefreshHud();
        }

        /* The web game's sizeCell and flex placement: the board's top-left in
           points and the cell size. Safe insets play the role of the body's
           env(safe-area-inset-*) padding. */
        public static Vector2 BoardPlacement(Vector2 screen, Vector4 safe, LevelData l, out float cell)
        {
            float innerH = screen.y - safe.y - safe.w;
            cell = Mathf.Floor(Mathf.Min((screen.x - 38f) / l.Width, (innerH - 262f) / l.Height, MaxCell));
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
            Tweens.Run(f, duration, t => f.Alpha(.35f * (1 - t)), () => Destroy(f.gameObject));
        }

        void ShowCard()
        {
            if (_card != null) return;
            // shown at once; card animations belong to the polish step (M6)
            _card = new EndCard(_world, _screen, _session, _index == _set.Levels.Count - 1, CardOrder);
        }

        void RefreshHud()
        {
            _hud.SetLeft(_session.Board.PresentCount);
            _hud.SetTime(_session.TimeLeft);
        }

        // ---- frame --------------------------------------------------------------------------

        void Update()
        {
            if (Screen.width != _screenPx.x || Screen.height != _screenPx.y)
            {
                _drag?.Cancel();
                Layout();
            }

            HandlePointer();

            if (!_session.IsOver) _session.Tick(Time.unscaledDeltaTime);
            if (_endAt > 0 && Time.unscaledTime >= _endAt) { _endAt = -1; ShowCard(); }
            RefreshHud();
            if (_debug != null) UpdateDebug();
        }

        void OnApplicationFocus(bool focus)
        {
            if (focus || _drag == null || !_drag.Active) return;
            int piece = _drag.Piece;
            _drag.Cancel();
            _board.EndDrag(piece, DragRelease.SnappedBack, Dir.U, 0);
        }

        void HandlePointer()
        {
            var p = Pointer.current;
            if (p == null) return;
            Vector2 px = p.position.ReadValue();
            var pt = new Vector2(px.x / _dpr, _screen.y - px.y / _dpr);    // layout points, y down

            if (p.press.wasPressedThisFrame) Press(pt);
            else if (p.press.isPressed && _drag.Active)
            {
                _drag.Move(pt.x, pt.y);
                _board.ShowDrag(_drag);
            }
            if (p.press.wasReleasedThisFrame && _drag.Active)
            {
                var lim = _drag.Limit;
                var result = _drag.Release(out int piece, out var dir);
                _board.EndDrag(piece, result, dir, lim.Cells);
            }
        }

        void Press(Vector2 pt)
        {
            if (_card != null)
            {
                if (_card.PrimaryRect.Contains(pt)) StartLevel(_card.PrimaryIsNext ? _index + 1 : _index);
                else if (_card.SecondaryRect.Contains(pt)) StartLevel(_index);
                return;
            }
            if (_hud.RestartRect.Contains(pt)) { StartLevel(_index); return; }
            if (_session.IsOver) return;
            int piece = _board.PieceAt(pt);
            if (piece >= 0 && _drag.Press(piece, pt.x, pt.y)) _board.ShowDrag(_drag);
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
            _debug.Text = $"{_session.Level.Id}  FPS {_fpsShown:0}  sürüklerken medyan {median}  hücre {_board.Cell:0} pt";
        }

        // ---- editor screenshots (M3 criterion 1) ---------------------------------------------

        /* Renders the given level at a point size and scale into a PNG. Play mode only.
           state: "" board at start; "win" / "bomb" / "time" the result card after
           playing it out; "charset" adds a line with every Turkish letter. */
        public string RenderShot(string levelId, int widthPt, int heightPt, int scale, string path, string state = "")
        {
            int idx = -1;
            for (int i = 0; i < _set.Levels.Count; i++) if (_set.Levels[i].Id == levelId) idx = i;
            if (idx < 0) return "no level " + levelId;

            var rt = new RenderTexture(widthPt * scale, heightPt * scale, 24);
            var prevTarget = _cam.targetTexture;
            if (_session != null) { _session.EventRaised -= OnEvent; if (!_session.IsOver) _session.Quit(); }
            _index = idx;
            _session = _log.Start(_set.Levels[idx], Client, new Mulberry32(1));
            switch (state)
            {
                case "win": foreach (var m in SmartPlayer.Play(_session.Level).Moves) _session.TryExit(m.Piece, m.Dir); break;
                case "bomb": foreach (var m in _session.Level.Solution) if (!_session.IsOver) _session.TryExit(m.Piece, m.Dir); break;
                case "time": _session.Tick(_session.TimeLeft); break;
            }

            // lay out for the requested size instead of the editor window
            _screenPx = new Vector2Int(rt.width, rt.height);
            _dpr = scale;
            _screen = new Vector2(widthPt, heightPt);
            LayoutFor(_screen);
            if (_session.IsOver) ShowCard();
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
            string info = $"{levelId} {widthPt}x{heightPt}: cell {_board.Cell} pt";
            _screenPx = Vector2Int.zero;      // forces a normal layout next frame
            return info;
        }

        void LayoutFor(Vector2 screen)
        {
            // Same as Layout() but with an explicit screen size and no safe-area insets.
            if (_world != null) DestroyImmediate(_world.gameObject);
            _world = new GameObject("World").transform;
            _card = null;
            _cam.orthographic = true;
            _cam.orthographicSize = screen.y / 2;
            _cam.transform.position = new Vector3(screen.x / 2, -screen.y / 2, -10);
            Background();
            _hud = new Hud(_world, screen.x, BodyPad, HudOrder);
            _hud.SetLevel(_session.Level.Number);
            var origin = BoardPlacement(screen, Vector4.zero, _session.Level, out float cell);
            _board = BoardView.Create(_world, _session, cell, origin);
            _drag = new DragModel(new SessionDragTarget(_session), cell);
            RefreshHud();
        }
    }
}
