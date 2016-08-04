﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entitas {

    public delegate void PoolChanged(Pool pool, Entity entity);
    public delegate void GroupChanged(Pool pool, Group group);

    /// Base interface for pools.
    /// 
    /// A pool manages the lifecycle of entities and groups.
    /// You can create and destroy entities and get groups of entities.
    /// The prefered way is to use the generated methods from the code generator to create a Pool, e.g. var pool = Pools.pool;
    public interface IPool<T> where T : IEntity {

        /// Occurs when an entity gets created.
        event PoolChanged OnEntityCreated;

        /// Occurs when an entity will be destroyed.
        event PoolChanged OnEntityWillBeDestroyed;

        /// Occurs when an entity got destroyed.
        event PoolChanged OnEntityDestroyed;

        /// Occurs when a group gets created for the first time.
        event GroupChanged OnGroupCreated;

        /// Occurs when a group gets cleared.
        event GroupChanged OnGroupCleared;

        /// Returns the sum of components that can be used in this pool.
        /// This value is generated by the code generator, e.g ComponentIds.TotalComponents.
        int totalComponents { get; }

        /// Returns all componentPools. componentPools is used to reuse removed components.
        /// Removed components will be pushed to the componentPool.
        /// Use entity.CreateComponent(index, type) to get a new or reusable component from the componentPool.
        Stack<IComponent>[] componentPools { get; }

        /// The metaData contains information about the pool.
        /// It's used to provide better error messages.
        PoolMetaData metaData { get; }

        /// Returns the number of entities in the pool.
        int count { get; }

        /// Returns the number of entities in the internal ObjectPool for entities which can be reused.
        int reusableEntitiesCount { get; }

        /// Returns the number of entities that are currently retained by other objects (e.g. Group, GroupObserver, ReactiveSystem).
        int retainedEntitiesCount { get; }

        /// Creates a new entity or gets a reusable entity from the internal ObjectPool for entities.
        T CreateEntity();

        /// Destroys the entity, removes all its components and pushs it back to the internal ObjectPool for entities.
        void DestroyEntity(T entity);

        /// Destroys all entities in the pool.
        void DestroyAllEntities();

        /// Determines whether the pool has the specified entity.
        bool HasEntity(T entity);

        /// Returns all entities which are currently in the pool.
        T[] GetEntities();

        /// Returns a group for the specified matcher.
        /// Calling pool.GetGroup(matcher) with the same matcher will always return the same instance of the group.
        Group GetGroup(IMatcher matcher);

        /// Clears all groups. This is useful when you want to soft-restart your application.
        void ClearGroups();

        /// Resets the creationIndex back to 0.
        void ResetCreationIndex();

        /// Clears the componentPool at the specified index.
        void ClearComponentPool(int index);

        /// Clears all componentPools.
        void ClearComponentPools();

        /// Resets the pool (clears all groups, destroys all entities and resets creationIndex back to 0).
        void Reset();

        string ToString();
    }
}
