using Entitas;

public class EntityCollectorSubSystemSpy : ReactiveSubSystemSpyBase, IEntityCollectorSystem {

    public EntityCollector entityCollector { get { return _collector; } }

    readonly EntityCollector _collector;

    public EntityCollectorSubSystemSpy(EntityCollector collector) {
        _collector = collector;
    }
}
