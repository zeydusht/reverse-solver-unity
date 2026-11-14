using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_72: the &test=1 store, the haptic switches. */
    public class Pm72Tests
    {
        [Test]
        public void TestLinkWritesStayInMemory()
        {
            var device = new MemoryStore();
            device.Set("rs_player", "Zeyd"); device.Set("rs_best", "{\"S01\":3}");
            var test = new TestHost.OverlayStore(device);
            Assert.That(test.Get("rs_player"), Is.EqualTo("Zeyd"), "the name is read from the device");
            test.Set("rs_best", "{\"S01\":3,\"S02\":3}");
            test.Delete("rs_player");
            test.Save();
            Assert.That(test.Get("rs_best"), Does.Contain("S02"));
            Assert.That(test.Get("rs_player"), Is.Null);
            Assert.That(device.Get("rs_best"), Is.EqualTo("{\"S01\":3}"), "test progress never reaches the normal save");
            Assert.That(device.Get("rs_player"), Is.EqualTo("Zeyd"));
            Assert.That(device.Saves, Is.Zero);
        }

        [Test]
        public void TestModeAmounts()
        {
            Assert.That(GameSession.TestStock, Is.EqualTo(999));
            Assert.That(SaveData.TestCoins, Is.EqualTo(999999));
        }

        [Test]
        public void VibrationSwitchesGateEveryTap()
        {
            bool on = Haptics.On, jam = Haptics.JamOn;
            try
            {
                Haptics.On = true; Haptics.JamOn = true;
                Assert.That(Haptics.Allowed(Haptics.Kind.Light) && Haptics.Allowed(Haptics.Kind.Jam), Is.True);
                Haptics.JamOn = false;
                Assert.That(Haptics.Allowed(Haptics.Kind.Jam), Is.False, "the jam tap has its own switch");
                Assert.That(Haptics.Allowed(Haptics.Kind.Medium), Is.True);
                Haptics.On = false; Haptics.JamOn = true;
                foreach (Haptics.Kind k in System.Enum.GetValues(typeof(Haptics.Kind)))
                    Assert.That(Haptics.Allowed(k), Is.False, $"the Titreşim switch turns {k} off");
                // web lengths: short taps only, the jam the shortest
                Assert.That(Haptics.WebMs(Haptics.Kind.Light), Is.InRange(10, 20));
                Assert.That(Haptics.WebMs(Haptics.Kind.Jam), Is.LessThan(Haptics.WebMs(Haptics.Kind.Light)));
            }
            finally { Haptics.On = on; Haptics.JamOn = jam; }
        }
    }
}
