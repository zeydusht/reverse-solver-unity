using System.Collections.Generic;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReverseSolver.LevelEditor
{
    /* The level editor's screen: the board drawn by the game's own BoardView
       (so every piece looks exactly as in the game) with editing overlays, and
       a panel of tools, settings and the live check. Clicks become calls on
       LevelEditorSession; the board is rebuilt whenever the session changes.

       Coordinates are points = pixels (the editor host's pixel ratio is 1),
       y down, like the game. The panel sits on the right in a landscape Game
       view and at the bottom in a portrait one. */
    public sealed class LevelEditorRoot
    {
        const int OverlayOrder = 2000;
        const float PanelW = 400, PanelH = 360;     // logical GUI units

        static LevelEditorSession S => LevelEditorSession.Current;

        Camera _cam;
        Transform _world;
        BoardView _board;
        ShapeView _hover;
        int _builtRevision = -1;
        Tool _builtTool;
        Vector2Int _builtScreen;
        float _cell;
        Vector2 _origin;
        Cell? _strokeCell;
        bool _stroking;
        Vector2 _scroll;
        bool _openList;
        GUIStyle _h1, _h2, _text, _small, _box, _num;

        public static LevelEditorRoot Instance { get; private set; }
        public float CellSize => _cell;
        public Vector2 Origin => _origin;

        float Ui => Mathf.Clamp(Mathf.Min(Screen.height, Screen.width) / 760f, 1f, 2f);
        bool Landscape => Screen.width >= Screen.height;
        /* The board area in pixels (y down). */
        Rect BoardArea => Landscape
            ? new Rect(0, 0, Screen.width - PanelW * Ui, Screen.height)
            : new Rect(0, 0, Screen.width, Screen.height - PanelH * Ui);
        Rect PanelRect => Landscape        // logical GUI units
            ? new Rect(Screen.width / Ui - PanelW, 0, PanelW, Screen.height / Ui)
            : new Rect(0, Screen.height / Ui - PanelH, Screen.width / Ui, PanelH);

        /* Editor assemblies cannot hold scene components, so the view is a plain
           object driven by an EditorHook. */
        public static LevelEditorRoot Attach()
        {
            var view = new LevelEditorRoot();
            var hook = new GameObject("LevelEditor").AddComponent<Presentation.Dev.EditorHook>();
            hook.OnUpdate = view.Update;
            hook.OnGui = view.OnGUI;
            hook.OnDestroyed = view.OnDestroy;
            view.Start();
            return view;
        }

        void Start()
        {
            Instance = this;
            const string dir = "Assets/_Game/Presentation/Materials/";
            Materials.Init(AssetDatabase.LoadAssetAtPath<Material>(dir + "Piece.mat"),
                           AssetDatabase.LoadAssetAtPath<Material>(dir + "Shape.mat"),
                           AssetDatabase.LoadAssetAtPath<Material>(dir + "Solid.mat"),
                           AssetDatabase.LoadAssetAtPath<Material>(dir + "Background.mat"));
            _cam = Camera.main;
            LevelEditorSession.EnsureStarted();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (S == null) return;
            if (S.Revision != _builtRevision || S.Tool != _builtTool || Screen.width != _builtScreen.x || Screen.height != _builtScreen.y)
                Rebuild();
            HandleMouse();
        }

        // ---- drawing ------------------------------------------------------------------------

        void Rebuild()
        {
            _builtRevision = S.Revision;
            _builtTool = S.Tool;
            _builtScreen = new Vector2Int(Screen.width, Screen.height);
            if (_world != null) Object.Destroy(_world.gameObject);
            _world = new GameObject("EditorWorld").transform;

            var screen = new Vector2(Screen.width, Screen.height);
            _cam.orthographic = true;
            _cam.orthographicSize = screen.y / 2;
            _cam.transform.position = new Vector3(screen.x / 2, -screen.y / 2, -10);
            Background(screen);

            var level = S.Draft.ToLevelData();
            var area = BoardArea;
            _cell = Mathf.Floor(Mathf.Min((area.width - 110) / level.Width, (area.height - 110) / level.Height, 78f));
            _cell = Mathf.Max(_cell, 16f);
            _origin = new Vector2(area.x + (area.width - level.Width * _cell) / 2, area.y + (area.height - level.Height * _cell) / 2);
            var session = new GameSession(level, 1, "editor", new Mulberry32(1));
            _board = BoardView.Create(_world, session, _cell, _origin);
            if (S.ImageTexture != null) _board.SetImage(S.ImageTexture);

            // problem pieces from the live check: red
            foreach (int p in S.Report.Pieces)
                if (p >= 0 && p < level.Pieces.Count)
                    foreach (var c in level.Pieces[p]) Mark(c, Draw.Rgba(232, 103, 76, .30f), Draw.Hex("#e8674c"), 3);
            if (S.Selected is Cell sel) Mark(sel, Draw.Rgba(240, 167, 66, .12f), Draw.Hex("#f0a742"), 3);
            if (S.ChainStart is Cell cs) Mark(cs, Draw.Rgba(90, 200, 250, .25f), Draw.Hex("#5ac8fa"), 4);

            if (S.Tool == Tool.Edge)
            {
                var joints = new List<(int x, int y, bool vertical)>();
                for (int y = 0; y < level.Height; y++) for (int x = 1; x < level.Width; x++) joints.Add((x, y, true));
                for (int y = 1; y < level.Height; y++) for (int x = 0; x < level.Width; x++) joints.Add((x, y, false));
                _board.ShowJoints(joints);
            }
            if (S.Tool == Tool.Wall)
                foreach (var side in DirExt.All)
                    for (int lane = 0; lane < S.Draft.LaneCount(side); lane++)
                        if (!S.Draft.IsSealed(side, lane))
                        {
                            var r = LaneRect(side, lane, level.Width, level.Height);
                            var ghost = ShapeView.Create(_world, "lane", r.size, 4, OverlayOrder).Radius(3)
                                .Fill(Draw.Rgba(255, 255, 255, .07f)).Stroke(Draw.Rgba(255, 255, 255, .28f), 1);
                            ghost.transform.localPosition = Draw.W(_origin + r.center);
                        }

            _hover = ShapeView.Create(_world, "hover", new Vector2(_cell, _cell), 2, OverlayOrder + 10).Radius(6)
                .Fill(Draw.Rgba(255, 255, 255, .10f)).Stroke(Draw.Rgba(255, 255, 255, .45f), 2);
            _hover.gameObject.SetActive(false);
        }

        void Mark(Cell c, Vector4 fill, Vector4 stroke, float width)
        {
            var m = ShapeView.Create(_world, "mark", new Vector2(_cell, _cell), 4, OverlayOrder + 5).Radius(6).Fill(fill).Stroke(stroke, width);
            m.transform.localPosition = Draw.W(_origin + new Vector2((c.X + .5f) * _cell, (c.Y + .5f) * _cell));
        }

        /* The strip outside the board where a wall segment goes (BoardView's wall rect, a bit larger to click). */
        Rect LaneRect(Dir side, int lane, int w, int h)
        {
            float S = _cell, W = w * S, H = h * S, t = Mathf.Max(10f, S * .22f);
            return side switch
            {
                Dir.U => new Rect(lane * S + 2, -t * 1.6f, S - 4, t),
                Dir.D => new Rect(lane * S + 2, H + t * .6f, S - 4, t),
                Dir.L => new Rect(-t * 1.6f, lane * S + 2, t, S - 4),
                _ => new Rect(W + t * .6f, lane * S + 2, t, S - 4)
            };
        }

        void Background(Vector2 screen)
        {
            var go = new GameObject("Background");
            go.transform.SetParent(_world, false);
            var mesh = new Mesh { name = "bg" };
            mesh.vertices = new[] { Draw.W(0, 0), Draw.W(screen.x, 0), Draw.W(screen.x, screen.y), Draw.W(0, screen.y) };
            mesh.uv = new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = Materials.Background;
            r.sortingOrder = -1000;
            var b = new MaterialPropertyBlock();
            b.SetVector("_Screen", screen);
            r.SetPropertyBlock(b);
        }

        // ---- input ---------------------------------------------------------------------------

        /* What a point (pixels, y down) on the board hits for the current tool. */
        public enum HitKind { None, Cell, Joint, Lane }

        public HitKind HitTest(Vector2 pt, out Cell cell, out (int x, int y, bool vertical) joint, out (Dir side, int lane) lane)
        {
            cell = default; joint = default; lane = default;
            var d = S.Draft;
            float fx = (pt.x - _origin.x) / _cell, fy = (pt.y - _origin.y) / _cell;
            bool inside = fx >= 0 && fx < d.Width && fy >= 0 && fy < d.Height;

            if (S.Tool == Tool.Wall && !inside)
            {
                if (fx >= 0 && fx < d.Width && fy < 0 && fy > -1.2f) { lane = (Dir.U, (int)fx); return HitKind.Lane; }
                if (fx >= 0 && fx < d.Width && fy >= d.Height && fy < d.Height + 1.2f) { lane = (Dir.D, (int)fx); return HitKind.Lane; }
                if (fy >= 0 && fy < d.Height && fx < 0 && fx > -1.2f) { lane = (Dir.L, (int)fy); return HitKind.Lane; }
                if (fy >= 0 && fy < d.Height && fx >= d.Width && fx < d.Width + 1.2f) { lane = (Dir.R, (int)fy); return HitKind.Lane; }
                return HitKind.None;
            }
            if (!inside) return HitKind.None;
            if (S.Tool == Tool.Edge)
            {
                float dv = Mathf.Abs(fx - Mathf.Round(fx)), dh = Mathf.Abs(fy - Mathf.Round(fy));
                if (dv <= dh && dv < .3f && d.IsInnerV(Mathf.RoundToInt(fx), (int)fy)) { joint = (Mathf.RoundToInt(fx), (int)fy, true); return HitKind.Joint; }
                if (dh < dv && dh < .3f && d.IsInnerH((int)fx, Mathf.RoundToInt(fy))) { joint = ((int)fx, Mathf.RoundToInt(fy), false); return HitKind.Joint; }
                return HitKind.None;
            }
            cell = new Cell((int)fx, (int)fy);
            return HitKind.Cell;
        }

        /* Same as a click at pt: for scripted runs of the editor (docs screenshots). */
        public void Click(Vector2 pt, bool right = false)
        {
            switch (HitTest(pt, out var c, out var j, out var l))
            {
                case HitKind.Cell: S.ClickCell(c, right); break;
                case HitKind.Joint: S.ClickJoint(j.x, j.y, j.vertical, right); break;
                case HitKind.Lane: S.ClickLane(l.side, l.lane, right); break;
            }
        }

        public Vector2 CellCenter(Cell c) => _origin + new Vector2((c.X + .5f) * _cell, (c.Y + .5f) * _cell);

        bool Brush => S.Tool == Tool.Template || S.Tool == Tool.Paint || S.Tool == Tool.Erase;

        void HandleMouse()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;
            var raw = mouse.position.ReadValue();
            var pt = new Vector2(raw.x, Screen.height - raw.y);
            bool overBoard = BoardArea.Contains(pt) && !_openList;
            Cell cell = default;
            var kind = overBoard ? HitTest(pt, out cell, out _, out _) : HitKind.None;

            if (_hover != null)
            {
                bool show = kind == HitKind.Cell;
                _hover.gameObject.SetActive(show);
                if (show) _hover.transform.localPosition = Draw.W(CellCenter(cell));
            }
            if (!overBoard) { if (mouse.leftButton.wasReleasedThisFrame) EndStroke(); return; }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (Brush) { S.BeginStroke(); _stroking = true; _strokeCell = kind == HitKind.Cell ? cell : (Cell?)null; }
                Click(pt);
            }
            else if (mouse.leftButton.isPressed && _stroking && kind == HitKind.Cell && !cell.Equals(_strokeCell))
            {
                _strokeCell = cell;
                S.ClickCell(cell);
            }
            if (mouse.leftButton.wasReleasedThisFrame) EndStroke();
            if (mouse.rightButton.wasPressedThisFrame) Click(pt, right: true);
        }

        void EndStroke()
        {
            if (!_stroking) return;
            _stroking = false;
            _strokeCell = null;
            S.EndStroke();
        }

        // ---- panel ---------------------------------------------------------------------------

        static readonly (Tool tool, string name)[] Tools =
        {
            (Tool.Select, "Seç"), (Tool.Template, "Parça"), (Tool.Edge, "Kenar"),
            (Tool.Nail, "Çivi"), (Tool.Bomb, "Bomba"), (Tool.Wall, "Mühür"),
            (Tool.Chain, "Zincir"), (Tool.Paint, "Boya"), (Tool.Erase, "Silgi"),
        };

        static string Hint(Tool t) => t switch
        {
            Tool.Select => "Hücreye tıkla: kenarları, rengi ve engeli aşağıda düzenlenir.",
            Tool.Template => "Şablon seç, hücreye tıkla; basılı tutup sürükleyerek birden çok hücreye uygula. Komşu kenarlar kendiliğinden uyar.",
            Tool.Edge => "İki hücre arasındaki turuncu noktaya tıkla: düz → çıkıntı → ters çıkıntı → düz. Sağ tık düzler.",
            Tool.Nail => "Hücreye tıkla: çivi koyar ya da sayacını aşağıdaki sayıya eşitler. Sağ tık kaldırır.",
            Tool.Bomb => "Hücreye tıkla: bomba koyar ya da fitilini aşağıdaki sayıya eşitler. Sağ tık kaldırır.",
            Tool.Wall => "Tahtanın dışındaki soluk şeride tıkla: o şeride duvar koyar ya da kaldırır.",
            Tool.Chain => "İki hücreye sırayla tıkla: zincirle bağlanır (uzak hücreler de olur). Zincirli hücreye sağ tık zinciri ayırır.",
            Tool.Paint => "Renk seç, hücrelere tıkla ya da sürükle. Renk yalnızca görünüştür.",
            _ => "Hücredeki çiviyi, bombayı ve zinciri kaldırır. Her araçta sağ tık da siler.",
        };

        void Styles()
        {
            if (_h1 != null) return;
            _h1 = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, wordWrap = true };
            _h2 = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, margin = new RectOffset(4, 4, 10, 2) };
            _text = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true, richText = true };
            _small = new GUIStyle(_text) { fontSize = 11 };
            _box = new GUIStyle(GUI.skin.box) { padding = new RectOffset(8, 8, 6, 6) };
            _num = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        }

        void OnGUI()
        {
            if (S == null) return;
            Styles();
            GUI.matrix = Matrix4x4.Scale(new Vector3(Ui, Ui, 1));
            Shortcuts();
            var panel = PanelRect;
            GUI.Box(panel, GUIContent.none);
            GUILayout.BeginArea(new Rect(panel.x + 8, panel.y + 8, panel.width - 16, panel.height - 16));
            _scroll = GUILayout.BeginScrollView(_scroll, GUIStyle.none, GUI.skin.verticalScrollbar);
            if (_openList) LevelList(); else Panel();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void Shortcuts()
        {
            var e = Event.current;
            if (e.type != EventType.KeyDown || !(e.control || e.command)) return;
            if (e.keyCode == KeyCode.Z && !e.shift) { S.Undo(); e.Use(); }
            else if (e.keyCode == KeyCode.Y || (e.keyCode == KeyCode.Z && e.shift)) { S.Redo(); e.Use(); }
            else if (e.keyCode == KeyCode.S) { S.Save(); e.Use(); }
        }

        void Panel()
        {
            var d = S.Draft;
            string name = d.Id == null ? "Yeni tasarım" : $"{d.Id} · v{d.Version}";
            GUILayout.Label($"Seviye Editörü — {name}{(S.Dirty ? "  ●" : "")}", _h1);
            GUILayout.Label(S.Dirty ? "Kaydedilmemiş değişiklik var." : (d.Id == null ? "Henüz kaydedilmedi." : "Kaydedildi."), _small);

            GUILayout.BeginHorizontal();
            GUI.enabled = S.UndoCount > 0;
            if (GUILayout.Button($"Geri al ({S.UndoCount})")) S.Undo();
            GUI.enabled = S.RedoCount > 0;
            if (GUILayout.Button($"Yinele ({S.RedoCount})")) S.Redo();
            GUI.enabled = true;
            if (GUILayout.Button("Kaydet")) S.Save();
            if (GUILayout.Button("Oyna ▶")) { LevelEditorLauncher.PlayDraft(); return; }
            GUILayout.EndHorizontal();

            Status();
            Check();

            GUILayout.Label("Araç", _h2);
            int toolIndex = System.Array.FindIndex(Tools, t => t.tool == S.Tool);
            int picked = GUILayout.SelectionGrid(toolIndex, System.Array.ConvertAll(Tools, t => t.name), 5);
            if (picked != toolIndex) { S.Tool = Tools[picked].tool; S.ChainStart = null; }
            GUILayout.Label(Hint(S.Tool), _small);
            ToolOptions();

            if (S.Selected is Cell c) SelectedCell(c);

            GUILayout.Label("Seviye", _h2);
            int timer = Stepper("Süre (sn)", d.Timer, 5);
            if (timer != d.Timer) S.Edit(x => x.SetTimer(timer));
            int lv = Stepper("Bölüm no", d.Number, 1);
            if (lv != d.Number && lv >= 1) S.Edit(x => { x.Number = lv; return EditResult.Done; });
            GUILayout.Label("Bölüm no, güçlendiricilerin açılışını belirler (makas 3, değnek 6, çekiç 9, saat 12).", _small);
            ImageSection();

            GUILayout.Label("Dosya", _h2);
            GUILayout.BeginHorizontal();
            _newW = Stepper("Genişlik", _newW, 1, LevelDraft.MinSize, LevelDraft.MaxSize);
            _newH = Stepper("Yükseklik", _newH, 1, LevelDraft.MinSize, LevelDraft.MaxSize);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Yeni {_newW}×{_newH} tahta") && ConfirmDiscard()) S.NewBoard(_newW, _newH);
            if (GUILayout.Button("Boyutu değiştir")) S.Edit(x => x.Resize(_newW, _newH), $"Boyut {_newW}×{_newH} oldu.");
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Seviye aç… (web seviyesi kopya olarak, ya da tasarım)")) _openList = true;
            if (GUILayout.Button("Tüm seviyeleri CSV'ye aktar (Excel)"))
            {
                string path = S.ExportCsv();
                if (!LevelEditorSession.Unattended) EditorUtility.RevealInFinder(path);
            }
            if (d.Id != null && S.Store.Find(d.Id) != null && GUILayout.Button($"{d.Id} tasarımını sil") && ConfirmDelete(d.Id)) S.Delete();
            GUILayout.Label($"Tasarımlar: {LevelEditorSession.DesignsPath} ({S.Store.Designs.Count}). Telefonda: ?debug=1&set=designs&lv=D01", _small);
        }

        int _newW = 5, _newH = 6;

        /* G1: the level's picture. */
        void ImageSection()
        {
            var d = S.Draft;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(d.Image == null ? "Görsel seç…" : "Görseli değiştir…"))
            {
                string picked = EditorUtility.OpenFilePanelWithFilters("Seviye görseli", "", new[] { "Görsel", "png,jpg,jpeg" });
                if (!string.IsNullOrEmpty(picked)) S.ChooseImage(picked);
            }
            GUI.enabled = d.Image != null;
            if (GUILayout.Button("Görseli kaldır")) S.RemoveImage();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (S.ImageTexture != null)
            {
                var rect = GUILayoutUtility.GetRect(160, 120, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(rect, S.ImageTexture, ScaleMode.ScaleToFit);
                GUILayout.Label($"{d.Image} · {S.ImageTexture.width}×{S.ImageTexture.height} px. Görsel yalnızca görünüştür, sürümü değiştirmez.", _small);
            }
            else GUILayout.Label("Görsel yok: parçalar seviyenin renkleriyle görünür.", _small);
        }

        void Status()
        {
            if (string.IsNullOrEmpty(S.Status)) return;
            var old = GUI.color;
            GUI.color = S.StatusIsError ? new Color(1f, .62f, .5f) : new Color(.75f, .95f, .85f);
            GUILayout.Label(S.Status, _text);
            GUI.color = old;
        }

        void Check()
        {
            var r = S.Report;
            var old = GUI.backgroundColor;
            GUI.backgroundColor = r.FollowsRule ? new Color(.35f, .8f, .5f) : r.Verdict == DesignVerdict.Unverified ? new Color(.95f, .75f, .3f) : new Color(.95f, .4f, .35f);
            GUILayout.BeginVertical(_box);
            GUI.backgroundColor = old;
            GUILayout.Label(r.Message, _text);
            var st = r.Stats;
            GUILayout.Label($"Tahta {st.Width}×{st.Height} · parça {st.Pieces} · zincir {st.Chains} · çivi {st.Nails} · bomba {st.Bombs} · duvar {st.SealedLanes} · açık kenar %{st.OpenPercent}", _small);
            GUILayout.Label($"Çözüm {r.Solution.Count} hamle · kontrol {S.CheckMs:0} ms", _small);
            if (r.Verdict == DesignVerdict.Unverified && GUILayout.Button("Derin kontrol (birkaç saniye sürebilir)")) S.DeepCheck();
            GUILayout.Label(r.Fits
                ? $"Telefonda (375×667) hücre {r.CellSize} pt."
                : $"<b>Uyarı:</b> telefonda (375×667) hücre {r.CellSize} pt, 44 pt'nin altında: parmakla zor oynanır.", _small);
            GUILayout.EndVertical();
        }

        void ToolOptions()
        {
            switch (S.Tool)
            {
                case Tool.Template:
                    S.Template = GUILayout.SelectionGrid(S.Template, System.Array.ConvertAll(PieceTemplate.All, t => t.Name), 2);
                    break;
                case Tool.Nail:
                    S.NailCount = Stepper("Çivi sayacı", S.NailCount, 1, 1, LevelDraft.MaxCount);
                    break;
                case Tool.Bomb:
                    S.BombFuse = Stepper("Bomba fitili", S.BombFuse, 1, 1, LevelDraft.MaxCount);
                    break;
                case Tool.Paint:
                    S.Color = Swatches(S.Color);
                    break;
            }
        }

        void SelectedCell(Cell c)
        {
            var d = S.Draft;
            if (!d.OnBoard(c)) return;
            GUILayout.Label($"Seçili hücre ({c.X}, {c.Y})", _h2);
            int chain = d.ChainAt(c);
            if (chain >= 0)
            {
                var ch = d.Chains[chain];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Zincirli: ({ch.A.X},{ch.A.Y}) — ({ch.B.X},{ch.B.Y})", _text);
                if (GUILayout.Button("Zinciri ayır", GUILayout.Width(110))) S.Edit(x => x.RemoveChain(c), "Zincir ayrıldı.");
                GUILayout.EndHorizontal();
            }
            foreach (var (dir, label) in new[] { (Dir.U, "Üst"), (Dir.R, "Sağ"), (Dir.D, "Alt"), (Dir.L, "Sol") })
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(label, _text, GUILayout.Width(40));
                var side = d.SideOf(c, dir);
                var names = new[] { "Girinti", "Düz", "Çıkıntı" };
                int now = (int)side + 1;
                int pick = GUILayout.Toolbar(now, names);
                if (pick != now) S.Edit(x => x.SetSide(c, dir, (Side)(pick - 1)));
                GUILayout.EndHorizontal();
            }
            int nail = Stepper("Çivi", d.NailAt(c), 1, 0, LevelDraft.MaxCount);
            if (nail != d.NailAt(c)) S.Edit(x => x.SetNail(c, nail));
            int fuse = Stepper("Bomba", d.BombAt(c), 1, 0, LevelDraft.MaxCount);
            if (fuse != d.BombAt(c)) S.Edit(x => x.SetBomb(c, fuse));
            int color = Swatches(d.ColorAt(c));
            if (color != d.ColorAt(c)) S.Edit(x => x.SetColor(c, color));
        }

        int Swatches(int current)
        {
            var d = S.Draft;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Renk", _text, GUILayout.Width(40));
            for (int i = 0; i < d.Palette.Count; i++)
            {
                ColorUtility.TryParseHtmlString(d.Palette[i], out var col);
                if (GUILayout.Button("", GUILayout.Width(44), GUILayout.Height(22))) current = i;
                var rect = GUILayoutUtility.GetLastRect();
                var old = GUI.color;
                GUI.color = col;
                GUI.DrawTexture(new Rect(rect.x + 3, rect.y + 3, rect.width - 6, rect.height - 6), Texture2D.whiteTexture);
                GUI.color = old;
                if (i == current) GUI.Label(rect, "✓", _num);
            }
            GUILayout.EndHorizontal();
            return current;
        }

        int Stepper(string label, int value, int step, int min = 0, int max = 999)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _text, GUILayout.Width(80));
            if (GUILayout.Button("−", GUILayout.Width(28))) value -= step;
            GUILayout.Label(value.ToString(), _num, GUILayout.Width(36), GUILayout.Height(20));
            if (GUILayout.Button("+", GUILayout.Width(28))) value += step;
            GUILayout.EndHorizontal();
            return Mathf.Clamp(value, min, max);
        }

        void LevelList()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Seviye aç", _h1);
            if (GUILayout.Button("Kapat", GUILayout.Width(80))) _openList = false;
            GUILayout.EndHorizontal();
            GUILayout.Label("Tasarımlar (düzenlemek için açılır)", _h2);
            if (S.Store.Designs.Count == 0) GUILayout.Label("Henüz tasarım yok.", _small);
            Grid(S.Store.Designs.Count, i => $"{S.Store.Designs[i].Id} v{S.Store.Designs[i].Version}",
                 i => { if (ConfirmDiscard()) S.OpenDesign(S.Store.Designs[i].Id); });
            GUILayout.Label("Web seviyeleri (kopya olarak açılır, kendileri değişmez)", _h2);
            Grid(S.WebLevels.Levels.Count, i => S.WebLevels.Levels[i].Id,
                 i => { if (ConfirmDiscard()) S.OpenCopy(S.WebLevels.Levels[i].Id); });
        }

        void Grid(int count, System.Func<int, string> label, System.Action<int> open)
        {
            const int perRow = 5;
            for (int i = 0; i < count; i += perRow)
            {
                GUILayout.BeginHorizontal();
                for (int k = i; k < Mathf.Min(count, i + perRow); k++)
                    if (GUILayout.Button(label(k))) { open(k); _openList = false; }
                GUILayout.EndHorizontal();
            }
        }

        bool ConfirmDiscard() =>
            !S.Dirty || LevelEditorSession.Unattended ||
            EditorUtility.DisplayDialog("Seviye Editörü", "Kaydedilmemiş değişiklikler kaybolacak. Devam edilsin mi?", "Devam", "Vazgeç");

        bool ConfirmDelete(string id) =>
            LevelEditorSession.Unattended ||
            EditorUtility.DisplayDialog("Seviye Editörü", $"{id} silinsin mi? Bu id bir daha kullanılmaz.", "Sil", "Vazgeç");
    }
}
