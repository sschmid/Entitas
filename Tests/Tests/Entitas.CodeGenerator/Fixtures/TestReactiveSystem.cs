using Entitas;

public class TestReactiveSystem : IReactiveSystem {

    public IMatcher trigger { get { return null; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(Entity[] entities) {
    }
}

