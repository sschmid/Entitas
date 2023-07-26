using UnityEngine;

namespace Entitas.Unity
{
    public static class ContextObserverExtension
    {
#if ENTITAS_DISABLE_VISUAL_DEBUGGING || !UNITY_EDITOR
        [System.Diagnostics.Conditional("false")]
#endif
        public static void CreateContextObserver(this IContext context)
        {
            Object.DontDestroyOnLoad(new ContextObserver(context).gameObject);
        }

        public static ContextObserverBehaviour FindContextObserver(this IContext context)
        {
            var observers = Object.FindObjectsOfType<ContextObserverBehaviour>();
            for (var i = 0; i < observers.Length; i++)
            {
                var observer = observers[i];
                if (observer.contextObserver.context == context)
                    return observer;
            }

            return null;
        }
    }
}
