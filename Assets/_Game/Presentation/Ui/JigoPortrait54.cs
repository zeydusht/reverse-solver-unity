using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class JigoPortrait54
    {
        readonly Sprite[][] _frames = new Sprite[5][];
        readonly SpriteRenderer _front, _back;
        readonly int _forced;
        readonly bool _joy;
        int _current = -1, _previous, _time;
        int? _fuse;
        bool _lost, _won;
        float _happyUntil, _changedAt;

        public JigoPortrait54(Transform parent, Vector2 center, Vector2 size, int order, GameSession session)
        {
            string forced = Host.Current.Query("jigo");
            _forced = System.Array.IndexOf(Art54.Moods, forced);
            _joy = forced == "sevinc";
            for (int m = 0; m < _frames.Length; m++)
            {
                _frames[m] = new Sprite[6];
                for (int f = 0; f < 6; f++) _frames[m][f] = Art.Get((m == 4 ? "jigo-clip/" : "jigo/") + Art54.Moods[m] + "/" + (f + 1).ToString("00"));
            }
            // Frames are pre-clipped to the arch: High stripping removes SpriteMask from the player.
            var portraitSize = Vector2.one * (size.x * .85f);
            _back = Art54.Place(parent, _joy ? Art54.Joy : Art54.NormalFrame, center, portraitSize, order);
            _front = Art54.Place(parent, _joy ? Art54.Joy : Art54.NormalFrame, center, portraitSize, order + 1);
            Sync(session);
            Update();
        }

        void Sync(GameSession session)
        {
            _time = session.TimeLeft;
            _lost = session.IsOver && session.Outcome != Outcome.Win;
            _won = session.Outcome == Outcome.Win;
            _fuse = null;
            foreach (var bomb in session.Bombs)
                if (!_fuse.HasValue || bomb.Value < _fuse.Value) _fuse = bomb.Value;
        }

        public void OnEvent(GameEvent e, GameSession session)
        {
            if (e.Kind == EventKind.PieceRemoved) _happyUntil = Time.unscaledTime + JigoMood.HappySeconds;
            Sync(session);
        }

        public void Update()
        {
            if (_joy) { _back.color = Color.clear; return; }
            float now = Time.unscaledTime;
            int mood = _forced >= 0 ? _forced : (int)JigoMood.Read(_time, _fuse, now < _happyUntil, _lost, _won);
            if (mood != _current)
            {
                _previous = _current < 0 ? mood : _current;
                _current = mood; _changedAt = now;
            }
            int frame = (int)(now * JigoMood.FramesPerSecond) % 6;
            _front.sprite = _frames[_current][frame];
            _back.sprite = _frames[_previous][frame];
            float alpha = Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds);
            _front.color = new Color(1, 1, 1, alpha);
            _back.color = new Color(1, 1, 1, 1 - alpha);
        }
    }
}
