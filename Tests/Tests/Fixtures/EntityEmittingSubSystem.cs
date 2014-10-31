using Entitas;
using System.Collections.Generic;
using System.Linq;

public class EntityEmittingSubSystem : IReactiveSubEntitySystem {
    public int didExecute { get { return _didExecute; } }

    public Entity[] entites { get { return _entites; } }

    EntityRepository _repo;
    int _didExecute;
    Entity[] _entites;

    public EntityEmittingSubSystem(EntityRepository repo) {
        _repo = repo;
    }

    public IEntityMatcher GetTriggeringMatcher() {
        return EntityMatcher.AllOf(new [] { CP.ComponentA });
    }

    public EntityCollectionEventType GetEventType() {
        return EntityCollectionEventType.OnEntityAdded;
    }

    public void Execute(IList<Entity> entities) {
        _repo.CreateEntity().AddComponent(CP.ComponentA, new ComponentA());
        _entites = entities.ToArray();
        _didExecute++;
    }
}

