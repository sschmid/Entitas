using System.Collections.Generic;
using Entitas;

public class MultiReactiveSubSystemSpy : IStartSystem, IMultiReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool started { get { return _started; } }

    public Entity[] entities { get { return _entities; } }

    readonly IMatcher[] _matchers;
    readonly GroupEventType[] _eventTypes;
    int _didExecute;
    bool _started;
    Entity[] _entities;

    public MultiReactiveSubSystemSpy(IMatcher[] matchers, GroupEventType[] eventTypes) {
        _matchers = matchers;
        _eventTypes = eventTypes;
    }

    public IMatcher[] triggers { get { return _matchers; } }

    public GroupEventType[] eventTypes { get { return _eventTypes; } }

    public void Start() {
        _started = true;
    }

    public void Execute(List<Entity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
