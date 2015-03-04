using UnityEngine;
using Entitas;

namespace Entitas.Debug {
    public class EntityDebugBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }
        public Entity entity { get { return _entity; } }

        Pool _pool;
        Entity _entity;
        int _debugIndex;

        public void Init(Pool pool, Entity entity, int debugIndex) {
            _pool = pool;
            _entity = entity;
            _debugIndex = debugIndex;
            _entity.OnComponentAdded += onEntityChanged;
            _entity.OnComponentRemoved += onEntityChanged;
            updateName();
        }

        public void DestroyBehaviour() {
            _entity.OnComponentAdded -= onEntityChanged;
            _entity.OnComponentRemoved -= onEntityChanged;
            Destroy(gameObject);
        }

        void onEntityChanged(Entity e, int index, IComponent component) {
            if (!e.HasComponent(_debugIndex)) {
                DestroyBehaviour();
            } else {
                updateName();
            }
        }

        void updateName() {
            name = _entity.ToString();
        }
    }
}