using Entitas;

public class ReactiveSubSystemSpy : ReactiveSubSystemSpyBase, IReactiveSystem {

    readonly EntityCollector _collector;

    public ReactiveSubSystemSpy(EntityCollector collector) {
        _collector = collector;
    }

    public override EntityCollector GetTrigger(Pools pools) {
        return _collector;
    }
}

public class ClearReactiveSubSystemSpy : ReactiveSubSystemSpy, IClearReactiveSystem {
    public ClearReactiveSubSystemSpy(EntityCollector collector) :
        base(collector) {
    }
}

public class ReactiveEnsureSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public ReactiveEnsureSubSystemSpy(EntityCollector collector, IMatcher ensureComponent) :
        base(collector) {
        _ensureComponent = ensureComponent;
    }
}

public class ReactiveExcludeSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents {

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;

    public ReactiveExcludeSubSystemSpy(EntityCollector collector, IMatcher excludeComponent) :
        base(collector) {
        _excludeComponent = excludeComponent;
    }
}

public class ReactiveEnsureExcludeSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;

    public ReactiveEnsureExcludeSubSystemSpy(EntityCollector collector, IMatcher ensureComponent, IMatcher excludeComponent) :
        base(collector) {
        _ensureComponent = ensureComponent;
        _excludeComponent = excludeComponent;
    }
}
