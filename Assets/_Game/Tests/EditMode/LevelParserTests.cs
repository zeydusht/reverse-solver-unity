using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* M1 acceptance 5: a malformed level file is rejected with a clear error
       that names where the problem is. */
    public class LevelParserTests
    {
        // Smallest valid level: 2x1, two single-cell pieces with a flat seam.
        const string Level =
            "{\"id\":\"T1\",\"version\":1,\"w\":2,\"h\":1,\"pieces\":[[[0,0]],[[1,0]]]," +
            "\"vEdge\":[[null,0,null]],\"hEdge\":[[null,null],[null,null]]," +
            "\"solution\":[{\"piece\":0,\"dir\":\"L\"},{\"piece\":1,\"dir\":\"R\"}]}";

        static string File(string levels, int format = 1) => $"{{\"format\":{format},\"levels\":[{levels}]}}";

        static string Fails(string json)
        {
            var e = Assert.Throws<LevelFormatException>(() => LevelParser.Parse(json));
            return e.Message;
        }

        [Test]
        public void MinimalLevelParses()
        {
            var set = LevelParser.Parse(File(Level));
            var l = set["T1"];
            Assert.That(l.Width, Is.EqualTo(2));
            Assert.That(GreedySolver.Solve(l).Solved);
        }

        [Test]
        public void UnknownFormatIsRejected() =>
            StringAssert.Contains("unsupported format 2", Fails(File(Level, format: 2)));

        [Test]
        public void MissingFormatIsRejected() =>
            StringAssert.Contains("format: is missing", Fails($"{{\"levels\":[{Level}]}}"));

        [Test]
        public void DuplicateIdIsRejected() =>
            StringAssert.Contains("duplicate id 'T1'", Fails(File(Level + "," + Level)));

        [Test]
        public void MissingIdIsRejected() =>
            StringAssert.Contains("levels[0].id: missing or empty id", Fails(File(Level.Replace("\"id\":\"T1\",", ""))));

        [Test]
        public void EmptyIdIsRejected() =>
            StringAssert.Contains("missing or empty id", Fails(File(Level.Replace("\"T1\"", "\"  \""))));

        [Test]
        public void MissingVersionIsRejected() =>
            StringAssert.Contains("T1.version: is missing", Fails(File(Level.Replace("\"version\":1,", ""))));

        [Test]
        public void ZeroVersionIsRejected() =>
            StringAssert.Contains("T1.version: must be >= 1", Fails(File(Level.Replace("\"version\":1", "\"version\":0"))));

        [Test]
        public void JointGridOfWrongSizeIsRejected() =>
            StringAssert.Contains("T1.vEdge[0]: must have 3 entries", Fails(File(Level.Replace("[[null,0,null]]", "[[null,0]]"))));

        [Test]
        public void OffBoardCellIsRejected() =>
            StringAssert.Contains("off the 2x1 board", Fails(File(Level.Replace("[[1,0]]]", "[[2,0]]]"))));

        [Test]
        public void OverlappingPiecesAreRejected() =>
            StringAssert.Contains("already belongs to piece 0", Fails(File(Level.Replace("[[1,0]]]", "[[0,0]]]"))));

        [Test]
        public void BadDirectionIsRejected() =>
            StringAssert.Contains("must be U, D, L or R", Fails(File(Level.Replace("\"dir\":\"R\"", "\"dir\":\"X\""))));

        [Test]
        public void MalformedJsonReportsPosition() =>
            StringAssert.Contains("line 1, column", Fails("{\"format\":1,\"levels\":[}"));
    }

    public class JsonReaderTests
    {
        [Test]
        public void ReadsTurkishTextAndEscapes()
        {
            var n = JsonReader.Parse("{\"t\":\"Çivili parça \\u00e7 \\\"x\\\"\\n\"}");
            Assert.That(n.TryGet("t", out var t));
            Assert.That(t.AsString, Is.EqualTo("Çivili parça ç \"x\"\n"));
        }

        [Test]
        public void ReadsNumbersCultureIndependently()
        {
            var a = JsonReader.Parse("[0,-1,2.5,1e3,-0.25E-2]");
            Assert.That(a.Items[2].AsNumber, Is.EqualTo(2.5));
            Assert.That(a.Items[3].AsNumber, Is.EqualTo(1000));
            Assert.That(a.Items[4].AsNumber, Is.EqualTo(-0.0025));
        }

        [Test]
        public void KeepsObjectKeyOrder()
        {
            var o = JsonReader.Parse("{\"b\":1,\"a\":2}");
            Assert.That(o.Members[0].Key, Is.EqualTo("b"));
        }

        [TestCase("[1,]")]
        [TestCase("{\"a\":1,}")]
        [TestCase("[01]")]
        [TestCase("\"open")]
        [TestCase("[1] 2")]
        [TestCase("tru")]
        public void RejectsInvalidJson(string json) =>
            Assert.Throws<JsonException>(() => JsonReader.Parse(json));
    }
}
