using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ContextMatcherGenerator : ICodeGenerator
    {
        public string Name => "Context (Matcher API)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed partial class ${MatcherType} {

    public static Entitas.IAllOfMatcher<${EntityType}> AllOf(params int[] indexes) {
        return Entitas.Matcher<${EntityType}>.AllOf(indexes);
    }

    public static Entitas.IAllOfMatcher<${EntityType}> AllOf(params Entitas.IMatcher<${EntityType}>[] matchers) {
          return Entitas.Matcher<${EntityType}>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<${EntityType}> AnyOf(params int[] indexes) {
          return Entitas.Matcher<${EntityType}>.AnyOf(indexes);
    }

    public static Entitas.IAnyOfMatcher<${EntityType}> AnyOf(params Entitas.IMatcher<${EntityType}>[] matchers) {
          return Entitas.Matcher<${EntityType}>.AnyOf(matchers);
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data)
        {
            var context = data.Name;
            return new CodeGenFile(
                Path.Combine(context, $"{context.AddMatcherSuffix()}.cs"),
                Template.Replace(context),
                GetType().FullName
            );
        }
    }
}
