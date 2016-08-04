using System.Collections.Generic;
using Entitas;

public class TestMultiReactiveSystem : IMultiReactiveSystem {
    public TriggerOnEvent[] triggers {
        get {
            return new [] {
                Matcher.AllOf(0).OnEntityAdded(),
                Matcher.AllOf(1).OnEntityAdded()
            };
        }
    }

    public void Execute(List<Entity> entities) {
    }
}
