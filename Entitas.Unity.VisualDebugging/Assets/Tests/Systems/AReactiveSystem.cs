using Entitas;
using System.Threading;

public class AReactiveSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.MyString; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(Entity[] entities) {
        Thread.Sleep(4);
    }
}

