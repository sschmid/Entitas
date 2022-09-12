using System.Collections.Generic;
using Entitas;

public class RenderPositionSystem : IReactiveSystem {
    public IMatcher trigger
    { get
        {
            return Matcher.AllOf(Matcher.Position, Matcher.View)
                ;
        }
    }

    public GroupEventType
    eventType { get { return
            GroupEventType.OnEntityAdded

            ; } }

    public void Execute(List<Entity> entities) {
    }
}
