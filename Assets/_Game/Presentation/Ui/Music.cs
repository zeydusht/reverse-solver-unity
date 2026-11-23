using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Presentation
{
    /* Background music, low under the effects. PM_96: two loops, the lobby's (the menu, calm) and the level's (brighter),
       crossfaded on the dive (Scene); the win fanfare cuts the level's music and plays on the same clock, so the win screen
       follows its beats (FanfareTime, Beats); a loss turns the level's music down (Duck).
       Web: never in the build; Platform/Web/Music.jslib streams the MP3s from GitHub Pages into WebAudio loops, started on
       the first touch (browsers play nothing before one). iOS: the old Vorbis clip IosBuild puts in Resources for that
       build only (one loop for both). The settings switch is kept in PlayerPrefs; the editor plays only the fanfare. */
    public static class Music
    {
        public const string Url = "https://zeydusht.github.io/reverse-solver-unity/music/music.mp3";
        const string Pages = "https://zeydusht.github.io/reverse-solver-unity/music/";
        const float Volume = .35f;
        const double LoopSeconds = 2540160 / 44100.0;          // the old WAV's exact length: the loop point

        /* Every loop by its letter: PM_86's five (docs/art/pm86/MUZIK_LISANS.md), PM_95's eight (docs/art/pm95/LISANS.md),
           PM_96's ten (docs/art/pm96/LISANS.md: n-q for the lobby, r-w for the level). Each MP3's loop start (the
           encoder's lead-in) and length, from Assets/Audio/Music86|95|96/loops.json. */
        static readonly (char letter, string folder, double start, double length)[] Loops =
        {
            ('a', "pm86", 1105 / 44100.0, 124.44444444444444), ('b', "pm86", 1105 / 44100.0, 60.6315873015873),
            ('c', "pm86", 1105 / 44100.0, 128.0), ('d', "pm86", 1105 / 44100.0, 130.16950113378684), ('e', "pm86", 1106 / 44100.0, 106.62603174603174),
            ('f', "pm95", 1105 / 44100.0, 86.4), ('g', "pm95", 1105 / 44100.0, 96.0), ('h', "pm95", 1105 / 44100.0, 122.18181405895692),
            ('i', "pm95", 1105 / 44100.0, 76.8), ('j', "pm95", 1105 / 44100.0, 120.0), ('k', "pm95", 1105 / 44100.0, 82.28571428571429),
            ('l', "pm95", 1105 / 44100.0, 96.00002267573696), ('m', "pm95", 1105 / 44100.0, 57.17646258503402),
            ('n', "pm96", 1105 / 44100.0, 83.47825396825397), ('o', "pm96", 1105 / 44100.0, 69.8180045351474), ('p', "pm96", 1105 / 44100.0, 130.23399092970521),
            ('q', "pm96", 1105 / 44100.0, 63.999977324263035), ('r', "pm96", 1105 / 44100.0, 60.00131519274377), ('s', "pm96", 1105 / 44100.0, 139.13043083900226),
            ('t', "pm96", 1105 / 44100.0, 108.38709750566893), ('u', "pm96", 1105 / 44100.0, 108.38709750566893), ('v', "pm96", 1105 / 44100.0, 80.0),
            ('w', "pm96", 1105 / 44100.0, 87.6521768707483),
        };
        /* PM_96 §1: Zeyd chose c; until he picks from the new ones it is the lobby's, the level's is docs/art/pm96/RAPOR.md's. */
        public const char DefaultLobby = 'c', DefaultLevel = 'r', DefaultFanfare = 'b';

        static char Letter(string q, char fallback)
        {
            if (string.IsNullOrEmpty(q)) return fallback;
            if (q[0] == '0') return '\0';
            foreach (var l in Loops) if (l.letter == q[0]) return q[0];
            return fallback;
        }
        /* &lobi= (the old &muzik= too), &bolummuzik=; a letter, 0: the old loop. */
        public static char LobbyVariant
        {
            get
            {
                string q = Host.Current?.Query("lobi");
                return Letter(string.IsNullOrEmpty(q) ? Host.Current?.Query("muzik") : q, DefaultLobby);
            }
        }
        public static char LevelVariant => Letter(Host.Current?.Query("bolummuzik"), DefaultLevel);
        static (string url, double start, double length) Track(char v)
        {
            foreach (var l in Loops)
                if (l.letter == v) return ($"{Pages}{l.folder}/music-{v}.mp3", l.start, l.length);
            return (Url, -1, LoopSeconds);   // the old one: its lead-in is found by its first sound
        }

        /* The win fanfares (&kazanma=, the old &seskazan= too): 0 the old one, a-c PM_95's, d-g PM_96's; each a WAV on
           GitHub Pages (music/pm96/kazanma-X.wav) and its beats in Resources/Art/audio/pm96/vurus_haritasi.json
           (Tools/make_beatmap96.py). */
        public static char FanfareVariant
        {
            get
            {
                string q = Host.Current?.Query("kazanma");
                if (string.IsNullOrEmpty(q)) q = Host.Current?.Query("seskazan");
                if (string.IsNullOrEmpty(q)) return DefaultFanfare;
                return BeatMap.Has(q[0]) ? q[0] : DefaultFanfare;
            }
        }
        static string FanfareUrl(char v) => $"{Pages}pm96/kazanma-{v}.wav";

        /* The chosen fanfare's beats (s from its start, the screen's events) and the strongest one's index. */
        public static float[] Beats => BeatMap.Of(FanfareVariant).beats;
        public static int StrongBeat => BeatMap.Of(FanfareVariant).strong;
        public static float FanfareLength => BeatMap.Of(FanfareVariant).length;

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_music", 1) == 1;
            set { PlayerPrefs.SetInt("rs_music", value ? 1 : 0); PlayerPrefs.Save(); Apply(); }
        }

        public enum Scene { Lobby, Level }
        /* The dive's crossfade (PM_96 §4). */
        public const float CrossFade = .6f;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_MusicInit(string lobbyUrl, double lobbyStart, double lobbyLength, string levelUrl, double levelStart, double levelLength, string fanfareUrl, float volume, int on);
        [DllImport("__Internal")] static extern void RS_MusicSetOn(int on);
        [DllImport("__Internal")] static extern void RS_MusicScene(int scene, float fadeOut, float fadeIn);
        [DllImport("__Internal")] static extern void RS_MusicDuck(int on);
        [DllImport("__Internal")] static extern float RS_MusicFanfare(int sound);
        [DllImport("__Internal")] static extern float RS_MusicFanfareTime();

        public static void Start(MonoBehaviour host)
        {
            var a = Track(LobbyVariant); var b = Track(LevelVariant);
            RS_MusicInit(a.url, a.start, a.length, b.url, b.start, b.length, FanfareUrl(FanfareVariant), Volume, On ? 1 : 0);
            Debug.Log($"[muzik] lobi {Name(LobbyVariant)}, bolum {Name(LevelVariant)}, kazanma {FanfareVariant} ({Beats.Length} vurus)");
        }
        static void Apply() => RS_MusicSetOn(On ? 1 : 0);
        public static void Play(Scene scene, float fadeOut = CrossFade, float fadeIn = CrossFade) => RS_MusicScene(scene == Scene.Level ? 1 : 0, fadeOut, fadeIn);
        public static void Duck(bool on) => RS_MusicDuck(on ? 1 : 0);
        /* Starts the fanfare (silent with the sound off: its clock still runs); false: none (the screen keeps its own time). */
        public static bool Fanfare() => RS_MusicFanfare(Art.Muted ? 0 : 1) > 0;
        /* The fanfare's position at the speaker (s); below -10: none. */
        public static float FanfareTime => RS_MusicFanfareTime();
#else
        static AudioSource _src, _fan;
        static double _fanAt = -1;

        public static void Start(MonoBehaviour host)
        {
            if (host == null) return;
            _fan = host.gameObject.AddComponent<AudioSource>();
            _fanAt = -1;
            var clip = Resources.Load<AudioClip>("Music/music");
            if (clip == null) return;
            _src = host.gameObject.AddComponent<AudioSource>();
            _src.clip = clip;
            _src.loop = true;
            _src.volume = On ? Volume : 0;
            _src.Play();
        }

        static void Apply() { if (_src != null) _src.volume = On ? Volume : 0; }
        public static void Play(Scene scene, float fadeOut = CrossFade, float fadeIn = CrossFade) { Apply(); }
        public static void Duck(bool on) { if (_src != null) _src.volume = On ? Volume * (on ? .4f : 1) : 0; }

        /* The fanfare's clip: PM_95's recorded jingles and PM_96's (Resources), 0: the old one. */
        static AudioClip FanfareClip(char v)
        {
            if (v == '0') return Art.Sound("win");
            return Resources.Load<AudioClip>($"Art/audio/pm96/kazanma-{v}") ?? Resources.Load<AudioClip>($"Art/audio/pm95/kazan-{v}1");
        }
        public static bool Fanfare()
        {
            _fanAt = -1;
            var clip = FanfareClip(FanfareVariant);
            if (_fan == null || clip == null) return false;
            _fan.Stop();
            _fan.clip = clip; _fan.volume = Art.Muted ? 0 : 1;
            _fanAt = AudioSettings.dspTime + .03;
            _fan.PlayScheduled(_fanAt);
            if (_src != null) _src.volume = On ? Volume * .25f : 0;   // the one loop steps back under it
            return true;
        }
        public static float FanfareTime => _fanAt < 0 ? -100 : (float)(AudioSettings.dspTime - _fanAt);
#endif

        static string Name(char v) => v == '\0' ? "eski" : v.ToString();
    }
}
