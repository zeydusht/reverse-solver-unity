using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Presentation
{
    /* Background music (Zeyd chose music-c: 57.6 s, a seamless loop), low under the effects, on in the menu and in play.
       Web: never in the build; Platform/Web/Music.jslib streams the MP3 from GitHub Pages into its own WebAudio loop,
       started on the first touch (browsers play nothing before one). iOS: the Vorbis clip IosBuild puts in Resources
       for that build only. The settings switch is kept in PlayerPrefs; the editor stays silent. */
    public static class Music
    {
        public const string Url = "https://zeydusht.github.io/reverse-solver-unity/music/music.mp3";
        const float Volume = .35f;
        const double LoopSeconds = 2540160 / 44100.0;          // the WAV's exact length: the loop point

        /* PM_86 §5: five CC0 candidates (docs/art/pm86/MUZIK_LISANS.md, Tools/make_music86.py), &muzik=a..e; none: the
           old loop. Each MP3's loop start (the encoder's lead-in) and length, from Assets/Audio/Music86/loops.json. */
        static readonly (char letter, double start, double length)[] Loops86 =
        {
            ('a', 1105 / 44100.0, 124.44444444444444), ('b', 1105 / 44100.0, 60.6315873015873),
            ('c', 1105 / 44100.0, 128.0), ('d', 1105 / 44100.0, 130.16950113378684), ('e', 1106 / 44100.0, 106.62603174603174),
        };
        /* PM_95 §2: eight more CC0 candidates (docs/art/pm95/LISANS.md, Tools/make_music95.py), &muzik=f..m, -18 LUFS;
           the web's default is now f (docs/art/pm95/RAPOR.md: why); &muzik=0: the old loop. From Assets/Audio/Music95/loops.json. */
        static readonly (char letter, double start, double length)[] Loops95 =
        {
            ('f', 1105 / 44100.0, 86.4), ('g', 1105 / 44100.0, 96.0), ('h', 1105 / 44100.0, 122.18181405895692),
            ('i', 1105 / 44100.0, 76.8), ('j', 1105 / 44100.0, 120.0), ('k', 1105 / 44100.0, 82.28571428571429),
            ('l', 1105 / 44100.0, 96.00002267573696), ('m', 1105 / 44100.0, 57.17646258503402),
        };
        public const char Default95 = 'f';
        public static char Variant
        {
            get
            {
                string q = Host.Current?.Query("muzik");
                if (string.IsNullOrEmpty(q)) return Default95;
                if (q[0] == '0') return '\0';
                foreach (var l in Loops86) if (l.letter == q[0]) return q[0];
                foreach (var l in Loops95) if (l.letter == q[0]) return q[0];
                return Default95;
            }
        }
        static (string url, double start, double length) Track
        {
            get
            {
                char v = Variant;
                foreach (var l in Loops86)
                    if (l.letter == v) return ($"https://zeydusht.github.io/reverse-solver-unity/music/pm86/music-{v}.mp3", l.start, l.length);
                foreach (var l in Loops95)
                    if (l.letter == v) return ($"https://zeydusht.github.io/reverse-solver-unity/music/pm95/music-{v}.mp3", l.start, l.length);
                return (Url, -1, LoopSeconds);   // the old one: its lead-in is found by its first sound
            }
        }

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_music", 1) == 1;
            set { PlayerPrefs.SetInt("rs_music", value ? 1 : 0); PlayerPrefs.Save(); Apply(); }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_MusicInit(string url, float volume, double loopStart, double loopSeconds, int on);
        [DllImport("__Internal")] static extern void RS_MusicSetOn(int on);
        [DllImport("__Internal")] static extern void RS_MusicDuck(float delay, float seconds);

        public static void Start(MonoBehaviour host) { var t = Track; RS_MusicInit(t.url, Volume, t.start, t.length, On ? 1 : 0); }
        static void Apply() => RS_MusicSetOn(On ? 1 : 0);
        /* The win fanfare: the music steps back for `seconds`, `delay` from now. */
        public static void Duck(float delay, float seconds) => RS_MusicDuck(delay, seconds);
#else
        static AudioSource _src;
        static MonoBehaviour _host;

        public static void Start(MonoBehaviour host)
        {
            var clip = Resources.Load<AudioClip>("Music/music");
            if (clip == null || host == null) return;
            _host = host;
            _src = host.gameObject.AddComponent<AudioSource>();
            _src.clip = clip;
            _src.loop = true;
            _src.volume = On ? Volume : 0;
            _src.Play();
        }

        static void Apply() { if (_src != null) _src.volume = On ? Volume : 0; }

        /* The win fanfare: the music steps back for `seconds`, `delay` from now. */
        public static void Duck(float delay, float seconds)
        {
            if (_src == null || !On) return;
            _host.StartCoroutine(DuckRun(delay, seconds));
        }

        static System.Collections.IEnumerator DuckRun(float delay, float seconds)
        {
            yield return new WaitForSecondsRealtime(delay);
            for (float t = 0; t < .2f; t += Time.unscaledDeltaTime)
            {
                if (_src == null) yield break;
                _src.volume = Mathf.Lerp(Volume, Volume * .25f, t / .2f) * (On ? 1 : 0);
                yield return null;
            }
            yield return new WaitForSecondsRealtime(seconds);
            for (float t = 0; t < .6f; t += Time.unscaledDeltaTime)
            {
                if (_src == null) yield break;
                _src.volume = Mathf.Lerp(Volume * .25f, Volume, t / .6f) * (On ? 1 : 0);
                yield return null;
            }
            Apply();
        }
#endif
    }
}
