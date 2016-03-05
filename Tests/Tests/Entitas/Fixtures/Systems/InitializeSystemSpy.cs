using Entitas;

public class InitializeSystemSpy : IInitializeSystem {
    public bool initialized { get { return _initialized; } }

    bool _initialized;

    public void Initialize() {
        _initialized = true;
    }
}

