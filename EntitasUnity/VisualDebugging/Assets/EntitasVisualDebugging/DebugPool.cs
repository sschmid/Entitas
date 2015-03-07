using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class DebugPool : Pool {
        public GameObject entitiesContainer { get { return _entitiesContainer.gameObject; } }

        int _debugIndex;
        Transform _entitiesContainer;

        public DebugPool(int totalComponents) : base(totalComponents + 1) {
            init(totalComponents);
        }

        public DebugPool(int totalComponents, int startCreationIndex) : base(totalComponents + 1, startCreationIndex) {
            init(totalComponents);
        }

        void init(int totalComponents) {
            _debugIndex = totalComponents;
            _entitiesContainer = new GameObject().transform;
            _entitiesContainer.gameObject.AddComponent<PoolDebugBehaviour>().Init(this);
            updateName();
        }

        public override Entity CreateEntity() {
            var entity = base.CreateEntity();
            addDebugComponent(entity);
            updateName();

            return entity;
        }

        public override void DestroyEntity(Entity entity) {
            var debugComponent = (DebugComponent)entity.GetComponent(_debugIndex);
            debugComponent.debugBehaviour.DestroyBehaviour();
            base.DestroyEntity(entity);
            updateName();
        }

        void addDebugComponent(Entity entity) {
            var debugBehaviour = new GameObject().AddComponent<EntityDebugBehaviour>();
            debugBehaviour.Init(this, entity, _debugIndex);
            debugBehaviour.transform.SetParent(_entitiesContainer, false);
            var debugComponent = new DebugComponent();
            debugComponent.debugBehaviour = debugBehaviour;
            entity.AddComponent(_debugIndex, debugComponent);
        }

        void updateName() {
            _entitiesContainer.name = "Debug Pool (" + Count + " entities, " + pooledEntitiesCount + " reusable)";
        }
    }
}