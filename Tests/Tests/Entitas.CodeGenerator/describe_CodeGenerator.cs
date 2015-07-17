using NSpec;
using Entitas.CodeGenerator;
using System;
using Entitas;

class describe_CodeGenerator : nspec {
    static void componentFilterIgnores(Type type) {
        var types = new[] { type };
        var filtered = CodeGenerator.GetComponents(types);
        filtered.should_not_contain(type);
    }

    static void componentFilterKeeps(Type type) {
        var types = new[] { type };
        var filtered = CodeGenerator.GetComponents(types);
        filtered.should_contain(type);
    }

    static void systemFilterIgnores(Type type) {
        var types = new[] { type };
        var filtered = CodeGenerator.GetSystems(types);
        filtered.should_not_contain(type);
    }

    static void systemFilterKeeps(Type type) {
        var types = new[] { type };
        var filtered = CodeGenerator.GetSystems(types);
        filtered.should_contain(type);
    }

    void when_filtering_components() {
        it["ignores non IComponents"] = () => componentFilterIgnores(typeof(SomeClass));
        it["ignores non IComponents ending with *Component"] = () => componentFilterIgnores(typeof(FakeComponent));
        it["contains IComponents"] = () => componentFilterKeeps(typeof(SomeComponent));
        it["contains IComponents not ending with *Component"] = () => componentFilterKeeps(typeof(View));
    }

    void when_filtering_systems() {
        it["ignores ISystems interface"] = () => systemFilterIgnores(typeof(ISystem));
        it["ignores IStartSystems interface"] = () => systemFilterIgnores(typeof(IStartSystem));
        it["ignores IExecuteSystems interface"] = () => systemFilterIgnores(typeof(IExecuteSystem));
        it["ignores IReactiveSystem interface"] = () => systemFilterIgnores(typeof(IReactiveSystem));

        it["ignores ReactiveSystem"] = () => systemFilterIgnores(typeof(ReactiveSystem));
        it["ignores Systems"] = () => systemFilterIgnores(typeof(Systems));

        it["ignores non ISystems"] = () => systemFilterIgnores(typeof(SomeClass));
        it["ignores non ISystems ending with *System"] = () => systemFilterIgnores(typeof(FakeSystem));
        it["contains ISystem"] = () => systemFilterKeeps(typeof(TestSystem));
        it["contains ISystem not ending with *System"] = () => systemFilterKeeps(typeof(SomeSystem));
    }

    void safe_dir() {
        it["appends '/' and 'Generated/ 'if necessary"] = () => CodeGenerator.GetSafeDir("Assets").should_be("Assets/Generated/");
        it["appends '/' and 'Generated/ 'if necessary"] = () => CodeGenerator.GetSafeDir("Assets/").should_be("Assets/Generated/");
        it["appends '/' and 'Generated/ 'if necessary"] = () => CodeGenerator.GetSafeDir("Assets/Generated").should_be("Assets/Generated/");
        it["appends '/' and 'Generated/ 'if necessary"] = () => CodeGenerator.GetSafeDir("Assets/Generated/").should_be("Assets/Generated/");
        it["appends '/' and 'Generated/ 'if necessary"] = () => CodeGenerator.GetSafeDir("/").should_be("/Generated/");
    }
}

