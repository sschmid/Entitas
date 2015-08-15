using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : IReactiveSystem {
    public TriggerOnEvent trigger { get { return Matcher.AllOf(0).OnEntityAdded(); } }

    public void Execute(List<Entity> entities) {
    }
}

