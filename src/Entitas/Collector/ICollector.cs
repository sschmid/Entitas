using System.Collections.Generic;

namespace Entitas
{
    public interface ICollector
    {
        int Count { get; }

        void Activate();
        void Deactivate();
        void ClearCollectedEntities();
    }

    public interface ICollector<TEntity> : ICollector where TEntity : Entity
    {
        HashSet<TEntity> CollectedEntities { get; }
    }
}
