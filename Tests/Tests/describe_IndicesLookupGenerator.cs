using NSpec;
using Entitas.CodeGenerator;
using System;

class describe_IndicesLookupGenerator : nspec {
    const string defaultLookupName = "ComponentIds";

    void generates(Type type, string lookupName, string lookupCode) {
        generates(new [] { type }, lookupName, lookupCode);
    }

    void generates(Type[] types, string lookupName, string lookupCode) {
        var generator = new IndicesLookupGenerator();
        var lookups = generator.GenerateIndicesLookup(types);
        lookups.Count.should_be(1);
        lookups.ContainsKey(lookupName).should_be_true();
        lookups[lookupName].should_be(lookupCode);
    }

    void when_generating() {

        it["generates default lookup"] = () => {
            generates(typeof(SomeComponent), defaultLookupName,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;
}");
        };
        
        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherPoolComponent), "Other",
                @"public static class Other {
    public const int OtherPool = 0;

    public const int TotalComponents = 1;
}");
        };
    
        it["generates id for [DontGenerate]"] = () => {
            generates(typeof(DontGenerateComponent), defaultLookupName,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;

    public const int TotalComponents = 1;
}");
        };

        it["generates ids for all types"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateComponent)
            }, defaultLookupName,
                @"public static class ComponentIds {
    public const int Some = 0;
    public const int DontGenerate = 1;

    public const int TotalComponents = 2;
}");
        };
    }
}

