using System.Collections.Generic;
using Entitas;

public class MultiReactiveSubSystemSpy : IInitializeSystem, IMultiReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool initialized { get { return _initialized; } }

    public Entity[] entities { get { return _entities; } }

    readonly TriggerOnEvent[] _triggers;
    int _didExecute;
    bool _initialized;
    Entity[] _entities;

    public MultiReactiveSubSystemSpy(TriggerOnEvent[] triggers) {
        _triggers = triggers;
    }

    public TriggerOnEvent[] triggers { get { return _triggers; } }

    public void Initialize() {
        _initialized = true;
    }

    public void Execute(List<Entity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
