using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentMatcherGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Component (Matcher API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string STANDARD_COMPONENT_TEMPLATE =
@"public sealed partial class ${ContextName}Matcher {

    static Entitas.IMatcher<${ContextName}Entity> _matcher${ComponentName};

    public static Entitas.IMatcher<${ContextName}Entity> ${ComponentName} {
        get {
            if (_matcher${ComponentName} == null) {
                var matcher = (Entitas.Matcher<${ContextName}Entity>)Entitas.Matcher<${ContextName}Entity>.AllOf(${Index});
                matcher.componentNames = ${ComponentNames};
                _matcher${ComponentName} = matcher;
            }

            return _matcher${ComponentName};
        }
    }
}
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .SelectMany(generateMatcher)
                .ToArray();
        }

        CodeGenFile[] generateMatcher(ComponentData data) {
            return data.GetContextNames()
                .Select(context => generateMatcher(context, data))
                .ToArray();
        }

        CodeGenFile generateMatcher(string contextName, ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + componentName;
            var componentNames = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + ".componentNames";

            var fileContent = STANDARD_COMPONENT_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentName}", componentName)
                .Replace("${Index}", index)
                .Replace("${ComponentNames}", componentNames);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + componentName.AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
