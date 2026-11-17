"""PM_82: the sl100 set from the design tool's picks (Builds/sl100_picks.json, Tools/sl100/Design.cs).

    dotnet run -c Release --project Tools/sl100 -- design 10      (the picks; ~2 min)
    python -I Tools/sl100/finalize.py                              (this: the set, the report, the curve)

Writes Assets/_Game/Levels/sl100.json (levels.json format + role / lesson / placeholder), docs/analiz/v2/pred_sl100.csv,
docs/analiz/PM82_100_BOLUM.md and docs/analiz/pm82_egri.png.
Ids: a p22 board keeps its id (its pt1 data); its version goes up only when its play changed (timer, an obstacle taken
away). Every other board gets a new permanent id SL<slot> (never reused). Paintings painting-NN by slot. Booster
unlocks (PM_82): scissors 6, hammer 13, clock 18, wand 25 (the intro card on that level); obstacle lessons 3 lock,
7 chain, 11 bomb, 21 wall (p22's lessons with their cards)."""
import json, os, sys
from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
P = lambda *a: os.path.join(ROOT, *a)
UNLOCK = {"scissors": 6, "hammer": 13, "clock": 18, "wand": 25}
BINTRO_FROM = {"scissors": "S44", "hammer": "S27", "clock": "S13", "wand": "S20"}
LESSON = {3: "lock", 7: "chain", 11: "bomb", 21: "wall"}
ROLE = {"Guide": "guide", "Learn": "learn", "Lesson": "lesson", "Normal": "normal", "Combo": "combo", "Hard": "hard", "VeryHard": "veryhard", "Breather": "breather"}
TR = {"guide": "rehber", "learn": "öğrenme", "lesson": "ders", "normal": "normal", "combo": "birleşim", "hard": "zor", "veryhard": "çok zor", "breather": "nefes"}


def kinds(l):
    t = []
    if l.get("nails"): t.append("kilit")
    if any(len(p) > 1 for p in l["pieces"]): t.append("zincir")
    if l.get("bombs"): t.append("bomba")
    if sum(len(v) for v in (l.get("sealed") or {}).values()): t.append("duvar")
    return t


def main():
    picks = json.load(open(P("Builds", "sl100_picks.json"), encoding="utf8"))["levels"]
    redesign = {l["id"]: l for l in json.load(open(P("Assets", "_Game", "Levels", "redesign.json"), encoding="utf8"))["levels"]}
    bintro = {b: redesign[i]["bintro"] for b, i in BINTRO_FROM.items()}
    unlock_at = {v: k for k, v in UNLOCK.items()}
    out, rows = [], []
    for l in sorted(picks, key=lambda x: x["_slot"]):
        slot, role = l["_slot"], ROLE[l["_role"]]
        src, played = l["_source"], l["_played"]
        stripped = "−" in src
        if played and not stripped and l["timer"] == l["_orig_timer"]:
            l["version"] = l["_orig_version"]; origin = "eski (p22 · aynen)"
        elif played:
            l["version"] = l["_orig_version"] + 1; origin = "eski (p22 · ayarlı)"
        else:
            l["id"] = f"SL{slot:03d}"; l["version"] = 1
            origin = "yeni (üretici)" if src.startswith("gen") else f"yeni (havuz: {src.split()[0]})"
        l["lv"] = slot
        l["image"] = f"painting-{slot:02d}"
        l["role"] = role
        l["hard"] = role in ("hard", "veryhard")
        l["unlock"] = dict(UNLOCK)
        l["bintro"] = bintro[unlock_at[slot]] if slot in unlock_at else None
        if slot in LESSON:
            l["lesson"] = LESSON[slot]
        else:
            l["intro"] = None
        l["tutorial"] = 1 if slot == 1 else 0
        meta = {k: l.pop(k) for k in [k for k in l if k.startswith("_")]}
        out.append(l)
        rows.append(dict(slot=slot, role=role, id=l["id"], version=l["version"], kinds=kinds(l), w=l["w"], h=l["h"], pieces=len(l["pieces"]),
                         timer=l["timer"], p=meta["_p"], origin=origin, source=src, cands=meta["_candidates"]))
    ids = [l["id"] for l in out]
    assert len(set(ids)) == len(ids), "duplicate ids"
    with open(P("Assets", "_Game", "Levels", "sl100.json"), "w", encoding="utf8") as f:
        f.write('{"format":1,"levels":[\n' + ",\n".join(json.dumps(l, ensure_ascii=False, separators=(",", ":")) for l in out) + "\n]}\n")

    os.makedirs(P("docs", "analiz", "v2"), exist_ok=True)
    with open(P("docs", "analiz", "v2", "pred_sl100.csv"), "w", encoding="utf8") as f:
        f.write("slot,role,level_id,level_version,kinds,w,h,pieces,timer_s,p_win1_v2,origin,candidates\n")
        for r in rows:
            f.write(f'{r["slot"]},{r["role"]},{r["id"]},{r["version"]},{"+".join(r["kinds"]) or "-"},{r["w"]},{r["h"]},{r["pieces"]},{r["timer"]},{r["p"]:.3f},{r["origin"]},{r["cands"]}\n')
    curve(rows)
    report(rows)
    print(f"sl100: {len(out)} levels, squares {sum(1 for r in rows if r['w'] == r['h'])}, p22 {sum(1 for r in rows if r['origin'].startswith('eski'))}")


def band(slot, role):
    k = min(max((slot - 20) / 80, 0), 1)
    lo, hi = .75 - .10 * k, .85 - .10 * k
    return {"guide": (.90, 1), "lesson": (.90, 1), "learn": (.90, 1), "hard": (.35, .50), "veryhard": (.20, .30), "breather": (.85, 1), "combo": (hi - .02, 1)}.get(role, (lo, hi))


def curve(rows):
    W, H, L, R, T, B = 1200, 480, 60, 20, 70, 50
    im = Image.new("RGB", (W, H), "white"); d = ImageDraw.Draw(im)
    try: font = ImageFont.truetype("arial.ttf", 13); big = ImageFont.truetype("arial.ttf", 17)
    except OSError: font = big = ImageFont.load_default()
    X = lambda s: L + (W - L - R) * (s - .5) / 100
    Y = lambda p: H - B - (H - T - B) * p
    col = {"guide": (120, 120, 120), "learn": (90, 170, 220), "lesson": (60, 120, 200), "normal": (40, 160, 90), "combo": (150, 90, 200),
           "hard": (230, 120, 40), "veryhard": (200, 40, 40), "breather": (120, 200, 120)}
    for r in rows:
        lo, hi = band(r["slot"], r["role"])
        d.rectangle((X(r["slot"] - .45), Y(min(hi, 1)), X(r["slot"] + .45), Y(lo)), fill=(236, 240, 246))
    for p in (0, .2, .4, .6, .8, 1):
        d.line((L, Y(p), W - R, Y(p)), fill=(220, 220, 220)); d.text((10, Y(p) - 7), f"{p:.1f}", fill=(0, 0, 0), font=font)
    for s in range(10, 101, 10):
        d.text((X(s) - 8, H - B + 6), str(s), fill=(0, 0, 0), font=font)
    pts = [(X(r["slot"]), Y(r["p"])) for r in rows]
    d.line(pts, fill=(170, 170, 170), width=1)
    for r in rows:
        x, y = X(r["slot"]), Y(r["p"])
        d.ellipse((x - 4, y - 4, x + 4, y + 4), fill=col[r["role"]])
    d.text((L, 12), "sl100: ilk deneme kazanma tahmini (model v2, ortalama oyuncu) — gri kutular hedef bant", fill=(0, 0, 0), font=big)
    x = L
    for k in ("learn", "lesson", "normal", "combo", "hard", "veryhard", "breather"):
        d.ellipse((x, 40, x + 9, 49), fill=col[k]); d.text((x + 13, 37), TR[k], fill=(0, 0, 0), font=font); x += 90
    im.save(P("docs", "analiz", "pm82_egri.png"))


def report(rows):
    L = ["# PM_82: İlk 100 bölüm (sl100)", "",
         "Model v2 (ortalama oyuncu, ilk deneme kazanma) ile hedefe göre seçildi: `Tools/sl100/Design.cs` (aday havuzu + üretici + süre ayarı), `Tools/sl100/finalize.py` (set, rapor, grafik).",
         "Eğri: `docs/analiz/pm82_egri.png`. Tahminler: `docs/analiz/v2/pred_sl100.csv`.", "",
         "| Bölüm | Tip | Engeller | Tahta | Parça | Süre | İlk deneme | Kaynak | Aday |", "|---|---|---|---|---|---|---|---|---|"]
    for r in rows:
        L.append(f'| {r["slot"]} | {TR[r["role"]]} | {", ".join(r["kinds"]) or "—"} | {r["w"]}×{r["h"]} | {r["pieces"]} | {r["timer"]} sn | %{r["p"] * 100:.0f} | {r["origin"]} | {r["cands"]} |')
    open(P("docs", "analiz", "PM82_100_BOLUM.md"), "w", encoding="utf8").write("\n".join(L) + "\n")


if __name__ == "__main__":
    main()
