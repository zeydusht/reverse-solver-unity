using ReverseSolver.Core;
using UnityEngine;
using TMPro;

namespace ReverseSolver.Presentation
{
    // Passive art only: GameRoot owns the four steps and all input still goes through DragModel/Core.
    sealed class P21TutorialView : MonoBehaviour
    {
        const int Order = 3350;
        const float ChatHeight = 86, ChatGap = 10;
        public const float ChatClearance = ChatHeight + ChatGap;
        TextView _speech;
        SpriteRenderer _hand, _mark;
        Transform _guide;
        ShapeView[] _trail;
        ShapeView _glow;
        float _feedbackUntil;
        bool _correct;
        Transform _chat;
        Rect _chatRect;
        TextMeshPro _typed;
        float _typedAt;
        PieceView _storyPiece;

        public static P21TutorialView Story(Transform world, Vector2 screen, float safeBottom, BoardView board,
            int piece, string text, bool visible)
        {
            var v = Draw.Child<P21TutorialView>(world, "P21 story");
            float width = Mathf.Min(screen.x - 24, 366), height = ChatHeight;
            var center = new Vector2(screen.x / 2, Hud54.ButtonsTop(screen, safeBottom) - height / 2 - ChatGap);
            v._chatRect = new Rect(center - new Vector2(width, height) / 2, new Vector2(width, height));
            v._chat = new GameObject("chat").transform; v._chat.SetParent(v.transform, false);
            ShapeView.Create(v._chat, "cream blue chat", v._chatRect.size, 8, Order).Radius(14)
                .Fill(Palette.Cream).Stroke(Draw.Hex("#4b96c6"), 2).transform.localPosition = Draw.W(center);
            Place(v._chat, "Speech/jigo-avatar", center + new Vector2(-width / 2 + 29, 0), new Vector2(46, 54), Order + 1);
            v._speech = TextView.Create(v._chat, "story", text, 15, Palette.Navy, order: Order + 2);
            v._speech.transform.localPosition = Draw.W(center + new Vector2(25, 0));
            v._typed = v._speech.GetComponent<TextMeshPro>();
            v._typed.textWrappingMode = TextWrappingModes.Normal;
            v._typed.rectTransform.sizeDelta = new Vector2(width - 80, height - 16);
            v._typed.enableAutoSizing = true; v._typed.fontSizeMin = 12 * TextView.PointToFontSize;
            v._typed.fontSizeMax = 15 * TextView.PointToFontSize;
            v.StoryText(text); v._chat.gameObject.SetActive(visible);
            v.StoryTarget(board, piece);
            return v;
        }

        public static P21TutorialView Guided(Transform world, Vector2 screen, float safeBottom, BoardView board,
            int piece, string text, Dir? direction)
        {
            var v = Story(world, screen, safeBottom, board, piece, text, true);
            var c = board.Session.Level.Pieces[piece][0];
            var center = board.Origin + new Vector2(c.X + .5f, c.Y + .5f) * board.Cell;
            var axis = direction.HasValue ? new Vector2(direction.Value.Dx(), direction.Value.Dy()) : Vector2.zero;
            v._mark = Place(v.transform, "Correct/01", center + axis * board.Cell * .34f, Vector2.one * 46, Order + 4);
            if (v._mark != null) v._mark.enabled = false;
            if (!direction.HasValue) return v;

            v._guide = new GameObject("Swipe path").transform;
            v._guide.SetParent(v.transform, false);
            var start = center + axis * (board.Cell * .18f);
            var tip = center + axis * (board.Cell * .62f);
            var gold = Draw.Hex("#FFCF59");
            v._trail = new ShapeView[3];
            for (int i = 0; i < v._trail.Length; i++)
            {
                var dot = ShapeView.Create(v._guide, "Swipe path dot", Vector2.one * 6, 3, Order + 2)
                    .Radius(3).Fill(gold).Stroke(Palette.Navy, 1);
                dot.transform.localPosition = Draw.W(Vector2.Lerp(start, tip, i * .24f));
                v._trail[i] = dot;
            }
            var across = new Vector2(-axis.y, axis.x);
            var basePoint = tip - axis * 11;
            SolidView.Polygon(v._guide, "Swipe path arrow",
                new[] { tip, basePoint + across * 6, basePoint - across * 6 }, gold, Order + 2);
            return v;
        }

        public void StoryText(string text)
        {
            if (_chat == null) return;
            _chat.gameObject.SetActive(true); _speech.Text = text;
            _typedAt = Time.unscaledTime; _typed.maxVisibleCharacters = 0;
        }

        public void StoryTarget(BoardView board, int piece)
        {
            if (_storyPiece != null) _storyPiece.SetGlow(Vector4.zero, 0);
            _storyPiece = piece >= 0 ? board.PieceViewOf(piece) : null;
        }

        public bool DismissStory(Vector2 point)
        {
            if (_chat == null || !_chat.gameObject.activeSelf || !_chatRect.Contains(point)) return false;
            _chat.gameObject.SetActive(false); return true;
        }

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
            if (_typed != null) _typedAt = Time.unscaledTime - 5;
            if (_guide != null) _guide.gameObject.SetActive(false);
            if (_glow != null) _glow.gameObject.SetActive(!correct);
            if (correct && _storyPiece != null)
            {
                _storyPiece.SetGlow(Vector4.zero, 0);
                _storyPiece = null;
            }
        }

        void Update()
        {
            if (_typed != null) _typed.maxVisibleCharacters = (int)((Time.unscaledTime - _typedAt) * 40);
            if (_storyPiece != null) _storyPiece.SetGlow(Draw.Hex("#ffcf59"), .55f + .2f * Mathf.Sin(Time.unscaledTime * 3));
            if (_trail != null)
                for (int i = 0; i < _trail.Length; i++)
                    _trail[i].Alpha(.66f + .34f * Mathf.Sin(Time.unscaledTime * 7 - i));
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
