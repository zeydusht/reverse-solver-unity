using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReverseSolver.Core.Tests
{
    /* PM_106_EK: the tests kept opening "Reverse Solver — Gizlilik Politikası" tabs and the mailto chooser in Zeyd's Chrome:
       the PlayMode sweep over every button pressed Ayarlar's Gizlilik and the e-mail button, and Application.OpenURL
       opens the system's browser from the editor. Every link now goes through LinkAcici.Ac: logged in the editor and
       with &test=1, opened in the real game. (PlayMode: OpeningFlowTests.SettingsLinksAreLoggedNotOpened and the sweep.) */
    public class Pm106EkTests
    {
        [Test]
        public void TheEditorLogsALinkInsteadOfOpeningIt()
        {
            LinkAcici.Kayit.Clear();
            Assert.That(LinkAcici.Holds, Is.True, "the editor never opens one");
            LogAssert.Expect(LogType.Log, "[link] açılacaktı: " + MenuSettings.PrivacyUrl);
            LinkAcici.Ac(MenuSettings.PrivacyUrl);
            Assert.That(LinkAcici.Kayit, Is.EqualTo(new[] { MenuSettings.PrivacyUrl }));
            LinkAcici.Kayit.Clear();
        }

        [Test]
        public void EveryLinkGoesThroughLinkAcici()
        {
            var opening = Directory.GetFiles("Assets/_Game", "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Replace('\\', '/').Contains("/Tests/") && !f.EndsWith("LinkAcici.cs"))
                .Where(f => File.ReadAllText(f).Contains("Application.OpenURL("))
                .ToList();
            Assert.That(opening, Is.Empty, "Application.OpenURL outside LinkAcici");
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("LinkAcici.Ac(MenuSettings.PrivacyUrl)", root);
            StringAssert.Contains("LinkAcici.Ac(MenuSettings.ContactUrl)", root);
            StringAssert.Contains("LinkAcici.TestPlay = _testMode;", root);
            // the real game: no editor, no test flag -> Application.OpenURL
            StringAssert.Contains("public static bool Holds => Application.isEditor || TestPlay;", File.ReadAllText("Assets/_Game/Presentation/Ui/LinkAcici.cs"));
        }
    }
}
