using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public class EventListenerComponentGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Listener Component)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string COMPONENT_TEMPLATE =
            @"public sealed class ${OptionalContextName}${ComponentName}${EventType}ListenerComponent : Entitas.IComponent {
    public System.Collections.Generic.List<I${OptionalContextName}${ComponentName}${EventType}Listener> value;
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
                .SelectMany(contextName => generateComponent(contextName, data))
                .ToArray();
        }

        CodeGenFile[] generateComponent(string contextName, ComponentData data) {
            return data.GetEventData()
                .Select(eventData => {
                    var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
                    var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
                    var eventTypeSuffix = data.GetEventTypeSuffix(eventData);

                    var fileContent = COMPONENT_TEMPLATE
                        .Replace("${OptionalContextName}", optionalContextName)
                        .Replace("${ComponentName}", componentName)
                        .Replace("${EventType}", eventTypeSuffix);

                    return new CodeGenFile(
                        "Events" + Path.DirectorySeparatorChar +
                        "Components" + Path.DirectorySeparatorChar +
                        optionalContextName + componentName + eventTypeSuffix + "Listener".AddComponentSuffix() + ".cs",
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }
    }
}
