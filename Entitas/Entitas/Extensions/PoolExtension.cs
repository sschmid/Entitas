using System;

namespace Entitas {

    /// Implement this interface if you want to create a system which needs a
    /// reference to a pool. Recommended way to create systems in general:
    /// pool.CreateSystem(new MySystem());
    /// Calling pool.CreateSystem(new MySystem()) will automatically inject
    /// the pool if ISetPool is implemented.
    /// It's recommended to pass in the pool as a dependency using ISetPool
    /// rather than using Pools.sharedInstance.pool directly within the system
    /// to avoid tight coupling.
    [Obsolete(
        "Since 0.36.0. Consider passing in the pool as a constructor argument."
    )]
    public interface ISetPool {
        void SetPool(Pool pool);
    }

    /// Implement this interface if you want to create a system which needs a
    /// reference to pools. Recommended way to create systems in general:
    /// pool.CreateSystem(new MySystem());
    /// Calling pool.CreateSystem(new MySystem()) will automatically inject
    /// the pools if ISetPools is implemented.
    /// It's recommended to pass in the pools as a dependency using ISetPools
    /// rather than using Pools.sharedInstance directly within the system
    /// to avoid tight coupling.
    [Obsolete(
        "Since 0.36.0. Consider passing in pools as a constructor argument."
    )]
    public interface ISetPools {
        void SetPools(Pools pools);
    }

    public static class PoolExtension {

        /// Returns all entities matching the specified matcher.
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        /// This will set the pool if ISetPool is implemented.
        public static void SetPool(ISystem system, Pool pool) {
            var poolSystem = system as ISetPool;
            if(poolSystem != null) {
                poolSystem.SetPool(pool);
            }
        }

        /// This will set the pools if ISetPools is implemented.
        public static void SetPools(ISystem system, Pools pools) {
            var poolsSystem = system as ISetPools;
            if(poolsSystem != null) {
                poolsSystem.SetPools(pools);
            }
        }

        public static EntityCollector CreateEntityCollector(this Pool pool, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(pool.GetGroup(matcher), eventType);
        }

        /// Creates an EntityCollector which observes all specified pools.
        /// This is useful when you want to create an EntityCollector
        /// for multiple pools which can be used with IReactiveSystem.
        public static EntityCollector CreateEntityCollector(
            this Pool[] pools,
            IMatcher matcher,
            GroupEventType eventType = GroupEventType.OnEntityAdded) {
            var groups = new Group[pools.Length];
            var eventTypes = new GroupEventType[pools.Length];

            for (int i = 0; i < pools.Length; i++) {
                groups[i] = pools[i].GetGroup(matcher);
                eventTypes[i] = eventType;
            }

            return new EntityCollector(groups, eventTypes);
        }

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exisintg components.
        public static Entity CloneEntity(this Pool pool,
                                         Entity entity,
                                         bool replaceExisting = false,
                                         params int[] indices) {
            var target = pool.CreateEntity();
            entity.CopyTo(target, replaceExisting, indices);
            return target;
        }
    }
}
