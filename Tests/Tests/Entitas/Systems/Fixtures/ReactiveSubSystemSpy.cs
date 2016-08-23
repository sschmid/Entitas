using System;
using System.Collections.Generic;
using Entitas;

public interface IReactiveSubSystemSpy {
    int didExecute { get; }
    int didInitialize { get; }
    Entity[] entities { get; }
}

public class ReactiveSubSystemSpy : IReactiveSubSystemSpy, IInitializeSystem, IReactiveSystem, ICleanupSystem, IDeinitializeSystem {

    public int didExecute { get { return _didExecute; } }
    public int didInitialize { get { return _didInitialize; } }
    public int didCleanup { get { return _didCleanup; } }
    public int didDeinitialize { get { return _didDeinitialize; } }
    public Entity[] entities { get { return _entities; } }

    public Action<List<Entity>> executeAction;

    int _didExecute;
    int _didInitialize;
    int _didCleanup;
    int _didDeinitialize;
    Entity[] _entities;

    readonly IMatcher _matcher;
    readonly GroupEventType _eventType;

    public ReactiveSubSystemSpy(IMatcher matcher, GroupEventType eventType) {
        _matcher = matcher;
        _eventType = eventType;
    }

    public TriggerOnEvent trigger { get { return new TriggerOnEvent(_matcher, _eventType); } }

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute(List<Entity> entities) {
        _didExecute += 1;
        _entities = entities.ToArray();

        if (executeAction != null) {
            executeAction(entities);
        }
    }
    
    public void Cleanup() {
        _didCleanup += 1;
    }

    public void Deinitialize() {
        _didDeinitialize += 1;
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

