using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_107 §C: the game's mix in one place (Zeyd: "nothing should scratch my ear, not too much, not too little; clear
       and clean. I stopped the clock and never heard its tick-tock over the slides").
       Buses: every sound the game plays belongs to one, and its source's volume is multiplied by the bus's gain:
         Music    the lobby's and the level's loops (Music: RS_MusicMix on the web, the sources' volume elsewhere)
         Sfx      the board's effects: the slide/exit, the knock, the obstacles, the boosters' one-shots
         Ui       the buttons' tok, the panels' swish, the transitions (Gecis100)
         Bed      the chain's rattle (continuous)
         Clock    the clock booster's tick-tock (continuous)
         Fanfare  the win's fanfare and jingles
       Each bus's base level is Base (the levels of the files themselves are set by Tools/make_miks107.py: the chosen
       sounds normalised to their targets, their harsh highs softened, their attacks rounded; Resources/Art/audio/pm107
       overrides the original file by name, SesSecim.Load).
       Ducking (§C3): while the clock runs, the effects 5 dB down and the music 3 dB (the tick-tock in front); while the
       fanfare plays, the effects and the chain 6 dB down, the buttons 3; a bomb's bang: the music 4 dB down
       (on top of the loss's own duck, Music.Duck) for 1.6 s. Gains glide: down at 80 dB/s, back up at 20 dB/s (no pumping).
       Voices (§C4): the same clip at most twice at once (a burst of flicks does not stack up into a crack).
       &miks=0 turns all of it off (the before recordings: the old files, no ducking, no voice limit). */
    public static class Mix107
    {
        public enum Bus { Music, Sfx, Ui, Bed, Clock, Fanfare }
        const int Buses = 6;

        public static bool On => Host.Current?.Query("miks") != "0";

        /* Each bus's level (linear) with nothing ducked. */
        public static float Base(Bus b) => 1f;

        /* The ducks (dB, negative) per state. */
        public const float ClockSfxDb = -5f, ClockMusicDb = -3f, FanfareSfxDb = -6f, FanfareUiDb = -3f, BangMusicDb = -4f, BangSeconds = 1.6f;
        public const float DownDbPerSecond = 80f, UpDbPerSecond = 20f;
        public const int MaxVoices = 2;

        static bool _clock;
        static float _fanfareUntil = -1, _bangUntil = -1;
        static readonly float[] _db = new float[Buses];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            _clock = false; _fanfareUntil = _bangUntil = -1;
            for (int i = 0; i < Buses; i++) _db[i] = 0;
            _ends.Clear();
        }

        /* The clock booster's freeze on or off. */
        public static void Clock(bool on) => _clock = on;
        public static bool ClockOn => _clock;
        /* The fanfare starts (its length, s). */
        public static void Fanfare(float seconds) { _fanfareUntil = Time.unscaledTime + Mathf.Max(.5f, seconds); Log(Bus.Fanfare, "kazanma-" + Music.FanfareClipKey, 1); }
        /* The win screen left (the music's next scene): the fanfare's duck ends with it. */
        public static void FanfareEnd() => _fanfareUntil = -1;
        /* A bomb's bang: the music steps back a little more (the loss's own duck under it). */
        public static void Bang() => _bangUntil = Time.unscaledTime + BangSeconds;

        /* Where each bus is heading (dB). */
        public static float TargetDb(Bus b)
        {
            if (!On) return 0;
            float db = 0, now = Time.unscaledTime;
            if (_clock) { if (b == Bus.Sfx) db += ClockSfxDb; if (b == Bus.Music) db += ClockMusicDb; }
            if (now < _fanfareUntil) { if (b == Bus.Sfx || b == Bus.Bed) db += FanfareSfxDb; if (b == Bus.Ui) db += FanfareUiDb; }
            if (now < _bangUntil && b == Bus.Music) db += BangMusicDb;
            return db;
        }

        /* Each frame (GameRoot.Update): the gains glide to their targets; the music's is handed on when it moves. */
        static float _musicSent = -1;
        public static void Tick(float dt)
        {
            for (int i = 0; i < Buses; i++)
            {
                float target = TargetDb((Bus)i);
                _db[i] = Mathf.MoveTowards(_db[i], target, dt * (target < _db[i] ? DownDbPerSecond : UpDbPerSecond));
            }
            BoosterSounds102.Mix();
            float m = Gain(Bus.Music);
            if (Mathf.Abs(m - _musicSent) > .005f) { _musicSent = m; Music.Mix(m); }
        }

        public static float Db(Bus b) => _db[(int)b];
        /* The bus's gain now (linear): its base times its duck. */
        public static float Gain(Bus b) => On ? Base(b) * Mathf.Pow(10, _db[(int)b] / 20) : 1;

        /* §C4: may this clip start now? At most MaxVoices of the same clip at once (by when each copy ends). */
        static readonly Dictionary<string, List<float>> _ends = new Dictionary<string, List<float>>();
        public static bool Admit(AudioClip clip, float pitch = 1)
        {
            if (clip == null) return false;
            if (!On) return true;
            float now = Time.unscaledTime;
            if (!_ends.TryGetValue(clip.name, out var l)) _ends[clip.name] = l = new List<float>();
            l.RemoveAll(e => e <= now);
            if (l.Count >= MaxVoices) return false;
            l.Add(now + clip.length / Mathf.Max(.25f, Mathf.Abs(pitch)));
            return true;
        }

        /* &seslog=1: every sound with its bus and final volume ([mix] bus clip volume; "~" a continuous one's level), for
           Tools/pm107_miks_olc.py. */
        public static bool Logging => Host.Current?.Query("seslog") == "1";
        public static void Log(Bus bus, string clip, float volume, bool continuous = false)
        {
            if (!Logging || string.IsNullOrEmpty(clip)) return;
            Debug.Log($"[mix] {(continuous ? "~" : "")}{bus} {clip} {volume.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)} {Time.unscaledTime:0.000}");
        }
        static readonly Dictionary<string, float> _cont = new Dictionary<string, float>(), _contAt = new Dictionary<string, float>();
        /* A continuous sound's level, logged when it moves by more than 1.5 dB (or starts or stops). */
        public static void LogLevel(Bus bus, string clip, float volume)
        {
            if (!Logging || string.IsNullOrEmpty(clip)) return;
            string key = bus + clip.Split('-')[0];
            _cont.TryGetValue(key, out float was);
            _contAt.TryGetValue(key, out float at);
            bool moved = (was <= .001f) != (volume <= .001f) || (volume > .001f && Mathf.Abs(20 * Mathf.Log10(volume / Mathf.Max(was, 1e-4f))) > 1.5f)
                || (volume > .001f && Time.unscaledTime - at > .3f);   // and every .3 s while it plays (its length in the log)
            if (!moved) return;
            _cont[key] = volume; _contAt[key] = Time.unscaledTime;
            Log(bus, clip, volume, true);
        }
    }
}
