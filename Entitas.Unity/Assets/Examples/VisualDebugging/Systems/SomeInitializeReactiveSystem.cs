using System.Collections.Generic;
using Entitas;

public class SomeInitializeReactiveSystem : IInitializeSystem, IReactiveSystem {
    public TriggerOnEvent trigger { get { return Matcher.AllOf(0).OnEntityAdded(); } }

    public void Initialize() {
    }

    public void Execute(List<Entity> entities) {
    }
}

