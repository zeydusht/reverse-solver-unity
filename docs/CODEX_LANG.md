# Codex job: English / Turkish texts (PM_MESAJ_19 §3–4)

Unity 6 C# project. You cannot run Unity; write code that compiles (C# 9). Keep the code style of the surrounding files.
`Assets/_Game/Presentation/Ui/Lang.cs` already exists: `Lang.Tr` (bool, PlayerPrefs "rs_lang", default English), `Lang.T(en, tr)`, event `Lang.Changed`.

## 1. Texts table
- `Assets/_Game/Presentation/Ui/Texts.cs`: turn every `const string` into `public static string X => Lang.T("<English>", "<Turkish>");` Keep the Turkish exactly as now, except:
  - Goal: "Save the Jiggles!" / "Jiggle'ları kurtar!"
  - Language button: "Language: English" / "Dil: Türkçe"; HapticsOn/Off: "Vibration: On/Off" / "Titreşim: Açık/Kapalı"; SoundOn/Off "Sound: On/Off".
- Move EVERY player-visible Turkish string literal in `Assets/_Game/Presentation/**/*.cs` into Texts with an English version (skip `Dev/`, comments, `Debug.Log`, the debug overlay, file names, ids). Format strings keep `{0}`. Fix call sites that used them as constants (switch cases, default parameter values: use `null` and resolve inside).
- Required wording: win title "Board cleared!" / "Tahta boşaldı!"; Next level / Sonraki bölüm; Play again / Tekrar oyna; Menu / Menü; Continue / Devam; Restart / Yeniden başla; Main menu / Ana menü; Start / Başla; "Did you like this level?" / "Bu bölümü beğendin mi?"; "How hard was it? 1 very easy · 10 very hard" / "Ne kadar zordu? 1 çok kolay · 10 çok zor"; Liked / Beğendim; Disliked / Beğenmedim. Natural, short English everywhere else (casual mobile game tone).
- Also the iOS name prompt texts in `Assets/_Game/Platform/Native/NativeHost.cs` (use Texts).

## 2. Level card texts
Level files carry Turkish card texts (`intro` / `bintro`: title, body, tip). Do not change the level files or `Assets/_Game/Core`.
- Create `Assets/_Game/Presentation/Resources/level_texts_en.json`: `{ "<level id>": { "intro": {"title":"","body":"","tip":""}, "bintro": {...} } }` with English translations for every level in `Assets/_Game/Levels/redesign.json`, `Assets/_Game/Levels/levels.json` and `Assets/_Game/Presentation/Resources/show.json` that has such cards.
- Add a small loader in Presentation (e.g. `Ui/LevelTexts.cs`) and use it where the cards are drawn: English when `!Lang.Tr` and a translation exists, else the level's own text.

## 3. Web name gate (`Assets/WebGLTemplates/ReverseSolver/index.html`)
- Texts in English by default, Turkish when `localStorage.getItem('rs_lang') === 'tr'`.
- Style: input field cream background `#F8EBD0`, text navy `#0B2A36`, `maxlength="12"`; the box/card inner background cream, the title teal `#1C7F8C`; no big yellow/orange areas. Keep the ids (`rs-name-in`, `rs-name-go`) and the behaviour.
- `Assets/_Game/Platform/Web/WebBridge.jslib`: add `RS_SetLang(code)` writing `localStorage.rs_lang` (try/catch). In the web host class in `Assets/_Game/Platform` (find the one that uses WebBridge), subscribe to `Lang.Changed` and call it with "tr"/"en" (only `#if UNITY_WEBGL && !UNITY_EDITOR`), and call it once at start too.

## 4. Player name
- Name max 12 characters wherever it is accepted (web gate, iOS prompt, C# side trim).
- Menu profile box name (`Screens.cs`, Task13, TextView "name"): shrink the font to fit the box's free width (~100 pt), min 10 pt; if still too long cut with "…".

## Do not touch
`Assets/_Game/Core`, telemetry payloads, `BoardView.cs`, `Card.cs`, shaders, `GameRoot.cs` logic beyond replacing literals, art files, levels.
EditMode tests: if a test asserts a Turkish UI text, set `Lang.Tr = true` in that test (or compare with Texts).

When done: reply with a SHORT list of changed files and anything you could not do. Nothing else.
