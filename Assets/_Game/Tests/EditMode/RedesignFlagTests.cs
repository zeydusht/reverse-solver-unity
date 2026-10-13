using NUnit.Framework;
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
    }
}
