using System;
using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    [Serializable]
    public sealed class TutorialStep
    {
        public int slot, adim, parca;
        public string tur, engel, metinAnahtari;
    }

    [Serializable]
    public sealed class TutorialScript
    {
        public TutorialStep[] steps;

        public static TutorialScript Parse(string json) => JsonUtility.FromJson<TutorialScript>(json);
        public static TutorialScript Load()
        {
            var asset = Resources.Load<TextAsset>("tutorial");
            return asset == null ? new TutorialScript() : Parse(asset.text);
        }

        public List<TutorialStep> ForSlot(int slot, bool firstVisit)
        {
            var result = new List<TutorialStep>();
            foreach (var step in steps ?? Array.Empty<TutorialStep>())
                if (step.slot == slot && (step.tur != "goster" || firstVisit)) result.Add(step);
            result.Sort((a, b) => a.adim.CompareTo(b.adim));
            return result;
        }

        // Direction has a single source: the authored solution, never the script.
        public static bool MoveOf(LevelData level, TutorialStep step, out Move move)
        {
            foreach (var candidate in level.Solution)
                if (candidate.Piece == step.parca) { move = candidate; return true; }
            move = default;
            return false;
        }

        public static bool WrongDirection(Dir required, Dir? locked, float dx, float dy, out Dir attempted)
        {
            attempted = required;
            if (!locked.HasValue && dx * dx + dy * dy < DragModel.MinSwipe * DragModel.MinSwipe) return false;
            bool horizontal = locked.HasValue ? !locked.Value.IsVertical() : Math.Abs(dx) > Math.Abs(dy);
            attempted = horizontal ? (dx < 0 ? Dir.L : Dir.R) : (dy < 0 ? Dir.U : Dir.D);
            return attempted != required;
        }

        // Stable first cell in board order. Walls are represented by their boundary cell + side.
        public static bool ObstacleOf(LevelData level, string kind, out Cell cell, out Dir? side)
        {
            side = null;
            for (int y = 0; y < level.Height; y++)
                for (int x = 0; x < level.Width; x++)
                {
                    cell = new Cell(x, y);
                    if (kind == "wall")
                    {
                        foreach (var d in DirExt.All)
                        {
                            bool edge = d == Dir.U ? y == 0 : d == Dir.D ? y == level.Height - 1 : d == Dir.L ? x == 0 : x == level.Width - 1;
                            if (edge && level.Sealed.TryGetValue(d, out var lanes) && Array.IndexOf(lanes, d.IsVertical() ? x : y) >= 0)
                            { side = d; return true; }
                        }
                    }
                    else
                        for (int p = 0; p < level.Pieces.Count; p++)
                        {
                            bool matches = kind == "nail" ? level.Nails.ContainsKey(p) : kind == "bomb" ? level.Bombs.ContainsKey(p) : kind == "chain" && level.Pieces[p].Length > 1;
                            if (matches && Array.IndexOf(level.Pieces[p], cell) >= 0) return true;
                        }
                }
            cell = default;
            return false;
        }
    }
}
