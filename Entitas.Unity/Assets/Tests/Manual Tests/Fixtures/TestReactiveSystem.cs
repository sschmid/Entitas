using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : IReactiveSystem {

    public TriggerOnEvent trigger { get { return VisualDebuggingMatcher.Test.OnEntityAdded(); } }

    public void Execute(List<Entity> entities) {
    }
}
