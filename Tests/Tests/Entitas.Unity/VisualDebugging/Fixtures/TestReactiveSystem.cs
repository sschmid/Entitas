using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : IReactiveSystem {

    public EntityCollector GetTrigger(Pools pools) {
        return pools.test.CreateEntityCollector(Matcher.AllOf(0));
    }

    public void Execute(List<Entity> entities) {
    }
}
