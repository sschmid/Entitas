using System.Collections.Generic;
using Entitas;

public class MultiReactiveSubSystemSpy : IStartSystem, IMultiReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool started { get { return _started; } }

    public Entity[] entities { get { return _entities; } }

    readonly TriggerOnEvent[] _triggers;
    int _didExecute;
    bool _started;
    Entity[] _entities;

    public MultiReactiveSubSystemSpy(TriggerOnEvent[] triggers) {
        _triggers = triggers;
    }

    public TriggerOnEvent[] triggers { get { return _triggers; } }

    public void Start() {
        _started = true;
    }

    public void Execute(List<Entity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
