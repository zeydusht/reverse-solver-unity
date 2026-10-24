// skin.json ile aynı veriler; file:// incelemesi için fetch kullanılmaz.
window.PIECE_SKIN = {
  "version": 1,
  "name": "Petrol / sıcak porselen",
  "units": "pt",
  "body": { "texture": "body-256.png", "wrap": "repeat", "paintingOpacity": 0.75 },
  "outline": { "color": "#123F46", "widthPt": 2.2 },
  "bevel": {
    "widthPt": 1.8,
    "highlight": { "color": "#FFF4DA", "opacity": 0.85 },
    "shadow": { "color": "#19252D", "opacity": 0.38 }
  },
  "dropShadow": { "color": "#19252D99", "offsetYPt": 2, "blurPt": 2.5 },
  "selected": { "outlineColor": "#123F46", "glowColor": "#E3B65ACC", "glowRadiusPt": 3 },
  "dragging": { "scale": 1.06, "shadowOffsetYPt": 5 },
  "nailed": { "saturation": 0.25, "darken": 0.22, "outlineColor": "#19252D" }
};