using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public class EventComponentGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Component)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string COMPONENT_TEMPLATE =
            @"public sealed class ${OptionalContextName}${ComponentName}ListenerComponent : Entitas.IComponent {
    public I${OptionalContextName}${ComponentName}Listener value;
}
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(generateComponents)
                .ToArray();
        }

        CodeGenFile[] generateComponents(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateComponent(contextName, data))
                .ToArray();
        }

        CodeGenFile generateComponent(string contextName, ComponentData data) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);

            var fileContent = COMPONENT_TEMPLATE
                .Replace("${OptionalContextName}", optionalContextName)
                .Replace("${ComponentName}", componentName);

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                optionalContextName + componentName + "Listener".AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
