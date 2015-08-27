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

            _pool.OnEntityCreated += onEntityCreated;
            _pool.OnGroupCreated += onGroupCreated;
        }

        void onEntityCreated(Pool pool, Entity entity) {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(_pool, entity);
            entityBehaviour.transform.SetParent(_entitiesContainer, false);
        }

        void onGroupCreated(Pool pool, Group group) {
            _groups.Add(group);
        }

        public override string ToString() {
            return _entitiesContainer.name = 
                _name + " (" +
                _pool.Count + " entities, " +
                _pool.reusableEntitiesCount + " reusable, " +
                _groups.Count + " groups)";
        }
    }
}