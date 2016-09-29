using System.Collections.Generic;
using System.Threading;
using Entitas;

public class AReactiveSystem : IReactiveSystem {

    public TriggerOnEvent trigger { get { return VisualDebuggingMatcher.MyString.OnEntityAdded(); } }

    public void Execute(List<Entity> entities) {
        Thread.Sleep(2);
    }
}
