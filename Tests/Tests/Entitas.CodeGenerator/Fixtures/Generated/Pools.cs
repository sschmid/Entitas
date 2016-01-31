using Entitas;

public static class Pools {

    static Pool[] _allPools;

    public static Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { test };
            }

            return _allPools;
        }
    }

    #pragma warning disable
    static Pool _test;

    public static Pool test {
        get {
            if (_test == null) {
//                _test = new Pool(TestComponentIds.TotalComponents, 0, new PoolMetaData("Test Pool", TestComponentIds.componentNames));
//                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
//                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_test, TestComponentIds.componentTypes);
//                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
//                #endif
            }

            return _test;
        }
    }
}