using System.Collections.Generic;
using Entitas;

public class SomeStartReactiveSystem : IStartSystem, IReactiveSystem {
    public TriggerOnEvent trigger { get { return Matcher.AllOf(0).OnEntityAdded(); } }

    public void Start() {
    }

    public void Execute(List<Entity> entities) {
    }
}

