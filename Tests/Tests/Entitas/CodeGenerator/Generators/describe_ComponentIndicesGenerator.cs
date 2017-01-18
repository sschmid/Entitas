using System;
using System.Linq;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_ComponentIndicesGenerator : nspec {

    const bool logResults = !false;

    static void generates(Type type, string expectedLookupName, string expectedLookupCode) {
        generates(new [] { type }, expectedLookupName, expectedLookupCode);
    }

    static void generates(Type[] types, string expectedLookupName, string expectedLookupCode) {
        generates(types, new [] { expectedLookupName }, new [] { expectedLookupCode });
    }

    static void generates(Type[] types, string[] expectedLookupNames, string[] expectedLookupCodes) {
        var infos = TypeReflectionProvider.GetComponentInfos(types);

        var files = new ComponentIndicesGenerator().Generate(infos);

        files.Length.should_be(expectedLookupNames.Length);


        for(int i = 0; i < expectedLookupNames.Length; i++) {
            var expectedLookupName = expectedLookupNames[i];
            var expectedLookupCode = expectedLookupCodes[i].ToUnixLineEndings();

            files.Any(f => f.fileName == expectedLookupName).should_be_true();
            var file = files.Single(f => f.fileName == expectedLookupName);

#pragma warning disable
            if(logResults) {
                Console.WriteLine("should:\n" + expectedLookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }

            file.fileContent.should_be(expectedLookupCode);
        }
    }

    static void generatesEmptyLookup(string[] contextNames, string[] expectedLookupNames, string[] expectedLookupCodes) {
        var files = new ComponentIndicesGenerator().Generate(contextNames);
        files.Length.should_be(contextNames.Length);

        for(int i = 0; i < expectedLookupNames.Length; i++) {
            var expectedLookupName = expectedLookupNames[i];
            var expectedLookupCode = expectedLookupCodes[i].ToUnixLineEndings();

            files.Any(f => f.fileName == expectedLookupName).should_be_true();
            var file = files.First(f => f.fileName == expectedLookupName);
            if(logResults) {
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
}
");
        };



        it["generates compatible field names for components with namespace"] = () => {
            generates(typeof(NamespaceComponent), "Test" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG,
                @"public static class TestComponentIds {

    public const int Namespace = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""Namespace""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(My.Namespace.NamespaceComponent)
    };
}
");
        };



        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherContextComponent), "OtherComponentIds",
                @"public static class OtherComponentIds {

    public const int OtherContext = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""OtherContext""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(OtherContextComponent)
    };
}
");
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
}
");
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
}
");
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
}
");
        };



        it["generates empty lookup with total components when for default context"] = () => {
            generatesEmptyLookup(new [] { CodeGenerator.DEFAULT_CONTEXT_NAME }, new [] { CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG }, new [] { @"public static class ComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}
" });
        };



        it["generates empty lookup with total components when for context names"] = () => {
            generatesEmptyLookup(new [] { "Meta" }, new [] { "Meta" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG }, new [] { @"public static class MetaComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}
" });
        };



        it["generates multiple empty lookup with total components when for context names"] = () => {
            generatesEmptyLookup(new [] { "Meta", "Core" },
                new [] { "Meta" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG, "Core" + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG },
                new [] { @"public static class MetaComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}
", @"public static class CoreComponentIds {

    public const int TotalComponents = 0;

    public static readonly string[] componentNames = {
    };

    public static readonly System.Type[] componentTypes = {
    };
}
" });
        };



        context["when component is in multiple contexts"] = () => {

            it["rearranges ids to have the same index in every lookup (2 contexts)"] = () => {
                generates(new [] {
                        typeof(AComponent),
                        typeof(BComponent)
                    }, new [] { "ContextAComponentIds", "ContextBComponentIds" }, new [] {
                    @"public static class ContextAComponentIds {

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
}
",
                    @"public static class ContextBComponentIds {

    public const int B = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        ""B""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BComponent)
    };
}
"
                });
            };



            it["rearranges ids to have the same index in every lookup (3 contexts)"] = () => {
                generates(new [] {
                        typeof(AComponent),
                        typeof(BComponent),
                        typeof(CComponent),
                        typeof(DComponent),
                        typeof(EComponent),
                        typeof(FComponent)
                    }, new [] {
                        "ContextAComponentIds",
                        "ContextBComponentIds",
                        "ContextCComponentIds",
                    }, new [] {
                        @"public static class ContextAComponentIds {

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
}
",
                    @"public static class ContextBComponentIds {

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
}
",
                    @"public static class ContextCComponentIds {

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
}
"              });
            };



            it["rearranges ids to have the same index in every lookup (#124)"] = () => {
                generates(new [] {
                        typeof(DComponent),
                        typeof(GComponent),
                        typeof(BComponent),
                    }, new [] {
                        "ContextAComponentIds",
                        "ContextBComponentIds",
                        "ContextCComponentIds",
                    }, new [] {
                @"public static class ContextAComponentIds {

    public const int B = 0;
    public const int G = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = {
        ""B"",
        null,
        ""G""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BComponent),
        null,
        typeof(GComponent)
    };
}
",
                @"public static class ContextBComponentIds {

    public const int B = 0;
    public const int D = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = {
        ""B"",
        ""D""
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BComponent),
        typeof(DComponent)
    };
}
",
                @"public static class ContextCComponentIds {

    public const int D = 1;
    public const int G = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = {
        null,
        ""D"",
        ""G""
    };

    public static readonly System.Type[] componentTypes = {
        null,
        typeof(DComponent),
        typeof(GComponent)
    };
}
"              });
            };
        };
    }
}
