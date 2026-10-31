using NUnit.Framework;
using ReverseSolver.Core;

namespace ReverseSolver.Core.Tests
{
    /* PM_MESAJ_25: cohort / device / platform / version reach plays and moves; no meta writes nothing new. */
    public class TelemetryMetaTests
    {
        [TearDown] public void Clear() => TelemetryMeta.Current = null;

        [Test]
        public void CohortCodeIsCleanedOrDropped()
        {
            Assert.That(TelemetryMeta.CleanCohort("pt1"), Is.EqualTo("pt1"));
            Assert.That(TelemetryMeta.CleanCohort("ic"), Is.EqualTo("ic"));
            Assert.That(TelemetryMeta.CleanCohort("PT1"), Is.Null);
            Assert.That(TelemetryMeta.CleanCohort("a b"), Is.Null);
            Assert.That(TelemetryMeta.CleanCohort(""), Is.Null);
            Assert.That(TelemetryMeta.CleanCohort(new string('a', 17)), Is.Null);
        }

        [Test]
        public void RowsCarryTheMeta()
        {
            string before = MoveRow.Json("r", "P", "S41", 1, 1, 0, "start", -1, null, 0, 0, 120, "");
            Assert.That(before, Does.Not.Contain("cohort"));
            TelemetryMeta.Current = new TelemetryMeta { Cohort = "ic", DeviceId = "dev1", Platform = "desktop", AppVersion = "web-x" };
            string move = MoveRow.Json("r", "P", "S41", 1, 1, 0, "start", -1, null, 0, 0, 120, "");
            Assert.That(move, Does.Contain("\"cohort\":\"ic\"").And.Contain("\"device_id\":\"dev1\"").And.Contain("\"kind\":\"start\""));
            TelemetryMeta.Current.Cohort = null;
            Assert.That(MoveRow.Json("r", "P", "S41", 1, 1, 1, "jam", 3, "L", 0, 0, 120, ""), Does.Contain("\"cohort\":null").And.Contain("\"dir\":\"L\""));
            Assert.That(ProfileRow.Json("pt1", "Ali", "dev1", "ios", 2), Is.EqualTo("{\"cohort\":\"pt1\",\"player\":\"Ali\",\"device_id\":\"dev1\",\"platform\":\"ios\",\"puzzle_exp\":2}"));
        }
    }
}
