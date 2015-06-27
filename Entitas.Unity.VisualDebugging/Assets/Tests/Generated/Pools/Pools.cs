using Entitas;

public static class Pools {

    static Pool _pool;

    public static Pool pool {
        get {
            if (_pool == null) {
                #if (UNITY_EDITOR)
                _pool = new Entitas.Unity.VisualDebugging.DebugPool(ComponentIds.TotalComponents, "Pool");
                #else
                _pool = new Pool(ComponentIds.TotalComponents);
                #endif
            }

            return _pool;
        }
    }
}