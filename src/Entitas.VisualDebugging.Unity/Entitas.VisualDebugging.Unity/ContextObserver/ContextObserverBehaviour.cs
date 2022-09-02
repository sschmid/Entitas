using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    [ExecuteInEditMode]
    public class ContextObserverBehaviour : MonoBehaviour {

        public ContextObserver contextObserver { get { return _contextObserver; } }

        ContextObserver _contextObserver;

        public void Init(ContextObserver contextObserver) {
            _contextObserver = contextObserver;
            Update();
        }

        void Update() {
            if (_contextObserver == null) {
                gameObject.DestroyGameObject();
            } else if (_contextObserver.gameObject != null) {
                _contextObserver.gameObject.name = _contextObserver.ToString();
            }
        }

        void OnDestroy() {
            _contextObserver.Deactivate();
        }
    }
}
