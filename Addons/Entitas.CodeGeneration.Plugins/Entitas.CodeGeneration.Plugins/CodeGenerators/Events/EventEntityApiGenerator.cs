using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EventEntityApiGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Entity API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string COMPONENT_TEMPLATE =
            @"public partial class ${ContextName}Entity {

    public void Add${OptionalContextName}${ComponentName}${EventType}Listener(I${OptionalContextName}${ComponentName}${EventType}Listener value) {
        var listeners = has${OptionalContextName}${ComponentName}${EventType}Listener
            ? ${optionalContextName}${contextDependentComponentName}${EventType}Listener.value
            : new System.Collections.Generic.List<I${OptionalContextName}${ComponentName}${EventType}Listener>();
        listeners.Add(value);
        Replace${OptionalContextName}${ComponentName}${EventType}Listener(listeners);
    }

    public void Remove${OptionalContextName}${ComponentName}Listener(I${OptionalContextName}${ComponentName}${EventType}Listener value, bool removeComponentWhenEmpty = true) {
        var listeners = ${optionalContextName}${contextDependentComponentName}${EventType}Listener.value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            Remove${OptionalContextName}${ComponentName}${EventType}Listener();
        } else {
            Replace${OptionalContextName}${ComponentName}${EventType}Listener(listeners);
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
                        .Replace("${ContextName}", contextName)
                        .Replace("${OptionalContextName}", optionalContextName)
                        .Replace("${optionalContextName}", optionalContextName == string.Empty ? string.Empty : optionalContextName.LowercaseFirst())
                        .Replace("${ComponentName}", componentName)
                        .Replace("${componentName}", componentName.LowercaseFirst())
                        .Replace("${EventType}", eventTypeSuffix)
                        .Replace("${contextDependentComponentName}", optionalContextName == string.Empty ? componentName.LowercaseFirst() : componentName);

                    return new CodeGenFile(
                        contextName + Path.DirectorySeparatorChar +
                        "Components" + Path.DirectorySeparatorChar +
                        contextName + optionalContextName + componentName + eventTypeSuffix + "Listener".AddComponentSuffix() + ".cs",
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }
    }
}
