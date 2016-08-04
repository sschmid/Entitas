using System.Collections.Generic;
using Entitas;

public class MultiReactiveSubSystemSpy : IInitializeSystem, IMultiReactiveSystem {

    public int didExecute { get { return _didExecute; } }
    public bool initialized { get { return _initialized; } }
    public IEntity[] entities { get { return _entities; } }

    readonly TriggerOnEvent[] _triggers;
    int _didExecute;
    bool _initialized;
    IEntity[] _entities;

    public MultiReactiveSubSystemSpy(TriggerOnEvent[] triggers) {
        _triggers = triggers;
    }

    public TriggerOnEvent[] triggers { get { return _triggers; } }

    public void Initialize() {
        _initialized = true;
    }

    public void Execute(List<IEntity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
