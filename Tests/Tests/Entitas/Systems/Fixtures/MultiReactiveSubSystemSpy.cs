using System.Collections.Generic;
using Entitas;

public class MultiReactiveSubSystemSpy : IReactiveSubSystemSpy, IInitializeSystem, IMultiReactiveSystem {

    public int didExecute { get { return _didExecute; } }
    public int didInitialize { get { return _didInitialize; } }
    public Entity[] entities { get { return _entities; } }

    int _didExecute;
    int _didInitialize;
    Entity[] _entities;

    readonly TriggerOnEvent[] _triggers;

    public MultiReactiveSubSystemSpy(TriggerOnEvent[] triggers) {
        _triggers = triggers;
    }

    public TriggerOnEvent[] triggers { get { return _triggers; } }

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute(List<Entity> entities) {
        _didExecute += 1;
        _entities = entities.ToArray();
    }
}

public class MultiReactiveEnsureSubSystemSpy : MultiReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponents; } }

    readonly IMatcher _ensureComponents;

    public MultiReactiveEnsureSubSystemSpy(TriggerOnEvent[] triggers, IMatcher ensureComponents) :
        base(triggers) {
        _ensureComponents = ensureComponents;
    }
}

public class MultiReactiveExcludeSubSystemSpy : MultiReactiveSubSystemSpy, IExcludeComponents {

    public IMatcher excludeComponents { get { return _excludeComponents; } }

    readonly IMatcher _excludeComponents;

    public MultiReactiveExcludeSubSystemSpy(TriggerOnEvent[] triggers, IMatcher excludeComponents) :
        base(triggers) {
        _excludeComponents = excludeComponents;
    }
}

