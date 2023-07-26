using UnityEngine;

namespace Entitas.Unity
{
    public static class ContextObserverExtension
    {
#if !UNITY_EDITOR || ENTITAS_DISABLE_VISUAL_DEBUGGING
        [System.Diagnostics.Conditional("false")]
#endif
        public static void CreateContextObserver(this IContext context)
        {
            var contextObserver = new GameObject().AddComponent<ContextObserverBehaviour>();
            contextObserver.Initialize(context);
            Object.DontDestroyOnLoad(contextObserver.gameObject);
        }

        public static ContextObserverBehaviour FindContextObserver(this IContext context)
        {
            foreach (var observer in Object.FindObjectsOfType<ContextObserverBehaviour>())
                if (observer.Context == context)
                    return observer;

            return null;
        }
    }
}
