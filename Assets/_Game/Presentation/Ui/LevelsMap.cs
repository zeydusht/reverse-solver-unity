using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LevelsMap
    {
        // MenuUi.Sprite adds the t20- prefix; node art is the Task23 final set (PM_MESAJ_37).
        const string BackSprite = "button-back";
        const string PathSprite = "t20-levels-path-repeat.png";
        const string CurrentNodeSprite = "t23-node-current";
        const string CompleteNodeSprite = "t23-node-complete";
        const string LockedBaseSprite = "t23-node-locked";
        const string HardLockedBaseSprite = "t23-node-hard-locked";
        const string ShadowSprite = "t23-node-contact-shadow";
        const string PlayBubbleSprite = "t23-play-bubble";
        const string HardBadgeSprite = "t23-hard-marker";
        const string LockBadgeSprite = "t23-lock-badge";
        const string WonBadgeSprite = "t23-complete-check";

        public readonly MenuScroll Scroll;
        readonly Rect _back;
        readonly List<(Rect rect, int index)> _nodes = new List<(Rect, int)>();
        public LevelsMap(Transform root, Vector2 screen, float top, float bottom, IReadOnlyList<LevelData> levels, Progress progress, LevelImages images, int order, float? offset)
        {
            MenuUi.Scene(root, "levels", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Levels, order + 30);
            _back = MenuUi.Button(root, BackSprite, null, new Vector2(30, top + 34), Vector2.one * 38, order + 32);
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
            var pathSprite = MenuArt.Instance.Get(PathSprite, showPath);
            if (pathSprite != null) showPath(pathSprite);
            for (int i = 0; i < levels.Count; i++)
            {
                bool current = i == progress.Next(), open = progress.Open(i), won = progress.Won(i), hard = levels[i].Hard;
                float x = screen.x / 2 + (i % 2 == 0 ? -64 : 64), y = start + 90 + (levels.Count - 1 - i) * pitch;
                var cell = Scroll.Cell(x, y, current ? 108 : 44);
                bool locked = !open && !won;
                float diameter = current ? 66 : 54;
                // The node sits on its step: a soft contact shadow under the coin, no outer glow.
                MenuUi.Sprite(cell, ShadowSprite, new Vector2(0, diameter / 2 - 2), new Vector2(diameter * .93f, diameter * .24f), order + 1);
                // A hard level keeps the red rim; when open, its gold or teal coin sits inside the rim.
                string coin = current ? CurrentNodeSprite : won ? CompleteNodeSprite : CurrentNodeSprite;
                if (locked) MenuUi.Sprite(cell, hard ? HardLockedBaseSprite : LockedBaseSprite, Vector2.zero, Vector2.one * diameter, order + 2);
                else if (hard)
                {
                    MenuUi.Sprite(cell, HardLockedBaseSprite, Vector2.zero, Vector2.one * diameter, order + 2);
                    MenuUi.Sprite(cell, coin, Vector2.zero, Vector2.one * diameter * .78f, order + 3);
                }
                else MenuUi.Sprite(cell, coin, Vector2.zero, Vector2.one * diameter, order + 2);
                if (current)
                {
                    var avatar = Art.Avatar(cell, Art.AvatarIndex, Vector2.one * 38, order + 4);
                    // on a hard node the avatar steps left so it clears the Baron badge (top right)
                    if (avatar != null) avatar.transform.localPosition = Draw.W(hard ? -12 : 0, -53);
                    var bubbleAt = new Vector2(0, -94);
                    if (MenuUi.Sprite(cell, PlayBubbleSprite, bubbleAt, new Vector2(84, 44), order + 6) == null)
                        ShapeView.Create(cell, "PlayBubble", new Vector2(84, 44), 2, order + 6).Radius(14)
                            .Fill(Draw.Hex("#3e9a51")).transform.localPosition = Draw.W(bubbleAt);
                    MenuUi.Label(cell, Texts.Play, bubbleAt + new Vector2(0, -2), 16, order + 7);
                    _nodes.Add((new Rect(x - 42, y - 116, 84, 44), i));
                }
                // Baron on top right, lock or tick on bottom right: the two never overlap.
                if (hard) MenuUi.Sprite(cell, HardBadgeSprite, new Vector2(diameter * .36f, -diameter * .4f), new Vector2(25, 29), order + 8);
                MenuUi.Label(cell, (i + 1).ToString(), Vector2.zero, 20, order + 5, current);
                if (locked) MenuUi.Sprite(cell, LockBadgeSprite, Vector2.one * (diameter * .36f), new Vector2(20, 22), order + 7);
                else if (won) MenuUi.Sprite(cell, WonBadgeSprite, Vector2.one * (diameter * .36f), Vector2.one * 18, order + 7);
                if (open || won) _nodes.Add((new Rect(x - 33, y - 33, 66, 66), i)); // at least 54 x 54 pt
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
