using System;
using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class CollectionScreen
    {
        public readonly MenuScroll Scroll;
        readonly List<(Rect rect, int index)> _cells = new List<(Rect, int)>();
        public CollectionScreen(Transform root, Vector2 screen, float top, float bottom, IReadOnlyList<LevelData> levels, Progress progress, LevelImages images, int order, float offset)
        {
            MenuUi.Scene(root, "collection", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Tab(1), order + 30);
            int won = 0;
            for (int i = 0; i < levels.Count; i++) if (progress.Won(i)) won++;
            MenuUi.Sprite(root, "counter-pill", new Vector2(screen.x / 2, top + 88), new Vector2(96, 36), order + 30, true);
            MenuUi.Label(root, $"{won} / {levels.Count}", new Vector2(screen.x / 2, top + 88), 19, order + 31, true);
            float start = top + 120, pitch = 142;
            Scroll = new MenuScroll(root, start, bottom, Mathf.CeilToInt(levels.Count / 3f) * pitch);
            MenuUi.Viewport(root, "collection", screen, start, bottom, order + 20);
            float gap = Mathf.Min(112, (screen.x - 24) / 3);
            for (int i = 0; i < levels.Count; i++)
            {
                float x = screen.x / 2 + (i % 3 - 1) * gap, y = start + 61 + i / 3 * pitch;
                int index = i;
                var cell = Scroll.Cell(x, y, 67, progress.Won(i)
                    ? t => MenuUi.Painting(t, images, index, new Vector2(0, -5), new Vector2(69, 92), order + 1)
                    : (Action<Transform>)null);
                MenuUi.Sprite(cell, "portrait-placeholder", new Vector2(0, -5), new Vector2(81, 108), order);
                if (!progress.Won(i) && i != progress.Next()) MenuUi.Sprite(cell, "icon-lock", new Vector2(0, -5), Vector2.one * 28, order + 2);
                if (!progress.Won(i) && i == progress.Next())
                    ShapeView.Create(cell, "next-highlight", new Vector2(85, 112), 3, order + 2).Radius(8)
                        .Fill(Vector4.zero).Stroke(Draw.Hex("#ffe47e"), 2).transform.localPosition = Draw.W(0, -5);
                var frame = MenuUi.Sprite(cell, "portrait-frame", new Vector2(0, -5), new Vector2(81, 108), order + 3);
                if (!progress.Won(i) && i == progress.Next() && frame != null) frame.color = new Color(1, 1, .6f);
                if (levels[i].Hard) MenuUi.Sprite(cell, "hard-corner-flag", new Vector2(27, -39), new Vector2(27, 30), order + 4);
                MenuUi.Label(cell, !progress.Won(i) && i == progress.Next() ? Texts.CollectionNext : string.Format(Texts.CollectionLevel, i + 1), new Vector2(0, 59), 14, order + 5, width: gap - 4);
                if (progress.Won(i)) _cells.Add((new Rect(x - 41, y - 59, 82, 126), i));
            }
            Scroll.Set(offset);
        }
        public int Hit(Vector2 pt)
        {
            if (!Scroll.Contains(pt)) return -1;
            foreach (var c in _cells)
                if (c.rect.Contains(pt + new Vector2(0, Scroll.Offset))) return c.index;
            return -1;
        }
    }

    public sealed class PaintingDetail
    {
        [Serializable] sealed class Names { public Name[] paintings; }
        [Serializable] sealed class Name { public int level; public string tr, en; }
        public readonly Transform Root;
        public readonly int Index;
        readonly Rect _close, _replay;
        public PaintingDetail(Transform parent, Vector2 screen, int index, LevelImages images, int order, Func<int, int?> bestSeconds = null)
        {
            Index = index; Root = new GameObject("painting-detail").transform; Root.SetParent(parent, false);
            var c = screen / 2;
            ShapeView.Create(Root, "veil", screen, 0, order).Fill(Draw.Rgba(0, 0, 0, .65f)).transform.localPosition = Draw.W(c);
            float h = Mathf.Min(560, screen.y - 70), w = Mathf.Min(330, screen.x - 30);
            MenuUi.Sprite(Root, "modal-panel", c, new Vector2(w, h), order + 1, true);
            float top = c.y - h / 2;
            MenuUi.Sprite(Root, "title-plaque", new Vector2(c.x, top + 42), new Vector2(w - 68, 55), order + 3, true);
            MenuUi.Label(Root, Texts.CollectionDetail, new Vector2(c.x, top + 42), 25, order + 4, width: w - 92);
            _close = MenuUi.Button(Root, "button-close", null, new Vector2(c.x + w / 2 - 23, top + 24), Vector2.one * 36, order + 10);
            float ph = Mathf.Min(260, h - 280), py = top + 88 + ph / 2;
            MenuUi.Sprite(Root, "portrait-placeholder", new Vector2(c.x, py), new Vector2(ph * .75f, ph), order + 2);
            MenuUi.Painting(Root, images, index, new Vector2(c.x, py), new Vector2(ph * .7f, ph * .9333f), order + 3);
            MenuUi.Sprite(Root, "portrait-frame-large", new Vector2(c.x, py), new Vector2(ph * .75f, ph), order + 4);
            string title = string.Format(Texts.CollectionLevel, index + 1);
            var asset = Resources.Load<TextAsset>("painting_names");
            if (asset != null)
                foreach (var name in JsonUtility.FromJson<Names>(asset.text).paintings)
                    if (name.level == index + 1) { title = Lang.Tr ? name.tr : name.en; break; }
            MenuUi.Sprite(Root, "name-band", new Vector2(c.x, py + ph / 2 + 43), new Vector2(w - 42, 72), order + 4, true);
            MenuUi.Label(Root, title, new Vector2(c.x, py + ph / 2 + 28), 21, order + 5, true, w - 54);
            int? seconds = bestSeconds?.Invoke(index);
            string earned = string.Format(Texts.CollectionEarned, index + 1);
            if (seconds.HasValue) earned += $" · {seconds.Value / 60}:{seconds.Value % 60:00}";
            MenuUi.Label(Root, earned, new Vector2(c.x, py + ph / 2 + 61), 14, order + 5, true, w - 30);
            _replay = MenuUi.Button(Root, "button-secondary", Texts.CollectionReplay, new Vector2(c.x, top + h - 57), new Vector2(w - 60, 59), order + 6);
        }
        public string Hit(Vector2 pt) => _close.Contains(pt) ? "close" : _replay.Contains(pt) ? "replay" : null;
        public void Destroy() { if (Root != null) UnityEngine.Object.Destroy(Root.gameObject); }
    }
}
