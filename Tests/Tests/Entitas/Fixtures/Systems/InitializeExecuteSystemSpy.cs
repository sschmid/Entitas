using Entitas;

public class InitializeExecuteSystemSpy : IInitializeSystem, IExecuteSystem {

    public int didInitialize { get { return _didInitialize; } }
    public int didExecute { get { return _didExecute; } }

    int _didInitialize;
    int _didExecute;

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute() {
        _didExecute += 1;
    }
}

