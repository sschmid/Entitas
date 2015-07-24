using System.Collections.Generic;
using Entitas;

public class EntityEmittingSubSystem : IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public Entity[] entites { get { return _entites; } }

    readonly Pool _pool;
    int _didExecute;
    Entity[] _entites;

    public EntityEmittingSubSystem(Pool pool) {
        _pool = pool;
    }

    public IMatcher trigger { get { return Matcher.AllOf(new [] { CID.ComponentA }); } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(List<Entity> entities) {
        _pool.CreateEntity().AddComponent(CID.ComponentA, new ComponentA());
        _entites = entities.ToArray();
        _didExecute++;
    }
}

