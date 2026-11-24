using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_95 §2: recorded sound candidates (Tools/make_sfx95.py, docs/art/pm95/LISANS.md) for the exit, the rubber knock,
       the last piece, the win jingle and the button tick. Each candidate is 1-4 takes in Resources/Art/audio/pm95
       (<kind>-<letter><n>); a flag picks the candidate (&sescikis=, &sescarpma=, &sesbitis=, &seskazan=, &sestik=, a
       letter; 0: the old sound), otherwise the default. The chosen takes are loaded when the game starts (no decode
       on the first play). A play picks a take at random, never the one just played, and shifts the pitch a little. */
    public static class Sfx95
    {
        /* PM_100 §2-3: Lift, the short soft lift-off of a jam that slides (the exit's own first 45 ms, its letter the
           exit's); Bomb and Unlock, the bomb's defuse and the lock's opening at the effects' level (pm100). */
        public enum Kind { Exit, Knock, Last, Win, Tick, Lift, Bomb, Unlock, LiftShort }
        static readonly string[] Keys = { "cikis", "carpma", "bitis", "kazan", "tik", "kalkis", "bomba", "kilit", "kalkisk" };
        /* PM_103: the exit, the knock and the tick are Zeyd's choices (SesSecim, ses.json); the lift-offs follow the exit;
           the rest have one sound each (the last piece's and the win's are no longer played). */
        static char DefaultOf(int k) => (Kind)k switch
        {
            Kind.Exit or Kind.Lift or Kind.LiftShort => SesSecim.Letter("cikis"),
            Kind.Knock => SesSecim.Letter("carpma"),
            Kind.Tick => '0',   // PM_105: the tick is gone (every button: UiSounds' tok)
            Kind.Win => 'b',
            _ => 'a',
        };
        static readonly float[] Pitch = { .04f, .03f, .02f, 0, .04f, .04f, .02f, .03f, .04f };   // PM_99: the exit +-4 %; PM_102: the knock +-3 %, its volume never random
        static AudioClip[][] _takes;
        static readonly char[] _letters = new char[9];
        static readonly int[] _last = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        /* PM_100 §3: &sfx= a factor on the game's effects (exit, lift, knock, bomb, lock; 1.5: +3.5 dB), for Zeyd to try
           before a new default. Above 1 the game plays a second copy with the first (GameRoot.PlaySfx): a source's volume
           stops at 1. */
        public static float Gain
        {
            get
            {
                string q = Host.Current?.Query("sfx");
                return !string.IsNullOrEmpty(q) && float.TryParse(q, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float g) ? Mathf.Clamp(g, 0, 3) : 1;
            }
        }

        /* The candidate's takes, null: the old sound. */
        static AudioClip[] Takes(Kind kind)
        {
            if (_takes == null) Load();
            return _takes[(int)kind];
        }

        public static void Load()
        {
            _takes = new AudioClip[Keys.Length][];
            for (int k = 0; k < Keys.Length; k++)
            {
                string q = (Kind)k == Kind.Tick ? null : Host.Current?.Query("ses" + Keys[k]);   // PM_105: no &sestik=
                char letter = string.IsNullOrEmpty(q) ? DefaultOf(k) : q[0];
                if ((Kind)k == Kind.Lift || (Kind)k == Kind.LiftShort) letter = _letters[(int)Kind.Exit] == '0' || ExitFixed ? '0' : Letter(Kind.Exit);   // the exit's own; PM_106: none (Block Out: silent until it lands)
                _letters[k] = letter;
                if (letter == '0') continue;
                var list = new System.Collections.Generic.List<AudioClip>();
                for (int n = 1; n <= 4; n++)
                {
                    var clip = Take(Keys[k], letter, n);
                    if (clip == null) break;
                    clip.LoadAudioData();
                    list.Add(clip);
                }
                if (list.Count == 0 && letter != DefaultOf(k))   // an unknown letter: the default
                    for (int n = 1; n <= 4; n++)
                    {
                        var clip = Take(Keys[k], DefaultOf(k), n);
                        if (clip == null) break;
                        clip.LoadAudioData();
                        list.Add(clip);
                    }
                _takes[k] = list.Count > 0 ? list.ToArray() : null;
            }
            var said = new System.Text.StringBuilder("[ses]");
            for (int k = 0; k < Keys.Length; k++)
                said.Append(' ').Append(Keys[k]).Append(' ').Append(_takes[k] == null ? "eski" : $"{_takes[k][0].name.Substring(Keys[k].Length + 1, 1)} ({_takes[k].Length} kayıt, {_takes[k][0].loadState})");
            Debug.Log(said.ToString());
        }

        public static bool On(Kind kind) => Takes(kind) != null;

        /* PM_106 §2: the exits q-u (Tools/make_sfx106.py, docs/art/pm106/BLOCKOUT_SES_ANALIZ.md) are Block Out's: one
           take, always the same pitch and level (no random pitch, none with the speed), and no lift-off before a jam's
           knock (silent until something happens). */
        public static bool ExitFixed
        {
            get
            {
                if (_takes == null) Load();
                var t = _takes[(int)Kind.Exit];
                if (t == null) return false;
                char l = t[0].name.Length > 6 ? t[0].name[6] : '0';   // cikis-<letter><n>
                return l >= 'q' && l <= 'u';
            }
        }

        /* PM_99: a take from PM_99's sounds (the soft exits f-j, the louder knock a) before PM_95's; PM_100: its own
           (the exits high-passed, the knock softened on top, all at the effects' level) before both. */
        static AudioClip Take(string key, char letter, int n) =>
            (letter >= 'q' && letter <= 'u' ? SesSecim.Load<AudioClip>($"Art/audio/pm106/{key}-{letter}{n}") : null) ??   // PM_106 §2: Block Out's manner
            (letter >= 'k' && letter <= 'p' ? SesSecim.Load<AudioClip>($"Art/audio/pm104/{key}-{letter}{n}") : null) ??   // PM_104 §2: the swooshes k-p
            SesSecim.Load<AudioClip>($"Art/audio/pm102/{key}-{letter}{n}") ??   // PM_102: the knock's takes evened out
            SesSecim.Load<AudioClip>($"Art/audio/pm100/{key}-{letter}{n}") ?? SesSecim.Load<AudioClip>($"Art/audio/pm99/{key}-{letter}{n}") ??
            SesSecim.Load<AudioClip>($"Art/audio/pm95/{key}-{letter}{n}");

        /* PM_98: the candidate's letter in play (an unknown one: the default; '0': the old sound). */
        public static char Letter(Kind kind)
        {
            var takes = Takes(kind);
            int k = (int)kind;
            if (takes == null) return '0';
            string name = takes[0].name;   // <key>-<letter><n>
            return name.Length > Keys[k].Length + 1 ? name[Keys[k].Length + 1] : _letters[k];
        }

        /* A take (not the last one played) and its pitch; null: the old sound. */
        public static AudioClip Pick(Kind kind, out float pitch)
        {
            pitch = 1;
            var takes = Takes(kind);
            if (takes == null) return null;
            int k = (int)kind, i;
            if (takes.Length == 1) i = 0;
            else if (_last[k] < 0) i = Random.Range(0, takes.Length);
            else { i = Random.Range(0, takes.Length - 1); if (i >= _last[k]) i++; }   // never the same take twice in a row
            _last[k] = i;
            pitch = kind == Kind.Exit && ExitFixed ? 1 : 1 + Random.Range(-Pitch[k], Pitch[k]);
            return takes[i];
        }

        public static float Length(Kind kind)
        {
            var takes = Takes(kind);
            return takes == null ? 0 : takes[0].length;
        }
    }
}
