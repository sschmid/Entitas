using Entitas;

public class InitializeSystemSpy : IInitializeSystem {

    public int didInitialize { get { return _didInitialize; } }

    int _didInitialize;

    public void Initialize() {
        _didInitialize += 1;
    }
}
