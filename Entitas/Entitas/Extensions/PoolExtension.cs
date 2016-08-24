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
        /// It will automatically create a ReactiveSystem if it is a IReactiveSystem or IMultiReactiveSystem.
        public static ISystem CreateSystem(this Pool pool, ISystem system, Pools pools) {
            var poolSystem = system as ISetPool;
            if (poolSystem != null) {
                poolSystem.SetPool(pool);
            }
            var poolsSystem = system as ISetPools;
            if (poolsSystem != null) {
                poolsSystem.SetPools(pools);
            }
            var reactiveSystem = system as IReactiveSystem;
            if (reactiveSystem != null) {
                return new ReactiveSystem(pool, reactiveSystem);
            }
            var multiReactiveSystem = system as IMultiReactiveSystem;
            if (multiReactiveSystem != null) {
                return new ReactiveSystem(pool, multiReactiveSystem);
            }

            return system;
        }

        /// Creates a GroupObserver which observes all specified pools.
        /// This is useful when you want to create a GroupObserver for multiple pools
        /// which can be used with IGroupObserverSystem.
        public static GroupObserver CreateGroupObserver(this Pool[] pools, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            var groups = new Group[pools.Length];
            var eventTypes = new GroupEventType[pools.Length];

            for (int i = 0, poolsLength = pools.Length; i < poolsLength; i++) {
                groups[i] = pools[i].GetGroup(matcher);
                eventTypes[i] = eventType;
            }

            return new GroupObserver(groups, eventTypes);
        }
    }
}

