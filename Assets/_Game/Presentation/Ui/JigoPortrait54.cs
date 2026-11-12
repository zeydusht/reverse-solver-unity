using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class JigoPortrait54
    {
        const string OpenEyesKey = "hud/jigo-portrait-open", ClosedEyesKey = "hud/jigo-portrait-closed";
        static readonly int WindowId = Shader.PropertyToID("_Window"), ClosedEyesId = Shader.PropertyToID("_ClosedEyes"),
            BlinkId = Shader.PropertyToID("_Blink"), OpacityId = Shader.PropertyToID("_Opacity");
        static Material _material;

        readonly Sprite[] _faces = new Sprite[5];
        readonly SpriteRenderer _front, _back, _shield, _grip;
        readonly MaterialPropertyBlock _properties = new MaterialPropertyBlock();
        readonly Texture _closedEyes;
        readonly float _portraitHeight;
        readonly int _forced;
        readonly bool _joy;
        int _current = -1, _previous, _time;
        int? _fuse;
        bool _lost, _won, _shieldAway;
        float _happyUntil, _changedAt, _nextBlink, _blinkAt = -10;

        public JigoPortrait54(Transform parent, Vector2 center, Vector2 size, int order, GameSession session)
        {
            string forced = Host.Current.Query("jigo");
            _forced = System.Array.IndexOf(Art54.Moods, forced);
            _joy = forced == "sevinc";
            for (int i = 0; i < _faces.Length; i++)
                _faces[i] = Art.Get((i == 4 ? "jigo-clip/" : "jigo/") + Art54.Moods[i] + "/01");
            var open = Art.Get(OpenEyesKey);
            if (open != null) _faces[0] = open;
            _closedEyes = Art.Get(ClosedEyesKey)?.texture;

            var aperture = Draw.W(center);
            _properties.SetVector(WindowId, new Vector4(aperture.x, aperture.y, size.x / 2, size.y / 2));
            if (_closedEyes != null) _properties.SetTexture(ClosedEyesId, _closedEyes);
            if (_material == null)
                _material = new Material(Resources.Load<Shader>("Shaders/JigoPortrait")) { hideFlags = HideFlags.HideAndDontSave };

            float scale = size.x / 127f;
            _portraitHeight = size.y * .91f;
            var portraitCenter = center + new Vector2(0, 2.5f * scale);
            _back = Layer(parent, "Jigo previous expression", portraitCenter, order);
            _front = Layer(parent, "Jigo portrait", portraitCenter, order + 1);
            float shieldSize = Hud54.RestingShieldSize * scale;
            var shieldRest = Hud54.ShieldRest(center, scale);
            _shield = Layer(parent, "Jigo resting shield", shieldRest, order + 2);
            _grip = Layer(parent, "Jigo resting grip", Hud54.ShieldGrip(shieldRest, shieldSize, 0), order + 3);
            if (open != null)
            {
                _shield.sprite = Resources.Load<Sprite>("Art/yeni/boosters21/Clock/Layers/button-shield");
                _grip.sprite = Art.Get("hud/jigo-shield-grip");
                if (_shield.sprite != null) _shield.transform.localScale = Vector3.one * (shieldSize / _shield.sprite.bounds.size.x);
                if (_grip.sprite != null) _grip.transform.localScale = Vector3.one * (23 * scale / _grip.sprite.bounds.size.x);
            }

            _nextBlink = Time.unscaledTime + Random.Range(4f, 7f);
            Sync(session);
            Update();
        }

        SpriteRenderer Layer(Transform parent, string name, Vector2 center, int order)
        {
            var layer = Draw.Child<SpriteRenderer>(parent, name);
            layer.sharedMaterial = _material;
            layer.sortingOrder = order;
            layer.transform.localPosition = Draw.W(center);
            return layer;
        }

        void Show(SpriteRenderer layer, float alpha, float blink = 0)
        {
            layer.enabled = layer.sprite != null && alpha > 0;
            if (!layer.enabled) return;
            _properties.SetFloat(OpacityId, alpha);
            _properties.SetFloat(BlinkId, blink);
            layer.SetPropertyBlock(_properties);
        }

        void Face(SpriteRenderer layer, Sprite sprite, float alpha, float blink)
        {
            layer.sprite = sprite;
            if (sprite != null) layer.transform.localScale = Vector3.one * (_portraitHeight / sprite.bounds.size.y);
            Show(layer, alpha, blink);
        }

        public void ShieldAway(bool away) => _shieldAway = away;

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
            if (_joy)
            {
                Face(_front, Art.Get(Art54.Joy), 1, 0);
                Show(_back, 0);
                Show(_shield, 0);
                Show(_grip, 0);
                return;
            }

            float now = Time.unscaledTime;
            int mood = _shieldAway ? 0 : _forced >= 0 ? _forced : (int)JigoMood.Read(_time, _fuse, now < _happyUntil, _lost, _won);
            if (mood != _current)
            {
                _previous = _current < 0 || _shieldAway ? mood : _current;
                _current = mood;
                _changedAt = now;
            }
            if (now >= _nextBlink)
            {
                if (mood == 0) _blinkAt = now;
                _nextBlink = now + Random.Range(4f, 7f);
            }

            float fade = _previous == _current ? 1 : Mathf.Clamp01((now - _changedAt) / JigoMood.FadeSeconds);
            float blink = _current == 0 && _closedEyes != null ? JigoMood.BlinkClosure(now - _blinkAt) : 0;
            Face(_front, _faces[_current] ?? _faces[0], fade, blink);
            Face(_back, _faces[_previous] ?? _faces[0], 1 - fade, 0);
            float shieldAlpha = _shieldAway ? 0 : _current == 0 ? fade : _previous == 0 ? 1 - fade : 0;
            Show(_shield, shieldAlpha);
            Show(_grip, shieldAlpha);
        }
    }
}
