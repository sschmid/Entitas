using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class PoolObserverBehaviour : MonoBehaviour {
        public PoolObserver poolObserver { get { return _poolObserver; } }

        PoolObserver _poolObserver;

        public void Init(PoolObserver poolObserver) {
            _poolObserver = poolObserver;
        }
    }
}