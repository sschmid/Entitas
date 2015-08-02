using Entitas;

public class MultiReactiveEnsureSubSystemSpy : MultiReactiveSubSystemSpy, IEnsureComponents {
    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public MultiReactiveEnsureSubSystemSpy(IMatcher[] matchers, GroupEventType[] eventTypes, IMatcher ensureComponent) :
        base(matchers, eventTypes) {
        _ensureComponent = ensureComponent;
    }
}

