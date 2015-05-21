using Entitas;
using System.Threading;

public class AReactiveSystem : IReactiveSystem {
    public IMatcher GetTriggeringMatcher() {
        return Matcher.MyString;
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        Thread.Sleep(4);
    }
}

