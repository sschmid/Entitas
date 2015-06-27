using Entitas;

public static class Pools {

    static Pool _test;

    public static Pool test {
        get {
            if (_test == null) {
                #if (UNITY_EDITOR)
//                var pool = new Entitas.Unity.VisualDebugging.DebugPool(TestComponentIds.TotalComponents, "Test Pool");
//                UnityEngine.Object.DontDestroyOnLoad(pool.entitiesContainer);
//                _test = pool;
                #else
                _test = new Pool(TestComponentIds.TotalComponents);
                #endif
            }

            return _test;
        }
    }
}