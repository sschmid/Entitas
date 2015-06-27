using Entitas;

public class TestReactiveSystem : IReactiveSystem {
    public IMatcher GetTriggeringMatcher() {
        return null;
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
    }
}

