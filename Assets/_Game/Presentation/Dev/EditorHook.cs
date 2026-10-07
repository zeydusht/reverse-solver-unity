#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ReverseSolver.Presentation.Dev
{
    /* Frame callbacks for editor-only tools that run in Play mode (the level
       editor). Unity will not add a MonoBehaviour defined in an editor
       assembly to a scene, so those tools hang their Update/OnGUI on this.
       Compiled out of every build. */
    public sealed class EditorHook : MonoBehaviour
    {
        public Action OnUpdate, OnGui, OnDestroyed;

        void Update() => OnUpdate?.Invoke();
        void OnGUI() => OnGui?.Invoke();
        void OnDestroy() => OnDestroyed?.Invoke();
    }
}
#endif
