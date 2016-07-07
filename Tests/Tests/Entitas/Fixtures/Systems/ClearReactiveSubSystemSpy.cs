using Entitas;

public class ClearReactiveSubSystemSpy : ReactiveSubSystemSpy, IClearReactiveSystem {
    public ClearReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) :
        base(matcher, eventType) {
    }
}
