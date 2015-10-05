using Entitas;

public static class Pools {

    static Pool[] _allPools;

    public static Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { pool };
            }

            return _allPools;
        }
    }

    static Pool _pool;

    public static Pool pool {
        get {
            if (_pool == null) {
                _pool = new Pool(ComponentIds.TotalComponents);
//                #if (UNITY_EDITOR)
//                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_pool, ComponentIds.componentNames, ComponentIds.componentTypes, "Pool");
//                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
//                #endif
            }

            return _pool;
        }
    }
}