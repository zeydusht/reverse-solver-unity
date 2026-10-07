using System.Runtime.CompilerServices;

// Game rules live here as plain C#: no UnityEngine, no MonoBehaviour, no
// engine clock or random. That keeps them runnable headless in the editor
// for the solver, Monte Carlo runs and EditMode tests.
[assembly: InternalsVisibleTo("ReverseSolver.Core.Tests")]
[assembly: InternalsVisibleTo("ReverseSolver.Editor")]
// The level editor's model builds LevelData directly (editor only, never in a build).
[assembly: InternalsVisibleTo("ReverseSolver.Editing")]
