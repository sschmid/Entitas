using NSpec;
using Entitas.CodeGenerator;
using System;
using System.Linq;

class describe_IndicesLookupGenerator : nspec {
    bool logResults = false;

    void generates(Type type, string lookupName, string lookupCode) {
        generates(new [] { type }, lookupName, lookupCode);
    }

    void generates(Type[] types, string lookupName, string lookupCode) {
        generates(types, new [] { lookupName }, new [] { lookupCode });
    }

    void generates(Type[] types, string[] lookupNames, string[] lookupCodes) {
        var files = new IndicesLookupGenerator().Generate(types);
        files.Length.should_be(lookupNames.Length);

        for (int i = 0; i < lookupNames.Length; i++) {
            var lookupName = lookupNames[i];
            var expectedLookupCode = lookupCodes[i].ToUnixLineEndings();;
            files.Any(f => f.fileName == lookupName).should_be_true();
            var file = files.Single(f => f.fileName == lookupName);
            if (logResults) {
                Console.WriteLine("should:\n" + expectedLookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }
            file.fileContent.should_be(expectedLookupCode);
        }
    }

    void generatesEmptyLookup(string[] poolNames, string[] lookupNames, string[] lookupCodes) {
        var files = new IndicesLookupGenerator().Generate(poolNames);
        files.Length.should_be(poolNames.Length == 0 ? 1 : poolNames.Length);

//        foreach (var f in files) {
//            System.Console.WriteLine("f.fileName: " + f.fileName);
//        }

        for (int i = 0; i < lookupNames.Length; i++) {
            var lookupName = lookupNames[i];
            var expectedLookupCode = lookupCodes[i].ToUnixLineEndings();
            files.Any(f => f.fileName == lookupName).should_be_true();
            var file = files.First(f => f.fileName == lookupName);

            if (logResults) {
                Console.WriteLine("should:\n" + expectedLookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }

            file.fileContent.should_be(expectedLookupCode);
        }
    }

    void when_generating() {

        it["generates default lookup"] = () => {
            generates(typeof(SomeComponent), CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}");
        };



        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherPoolComponent), "OtherComponentIds",
                @"using Entitas;

public static class OtherComponentIds {
    public const int OtherPool = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        ""OtherPool""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}");
        };



        it["generates id for [DontGenerate]"] = () => {
            generates(typeof(DontGenerateComponent), CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        ""DontGenerate""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}");
        };



        it["generates ids for all types ordered alphabetically"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateComponent)
            }, CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;
    public const int Some = 1;

    public const int TotalComponents = 2;

    static readonly string[] _components = {
        ""DontGenerate"",
        ""Some""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}");
        };



        it["ignores [DontGenerate(false)]"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateIndexComponent)
            }, CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}");
        };

        it["generates empty lookup with total components when for default pool"] = () => {
            generatesEmptyLookup(new string[0], new [] { CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG }, new [] { @"public static class ComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] _components = {
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}" });
        };

        it["generates empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta" }, new [] { "Meta" + CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG }, new [] { @"using Entitas;

public static class MetaComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] _components = {
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}" });
        };

        it["generates multiple empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta", "Core" },
                new [] { "Meta" + CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG, "Core" + CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG },
                new [] { @"using Entitas;

public static class MetaComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] _components = {
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}", @"using Entitas;

public static class CoreComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] _components = {
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}" });
        };

        context["when component is in multiple pools"] = () => {

            it["rearranges ids to have the same index in every lookup (2 pools)"] = () => {
                generates(new [] {
                    typeof(AComponent),
                    typeof(BComponent)
                }, new [] { "PoolAComponentIds", "PoolBComponentIds" }, new [] {
                    @"using Entitas;

public static class PoolAComponentIds {
    public const int B = 0;
    public const int A = 1;

    public const int TotalComponents = 2;

    static readonly string[] _components = {
        ""B"",
        ""A""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}",
                    @"using Entitas;

public static class PoolBComponentIds {
    public const int B = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        ""B""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}"
                });
            };

            it["rearranges ids to have the same index in every lookup (3 pools)"] = () => {
                generates(new [] {
                    typeof(AComponent),
                    typeof(BComponent),
                    typeof(CComponent),
                    typeof(DComponent),
                    typeof(EComponent),
                    typeof(FComponent)
                }, new [] {
                    "PoolAComponentIds",
                    "PoolBComponentIds",
                    "PoolCComponentIds",
                }, new [] {
                    @"using Entitas;

public static class PoolAComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int A = 2;

    public const int TotalComponents = 3;

    static readonly string[] _components = {
        ""C"",
        ""B"",
        ""A""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}",
                    @"using Entitas;

public static class PoolBComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int D = 2;

    public const int TotalComponents = 3;

    static readonly string[] _components = {
        ""C"",
        ""B"",
        ""D""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}",
                    @"using Entitas;

public static class PoolCComponentIds {
    public const int C = 0;
    public const int E = 1;
    public const int D = 2;
    public const int F = 3;

    public const int TotalComponents = 4;

    static readonly string[] _components = {
        ""C"",
        ""E"",
        ""D"",
        ""F""
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}"
                });
            };        };
    }
}

