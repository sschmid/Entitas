using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class DebugPool : Pool {
        public GameObject entitiesContainer { get { return _entitiesContainer.gameObject; } }
        public Dictionary<IMatcher, Group> groups { get { return _groups; }}

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

        public override void DestroyAllEntities() {
            base.DestroyAllEntities();
            Object.Destroy(_entitiesContainer.gameObject);
        }

        public override Group GetGroup(IMatcher matcher) {
            var group = base.GetGroup(matcher);
            updateName();
            return group;
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
            _entitiesContainer.name = string.Format("Debug Pool ({0} entities, {1} reusable, {2} groups)", Count, pooledEntitiesCount, groups.Count);
        }
    }
}