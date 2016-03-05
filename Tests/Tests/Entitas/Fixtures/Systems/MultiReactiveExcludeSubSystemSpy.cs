using Entitas;

public class MultiReactiveExcludeSubSystemSpy : MultiReactiveSubSystemSpy, IExcludeComponents {
    public IMatcher excludeComponents { get { return _excludeComponents; } }

    readonly IMatcher _excludeComponents;

    public MultiReactiveExcludeSubSystemSpy(TriggerOnEvent[] triggers, IMatcher excludeComponents) :
        base(triggers) {
        _excludeComponents = excludeComponents;
    }
}

