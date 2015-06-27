using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_SystemExtensionsGenerator : nspec {

    const string classSuffix = "GeneratedExtension";

    const string system = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestSystem() {
            return this.CreateSystem<TestSystem>();
        }
    }
}";

    const string startSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestStartSystem() {
            return this.CreateSystem<TestStartSystem>();
        }
    }
}";

    const string executeSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestExecuteSystem() {
            return this.CreateSystem<TestExecuteSystem>();
        }
    }
}";

    const string startExecuteSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestStartExecuteSystem() {
            return this.CreateSystem<TestStartExecuteSystem>();
        }
    }
}";

    const string reactiveSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestReactiveSystem() {
            return this.CreateSystem<TestReactiveSystem>();
        }
    }
}";

    const string namespaceSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateNamespaceSystem() {
            return this.CreateSystem<Tests.NamespaceSystem>();
        }
    }
}";

    void generates<T>(string code) {
        var type = typeof(T);
        var files = new SystemExtensionsGenerator().Generate(new [] { type });
        files.Length.should_be(1);
        var file = files[0];
        file.fileName.should_be(type + classSuffix);
        file.fileContent.should_be(code);
    }

    void ignores<T>() {
        var type = typeof(T);
        var files = new SystemExtensionsGenerator().Generate(new [] { type });
        files.Length.should_be(0);
    }

    void when_generating() {
        it["System"] = () => generates<TestSystem>(system);
        it["StartSystem"] = () => generates<TestStartSystem>(startSystem);
        it["ExecuteSystem"] = () => generates<TestExecuteSystem>(executeSystem);
        it["StartExecuteSystem"] = () => generates<TestStartExecuteSystem>(startExecuteSystem);
        it["ReactiveSystem"] = () => generates<TestReactiveSystem>(reactiveSystem);
        it["Ignores namespace in method name"] = () => generates<Tests.NamespaceSystem>(namespaceSystem);
        it["Ignores system with constructor args"] = () => ignores<CtorArgsSystem>();
    }
}

