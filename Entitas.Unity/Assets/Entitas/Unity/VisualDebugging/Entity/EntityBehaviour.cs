using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [ExecuteInEditMode]
    public class EntityBehaviour : MonoBehaviour {

        public Context context { get { return _context; } }
        public Entity entity { get { return _entity; } }

        Context _context;
        Entity _entity;
        string _cachedName;

        public void Init(Context context, Entity entity) {
            _context = context;
            _entity = entity;
            _entity.OnEntityReleased += onEntityReleased;
            Update();
        }

        void onEntityReleased(Entity e) {
            gameObject.DestroyGameObject();
        }

        void Update() {
            if(_entity != null && _cachedName != _entity.ToString()) {
                name = _cachedName = _entity.ToString();
            }
        }

        void OnDestroy() {
            if(_entity != null) {
                _entity.OnEntityReleased -= onEntityReleased;
            }
        }
    }
}
