using Entitas;

public class ExecuteSystemSpy : IExecuteSystem
{
    public int DidExecute => _didExecute;

    int _didExecute;

    public void Execute() => _didExecute += 1;
}
