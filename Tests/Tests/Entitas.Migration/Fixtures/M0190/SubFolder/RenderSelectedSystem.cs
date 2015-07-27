using Entitas;

public class RenderSelectedSystem
    :
IReactiveSystem 
{
    public IMatcher GetTriggeringMatcher()
    {
        return 
            Matcher.AllOf(CoreMatcher.Selected, CoreMatcher.View);
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        // Do sth
    }
}
