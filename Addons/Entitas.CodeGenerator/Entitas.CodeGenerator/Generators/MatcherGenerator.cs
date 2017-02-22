using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class MatcherGenerator : ICodeGenerator {

        public string name { get { return "Matcher"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string MATCHER_TEMPLATE =
@"public sealed partial class ${Context}Matcher {

    static Entitas.IMatcher<${Context}Entity> _matcher${Name};

    public static Entitas.IMatcher<${Context}Entity> ${Name} {
        get {
            if(_matcher${Name} == null) {
                var matcher = (Entitas.Matcher<${Context}Entity>)Entitas.Matcher<${Context}Entity>.AllOf(${Index});
                matcher.componentNames = ${ComponentNames};
                _matcher${Name} = matcher;
            }

            return _matcher${Name};
        }
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .SelectMany(d => generateMatcher(d))
                .ToArray();
        }

        CodeGenFile[] generateMatcher(ComponentData data) {
            return data.GetContextNames()
                       .Select(context => generateMatcher(context, data))
                       .ToArray();
        }

        CodeGenFile generateMatcher(string contextName, ComponentData data) {
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + data.GetComponentName();
            var componentNames = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + ".componentNames";

            var fileContent = MATCHER_TEMPLATE
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetComponentName())
                .Replace("${Index}", index)
                .Replace("${ComponentNames}", componentNames);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + data.GetFullComponentName() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
