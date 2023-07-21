using Entitas;

public class TearDownSystemSpy : ITearDownSystem
{
    public int DidTearDown => _didTearDown;

    int _didTearDown;

    public void TearDown() => _didTearDown += 1;
}
