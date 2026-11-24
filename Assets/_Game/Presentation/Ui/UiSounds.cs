using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_98 §4: the UI's own sounds (Tools/make_ui98.py, Resources/Art/audio/pm98), loud enough over the music:
         Tick    every button (the &sestik= candidate, Zeyd's b; -14 LUFS)
         Toggle  a switch (sound, music, vibration, language)
         Open    a panel opens (a soft swish), Close: it closes (the same swish backwards)
         Dull    a button that can't act now (a locked booster, too little gold): a dull wooden knock
       A press asks for one sound per frame: Toggle and Dull win over the plain Tick. Open and Close play beside it; both
       in one frame (a panel rebuilt in place, e.g. after a switch) play neither. GameRoot plays them (Flush, each
       frame) on a source the pause does not stop. Played: every sound, for the tests (docs/TITRESIM_HARITASI.md).
       PM_104 §3: Tok, a short full "tok" (Tools/make_sfx104.py, pm104/ui-tok-<letter>) instead of the tick on Durdur,
       Devam, Menü, the bottom tabs and Ayarlar (AsTok: the press of this frame); &sesdugme= a-g, 0 the tick; without
       one ses.json's "dugme". A panel's swish under a tok plays 9 dB down (SwishUnderTok): the tok leads.
       PM_105 §3: Zeyd liked Durdur's tok and not the "çıt" (the tick b: four takes, a random pitch, -1 dB peaks): every
       button now plays the tok, one take (ui-tok-<letter>1), at one pitch and one level. The tick's files are gone;
       Tick stays only as the kind of a plain press (it sounds the tok).
       PM_106 §1: one sound per frame: a press drops the panel's swish (Ayarlar sounded twice). */
    public static class UiSounds
    {
        public enum Kind { Tick, Toggle, Open, Close, Dull, Tok }
        public static System.Action<Kind> Played;
        /* PM_105: the file each UI sound played (GameRoot.PlayUi), for the tests: the old tick never. */
        public static System.Action<string> PlayedClip;

        static Kind? _press;
        static bool _open, _close;

        public static void Request(Kind kind)
        {
            if (kind == Kind.Open) { _open = true; return; }
            if (kind == Kind.Close) { _close = true; return; }
            if (_press == null || Rank(kind) > Rank(_press.Value)) _press = kind;
        }
        static int Rank(Kind k) => k == Kind.Dull ? 3 : k == Kind.Toggle ? 2 : k == Kind.Tok ? 1 : 0;

        /* PM_106 §1: a press already asked for its sound this frame (a sound of the press's own result stays quiet). */
        public static bool Pressing => _press.HasValue;

        /* PM_104 §3: this frame's button press is a tok (a plain tick turns into it; none yet: a tok). */
        public static void AsTok()
        {
            if (_press == null || _press == Kind.Tick) _press = Kind.Tok;
        }

        /* The tok's candidate: &sesdugme=, else ses.json "dugme". */
        public static char DugmeLetter
        {
            get
            {
                string q = Host.Current?.Query("sesdugme");
                return !string.IsNullOrEmpty(q) && q[0] >= 'a' && q[0] <= 'g' ? q[0] : SesSecim.Letter("dugme");
            }
        }

        /* The last Take had a tok with a panel's swish: the swish 9 dB down. */
        public static bool SwishUnderTok { get; private set; }
        public const float SwishUnderTokVolume = .35f;

        /* What this frame asked for (then cleared). */
        public static List<Kind> Take()
        {
            var list = new List<Kind>();
            if (_press.HasValue) list.Add(_press.Value);
            // PM_106 §1: one sound per press: a panel opened or closed by a press is the press's sound alone (Ayarlar played
            // the tok and the panel's swish: Zeyd heard two ticks); the swish only when a panel moves on its own
            if (_open != _close && !_press.HasValue) list.Add(_open ? Kind.Open : Kind.Close);
            SwishUnderTok = false;
            _press = null; _open = _close = false;
            return list;
        }

        static readonly Dictionary<string, AudioClip[]> Clips = new Dictionary<string, AudioClip[]>();
        static readonly Dictionary<Kind, int> Last = new Dictionary<Kind, int>();

        static AudioClip[] Load(string prefix)
        {
            if (Clips.TryGetValue(prefix, out var c)) return c;
            var list = new List<AudioClip>();
            for (int n = 1; n <= (prefix.StartsWith("pm104/ui-tok-") ? 1 : 6); n++)   // PM_105: the tok: one take
            {
                var clip = SesSecim.Load<AudioClip>($"Art/audio/{prefix}{n}");
                if (clip == null) break;
                clip.LoadAudioData(); list.Add(clip);
            }
            return Clips[prefix] = list.ToArray();
        }

        /* The tick follows the button tick candidate (&sestik=: a, b, c; 0 the old one). */
        static string Prefix(Kind kind) => kind switch
        {
            Kind.Tick or Kind.Tok => "pm104/ui-tok-" + DugmeLetter,
            Kind.Toggle => "pm98/ui-toggle", Kind.Open => "pm98/ui-open", Kind.Close => "pm98/ui-close", _ => "pm98/ui-dull",
        };

        /* PM_103: read again (SesSecim.Collect). */
        public static void Clear() { Clips.Clear(); Last.Clear(); }

        public static void Preload() { foreach (Kind k in System.Enum.GetValues(typeof(Kind))) Load(Prefix(k)); }

        /* A take (never the one just played) and a small pitch shift; null: none (the old tick plays). */
        public static AudioClip Pick(Kind kind, out float pitch)
        {
            pitch = 1;
            var takes = Load(Prefix(kind));
            if (takes.Length == 0) return null;
            int last = Last.TryGetValue(kind, out int l) ? l : -1, i;
            if (takes.Length == 1) i = 0;
            else { i = Random.Range(0, takes.Length - (last >= 0 ? 1 : 0)); if (last >= 0 && i >= last) i++; }
            Last[kind] = i;
            pitch = kind == Kind.Tick || kind == Kind.Tok ? 1 : 1 + Random.Range(-.04f, .04f);   // PM_105: the tok always the same
            return takes[i];
        }
    }
}
