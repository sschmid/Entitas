using System.Collections.Generic;
using Entitas;

public class GroupObserverSubSystemSpy : IReactiveSubSystemSpy, IInitializeSystem, IGroupObserverSystem {

    public int didExecute { get { return _didExecute; } }
    public int didInitialize { get { return _didInitialize; } }
    public Entity[] entities { get { return _entities; } }

    int _didExecute;
    int _didInitialize;
    Entity[] _entities;

    readonly GroupObserver _groupObserver;

    public GroupObserverSubSystemSpy(GroupObserver groupObserver) {
        _groupObserver = groupObserver;
    }

    public GroupObserver groupObserver { get { return _groupObserver; } }

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute(List<Entity> entities) {
        _didExecute += 1;
        _entities = entities.ToArray();
    }
}
