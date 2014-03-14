using ToolKit;
using System.Collections.Generic;

namespace Entitas {
    public class EntityRepository {
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        ulong _creationIndex;
        Entity[] _entitiesCache;

        public EntityRepository() {
            _creationIndex = 0;
        }

        public EntityRepository(ulong startCreationIndex) {
            _creationIndex = startCreationIndex;
        }

        public Entity CreateEntity() {
            var entity = new Entity(_creationIndex++);
            _entities.Add(entity);
            _entitiesCache = null;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.RemoveAllComponents();
            _entities.Remove(entity);
            _entitiesCache = null;
        }

        public void DestroyAllEntities() {
            var entities = GetEntities();
            foreach (var e in entities)
                DestroyEntity(e);
        }

        public bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null)
                _entitiesCache = _entities.ToArray();

            return _entitiesCache;
        }

        public Entity[] GetEntities(IEntityMatcher matcher) {
            var entities = GetEntities();
            var matchingEntities = new List<Entity>();
            foreach (var e in entities)
                if (matcher.Matches(e))
                    matchingEntities.Add(e);

            return matchingEntities.ToArray();
        }
    }
}

