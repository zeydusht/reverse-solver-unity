using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public static class BoardSizing
    {
        public const float SafeMarginPt = 24f;
        public static float SideGap(float screenWidth, int columns) =>
            Mathf.Min(34f, Mathf.Max(SafeMarginPt, (screenWidth - 44f * columns) / 2f));

        public static float CellSize(float screenWidth, float innerHeight, int columns, int rows) =>
            Mathf.Floor(Mathf.Min((screenWidth - 2 * SideGap(screenWidth, columns)) / columns,
                (innerHeight - BoardLayout.VerticalReserve) / rows, BoardLayout.MaxCell));

        public static float CellSize(Vector2 screen, Vector4 safe, LevelData level) =>
            CellSize(screen.x - safe.x - safe.z, screen.y - safe.y - safe.w, level.Width, level.Height);

        public static float Left(float screenWidth, Vector4 safe, float boardWidth) =>
            (screenWidth + safe.x - safe.z - boardWidth) / 2;
    }
}
