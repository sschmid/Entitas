using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolsGenerator : nspec {

    bool logResults = false;

    const string defaultPool = @"using Entitas;

public static class Pools {

    static Pool _pool;

    public static Pool pool {
        get {
            if (_pool == null) {
                #if (UNITY_EDITOR)
                var pool = new Entitas.Unity.VisualDebugging.DebugPool(ComponentIds.TotalComponents, ""Pool"");
                DontDestroyOnLoad(pool.entitiesContainer);
                _pool = pool;
                #else
                _pool = new Pool(ComponentIds.TotalComponents);
                #endif
            }

            return _pool;
        }
    }
}";

    const string metaPool = @"using Entitas;

public static class Pools {

    static Pool _meta;

    public static Pool meta {
        get {
            if (_meta == null) {
                #if (UNITY_EDITOR)
                var pool = new Entitas.Unity.VisualDebugging.DebugPool(MetaComponentIds.TotalComponents, ""Meta Pool"");
                DontDestroyOnLoad(pool.entitiesContainer);
                _meta = pool;
                #else
                _meta = new Pool(MetaComponentIds.TotalComponents);
                #endif
            }

            return _meta;
        }
    }
}";

    const string metaCorePool = @"using Entitas;

public static class Pools {

    static Pool _meta;

    public static Pool meta {
        get {
            if (_meta == null) {
                #if (UNITY_EDITOR)
                var pool = new Entitas.Unity.VisualDebugging.DebugPool(MetaComponentIds.TotalComponents, ""Meta Pool"");
                DontDestroyOnLoad(pool.entitiesContainer);
                _meta = pool;
                #else
                _meta = new Pool(MetaComponentIds.TotalComponents);
                #endif
            }

            return _meta;
        }
    }

    static Pool _core;

    public static Pool core {
        get {
            if (_core == null) {
                #if (UNITY_EDITOR)
                var pool = new Entitas.Unity.VisualDebugging.DebugPool(CoreComponentIds.TotalComponents, ""Core Pool"");
                DontDestroyOnLoad(pool.entitiesContainer);
                _core = pool;
                #else
                _core = new Pool(CoreComponentIds.TotalComponents);
                #endif
            }

            return _core;
        }
    }
}";

    void generates(string[] poolNames, string fileContent) {
        var files = new PoolsGenerator().Generate(poolNames);
        files.Length.should_be(1);
        var file = files[0];
        if (logResults) {
            Console.WriteLine("should:\n" + fileContent);
            Console.WriteLine("was:\n" + file.fileContent);
        }

        file.fileName.should_be("Pools");
        file.fileContent.should_be(fileContent);
    }

    void when_generating() {
        it["generates default pool"] = () => generates(new string[0], defaultPool);
        it["generates one custom pool"] = () => generates(new [] { "Meta" }, metaPool);
        it["generates multiple pools"] = () => generates(new [] { "Meta", "Core" }, metaCorePool);
    }
}

