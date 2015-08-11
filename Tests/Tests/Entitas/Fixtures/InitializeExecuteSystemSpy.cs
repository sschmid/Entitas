using Entitas;

public class InitializeExecuteSystemSpy : IInitializeSystem, IExecuteSystem {

    public bool started { get { return _started; } }
    public bool executed { get { return _executed; } }

    bool _started;
    bool _executed;

    public void Initialize() {
        _started = true;
    }

    public void Execute() {
        _executed = true;
    }
}

