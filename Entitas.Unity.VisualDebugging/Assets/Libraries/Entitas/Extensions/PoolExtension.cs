using System;

namespace Entitas {

    public interface ISetPool {
        void SetPool(Pool pool);
    }

    public static class PoolExtension {
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        public static ISystem CreateSystem<T>(this Pool pool) where T: ISystem, new() {
            return pool.CreateSystem(typeof(T));
        }

        public static ISystem CreateSystem(this Pool pool, Type systemType) {
            var system = (ISystem)Activator.CreateInstance(systemType);
            return pool.CreateSystem(system);
        }

        public static ISystem CreateSystem(this Pool pool, ISystem system) {
            setPool(system, pool);
            var reactiveSystem = system as IReactiveSystem;
            return reactiveSystem != null
                    ? new ReactiveSystem(pool, reactiveSystem)
                    : system;
        }

        static void setPool(ISystem system, Pool pool) {
            var poolSystem = system as ISetPool;
            if (poolSystem != null) {
                poolSystem.SetPool(pool);
            }
        }
    }
}

