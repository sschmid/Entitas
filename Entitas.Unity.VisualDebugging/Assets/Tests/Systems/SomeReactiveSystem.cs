using Entitas;

public class SomeReactiveSystem : IReactiveSystem {
    public IMatcher GetTriggeringMatcher() {
        return Matcher.AllOf(0);
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
    }
}

