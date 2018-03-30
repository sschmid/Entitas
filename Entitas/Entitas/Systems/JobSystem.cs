using System;
using System.Threading;

namespace Entitas {

    /// A JobSystem calls Execute(entities) with subsets of entities
    /// and distributes the workload over the specified amount of threads.
    /// Don't use the generated methods like AddXyz() and ReplaceXyz() when
    /// writing multi-threaded code in Entitas.
    public abstract class JobSystem<TEntity> : IExecuteSystem where TEntity : class, IEntity {

        readonly IGroup<TEntity> _group;
        readonly int _threads;
        readonly Job<TEntity>[] _jobs;

        int _threadsRunning;

        protected JobSystem(IGroup<TEntity> group, int threads) {
            _group = group;
            _threads = threads;
            _jobs = new Job<TEntity>[threads];
            for (int i = 0; i < _jobs.Length; i++) {
                _jobs[i] = new Job<TEntity>();
            }
        }

        public void Execute() {
            _threadsRunning = _threads;
            var entities = _group.GetEntities();
            var remainder = entities.Length % _threads;
            var slice = entities.Length / _threads + (remainder == 0 ? 0 : 1);
            for (int t = 0; t < _threads; t++) {
                var from = t * slice;
                var to = from + slice;
                if (to > entities.Length) {
                    to = entities.Length;
                }

                var job = _jobs[t];
                job.Set(entities, from, to);
                if (from != to) {
                    ThreadPool.QueueUserWorkItem(queueOnThread, _jobs[t]);
                } else {
                    Interlocked.Decrement(ref _threadsRunning);
                }
            }

            while (_threadsRunning != 0) {
            }

            foreach (var job in _jobs) {
                if (job.exception != null) {
                    throw job.exception;
                }
            }
        }

        void queueOnThread(object state) {
            var job = (Job<TEntity>)state;
            try {
                for (int i = job.from; i < job.to; i++) {
                    Execute(job.entities[i]);
                }
            } catch (Exception ex) {
                job.exception = ex;
            } finally {
                Interlocked.Decrement(ref _threadsRunning);
            }
        }

        protected abstract void Execute(TEntity entity);
    }

    class Job<TEntity> where TEntity : class, IEntity {

        public TEntity[] entities;
        public int from;
        public int to;
        public Exception exception;

        public void Set(TEntity[] entities, int from, int to) {
            this.entities = entities;
            this.from = from;
            this.to = to;
            exception = null;
        }
    }
}
