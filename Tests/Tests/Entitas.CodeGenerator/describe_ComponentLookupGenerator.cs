using NSpec;
using Entitas.CodeGenerator;
using System;
using System.Linq;

class describe_ComponentLookupGenerator : nspec {
    bool logResults = false;

    void generates(Type type, string lookupName, string lookupCode) {
        generates(new [] { type }, lookupName, lookupCode);
    }

    void generates(Type[] types, string lookupName, string lookupCode) {
        generates(types, new [] { lookupName }, new [] { lookupCode });
    }

    void generates(Type[] types, string[] lookupNames, string[] lookupCodes) {
        var files = new ComponentLookupGenerator().Generate(types);
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
        var files = new ComponentLookupGenerator().Generate(poolNames);
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
            generates(typeof(SomeComponent), CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""Some""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(SomeComponent)
    };
}");
        };



        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherPoolComponent), "OtherComponentIds",
                @"public static class OtherComponentIds {
    public const int OtherPool = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""OtherPool""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(OtherPoolComponent)
    };
}");
        };



        it["generates id for [DontGenerate]"] = () => {
            generates(typeof(DontGenerateComponent), CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""DontGenerate""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(DontGenerateComponent)
    };
}");
        };



        it["generates ids for all types ordered alphabetically"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateComponent)
            }, CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;
    public const int Some = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = {
        ""DontGenerate"",
        ""Some""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(DontGenerateComponent),
        typeof(SomeComponent)
    };
}");
        };



        it["ignores [DontGenerate(false)]"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateIndexComponent)
            }, CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""Some""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(SomeComponent)
    };
}");
        };

        it["generates empty lookup with total components when for default pool"] = () => {
            generatesEmptyLookup(new string[0], new [] { CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG }, new [] { @"public static class ComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}" });
        };

        it["generates empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta" }, new [] { "Meta" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG }, new [] { @"public static class MetaComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}" });
        };

        it["generates multiple empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta", "Core" },
                new [] { "Meta" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG, "Core" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG },
                new [] { @"public static class MetaComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}", @"public static class CoreComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}" });
        };

        context["when component is in multiple pools"] = () => {

            it["rearranges ids to have the same index in every lookup (2 pools)"] = () => {
                generates(new [] {
                    typeof(AComponent),
                    typeof(BComponent)
                }, new [] { "PoolAComponentIds", "PoolBComponentIds" }, new [] {
                    @"public static class PoolAComponentIds {
    public const int B = 0;
    public const int A = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = {
        ""B"",
        ""A""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BComponent),
        typeof(AComponent)
    };
}",
                    @"public static class PoolBComponentIds {
    public const int B = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""B""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BComponent)
    };
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
                    @"public static class PoolAComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int A = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = {
        ""C"",
        ""B"",
        ""A""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(CComponent),
        typeof(BComponent),
        typeof(AComponent)
    };
}",
                    @"public static class PoolBComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int D = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = {
        ""C"",
        ""B"",
        ""D""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(CComponent),
        typeof(BComponent),
        typeof(DComponent)
    };
}",
                    @"public static class PoolCComponentIds {
    public const int C = 0;
    public const int E = 1;
    public const int D = 2;
    public const int F = 3;

    public const int TotalComponents = 4;

    public static readonly string[] componentNames = {
        ""C"",
        ""E"",
        ""D"",
        ""F""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(CComponent),
        typeof(EComponent),
        typeof(DComponent),
        typeof(FComponent)
    };
}"
                });
            };        };
    }
}

