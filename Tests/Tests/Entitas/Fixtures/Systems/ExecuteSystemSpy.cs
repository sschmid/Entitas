using Entitas;

public class ExecuteSystemSpy : IExecuteSystem {
    public bool executed { get { return _executed; } }

    bool _executed;

    public void Execute() {
        _executed = true;
    }
}

