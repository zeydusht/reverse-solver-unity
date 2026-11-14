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
            if (MenuSettings.T48 && Art.Get("T48/profile/stat-tile") != null)
            {
                (_close, _name, _avatar) = BuildT48(c, w, h, top, name, values, order);
                return;
            }
            MenuUi.Sprite(Root, "modal-panel", c, new Vector2(w, h), order + 1, true);
            MenuUi.Sprite(Root, "title-plaque", new Vector2(c.x, top + 32), new Vector2(w - 40, 55), order + 2, true);
            MenuUi.Banner(Root, Texts.ProfileTitle, new Vector2(c.x, top + 32), 28, order + 3);
            _close = MenuUi.Button(Root, "button-close", null, new Vector2(c.x + w / 2 - 21, top + 22), Vector2.one * 36, order + 5);
            _close = new Rect(_close.center - Vector2.one * 22, Vector2.one * 44);
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
        /* PM_67: TASK48 profile (cream panel, blue frame, 3D title, cream-over-light-blue tiles, green button, red X). */
        (Rect close, Rect name, Rect avatar) BuildT48(Vector2 c, float w, float h, float top, string name, string[] values, int order)
        {
            const string P = "T48/profile/";
            SpriteRenderer Put(string key, Vector2 at, Vector2 size, int o, bool sliced = false)
            {
                var r = Art.Place(Root, key, size, o, sliced);
                if (r != null) r.transform.localPosition = Draw.W(at);
                return r;
            }
            Put("devam_v2/panel", c, new Vector2(w, h), order + 1, true);
            var title = TextView.Create(Root, "title", Texts.ProfileTitle, 32, Palette.Cream, false, order: order + 3).Title3D().FitWidth(w - 90, 20);
            title.transform.localPosition = Draw.W(c.x, top + 40);
            var x = new Vector2(c.x + w / 2 - 6, top + 6);
            Put(P + "close-button", x, new Vector2(41, 43), order + 5);
            Put(P + "icon-close", x, Vector2.one * 19, order + 6);
            float avSize = Mathf.Min(90, h * .16f), avY = top + 74 + avSize / 2;
            Put(P + "avatar-disc", new Vector2(c.x, avY), Vector2.one * avSize * .7f, order + 2);
            JigglesScreen.Avatar(Root, Art.AvatarIndex, new Vector2(c.x, avY), avSize, order + 3, true);
            float ny = avY + avSize / 2 + 26;
            Put(P + "name-field", new Vector2(c.x, ny), new Vector2(w - 54, 40), order + 2, true);
            var playerName = MenuUi.Label(Root, name, new Vector2(c.x - 12, ny), 21, order + 3, true, w - 110);
            playerName.GetComponent<TMPro.TextMeshPro>().richText = false;
            var edit = new Vector2(c.x + w / 2 - 50, ny);
            Put(P + "edit-button", edit, Vector2.one * 30, order + 3);
            Put(P + "icon-edit", edit, Vector2.one * 18, order + 4);
            float gridTop = ny + 30, gridBottom = top + h - 86, pitch = (gridBottom - gridTop) / 3, tileW = (w - 52) / 2;
            for (int i = 0; i < 6; i++)
            {
                float tx = c.x + (i % 2 == 0 ? -1 : 1) * (tileW / 2 + 4), ty = gridTop + (i / 2 + .5f) * pitch;
                Put(P + "stat-tile", new Vector2(tx, ty), new Vector2(tileW, pitch - 7), order + 2, true);
                MenuUi.Label(Root, values[i], new Vector2(tx, ty - 11), 22, order + 3, true, tileW - 12);
                MenuUi.Label(Root, Texts.ProfileStat(i), new Vector2(tx, ty + 15), 12, order + 3, true, tileW - 10);
            }
            var buttonC = new Vector2(c.x, top + h - 46);
            Put(P + "avatar-button", buttonC, new Vector2(w - 70, 51), order + 3, true);
            var label = TextView.Create(Root, "avatar label", Texts.ChangeAvatar, 19, Palette.Cream, false, order: order + 4).Heading(TextView.RimGreen).FitWidth(w - 110, 13);
            label.transform.localPosition = Draw.W(buttonC);
            return (new Rect(x - Vector2.one * 24, Vector2.one * 48), new Rect(c.x - (w - 54) / 2, ny - 22, w - 54, 44),
                    new Rect(buttonC - new Vector2(w - 70, 51) / 2, new Vector2(w - 70, 51)));
        }

        public string Hit(Vector2 pt) => _close.Contains(pt) ? "close" : _name.Contains(pt) ? "name" : _avatar.Contains(pt) ? "avatar" : null;
        public void Destroy() { if (Root != null) Object.Destroy(Root.gameObject); }
    }
}
