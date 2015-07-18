using UnityEngine;
using System.Collections.Generic;

namespace Entitas.Unity.VisualDebugging {
    public class PoolObserver {
        public Pool pool { get { return _pool; } }
        public string name { get { return _name; } }
        public GameObject entitiesContainer { get { return _entitiesContainer.gameObject; } }
        public Group[] groups { get { return _groups.ToArray(); }}

        readonly Pool _pool;
        readonly string _name;
        readonly List<Group> _groups;
        readonly Transform _entitiesContainer;

        public PoolObserver(Pool pool, string name = "Pool") {
            _pool = pool;
            _name = name;
            _groups = new List<Group>();
            _entitiesContainer = new GameObject().transform;
            _entitiesContainer.gameObject.AddComponent<PoolObserverBehaviour>().Init(this);
            updateName();

            _pool.OnEntityCreated += onEntityCreated;
            _pool.OnEntityDestroyed += onEntityDestroyed;
            _pool.OnGroupCreated += onGroupCreated;
        }

        void onEntityCreated(Pool pool, Entity entity) {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(_pool, entity);
            entityBehaviour.transform.SetParent(_entitiesContainer, false);
            updateName();
        }

        void onEntityDestroyed(Pool pool, Entity entity) {
            updateName();
        }

        void onGroupCreated(Pool pool, Group group) {
            _groups.Add(group);
            updateName();
        }

        void updateName() {
            if (_entitiesContainer != null) {
                _entitiesContainer.name = string.Format(_name + " ({0} entities, {1} reusable, {2} groups)", _pool.Count, _pool.pooledEntitiesCount, _groups.Count);
            }
        }
    }
}