using Entitas;

public static class Pools {

    static Pool _test;

    public static Pool test {
        get {
            if (_test == null) {
                #if (UNITY_EDITOR)
//                _test = new Entitas.Unity.VisualDebugging.DebugPool(TestComponentIds.TotalComponents, "Test Pool");
                #else
                _test = new Pool(TestComponentIds.TotalComponents);
                #endif
            }

            return _test;
        }
    }
}