using ReverseSolver.Presentation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReverseSolver.LevelEditor
{
    /* Reverse Solver -> Seviye Editörü. The editor runs in Play mode inside the
       game's own scene, so the board is drawn by the game's BoardView (pieces
       look exactly as in the game) and "Oyna" opens the design in the real
       game. GameRoot is switched off while editing and on while playing; the
       scene is reloaded between the two.

       The project runs with domain reload off, so everything static here is
       reset by hand when Play mode ends. */
    [InitializeOnLoad]
    public static class LevelEditorLauncher
    {
        public enum Mode { Off, Editing, Playing }

        const string OpenFlag = "rs.leveleditor.open";
        const int GameScene = 0;

        public static Mode Current { get; private set; }

        static LevelEditorLauncher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem("Reverse Solver/Seviye Editörü %#l", priority = 0)]
        public static void Open()
        {
            if (EditorApplication.isPlaying)
            {
                Begin();
                SceneManager.LoadScene(GameScene);
                return;
            }
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes[GameScene].path);
            SessionState.SetBool(OpenFlag, true);
            EditorApplication.EnterPlaymode();
        }

        /* Before the first scene loads: if the menu asked for the editor, take the scene over. */
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnPlayModeStart()
        {
            if (!SessionState.GetBool(OpenFlag, false)) return;
            SessionState.EraseBool(OpenFlag);
            Begin();
        }

        static void Begin()
        {
            Current = Mode.Editing;
            PlayOverride.Clear();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            LevelEditorSession.EnsureStarted();
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Current != Mode.Editing) return;
            var game = Object.FindAnyObjectByType<GameRoot>();
            if (game != null) game.enabled = false;              // before its Start: the game never boots
            LevelEditorRoot.Attach();
        }

        /* "Oyna": the draft as it is now, in the real game scene. The menu
           button or Esc comes back with the draft and its undo history intact. */
        public static void PlayDraft()
        {
            var s = LevelEditorSession.Current;
            if (s == null) return;
            PlayOverride.Set = s.PlaySet();
            PlayOverride.Index = 0;
            PlayOverride.Exit = BackToEditor;
            Current = Mode.Playing;
            SceneManager.LoadScene(GameScene);
        }

        public static void BackToEditor()
        {
            PlayOverride.Clear();
            Current = Mode.Editing;
            SceneManager.LoadScene(GameScene);
        }

        static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingPlayMode || Current == Mode.Off) return;
            var s = LevelEditorSession.Current;
            if (s != null && s.AskSaveOnExit()) s.Save();
            Current = Mode.Off;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            PlayOverride.Clear();
        }
    }
}
