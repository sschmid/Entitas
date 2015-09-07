using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolsGenerator : nspec {

    bool logResults = true;

    const string defaultPool = @"using Entitas;
using System.Collections.Generic;

public static class Pools {

    static Pool __pool;

    public static Pool pool {
        get {
            if (__pool == null) {
                __pool = new Pool(ComponentIds.TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(__pool, ""Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return __pool;
        }
    }

    /// <summary>
    /// Creates a listing of all Pools, instantiating those which have not been instantiated.
    /// </summary>
    static ReadOnlyCollection<Pool> _list;

    public static ReadOnlyCollection<Pool> List {
        get {
            if(_list == null){
                _list = new ReadOnlyCollection<Pool>(new List<Pool>{
                    pool
                });
            }
            return _list;
        }
    }

}";

    const string metaPool = @"using Entitas;
using System.Collections.Generic;

public static class Pools {

    static Pool __meta;

    public static Pool meta {
        get {
            if (__meta == null) {
                __meta = new Pool(MetaComponentIds.TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(__meta, ""Meta Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return __meta;
        }
    }

    /// <summary>
    /// Creates a listing of all Pools, instantiating those which have not been instantiated.
    /// </summary>
    static ReadOnlyCollection<Pool> _list;

    public static ReadOnlyCollection<Pool> List {
        get {
            if(_list == null){
                _list = new ReadOnlyCollection<Pool>(new List<Pool>{
                    meta
                });
            }
            return _list;
        }
    }

}";

    const string metaCorePool = @"using Entitas;
using System.Collections.Generic;

public static class Pools {

    static Pool __meta;

    public static Pool meta {
        get {
            if (__meta == null) {
                __meta = new Pool(MetaComponentIds.TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(__meta, ""Meta Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return __meta;
        }
    }

    static Pool __core;

    public static Pool core {
        get {
            if (__core == null) {
                __core = new Pool(CoreComponentIds.TotalComponents);
                #if (UNITY_EDITOR)
                var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(__core, ""Core Pool"");
                UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
                #endif
            }

            return __core;
        }
    }

    /// <summary>
    /// Creates a listing of all Pools, instantiating those which have not been instantiated.
    /// </summary>
    static ReadOnlyCollection<Pool> _list;

    public static ReadOnlyCollection<Pool> List {
        get {
            if(_list == null){
                _list = new ReadOnlyCollection<Pool>(new List<Pool>{
                    meta,
                    core
                });
            }
            return _list;
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

