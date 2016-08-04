using System.Collections.Generic;
using Entitas;

public class EntityEmittingSubSystem : IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public IEntity[] entities { get { return _entities; } }

    readonly Pool _pool;
    int _didExecute;
    IEntity[] _entities;

    public EntityEmittingSubSystem(Pool pool) {
        _pool = pool;
    }

    public TriggerOnEvent trigger { get { return Matcher.AllOf(new [] { CID.ComponentA }).OnEntityAdded(); } }

    public void Execute(List<IEntity> entities) {
        _pool.CreateEntity().AddComponent(CID.ComponentA, new ComponentA());
        _entities = entities.ToArray();
        _didExecute++;
    }
}

