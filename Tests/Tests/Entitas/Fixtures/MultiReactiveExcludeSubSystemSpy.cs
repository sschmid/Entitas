using Entitas;

public class MultiReactiveExcludeSubSystemSpy : MultiReactiveSubSystemSpy, IExcludeComponents {
    public IMatcher excludeComponents { get { return _excludeComponents; } }

    readonly IMatcher _excludeComponents;

    public MultiReactiveExcludeSubSystemSpy(IMatcher[] matchers, GroupEventType[] eventTypes, IMatcher excludeComponents) :
    base(matchers, eventTypes) {
        _excludeComponents = excludeComponents;
    }
}

