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

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_music", 1) == 1;
            set { PlayerPrefs.SetInt("rs_music", value ? 1 : 0); PlayerPrefs.Save(); Apply(); }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_MusicInit(string url, float volume, double loopSeconds, int on);
        [DllImport("__Internal")] static extern void RS_MusicSetOn(int on);
        [DllImport("__Internal")] static extern void RS_MusicDuck(float delay, float seconds);

        public static void Start(MonoBehaviour host) => RS_MusicInit(Url, Volume, LoopSeconds, On ? 1 : 0);
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
