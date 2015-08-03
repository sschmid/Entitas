using Entitas;

public class ReactiveExcludeSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents {
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;

    public ReactiveExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher excludeComponent) : base(matcher, eventType) {
        _excludeComponent = excludeComponent;
    }
}

