using Entitas;

public class InitializeSystemSpy : IInitializeSystem {
    public bool started { get { return _started; } }

    bool _started;

    public void Initialize() {
        _started = true;
    }
}

