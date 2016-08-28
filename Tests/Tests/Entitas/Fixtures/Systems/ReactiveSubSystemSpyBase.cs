using System;
using System.Collections.Generic;
using Entitas;

public interface IReactiveSubSystemSpy {
    int didInitialize { get; }
    int didExecute { get; }
    int didCleanup { get; }
    int didDeinitialize { get; }
    Entity[] entities { get; }
}

public abstract class ReactiveSubSystemSpyBase : IReactiveSubSystemSpy, IInitializeSystem, ICleanupSystem, IDeinitializeSystem {

    public int didInitialize { get { return _didInitialize; } }
    public int didExecute { get { return _didExecute; } }
    public int didCleanup { get { return _didCleanup; } }
    public int didDeinitialize { get { return _didDeinitialize; } }
    public Entity[] entities { get { return _entities; } }

    public Action<List<Entity>> executeAction;

    protected int _didInitialize;
    protected int _didExecute;
    protected int _didCleanup;
    protected int _didDeinitialize;
    protected Entity[] _entities;

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute(List<Entity> entities) {
        _didExecute += 1;

        if (entities != null) {
            _entities = entities.ToArray();
        } else {
            _entities = null;
        }

        if(executeAction != null) {
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

