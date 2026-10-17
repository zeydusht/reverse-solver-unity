"""Writes the factory agents' input (Faz 4 agent rounds): one short markdown file per
group of slots, from redesign.json and fabrika_plan.json, so an agent reads a few KB
instead of the whole report.

    python Tools/agent_packs.py [--group 10] [--out Builds/fabrika/ajan]

Each finalist on one block: id, board, numbers (fun with and without boosters, first try,
early time-out, near-miss, curve fit and peak, reading, k0, timer), the curve sparkline
and the notes. Numbers are the fun model's estimate, not real players.
"""
import argparse
import json
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
GENRE = {"tanitim": "Tanıtım", "nefes": "Nefes", "kolay": "Kolay", "orta": "Orta", "zor": "Zor", "cokzor": "Çok zor"}


def pct(v):
    return "—" if v is None else f"%{v * 100:.0f}"


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--group", type=int, default=10)
    ap.add_argument("--out", default=os.path.join("Builds", "fabrika", "ajan"))
    a = ap.parse_args()
    levels = json.load(open(os.path.join(ROOT, "Assets/_Game/Levels/redesign.json"), encoding="utf-8"))["levels"]
    plan = {s["slot"]: s for s in json.load(open(os.path.join(ROOT, "Assets/_Game/Levels/fabrika_plan.json"), encoding="utf-8"))["slots"]}
    out = os.path.join(ROOT, a.out)
    os.makedirs(out, exist_ok=True)
    for first in range(1, 41, a.group):
        slots = range(first, min(40, first + a.group - 1) + 1)
        lines = [f"# Slot {slots[0]}–{slots[-1]}: finalistler (V5, 2026-10-17)", "",
                 "Keyif ve hk booster'lı (V5), ilk deneme / erken süre / near-miss booster'sız. Sayılar modelin tahmini.", ""]
        for s in slots:
            p = plan[s]
            rng = lambda k: f"{p[k][0]}–{p[k][1]}"
            lines.append(f"## Slot {s} — {GENRE[p['genre']]}, yeni: {p['new'] or 'yok'} · gövde {rng('bodies')}, çivi {rng('nails')}, "
                         f"mühür {rng('walls')}, zincir {rng('chains')}, bomba {rng('bombs')}")
            fs = [l for l in levels if l["fabrika"]["slot"] == s and l["fabrika"]["status"] != "ret"]
            if not fs:
                lines.append("- finalist yok")
            for l in fs:
                f, sc = l["fabrika"], l["fabrika"]["scores"]
                g = sc.get
                lines.append(
                    f"- **{l['id']}** {l['w']}×{l['h']}, {len(l['pieces'])} parça, çivi {len(l['nails'])}, bomba {len(l['bombs'])}, süre {l['timer']} sn · "
                    f"keyif {pct(g('keyif'))} (booster'sız {pct(g('keyifSiz'))}), hk {pct(g('hayalKirikligi'))} (haksız {pct(g('haksiz'))}), "
                    f"ilk {pct(g('ilkDeneme'))}, 3 dn {pct(g('ucDeneme'))}, erken {pct(g('erkenSure'))}, near-miss {pct(g('nearMiss'))}, "
                    f"çekiç {pct(g('cekic'))}, saat {pct(g('saat'))} · okuma {g('okuma')}, k₀ {g('k0')}, açık %{g('acikDikis')}, darboğaz {g('darbogaz')}, "
                    f"uyum {g('uyum', '—')}, zirve {pct(g('zirve'))} · {f['curve'] or ''} · düğmeler: {f['params']}"
                    + (f" · notlar: {'; '.join(f['notes'])}" if f["notes"] else ""))
            lines.append("")
        path = os.path.join(out, f"slot_{slots[0]:02d}-{slots[-1]:02d}.md")
        open(path, "w", encoding="utf-8", newline="\n").write("\n".join(lines))
        print(path, os.path.getsize(path))


if __name__ == "__main__":
    main()
