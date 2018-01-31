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
@"${Contexts}
public sealed class ${ComponentName}ListenerComponent : Entitas.IComponent {

    public I${ComponentName}Listener value;
}
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.GetEventData() != null)
                .Select(generateComponent)
                .ToArray();
        }

        CodeGenFile generateComponent(ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var contexts = string.Join(", ", data.GetContextNames());
            if (!string.IsNullOrEmpty(contexts)) {
                contexts = "[" + contexts + "]";
            }

            var fileContent = COMPONENT_TEMPLATE
                .Replace("${Contexts}", contexts)
                .Replace("${ComponentName}", componentName);

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                componentName + "Listener".AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
