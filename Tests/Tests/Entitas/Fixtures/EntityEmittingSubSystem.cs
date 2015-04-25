using Entitas;
using System.Linq;

public class EntityEmittingSubSystem : IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public Entity[] entites { get { return _entites; } }

    readonly Pool _pool;
    int _didExecute;
    Entity[] _entites;

    public EntityEmittingSubSystem(Pool pool) {
        _pool = pool;
    }

    public IMatcher GetTriggeringMatcher() {
        return Matcher.AllOf(new [] { CID.ComponentA });
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        _pool.CreateEntity().AddComponent(CID.ComponentA, new ComponentA());
        _entites = entities.ToArray();
        _didExecute++;
    }
}

