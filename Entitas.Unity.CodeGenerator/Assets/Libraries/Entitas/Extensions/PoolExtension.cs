using System;

namespace Entitas {

    public interface ISetPool {
        void SetPool(Pool pool);
    }

    public static class PoolExtension {
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        public static IStartSystem CreateStartSystem<T>(this Pool pool) where T: ISystem, new() {
            return pool.CreateStartSystem(typeof(T));
        }

        public static IStartSystem CreateStartSystem(this Pool pool, Type systemType) {
            var system = (ISystem)Activator.CreateInstance(systemType);

            var startSystem = system as IStartSystem;
            if (startSystem != null) {
                setPool(system, pool);
                return (IStartSystem)system;
            }

            throw new Exception("Cannot create IStartSystem for " + systemType);
        }

        public static IExecuteSystem CreateExecuteSystem<T>(this Pool pool) where T: ISystem, new() {
            return pool.CreateExecuteSystem(typeof(T));
        }

        public static IExecuteSystem CreateExecuteSystem(this Pool pool, Type systemType) {
            var system = (ISystem)Activator.CreateInstance(systemType);

            var executeSystem = system as IExecuteSystem;
            if (executeSystem != null) {
                setPool(system, pool);
                return (IExecuteSystem)system;
            }

            var reactiveSystem = system as IReactiveSystem;
            if (reactiveSystem != null) {
                setPool(system, pool);
                return new ReactiveSystem(pool, reactiveSystem);
            }

            throw new Exception("Cannot create IExecuteSystem for " + systemType);
        }

        static void setPool(ISystem system, Pool pool) {
            var poolSystem = system as ISetPool;
            if (poolSystem != null) {
                poolSystem.SetPool(pool);
            }
        }
    }
}

