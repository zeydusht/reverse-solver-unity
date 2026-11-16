using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_75: a stuck piece slides to where it really touches. Hand-built boards of single cells, flat joints except
       the ones each case sets (SetVJoint(x, y, 1): the left cell owns the tab; SetHJoint(x, y, 1): the upper one). */
    public class ContactTests
    {
        static Board Grid(int w, int h, params (int x, int y)[] empty)
        {
            var pieces = string.Join(",", Enumerable.Range(0, w * h).Select(i => $"[[{i % w},{i / w}]]"));
            var v = string.Join(",", Enumerable.Range(0, h).Select(_ => "[null," + string.Join(",", Enumerable.Repeat("0", w - 1).Append("null")) + "]"));
            var hRows = Enumerable.Range(0, h + 1).Select(y => "[" + string.Join(",", Enumerable.Repeat(y == 0 || y == h ? "null" : "0", w)) + "]");
            var level = LevelParser.Parse("{\"format\":1,\"levels\":[{\"id\":\"C\",\"version\":1,\"lv\":30,\"w\":" + w + ",\"h\":" + h + ",\"timer\":60," +
                $"\"pieces\":[{pieces}],\"vEdge\":[{v}],\"hEdge\":[{string.Join(",", hRows)}]}}]}}").Levels[0];
            var b = new Board(level);
            foreach (var (x, y) in empty) b.Remove(y * w + x);
            return b;
        }

        [Test]
        public void ZeydsVideoTheNeighboursTabStopsAThirdOfTheWayDown()
        {
            // the top-right piece, the cell below it empty; the lower-left neighbour's tab pokes into that cell
            var b = Grid(2, 3, (1, 1), (0, 2), (1, 2));
            b.SetVJoint(1, 1, 1);
            var limit = TravelRule.Limit(b, 1, Dir.D);
            Assert.That(limit.Exit, Is.False);
            Assert.That(limit.Cells, Is.EqualTo(0), "the rule: no full cell of travel");
            Assert.That(Contact.Distance(b, 1, Dir.D), Is.EqualTo(Contact.TabClear).Within(1e-5), "slides down to the tab");
            // one more empty cell above the tab: one more cell of travel
            var far = Grid(2, 4, (1, 1), (0, 1), (1, 2), (1, 3), (0, 3));
            far.SetVJoint(1, 2, 1);
            Assert.That(Contact.Distance(far, 1, Dir.D), Is.EqualTo(1 + Contact.TabClear).Within(1e-5));
        }

        [Test]
        public void APieceInTheWayStopsItTabToTabOrNested()
        {
            // a column: the piece at the top, two empty cells, a piece at the bottom
            var b = Grid(1, 4, (0, 1), (0, 2));
            b.SetHJoint(0, 1, 1);      // the moving piece has a tab down
            b.SetHJoint(0, 3, -1);     // the piece in the way has a tab up: tips meet
            Assert.That(TravelRule.Limit(b, 0, Dir.D).Cells, Is.EqualTo(2));
            Assert.That(Contact.Distance(b, 0, Dir.D), Is.EqualTo(2 - 2 * Contact.TabReach).Within(1e-5));
            b.SetHJoint(0, 3, 1);      // a socket up: the tab nests, the bodies meet
            Assert.That(Contact.Distance(b, 0, Dir.D), Is.EqualTo(2).Within(1e-5));
            b.SetHJoint(0, 1, 0); b.SetHJoint(0, 3, -1);   // flat against a tab
            Assert.That(Contact.Distance(b, 0, Dir.D), Is.EqualTo(2 - Contact.TabReach).Within(1e-5));
        }

        [Test]
        public void AnInterlockedPieceOnlyJiggles()
        {
            // the neighbour on the right is locked to it by a tab: no travel at all
            var b = Grid(2, 2, (0, 1));
            b.SetVJoint(1, 0, 1);
            Assert.That(TravelRule.Limit(b, 0, Dir.D).Cells, Is.EqualTo(0));
            Assert.That(Contact.Distance(b, 0, Dir.D), Is.EqualTo(0));
        }
    }
}
