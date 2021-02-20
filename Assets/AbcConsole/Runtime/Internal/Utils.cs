using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AbcConsole.Internal
{
    public static class Utils
    {
        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) return null;
#endif

            var go = new GameObject("AbcConsoleCoroutineRunner");
            Object.DontDestroyOnLoad(go);

            IEnumerator Internal()
            {
                yield return enumerator;
                Object.Destroy(go);
            }

            var coroutineRunner = go.AddComponent<CoroutineRunner>();
            return coroutineRunner.StartCoroutine(Internal());
        }
    }
}