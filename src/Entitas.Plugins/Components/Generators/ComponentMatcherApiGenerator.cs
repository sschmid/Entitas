using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentMatcherApiGenerator : ICodeGenerator
    {
        public string Name => "Component (Matcher API)";
        public int Order => 0;
        public bool RunInDryMode => true;

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

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.GeneratesIndex)
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ComponentData data) => new CodeGenFile(
            Path.Combine(data.Context, "Components", $"{data.Context + data.Name.AddComponentSuffix()}.cs"),
            data.ReplacePlaceholders(Template)
                .Replace("${ComponentNames}", $"{data.Context}{CodeGeneratorExtensions.ComponentLookup}.ComponentNames")
                .Replace(data, data.Context),
            GetType().FullName
        );
    }
}
