using UnityEngine;

namespace Entitas.Unity
{
    public static class ContextObserverExtension
    {
        public static ContextObserverBehaviour FindContextObserver(this IContext context)
        {
            var observers = Object.FindObjectsOfType<ContextObserverBehaviour>();
            foreach (var observer in observers)
            {
                if (observer.ContextObserver.Context == context)
                    return observer;
            }

            return null;
        }
    }
}
