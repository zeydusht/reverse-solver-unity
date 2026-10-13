using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* The 40-slot order of the redesigned set (PM 2026-10-13 ~18:40, PRODUCT.md
       "Sıra planı"). T Tanıtım, N Nefes, K Kolay, O Orta, Z Zor, ÇZ Çok zor;
       the new thing in brackets.
         1T 2K 3T(çivi) 4K 5T(makas) 6O 7T(mühür) 8K 9T(değnek) 10Z
         11N 12T(zincir) 13O 14Z 15T(saat) 16O 17Z 18N 19Z 20T(bomba)
         21K 22Z 23T(çekiç) 24O 25Z 26N 27O 28Z 29N 30ÇZ
         31N 32Z 33O 34ÇZ 35N 36Z 37O 38Z 39N 40ÇZ

       The approved plan lives in Assets/_Game/Levels/fabrika_plan.json (the PM
       approves it; the factory reads it). Default() writes its first version. */
    public static class FactoryPlan
    {
        public const string PlanPath = "Assets/_Game/Levels/fabrika_plan.json";
        public const int Slots = 40;

        static readonly (Genre g, string thing)[] Order =
        {
            (Genre.Tanitim, null), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Nail), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Scissors),
            (Genre.Orta, null), (Genre.Tanitim, SlotSpec.Seal), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Wand), (Genre.Zor, null),
            (Genre.Nefes, null), (Genre.Tanitim, SlotSpec.Chain), (Genre.Orta, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Clock),
            (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Bomb),
            (Genre.Kolay, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Hammer), (Genre.Orta, null), (Genre.Zor, null),
            (Genre.Nefes, null), (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.CokZor, null),
            (Genre.Nefes, null), (Genre.Zor, null), (Genre.Orta, null), (Genre.CokZor, null), (Genre.Nefes, null),
            (Genre.Zor, null), (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.CokZor, null),
        };

        /* Chapter ends that are knowingly not their chapter's peak, with the reason. */
        public static readonly IReadOnlyDictionary<int, string> PeakExceptions = new Dictionary<int, string>
        {
            [20] = "bomba tanıtımı: tanıtımda kaybetmek hayal kırıklığı sayıldığı için bölüm 2'nin zirvesi 19 (PM 2026-10-13 ~18:40)"
        };

        public static SlotSpec Slot(int slot) => SlotSpec.For(slot, Order[slot - 1].g, Order[slot - 1].thing);

        public static List<SlotSpec> Default()
        {
            var list = new List<SlotSpec>(Slots);
            for (int i = 1; i <= Slots; i++) list.Add(Slot(i));
            return list;
        }

        /* The approved plan when the file exists, otherwise the default. */
        public static List<SlotSpec> All() => File.Exists(PlanPath) ? Parse(File.ReadAllText(PlanPath)) : Default();

        // ---- file ---------------------------------------------------------------------------

        /* One slot per line, so a reviewer can read the diff. */
        public static string ToJson(IEnumerable<SlotSpec> plan)
        {
            var lines = new List<string>();
            foreach (var s in plan)
            {
                var w = new JsonWriter();
                w.BeginObject().Field("slot", s.Slot).Field("genre", s.Genre.Id()).Field("new", s.NewThing);
                w.Key("boards").BeginArray();
                foreach (var (bw, bh) in s.Boards) w.BeginArray().Value(bw).Value(bh).EndArray();
                w.EndArray();
                Range(w, "bodies", s.MinBodies, s.MaxBodies);
                Range(w, "nails", s.MinNails, s.MaxNails);
                Range(w, "walls", s.MinSeals, s.MaxSeals);
                Range(w, "chains", s.MinChains, s.MaxChains);
                Range(w, "remoteChains", s.MinRemote, s.MaxRemote);
                Range(w, "bombs", s.MinBombs, s.MaxBombs);
                w.EndObject();
                lines.Add("    " + w);
            }
            return "{\n  \"format\": 1,\n  \"note\": \"Faz 4 sıra planı (PRODUCT.md 'Sıra planı'); slot başına tür, yeni şey ve üreticinin sınırları. PM onaylar.\",\n" +
                   "  \"slots\": [\n" + string.Join(",\n", lines) + "\n  ]\n}\n";
        }

        static void Range(JsonWriter w, string key, int lo, int hi) => w.Key(key).BeginArray().Value(lo).Value(hi).EndArray();

        public static List<SlotSpec> Parse(string json)
        {
            var root = JsonReader.Parse(json);
            if (!root.TryGet("slots", out var slots)) throw new FormatException("fabrika_plan: no slots");
            var list = new List<SlotSpec>();
            foreach (var n in slots.Items)
            {
                if (!GenreExt.TryParse(Str(n, "genre"), out var genre)) throw new FormatException("fabrika_plan: bad genre");
                var s = new SlotSpec { Slot = Int(n, "slot"), Genre = genre, NewThing = Str(n, "new") };
                n.TryGet("boards", out var boards);
                s.Boards = boards.Items.Select(b => ((int)b[0].AsNumber, (int)b[1].AsNumber)).ToArray();
                (s.MinBodies, s.MaxBodies) = Pair(n, "bodies");
                (s.MinNails, s.MaxNails) = Pair(n, "nails");
                (s.MinSeals, s.MaxSeals) = Pair(n, "walls");
                (s.MinChains, s.MaxChains) = Pair(n, "chains");
                (s.MinRemote, s.MaxRemote) = Pair(n, "remoteChains");
                (s.MinBombs, s.MaxBombs) = Pair(n, "bombs");
                list.Add(s);
            }
            return list;
        }

        static string Str(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.String ? v.AsString : null;
        static int Int(JsonNode n, string k) => n.TryGet(k, out var v) ? (int)v.AsNumber : 0;
        static (int, int) Pair(JsonNode n, string k) => n.TryGet(k, out var v) && v.Count == 2 ? ((int)v[0].AsNumber, (int)v[1].AsNumber) : (0, 0);

        // ---- order rules ----------------------------------------------------------------------

        /* 0 Tanıtım … 4 Çok zor: how hard a slot is meant to feel. */
        public static int Rank(Genre g) => g switch
        {
            Genre.Tanitim => 0, Genre.Nefes => 1, Genre.Kolay => 1, Genre.Orta => 2, Genre.Zor => 3, _ => 4
        };

        static bool Hard(Genre g) => g == Genre.Zor || g == Genre.CokZor;

        /* The plan's order rules (FAZ4 "Sıra planı"); empty when the plan follows them. */
        public static List<string> Problems(IReadOnlyList<SlotSpec> plan)
        {
            var p = new List<string>();
            if (plan.Count != Slots) p.Add($"{plan.Count} slot var, {Slots} olmalı");
            for (int i = 0; i < plan.Count; i++)
            {
                var s = plan[i];
                if (s.Slot != i + 1) p.Add($"{i + 1}. sırada slot {s.Slot}");
                if (i > 0 && Hard(s.Genre) && Hard(plan[i - 1].Genre)) p.Add($"slot {s.Slot}: iki zor seviye arka arkaya");
                if (i > 0 && plan[i - 1].Genre == Genre.CokZor && s.Genre != Genre.Nefes) p.Add($"slot {s.Slot}: çok zordan sonra nefes gelmeli");
                // after an obstacle introduction (and the first level) the player practises, so no hard slot;
                // booster introductions do not change the board and are exempt (PM 2026-10-13 ~19:10)
                if (i > 0 && Hard(s.Genre) && plan[i - 1].Genre == Genre.Tanitim
                    && (plan[i - 1].NewThing == null || SlotSpec.IsObstacle(plan[i - 1].NewThing)))
                    p.Add($"slot {s.Slot}: engel tanıtımından sonra zor seviye");
                if (s.NewThing != null)
                {
                    if (!SlotSpec.IntroducedAt.TryGetValue(s.NewThing, out int at) || at != s.Slot) p.Add($"slot {s.Slot}: {s.NewThing} takvimde slot {at}");
                    if (s.Genre != Genre.Tanitim) p.Add($"slot {s.Slot}: yeni şey tanıtım seviyesinde gelmeli");
                }
                foreach (var kv in SlotSpec.IntroducedAt)
                    if (kv.Value == s.Slot && s.NewThing != kv.Key) p.Add($"slot {s.Slot}: takvimdeki {kv.Key} yok");
                if (s.MaxNails > 0 && !s.Allows(SlotSpec.Nail)) p.Add($"slot {s.Slot}: çivi slot 3'ten önce");
                if (s.MaxSeals > 0 && !s.Allows(SlotSpec.Seal)) p.Add($"slot {s.Slot}: mühür slot 7'den önce");
                if (s.MaxChains > 0 && !s.Allows(SlotSpec.Chain)) p.Add($"slot {s.Slot}: zincir slot 12'den önce");
                if (s.MinChains < s.MinRemote || s.MaxRemote > s.MaxChains) p.Add($"slot {s.Slot}: uzak zincir aralığı zincir aralığının dışında");
                if (s.MaxBombs > 0 && !s.Allows(SlotSpec.Bomb)) p.Add($"slot {s.Slot}: bomba slot 20'den önce");
                if (s.Genre == Genre.Tanitim && s.NewThing != null && SlotSpec.IsObstacle(s.NewThing))
                {
                    int others = (s.NewThing != SlotSpec.Nail ? s.MaxNails : 0) + (s.NewThing != SlotSpec.Seal ? s.MaxSeals : 0)
                               + (s.NewThing != SlotSpec.Chain ? s.MaxChains : 0) + (s.NewThing != SlotSpec.Bomb ? s.MaxBombs : 0);
                    if (others > 0) p.Add($"slot {s.Slot}: tanıtımda başka engel var (tek yenilik)");
                }
            }
            for (int end = 10; end <= plan.Count; end += 10)
            {
                var chapter = plan.Skip(end - 10).Take(10).ToList();
                int peak = chapter.Max(s => Rank(s.Genre));
                if (Rank(plan[end - 1].Genre) < peak && !PeakExceptions.ContainsKey(end)) p.Add($"slot {end}: bölüm sonu zirve değil");
            }
            return p;
        }
    }
}
