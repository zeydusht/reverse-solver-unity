using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_75: one button family for the menu's "Level N" and the result screens (Zeyd: the win screen's buttons
       should look like the menu's): the menu's green 9-slice "level-button" for the main action, its navy twin
       (Tools/make_pm75_buttons.py) for the small one, Baloo labels with the matching rim. No idle motion; a press
       shrinks the button a little. */
    static class Button75
    {
        public const string Green = "level-button", GreenPressed = "level-button-pressed";
        public const string Blue = "level-button-blue", BluePressed = "level-button-blue-pressed";
        public const float Aspect = 228f / 112f;     // the menu button's proportions

        public static bool Available => Art.Get(Green) != null && Art.Get(Blue) != null;

        public sealed class View
        {
            public SpriteRenderer Body;
            public TextView Label;
            public Rect Rect;
            public Transform Root;
            string _normal, _pressed;

            /* The pressed look for a moment (the screen acts right after). */
            public void Press()
            {
                if (Body == null) return;
                var s = Art.Get(_pressed);
                if (s != null) Body.sprite = s;
                if (Root != null) Root.localScale = Vector3.one * .96f;
            }

            internal void Keys(string normal, string pressed) { _normal = normal; _pressed = pressed; }
        }

        /* The main action: the green button, size w x h points at centre c. */
        public static View Main(Transform parent, Vector2 c, Vector2 size, string text, int order) =>
            Make(parent, c, size, text, order, Green, GreenPressed, TextView.RimGreen, .34f);

        /* The small one (Menu, No thanks): the navy twin. */
        public static View Small(Transform parent, Vector2 c, Vector2 size, string text, int order) =>
            Make(parent, c, size, text, order, Blue, BluePressed, Draw.Hex("#0E2244"), .40f);

        /* PM_82: the win screen's own faces (win-next, win-menu; Codex option A), any size. */
        public static View Custom(Transform parent, Vector2 c, Vector2 size, string text, int order, string normal, string pressed, Vector4 rim, float textShare = .36f) =>
            Make(parent, c, size, text, order, normal, pressed, rim, textShare);

        static View Make(Transform parent, Vector2 c, Vector2 size, string text, int order, string normal, string pressed, Vector4 rim, float textShare)
        {
            var root = new GameObject("button " + normal).transform;
            root.SetParent(parent, false);
            root.localPosition = Draw.W(c);
            var v = new View { Root = root, Rect = new Rect(c - size / 2, size) };
            v.Keys(normal, pressed);
            v.Body = Art.Place(root, normal, size, order, sliced: true);
            v.Label = TextView.Create(root, "label", text, size.y * textShare, Palette.Cream, false, order: order + 2)
                .Heading(rim).FitWidth(size.x - Mathf.Min(40, size.x * .18f), size.y * .22f);
            v.Label.transform.localPosition = Draw.W(0, -2 * size.y / 72f);
            return v;
        }
    }
}
