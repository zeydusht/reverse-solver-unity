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
        public enum Kind { Exit, Knock, Last, Win, Tick }
        static readonly string[] Keys = { "cikis", "carpma", "bitis", "kazan", "tik" };
        static readonly char[] Defaults = { 'f', 'a', 'a', 'b', 'b' };   // PM_96 §1: Zeyd's choices; PM_99 §2: the soft swish f (docs/art/pm99)
        static readonly float[] Pitch = { .04f, .05f, .02f, 0, .04f };   // PM_99: the exit +-4 %; the jingles stay near their key
        static AudioClip[][] _takes;
        static readonly char[] _letters = new char[5];
        static readonly int[] _last = { -1, -1, -1, -1, -1 };

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
                string q = Host.Current?.Query("ses" + Keys[k]);
                char letter = string.IsNullOrEmpty(q) ? Defaults[k] : q[0];
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
                if (list.Count == 0 && letter != Defaults[k])   // an unknown letter: the default
                    for (int n = 1; n <= 4; n++)
                    {
                        var clip = Take(Keys[k], Defaults[k], n);
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

        /* PM_99: a take from PM_99's sounds (the soft exits f-j, the louder knock a) before PM_95's. */
        static AudioClip Take(string key, char letter, int n) =>
            Resources.Load<AudioClip>($"Art/audio/pm99/{key}-{letter}{n}") ?? Resources.Load<AudioClip>($"Art/audio/pm95/{key}-{letter}{n}");

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
            pitch = 1 + Random.Range(-Pitch[k], Pitch[k]);
            return takes[i];
        }

        public static float Length(Kind kind)
        {
            var takes = Takes(kind);
            return takes == null ? 0 : takes[0].length;
        }
    }
}
