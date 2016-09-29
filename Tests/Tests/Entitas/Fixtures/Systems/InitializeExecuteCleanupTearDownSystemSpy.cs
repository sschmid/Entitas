using Entitas;

public class InitializeExecuteCleanupTearDownSystemSpy : ReactiveSubSystemSpyBase, IExecuteSystem {

    public void Execute() {
        Execute(null);
    }
}
