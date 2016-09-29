using Entitas;

public class MultiReactiveSubSystemSpy : ReactiveSubSystemSpyBase, IMultiReactiveSystem {

    public TriggerOnEvent[] triggers { get { return _triggers; } }

    readonly TriggerOnEvent[] _triggers;

    public MultiReactiveSubSystemSpy(TriggerOnEvent[] triggers) {
        _triggers = triggers;
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

public class MultiReactiveEnsureExcludeSubSystemSpy : MultiReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponents; } }
    public IMatcher excludeComponents { get { return _excludeComponents; } }

    readonly IMatcher _ensureComponents;
    readonly IMatcher _excludeComponents;

    public MultiReactiveEnsureExcludeSubSystemSpy(TriggerOnEvent[] triggers, IMatcher ensureComponents, IMatcher excludeComponents) :
        base(triggers) {
        _ensureComponents = ensureComponents;
        _excludeComponents = excludeComponents;
    }
}
