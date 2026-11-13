using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // PM_62 portrait (default since PM_64): T46 loops and transitions, breathing and sway on top of the frames.
    // Codex's fixed arch portrait (JigoPortrait54) is used with &portre=sabit or &saat=kalkan.
    sealed class JigoPortraitLive54
    {
        readonly Sprite[][] _frames = new Sprite[5][];
        readonly SpriteRenderer _front, _back;
        readonly int _forced;
        readonly bool _joy;
        public void Visible(bool visible) { _front.gameObject.SetActive(visible); _back.gameObject.SetActive(visible); }
        int _current = -1, _previous, _time;
        int? _fuse;
        bool _lost, _won;
        float _happyUntil, _changedAt, _nextBlink, _blinkAt = -10;
        float _portraitHeight;
        readonly Sprite[][] _loops = new Sprite[2][], _transitions = new Sprite[2][];
        static readonly int[] OpenEyeFrames = { 0, 1, 2, 3, 2, 1 };
        static Sprite[] Frames(string path)
        {
            var frames = new System.Collections.Generic.List<Sprite>();
            for (int i = 1; i <= 48; i++) { var sprite = Art.Get("T46/jigo/" + path + "/" + i.ToString("00")); if (sprite == null) break; frames.Add(sprite); }
            return frames.ToArray();
        }

        public JigoPortraitLive54(Transform parent, Vector2 center, Vector2 size, int order, GameSession session)
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
            _portraitHeight = portraitSize.y;
            _loops[0] = Frames("normal_loop"); _loops[1] = Frames("happy_loop");
            _transitions[0] = Frames("normal_to_happy"); _transitions[1] = Frames("happy_to_normal");
            _nextBlink = Time.unscaledTime + Random.Range(4f, 7f);
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
            if (now >= _nextBlink) { _blinkAt = now; _nextBlink = now + Random.Range(4f, 7f); }
            // Legacy loop frames include a blink; play that sequence only at random intervals.
            int frame = now - _blinkAt < .375f ? Mathf.Min(5, (int)((now - _blinkAt) * 16)) : 0;
            Sprite Read(int m)
            {
                var loop = m == 0 ? _loops[0] : m == 3 ? _loops[1] : null;
                if (loop != null && loop.Length > 0)
                {
                    // The supplied normal loop embeds a blink in its latter half.
                    // Keep its open-eye motion looping; schedule the blink once every 4–7 seconds.
                    int index = m == 0 ? now - _blinkAt < .4f
                        ? 4 + (int)((now - _blinkAt) * 20)
                        : OpenEyeFrames[(int)(now * 20) % OpenEyeFrames.Length]
                        : (int)(Mathf.Max(0, now - _changedAt - JigoMood.FadeSeconds) * 20) % loop.Length;
                    return loop[Mathf.Clamp(index, 0, loop.Length - 1)];
                }
                return _frames[m][frame] ?? Art.Get(Art54.NormalFrame);
            }
            _front.sprite = Read(_current);
            _back.sprite = Read(_previous);
            int transition = _previous == 0 && _current == 3 ? 0 : _previous == 3 && _current == 0 ? 1 : -1;
            if (transition >= 0 && now - _changedAt < JigoMood.FadeSeconds && _transitions[transition].Length > 0)
            {
                var frames = _transitions[transition];
                _front.sprite = frames[Mathf.Min(frames.Length - 1, (int)((now - _changedAt) / JigoMood.FadeSeconds * frames.Length))];
            }
            float breath = Mathf.Sin(now * Mathf.PI * 2 / 2.2f);
            float stretch = .035f * Mathf.Sin(Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds) * Mathf.PI);
            var scale = new Vector3(1 + .02f * breath + stretch, 1 + .02f * breath - stretch, 1);
            if (_front.sprite != null) _front.transform.localScale = scale * (_portraitHeight / _front.sprite.bounds.size.y);
            if (_back.sprite != null) _back.transform.localScale = scale * (_portraitHeight / _back.sprite.bounds.size.y);
            _front.transform.localRotation = _back.transform.localRotation = Quaternion.Euler(0, 0, 1.5f * breath);
            float alpha = Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds);
            _front.color = new Color(1, 1, 1, alpha);
            _back.color = new Color(1, 1, 1, 1 - alpha);
        }
    }
}
