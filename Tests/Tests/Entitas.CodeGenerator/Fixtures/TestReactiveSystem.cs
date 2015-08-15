using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : IReactiveSystem {

    public TriggerOnEvent trigger { get { return default(TriggerOnEvent); } }

    public void Execute(List<Entity> entities) {
    }
}

