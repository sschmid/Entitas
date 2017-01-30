using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class MatcherGenerator : ICodeGenerator {

        const string matcherTemplate =
@"using Entitas;

public sealed partial class ${Context}Matcher {

    static IMatcher<${Context}Entity> _matcher${Name};

    public static IMatcher<${Context}Entity> ${Name} {
        get {
            if(_matcher${Name} == null) {
                var matcher = (Matcher<${Context}Entity>)Matcher<${Context}Entity>.AllOf(${Index});
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

            var fileContent = matcherTemplate
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetComponentName())
                .Replace("${Index}", index)
                .Replace("${ComponentNames}", componentNames);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + "Matchers" +
                Path.DirectorySeparatorChar + contextName + "Matcher" + data.GetComponentName() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
