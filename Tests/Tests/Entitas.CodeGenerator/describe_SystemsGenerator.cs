using System;
using Entitas.CodeGenerator;
using NSpec;

class describe_SystemsGenerator : nspec {

    const string classSuffix = "GeneratedExtension";

    const string system = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestSystem() {
            return this.CreateSystem<TestSystem>();
        }
    }
}";

    const string initializeSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestInitializeSystem() {
            return this.CreateSystem<TestInitializeSystem>();
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

    const string initializeExecuteSystem = @"namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestInitializeExecuteSystem() {
            return this.CreateSystem<TestInitializeExecuteSystem>();
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

    void generates<T>(string expectedCode) {
        expectedCode = expectedCode.ToUnixLineEndings();
        var type = typeof(T);
        var files = new SystemsGenerator().Generate(new [] { type });
        files.Length.should_be(1);
        var file = files[0];
        file.fileName.should_be(type + classSuffix);
        file.fileContent.should_be(expectedCode);
    }

    void ignores<T>() {
        var type = typeof(T);
        var files = new SystemsGenerator().Generate(new [] { type });
        files.Length.should_be(0);
    }

    void when_generating() {
        it["System"] = () => generates<TestSystem>(system);
        it["InitializeSystem"] = () => generates<TestInitializeSystem>(initializeSystem);
        it["ExecuteSystem"] = () => generates<TestExecuteSystem>(executeSystem);
        it["InitializeExecuteSystem"] = () => generates<TestInitializeExecuteSystem>(initializeExecuteSystem);
        it["ReactiveSystem"] = () => generates<TestReactiveSystem>(reactiveSystem);
        it["Ignores namespace in method name"] = () => generates<Tests.NamespaceSystem>(namespaceSystem);
        it["Ignores system with constructor args"] = () => ignores<CtorArgsSystem>();
    }
}

