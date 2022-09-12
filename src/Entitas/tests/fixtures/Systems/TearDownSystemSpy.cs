using Entitas;

public class TearDownSystemSpy : ITearDownSystem {

    public int didTearDown { get { return _didTearDown; } }

    int _didTearDown;

    public void TearDown() {
        _didTearDown += 1;
    }
}
