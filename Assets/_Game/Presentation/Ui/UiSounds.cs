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
       frame) on a source the pause does not stop. Played: every sound, for the tests (docs/TITRESIM_HARITASI.md). */
    public static class UiSounds
    {
        public enum Kind { Tick, Toggle, Open, Close, Dull }
        public static System.Action<Kind> Played;

        static Kind? _press;
        static bool _open, _close;

        public static void Request(Kind kind)
        {
            if (kind == Kind.Open) { _open = true; return; }
            if (kind == Kind.Close) { _close = true; return; }
            if (_press == null || Rank(kind) > Rank(_press.Value)) _press = kind;
        }
        static int Rank(Kind k) => k == Kind.Dull ? 2 : k == Kind.Toggle ? 1 : 0;

        /* What this frame asked for (then cleared). */
        public static List<Kind> Take()
        {
            var list = new List<Kind>();
            if (_press.HasValue) list.Add(_press.Value);
            if (_open != _close) list.Add(_open ? Kind.Open : Kind.Close);
            _press = null; _open = _close = false;
            return list;
        }

        static readonly Dictionary<string, AudioClip[]> Clips = new Dictionary<string, AudioClip[]>();
        static readonly Dictionary<Kind, int> Last = new Dictionary<Kind, int>();

        static AudioClip[] Load(string prefix)
        {
            if (Clips.TryGetValue(prefix, out var c)) return c;
            var list = new List<AudioClip>();
            for (int n = 1; n <= 6; n++)
            {
                var clip = Resources.Load<AudioClip>($"Art/audio/pm98/{prefix}{n}");
                if (clip == null) break;
                clip.LoadAudioData(); list.Add(clip);
            }
            return Clips[prefix] = list.ToArray();
        }

        /* The tick follows the button tick candidate (&sestik=: a, b, c; 0 the old one). */
        static string Prefix(Kind kind) => kind switch
        {
            Kind.Tick => "ui-tick-" + Sfx95.Letter(Sfx95.Kind.Tick),
            Kind.Toggle => "ui-toggle", Kind.Open => "ui-open", Kind.Close => "ui-close", _ => "ui-dull",
        };

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
            pitch = 1 + Random.Range(-.04f, .04f);
            return takes[i];
        }
    }
}
