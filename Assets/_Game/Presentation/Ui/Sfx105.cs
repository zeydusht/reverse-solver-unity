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
        /* PM_106 §3: the real chain (Tools/make_sfx106.py, Freesound CC0): zincir and zincirger d-f in pm106. */
        public const string Folder106 = "Art/audio/pm106/";
        public static string FolderOf(Kind kind, char letter) => (kind == Kind.Zincir || kind == Kind.ZincirGer) && letter >= 'd' && letter <= 'f' ? Folder106 : Folder;
        static AudioClip[][] _takes;
        static readonly int[] _last = { -1, -1, -1, -1, -1, -1 };
        /* Pitch spread (+-): the rattle's grains vary, the rest barely. */
        static readonly float[] Pitch = { .06f, .02f, .02f, 0, 0, .03f };

        public static char Letter(Kind kind)
        {
            string key = Keys[(int)kind];
            string q = Host.Current?.Query("ses" + key);
            char top = kind == Kind.Zincir || kind == Kind.ZincirGer ? 'f' : 'c';   // PM_106: the chain's d-f
            if (!string.IsNullOrEmpty(q) && (q[0] == '0' || q[0] >= 'a' && q[0] <= top)) return q[0];
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
                        var clip = SesSecim.Load<AudioClip>($"{FolderOf((Kind)k, l)}{Keys[k]}-{l}{n}");
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

        /* PM_106 §3: the real chain (d-f) is a bed, not grains: its loops play on and on while the chain moves, the
           level following the speed (RattleLevel, eased: up in Attack, down in Release), and at a loop's end, or on a
           new movement, another one (never the one just played) crossfades in over Cross. */
        public static bool Continuous
        {
            get
            {
                if (_takes == null) Load();
                var t = _takes[(int)Kind.Zincir];
                if (t == null) return false;
                char l = t[0].name.Length > 7 ? t[0].name[7] : 'a';   // zincir-<letter><n>
                return l >= 'd' && l <= 'f';
            }
        }
        public static AudioClip[] Takes(Kind kind) { if (_takes == null) Load(); return _takes[(int)kind]; }

        public sealed class Bed
        {
            public const float Attack = .05f, Release = .18f, Cross = .08f;
            float _vol; int _last = -1; bool _on;
            public float Volume => _vol;
            public int Last => _last;
            /* Each frame: the chain's speed, dt, how many loops, and the playing one's time left (s; below 0: none).
               Returns the loop to start now (crossfading from the one playing), -1: none; Volume 0: stop them. */
            public int Tick(float cellsPerSecond, float dt, int loops, float left)
            {
                float target = RattleLevel(cellsPerSecond);
                _vol = Mathf.MoveTowards(_vol, target, dt / (target > _vol ? Attack : Release));
                if (_vol <= .001f) { _vol = 0; _on = false; return -1; }
                if (_on && left >= Cross) return -1;
                _on = true;
                int i = loops <= 1 ? 0 : _last < 0 ? Random.Range(0, loops) : Random.Range(0, loops - 1);
                if (loops > 1 && _last >= 0 && i >= _last) i++;
                return _last = i;
            }
        }

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
