using Entitas;
using System.Linq;

public class ReactiveSubSystemSpy : IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public Entity[] entites { get { return _entites; } }

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;
    int _didExecute;
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

    public void Execute(Entity[] entities) {
        _didExecute++;
        _entites = entities.ToArray();
    }
}
