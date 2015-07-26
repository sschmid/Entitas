using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.AllOf(0); } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(List<Entity> entities) {
    }
}

