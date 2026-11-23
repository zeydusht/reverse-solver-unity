using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_100 §4: the transitions' sounds, light and under the music, on their own few sources (the Sound switch turns
       them off with the rest; &gecisses=0 only them, to compare). Recorded CC0, three candidates each
       (Tools/make_sfx100.py, docs/art/pm100/LISANS.md), Resources/Art/audio/pm100/gecis-<kind>-<letter>:
         koridor  the win's corridor slide: a steady texture, its loudness (and a little its pitch) following the slide's
                  speed each frame (CorridorTransition: slow off, loudest in the middle, gone as it settles)
         dalis    the dive's way in: a rising whoosh ending as the picture fills the screen, and there a ting
         cizgi    the cuts drawing: paper / brush, very quiet
         geri     the pull back to the board: a falling whoosh, and a tiny tick as the HUD settles
         sekme    the menu's pages moving under the tab bar: a short swish
       (the panels keep PM_98's swish). &geciskoridor=, &gecisdalis= (its ting too), &geciscizgi=, &gecisgeri= (its tick
       too), &gecissekme=: a, b or c (the default a), 0 none. */
    public static class Gecis100
    {
        public enum Kind { Koridor, Dalis, Ting, Cizgi, Geri, Hud, Sekme }
        static readonly string[] Keys = { "koridor", "dalis", "ting", "cizgi", "geri", "hud", "sekme" };
        static readonly int[] FlagOf = { 0, 1, 1, 3, 4, 4, 6 };   // the ting follows the dive's letter, the HUD's tick the pull back's
        public const char Default = 'a';
        /* The rising whoosh's length: it starts this long before the touch. */
        public const float DiveLead = .6f;

        static AudioClip[] _clips;
        static readonly char[] _letters = new char[7];
        static GameObject _host;
        static AudioSource[] _src;
        static int _next;
        static AudioSource _loop;

        public static bool Off => Host.Current?.Query("gecisses") == "0";
        /* PM_102 §5: &seslog=1: every transition sound as it starts, and the motions they belong to ([gecis] olay ...),
           for the timing table (Tools/pm102_record.js). */
        public static bool Log => Host.Current?.Query("seslog") == "1";
        public static void Event(string what) { if (Log) Debug.Log($"[gecis] olay {what}"); }

        public static char Letter(Kind kind)
        {
            string q = Host.Current?.Query("gecis" + Keys[FlagOf[(int)kind]]);
            if (string.IsNullOrEmpty(q)) return Default;
            return q[0] == '0' || q[0] == 'a' || q[0] == 'b' || q[0] == 'c' ? q[0] : Default;
        }

        /* The chosen clips, read now (no decode on the first play). */
        public static void Load()
        {
            _clips = new AudioClip[Keys.Length];
            var said = new System.Text.StringBuilder("[gecis]");
            for (int k = 0; k < Keys.Length; k++)
            {
                char l = _letters[k] = Letter((Kind)k);
                // PM_102: the corridor's texture louder (pm102) before PM_100's
                if (l != '0') _clips[k] = Resources.Load<AudioClip>($"Art/audio/pm102/gecis-{Keys[k]}-{l}") ?? Resources.Load<AudioClip>($"Art/audio/pm100/gecis-{Keys[k]}-{l}");
                _clips[k]?.LoadAudioData();
                said.Append(' ').Append(Keys[k]).Append(' ').Append(_clips[k] == null ? "-" : l.ToString());
            }
            if (Off) said.Append(" (kapalı: &gecisses=0)");
            Debug.Log(said.ToString());
        }

        public static AudioClip Clip(Kind kind)
        {
            if (_clips == null) Load();
            return _clips[(int)kind];
        }

        static AudioSource Source()
        {
            if (_host == null)
            {
                _host = new GameObject("Gecis100");
                if (Application.isPlaying) Object.DontDestroyOnLoad(_host);
                _src = new AudioSource[3];
                for (int i = 0; i < _src.Length; i++) { _src[i] = _host.AddComponent<AudioSource>(); _src[i].playOnAwake = false; }
                _loop = _host.AddComponent<AudioSource>(); _loop.playOnAwake = false; _loop.loop = true;
            }
            var s = _src[_next]; _next = (_next + 1) % _src.Length;
            return s;
        }

        /* A one-shot transition sound; false: none (off, muted, or no such clip). */
        public static bool Play(Kind kind, float volume = 1, float from = 0)
        {
            if (Off || Art.Muted || !Application.isPlaying) return false;
            var clip = Clip(kind);
            if (clip == null) return false;
            var s = Source();
            s.Stop(); s.clip = clip; s.pitch = 1; s.volume = volume;
            s.time = Mathf.Clamp(from, 0, clip.length - .01f);   // PM_102: a sound that must end on a moment, started late, joins it part way
            s.Play();
            Played?.Invoke(kind);
            if (Log) Debug.Log($"[gecis] ses {kind}");
            return true;
        }

        /* For the tests: every transition sound as it starts. */
        public static System.Action<Kind> Played;

        /* The corridor's slide, each frame: speed 0..1 (1 at its fastest); 0 ends it. The texture's loudness follows the
           speed (the slide's bell), its pitch a little (.9 at rest, 1.1 at the top). */
        public static void Corridor(float speed)
        {
            if (speed <= 0 || Off || Art.Muted || !Application.isPlaying)
            {
                if (_loop != null && _loop.isPlaying) _loop.Stop();
                return;
            }
            var clip = Clip(Kind.Koridor);
            if (clip == null) return;
            if (_host == null || _loop == null) Source();
            if (!_loop.isPlaying || _loop.clip != clip)
            {
                _loop.clip = clip; _loop.time = 0; _loop.volume = 0; _loop.Play();
                Played?.Invoke(Kind.Koridor);
                if (Log) Debug.Log("[gecis] ses Koridor");
            }
            // PM_102 §5: a little fuller in its rise and fall (it was under the lobby's music coming back)
            _loop.volume = Mathf.Pow(Mathf.Clamp01(speed), .75f);
            _loop.pitch = .9f + .2f * Mathf.Clamp01(speed);
        }

        public static float CorridorVolume => _loop != null && _loop.isPlaying ? _loop.volume : 0;
    }
}
