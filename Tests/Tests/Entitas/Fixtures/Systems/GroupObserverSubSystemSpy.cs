using Entitas;

public class GroupObserverSubSystemSpy : ReactiveSubSystemSpyBase, IGroupObserverSystem {

    public GroupObserver groupObserver { get { return _groupObserver; } }

    readonly GroupObserver _groupObserver;

    public GroupObserverSubSystemSpy(GroupObserver groupObserver) {
        _groupObserver = groupObserver;
    }
}
