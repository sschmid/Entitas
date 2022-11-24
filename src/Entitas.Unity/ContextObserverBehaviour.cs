using UnityEngine;

namespace Entitas.Unity
{
    [ExecuteInEditMode]
    public class ContextObserverBehaviour : MonoBehaviour
    {
        public ContextObserver ContextObserver => _contextObserver;

        ContextObserver _contextObserver;

        public void Init(ContextObserver contextObserver)
        {
            _contextObserver = contextObserver;
            Update();
        }

        void Update()
        {
            if (_contextObserver == null)
                gameObject.DestroyGameObject();
            else if (_contextObserver.GameObject != null)
                _contextObserver.GameObject.name = _contextObserver.ToString();
        }

        void OnDestroy() => _contextObserver.Deactivate();
    }
}
