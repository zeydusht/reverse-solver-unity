using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using SetKind = ReverseSolver.Presentation.SetChoice.Kind;

namespace ReverseSolver.Core.Tests
{
    /* Faz 4 G8: the redesigned set and the 👍/👎 question stay behind flags that are off. */
    public class RedesignFlagTests
    {
        [Test]
        public void FlagsAreOff()
        {
            Assert.That(SetFlags.RedesignDefault, Is.False, "the new set goes live only on Zeyd's command");
            Assert.That(SetFlags.AskFun, Is.False, "the 👍/👎 question waits for 004_keyif.sql and Zeyd's approval");
        }

        [Test]
        public void SetChoiceKeepsTheLiveFortyByDefault()
        {
            Assert.That(SetChoice.For(false, null, true, true, false), Is.EqualTo(SetKind.Levels), "players get the live 40");
            Assert.That(SetChoice.For(false, "redesign", true, true, false), Is.EqualTo(SetKind.Levels), "set= only works with debug");
            Assert.That(SetChoice.For(true, "redesign", true, true, false), Is.EqualTo(SetKind.Redesign));
            Assert.That(SetChoice.For(true, "redesign", true, false, false), Is.EqualTo(SetKind.Levels), "an empty redesign.json falls back");
            Assert.That(SetChoice.For(true, "designs", true, true, false), Is.EqualTo(SetKind.Designs));
            Assert.That(SetChoice.For(false, null, true, true, true), Is.EqualTo(SetKind.Redesign), "only when the flag is turned on");
            Assert.That(SetChoice.For(true, "levels", true, true, true), Is.EqualTo(SetKind.Levels), "debug can still open the old set");
        }

        [Test]
        public void FunAnswerOnlyInTheRowWhenGiven()
        {
            var r = new AttemptRecord { LevelId = "R01", LevelVersion = 1, Result = "win", Client = "unity-web" };
            string plain = PlayRow.Json(r, "TEST_X", "inst", "row1");
            Assert.That(plain, Does.Not.Contain("keyif"), "rows stay as they are while the question is off");
            r.Fun = 1;
            Assert.That(PlayRow.Json(r, "TEST_X", "inst", "row1"), Does.EndWith("\"row_id\":\"row1\",\"keyif\":1}"));
            r.Fun = -1;
            Assert.That(PlayRow.Json(r, "TEST_X", "inst", "row1"), Does.Contain("\"keyif\":-1"));
        }

        /* P1 karar 4: once per player and level, on the win or after the third loss. */
        [Test]
        public void FunQuestionOnWinOrThirdLossOnce()
        {
            Assert.That(FunAsk.ShouldAsk(Outcome.Win, 0, false), Is.True, "a win asks");
            Assert.That(FunAsk.ShouldAsk(Outcome.Time, 0, false), Is.False, "1st loss: no");
            Assert.That(FunAsk.ShouldAsk(Outcome.Bomb, 1, false), Is.False, "2nd loss: no");
            Assert.That(FunAsk.ShouldAsk(Outcome.Time, 2, false), Is.True, "3rd loss: yes");
            Assert.That(FunAsk.ShouldAsk(Outcome.Quit, 5, false), Is.False, "a quit never asks");
            Assert.That(FunAsk.ShouldAsk(Outcome.Win, 0, true), Is.False, "never twice on a level");
            Assert.That(FunAsk.ShouldAsk(Outcome.Time, 7, true), Is.False);
        }

        /* Losses and the asked flag survive a reload, per level id. */
        [Test]
        public void FunStateIsSavedPerLevel()
        {
            var store = new MemoryStore();
            var a = new SaveData(store, () => "install");
            a.RecordLoss("R05"); a.RecordLoss("R05"); a.MarkFunAsked("R02");
            var b = new SaveData(store, () => "other");
            Assert.That((b.LossesOf("R05"), b.LossesOf("R06"), b.WasFunAsked("R02"), b.WasFunAsked("R05")), Is.EqualTo((2, 0, true, false)));
            // the row tells which case: the answer rides on that attempt's row, whose result is win or time/bomb
            var won = new AttemptRecord { LevelId = "R02", Result = "win", Fun = 1 };
            var lost = new AttemptRecord { LevelId = "R05", Result = "time", Fun = -1 };
            Assert.That(PlayRow.Json(won, "TEST_X", "i", "r1"), Does.Contain("\"result\":\"win\"").And.Contain("\"keyif\":1"));
            Assert.That(PlayRow.Json(lost, "TEST_X", "i", "r2"), Does.Contain("\"result\":\"time\"").And.Contain("\"keyif\":-1"));
        }

    }
}
