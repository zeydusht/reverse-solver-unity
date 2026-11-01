using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class ProfileCard
    {
        public readonly Transform Root;
        readonly Rect _close, _name, _avatar;
        public ProfileCard(Transform parent, Vector2 screen, string name, string[] values, int order)
        {
            Root = new GameObject("player-profile").transform; Root.SetParent(parent, false);
            var c = screen / 2;
            ShapeView.Create(Root, "veil", screen, 0, order).Fill(Draw.Rgba(0, 0, 0, .65f)).transform.localPosition = Draw.W(c);
            float w = Mathf.Min(330, screen.x - 30), h = Mathf.Min(560, screen.y - 60), top = c.y - h / 2;
            MenuUi.Sprite(Root, "modal-panel", c, new Vector2(w, h), order + 1, true);
            MenuUi.Sprite(Root, "title-plaque", new Vector2(c.x, top + 32), new Vector2(w - 40, 55), order + 2, true);
            MenuUi.Label(Root, Texts.ProfileTitle, new Vector2(c.x, top + 32), 28, order + 3);
            _close = MenuUi.Button(Root, "button-close", null, new Vector2(c.x + w / 2 - 21, top + 22), Vector2.one * 36, order + 5);
            float avSize = Mathf.Min(94, h * .17f), avY = top + 66 + avSize / 2;
            JigglesScreen.Avatar(Root, Art.AvatarIndex, new Vector2(c.x, avY), avSize, order + 2, true);
            float ny = avY + avSize / 2 + 28;
            _name = MenuUi.Button(Root, "name-band", null, new Vector2(c.x, ny), new Vector2(w - 42, 45), order + 2);
            var playerName = MenuUi.Label(Root, name, new Vector2(c.x - 14, ny), 23, order + 3, true, w - 100);
            playerName.GetComponent<TMPro.TextMeshPro>().richText = false;
            MenuUi.Sprite(Root, "icon-pencil", new Vector2(c.x + w / 2 - 43, ny), Vector2.one * 26, order + 4);
            float gridTop = ny + 34, gridBottom = top + h - 91, pitch = (gridBottom - gridTop) / 3, tileW = (w - 48) / 2;
            for (int i = 0; i < 6; i++)
            {
                float x = c.x + (i % 2 == 0 ? -1 : 1) * (tileW / 2 + 4), y = gridTop + (i / 2 + .5f) * pitch;
                MenuUi.Sprite(Root, "stat-tile", new Vector2(x, y), new Vector2(tileW, pitch - 6), order + 2, true);
                MenuUi.Label(Root, values[i], new Vector2(x, y - 12), 23, order + 3, true, tileW - 12);
                MenuUi.Label(Root, Texts.ProfileStat(i), new Vector2(x, y + 16), 13, order + 3, true, tileW - 10);
            }
            _avatar = MenuUi.Button(Root, "button-secondary", Texts.ChangeAvatar, new Vector2(c.x, top + h - 49), new Vector2(w - 62, 57), order + 3);
        }
        public string Hit(Vector2 pt) => _close.Contains(pt) ? "close" : _name.Contains(pt) ? "name" : _avatar.Contains(pt) ? "avatar" : null;
        public void Destroy() { if (Root != null) Object.Destroy(Root.gameObject); }
    }
}
