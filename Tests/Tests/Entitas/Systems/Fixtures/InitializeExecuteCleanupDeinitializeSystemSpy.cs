using Entitas;

public class InitializeExecuteCleanupDeinitializeSystemSpy : IInitializeSystem, IExecuteSystem, ICleanupSystem, IDeinitializeSystem {

    public int didInitialize { get { return _didInitialize; } }
    public int didExecute { get { return _didExecute; } }
    public int didCleanup { get { return _didCleanup; } }
    public int didDeinitialize { get { return _didDeinitialize; } }

    int _didInitialize;
    int _didExecute;
    int _didCleanup;
    int _didDeinitialize;

    public void Initialize() {
        _didInitialize += 1;
    }

    public void Execute() {
        _didExecute += 1;
    }

    public void Cleanup() {
        _didCleanup += 1;
    }

    public void Deinitialize() {
        _didDeinitialize += 1;
    }
}

