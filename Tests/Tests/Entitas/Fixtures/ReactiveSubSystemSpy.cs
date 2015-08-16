using System.Collections.Generic;
using Entitas;

public class ReactiveSubSystemSpy : IInitializeSystem, IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool started { get { return _started; } }

    public Entity[] entities { get { return _entities; } }

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;
    int _didExecute;
    bool _started;
    Entity[] _entities;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }

    public TriggerOnEvent trigger { get { return new TriggerOnEvent(_matcher, _eventType); } }

    public void Initialize() {
        _started = true;
    }

    public void Execute(List<Entity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
