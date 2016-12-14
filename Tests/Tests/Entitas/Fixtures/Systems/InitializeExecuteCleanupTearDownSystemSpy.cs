using Entitas;

public class InitializeExecuteCleanupTearDownSystemSpy : ReactiveSubSystemSpyBase, IExecuteSystem {

    public override EntityCollector GetTrigger(Pools pools) {
        return pools.test.CreateEntityCollector(Matcher.AllOf(CID.ComponentA));
    }

    public void Execute() {
        Execute(null);
    }
}
