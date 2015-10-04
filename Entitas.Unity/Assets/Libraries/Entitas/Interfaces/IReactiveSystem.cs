using System.Collections.Generic;

namespace Entitas {
    public interface IReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent trigger { get; }
    }

    public interface IMultiReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent[] triggers { get; }
    }

    public interface IReactiveExecuteSystem : ISystem {
        void Execute(List<Entity> entities);
    }

    public interface IEnsureComponents {
        IMatcher ensureComponents { get; }
    }

    public interface IExcludeComponents {
        IMatcher excludeComponents { get; }
    }

    public interface IClearReactiveSystem {
    }
}

