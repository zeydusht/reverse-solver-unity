using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class ChainRope : MonoBehaviour
    {
        readonly List<SpriteRenderer> _links = new List<SpriteRenderer>();
        readonly Transform[] _anchors = new Transform[2];
        Transform _piece;
        Vector3 _a, _b;
        RopeSim _sim;
        float _cell, _brokenAt = -1;
        int _order;

        public void Init(Vector2 a, Vector2 b, float cell, int colour, int order, Vector2 min, Vector2 max)
        {
            _piece = transform.parent; _a = Draw.W(a); _b = Draw.W(b);
            _cell = cell; _order = order;
            Vector2 start = _piece.TransformPoint(_a), end = _piece.TransformPoint(_b);
            _sim = new RopeSim(start, end, cell, min, max);
            string[] names = { "altin", "turkuaz", "mor", "mercan", "yesil" };
            var tint = Palette.C(Draw.Hex(colour == 4 ? "#7fd67a" : ChainView.Tints[colour]));
            for (int i = 0; i < 2; i++)
            {
                var anchor = Art.Place(transform, (Art.T54 ? "obstacles_v3/chain/" : "") + "chain-anchor-" + names[colour],
                    Vector2.one * cell * .63f * ChainTuning.AnchorScale, order + 2);
                if (anchor != null) _anchors[i] = anchor.transform;
            }
            float spacing = Art.T54 ? (Vector2.Distance(a, b) <= 2 * cell ? .30f : .434f * .86f)
                : (Vector2.Distance(a, b) <= 2 * cell ? .28f : .22f * 1.6f);
            int count = Mathf.Max(4, Mathf.RoundToInt(_sim.Length / (cell * spacing)));
            for (int i = 0; i < count; i++)
            {
                float height = cell * .217f * (Art.T54 ? 1 : 1.6f) * ChainTuning.LinkScale;
                var link = Art.Place(transform, Art.T54 ? "obstacles_v3/chain/chain-link-" + names[colour] : "chain-link",
                    new Vector2(height * 128 / (Art.T54 ? 64 : 96), height), order);
                if (link == null) continue;
                if (!Art.T54) link.color = tint;
                _links.Add(link);
            }
            Layout();
        }

        void LateUpdate()
        {
            if (_sim == null) return;
            bool broken = _brokenAt >= 0;
            float age = broken ? Time.unscaledTime - _brokenAt : 0;
            if (age >= ChainTuning.BreakLife) { Destroy(gameObject); return; }
            if (broken && age >= ChainTuning.Hang) _sim.Release();
            Vector2 a = broken ? _sim[0] : (Vector2)_piece.TransformPoint(_a);
            Vector2 b = broken ? _sim[_sim.Count - 1] : (Vector2)_piece.TransformPoint(_b);
            _sim.Advance(Time.unscaledDeltaTime, a, b);
            Layout();
            if (!broken) return;
            float alpha = 1 - Mathf.InverseLerp(ChainTuning.Hang, ChainTuning.BreakLife, age);
            for (int i = 0; i < _links.Count; i++)
            {
                var c = _links[i].color; c.a = alpha; _links[i].color = c;
            }
            for (int i = 0; i < 2; i++)
                if (_anchors[i] != null)
                {
                    var r = _anchors[i].GetComponent<SpriteRenderer>();
                    var c = r.color; c.a = alpha; r.color = c;
                }
        }

        void Layout()
        {
            bool broken = _brokenAt >= 0;
            for (int i = 0; i < _links.Count; i++)
            {
                float node;
                if (broken)
                {
                    int half = _links.Count / 2, cut = (_sim.Count - 1) / 2;
                    node = i < half ? (i + .5f) / half * cut
                        : cut + 1 + (i - half + .5f) / (_links.Count - half) * (_sim.Count - cut - 2);
                }
                else node = (i + .5f) / _links.Count * (_sim.Count - 1);
                int index = Mathf.Min(_sim.Count - 2, Mathf.FloorToInt(node));
                Vector2 direction = _sim[index + 1] - _sim[index];
                var link = _links[i].transform;
                link.position = Vector2.Lerp(_sim[index], _sim[index + 1], node - index);
                link.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
            for (int i = 0; i < 2; i++)
                if (_anchors[i] != null)
                {
                    _anchors[i].position = _sim[i == 0 ? 0 : _sim.Count - 1];
                    _anchors[i].rotation = Quaternion.identity;
                }
        }

        public void SetOrder(int order)
        {
            int delta = order - _order;
            if (delta == 0) return;
            for (int i = 0; i < _links.Count; i++) _links[i].sortingOrder += delta;
            for (int i = 0; i < 2; i++)
                if (_anchors[i] != null) _anchors[i].GetComponent<SpriteRenderer>().sortingOrder += delta;
            _order = order;
        }

        public void Kick()
        {
            if (_brokenAt >= 0) return;
            Vector2 d = _sim[_sim.Count - 1] - _sim[0];
            _sim.Kick(new Vector2(-d.y, d.x).normalized * (_cell * ChainTuning.Kick));
        }

        public void Break(Transform board)
        {
            if (_brokenAt >= 0) return;
            transform.SetParent(board, true);
            _brokenAt = Time.unscaledTime;
            _sim.Split();
            _sim.Kick(Vector2.down * (_cell * ChainTuning.Kick));
        }
    }
}
