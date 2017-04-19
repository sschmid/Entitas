using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    [ExecuteInEditMode]
    public class EntityBehaviour : MonoBehaviour {

        public IContext context { get { return _context; } }
        public IEntity entity { get { return _entity; } }

        IContext _context;
        IEntity _entity;
        string _cachedName;

        public void Init(IContext context, IEntity entity) {
            _context = context;
            _entity = entity;
            _entity.OnEntityReleased += onEntityReleased;
            Update();
        }

        void onEntityReleased(IEntity e) {
            gameObject.DestroyGameObject();
        }

        void Update() {
            if (_entity != null && _cachedName != _entity.ToString()) {
                name = _cachedName = _entity.ToString();
            }
        }

        void OnDestroy() {
            if (_entity != null) {
                _entity.OnEntityReleased -= onEntityReleased;
            }
        }
    }
}
