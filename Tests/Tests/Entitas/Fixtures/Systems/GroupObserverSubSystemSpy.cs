using System.Collections.Generic;
using Entitas;

public class GroupObserverSubSystemSpy : IInitializeSystem, IGroupObserverSystem {

    public int didExecute { get { return _didExecute; } }
    public bool initialized { get { return _initialized; } }
    public IEntity[] entities { get { return _entities; } }

    readonly GroupObserver _groupObserver;

    int _didExecute;
    bool _initialized;
    IEntity[] _entities;

    public GroupObserverSubSystemSpy(GroupObserver groupObserver) {
        _groupObserver = groupObserver;
    }

    public GroupObserver groupObserver { get { return _groupObserver; } }

    public void Initialize() {
        _initialized = true;
    }

    public void Execute(List<IEntity> entities) {
        _didExecute++;
        _entities = entities.ToArray();
    }
}
