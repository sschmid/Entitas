using Entitas;

public class SomeStartReactiveSystem : IStartSystem, IReactiveSystem {
    public IMatcher GetTriggeringMatcher() {
        return Matcher.AllOf(0);
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Start() {
    }

    public void Execute(Entity[] entities) {
    }
}

