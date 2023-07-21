using Entitas;

public class InitializeSystemSpy : IInitializeSystem
{
    public int DidInitialize => _didInitialize;

    int _didInitialize;

    public void Initialize() => _didInitialize += 1;
}
