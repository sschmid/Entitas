//using System;
//using Entitas.CodeGenerator;
//using NSpec;

//class describe_ContextsGenerator : nspec {

//    const bool logResults = false;

//    const string defaultContext = @"namespace Entitas {

//    public partial class Contexts {

//        public static Context CreateContext() {
//            return CreateContext(""Context"", ComponentIds.TotalComponents, ComponentIds.componentNames, ComponentIds.componentTypes);
//        }

//        public Context[] allContexts { get { return new [] { context }; } }

//        public Context context;

//        public void SetAllContexts() {
//            context = CreateContext();
//        }
//    }
//}
//";

//    const string metaContext = @"namespace Entitas {

//    public partial class Contexts {

//        public static Context CreateMetaContext() {
//            return CreateContext(""Meta"", MetaComponentIds.TotalComponents, MetaComponentIds.componentNames, MetaComponentIds.componentTypes);
//        }

//        public Context[] allContexts { get { return new [] { meta }; } }

//        public Context meta;

//        public void SetAllContexts() {
//            meta = CreateMetaContext();
//        }
//    }
//}
//";

//    const string coreMetaContext = @"namespace Entitas {

//    public partial class Contexts {

//        public static Context CreateMetaContext() {
//            return CreateContext(""Meta"", MetaComponentIds.TotalComponents, MetaComponentIds.componentNames, MetaComponentIds.componentTypes);
//        }

//        public static Context CreateCoreContext() {
//            return CreateContext(""Core"", CoreComponentIds.TotalComponents, CoreComponentIds.componentNames, CoreComponentIds.componentTypes);
//        }

//        public Context[] allContexts { get { return new [] { meta, core }; } }

//        public Context meta;
//        public Context core;

//        public void SetAllContexts() {
//            meta = CreateMetaContext();
//            core = CreateCoreContext();
//        }
//    }
//}
//";

//    void generates(string[] contextNames, string expectedFileContent) {
//        expectedFileContent = expectedFileContent.ToUnixLineEndings();

//        var files = new ContextsGenerator().Generate(contextNames);
//        files.Length.should_be(1);
//        var file = files[0];

//        #pragma warning disable
//        if(logResults) {
//            Console.WriteLine("should:\n" + expectedFileContent);
//            Console.WriteLine("was:\n" + file.fileContent);
//        }

//        file.fileName.should_be("Contexts");
//        file.fileContent.should_be(expectedFileContent);
//    }

//    void when_generating() {
//        it["generates default context"] = () => generates(new [] { CodeGenerator.DEFAULT_CONTEXT_NAME }, defaultContext);
//        it["generates one custom context"] = () => generates(new [] { "Meta" }, metaContext);
//        it["generates multiple contexts"] = () => generates(new [] { "Meta", "Core" }, coreMetaContext);
//    }
//}
