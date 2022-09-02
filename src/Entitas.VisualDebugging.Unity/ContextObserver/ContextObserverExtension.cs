using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    public static class ContextObserverExtension {

        public static ContextObserverBehaviour FindContextObserver(this IContext context) {
            var observers = Object.FindObjectsOfType<ContextObserverBehaviour>();
            for (int i = 0; i < observers.Length; i++) {
                var observer = observers[i];
                if (observer.contextObserver.context == context) {
                    return observer;
                }
            }

            return null;
        }
    }
}
