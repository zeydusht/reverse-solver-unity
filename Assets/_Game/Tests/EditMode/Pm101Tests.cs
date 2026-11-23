using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_101: AdMob in place of AppLovin MAX: which ids a device uses, and the store build's guard. */
    public class Pm101Tests
    {
        const string Real = "{\"build\": \"store\", \"ios_app_id\": \"ca-app-pub-4171335473466896~6469853119\", " +
                            "\"ios_rewarded\": \"ca-app-pub-4171335473466896/2134575175\", \"ios_interstitial\": \"ca-app-pub-4171335473466896/7814724376\", \"test_devices\": [\"abc\"]}";

        [Test]
        public void TheStoreBuildUsesTheRealIds()
        {
            var k = AdKeys.Choose(Real, false, false);
            Assert.That(k.Test, Is.False);
            Assert.That(k.AppId, Is.EqualTo("ca-app-pub-4171335473466896~6469853119"));
            Assert.That(k.Rewarded, Is.EqualTo("ca-app-pub-4171335473466896/2134575175"));
            Assert.That(k.Interstitial, Is.EqualTo("ca-app-pub-4171335473466896/7814724376"));
            Assert.That(k.TestDevices, Is.EqualTo(new[] { "abc" }));
        }

        [Test]
        public void TestBuildsAndTestDevicesGetGooglesTestIds()
        {
            foreach (var k in new[] { AdKeys.Choose(Real, true, false), AdKeys.Choose(Real, false, true), AdKeys.Choose(Real.Replace("\"store\"", "\"test\""), false, false),
                                      AdKeys.Choose(Real.Replace("\"build\": \"store\", ", ""), false, false) })
            {
                Assert.That(k.Test, Is.True);
                Assert.That(k.Rewarded, Is.EqualTo(AdKeys.TestRewarded));
                Assert.That(k.Interstitial, Is.EqualTo(AdKeys.TestInterstitial));
                Assert.That(k.AppId, Is.EqualTo(AdKeys.TestAppId));
            }
            Assert.That(AdKeys.TestRewarded, Is.EqualTo("ca-app-pub-3940256099942544/1712485313"), "Google's iOS test rewarded");
            Assert.That(AdKeys.TestInterstitial, Is.EqualTo("ca-app-pub-3940256099942544/4411468910"), "Google's iOS test interstitial");
        }

        [Test]
        public void TheStoreBuildStopsOnTestOrMissingIds()
        {
            Assert.That(AdKeys.StoreProblem(Real), Is.Null);
            Assert.That(AdKeys.StoreProblem(Real.Replace("4171335473466896/2134575175", "3940256099942544/1712485313")), Does.Contain("test"));
            Assert.That(AdKeys.StoreProblem(Real.Replace("ca-app-pub-4171335473466896/7814724376", "BURAYA_ID")), Does.Contain("yer tutucu"));
            Assert.That(AdKeys.StoreProblem(Real.Replace("~6469853119", "/6469853119")), Is.Not.Null, "an App ID has a ~");
            Assert.That(AdKeys.StoreProblem("not json"), Is.Not.Null);
        }

        [Test]
        public void TheConfigHasTheRealIds()
        {
            string json = System.IO.File.ReadAllText("Assets/Config/ads.json");
            Assert.That(AdKeys.StoreProblem(json), Is.Null, "Assets/Config/ads.json: the store build can go");
            Assert.That(json, Does.Not.Contain("sdk_key"), "no AppLovin key left");
        }
    }
}
