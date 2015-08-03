using Entitas;

public class MultiReactiveEnsureSubSystemSpy : MultiReactiveSubSystemSpy, IEnsureComponents {
    public IMatcher ensureComponents { get { return _excludeComponents; } }

    readonly IMatcher _excludeComponents;

    public MultiReactiveEnsureSubSystemSpy(IMatcher[] matchers, GroupEventType[] eventTypes, IMatcher ensureComponents) :
        base(matchers, eventTypes) {
        _excludeComponents = ensureComponents;
    }
}

