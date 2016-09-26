using Entitas;

public class GroupObserverSubSystemSpy : ReactiveSubSystemSpyBase, IGroupObserverSystem {

    public EntityCollector groupObserver { get { return _groupObserver; } }

    readonly EntityCollector _groupObserver;

    public GroupObserverSubSystemSpy(EntityCollector groupObserver) {
        _groupObserver = groupObserver;
    }
}
