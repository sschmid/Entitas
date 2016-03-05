using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : IReactiveSystem {

    public TriggerOnEvent trigger { get { return Matcher.AllOf(0).OnEntityAdded(); } }

    public void Execute(List<Entity> entities) {
    }
}

