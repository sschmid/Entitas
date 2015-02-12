using System.Collections.Generic;

namespace Entitas {
    public struct EntityComponentPair {
        public Entity entity { get { return _entity; } }

        public IComponent component { get { return _component; } }

        readonly Entity _entity;
        readonly IComponent _component;

        public EntityComponentPair(Entity entity, IComponent component) {
            _entity = entity;
            _component = component;
        }
    }

    public class WillBeRemovedPoolObserver {
        public List<EntityComponentPair> collectedEntityComponentPairs { get { return _collectedEntityComponentPairs; } }

        readonly HashSet<Entity> _collectedEntities;
        readonly List<EntityComponentPair> _collectedEntityComponentPairs;
        readonly Group _group;
        readonly int _index;

        public WillBeRemovedPoolObserver(Pool pool, AllOfMatcher matcher) {
            if (matcher.indices.Length != 1) {
                throw new MatcherException(matcher);
            }

            _collectedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);
            _collectedEntityComponentPairs = new List<EntityComponentPair>();
            _group = pool.GetGroup(matcher);
            _index = matcher.indices[0];
            Activate();
        }

        public void Activate() {
            _group.OnEntityWillBeRemoved += addEntity;
        }

        public void Deactivate() {
            _group.OnEntityWillBeRemoved -= addEntity;
            _collectedEntities.Clear();
            _collectedEntityComponentPairs.Clear();
        }

        public void ClearCollectedEntites() {
            _collectedEntities.Clear();
            _collectedEntityComponentPairs.Clear();
        }

        void addEntity(Group group, Entity entity) {
            var added = _collectedEntities.Add(entity);
            if (added) {
                _collectedEntityComponentPairs.Add(new EntityComponentPair(entity, entity.GetComponent(_index)));
            }
        }
    }
}

