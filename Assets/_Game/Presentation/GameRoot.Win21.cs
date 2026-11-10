using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed partial class GameRoot
    {
        bool P21WinArt => Art.T54 && TelemetryMeta.Current?.Cohort == "p21";
        P21WinView _win21;
        bool _winPreview21;
        Rect[] WinTargets21()
        {
            var targets = new Rect[4];
            foreach (var b in BoosterExt.All) targets[(int)b] = _bar.RectOf(b);
            return targets;
        }
        void ShowWin21(bool chest = false, bool preview = false)
        {
            if (_win21 != null) return;
            CancelCardTap(); CloseCard(); _introOpen = false; _endAt = -1;
            _winPreview21 = preview;
            var save = _save;
            if (preview)
            {
                save = new SaveData(new MemoryStore(), () => "chest-preview");
                save.RecordChestWin21(chest ? 3 : _session.Level.Number);
            }
            else _save.RecordChestWin21(_session.Level.Number);
            _win21 = new P21WinView(transform, _screen, WinTargets21(), save, _session.Level.Pieces.Count,
                chest, NextWin21, () =>
                {
                    if (preview) return;
                    _inventory = new BoosterInventory(Host.Current.Store, _session.Level, i => i >= 0 && i < _set.Levels.Count && _progress.Won(i));
                    _inventory.Apply(_session); RefreshPlaying();
                });
        }
        void NextWin21()
        {
            bool preview = _winPreview21;
            _win21?.Destroy(); _win21 = null; _winPreview21 = false;
            if (preview) { Layout(); return; }
            if (_index + 1 < _set.Levels.Count) StartLevel(_index + 1); else ShowMenu();
        }
        void TestWin21()
        {
            if (!P21WinArt) return;
            if (Host.Current.Query("sandik") == "1") ShowWin21(true, true);
            else if (Host.Current.Query("kazan") == "1") ShowWin21(false, true);
        }
    }
}
