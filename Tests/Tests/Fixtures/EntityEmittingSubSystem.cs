using Entitas;
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
        return Matcher.AllOf(new [] { CID.ComponentA });
    }

    public EntityCollectionEventType GetEventType() {
        return EntityCollectionEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        _repo.CreateEntity().AddComponent(CID.ComponentA, new ComponentA());
        _entites = entities.ToArray();
        _didExecute++;
    }
}

