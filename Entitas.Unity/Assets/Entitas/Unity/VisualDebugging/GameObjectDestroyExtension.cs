using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    
    public static class GameObjectDestroyExtension {

        public static void DestroyGameObject(this GameObject gameObject) {

            #if (UNITY_EDITOR)

            if (Application.isPlaying) {
                Object.Destroy(gameObject);
            } else {
                Object.DestroyImmediate(gameObject);
            }

            #else

            Object.Destroy(gameObject);

            #endif
        }
    }
}
