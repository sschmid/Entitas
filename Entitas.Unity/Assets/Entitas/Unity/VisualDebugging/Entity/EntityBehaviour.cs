using Entitas;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class EntityBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }
        public Entity entity { get { return _entity; } }

        Pool _pool;
        Entity _entity;

        public void Init(Pool pool, Entity entity) {
            _pool = pool;
            _entity = entity;
            _entity.OnEntityReleased += onEntityReleased;
            Update();
        }

        void onEntityReleased(Entity e) {
            Destroy(gameObject);
        }

        void Update() {
            name = _entity.ToString();
        }

        void OnDestroy() {
            _entity.OnEntityReleased -= onEntityReleased;
        }
    }
}