using Entitas;

public class ReactiveEnsureExcludeSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;

    public ReactiveEnsureExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType,
        IMatcher ensureComponent, IMatcher excludeComponent) : base(matcher, eventType) {
        _ensureComponent = ensureComponent;
        _excludeComponent = excludeComponent;
    }
}

