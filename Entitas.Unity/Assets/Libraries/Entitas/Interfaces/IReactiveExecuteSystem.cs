using System.Collections.Generic;

namespace Entitas {
    public interface IReactiveExecuteSystem : ISystem {
        void Execute(List<Entity> entities);
    }
}

