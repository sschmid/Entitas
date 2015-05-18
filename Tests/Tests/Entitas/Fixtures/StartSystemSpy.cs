using Entitas;

public class StartSystemSpy : IStartSystem {
    public bool started { get { return _started; } }

    bool _started;

    public void Start() {
        _started = true;
    }
}

