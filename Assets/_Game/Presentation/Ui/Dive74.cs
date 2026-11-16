using System;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_74: the dive into a painting. The camera eases into a picture until it covers the screen; the level is
       built behind a short light; the camera starts on the board's picture and pulls back to the game; the HUD and
       the booster bar slide in last. Menu -> level ≈ .95 s; win -> next adds the finished picture shrinking to the
       wall while the next one comes in (≈ 1 s). &gecis=yok: off (menu -> level at once, win -> the corridor). */
    public sealed class Dive74 : MonoBehaviour
    {
        public static bool On = true;
        public const float In = .45f, Out = .4f, HudSlide = .26f, Light = .16f;

        Camera _cam;
        Vector3 _fromPos, _toPos;
        float _fromSize, _toSize, _start, _seconds;
        bool _easeIn;
        Action _done;
        Action<float> _step;

        public static float CoverSize(Vector2 size, Vector2 screen) => Mathf.Min(size.y / 2, size.x / (2 * screen.x / screen.y));

        /* Eases the camera to (pos, size); `step` gets the eased 0..1 every frame, `done` runs once at the end. */
        public static Dive74 Run(Camera cam, Vector3 pos, float size, float seconds, bool easeIn, Action done, Action<float> step = null)
        {
            var d = new GameObject("PM74 dive").AddComponent<Dive74>();
            d._cam = cam; d._fromPos = cam.transform.position; d._fromSize = cam.orthographicSize;
            d._toPos = pos; d._toSize = size; d._seconds = seconds; d._easeIn = easeIn; d._done = done; d._step = step;
            d._start = Time.unscaledTime;
            return d;
        }

        void Update()
        {
            if (_cam == null) { Destroy(gameObject); return; }
            float t = Mathf.Clamp01((Time.unscaledTime - _start) / _seconds);
            float e = _easeIn ? BoosterFx74.InCubic(t) : BoosterFx74.OutCubic(t);
            // zoom in log space, so the speed feels even across the scale change
            _cam.orthographicSize = Mathf.Exp(Mathf.Lerp(Mathf.Log(_fromSize), Mathf.Log(_toSize), e));
            _cam.transform.position = Vector3.Lerp(_fromPos, _toPos, e);
            _step?.Invoke(e);
            if (t >= 1)
            {
                var done = _done; _done = null;
                Destroy(gameObject);
                done?.Invoke();
            }
        }

        /* A soft cream light over the whole view, fading out (hides the switch from the painting to the board). */
        public static void Flash(Camera cam, float seconds, int order)
        {
            var go = new GameObject("PM74 dive light");
            go.transform.SetParent(cam.transform, false);
            go.transform.localPosition = new Vector3(0, 0, 9);
            var r = go.AddComponent<SpriteRenderer>();
            var tex = new Texture2D(2, 2) { hideFlags = HideFlags.HideAndDontSave };
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white }); tex.Apply(false, true);
            r.sprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(.5f, .5f), 2);
            r.sortingOrder = order;
            float start = Time.unscaledTime;
            var light = new Color(1f, .96f, .86f);
            Tweens.Run(go, seconds, t =>
            {
                if (r == null || cam == null) return;
                float h = cam.orthographicSize * 2, w = h * cam.aspect;
                go.transform.localScale = new Vector3(w * 1.1f, h * 1.1f, 1);
                r.color = new Color(light.r, light.g, light.b, .85f * (1 - t) * (1 - t));
            }, () => { if (go != null) Destroy(go); Destroy(tex); });
        }
    }
}
