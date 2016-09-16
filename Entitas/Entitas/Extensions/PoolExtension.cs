using System;

namespace Entitas {

    /// Implement this interface if you want to create a system which needs a reference to a pool.
    /// Recommended way to create systems in general: pool.CreateSystem(new MySystem());
    /// Calling pool.CreateSystem(new MySystem()) will automatically inject the pool if ISetPool is implemented.
    /// It's recommended to pass in the pool as a dependency using ISetPool rather than using Pools.sharedInstance.pool directly within the system to avoid tight coupling.
    public interface ISetPool {
        void SetPool(Pool pool);
    }

    /// Implement this interface if you want to create a system which needs a reference to pools.
    /// Recommended way to create systems in general: pool.CreateSystem(new MySystem());
    /// Calling pool.CreateSystem(new MySystem()) will automatically inject the pools if ISetPools is implemented.
    /// It's recommended to pass in the pools as a dependency using ISetPools rather than using Pools.sharedInstance directly within the system to avoid tight coupling.
    public interface ISetPools {
        void SetPools(Pools pools);
    }

    public static class PoolExtension {

        /// Returns all entities matching the specified matcher.
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        /// This is the recommended way to create systems.
        /// It will inject the pool if ISetPool is implemented.
        /// It will inject the Pools.sharedInstance if ISetPools is implemented.
        /// It will automatically create a ReactiveSystem if it is a IReactiveSystem or IMultiReactiveSystem.
        public static ISystem CreateSystem(this Pool pool, ISystem system) {
            return CreateSystem(pool, system, Pools.sharedInstance);
        }

        /// This is the recommended way to create systems.
        /// It will inject the pool if ISetPool is implemented.
        /// It will inject the pools if ISetPools is implemented.
        public static ISystem CreateSystem(this Pool pool, ISystem system, Pools pools) {
            SetPool(system, pool);
            SetPools(system, pools);
            return system;
        }

        /// This is the recommended way to create systems.
        /// It will inject the pool if ISetPool is implemented.
        /// It will inject the pools if ISetPools is implemented.
        /// It will automatically create a ReactiveSystem if it is a IReactiveSystem or IMultiReactiveSystem.
        public static ISystem CreateSystem(this Pool pool, IReactiveExecuteSystem system) {
            return CreateSystem(pool, system, Pools.sharedInstance);
        }

        /// This is the recommended way to create systems.
        /// It will inject the pool if ISetPool is implemented.
        /// It will inject the pools if ISetPools is implemented.
        /// It will automatically create a ReactiveSystem if it is a IReactiveSystem or IMultiReactiveSystem.
        public static ISystem CreateSystem(this Pool pool, IReactiveExecuteSystem system, Pools pools) {
            SetPool(system, pool);
            SetPools(system, pools);

            var reactiveSystem = system as IReactiveSystem;
            if (reactiveSystem != null) {
                return new ReactiveSystem(pool, reactiveSystem);
            }
            var multiReactiveSystem = system as IMultiReactiveSystem;
            if (multiReactiveSystem != null) {
                return new ReactiveSystem(pool, multiReactiveSystem);
            }
            var groupObserverSystem = system as IGroupObserverSystem;
            if (groupObserverSystem != null) {
                return new ReactiveSystem(groupObserverSystem);
            }

            throw new EntitasException(
                "Could not create ReactiveSystem for " + system + "!",
                "The system has to implement IReactiveSystem, IMultiReactiveSystem or IGroupObserverSystem."
            );
        }

        /// This is the recommended way to create systems.
        /// It will inject the pools if ISetPools is implemented.
        public static ISystem CreateSystem(this Pools pools, ISystem system) {
            SetPools(system, pools);
            return system;
        }

        /// This is the recommended way to create systems.
        /// It will inject the pools if ISetPools is implemented.
        public static ISystem CreateSystem(this Pools pools, IReactiveExecuteSystem system) {
            SetPools(system, pools);

            var groupObserverSystem = system as IGroupObserverSystem;
            if (groupObserverSystem != null) {
                return new ReactiveSystem(groupObserverSystem);
            }

            throw new EntitasException(
                "Could not create ReactiveSystem for " + system + "!",
                "Only IGroupObserverSystem is supported for pools.CreateSystem(system)."
            );
        }

        [Obsolete("pools.CreateSystem(system) can not infer which pool to set for ISetPool!", true)]
        public static ISystem CreateSystem(this Pools pools, ISetPool system) {
            throw new EntitasException(
                "pools.CreateSystem(" + system + ") can not infer which pool to set for ISetPool!",
                "pools.CreateSystem(system) only supports IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem and IGroupObserverSystem."
            );
        }

        [Obsolete("pools.CreateSystem(system) can not infer which pool to use to create a ReactiveSystem!", true)]
        public static ISystem CreateSystem(this Pools pools, IReactiveSystem system) {
            throw new EntitasException(
                "pools.CreateSystem(" + system + ") can not infer which pool to use to create a ReactiveSystem!",
                "pools.CreateSystem(system) only supports IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem and IGroupObserverSystem."
            );
        }

        [Obsolete("pools.CreateSystem(system) can not infer which pool to use to create a ReactiveSystem!", true)]
        public static ISystem CreateSystem(this Pools pools, IMultiReactiveSystem system) {
            throw new EntitasException(
                "pools.CreateSystem(" + system + ") can not infer which pool to use to create a ReactiveSystem!",
                "pools.CreateSystem(system) only supports IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem and IGroupObserverSystem."
            );
        }

        /// This will set the pool if ISetPool is implemented.
        public static void SetPool(ISystem system, Pool pool) {
            var poolSystem = system as ISetPool;
            if (poolSystem != null) {
                poolSystem.SetPool(pool);
            }
        }

        /// This will set the pools if ISetPools is implemented.
        public static void SetPools(ISystem system, Pools pools) {
            var poolsSystem = system as ISetPools;
            if (poolsSystem != null) {
                poolsSystem.SetPools(pools);
            }
        }

        /// Creates a GroupObserver which observes all specified pools.
        /// This is useful when you want to create a GroupObserver for multiple pools
        /// which can be used with IGroupObserverSystem.
        public static GroupObserver CreateGroupObserver(this Pool[] pools, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            var groups = new Group[pools.Length];
            var eventTypes = new GroupEventType[pools.Length];

            for (int i = 0; i < pools.Length; i++) {
                groups[i] = pools[i].GetGroup(matcher);
                eventTypes[i] = eventType;
            }

            return new GroupObserver(groups, eventTypes);
        }
    }
}
