using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentMatcherApiGenerator : AbstractGenerator
    {
        public override string Name => "Component (Matcher API)";

        const string Template =
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
            .Where(d => d.GeneratesIndex)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data
            .Contexts.Select(context => Generate(context, data));

        CodeGenFile Generate(string context, ComponentData data) => new CodeGenFile(
            Path.Combine(context, "Components", $"{data.ComponentNameWithContext(context).AddComponentSuffix()}.cs"),
            Template
                .Replace("${componentNames}", $"{context}{CodeGeneratorExtensions.ComponentLookup}.componentNames")
                .Replace(data, context),
            GetType().FullName
        );
    }
}
