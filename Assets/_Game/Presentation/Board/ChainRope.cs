using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class ChainRope : MonoBehaviour
    {
        readonly List<Transform> _links = new List<Transform>();
        Vector2 _a, _b;
        float _sag, _cell;
        Color _tint;
        int _order;

        public void Init(Vector2 a, Vector2 b, float cell, int colour, int order, ISet<Cell> occupied)
        {
            _a = a; _b = b; _cell = cell; _order = order;
            _sag = ChainCurve.FitSag(a, b, ChainCurve.Sag(a, b, cell), cell, occupied);
            string[] names = { "altin", "turkuaz", "mor", "mercan", "yesil" };
            _tint = Palette.C(Draw.Hex(colour == 4 ? "#7fd67a" : ChainView.Tints[colour]));
            foreach (var p in new[] { a, b })
            {
                var anchor = Art.Place(transform, (Art.T54 ? "obstacles_v3/chain/" : "") + "chain-anchor-" + names[colour], Vector2.one * cell * .63f, order + 2);
                if (anchor != null) anchor.transform.localPosition = Draw.W(p);
            }
            float spacing = Art.T54 ? (Vector2.Distance(a, b) <= 2 * cell ? .30f : .434f * .86f)
                : (Vector2.Distance(a, b) <= 2 * cell ? .28f : .22f * 1.6f);
            int count = ChainCurve.Samples(a, b, _sag, cell * spacing).Length;
            for (int i = 0; i < count; i++)
            {
                float height = cell * .217f * (Art.T54 ? 1 : 1.6f);
                var link = Art.Place(transform, Art.T54 ? "obstacles_v3/chain/chain-link-" + names[colour] : "chain-link",
                    new Vector2(height * 128 / (Art.T54 ? 64 : 96), height), order);
                if (link == null) continue;
                if (!Art.T54) link.color = _tint;
                _links.Add(link.transform);
            }
            Layout(_sag);
        }

        void Layout(float sag)
        {
            // Keep the link count stable throughout a kick.
            float arc = 0;
            var previous = _a;
            for (int i = 1; i <= 512; i++)
            {
                var p = ChainCurve.Point(_a, _b, sag, i / 512f);
                arc += Vector2.Distance(previous, p);
                previous = p;
            }
            var samples = ChainCurve.Samples(_a, _b, sag, arc / (_links.Count + 1));
            for (int i = 0; i < _links.Count && i < samples.Length; i++)
            {
                _links[i].localPosition = Draw.W(samples[i].Position);
                _links[i].localRotation = Quaternion.Euler(0, 0, -samples[i].Angle);
            }
        }

        /* Moves links and anchors together, keeping the anchors above the links. */
        public void SetOrder(int order)
        {
            int delta = order - _order;
            if (delta == 0) return;
            foreach (var part in GetComponentsInChildren<SpriteRenderer>(true)) part.sortingOrder += delta;
            _order = order;
        }

        public void Kick()
        {
            Tweens.Run(this, .4f, t =>
            {
                if (this == null) return;
                float seconds = t * .4f;
                Layout(_sag * (1 + .35f * Mathf.Exp(-seconds / .14f) * Mathf.Cos(2 * Mathf.PI * seconds / .16f)));
            }, () => { if (this != null) Layout(_sag); });
        }

        public void Break(Transform board)
        {
            Tweens.Kill(this);
            foreach (Transform child in transform) child.gameObject.SetActive(false);
            var effect = new GameObject("ChainBreak").transform;
            effect.SetParent(board, false);
            effect.position = transform.TransformPoint(Draw.W(ChainCurve.Point(_a, _b, _sag, .5f)));
            effect.localRotation = Quaternion.Euler(0, 0, -ChainCurve.Angle(_a, _b, _sag, .5f));
            var sprite = Art.Place(effect, "chain-break-01", new Vector2(_cell * .43f * 256 / 160, _cell * .43f), _order + 3);
            if (sprite == null) { Destroy(effect.gameObject); return; }
            sprite.color = _tint;
            Tweens.Run(effect, 4f / 14, t =>
            {
                if (sprite != null) sprite.sprite = Art.Get("chain-break-" + (Mathf.Min(3, Mathf.FloorToInt(t * 4)) + 1).ToString("00"));
            }, () => { if (effect != null) Destroy(effect.gameObject); });
        }

        void OnDestroy() { Tweens.Kill(this); }
    }
}
