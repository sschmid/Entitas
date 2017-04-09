using Entitas.CodeGeneration;

namespace Entitas.VisualDebugging.CodeGeneration.Plugins {

    public class FeatureClassGenerator : ICodeGenerator {

        public string name { get { return "Feature Class"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string FEATURE_TEMPLATE =
@"#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

public class Feature : Entitas.VisualDebugging.Unity.DebugSystems {

    public Feature(string name) : base(name) {
    }

    public Feature() : base(true) {
        var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
        var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
        initialize(toSpacedCamelCase(shortType));
    }

    static string toSpacedCamelCase(string text) {
        var sb = new System.Text.StringBuilder(text.Length * 2);
        sb.Append(char.ToUpper(text[0]));
        for(int i = 1; i < text.Length; i++) {
            if(char.IsUpper(text[i]) && text[i - 1] != ' ') {
                sb.Append(' ');
            }
            sb.Append(text[i]);
        }

        return sb.ToString();
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
            return new[] { new CodeGenFile(
                "Feature.cs",
                FEATURE_TEMPLATE,
                GetType().FullName)
            };
        }
    }
}
