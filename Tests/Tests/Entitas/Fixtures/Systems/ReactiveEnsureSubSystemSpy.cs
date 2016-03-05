using Entitas;

public class ReactiveEnsureSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public ReactiveEnsureSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent) : base(matcher, eventType) {
        _ensureComponent = ensureComponent;
    }
}

