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

public class ReactiveEnsureSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public ReactiveEnsureSubSystemSpy(EntityCollector collector, IMatcher ensureComponent) :
        base(collector) {
        _ensureComponent = ensureComponent;
    }
}

public class ReactiveExcludeSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents {

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;

    public ReactiveExcludeSubSystemSpy(EntityCollector collector, IMatcher excludeComponent) :
        base(collector) {
        _excludeComponent = excludeComponent;
    }
}

public class ReactiveEnsureExcludeSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;

    public ReactiveEnsureExcludeSubSystemSpy(EntityCollector collector, IMatcher ensureComponent, IMatcher excludeComponent) :
        base(collector) {
        _ensureComponent = ensureComponent;
        _excludeComponent = excludeComponent;
    }
}

public class ReactiveFilterEntitiesSubSystemSpy : ReactiveSubSystemSpy, IFilterEntities {

    readonly Func<Entity, bool> _filter;

    public bool filter(Entity entity) {
        return _filter(entity);
    }

    public ReactiveFilterEntitiesSubSystemSpy(IMatcher matcher, GroupEventType eventType, Func<Entity, bool> filter) :
        base(matcher, eventType) {
        _filter = filter;
    }
}

public class ReactiveEnsureFilterEntitiesSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IFilterEntities {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;
    readonly Func<Entity, bool> _filter;

    public bool filter(Entity entity) {
        return _filter(entity);
    }

    public ReactiveEnsureFilterEntitiesSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent, Func<Entity, bool> filter) :
        base(matcher, eventType) {
        _ensureComponent = ensureComponent;
        _filter = filter;
    }
}

public class ReactiveExcludeFilterEntitiesSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents, IFilterEntities {

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;
    readonly Func<Entity, bool> _filter;

    public bool filter(Entity entity) {
        return _filter(entity);
    }

    public ReactiveExcludeFilterEntitiesSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher excludeComponent, Func<Entity, bool> filter) :
        base(matcher, eventType) {
        _excludeComponent = excludeComponent;
        _filter = filter;
    }
}

public class ReactiveEnsureExcludeFilterEntitiesSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents, IFilterEntities {

    public IMatcher ensureComponents { get { return _ensureComponent; } }
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;
    readonly Func<Entity, bool> _filter;

    public bool filter(Entity entity) {
        return _filter(entity);
    }

    public ReactiveEnsureExcludeFilterEntitiesSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent, IMatcher excludeComponent, Func<Entity, bool> filter) :
        base(matcher, eventType) {
        _ensureComponent = ensureComponent;
        _excludeComponent = excludeComponent;
        _filter = filter;
    }
}
