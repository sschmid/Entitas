using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolsGenerator : nspec {

    const bool logResults = false;

    const string defaultPool = @"public static class Pools {
    public static Entitas.Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { pool };
            }

            return _allPools;
        }
    }
    public static Entitas.Pool pool {
        get {
            if (_pool == null) {
                _pool = new Entitas.Pool(ComponentIds.TotalComponents, 0, new Entitas.PoolMetaData(""Pool"", ComponentIds.componentNames));
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_pool, ComponentIds.componentTypes);
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _pool;
        }
    }

    static Entitas.Pool[] _allPools;
    static Entitas.Pool _pool;
}";

    const string metaPool = @"public static class Pools {
    public static Entitas.Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { meta };
            }

            return _allPools;
        }
    }
    public static Entitas.Pool meta {
        get {
            if (_meta == null) {
                _meta = new Entitas.Pool(MetaComponentIds.TotalComponents, 0, new Entitas.PoolMetaData(""Meta Pool"", MetaComponentIds.componentNames));
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_meta, MetaComponentIds.componentTypes);
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _meta;
        }
    }

    static Entitas.Pool[] _allPools;
    static Entitas.Pool _meta;
}";

    const string metaCorePool = @"public static class Pools {
    public static Entitas.Pool[] allPools {
        get {
            if (_allPools == null) {
                _allPools = new [] { meta, core };
            }

            return _allPools;
        }
    }
    public static Entitas.Pool meta {
        get {
            if (_meta == null) {
                _meta = new Entitas.Pool(MetaComponentIds.TotalComponents, 0, new Entitas.PoolMetaData(""Meta Pool"", MetaComponentIds.componentNames));
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_meta, MetaComponentIds.componentTypes);
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _meta;
        }
    }
    public static Entitas.Pool core {
        get {
            if (_core == null) {
                _core = new Entitas.Pool(CoreComponentIds.TotalComponents, 0, new Entitas.PoolMetaData(""Core Pool"", CoreComponentIds.componentNames));
                #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_core, CoreComponentIds.componentTypes);
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return _core;
        }
    }

    static Entitas.Pool[] _allPools;
    static Entitas.Pool _meta;
    static Entitas.Pool _core;
}";

    void generates(string[] poolNames, string fileContent) {
        fileContent = fileContent.ToUnixLineEndings();
        var files = new PoolsGenerator().Generate(poolNames);
        files.Length.should_be(1);
        var file = files[0];
        #pragma warning disable
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

