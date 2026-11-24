using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_106_EK: every link out of the game (Ayarlar: Gizlilik, the data deletion e-mail) goes through here. In the Unity
       editor (Play mode, PlayMode tests, the tap sweeps over every button) and with &test=1 nothing opens: the URL is
       logged ("[link] açılacaktı: ...") and kept in Kayit for the tests. Before this, the sweeps pressed Gizlilik and
       the mail button and Application.OpenURL opened a real Chrome tab (and the mailto app chooser) on Zeyd's computer
       at every run. In the real game (iOS, the web without the test flag) it opens as before. */
    public static class LinkAcici
    {
        /* what would have opened, oldest first (the editor and test play only) */
        public static readonly List<string> Kayit = new List<string>();

        /* set by GameRoot from the launch (&test=1, a test cohort) */
        public static bool TestPlay { get; set; }

        /* the editor (any Play, any test) or test play: log instead of opening */
        public static bool Holds => Application.isEditor || TestPlay;

        public static void Ac(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            if (Holds)
            {
                Kayit.Add(url);
                Debug.Log("[link] açılacaktı: " + url);
                return;
            }
            Application.OpenURL(url);
        }
    }
}
