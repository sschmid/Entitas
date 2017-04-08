using Entitas;

public class ExecuteSystemSpy : IExecuteSystem {

    public int didExecute { get { return _didExecute; } }

    int _didExecute;

    public void Execute() {
        _didExecute += 1;
    }
}
