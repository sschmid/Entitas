//using System;
//using System.IO;
//using System.Linq;
//using Entitas.CodeGenerator;
//using My.Namespace;
//using NSpec;

//class describe_ComponentExtensionsGenerator : nspec {

//    const bool logResults = false;

//    static void generates<T>(params string[] contextNames) {
//        contextNames.Length.should_not_be(0);

//        var infos = TypeReflectionProvider.GetComponentInfos(typeof(T));
//        var codeGenFiles = new ComponentExtensionsGenerator().Generate(infos);
//        codeGenFiles.Length.should_be(contextNames.Length);

//        var projectRoot = TestExtensions.GetProjectRoot();

//        foreach(var contextName in contextNames) {
//            var fixturesDir = projectRoot + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";
//            var expectedFileName = contextName + Path.DirectorySeparatorChar +
//                                   "Components" + Path.DirectorySeparatorChar +
//                                   contextName + typeof(T).FullName;
            
//            var path = fixturesDir + expectedFileName + ".cs";

//            var expectedFileContent = File.ReadAllText(path);

//            var codeGenFile = codeGenFiles.Single(cgf => cgf.fileName == expectedFileName);
//            var header = string.Format(CodeGenerator.AUTO_GENERATED_HEADER_FORMAT, typeof(ComponentExtensionsGenerator));

//            #pragma warning disable
//            if(logResults && (header + codeGenFile.fileContent != expectedFileContent)) {
//                Console.WriteLine("should:\n" + expectedFileContent);
//                Console.WriteLine("was: " + codeGenFile.fileName + "\n" + codeGenFile.fileContent);
//            }

//            codeGenFile.fileName.should_be(expectedFileName);
//            (header + codeGenFile.fileContent).should_be(expectedFileContent);
//        }
//    }

//    static void generatesComponent<T>(params string[] contextNames) {
//        var infos = TypeReflectionProvider.GetComponentInfos(typeof(T));
//        var codeGenFiles = new ComponentExtensionsGenerator().Generate(infos);

//        codeGenFiles.Length.should_be(contextNames.Length + 1);

//        var projectRoot = TestExtensions.GetProjectRoot();

//        var fixturesDir = projectRoot + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";
//        var expectedFileName = "Components" + Path.DirectorySeparatorChar + typeof(T).FullName + "Component";

//        var path = fixturesDir + expectedFileName + ".cs";

//        var expectedFileContent = File.ReadAllText(path);

//        var codeGenFile = codeGenFiles.Single(cgf => cgf.fileName == expectedFileName);
//        var header = string.Format(CodeGenerator.AUTO_GENERATED_HEADER_FORMAT, typeof(ComponentExtensionsGenerator));

//        #pragma warning disable
//        if(logResults && (header + codeGenFile.fileContent != expectedFileContent)) {
//            Console.WriteLine("should:\n" + expectedFileContent);
//            Console.WriteLine("was: " + codeGenFile.fileName + "\n" + codeGenFile.fileContent);
//        }

//        codeGenFile.fileName.should_be(expectedFileName);
//        (header + codeGenFile.fileContent).should_be(expectedFileContent);
//    }

//    void when_generating() {
//        it["component without fields"] = () => generates<MovableComponent>("Test");
//        it["component with fields"] = () => generates<PersonComponent>("Test");
//        it["single component without fields"] = () => generates<AnimatingComponent>("Test");
//        it["single component with fields"] = () => generates<UserComponent>("Test");
//        it["component for custom context"] = () => generates<OtherContextComponent>("Other");
//        it["supports properties"] = () => generates<ComponentWithFieldsAndProperties>("Test");
//        it["ignores [DontGenerate]"] = () => {
//            var info = TypeReflectionProvider.GetComponentInfos(typeof(DontGenerateComponent))[0];
//            var files = new ComponentExtensionsGenerator().Generate(new [] { info });
//            files.Length.should_be(0);
//        };

//        it["works with namespaces"] = () => generates<NamespaceComponent>("Test");
//        it["generates matchers for each context"] = () => generates<CComponent>("ContextA", "ContextB", "ContextC");
//        it["generates custom prefix"] = () => generates<CustomPrefixComponent>("Test");

//        it["generates component for class"] = () => generatesComponent<SomeClass>("SomeContext", "SomeOtherContext");
//        it["generates component for struct"] = () => generatesComponent<SomeStruct>("Test");

//        it["generates component for class with HideInBlueprintInspectorAttribute"] = () =>
//            generatesComponent<SomeClassHideInBlueprintInspector>("Test");
//    }
//}
