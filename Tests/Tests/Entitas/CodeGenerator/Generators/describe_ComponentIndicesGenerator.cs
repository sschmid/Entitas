using NSpec;
using Entitas.CodeGenerator;
using System;
using System.Linq;
using My.Namespace;

class describe_ComponentIndicesGenerator : nspec {

    const bool logResults = false;

    static void generates(ComponentInfo componentInfo, string lookupName, string lookupCode) {
        generates(new [] { componentInfo }, lookupName, lookupCode);
    }

    static void generates(ComponentInfo[] componentInfos, string lookupName, string lookupCode) {
        generates(componentInfos, new [] { lookupName }, new [] { lookupCode });
    }

    static void generates(ComponentInfo[] componentInfos, string[] lookupNames, string[] lookupCodes) {
        var files = new ComponentIndicesGenerator().Generate(componentInfos);
        files.Length.should_be(lookupNames.Length);

        for (int i = 0; i < lookupNames.Length; i++) {
            var lookupName = lookupNames[i];
            var expectedLookupCode = lookupCodes[i].ToUnixLineEndings();

            foreach (var f in files) {
                System.Console.WriteLine("file.fileName: " + f.fileName);
            }

            files.Any(f => f.fileName == lookupName).should_be_true();
            var file = files.Single(f => f.fileName == lookupName);
            #pragma warning disable
            if (logResults) {
                Console.WriteLine("should:\n" + expectedLookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }
            file.fileContent.should_be(expectedLookupCode);
        }
    }

    static void generatesEmptyLookup(string[] poolNames, string[] lookupNames, string[] lookupCodes) {
        var files = new ComponentIndicesGenerator().Generate(poolNames, new ComponentInfo[0]);
        files.Length.should_be(poolNames.Length);

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
            generates(SomeComponent.componentInfo, CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
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



        it["generates compatible field names for components with namespace"] = () => {
            generates(NamespaceComponent.componentInfo, CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class ComponentIds {
    public const int Namespace = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""Namespace""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(My.Namespace.NamespaceComponent)
    };
}");
        };



        it["generates lookup with name from attribute"] = () => {
            generates(OtherPoolComponent.componentInfo, "OtherComponentIds",
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
            generates(DontGenerateComponent.componentInfo, CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
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



        it["ignores [DontGenerate(false)]"] = () => {
            generates(new [] {
                SomeComponent.componentInfo,
                DontGenerateIndexComponent.componentInfo
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



        it["generates ids for all types ordered alphabetically"] = () => {
            generates(new [] {
                SomeComponent.componentInfo,
                DontGenerateComponent.componentInfo
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



        it["generates empty lookup with total components when for default pool"] = () => {
            generatesEmptyLookup(new [] { CodeGenerator.DEFAULT_POOL_NAME }, new [] { CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG }, new [] { @"public static class ComponentIds {

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
                    AComponent.componentInfo,
                    BComponent.componentInfo
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
                    AComponent.componentInfo,
                    BComponent.componentInfo,
                    CComponent.componentInfo,
                    DComponent.componentInfo,
                    EComponent.componentInfo,
                    FComponent.componentInfo
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

