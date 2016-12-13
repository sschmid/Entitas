using System;
using Entitas;

public class ReactiveSubSystemSpy : ReactiveSubSystemSpyBase, IReactiveSystem {

    public TriggerOnEvent trigger { get { return new TriggerOnEvent(_matcher, _eventType); } }

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }
}

public class ClearReactiveSubSystemSpy : ReactiveSubSystemSpy, IClearReactiveSystem {
    public ClearReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) :
        base(matcher, eventType) {
    }
}

public class ReactiveEnsureSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }

    readonly IMatcher _ensureComponent;

    public ReactiveEnsureSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent) :
        base(matcher, eventType) {
        _ensureComponent = ensureComponent;
    }
}

public class ReactiveExcludeSubSystemSpy : ReactiveSubSystemSpy, IExcludeComponents {

    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _excludeComponent;

    public ReactiveExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher excludeComponent) :
        base(matcher, eventType) {
        _excludeComponent = excludeComponent;
    }
}

public class ReactiveEnsureExcludeSubSystemSpy : ReactiveSubSystemSpy, IEnsureComponents, IExcludeComponents {

    public IMatcher ensureComponents { get { return _ensureComponent; } }
    public IMatcher excludeComponents { get { return _excludeComponent; } }

    readonly IMatcher _ensureComponent;
    readonly IMatcher _excludeComponent;

    public ReactiveEnsureExcludeSubSystemSpy(IMatcher matcher, GroupEventType eventType, IMatcher ensureComponent, IMatcher excludeComponent) :
        base(matcher, eventType) {
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
