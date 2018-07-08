using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextMatcherGenerator : ICodeGenerator {

        public string name { get { return "Context (Matcher API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string TEMPLATE =
            @"public sealed partial class ${MatcherType} {

    public static Entitas.IAllOfMatcher<${EntityType}> AllOf(params int[] indices) {
        return Entitas.Matcher<${EntityType}>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<${EntityType}> AllOf(params Entitas.IMatcher<${EntityType}>[] matchers) {
          return Entitas.Matcher<${EntityType}>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<${EntityType}> AnyOf(params int[] indices) {
          return Entitas.Matcher<${EntityType}>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<${EntityType}> AnyOf(params Entitas.IMatcher<${EntityType}>[] matchers) {
          return Entitas.Matcher<${EntityType}>.AnyOf(matchers);
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generate)
                .ToArray();
        }

        CodeGenFile generate(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                contextName.AddMatcherSuffix() + ".cs",
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
