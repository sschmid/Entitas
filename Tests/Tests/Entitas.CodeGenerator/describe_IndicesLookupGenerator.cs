using NSpec;
using Entitas.CodeGenerator;
using System;
using System.Linq;

class describe_IndicesLookupGenerator : nspec {
    bool logResults = false;

    void generates(Type type, string lookupName, string lookupCode) {
        generates(new [] { type }, lookupName, lookupCode);
    }

    void generates(Type[] types, string lookupName, string lookupCode) {
        var files = new IndicesLookupGenerator().Generate(types);
        files.Length.should_be(1);

        files.Any(f => f.fileName == lookupName).should_be_true();
        
        var file = files.First(f => f.fileName == lookupName);

        if (logResults) {
            Console.WriteLine("should:\n" + lookupCode);
            Console.WriteLine("was:\n" + file.fileContent);
        }

        file.fileContent.should_be(lookupCode);
    }

    void generatesEmptyLookup(string[] poolNames, string[] lookupNames, string[] lookupCodes) {
        var files = new IndicesLookupGenerator().Generate(poolNames);
        files.Length.should_be(poolNames.Length == 0 ? 1 : poolNames.Length);

//        foreach (var f in files) {
//            System.Console.WriteLine("f.fileName: " + f.fileName);
//        }

        for (int i = 0; i < lookupNames.Length; i++) {
            var lookupName = lookupNames[i];
            var lookupCode = lookupCodes[i];
            files.Any(f => f.fileName == lookupName).should_be_true();
            var file = files.First(f => f.fileName == lookupName);

            if (logResults) {
                Console.WriteLine("should:\n" + lookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }

            file.fileContent.should_be(lookupCode);
        }
    }

    const string defaultTagCode = @"

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return ComponentIds.IdToString(indices[0]);
        }
    }
}";

    void when_generating() {

        it["generates default lookup"] = () => {
            generates(typeof(SomeComponent), CodeGenerator.defaultIndicesLookupTag,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}" + defaultTagCode);
        };



        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherPoolComponent), "OtherComponentIds",
                @"using Entitas;

public static class OtherComponentIds {
    public const int OtherPool = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        ""OtherPool""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class OtherMatcher : AllOfMatcher {
    public OtherMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return OtherComponentIds.IdToString(indices[0]);
    }
}");
        };



        it["generates id for [DontGenerate]"] = () => {
            generates(typeof(DontGenerateComponent), CodeGenerator.defaultIndicesLookupTag,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        ""DontGenerate""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}" + defaultTagCode);
        };



        it["generates ids for all types ordered alphabetically"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateComponent)
            }, CodeGenerator.defaultIndicesLookupTag,
                @"public static class ComponentIds {
    public const int DontGenerate = 0;
    public const int Some = 1;

    public const int TotalComponents = 2;

    static readonly string[] components = {
        ""DontGenerate"",
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}" + defaultTagCode);
        };



        it["ignores [DontGenerate(false)]"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateIndexComponent)
            }, CodeGenerator.defaultIndicesLookupTag,
                @"public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}" + defaultTagCode);
        };

        it["generates empty lookup with total components when for default pool"] = () => {
            generatesEmptyLookup(new string[0], new [] { CodeGenerator.defaultIndicesLookupTag }, new [] { @"public static class ComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] components = {
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return ComponentIds.IdToString(indices[0]);
        }
    }
}" });
        };

        it["generates empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta" }, new [] { "Meta" + CodeGenerator.defaultIndicesLookupTag }, new [] { @"using Entitas;

public static class MetaComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] components = {
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class MetaMatcher : AllOfMatcher {
    public MetaMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return MetaComponentIds.IdToString(indices[0]);
    }
}" });
        };

        it["generates multiple empty lookup with total components when for pool names"] = () => {
            generatesEmptyLookup(new [] { "Meta", "Core" },
                new [] { "Meta" + CodeGenerator.defaultIndicesLookupTag, "Core" + CodeGenerator.defaultIndicesLookupTag },
                new [] { @"using Entitas;

public static class MetaComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] components = {
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class MetaMatcher : AllOfMatcher {
    public MetaMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return MetaComponentIds.IdToString(indices[0]);
    }
}", @"using Entitas;

public static class CoreComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] components = {
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class CoreMatcher : AllOfMatcher {
    public CoreMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return CoreComponentIds.IdToString(indices[0]);
    }
}" });
        };
    }
}

