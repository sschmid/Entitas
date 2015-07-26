using System.Collections.Generic;
using System.Threading;
using Entitas;

public class AReactiveSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.MyString; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(List<Entity> entities) {
        Thread.Sleep(4);
    }
}

