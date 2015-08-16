using Entitas;

public class InitializeExecuteSystemSpy : IInitializeSystem, IExecuteSystem {

    public bool initialized { get { return _initialized; } }
    public bool executed { get { return _executed; } }

    bool _initialized;
    bool _executed;

    public void Initialize() {
        _initialized = true;
    }

    public void Execute() {
        _executed = true;
    }
}

