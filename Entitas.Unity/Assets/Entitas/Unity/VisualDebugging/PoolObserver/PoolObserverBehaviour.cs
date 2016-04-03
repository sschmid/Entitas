using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [ExecuteInEditMode]
    public class PoolObserverBehaviour : MonoBehaviour {
        public PoolObserver poolObserver { get { return _poolObserver; } }

        PoolObserver _poolObserver;

        public void Init(PoolObserver poolObserver) {
            _poolObserver = poolObserver;
            Update();
        }

        void Update() {
            if (_poolObserver == null) {
                gameObject.DestroyGameObject();
            } else if (_poolObserver.entitiesContainer != null) {
                _poolObserver.entitiesContainer.name = _poolObserver.ToString();
            }
        }

        void OnDestroy() {
            _poolObserver.Deactivate();
        }
    }
}