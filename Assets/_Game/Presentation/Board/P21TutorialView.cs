using ReverseSolver.Core;
using UnityEngine;
using TMPro;

namespace ReverseSolver.Presentation
{
    // Passive art only: GameRoot owns the four steps and all input still goes through DragModel/Core.
    sealed class P21TutorialView : MonoBehaviour
    {
        const int Order = 3350;
        TextView _speech;
        SpriteRenderer _hand, _mark;
        Transform _guide;
        ShapeView _glow;
        float _feedbackUntil;
        bool _correct;

        static Sprite Sprite(string key) => Resources.Load<Sprite>("Art/yeni/tutorial/" + key);
        static SpriteRenderer Place(Transform parent, string key, Vector2 point, Vector2 size, int order)
        {
            var sprite = Sprite(key);
            if (sprite == null) return null;
            var r = Draw.Child<SpriteRenderer>(parent, key);
            r.sprite = sprite; r.sortingOrder = order;
            r.transform.localPosition = Draw.W(point);
            r.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1);
            return r;
        }

        public static P21TutorialView Show(Transform world, Vector2 screen, BoardView board, int piece, string text, Dir? arrow)
        {
            var v = Draw.Child<P21TutorialView>(world, "P21 tutorial");
            float width = Mathf.Min(screen.x - 24, 366), height = 116;
            float y = Mathf.Min(screen.y - 142, board.Origin.y + board.Size.y + 92);
            Place(v.transform, "Speech/bubble", new Vector2(screen.x / 2, y), new Vector2(width, height), Order);
            Place(v.transform, "Speech/jigo-avatar", new Vector2(screen.x / 2 - width / 2 + 27, y), new Vector2(48, 56), Order + 1);
            v._speech = TextView.Create(v.transform, "instruction", text, 15, Palette.Navy, order: Order + 2);
            v._speech.transform.localPosition = Draw.W(screen.x / 2 + 23, y - 3);
            var tmp = v._speech.GetComponent<TextMeshPro>();
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.rectTransform.sizeDelta = new Vector2(width - 86, height - 20);
            tmp.enableAutoSizing = true; tmp.fontSizeMin = 12 * TextView.PointToFontSize; tmp.fontSizeMax = 15 * TextView.PointToFontSize;
            if (piece < 0) return v;
            float cell = board.Cell;
            var c = board.Session.Level.Pieces[piece][0];
            var center = board.Origin + new Vector2(c.X + .5f, c.Y + .5f) * cell;
            v._glow = ShapeView.Create(v.transform, "target", Vector2.one * cell, 8, Order - 1).Radius(7)
                .Fill(Draw.Rgba(255, 230, 140, 0)).Stroke(Draw.Hex("#fff3b0"), 3).Glow(Draw.Hex("#ffcf59"), 12);
            v._glow.transform.localPosition = Draw.W(center);
            v._mark = Place(v.transform, "Correct/01", center, Vector2.one * 55, Order + 4);
            if (v._mark != null) v._mark.enabled = false;
            if (arrow.HasValue)
            {
                v._guide = new GameObject("guide").transform; v._guide.SetParent(v.transform, false);
                string path = arrow == Dir.D ? "down" : arrow == Dir.L ? "left" : "right";
                var d = new Vector2(arrow.Value.Dx(), arrow.Value.Dy());
                var point = center + d * cell * .65f;
                point.x = Mathf.Clamp(point.x, 28, screen.x - 28);
                Place(v._guide, "Path/straight-" + path, point, new Vector2(42, 64), Order + 1);
                v._hand = Place(v._guide, "Hand/drag/01", center + new Vector2(16, 18), new Vector2(60, 78), Order + 2);
            }
            return v;
        }

        public void Feedback(bool correct, string text)
        {
            _correct = correct; _feedbackUntil = Time.unscaledTime + .65f;
            _speech.Text = text;
            if (_guide != null) _guide.gameObject.SetActive(!correct);
            if (_glow != null) _glow.gameObject.SetActive(!correct);
        }

        void Update()
        {
            if (_hand != null) _hand.sprite = Sprite("Hand/drag/" + (1 + (int)(Time.unscaledTime * 7) % 5).ToString("00"));
            if (_glow != null) _glow.Alpha(.65f + .25f * Mathf.Sin(Time.unscaledTime * 4));
            if (_mark != null)
            {
                _mark.enabled = Time.unscaledTime < _feedbackUntil;
                if (_mark.enabled) _mark.sprite = Sprite((_correct ? "Correct/" : "Jam/") + (1 + (int)(Time.unscaledTime * 9) % 4).ToString("00"));
            }
        }

        public void Remove() { if (this != null) Destroy(gameObject); }

        public static void Banner(Transform world, Vector2 screen, float safeTop, float seconds)
        {
            var root = new GameObject("Clear every piece banner").transform; root.SetParent(world, false);
            var size = new Vector2(screen.x - 20, 40);
            var center = new Vector2(screen.x / 2, safeTop + 34);
            ShapeView.Create(root, "background", size, 8, 3450).Radius(12).Fill(Palette.Navy).transform.localPosition = Draw.W(center);
            var text = TextView.Create(root, "goal", Lang.T("Clear every piece off the board!", "Bütün parçaları tahtadan çıkar!"), 16, Palette.Cream, order: 3451);
            text.transform.localPosition = Draw.W(center); text.FitWidth(size.x - 16, 12);
            Destroy(root.gameObject, seconds);
        }
    }
}
