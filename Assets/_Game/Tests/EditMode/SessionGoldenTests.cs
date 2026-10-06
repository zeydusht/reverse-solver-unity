using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* M2 criteria 1a-4: GameSession agrees with the web game step by step.
       Scenarios come from Tools/make_session_golden.js, which runs the web
       game's own session code (flyOut, tickNails, releaseValve, tickBombs,
       finish, boosters, clock) under Node. Every command goes through the real
       GameSession API; nothing here bypasses a rule. */
    public class SessionGoldenTests
    {
        const string GoldenPath = "Assets/_Game/Tests/EditMode/Golden/session_golden.json";
        static JsonNode _golden;
        static JsonNode Golden => _golden ??= JsonReader.Parse(File.ReadAllText(GoldenPath));

        static IEnumerable<string> LevelScenarios => Golden.Get("levels").Items.Select(n => n.Get("id").AsString);
        static IEnumerable<string> SyntheticScenarios => Golden.Get("synthetic").Items.Select(n => n.Get("level").Get("id").AsString);
        static IEnumerable<string> WandScenarios =>
            Golden.Get("wand").Items.Select(n => $"{n.Get("id").AsString}/{n.Get("seed").Int()}");

        // ---- criterion 1a: nail-aware play on all 40 levels -----------------------

        [TestCaseSource(nameof(LevelScenarios))]
        public void LevelPlayMatchesWeb(string id)
        {
            var sc = Golden.Get("levels").Items.First(n => n.Get("id").AsString == id);
            Replay(TestData.Levels[id], new Mulberry32(1), sc.Get("steps"), id);
        }

        [Test]
        public void BombsGoOffWhereTheWebGameSaysTheyDo()
        {
            var bombed = Golden.Get("levels").Items
                .Where(n => n.Get("steps").Items.Last().Get("after").Get("res").AsString == "bomb")
                .Select(n => n.Get("id").AsString);
            Assert.That(bombed, Is.EqualTo(new[] { "L24", "L29", "L38" }));
        }

        // ---- criteria 2-5: move-order edge cases, valve, boosters, endings --------

        [TestCaseSource(nameof(SyntheticScenarios))]
        public void SyntheticScenarioMatchesWeb(string id)
        {
            var sc = Golden.Get("synthetic").Items.First(n => n.Get("level").Get("id").AsString == id);
            var level = LevelParser.Parse($"{{\"format\":1,\"levels\":[{sc.Get("levelJson").AsString}]}}").Levels[0];
            var seed = sc.Get("seed");
            IRandom rng = seed.Kind == JsonKind.String ? new ConstantRandom(0) : new Mulberry32((uint)seed.Int());
            TestContext.WriteLine(sc.Get("name").AsString);
            Replay(level, rng, sc.Get("steps"), id);
        }

        [TestCaseSource(nameof(WandScenarios))]
        public void WandOnRealLevelMatchesWeb(string key)
        {
            var sc = Golden.Get("wand").Items.First(n => $"{n.Get("id").AsString}/{n.Get("seed").Int()}" == key);
            Replay(TestData.Levels[sc.Get("id").AsString], new Mulberry32((uint)sc.Get("seed").Int()), sc.Get("steps"), key);
        }

        // ---- replay ---------------------------------------------------------------

        static void Replay(LevelData level, IRandom rng, JsonNode steps, string name)
        {
            var s = new GameSession(level, 1, "test", rng);
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var cmd = step.Get("cmd");
                int valvesBefore = s.ValveCount;
                string at = $"{name} step {i} [{string.Join(" ", cmd.Items.Select(Show))}]";

                switch (cmd[0].AsString)
                {
                    case "exit":
                        DirExt.TryParse(cmd[2].AsString, out var dir);
                        Assert.That(s.TryExit(cmd[1].Int(), dir), Is.EqualTo(CommandResult.Ok), at);
                        break;
                    case "hammer":
                        Assert.That(s.UseHammer(cmd[1].Int()), Is.EqualTo(CommandResult.Ok), at);
                        break;
                    case "scissors":
                        Assert.That(s.UseScissors(cmd[2].Int(), cmd[3].Int(), cmd[1].AsString == "v"), Is.EqualTo(CommandResult.Ok), at);
                        break;
                    case "wand": s.UseWand(); break;          // success or not is checked through stock/used/joints
                    case "clock":
                        Assert.That(s.UseClock(), Is.EqualTo(CommandResult.Ok), at);
                        break;
                    case "tick":
                        for (int t = 0; t < cmd[1].Int(); t++) s.Tick(1.0);
                        break;
                    default: Assert.Fail($"{at}: unknown command"); break;
                }
                AssertState(s, step.Get("after"), s.ValveCount > valvesBefore, at);
            }
        }

        static void AssertState(GameSession s, JsonNode w, bool valveFired, string at)
        {
            Assert.That(s.Board.PresentCount, Is.EqualTo(w.Get("present").Int()), at + " present");
            Assert.That(s.Moves, Is.EqualTo(w.Get("moves").Int()), at + " moves");
            Assert.That(s.TimeLeft, Is.EqualTo(w.Get("time").Int()), at + " time");
            Assert.That(valveFired, Is.EqualTo(w.Get("valve").AsBool), at + " valve");
            Assert.That(Counters(s.Nails), Is.EqualTo(Counters(w.Get("nails"))), at + " nails");
            Assert.That(Counters(s.Bombs), Is.EqualTo(Counters(w.Get("bombs"))), at + " bombs");
            Assert.That(BoosterExt.All.Select(s.Stock), Is.EqualTo(w.Get("stock").Items.Select(n => n.Int())), at + " stock");
            Assert.That(BoosterExt.All.Select(s.Used), Is.EqualTo(w.Get("used").Items.Select(n => n.Int())), at + " used");
            Assert.That(s.Outcome.Id(), Is.EqualTo(w.Get("res").AsString), at + " result");
            Assert.That(s.Stars, Is.EqualTo(w.Get("stars").Int()), at + " stars");

            var l = s.Level;
            var v = w.Get("V");
            for (int y = 0; y < l.Height; y++)
                for (int x = 0; x <= l.Width; x++)
                    Assert.That(s.Board.VJoint(x, y), Is.EqualTo(v[y][x].Int()), $"{at} V[{y}][{x}]");
            var h = w.Get("H");
            for (int y = 0; y <= l.Height; y++)
                for (int x = 0; x < l.Width; x++)
                    Assert.That(s.Board.HJoint(x, y), Is.EqualTo(h[y][x].Int()), $"{at} H[{y}][{x}]");
        }

        static string Counters(IReadOnlyDictionary<int, int> d) =>
            string.Join(",", d.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

        static string Counters(JsonNode o) =>
            string.Join(",", o.Members.Select(m => (int.Parse(m.Key), m.Value.Int())).OrderBy(t => t.Item1)
                              .Select(t => $"{t.Item1}:{t.Item2}"));

        static string Show(JsonNode n) => n.Kind == JsonKind.String ? n.AsString : n.AsNumber.ToString();
    }

    /* Always returns the same value; with 0 every wand draw picks +1. */
    sealed class ConstantRandom : IRandom
    {
        readonly double _v;
        public ConstantRandom(double v) { _v = v; }
        public double NextDouble() => _v;
    }
}
