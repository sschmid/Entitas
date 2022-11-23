using System.Collections.Generic;

namespace Entitas
{
    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    public abstract class MultiReactiveSystem<TEntity, TContexts> : IReactiveSystem
        where TEntity : class, IEntity
        where TContexts : class, IContexts
    {
        readonly ICollector[] _collectors;
        readonly HashSet<TEntity> _collectedEntities = new HashSet<TEntity>();
        readonly List<TEntity> _buffer = new List<TEntity>();
        string _toStringCache;

        protected MultiReactiveSystem(TContexts contexts)
        {
            _collectors = GetTrigger(contexts);
        }

        protected MultiReactiveSystem(ICollector[] collectors)
        {
            _collectors = collectors;
        }

        /// Specify the collector that will trigger the ReactiveSystem.
        protected abstract ICollector[] GetTrigger(TContexts contexts);

        /// This will exclude all entities which don't pass the filter.
        protected abstract bool Filter(TEntity entity);

        protected abstract void Execute(List<TEntity> entities);

        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        public void Activate()
        {
            foreach (var collector in _collectors)
                collector.Activate();
        }

        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        public void Deactivate()
        {
            foreach (var collector in _collectors)
                collector.Deactivate();
        }

        /// Clears all accumulated changes.
        public void Clear()
        {
            foreach (var collector in _collectors)
                collector.ClearCollectedEntities();
        }

        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        public void Execute()
        {
            foreach (var collector in _collectors)
            {
                if (collector.Count != 0)
                {
                    _collectedEntities.UnionWith(collector.GetCollectedEntities<TEntity>());
                    collector.ClearCollectedEntities();
                }
            }

            foreach (var e in _collectedEntities)
            {
                if (Filter(e))
                {
                    e.Retain(this);
                    _buffer.Add(e);
                }
            }

            if (_buffer.Count != 0)
            {
                try
                {
                    Execute(_buffer);
                }
                finally
                {
                    foreach (var entity in _buffer)
                        entity.Release(this);

                    _collectedEntities.Clear();
                    _buffer.Clear();
                }
            }
        }

        public override string ToString() => _toStringCache ?? (_toStringCache = $"MultiReactiveSystem({GetType().Name})");

        ~MultiReactiveSystem() => Deactivate();
    }
}
