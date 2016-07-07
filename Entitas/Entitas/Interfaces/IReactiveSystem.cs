using System.Collections.Generic;

namespace Entitas {

    /// Implement this interface if you want to create a reactive system which is triggered by the specified trigger.
    public interface IReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent trigger { get; }
    }

    /// Implement this interface if you want to create a reactive system which is triggered by any of the specified triggers.
    public interface IMultiReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent[] triggers { get; }
    }

    /// Implement this interface if you want to create a reactive system which is triggered by a GroupObserver.
    /// This is useful when you want to react to changes in multiple groups from different pools.
    public interface IGroupObserverSystem : IReactiveExecuteSystem {
        GroupObserver groupObserver { get; }
    }

    /// Not meant to be implemented. Use either IReactiveSystem or IMultiReactiveSystem.
    public interface IReactiveExecuteSystem : ISystem {
        void Execute(List<Entity> entities);
    }

    /// Implement this interface in combination with IReactiveSystem or IMultiReactiveSystem.
    /// It will ensure that all entities will match the specified matcher.
    /// This is useful when a component triggered the reactive system, but once the system gets executed the component already has been removed.
    /// Implementing IEnsureComponents can filter these enities.
    public interface IEnsureComponents {
        IMatcher ensureComponents { get; }
    }

    /// Implement this interface in combination with IReactiveSystem or IMultiReactiveSystem.
    /// It will exclude all entities which match the specified matcher.
    /// To exclude multiple components use Matcher.AnyOf(ComponentX, ComponentY, ComponentZ).
    public interface IExcludeComponents {
        IMatcher excludeComponents { get; }
    }

    /// Implement this interface in combination with IReactiveSystem or IMultiReactiveSystem.
    /// If a system changes entities which in turn would trigger itself consider implementing IClearReactiveSystem
    /// which will ignore the changes made by the system.
    public interface IClearReactiveSystem {
    }
}

