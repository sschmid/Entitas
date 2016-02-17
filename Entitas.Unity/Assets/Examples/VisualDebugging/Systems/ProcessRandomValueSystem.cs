using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : IReactiveSystem, ISetPool {
    public TriggerOnEvent trigger { get { return Matcher.MyFloat.OnEntityAdded(); } }

    Pool _pool;

    public void SetPool(Pool pool) {
        _pool = pool;
    }

    public void Execute(List<Entity> entities) {
        foreach (var e in entities) {
            _pool.DestroyEntity(e);
        }
    }
}

