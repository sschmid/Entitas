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
            @"public sealed partial class ${Context.Matcher.Type}
{
    public static Entitas.IAllOfMatcher<${Context.Entity.Type}> AllOf(params int[] indexes) =>
        Entitas.Matcher<${Context.Entity.Type}>.AllOf(indexes);

    public static Entitas.IAllOfMatcher<${Context.Entity.Type}> AllOf(params Entitas.IMatcher<${Context.Entity.Type}>[] matchers) =>
        Entitas.Matcher<${Context.Entity.Type}>.AllOf(matchers);

    public static Entitas.IAnyOfMatcher<${Context.Entity.Type}> AnyOf(params int[] indexes) =>
        Entitas.Matcher<${Context.Entity.Type}>.AnyOf(indexes);

    public static Entitas.IAnyOfMatcher<${Context.Entity.Type}> AnyOf(params Entitas.IMatcher<${Context.Entity.Type}>[] matchers) =>
        Entitas.Matcher<${Context.Entity.Type}>.AnyOf(matchers);
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data) => new CodeGenFile(
            Path.Combine(data.Name, $"{data.MatcherType}.cs"),
            data.ReplacePlaceholders(Template),
            GetType().FullName
        );
    }
}
