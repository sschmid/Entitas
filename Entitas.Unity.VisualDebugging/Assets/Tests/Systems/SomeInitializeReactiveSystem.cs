using System.Collections.Generic;
using Entitas;

public class SomeInitializeReactiveSystem : IInitializeSystem, IReactiveSystem {
    public IMatcher trigger { get { return Matcher.AllOf(0); } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Initialize() {
    }

    public void Execute(List<Entity> entities) {
    }
}

