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
       the first touch (browsers play nothing before one). iOS (PM_103): the chosen lobby, level and fanfare are in the
       build (IosBuild copies them into Resources/Music for that build only: lobi, bolum, kazanma), played offline with
       the web's crossfades (MusicDriver). The editor plays the same files from Assets/Audio. The settings switch is
       kept in PlayerPrefs. */
    public static class Music
    {
        public const string Url = "https://zeydusht.github.io/reverse-solver-unity/music/music.mp3";
        const string Pages = "https://zeydusht.github.io/reverse-solver-unity/music/";
        /* PM_100 §3 (Zeyd: "the knock and the drag sounds stay far behind"): the music 3 dB down (.35 -> .248);
           &muzikses= a factor on it (0.7: 3 dB less again), for Zeyd to try before a new default. */
        public const float BaseVolume = .248f;
        public static float Gain
        {
            get
            {
                string q = Host.Current?.Query("muzikses");
                return !string.IsNullOrEmpty(q) && float.TryParse(q, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float g) ? Mathf.Clamp(g, 0, 2) : 1;
            }
        }
        static float Volume => BaseVolume * Gain;
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
            // PM_98 §2: the level's new candidates (docs/art/pm98): A-D ours (Tools/make_level98.py), E-G found (make_music98.py)
            ('A', "pm98", 1105 / 44100.0, 70.58823129251701), ('B', "pm98", 1105 / 44100.0, 69.56521541950113), ('C', "pm98", 1105 / 44100.0, 72.72725623582767),
            ('D', "pm98", 1105 / 44100.0, 70.58823129251701), ('E', "pm98", 1105 / 44100.0, 64.0), ('F', "pm98", 1105 / 44100.0, 87.27272108843538),
            ('G', "pm98", 1105 / 44100.0, 39.999977324263035),
        };
        /* PM_103: Zeyd's final choices (SesSecim, ses.json): the lobby n (Urban Shop), the level E (Cat in Space, its two
           32 s halves as an A/B loop), the win g ("Piano ta-da", 5 beats) in its marimba tone. */
        public static char DefaultLobby => SesSecim.Letter("lobi", 'n');
        public static char DefaultLevel => SesSecim.Letter("bolum", 'E');
        public static char DefaultFanfare => SesSecim.Letter("kazanma", 'g');

        /* PM_103: a loop's own file (no encoder lead-in, its exact loop length): what the iOS build embeds. */
        public static string SourceOf(char letter)
        {
            foreach (var l in Loops) if (l.letter == letter) return $"Assets/Audio/Music{l.folder.Substring(2)}/music-{letter}.ogg";
            return "Assets/Audio/Music/music.ogg";
        }
        /* The fanfare's own file by its clip key (g-marimba, b5 ...). */
        public static string FanfareSource(string clipKey) => $"Assets/Audio/Fanfare96/kazanma-{clipKey}.wav";

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
                return BeatMap.Has(q[0].ToString()) ? q[0] : DefaultFanfare;
            }
        }
        /* PM_99 §5: &kazanmavurus=5 (the default) plays the fanfare's 5-beat cut where it has one (kazanma-b5: its two
           middle beats left out), 7 the whole one. */
        public const int DefaultWinBeats = 5;
        public static int WinBeats => Host.Current?.Query("kazanmavurus") == "7" ? 7 : DefaultWinBeats;
        public static string FanfareKey
        {
            get
            {
                string full = FanfareVariant.ToString();
                return WinBeats == 5 && BeatMap.Has(full + "5") ? full + "5" : full;
            }
        }
        static string FanfareUrl(string key) => $"{Pages}pm96/kazanma-{key}.wav";

        /* The win fanfare in other instruments, the same notes, rhythm and length, so its beat map is theirs too:
           &kazanmaton=piyano (the fanfare as it is) | marimba (PM_103: Zeyd's choice) | pizz | ksilofon | kutu (| bakir
           for b). PM_102: g's (Tools/make_fanfare102.py); PM_100's b ones stay (&kazanma=b). The beats stay FanfareKey's. */
        public static string[] TonesOf(char fanfare) =>
            fanfare == 'g' ? new[] { "piyano", "marimba", "pizz", "ksilofon", "kutu" } :
            fanfare == 'b' ? new[] { "piyano", "marimba", "pizz", "ksilofon", "kutu", "bakir" } : new[] { "piyano" };
        public static string[] Tones => TonesOf(FanfareVariant);
        public const string Piano = "piyano";
        public static string DefaultTone => SesSecim.Of("kazanmaton") ?? Piano;
        public static string Tone
        {
            get
            {
                string q = Host.Current?.Query("kazanmaton");
                if (!string.IsNullOrEmpty(q)) foreach (var t in Tones) if (t == q) return t;
                foreach (var t in Tones) if (t == DefaultTone) return t;
                return Piano;   // a fanfare without that tone: itself
            }
        }
        /* The clip's key: g-marimba, b5-pizz ...; the beats' own key with the piano. */
        public static string FanfareClipKey => Tone != Piano ? FanfareKey + "-" + Tone : FanfareKey;

        /* The chosen fanfare's beats (s from its start, the screen's events) and the strongest one's index. */
        public static float[] Beats => BeatMap.Of(FanfareKey).beats;
        public static int StrongBeat => BeatMap.Of(FanfareKey).strong;
        public static float FanfareLength => BeatMap.Of(FanfareKey).length;

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_music", 1) == 1;
            set { PlayerPrefs.SetInt("rs_music", value ? 1 : 0); PlayerPrefs.Save(); Apply(); }
        }

        public enum Scene { Lobby, Level }
        /* The dive's crossfade (PM_96 §4). */
        public const float CrossFade = .6f;
        /* PM_98 §3: the music under a pause or settings panel, and how fast it comes back. */
        public const float PanelLevel = .35f, PanelBack = .3f;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_MusicInit(string lobbyUrl, double lobbyStart, double lobbyLength, string levelUrl, double levelStart, double levelLength, string fanfareUrl, float volume, int on);
        [DllImport("__Internal")] static extern void RS_MusicSetOn(int on);
        [DllImport("__Internal")] static extern void RS_MusicScene(int scene, float fadeOut, float fadeIn);
        [DllImport("__Internal")] static extern void RS_MusicDuck(int on);
        [DllImport("__Internal")] static extern float RS_MusicFanfare(int sound);
        [DllImport("__Internal")] static extern float RS_MusicFanfareTime();
        [DllImport("__Internal")] static extern void RS_MusicPanel(int on);

        public static void Start(MonoBehaviour host)
        {
            var a = Track(LobbyVariant); var b = Track(LevelVariant);
            RS_MusicInit(a.url, a.start, a.length, b.url, b.start, b.length, FanfareUrl(FanfareClipKey), Volume, On ? 1 : 0);
            Debug.Log($"[muzik] lobi {Name(LobbyVariant)}, bolum {Name(LevelVariant)}, kazanma {FanfareKey} ({Beats.Length} vurus, {Tone}), ses {Volume:0.000}");
        }
        static void Apply() => RS_MusicSetOn(On ? 1 : 0);
        public static void Play(Scene scene, float fadeOut = CrossFade, float fadeIn = CrossFade) => RS_MusicScene(scene == Scene.Level ? 1 : 0, fadeOut, fadeIn);
        public static void Duck(bool on) => RS_MusicDuck(on ? 1 : 0);
        /* PM_98 §3: a panel over the game (pause, settings): the music down to 35 %, back in .3 s when it closes. */
        public static void Panel(bool on) => RS_MusicPanel(on ? 1 : 0);
        /* Starts the fanfare (silent with the sound off: its clock still runs); false: none (the screen keeps its own time). */
        public static bool Fanfare() => RS_MusicFanfare(Art.Muted ? 0 : 1) > 0;
        /* The fanfare's position at the speaker (s); below -10: none. */
        public static float FanfareTime => RS_MusicFanfareTime();
#else
        /* PM_103: the web's music (Music.jslib) on AudioSources: the lobby's loop and the level's, each with its own fade
           (the lobby pauses where it stopped and resumes there, the level starts from its top), the level's duck under a
           loss, the panel's 35 %, the switch and the volume over both; the fanfare on its own source. MusicDriver moves the
           fades every frame; a lobby waiting for the fanfare's end starts on the audio clock. */
        sealed class Channel
        {
            public AudioSource src;
            public bool resume;
            public float fade, target, rate;
            public double startAt = -1;
            public float startFade;
        }
        static Channel _lobby, _level;
        static AudioSource _fan;
        static double _fanAt = -1;
        static float _duck = 1, _duckTarget = 1, _duckRate = 1, _panel = 1, _panelTarget = 1, _panelRate = 1;

        /* The chosen loop: the build's copy (iOS), else (the editor) its own file. */
        static AudioClip Clip(string embedded, string source, bool chosen)
        {
            var c = chosen ? Resources.Load<AudioClip>("Music/" + embedded) : null;
#if UNITY_EDITOR
            if (c == null) c = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(source);
#endif
            return c;
        }

        static Channel NewTrack(GameObject go, AudioClip clip, bool resume)
        {
            if (clip == null) return null;
            var t = new Channel { src = go.AddComponent<AudioSource>(), resume = resume };
            t.src.clip = clip; t.src.loop = true; t.src.playOnAwake = false; t.src.volume = 0;
            return t;
        }

        public static void Start(MonoBehaviour host)
        {
            if (host == null) return;
            var go = host.gameObject;
            _fan = go.AddComponent<AudioSource>(); _fan.playOnAwake = false;
            _fanAt = -1;
            _lobby = NewTrack(go, Clip("lobi", SourceOf(LobbyVariant), LobbyVariant == DefaultLobby), true);
            _level = NewTrack(go, Clip("bolum", SourceOf(LevelVariant), LevelVariant == DefaultLevel), false);
            _duck = _duckTarget = _panel = _panelTarget = 1;
            if (go.GetComponent<MusicDriver>() == null) go.AddComponent<MusicDriver>();
            Voice(_lobby, .3f);
            Debug.Log($"[muzik] lobi {Name(LobbyVariant)} ({(_lobby != null ? _lobby.src.clip.name : "yok")}), bolum {Name(LevelVariant)} ({(_level != null ? _level.src.clip.name : "yok")}), kazanma {FanfareClipKey}, ses {Volume:0.000}");
        }

        /* The track from silence up to full over `fade` s: the lobby where it paused, the level from its top. */
        static void Voice(Channel t, float fade)
        {
            if (t == null || t.src == null) return;
            t.startAt = -1;
            if (!t.resume || !t.src.isPlaying && t.src.time <= 0) { t.src.Stop(); t.src.time = 0; t.src.Play(); t.fade = 0; }
            else if (!t.src.isPlaying) { t.src.UnPause(); t.fade = 0; }
            t.target = 1; t.rate = fade > 0 ? 1 / fade : 1000;
        }
        /* The track down to silence over `fade` s; there it pauses (the lobby keeps its place) or stops. */
        static void FadeOut(Channel t, float fade)
        {
            if (t == null || t.src == null) return;
            t.startAt = -1;
            t.target = 0; t.rate = 1 / Mathf.Max(fade, .005f);
        }

        /* Every frame (MusicDriver): the fades, the volumes, a lobby due at the fanfare's end. */
        internal static void Tick(float dt)
        {
            _duck = Mathf.MoveTowards(_duck, _duckTarget, _duckRate * dt);
            _panel = Mathf.MoveTowards(_panel, _panelTarget, _panelRate * dt);
            float master = On ? Volume : 0;
            foreach (var t in new[] { _lobby, _level })
            {
                if (t == null || t.src == null) continue;
                if (t.startAt >= 0 && AudioSettings.dspTime >= t.startAt) Voice(t, t.startFade);
                t.fade = Mathf.MoveTowards(t.fade, t.target, t.rate * dt);
                if (t.fade <= 0 && t.target <= 0 && t.src.isPlaying) { if (t.resume) t.src.Pause(); else t.src.Stop(); }
                t.src.volume = master * _panel * (t == _level ? _duck : 1) * t.fade;
            }
        }

        static void Apply() => Tick(0);

        public static void Play(Scene scene, float fadeOut = CrossFade, float fadeIn = CrossFade)
        {
            var t = scene == Scene.Level ? _level : _lobby;
            var other = scene == Scene.Level ? _lobby : _level;
            if (_lobby != null) _lobby.startAt = -1;
            FadeOut(other, fadeOut);
            _duckTarget = 1; _duckRate = 1 / .15f;
            if (t == null || t.src == null) return;
            if (scene == Scene.Lobby && t.src.isPlaying && t.target > 0) return;   // the lobby already playing stays as it is
            Voice(t, fadeIn);
        }
        /* A loss / time up: the level's music down to 40 % in .25 s (on) and back in .5 s (off). */
        public static void Duck(bool on) { _duckTarget = on ? .4f : 1; _duckRate = (1 - .4f) / (on ? .25f : .5f); }
        /* PM_98 §3: a panel over the game (pause, settings): the music down to 35 % in .15 s, back in .3 s. */
        public static void Panel(bool on) { _panelTarget = on ? PanelLevel : 1; _panelRate = (1 - PanelLevel) / (on ? .15f : PanelBack); }

        /* The fanfare's clip: the build's copy (iOS: the chosen one), in the editor its own file; PM_96's (Resources), PM_95's
           recorded jingles, 0: the old one. */
        static AudioClip FanfareClip(string key)
        {
            if (key == "0") return Art.Sound("win");
            if (Tone == DefaultTone && FanfareVariant == DefaultFanfare)
            {
                var embedded = Resources.Load<AudioClip>("Music/kazanma");
                if (embedded != null) return embedded;
            }
#if UNITY_EDITOR
            var own = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(FanfareSource(key));
            if (own != null) return own;
            if (key.Contains("-")) key = key.Substring(0, key.IndexOf('-'));
#endif
            return Resources.Load<AudioClip>($"Art/audio/pm96/kazanma-{key}") ?? Resources.Load<AudioClip>($"Art/audio/pm95/kazan-{key[0]}1");
        }
        /* The win: the fanfare starts (silent with the sound off: its clock still runs), the music is cut in .15 s, and
           after the fanfare the lobby's music comes back in 1 s. */
        public static bool Fanfare()
        {
            _fanAt = -1;
            var clip = FanfareClip(FanfareClipKey);
            FadeOut(_level, .15f); FadeOut(_lobby, .15f);
            if (_fan == null || clip == null)
            {
                if (_lobby != null) { _lobby.startAt = AudioSettings.dspTime + 1.5; _lobby.startFade = 1; }
                return false;
            }
            _fan.Stop();
            _fan.clip = clip; _fan.volume = Art.Muted ? 0 : 1;
            _fanAt = AudioSettings.dspTime + .03;
            _fan.PlayScheduled(_fanAt);
            if (_lobby != null) { _lobby.startAt = _fanAt + clip.length; _lobby.startFade = 1; }
            return true;
        }
        public static float FanfareTime => _fanAt < 0 ? -100 : (float)(AudioSettings.dspTime - _fanAt);

        /* For the tests: what plays now. */
        public static (bool lobby, bool level, float lobbyVolume, float levelVolume, string lobbyClip, string levelClip) Now =>
            (_lobby?.src != null && _lobby.src.isPlaying, _level?.src != null && _level.src.isPlaying,
             _lobby?.src != null ? _lobby.src.volume : 0, _level?.src != null ? _level.src.volume : 0,
             _lobby?.src != null && _lobby.src.clip != null ? _lobby.src.clip.name : null,
             _level?.src != null && _level.src.clip != null ? _level.src.clip.name : null);
        public static string FanfareClipName => _fan != null && _fan.clip != null ? _fan.clip.name : null;
#endif

        static string Name(char v) => v == '\0' ? "eski" : v.ToString();
    }
}
