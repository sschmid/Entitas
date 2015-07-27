using Entitas;

public class RenderRotationSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.AllOf(CoreMatcher.Rotation, CoreMatcher.View); } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(Entity[] entities) {
        // Do sth
    }
}
