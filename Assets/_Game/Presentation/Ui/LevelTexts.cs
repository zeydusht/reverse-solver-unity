using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* English card copy is separate from authored levels and never mutates them. */
    public static class LevelTexts
    {
        static JsonNode _english;
        static bool _loaded;

        public static LevelIntro Get(LevelData level, bool booster)
        {
            var own = booster ? level.BoosterIntro : level.Intro;
            if (own == null || Lang.Tr) return own;
            if (!_loaded)
            {
                _loaded = true;
                var asset = Resources.Load<TextAsset>("level_texts_en");
                if (asset != null)
                    try { _english = JsonReader.Parse(asset.text); }
                    catch (JsonException) { }   // missing or invalid copy: use the level's own text
            }
            if (_english == null || !_english.TryGet(level.Id, out var entry) ||
                !entry.TryGet(booster ? "bintro" : "intro", out var card) || card.Kind != JsonKind.Object) return own;
            return new LevelIntro
            {
                Id = own.Id, Icon = own.Icon,
                Title = Field(card, "title", own.Title),
                Body = Field(card, "body", own.Body),
                Tip = Field(card, "tip", own.Tip)
            };
        }

        static string Field(JsonNode card, string key, string fallback) =>
            card.TryGet(key, out var value) && value.Kind == JsonKind.String ? value.AsString : fallback;
    }
}
