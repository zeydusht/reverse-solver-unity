using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* M4 criterion 1: every booster's path from the bar (tap / confirm /
       target) to a GameSession command and its events, without Unity. */
    public class BoosterControlsTests
    {
        static GameSession Session(string id, uint seed = 1) => new GameSession(TestData.Levels[id], 1, "test", new Mulberry32(seed));

        static int Count(GameSession s, EventKind k) => s.Events.Count(e => e.Kind == k);

        [Test]
        public void LockedBoosterSaysWhereItUnlocksAndDoesNothing()
        {
            var s = Session("L01");
            var bar = new BoosterControls(s);
            Assert.That(bar.StateOf(Booster.Hammer), Is.EqualTo(BoosterButton.Locked));
            bar.Tap(Booster.Hammer);
            Assert.That(bar.Prompt, Is.EqualTo("9. bölümde açılıyor"));
            Assert.That(bar.Armed, Is.Null);
            Assert.That(s.Events, Is.Empty);
        }

        [Test]
        public void OutOfStockBoosterDoesNothing()
        {
            var s6 = Session("L06");                      // wand 1
            var bar6 = new BoosterControls(s6);
            bar6.Tap(Booster.Wand); bar6.Confirm();
            Assert.That(bar6.StateOf(Booster.Wand), Is.EqualTo(BoosterButton.Out));
            bar6.Tap(Booster.Wand);
            Assert.That(bar6.Asking, Is.Null);
            Assert.That(s6.Used(Booster.Wand), Is.EqualTo(1));
        }

        [Test]
        public void ScissorsArmsListsJointsAndCuts()
        {
            var s = Session("L03");                       // scissors 2, unlocked at 3
            var bar = new BoosterControls(s);
            Assert.That(bar.Joints(), Is.Empty, "not armed yet");
            bar.Tap(Booster.Scissors);
            Assert.That(bar.Armed, Is.EqualTo(Booster.Scissors));
            Assert.That(bar.Prompt, Is.EqualTo(BoosterControls.ScissorsHint));
            Assert.That(bar.BlocksDrag);
            var joints = bar.Joints();
            Assert.That(joints, Is.Not.Empty);
            Assert.That(joints.All(j => s.IsCuttable(j.x, j.y, j.vertical)));
            var j0 = joints[0];
            Assert.That(bar.TapJoint(j0.x, j0.y, j0.vertical), Is.EqualTo(CommandResult.Ok));
            Assert.That(Count(s, EventKind.JointCut), Is.EqualTo(1));
            Assert.That(s.Used(Booster.Scissors), Is.EqualTo(1));
            Assert.That(bar.Armed, Is.Null, "spend disarms");
            Assert.That(bar.Prompt, Is.Empty);
        }

        [Test]
        public void TappingAnArmedBoosterAgainDisarms()
        {
            var bar = new BoosterControls(Session("L03"));
            bar.Tap(Booster.Scissors);
            bar.Tap(Booster.Scissors);
            Assert.That(bar.Armed, Is.Null);
            Assert.That(bar.BlocksDrag, Is.False);
        }

        [Test]
        public void HammerArmsThenBreaksATappedPiece()
        {
            var s = Session("L09");                       // hammer 2, unlocked at 9; nails present
            var bar = new BoosterControls(s);
            int nailed = s.Nails.First(n => n.Value > 0).Key;
            Assert.That(bar.TapPiece(nailed), Is.EqualTo(CommandResult.Ok), "not armed: ignored");
            Assert.That(s.Board.IsPresent(nailed));
            bar.Tap(Booster.Hammer);
            Assert.That(bar.Prompt, Is.EqualTo(BoosterControls.HammerHint));
            Assert.That(bar.TapPiece(nailed), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Board.IsPresent(nailed), Is.False);
            Assert.That(s.Events.Any(e => e.Kind == EventKind.PieceRemoved && e.ByHammer && e.Piece == nailed));
            Assert.That(bar.Armed, Is.Null);
        }

        [Test]
        public void WandAsksFirstThenReshufflesOnConfirm()
        {
            var s = Session("L06");
            var bar = new BoosterControls(s);
            bar.Tap(Booster.Wand);
            Assert.That(bar.Asking, Is.EqualTo(Booster.Wand));
            Assert.That(bar.Question(Booster.Wand).title, Is.EqualTo("Değnek kullanılsın mı?"));
            Assert.That(s.Events, Is.Empty, "nothing happens before confirming");
            Assert.That(bar.Confirm(), Is.EqualTo(CommandResult.Ok));
            Assert.That(Count(s, EventKind.JointsReshuffled), Is.EqualTo(1));
            Assert.That(bar.Asking, Is.Null);
        }

        [Test]
        public void CancellingTheQuestionUsesNothing()
        {
            var s = Session("L12");
            var bar = new BoosterControls(s);
            bar.Tap(Booster.Clock);
            bar.CancelAsk();
            Assert.That(bar.Asking, Is.Null);
            Assert.That(s.Used(Booster.Clock), Is.Zero);
            Assert.That(s.Events, Is.Empty);
        }

        [Test]
        public void ClockAddsTwentySecondsAndSaysSo()
        {
            var s = Session("L12");
            var bar = new BoosterControls(s);
            int t = s.TimeLeft;
            bar.Tap(Booster.Clock);
            Assert.That(bar.Confirm(), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.TimeLeft, Is.EqualTo(t + 20));
            Assert.That(bar.Prompt, Is.EqualTo(BoosterControls.ClockDone));
            Assert.That(Count(s, EventKind.TimeAdded), Is.EqualTo(1));
        }

        [Test]
        public void ClockKeepsAnArmedScissorsArmedLikeTheWebGame()
        {
            var s = Session("L12");
            var bar = new BoosterControls(s);
            bar.Tap(Booster.Scissors);
            bar.Tap(Booster.Clock);
            bar.Confirm();
            Assert.That(bar.Armed, Is.EqualTo(Booster.Scissors));
        }

        [Test]
        public void FreshMarksTheBoosterTheLevelIntroducesUntilUsed()
        {
            var s = Session("L03");                       // bintro: scissors
            var bar = new BoosterControls(s);
            Assert.That(bar.IsFresh(Booster.Scissors));
            Assert.That(bar.IsFresh(Booster.Hammer), Is.False);
            bar.Tap(Booster.Scissors);
            var j = bar.Joints()[0];
            bar.TapJoint(j.x, j.y, j.vertical);
            Assert.That(bar.IsFresh(Booster.Scissors), Is.False);
        }

        [Test]
        public void NothingHappensAfterTheEnd()
        {
            var s = Session("L12");
            s.Quit();
            var bar = new BoosterControls(s);
            int events = s.Events.Count;
            bar.Tap(Booster.Clock);
            Assert.That(bar.Asking, Is.Null);
            Assert.That(s.Events.Count, Is.EqualTo(events));
        }

        // ---- criterion 2: leaving a level is a quit row --------------------------------

        [Test]
        public void RestartAndBackToMenuWriteQuitRows()
        {
            var log = new AttemptLog();
            var level = TestData.Levels["L05"];
            var a = log.Start(level, "unity-web", new Mulberry32(1));
            a.Tick(3);
            a.Quit();                                     // restart
            log.Add(a.Record);
            var b = log.Start(level, "unity-web", new Mulberry32(1));
            b.Quit();                                     // back to the menu
            log.Add(b.Record);
            Assert.That(log.Records.Select(r => (r.Attempt, r.Result)), Is.EqualTo(new[] { (1, "quit"), (2, "quit") }));
            Assert.That(log.Records[0].DurationSeconds, Is.EqualTo(3));
        }
    }
}
