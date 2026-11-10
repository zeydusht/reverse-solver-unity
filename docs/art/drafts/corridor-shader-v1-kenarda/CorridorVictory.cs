using UnityEngine;

namespace ReverseSolver.Presentation
{
    /// <summary>One background-only camera travel. No Update/material cost while idle.</summary>
    public sealed class CorridorVictory : MonoBehaviour
    {
        public const float Duration = 3f;
        SpriteRenderer _background;
        Material _original, _travel, _still;
        Texture2D _picture;
        float _startedAt;

        public static float Ease(float progress)
        {
            float t = Mathf.Clamp01(progress);
            // Smooth acceleration/deceleration, with a readable constant-speed middle.
            const float ramp = .22f, gain = 1f / (1f - ramp);
            if (t < ramp) {
                float u = t / ramp;
                return gain * ramp * (u * u * u - .5f * u * u * u * u);
            }
            if (t > 1f - ramp) return 1f - Ease(1f - t);
            return gain * (t - .5f * ramp);
        }

        public static Vector3 CameraAt(float progress)
        {
            float t = Ease(progress), pull = Mathf.Sin(Mathf.PI * t);
            return new Vector3(-3.42f * t, 0, -2.0007f * t - .32f * pull * pull);
        }

        void Awake() { enabled = false; }

        void WritePainting(Material material)
        {
            if (material == null) return;
            material.SetFloat("_HasPicture", _picture != null ? 1 : 0);
            if (_picture != null) {
                material.SetTexture("_Picture", _picture);
                material.SetVector("_PictureSize", new Vector4(_picture.width, _picture.height, 0, 0));
            }
        }

        public void SetPainting(Texture2D texture)
        {
            _picture = texture;
            WritePainting(_travel); WritePainting(_still);
        }

        public void ShowPainting(SpriteRenderer background, Texture2D texture)
        {
            _background = background;
            if (_still == null) {
                var shader = Resources.Load<Shader>("CorridorVictory/Projection");
                if (shader == null || !shader.isSupported) return;
                _still = new Material(shader) { name = "Corridor static painting" };
                _still.SetFloat("_Progress", 0);
            }
            SetPainting(texture);
            if (!enabled) background.sharedMaterial = _still;
        }

        public bool Play(SpriteRenderer background, float startedAt)
        {
            var shader = Resources.Load<Shader>("CorridorVictory/Projection");
            if (shader == null || !shader.isSupported) { enabled = false; return false; }
            if (_travel == null) {
                _travel = new Material(shader) { name = "Corridor victory instance" };
                _travel.SetTexture("_Source", Resources.Load<Texture2D>("CorridorVictory/Source") ?? Resources.Load<Texture2D>("menu_bg"));
                foreach (string part in new[] { "Wall", "Floor", "Frame", "Columns", "Arches" }) {
                    var tex = Resources.Load<Texture2D>("CorridorVictory/" + part.ToLowerInvariant());
                    if (tex == null) { Destroy(_travel); _travel = null; enabled = false; return false; }
                    _travel.SetTexture("_" + part, tex);
                }
            }
            _background = background; _original = background.sharedMaterial; _startedAt = startedAt;
            background.sharedMaterial = _travel;
            enabled = true;
            SetPainting(_picture);
            Update();
            return true;
        }

        void Update()
        {
            if (_travel == null || _background == null) { enabled = false; return; }
            float p = Mathf.Clamp01((Time.unscaledTime - _startedAt) / Duration);
            _travel.SetFloat("_Progress", p);
            _travel.SetVector("_CameraOffset", CameraAt(p));
            if (p >= 1) Stop();
        }

        void Stop()
        {
            if (_background != null) _background.sharedMaterial = _still != null ? _still : _original;
            enabled = false;
        }

        void OnDestroy()
        {
            Stop();
            if (_travel != null) Destroy(_travel);
            if (_still != null) Destroy(_still);
        }
    }
}
