using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextMatcherGenerator : ICodeGenerator {

        public string name { get { return "Context (Matcher API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXT_TEMPLATE =
@"public sealed partial class ${ContextName}Matcher {

    public static Entitas.IAllOfMatcher<${ContextName}Entity> AllOf(params int[] indices) {
        return Entitas.Matcher<${ContextName}Entity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<${ContextName}Entity> AllOf(params Entitas.IMatcher<${ContextName}Entity>[] matchers) {
          return Entitas.Matcher<${ContextName}Entity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<${ContextName}Entity> AnyOf(params int[] indices) {
          return Entitas.Matcher<${ContextName}Entity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<${ContextName}Entity> AnyOf(params Entitas.IMatcher<${ContextName}Entity>[] matchers) {
          return Entitas.Matcher<${ContextName}Entity>.AnyOf(matchers);
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generateContextClass)
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Matcher.cs",
                CONTEXT_TEMPLATE
                    .Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
