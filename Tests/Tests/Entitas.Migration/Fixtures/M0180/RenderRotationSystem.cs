using Entitas;

public class RenderRotationSystem : IReactiveSystem
{
    public IMatcher GetTriggeringMatcher() {
        return Matcher.AllOf(CoreMatcher.Rotation, CoreMatcher.View);
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        // Do sth
    }
}
