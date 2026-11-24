using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_105 §2: the obstacles' own sounds (Tools/make_sfx105.py, CC0: docs/art/pm105/LISANS.md), three candidates each
       (a-c) in Resources/Art/audio/pm105/<key>-<letter><n>:
         zincir      a chained pair's chain rattling as it moves (drag, swing): short grains, their rate and level follow
                     the chain's speed (BoardView.ChainMotion), none when it is still
         zincirger   the chain pulled taut at a chained pair's knock: a short metal "ting" and a rattle (with the knock)
         kilitdokun  a press on a locked (nailed) piece: a short locked rattle
         kilitac     a lock opening: a key turning and the lock dropping (0: PM_72's crack, Sfx95 kilit a)
         patlama     a bomb going off (the loss): a toy's pop and puff, not frightening; then the loss panel
         duvar       a knock against the red wall: a firmer "bonk" than the knock against a piece
       Default: ses.json (<key>), a flag overrides it (&seszincir=, &seszincirger=, &seskilitdokun=, &seskilitac=,
       &sespatlama=, &sesduvar=; a letter, 0: none / the old sound). The chosen takes load when the game starts; through
       SesSecim, so on the web they are raw samples (PM_104_EK: Safari decodes nothing). */
    public static class Sfx105
    {
        public enum Kind { Zincir, ZincirGer, KilitDokun, KilitAc, Patlama, Duvar }
        public static readonly string[] Keys = { "zincir", "zincirger", "kilitdokun", "kilitac", "patlama", "duvar" };
        public const string Folder = "Art/audio/pm105/";
        static AudioClip[][] _takes;
        static readonly int[] _last = { -1, -1, -1, -1, -1, -1 };
        /* Pitch spread (+-): the rattle's grains vary, the rest barely. */
        static readonly float[] Pitch = { .06f, .02f, .02f, 0, 0, .03f };

        public static char Letter(Kind kind)
        {
            string key = Keys[(int)kind];
            string q = Host.Current?.Query("ses" + key);
            if (!string.IsNullOrEmpty(q) && (q[0] == '0' || q[0] >= 'a' && q[0] <= 'c')) return q[0];
            return SesSecim.Letter(key, '0');
        }

        public static void Load()
        {
            _takes = new AudioClip[Keys.Length][];
            var said = new System.Text.StringBuilder("[ses105]");
            for (int k = 0; k < Keys.Length; k++)
            {
                char l = Letter((Kind)k);
                var list = new System.Collections.Generic.List<AudioClip>();
                if (l != '0')
                    for (int n = 1; n <= 6; n++)
                    {
                        var clip = SesSecim.Load<AudioClip>($"{Folder}{Keys[k]}-{l}{n}");
                        if (clip == null) break;
                        clip.LoadAudioData();
                        list.Add(clip);
                    }
                _takes[k] = list.Count > 0 ? list.ToArray() : null;
                said.Append(' ').Append(Keys[k]).Append(' ').Append(_takes[k] == null ? "yok" : $"{l} ({list.Count})");
            }
            Debug.Log(said.ToString());
        }

        public static bool On(Kind kind) { if (_takes == null) Load(); return _takes[(int)kind] != null; }

        /* A take (never the one just played) and its pitch; null: none chosen. */
        public static AudioClip Pick(Kind kind, out float pitch)
        {
            pitch = 1;
            if (_takes == null) Load();
            var takes = _takes[(int)kind];
            if (takes == null) return null;
            int k = (int)kind, i;
            if (takes.Length == 1) i = 0;
            else if (_last[k] < 0) i = Random.Range(0, takes.Length);
            else { i = Random.Range(0, takes.Length - 1); if (i >= _last[k]) i++; }
            _last[k] = i;
            pitch = 1 + Random.Range(-Pitch[k], Pitch[k]);
            return takes[i];
        }

        /* The rattle: a grain each time the chain has moved Step cells (so more grains the faster it goes), at a level
           that rises with its speed; a still chain (under Still cells/s) is silent. */
        public const float Still = 1.2f, Full = 9f, Step = .5f, MinGap = .07f, Quietest = .25f;
        public static float RattleLevel(float cellsPerSecond) =>
            cellsPerSecond <= Still ? 0 : Mathf.Lerp(Quietest, 1, Mathf.Clamp01((cellsPerSecond - Still) / (Full - Still)));

        /* Feed it the chain's speed every frame; true: play a grain now at `level`. */
        public sealed class Rattle
        {
            float _travel, _since = 99;
            public bool Tick(float cellsPerSecond, float dt, out float level)
            {
                level = RattleLevel(cellsPerSecond);
                _since += dt;
                if (level <= 0) { _travel = 0; return false; }
                _travel += cellsPerSecond * dt;
                if (_travel < Step || _since < MinGap) return false;
                _travel = 0; _since = 0;
                return true;
            }
        }
    }
}
