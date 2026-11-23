using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_102 §4: the boosters' own recorded sounds (Tools/make_sfx102.py, docs/art/pm102/LISANS.md), three candidates each,
       Resources/Art/audio/pm102/booster-<booster>-<part>-<letter>, at the game's effects' level, on the animation's frame
       (BoosterFx74):
         makas   kesim    the blades closing (the cut)
         degnek  donusum  a rising shimmer whose poof falls on the turn: it starts WandLead s before it
         cekic   vurus    the blow and the break, on the strike;   kirinti  a soft clatter as the piece falls
         saat    tiktak   a quiet tick-tock loop through the freeze;   donma  the ice crackling as it freezes;
                 kirilma  the ice breaking as the frozen bar breaks
       &sesmakas=, &sesdegnek=, &sescekic=, &sessaat=: a, b or c (the default a), 0 the old sounds. Off with the sound
       switch; &sfx= as the other effects. */
    public static class BoosterSounds102
    {
        public const float WandLead = .40f;
        public const char Default = 'a';
        public enum Part { MakasKesim, DegnekDonusum, CekicVurus, CekicKirinti, SaatTiktak, SaatDonma, SaatKirilma }
        static readonly string[] Booster = { "makas", "degnek", "cekic", "cekic", "saat", "saat", "saat" };
        static readonly string[] Name = { "kesim", "donusum", "vurus", "kirinti", "tiktak", "donma", "kirilma" };

        static AudioClip[] _clips;
        static GameObject _host;
        static AudioSource[] _src;
        static AudioSource _loop;
        static int _next;

        public static char Letter(string booster)
        {
            string q = Host.Current?.Query("ses" + booster);
            if (string.IsNullOrEmpty(q)) return Default;
            return q[0] == '0' || q[0] == 'a' || q[0] == 'b' || q[0] == 'c' ? q[0] : Default;
        }

        public static void Load()
        {
            _clips = new AudioClip[Name.Length];
            var said = new System.Text.StringBuilder("[booster ses]");
            for (int i = 0; i < Name.Length; i++)
            {
                char l = Letter(Booster[i]);
                if (l != '0') _clips[i] = Resources.Load<AudioClip>($"Art/audio/pm102/booster-{Booster[i]}-{Name[i]}-{l}");
                _clips[i]?.LoadAudioData();
                said.Append(' ').Append(Booster[i]).Append('/').Append(Name[i]).Append(' ').Append(_clips[i] == null ? "eski" : l.ToString());
            }
            Debug.Log(said.ToString());
        }

        public static AudioClip Clip(Part p)
        {
            if (_clips == null) Load();
            return _clips[(int)p];
        }

        /* Whether the booster has its new sound (else BoosterFx74 plays the old one). */
        public static bool On(Part p) => Clip(p) != null;

        static AudioSource Source()
        {
            if (_host == null)
            {
                _host = new GameObject("BoosterSounds102");
                if (Application.isPlaying) Object.DontDestroyOnLoad(_host);
                _src = new AudioSource[3];
                for (int i = 0; i < _src.Length; i++) { _src[i] = _host.AddComponent<AudioSource>(); _src[i].playOnAwake = false; }
                _loop = _host.AddComponent<AudioSource>(); _loop.playOnAwake = false; _loop.loop = true;
            }
            var s = _src[_next]; _next = (_next + 1) % _src.Length;
            return s;
        }

        /* One part; false: none (the old sound plays). */
        public static bool Play(Part p)
        {
            var clip = Clip(p);
            if (clip == null) return false;
            if (Art.Muted || !Application.isPlaying) return true;
            var s = Source();
            s.Stop(); s.clip = clip; s.pitch = 1 + Random.Range(-.02f, .02f); s.volume = Mathf.Min(1, Sfx95.Gain); s.Play();
            Played?.Invoke(p);
            if (Host.Current?.Query("seslog") == "1") Debug.Log($"[booster ses] {Booster[(int)p]} {Name[(int)p]}");
            return true;
        }

        /* The clock's tick-tock through the freeze: on while frozen, off when it thaws (or the level ends). */
        public static void Ticking(bool on)
        {
            var clip = Clip(Part.SaatTiktak);
            if (!on || clip == null || Art.Muted || !Application.isPlaying)
            {
                if (_loop != null && _loop.isPlaying) _loop.Stop();
                return;
            }
            if (_host == null || _loop == null) Source();
            if (_loop.isPlaying && _loop.clip == clip) return;
            _loop.clip = clip; _loop.volume = Mathf.Min(1, Sfx95.Gain); _loop.Play();
            Played?.Invoke(Part.SaatTiktak);
        }

        public static bool TickingNow => _loop != null && _loop.isPlaying;

        /* For the tests: every booster sound as it starts. */
        public static System.Action<Part> Played;
    }
}
