using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using UnityEditor;

namespace ReverseSolver.LevelEditor
{
    public enum Tool { Select, Template, Edge, Nail, Bomb, Wall, Chain, Paint, Erase }

    /* The level editor's state and every action its buttons and clicks take.
       It outlives the editor's scene (Play loads the game scene and back), so
       a design is never lost by trying it out. The view (LevelEditorRoot)
       only draws this and turns clicks into the calls below. */
    public sealed class LevelEditorSession
    {
        public const string DesignsPath = "Assets/_Game/Levels/designs.json";
        public const string LevelsPath = "Assets/_Game/Levels/levels.json";
        public const int UndoLimit = 100;

        public static LevelEditorSession Current { get; private set; }

        /* Dialogs need a person; automated runs turn them off. */
        public static bool Unattended;

        public LevelDraft Draft { get; private set; }
        public DesignStore Store { get; private set; }
        public LevelSet WebLevels { get; }

        public Tool Tool = Tool.Template;
        public int Template;
        public int NailCount = 5, BombFuse = 15, Color = 1;
        public Cell? Selected;
        public Cell? ChainStart;

        public DesignReport Report { get; private set; }
        public double CheckMs { get; private set; }
        public string Status { get; private set; } = "";
        public bool StatusIsError { get; private set; }
        /* Bumped on every change; the view rebuilds when it moves. */
        public int Revision { get; private set; }

        readonly List<LevelDraft> _undo = new List<LevelDraft>();
        readonly List<LevelDraft> _redo = new List<LevelDraft>();
        string _savedJson;
        bool _inStroke;

        public int UndoCount => _undo.Count;
        public int RedoCount => _redo.Count;
        public bool Dirty => Json(Draft) != _savedJson;

        LevelEditorSession()
        {
            WebLevels = LevelParser.Parse(File.ReadAllText(LevelsPath));
            Store = DesignStore.Parse(File.Exists(DesignsPath) ? File.ReadAllText(DesignsPath) : "");
            NewBoard(5, 6);
            Say("Yeni 5×6 tahta. Sağdaki panelden araç seç, tahtaya tıkla.");
        }

        public static LevelEditorSession EnsureStarted() => Current ??= new LevelEditorSession();

        /* Drops the session (the editor was closed). */
        public static void End() => Current = null;

        static string Json(LevelDraft d)
        {
            var w = new JsonWriter();
            d.WriteJson(w);
            return w.ToString();
        }

        // ---- open / new ------------------------------------------------------------------

        public void NewBoard(int width, int height)
        {
            var d = LevelDraft.New(width, height);
            d.Number = 1;
            Replace(d);
            Say($"Yeni {d.Width}×{d.Height} tahta.");
        }

        /* A web level opens as a new design; levels.json is never written. */
        public void OpenCopy(string levelId)
        {
            if (!WebLevels.TryGet(levelId, out var l)) { Say($"{levelId} bulunamadı.", true); return; }
            var d = LevelDraft.FromLevel(l);
            d.Id = null;
            d.Version = 1;
            d.Load = 0;                       // the web generator's estimate no longer applies
            Replace(d);
            Say($"{levelId} kopya olarak açıldı. Kaydedince yeni bir tasarım olur; {levelId} değişmez.");
        }

        public void OpenDesign(string id)
        {
            var d = Store.Find(id);
            if (d == null) { Say($"{id} bulunamadı.", true); return; }
            Replace(d.Clone());
            Say($"{id} v{d.Version} açıldı.");
        }

        void Replace(LevelDraft d)
        {
            Draft = d;
            _undo.Clear();
            _redo.Clear();
            _savedJson = Json(d);
            Selected = null;
            ChainStart = null;
            Changed();
        }

        // ---- edits -----------------------------------------------------------------------

        /* Applies one edit as one undo step (or as part of the open stroke).
           A refused or no-op edit leaves no step behind. */
        public bool Edit(Func<LevelDraft, EditResult> op, string done = null)
        {
            string before = Json(Draft);
            var backup = Draft.Clone();
            var r = op(Draft);
            if (!r.Ok)
            {
                Draft = backup;
                Say(r.Reason, true);
                return false;
            }
            if (Json(Draft) == before) return true;
            if (!(_inStroke && _strokeHasStep))
            {
                _undo.Add(backup);
                if (_undo.Count > UndoLimit) _undo.RemoveAt(0);
                _strokeHasStep = _inStroke;
            }
            _redo.Clear();
            if (done != null) Say(done);
            Changed();
            return true;
        }

        bool _strokeHasStep;

        /* Dragging a brush over several cells is one undo step. */
        public void BeginStroke() { _inStroke = true; _strokeHasStep = false; }
        public void EndStroke() { _inStroke = false; _strokeHasStep = false; }

        public void Undo()
        {
            if (_undo.Count == 0) { Say("Geri alınacak adım yok."); return; }
            _redo.Add(Draft);
            Draft = _undo[_undo.Count - 1];
            _undo.RemoveAt(_undo.Count - 1);
            Say($"Geri alındı ({_undo.Count} adım kaldı).");
            Changed();
        }

        public void Redo()
        {
            if (_redo.Count == 0) { Say("Yinelenecek adım yok."); return; }
            _undo.Add(Draft);
            Draft = _redo[_redo.Count - 1];
            _redo.RemoveAt(_redo.Count - 1);
            Say("Yinelendi.");
            Changed();
        }

        void Changed()
        {
            if (Selected is Cell s && !Draft.OnBoard(s)) Selected = null;
            if (ChainStart is Cell c && !Draft.OnBoard(c)) ChainStart = null;
            Check();
            Revision++;
        }

        /* Live check after every edit: quick budget, stays under a second. */
        public void Check()
        {
            var sw = Stopwatch.StartNew();
            Report = DesignCheck.Run(Draft.ToLevelData(), DesignCheck.QuickBudget);
            CheckMs = sw.Elapsed.TotalMilliseconds;
        }

        /* Full search budget, for an Unverified verdict; may take seconds. */
        public void DeepCheck()
        {
            var sw = Stopwatch.StartNew();
            Report = DesignCheck.Run(Draft.ToLevelData());
            CheckMs = sw.Elapsed.TotalMilliseconds;
            Say($"Derin kontrol {CheckMs / 1000:0.0} sn sürdü.");
            Revision++;
        }

        public void Say(string message, bool error = false)
        {
            Status = message ?? "";
            StatusIsError = error;
        }

        // ---- tool actions ------------------------------------------------------------------

        /* A left (or right = erase) click on a cell with the current tool. */
        public void ClickCell(Cell c, bool right = false)
        {
            if (!Draft.OnBoard(c)) return;
            if (right)
            {
                if (Tool == Tool.Chain && Draft.ChainAt(c) >= 0) { Edit(d => d.RemoveChain(c), "Zincir ayrıldı."); return; }
                Edit(d => d.ClearCell(c) ? EditResult.Done : EditResult.No("Bu hücrede silinecek engel yok."), "Engel kaldırıldı.");
                return;
            }
            switch (Tool)
            {
                case Tool.Select:
                    Selected = c;
                    Revision++;
                    break;
                case Tool.Template:
                    var t = PieceTemplate.All[Template];
                    Edit(d => d.ApplyTemplate(c, t) > 0 ? EditResult.Done : EditResult.No("Bu hücrenin iç kenarı yok."), $"{t.Name} yerleştirildi.");
                    Selected = c;
                    break;
                case Tool.Nail:
                    Edit(d => d.SetNail(c, NailCount), $"Çivi ({NailCount}) kondu.");
                    Selected = c;
                    break;
                case Tool.Bomb:
                    Edit(d => d.SetBomb(c, BombFuse), $"Bomba (fitil {BombFuse}) kondu.");
                    Selected = c;
                    break;
                case Tool.Chain:
                    if (ChainStart is Cell a)
                    {
                        ChainStart = null;
                        if (!a.Equals(c)) Edit(d => d.AddChain(a, c), "Zincir kuruldu.");
                        else { Say("Zincir iptal edildi."); Revision++; }
                    }
                    else if (Draft.ChainAt(c) >= 0) { Say("Bu hücre zaten zincirli. Ayırmak için sağ tıkla.", true); }
                    else { ChainStart = c; Say("Şimdi bağlanacak ikinci hücreye tıkla."); Revision++; }
                    break;
                case Tool.Paint:
                    Edit(d => d.SetColor(c, Color));
                    break;
                case Tool.Erase:
                    Edit(d => d.ClearCell(c) ? EditResult.Done : EditResult.No("Bu hücrede silinecek engel yok."), "Engel kaldırıldı.");
                    break;
            }
        }

        public void ClickJoint(int x, int y, bool vertical, bool right = false)
        {
            if (right) Edit(d => d.SetJoint(x, y, vertical, 0), "Kenar düzlendi.");
            else Edit(d => d.CycleJoint(x, y, vertical), "Kenar değişti.");
        }

        public void ClickLane(Dir side, int lane, bool right = false)
        {
            if (right) Edit(d => d.SetSealed(side, lane, false), "Duvar kaldırıldı.");
            else Edit(d => d.ToggleSealed(side, lane), Draft.IsSealed(side, lane) ? "Duvar kaldırıldı." : "Duvar kondu.");
        }

        // ---- save / play ---------------------------------------------------------------------

        /* Writes designs.json. A design that breaks the rule (0 bombs, 0 freezes,
           0 valve) is saved too, after a warning: experiments may need one. */
        public LevelDraft Save()
        {
            if (!Report.FollowsRule && !Unattended &&
                !EditorUtility.DisplayDialog("Kural dışı tasarım",
                    Report.Message + "\n\nTasarım kuralı: 0 patlama, 0 donma, 0 supapla çözülebilmeli. Yine de kaydedilsin mi?",
                    "Yine de kaydet", "Vazgeç"))
            {
                Say("Kaydedilmedi.");
                return null;
            }
            bool solved = Report.Verdict == DesignVerdict.Solvable || Report.Verdict == DesignVerdict.NeedsValve;
            Draft.Solution = solved ? new List<Move>(Report.Solution) : new List<Move>();
            var saved = Store.Save(Draft);
            File.WriteAllText(DesignsPath, Store.ToJson());
            AssetDatabase.ImportAsset(DesignsPath);
            _savedJson = Json(Draft);
            Say($"{saved.Id} v{saved.Version} kaydedildi" + (Report.FollowsRule ? "." : " (kural dışı!)."), !Report.FollowsRule);
            Revision++;
            return saved;
        }

        public bool Delete()
        {
            if (Draft.Id == null || Store.Find(Draft.Id) == null) { Say("Bu tasarım henüz kaydedilmedi.", true); return false; }
            string id = Draft.Id;
            Store.Delete(id);
            File.WriteAllText(DesignsPath, Store.ToJson());
            AssetDatabase.ImportAsset(DesignsPath);
            NewBoard(Draft.Width, Draft.Height);
            Say($"{id} silindi. Bu id bir daha verilmez.");
            return true;
        }

        public const string CsvPath = "Builds/seviyeler.csv";

        /* The feature table of every web level and saved design (Faz 4 input). */
        public string ExportCsv()
        {
            var rows = new List<(string, LevelData)>();
            foreach (var l in WebLevels.Levels) rows.Add(("web", l));
            foreach (var d in Store.Designs) rows.Add(("tasarim", d.ToLevelData()));
            Directory.CreateDirectory(Path.GetDirectoryName(CsvPath));
            File.WriteAllBytes(CsvPath, LevelCsv.ToBytes(LevelCsv.Build(rows)));
            Say($"CSV yazıldı: {Path.GetFullPath(CsvPath)} ({rows.Count} seviye).");
            return Path.GetFullPath(CsvPath);
        }

        /* The current draft, saved or not, as a one-level set for the game. */
        public LevelSet PlaySet()
        {
            var w = new JsonWriter();
            Draft.WriteJson(w);
            return LevelParser.Parse("{\"format\":1,\"levels\":[" + w + "]}");
        }

        /* Undo history is about edits; leaving play mode keeps it. */
        public bool AskSaveOnExit()
        {
            if (!Dirty || Unattended) return false;
            return EditorUtility.DisplayDialog("Seviye Editörü",
                "Kaydedilmemiş değişiklikler var. Kaydedilsin mi?", "Kaydet", "Kaydetmeden çık");
        }
    }
}
