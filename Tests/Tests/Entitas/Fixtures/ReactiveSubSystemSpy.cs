using Entitas;
using System.Linq;

public class ReactiveSubSystemSpy : IStartSystem, IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool started { get { return _started; } }

    public Entity[] entites { get { return _entites; } }

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;
    int _didExecute;
    bool _started;
    Entity[] _entites;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }

    public IMatcher GetTriggeringMatcher() {
        return _matcher;
    }

    public GroupEventType GetEventType() {
        return _eventType;
    }

    public void Start() {
        _started = true;
    }

    public void Execute(Entity[] entities) {
        _didExecute++;
        _entites = entities.ToArray();
    }
}
