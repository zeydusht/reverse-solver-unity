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
        readonly Rect _back, _topButton, _bottomButton;
        readonly Transform _currentPill;
        readonly TextView _currentLabel;
        readonly float _currentY, _height, _width;
        readonly int _currentLevel;
        Rect _pill;
        void SmoothTo(float target)
        {
            float from = Scroll.Offset;
            Tweens.Run(Scroll, .45f, t => { if (Scroll.Root != null && !Scroll.Held) Scroll.Set(Mathf.Lerp(from, target, t * t * (3 - 2 * t))); });
        }
        public void Tick()
        {
            float y = _currentY - Scroll.Offset;
            bool above = y < Scroll.Top + 35, below = y > Scroll.Bottom - 35;
            _currentPill.gameObject.SetActive(above || below);
            if (!above && !below) return;
            var center = new Vector2(_width / 2, above ? Scroll.Top + 22 : Scroll.Bottom - 22);
            _pill = new Rect(center - new Vector2(68, 18), new Vector2(136, 36));
            _currentPill.localPosition = Draw.W(center);
            _currentLabel.Text = Lang.T($"Level {_currentLevel}", $"Seviye {_currentLevel}") + (above ? " ↑" : " ↓");
        }
        readonly List<(Rect rect, int index)> _nodes = new List<(Rect, int)>();
        public LevelsMap(Transform root, Vector2 screen, float top, float bottom, IReadOnlyList<LevelData> levels, Progress progress, LevelImages images, int order, float? offset)
        {
            MenuUi.Scene(root, "levels", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Levels, order + 30, heading: true);
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
                    TextView.Create(cell, "play", Texts.Play, 20, Palette.Cream, false, order: order + 7).Heading(TextView.RimGreen).FitWidth(70, 14)
                        .transform.localPosition = Draw.W(bubbleAt + new Vector2(0, -3));
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
            _currentY = currentY; _currentLevel = progress.Next() + 1; _height = height; _width = screen.x;
            _topButton = MenuUi.Button(root, "button-small", Lang.T("Top", "En üste"), new Vector2(screen.x - 49, start + 22), new Vector2(82, 32), order + 32);
            _bottomButton = MenuUi.Button(root, "button-small", Lang.T("Bottom", "En alta"), new Vector2(screen.x - 49, bottom - 22), new Vector2(82, 32), order + 32);
            _currentPill = new GameObject("Current level shortcut").transform; _currentPill.SetParent(root, false);
            ShapeView.Create(_currentPill, "pill", new Vector2(136, 36), 0, order + 33).Radius(18).Fill(Draw.Rgba(20, 48, 70, .65f));
            _currentLabel = MenuUi.Label(_currentPill, "", Vector2.zero, 16, order + 34);
            Scroll.Set(offset ?? currentY - (start + (bottom - start) * .7f));
            Tick();
        }
        public int Hit(Vector2 pt)
        {
            if (_back.Contains(pt)) return -2;
            if (_topButton.Contains(pt)) { SmoothTo(0); return -1; }
            if (_bottomButton.Contains(pt)) { SmoothTo(_height - (Scroll.Bottom - Scroll.Top)); return -1; }
            if (_currentPill.gameObject.activeSelf && _pill.Contains(pt)) { SmoothTo(_currentY - (Scroll.Top + Scroll.Bottom) / 2); return -1; }
            if (!Scroll.Contains(pt)) return -1;
            foreach (var node in _nodes)
                if (node.rect.Contains(pt + new Vector2(0, Scroll.Offset))) return node.index;
            return -1;
        }
    }
}
