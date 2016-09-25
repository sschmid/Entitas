using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolsGenerator : nspec {

    const bool logResults = false;

    const string defaultPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreatePool() {
            return CreatePool(""Pool"", ComponentIds.TotalComponents, ComponentIds.componentNames, ComponentIds.componentTypes);
        }

        public Pool[] allPools { get { return new [] { pool }; } }

        public Pool pool;

        public void SetAllPools() {
            pool = CreatePool();
        }
    }
}
";

    const string metaPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreateMetaPool() {
            return CreatePool(""Meta"", MetaComponentIds.TotalComponents, MetaComponentIds.componentNames, MetaComponentIds.componentTypes);
        }

        public Pool[] allPools { get { return new [] { meta }; } }

        public Pool meta;

        public void SetAllPools() {
            meta = CreateMetaPool();
        }
    }
}
";

    const string coreMetaPool = @"namespace Entitas {

    public partial class Pools {

        public static Pool CreateMetaPool() {
            return CreatePool(""Meta"", MetaComponentIds.TotalComponents, MetaComponentIds.componentNames, MetaComponentIds.componentTypes);
        }

        public static Pool CreateCorePool() {
            return CreatePool(""Core"", CoreComponentIds.TotalComponents, CoreComponentIds.componentNames, CoreComponentIds.componentTypes);
        }

        public Pool[] allPools { get { return new [] { meta, core }; } }

        public Pool meta;
        public Pool core;

        public void SetAllPools() {
            meta = CreateMetaPool();
            core = CreateCorePool();
        }
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
        it["generates default pool"] = () => generates(new [] { CodeGenerator.DEFAULT_POOL_NAME }, defaultPool);
        it["generates one custom pool"] = () => generates(new [] { "Meta" }, metaPool);
        it["generates multiple pools"] = () => generates(new [] { "Meta", "Core" }, coreMetaPool);
    }
}
