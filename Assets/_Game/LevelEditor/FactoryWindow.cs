using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReverseSolver.LevelEditor
{
    /* Reverse Solver → Seviye fabrikası, two tabs:
         Fabrika  G6: run one slot or all 40 (progress bar; cancel stops after the
                  current slot), then the report.
         Onay     G7: Zeyd's approval queue. Slots in order; every finalist on a
                  card (numbers, curve, the designer's idea, the psychologist's
                  note) with Oyna ▶ (the real game in Play mode), Onayla (R id,
                  never reused) and Reddet + not (the note goes to the designer).
       The batchmode twin of the Fabrika tab is FactoryBatch.Run. */
    public sealed class FactoryWindow : EditorWindow
    {
        static readonly string[] Tabs = { "Fabrika", "Onay" };
        static readonly string[] Presets = { "Hızlı (deneme)", "Normal", "Derin (gece)" };

        int _tab;
        string _slots = "1-40";
        int _preset = 1;
        string _log = "";
        Vector2 _scroll, _queueScroll;
        RedesignStore _store;
        bool _onlyOpen = true;
        readonly Dictionary<string, string> _notes = new Dictionary<string, string>();
        readonly Dictionary<string, string> _curves = new Dictionary<string, string>();
        GUIStyle _card, _mono;

        [MenuItem("Reverse Solver/Seviye fabrikası", priority = 20)]
        static void OpenFactory() => GetWindow<FactoryWindow>("Seviye fabrikası")._tab = 0;

        [MenuItem("Reverse Solver/Onay kuyruğu", priority = 21)]
        public static void OpenQueue()
        {
            var w = GetWindow<FactoryWindow>("Seviye fabrikası");
            w._tab = 1;
            w._store = null;
        }

        public static FactoryOptions OptionsFor(int preset) => preset switch
        {
            0 => new FactoryOptions { BudgetEasy = 40, BudgetOrta = 80, BudgetZor = 120, BudgetCokZor = 120, BandWant = 4, Players = 200, MemoryTable = false },
            2 => new FactoryOptions { BudgetEasy = 300, BudgetOrta = 1200, BudgetZor = 2500, BudgetCokZor = 2500, BandWant = 16 },
            _ => new FactoryOptions(),
        };

        void OnGUI()
        {
            _card ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(8, 8, 6, 6) };
            _mono ??= new GUIStyle(EditorStyles.label) { font = EditorStyles.miniFont, richText = true, wordWrap = true };
            _tab = GUILayout.Toolbar(_tab, Tabs);
            if (_tab == 0) FactoryTab(); else QueueTab();
        }

        // ---- Fabrika -----------------------------------------------------------------------------

        void FactoryTab()
        {
            EditorGUILayout.HelpBox("Fabrika adayları üretir, bant ve haksız zorluk elemesinden geçirir, 1000 sanal oyuncuyla keyif tahmini yapar ve " +
                                    "slot başına en iyi 2–3 finalisti redesign.json'a yazar. Onaylı seviyelere dokunmaz. Canlıya hiçbir şey çıkmaz.", MessageType.Info);
            _slots = EditorGUILayout.TextField("Slotlar (1-40, 12, 3,5,20-22)", _slots);
            _preset = EditorGUILayout.Popup("Derinlik", _preset, Presets);
            if (GUILayout.Button("Çalıştır")) RunNow();
            if (GUILayout.Button("Raporu aç")) EditorUtility.OpenWithDefaultApp(FactoryReport.Path);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.TextArea(_log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        void RunNow()
        {
            var slots = FactoryRunner.ParseSlots(_slots);
            string run = DateTime.Now.ToString("yyyyMMdd-HHmm");
            _log = $"koşu {run}: {slots.Count} slot\n";
            try
            {
                FactoryRunner.Run(slots, OptionsFor(_preset), run, (done, total, line) =>
                {
                    _log += line + "\n";
                    return !EditorUtility.DisplayCancelableProgressBar("Seviye fabrikası", $"{done}/{total} · {line}", (float)done / total);
                });
            }
            finally { EditorUtility.ClearProgressBar(); }
            AssetDatabase.Refresh();
            _store = null;
            _log += "rapor: " + FactoryReport.Path + "\n";
        }

        // ---- Onay (G7) --------------------------------------------------------------------------------

        void QueueTab()
        {
            _store ??= RedesignStore.LoadOrNew();
            var plan = FactoryPlan.All();
            int approved = _store.Entries.Count(e => e.Status == RedesignStore.Approved);
            int ready = plan.Count(s => _store.ForSlot(s.Slot).Any(e => e.Status == RedesignStore.Finalist || e.Status == RedesignStore.Approved));
            EditorGUILayout.HelpBox($"{ready}/40 slotta aday var · {approved} seviye onaylı. Her slotta finalistleri Oyna ▶ ile oyna; beğendiğini Onayla, " +
                                    "beğenmediğini kısa bir notla Reddet. Onay canlıya çıkarmaz: yeni set yalnızca senin \"yeni seti canlıya al\" komutunla varsayılan olur. " +
                                    "Sayılar modelin keyif tahminidir, gerçek oyuncunun yerini tutmaz.", MessageType.Info);
            GUILayout.BeginHorizontal();
            _onlyOpen = GUILayout.Toggle(_onlyOpen, "Yalnızca onay bekleyen slotlar");
            if (GUILayout.Button("Yenile", GUILayout.Width(80))) { _store = null; return; }
            GUILayout.EndHorizontal();

            _queueScroll = EditorGUILayout.BeginScrollView(_queueScroll);
            foreach (var spec in plan)
            {
                var entries = _store.ForSlot(spec.Slot).ToList();
                bool done = entries.Any(e => e.Status == RedesignStore.Approved);
                if (_onlyOpen && (done || entries.All(e => e.Status == RedesignStore.Rejected))) continue;
                GUILayout.Label($"Slot {spec.Slot} — {spec.Genre.Label()}{(spec.NewThing != null ? ", yeni: " + SlotSpec.Label(spec.NewThing) : "")}" +
                                (done ? "  ✔ onaylı" : ""), EditorStyles.boldLabel);
                if (entries.Count == 0) { GUILayout.Label("Henüz finalist yok.", EditorStyles.miniLabel); continue; }
                foreach (var e in entries) Card(e, spec);
                GUILayout.Space(6);
            }
            EditorGUILayout.EndScrollView();
        }

        static string P(RedesignStore.Entry e, string k) => e.Scores.TryGetValue(k, out var v) ? "%" + Math.Round(v * 100) : "—";
        static string N(RedesignStore.Entry e, string k, string f = "0.##") => e.Scores.TryGetValue(k, out var v) ? v.ToString(f) : "—";

        void Card(RedesignStore.Entry e, SlotSpec spec)
        {
            GUILayout.BeginVertical(_card);
            string status = e.Status == RedesignStore.Approved ? "<color=#4caf50>onaylı</color>" : e.Status == RedesignStore.Rejected ? "<color=#e57373>reddedildi</color>" : "finalist";
            GUILayout.Label($"<b>{e.Draft.Id}</b> · {status} · {e.Draft.Width}×{e.Draft.Height}, {N(e, "govde", "0")} gövde, süre {N(e, "sure", "0")} sn", _mono);
            GUILayout.Label($"Keyif tahmini {P(e, "keyif")} · hayal kırıklığı {P(e, "hayalKirikligi")} · sıkıcı {P(e, "sikici")} · " +
                            $"ilk denemede {P(e, "ilkDeneme")} · 3 denemede {P(e, "ucDeneme")} · near-miss {P(e, "nearMiss")} · medyan yıldız {N(e, "medyanYildiz", "0")}", _mono);
            GUILayout.Label($"Okuma {N(e, "okuma", "0.00")} · ilk hamlede serbest {N(e, "k0", "0")} · açık dikiş %{N(e, "acikDikis", "0")} · darboğaz {N(e, "darbogaz", "0")}" +
                            (e.Scores.ContainsKey("uyum") ? $" · eğri uyumu {N(e, "uyum", "0.00")}, zirve {P(e, "zirve")}" : ""), _mono);
            string curve = CurveOf(e, spec);
            if (curve != null) GUILayout.Label($"Eğri (ilerleme %0 → %100): <b>{curve}</b>", _mono);
            if (!string.IsNullOrEmpty(e.Aha)) GUILayout.Label($"<b>Fikir:</b> {e.Aha}", _mono);
            if (!string.IsNullOrEmpty(e.Psych)) GUILayout.Label($"<b>Psikolog:</b> {e.Psych}", _mono);
            foreach (var n in e.Notes) GUILayout.Label("Not: " + n, _mono);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Oyna ▶", GUILayout.Width(90))) ApprovalPlay.Play(e.Draft);
            GUI.enabled = e.Status == RedesignStore.Finalist && !EditorApplication.isPlaying;
            if (GUILayout.Button("Onayla", GUILayout.Width(90)))
            {
                string id = _store.Approve(e, Note(e));
                _store.Save();
                AssetDatabase.Refresh();
                ShowNotification(new GUIContent($"{id} onaylandı"));
            }
            _notes[e.Candidate] = GUILayout.TextField(Note(e) ?? "", GUILayout.MinWidth(160));
            if (GUILayout.Button("Reddet + not", GUILayout.Width(110)))
            {
                if (string.IsNullOrWhiteSpace(Note(e))) ShowNotification(new GUIContent("Önce kısa bir not yaz"));
                else { _store.Reject(e, Note(e)); _store.Save(); AssetDatabase.Refresh(); }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        string Note(RedesignStore.Entry e) => _notes.TryGetValue(e.Candidate, out var n) ? n : null;

        string CurveOf(RedesignStore.Entry e, SlotSpec spec)
        {
            if (e.Curve != null) return e.Curve;
            if (!Calibration.CurveMatters(spec.Genre)) return null;
            if (!_curves.TryGetValue(e.Candidate, out var c))
            {
                var lc = Curve.Of(e.Draft.ToLevelData(), Calibration.CurveRuns);
                _curves[e.Candidate] = c = lc.Defined ? lc.Spark() : "eğri yok";
            }
            return c;
        }
    }

    /* Oyna ▶ for a finalist: the real game scene in Play mode with just that level
       (debug in the editor: nothing is sent). The game's menu button or Esc ends Play mode. */
    [InitializeOnLoad]
    public static class ApprovalPlay
    {
        const string Flag = "rs.approval.level";

        static ApprovalPlay()
        {
            EditorApplication.playModeStateChanged += c => { if (c == PlayModeStateChange.ExitingPlayMode && SessionState.GetString(Flag, "") != "") { SessionState.EraseString(Flag); PlayOverride.Clear(); } };
        }

        public static void Play(LevelDraft d)
        {
            var w = new JsonWriter();
            d.WriteJson(w);
            string json = "{\"format\":1,\"levels\":[" + w + "]}";
            if (EditorApplication.isPlaying) { Arm(json); SceneManager.LoadScene(0); return; }
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            SessionState.SetString(Flag, json);
            EditorApplication.EnterPlaymode();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnPlayModeStart()
        {
            string json = SessionState.GetString(Flag, "");
            if (json != "") Arm(json);
        }

        static void Arm(string json)
        {
            SessionState.SetString(Flag, json);
            PlayOverride.Set = LevelParser.Parse(json);
            PlayOverride.Index = 0;
            PlayOverride.Exit = () => EditorApplication.ExitPlaymode();
        }
    }
}
