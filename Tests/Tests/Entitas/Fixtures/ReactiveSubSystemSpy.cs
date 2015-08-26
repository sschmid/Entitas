using System.Collections.Generic;
using Entitas;

public class ReactiveSubSystemSpy : IInitializeSystem, IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public bool initialized { get { return _initialized; } }

    public Entity[] entities { get { return _entities; } }

    public bool replaceComponentAOnExecute;

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;
    int _didExecute;
    bool _initialized;
    Entity[] _entities;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }

    public TriggerOnEvent trigger { get { return new TriggerOnEvent(_matcher, _eventType); } }

    public void Initialize() {
        _initialized = true;
    }

    public System.Action<List<Entity>> executeBlock;
    public void Execute(List<Entity> entities) {
	    if(executeBlock == null) {
			_didExecute++;
			_entities = entities.ToArray();
			
			if (replaceComponentAOnExecute) {
				var e = _entities[0];
				e.ReplaceComponentA(Component.A);
			}
        } else {
            executeBlock(entities);
        }
    }
}
