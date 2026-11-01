using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Runtime-created sprites share downloaded textures, and own only their crop geometry.
    public sealed class MenuPainting : MonoBehaviour
    {
        public Sprite Owned;
        void OnDestroy() { if (Owned != null) Destroy(Owned); }
    }
}
