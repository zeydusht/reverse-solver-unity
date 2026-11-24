using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_103: moves the native music's fades every frame (Music; in the web build Music.jslib does it). */
    public sealed class MusicDriver : MonoBehaviour
    {
#if !(UNITY_WEBGL && !UNITY_EDITOR)
        void Update() => Music.Tick(Time.unscaledDeltaTime);
#endif
    }
}
