using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    public static class GameObjectDestroyExtension {

        public static void DestroyGameObject(this GameObject gameObject) {
            if (Application.isPlaying) {
                Object.Destroy(gameObject);
            } else {
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}
