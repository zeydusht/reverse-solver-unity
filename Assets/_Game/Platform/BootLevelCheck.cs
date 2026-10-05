using System;
using System.Diagnostics;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Platform
{
    /* TEMPORARY (M1): proves the level file parses in the shipped web build,
       where managed stripping is High and EditMode tests cannot reach. Shows
       the result on the load-time panel. Remove once the game itself loads
       levels at startup (M3). */
    public sealed class BootLevelCheck : MonoBehaviour
    {
        [SerializeField] TextAsset levels;

        void Start()
        {
            if (levels == null)
            {
                WebBridge.BootReport("Seviye dosyası bağlı değil");
                return;
            }
            var sw = Stopwatch.StartNew();
            try
            {
                var set = LevelParser.Parse(levels.text);
                int solved = 0;
                foreach (var l in set.Levels)
                    if (GreedySolver.Solve(l).Solved) solved++;
                sw.Stop();
                WebBridge.BootReport($"{set.Levels.Count} seviye okundu, {solved} çözülebilir ({sw.ElapsedMilliseconds} ms)");
            }
            catch (Exception e)
            {
                WebBridge.BootReport("Seviye dosyası okunamadı: " + e.Message);
            }
        }
    }
}
