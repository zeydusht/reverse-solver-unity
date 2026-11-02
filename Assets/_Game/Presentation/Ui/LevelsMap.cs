using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LevelsMap
    {
        // MenuUi.Sprite adds the t20- prefix; swap map art here when Task23 arrives.
        const string BackSprite = "button-back";
        const string PathSprite = "t20-levels-path-repeat.png";
        const string CurrentNodeSprite = "node-current";
        const string CompleteNodeSprite = "node-complete";
        const string HardOpenNodeSprite = "node-hard-open";
        const string LockedBaseSprite = "node-locked-base";
        const string HardLockedBaseSprite = "node-hard-base";
        const string GlowSprite = "node-glow";
        const string HardBadgeSprite = "baron-head";
        const string LockBadgeSprite = "icon-lock";
        const string WonBadgeSprite = "icon-green-check";

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
                // Radial alpha makes a soft ellipse without changing the shared shape shader.
                ShapeView.Create(cell, "NodeStepShadow", new Vector2(diameter * .8f, 9), 1, order + 1).Radius(4.5f)
                    .Radial(new Vector4(0, 0, 0, .25f), new Vector4(0, 0, 0, .12f), Vector4.zero,
                        new Vector4(.5f, .5f, .5f, .5f))
                    .transform.localPosition = Draw.W(0, diameter / 2 + 4.5f);
                string key = locked ? (hard ? HardLockedBaseSprite : LockedBaseSprite) :
                    hard ? HardOpenNodeSprite : current ? CurrentNodeSprite : won ? CompleteNodeSprite : CurrentNodeSprite;
                if (current) MenuUi.Sprite(cell, GlowSprite, Vector2.zero, Vector2.one * 94, order + 1);
                var node = MenuUi.Sprite(cell, key, Vector2.zero, Vector2.one * diameter, order + 2);
                if (locked)
                {
                    if (node != null) node.color = new Color(.65f, .65f, .65f, 1);
                    else ShapeView.Create(cell, "LockedNodeBase", Vector2.one * diameter, 1, order + 2)
                        .Radius(diameter / 2).Fill(Draw.Hex("#48525b"));
                }
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
                if (hard && open) MenuUi.Sprite(cell, HardBadgeSprite, new Vector2(25, -20), Vector2.one * 30, order + 4);
                MenuUi.Label(cell, (i + 1).ToString(), Vector2.zero, 20, order + 5, current);
                if (locked || won)
                {
                    var badgeAt = Vector2.one * (diameter * .33f);
                    if (locked)
                        ShapeView.Create(cell, "LockBadgeBase", Vector2.one * 23, 1, order + 6).Radius(11.5f)
                            .Fill(Draw.Hex("#24313b")).transform.localPosition = Draw.W(badgeAt);
                    MenuUi.Sprite(cell, locked ? LockBadgeSprite : WonBadgeSprite, badgeAt,
                        Vector2.one * 20, order + 7);
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
