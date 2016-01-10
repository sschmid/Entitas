using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class PoolObserver {
        public Pool pool { get { return _pool; } }
        public string name { get { return _name; } }
        public GameObject entitiesContainer { get { return _entitiesContainer.gameObject; } }
        public Group[] groups { get { return _groups.ToArray(); }}

        readonly Pool _pool;
        readonly string[] _componentNames;
        readonly Type[] _componentTypes;
        readonly string _name;
        readonly List<Group> _groups;
        readonly Transform _entitiesContainer;

        public PoolObserver(Pool pool, string[] componentNames, Type[] componentTypes, string name) {
            _pool = pool;
            _componentNames = componentNames;
            _componentTypes = componentTypes;
            _name = name;
            _groups = new List<Group>();
            _entitiesContainer = new GameObject().transform;
            _entitiesContainer.gameObject.AddComponent<PoolObserverBehaviour>().Init(this);

            _pool.OnEntityCreated += onEntityCreated;
            _pool.OnGroupCreated += onGroupCreated;
            _pool.OnGroupCleared += onGroupCleared;
        }

        void onEntityCreated(Pool pool, Entity entity) {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(_pool, entity, _componentNames, _componentTypes);
            entityBehaviour.transform.SetParent(_entitiesContainer, false);
        }

        void onGroupCreated(Pool pool, Group group) {
            _groups.Add(group);
        }

        void onGroupCleared(Pool pool, Group group) {
            _groups.Remove(group);
        }

        public override string ToString() {
            return _entitiesContainer.name = 
                _name + " (" +
                _pool.count + " entities, " +
                _pool.reusableEntitiesCount + " reusable, " +
                _pool.retainedEntitiesCount + " retained, " +
                _groups.Count + " groups)";
        }
    }
}