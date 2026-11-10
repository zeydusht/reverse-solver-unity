using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class BombMotion : MonoBehaviour
    {
        Transform _body;
        SpriteRenderer _shell, _fuse, _halo;
        readonly Sprite[] _frames = new Sprite[8];
        readonly Sprite[] _urgentFrames = new Sprite[8];
        bool _t54;
        Sprite _urgentFire;
        Vector3 _fireScale;
        float _width, _start, _pump, _shakeX;
        bool _urgent, _exploding;
        static Texture2D _haloTexture;
        static Sprite _haloSprite;
        static Material _haloMaterial;

        // Alpha-weighted pixel centres measured from the 384x384 Resources/Art/yeni PNGs
        // with a throwaway System.Drawing script. Normalized UVs, origin bottom-left:
        // 08: (241.034751, 17.345850) top-left px; urgent: (244.114898, 20.782118).
        static readonly Vector2 SparkNormal = new Vector2(.62769466f, .95482851f);
        static readonly Vector2 SparkUrgent = new Vector2(.63571588f, .94587990f);
        const float FireSize = 1.8f;
        // 576px canvases: match PM55's visible widths (51/96px at 1.8x).
        const float FireNormal54 = .504396f, FireUrgent54 = .696774f;
        static readonly Vector2 SparkNormal54 = new Vector2(.60654787f, .80007222f);
        static readonly Vector2 SparkUrgent54 = new Vector2(.60674304f, .80017937f);
        float FireFactor => _t54 ? (_urgent ? FireUrgent54 : FireNormal54) : FireSize;

        public void Init(Transform body, float width, int order)
        {
            _body = body; _width = width; _start = Time.unscaledTime;
            _t54 = Art.T54;
            _shell = Art.Place(body, "bomb-v2", Vector2.one * width, order);
            _fuse = Art.Place(body, _t54 ? "obstacles_v2/bomb/normal/spark-01" : "bomb-fuse-08",
                Vector2.one * width * (_t54 ? 1.5f : 1), order + 1);
            for (int i = 0; i < _frames.Length; i++)
            {
                _frames[i] = Art.Get(_t54 ? "obstacles_v2/bomb/normal/spark-" + (i + 1).ToString("00") : "bomb-fuse-" + (8 - i).ToString("00"));
                if (_t54) _urgentFrames[i] = Art.Get("obstacles_v2/bomb/urgent/spark-" + (i + 1).ToString("00"));
            }
            _urgentFire = Art.Get("bomb-fuse-urgent");
            if (_fuse != null)
            {
                _fireScale = _fuse.transform.localScale;
                _fuse.transform.localScale = _fireScale * FireFactor;
                CreateHalo(order);
                PositionFire();
            }
        }

        public void SetFuse(int value)
        {
            bool urgent = value <= 3;
            if (_urgent != urgent)
            {
                Tweens.Kill(this);
                _pump = _shakeX = 0;
                _body.localScale = Vector3.one;
                _body.localPosition = Vector3.zero;
            }
            _urgent = urgent;
            if (_shell != null) _shell.sprite = Art.Get(urgent ? "bomb-v2-urgent" : "bomb-v2");
            if (_fuse != null)
            {
                _fuse.sprite = urgent ? (_t54 ? _urgentFrames[0] : _urgentFire) : _frames[0];
                _fuse.transform.localScale = _fireScale * FireFactor;
                PositionFire();
            }
        }

        void Update()
        {
            if (_exploding) return;
            float seconds = Time.unscaledTime - _start;
            float pulse = (_urgent ? .08f : .055f) * Mathf.Sin(seconds * 2 * Mathf.PI / (_urgent ? .6f : .9f));
            _body.localScale = Vector3.one * ((1 + pulse) * (1 + _pump));
            _body.localPosition = Draw.W(_shakeX, 0);
            if (_fuse != null && (!_urgent || _t54))
                _fuse.sprite = (_urgent ? _urgentFrames : _frames)[Mathf.FloorToInt(seconds * 11) % 8];
            if (_halo != null)
            {
                float flicker = 1 + .06f * Mathf.Sin(seconds * 17) + .035f * Mathf.Sin(seconds * 29);
                _halo.color = new Color(1, 1, 1, .65f * flicker);
                _halo.transform.localScale = Vector3.one * (_width * .42f * flicker);
            }
        }

        public void Tick()
        {
            if (_exploding) return;
            Tweens.Run(this, .28f, t =>
            {
                if (this == null) return;
                _pump = (_urgent ? .08f : .03f) * Mathf.Sin(t * Mathf.PI);
                float seconds = t * .28f;
                _shakeX = _urgent ? _width * .03f * Mathf.Max(0, 1 - seconds / .2f) * Mathf.Sin(seconds * 100) : 0;
            }, () => { if (this != null) _pump = _shakeX = 0; });
        }

        void PositionFire()
        {
            if (_fuse.sprite == null) return;
            var sprite = _fuse.sprite;
            Vector2 uv = _t54 ? (_urgent ? SparkUrgent54 : SparkNormal54) : (_urgent ? SparkUrgent : SparkNormal);
            Vector2 spark = Vector2.Scale(sprite.rect.size, uv) - sprite.pivot;
            spark /= sprite.pixelsPerUnit;
            Vector3 position = Vector3.Scale(new Vector3(spark.x, spark.y, 0), _fireScale);
            // S*p + (1-S)*p = p: scale around the visible spark.
            _fuse.transform.localPosition = position * (1 - FireFactor);
            if (_halo != null) _halo.transform.localPosition = position;
        }

        void CreateHalo(int order)
        {
            if (_haloMaterial == null) _haloMaterial = Resources.Load<Material>("Materials/BombHalo");
            if (_haloMaterial == null) return;
            if (_haloSprite == null)
            {
                const int size = 64;
                _haloTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                { name = "Bomb warm halo", wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
                var pixels = new Color[size * size];
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float radius = new Vector2((x + .5f) * 2 / size - 1, (y + .5f) * 2 / size - 1).magnitude;
                        var color = Color.Lerp(new Color(1, .9f, .25f), new Color(1, .25f, .015f), Mathf.Clamp01(radius));
                        color.a = Mathf.Pow(Mathf.Max(0, 1 - radius), 2);
                        pixels[y * size + x] = color;
                    }
                _haloTexture.SetPixels(pixels);
                _haloTexture.Apply(false, true);
                _haloSprite = Sprite.Create(_haloTexture, new Rect(0, 0, size, size), Vector2.one * .5f, size,
                    0, SpriteMeshType.FullRect);
            }
            _halo = new GameObject("Bomb warm halo").AddComponent<SpriteRenderer>();
            _halo.transform.SetParent(_body, false);
            _halo.sprite = _haloSprite;
            _halo.sharedMaterial = _haloMaterial;
            // Same sorting order as the shell, with Transparent+1 material queue:
            // shell (3000) < halo (3001) < fire (next sorting order) < label/number.
            _halo.sortingOrder = order;
            _halo.transform.localScale = Vector3.one * (_width * .42f);
            _halo.color = new Color(1, 1, 1, .65f);
        }

        public void Revive(int fuse)
        {
            Tweens.Kill(this);
            _exploding = false;
            _pump = _shakeX = 0;
            _start = Time.unscaledTime;
            _body.localPosition = Vector3.zero;
            _body.localScale = Vector3.one;
            SetFuse(fuse);
        }

        public void Stop() { _exploding = true; Tweens.Kill(this); _pump = _shakeX = 0; _body.localPosition = Vector3.zero; }
        void OnDestroy() { Tweens.Kill(this); }
    }
}
