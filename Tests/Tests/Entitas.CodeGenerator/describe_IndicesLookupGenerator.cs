using NSpec;
using Entitas.CodeGenerator;
using System;
using System.Linq;

class describe_IndicesLookupGenerator : nspec {
    bool logResults = true;

    void generates(Type type, string lookupName, string lookupCode) {
        generates(new [] { type }, lookupName, lookupCode);
    }

    void generates(Type[] types, string lookupName, string lookupCode) {
        generates(types, new [] { lookupName }, new [] { lookupCode });
    }

    void generates(Type[] types, string[] lookupNames, string[] lookupCodes) {
        var files = new IndicesLookupGenerator().Generate(types);
        
        files.Length.should_be(lookupNames.Length);

        for (int i = 0; i < lookupNames.Length; i++) {
            var lookupName = lookupNames[i];
            var lookupCode = lookupCodes[i];

            files.Any(f => f.fileName == lookupName).should_be_true();
            var file = files.Single(f => f.fileName == lookupName);
            if (logResults) {
                Console.WriteLine("should:\n" + lookupCode);
                Console.WriteLine("was:\n" + file.fileContent);
            }

            var expectedUnified = CodeGenerator.sanitizeString(lookupCode);
            var generatedUnified = CodeGenerator.sanitizeString(file.fileContent);

            generatedUnified.should_be(expectedUnified);
        }
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
                @"using System;
using System.Collections.Generic;
public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(SomeComponent), 0}
    };

    static readonly string[] components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
    }

}" + defaultTagCode);
        };



        it["generates lookup with name from attribute"] = () => {
            generates(typeof(OtherPoolComponent), "OtherComponentIds",
                @"using System;
using System.Collections.Generic;
using Entitas;

public static class OtherComponentIds {
    public const int OtherPool = 0;

    public const int TotalComponents = 1;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(OtherPoolComponent), 0}
    };

    static readonly string[] components = {
        ""OtherPool""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
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
                @"using System;
using System.Collections.Generic;
public static class ComponentIds {
    public const int DontGenerate = 0;

    public const int TotalComponents = 1;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(DontGenerateComponent), 0}
    };

    static readonly string[] components = {
        ""DontGenerate""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
    }

}" + defaultTagCode);
        };



        it["generates ids for all types ordered alphabetically"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateComponent)
            }, CodeGenerator.defaultIndicesLookupTag,
                @"using System;
using System.Collections.Generic;
public static class ComponentIds {
    public const int DontGenerate = 0;
    public const int Some = 1;

    public const int TotalComponents = 2;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(DontGenerateComponent), 0},
        {typeof(SomeComponent), 1}
    };

    static readonly string[] components = {
        ""DontGenerate"",
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
    }

}" + defaultTagCode);
        };



        it["ignores [DontGenerate(false)]"] = () => {
            generates(new [] {
                typeof(SomeComponent),
                typeof(DontGenerateIndexComponent)
            }, CodeGenerator.defaultIndicesLookupTag,
                @"using System;
using System.Collections.Generic;
public static class ComponentIds {
    public const int Some = 0;

    public const int TotalComponents = 1;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(SomeComponent), 0}
    };

    static readonly string[] components = {
        ""Some""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
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

        context["when component is in multiple pools"] = () => {

            it["rearranges ids to have the same index in every lookup (2 pools)"] = () => {
                generates(new [] {
                    typeof(AComponent),
                    typeof(BComponent)
                }, new [] { "PoolAComponentIds", "PoolBComponentIds" }, new [] {
                    @"using System;
using System.Collections.Generic;
using Entitas;

public static class PoolAComponentIds {
    public const int B = 0;
    public const int A = 1;

    public const int TotalComponents = 2;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(BComponent), 0},
        {typeof(AComponent), 1}
    };

    static readonly string[] components = {
        ""B"",
        ""A""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
    }

}

public partial class PoolAMatcher : AllOfMatcher {
    public PoolAMatcher(int index) : base(new[] { index })
    {
    }

    public override string ToString()
    {
        return PoolAComponentIds.IdToString(indices[0]);
    }
}",
                    @"using Entitas;

public static class PoolBComponentIds {
    public const int B = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        ""B""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class PoolBMatcher : AllOfMatcher {
    public PoolBMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return PoolBComponentIds.IdToString(indices[0]);
    }
}"
                });
            };

            it["rearranges ids to have the same index in every lookup (3 pools)"] = () => {
                generates(new [] {
                    typeof(AComponent),
                    typeof(BComponent),
                    typeof(CComponent),
                    typeof(DComponent),
                    typeof(EComponent),
                    typeof(FComponent)
                }, new [] {
                    "PoolAComponentIds",
                    "PoolBComponentIds",
                    "PoolCComponentIds",
                }, new [] {
                    @"using System;
using System.Collections.Generic;
using Entitas;

public static class PoolAComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int A = 2;

    public const int TotalComponents = 3;

    private static readonly Dictionary<Type, int> typeToInt = new Dictionary<Type, int>() {
        {typeof(CComponent), 0},
        {typeof(BComponent), 1},
        {typeof(AComponent), 2}
    };

    static readonly string[] components = {
        ""C"",
        ""B"",
        ""A""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
    public static int ComponentToId(IComponent c) {
        return typeToInt[c.getType()];
    }

}

public partial class PoolAMatcher : AllOfMatcher
{
    public PoolAMatcher(int index) : base(new[] { index }) {
    }

    public override string ToString() {
        return PoolAComponentIds.IdToString(indices[0]);
    }
}",
                    @"using Entitas;

public static class PoolBComponentIds {
    public const int C = 0;
    public const int B = 1;
    public const int D = 2;

    public const int TotalComponents = 3;

    static readonly string[] components = {
        ""C"",
        ""B"",
        ""D""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class PoolBMatcher : AllOfMatcher {
    public PoolBMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return PoolBComponentIds.IdToString(indices[0]);
    }
}",
                    @"using Entitas;

public static class PoolCComponentIds {
    public const int C = 0;
    public const int E = 1;
    public const int D = 2;
    public const int F = 3;

    public const int TotalComponents = 4;

    static readonly string[] components = {
        ""C"",
        ""E"",
        ""D"",
        ""F""
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class PoolCMatcher : AllOfMatcher {
    public PoolCMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return PoolCComponentIds.IdToString(indices[0]);
    }
}"
                });
            };        };
    }
}

