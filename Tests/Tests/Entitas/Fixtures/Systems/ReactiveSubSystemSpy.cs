using Entitas;

public class ReactiveSubSystemSpy : ReactiveSubSystemSpyBase, IReactiveSystem {

    public TriggerOnEvent trigger { get { return new TriggerOnEvent(_matcher, _eventType); } }

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }
}

public class ClearReactiveSubSystemSpy : ReactiveSubSystemSpy, IClearReactiveSystem {
    public ClearReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) :
        base(matcher, eventType) {
    }
}

public class ReactiveEnsureSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public ReactiveEnsureSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent) :
        base(matcher, eventType) {
        _ensureComponent = ensureComponent;
    }
}

public class ReactiveExcludeSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents {

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;

    public ReactiveExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher excludeComponent) :
        base(matcher, eventType) {
        _excludeComponent = excludeComponent;
    }
}

public class ReactiveEnsureExcludeSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;

    public ReactiveEnsureExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent, IMatcher excludeComponent) :
        base(matcher, eventType) {
        _ensureComponent = ensureComponent;
        _excludeComponent = excludeComponent;
    }
}

