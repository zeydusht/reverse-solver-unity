using UnityEngine;

namespace ReverseSolver.Presentation
{
    /// <summary>One background-only camera travel. No Update/material cost while idle.</summary>
    public sealed class CorridorVictory : MonoBehaviour
    {
        public const float Duration = 3f;
        SpriteRenderer _background, _painting;
        Material _original, _travel;
        Texture2D _picture;
        float _startedAt;

        public static float Ease(float progress)
        {
            float t = Mathf.Clamp01(progress);
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        public static Vector3 CameraAt(float progress)
        {
            float t = Ease(progress), pull = Mathf.Sin(Mathf.PI * t);
            return new Vector3(-3.42f * t, 0, -2.0007f * t - .32f * pull * pull);
        }

        public void SetPainting(SpriteRenderer painting, Texture2D texture)
        {
            _painting = painting; _picture = texture;
            if (_travel == null) return;
            _travel.SetFloat("_HasPicture", texture != null ? 1 : 0);
            if (texture != null) {
                _travel.SetTexture("_Picture", texture);
                _travel.SetVector("_PictureSize", new Vector4(texture.width, texture.height, 0, 0));
            }
            if (_painting != null && enabled) _painting.enabled = false;
        }

        public bool Play(SpriteRenderer background, float startedAt)
        {
            var shader = Resources.Load<Shader>("CorridorVictory/Projection");
            if (shader == null || !shader.isSupported) { enabled = false; return false; }
            if (_travel == null) {
                _travel = new Material(shader) { name = "Corridor victory instance" };
                foreach (string part in new[] { "Wall", "Floor", "Frame", "Columns", "Arches" }) {
                    var tex = Resources.Load<Texture2D>("CorridorVictory/" + part.ToLowerInvariant());
                    if (tex == null) { Destroy(_travel); _travel = null; enabled = false; return false; }
                    _travel.SetTexture("_" + part, tex);
                }
            }
            _background = background; _original = background.sharedMaterial; _startedAt = startedAt;
            background.sharedMaterial = _travel;
            enabled = true;
            SetPainting(_painting, _picture);
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
            if (_background != null) _background.sharedMaterial = _original;
            if (_painting != null) _painting.enabled = true;
            enabled = false;
        }

        void OnDestroy()
        {
            Stop();
            if (_travel != null) Destroy(_travel);
        }
    }
}
