using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Generation;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* G6: Reverse Solver → Seviye fabrikası. Runs one slot or all 40 with a
       progress bar (cancel stops after the current slot), then shows the
       report. The batchmode twin is FactoryBatch.Run. */
    public sealed class FactoryWindow : EditorWindow
    {
        string _slots = "1-40";
        int _preset = 1;
        string _log = "";
        Vector2 _scroll;

        static readonly string[] Presets = { "Hızlı (deneme)", "Normal", "Derin (gece)" };

        [MenuItem("Reverse Solver/Seviye fabrikası", priority = 20)]
        static void Open() => GetWindow<FactoryWindow>("Seviye fabrikası");

        public static FactoryOptions OptionsFor(int preset) => preset switch
        {
            0 => new FactoryOptions { BudgetEasy = 40, BudgetOrta = 80, BudgetZor = 120, BudgetCokZor = 120, BandWant = 4, Players = 200 },
            2 => new FactoryOptions { BudgetEasy = 300, BudgetOrta = 1200, BudgetZor = 2500, BudgetCokZor = 2500, BandWant = 16 },
            _ => new FactoryOptions(),
        };

        void OnGUI()
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
            _log += "rapor: " + FactoryReport.Path + "\n";
        }
    }

    /* Batchmode: Unity.exe -batchmode -quit -projectPath P -executeMethod
       ReverseSolver.LevelEditor.FactoryBatch.Run -rsSlots 1-40 -rsDepth 1 [-rsRun name] */
    public static class FactoryBatch
    {
        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            string Arg(string k, string d) { int i = Array.IndexOf(args, k); return i >= 0 && i + 1 < args.Length ? args[i + 1] : d; }
            var slots = FactoryRunner.ParseSlots(Arg("-rsSlots", "1-40"));
            int depth = int.Parse(Arg("-rsDepth", "1"));
            string run = Arg("-rsRun", DateTime.Now.ToString("yyyyMMdd-HHmm"));
            Debug.Log($"[fabrika] koşu {run}: slotlar {string.Join(",", slots)}, derinlik {depth}");
            FactoryRunner.Run(slots, FactoryWindow.OptionsFor(depth), run, (done, total, line) => { Debug.Log($"[fabrika] {done}/{total} {line}"); return true; });
            Debug.Log("[fabrika] bitti, rapor " + FactoryReport.Path);
        }
    }
}
