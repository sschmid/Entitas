using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentMatcherApiGenerator : AbstractGenerator
    {
        public override string Name => "Component (Matcher API)";

        const string Template =
            @"public sealed partial class ${Matcher.Type}
{
    static Entitas.IMatcher<${EntityType}> _matcher${Component.Name};

    public static Entitas.IMatcher<${EntityType}> ${ComponentName}
    {
        get
        {
            if (_matcher${Component.Name} == null)
            {
                var matcher = (Entitas.Matcher<${Entity.Type}>)Entitas.Matcher<${Entity.Type}>.AllOf(${Index});
                matcher.ComponentNames = ${ComponentNames};
                _matcher${Component.Name} = matcher;
            }

            return _matcher${Component.Name};
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
            Path.Combine(context, "Components", $"{context + data.Name.AddComponentSuffix()}.cs"),
            data.ReplacePlaceholders(Template)
                .Replace("${ComponentNames}", $"{context}{CodeGeneratorExtensions.ComponentLookup}.ComponentNames")
                .Replace(data, context),
            GetType().FullName
        );
    }
}
