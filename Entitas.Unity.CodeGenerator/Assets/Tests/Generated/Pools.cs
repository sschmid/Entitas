using Entitas;

public static class Pools {

    static Pool _pool;

    public static Pool pool {
        get {
            if (_pool == null) {
                #if (UNITY_EDITOR)
//                var pool = new Entitas.Unity.VisualDebugging.DebugPool(ComponentIds.TotalComponents, "Pool");
//                UnityEngine.Object.DontDestroyOnLoad(pool.entitiesContainer);
//                _pool = pool;
                #else
                _pool = new Pool(ComponentIds.TotalComponents);
                #endif
            }

            return _pool;
        }
    }
}