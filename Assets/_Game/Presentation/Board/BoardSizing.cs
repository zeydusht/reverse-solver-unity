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
    }
}
