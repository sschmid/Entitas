using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolsGenerator : nspec {

    const bool logResults = !false;

    const string defaultPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreatePool() {
            var pool = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData(""Pool"", ComponentIds.componentNames, ComponentIds.componentTypes));
            #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(pool);
            UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
            #endif

            return pool;
        }

        public Pool[] allPools { get { return new[] { pool }; } }

        public Pool pool;
    }
}
";

    const string metaPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreateMetaPool() {
            var pool = new Pool(MetaComponentIds.TotalComponents, 0, new PoolMetaData(""Meta"", MetaComponentIds.componentNames, MetaComponentIds.componentTypes));
            #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(pool);
            UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
            #endif

            return pool;
        }

        public Pool[] allPools { get { return new[] { meta }; } }

        public Pool meta;
    }
}
";

    const string coreMetaPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreateMetaPool() {
            var pool = new Pool(MetaComponentIds.TotalComponents, 0, new PoolMetaData(""Meta"", MetaComponentIds.componentNames, MetaComponentIds.componentTypes));
            #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(pool);
            UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
            #endif

            return pool;
        }

        public static Pool CreateCorePool() {
            var pool = new Pool(CoreComponentIds.TotalComponents, 0, new PoolMetaData(""Core"", CoreComponentIds.componentNames, CoreComponentIds.componentTypes));
            #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(pool);
            UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
            #endif

            return pool;
        }

        public Pool[] allPools { get { return new[] { meta, core }; } }

        public Pool meta;
        public Pool core;
    }
}
";

    void generates(string[] poolNames, string expectedFileContent) {
        expectedFileContent = expectedFileContent.ToUnixLineEndings();

        var files = new PoolsGenerator().Generate(poolNames);
        files.Length.should_be(1);
        var file = files[0];

        #pragma warning disable
        if(logResults) {
            Console.WriteLine("should:\n" + expectedFileContent);
            Console.WriteLine("was:\n" + file.fileContent);
        }

        file.fileName.should_be("Pools");
        file.fileContent.should_be(expectedFileContent);
    }

    void when_generating() {

        it["generates default pool"] = () => generates(new[] { CodeGenerator.DEFAULT_POOL_NAME }, defaultPool);
        it["generates one custom pool"] = () => generates(new[] { "Meta" }, metaPool);
        it["generates multiple pools"] = () => generates(new[] { "Meta", "Core" }, coreMetaPool);
    }
}

