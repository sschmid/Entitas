using Entitas;

public class DeinitializeSystemSpy : IDeinitializeSystem {

    public int didDeinitialize { get { return _didDeinitialize; } }

    int _didDeinitialize;

    public void Deinitialize() {
        _didDeinitialize += 1;
    }
}

