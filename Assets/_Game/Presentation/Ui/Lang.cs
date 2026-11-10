using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_MESAJ_19 §3: the player's language, English by default, Turkish from the settings; kept in PlayerPrefs.
       Texts reads it; the web page's name gate reads the same choice from localStorage (WebBridge). */
    public static class Lang
    {
        public static int Revision { get; private set; }

        public static bool Tr
        {
            get => PlayerPrefs.GetString("rs_lang", "en") == "tr";
            set { PlayerPrefs.SetString("rs_lang", value ? "tr" : "en"); PlayerPrefs.Save(); Revision++; Changed?.Invoke(); }
        }

        /* Hosts that show their own texts (the web page) follow the choice. */
        public static event System.Action Changed;

        public static string T(string en, string tr) => Tr ? tr : en;
    }
}
