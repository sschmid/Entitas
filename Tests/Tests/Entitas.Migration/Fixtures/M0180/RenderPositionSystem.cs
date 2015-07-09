using Entitas;

public class RenderPositionSystem : IReactiveSystem {
    public IMatcher GetTriggeringMatcher(){return Matcher.AllOf(CoreMatcher.Position, CoreMatcher.View);
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        // Do sth
    }
}
