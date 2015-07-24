using System.Collections.Generic;

namespace Entitas {
    public interface IReactiveSystem : ISystem {
        IMatcher trigger { get; }

        GroupEventType eventType { get; }

        void Execute(List<Entity> entities);
    }
}

