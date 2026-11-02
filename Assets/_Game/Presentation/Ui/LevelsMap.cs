using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LevelsMap
    {
        public readonly MenuScroll Scroll;
        readonly Rect _back;
        readonly List<(Rect rect, int index)> _nodes = new List<(Rect, int)>();
        public LevelsMap(Transform root, Vector2 screen, float top, float bottom, IReadOnlyList<LevelData> levels, Progress progress, LevelImages images, int order, float? offset)
        {
            MenuUi.Scene(root, "levels", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Levels, order + 30);
            _back = MenuUi.Button(root, "button-back", null, new Vector2(30, top + 34), Vector2.one * 38, order + 32);
            _back = new Rect(_back.center - Vector2.one * 22, Vector2.one * 44);
            const float pitch = 76;
            float start = top + 90, height = levels.Count * pitch + 120;
            Scroll = new MenuScroll(root, start, bottom, height);
            MenuUi.Viewport(root, "levels", screen, start, bottom, order + 20);
            // Each repeat is 256 x 512 pt, centred on the same vertical path.
            var pathRoot = Scroll.Root;
            System.Action<Sprite> showPath = sprite =>
            {
                if (pathRoot == null) return;
                for (float y = start + 256; y < start + height + 256; y += 512)
                {
                    var path = new GameObject("levels-path-repeat").AddComponent<SpriteRenderer>();
                    path.transform.SetParent(pathRoot, false);
                    path.transform.localPosition = Draw.W(screen.x / 2, y + 256);
                    path.sprite = sprite; path.sortingOrder = order;
                    path.drawMode = SpriteDrawMode.Tiled; path.tileMode = SpriteTileMode.Continuous;
                    path.size = new Vector2(256, 512); path.transform.localScale = Vector3.one;
                }
            };
            var pathSprite = MenuArt.Instance.Get("t20-levels-path-repeat.png", showPath);
            if (pathSprite != null) showPath(pathSprite);
            for (int i = 0; i < levels.Count; i++)
            {
                bool current = i == progress.Next(), open = progress.Open(i), won = progress.Won(i), hard = levels[i].Hard;
                float x = screen.x / 2 + (i % 2 == 0 ? -64 : 64), y = start + 90 + (levels.Count - 1 - i) * pitch;
                var cell = Scroll.Cell(x, y, current ? 108 : 44);
                string key = hard ? (open ? "node-hard-open" : "node-hard-locked") : current ? "node-current" : won ? "node-complete" : "node-locked";
                if (current) MenuUi.Sprite(cell, "node-glow", Vector2.zero, Vector2.one * 94, order + 1);
                MenuUi.Sprite(cell, key, Vector2.zero, Vector2.one * (current ? 66 : 54), order + 2);
                if (current)
                {
                    var avatar = Art.Avatar(cell, Art.AvatarIndex, Vector2.one * 38, order + 4);
                    if (avatar != null) avatar.transform.localPosition = Draw.W(0, -53);
                    var bubbleAt = new Vector2(0, -88);
                    ShapeView.Create(cell, "PlayBubble", new Vector2(70, 36), 2, order + 6).Radius(14)
                        .Fill(Draw.Hex("#3e9a51")).transform.localPosition = Draw.W(bubbleAt);
                    MenuUi.Label(cell, Texts.Play, bubbleAt, 14, order + 7);
                    _nodes.Add((new Rect(x - 35, y - 110, 70, 44), i));
                }
                if (hard && open) MenuUi.Sprite(cell, "baron-head", new Vector2(25, -20), Vector2.one * 30, order + 4);
                MenuUi.Label(cell, (i + 1).ToString(), new Vector2(0, open || won ? 0 : 25), 20, order + 5, current);
                if ((i + 1) % 10 == 0)
                {
                    var at = new Vector2(i % 2 == 0 ? 66 : -66, 0);
                    if (won) MenuUi.Painting(cell, images, i, at, new Vector2(30, 40), order + 1);
                    MenuUi.Sprite(cell, "milestone-picture-frame", at, new Vector2(42, 56), order + 3);
                }
                if (open || won) _nodes.Add((new Rect(x - 33, y - 33, 66, 66), i));
            }
            float currentY = start + 90 + (levels.Count - 1 - progress.Next()) * pitch;
            Scroll.Set(offset ?? currentY - (start + (bottom - start) * .7f));
        }
        public int Hit(Vector2 pt)
        {
            if (_back.Contains(pt)) return -2;
            if (!Scroll.Contains(pt)) return -1;
            foreach (var node in _nodes)
                if (node.rect.Contains(pt + new Vector2(0, Scroll.Offset))) return node.index;
            return -1;
        }
    }
}
