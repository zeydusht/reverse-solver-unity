using System;
using System.Diagnostics;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using UnityEngine;

namespace ReverseSolver.Platform
{
    /* Debug check (?debug=1): proves, in the shipped web build where managed
       stripping is High and EditMode tests cannot reach, that
         - the level file parses and every level is solvable (M1), and
         - GameSession plays the web game's reference scenarios with the same
           result on every level (M2; Tools/make_session_golden.js).
       Shows the result on the load-time panel. */
    public sealed class BootLevelCheck : MonoBehaviour
    {
        [SerializeField] TextAsset levels;
        [SerializeField] TextAsset sessionCheck;

        void Start()
        {
            if (!ReverseSolver.Presentation.Host.Debug) return;   // ?debug=1 only: it costs ~0.3 s at startup
            if (levels == null)
            {
                WebBridge.BootReport("Seviye dosyası bağlı değil");
                return;
            }
            var sw = Stopwatch.StartNew();
            string msg;
            try
            {
                var set = LevelParser.Parse(levels.text);
                int solvable = 0;
                foreach (var l in set.Levels)
                    if (GreedySolver.Solve(l).Solved) solvable++;
                msg = $"{set.Levels.Count} seviye okundu, {solvable} çözülebilir";
                if (sessionCheck != null) msg += "; " + CheckSessions(set);
            }
            catch (Exception e)
            {
                msg = "Seviye kontrolü hata verdi: " + e.Message;
            }
            sw.Stop();
            WebBridge.BootReport($"{msg} ({sw.ElapsedMilliseconds} ms)");
        }

        string CheckSessions(LevelSet set)
        {
            var root = JsonReader.Parse(sessionCheck.text);
            root.TryGet("seed", out var seedNode);
            root.TryGet("levels", out var list);
            int same = 0, won = 0, bombs = 0;
            string firstDiff = null;
            foreach (var sc in list.Items)
            {
                sc.TryGet("id", out var id);
                sc.TryGet("moves", out var moves);
                sc.TryGet("res", out var res);
                sc.TryGet("n", out var n);
                sc.TryGet("stars", out var stars);

                var s = new GameSession(set[id.AsString], 1, "boot", new Mulberry32((uint)seedNode.AsNumber));
                bool ok = true;
                foreach (var m in moves.Items)
                {
                    DirExt.TryParse(m[1].AsString, out var dir);
                    if (s.TryExit((int)m[0].AsNumber, dir) != CommandResult.Ok) { ok = false; break; }
                }
                ok &= s.Outcome.Id() == res.AsString && s.Moves == (int)n.AsNumber && s.Stars == (int)stars.AsNumber;
                if (ok) same++;
                else firstDiff ??= id.AsString;
                if (s.Outcome == Outcome.Win) won++;
                if (s.Outcome == Outcome.Bomb) bombs++;
            }
            string result = $"oturum {same}/{list.Count} web'le aynı ({won} kazandı, {bombs} bomba)";
            return firstDiff == null ? result : result + $", ilk fark {firstDiff}";
        }
    }
}
