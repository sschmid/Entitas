using Entitas;

public class InitializeExecuteCleanupDeinitializeSystemSpy : ReactiveSubSystemSpyBase, IExecuteSystem {

    public void Execute() {
        Execute(null);
    }
}

