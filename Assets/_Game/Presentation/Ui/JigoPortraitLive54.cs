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
        // PM_74: Jigo sits wholly inside the arch (thimble included) and the arch clips the rest (JigoPortrait shader).
        // The open eyes hold one frame: cycling frames 1-4 at 20 fps read as constant blinking. A blink every 3-6 s.
        public const float PortraitShare = .725f, ThimbleClearance = 3f, BlinkMin = 3f, BlinkMax = 6f;
        static Material _clip;
        static readonly int WindowId = Shader.PropertyToID("_Window");
        readonly MaterialPropertyBlock _block = new MaterialPropertyBlock();
        readonly Transform _parent;
        readonly Vector2 _windowCenter, _windowHalf;
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
            // High stripping removes SpriteMask from the player: the arch is clipped in the JigoPortrait shader.
            float scale = size.x / 127f;
            var portraitSize = Vector2.one * (size.x * PortraitShare);
            // the thimble's top (8/256 of the frame) a few points under the arch's crown (.845 of the half height)
            float top = size.y / 2 * .845f - ThimbleClearance * scale;
            var portraitCenter = center + new Vector2(0, portraitSize.y / 2 - top - portraitSize.y * 8f / 256f);
            _back = Art54.Place(parent, _joy ? Art54.Joy : Art54.NormalFrame, portraitCenter, portraitSize, order);
            _front = Art54.Place(parent, _joy ? Art54.Joy : Art54.NormalFrame, portraitCenter, portraitSize, order + 1);
            _portraitHeight = portraitSize.y;
            _parent = parent; _windowCenter = center; _windowHalf = size / 2;
            var shader = Resources.Load<Shader>("Shaders/JigoPortrait");
            if (_clip == null && shader != null) _clip = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            if (_clip != null) { _front.sharedMaterial = _clip; _back.sharedMaterial = _clip; }
            _loops[0] = Frames("normal_loop"); _loops[1] = Frames("happy_loop");
            _transitions[0] = Frames("normal_to_happy"); _transitions[1] = Frames("happy_to_normal");
            _nextBlink = Time.unscaledTime + Random.Range(BlinkMin, BlinkMax);
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
            if (_clip != null)
            {
                var c = _parent.TransformPoint(Draw.W(_windowCenter));
                var half = Vector2.Scale(_windowHalf, (Vector2)_parent.lossyScale);
                _block.SetVector(WindowId, new Vector4(c.x, c.y, half.x, half.y));
                _front.SetPropertyBlock(_block); _back.SetPropertyBlock(_block);
            }
            if (_joy) { _back.color = Color.clear; return; }
            float now = Time.unscaledTime;
            int mood = _forced >= 0 ? _forced : (int)JigoMood.Read(_time, _fuse, now < _happyUntil, _lost, _won);
            if (mood != _current)
            {
                _previous = _current < 0 ? mood : _current;
                _current = mood; _changedAt = now;
            }
            if (now >= _nextBlink) { _blinkAt = now; _nextBlink = now + Random.Range(BlinkMin, BlinkMax); }
            // Legacy loop frames include a blink; play that sequence only at random intervals.
            int frame = now - _blinkAt < .375f ? Mathf.Min(5, (int)((now - _blinkAt) * 16)) : 0;
            Sprite Read(int m)
            {
                var loop = m == 0 ? _loops[0] : m == 3 ? _loops[1] : null;
                if (loop != null && loop.Length > 0)
                {
                    // The supplied normal loop embeds a blink in its latter half (frames 5-12): play it once per
                    // blink, otherwise hold the open-eye first frame.
                    int index = m == 0 ? now - _blinkAt < .4f ? 4 + (int)((now - _blinkAt) * 20) : 0
                        : (int)(Mathf.Max(0, now - _changedAt - JigoMood.FadeSeconds) * 12) % loop.Length;
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
            // a soft idle breath (2.6 s), small enough to keep the thimble inside the arch
            float breath = Mathf.Sin(now * Mathf.PI * 2 / 2.6f);
            float stretch = .02f * Mathf.Sin(Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds) * Mathf.PI);
            var scale = new Vector3(1 + .008f * breath + stretch, 1 + .012f * breath - stretch, 1);
            if (_front.sprite != null) _front.transform.localScale = scale * (_portraitHeight / _front.sprite.bounds.size.y);
            if (_back.sprite != null) _back.transform.localScale = scale * (_portraitHeight / _back.sprite.bounds.size.y);
            _front.transform.localRotation = _back.transform.localRotation = Quaternion.Euler(0, 0, .6f * breath);
            float alpha = Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds);
            _front.color = new Color(1, 1, 1, alpha);
            _back.color = new Color(1, 1, 1, 1 - alpha);
        }
    }
}
