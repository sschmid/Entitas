using System.Linq;

namespace Entitas
{
    /// A ParallelSystem calls Execute(entities) with subsets of entities
    /// and distributes the workload over multiple threads.
    /// Don't use the generated methods like AddXyz() and ReplaceXyz() when
    /// writing multi-threaded code in Entitas.
    public abstract class ParallelSystem<TEntity> : IExecuteSystem where TEntity : class, IEntity
    {
        readonly IGroup<TEntity> _group;

        protected ParallelSystem(IGroup<TEntity> group)
        {
            _group = group;
        }

        public virtual void Execute()
        {
            _group
                .GetEntities()
                .AsParallel()
                .ForAll(Execute);
        }

        protected abstract void Execute(TEntity entity);
    }
}
