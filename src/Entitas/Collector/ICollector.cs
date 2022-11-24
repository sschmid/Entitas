using System.Collections.Generic;

namespace Entitas
{
    public interface ICollector
    {
        int Count { get; }

        void Activate();
        void Deactivate();
        void ClearCollectedEntities();

        IEnumerable<TCast> GetCollectedEntities<TCast>() where TCast : class, IEntity;
    }

    public interface ICollector<TEntity> : ICollector where TEntity : class, IEntity
    {
        HashSet<TEntity> CollectedEntities { get; }
    }
}
