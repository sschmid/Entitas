using System.Threading;

namespace Entitas {

    /// A JobSystem calls Execute(entities) with subsets of entities
    /// and distributes the workload over the specified amount of threads.
    /// Don't use the generated methods like AddXyz() and ReplaceXyz() when
    /// writing multi-threaded code in Entitas.
    public abstract class JobSystem<TEntity> : IExecuteSystem where TEntity : class, IEntity {

        readonly IGroup<TEntity> _group;
        readonly int _threads;

        protected JobSystem(IGroup<TEntity> group, int threads) {
            _group = group;
            _threads = threads;
        }

        public void Execute() {
            var threadsRunning = _threads;
            var entities = _group.GetEntities();
            var remainder = entities.Length % _threads;
            var slice = entities.Length / _threads;
            for (int t = 0; t < _threads; t++) {
                var from = t * slice;
                var to = t == _threads - 1
                    ? from + slice + remainder
                    : from + slice;

                if (to - from > 0) {
                    ThreadPool.QueueUserWorkItem(state => {
                        for (int i = from; i < to; i++) {
                            Execute(entities[i]);
                        }

                        Interlocked.Decrement(ref threadsRunning);
                    });
                } else {
                    Interlocked.Decrement(ref threadsRunning);
                }
            }

            while (threadsRunning != 0) {
            }
        }

        protected abstract void Execute(TEntity entity);
    }
}
