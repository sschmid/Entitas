using System.Collections.Generic;

namespace Entitas {
    public interface IReactiveSubEntityWillBeRemovedSystem {
        int GetTriggeringIndex();

        void Execute(List<EntityComponentPair> pairs);
    }
}

