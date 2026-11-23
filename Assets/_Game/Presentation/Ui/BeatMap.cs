using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_96 §5: the win fanfares' beats, Resources/Art/audio/pm96/vurus_haritasi.json, written by
       Tools/make_beatmap96.py from the fanfares themselves (onset detection): for each fanfare its length and the 5-7
       beats the win screen puts its events on (s from the file's start), their strengths and the strongest one. A new
       fanfare: run the tool again, the screen follows the new map. */
    public static class BeatMap
    {
        [System.Serializable] public sealed class Fanfare { public string letter; public float length; public float[] beats; public float[] strength; public int strong; }
        [System.Serializable] sealed class File { public Fanfare[] fanfares; }

        /* No map (or no such fanfare): PM_76's 1-second entrance as beats. */
        public static readonly Fanfare Fallback = new Fanfare
        {
            letter = "-", length = 1.15f, beats = new[] { 0f, .12f, .36f, .54f, .72f, .9f }, strength = new[] { 1f, 1, 1, 1, 1, 1 }, strong = 4,
        };

        static Fanfare[] _all;
        static Fanfare[] All
        {
            get
            {
                if (_all != null) return _all;
                var text = Resources.Load<TextAsset>("Art/audio/pm96/vurus_haritasi");
                _all = text == null ? new Fanfare[0] : JsonUtility.FromJson<File>(text.text)?.fanfares ?? new Fanfare[0];
                return _all;
            }
        }

        public static bool Has(char letter)
        {
            foreach (var f in All) if (!string.IsNullOrEmpty(f.letter) && f.letter[0] == letter) return true;
            return false;
        }

        public static Fanfare Of(char letter)
        {
            foreach (var f in All)
                if (!string.IsNullOrEmpty(f.letter) && f.letter[0] == letter && f.beats != null && f.beats.Length >= 3) return f;
            return Fallback;
        }
    }
}
