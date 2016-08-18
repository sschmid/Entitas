using Entitas;

public class InitializeExecuteCleanupSystemSpy : IInitializeSystem, IExecuteSystem, ICleanupSystem {

    public int didInitialize { get { return _didInitialize; } }
    public int didExecute { get { return _didExecute; } }
    public int didCleanup { get { return _didCleanup; } }

    int _didInitialize;
    int _didExecute;
    int _didCleanup;

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute() {
        _didExecute += 1;
    }

    public void Cleanup() {
        _didCleanup += 1;
    }
}

