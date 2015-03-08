using UnityEngine;
using Entitas;

namespace Entitas.Unity.VisualDebugging {
    public class EntityDebugBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }
        public Entity entity { get { return _entity; } }
        public bool[] unfoldedComponents { get { return _unfoldedComponents; } }

        Pool _pool;
        Entity _entity;
        int _debugIndex;
        bool[] _unfoldedComponents;

        public void Init(Pool pool, Entity entity, int debugIndex) {
            _pool = pool;
            _entity = entity;
            _debugIndex = debugIndex;
            _unfoldedComponents = new bool[_pool.totalComponents];
            _entity.OnComponentAdded += onEntityChanged;
            _entity.OnComponentRemoved += onEntityChanged;
            updateName();

            UnfoldAllComponents();
        }

        public void UnfoldAllComponents() {
            for (int i = 0; i < _unfoldedComponents.Length; i++) {
                _unfoldedComponents[i] = true;
            }
        }

        public void FoldAllComponents() {
            for (int i = 0; i < _unfoldedComponents.Length; i++) {
                _unfoldedComponents[i] = false;
            }
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