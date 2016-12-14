using System;
using Entitas;

public class ReactiveSubSystemSpy : ReactiveSubSystemSpyBase, IReactiveSystem {

    readonly EntityCollector _collector;

    public ReactiveSubSystemSpy(EntityCollector collector) {
        _collector = collector;
    }

    public override EntityCollector GetTrigger(Pools pools) {
        return _collector;
    }
}

public class ClearReactiveSubSystemSpy : ReactiveSubSystemSpy, IClearReactiveSystem {
    public ClearReactiveSubSystemSpy(EntityCollector collector) :
        base(collector) {
    }
}

public class ReactiveFilterEntitiesSubSystemSpy : ReactiveSubSystemSpy, IFilterEntities {

    readonly Func<Entity, bool> _filter;

    public bool filter(Entity entity) {
        return _filter(entity);
    }

    public ReactiveFilterEntitiesSubSystemSpy(EntityCollector collector, Func<Entity, bool> filter) :
        base(collector) {
        _filter = filter;
    }
}
