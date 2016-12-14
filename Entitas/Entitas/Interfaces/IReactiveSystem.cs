using System;
using System.Collections.Generic;

namespace Entitas {

    /// Implement this interface if you want to create a reactive system which
    /// is triggered by an EntityCollector.
    /// This can also be used to react to changes in multiple groups
    /// from different pools.
    public interface IReactiveSystem : ISystem {
        EntityCollector GetTrigger(Pools pools);
        void Execute(List<Entity> entities);
    }

    /// Implement this interface in combination with IReactiveSystem.
    /// It will ensure that all entities will match the specified matcher.
    /// This is useful when a component triggered the reactive system, but once
    /// the system gets executed the component already has been removed.
    /// Implementing IEnsureComponents can filter these enities.
    [Obsolete("Since 0.36.0. It's recommended to use IFilterEntities.")]
    public interface IEnsureComponents {
        IMatcher ensureComponents { get; }
    }

    /// Implement this interface in combination with IReactiveSystem.
    /// It will exclude all entities which match the specified matcher.
    /// To exclude multiple components use
    /// Matcher.AnyOf(ComponentX, ComponentY, ComponentZ).
    [Obsolete("Since 0.36.0. It's recommended to use IFilterEntities.")]
    public interface IExcludeComponents {
        IMatcher excludeComponents { get; }
    }

    /// Implement this interface in combination with IReactiveSystem.
    /// It will exclude all entities which don't pass the filter.
    public interface IFilterEntities {
        bool filter(Entity entity);
    }

    /// Implement this interface in combination with IReactiveSystem.
    /// If a system changes entities which in turn would trigger itself
    /// consider implementing IClearReactiveSystem
    /// which will ignore the changes made by the system.
    public interface IClearReactiveSystem {
    }
}
