using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class PoolObserver {
        public Pool pool { get { return _pool; } }
        public Group[] groups { get { return _groups.ToArray(); }}
        public GameObject entitiesContainer { get { return _entitiesContainer.gameObject; } }

        readonly Pool _pool;
        readonly List<Group> _groups;
        readonly Transform _entitiesContainer;

        public PoolObserver(Pool pool) {
            _pool = pool;
            _groups = new List<Group>();
            _entitiesContainer = new GameObject().transform;
            _entitiesContainer.gameObject.AddComponent<PoolObserverBehaviour>().Init(this);

            _pool.OnEntityCreated += onEntityCreated;
            _pool.OnGroupCreated += onGroupCreated;
            _pool.OnGroupCleared += onGroupCleared;
        }

        public void Deactivate() {
            _pool.OnEntityCreated -= onEntityCreated;
            _pool.OnGroupCreated -= onGroupCreated;
            _pool.OnGroupCleared -= onGroupCleared;
        }

        void onEntityCreated(Pool pool, Entity entity) {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(pool, entity);
            entityBehaviour.transform.SetParent(_entitiesContainer, false);
        }

        void onGroupCreated(Pool pool, Group group) {
            _groups.Add(group);
        }

        void onGroupCleared(Pool pool, Group group) {
            _groups.Remove(group);
        }

        public override string ToString() {
            if (_pool.retainedEntitiesCount != 0) {
                return _entitiesContainer.name = 
                    _pool.metaData.poolName + " (" +
                    _pool.count + " entities, " +
                    _pool.reusableEntitiesCount + " reusable, " +
                    _pool.retainedEntitiesCount + " retained, " +
                    _groups.Count + " groups)";
            }

            return _entitiesContainer.name = 
                _pool.metaData.poolName + " (" +
                _pool.count + " entities, " +
                _pool.reusableEntitiesCount + " reusable, " +
                _groups.Count + " groups)";
        }
    }
}