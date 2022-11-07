using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentMatcherApiGenerator : AbstractGenerator
    {
        public override string Name => "Component (Matcher API)";

        const string TEMPLATE =
            @"public sealed partial class ${MatcherType} {

    static Entitas.IMatcher<${EntityType}> _matcher${ComponentName};

    public static Entitas.IMatcher<${EntityType}> ${ComponentName} {
        get {
            if (_matcher${ComponentName} == null) {
                var matcher = (Entitas.Matcher<${EntityType}>)Entitas.Matcher<${EntityType}>.AllOf(${Index});
                matcher.componentNames = ${componentNames};
                _matcher${ComponentName} = matcher;
            }

            return _matcher${ComponentName};
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.ShouldGenerateIndex())
            .SelectMany(generate)
            .ToArray();

        CodeGenFile[] generate(ComponentData data) => data
            .GetContextNames()
            .Select(context => generate(context, data))
            .ToArray();

        CodeGenFile generate(string contextName, ComponentData data)
        {
            var fileContent = TEMPLATE
                .Replace("${componentNames}", $"{contextName}{CodeGeneratorExtensions.LOOKUP}.componentNames")
                .Replace(data, contextName);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                data.ComponentNameWithContext(contextName).AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
