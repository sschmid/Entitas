using NSpec;
using Entitas.CodeGenerator;
using System;

class describe_EntitasCodeGenerator : nspec {
    static void filterIgnores(Type type) {
        var types = new[] { type };
        var filtered = EntitasCodeGenerator.GetComponents(types);
        filtered.should_not_contain(type);
    }

    static void filterKeeps(Type type) {
        var types = new[] { type };
        var filtered = EntitasCodeGenerator.GetComponents(types);
        filtered.should_contain(type);
    }

    void when_filtering_components() {
        it["ignores non IComponents"] = () => filterIgnores(typeof(SomeClass));
        it["ignores non IComponents ending with *Component"] = () => filterIgnores(typeof(FakeComponent));
        it["contains IComponents"] = () => filterKeeps(typeof(SomeComponent));
        it["contains IComponents not ending with *Component"] = () => filterKeeps(typeof(View));
    }
}

