using Entitas;

public class StartExecuteSystemSpy : IStartSystem, IExecuteSystem {

    public bool started { get { return _started; } }
    public bool executed { get { return _executed; } }

    bool _started;
    bool _executed;

    public void Start() {
        _started = true;
    }

    public void Execute() {
        _executed = true;
    }
}

