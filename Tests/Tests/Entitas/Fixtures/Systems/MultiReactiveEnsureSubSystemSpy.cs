using Entitas;

public class MultiReactiveEnsureSubSystemSpy : MultiReactiveSubSystemSpy, IEnsureComponents {
    public IMatcher ensureComponents { get { return _excludeComponents; } }

    readonly IMatcher _excludeComponents;

    public MultiReactiveEnsureSubSystemSpy(TriggerOnEvent[] triggers, IMatcher ensureComponents) :
        base(triggers) {
        _excludeComponents = ensureComponents;
    }
}

