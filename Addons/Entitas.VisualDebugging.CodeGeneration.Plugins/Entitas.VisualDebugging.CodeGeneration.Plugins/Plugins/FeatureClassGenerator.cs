using Entitas.CodeGeneration;

namespace Entitas.VisualDebugging.CodeGeneration.Plugins {

    public class FeatureClassGenerator : ICodeGenerator {

        public string name { get { return "Feature Class"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string FEATURE_TEMPLATE =
@"#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

public class Feature : Entitas.VisualDebugging.Unity.DebugSystems {

    public Feature(string name) : base(name) {
    }

    public Feature() : base(true) {
        var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
        var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
        var readableType = Entitas.Utils.StringExtension.ToSpacedCamelCase(shortType);

        initialize(readableType);
    }
}

#else

public class Feature : Entitas.Systems {

    public Feature(string name) {
    }

    public Feature() {
    }
}

#endif
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return new [] { new CodeGenFile(
                "Feature.cs",
                FEATURE_TEMPLATE,
                GetType().FullName)
            };
        }
    }
}
